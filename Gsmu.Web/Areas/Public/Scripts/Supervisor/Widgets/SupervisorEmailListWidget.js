function SupervisorEmailListandReportWidget(dashboard) {
    var self = this;

    self.State.dashboard = dashboard;
}

SupervisorEmailListandReportWidget.prototype = new WidgetBase();

SupervisorEmailListandReportWidget.constructor = SupervisorEmailListandReportWidget;

SupervisorEmailListandReportWidget.prototype.Options = {
};

SupervisorEmailListandReportWidget.prototype.State = {
    container: null,
    dashboard: null,
    widget: null
};

SupervisorEmailListandReportWidget.prototype.ReportAction = function(cmd)
{
    var self = this;
    switch (cmd) {
        case 'addstudent':
            window.location.assign("/Public/User/RegisterUser")
            break;
        case 'supervisorreport':
            self.SupervisorReport();
            break;
        case 'transcriptreport':
            self.TranscriptReport();
            break;
        case 'enrolledreport':
            self.EnrolledReport();
            break;
        case 'coursegrid':
            self.SupervisorCourseReport();
            break;
        case 'certificationreport':
            self.CertificationReport();
            break;
        case 'approvetowaitlist':
            self.RenderStudentsWaitList();
            break;
          
    }
}
SupervisorEmailListandReportWidget.prototype.SupervisorReport = function(cmd)
{
    var self = this;
    winreport = Ext.create('Ext.form.Panel', {
        floating: true,
        centered: true,
        modal: true,
        width: 970,
        height: 640,
        closable: true,
        defaultType: 'textfield',
        bodyPadding: 10,
        html: '<iframe width="950" height="630" src="/admin/dev_supervisors.asp?action=ManagersReport&misc=552&rubyrequest=1&uname=' + username + '&usersessionid=' + supervisorsessionId + '"></iframe>',

        renderTo: Ext.getBody(),
    });
    winreport.show();
    //self.LogoffClassic();
}
SupervisorEmailListandReportWidget.prototype.CertificationReport = function (cmd) {
    winreport = Ext.create('Ext.form.Panel', {
        floating: true,
        centered: true,
        modal: true,
        width: 970,
        height: 640,
        closable: true,
        defaultType: 'textfield',
        bodyPadding: 10,
        html: '<iframe width="950" height="630" src="/admin/dev_supervisors.asp?action=certifications&misc=552&rubyrequest=1&uname=' + username + '&usersessionid=' + supervisorsessionId + '"></iframe>',

        renderTo: Ext.getBody(),
    });
    winreport.show();
}
SupervisorEmailListandReportWidget.prototype.TranscriptReport = function (cmd) {
    winreport = Ext.create('Ext.form.Panel', {
        floating: true,
        centered: true,
        modal: true,
        width: 970,
        height: 640,
        closable: true,
        defaultType: 'textfield',
        bodyPadding: 10,
        html: '<iframe width="950" height="630" src="/admin/dev_supervisors.asp?action=reportchoice&reportaction=viewtranscripts&misc=552&rubyrequest=1&uname=' + username + '&usersessionid=' + supervisorsessionId + '"></iframe>',

        renderTo: Ext.getBody()
    });
    winreport.show();
}

SupervisorEmailListandReportWidget.prototype.EnrolledReport = function (cmd) {
    winreport = Ext.create('Ext.form.Panel', {
        floating: true,
        centered: true,
        modal: true,
        width: 970,
        height: 640,
        closable: true,
        defaultType: 'textfield',
        bodyPadding: 10,
        html: '<iframe width="950" height="630" src="/admin/dev_supervisors.asp?action=reportchoice&reportaction=viewenrolled&misc=552&rubyrequest=1&uname=' + username + '&usersessionid=' + supervisorsessionId + '"></iframe>',

        renderTo: Ext.getBody(),
    });
    winreport.show();
}
SupervisorEmailListandReportWidget.prototype.SupervisorCourseReport = function (cmd) {
    var self = this;
    winreport = Ext.create('Ext.form.Panel', {
        floating: true,
        centered: true,
        modal: true,
        width: 970,
        height: 640,
        closable: true,
        defaultType: 'textfield',
        bodyPadding: 10,
        html: '<iframe width="950" height="630" src="/admin/dev_supervisors.asp?action=coursegridcurrent&misc=552&rubyrequest=1&uname=' + username + '&usersessionid=' + supervisorsessionId + '"></iframe>',

        renderTo: Ext.getBody(),
    });
    winreport.show();
}
SupervisorEmailListandReportWidget.prototype.LogoffClassic = function (cmd) {
    off = Ext.create('Ext.form.Panel', {
        floating: true,
        centered: true,
        modal: true,
        width: 970,
        height: 640,
        closable: true,
        defaultType: 'textfield',
        bodyPadding: 10,
        html: '<iframe width="950" height="630" src="/admin/logoff.asp?type=supervisor&misc=253"></iframe>',

        renderTo: Ext.getBody(),
    });
    off.show();
    off.hide();
}
SupervisorEmailListandReportWidget.prototype.RenderImplementation = function () {
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
    var grid = Ext.create('Ext.grid.Panel', {
        region: 'center',
        flex: 3,
        width: '90%',
        frame: true,
        title: 'Received Email',
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
                                        attachments += '<a target="_blank" href="@(adminPath)datastores/datastore-user.asp?action=get-file&param=' + encodeURIComponent(list[index]) + '">Attachment ' + count + '</a><br/>';
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
        certificationlink = "";
        var enrollToWaitListLink = '';
    }
    else {
        certificationlink = '<tr><td><div style="cursor:pointer;" onclick="SupervisorEmailListandReportWidget.prototype.ReportAction(\'certificationreport\')"><img src="Areas/Public/Images/Icons/course-center-actions-certs.png" style="width:50px; height:40px;" /></div></td><td><div style="cursor:pointer;" onclick="SupervisorEmailListandReportWidget.prototype.ReportAction(\'certificationreport\')">Certification Report</div></td></tr>'

    }
    if (enrollToWaitList && JSON.parse(enrollToWaitList.toLocaleLowerCase()))
    {
        enrollToWaitListLink = '<tr><td><div style="cursor:pointer;" onclick="SupervisorEmailListandReportWidget.prototype.ReportAction(\'approvetowaitlist\')"><img src="Areas/Public/Images/Icons/action-clock.png" style="width:50px; height:40px;" /></div></td><td><div style="cursor:pointer;" onclick="SupervisorEmailListandReportWidget.prototype.ReportAction(\'approvetowaitlist\')">Approve Waitlist Report</div></td></tr>';
    }
    if (SupervisorDashboard.prototype.State.supervisorData.AdvanceOptions!="0") {
    
        optionalhtml = '<tr><td><div style="cursor:pointer;" onclick="SupervisorEmailListandReportWidget.prototype.ReportAction(\'addstudent\')"><img src="Areas/Public/Images/Icons/course-center-actions-addstudent_thumb.png" style="width:50px; height:40px;"  /></div></td><td><div style="cursor:pointer;" onclick="SupervisorEmailListandReportWidget.prototype.ReportAction(\'addstudent\')">Add New Student</div></td></tr>';
    }
    var reportpanel = Ext.create('Ext.Panel', {
        title:'Reports',
        width: '10%',
        height: 400,
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
            html: ['<table>'+optionalhtml,
                '<tr><td><div style="cursor:pointer;" onclick="SupervisorEmailListandReportWidget.prototype.ReportAction(\'supervisorreport\')"><img src="Areas/Public/Images/Icons/course-center-actions-supervisor_thumb.png" style="width:50px; height:40px;" /></div></td><td><div style="cursor:pointer;" onclick="SupervisorEmailListandReportWidget.prototype.ReportAction(\'supervisorreport\')">Supervisor Report</div></td></tr>',
                '<tr><td><div style="cursor:pointer;" onclick="SupervisorEmailListandReportWidget.prototype.ReportAction(\'transcriptreport\')"><img src="Areas/Public/Images/Icons/course-center-actions-transcript_thumb.png" style="width:50px; height:40px;" /></div></td><td><div style="cursor:pointer;" onclick="SupervisorEmailListandReportWidget.prototype.ReportAction(\'transcriptreport\')">Transcript Report</div></td></tr>',
                '<tr><td><div style="cursor:pointer;" onclick="SupervisorEmailListandReportWidget.prototype.ReportAction(\'enrolledreport\')"><img src="Areas/Public/Images/Icons/course-center-actions-enroll_thumb.png" style="width:50px; height:40px;" /></div></td><td><div style="cursor:pointer;" onclick="SupervisorEmailListandReportWidget.prototype.ReportAction(\'enrolledreport\')">Enrollment Report</div></td></tr>',
                '<tr><td><div style="cursor:pointer;" onclick="SupervisorEmailListandReportWidget.prototype.ReportAction(\'coursegrid\')"><img src="Areas/Public/Images/Icons/course-center-actions-duplicate.png" style="width:50px; height:40px;" /></div></td><td><div style="cursor:pointer;" onclick="SupervisorEmailListandReportWidget.prototype.ReportAction(\'coursegrid\')">' + Terminology.lower('course') + ' Report</div></td></tr>',
                enrollToWaitListLink,
                certificationlink,
                '</table>'
            ].join(""),
            flex: 1
        }

        ]
    });
   var mainpanel =  Ext.create('Ext.Panel', {
       preventHeader: true,
       frame: false,
       shrinkWrap:false,
        layout: {
            type: 'hbox',
            border: false
        },
        border: false,
        items: [grid, reportpanel]
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

SupervisorEmailListandReportWidget.prototype.RenderStudentsWaitList = function () {
    var url = '/public/supervisor/StudentsWaitingList';
    var self = this;
    winreport = Ext.create('Ext.form.Panel', {
        floating: true,
        centered: true,
        modal: true,
        width: 970,
        height: 640,
        closable: true,
        defaultType: 'textfield',
        bodyPadding: 10,
        html: '<iframe width="950" height="630" src="' + url + '"></iframe>',

        renderTo: Ext.getBody(),
    });
    winreport.show();
}
