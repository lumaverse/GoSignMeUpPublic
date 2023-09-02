function InstructorEmailListandReportWidget(dashboard) {
    var self = this;

    self.State.dashboard = dashboard;
}

InstructorEmailListandReportWidget.prototype = new WidgetBase();

InstructorEmailListandReportWidget.constructor = InstructorEmailListandReportWidget;

InstructorEmailListandReportWidget.prototype.Options = {
};

InstructorEmailListandReportWidget.prototype.State = {
    container: null,
    dashboard: null,
    widget: null
};

InstructorEmailListandReportWidget.prototype.ReportAction = function (cmd)
{
    var self = this;
    switch (cmd) {
        case 'addstudent':
            window.location.assign("/Public/User/RegisterUser")
            break;
        case 'editstudent':
            Cart.prototype.DisplayAllStudents_ForEnrollment(0,1);
            break;
        case 'enroll':
            Cart.prototype.DisplayAllStudents_ForEnrollment(0);
            break;
        case 'coursereq':
            self.CourseRequest();
            break;
        case 'roommgt':
            self.RoomManagement();
            break;
        case 'certificationreport':
            self.CertificationReport();
            break;
        case 'rosterreport':
            self.RosterReportAllCourses();
            break;
        case 'attendancereport':
            self.AttendanceReportData();
            break;
        case 'subadmin':
            window.location.href = "/admin/index.asp?name=" + username + "&valauth=" + instructorsessionId +"&pass=UseSession";
            break;


          
    }
}

InstructorEmailListandReportWidget.prototype.dateFilters = {
    date: {
        from: null,
        to: null
    },
    from:
        {
            newvalue: null,
            oldvalue: null
        },
    to:
        {
            newvalue: null,
            oldvalue: null
        }

}

InstructorEmailListandReportWidget.prototype.executeCommand = function (cmd, param, param2) {
    var self = this;



    switch (cmd) {
        case 'attendance-executequery':
        case 'attendance-grid-filter-keyword':
            var filters = [];

            //date-to
            var toParam = self.dateFilters.date.to;
            if (toParam.getErrors().length == 0 && toParam.getValue() !== null && typeof (toParam.getValue() == 'string')) {
                //var toParamVal = toParam.getValue().format(Portal.SERVER_DATE_FORMAT)
                var toParamVal = Ext.util.Format.date(toParam.getValue(), 'm/d/Y')
            } else {
                alert("Invalid To Date.")
                toParam.focus();
                return;
            }

            //date-from
            var fromParam = self.dateFilters.date.from;
            if (fromParam.getErrors().length == 0 && fromParam.getValue() !== null && typeof (fromParam.getValue() == 'string')) {
                //var fromParamVal = fromParam.getValue().format(Portal.SERVER_DATE_FORMAT)
                var fromParamVal = Ext.util.Format.date(fromParam.getValue(), 'm/d/Y')
            } else {
                alert("Invalid From Date.")
                fromParam.focus();
                return;
            }

            filters.push({ id: 'date', property: 'date', value: 'all' },
			{ id: 'date-from', property: 'date-from', value: fromParamVal },
			{ id: 'date-to', property: 'date-to', value: toParamVal });

            //keyword
            var input = Ext.getCmp('searchname').getValue().trim();
            if (input == null) {
                input = ''
            }
            filters.push({ id: 'keyword', property: 'keyword', value: input });




            //export
            filters.push({ id: 'export', property: 'export', value: param });
            self.grid.store.getProxy().setExtraParam("param", param);

            var finalCols = [];
            if (param == 'exportall' || param == 'getpdf') {
                var tempCols = Ext.getCmp('AttendanceReportGrid').query('gridcolumn:not([hidden])');
                for (var x = 0; x < tempCols.length; x++) {
                    if (tempCols[x].dataIndex != 'count') {
                        finalCols.push({ property: tempCols[x].dataIndex, value: tempCols[x].text });
                    }
                }
                if (param == 'getpdf') {
                    self.grid.store.getProxy().setExtraParam("selectedflds", param2);
                }
                self.grid.store.getProxy().setExtraParam("columns", Ext.JSON.encode(finalCols));

            } else {
                finalCols.push({ property: "empty", value: "empty" });
                self.grid.store.getProxy().setExtraParam("columns", Ext.JSON.encode(finalCols));
            }
            self.grid.store.filter(filters, {
                load: function (e) {
                    console.log(e)
                    if (param == 'exportall') { }
                }
            });



            break;

        case 'attendance-grid-filter-sortdirection':
        case 'attendance-grid-filter-sortoption':
            var grpoption = Ext.getCmp('rosterssortoptiondate').getGroupValue();
            if (grpoption == 'coursedateid') {
                var grpnym = Ext.getCmp('rosterssortdirectiondate').getValue();
            } else {
                var grpnym = Ext.getCmp('rosterssortdirectionname').getValue();
            }
            //self.store.group(grpoption,grpnym);
            self.store.sort(grpoption, grpnym);
            break;
        case 'attendance-export':
            var MaskLoading = new Ext.LoadMask(self.contentId, { msg: "Loading..." });
            var calltype = 'ajax';
            var callurl = 'datastores/datastore-reports_BO.asp?action=read';
            if ((self.portal.configuration.DotNetSiteRootUrl != "") && (self.portal.configuration.DotNetSiteRootUrl != 'null')) {
                debugger;
                calltype = 'jsonp';
                callurl = self.portal.configuration.DotNetSiteRootUrl + '/application/AdminFunction?call=portal-attendance-report-export'
                var finalCols = [];
                var tempCols = Ext.getCmp('AttendanceReportGrid').query('gridcolumn:not([hidden])');
                for (var x = 0; x < tempCols.length; x++) {
                    finalCols.push(tempCols[x].dataIndex);
                }
                if (finalCols.length == tempCols.length) {
                    var params =
                    {
                        sort: sort = Ext.encode(self.store.sorters.items),
                        filter: Ext.encode(self.store.filters.items),
                        columns: finalCols.toString()
                    }
                    console.log(finalCols.toString())
                    MaskLoading.show();
                    $.ajax({
                        url: callurl,
                        jsonp: "callback",
                        dataType: "jsonp",
                        data: params,
                        success: function (response) {
                            if (response) {
                                console.log(response.exportFileName); // server response
                                document.location = self.portal.configuration.DotNetSiteRootUrl + '/Temp/' + response.exportFileName;
                            }
                            MaskLoading.hide();
                        }
                    });
                }
            }
            else {
                self.exportGrid('datastores/datastore-reports_BO.asp');
            }
        default:
    }
}

InstructorEmailListandReportWidget.prototype.AttendanceReportData = function () {
    var self = this;
    Ext.Ajax.request({
        url: 'admin/datastores/datastore-configuration.asp?rndm=' + new Date().getTime() +'&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
        params: {
            action: 'config'
        },
        success: function (response) {
            configuration = Ext.JSON.decode(response.responseText).configuration;
            self.AttendanceReport(configuration);
        }
    });
}

InstructorEmailListandReportWidget.prototype.AttendanceReport = function (confg) {
    var self = this;
    var currentStudentId = 0;
    var currentStudentTotalAttendedHours = 0;
    self.store = PortalAttendanceStore.getStore();

    var columns =
         [
         {
             id: 'rosterid', dataIndex: 'RosterId', text: 'Roster ID', hidden: true, hideable: true
         },
         {
             id: 'coursename', dataIndex: 'CourseName', text: 'Course Name', hideable: true
         },
         {
             id: 'coursenum', dataIndex: 'CourseNum', text: 'Course Number', hideable: true
         },
         {
             id: 'courseid', dataIndex: 'CourseId', text: 'Course ID', hidden: true, hideable: true
         },
         {
             id: 'location', dataIndex: 'CourseLocation', text: 'Location', hideable: true
         },
         {
             id: 'coursedatestart', dataIndex: 'CourseDateStart', text: 'Start Date', align: 'center', hidden: false, hideable: true
         },
         {
             id: 'coursedateend', dataIndex: 'CourseDateEnd', text: 'End Date', align: 'center', hidden: false, hideable: true
         },
         {
             id: 'studentid', dataIndex: 'StudentId', text: 'Student ID', width: 0, align: 'left', hidden: true, hideable: false
         },
         {
             id: 'first', dataIndex: 'StudentFirstName', text: 'First Name', width: 110, align: 'left',
             //xtype: 'templatecolumn', tpl: '<tpl if="first != null"><a href="students_edit.asp?sid={StudentId}">{StudentFirstName}</a></tpl>'
         },
         {
             id: 'last', dataIndex: 'StudentLast', text: 'Last Name', width: 110, align: 'left',
             //xtype: 'templatecolumn', tpl: '<tpl if="first != null"><a href="students_edit.asp?sid={StudentId}">{StudentLast}</a></tpl>'
         },
         {
             id: 'StudentUsername', dataIndex: 'StudentUsername', text: 'Username', hideable: true, hidden: true,
         },
         {
             id: 'attdistrict', dataIndex: 'District', text: confg.districtfield, align: 'center', hidden: false, hideable: true
         },
         {
             id: 'attschool', dataIndex: 'School', text: confg.field2name, align: 'center', hidden: false, hideable: true
         },
         {
             id: 'attgradelevel', dataIndex: 'GradeLevel', text: confg.field1name, align: 'center', hidden: false, hideable: true
         },
         {
             id: 'studregfield1', dataIndex: 'StudRegField1', text: confg.studregfield1name, align: 'center', hidden: false, hideable: true
         },
         {
             id: 'attendancedate', dataIndex: 'AttendanceDate', text: 'Date', align: 'center', hidden: true, hideable: true
         },
         {
             id: 'attendancedatestring', dataIndex: 'AttendanceDateString', text: 'Attendance Date', align: 'center', hidden: false, hideable: true
         },
         {
             id: 'attended', dataIndex: 'Attended', text: 'Attended', align: 'center', hidden: false, hideable: true,
             renderer: function (value) {
                 if (value == 1) {
                     return '<span class="status-label status-yes"> Yes </span>';
                 }
                 else {
                     return '<span class="status-label status-no"> No </span>';
                 }
             }
         },
         {
             id: 'attendedhours', dataIndex: 'AttendedHours',
             text: confg.credithoursname,
             align: 'center', width: 100, hidden: false, hideable: true,
             renderer: function (value, metaData, record, row, col, store, gridView) {
                 var data = record.data;
                 var isLastAttendanceRecord = data.LastAttendanceRecord;
                 var totalAttendedHours = data.TotalAttendedHours;
                 if (isLastAttendanceRecord == 1) {
                     return '<span class="span-grid-total-section">' + value + '</span>';
                 }
                 else {
                     return value;
                 }
             }
         },
        {
            id: 'attendedhourstotal', dataIndex: 'TotalAttendedHours',
            text: 'Total ' + confg.credithoursname,
            align: 'center', width: 120, hidden: false, hideable: true,
            renderer: function (value, metaData, record, row, col, store, gridView) {
                var data = record.data;
                var isLastAttendanceRecord = data.LastAttendanceRecord;
                var totalAttendedHours = data.TotalAttendedHours;
                if (isLastAttendanceRecord == 1) {
                    return '<div class="total-grid"><span class="span-emphasis">' + value + '</span></div>';
                }
                else {
                    return '<div class="blank-grid">&nbsp</div>';
                }
            }
        },
        {
            id: 'inservicehours', dataIndex: 'InserviceHours',
            text: confg.inservicehoursname,
            align: 'center', hidden: false, hideable: true
        },
        {
            id: 'customcredithours', dataIndex: 'CustomCreditHours',
            text: confg.creditcustomname,
            align: 'center', hidden: false, hideable: true
        },
        {
            id: 'ceucredits', dataIndex: 'CEUCredits',
            text: confg.CEUCreditLabel + ' Credits',
            align: 'center', hidden: true, hideable: true,
            renderer: function (value, metaData, record, row, col, store, gridView) {
                var data = record.data;
                var isLastAttendanceRecord = data.LastAttendanceRecord;
                var totaldCreditHours = data.TotalCreditHours;
                if (isLastAttendanceRecord == 1) {
                    return '<span class="span-grid-total-section">' + value + '</span>';
                }
                else {
                    return value;
                }
            }
        },
        {
            id: 'graduatecredits', dataIndex: 'GraduateCredits',
            text: 'Graduate Credits',
            align: 'center', hidden: true, hideable: true,
            renderer: function (value, metaData, record, row, col, store, gridView) {
                var data = record.data;
                var isLastAttendanceRecord = data.LastAttendanceRecord;
                var totalGraduateCreditsHours = data.TotalGraduateCreditHours;
                if (isLastAttendanceRecord == 1) {
                    return '<span class="span-grid-total-section">' + value + '</span>';
                }
                else {
                    return value;
                }
            }
        },
        {
            id: 'attgrade', dataIndex: 'Grade', text: 'Grade', align: 'center', hidden: false, hideable: true
        },
        {
            id: 'MainCategories', dataIndex: 'MainCategories', text: 'Main Categories', align: 'center', hidden: false, hideable: true
        },
        {
            id: 'SubCategories', dataIndex: 'SubCategories', text: 'Sub Categories', align: 'center', hidden: false, hideable: true
        }
         ];



    // pluggable renders
    function renderWrap(value) {
        if (value == null) {
            return '';
        } else {
            return '<div style="white-space:normal !important; line-height:12px !important;">' + value + '</div>';
        }
    }

    function renderEdit(value) {
        if (value == null) { value = '' }
        return '<div style="background-image:url(images/icons/pencil.png); background-position:right; background-repeat:no-repeat; background-size:12px">&nbsp;' + value + '</div>';
    }

    function renderEditDate(value) {
        return '<div style="background-image:url(images/icons/pencil.png); background-position:right; background-repeat:no-repeat; background-size:12px">&nbsp;' + Ext.util.Format.date(value, 'm/d/Y') + '</div>';
    }

    function renderOrdrnum(value) {
        return '<div style="white-space:normal !important; line-height:12px !important;">' + value + '</br><a href="reports_editOrder.asp?id={ordernum}">{ordernum}</a></div>';
    }

    function renderStudName(value) {
        return '<div><a href="students_edit.asp?sid={[values.rows[0].studentid]}>' + value + '</a></div>';
    }

    function renderSummryMoney(value) {
        return '<div style="font-size:11px;text-decoration:underline; font-weight:800; background-color:#C9C9C9">' + Ext.util.Format.usMoney(value) + '</div>';
    }

    function renderCreditSummry(value) {
        return '<div style="font-size:11px;text-decoration:underline; font-weight:800;color:#FF0000; background-color:#C9C9C9">-' + Ext.util.Format.usMoney(value) + '</div>';
    }

    function renderCredit(value) {
        return '<div style="font-size:11px;color:#FF0000">-' + Ext.util.Format.usMoney(value) + '</div>';
    }

    function renderTotal(value) {
        return '<div style="font-size:11px;font-weight:800;text-align:right; background-color:#C9C9C9">Total</div>';
    }
    var tplSpace = '&nbsp;&nbsp;';
    var courseHeaderInfo = '{[values.rows[0].data["CourseName"]]} #{[values.rows[0].data["CourseNum"]]} | ID : {[values.rows[0].data["CourseId"]]} | ';
    var courseDateHeaderInfo = '{[values.rows[0].data["CourseDateStart"]]} - {[values.rows[0].data["CourseDateEnd"]]}';
    var courseLocationHeaderInfo = '{[values.rows[0].data["CourseLocation"]]}';
    var groupHeaderTplDiv = '<div class="group-header-tpl">';
    groupHeaderTplDiv += courseHeaderInfo + tplSpace + courseDateHeaderInfo + tplSpace + courseLocationHeaderInfo;
    groupHeaderTplDiv += '</div>';


    var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1
    });

    var ActvStudID = 0;

    var CFieldStoreChrtBar = Ext.create('Ext.data.JsonStore', {
        fields: [{ name: 'fldname', type: 'string' },
                 { name: 'valcnt', type: 'int' }],
        data: []
    });

    var CFieldStoreChrtPie = Ext.create('Ext.data.JsonStore', {
        fields: [{ name: 'fldname', type: 'string' },
                 { name: 'valcnt', type: 'int' }],
        data: []
    });


    //self.chartbar = Ext.create('Ext.chart.Chart', {
    //    id: 'AttendanceReportChartBar',
    //    xtype: 'chart',
    //    border: false,
    //    width: 600,
    //    height: 500,
    //    animate: true,
    //    //align : 'center',
    //    insetPadding: 50,
    //    shadow: true,
    //    store: CFieldStoreChrtBar,
    //    axes: [
    //        {
    //            type: 'Numeric',
    //            fields: [
    //                'valcnt'
    //            ],
    //            position: 'left',
    //            title: 'No. of students'
    //        },
    //        {
    //            type: 'Category',
    //            fields: [
    //                'fldname'
    //            ],
    //            position: 'bottom',
    //        }
    //    ],
    //    series: [
    //        {
    //            type: 'column',
    //            label: {
    //                display: 'insideEnd',
    //                field: 'valcnt',
    //                color: '#333',
    //                renderer: Ext.util.Format.numberRenderer('0,000'),
    //                'text-anchor': 'middle'
    //            },
    //            renderer: function (sprite, record, attr, index, store) {
    //                var value = 0;
    //                var color = ['rgb(121, 173, 62)'][value];
    //                return Ext.apply(attr, {
    //                    fill: color
    //                });
    //            },
    //            axis: 'bottom',
    //            xField: 'fldname',
    //            yField: [
    //                'valcnt'
    //            ]
    //        }
    //    ]
    //});


    //self.chartpie = Ext.create('Ext.chart.Chart', {
    //    id: 'AttendanceReportChartPie',
    //    width: 600,
    //    height: 500,
    //    animate: true,
    //    border: false,
    //    store: CFieldStoreChrtPie,
    //    //theme: 'Base:gradients',
    //    series: [{
    //        type: 'pie',
    //        angleField: 'valcnt',
    //        showInLegend: true,
    //        highlight: {
    //            segment: {
    //                margin: 20
    //            }
    //        },
    //        label: {
    //            field: 'fldname',
    //            display: 'rotate',
    //            contrast: true,
    //            font: '18px Arial'
    //        }
    //    }]
    //});

    self.panelbar = Ext.create('Ext.Panel', {
        id: 'AttendanceReportChartBarPanel',
        width: 600,
        height: 500,
        border: false,
        align: 'center',
        //items: [self.chartbar
        //]
    });

    self.panelpie = Ext.create('Ext.Panel', {
        id: 'AttendanceReportChartPiePanel',
        width: 600,
        height: 500,
        border: false,
        align: 'center',
        //items: [self.chartpie
        //]
    });

    self.pagingtoolbar = Ext.create('Ext.toolbar.Paging', {
        dock: 'bottom',
        store: self.store,
        displayInfo: true,
        displayMsg: 'Displaying {0} - {1} of {2}'
    });


    self.grid = Ext.create('Ext.grid.Panel', {
        id: 'AttendanceReportGrid',
        flex: 1,
        height: 500,
        store: self.store,
        columns: columns,
        //dockedItems: [self.searchtoolbar,self.searchoptiontoolbar,self.cattoolbar,self.searchtoolbar2,self.pagingtoolbar],
        stateful: true,
        stateId: 'portal-attendnace-grid',
        features: [{
            id: 'group',
            ftype: 'groupingsummary',
            groupHeaderTpl: groupHeaderTplDiv,
            hideGroupedHeader: true,
            collapsible: false,
            enableGroupingMenu: false,
        }],
        plugins: [cellEditing],
        loadMask: true,
        emptyText: 'No Matching Records',
        viewConfig: {
            getRowClass: function (record, rowIndex, rowParams, store) {
                var data = record.data;
                var isLastAttendanceRecord = data.LastAttendanceRecord;
                if (isLastAttendanceRecord == 1) {
                    return "total-row";
                }
                return '';
            }
        },
        listeners: {
            afterRender: function () {
                console.log('after render')
                setTimeout(function () {
                    self.executeCommand('attendance-executequery', 'noexport');
                }, 1000);
                //




            }
        },




    });

    //toolbars

    var input = Ext.create('Ext.form.field.Text', {
        id: 'searchname',
        emptyText: 'Course Name, Course Number, Student Name, Course Location',
        width: 400,
        labelStyle: 'width:50px',
        cls: 'search_name_cls',
        fieldLabel: 'Search',
    });

    var now = new Date();
    var defaultFromDate = new Date()
    var defaultToDate = new Date();
    //defaultFromDate.setDate(now.getDate()-15);
    defaultFromDate.setDate(now.getDate() - 60);
    defaultToDate.setDate(now.getDate() + 15);

    var fromDate = Ext.create('Ext.form.field.Date', {
        style: {
            float: 'right',
            marginRight: '8px',
            marginLeft: '8px'
        },
        renderTo: self.DATE_FILTER_ID,
        emptyText: 'From',
        width: 230,
        cls: 'date_from_filter_roster',
        fieldLabel: 'Course Date:',
        labelStyle: 'width:80px',
        value: defaultFromDate,
        listeners: {
            change: function (field, newvalue, oldvalue, eopts) {
                if (self.dateFilters.simple) {
                    return;
                }

                self.dateFilters.from.newvalue = newvalue;
                self.dateFilters.from.oldvalue = oldvalue;

                if (this.getErrors().length == 0) {
                    if (typeof (newvalue) == 'string') {
                        return;
                    }
                    toDate.setMinValue(newvalue);
                }
            }
        }
    });

    var toDate = Ext.create('Ext.form.field.Date', {
        style: {
            float: 'left'
        },
        renderTo: self.DATE_FILTER_ID,
        emptyText: 'To',
        width: 150,
        cls: 'date_to_filter_roster',
        value: defaultToDate,
        listeners: {
            change: function (field, newvalue, oldvalue, eopts) {
                if (self.dateFilters.simple) {
                    return;
                }

                self.dateFilters.to.newvalue = newvalue;
                self.dateFilters.to.oldvalue = oldvalue;

                if (this.getErrors().length == 0) {
                    if (typeof (newvalue) == 'string') {
                        return;
                    }
                    fromDate.setMaxValue(newvalue);
                }

            }
        }
    });


    var getSimpleDateFilterType = function (fromvalue, tovalue) {

        if (typeof (fromvalue) == 'string') {
            return 'none';
        }
        if (typeof (tovalue) == 'string') {
            return 'none';
        }

        if (!Ext.isEmpty(tovalue)) {
            tovalue = tovalue.format(Portal.SERVER_DATE_FORMAT);
        }

        if (!Ext.isEmpty(fromvalue)) {
            fromvalue = fromvalue.format(Portal.SERVER_DATE_FORMAT);
        }

        var result = 'none';
        var isAll = Ext.isEmpty(tovalue) && Ext.isEmpty(fromvalue);
        if (isAll) {
            result = 'all';
        }

        var isPast = tovalue == Portal.SERVER_DATE && Ext.isEmpty(fromvalue);
        if (isPast) {
            result = 'past';
        }

        var isCurrent = Ext.isEmpty(tovalue) && fromvalue == Portal.SERVER_DATE;
        if (isCurrent) {
            result = 'current';
        }

        return result;
    };

    self.dateFilters.date.from = fromDate;
    self.dateFilters.date.to = toDate;

    self.searchtoolbar = Ext.create('Ext.toolbar.Toolbar', {
        flex: 1,
        height: 30,
        dock: 'top',
        items: [
            {
                xtype: 'tbfill',
                flex: 9
            },

            input,
            self.dateFilters.date.from,
            self.dateFilters.date.to,
             {
                 xtype: 'button',
                 style: {
                     float: 'left'
                 },
                 text: 'Search',
                 icon: '/Images/Icons/FamFamFam/magnifier.png',
                 listeners: {
                     click: function (button, e, options) {
                         self.executeCommand('attendance-executequery', 'noexport');
                     }
                 }

             },
             {
                 xtype: 'button',
                 style: {
                     float: 'left'
                 },
                 text: 'Export to excel',
                 icon: '/Images/Icons/FamFamFam/page_excel.png',
                 listeners: {
                     //Export to excel
                     click: function (button, e, options) {
                         //self.executeCommand('attendance-export');
                         self.executeCommand('attendance-executequery', 'exportall');
                     }
                 }


        }
        ]
    });




    var storesortdirectiondate = Ext.create('Ext.data.Store', {
        fields: ['vlue', 'name'],
        data: [
			{ "vlue": "ASC", "name": "Oldest to Newest" },
			{ "vlue": "DESC", "name": "Newest to Oldest" }
        ]
    });

    var storesortdirectionanme = Ext.create('Ext.data.Store', {
        fields: ['vlue', 'name'],
        data: [
			{ "vlue": "ASC", "name": "A to Z" },
			{ "vlue": "DESC", "name": "Z to A" }
        ]
    });

    var sortdirectiondate = Ext.create('Ext.form.field.ComboBox', {
        id: 'rosterssortdirectiondate',
        store: storesortdirectiondate,
        displayField: 'name',
        valueField: 'vlue',
        width: 130,
        value: 'DESC',
    });
    sortdirectiondate.on('change', function (combo, newVal, oldVal, options) {
        self.executeCommand('attendance-grid-filter-sortdirection', newVal);
    }, sortdirectiondate, { buffer: 500 });

    var sortdirectionnanme = Ext.create('Ext.form.field.ComboBox', {
        id: 'rosterssortdirectionname',
        store: storesortdirectionanme,
        displayField: 'name',
        valueField: 'vlue',
        width: 60,
        value: 'ASC',
        disabled: true
    });
    sortdirectionnanme.on('change', function (combo, newVal, oldVal, options) {
        self.executeCommand('attendance-grid-filter-sortdirection', newVal);
    }, sortdirectionnanme, { buffer: 500 });

    var includeCancelledRoster = Ext.create('Ext.form.RadioGroup', {
        id: 'includeCancelledRoster',
        fieldLabel: 'Include Cancelled',
        width: 250,
        labelWidth: 100,
        bodyPadding: 10,
        style: 'float:left!important;padding-left:5px;left:0px!important',
        items: [
				{
				    boxLabel: 'Yes',
				    id: 'rosterscancalledoptionnameyes',
				    name: 'rbc',
				    inputValue: 1,
				    width: 50,
				    checked: true
				},
				{
				    boxLabel: 'No',
				    id: 'rosterscancalledoptionnameno',
				    name: 'rbc',
				    inputValue: 0,
				    width: 50

				}
        ]
    });


    var sortoption = Ext.create('Ext.form.RadioGroup', {
        fieldLabel: 'Sort by',
        width: 500,
        labelWidth: 70,
        bodyPadding: 10,
        style: 'padding-left:5px;left:0px!important',
        items: [
				{
				    boxLabel: 'Course Name',
				    id: 'rosterssortoptionname',
				    name: 'rb',
				    inputValue: 'coursenameid',
				    width: 100,
				},
				sortdirectionnanme,
				{
				    boxLabel: 'Course Date',
				    id: 'rosterssortoptiondate',
				    name: 'rb',
				    inputValue: 'coursedateid',
				    checked: true,
				    width: 100,
				    style: {
				        marginLeft: '10px'
				    }
				},
				sortdirectiondate,
				{
				    xtype: 'tbfill',
				}

        ]
    });

    sortoption.on('change', function (combo, newVal, oldVal, options) {

        var grpoption = Ext.getCmp('rosterssortoptiondate').getGroupValue();
        if (grpoption == 'coursedateid') {
            Ext.getCmp('rosterssortdirectiondate').enable();
            Ext.getCmp('rosterssortdirectionname').disable();

        } else {
            Ext.getCmp('rosterssortdirectionname').enable();
            Ext.getCmp('rosterssortdirectiondate').disable();
        }

        self.executeCommand('attendance-grid-filter-sortoption');
    }, sortoption, { buffer: 500 });


    self.searchoptiontoolbar = Ext.create('Ext.toolbar.Toolbar', {
        flex: 1,
        heght: 20,
        dock: 'top',
        items: [
					//includeCancelledRoster,
					sortoption
        ]
    });

    winreport = Ext.create('Ext.form.Panel', {
        floating: true,
        centered: true,
        modal: true,
        width: 970,
        height: 600,
        closable: true,
        bodyPadding: 10,
        title: 'Attendance Report',
        dockedItems: [
            //self.tabmenu,
            self.searchtoolbar,
            self.searchoptiontoolbar,
            //self.cattoolbar,
            //self.searchtoolbar2,
            //self.pagingtoolbar
        ],
        items: [self.grid],
    });
    winreport.show();
}



InstructorEmailListandReportWidget.prototype.RosterReportAllCourses = function () {
    var self = this;
    Ext.Ajax.request({
        url: "admin/datastores/datastore-reports_BO.asp?action=getVariables&rndm=" + new Date().getTime() +'&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
        success: function (response) {
            var dtaArray = eval("[" + response.responseText + "]");
            var GobalVar = dtaArray[0]
            self.RosterReport('rosterreport', 0, '', GobalVar);
        }
    });
}
InstructorEmailListandReportWidget.prototype.RosterReport = function (cmd, cid,coursename, GobalVar) {
    var self = this;
    self.store = PortalRostersStore.getStore(cid);
    if (GobalVar.AccountNum == "Hide") { AccountNumHide = true } else { AccountNumHide = false }
    if (GobalVar.ShowInvoice == "Hide") { ShowInvoiceHide = true } else { ShowInvoiceHide = false }
    if (GobalVar.HiddenStudRegField1Name == "") { HiddenStudRegField1NameHide = true } else { HiddenStudRegField1NameHide = false }
    if (GobalVar.HiddenStudRegField2Name == "") { HiddenStudRegField2NameHide = true } else { HiddenStudRegField2NameHide = false }
    if (GobalVar.ReadOnlyStudRegField1Name == "") { ReadOnlyStudRegField1NameHide = true } else { ReadOnlyStudRegField1NameHide = false }
    if (GobalVar.ReadOnlyStudRegField2Name == "") { ReadOnlyStudRegField2NameHide = true } else { ReadOnlyStudRegField2NameHide = false }
    if (GobalVar.ReadOnlyStudRegField3Name == "") { ReadOnlyStudRegField3NameHide = true } else { ReadOnlyStudRegField3NameHide = false }
    if (GobalVar.ReadOnlyStudRegField4Name == "") { ReadOnlyStudRegField4NameHide = true } else { ReadOnlyStudRegField4NameHide = false }
    if (GobalVar.Field3Name === null) { Field3Hide = true; Field3Hideable = false } else { Field3Hide = false; Field3Hideable = true }
    if (GobalVar.Field2Name === null) { Field2Hide = true; Field2Hideable = false } else { Field2Hide = false; Field2Hideable = true }
    if (GobalVar.Field1Name === null) { Field1Hide = true; Field1Hideable = false } else { Field1Hide = false; Field1Hideable = true }
    var search = Ext.create('Ext.form.field.Text', {
        id: 'searchtxtidclasslist',
        emptyText: 'Search: Student Name, Course Name, Course Number',
        width: 820
    });
    search.on('change', function (that, newvalue, oldvalue) {
        Ext.getCmp('RosterReportGrid').getStore().filter([
    { id: 'keyword', property: 'keyword', value: newvalue }
        ]);
    }, null, { buffer: 500 });
    var button_export_roster = Ext.create('Ext.Button', {
        text: '<div style="margin-bottom:0px;"><img style="float:left;" src="/Images/Icons/FamFamFam/page_excel.png" />&nbsp;&nbsp;Export to Excel</div>',
     handler: function () {
            var finalCols = [];
            var maxcol = 50;
            var tempCols = Ext.getCmp('RosterReportGrid').query('gridcolumn:not([hidden])');
            for (var x = 0; x < tempCols.length; x++) {
                if (tempCols[x].dataIndex != 'count') {
                    if (maxcol != 0) {
                        finalCols.push({ property: tempCols[x].dataIndex, value: tempCols[x].text });
                        maxcol = maxcol - 1;
                    }
                }
            }
            self.grid.store.getProxy().setExtraParam("columns", Ext.JSON.encode(finalCols));
            Ext.getCmp('RosterReportGrid').getStore().load({
                scope: this,
                params :{isexport:'Yes'},
                callback: function (records, operation, success) {
                    if (success) {
                        window.location = "/Temp/" + "RosterReport_" +"export"+ cid + "_" + CurrentInstructorId + ".csv";
                        Ext.getCmp('RosterReportGrid').getStore().load();
                    } else {
                        console.log('error');
                    }
                }
            });
            

        }
    });
    var StudentsearchBar = Ext.create('Ext.toolbar.Toolbar', {
        dock: 'top',
        height: 30,
        width:600,
        items: [
			{
			    xtype: 'container',
			    layout: 'hbox',
			    padding: '0 0 0 0',
			    items: [
					search, button_export_roster
			    ]
			}],
    });
    var groupHeaderTplDiv = '<div style="vertical-align:middle; height:30px; background-color:#575757;color:#FFFFFF";">';
    var TplSpace = '&nbsp;&nbsp;';
    groupHeaderTplDiv += '<font style="font-size:13px;">' + TplSpace;
    groupHeaderTplDiv += '{[values.rows[0].data["coursename"]]} - #{[values.rows[0].data["coursenum"]]}';
    groupHeaderTplDiv += TplSpace + ' | ' + TplSpace + 'ID:{[values.rows[0].data["courseid"]]}';
    groupHeaderTplDiv += TplSpace + ' | ' + TplSpace + '{[values.rows[0].data["startdate"]]} - {[values.rows[0].data["enddate"]]}';
    groupHeaderTplDiv += TplSpace + ' | ' + TplSpace + '{[values.rows[0].data["Location"]]}';
    groupHeaderTplDiv += '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;';
    groupHeaderTplDiv += '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;';
    groupHeaderTplDiv += '{[values.rows[0].instructor.length > 6 ? "' + Terminology.capital('instructor') + '(s):   " : ""]}';
    groupHeaderTplDiv += '{[values.rows[0].data["instructor"]]}';
    groupHeaderTplDiv += '{[values.rows[0].instructor2.length > 6 ? ", " : ""]}';
    groupHeaderTplDiv += '{[values.rows[0].instructor2]}';
    groupHeaderTplDiv += '{[values.rows[0].instructor3.length > 6 ? ", " : ""]}';
    groupHeaderTplDiv += '{[values.rows[0].instructor3]}</font>';
    groupHeaderTplDiv += '</div>';
    var columns = [{
        dataIndex: 'count', text: 'count', width: 40
    }, {
        id: 'rosterid', dataIndex: 'rosterid', text: 'Roster ID', width: 60, hidden: true, hideable: true
    }, {
        id: 'coursenameid', dataIndex: 'coursenameid', text: 'Course Name ID', width: 220, hidden: true, hideable: false
    }, {
        id: 'coursename', dataIndex: 'coursename', text: 'Course Name', width: 1, hidden: false, hideable: false
    }, {
        id: 'coursenum', dataIndex: 'coursenum', text: 'Course Number', width: 1, hidden: false, hideable: false
    },
         {
             id: 'courseid', dataIndex: 'courseid', text: 'Course ID', width: 1, hidden: false, hideable: false
         },
         {
             id: 'courselocation', dataIndex: 'courselocation', text: 'courselocation', width: 0, align: 'center', hidden: true, hideable: false
         }, {
             id: 'instructor', dataIndex: 'instructor', text: Terminology.capital('instructor'), width: 0, align: 'center', hidden: true, hideable: false
         }, {
             id: 'instructor2', dataIndex: 'instructor2', text: Terminology.capital('instructor'), width: 0, align: 'center', hidden: true, hideable: false
         }, {
             id: 'instructor3', dataIndex: 'instructor3', text: Terminology.capital('instructor'), width: 0, align: 'center', hidden: true, hideable: false
         }, {
             id: 'studentid', dataIndex: 'studentid', text: 'Student ID', width: 0, align: 'left', hidden: true, hideable: false
         }, {
             id: 'last', dataIndex: 'last', text: 'Last Name', width: 110, align: 'left',
             xtype: 'templatecolumn', tpl: '<tpl if="last != null">{last}</tpl>'
         }, {
             id: 'first', dataIndex: 'first', text: 'First Name', width: 110, align: 'left',
             xtype: 'templatecolumn', tpl: '<tpl if="first != null">{first}</tpl>'
         }, {
             dataIndex: 'district', text: GobalVar.Field3Name, renderer: renderWrap, width: 150, align: 'left', hidden: Field3Hide, hideable: Field3Hideable
         }, {
             dataIndex: 'daddress', text: GobalVar.Field3Name + " Address", renderer: renderWrap, width: 150, align: 'left', hidden: Field3Hide, hideable: Field3Hideable
         }, {
             dataIndex: 'studentschool', text: GobalVar.Field2Name, renderer: renderWrap, width: 140, align: 'left', hidden: Field2Hide, hideable: Field3Hideable
         }, {
             dataIndex: 'studentgradelevel', text: GobalVar.Field1Name, renderer: renderWrap, width: 100, align: 'left', hidden: Field1Hide, hideable: Field1Hideable
         }, {
             dataIndex: 'cancelledtxt', text: 'Cnxl?', width: 50, align: 'center'
         }, {
             dataIndex: 'waitingtxt', text: 'Waiting?', width: 60, align: 'center'
         }, {
             dataIndex: 'attendedtxt', text: 'Attended?', width: 60, align: 'center'
         }, {
             dataIndex: 'paidfulltxt', text: 'Paid', width: 50, align: 'center',
             summaryType: 'text', summaryRenderer: renderTotal
         }, {
             id: 'course', dataIndex: 'course', text: 'Course', width: 90, align: 'right',
             summaryType: 'sum', summaryRenderer: renderSummryMoney,
             xtype: 'templatecolumn', tpl: '{course:usMoney()}'
         },
        {
            id: 'startdate', dataIndex: 'startdate', text: 'Start Date', width: 1, align: 'center', hideable: true
        },
        {
            id: 'enddate', dataIndex: 'enddate', text: 'End Date', width: 1, align: 'center', hideable: true
        },
         {
             id: 'material', dataIndex: 'material', text: 'Materials', width: 90, align: 'right',
             summaryType: 'sum', summaryRenderer: renderSummryMoney,
             xtype: 'templatecolumn', tpl: '{material:usMoney()}'
         }, {
             id: 'coursetotal', dataIndex: 'coursetotal', text: 'Course Total', width: 90, align: 'right',
             summaryType: 'sum', summaryRenderer: renderSummryMoney,
             xtype: 'templatecolumn', tpl: '{coursetotal:usMoney()}'
         }, {
             id: 'txtotal', dataIndex: 'txtotal', text: 'Transaction Total(before tax) ', width: 160, align: 'right',
             summaryType: 'sum', summaryRenderer: renderSummryMoney,
             xtype: 'templatecolumn', tpl: '{txtotal:usMoney()}'
         }, {
             id: 'amountpaid', dataIndex: 'amountpaid', text: 'Amount Paid', width: 90, align: 'right',
             summaryType: 'sum', summaryRenderer: renderSummryMoney,
             xtype: 'templatecolumn', tpl: '{amountpaid:usMoney()}'
         }, {
             dataIndex: 'credited', text: 'credited ', width: 90, align: 'right',
             summaryType: 'sum', renderer: renderCredit, summaryRenderer: renderCreditSummry
         }, {
             id: 'paymethod', dataIndex: 'paymethod', text: 'Payment Method', xtype: 'templatecolumn',
             tpl: '<div style="white-space:normal !important; line-height:12px !important;">{paymethod}</br><font style="font-size:10px;">Order#:{ordernum}</font></div>',
             width: 150, align: 'center'
         }, {
             dataIndex: 'materialnames', text: 'Material Names', renderer: renderWrap, width: 150, align: 'center', hidden: false
         }, {
             dataIndex: 'payNumber', text: "Payment Number", renderer: renderWrap, width: 100, align: 'center', hidden: false
         }, {
             dataIndex: 'authnum', text: "Authorization Number", renderer: renderWrap, width: 100, align: 'center', hidden: true
         }, {
             dataIndex: 'refnumber', text: "Reference Number", renderer: renderWrap, width: 100, align: 'center', hidden: true
         }, {
             dataIndex: 'studentgrade', text: "Student Grade", renderer: renderWrap, width: 100, align: 'left', hidden: true
         }, {
             id: 'ordernum', dataIndex: 'ordernum', text: 'OrderNum', renderer: renderWrap, width: 150, align: 'center', hidden: true, hideable: false
         }, {
             dataIndex: 'accountnum', text: 'Account #', width: 150, align: 'center', hidden: AccountNumHide, hideable: (AccountNumHide ? false : true)
         }, {
             dataIndex: 'internalnote', text: 'Order Notes', width: 130, align: 'center',
             renderer: renderEdit,
             editor: {
                 allowBlank: true,
                 listeners: {
                     focus: function () {
                         var selection = self.grid.getView().getSelectionModel().getSelection()[0];
                         ActiveOrderNum = selection.get('ordernum');
                     },
                     blur: function (t, ev, b) {
                         Ext.Ajax.request({
                             url: 'datastores/datastore-reports_BO.asp?action=update' + '&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
                             params: {
                                 fld: 'internalnote',
                                 jActiveOrderNum: ActiveOrderNum,
                                 vlu: t.lastValue
                             },
                             success: function (response) {
                                 //this will update other internalnote with same OrderNumber
                                 for (var irec = 0; irec < self.grid.getStore().data.length; irec++) {
                                     var record = self.grid.getStore().getAt(irec);
                                     if (record.data.ordernum == ActiveOrderNum) {
                                         record.set('internalnote', t.lastValue);
                                     }
                                 }

                                 //refresh grid to reflect updates to other courses with the same course number
                                 //self.grid.getStore().load();
                                 //self.grid.getView().reload();
                             },
                             failure: function (response) {
                                 alert('Failed to update.\nPlease contact your administrator.');
                             }
                         });
                     }
                 }
             }
         }, {
             dataIndex: 'enrollmentnote', text: Terminology.capital('enrollment') + ' Notes', width: 130, align: 'center',
             renderer: renderEdit,
             editor: {
                 allowBlank: true,
                 listeners: {
                     focus: function () {
                         var selection = self.grid.getView().getSelectionModel().getSelection()[0];
                         ActiveRosterId = selection.get('rosterid');
                     },
                     blur: function (t, ev, b) {
                         Ext.Ajax.request({
                             url: 'datastores/datastore-reports_BO.asp?action=update' + '&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
                             params: {
                                 fld: 'enrollmentnote',
                                 jActiveRosterId: ActiveRosterId,
                                 vlu: t.lastValue
                             },
                             success: function (response) {
                                 //refresh grid to reflect updates to other courses with the same course number
                                 //self.grid.getStore().load();
                                 //self.grid.getView().reload();
                             },
                             failure: function (response) {
                                 alert('Failed to update.\nPlease contact your administrator.');
                             }
                         });
                     }
                 }
             }
         }, {
             dataIndex: 'invoicenumber', text: 'Invoice Number', width: 130, align: 'center', hidden: ShowInvoiceHide, hideable: (ShowInvoiceHide ? false : true),
             renderer: renderEdit,
             editor: {
                 allowBlank: true,
                 listeners: {
                     focus: function () {
                         var selection = self.grid.getView().getSelectionModel().getSelection()[0];
                         ActiveRosterId = selection.get('rosterid');
                     },
                     blur: function (t, ev, b) {
                         Ext.Ajax.request({
                             url: 'datastores/datastore-reports_BO.asp?action=update' + '&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
                             params: {
                                 fld: 'invoicenumber',
                                 jActiveRosterId: ActiveRosterId,
                                 vlu: t.lastValue
                             },
                             success: function (response) {
                                 //refresh grid to reflect updates to other courses with the same course number
                                 //self.grid.getStore().load();
                                 //self.grid.getView().reload();
                             },
                             failure: function (response) {
                                 alert('Failed to update.\nPlease contact your administrator.');
                             }
                         });
                     }
                 }
             }
         }, {
             id: 'invoicedate', dataIndex: 'invoicedate', text: 'Invoice Date', width: 130, align: 'center', hidden: ShowInvoiceHide, hideable: (ShowInvoiceHide ? false : true),
             renderer: renderEditDate,
             editor: {
                 xtype: 'datefield',
                 allowBlank: true,
                 listeners: {
                     focus: function () {
                         var selection = self.grid.getView().getSelectionModel().getSelection()[0];
                         ActiveRosterId = selection.get('rosterid');
                     },
                     blur: function (t, ev, b) {
                         Ext.Ajax.request({
                             url: 'datastores/datastore-reports_BO.asp?action=update' + '&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
                             params: {
                                 fld: 'invoicedate',
                                 jActiveRosterId: ActiveRosterId,
                                 vlu: t.lastValue
                             },
                             success: function (response) {
                                 //refresh grid to reflect updates to other courses with the same course number
                                 //self.grid.getStore().load();
                                 //self.grid.getView().reload();
                             },
                             failure: function (response) {
                                 alert('Failed to update.\nPlease contact your administrator.');
                             }
                         });
                     }
                 }
             }
         }, {
             dataIndex: 'coursechoice', text: 'Course Choice', renderer: renderWrap, width: 150, align: 'center', hidden: true, hideable: true
         }, {
             dataIndex: 'homephone', text: GobalVar.homephoneFS, renderer: renderWrap, width: 150, align: 'center', hidden: (GobalVar.VisibleStudentHOMEPHONE == 0 ? true : false), hideable: (GobalVar.VisibleStudentHOMEPHONE != 0 ? true : false)
         }, {
             dataIndex: 'workphone', text: GobalVar.workphoneFS, renderer: renderWrap, width: 150, align: 'center', hidden: (GobalVar.VisibleStudentWORKPHONE == 0 ? true : false), hideable: (GobalVar.VisibleStudentWORKPHONE != 0 ? true : false)
         }, {
             dataIndex: 'fax', text: GobalVar.faxFS, renderer: renderWrap, width: 150, align: 'center', hidden: (GobalVar.VisibleStudentFAX == 0 ? true : false), hideable: (GobalVar.VisibleStudentFAX != 0 ? true : false)
         }, {
             dataIndex: 'email', text: GobalVar.emailFS, renderer: renderWrap, width: 150, align: 'center', hidden: (GobalVar.VisibleStudentEMAIL == 0 ? true : false), hideable: (GobalVar.VisibleStudentEMAIL != 0 ? true : false)
         }, {
             dataIndex: 'completeaddress', text: GobalVar.addressFS, renderer: renderWrap, width: 200, align: 'center', hidden: (GobalVar.VisibleStudentADDRESS == 0 ? true : false), hideable: (GobalVar.VisibleStudentADDRESS != 0 ? true : false)
         }, {
             dataIndex: 'hiddenstudregfield1', text: GobalVar.HiddenStudRegField1Name, renderer: renderWrap, width: 150, align: 'center', hidden: HiddenStudRegField1NameHide, hideable: (HiddenStudRegField1NameHide ? false : true)
         }, {
             dataIndex: 'hiddenstudregfield2', text: GobalVar.HiddenStudRegField2Name, renderer: renderWrap, width: 150, align: 'center', hidden: HiddenStudRegField2NameHide, hideable: (HiddenStudRegField2NameHide ? false : true)
         }, {
             dataIndex: 'readonlystudregfield1', text: GobalVar.ReadOnlyStudRegField1Name, renderer: renderWrap, width: 150, align: 'center', hidden: ReadOnlyStudRegField1NameHide, hideable: (ReadOnlyStudRegField1NameHide ? false : true)
         }, {
             dataIndex: 'readonlystudregfield2', text: GobalVar.ReadOnlyStudRegField2Name, renderer: renderWrap, width: 150, align: 'center', hidden: ReadOnlyStudRegField2NameHide, hideable: (ReadOnlyStudRegField2NameHide ? false : true)
         }, {
             dataIndex: 'readonlystudregfield3', text: GobalVar.ReadOnlyStudRegField3Name, renderer: renderWrap, width: 150, align: 'center', hidden: ReadOnlyStudRegField3NameHide, hideable: (ReadOnlyStudRegField3NameHide ? false : true)
         }, {
             dataIndex: 'readonlystudregfield4', text: GobalVar.ReadOnlyStudRegField4Name, renderer: renderWrap, width: 150, align: 'center', hidden: ReadOnlyStudRegField4NameHide, hideable: (ReadOnlyStudRegField4NameHide ? false : true)
         }, {
             dataIndex: 'studregfield1', text: GobalVar.StudRegField1Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField1Name, hideable: (GobalVar.hideStudRegField1Name ? false : true)
         }, {
             dataIndex: 'studregfield2', text: GobalVar.StudRegField2Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField2Name, hideable: (GobalVar.hideStudRegField2Name ? false : true)
         }, {
             dataIndex: 'studregfield3', text: GobalVar.StudRegField3Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField3Name, hideable: (GobalVar.hideStudRegField3Name ? false : true)
         }, {
             dataIndex: 'studregfield4', text: GobalVar.StudRegField4Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField4Name, hideable: (GobalVar.hideStudRegField4Name ? false : true)
         }, {
             dataIndex: 'studregfield5', text: GobalVar.StudRegField5Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField5Name, hideable: (GobalVar.hideStudRegField5Name ? false : true)
         }, {
             dataIndex: 'studregfield6', text: GobalVar.StudRegField6Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField6Name, hideable: (GobalVar.hideStudRegField6Name ? false : true)
         }, {
             dataIndex: 'studregfield7', text: GobalVar.StudRegField7Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField7Name, hideable: (GobalVar.hideStudRegField7Name ? false : true)
         }, {
             dataIndex: 'studregfield8', text: GobalVar.StudRegField8Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField8Name, hideable: (GobalVar.hideStudRegField8Name ? false : true)
         }, {
             dataIndex: 'studregfield9', text: GobalVar.StudRegField9Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField9Name, hideable: (GobalVar.hideStudRegField9Name ? false : true)
         }, {
             dataIndex: 'studregfield10', text: GobalVar.StudRegField10Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField10Name, hideable: (GobalVar.hideStudRegField10Name ? false : true)
         }, {
             dataIndex: 'studregfield11', text: GobalVar.StudRegField11Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField11Name, hideable: (GobalVar.hideStudRegField11Name ? false : true)
         }, {
             dataIndex: 'studregfield12', text: GobalVar.StudRegField12Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField12Name, hideable: (GobalVar.hideStudRegField12Name ? false : true)
         }, {
             dataIndex: 'studregfield13', text: GobalVar.StudRegField13Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField13Name, hideable: (GobalVar.hideStudRegField13Name ? false : true)
         }, {
             dataIndex: 'studregfield14', text: GobalVar.StudRegField14Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField14Name, hideable: (GobalVar.hideStudRegField14Name ? false : true)
         }, {
             dataIndex: 'studregfield15', text: GobalVar.StudRegField15Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField15Name, hideable: (GobalVar.hideStudRegField15Name ? false : true)
         }, {
             dataIndex: 'studregfield16', text: GobalVar.StudRegField16Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField16Name, hideable: (GobalVar.hideStudRegField16Name ? false : true)
         }, {
             dataIndex: 'studregfield17', text: GobalVar.StudRegField17Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField17Name, hideable: (GobalVar.hideStudRegField17Name ? false : true)
         }, {
             dataIndex: 'studregfield18', text: GobalVar.StudRegField18Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField18Name, hideable: (GobalVar.hideStudRegField18Name ? false : true)
         }, {
             dataIndex: 'studregfield19', text: GobalVar.StudRegField19Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField19Name, hideable: (GobalVar.hideStudRegField19Name ? false : true)
         }, {
             dataIndex: 'studregfield20', text: GobalVar.StudRegField20Name, renderer: renderWrap, width: 150, align: 'center', hidden: GobalVar.hideStudRegField20Name, hideable: (GobalVar.hideStudRegField20Name ? false : true)
         }, {
             dataIndex: 'address', text: 'Address', renderer: renderWrap, width: 150, align: 'center', hidden: true
         }, {
             dataIndex: 'city', text: 'City', renderer: renderWrap, width: 150, align: 'center', hidden: true
         }
         , {
             dataIndex: 'state', text: 'State', renderer: renderWrap, width: 150, align: 'center', hidden: true
         }, {
             dataIndex: 'zip', text: 'Zip', renderer: renderWrap, width: 150, align: 'center', hidden: true
         }, {
             id: 'dateadded', dataIndex: 'dateadded', text: 'Date Added', width: 150, align: 'center'
         }, {
             id: 'crinitialauditinfo', dataIndex: 'crinitialauditinfo', text: 'Wait List Added Date', width: 150, align: 'center',
             renderer: function (value, metaData, record, row, col, store, gridView) {
                 if (value) {
                     if (value.indexOf('WaitListEnrolledDate:') > -1) {
                         return value.split('WaitListEnrolledDate:')[1];
                     }
                     else {
                         return value;
                     }
                 }
             }
         },
         {
             id: 'maincategory', dataIndex: 'maincategory', text: 'Main Category', width: 110, hidden: false, hideable: true
         },
         {
             id: 'subcategory', dataIndex: 'subcategory', text: 'Sub-Category', width: 110, hidden: false, hideable: true
         }
    ];


    // pluggable renders
    function renderWrap(value) {
        if (value == null) {
            return '';
        } else {
            return '<div style="white-space:normal !important; line-height:12px !important;">' + value + '</div>';
        }
    }

    function renderEdit(value) {
        if (value == null) { value = '' }
        return '<div style="background-image:url(images/icons/pencil.png); background-position:right; background-repeat:no-repeat; background-size:12px">&nbsp;' + value + '</div>';
    }

    function renderEditDate(value) {
        return '<div style="background-image:url(images/icons/pencil.png); background-position:right; background-repeat:no-repeat; background-size:12px">&nbsp;' + Ext.util.Format.date(value, 'm/d/Y') + '</div>';
    }

    function renderOrdrnum(value) {
        return '<div style="white-space:normal !important; line-height:12px !important;">' + value + '</br>{ordernum}</div>';
    }

    function renderStudName(value) {
        return '<div><a href="students_edit.asp?sid={[values.rows[0].studentid]}>' + value + '</a></div>';
    }

    function renderSummryMoney(value) {
        return '<div style="font-size:11px;text-decoration:underline; font-weight:800; background-color:#C9C9C9">' + Ext.util.Format.usMoney(value) + '</div>';
    }

    function renderCreditSummry(value) {
        return '<div style="font-size:11px;text-decoration:underline; font-weight:800;color:#FF0000; background-color:#C9C9C9">-' + Ext.util.Format.usMoney(value) + '</div>';
    }

    function renderCredit(value) {
        return '<div style="font-size:11px;color:#FF0000">-' + Ext.util.Format.usMoney(value) + '</div>';
    }

    function renderTotal(value) {
        return '<div style="font-size:11px;font-weight:800;text-align:right; background-color:#C9C9C9">Total</div>';
    }

    var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1
    });

    var ActvStudID = 0;
    var button_export_roster = Ext.create('Ext.Button', {
        text: '<div style="margin-bottom:0px;"><img style="float:left;" src="/Images/Icons/FamFamFam/page_excel.png" />&nbsp;&nbsp;Export to Excel</div>',
   
    });

    self.pagingtoolbar = Ext.create('Ext.toolbar.Paging', {
        dock: 'bottom',
        store: self.store,
        displayInfo: true,
        displayMsg: 'Displaying {0} - {1} of {2}'
    });

    self.grid = Ext.create('Ext.grid.Panel', {
        id: 'RosterReportGrid',
        flex: 1,
        height: 530,
        store: self.store,
        columns: columns,
        dockedItems: [
            StudentsearchBar, {
                xtype: 'pagingtoolbar',
                store: self.store,
                dock: 'top',
                 displayInfo: true
            }],
        stateful: true,
        stateId: 'portal-rosters-grid',
        features: [{
            id: 'group',
            ftype: 'groupingsummary',
            groupHeaderTpl: groupHeaderTplDiv,
            hideGroupedHeader: true,
            collapsible: false,
            enableGroupingMenu: false,
        }],
        plugins: [cellEditing],
        loadMask: true,
        emptyText: 'No Matching Records'
    });
    winreport = Ext.create('Ext.form.Panel', {
        floating: true,
        centered: true,
        modal: true,
        width: 970,
        height: 600,
        closable: true,
        bodyPadding: 10,
        title: 'Roster Report: ' +coursename,
        items: [self.grid],
    });
    winreport.show();

}
InstructorEmailListandReportWidget.prototype.SigninSheet = function (cmd,cid)
{
    var self = this;
    winreport = Ext.create('Ext.form.Panel', {
        id: 'InstructorWidgetSigninSheetPanel',
        floating: true,
        centered: true,
        modal: true,
        width: 970,
        height: 640,
        closable: true,
        defaultType: 'textfield',
        bodyPadding: 10,
        items: [
			{
			    xtype: 'component',
			    autoScroll: true,
			    autoEl: {
			        tag: 'iframe',
			        id: 'InstructorWidgetSigninSheetIframe',
			        style: 'height: 650px; width: 950px;',
			        src: '/admin/reports_roster_classlist.asp?cid=' + cid + '&Caller=public&Print_all=&misc=465&misc=552&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId
			    },
			    listeners: {
			        afterrender: function () {
			            this.getEl().on('load', function () {
			                var iframeHT = $("#InstructorWidgetSigninSheetIframe").height()
			                var panel = Ext.getCmp('InstructorWidgetSigninSheetPanel');
			                panel.setHeight(iframeHT)
			            });
			        }
			    }
			}
        ],

        //html: '<iframe width="950" height="630" src="/admin/reports_roster_classlist.asp?cid='+cid+'&Caller=public&Print_all=&misc=465&misc=552&rubyrequest=1&uname=' + username + '"></iframe>',

        renderTo: Ext.getBody(),
    });
    winreport.show();
    //self.LogoffClassic();
}

InstructorEmailListandReportWidget.prototype.SigninSheet_New = function (cmd, cid) {
    url = 'Application/AdminFunction';
    buildNewPortalSignInSheet(cid);
    /*
    Ext.Ajax.request({
        url: url,
        params: {
            call: 'portal-signinsheet',
            courseId: cid,
        },
        success: function (response) {
            var result = response.responseText;
            result = result.replace('("', '').replace('");', '');
            window.open('/temp/'+result, '_blank');
           // document.location = result;
        }
    });
    */
}

InstructorEmailListandReportWidget.prototype.SendSurvey = function (cmd, cid) {
    url = 'Instructor/SendSurvey';
    window.LAYOUT.MaskLayout('Sending Survey...');
    Ext.Ajax.request({
        url: url,
        params: {
            call: 'instructor-sendsurvey',
            courseId: cid,
        },
        success: function (response) {
            var result = response.responseText;
            result = result.replace('("', '').replace('");', '');
            
            // document.location = result;
            window.LAYOUT.UnmaskLayout();
            alert(result);
        }
    });
}


InstructorEmailListandReportWidget.prototype.NameTags = function (cmd, cid) {
    var self = this;
    winreport = Ext.create('Ext.form.Panel', {
        id: 'InstructorWidgetNameTagsPanel',
        floating: true,
        centered: true,
        modal: true,
        width: 970,
        height: 640,
        closable: true,
        defaultType: 'textfield',
        bodyPadding: 10,
        items: [
			{
			    xtype: 'component',
			    autoScroll: true,
			    autoEl: {
			        tag: 'iframe',
			        id: 'InstructorWidgetNameTagsIframe',
			        style: 'height: 650px; width: 950px;',
			        src: '/admin/nametags/name_tags_classlist.asp?cid=' + cid + '&Caller=public&misc=564&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId
			    },
			    listeners: {
			        afterrender: function () {
			            this.getEl().on('load', function () {
			                var iframeHT = $("#InstructorWidgetNameTagsIframe").height()
			                var panel = Ext.getCmp('InstructorWidgetNameTagsPanel');
			                panel.setHeight(iframeHT)
			            });
			        }
			    }
			}
        ],
        //html: '<iframe width="950px" height="630px" src="/admin/nametags/name_tags_classlist.asp?cid='+cid+'&Caller=public&misc=564&rubyrequest=1&uname=' + username + '"></iframe>',

        renderTo: Ext.getBody(),
    });
    winreport.show();
    //self.LogoffClassic();
}

InstructorEmailListandReportWidget.prototype.Attendance = function (cmd, cid) {
    var self = this;
    winreport = Ext.create('Ext.form.Panel', {
        id: "InstructorWidgetAttendancePanel",
        floating: true,
        centered: true,
        modal: true,
        width: 970,
        height: 640,
        closable: true,
        defaultType: 'textfield',
        bodyPadding: 10,
        items: [
			{
			    xtype: 'component',
			    autoScroll: true,
			    autoEl: {
			        tag: 'iframe',
			        id: 'InstructorWidgetAttendanceIframe',
			        style: 'height: 650px; width: 950px;',
			        src: '/admin/attendance_detail_instructor.asp?cid=' + cid + '&Caller=public&misc=564&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId
			    },
			    listeners: {
			        afterrender: function () {
			            this.getEl().on('load', function () {
			                var iframeHT = $("#InstructorWidgetAttendanceIframe").height()
			                var panel = Ext.getCmp('InstructorWidgetAttendancePanel');
			                panel.setHeight(iframeHT)
			            });
			        }
			    }
			}
        ],
        //html: '<iframe id="InsWidgetEmailList" width="950px" height="630px" src="/admin/attendance_detail_instructor.asp?cid=' + cid + '&Caller=public&misc=564&rubyrequest=1&uname=' + username + '"></iframe>',
        renderTo: Ext.getBody(),
    });
    winreport.show();
    //self.LogoffClassic();
}

InstructorEmailListandReportWidget.prototype.Email = function (cmd, cid) {
    var self = this;
    winreport = Ext.create('Ext.form.Panel', {
        id: 'InstructorWidgetEmailPanel',
        floating: true,
        centered: true,
        modal: true,
        width: 970,
        height: 640,
        closable: true,
        defaultType: 'textfield',
        bodyPadding: 10,
        items: [
			{
			    xtype: 'component',
			    autoScroll: true,
			    autoEl: {
			        tag: 'iframe',
			        id: 'InstructorWidgetEmailIframe',
			        style: 'height: 650px; width: 950px;',
			        src: '/admin/email_course_email.asp?cid=' + cid + '&caller=public&misc=564&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId
			    },
			    listeners: {
			        afterrender: function () {
			            this.getEl().on('load', function () {
			                //For mobile, scrollbar and iframe height cannot be controlled. Extjs bug.
			                var iframeHT = $("#InstructorWidgetEmailIframe").height()
			                var panel = Ext.getCmp('InstructorWidgetEmailPanel');
			                panel.setHeight(iframeHT)
			            });
			        }
			    }
			}
        ],
        //html: '<iframe width="950" height="630" src="/admin/email_course_email.asp?cid=' + cid + '&caller=public&misc=564&rubyrequest=1&uname=' + username + '"></iframe>',

        renderTo: Ext.getBody(),
    });
    winreport.show();
    //self.LogoffClassic();
}
InstructorEmailListandReportWidget.prototype.CertificationReport = function (cmd,cid) {
    winreport = Ext.create('Ext.form.Panel', {
        id: 'InstructorWidgetCertificationReportPanel',
        floating: true,
        centered: true,
        modal: true,
        width: 970,
        height: 640,
        closable: true,
        defaultType: 'textfield',
        bodyPadding: 10,
        items: [
			{
			    xtype: 'component',
			    autoScroll: true,
			    autoEl: {
			        tag: 'iframe',
			        id: 'InstructorWidgetCertificationReportIframe',
			        style: 'height: 650px; width: 950px;',
			        src: '/admin/reports_certificate_print.asp?cid=' + cid + '&caller=public&misc=885&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId
			    },
			    listeners: {
			        afterrender: function () {
			            this.getEl().on('load', function () {
			                //For mobile, scrollbar and iframe height cannot be controlled. Extjs bug.
			                var iframeHT = $("#InstructorWidgetCertificationReportIframe").height()
			                var panel = Ext.getCmp('InstructorWidgetCertificationReportPanel');
			                panel.setHeight(iframeHT)
			            });
			        }
			    }
			}
        ],
        //html: '<iframe width="950" height="630" src="/admin/reports_certificate_print.asp?cid='+cid+'&caller=public&misc=885&rubyrequest=1&uname=' + username + '"></iframe>',

        renderTo: Ext.getBody(),
    });
    winreport.show();
}
InstructorEmailListandReportWidget.prototype.CourseRequest = function (cmd) {
    winreport = Ext.create('Ext.form.Panel', {
        id: 'InstructorWidgetCourseRequestPanel',
        floating: true,
        centered: true,
        modal: true,
        width: 970,
        height: 640,
        closable: true,
        defaultType: 'textfield',
        bodyPadding: 10,
        items: [
			{
			    xtype: 'component',
			    autoScroll: true,
			    autoEl: {
			        tag: 'iframe',
			        id: 'InstructorWidgetCourseRequestIframe',
			        style: 'height: 650px; width: 950px;',
			        src: '/admin/public_add_Course_Request.asp?source=dev_instructors.asp&misc=552&caller=public&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId
			    },
			    listeners: {
			        afterrender: function () {
			            this.getEl().on('load', function () {
			                //For mobile, scrollbar and iframe height cannot be controlled. Extjs bug.
			                var iframeHT = $("#InstructorWidgetCourseRequestIframe").height()
			                var panel = Ext.getCmp('InstructorWidgetCourseRequestPanel');
			                panel.setHeight(iframeHT)
			            });
			        }
			    }
			}
        ],
        //html: '<iframe width="950" height="630" src="/admin/public_add_Course_Request.asp?source=dev_instructors.asp&misc=552&caller=public&rubyrequest=1&uname=' + username + '"></iframe>',

        renderTo: Ext.getBody()
    });
    winreport.show();
}

InstructorEmailListandReportWidget.prototype.RoomManagement = function (cmd) {
    winreport = Ext.create('Ext.form.Panel', {
        id: 'InstructorWidgetRoomManagementPanel',
        floating: true,
        centered: true,
        modal: true,
        width: 970,
        height: 640,
        closable: true,
        defaultType: 'textfield',
        bodyPadding: 10,
        items: [
			{
			    xtype: 'component',
			    autoScroll: true,
			    autoEl: {
			        tag: 'iframe',
			        id: 'InstructorWidgetRoomManagementIframe',
			        style: 'height: 650px; width: 950px;',
                    src: '/admin/roommgmt/index.asp?source=dev_instructors.asp&misc=552&rubyrequest=1&usergroup=instructor&uname=' + username + '&usersessionid=' + instructorsessionId
			    },
			    listeners: {
			        afterrender: function () {
			            this.getEl().on('load', function () {
			                //For mobile, scrollbar and iframe height cannot be controlled. Extjs bug.
			                var iframeHT = $("#InstructorWidgetRoomManagementIframe").height()
			                var panel = Ext.getCmp('InstructorWidgetRoomManagementPanel');
			                panel.setHeight(iframeHT)
			            });
			        }
			    }
			}
        ],
        //html: '<iframe width="950" height="630" src="/admin/roommgmt/index.asp?source=dev_instructors.asp&misc=552&rubyrequest=1&uname=' + username + '"></iframe>',

        renderTo: Ext.getBody(),
    });
    winreport.show();
}
InstructorEmailListandReportWidget.prototype.StudentList = function (cmd) {
    var self = this;
    var store = Ext.create('Ext.data.Store', {
        autoLoad: true,
        autoSync: true,
        pageSize: 12,
        remoteFilter: true,
        remoteSort: true,
        remoteSort: true,
        model: 'EmailAuditTrail',
        proxy: {
            type: 'ajax',
            url: config.getUrl('public/Instructor/Students'),
            reader: {
                type: 'json',
                rootProperty: 'Result',
                totalProperty: 'TotalCount',
                listeners: {
                    exception: function (reader, response, error, opts) {
                        log(error);
                    }
                }
            }
        }
    });
    var searchField = Ext.create('Ext.form.field.Text', {
        region: 'north',
        emptyText: 'Search',
        width: 550
    });

    searchField.on('change', function (that, value, oldValue, options) {
        store.filter(
            [{ id: 'keyword', property: 'keyword', value: value }]
        );
    }, searchField, {
        buffer: 500
    });
    var searchfield_1 = Ext.create('Ext.Panel', {
        preventHeader: true,
        frame: false,
        shrinkWrap: false,
        margin: { top: 0, left: 0, right: 0, bottom: 10 },
        layout: {
            type: 'hbox',
            border: false
        },
        border: false,
        items: [searchField]
    });

    var grid = Ext.create('Ext.grid.Panel', {
        id: 'StudentListWindowsGrid',
        region: 'center',
        frame: true,
        width: 900,
        title: studentTerm+' List',
        dockedItems: [
            searchfield_1, {
                xtype: 'pagingtoolbar',
                store: store,
                dock: 'top',
                displayInfo: true
            },
        ],
        store: store,
        emptyText: 'No Students found',
        columns: [
            {
                text: 'First Name',
                dataIndex: 'StudentFirstName',
                width: 120,
                renderer: function (myValue, val, myRecord) {
                    var sid = myRecord.get('StudentId');
                    return '<a href="/Public/Supervisor/EditStudentInfo?sid=' + sid + '" target="_blank">' + myValue + '</a>';
                }
            },
            {
                text: 'Last Name',
                dataIndex: 'StudentLastName',
                flex: 1,
                renderer: function (myValue, val, myRecord) {
                    var sid = myRecord.get('StudentId');
                    return '<a href="/Public/Supervisor/EditStudentInfo?sid=' + sid + '" target="_blank">' + myValue + '</a>';
                }
            },
        ]
    });


    winreport = Ext.create('Ext.form.Panel', {
        floating: true,
        centered: true,
        modal: true,
        width: 400,
        height: 400,
        closable: true,
        defaultType: 'textfield',
        bodyPadding: 10,
        items: [grid],

        renderTo: Ext.getBody(),
    });
    winreport.show();
}

InstructorEmailListandReportWidget.prototype.LogoffClassic = function (cmd) {
    off = Ext.create('Ext.form.Panel', {
        floating: true,
        centered: true,
        modal: true,
        width: 970,
        height: 640,
        closable: true,
        defaultType: 'textfield',
        bodyPadding: 10,
        html: '<iframe width="950" height="630" src="/admin/logoff.asp?type=instructor&misc=253"></iframe>',

        renderTo: Ext.getBody(),
    });
    off.show();
    off.hide();
}

InstructorEmailListandReportWidget.prototype.SurveyViewComments= function (sid,cid) {
    var window = Ext.create('Ext.window.Window', {
        title: 'View Comments',
        layout: {
            type: 'fit',
            manageOverflow: 2
        },
        modal: true,
        width: 800,
        height: 1000,
        items: [
            {
            }]
    });

    window.show();
}
InstructorEmailListandReportWidget.prototype.RenderImplementation = function () {
    var self = this;
    var store = Ext.create('Ext.data.Store', {
        autoLoad: true,
        autoSync: true,
        pageSize: 5,
        remoteFilter: true,
        remoteSort: true,
        remoteSort: true,
        model: 'EmailAuditTrail',
        proxy: {
            type: 'ajax',
            url: config.getUrl('public/user/email'),
            reader: {
                type: 'json',
                rootProperty: 'Result',
                totalProperty: 'TotalCount',
                listeners: {
                    exception: function (reader, response, error, opts) {
                        log(error);
                    }
                }
            }
        }
    });
    var survey_result_store = Ext.create('Ext.data.Store', {
        autoLoad: true,
        autoSync: true,
        pageSize: 5,
        remoteFilter: true,
        remoteSort: true,
        remoteSort: true,
        model: 'EmailAuditTrail',
        proxy: {
            type: 'ajax',
            url: config.getUrl('public/instructor/GetSurveyResultDetails'),
            reader: {
                type: 'json',
                rootProperty: 'Result',
                totalProperty: 'TotalCount',
                listeners: {
                    exception: function (reader, response, error, opts) {
                        log(error);
                    }
                }
            }
        }
    });
    var searchField = Ext.create('Ext.form.field.Text', {
        region: 'north',
        emptyText: 'Search'
    });
    var emailIcon = config.getUrl('images/icons/famfamfam/email_open.png');
    searchField.on('change', function (that, value, oldValue, options) {
        store.filter(
            [{ id: 'keyword', property: 'keyword', value: value }]
        );
    }, searchField, {
        buffer: 500
    });
    var grid_survey = Ext.create('Ext.grid.Panel', {
        region: 'center',
        flex: 3,
        margin: 3,
        columnWidth: 10,
        frame: true,
        title: 'Surveys',
        dockedItems: [
            {
                xtype: 'pagingtoolbar',
                dock: 'top',
                displayInfo: true
            }
        ],
        emptyText: 'No Surveys Available',
        columns: [
            {
                text: 'Survey',
                dataIndex: 'AuditDate',
                renderer: Ext.util.Format.dateRenderer('m/d/Y h:i A'),
                width: 120,
            }, ]
    });

    var grid_survey_result = Ext.create('Ext.grid.Panel', {
        region: 'center',
        flex: 3,
        margin: 3,
        store: survey_result_store,
        columnWidth: 10,
        frame: true,
        dockedItems: [
    {
        xtype: 'pagingtoolbar',
        store: survey_result_store,
        dock: 'top',
        displayInfo: true
    }
        ],
        columns: [
            {
                text: 'Survey',
                dataIndex: 'SurveyName',
                width: 250,
            },
             {
                 text: 'Survey ID',
                 dataIndex: 'SurveyId',
                width: 80,
             },
             {
                 text: 'Actions',
                 width: 140,
                 renderer: function (value, metadata, record, rowindex, colindex, store, view) {
                     var surveydetails = record.get('SurveyId').split("-");
                     linkvalue = "<a href='/public/survey/showsurveycomments?sid=" + surveydetails[0] + "&cid=" + surveydetails[1] + "' target='_blank'>View Comments</a><br><a href='/public/survey/showsurveyresults?sid=" + surveydetails[0] + "&cid=" + surveydetails[1] + "' target='_blank'>View Results</a>";
                     return linkvalue;
                 }
             },
        ]
    });

    var tabs_survey = Ext.create('Ext.tab.Panel', {
        fieldDefaults: {
            labelAlign: 'right',
            labelWidth: 200,
            labelClsExtra: 'widget-field',
        },
        margins: {
            left: '0px',
            right: '0px',
            top: '0px',
            bottom: '0px'
        },
        items: [{
            title: 'Surveys',
            itemId: 'current',
            items: [grid_survey],

        }, {
            title: 'View Result',
            itemId: 'past',
            items: [grid_survey_result],
        }],
		listeners: {
			'tabchange': function(tabPanel, tab) {
				survey_result_store.load()
		}
    }});
    var grid = Ext.create('Ext.grid.Panel', {
        region: 'center',
        flex: 3,
        frame: true,
        columnWidth: 10,
        title: 'Received Email',
        margin:3,
        dockedItems: [
            {
                xtype: 'pagingtoolbar',
                store: store,
                dock: 'top',
                displayInfo: true
            }
        ],
        store: store,
        emptyText: 'No Students found',
        columns: [
            {
                text: 'Date',
                dataIndex: 'AuditDate',
                renderer: Ext.util.Format.dateRenderer('m/d/Y h:i A'),
                width: 120,
            },
            {
                text: 'Subject',
                dataIndex: 'EmailSubject',
                flex: 1
            },

            {
                xtype: 'actioncolumn',
                width: 22,
                items: [ {icon: emailIcon,
                    tooltip: 'View e-mail',
                    handler: function (view, rowIndex, colIndex, item, e, record) {

                        var data = record.data;

                        var attachments = 'None';
                        if (data.AttachmentNameMemo != null && data.AttachmentNameMemo != '') {
                            var list = data.AttachmentNameMemo.split('|');
                            var count = 1;
                            if (list.length > 0) {
                                attachments = '';
                                for (var index = 0; index < list.length; index++) {
                                    if (list[index] != '') {
                                        attachments += '<a target="_blank" href="'+insadminPath+'datastores/datastore-user.asp?action=get-file&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId+'&param=' + encodeURIComponent(list[index]) + '">Attachment ' + count + '</a><br/>';
                                        count++;
                                    }
                                }
                            }
                        }

                        var window = Ext.create('Ext.window.Window', {
                            icon: emailIcon,
                            title: 'E-mail details',
                            layout: {
                                type: 'fit',
                                manageOverflow: 2
                            },
                            modal: true,
                            width: 800,
                            height: 400,
                            items: [
                                {
                                    xtype: 'form',
                                    autoScroll: true,
                                    items: [
                                        {
                                            xtype: 'displayfield',
                                            fieldLabel: 'Audit date',
                                            value: Ext.Date.format(data.AuditDate, 'm/d/Y h:i:s A')
                                        },
                                        {
                                            xtype: 'displayfield',
                                            fieldLabel: 'Subject',
                                            value: data.EmailSubject
                                        },
                                        {
                                            xtype: 'displayfield',
                                            fieldLabel: 'From',
                                            value: data.EmailFrom
                                        },
                                        {
                                            xtype: 'displayfield',
                                            fieldLabel: 'To',
                                            value: data.EmailTo
                                        },
                                        {
                                            xtype: 'displayfield',
                                            fieldLabel: 'Pending',
                                            value: data.Pending == '0' ? 'No' : 'Yes'
                                        },
                                        {
                                            xtype: 'displayfield',
                                            fieldLabel: 'Attachments',
                                            value: attachments
                                        },
                                        {
                                            xtype: 'displayfield',
                                            fieldLabel: 'Body',
                                            value: data.EmailBody
                                        }
                                    ]
                                }
                            ]
                        });
                        window.show();
                    }

                }
                ]
            }
        ]
        });
    var optionalhtml = "";
    var certificationlink = "";
    if (certificationsettings == "0") {
        certificationlink = ""
    }
    else {
       // certificationlink = '<tr><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'certificationreport\')"><img src="Areas/Public/Images/Icons/course-center-actions-certs.png" style="width:50px; height:50px;" /></div></td><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'certificationreport\')">Certification Report</div></td></tr>'

    }
    if (allowstudentaddedit == "1") {
    
        optionalhtml = '<tr><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'addstudent\')"><img src="Areas/Public/Images/Icons/course-center-actions-addstudent_thumb.png" style="width:32px; height:32px;"  /></div></td><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'addstudent\')">Add'+studentTerm+'</div></td></tr>';
    }
    var enrolladdeditstudent = "";
    if (allowstudentenroll == "1") {
        enrolladdeditstudent = '<tr><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'editstudent\')"><img src="Areas/Public/Images/Icons/course-center-actions-duplicate.png" style="width:32px; height:32px;" /></div></td><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'editstudent\')">Edit'+studentTerm+'</div></td></tr>'
    }
    if (allowstudentaddedit=="1") {
        enrolladdeditstudent = enrolladdeditstudent + '<tr><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'enroll\')"><img src="Areas/Public/Images/Icons/course-center-actions-supervisor_thumb.png" style="width:32px; height:32px;;" /></div></td><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'enroll\')">Enroll</div></td></tr>'

    }

    var reportpanelHeight = 230

    var roommgthtml = '<tr><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'roommgt\')"><img src="Areas/Public/Images/Icons/roommanage32x32.png" style="width:32px; height:32px;" /></div></td><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'roommgt\')">Room Management</div></td></tr>'
    if (HideManagerRoomInPublic == "1") {
        roommgthtml = ""
        reportpanelHeight -= 34
    }

    var subadminhtml = '<tr><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'subadmin\')"><img src="Areas/Public/Images/Icons/roommanage32x32.png" style="width:32px; height:32px;" /></div></td><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'subadmin\')">Sub Admin Portal</div></td></tr>'
    if (HasSubAdminAccount == "0") {
        subadminhtml = ""
        reportpanelHeight -= 34
    }

    var actioncourserequests = "";
    if (allowcourserequests == "1") {
        actioncourserequests = '<tr><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'coursereq\')"><img src="Areas/Public/Images/Icons/courserequest32x32.png" style="width:32px; height:32px;" /></div></td><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'coursereq\')">'+Terminology.capital('course')+' Request</div></td></tr>'
    } else {
        reportpanelHeight -= 34
    }
    actionrostereport = '<tr><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'rosterreport\')"><img src="Areas/Public/Images/Icons/course-center-actions-duplicate.png" style="width:32px; height:32px;" /></div></td><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'rosterreport\')">Roster Report</div></td></tr>'
    actionattendancereport = '<tr><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'attendancereport\')"><img src="Areas/Public/Images/Icons/course-center-take-attendance.png" style="width:32px; height:32px;" /></div></td><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'attendancereport\')">Attendance Report</div></td></tr>'
    reportpanelHeight += 34
    var reportpanel = Ext.create('Ext.Panel', {
        title:'Actions',
        width: '10%',
        height: reportpanelHeight,
        frame: true,
        flex: 1,
        layout: {
            type: 'vbox',
            pack: 'center'
        },
        margin: { top: 0, left: 10, right: 0, bottom: 0 },
        items: [{
            xtype: 'panel',
            frame: false,
            width:'100%',
            html: ['<table>' + optionalhtml + enrolladdeditstudent,
              //  '<tr><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'transcriptreport\')"><img src="Areas/Public/Images/Icons/course-center-actions-transcript_thumb.png" style="width:50px; height:50px;" /></div></td><td><div style="cursor:pointer;" onclick="InstructorEmailListandReportWidget.prototype.ReportAction(\'transcriptreport\')"> Email</div></td></tr>',
                
                roommgthtml,
                subadminhtml,
                actioncourserequests,
                certificationlink,
                actionrostereport,
                actionattendancereport,
                '</table>'
            ].join(""),
            flex: 1
        }

        ]
    });
    var subpanel1 = Ext.create('Ext.Panel', {
        border: false,
        frame: false,
        preventHeader: true,
        width:'75%',
        frame: false,
        items: [grid, tabs_survey]
    });
   var mainpanel =  Ext.create('Ext.Panel', {
       preventHeader: true,
       frame: false,
        layout: {
            type: 'hbox',
            border: false,
        },
        border: false,
        items: [subpanel1,reportpanel]
    });


   self.State.widget = Ext.create('Ext.form.Panel', self.getWidgetDefaults({
       fieldDefaults: {
           labelAlign: 'right',
           labelWidth: 200,
           labelClsExtra: 'widget-field',
           width: 1000,
            
       },

       title: 'sss',
       preventHeader: true,
       border: false,
        items: [mainpanel]
    }));
};
