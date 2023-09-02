function BlackboardSettings(options) {
    var self = this;

    self.Options = Ext.merge(self.Options, options);

    Ext.onDocumentReady(function () {
        Ext.define('NodeTree', {
            idProperty: 'nodeId',
            extend: 'Ext.data.TreeModel',
            fields: [
                { name: 'nodeName', type: 'string' },
                { name: 'nodeId', type: 'string' }
            ]
        });

        self.State.dataSourcesStore = self.getDataSourcesStore()
        self.State.insitutionalHierarchyStore = self.getInstitutionalHierarchyStore();

        LAYOUT.onLayoutComplete(function () {

            LAYOUT.MaskLayout('Loading Blackboard data ...');

            var wait = function () {
                if (self.State.dataSourcesStore.isLoading() || self.State.insitutionalHierarchyStore.isLoading()) {
                    setTimeout(wait, 100);
                    return;
                }
                LAYOUT.UnmaskLayout();
                self.Render();
            }
            wait();
        });
    });
}

BlackboardSettings.constructor = BlackboardSettings;

BlackboardSettings.prototype.Options = {
    containerId: null,
    dskList: [],
    settings: {
        "BlackboardRealtimeStudentSyncEnabled": false,
        "BlacboardSsoUserIntegrationEnabled": false,
        "BlackboardSsoEnabled": false,
        "BlackboardConnectionUrl": null,
        "BlackboardInstructorRole": null,
        "BlackboardPortalSecRole": null,
        "BlackboardStudentIntegrationFields": [],
        "StudentDsk": null,
        "InstructorsDsk": null,
        "CoursesDsk": null,
        "CourseRosterDsk": null,
        "CourseInstitutionalHierarchyNodeId": "",
        "InstructorInstitutionalHierarchyNodeId": "",
        "StudentInstitutionalHierarchyNodeId": ""
    }
}

BlackboardSettings.prototype.State = {
    insitutionalHierarchyStore: null,
    dataSourcesStore: null
}

BlackboardSettings.prototype.getInstitutionalHierarchyStore = function () {
    var self = this;

    var store = Ext.create('Ext.data.TreeStore', {
        model: 'NodeTree',
        proxy: {
            type: 'ajax',
            url: config.getUrl('adm/blackboard/InstitutionalHierarchy')
        }
    });
    store.load();
    return store;
}

BlackboardSettings.prototype.getDataSourcesStore = function () {
    var self = this;

    var store = Ext.create('Ext.data.Store',
    {
        queryMode: 'local',
        xtype: 'store',
        fields: ['display', 'value'],
        proxy: {
            type: 'ajax',
            url: config.getUrl('adm/blackboard/DataSourceStore')
        },
        listeners: {
                load: function (store, records) {
                    store.insert(0, {display: '', value: ''})
                }
        }
    })
    store.load();
    return store;
}

BlackboardSettings.prototype.Render = function () {
    var self = this;


    var tabItems = [];

    var baseSettings = {
        xtype: 'combobox',
        store: self.State.dataSourcesStore,
        valueField: 'value',
        queryMode: 'local',
        displayField: 'display',
        allowBlank: true,
        editable: false,
        tpl: Ext.create('Ext.XTemplate',
            '<tpl for=".">',
                '<div class="x-boundlist-item">',
                    '<tpl if="value == \'\'"><div style="font-weight: bold;">DISABLE</div><tpl else>{display}</tpl>',
                '</div>',
            '</tpl>'
        ),
        // template for the content inside text field
        displayTpl: Ext.create('Ext.XTemplate',
            '<tpl for=".">',
                    '<tpl if="value == \'\'">-- DISABLED --<tpl else>{display}</tpl>',
            '</tpl>'
        )
    };

    var institutionalBaseSettings = {
        xtype: 'gsmutreepicker',
        store: self.State.insitutionalHierarchyStore,
        displayField: 'nodeName',
        treeConfig: {
            rootVisible: true
        }
    };


    tabItems.push({
        xtype: 'panel',
        icon: config.getUrl('images/icons/famfamfam/database_gear.png'),
        layout: 'anchor',
        defaults: {
            anchor: '100%',
        },
        bodyPadding: 5,
        defaultType: 'textfield',
        xtype: 'panel',
        title: 'Data sources',
        items: [
            Ext.merge({}, baseSettings, {
                name: 'blackboard_students_dsk',
                value: self.Options.settings.StudentDsk,
                fieldLabel: 'Student data source key'
            }),
            Ext.merge({}, baseSettings, {
                name: 'blackboard_instructors_dsk',
                fieldLabel: Terminology.capital('instructor') + ' data source key',
                value: self.Options.settings.InstructorsDsk
            }),
            Ext.merge({}, baseSettings, {
                name: 'blackboard_courses_dsk',
                fieldLabel: 'Courses data source key',
                value: self.Options.settings.CoursesDsk
            }),
            Ext.merge({}, baseSettings, {
                name: 'blackboard_course_roster_dsk',
                fieldLabel: 'Course roster data source key',
                value: self.Options.settings.CourseRosterDsk
            })
        ]
    });

    var institutionalHierarchyItems = [];

    institutionalHierarchyItems.push({
        xtype: 'displayfield',
        value: '<img src="' + config.getUrl('images/icons/famfamfam/information.png') + '" style="vertical-align:top;"/> If you choose the empty node, it means the settings is turned off during integration and no node change will be executed on the integrated object.'
    });

    institutionalHierarchyItems.push(Ext.merge({}, institutionalBaseSettings, {
        fieldLabel: 'Course intitution node',
        name: 'blackboard_courses_node_id',
        value: self.Options.settings.CourseInstitutionalHierarchyNodeId
    }));

    institutionalHierarchyItems.push(Ext.merge({}, institutionalBaseSettings, {
        fieldLabel: 'Instructor intitution node',
        name: 'blackboard_instructors_node_id',
        value: self.Options.settings.InstructorInstitutionalHierarchyNodeId
    }));

    institutionalHierarchyItems.push(Ext.merge({}, institutionalBaseSettings, {
        fieldLabel: 'Student intitution node',
        name: 'blackboard_students_node_id',
        value: self.Options.settings.StudentInstitutionalHierarchyNodeId
    }));

    tabItems.push({
        layout: 'anchor',
        defaults: {
            anchor: '100%',
        },
        bodyPadding: 5,
        xtype: 'panel',
        icon: config.getUrl('images/icons/famfamfam/chart_organisation.png'),
        title: 'Institutional Hierarchy',
        items: institutionalHierarchyItems
    });

    var form = Ext.create('Ext.form.Panel', {
        renderTo: self.Options.containerId,
        icon: config.getUrl('images/icons/famfamfam/cog.png'),
        title: 'Configuration',
        id: 'blackboard-main-form',
        url: config.getUrl('adm/blackboard/savesettings'),
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
                id: 'blackboard-tab',
                xtype: 'tabpanel',
                items: tabItems
            }
        ],
        // Reset and Submit buttons
        buttons: [
            {
                id: 'blackboard-settings-buttons',
                xtype: 'panel',
                defaultType: 'button',
                frame: false,
                border: 0,
                items: [
                    {
                        text: 'Reset settings',
                        tooltip: 'Resets the values to the previously saved values',
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
    buttons = Ext.getCmp('blackboard-settings-buttons');

    Ext.on('resize', function () {
        form.doLayout();
    });

    var saveForm = function (success) {
        if (form.isValid()) {

            window.LAYOUT.MaskLayout('Saving Blackboard settings ...');
            form.submit({
                success: function (form, action) {
                    window.LAYOUT.UnmaskLayout();
                    self.Options.settings = Ext.merge(self.Options.settings, action.result.config);
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
  
    if (location.hash.length > 1) {
        var activeTab = parseInt(location.hash.substring(1));
        Ext.getCmp('blackboard-tab').setActiveTab(activeTab);        
    }

    var reRenderForm = function () {
        var tab = Ext.getCmp('blackboard-tab');
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
        var form = Ext.getCmp('blackboard-main-form');
        form.destroy();
        self.Render(true);
        tab = Ext.getCmp('blackboard-tab');
        tab.setActiveTab(activeIndex);
    }
}
