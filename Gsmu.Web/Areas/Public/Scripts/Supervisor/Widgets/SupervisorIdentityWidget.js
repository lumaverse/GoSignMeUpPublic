function SupervisorIdentityWidget(dashboard) {
    var self = this;

    self.State.dashboard = dashboard;
}

SupervisorIdentityWidget.prototype = new WidgetBase(); 

SupervisorIdentityWidget.constructor = SupervisorIdentityWidget;

SupervisorIdentityWidget.prototype.Options = {
};

SupervisorIdentityWidget.prototype.State = {    
    container: null,
    dashboard: null,
    widget: null
};

SupervisorIdentityWidget.prototype.RenderImplementation = function () {
    var self = this;

    var supervisorData = self.State.dashboard.State.supervisorData;
    var noProfileImage = self.State.dashboard.Options.noProfileImage;

    var profileImage = null;

    var getProfileImage = function () {
        profileImage = noProfileImage;
        if (supervisorData.ProfileImageFile != null && supervisorData.ProfileImageFile != '') {
            profileImage = self.State.dashboard.Options.profileImagePath + supervisorData.ProfileImageFile + '?rnd=' + Math.random();
        }
    };
    getProfileImage();

    self.State.widget = Ext.create('Ext.form.Panel', self.getWidgetDefaults({
        userMembers: {
            viewItems: ['username-display', 'password-display'],
            editItems: ['profile-upload', 'username', 'password', 'password-confirm'],

            setEditVisibilityComplete: function (panel, visible) {
                var uploadPanel = panel.getComponent('profile-upload');
                var clearButton = uploadPanel.getComponent('profile-upload-clear');
                clearButton.toggle(false);
                panel.getComponent('profile-upload-remove').setValue('false');
            },
            editModeComplete: function(panel) {
                panel.userMembers.layout(panel);
            },
            viewModeComplete: function(panel) {
                panel.userMembers.layout(panel);
            },
            layout: function (panel) {
                var image = panel.getComponent('profile-image');
                image.userMembers.setDimensions();
            }
        },
        listeners: {
            render: function(panel) {
                panel.userMembers.viewMode(panel);
            }
        },
        frame: true,
        title: 'Identity',
        url: config.getUrl('public/supervisor/saveidentity'),
        icon: config.getUrl('Images/Icons/Famfamfam/user.png'),
        items: [
            {
                userMembers: {
                    cmp: null,
                    width: null,
                    height: null,
                    setDimensions: function () {
                        var cmp = this.cmp;
                        if (cmp == null) {
                            return;
                        }
                        var heightExtension = 10;
                        if (cmp.getHeight() != cmp.userMembers.height + heightExtension) {
                            cmp.setHeight(cmp.userMembers.height + heightExtension );
                        }                    
                        var imageEl = cmp.getEl();
                        if (imageEl.getWidth() != cmp.userMembers.width) {
                            imageEl.setWidth(cmp.userMembers.width);
                        }
                    }
                },
                xtype: 'image',
                style: {
                    marginTop: self.State.dashboard.Options.margins.top,
                    marginLeft: self.State.dashboard.Options.margins.left,
                    marginRight: self.State.dashboard.Options.margins.right,
                    border: "3px solid #cccccc",
                    cursor: profileImage == noProfileImage ? 'cursor' : 'pointer'
                },
                itemId: 'profile-image',
                src: profileImage,
                listeners: {
                    render: function (cmp) {
                        var imageEl = cmp.getEl();
                        
                        imageEl.on('click', function () {                            
                            window.open(imageEl.dom.src);
                        });

                        imageEl.on('load', function (eventCmp, dom) {
                            var dash = self.State.dashboard;

                            var maxWidth = dash.Options.profileImageMaxWidth;
                            var maxHeight = dash.Options.profileImageMaxHeight;

                            var currentWidthDifference = maxWidth / dom.naturalWidth;
                            var currentHeightDifference = maxHeight / dom.naturalHeight;

                            var multiplier = Math.min(currentWidthDifference, currentHeightDifference);

                            cmp.userMembers.width = dom.naturalWidth * multiplier;
                            cmp.userMembers.height = dom.naturalHeight * multiplier;

                            cmp.userMembers.cmp = cmp;
                            cmp.userMembers.setDimensions();
                        });
                    }
                }
            },
            {
                itemId: 'profile-upload',
                xtype: 'fieldcontainer',
                style: {
                    marginTop: self.State.dashboard.Options.margins.top,
                    marginLeft: self.State.dashboard.Options.margins.left,
                    marginRight: self.State.dashboard.Options.margins.right
                },
                layout: 'hbox',
                items: [
                    {
                        xtype: 'filefield',
                        hideLabel: true,
                        itemId: 'profile-upload-file',
                        name: 'upload',
                        buttonConfig: {
                            text: 'Select',
                            icon: config.getUrl('Images/icons/famfamfam/image_add.png')
                        },
                        listeners: {
                            change: function (cmp, value) {
                                var form = cmp.up('form');
                                form.down('#profile-upload-file-cancel').show();
                                form.down('#profile-upload-clear').hide();
                                form.userMembers.layout(form);
                            }
                        }
                    },
                    {
                        xtype: 'button',
                        icon: config.getUrl('Images/icons/famfamfam/delete.png'),
                        hidden: true,
                        itemId: 'profile-upload-file-cancel',
                        text: 'Cancel',
                        handler: function (cmp) {
                            var form = cmp.up('form');
                            var container = form.down('#profile-upload');
                            var fileField = form.down('#profile-upload-file');
                            var config = fileField.initialConfig;
                            fileField.destroy();
                            container.insert(0, Ext.create(config));
                            cmp.hide();
                            cmp.up('form').down('#profile-upload-clear').setVisible(!(noProfileImage == profileImage));
                            form.userMembers.layout(form);
                        }
                    },
                    {
                        itemId: 'profile-upload-clear',
                        xtype: 'button',
                        text: 'Remove',
                        hidden: noProfileImage == profileImage,
                        enableToggle: true,
                        handler: function (cmp) {
                            cmp.up('form').down('#profile-upload-remove').setValue(cmp.pressed);
                        },
                        icon: config.getUrl('Images/icons/famfamfam/image_delete.png')
                    }
                ]
            },
            {
                itemId: 'profile-upload-remove',
                xtype: 'hiddenfield',
                name: 'profileImageRemove',
                value: 'false'
            },
            {
                itemId: 'username-display',
                submitValue: false,
                xtype: 'displayfield',
                fieldLabel: 'Username',
                value: supervisorData.UserName
            },
            {
                itemId: 'username',
                xtype: 'textfield',
                name: 'username',
                allowBlank: false,
                maxLength: 50,
                fieldLabel: self.requiredIndicator + 'Username',
                value: supervisorData.UserName
            },
            {
                itemId: 'password-display',
                xtype: 'displayfield',
                submitValue: false,
                fieldLabel: 'Password',
                value: '********'
            },
            {
                itemId: 'password',
                xtype: 'textfield',
                name: 'password',
                allowBlank: false,
                maxLength: 50,
                fieldLabel: self.requiredIndicator + 'Password',
                inputType: 'password',
                value: '**********'
            },
            {
                itemId: 'password-confirm',
                submitValue: false,
                maxLength: 50,
                allowBlank: false,
                xtype: 'textfield',
                fieldLabel: self.requiredIndicator + 'Confirm password',
                inputType: 'password',
                value: '**********'
            }
        ]
    }));
};
