function SupervisorStudentListWidget(dashboard) {
    var self = this;

    self.State.dashboard = dashboard;
}

SupervisorStudentListWidget.prototype = new WidgetBase();

SupervisorStudentListWidget.constructor = SupervisorStudentListWidget;

SupervisorStudentListWidget.prototype.Options = {
};

SupervisorStudentListWidget.prototype.State = {
    container: null,
    dashboard: null,
    widget: null
};
SupervisorStudentListWidget.prototype.executeCourseAction = function (cmd, record) {
    var self = this;
    if (cmd == '') {
        return;
    }


    switch (cmd) {
        case 'enroll':
            self.EnrollStudent(record);
            break;
        case 'emailstudent':
            self.EmailStudent(record);
            break;
        case 'transcript':
            self.StudentTranscript(record);
            break;
        case 'editstudent':

            window.open('/Public/Supervisor/EditStudentInfo?sid=' + record);
            break;


    }

}
SupervisorStudentListWidget.prototype.StudentTranscript = function (sid) {
    Ext.Ajax.request({
        url: config.getUrl('public/Supervisor/GetStudentRecord'),
        params: {
            studentId: sid
        },
        success: function (response) {
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
                    url: config.getUrl('public/Supervisor/GetStudentTranscript?studentId=' + sid),
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


            var data = Ext.decode(response.responseText);
            var grid = Ext.create('Ext.grid.Panel', {
                region: 'center',
                flex: 3,
                width: '100%',
                frame: false,
                store: store,
                emptyText: 'No courses have been Transcripted for this student.',
                columns: [
                    {
                        text: 'Course Number',
                        dataIndex: 'CourseNumber',
                        width: 120,
                    },
                    {
                        text: 'Course Name',
                        dataIndex: 'CourseName',
                        flex: 1
                    },
                    {
                        text: 'Grade',
                        dataIndex: 'Grade',
                        flex: 1,
                        width: 120,
                    },
                            {
                                text: 'Start Date',
                                dataIndex: 'StartDate_string',
                                width: 120,
                            },
                            {
                                text: 'Completion Date',
                                dataIndex: 'CompletionDate_string',
                                width: 120,
                            }]
            });

            var window_open = Ext.create('Ext.window.Window', {
                title: 'Transcript for ' + data.StudentFirstName + " " + data.StudentLastName,
                layout: {
                    manageOverflow: 2
                },
                modal: true,
                width: 800,
                margin: { top: 0, left: 12, right: 12, bottom: 0 },
                items: [
                        {
                            xtype: 'button',
                            text: '<div style="margin-bottom:2px;"><img style="float:left;" src="/Images/Icons/FamFamFam/printer.png" />&nbsp;&nbsp;Print</div>',
                            tooltip: 'Print',
                            width: 80,
                            margin: { top: 5, left: 700, right: 12, bottom: 5 },
                            handler: function (e, toolEl, panel, tc) {
                                window.open(classicurl + sid +'&rubyrequest=1&usersessionid='+supervisorsessionId);
                            }
                        }, grid]
            });
            window_open.show();
        }
    });



}
SupervisorStudentListWidget.prototype.EmailStudent = function (sid) {
    Ext.Ajax.request({
        url: config.getUrl('public/Supervisor/GetStudentRecord'),
        params: {
            studentId: sid
        },
        success: function (response) {
            var data = Ext.decode(response.responseText);
            var textTo = Ext.create('Ext.form.field.Text', {
                xtype: 'textfield',
                name: 'ETo',
                id: 'ETo',
                labelWidth: 62,
                width: 750,
                Height: 50,
                fieldLabel: 'To',
                margin: { top: 10, left: 12, right: 12, bottom: 10 },
                value: data.Email,
            });

            var textSubject = Ext.create('Ext.form.field.Text', {
                xtype: 'textfield',
                name: 'ESubject',
                id: 'ESubject',
                labelWidth: 62,
                width: 750,
                Height: 50,
                fieldLabel: 'Subject',
                margin: { top: 10, left: 12, right: 12, bottom: 10 },
                value: '',
            });

            var textMessage = Ext.create('Gsmu.HtmlEditor', {
                name: 'textMessage',
                id: 'textMessage',
                labelAlign: 'left',
                height: 250,
                width: 750,
                fieldLabel: '',
                value: '',
                margin: { top: 10, left: 12, right: 12, bottom: 10 }

            });

            var window = Ext.create('Ext.window.Window', {
                title: 'E-mail for ' + data.StudentFirstName + " " + data.StudentLastName,
                layout: {
                    manageOverflow: 2
                },
                modal: true,
                width: 800,
                height: 400,
                margin: { top: 0, left: 12, right: 12, bottom: 0 },
                items: [textTo, textSubject, textMessage,
                            {
                                xtype: 'button',
                                text: 'SEND',
                                width: 100,
                                height: 20,
                                margin: { top: 0, left: 660, right: 12, bottom: 0 },
                                align: 'right',
                                listeners: {
                                    click: function () {


                                        Ext.Ajax.request({
                                            url: config.getUrl('public/Supervisor/SendStudentEmail'),
                                            params: {
                                                Subject: Ext.getCmp('ESubject').value,
                                                To: Ext.getCmp('ETo').value,
                                                Message: HtmlEncode(Ext.getCmp('textMessage').value)
                                            },
                                            success: function (response) {
                                                Ext.Msg.alert('Result', response.responseText.replace(/"/g, ''));
                                            }
                                        });
                                    }
                                }
                            }

                ]
            });
            window.show();
        }
    });
}

function HtmlEncode(s) {
    var el = document.createElement("div");
    el.innerText = el.textContent = s;
    s = el.innerHTML;
    return s;
}

SupervisorStudentListWidget.prototype.EnrollStudent = function (sid) {
    Ext.Ajax.request({
        url: config.getUrl('public/Supervisor/SetPrincipalStudentonCart'),
        params: {
            studentId: sid
        },
        success: function (response) {
            result = response.responseText;
            if (result == "success") {
                window.location = "/public/course/browse"
            }
            else {
                if (result.indexOf("notallowed") > -1) {
                    alert("You currently have an active enrollment for " + result.replace("notallowed:", "") + ". You can only have one active registration at a time. Please use multiple enrollment on the courses to choose different users. ");
                    window.location = "/public/course/browse"
                }
                else if (result.indexOf("missingfield") > -1) {
                    alert("Selected student has a required field missing. ")
                    window.location = "/Public/Supervisor/EditStudentInfo?sid=" + result.replace("missingfield", "")
                }
            }
        }
    });
}
SupervisorStudentListWidget.prototype.RenderImplementation = function () {
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
            url: config.getUrl('public/Supervisor/Students'),
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

    var button = Ext.create('Ext.Button', {
        text: '<div style="margin-bottom:0px;"><img style="float:left;" src="/Images/Icons/FamFamFam/page_excel.png" />&nbsp;&nbsp;Export to Excel</div>',
        handler: function () {

            window.location = "/Temp/" + "StudentList-" + username + ".csv";

            //Ext.Ajax.request({
            //    url: config.getUrl('public/Supervisor/ExporttoCSVfile'),
            //    success: function (response) {
            //        var filename = response.responseText;
            //        window.location = "/Temp/" + filename;
            //    }
            //});
        }
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
        items: [searchField, button]
    });


    var ActionsMenuList = [];
    if (AllowEnrollment != "1") {
        ActionsMenuList.push({ vlu: 'enroll', labl: 'Enroll' });
    }
    ActionsMenuList.push({ vlu: 'editstudent', labl: 'Edit' });
    ActionsMenuList.push({ vlu: 'emailstudent', labl: 'Email this Student' });
    ActionsMenuList.push({ vlu: 'transcript', labl: 'Transcript' });

    var ActionsMenuStore = Ext.create('Ext.data.Store', {
        fields: ['vlu', 'labl'],
        data: ActionsMenuList
    });

    var combotpl = new Ext.XTemplate(
        '<div style="width:90px; border: 1px solid grey; height: 20px; padding: 0px;"><div style="color:grey;float: left;width: 55px; margin-top:2px;">&nbsp;&nbsp;select...</div><div style="float: left; text-align:right; width: 30px; margin-top:2px;">&#9660;&nbsp;</div></div>'
    );

    var grid = Ext.create('Ext.grid.Panel', {
        id: 'StudentListWidgetGrid',
        region: 'center',
        frame: true,
        width:900,
        title: 'Student List',
        dockedItems: [
            searchfield_1,
            {
                xtype: 'pagingtoolbar',
                store: store,
                dock: 'top',
                displayInfo: true,
                inputItemWidth: 35,
                items: [
                    '-',
                    {
                        xtype: 'checkbox',
                        fieldLabel: 'Hide Inactive',
                        labelWidth: 120,
                        labelStyle: 'text-align:left!important;',
                        listeners:
                        {
                            change: function (field, newValue, oldValue, eOpts)
                            {
                                var mask = new Ext.LoadMask(Ext.getCmp('StudentListWidgetGrid'), { msg: "Please wait..." });
                                mask.show();
                                store.clearFilter();
                                if (newValue)
                                {
                                    store.filter('InActive', 1);
                                }
                                else
                                {
                                    store.filter('InActive', 0);
                                }
                                store.on('load', function () {
                                    mask.hide();
                                }, this, { single: true })

                            }
                        }
                    }]
            },
        ],
        store: store,
        emptyText: 'No Students found',
        columns: [
            {
                            text: '',
                            dataIndex: 'hasbalance',
                            width: 40,
                            renderer: function (myValue, val, myRecord) {
                                var sid = myRecord.get('StudentId');
                                var hasbalance = myRecord.get('HasBalance');
                                var balindicator = '';
                                var cls = '';
                                var inActive = myRecord.get('InActive')
                                if (inActive === 1) {
                                    cls = 'inactive';
                                    val.tdCls = cls;
                                }
                                if (hasbalance > 0) {
                                    balindicator = '<img src="/Images/Icons/FamFamFam/basket_delete.png" alt="with balance">'
                                }
                                return '<a href="/Public/Supervisor/EditStudentInfo?sid=' + sid + '" target="_blank">' + balindicator +'</a>'
                            }
             },
            {
                text: 'First Name',
                dataIndex: 'StudentFirstName',
                width: 120,
                renderer: function (myValue, val, myRecord) {
                    var sid = myRecord.get('StudentId');
                    var cls = '';
                    var result = ' <a href="/Public/Supervisor/EditStudentInfo?sid=' + sid + '" target="_blank">' + myValue + '</a>';
                    var inActive = myRecord.get('InActive')
                    if (inActive === 1) {
                        cls = 'inactive';
                        val.tdCls = cls;
                        result = myValue;
                    }

                    return result;
                }
            },
            {
                text: 'Last Name',
                dataIndex: 'StudentLastName',
                flex: 1,
                renderer: function (myValue, val, myRecord, obj) {
                    var sid = myRecord.get('StudentId');
                    var cls = '';
                    var result = '<a href="/Public/Supervisor/EditStudentInfo?sid=' + sid + '" target="_blank">' + myValue + '</a>';
                    var inActive = myRecord.get('InActive')
                    if (inActive === 1) {
                        cls = 'inactive';
                        val.tdCls = cls;
                        result = myValue;
                        val.style = "color:lightgray";
                    }
                    return result;
                }
            },
            {
                text: 'Email',
                dataIndex: 'Email',
                flex: 1,
                renderer: function (myValue, val, myRecord) {
                    var cls = '';
                    var inActive = myRecord.get('InActive')
                    if (inActive === 1) {
                        cls = 'inactive';
                        val.tdCls = cls;
                        val.style = "color:lightgray";
                    }
                    return myValue;
                }
            },
            {
                text: 'UserName',
                dataIndex: 'UserName',
                flex: 1,
                renderer: function (myValue, val, myRecord) {
                    var cls = '';
                    var inActive = myRecord.get('InActive')
                    if (inActive === 1) {
                        cls = 'inactive';
                        val.tdCls = cls;
                        val.style = "color:lightgray";
                    }
                    return myValue;
                }
            },
            {
                text: 'Enrolled',
                dataIndex: 'Enrolled',
                flex: 1,
                renderer: function (myValue, val, myRecord) {
                    var cls = '';
                    var inActive = myRecord.get('InActive')
                    if (inActive === 1) {
                        cls = 'inactive';
                        val.tdCls = cls;
                        val.style = "color:lightgray";
                    }
                    return myValue;
                }

            },
            {
                text: 'Complete',
                dataIndex: 'Complete',
                flex: 1,
                renderer: function (myValue, val, myRecord) {
                    var cls = '';
                    var inActive = myRecord.get('InActive')
                    if (inActive === 1) {
                        cls = 'inactive';
                        val.tdCls = cls;
                        val.style = "color:lightgray";
                    }
                    return myValue;
                }
            },
            {
                text: 'Inactive',
                dataIndex: 'InActive',
                flex: 1,
                renderer: function (myValue, val, myRecord) {
                    var status = 'No';
                    var inActive = myRecord.get('InActive')
                    if (inActive === 1) {
                        status = 'Yes';
                        cls = 'inactive';
                        val.tdCls = cls;
                        val.style = "color:lightgray";
                    }
                    return status;
                }
            },
            {
                text: 'Waiting',
                dataIndex: 'Waiting',
                flex: 1,
                renderer: function (myValue, val, myRecord) {
                    var cls = '';
                    var inActive = myRecord.get('InActive')
                    if (inActive === 1) {
                        cls = 'inactive';
                        val.tdCls = cls;
                        val.style = "color:lightgray";
                    }
                    return myValue;
                }
            },
            // another weird bug in IE. not working in win10 ie11
            //{
            //    width: 122,
            //    text: 'Actions',
            //    tooltip: 'Actions',
            //    renderer: function (value, metadata, record, rowindex, colindex, store, view) {
            //        var sid = record.get('StudentId');

            //        var actionTemplate = '<select style="width:100px;" onchange="var value = this.options[this.selectedIndex].value; SupervisorStudentListWidget.prototype.executeCourseAction(value,' + sid + '); this.selectedIndex = 0;">';
            //        actionTemplate += '<option value="">Select...</option>';
            //        if (SupervisorDashboard.prototype.State.supervisorData.AdvanceOptions!="0") {
            //            actionTemplate += '<option value="enroll">Enroll</option>';
            //            actionTemplate += '<option value="editstudent">Edit</option>';
            //        }
            //        actionTemplate += '<option value="emailstudent">Email this Student</option>';
            //        actionTemplate += '<option value="transcript">Transcript</option>';
            //        actionTemplate += '</select>';
            //        return actionTemplate;

            //    }
            //}

                {
                    text: 'Actions',
                    xtype: 'templatecolumn',
                    width: 100,
                    tpl: combotpl,
                    editor:
                        {
                            xtype: 'combobox',
                            id: 'supstudmenucombo',
                            forceSelection: true,
                            editable: false,
                            store: ActionsMenuStore,
                            emptyText: 'select...',
                            displayField: 'labl',
                            valueField: 'vlu',
                            listeners: {
                                'focus': function (fld) { this.onTriggerClick(); console.log('dfgdfgdf')},
                                'change': function (field, selectedValue) {

                                    var ucwg = Ext.getCmp('StudentListWidgetGrid');
                                    var selection = ucwg.getSelectionModel().getSelection()[0];
                                    var sid = selection.get('StudentId');

                                    self.executeCourseAction(this.value,sid);
                                }
                            }
                        }
                }
        ],

        plugins: [
            Ext.create('Ext.grid.plugin.CellEditing', {
                clicksToEdit: 1
            })
        ],
        listeners: {
            itemclick: function (dv, record, item, index, e) {
                var cmbostore = Ext.getCmp('supstudmenucombo').getStore();
                cmbostore.clearFilter(true);

                cmbostore.filter(function (r) {
                    var value = r.get('vlu');

                    var retvlu = true;
                    if (SupervisorDashboard.prototype.State.supervisorData.AdvanceOptions == "0") {
                        if (value == 'enroll' || value == 'editstudent') {
                            retvlu = false;
                        }
                    }
                    return retvlu;
                });
                cmbostore.reload();
            }
        }

    });
    self.State.widget = Ext.create('Ext.form.Panel', self.getWidgetDefaults({
        fieldDefaults: {
            labelAlign: 'right',
            labelWidth: 200,
            labelClsExtra: 'widget-field',
            width: 2100
        },
        margins: {
            left: '0px',
            right: '0px',
            top: '0px',
            bottom: '0px'
        },
        preventHeader: true,
        border: false,
        title: 'Enroll Student',
        items: [grid]
    }));
};
