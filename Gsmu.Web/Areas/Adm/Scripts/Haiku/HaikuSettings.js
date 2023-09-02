function HaikuSettings(options) {

    var self = this;

    self.Options = options;
    Ext.onDocumentReady(function () {    
        self.Render();
    });
}

HaikuSettings.constructor = HaikuSettings;

HaikuSettings.prototype.Options = {
    containerId: null,
    settings: {
        HaikuAuthenticationEnabled: false,
        HaikuUrl: "",
        OAuthServiceKey: "",
        OAuthServiceSecret: "",
        OAuthRequestToken: "",
        OAuthRequestSecret: "",
        HaikuUserImportEnabled: false,
        EnableExportGoogleUser2Haiku: false,
        HaikuUserSynchronizationEnabled: false,
        EnableCourseGridButtons: false,
        EnablePortalWelcomeScreenWidget: false,
        ExportUserToHaikuAfterRegistration: false,
        ExportRosterToHaikuAfterCheckout: false,
        UseUnconfirmedEmailWhenEmailIsEmpty: true,

        SftpPort: '',
        SftpHost: '',
        SftpUsername: '',
        SftpPassword: '',
        SftpFile: '',
        SftpSshHostKeyFingerprint: '',

        HaikuUserEntityFields: [
            { "Name": "DisplayId", "Type": "String" }
        ],
        GsmuStudentTableColumns: [
            { "Name": "STUDNUM", "SqlType": "nvarchar", "Length": 500 }
        ],
        UserFieldMapping: {
            "Email": "EMAIL",
            "FirstName": "FIRST",
            "LastName": "LAST",
            "Password": "STUDNUM",
            "Enabled": "InActive",
            "Id": "haiku_user_id",
            "ImportId": "haiku_import_id"
        },
        DefaultUserFieldMapping: {
            "Email": "EMAIL",
            "FirstName": "FIRST",
            "LastName": "LAST",
            "Password": "STUDNUM",
            "Enabled": "InActive",
            "Id": "haiku_user_id",
            "ImportId": "haiku_import_id"
        }
    }
};

HaikuSettings.prototype.State = {
    container: null,
    editRecord: null
};


HaikuSettings.prototype.CreateCourseGrid = function (result, status) {
    var self = this;

    var panel = Ext.getCmp('haiku-course-panel');
    panel.removeAll();

    var store = {
        xtype: 'store',
        fields: [
            'Name',
            'Code',
            'GsmuSynchronizationStatus'
        ],
        data: result
    };

    var columns = [];

    columns.push(
    {
        text: 'Title',
        flex: 1,
        dataIndex: 'Name'
    });
    columns.push({
        text: 'Code',
        dataIndex: 'Code'
    });
    
    if (status) {
        columns.push({
            text: 'Status',
            dataIndex: 'GsmuSynchronizationStatus',
            renderer: function (value) {
                switch (value) {
                    case 0:
                        return 'Unset';

                    case 1:
                        return 'Inserted';

                    case 2:
                        return 'Updated';

                    case 3:
                        return 'Skipped';

                    case 4:
                        return 'Error';

                    default:
                        return value;

                }
            }
        });
    }

    var grid = Ext.create('Ext.grid.Panel', {
        title: 'Haiku courses',
        store: store,
        columns: columns,
        border: 0
    });
    panel.add(grid);

    Ext.getCmp('haiku-course-grid-remove-button').show();
}

HaikuSettings.prototype.CreateCourseRosterGrid = function (result) {
    var self = this;
    var rosterData = [];
    for (var classId in result.RostersByClassId) {
        var classObject = result.ClassesByClassId[classId];
        var rosters = result.RostersByClassId[classId];

        for (var rosterIndex in rosters) {
            var roster = rosters[rosterIndex];
            var user = result.UsersByUserId[roster.UserId];
            if (typeof (user) == 'undefined') {
                user = {
                    First_Name: 'MISSING USER',
                    Last_Name: 'CHECK ERRORS'
                };
            }
            var rosterDetails = {
                Role: roster.Role,
                Name: user.FirstName + ' ' + user.LastName,
                Class: classObject.Code + ' ' + classObject.Name
            };
            rosterData.push(rosterDetails);
        }
    }

    var panel = Ext.getCmp('haiku-course-roster-panel');
    panel.removeAll();

    var store = {
        xtype: 'store',
        groupField: 'Class',
        fields: [
            'Name',
            'Role',
            'Class'
        ],
        data: rosterData
    };

    var grid = Ext.create('Ext.grid.Panel', {
        border: 0,
        title: 'Haiku courses ' + Terminology.lower('enrollment') + ' information',
        store: store,
        features: [
            {
                ftype: 'groupingsummary',
                startCollapsed: true,
                enableGroupingMenu: false
            }
        ],
        columns: [
            {
                text: 'Name',
                flex: 1,
                dataIndex: 'Name'
            }, {
                text: 'Role',
                dataIndex: 'Role',
                width: 120,
                summaryType: 'count',
                summaryRenderer: function (value) {
                    return Terminology.capital('enrollment') + ' count: ' + value;
                }
            }
        ]
    });
    panel.add(grid);


    if (result.ContainsErrors) {
        window.LAYOUT.notify('There were errors, please check the bottom of the panel.');
        var errors = self.CreateErrorComponent(result.Errors);
        panel.add(errors);
    }

    Ext.getCmp('haiku-course-roster-grid-remove-button').show();

}

HaikuSettings.prototype.CreateErrorComponent = function (errors) {
    var self = this;

    var errorData = [];
    for (var index in errors) {
        var error = errors[index];
        error.date = ModelHelper.ConvertDotNetDate(index);
        errorData.push(error);
    }

    var store = Ext.create('Ext.data.Store', {
        fields: [
            'date',
            'Message'
        ],
        data: errorData
    });

    var template = new Ext.XTemplate(
        '<tpl for=".">',
            '<div class="error-selector">',
              '<strong>{date:date("m/d/Y g:i A")}:</strong> {Message}',
            '</div>',
        '</tpl>'
    );

    var cmp = Ext.create('Ext.view.View', {
        store: store,
        border: 1,
        tpl: template,
        itemSelector: 'div.error-selector',
        emptyText: 'No errors'
    });

    return cmp;
}

HaikuSettings.prototype.Render = function () {
    var self = this;

    self.State.container = Ext.get(self.Options.containerId);

    var buttons = null;

    var haikuFieldStore = Ext.create('Ext.data.Store', {
        data: self.Options.settings.HaikuUserEntityFields,
        fields: [
            'Name', 'Type'
        ],
        sorters: ['Name']
    });

    var stringFields = ['district', 'school', 'grade'];
    var gsmuStudentTableData = Ext.clone(self.Options.settings.GsmuStudentTableColumns);
    for (var index = 0; index < gsmuStudentTableData.length; index++) {
        var record = gsmuStudentTableData[index];
        var fieldName = record.Name.toLowerCase();
        record.Label = record.Name;
        if (stringFields.indexOf(fieldName) > -1) {
            record.Type = 'string';
        }
        record.HasLabel = false;
        var fieldLabel = self.Options.settings.GsmuStudentTableFieldMapLowercase[fieldName];
        if (typeof (fieldLabel) != 'undefined' && fieldLabel != null && fieldLabel != '') {
            record.Label = fieldLabel;
            record.HasLabel = true;
        }
    }

    var gsmuFieldStore = Ext.create('Ext.data.Store', {
        xtype: 'store',
        data: gsmuStudentTableData,
        fields: [
            'Name', 'Type', 'Length', 'Label', 'HasLabel'
        ],
        sorters: ['Name']
    });


    var comboStore = Ext.create('Ext.data.Store', {
        xtype: 'store',
        data: gsmuStudentTableData,
        fields: [
            'Name', 'Type', 'Length', 'Label', 'HasLabel'
        ],
        sorters: [{
            property: 'HasLabel',
            direction: 'DESC'
        }, {
            property: 'Label',
            direction: 'ASC'
        }],
        listeners: {
            load: function (store, records, successful, operation, options) {
            }
        }
    });
    var emptyComboValue = '---------- Not Set ----------';
    comboStore.add({
        Name: emptyComboValue,
        Label: emptyComboValue,
        HasLabel: true
    })

    var getGsmuRecord = function (haikuField) {
        var connectedField = self.Options.settings.UserFieldMapping[haikuField];
        if (typeof (connectedField) != 'undefined') {
            var gsmuRecordIndex = gsmuFieldStore.find("Name", connectedField);
            var gsmuRecord = gsmuFieldStore.getAt(gsmuRecordIndex);
            return gsmuRecord;
        }
        return null;
    };

    var createUserFieldMappingString = function() {
        var result = '';
        var divider = '';

        var keys = Object.keys(self.Options.settings.UserFieldMapping);
        for (var index = 0; index < keys.length ; index++) {
            var key = keys[index];
            result += divider + key + '=' + self.Options.settings.UserFieldMapping[key];
            divider = '|';
        }
        return result;
    };

    var helpText = '<ul>';
    helpText += '<li><strong>To change the the Connected Gsmu Student Field, click that field and choose the appropriate field.</strong><br/>&nbsp;</li>';
    helpText += '<li><strong>The following GSMU Student fields are of special use and have built in association, which means when :</strong><br/> ' + self.Options.settings.ReservedGsmuFields.join(', ') + '<br/>&nbsp;</li>';
    helpText += '<li><strong>The following Haiku fields are of special use and have built in association:</strong><br/> ' + self.Options.settings.ReservedHaikuFields.join(', ') + '<br/>&nbsp;</li>';
    helpText += '<li><strong>The following Haiku fields are required to be associated with a GSMU field:</strong><br/> ' + self.Options.settings.RequiredHaikuUserFields.join(', ') + '<br/>&nbsp;</li>';

    var convertMapToString = function (map) {
        var result = '';
        var keys = Object.keys(map);
        var divider = '';
        for (var index = 0; index < keys.length; index++) {
            var key = keys[index];
            var value = map[key];
            result += divider + key + ' = ' + value;
            divider = ', ';
        };
        return result;
    };
    var reservedUserFieldMapping = convertMapToString(self.Options.settings.ReservedUserFieldMapping);
    var defaultUserFieldMapping = convertMapToString(self.Options.settings.DefaultUserFieldMapping);

    helpText += '<li><strong>The following mapping is reserved and if it is overwritten during export/import, the system will make sure to use the required ones and leave the one out which in the current process is not acceptible (import/export - Haiku = GSMU):</strong><br/> ' + reservedUserFieldMapping + '<br/>&nbsp;</li>';
    helpText += '<li><strong>The defailt user mapping that is reset when you click the restore default mapping button:</strong><br/> ' + defaultUserFieldMapping + '<br/>&nbsp;</li>';
    helpText += '</ul>';
    
    var css = Ext.util.CSS.createStyleSheet('.x-tool-restore-mapping {  background-image: url(' + config.getUrl('images/icons/famfamfam/arrow_undo.png') + ') !important; }', 'haiku-css');

    var form = Ext.create('Ext.form.Panel', {
        renderTo: self.State.container,
        title: 'Haiku settings',

        // The form will submit an AJAX request to this URL when submitted
        url: config.getUrl('adm/haiku/savesettings'),

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
        items: [
            {
                xtype: 'tabpanel',
                items: [
                    {
                        layout: 'anchor',
                        defaults: {
                            anchor: '100%'
                        },
                        bodyPadding: 5,
                        defaultType: 'textfield',
                        xtype: 'panel',
                        title: 'API',
                        items: [
                            {
                                name: 'haikuUrl',
                                fieldLabel: 'Haiku Root Site Url',
                                value: self.Options.settings.HaikuUrl,
                                allowBlank: false,
                                listeners: {
                                    change: function(control, value) {
                                        self.Options.settings.HaikuUrl = value;
                                    }
                                }
                            },
                            {
                                name: 'oAuthServiceKey',
                                fieldLabel: 'Haiku OAuth Service Key',
                                value: self.Options.settings.OAuthServiceKey
                            },
                            {
                                name: 'oAuthServiceSecret',
                                fieldLabel: 'Haiku OAuth Service Secret',
                                value: self.Options.settings.OAuthServiceSecret
                            },
                            {
                                name: 'oAuthRequestToken',
                                fieldLabel: 'Haiku OAuth Request Token',
                                value: self.Options.settings.OAuthRequestToken
                            },
                            {
                                name: 'oAuthRequestSecret',
                                fieldLabel: 'Haiku OAuth Request Secret',
                                value: self.Options.settings.OAuthRequestSecret
                            },
                            {
                                xtype: 'checkbox',
                                name: 'enableCourseGridButtons',
                                boxLabel: 'Turn on course grid buttons (export course and ' + Terminology.lower('enrollment') + ')',
                                fieldLabel: '&nbsp;',
                                labelSeparator: ' ',
                                checked: self.Options.settings.EnableCourseGridButtons,
                                inputValue: true
                            },
                            {
                                xtype: 'checkbox',
                                name: 'enablePortalWelcomeScreenWidget',
                                boxLabel: 'Turn on the Haiku portal screen widget',
                                fieldLabel: '&nbsp;',
                                labelSeparator: ' ',
                                checked: self.Options.settings.EnablePortalWelcomeScreenWidget,
                                inputValue: true
                            }
                        ]
                    },
                    {
                        defaults: {
                            anchor: '100%'
                        },
                        bodyPadding: 0,
                        defaultType: 'textfield',
                        xtype: 'panel',
                        title: 'Users',
                        layout:     {
                            type: 'accordion',
                            titleCollapse: true,
                            animate: true,
                            activeOnTop: false
                        },
                        items: [
                            {
                                xtype: 'panel',
                                icon: config.getUrl('images/icons/famfamfam/wrench.png'),
                                title: '<strong>Settings</strong>',
                                items: [
                                    {
                                        xtype: 'checkbox',
                                        name: 'haikuAuthenticationEnabled',
                                        fieldLabel: 'Turn on Haiku authentication',
                                        boxLabel: '<img style="vertical-align: bottom;" src="' + config.getUrl('images/icons/famfamfam/information.png') + '">&nbsp;The login box will try authenticating in Haiku if the user is not present in GSMU.',
                                        checked: self.Options.settings.HaikuAuthenticationEnabled,
                                        inputValue: true
                                    },
                                    {
                                        xtype: 'checkbox',
                                        name: 'haikuUserImportEnabled',
                                        fieldLabel: 'Turn on Haiku user import during authentication',
                                        boxLabel: '<img style="vertical-align: bottom;" src="' + config.getUrl('images/icons/famfamfam/information.png') + '">&nbsp;New users will be imported during login if this is enabled.',
                                        checked: self.Options.settings.HaikuUserImportEnabled,
                                        inputValue: true
                                    },
                                    {
                                        xtype: 'checkbox',
                                        name: 'haikuUserSynchronizationEnabled',
                                        fieldLabel: 'Turn on Haiku user sycnhronization',
                                        boxLabel: '<img style="vertical-align: bottom;" src="' + config.getUrl('images/icons/famfamfam/information.png') + '">&nbsp;Will synchronize during user dashboard updates.',
                                        checked: self.Options.settings.HaikuUserSynchronizationEnabled,
                                        inputValue: true
                                    },
                                    {
                                        xtype: 'checkbox',
                                        name: 'EnableExportGoogleUser2Haiku',
                                        fieldLabel: 'Export user to Haiku after authenticated by Google',
                                        checked: self.Options.settings.EnableExportGoogleUser2Haiku,
                                        inputValue: true
                                    },
                                    {
                                        xtype: 'checkbox',
                                        name: 'exportUserToHaikuAfterRegistration',
                                        fieldLabel: 'Export user to Haiku after registration',
                                        checked: self.Options.settings.ExportUserToHaikuAfterRegistration,
                                        inputValue: true
                                    },
                                    {
                                        xtype: 'checkbox',
                                        name: 'useUnconfirmedEmailWhenEmailIsEmpty',
                                        fieldLabel: 'Use unconfirmed e-mail address',
                                        boxLabel: '<img style="vertical-align: bottom;" src="' + config.getUrl('images/icons/famfamfam/information.png') + '">&nbsp;If the user\'s e-mail field is empty, the unconfirmed e-mail will be used.',
                                        checked: self.Options.settings.UseUnconfirmedEmailWhenEmailIsEmpty,
                                        inputValue: true
                                    },
                                    {
                                        xtype: 'hidden',
                                        id: 'haiku-userfield-mapping',
                                        name: 'userFieldMapping',
                                        value: createUserFieldMappingString(),
                                        validator: function (value) {
                                            return false;
                                        }
                                    }

                                ]
                            },
                            {
                                xtype: 'panel',
                                icon: config.getUrl('images/icons/famfamfam/database_table.png'),
                                title: '<strong>Field mapping</strong>',                
                                items: [
                                    {
                                        height: 400,
                                        xtype: 'panel',
                                        layout: 'border',
                                        items: [
                                            {
                                                tools: [
                                                    {
                                                        type: 'restore-mapping',
                                                        renderData: {
                                                            blank: config.getUrl('images/icons/famfamfam/arrow_undo.png')
                                                        },
                                                        tooltip: 'Restore default mapping',
                                                        handler: function () {
                                                            Ext.MessageBox.confirm('Please confirm if you are sure', 'This will restore the field mapping to the factory settings.', function (buttonId) {
                                                                if (buttonId == 'yes') {

                                                                    var grid = Ext.getCmp('haiku-user-mapping-grid');
                                                                    self.Options.settings.UserFieldMapping = Ext.clone(self.Options.settings.DefaultUserFieldMapping);
                                                                    grid.getView().refresh();

                                                                }
                                                            });

                                                        }
                                                    }
                                                ],
                                                region: 'center',
                                                layout: 'fit',
                                                xtype: 'grid',
                                                frame: false,
                                                border: 0,
                                                title: '<span style="font-weight: normal;">Student field mapping</span>',
                                                id: 'haiku-user-mapping-grid',
                                                icon: config.getUrl('images/icons/famfamfam/user.png'),
                                                listeners: {
                                                    render: function (cmp, options) {
                                                        var view = cmp.getView();
                                                        view.on('refresh', function () {
                                                            comboStore.clearFilter(true);
                                                            var keys = Object.keys(self.Options.settings.UserFieldMapping);
                                                            for (var index = 0; index < keys.length; index++) {
                                                                var key = keys[index];
                                                                var gsmuField = self.Options.settings.UserFieldMapping[key];
                                                                var filter = {
                                                                    property: 'Name',
                                                                    value: gsmuField,
                                                                    id: gsmuField,
                                                                    operator: '!='
                                                                };

                                                                comboStore.filter(filter);
                                                            }
                                                        });
                                                    }
                                                },
                                                selType: 'cellmodel',
                                                plugins: [
                                                    Ext.create('Ext.grid.plugin.CellEditing', {
                                                        clicksToEdit: 1,
                                                        listeners: {
                                                            beforeedit: function (editor, e, eopts) {
                                                                self.State.editRecord = e.record;
                                                            }
                                                        }
                                                    })
                                                ],
                                                columns: [
                                                    {
                                                        text: 'Haiku User Entity Field',
                                                        dataIndex: 'Name',
                                                        flex: 1,
                                                        editable: false
                                                    }, {
                                                        text: 'Entity Field Type',
                                                        dataIndex: 'Type',
                                                        width: 100,
                                                        editable: false
                                                    }, {
                                                        text: '<strong>Connected Gsmu Student Field</strong>',
                                                        flex: 1,
                                                        sortable: false,
                                                        editable: true,
                                                        renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {
                                                            var field = record.get("Name");
                                                            var gsmuRecord = getGsmuRecord(field);
                                                            if (gsmuRecord != null) {
                                                                var fieldName = gsmuRecord.get('Name').toLowerCase();
                                                                var label = self.Options.settings.GsmuStudentTableFieldMapLowercase[fieldName];
                                                                if (typeof (label) != 'undefined' && label != null && label != '') {
                                                                    return label;
                                                                }
                                                                return fieldName;
                                                            }
                                                            return '<span style="color: silver;">Not set</span>'
                                                        },
                                                        editor: {
                                                            xtype: 'combobox',
                                                            store: comboStore,
                                                            displayField: 'Label',
                                                            valueField: 'Name',
                                                            queryMode: 'local',
                                                            forceSelection: true,
                                                            allowBlank: true,
                                                            editable: true,
                                                            listeners: {
                                                                select: function (combo, records, options) {
                                                                    if (!Ext.isArray(records)) {
                                                                        return;
                                                                    }
                                                                    var selection = records[0];
                                                                    var value = selection.get('Name');
                                                                    var grid = Ext.getCmp('haiku-user-mapping-grid');
                                                                    var record = self.State.editRecord;
                                                                    var haikuField = record.get('Name');

                                                                    if (value == emptyComboValue) {
                                                                        delete self.Options.settings.UserFieldMapping[haikuField];
                                                                    } else {
                                                                        self.Options.settings.UserFieldMapping[haikuField] = value;
                                                                    }
                                                                    grid.getView().refresh();
                                                                }
                                                            }
                                                        }
                                                    }, {
                                                        text: 'Connected Gsmu Field Type',
                                                        sortable: false,
                                                        width: 200,
                                                        editable: false,
                                                        renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {
                                                            var field = record.get("Name");
                                                            var gsmuRecord = getGsmuRecord(field);
                                                            if (gsmuRecord != null) {
                                                                return gsmuRecord.get('Type');
                                                            }
                                                            return '<span style="color: silver;">Not set</span>'
                                                        }
                                                    }
                                                ],
                                                store: haikuFieldStore
                                            },
                                            {
                                                autoScroll: true,
                                                layout: 'fit',
                                                region: 'east',
                                                width: 300,
                                                collapsed: true,
                                                frame: false,
                                                border: 0,
                                                split: true,
                                                collapsible: true,
                                                xtype: 'panel',
                                                bodyPadding: 5,
                                                title: '<span style="font-weight: normal;">Mapping help</span>',
                                                icon: config.getUrl('images/icons/famfamfam/help.png'),
                                                html: helpText
                                            }

                                        ]
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        layout: 'anchor',
                        defaults: {
                            anchor: '100%'
                        },
                        bodyPadding: 0,
                        defaultType: 'textfield',
                        xtype: 'panel',
                        title: 'Courses',
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
                                        text: 'Get the list of Haiku courses',
                                        icon: config.getUrl('images/icons/famfamfam/arrow_down.png'),
                                        handler: function () {
                                            window.LAYOUT.MaskLayout('Listing Haiku courses');
                                            Ext.Ajax.request({
                                                timeout: 1 /* number of hours */ * 60 /* hour */ * 60 /* minute */ * 1000 /* second */,
                                                url: config.getUrl('adm/haiku/haikurequest?method=listcourses'),
                                                success: function (response) {
                                                    var result = Ext.decode(response.responseText);
                                                    self.CreateCourseGrid(result.Classes.AllRecords, false);
                                                    window.LAYOUT.UnmaskLayout();
                                                },
                                                complete: function () {
                                                    config.showWarning('Error retrieving Haiku courses, please contact customer support.');
                                                    window.LAYOUT.UnmaskLayout();
                                                }
                                            });
                                        }
                                    }, {
                                        xtype: 'button',
                                        text: 'Synchronize the Haiku courses with GSMU',
                                        icon: config.getUrl('images/icons/famfamfam/arrow_refresh.png'),
                                        handler: function () {
                                            window.LAYOUT.MaskLayout('Synchronizing Haiku courses');
                                            Ext.Ajax.request({
                                                timeout: 1 /* number of hours */ * 60 /* hour */ * 60 /* minute */ * 1000 /* second */,
                                                url: config.getUrl('adm/haiku/haikurequest?method=synchronizecourses'),
                                                success: function (response) {
                                                    var result = Ext.decode(response.responseText);
                                                    self.CreateCourseGrid(result.Classes.AllRecords, true);
                                                    window.LAYOUT.UnmaskLayout();
                                                },
                                                complete: function () {
                                                    config.showWarning('Error synchronizing Haiku courses, please contact customer support.');
                                                    window.LAYOUT.UnmaskLayout();
                                                }
                                            });
                                        }
                                    }, {
                                        xtype: 'button',
                                        text: 'Remove grid',
                                        id: 'haiku-course-grid-remove-button',
                                        icon: config.getUrl('Images/Icons/famfamfam/delete.png'),
                                        hidden: true,
                                        handler: function (button) {
                                            var panel = Ext.getCmp('haiku-course-panel');
                                            panel.removeAll();
                                            button.hide();
                                        }
                                    }
                                ]
                            }, {
                                xtype: 'panel',
                                id: 'haiku-course-panel',
                                bodyPadding: 0,
                                frame: false,
                                border: 0
                            }
                        ]
                    },
                    {
                        layout: 'anchor',
                        defaults: {
                            anchor: '100%'
                        },
                        defaultType: 'textfield',
                        xtype: 'panel',
                        title: Terminology.capital('enrollment'),
                        layout: {
                            type: 'accordion',
                            titleCollapse: true,
                            animate: true,
                            activeOnTop: false
                        },
                        items: [
                            {
                                xtype: 'panel',
                                icon: config.getUrl('images/icons/famfamfam/cog.png'),
                                title: '<strong>Settings</strong>',
                                items: [
                                    {
                                        xtype: 'checkbox',
                                        name: 'exportRosterToHaikuAfterCheckout',
                                        fieldLabel: 'Export ' + Terminology.lower('enrollment') + ' to Haiku after checkout',
                                        checked: self.Options.settings.ExportRosterToHaikuAfterCheckout,
                                        inputValue: true
                                    },
                                    {
                                        xtype: 'checkbox',
                                        name: 'enableRosterCancellationSynchronization',
                                        fieldLabel: Terminology.capital('enrollment') + ' cancellation is done in Haiku as well',
                                        checked: self.Options.settings.EnableRosterCancellationSynchronization,
                                        inputValue: true
                                    },
                                    {
                                        xtype: 'checkbox',
                                        name: 'disableRosterNormalization',
                                        fieldLabel: 'Disable Roster Normalization (will leave Haiku roster alone)',
                                        checked: self.Options.settings.disableRosterNormalization,
                                        inputValue: true
                                    }
                                ]
                            },
                            {
                                xtype: 'panel',
                                title: '<strong>' + Terminology.capital('enrollment') + ' integration</strong>',
                                icon: config.getUrl('images/icons/famfamfam/door_in.png'),
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
                                            icon: config.getUrl('images/icons/famfamfam/arrow_down.png'),
                                            text: 'Get the Haiku ' + Terminology.lower('enrollment') + ' info',
                                            handler: function () {
                                                window.LAYOUT.MaskLayout('Listing Haiku ' + Terminology.lower('enrollment') + ' information');
                                                Ext.Ajax.request({
                                                    timeout: 1 /* number of hours */ * 60 /* hour */ * 60 /* minute */ * 1000 /* second */,
                                                    url: config.getUrl('adm/haiku/haikurequest?method=listcourses4roster'),
                                                    success: function (response) {
                                                        var result = Ext.decode(response.responseText);
                                                        self.CreateCourseRosterGrid(result);
                                                        window.LAYOUT.UnmaskLayout();
                                                    },
                                                    complete: function () {
                                                        config.showWarning('Error retrieving Haiku ' + Terminology.lower('enrollment') + ', please contact customer support.');
                                                        window.LAYOUT.UnmaskLayout();
                                                    }
                                                });
                                            }
                                        },
                                        {
                                            xtype: 'button',
                                            text: 'Synchronize the Haiku ' + Terminology.lower('enrollment') + ' with GSMU',
                                            icon: config.getUrl('images/icons/famfamfam/arrow_refresh.png'),
                                            handler: function () {
                                                window.LAYOUT.MaskLayout('Synchronizing Haiku ' + Terminology.lower('enrolls') + ' information');
                                                Ext.Ajax.request({
                                                    timeout: 1 /* number of hours */ * 60 /* hour */ * 60 /* minute */ * 1000 /* second */,
                                                    url: config.getUrl('adm/haiku/haikurequest?method=synchronizerosters'),
                                                    success: function (response) {
                                                        var result = Ext.decode(response.responseText);
                                                        self.CreateCourseRosterGrid(result);
                                                        window.LAYOUT.UnmaskLayout();
                                                    },
                                                    complete: function () {
                                                        config.showWarning('Error synchronizing Haiku ' + Terminology.lower('enrollment') + ', please contact customer support.');
                                                        window.LAYOUT.UnmaskLayout();
                                                    }
                                                });
                                            }
                                        }, {
                                            xtype: 'button',
                                            text: 'Remove grid',
                                            id: 'haiku-course-roster-grid-remove-button',
                                            icon: config.getUrl('Images/Icons/famfamfam/delete.png'),
                                            hidden: true,
                                            handler: function (button) {
                                                var panel = Ext.getCmp('haiku-course-roster-panel');
                                                panel.removeAll();
                                                button.hide();
                                            }
                                        }
                                    ]
                                },
                                {
                                    xtype: 'panel',
                                    id: 'haiku-course-roster-panel',
                                    bodyPadding: 0,
                                    frame: false,
                                    border: 0
                                }
                                    ]
                                }
                        ]
                    },

                    {
                        xtype: 'panel',
                        title: 'SFTP Config',
                        defaultType: 'textfield',
                        frame: false,
                        bodyPadding: 0,
                        defaults: {
                            anchor: '100%'
                        },
                        layout: {
                            type: 'accordion',
                            titleCollapse: true,
                            animate: true,
                            activeOnTop: false
                        },
                        items: [
                                    {
                                        bodyPadding: 0,
                                        xtype: 'panel',
                                        title: 'SFTP Setup',
                                        defaultType: 'textfield',
                                        icon: config.getUrl('images/icons/famfamfam/cog.png'),
                                        layout: 'anchor',
                                        defaults: {
                                            anchor: '100%'
                                        },
                                        frame: false,
                                        items: [
                                            {
                                                xtype: 'numberfield',
                                                name: 'sftpPort',
                                                fieldLabel: 'SFTP Port',
                                                value: self.Options.settings.SftpPort,
                                                allowBlank: false,
                                            },
                                            {
                                                name: 'sftpHost',
                                                fieldLabel: 'SFTP Host',
                                                value: self.Options.settings.SftpHost
                                            },
                                            {
                                                name: 'sftpUsername',
                                                fieldLabel: 'SFTP Username',
                                                value: self.Options.settings.SftpUsername
                                            },
                                            {
                                                name: 'sftpPassword',
                                                fieldLabel: 'SFTP Password',
                                                value: self.Options.settings.SftpPassword
                                            },
                                            {
                                                name: 'sftpFile',
                                                fieldLabel: 'SFTP File',
                                                value: self.Options.settings.SftpFile
                                            },
                                            {
                                                name: 'sftpSshHostKeyFingerprint',
                                                fieldLabel: 'SFTP Ssh Host Key Fingerprint',
                                                value: self.Options.settings.SftpSshHostKeyFingerprint
                                            }
                                    ]
                            },
                            {
                                xtype: 'panel',
                                icon: config.getUrl('images/icons/famfamfam/cd_add.png'),
                                title: 'SFTP CSV Import File',
                                bodyPadding: 0,
                                frame: false,
                                items: [
                                    {
                                        xtype: 'fieldcontainer',
                                        layout: 'hbox',
                                        items: [
                                            {
                                                itemId: 'sftp-file',
                                                xtype: 'filefield',
                                                name: 'csvFile',
                                                fieldLabel: 'File',
                                                layout: 'hbox',
                                                buttonConfig: {
                                                    text: 'Select',
                                                    icon: config.getUrl('Images/icons/famfamfam/find.png')
                                                },
                                            },
                                            {
                                                xtype: 'button',
                                                icon: config.getUrl('Images/icons/famfamfam/cd_add.png'),
                                                text: 'Upload Haiku SFTP CSV file',
                                                handler: function (cmp) {

                                                    var form = this.up('form').getForm();
                                                    form.submit({
                                                        url: config.getUrl('adm/haiku/HaikuCsvImport'),
                                                        waitMsg: 'Uploading your SFTP CSV ...',
                                                        success: function (form, action) {
                                                            LAYOUT.notify(action.result.message);
                                                        },
                                                        failure: function (form, action) {
                                                            LAYOUT.notify(action.result.message);
                                                        }
                                                    });
                                                }
                                            }
                                        ]
                                    },
                                    {
                                        xtype: 'fieldcontainer',
                                        fieldLabel: 'SFTP',
                                        layout: 'hbox',
                                        items: [
                                            {
                                                xtype: 'button',
                                                icon: config.getUrl('Images/icons/famfamfam/key_go.png'),
                                                text: 'Upload Haiku SFTP CSV',
                                                handler: function (cmp) {

                                                    var form = this.up('form').getForm();
                                                    form.submit({
                                                        url: config.getUrl('adm/haiku/HaikuSftp'),
                                                        waitMsg: 'Uploading your SFTP CSV ...',
                                                        success: function (form, action) {
                                                            LAYOUT.notify(action.result.message);
                                                        },
                                                        failure: function (form, action) {
                                                            LAYOUT.notify(action.result.message);
                                                        }
                                                    });
                                                }
                                            },
                                            {
                                                xtype: 'component',
                                                html: ' &nbsp; <a target="_blank" href="' + location.protocol + '//' + location.hostname + config.getUrl('adm/haiku/HaikuSftp') + '">' + location.protocol + '//' + location.hostname + config.getUrl('adm/haiku/HaikuSftp') + '</a>'
                                            }
                                        ]
                                    }
                                ],
                                frame: false,
                                layout: 'anchor',
                                defaults: {
                                    anchor: '100%'
                                }
                            }
                        ]
                    },
                    {
                    layout: 'anchor',
                    defaults: {
                        anchor: '100%'
                    },
                    bodyPadding: 5,
                    defaultType: 'textfield',
                    xtype: 'panel',
                    title: 'Debug',
                    items: [
                            {
                                id: 'haiku-request-method',
                                name: 'haiku-request-method',
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
                            },
                            {
                                id: 'haiku-request-url',
                                name: 'haiku-request-url',
                                fieldLabel: 'HTTP URL (/do/services/)',
                                xtype: 'combo',
                                submitValue: false,
                                store: [
                                    'test/ping/gosignmeup',
                                    'authentication?',
                                    'organization?',
                                    'user?',
                                    'class?',
                                    'class/:class_id/page',
                                    'class/:class_id/roster',
                                    'class/:class_id/roster/:user_id',
                                    'report?'
                                ],
                                listeners: {
                                    change: function (cmp, value) {
                                        var button = Ext.getCmp('haiku-query-button');

                                        if (Ext.isEmpty(value)) {
                                            button.setDisabled(true);
                                        } else {
                                            button.setDisabled(false);
                                        }
                                    }
                                }
                            },
                            {
                                id: 'haiku-request-query',
                                name: 'haiku-request-query',
                                fieldLabel: 'HTTP Query',                                
                                xtype: 'textarea',
                                submitValue: false,
                                value: 'import_id=0023992&first_name=Susan&last_name=Newman&nickname=Suzie&login=susan&password=abcd1234&email=susan@examplevillehs.com&user_type=T&enabled=true&display_id=S123'
                            },
                            {
                                id: 'haiku-request-result',
                                name: 'haiku-request-result',
                                fieldLabel: 'Response Result',
                                xtype: 'textarea',
                                height: 300,
                                submitValue: false
                            },
                            {
                                xtype: 'fieldcontainer',
                                layout: {
                                    type: 'vbox',
                                    align: 'right'
                                },
                                items: [
                                    {
                                        id: 'haiku-query-button',
                                        xtype: 'button',
                                        text: 'Query Haiku server',
                                        disabled: true,
                                        icon: config.getUrl('images/icons/famfamfam/server.png'),
                                        handler: function () {
                                            var method = Ext.getCmp('haiku-request-method').getValue();
                                            var url = Ext.getCmp('haiku-request-url').getValue();
                                            var debugq = Ext.getCmp('haiku-request-query').getValue();
                                            var result = Ext.getCmp('haiku-request-result');
                                            
                                            window.LAYOUT.MaskLayout('Communicating with Haiku server');
                                            Ext.Ajax.request({
                                                timeout: 1 /* number of hours */ * 60 /* hour */ * 60 /* minute */ * 1000 /* second */,
                                                url: config.getUrl('adm/haiku/haikudebugrequest?debugq=' + encodeURIComponent(debugq) + '&url=' + url + '&method=' + method + '&debugquery=' + encodeURIComponent(debugq)),
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
            }
        ],

        // Reset and Submit buttons
        buttons: [
            {
                id: 'haiku-settings-buttons',
                xtype: 'panel',
                defaultType: 'button',
                frame: false,
                border: 0,
                items: [
                    {
                        text: 'Ping Haiku service',
                        icon: config.getUrl('images/icons/famfamfam/brick_link.png'),
                        formBind: true,
                        handler: function () {
                            Ext.MessageBox.confirm('Please confirm if you are sure', 'This will only work if there are no validation errors. It will also save the current configuration back to the server. Please click Yes if you are sure you want to do this.', function (buttonId) {
                                if (buttonId == 'yes') {

                                    saveForm(function () {
                                        Ext.Ajax.request({
                                            suspendException: true,
                                            method: 'POST',
                                            url: config.getUrl('adm/haiku/haikurequest?method=ping'),
                                            success: function (response, ajax) {
                                                var result = Ext.decode(response.responseText);
                                                if (result.Status == 'ok') {
                                                    config.showInfo('Haiku Pong received, connection OK.', 'Haiku status');
                                                }
                                            },
                                            failure: function () {
                                                config.showWarning('Haiku connection error.', 'Haiku status');
                                            }
                                        });
                                    });

                                }
                            });


                        }
                    }, {
                        text: 'Reset',
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
    buttons = Ext.getCmp('haiku-settings-buttons');
    
    Ext.on('resize', function () {
        form.doLayout();
    });

    var saveForm = function (success) {
        if (form.isValid()) {

            var valid = true;
            var missingFields = '';
            var comma = '';
            for (var index = 0; index < self.Options.settings.RequiredHaikuUserFields.length; index++) {
                var key = self.Options.settings.RequiredHaikuUserFields[index];
                if (typeof(self.Options.settings.UserFieldMapping[key]) == 'undefined') {
                    valid = false;
                    if (missingFields != '') {
                        comma = ', ';
                    }
                    missingFields += comma + key;
                }
            }
            if (!valid) {
                validationText = 'Under the Users tab, Field mapping then Student field mapping area, please make sure that the following Haiku fields have association: ' + self.Options.settings.RequiredHaikuUserFields.join(', ');
                validationText += '<br/><br/>The following field' + (comma == '' ? ' is' : 's are') + ' missing: ' + missingFields;
                config.showWarning(validationText, 'Settings validation error!');
                return;
            }

            var hidden = Ext.getCmp('haiku-userfield-mapping');
            hidden.setValue(
                createUserFieldMappingString()
            );
            window.LAYOUT.MaskLayout('Saving Haiku settings ...');
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
        } else {
        }
    };
}