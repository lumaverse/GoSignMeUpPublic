function AttendanceCompont(options) {
    var self = this;

    self.options = Ext.merge(self.options, options);

    self.prepareModels();

    Ext.onDocumentReady(function () {

        if (self.options.renderType == 'page') {
            self.renderCourseSelector()
        }

        if (options.model) {
            self.startup()
        }
    });
}

AttendanceCompont.prototype.constructor = AttendanceCompont;
AttendanceCompont.prototype.options = {
    attendanceTitle: 'Take attendance',
    dateFormat: 'm/d/Y',
    renderType: 'page',
    dateRendering: 'vertical',
    timeFormat: 'h:i A',
    containerId: null,
    courseSelectorContainerId: null,
    configuration: {
    },
    model: {
        // course model serialized
        CourseModel: {
            CourseId: null,
            Course: null,
            CourseTimes: [],
            Credits: []
        },
        // course times array
        Rosters: [],
        // students array
        Students: [],
        AttendanceStatus: [],
        Attendance: [],
        AttendanceDetails: [],
        Transcripts: []
    }
}

AttendanceCompont.prototype.state = {
    grid: null,
    viewport: null,
    courseSelectorCombo: null,
    stores: {
        student: null,
        grid: null,
        attendance: null,
        attendanceDetails: null,
        transcript: null,
        courseTimes: null
    }
}

AttendanceCompont.prototype.renderCourseSelector = function () {
    var self = this;

    var renderTo = self.options.renderType == 'page' ? self.options.courseSelectorContainerId : null;

    var courseStore = Ext.create('CourseStore', {
        pageSize: 100
    });

    self.state.courseSelectorCombo = Ext.create('Ext.form.field.ComboBox', {
        renderTo: renderTo,
        labelAlign: 'right',
        labelWidth: 80,
        minChars: 2,
        width: 300,
        emptyText: 'Course name, number or ID',
        editable: true,
        forceSelection: true,
        fieldLabel: 'Select course',
        queryMode: 'remote',
        store: courseStore,
        displayField: 'COURSENAME',
        valueField: 'COURSEID',
        listeners: {
            select: function (cmp, records) {
                if (records != null && Ext.isArray(records)) {
                    var record = records[0];
                    var courseId = record.get('COURSEID');
                    self.reset();
                    LAYOUT.MaskLayout('Loading course data...')
                    Ext.Ajax.request({
                        url: config.getUrl('adm/attendance/attendanceinfo?courseId=' + courseId ),
                        method: 'POST',
                        success: function (response) {
                            var model = Ext.decode(response.responseText);
                            self.startup(model);

                            if (history.pushState) {
                                history.pushState(null, model.CourseModel.Course.COURSENAME, location.pathname + '?courseId=' + courseId);
                            }
                        },
                        callback: function () {
                            LAYOUT.UnmaskLayout();
                        }
                    });
                }
            }
        }
    });
    return self.state.courseSelectorCombo;
}

AttendanceCompont.prototype.startup = function (model) {
    var self = this;

    if (model) {
        self.options.model = model;
    }
    
    self.prepareStores();
    self.render();

}

AttendanceCompont.prototype.prepareModels = function () {
    var self = this;

    Ext.define('Course_RosterAttendance', {
        extend: 'Course_Roster',
        fields: [
            {
                name: 'StudentFirst',
                convert: function (v, record) {
                    var sid = record.data.STUDENTID;
                    var student = self.state.stores.student.getById(sid);
                    return student.get('FIRST');
                }
            },
            {
                name: 'StudentLast',
                convert: function (v, record) {
                    var sid = record.data.STUDENTID;
                    var student = self.state.stores.student.getById(sid);
                    return student.get('LAST');
                }
            }
        ]
    });

    Ext.define('AttendanceDetailCustom', {
        extend: 'AttendanceDetail',
        fields: [

        ]
    });


}

AttendanceCompont.prototype.prepareStores = function () {
    var self = this;

    self.state.stores.student = Ext.create('Ext.data.Store', {
        storeId: 'attendance-student-store',
        model: 'Student',
        data: Ext.clone(self.options.model.Students)
    });

    self.state.stores.transcript = Ext.create('Ext.data.Store', {
        storeId: 'attendance-transcript-store',
        model: 'Transcript',
        data: Ext.clone(self.options.model.Transcripts)
    });

    self.state.stores.courseTimes = Ext.create('Ext.data.Store', {
        storeId: 'attendance-course-time-store',
        model: 'Course_Time',
        data: Ext.clone(self.options.model.CourseModel.CourseTimes)
    });

    self.state.stores.attendanceDetails = Ext.create('Ext.data.Store', {
        storeId: 'attendance-detail-store',
        model: 'AttendanceDetailCustom',
        data: Ext.clone(self.options.model.AttendanceDetails)
    });

    self.state.stores.grid = Ext.create('Ext.data.Store', {
        storeId: 'attendance-roster-store',
        model: 'Course_RosterAttendance',
        data: Ext.clone(self.options.model.Rosters)
    });


}

AttendanceCompont.prototype.render = function () {
    var self = this;

    var metrics = new Ext.util.TextMetrics();
    var transcribeLabel = Terminology.capital('Transcribe');
    var transcribeLabelMetric = metrics.getWidth(transcribeLabel);

    var title = self.options.attendanceTitle + ': ' + self.options.model.CourseModel.Course.COURSENAME;
    LAYOUT.setPageTitle(title);

    var gridColumns = [
        { xtype: 'rownumberer' },
        {
            dataIndex: 'RosterID',
            text: 'Roster ID',
            hidden: true
        } ,
        {
            text: transcribeLabel,
            width: transcribeLabelMetric + 10,
            locked: true,
            lockable: false,
            hideable: false,
            hidden: true,
            align: 'center',
            xtype: 'componentcolumn',
            renderer: function (name, meta, record) {

                var cmpId = 'attendance-form-transcribe-' + record.get('RosterID');
                return '<input type="checkbox" id="' + cmpId + '" onclick="arguments[0].stopPropagation();">';
            },
            listeners: {
                click: function (grid, meta, val, val, ev, record, grid, options) {
                    var cmpId = 'attendance-form-transcribe-' + record.get('RosterID');
                    var cmp = Ext.getDom(cmpId);
                    cmp.checked = !cmp.checked;
                }
            }
        },
        {
            text: 'Last Name',
            dataIndex: 'StudentLast',
            locked: true
        },
        {
            text: 'First Name',
            dataIndex: 'StudentFirst',
            flex: 1
        },
        {
            text: 'Attendance status',
            width: 120,
            renderer: function (name, meta, record) {

                var id  = 'attendance-form-status-' + record.get('RosterID');
                var value = record.get('AttendanceStatusId');

                var html = '';

                html += '<select id="' + id + '">';
                html += '<option value=""></option>';
                for (var i = 0; i < self.options.model.AttendanceStatus.length; i++) {
                    var status = self.options.model.AttendanceStatus[i];
                    var statusValue = status['AttendanceStatusId'];
                    html += '<option value="' + statusValue + '"';
                    if (value == statusValue) {
                        html += ' selected="selected"';
                    }
                    html += '>' + status['AttendanceStatus'] + '</option>';
                }
                html += '</select>';

                return html;
            }
        }
    ];

    var toolbarItems = [];
    var bottomToolbarItems = [];

    if (self.options.renderType != 'page') {
        toolbarItems.push(
            self.renderCourseSelector()
        );
    }


    bottomToolbarItems.push({
        xtype: 'combobox',
        forceSelection: true,
        editable: false,
        store: {
            xtype: 'store',
            fields: ['code', 'display'],
            data: [
                {
                    code: 'TranscribeNone',
                    display: 'None'
                },
                {
                    code: 'TranscribeChecked',
                    display: 'Checked'
                },
                {
                    code: 'TranscribeAll',
                    display: 'All'
                }
            ]
        },
        listeners: {
            change: function (cmp, value, oldValue) {
                if (value == 'TranscribeChecked') {
                    self.state.grid.columns[1].setVisible(true);
                } else {
                    self.state.grid.columns[1].setVisible(false);
                }
            }
        } ,
        fieldLabel: transcribeLabel,
        labelWidth: transcribeLabelMetric,
        labelAlign: 'right',
        displayField: 'display',
        valueField: 'code',
        value: 'TranscribeNone'
    });
    bottomToolbarItems.push('-');


    if (self.state.stores.courseTimes.count() > 0) {
        var dateColumnsAttended;
        var dateColumnsDidnotAttend;

        dateColumnsDidnotAttend = self.getVerticalDateColumns(false);
        dateColumnsAttended = self.getVerticalDateColumns(true);
        gridColumns.push(dateColumnsAttended);
        gridColumns.push(dateColumnsDidnotAttend);
    }


    if (self.options.model.CourseModel.Credits.length > 0) {

        toolbarItems.push('->');

        var creditInfoItems = [];
        var creditColumnList = [];
        for (var index in self.options.model.CourseModel.Credits) {
            var credit = self.options.model.CourseModel.Credits[index];
            var renderColumn = function (credit) {
                return {
                    text: credit.Label,
                    align: 'center',
                    renderer: function (name, meta, record) {
                        var id = 'attendance-form-day-' + record.get('RosterID') + '-' + credit.CourseCreditType;

                        var html = '';
                        html += '<input type="number" pattern="^(([1-9]*)|(([1-9]*).([0-9]*)))$" class="float" id="' + id + '" style="width: 90px;">';
                        return html;
                    }
                };
            }
            var column = renderColumn(credit);
            creditColumnList.push(column);


            /*
            toolbarItems.push('->');

            var creditMenuItem = {
                text: '<div style="float: right; text-align: left;">' + Ext.util.Format.number(credit.Credit, '0.00') + '</div><div style="font-weight: bold; white-space: normal !important; float: right; margin-right: 5px;">' + credit.Label + '</div>',
                xtype: 'menuitem',
                plain: true,
                style: LAYOUT.Options.plainMenuItemStyle
            };
            creditInfoItems.push(creditMenuItem);
            */

            toolbarItems.push({
                text: '<strong>' + credit.Label + '</strong>:&nbsp;' + Ext.util.Format.number(credit.Credit, '0.00')
            });
        }


        /*
        toolbarItems.push('->');

        toolbarItems.push({
            scale: 'small',
            xtype: 'button',
            text: 'Credit info',
            icon: config.getUrl('Images/Icons/glyph2/Icons16x16/suitcase.png'),
            textAlign: 'left',
            menuAlign: 'tr-br?',
            menu: new Ext.menu.Menu({
                showSeparator: false,
                items: creditInfoItems
            })
        });
        */
        var creditColumns = {
            text: 'Credits',
            columns: creditColumnList
        }
        gridColumns.push(creditColumns);
    }

    self.state.grid = Ext.create('Ext.grid.Panel', {
        border: 0,
        frame: false,
        store: self.state.stores.grid,
        enableLocking: true,
        viewConfig: {
            forceFit: true
        },
        columns: gridColumns,
        region: 'center'
    });

    var toolbar = {
        xtype: 'toolbar',
        frame: false,
        border: 0,
        region: 'north',
        items: toolbarItems
    }

    bottomToolbarItems.push({
        id: 'attendance-form-finalize',
        xtype: 'checkbox',
        boxLabel: 'Finalize attendance'
    });

    bottomToolbarItems.push('->');
    bottomToolbarItems.push({
        xtype: 'button',
        icon: config.getUrl('Images/Icons/glyph2/Icons16x16/button-2.png'),
        text: 'Record attendance',
        handler: function () {
            LAYOUT.MaskLayout('IMPLEMENT: Save data to server');
            setTimeout(function () {
                LAYOUT.UnmaskLayout();
            }, 3000);
        }
    });

    var bottomToolbar = Ext.create('Ext.toolbar.Toolbar', {
        region: 'south',
        frame: false,
        border: 0,
        items: bottomToolbarItems
    });

    var viewPortItems = [];
    viewPortItems.push(self.state.grid);
    viewPortItems.push(toolbar);
    viewPortItems.push(bottomToolbar);

    if (self.options.renderType == 'page') {
        self.state.viewport = Ext.create('Ext.panel.Panel', {
            renderTo: 'attendance-component',
            layout: 'border',
            height: 500,
            frame: false,
            broder: 0,
            viewConfig: {
                forceFit: true
            },
            items: viewPortItems
        });

    } else {
        self.state.viewport = Ext.create('Ext.container.Viewport', {
            layout: 'border',
            frame: false,
            broder: 0,
            viewConfig: {
                forceFit: true
            },
            items: viewPortItems
        });
    }

    self.state.grid.on('beforeitemmouseenter', function () {
        var items = Ext.query('.float');
        for (var index = 0; index < items.length; index++) {
            var item = Ext.get(items[index]);
            item
                .on('keydown', function (ev, dom) {
                    var code = ev.keyCode;
                    // period code = 190, 46
                    if ((code == 190 || code == 46) && new String(dom.value).indexOf('.') != -1) {
                        ev.stopEvent();
                        return false;
                    }

                    if ((code >= 33 && code <= 45 || code == 47) || (code > 57 && code != 190)) {
                        ev.stopEvent();
                        return false;
                    }
                })
                .on('change', function (ev, dom) {
                    var $e = $(dom);
                    var val = $e.val();
                    new String(val).replace('')
                    if (!Validators.IsNumber($e.val())) {
                        dom.value = dom.getAttribute('data-valid-value');
                    } else {
                        dom.setAttribute('data-valid-value', dom.value);
                    }
                })
            ;
        }
    }, null, { single: true });

    var radioSetup = function (type) {
        Ext.get('attended-select-all-' + type).on('click', function () {
            Ext.select('.' + type).each(function (el) {
                el.dom.checked = true;
            });
        });
    };
    radioSetup('present');
    radioSetup('missing');

    Ext.on('resize', self.resizeHandler, self);
}

AttendanceCompont.prototype.getVerticalDateColumns = function (attended) {
    var self = this;

    var title;
    var value;
    var cssClass;
    if (attended) {
        title = 'Attended';
        value = 1;
        cssClass = "present";
    } else {
        title = 'Did not attend';
        value = 0;
        cssClass = "missing";
    }

    var dateColumn = {
        text: '<div style="float: right; margin-right: 10px; position: relative; top: -3px;"><input type="button" value="ALL" class="column-component" id="attended-select-all-' + cssClass +'"/></div><span style="position: relative; top: 7px;">' + title + '</span>',
        width: 200,
        renderer: function (name, meta, record) {

            var dateItems = [];

            var html = '';
            var counter = 0;

            self.state.stores.courseTimes.each(function (ctRecord) {
                var courseDate = ctRecord.get('COURSEDATE');
                var courseTime = ctRecord.get('STARTTIME');
                var courseDateKey = Ext.Date.format(courseDate, self.options.dateFormat) + ' ' + Ext.Date.format(courseTime, self.options.timeFormat);
                var title = Ext.Date.format(courseDate, 'M/d/Y');

                var rosterId = record.get('RosterID');
                var cmpName = 'attendance-form-day-' + rosterId + '-' + ctRecord.get('ID');
                var cmpId = 'attendance-form-day-' + rosterId + '-' + ctRecord.get('ID') + '-' + value;
                var attended = false;
                self.state.stores.attendanceDetails.each(function (record) {
                    var attendanceRosterId = record.get('RosterId');
                    if (attendanceRosterId == rosterId) {
                        var attendanceDateKey = Ext.Date.format(record.get('CourseDate'), self.options.dateFormat + ' ' + self.options.timeFormat);
                        if (attendanceDateKey == courseDateKey) {
                            attended = record.get('Attended') == 1;
                        }
                    }
                });

                var highlight = counter % 2 == 0;
                var bgColor;
                if (highlight) {
                    bgColor = '#f0f0ff';
                } else {
                    bgColor = '#e0e0ee';
                }

                html += '<div style="height: 20px; background-color: ' + bgColor + ' ;">';
                html += '<input class="' + cssClass + '" style="position: relative; top: 2px;" type="radio" ' + (attended ? 'checked="checked"' : '') + ' id="' + cmpId + '" name="' + cmpName + '" value="' + value + '"/>';
                html += '<label for="' + cmpId + '">';
                html += title;
                html += '</label>';
                html += '</div>';
                counter++;
            });

            return html;
        }
    };
    return dateColumn;
}

AttendanceCompont.prototype.reset = function (model) {
    var self = this;

    if (typeof (ADMINLAYOUT) != 'undefined') {
        ADMINLAYOUT.setPageTitle(self.options.attendanceTitle);
    }

    self.options.model = null;
    Ext.un('resize', self.resizeHandler);

    if (self.state.viewport) {
        self.state.viewport.destroy();
    }

    if (model) {
        self.startup(model);
    }
}

AttendanceCompont.prototype.resizeHandler = function () {
    var self = this;
    self.state.viewport.doLayout();
}