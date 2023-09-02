function InstructorCourseListWidget(dashboard) {
    var self = this;

    self.State.dashboard = dashboard;
}

InstructorCourseListWidget.prototype = new WidgetBase();

InstructorCourseListWidget.constructor = InstructorCourseListWidget;

InstructorCourseListWidget.prototype.Options = {
};

InstructorCourseListWidget.prototype.State = {
    container: null,
    dashboard: null,
    widget: null
};
InstructorCourseListWidget.prototype.executeCourseAction = function (cmd, record,coursename) {
    var self = this;
    if (cmd == '') {
        return;
    }


    switch (cmd) {
        case 'signinsheet':
            InstructorEmailListandReportWidget.prototype.SigninSheet(cmd, record);
            break;
        case 'signinsheetnew':
            InstructorEmailListandReportWidget.prototype.SigninSheet_New(cmd, record);
            break;
        case 'mailcert':
            InstructorEmailListandReportWidget.prototype.CertificationReport(cmd, record);
            break;
        case 'nametag':
            InstructorEmailListandReportWidget.prototype.NameTags(cmd, record);
            break;
        case 'attendance':
            InstructorEmailListandReportWidget.prototype.Attendance(cmd, record);
            break;
        case 'email':
            InstructorEmailListandReportWidget.prototype.Email(cmd, record);
            break;
        case 'editstudent':

            window.open('/Public/Supervisor/EditStudentInfo?sid=' + record);
            break;
        case 'survey':
            InstructorEmailListandReportWidget.prototype.SendSurvey(cmd, record);
            break;
        case 'rosterreport':
            Ext.Ajax.request({
                url: "admin/datastores/datastore-reports_BO.asp?action=getVariables&rndm=" + new Date().getTime() + '&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
                success: function (response) {
                    var dtaArray = eval("[" + response.responseText + "]");
                    var GobalVar = dtaArray[0]
                    InstructorEmailListandReportWidget.prototype.RosterReport(cmd, record,coursename, GobalVar);
                }
            });
            
            break;


    }

}
InstructorCourseListWidget.prototype.StudentTranscript = function (sid) {
    Ext.Ajax.request({
        url: config.getUrl('public/Instructor/GetCourses'),
        params: {
            studentId: sid
        },
        success: function (response) {
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
                        text:Terminology.upper('course')+ ' Number',
                        dataIndex: 'CourseNumber',
                        width: 120,
                    },
                    {
                        text: + Terminology.capital('course')+' Name',
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
                                dataIndex: 'CDate',
                                width: 120,
                            }]
            });
        }
    });



}
InstructorCourseListWidget.prototype.EmailStudent = function (sid) {
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

InstructorCourseListWidget.prototype.EnrollStudent = function (sid) {
    Ext.Ajax.request({
        url: config.getUrl('public/Supervisor/SetPrincipalStudentonCart'),
        params: {
            studentId: sid
        },
        success: function (response) {
            window.location = "/"
        }
    });
}
InstructorCourseListWidget.prototype.RenderImplementation = function () {
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
            url: config.getUrl('public/Instructor/GetCourses'),
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
    var store_past = Ext.create('Ext.data.Store', {
        autoLoad: true,
        autoSync: true,
        pageSize: 12,
        remoteFilter: true,
        remoteSort: true,
        remoteSort: true,
        model: 'EmailAuditTrail',
        proxy: {
            type: 'ajax',
            url: config.getUrl('public/Instructor/GetPastCourses'),
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
    var store_cancelled = Ext.create('Ext.data.Store', {
        autoLoad: true,
        autoSync: true,
        pageSize: 12,
        remoteFilter: true,
        remoteSort: true,
        remoteSort: true,
        model: 'EmailAuditTrail',
        proxy: {
            type: 'ajax',
            url: config.getUrl('public/Instructor/GetCancelledCourses'),
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
    var store_needattendance = Ext.create('Ext.data.Store', {
        autoLoad: true,
        autoSync: true,
        pageSize: 12,
        remoteFilter: true,
        remoteSort: true,
        remoteSort: true,
        model: 'EmailAuditTrail',
        proxy: {
            type: 'ajax',
            url: config.getUrl('public/Instructor/GetNeedAttendanceCourses'),
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
        width: 530
    });

    searchField.on('change', function (that, value, oldValue, options) {
        store.filter(
            [{ id: 'keyword', property: 'keyword', value: value }]
        );
    }, searchField, {
        buffer: 500
    });

    var searchField_past = Ext.create('Ext.form.field.Text', {
        region: 'north',
        emptyText: 'Search',
        width: 530
    });

    searchField_past.on('change', function (that, value, oldValue, options) {
        store_past.filter(
            [{ id: 'keyword', property: 'keyword', value: value }]
        );
    }, searchField_past, {
        buffer: 500
    });
    var searchField_cancelled= Ext.create('Ext.form.field.Text', {
        region: 'north',
        emptyText: 'Search',
        width: 530
    });

    searchField_cancelled.on('change', function (that, value, oldValue, options) {
        store_past.filter(
            [{ id: 'keyword', property: 'keyword', value: value }]
        );
    }, searchField_cancelled, {
        buffer: 500
    });
    var searchField_needattendance = Ext.create('Ext.form.field.Text', {
        region: 'north',
        emptyText: 'Search',
        width: 530
    });

    searchField_needattendance.on('change', function (that, value, oldValue, options) {
        store_needattendance.filter(
            [{ id: 'keyword', property: 'keyword', value: value }]
        );
    }, searchField_needattendance, {
        buffer: 500
    });
    var button = Ext.create('Ext.Button', {
        text: '<div style="margin-bottom:0px;"><img style="float:left;" src="/Images/Icons/FamFamFam/page_excel.png" />&nbsp;&nbsp;Export to Excel</div>',
        handler: function () {
            window.location = "/Temp/" + "CourseList" + username.replace('/','') + ".csv";

        }
    });

    var button_past = Ext.create('Ext.Button', {
        text: '<div style="margin-bottom:0px;"><img style="float:left;" src="/Images/Icons/FamFamFam/page_excel.png" />&nbsp;&nbsp;Export to Excel</div>',
        handler: function () {
            window.location = "/Temp/" + "PastCourseList" + username.replace('/', '') + ".csv";

        }
    });
    var button_cancelled = Ext.create('Ext.Button', {
        text: '<div style="margin-bottom:0px;"><img style="float:left;" src="/Images/Icons/FamFamFam/page_excel.png" />&nbsp;&nbsp;Export to Excel</div>',
        handler: function () {
            window.location = "/Temp/" + "CancelCourseList" + username.replace('/', '') + ".csv";

        }
    });
    var button_needattendance = Ext.create('Ext.Button', {
        text: '<div style="margin-bottom:0px;"><img style="float:left;" src="/Images/Icons/FamFamFam/page_excel.png" />&nbsp;&nbsp;Export to Excel</div>',
        handler: function () {
            window.location = "/Temp/" + "need_attendance_CourseList" + username.replace('/', '') + ".csv";

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

    var searchfield_2 = Ext.create('Ext.Panel', {
        preventHeader: true,
        frame: false,
        shrinkWrap: false,
        margin: { top: 0, left: 0, right: 0, bottom: 10 },
        layout: {
            type: 'hbox',
            border: false
        },
        border: false,
        items: [searchField_past, button_past]
    });
    var searchfield_4 = Ext.create('Ext.Panel', {
        preventHeader: true,
        frame: false,
        shrinkWrap: false,
        margin: { top: 0, left: 0, right: 0, bottom: 10 },
        layout: {
            type: 'hbox',
            border: false
        },
        border: false,
        items: [searchField_cancelled, button_cancelled]
    });
    var searchfield_3 = Ext.create('Ext.Panel', {
        preventHeader: true,
        frame: false,
        shrinkWrap: false,
        margin: { top: 0, left: 0, right: 0, bottom: 10 },
        layout: {
            type: 'hbox',
            border: false
        },
        border: false,
        items: [searchField_needattendance, button_needattendance]
    });

    var ActionsMenuList = [];
    ActionsMenuList.push({ vlu: 'signinsheet', labl: 'Print Classic Sign-in Sheet' });
    ActionsMenuList.push({ vlu: 'signinsheetnew', labl: 'Print Sign-in Sheet' });
    if (HideCertificateLink == 0) {
        ActionsMenuList.push({ vlu: 'mailcert', labl: 'Print/Email Certificates' });
    }
    ActionsMenuList.push({ vlu: 'nametag', labl: 'Print Name Tag' });
    if ((AllowInstructortoTakeAttendace == 1) || (AllowInstructortoTakeAttendace == -1)) {
        ActionsMenuList.push({ vlu: 'attendance', labl: 'Take/Edit Attendance' });
    }
    ActionsMenuList.push({ vlu: 'email', labl: 'Email Course' });
    ActionsMenuList.push({ vlu: 'rosterreport', labl: 'Roster Report' });
   // ActionsMenuList.push({ vlu: 'survey', labl: 'Send Survey' });

    var ActionsMenuStore = Ext.create('Ext.data.Store', {
        fields: ['vlu', 'labl'],
        data: ActionsMenuList
    });

    var combotpl = new Ext.XTemplate(
        '<div style="width:140px; border: 1px solid grey; height: 20px; padding: 0px;"><div style="color:grey;float: left;width: 95px; margin-top:2px;">&nbsp;&nbsp;select...</div><div style="float: left; text-align:right; width: 30px; margin-top:2px;">&#9660;&nbsp;</div></div>'
    );

    var grid = Ext.create('Ext.grid.Panel', {
        id: 'StudentListWidgetGrid',
        region: 'center',
        frame: true,
        //width:650,
        title: Terminology.capital('course'),
        dockedItems: [
            searchfield_1, {
                xtype: 'pagingtoolbar',
                store: store,
                dock: 'top',
                displayInfo: true
            },
        ],
        store: store,
        emptyText: 'No Courses found',
        columns: [
            {
                text: 'CID',
                dataIndex: 'CourseId',
                width: 50,
                hidden: true,
                renderer: function (myValue, val, myRecord) {
                    var sid = myRecord.get('StudentId');
                    return '' + myValue + '';
                }
            },
                                    {
                                        text:  Terminology.capital('course')+' Number',
                                        dataIndex: 'CourseNum',
                                        width: 100,
                                        renderer: function (myValue, val, myRecord) {
                                            var sid = myRecord.get('StudentId');
                                            return '' + myValue + '';
                                        }
                                    },
            {
                text: Terminology.capital('course') + ' Name',
                dataIndex: 'CourseName',
                flex: 1,
                width: 200,
                renderer: function (myValue, val, myRecord) {
                    var cid = myRecord.get('CourseId');
                    return '<div onclick=\'CourseSearch.prototype.ShowCourseDetails(' + cid + ', "instructor");\' style="color:blue; text-decoration: underline; cursor:pointer;" >' + myValue + '</div>';
                }
            },
            {
                text: 'Start Date',
                dataIndex: 'CDates',
                flex: 1,
                width: 100,
                renderer: function (myValue, val, myRecord) {
                    var cid = myRecord.get('CourseId');
                    var tipdates = myRecord.get('CourseDatesandTime') + ''
                    val.tdAttr = 'data-qtip="' + tipdates.replace(/<[^>]*>/g, '') + '"';
                    return   myValue +'<img  src="/admin/images/icons/information.png" style="float:right;"/>';
                }

            },
            {
                text: 'Location and Room',
                dataIndex: 'Location',
                flex: 1,
                width: 100,
                renderer: function (value, metadata) {
                    metadata.tdAttr = 'data-qtip="' + value + '"';
                    return value;
                }
            },
            {
                text: 'Number Enrolled',
                dataIndex: 'Enrolled',
                flex: 1,

            },
            {
                text: 'Cancelled',
                dataIndex: 'Cancelled',
                flex: 1,
                hidden:true

            },
            {
                text: 'Waiting',
                dataIndex: 'NoWaiting',
                flex: 1,
                hidden: true

            },
            {
                text: 'Unpaid',
                dataIndex: 'Unpaid',
                flex: 1,
                hidden: true

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
                    width: 150,
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
                                'focus': function (fld) { this.onTriggerClick(); console.log('dfgdfgdf') },
                                'change': function (field, selectedValue) {

                                    var ucwg = Ext.getCmp('StudentListWidgetGrid');
                                    var selection = ucwg.getSelectionModel().getSelection()[0];
                                    var cid = selection.get('CourseId');
                                    var coursename = selection.get('CourseName');

                                    self.executeCourseAction(this.value, cid, coursename);
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
            afterRender: function () {
                var gridstore = Ext.getCmp('StudentListWidgetGrid').getStore();
                gridstore.reload();
            }
        }

    });

    var grid_Past = Ext.create('Ext.grid.Panel', {
        id: 'PastCourseListWidgetGrid',
        region: 'center',
        frame: true,
        //width:650,
        title: 'Courses',
        store: store_past,
        emptyText: 'No Courses found',
        dockedItems: [
        searchfield_2, {
            xtype: 'pagingtoolbar',
            store: store_past,
            dock: 'top',
            displayInfo: true
        },
        ],
        columns: [
            {
                text: '#',
                dataIndex: 'CourseId',
                width: 50,
                hidden: true,
                renderer: function (myValue, val, myRecord) {
                    var sid = myRecord.get('StudentId');
                    return '' + myValue + '';
                }
            },
                            {
                                text: Terminology.capital('course') + ' Number',
                                dataIndex: 'CourseNum',
                                width: 100,
                                renderer: function (myValue, val, myRecord) {
                                    var sid = myRecord.get('StudentId');
                                    return '' + myValue + '';
                                }
                            },
            {
                text: Terminology.capital('course') + ' Name',
                dataIndex: 'CourseName',
                flex: 1,
                width: 200,
                renderer: function (myValue, val, myRecord) {
                    var cid = myRecord.get('CourseId');
                    return '<div onclick=\'CourseSearch.prototype.ShowCourseDetails(' + cid + ', "instructor");\' style="color:blue; text-decoration: underline; cursor:pointer;" >' + myValue + '</div>';
                }
            },
            {
                text: 'Start Date',
                dataIndex: 'CDates',
                flex: 1,
                width: 100,
                renderer: function (myValue, val, myRecord) {
                    var cid = myRecord.get('CourseId');
                    var tipdates = myRecord.get('CourseDatesandTime') + ''
                    val.tdAttr = 'data-qtip="' + tipdates.replace(/<[^>]*>/g, '') + '"';
                    return myValue + '<img  src="/admin/images/icons/information.png" style="float:right;"/>';
                }
            },
            {
                text: 'Location and Room',
                dataIndex: 'Location',
                flex: 1,
                width: 100,
                renderer: function (value, metadata) {
                    metadata.tdAttr = 'data-qtip="' + value + '"';
                    return value;
                }
            },
            {
                text: 'Number Enrolled',
                dataIndex: 'Enrolled',
                flex: 1,
                width: 50,

            },
             {
                 text: 'Cancelled',
                 dataIndex: 'Cancelled',
                 flex: 1,
                 hidden: true

             },
                {
                    text: 'Waiting',
                    dataIndex: 'NoWaiting',
                    flex: 1,
                    hidden: true

                },
                {
                    text: 'Unpaid',
                    dataIndex: 'Unpaid',
                    flex: 1,
                    hidden: true

                },

            {
                text: 'Actions',
                xtype: 'templatecolumn',
                width: 150,
                tpl: combotpl,
                editor:
                    {
                        xtype: 'combobox',
                        id: 'supstudmenucombopast',
                        forceSelection: true,
                        editable: false,
                        store: ActionsMenuStore,
                        emptyText: 'select...',
                        displayField: 'labl',
                        valueField: 'vlu',
                        listeners: {
                            'focus': function (fld) { this.onTriggerClick(); console.log('dfgdfgdf') },
                            'change': function (field, selectedValue) {

                                var ucwg = Ext.getCmp('PastCourseListWidgetGrid');
                                var selection = ucwg.getSelectionModel().getSelection()[0];
                                var cid = selection.get('CourseId');

                                var coursename = selection.get('CourseName');

                                self.executeCourseAction(this.value, cid, coursename);
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
        }
    });

    var grid_Cancelled = Ext.create('Ext.grid.Panel', {
        id: 'CancelledCourseListWidgetGrid',
        region: 'center',
        frame: true,
        //width:650,
        title: 'Courses',
        store: store_cancelled,
        emptyText: 'No Courses found',
        dockedItems: [
        searchfield_4, {
            xtype: 'pagingtoolbar',
            store: store_cancelled,
            dock: 'top',
            displayInfo: true
        },
        ],
        columns: [
            {
                text: '#',
                dataIndex: 'CourseId',
                width: 50,
                hidden: true,
                renderer: function (myValue, val, myRecord) {
                    var sid = myRecord.get('StudentId');
                    return '' + myValue + '';
                }
            },
                            {
                                text: Terminology.capital('course') + ' Number',
                                dataIndex: 'CourseNum',
                                width: 100,
                                renderer: function (myValue, val, myRecord) {
                                    var sid = myRecord.get('StudentId');
                                    return '' + myValue + '';
                                }
                            },
            {
                text: Terminology.capital('course') + ' Name',
                dataIndex: 'CourseName',
                flex: 1,
                width: 200,
                renderer: function (myValue, val, myRecord) {
                    var cid = myRecord.get('CourseId');
                    return '<div onclick=\'CourseSearch.prototype.ShowCourseDetails(' + cid + ', "instructor");\' style="color:blue; text-decoration: underline; cursor:pointer;" >' + myValue + '</div>';
                }
            },
            {
                text: 'Cancel Date',
                dataIndex: 'CancelCourseDateTime',
                flex: 1,
                width: 100,
                renderer: function (myValue, val, myRecord) {
                    return myValue;
                }
            },
            {
                text: 'Actions',
                xtype: 'templatecolumn',
                width: 150,
                tpl: combotpl,
                editor:
                    {
                        xtype: 'combobox',
                        id: 'supstudmenucombocancelled',
                        forceSelection: true,
                        editable: false,
                        store: ActionsMenuStore,
                        emptyText: 'select...',
                        displayField: 'labl',
                        valueField: 'vlu',
                        listeners: {
                            'focus': function (fld) { this.onTriggerClick(); console.log('dfgdfgdf') },
                            'change': function (field, selectedValue) {

                                var ucwg = Ext.getCmp('CancelledCourseListWidgetGrid');
                                var selection = ucwg.getSelectionModel().getSelection()[0];
                                var cid = selection.get('CourseId');

                                var coursename = selection.get('CourseName');

                                self.executeCourseAction(this.value, cid, coursename);
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
        }
    });

    var grid_Need_Attendance = Ext.create('Ext.grid.Panel', {
        id: 'NeedAttendanceCourseListWidgetGrid',
        region: 'center',
        frame: true,
        //width:650,
        title: 'Courses',
        store: store_needattendance,
        emptyText: 'No Courses found',
        dockedItems: [
        searchfield_3, {
            xtype: 'pagingtoolbar',
            store: store_needattendance,
            dock: 'top',
            displayInfo: true
        },
        ],
        columns: [
            {
                text: '#',
                dataIndex: 'CourseId',
                width: 50,
                hidden: true,
                renderer: function (myValue, val, myRecord) {
                    var sid = myRecord.get('StudentId');
                    return '' + myValue + '';
                }
            },
                    {
                        text: Terminology.capital('course') + ' Number',
                        dataIndex: 'CourseNum',
                        width: 100,
                        renderer: function (myValue, val, myRecord) {
                            var sid = myRecord.get('StudentId');
                            return '' + myValue + '';
                        }
                    },
            {
                text: Terminology.capital('course') + ' Name',
                dataIndex: 'CourseName',
                flex: 1,
                width: 200,
                renderer: function (myValue, val, myRecord) {
                    var cid = myRecord.get('CourseId');
                    return '<div onclick=\'CourseSearch.prototype.ShowCourseDetails(' + cid + ', "instructor");\' style="color:blue; text-decoration: underline; cursor:pointer;" >' + myValue + '</div>';
                }
            },
            {
                text: 'Start Date',
                dataIndex: 'CDates',
                flex: 1,
                width: 100,
                renderer: function (myValue, val, myRecord) {
                    var cid = myRecord.get('CourseId');
                    var tipdates = myRecord.get('CourseDatesandTime') + ''
                    val.tdAttr = 'data-qtip="' + tipdates.replace(/<[^>]*>/g, '') + '"';
                    return myValue + '<img  src="/admin/images/icons/information.png" style="float:right;"/>';
                }

            },
            {
                text: 'Location and Room',
                dataIndex: 'Location',
                flex: 1,
                width: 100,
                renderer: function (value, metadata) {
                    metadata.tdAttr = 'data-qtip="' + value + '"';
                    return value;
                }
            },
            {
                text: 'Number Enrolled',
                dataIndex: 'Enrolled',
                flex: 1,
                width: 20,

            },
            {
                text: 'Cancelled',
                dataIndex: 'Cancelled',
                flex: 1,
                hidden: true

            },
            {
                text: 'Waiting',
                dataIndex: 'NoWaiting',
                flex: 1,
                hidden: true

            },
            {
                text: 'Unpaid',
                dataIndex: 'Unpaid',
                flex: 1,
                hidden: true

            },

            {
                text: 'Actions',
                xtype: 'templatecolumn',
                width: 150,
                tpl: combotpl,
                editor:
                    {
                        xtype: 'combobox',
                        id: 'supstudmenucomboneedattendance',
                        forceSelection: true,
                        editable: false,
                        store: ActionsMenuStore,
                        emptyText: 'select...',
                        displayField: 'labl',
                        valueField: 'vlu',
                        listeners: {
                            'focus': function (fld) { this.onTriggerClick(); console.log('dfgdfgdf') },
                            'change': function (field, selectedValue) {

                                var ucwg = Ext.getCmp('NeedAttendanceCourseListWidgetGrid');
                                var selection = ucwg.getSelectionModel().getSelection()[0];
                                var cid = selection.get('CourseId');

                                var coursename = selection.get('CourseName');

                                self.executeCourseAction(this.value, cid, coursename);
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
        }
    });
    var tabs = Ext.create('Ext.tab.Panel', {
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
        items: [{
            title: 'Current ' + Terminology.capital('course') + 's',
            itemId: 'current',
            items: [grid],
            listeners: {
                activate: function () {
                    var gridstore = Ext.getCmp('StudentListWidgetGrid').getStore();
                    gridstore.reload();
                }
            }

        }, {
            title: 'Past ' + Terminology.capital('course') + 's',
            itemId: 'past',
            items: [grid_Past],
            listeners: {
                activate: function () {
                    var gridstore = Ext.getCmp('PastCourseListWidgetGrid').getStore();
                    gridstore.reload();
                }
            }
        }, {
            title: 'Cancel ' + Terminology.capital('course') + 's',
            itemId: 'cancelled',
            items: [grid_Cancelled],
            listeners: {
                activate: function () {
                    var gridstore = Ext.getCmp('CancelledCourseListWidgetGrid').getStore();
                    gridstore.reload();
                }
            }
        },
        , {
            title: Terminology.capital('course')+'(s) need attendance taken',
            itemId: 'needattendance',
            items: [grid_Need_Attendance],
            listeners: {
                activate: function () {
                    var gridstore = Ext.getCmp('NeedAttendanceCourseListWidgetGrid').getStore();
                    gridstore.reload();
                }
            }
        }]
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
        items: [tabs]
    }));


};
