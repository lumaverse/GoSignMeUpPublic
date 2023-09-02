function AdminStudentListWidget(dashboard) {
    var self = this;

    self.State.dashboard = dashboard;
}

AdminStudentListWidget.constructor = AdminStudentListWidget;

AdminStudentListWidget.prototype.Options = {
};

AdminStudentListWidget.prototype.State = {
    container: null,
    dashboard: null,
    widget: null
};
AdminStudentListWidget.prototype.executeCourseAction = function (cmd, record) {
    var self = this;
    if (cmd == '') {
        return;
    }


    switch (cmd) {
        case 'emailstudent':
            self.EmailStudent(record);
            break;

    }

}

AdminStudentListWidget.prototype.EmailStudent = function (sid) {
    
    Ext.Ajax.request({
        url: config.getUrl('public/user/GetStudentRecord'),
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
                                //loadMask: { msg: 'please wait...' },
                                height: 20,
                                margin: { top: 0, left: 660, right: 12, bottom: 0 },
                                align: 'right',
                                listeners: {
                                    click: function () {
                                        //window.LAYOUT.MaskLayout('Sending ...');

                                        Ext.Ajax.request({
                                            url: config.getUrl('public/user/SendStudentEmail'),
                                            params: {
                                                Subject: Ext.getCmp('ESubject').value,
                                                To: Ext.getCmp('ETo').value,
                                                Message: HtmlEncode(Ext.getCmp('textMessage').value)
                                            },
                                            success: function (response) {
                                                Ext.Msg.alert('Result', response.responseText.replace(/"/g, ''));
                                                //window.LAYOUT.UnmaskLayout();
                                                window.close()
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



