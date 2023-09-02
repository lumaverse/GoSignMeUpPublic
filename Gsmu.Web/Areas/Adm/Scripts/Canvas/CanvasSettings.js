function CanvasSettings(options) {

    var self = this;

    self.Options = options;
    Ext.onDocumentReady(function () {
        self.Render();
    });
}

CanvasSettings.constructor = CanvasSettings;

CanvasSettings.prototype.Options = {
    containerId: null,
    settings: {
        CanvasServerUrl: null,
        AccessToken: null,
        UserSynchronizationInDashboard: false,
        ExportUserAfterRegistration: false,
        ExportEnrollmentAfterCheckout: false,
        ExportEnrollmentCancellation: false,
        ExportEnrollmentNotUpdateCourse: false,
        DisableRosterNormalizationOnExport: false,
        allowCanvasCourseSubAccountIntegration: false,
        allowCanvasCourseSectionIntegration: false,
        allowSupervisorIntegration: false,
        allowSyncSupStudRelationIndEnrollment: false,
        allowCourseSectionPerRegistration: false,
        allowCanvasCustomCourseSISID: null,
        enableCourseGradeIntegration: false,
        CourseGradeBookFinalField: null,
        GradeBookPassValue: null,
        GradeBookPassPercentValue: null,
        CanvasGradeFinalizeAttendance: null,
        CanvasGradeUpdateAttendance: null,
        CanvasUpdateNewGrade: null,
        CanvasTranscribeRegistration: null,
        EnableCanvasLtiAuhentication: false,
        autoMapCanvasAccount: false,
        UserCanvasUniqueIdentifierField: null,
        CourseGradeSendCertificate: null,
        UserGSMUCanvasUniqueIdentifierField: null
    },
    accounts: [
      {
          "id": 1,
          "name": "iObservation Academy",
          "parent_account_id": null,
          "root_account_id": null,
          "default_storage_quota_mb": null,
          "default_user_storage_quota_mb": null,
          "default_group_storage_quota_mb": null,
          "default_time_zone": null
      },
      {
          "id": 2,
          "name": "Site Admin",
          "parent_account_id": null,
          "root_account_id": null,
          "default_storage_quota_mb": null,
          "default_user_storage_quota_mb": null,
          "default_group_storage_quota_mb": null,
          "default_time_zone": null
      }],
      AuthenticationProviders: [
      {
          "id": 1,
          "auth_type": "saml"
      },
      {
          "id": 2,
          "auth_type": "canvas"
      }]
};

CanvasSettings.prototype.State = {
    container: null,
    editRecord: null
};

CanvasSettings.prototype.Render = function (refresh) {
    var self = this;

    if (typeof (refresh) == 'undefined') {
        refresh = false;
    }

    self.State.container = Ext.get(self.Options.containerId);

    var configurationItems = [
            {
                xtype: 'container',
                html: '<div style="margin-bottom: 5px; float: right;"><img style="vertical-align: middle;" src="' + config.getUrl('images/icons/famfamfam/information.png') + '"/> Some Canvas related settings are available in the <a href="' + config.getUrl('adm/lti/settings') + '">LTI settings page</a></div>'
            },
            {
                name: 'canvasId',
                fieldLabel: 'Canvas system Id',
                value: self.Options.settings.CanvasId,
                allowBlank: false,
                listeners: {
                    change: function (control, value) {
                        self.Options.settings.CanvasId = value;
                    }
                }
            },
            {
                name: 'canvasKey',
                fieldLabel: 'Canvas system key',
                value: self.Options.settings.CanvasKey,
                allowBlank: false,
                listeners: {
                    change: function (control, value) {
                        self.Options.settings.CanvasKey = value;
                    }
                }
            }, {
                name: 'canvasServerUrl',
                fieldLabel: 'Canvas Server Url',
                value: self.Options.settings.CanvasServerUrl,
                allowBlank: false,
                listeners: {
                    change: function (control, value) {
                        self.Options.settings.CanvasServerUrl = value;
                    }
                }
            }, {
                xtype: 'fieldcontainer',
                fieldLabel: 'System Access token',
                layout: {
                    type: 'hbox'
                },
                items: [
                    {
                        flex: 1,
                        xtype: 'textfield',
                        name: 'accessToken',
                        value: self.Options.settings.AccessToken,
                        listeners: {
                            change: function (control, value) {
                                self.Options.settings.AccessToken = value;
                            }
                        }
                    }, {
                        xtype: 'button',
                        //icon: config.getUrl('images/icons/glyph2/icons16x16/bar-code.png'),
                        icon: config.getUrl('images/icons/famfamfam/money.png'),
                        text: 'Get new system access token',
                        handler: function () {
                            Ext.MessageBox.show({
                                title: 'Confirm',
                                msg: 'If you click OK, the system will request a new Canvas access token for background application use.',
                                buttons: Ext.MessageBox.OKCANCEL,
                                fn: function (buttonId) {
                                    if (buttonId == 'ok') {
                                        if (window.self != window.top) {
                                            top.location = config.getUrl('sso/Canvas?reason=system-iframe');
                                        } else {
                                            document.location = config.getUrl('sso/Canvas?reason=system');
                                        }
                                    }
                                }
                            });
                        }
                    }
                ]
            }
    ];

    var accountStore = Ext.create('Ext.data.Store', {
        fields: ['id', 'name'],
        data : self.Options.accounts
    });

    var authProviderStore = Ext.create('Ext.data.Store', {
        fields: ['id', 'auth_type'],
        data: self.Options.AuthenticationProviders
    });

    configurationItems.push({
        xtype: 'combobox',
        name: 'canvasAccountId',
        fieldLabel: 'Canvas account ',
        store: accountStore,
        displayField: 'name',
        valueField: 'id',
        allowBlank: false,
        forceSelection: true,
        editable: false,
        blankText: 'Please select an account from which the Courses will be available for GSMU',
        value: self.Options.settings.CanvasAccountId,
        listeners: {
            render: function (cmp) {
                if (self.Options.accounts == null) {
                    cmp.setValue(null);
                    cmp.setDisabled(true);
                    cmp.hide();
                    return;
                }
                cmp.setDisabled(false);
                accountStore.loadData(self.Options.accounts);
                cmp.show();
            }
        }
    });

    configurationItems.push({
        xtype: 'combobox',
        name: 'canvasAuthProdiverId',
        fieldLabel: 'Canvas Authentication Provider ',
        store: authProviderStore,
        displayField: 'auth_type',
        valueField: 'id',
        allowBlank: false,
        forceSelection: true,
        editable: false,
        blankText: 'Please select an authentication method to assign to user to be created in Canvas',
        value: self.Options.settings.canvasAuthProdiverId,
        listeners: {
            render: function (cmp) {
                if (self.Options.accounts == null) {
                    cmp.setValue(null);
                    cmp.setDisabled(true);
                    cmp.hide();
                    return;
                }
                cmp.setDisabled(false);
                authProviderStore.loadData(self.Options.AuthenticationProviders);
                cmp.show();
            }
        }
    });
    var canvasTabItems = [];

    canvasTabItems.push({
        layout: 'anchor',
        defaults: {
            anchor: '100%'
        },
        bodyPadding: 5,
        defaultType: 'textfield',
        xtype: 'panel',
        title: 'Configuration',
        items: configurationItems
    });

    var CanvasUniqueIdentifierFieldStore = Ext.create('Ext.data.JsonStore', {
        fields: ['fieldname', 'rawfieldname'],
        data: [
	    	{ 'fieldname': 'Default', 'rawfieldname': 'unique_id' },
            { 'fieldname': 'login_id', 'rawfieldname': 'login_id' },
            { 'fieldname': 'sis_user_id', 'rawfieldname': 'sis_user_id' },
            { 'fieldname': 'sis_login_id', 'rawfieldname': 'sis_login_id' },
	    	{ 'fieldname': 'sis_import_id', 'rawfieldname': 'sis_import_id' },
            { 'fieldname': 'primary_email', 'rawfieldname': 'primary_email' }
        ]
    });

    var UserGSMUCanvasUniqueIdentifierFieldStore = Ext.create('Ext.data.JsonStore', {
        fields: ['fieldname', 'rawfieldname'],
        data: [
	    	{ 'fieldname': 'Default', 'rawfieldname': 'username' },
            { 'fieldname': 'studregfield1', 'rawfieldname': 'studregfield1' },
            { 'fieldname': 'studregfield2', 'rawfieldname': 'studregfield2' },
            { 'fieldname': 'studregfield3', 'rawfieldname': 'studregfield3' },
	    	{ 'fieldname': 'studregfield4', 'rawfieldname': 'studregfield4' },
            { 'fieldname': 'studregfield5', 'rawfieldname': 'studregfield5' }
        ]
    });

    var CanvasCustomCourseSISIDStore = Ext.create('Ext.data.JsonStore', {
        fields: ['fieldname', 'rawfieldname'],
        data: [
	    	{ 'fieldname': 'Default - Random (System auto generated ID)', 'rawfieldname': 'random' },
            { 'fieldname': 'Partial edit (unique internal ID will be attached at the end)', 'rawfieldname': 'partialedit' },
            { 'fieldname': 'Full edit (Admin responsibility to ensure SISID uniqueness in both GSMU & Canvas)', 'rawfieldname': 'fulledit' }
        ]
    });
    var defaultInstructorCourseRoleStore = Ext.create('Ext.data.JsonStore', {
        fields: ['fieldname', 'rawfieldname'],
        data: [
	    	{ 'fieldname': 'Default', 'rawfieldname': 'TeacherEnrollment' },
            { 'fieldname': 'TA Enrollment', 'rawfieldname': 'TaEnrollment' },
            { 'fieldname': 'Observer Enrollment', 'rawfieldname': 'ObserverEnrollment' },
            { 'fieldname': 'Designer Enrollment', 'rawfieldname': 'DesignerEnrollment' },
            { 'fieldname': 'Custom Defined Role', 'rawfieldname': 'CustomRole' }
            
        ]
    });

    var CourseGradeBookFinalFieldStore = Ext.create('Ext.data.JsonStore', {
        fields: ['fieldname', 'rawfieldname'],
        data: [
            { 'fieldname': 'final_score', 'rawfieldname': 'final_score' },
            { 'fieldname': 'current_score', 'rawfieldname': 'current_score' },
            { 'fieldname': 'final_grade', 'rawfieldname': 'final_grade' },
            { 'fieldname': 'current_grade', 'rawfieldname': 'current_grade' },
            { 'fieldname': 'unposted_current_score', 'rawfieldname': 'unposted_current_score' },
            { 'fieldname': 'unposted_final_score', 'rawfieldname': 'unposted_final_score' },
            { 'fieldname': 'unposted_current_grade', 'rawfieldname': 'unposted_current_grade' },            
            { 'fieldname': 'unposted_final_grade', 'rawfieldname': 'unposted_final_grade' }
        ]
    });


    var CanvasGradeUpdateAttendanceStore = Ext.create('Ext.data.JsonStore', {
        fields: ['fieldname', 'rawfieldname'],
        data: [
            { 'fieldname': 'Yes - Take GSMU attendance when grade has meet requirement', 'rawfieldname': 'yes' },
            { 'fieldname': 'No - Take GSMU attendance when grade has meet requirement', 'rawfieldname': 'no' }
        ]
    });

    var CanvasGradeFinalizeAttendanceStore = Ext.create('Ext.data.JsonStore', {
        fields: ['fieldname', 'rawfieldname'],
        data: [
            { 'fieldname': 'Yes - Finalize course attendance on initial grade synchronization', 'rawfieldname': 'yes' },
            { 'fieldname': 'No - Finalize course attendance on initial grade synchronization', 'rawfieldname': 'no' }
        ]
    });
    var CanvasUpdateNewGradeStore = Ext.create('Ext.data.JsonStore', {
        fields: ['fieldname', 'rawfieldname'],
        data: [
            { 'fieldname': 'Yes - Update new Student Grade', 'rawfieldname': 'yes' },
            { 'fieldname': 'No - Update new Student Grade', 'rawfieldname': 'no' }
        ]
    });

    var CanvasTranscribeRegistrationStore = Ext.create('Ext.data.JsonStore', {
        fields: ['fieldname', 'rawfieldname'],
        data: [
            { 'fieldname': 'Yes - Transcribe Registration', 'rawfieldname': 'yes' },
            { 'fieldname': 'No - Transcribe Registration', 'rawfieldname': 'no' }
        ]
    });
    var CourseGradeSendCertificateStore = Ext.create('Ext.data.JsonStore', {
        fields: ['fieldname', 'rawfieldname'],
        data: [
            { 'fieldname': 'Yes', 'rawfieldname': 'yes' },
            { 'fieldname': 'No', 'rawfieldname': 'no' }
        ]
    });
    
    var ssoLoginUrl = location.protocol + '//' + location.hostname + config.getUrl('SSO/Canvas');
    var ssoReturnUrl = location.protocol + '//' + location.hostname + config.getUrl('SSO/CanvasOAuthRedirect');

    canvasTabItems.push({
        layout: 'anchor',
        defaults: {
            anchor: '100%'
        },
        bodyPadding: 5,
        defaultType: 'textfield',
        xtype: 'panel',
        title: 'Authentication / User integration',
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: 'SSO login Url',
                value: '<a href="' + ssoLoginUrl + '" target="_blank">' + ssoLoginUrl + '</a>'
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'SSO return Url',
                value: '<a href="' + ssoReturnUrl + '" target="_blank">' + ssoReturnUrl + '</a>'
            },
            {
                xtype: 'checkbox',
                name: 'enableOAuth2Authentication',
                boxLabel: 'Enable Canvas OAuth authentication via GSMU Login',
                fieldLabel: '&nbsp;',
                labelSeparator: ' ',
                checked: self.Options.settings.EnableOAuth2Authentication,
                inputValue: true
            },
            {
                xtype: 'checkbox',
                name: 'enableGSMUMasterAuthentication',
                boxLabel: 'Make GSMU as Master Login Authentication',
                fieldLabel: '&nbsp;',
                labelSeparator: ' ',
                checked: self.Options.settings.enableGSMUMasterAuthentication,
                inputValue: true
            },
            {
                xtype: 'checkbox',
                name: 'enableTeacherLoginAsStudent',
                boxLabel: 'Allow Teacher to login as Student',
                fieldLabel: '&nbsp;',
                labelSeparator: ' ',
                checked: self.Options.settings.enableTeacherLoginAsStudent,
                inputValue: true
            },
            {
                xtype: 'checkbox',
                name: 'DisableGSMUAuthIfUserInCanvas',
                boxLabel: 'Disabled GSMU Student Authentication once in Canvas',
                fieldLabel: '&nbsp;',
                labelSeparator: ' ',
                checked: self.Options.settings.DisableGSMUAuthIfUserInCanvas,
                inputValue: true
            },
            {
                xtype: 'checkbox',
                name: 'HideLoginFormIfUserInCanvas',
                boxLabel: 'Hide GSMU login form (Use canvas auth only)',
                fieldLabel: '&nbsp;',
                labelSeparator: ' ',
                checked: self.Options.settings.HideLoginFormIfUserInCanvas,
                inputValue: true
            },
            {
                xtype: 'checkbox',
                name: 'enableCanvasLtiAuhentication',
                boxLabel: 'Enable Canvas authentication via LTI Login',
                fieldLabel: '&nbsp;',
                labelSeparator: ' ',
                checked: self.Options.settings.EnableCanvasLtiAuhentication,
                inputValue: true
            },
            {
                xtype: 'checkbox',
                name: 'userSynchronizationInDashboard',
                boxLabel: 'Enable user synchronization during dashboard updates',
                fieldLabel: '&nbsp;',
                labelSeparator: ' ',
                checked: self.Options.settings.UserSynchronizationInDashboard,
                inputValue: true
            },
            {
                xtype: 'checkbox',
                name: 'exportUserAfterRegistration',
                boxLabel: 'After a user registers in GSMU, export them to Canvas',
                fieldLabel: '&nbsp;',
                labelSeparator: ' ',
                checked: self.Options.settings.ExportUserAfterRegistration,
                inputValue: true
            },
            {
                xtype: 'checkbox',
                name: 'autoMapCanvasAccount',
                boxLabel: 'Automatically map to Canvas account upon 3rd party authentication completion',
                fieldLabel: '&nbsp;',
                labelSeparator: ' ',
                checked: self.Options.settings.autoMapCanvasAccount,
                inputValue: true
            },
            {
                xtype: 'combobox',
                name: 'UserCanvasUniqueIdentifierField',
                store: CanvasUniqueIdentifierFieldStore,
                fieldLabel: 'Set Unique Identifier field used by Canvas.',
                boxLabel: 'test',
                allowBlank: false,
                labelSeparator: ' ',
                editable: false,
                displayField: 'fieldname',
                valueField: 'rawfieldname',
                queryMode: 'local',
                blankText: 'Integration will use default (unique_id) if no selection made.',
                value: self.Options.settings.UserCanvasUniqueIdentifierField,
                grow: true,
                growToLongestValue: true,
                listWidth: 200,
                width: 200
            },
            {
                xtype: 'combobox',
                name: 'UserGSMUCanvasUniqueIdentifierField',
                store: UserGSMUCanvasUniqueIdentifierFieldStore,
                fieldLabel: 'Set Unique Identifier field used by GSMU to map with Canvas Unique field.',
                boxLabel: 'test',
                allowBlank: false,
                labelSeparator: ' ',
                editable: false,
                displayField: 'fieldname',
                valueField: 'rawfieldname',
                queryMode: 'local',
                blankText: 'Integration will use default (username) if no selection made.',
                value: self.Options.settings.UserGSMUCanvasUniqueIdentifierField,
                grow: true,
                growToLongestValue: true,
                listWidth: 200,
                width: 200
            }
        ]
    });

    canvasTabItems.push({
        layout: 'anchor',
        defaults: {
            anchor: '100%'
        },
        bodyPadding: 5,
        defaultType: 'textfield',
        xtype: 'panel',
        title: 'Enrollment integration',
        items: [
            {
                xtype: 'checkbox',
                name: 'exportEnrollmentAfterCheckout',
                boxLabel: 'After the user checks out from GSMU, export the enrollment info to Canvas',
                fieldLabel: '&nbsp;',
                labelSeparator: ' ',
                checked: self.Options.settings.ExportEnrollmentAfterCheckout,
                inputValue: true
            },
            {
                xtype: 'checkbox',
                name: 'exportEnrollmentCancellation',
                boxLabel: 'When a user cancels a course in GSMU distribute the change to Canvas',
                fieldLabel: '&nbsp;',
                labelSeparator: ' ',
                checked: self.Options.settings.ExportEnrollmentCancellation,
                inputValue: true
            },
            {
                xtype: 'checkbox',
                name: 'ExportEnrollmentNotUpdateCourse',
                boxLabel: 'Do not try to sync the course during enrollment integration',
                fieldLabel: '&nbsp;',
                labelSeparator: ' ',
                checked: self.Options.settings.ExportEnrollmentNotUpdateCourse,
                inputValue: true
            },
            {
                xtype: 'checkbox',
                name: 'DisableRosterNormalizationOnExport',
                boxLabel: 'Disable Roster Normalization',
                fieldLabel: '&nbsp;',
                labelSeparator: ' ',
                checked: self.Options.settings.DisableRosterNormalizationOnExport,
                inputValue: true
            },
            {
                xtype: 'checkbox',
                name: 'allowCanvasCourseSubAccountIntegration',
                boxLabel: 'Allow GSMU Course to create under Canvas Sub-Account',
                fieldLabel: '&nbsp;',
                labelSeparator: ' ',
                checked: self.Options.settings.allowCanvasCourseSubAccountIntegration,
                inputValue: true
            },
            {
                xtype: 'checkbox',
                name: 'allowCanvasCourseSectionIntegration',
                boxLabel: 'Allow GSMU Course to sync enrollment to Canvas Course Section',
                fieldLabel: '&nbsp;',
                labelSeparator: ' ',
                checked: self.Options.settings.allowCanvasCourseSectionIntegration,
                inputValue: true
            },
            {
                xtype: 'checkbox',
                name: 'allowSupervisorIntegration',
                boxLabel: 'Allow Supervisor integrate to Canvas as Parents',
                fieldLabel: '&nbsp;',
                labelSeparator: ' ',
                checked: self.Options.settings.allowSupervisorIntegration,
                inputValue: true
            },
            {
                xtype: 'checkbox',
                name: 'allowSyncSupStudRelationIndEnrollment',
                boxLabel: 'Synchronize Supervisor as Observer of Inividual Student Enrollment',
                fieldLabel: '&nbsp;',
                labelSeparator: ' ',
                checked: self.Options.settings.allowSyncSupStudRelationIndEnrollment,
                inputValue: true
            },
            {
                xtype: 'checkbox',
                name: 'allowCourseSectionPerRegistration',
                boxLabel: 'Allow Create course section per Registration',
                fieldLabel: '&nbsp;',
                labelSeparator: ' ',
                checked: self.Options.settings.allowCourseSectionPerRegistration,
                inputValue: true
            },
            {
                xtype: 'combobox',
                name: 'allowCanvasCustomCourseSISID',
                store: CanvasCustomCourseSISIDStore,
                fieldLabel: 'Allow Admin to customize the unique Course SIS ID',
                boxLabel: 'test',
                allowBlank: false,
                labelSeparator: ' ',
                editable: false,
                displayField: 'fieldname',
                valueField: 'rawfieldname',
                queryMode: 'local',
                blankText: 'Integration will use default random ID if no selection made.',
                value: self.Options.settings.allowCanvasCustomCourseSISID,
                grow: true,
                growToLongestValue: true,
                listWidth: 200,
                width: 200
            },
            {
                xtype: 'combobox',
                name: 'defaultInstructorCourseRole',
                store: defaultInstructorCourseRoleStore,
                fieldLabel: 'Set InstructorDefault Role for Course',
                boxLabel: 'test',
                allowBlank: false,
                labelSeparator: ' ',
                editable: false,
                displayField: 'fieldname',
                valueField: 'rawfieldname',
                queryMode: 'local',
                blankText: 'If no selection, TeacherEnrollment will be selected.',
                value: self.Options.settings.defaultInstructorCourseRole,
                grow: true,
                growToLongestValue: true,
                listWidth: 200,
                width: 200
            }
        ]
    });

    canvasTabItems.push({
        layout: 'anchor',
        defaults: {
            anchor: '100%'
        },
        bodyPadding: 5,
        defaultType: 'textfield',
        xtype: 'panel',
        title: 'Grade integration',
        items: [
            {
                xtype: 'checkbox',
                name: 'enableCourseGradeIntegration',
                boxLabel: 'Enable grade extraction from Canvas course.',
                fieldLabel: '&nbsp;',
                labelSeparator: ' ',
                checked: self.Options.settings.enableCourseGradeIntegration,
                inputValue: true
            },
            {
                xtype: 'combobox',
                name: 'CourseGradeBookFinalField',
                store: CourseGradeBookFinalFieldStore,
                fieldLabel: 'Choose Canvas Final grade watch column',
                boxLabel: 'test',
                allowBlank: false,
                labelSeparator: ' ',
                editable: false,
                displayField: 'fieldname',
                valueField: 'rawfieldname',
                queryMode: 'local',
                blankText: 'Integration will retrieve grade from this field for qualification of Transcript and Certificate.',
                value: self.Options.settings.CourseGradeBookFinalField,
                grow: true,
                growToLongestValue: true,
                listWidth: 200,
                width: 200
            }, {
                name: 'GradeBookPassValue',
                fieldLabel: 'Grade watch column Letter value (pass). ie. A@B@C',
                value: self.Options.settings.GradeBookPassValue,
                allowBlank: false,
                listeners: {
                    change: function (control, value) {
                        self.Options.settings.GradeBookPassValue = value;
                    }
                }
            }, {
                xtype: 'numberfield',
                name: 'GradeBookPassPercentValue',
                fieldLabel: 'Grade watch column Percent value (pass) above',
                value: self.Options.settings.GradeBookPassPercentValue,
                allowBlank: false,
                maxValue: 100,
                minValue: 0,
                listeners: {
                    change: function (control, value) {
                        self.Options.settings.GradeBookPassPercentValue = value;
                    }
                }
            },{
                xtype: 'combobox',
                name: 'CanvasGradeUpdateAttendance',
                store: CanvasGradeUpdateAttendanceStore,
                fieldLabel: 'Take Attendance',
                boxLabel: 'test',
                allowBlank: false,
                labelSeparator: ' ',
                editable: false,
                displayField: 'fieldname',
                valueField: 'rawfieldname',
                queryMode: 'local',
                blankText: 'This will update Gosignmeup attendance base on passing grade qualification.',
                value: self.Options.settings.CanvasGradeUpdateAttendance,
                grow: true,
                growToLongestValue: true,
                listWidth: 200,
                width: 200
            }, {
                xtype: 'combobox',
                name: 'CanvasGradeFinalizeAttendance',
                store: CanvasGradeFinalizeAttendanceStore,
                fieldLabel: 'Finalize Attendance',
                boxLabel: 'test',
                allowBlank: false,
                labelSeparator: ' ',
                editable: false,
                displayField: 'fieldname',
                valueField: 'rawfieldname',
                queryMode: 'local',
                blankText: 'This will will set finalize attendance option on first synchronization.',
                value: self.Options.settings.CanvasGradeFinalizeAttendance,
                grow: true,
                growToLongestValue: true,
                listWidth: 200,
                width: 200
            }, {
                xtype: 'combobox',
                name: 'CanvasUpdateNewGrade',
                store: CanvasUpdateNewGradeStore,
                fieldLabel: 'Update new Grade',
                boxLabel: 'test',
                allowBlank: false,
                labelSeparator: ' ',
                editable: false,
                displayField: 'fieldname',
                valueField: 'rawfieldname',
                queryMode: 'local',
                blankText: 'This will update student grade if there is new change in Canvas.',
                value: self.Options.settings.CanvasUpdateNewGrade,
                grow: true,
                growToLongestValue: true,
                listWidth: 200,
                width: 200
            }, {
                xtype: 'combobox',
                name: 'CanvasTranscribeRegistration',
                store: CanvasTranscribeRegistrationStore,
                fieldLabel: 'Transcribe Roster',
                boxLabel: 'test',
                allowBlank: false,
                labelSeparator: ' ',
                editable: false,
                displayField: 'fieldname',
                valueField: 'rawfieldname',
                queryMode: 'local',
                blankText: 'This will will set transcribe the regisration.',
                value: self.Options.settings.CanvasTranscribeRegistration,
                grow: true,
                growToLongestValue: true,
                listWidth: 200,
                width: 200
            }, {
                xtype: 'combobox',
                name: 'CourseGradeSendCertificate',
                store: CourseGradeSendCertificateStore,
                fieldLabel: 'Send Certificate',
                boxLabel: 'test',
                allowBlank: false,
                labelSeparator: ' ',
                editable: false,
                displayField: 'fieldname',
                valueField: 'rawfieldname',
                queryMode: 'local',
                blankText: 'Integration will send certificate if passing grade met the requirement.',
                value: self.Options.settings.CourseGradeSendCertificate,
                grow: true,
                growToLongestValue: true,
                listWidth: 200,
                width: 200
            }, {
                xtype: 'box',
                align: 'right',
                autoEl: { tag: 'a', target: '_blank', href: '/admin/process_canvas_grade.asp', html: '[Manual synchronize Canvas Grades]' }
            },
 
        ]
    });
    if (window.LAYOUT.Options.developmentMode) {
        if (!refresh) {
            window.LAYOUT.notify('Development mode is enabled. Debug tab is shown.', true);
        }

        var accountIdString = self.Options.settings.Account == null ? ':account_id' : self.Options.settings.Account.id;

        canvasTabItems.push({
            id: 'canvas-debug-tab',
            layout: 'anchor',
            defaults: {
                anchor: '100%'
            },
            bodyPadding: 0,
            defaultType: 'textfield',
            xtype: 'panel',
            title: 'Debug',
            items: [
                {
                    xtype: 'toolbar',
                    border: 0,
                    items: [
                        {
                            xtype: 'tbfill'
                        },
                        {
                            xtype: 'button',
                            text: 'Load test configuration settings',
                            icon: config.getUrl('images/icons/famfamfam/arrow_up.png'),
                            handler: function () {
                                window.LAYOUT.MaskLayout('Setting up config');
                                Ext.Ajax.request({
                                    url: config.getUrl('adm/canvas/initializetestsettings'),
                                    success: function (response) {
                                        var config = Ext.decode(response.responseText);
                                        self.Options.settings = Ext.merge(self.Options.settings, config);
                                        window.LAYOUT.UnmaskLayout();
                                        reRenderForm();
                                    },
                                    complete: function () {
                                        config.showWarning('Error, please contact customer support.');
                                        window.LAYOUT.UnmaskLayout();
                                    }
                                });
                            }
                        }
                    ]
                }, {
                    layout: 'anchor',
                    defaults: {
                        anchor: '100%'
                    },
                    bodyPadding: 5,
                    defaultType: 'textfield',
                    xtype: 'panel',
                    items: [
                        {
                            id: 'canvas-api-request-method',
                            fieldLabel: 'HTTP Method',
                            submitValue: false,
                            xtype: 'combo',
                            store: [
                                ['get', 'GET'],
                                ['post', 'POST'],
                                ['put', 'PUT'],
                                ['delete', 'DELETE']
                            ],
                            forceSelection: true,
                            value: 'get',
                            editable: false
                        } , {
                            id: 'canvas-api-request-url',
                            fieldLabel: 'URL',
                            xtype: 'combo',
                            submitValue: false,
                            store: [
                                '/api/v1/courses',
                                '/api/v1/accounts',
                                '/api/v1/accounts/' + accountIdString + '/users',
                                '/api/v1/accounts/' + accountIdString + '/courses',
                                '/api/v1/courses/:id',
                                '/api/v1/users/:user_id/profile',
                                '/api/v1/accounts/' + accountIdString,
                                '/api/v1/accounts/' + accountIdString + '/admins',
                                '/api/v1/accounts/' + accountIdString + '/roles',
                                '/api/v1/accounts/' + accountIdString + '/logins',
                                '/api/v1/users/:user_id/enrollments',
                                '/api/v1/users/:user_id/communication_channels'
                            ],
                            listeners: {
                                change: function (cmp, value) {
                                    var button = Ext.getCmp('canvas-query-button');

                                    if (Ext.isEmpty(value)) {
                                        button.setDisabled(true);
                                    } else {
                                        button.setDisabled(false);
                                    }
                                }
                            }
                        }, {
                            id: 'canvas-api-request-query',
                            fieldLabel: 'Query',
                            xtype: 'textarea',
                            submitValue: false,
                            value: 'per_page=10&gsmu-max-pages=1'
                        }, {
                            id: 'canvas-api-request-result',
                            fieldLabel: 'Result',
                            xtype: 'textarea',
                            height: 300,
                            submitValue: false
                        }, {
                            xtype: 'fieldcontainer',
                            layout: {
                                type: 'vbox',
                                align: 'right'
                            },
                            items: [
                                {
                                    id: 'canvas-query-button',
                                    xtype: 'button',
                                    text: 'Query Canvas server',
                                    disabled: true,
                                    icon: config.getUrl('images/icons/famfamfam/server.png'),
                                    handler: function () {
                                        var method = Ext.getCmp('canvas-api-request-method').getValue();
                                        var url = Ext.getCmp('canvas-api-request-url').getValue();
                                        var query = Ext.getCmp('canvas-api-request-query').getValue();
                                        var result = Ext.getCmp('canvas-api-request-result');

                                        window.LAYOUT.MaskLayout('Communicating with Canvas server');
                                        Ext.Ajax.request({
                                            timeout: 1 /* number of hours */ * 60 /* hour */ * 60 /* minute */ * 1000 /* second */,
                                            url: config.getUrl('adm/canvas/canvasrequest?function=raw&url=' + url + '&method=' + method + '&query=' + encodeURIComponent(query)),
                                            success: function (response) {
                                                window.LAYOUT.UnmaskLayout();
                                                var json = Ext.decode(response.responseText);
                                                var display = JSON.stringify(json, null, 4);
                                                result.setValue(display);

                                            },
                                            complete: function () {
                                                config.showWarning('Error! Please contact customer service!');
                                                window.LAYOUT.UnmaskLayout();
                                            }
                                        });

                                    }
                                }
                            ]
                        }
                    ]
                }
            ]
        });
    }

    var canvasItems = [
        {
            id: 'canvas-tab',
            xtype: 'tabpanel',
            activeTab: refresh ? 'canvas-debug-tab' : 0,
            items: canvasTabItems,
            listeners: {
                tabChange: function (cmp, newTab) {
                    if (newTab.title == 'Debug') {
                        buttons.hide();
                    } else {
                        buttons.show();
                    }
                }
            }
        }
    ];

    var form = Ext.create('Ext.form.Panel', {
        renderTo: self.State.container,
        title: 'Canvas Settings',
        id: 'canvas-main-form',
        // The form will submit an AJAX request to this URL when submitted
        url: config.getUrl('adm/canvas/savesettings'),

        // Fields will be arranged vertically, stretched to full width
        layout: 'anchor',
        defaults: {
            anchor: '100%'
        },
        fieldDefaults: {
            labelWidth: 280,
            labelAlign: 'right'
        },
        defaultType: 'textfield',
        // The fields
        items: canvasItems,
        // Reset and Submit buttons
        buttons: [
            {
                id: 'canvas-settings-buttons',
                xtype: 'panel',
                defaultType: 'button',
                frame: false,
                border: 0,
                items: [
                    {
                        text: 'Reset settings',
                        icon: config.getUrl('images/icons/famfamfam/arrow_rotate_anticlockwise.png'),
                        handler: function () {
                            Ext.MessageBox.confirm('Please confirm if you are sure', 'This will reset the form to the original settings, when the page loaded. Please click Yes if you are sure you want to do this.', function (buttonId) {
                                if (buttonId == 'yes') {
                                    form.getForm().reset();
                                }
                            });

                        }
                    }, {
                        icon: config.getUrl('images/icons/famfamfam/save.png'),
                        text: 'Save settings',
                        formBind: true, //only enabled once the form is valid
                        disabled: true,
                        handler: function () {
                            saveForm();
                        }
                    }
                ]
            }
        ]
    });

    form.isValid();
    buttons = Ext.getCmp('canvas-settings-buttons');

    Ext.on('resize', function () {
        form.doLayout();
    });

    var saveForm = function (success) {
        if (form.isValid()) {

            window.LAYOUT.MaskLayout('Saving Canvas settings ...');
            form.submit({
                success: function (form, action) {
                    window.LAYOUT.UnmaskLayout();
                    self.Options.settings = Ext.merge(self.Options.settings, action.result.config);
                    self.Options.accounts = action.result.accounts;
                    if (action.result.message) {
                        window.LAYOUT.notify(action.result.message);
                    }
                    if (Ext.isFunction(success)) {
                        success();
                    }
                    reRenderForm();
                },
                failure: function (form, action) {
                    window.LAYOUT.UnmaskLayout();
                    Ext.Msg.alert('Failed', action.result.message);
                }
            });
        }
    };

    var reRenderForm = function () {
        var tab = Ext.getCmp('canvas-tab');
        var items = tab.items;
        var activeTabTitle = tab.getActiveTab().title;
        var activeIndex = 0;
        for (var index = 0; index < items.items.length; index++) {
            var currentItem = items.items[index];
            if (currentItem.title == activeTabTitle) {
                activeIndex = index;
                break;
            }
        }
        var form = Ext.getCmp('canvas-main-form');
        form.destroy();
        self.Render(true);
        tab = Ext.getCmp('canvas-tab');
        tab.setActiveTab(activeIndex);
    }
}