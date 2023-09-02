
function Development() {
    var self = this;

    window.DEVELOPMENT = self;
}

Development.constructor = Development;

Development.prototype.SendEmail = function () {

    var win = Ext.create('Ext.window.Window', {
        modal: true,
        icon: config.getUrl('Images/Icons/FamFamFam/email.png'),
        title: 'Send a test e-mail',
        layout: 'fit',
        resizable: false,
        frame: false,
        items: [
            {
                xtype: 'form',
                id: 'email-form',
                defaultType: 'textfield',
                frame: false,
                border: 0,
                layout: 'form',
                bodyPadding: 5,
                items: [{
                    fieldLabel: 'Mail handler',
                    editable: false,
                    name: 'mailHandler',
                    xtype: 'combobox',
                    store: ['DotNet', 'JMail'],
                    forceSelection: true,
                    typeAhead: false
                }, {
                    fieldLabel: 'From',
                    name: 'from',
                    allowBlank: false,
                    vtype: 'email'
                }, {
                    fieldLabel: 'To',
                    name: 'to',
                    allowBlank: false,
                    vtype: 'email'
                }, {
                    fieldLabel: 'Subject',
                    name: 'subject',
                    allowBlank: false
                }, {
                    fieldLabel: 'Body',
                    name: 'body',
                    xtype: 'gsmuhtmleditor'
                }]
            }
        ],
        buttons: [
            {
                text: 'Close',
                icon: config.getUrl('Images/Icons/FamFamFam/door_out.png'),
                handler: function () {
                    win.close();
                }
            },
            {
            text: 'Send',
            icon: config.getUrl('Images/Icons/FamFamFam/email_go.png'),
            handler: function (that) {
                var form = Ext.getCmp('email-form');
                form.submit({
                    url: config.getUrl('Adm/development/email'),
                    success: function () {
                        LAYOUT.notify('Message sent...');
                    },
                    failure: function (form, action) {
                        config.showError(action.result);
                    }
                });
            }
        }]
    });
    win.show();

}