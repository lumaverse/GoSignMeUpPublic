function LtiSettings(options) {

    var self = this;

    self.Options = Ext.merge(self.Options, options);
    Ext.onDocumentReady(function () {    
        self.Render();
    });
}

LtiSettings.constructor = LtiSettings;

LtiSettings.prototype.Options = {
    containerId: null,
    settings: {
        OAuthServiceKey: "",
        OAuthServiceSecret: "",
        LtiConfigurationUrl: null,
        LtiConfigurations: []
    }
};

LtiSettings.prototype.State = {
    container: null
};


LtiSettings.prototype.Render = function () {
    var self = this;

    self.State.container = Ext.get(self.Options.containerId);

    var buttons = null;

    var ssoUrl = location.protocol + '//' + location.hostname + config.getUrl('SSO/Lti');

    var items = [
            {
                xtype: 'displayfield',
                fieldLabel: 'Lti SSO Url',
                value: '<a href="' + ssoUrl + '" target="_blank">' + ssoUrl + '</a>'
            },
            {
                name: 'oAuthServiceKey',
                fieldLabel: 'Lti OAuth Service Key',
                value: self.Options.settings.OAuthServiceKey
            },
            {
                name: 'oAuthServiceSecret',
                fieldLabel: 'Lti OAuth Service Secret',
                value: self.Options.settings.OAuthServiceSecret
            }
    ];

    if (self.Options.settings.LtiConfigurations.length > 0) {
        var value = '';
        var comma = '';
        for (var index = 0; index < self.Options.settings.LtiConfigurations.length; index++) {
            value += comma;
            var setting = self.Options.settings.LtiConfigurations[index];
            var url = self.Options.settings.LtiConfigurationUrl.replace('{0}', setting);
            value += '<a class="lti-config" id="lti-config-' + setting + '" href="javascript: void(0);  ">' + url + '</a>';
            if (setting.toLowerCase().indexOf('canvas') != -1) {
                value += '&nbsp;<span style="font-size: smaller;">(This is <a title="Click here to check the Canvas setup" href="' + config.getUrl('adm/canvas/settings') + '">Canvas</a> related configuration.)</span>'
            }
            comma = '<br/>';
        }
        items.push({
            xtype: 'displayfield',
            fieldLabel: 'Available XML and URL based LTI configurations ',
            value: value,
            listeners: {
                render: function () {
                    var links = Ext.query('.lti-config');
                    for (var index = 0; index < links.length; index++) {
                        var el = Ext.get(links[index]);

                        el.on('click', function () {
                            var link = el;
                            var id = el.dom.id.replace('lti-config-', '');
                            Ext.Ajax.request({
                                url: el.dom.innerText,
                                success: function (response) {

                                    var text = response.responseText;
                                    console.log(location);
                                    text = text.replace("{0}", location.host);
                                    text = Ext.String.htmlEncode(text);

                                    var win = new Ext.Window({
                                        title: 'LTI XML Configuration: ' + Ext.String.capitalize(id),
                                        width: Math.max(document.documentElement.clientWidth - 300, 100),
                                        height: Math.max(document.documentElement.clientHeight - 300, 100),
                                        layout: 'fit',
                                        modal: true,
                                        autoScroll: true,
                                        html: '<pre>' + text + '</pre>'
                                    });
                                    win.show();
                                }
                            });
                        });

                        var tip = Ext.create('Ext.tip.ToolTip', {
                            target: el,
                            html: 'Click to see the XML',
                        });
                    }
                }
            }
        });
    }

    var form = Ext.create('Ext.form.Panel', {
        renderTo: self.State.container,
        title: 'Lti settings',

        // The form will submit an AJAX request to this URL when submitted
        url: config.getUrl('adm/lti/savesettings'),

        // Fields will be arranged vertically, stretched to full width
        layout: 'anchor',
        defaults: {
            anchor: '100%'
        },
        fieldDefaults: {
            labelWidth: 300,
            labelAlign: 'right'
        },
        defaultType: 'textfield',
        // The fields
        items: items,

        // Reset and Submit buttons
        buttons: [
            {
                id: 'Lti-settings-buttons',
                xtype: 'panel',
                defaultType: 'button',
                frame: false,
                border: 0,
                items: [
                     {
                        text: 'Reset',
                        handler: function () {
                            var form = this.up('form').getForm();
                            form.reset();
                            saveForm();
                        }
                    }, {
                        text: 'Save settings',
                        formBind: true, //only enabled once the form is valid
                        disabled: true,
                        handler: function () {
                            var form = this.up('form').getForm();
                            saveForm();
                        }
                    }
                ]
            }
        ]
    });

    form.isValid();
    buttons = Ext.getCmp('Lti-settings-buttons');
    
    var saveForm = function (success) {
        if (form.isValid()) {
            window.LAYOUT.MaskLayout('Saving Lti settings ...');
            form.submit({
                success: function (form, action) {
                    window.LAYOUT.UnmaskLayout();
                    self.Options.settings = Ext.merge(self.Options.settings, action.result.config);
                    if (action.result.msg) {
                        Ext.Msg.alert('Success', action.result.msg);
                    }
                    if (Ext.isFunction(success)) {
                        success();
                    }
                },
                failure: function (form, action) {
                    window.LAYOUT.UnmaskLayout();
                    Ext.Msg.alert('Failed', action.result.msg);
                }
            });
        }
    };

    Ext.on('resize', function () {
        form.doLayout();
    });
}