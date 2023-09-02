function UserSettingsComponent(options) {
    var self = this;

    self.State.Stores.EmailRestriction = Ext.create('EmailRestriction');

    Ext.onDocumentReady(function () {
        self.initialize();
    });
}


UserSettingsComponent.constructor = UserSettingsComponent;
UserSettingsComponent.prototype.ActiveId = null;
UserSettingsComponent.prototype.GobalVar = null;
UserSettingsComponent.prototype.ActiveTab = 'whitelist';

UserSettingsComponent.prototype.initialize = function (renderTo) {
    var self = this;

    Ext.Ajax.request({
        url: "/adm/UserSettings/getEmailRestrictionParam",
        success: function (response) {
            self.GobalVar = Ext.decode(response.responseText);
            self.Render();
        }
    });
}

UserSettingsComponent.prototype.Options = {
    containerId: null,
    EnableAdditionalInformation: false,
    DefaultGridOptions: {
        frame: false,
        border: 0,
        stateful: true,
        selType: 'cellmodel',
        plugins: [
            {
                ptype: 'cellediting',
                clicksToEdit: 1
            }
        ],
        viewConfig: {
            emptyText: 'The requested query returned no records.',
            deferEmptyText: false
        },
        listeners: {
            edit: function (editor, e) {
                e.record.commit();
            }
        }
    },
    Columns: {
        Membership: null,
        SortOrder: {
            xtype: 'componentcolumn',
            renderer: function (value, meta, record, rowIndex, columnIndex, store, view) {
                var dataIndex = view.ownerCt.columns[columnIndex].dataIndex;
                return {
                    xtype: 'numberfield',
                    value: record.get(dataIndex),
                    listeners: {
                        change: function (cmp, value) {
                            clearTimeout(cmp.updateTimeout);
                            cmp.updateTimeout = setTimeout(function () {
                                record.set(dataIndex, value);
                            }, 500);
                        }
                    }
                };
            }
        }
    }
}

UserSettingsComponent.prototype.State = {
    AjaxRequestCount: 0,
    Panel: null,
    Stores: {
        EmailRestriction: null
    }
}

UserSettingsComponent.prototype.Render = function () {

    var self = this;
    var store = self.State.Stores.EmailRestriction;
    self.ConstructDistrictGrid();
    self.buildSettingsTab();
    


    var UserCoursesTab = Ext.create('Ext.tab.Panel', {
        layout: 'fit',
        items: [{
            title: 'Settings' ,
            listeners: {
                activate: function () {
                    Ext.getCmp('UserSettingsPanel').setVisible(true)
                    Ext.getCmp('UserSettingsGripOption').setVisible(false)
                }
            }
        }, {
            title: 'Email Whitelist',
            listeners: {
                activate: function () {
                    Ext.getCmp('UserSettingsPanel').setVisible(false)
                    Ext.getCmp('UserSettingsGripOption').setVisible(true)
                    self.ActiveTab = 'whitelist';
                    store.load();
                    store.clearFilter(true);
                    store.filter
                        ([{ id: 'grp', property: 'grp', value: self.ActiveTab }
                    ]);
                }
            }
        }, {
            title: 'Email Blacklist',
            listeners: {
                activate: function () {
                    Ext.getCmp('UserSettingsPanel').setVisible(false)
                    Ext.getCmp('UserSettingsGripOption').setVisible(true)
                    self.ActiveTab = 'blacklist';
                    store.load();
                    store.clearFilter(true);
                    store.filter
                        ([{ id: 'grp', property: 'grp', value: self.ActiveTab }
                        ]);
                }
            }
        }
        ]
    });


    self.State.Panel = Ext.create('Ext.Panel', {
        title: 'User Settings Manager',
        renderTo: Ext.get("user-settings-component-container"),
        frame: true,
        autoScroll: false,
        title: 'User Setting Manager',
        items: [
            UserCoursesTab,
            self.settingsPanel,
            self.gridOptions
        ]
    });


    Ext.on('resize', function () {
        self.State.Panel.doLayout();
    });

    Ext.QuickTips.init();
}


UserSettingsComponent.prototype.buildSettingsTab = function () {
    var self = this;
	
    self.settingsPanel = Ext.create('Ext.form.Panel', {
        id: 'UserSettingsPanel',
        bodyPadding: 4,
        height: 400,
        items: [
            {
                id: 'UserSettingsOnOff',
                xtype: 'checkbox',
                boxLabel: 'Enable email restriction',
                checked: self.GobalVar.OnOff == 1 ? true : false,
            },
            {
                id: 'UserSettingsShowList',
                xtype: 'checkbox',
                boxLabel: 'Show whitelist and blacklist at notification',
                name: 'ShowPastOnlineCourses',
                hidden: true,
                checked: self.GobalVar.ShowList == 1 ? true : false,
            },
            {
                id: 'UserSettingsEmailNotification',
                fieldLabel: 'Email Invalid Notification',
                labelWidth: 150,
                width: 800,
                xtype: 'textfield',
                value: self.GobalVar.EmailNotification,
            },
            {
                xtype: 'button',
                text: 'Save Settings',
                handler: function () {
                    Ext.Ajax.request({
                        url: '/adm/UserSettings/setEmailRestriction',
                        params: {
                            OnOff: Ext.getCmp("UserSettingsOnOff").getValue() ? 1 : 0,
                            ShowList: Ext.getCmp("UserSettingsShowList").getValue() ? 1 : 0,
                            EmailNotification: Ext.getCmp("UserSettingsEmailNotification").getValue()
                        },
                        success: function (response) {
                            LAYOUT.notify('Successfully saved');
                        },
                        failure: function (response) {
                            alert('Failed to update.\nPlease contact your administrator.');
                        }
                    });
                }
                
            }
        ]
    });

}

UserSettingsComponent.prototype.ConstructDistrictGrid = function () {
    var self = this;

    var getGrid = function () {
        return self.gridOptions;
    };

    var getNewRecord = function () {
        return District.create({
            email: ''
        });
    }

    var pagingtoolbar = Ext.create('Ext.toolbar.Paging', {
        dock: 'bottom',
        store: self.State.Stores.EmailRestriction,
        displayInfo: true,
        displayMsg: 'Displaying {0} - {1} of {2}'
    });


    var toolbar = {
        xtype: 'toolbar',
        dock: 'top',
        height: 30,
        items: [
            self.CreateSearchField(self.State.Stores.EmailRestriction),
             '->',
            self.CreateAddButton()
        ]
    }

    self.gridOptions = Ext.create('Ext.grid.Panel', {
        //title: 'Email Whitelist',
        id: 'UserSettingsGripOption',
        store: self.State.Stores.EmailRestriction,
        dockedItems: [
            toolbar,
            pagingtoolbar
        ],
        columns: [
                {
                    header: 'Email Domain (@example.com)',
                    dataIndex: 'email',
                    flex: 1,
                    editor: {
                        xtype: 'textfield',
                        maxLength: 50,
                        allowBlank: false,


                        listeners: {
                            focus: function () {
                                var selection = self.gridOptions.getView().getSelectionModel().getSelection()[0];
                                self.ActiveId = selection.get('id');
                            },
                            blur: function (t, ev, b) {
                                Ext.Ajax.request({
                                    url: '/adm/UserSettings/UpdateEmailRestrictions',
                                    params: {
                                        id: self.ActiveId,
                                        newemail: t.lastValue
                                    },
                                    success: function (response) {
                                    },
                                    failure: function (response) {
                                        alert('Failed to update.\nPlease contact your administrator.');
                                    }
                                });

                            }

                        }

                    }


                },
                {
                    header: 'id',
                    dataIndex: 'id',
                    hidden: true
                },
                self.CreateActionColumn(self.State.Stores.EmailRestriction)
        ],
            selType: 'cellmodel',
            plugins: [
                Ext.create('Ext.grid.plugin.CellEditing', {
                    clicksToEdit: 1
                })
            ],

    });

}



UserSettingsComponent.prototype.CreatePager = function (store) {
    var self = this;

    var pageSize = Ext.create('Ext.form.field.Number', {
        minValue: 1,
        maxValue: 1000,
        value: store.pageSize,
        fieldLabel: 'Per page',
        labelAlign: 'right',
        align: 'right',
        width: 200
    });

    pageSize.on('change', function () {
        if (this.getErrors().length > 1) {
            return;
        }
        store.pageSize = this.getValue();
        store.loadPage(1);
    }, pageSize, {
        buffer: 750
    });

    return Ext.create('Ext.toolbar.Paging', {
        store: store,   // same store GridPanel is using
        dock: 'bottom',
        displayInfo: true,
        items: [pageSize]
    });
}

UserSettingsComponent.prototype.CreateCellEditor = function () {
    var self = this;

    var cellEditor = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1
    });

    return cellEditor;
}

UserSettingsComponent.prototype.CreateSearchField = function (store) {
    var self = this;

    return {
        id: 'EmaiRestriction-search-field',
        width: 300,
        xtype: 'textfield',
        emptyText: 'Keyword search ...',
        triggers: {
            clear: {
                cls: 'x-form-clear-trigger',
                handler: function () {
                    Ext.getCmp('EmaiRestriction-search-field').setValue('');
                    store.clearFilter(true);
                    store.filter([
                        {
                            property: 'grp',
                            value: self.ActiveTab
                        },

                    ]);
                }
            }
        },
        checkChangeBuffer: 1000,
        listeners: {
            change: function (cmp, value, oldValue) {
                store.clearFilter(true);
                if (value != '') {
                    store.filter([
                        {
                            property: 'email',
                            value: value
                        },
                        {
                            property: 'grp',
                            value: self.ActiveTab
                        },

                    ]);
                }
            }
        }
    };
}


UserSettingsComponent.prototype.CreateActionColumn = function (store, items, deleteCallback) {
    var self = this;

    var actionItems;
    if (!Ext.isArray(items)) {
        actionItems = [];
        if (Ext.isDefined(items)) {
            actionItems.push(items);
        }
    } else {
        actionItems = items;
    }
    actionItems.push({
        icon: config.getUrl('images/icons/famfamfam/delete.png'),
        tooltip: 'Delete',
        handler: function (grid, rowIndex, colIndex) {
            var row = grid.getStore().getAt(rowIndex);

            Ext.MessageBox.show({
                title: 'Confirm Delete',
                msg: 'If you click OK, the record you have selected will be deleted from the database.',
                buttons: Ext.MessageBox.OKCANCEL,
                icon: Ext.window.MessageBox.WARNING,
                fn: function (buttonId) {
                    if (buttonId == 'ok') {

                        Ext.Ajax.request({
                            url: '/adm/UserSettings/DestroyEmailRestrictions',
                            params: {
                                id: row.data.id
                            },
                            success: function (response) {
                                store.load();
                            },
                            failure: function (response) {
                                alert('Failed to update.\nPlease contact your administrator.');
                            }
                        });

                    }
                }
            });
        }
    });

    for (var index in actionItems) {
        var item = actionItems[index];
        if (Ext.isString(item.iconCls)) {
            item.iconCls += ' grid-icon';
        } else {
            item.iconCls = 'grid-icon';
        }
    }

    var definition = {
        xtype: 'actioncolumn',
        width: 25 * actionItems.length,
        items: actionItems
    };

    return definition;
}

UserSettingsComponent.prototype.CreateAddButton = function () {
    var self = this;
    var store = self.State.Stores.EmailRestriction;



    var definition = {
        xtype: 'button',
        icon: config.getUrl('images/icons/famfamfam/add.png'),
        text: 'Add new',
        handler: function () {


            var vlutxt = Ext.MessageBox.prompt("Email " + self.ActiveTab, 'Add new domain (@exmple.com)', function (btn, text) {
                if (btn == 'ok') {

                    Ext.Ajax.request({
                        url: '/adm/UserSettings/CreateEmailRestrictions',
                        params: {
                            grpvlu: self.ActiveTab,
                            vlu: text
                        },
                        success: function (response) {

                            store.load();
                            LAYOUT.notify('A new record has been addded and the list has been sorted so that the new record is on the top. It is ready for you to edit.');
                        },
                        failure: function (response) {
                            alert('Failed to update.\nPlease contact your administrator.');
                        }
                    });


                }
            });



        }
    };

    return definition;
}




