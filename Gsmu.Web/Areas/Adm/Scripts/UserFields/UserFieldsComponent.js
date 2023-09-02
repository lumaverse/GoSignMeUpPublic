/*

REMEMBER SCHOOLS USE LOCATIONID INSTEAD OF SCHOOL ID FOR RELATIONS!

*/
function UserFieldsComponent(options) {
    var self = this;

    self.Options = Ext.merge(self.Options, options);

    Ext.Ajax.on('beforerequest', function () {
        self.State.AjaxRequestCount++;
    })
    Ext.Ajax.on('requestcomplete', function () {
        self.State.AjaxRequestCount--;
    })
    Ext.Ajax.on('requestexception', function () {
        self.State.AjaxRequestCount--;
    })

    self.State.Stores.District = Ext.create('DistrictStore');
    self.State.Stores.School = Ext.create('SchoolStore');
    self.State.Stores.Grade = Ext.create('GradeLevelStore');

    self.State.Stores.GradeToSchool.Relations = Ext.create('SchoolsGradeLevelsRelatedStore', {
        pageSize: -1,
        autoLoad: false
    });
    self.State.Stores.GradeToSchool.School = Ext.create('SchoolStore', {
        pageSize: -1,
        autoLoad: false
    });
    self.State.Stores.GradeToSchool.Grade = Ext.create('GradeLevelStore', {
        pageSize: -1,
        autoLoad: false
    });

    self.State.Stores.ExtraInfo.School = Ext.create('SchoolExtraInfoStore', {
        pageSize: 1,
        autoLoad: false
    });
    self.State.Stores.ExtraInfo.Grade = Ext.create('GradeExtraInfoStore', {
        pageSize: 1,
        autoLoad: false
    });
    self.State.Stores.ExtraInfo.District = Ext.create('DistrictExtraInfoStore', {
        pageSize: 1,
        autoLoad: false
    });

    self.State.Stores.Membership = Ext.create('Ext.data.Store', {
        autoLoad : true,
        fields: ['id', 'value'],
        data: [
                    [0, ''],
                    [1, self.Options.MembershipConfig.NonMemberLabel],
                    [2, self.Options.MembershipConfig.MemberLabel],
                    [3, self.Options.MembershipConfig.Special1Label]
        ]
    });

    self.Options.Columns.Membership = {
        xtype: 'componentcolumn',
        width: 140,
        renderer: function (value, meta, record, rowIndex, columnIndex, store, view) {
            var dataIndex = view.ownerCt.columns[columnIndex].dataIndex;
            return {
                xtype: 'combobox',
                editable: true,
                forceSelection: true,
                value: record.get(dataIndex),
                displayTpl: '<tpl for=".">{[values.field2.replace("&nbsp;", "")]}</tpl>',
                store: [
                    [0, '&nbsp;'],
                    [1, self.Options.MembershipConfig.NonMemberLabel],
                    [2, self.Options.MembershipConfig.MemberLabel],
                    [3, self.Options.MembershipConfig.Special1Label]
                ],
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
    };

    self.Options.Columns.MembershipEditor = {
        xtype: 'combobox',
        displayField: 'value',
        valueField: 'id',
        queryMode: 'local',
        editable: true,
        forceSelection: true,
        queryMode: 'remote',
        triggerAction: 'all',
        store: self.State.Stores.Membership,
        listeners: {
            select: function (cmp, records) {
                if (records == null) {
                    return;
                }
                //SEEMED TO WORK EVEN WITHOUT THIS PART
                //clearTimeout(cmp.updateTimeout);
                //cmp.updateTimeout = setTimeout(function () {
                //    record.set(dataIndex, value);
                //}, 500);
            }
        }
    }
    Ext.onDocumentReady(function () {
        self.Render();
    });
}

UserFieldsComponent.constructor = UserFieldsComponent;

UserFieldsComponent.prototype.Options = {
    containerId: null,
    EnableAdditionalInformation: false,
    Titles: {
        District: 'District',
        School: 'School',
        Grade: 'Grade',
        Membership: 'Membership'
    },
    MembershipConfig: {
        MembershipEnabled: true,
        MemberLabel: "Member2",
        NonMemberLabel: "Non Member",
        Special1Label: "Special"
    },
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
        },
        Check: {
            xtype: 'checkcolumn',
            listeners: {
                beforecheckchange: function (cmp, recordIndex, checked) {
                    var dataIndex = cmp.dataIndex;
                    var grid = cmp.ownerCt.grid;
                    var store = grid.store;
                    var record = store.getAt(recordIndex);
                    var value = checked ? 1 : 0;
                    record.set(dataIndex, value);
                    return false;
                }
            }
        }
    }
}

UserFieldsComponent.prototype.State = {
    AjaxRequestCount: 0,
    Panel: null,
    Tooltips: {
        AssignGrade: null,
        AssignSchool: null,
        DetailSchool: null,
        DetailDistrict: null,
        DetailGrade: null
    },
    Grids: {
        District: null,
        School: null,
        Grade: null
    },
    Stores: {
        District: null,
        School: null,
        Grade: null,
        ExtraInfo: {
            School: null,
            Grade: null,
            District: null
        },
        GradeToSchool: {
            Relations: null,
            School: null,
            Grade: null
        }
    }
}

UserFieldsComponent.prototype.Render = function () {

    var self = this;

    self.ConstructDistrictGrid();
    self.ConstructSchoolGrid();
    self.ConstructGradeGrid();

    var getEnableAdditionalInformationText = function() {
        var text = 'Enable additional information for records';
        if (self.Options.EnableAdditionalInformation) {
            text += '<input type="checkbox" checked="true" style=" vertical-align: -10%;">';
        } else {
            text += '<input type="checkbox" style=" vertical-align: -10%;">';
        }
        return text;
    }

    self.State.Panel = Ext.create('Ext.tab.Panel', {
        title: 'Student User fields manager',
        renderTo: self.Options.containerId,
        dockedItems: {
            xtype: 'toolbar',
            items: [
                 '->',
                {
                    xtype: 'checkbox',
                    boxLabel: 'Prevent user from editing ' + self.Options.Titles.District,
                    inputValue: '1',
                    checked: (self.Options.SetConfig.DisallowDistrictEdit == -1 ? true : false),
                    handler: function (cmp) {
                        var Value = (cmp.value ? "-1" : "0");
                        Ext.Ajax.request({
                            url: "/adm/UserFields/SaveSetting",
                            params: {
                                mifield: "DistrictEdit",
                                vlu: Value
                            },
                            success: function (response) {

                            }
                        });
                    }
                },
                '->',
                {
                    xtype: 'checkbox',
                    boxLabel: 'Prevent user from editing ' + self.Options.Titles.School,
                    inputValue: '1',
                    checked: (self.Options.SetConfig.DisallowSchoolEdit == -1 ? true : false),
                    handler: function (cmp) {
                        var Value = (cmp.value ? "-1" : "0");
                        Ext.Ajax.request({
                            url: "/adm/UserFields/SaveSetting",
                            params: {
                                mifield: "SchoolEdit",
                                vlu: Value
                            },
                            success: function (response) {

                            }
                        });
                    }
                },
                '->',
                {
                    xtype: 'checkbox',
                    boxLabel: 'Prevent user from editing ' + self.Options.Titles.Grade,
                    inputValue: '1',
                    checked: (self.Options.SetConfig.DisallowGradeEdit == -1 ? true : false),
                    handler: function (cmp) {
                        var Value = (cmp.value ? "-1" : "0");
                        Ext.Ajax.request({
                            url: "/adm/UserFields/SaveSetting",
                            params: {
                                mifield: "GradeEdit",
                                vlu: Value
                            },
                            success: function (response) {

                            }
                        });
                    }
                },
                '->',
                {
                    xtype: 'button',
                    icon: config.getUrl('images/icons/famfamfam/application_view_detail.png'),
                    text: getEnableAdditionalInformationText(),
                    enableToggle: true,
                    pressed: self.Options.EnableAdditionalInformation,
                    listeners: {
                        render: function (cmp) {
                            var id = 'enable-addition-info-tip';
                            var previous = Ext.getCmp(id);
                            if (previous) {
                                previous.destroy();
                            }

                            var tip = Ext.create('Ext.tip.ToolTip', {
                                id: id,
                                target: cmp.getEl(),
                                anchor: 'left',
                                html: 'You can assign additional detail info about the items you currently see when this option is enabled.'
                            });
                        }
                    },
                    handler: function (cmp) {

                        LAYOUT.MaskLayout('Processing ...');
                        self.Options.EnableAdditionalInformation = !self.Options.EnableAdditionalInformation;
                        var buttonText = getEnableAdditionalInformationText();
                        cmp.setText('');
                        cmp.setText(buttonText);

                        var setup = function (grid) {
                            grid.columns[grid.columns.length - 2].setVisible(self.Options.EnableAdditionalInformation);
                            grid.columns[grid.columns.length - 3].hideable = self.Options.EnableAdditionalInformation;
                            grid.columns[grid.columns.length - 3].setVisible(self.Options.EnableAdditionalInformation);
                        }
                        setup(self.State.Grids.District);
                        setup(self.State.Grids.School);
                        setup(self.State.Grids.Grade);

                        LAYOUT.UnmaskLayout();
                        LAYOUT.setMasterinfoValue(3, 'RegDDExtraInfo', self.Options.EnableAdditionalInformation ? 1 : 0, function () {
                            self.State.Panel.doLayout();
                        });
                    }
                }
            ]
        },
        stateful: true,
        stateId: 'userfields-component-tab',
        stateEvents: ['tabchange'],
        getState: function () {
            return {
                activeTab: this.items.findIndex('id', this.getActiveTab().id)
            };
        },
        applyState: function (s) {
            this.setActiveTab(s.activeTab);
        },
        items: [
            self.State.Grids.District,
            self.State.Grids.School,
            self.State.Grids.Grade
        ],
        listeners: {
            tabchange: function (panel, newCard, oldCard) {
                var view = newCard.getView();
                if (Ext.isFunction(view.refresh)) {
                    view.refresh();
                }
            }
        }
    });


    Ext.on('resize', function () {
        self.State.Panel.doLayout();
    });

    Ext.QuickTips.init();
}

UserFieldsComponent.prototype.ConstructDistrictGrid = function () {
    var self = this;

    var getGrid = function () {
        return self.State.Grids.District;
    };

    var getNewRecord = function () {
        return District.create({
            DISTRICT1: ''
        });
    }

    var titleCallback = function (value) {
        self.Options.Titles.District = value;
        var grid = self.State.Grids.School;
        var columns = grid.columns;
        columns[4].setText(value);
        if (self.State.Tooltips.DetailDistrict != null) {
            self.State.Tooltips.DetailDistrict.setHtml(self.getDetailTitle(value));
        }
    }

    var toolbar = {
        xtype: 'toolbar',
        dock: 'top',
        height: 30,
        items: [
            self.CreateSearchField('district', self.State.Stores.District, 'DISTRICT1'),
            '-',
            self.CreateTitleField('district', self.Options.Titles.District, getGrid, 'Field3Name', titleCallback),
            '->',
            self.CreateExportButton(getGrid, 'District'),
            '-',
            self.CreateAddButton(getGrid, self.State.Stores.District, getNewRecord, 'DISTID')
        ]
    }

    var gridOptions = Ext.merge(self.Options.DefaultGridOptions, {
        title: self.Options.Titles.District,
        hidden: self.Options.Titles.District == null ? true : false,
        store: self.State.Stores.District,
        dockedItems: [
            toolbar,
            self.CreatePager(self.State.Stores.District)
        ],
        columns: [
                {
                    header: self.Options.Titles.District,
                    dataIndex: 'DISTRICT1',
                    flex: 1,
                    editor: {
                        xtype: 'textfield',
                        maxLength: 50,
                        allowBlank: false
                    }
                },
                {
                    header: 'ID',
                    dataIndex: 'DISTID',
                    hidden: true
                },
                Ext.merge({
                    header: 'Sort order',
                    dataIndex: 'SortOrder'
                }, self.Options.Columns.SortOrder),
                Ext.merge({
                    width: 130,
                    header: 'Hide in public area',
                    dataIndex: 'HideInPublicArea'
                }, self.Options.Columns.Check),
                Ext.merge({
                    width: 140,
                    header: '<div style="float: right; margin-right: 10px;" data-qtitle="Explanatory information" data-qtip="Means students will not be charged sales tax or shipping."><img style="vertical-align: bottom;" src="' + config.getUrl('Images/icons/famfamfam/information.png') + '"/></div>' +
                        'No tax or shipping',
                    exportHeader: 'No tax or shipping',
                    dataIndex: 'NoTaxShipping'
                }, self.Options.Columns.Check),
                Ext.merge({
                    width: 175,
                    header: 'Alternate confirmation e-mail',
                    dataIndex: 'AltEmailConfirmation'
                }, self.Options.Columns.Check),
                Ext.merge({
                    header: '<div style="float: right; margin-right: 10px;" data-qtitle="Explanatory information" data-qtip="On Add or Edit of a person, only if they select this item, they will be flagged as having a ' + self.Options.Titles.Membership + ' type that is selected in the pull down next to the item below and not be allowed to overrule it.<br/></br>This choice can be overridden automatically by the Force option in the ' + self.Options.Titles.School + ' area of the system config.  They are still allowed to choose to be the other ' + self.Options.Titles.Membership + ' types even if this pulldown below is not selected"><img style="vertical-align: bottom;" src="' + config.getUrl('Images/icons/famfamfam/information.png') + '"/></div>' +
                        'Force ' + self.Options.Titles.Membership,
                    exportHeader: 'Force ' + self.Options.Titles.Membership,
                    dataIndex: 'MembershipFlag',
                    renderer: function (value, meta, record, rowIndex, columnIndex, store, view)
                    {
                        if (value == 0) return "";
                        return self.State.Stores.Membership.findRecord('id', value).data.value;
                    },
                    editor: self.Options.Columns.MembershipEditor
                }
                //, self.Options.Columns.Membership
                ),
                {
                    text: 'Additional information',
                    dataIndex: 'DISTID',
                    width: 120,
                    hidden: !self.Options.EnableAdditionalInformation,
                    hideable: self.Options.EnableAdditionalInformation,
                    renderer: function (value, metadata, record, rowIndex, colIndex, store, view) {

                        var id = 'district-additional-info-' + value;

                        setTimeout(function () {
                            var el = Ext.get(id);
                            if (el == null) {
                                return;
                            }
                            el.on('click', function () {
                                self.DistrictDetailsWindow(record);
                            });
                            self.LoadDistrictDetailsColumn(record);
                        }, 100);
                        return '<div id="' + id + '" style="cursor: pointer; text-overflow: ellipsis;"></div>';
                    }
                } ,
                {
                    // show/hide is managed by enable additional information button handler
                    hidden: !self.Options.EnableAdditionalInformation,
                    hideable: false,
                    xtype: 'actioncolumn',
                    width: 25,
                    iconCls: 'grid-icon assign-district-details',
                    items: [
                        {
                            icon: config.getUrl('images/icons/famfamfam/application_view_detail.png'),
                            handler: function (grid, rowIndex, colIndex) {
                                var districtRecord = grid.getStore().getAt(rowIndex);
                                self.DistrictDetailsWindow(districtRecord);
                            }
                        }
                    ]
                },
                self.CreateActionColumn(self.State.Stores.District)
        ],
        listeners: {
            render: function (grid) {
                self.State.Tooltips.DetailDistrict = Ext.create('Ext.tip.ToolTip', {
                    target: grid.getEl(),
                    delegate: '.assign-district-details',
                    anchor: 'left',
                    html: self.getDetailTitle(self.Options.Titles.District)
                });
            }
        }

    });

    self.State.Grids.District = Ext.create('Ext.grid.Panel', gridOptions);
}

UserFieldsComponent.prototype.ConstructSchoolGrid = function () {

    //LOAD STORE

    var districtStore = Ext.create('DistrictStore', {
        autoLoad: true,
        pageSize: -1,
        autoSync: true
    });
    districtStore.load();
    districtStore.on('load', function (store, records) {
        store.insert(0, [{
            DISTRICT1: '',
            DISTID: 0
        }]);
    });



    var self = this;

    var getGrid = function () {
        return self.State.Grids.School;
    };

    var getNewRecord = function () {
        return School.create({
            LOCATION: ''
        });
    }

    var titleCallback = function (value) {
        var grid = self.State.Grids.Grade;
        var columns = grid.columns;
        columns[3].setText(value);

        self.Options.Titles.School = value;
        if (self.State.Tooltips.AssignSchool != null) {
            self.State.Tooltips.AssignSchool.setHtml(self.getAssignTitle(value));
        }
        if (self.State.Tooltips.DetailSchool != null) {
            self.State.Tooltips.DetailSchool.setHtml(self.getDetailTitle(value));
        }

    }

    var toolbar = {
        xtype: 'toolbar',
        dock: 'top',
        height: 30,
        items: [
            self.CreateSearchField('school', self.State.Stores.School, 'LOCATION'),
            '-',
            self.CreateTitleField('school', self.Options.Titles.School, getGrid, 'Field2Name', titleCallback),
            '->',
            self.CreateExportButton(getGrid, 'School'),
            '-',
            self.CreateAddButton(getGrid, self.State.Stores.School, getNewRecord, 'ID')
        ]
    }

    //var districtField = {
    //    dataIndex: 'District',
    //    header: 'District',
    //    xtype: 'componentcolumn',
    //    width: 200,
    //    renderer: function (value, meta, record, rowIndex, columnIndex, store, view) {
    //        var dataIndex = view.ownerCt.columns[columnIndex].dataIndex;
    //        return {
    //            displayTpl: '<tpl for=".">{[values.DISTRICT1.replace("&nbsp;", "")]}</tpl>',
    //            xtype: 'combobox',
    //            editable: true,
    //            forceSelection: true,
    //            queryMode: 'remote',
    //            displayField: 'DISTRICT1',
    //            triggerAction: 'all',
    //            valueField: 'DISTID',
    //            value: record.get(dataIndex),
    //            store: districtStore,
    //            listeners: {
    //                change : function(){},
    //                //select: function (cmp, records) {
    //                //    if (records == null) {
    //                //        return;
    //                //    }
    //                //    clearTimeout(cmp.recordUpdate);
    //                //    cmp.recordUpdate = setTimeout(function () {
    //                //        record.set(dataIndex, records[0].get('DISTID'));

    //                //        /// this is because the componentcolumn is not extjs plugin and has issues
    //                //        districtStore.clearFilter();
    //                //    }, 1000);
    //                //}
    //            }
    //        };
    //    }
    //};

    var gridOptions = Ext.merge(self.Options.DefaultGridOptions, {
        //id:'UserFieldsComponentsGrid',
        title: self.Options.Titles.School,
        hidden: self.Options.Titles.School == null ? true : false,
        store: self.State.Stores.School,
        dockedItems: [
            toolbar,
            self.CreatePager(self.State.Stores.School)
        ],
        columns: [
            {
                dataIndex: 'LOCATION',
                header: self.Options.Titles.School,
                flex: 1,
                editor: {
                    xtype: 'textfield',
                    maxLength: 50,
                    allowBlank: false
                }
            },
            {
                dataIndex: 'ID',
                header: 'ID',
                hidden: true
            },
            {
                dataIndex: 'locationid',
                header: 'Location ID',
                hidden: true
            },
            {
                dataIndex: 'URL',
                header: 'URL',
                width: 200,
                renderer: function(value) {
                    return '<a href="' + encodeURI(value) +'" target="_blank">' + value + '</a>';
                },
                editor: {
                    xtype: 'textfield',
                    maxLength: 255,
                    allowBlank: true,
                    vtype: 'url'
                }
            },
            {
                dataIndex: 'District',
                header: 'District',
                width: 200,
                renderer: function (value, meta, record, rowIndex, columnIndex, store, view)
                {
                    return districtStore.findRecord('DISTID', value).data.DISTRICT1;
                },
                editor: {
                    xtype: 'combobox',
                    displayField: 'name',
                    valueField: 'type',
                    queryMode: 'local',
                    editable: true,
                    forceSelection: true,
                    queryMode: 'remote',
                    displayField: 'DISTRICT1',
                    triggerAction: 'all',
                    valueField: 'DISTID',
                    store: districtStore,
                    listeners: {
                        select: function (cmp, records) {
                            if (records == null) {
                                return;
                            }
                            clearTimeout(cmp.recordUpdate);
                            cmp.recordUpdate = setTimeout(function () {
                                records.set("DISTID", records.data.DISTID);
                                /// this is because the componentcolumn is not extjs plugin and has issues
                                districtStore.clearFilter();
                            }, 1000);
                        }
                    }
                }
            },
            Ext.merge({
                header: 'Sort order',
                dataIndex: 'SortOrder'
            }, self.Options.Columns.SortOrder),
            Ext.merge({
                header: 'Force ' + self.Options.Titles.Membership,
                dataIndex: 'MembershipFlag',
                renderer: function (value, meta, record, rowIndex, columnIndex, store, view)
                {
                    if (value == 0) return "";
                    return self.State.Stores.Membership.findRecord('id', value).data.value;
                },
                editor : self.Options.Columns.MembershipEditor
            }
            //, self.Options.Columns.Membership
            ),
            {
                dataIndex: 'LocationAlias',
                header: 'Location alias for LDAP',
                width: 130,
                editor: {
                    xtype: 'textfield',
                    maxLength: 50,
                    allowBlank: true
                }
            },
            {
                dataIndex: 'certid',
                header: 'Certification',
                hidden: true
            },
            {
                header: 'Active Students',
                align: 'right',
                sortable: false,
                renderer: function(value, metaData, record, rowIndex, colIndex, store, view) {
                    return record.raw.ActiveStudents;
                }
            },
            {
                text: 'Additional information',
                dataIndex: 'locationid',
                width: 120,
                hidden: !self.Options.EnableAdditionalInformation,
                hideable: self.Options.EnableAdditionalInformation,
                renderer: function (value, metadata, record, rowIndex, colIndex, store, view) {
                    var id = 'school-additional-info-' + value;

                    setTimeout(function () {
                        var el = Ext.get(id);
                        if (el == null) {
                            return;
                        }
                        el.on('click', function () {
                            self.SchoolDetailsWindow(record);
                        });
                        self.LoadSchoolDetailsColumn(record);
                    }, 100);
                    return '<div id="' + id + '" style="cursor: pointer;"></div>';
                }
            },
            {
                // show/hide is managed by enable additional information button handler
                hidden: !self.Options.EnableAdditionalInformation,
                hideable: false,
                xtype: 'actioncolumn',
                width: 25,
                iconCls: 'grid-icon assign-school-details',
                items: [
                    {
                        icon: config.getUrl('images/icons/famfamfam/application_view_detail.png'),
                        handler: function (grid, rowIndex, colIndex) {
                            var schoolRecord = grid.getStore().getAt(rowIndex);
                            self.SchoolDetailsWindow(schoolRecord);
                        }
                    }
                ]
            },
            self.CreateActionColumn(self.State.Stores.School, {
                icon: config.getUrl('images/icons/famfamfam/chart_bar.png'),
                iconCls: 'assign-grade',
                handler: function(grid, rowIndex, colIndex) {
                    var schoolRecord = grid.getStore().getAt(rowIndex);
                    self.AssignSchoolWindow(schoolRecord);
                }
            })
        ],
        listeners: {
            render: function (grid) {
                self.State.Tooltips.AssignGrade = Ext.create('Ext.tip.ToolTip', {
                    target: grid.getEl(),
                    delegate: '.assign-grade',
                    anchor: 'left',
                    html: self.getAssignTitle(self.Options.Titles.Grade)
                });

                self.State.Tooltips.DetailSchool = Ext.create('Ext.tip.ToolTip', {
                    target: grid.getEl(),
                    delegate: '.assign-school-details',
                    anchor: 'left',
                    html: self.getDetailTitle(self.Options.Titles.School)
                });

            }
        }
    })

    self.State.Grids.School = Ext.create('Ext.grid.Panel', gridOptions);
}

UserFieldsComponent.prototype.ConstructGradeGrid = function () {
    var self = this;

    var getGrid = function () {
        return self.State.Grids.Grade;
    };

    var getNewRecord = function () {
        return Grade_Level.create({
        });
    }

    var titleCallback = function (value) {
        self.Options.Titles.Grade = value;
        if (self.State.Tooltips.AssignGrade != null) {
            self.State.Tooltips.AssignGrade.setHtml(self.getAssignTitle(value));
        }
        if (self.State.Tooltips.DetailGrade != null) {
            self.State.Tooltips.DetailGrade.setHtml(self.getDetailTitle(value));
        }
    }

    var toolbar = {
        xtype: 'toolbar',
        dock: 'top',
        height: 30,
        items: [
            self.CreateSearchField('grade', self.State.Stores.Grade, 'GRADE'),
            '-',
            self.CreateTitleField('grade', self.Options.Titles.Grade, getGrid, 'Field1Name', titleCallback),
            '->',
            self.CreateExportButton(getGrid, 'Grade_Level'),
            '-',
            self.CreateAddButton(getGrid, self.State.Stores.Grade, getNewRecord, 'GRADEID')
        ]
    }

    var schoolStore = Ext.create('SchoolStore', {
        autoLoad: true,
        pageSize: -1,
        autoSync: true
    });
    schoolStore.on('load', function (store, records) {
        store.insert(0, [{
            LOCATION: '&nbsp;',
            ID: 0
        }]);
    });

    /*
    The single grade school field is not used as it is part of a one to many relationship (one grade to many schools type of relationship).

    var schoolField = {
        dataIndex: 'SchoolId',
        header: self.Options.Titles.School,
        xtype: 'componentcolumn',
        width: 200,
        renderer: function (value, meta, record, rowIndex, columnIndex, store, view) {
            var dataIndex = view.ownerCt.columns[columnIndex].dataIndex;
            return {
                xtype: 'combobox',
                displayTpl: '<tpl for=".">{[values.LOCATION.replace("&nbsp;", "")]}</tpl>',
                editable: true,
                forceSelection: true,
                queryMode: 'remote',
                displayField: 'LOCATION',
                valueField: 'ID',
                value: record.get(dataIndex),
                store: schoolStore,
                listeners: {
                    select: function (cmp, records) {
                        if (records == null) {
                            return;
                        }
                        clearTimeout(cmp.recordUpdate);
                        cmp.recordUpdate = setTimeout(function () {
                            record.set(dataIndex, records[0].get('ID'));

                            /// this is because the componentcolumn is not extjs plugin and has issues
                            schoolStore.clearFilter();
                        }, 1000);
                    }
                }
            };
        }
    };
    */

    var gridOptions = Ext.merge(self.Options.DefaultGridOptions, {
        title: self.Options.Titles.Grade,
        hidden: self.Options.Titles.Grade==null? true: false,
        store: self.State.Stores.Grade,
        dockedItems: [
            toolbar,
            self.CreatePager(self.State.Stores.Grade)
        ],
        columns: [
            {
                dataIndex: 'GRADE',
                header: self.Options.Titles.Grade,
                flex: 1,
                editor: {
                    xtype: 'textfield',
                    maxLength: 50,
                    allowBlank: false
                }
            },
            {
                dataIndex: 'GRADEID',
                header: 'ID',
                hidden: true
            },
            Ext.merge({
                header: 'Sort order',
                dataIndex: 'SortOrder'
            }, self.Options.Columns.SortOrder),
            //schoolField,
            {
                text: 'Additional information',
                dataIndex: 'GRADEID',
                width: 120,
                hidden: !self.Options.EnableAdditionalInformation,
                hideable: self.Options.EnableAdditionalInformation,
                renderer: function (value, metadata, record, rowIndex, colIndex, store, view) {
                    var id = 'grade-additional-info-' + value;

                    setTimeout(function () {
                        var el = Ext.get(id);
                        if (el == null) {
                            return;
                        };
                        el.on('click', function () {
                            self.GradeDetailsWindow(record);
                        });
                        self.LoadGradeDetailsColumn(record);
                    }, 100);
                    return '<div id="' + id + '" style="cursor: pointer;"></div>';
                }
            }, {
                // show/hide is managed by enable additional information button handler
                hidden: !self.Options.EnableAdditionalInformation,
                hideable: false,
                xtype: 'actioncolumn',
                width: 25,
                iconCls: 'grid-icon assign-grade-details',
                items: [
                    {
                        icon: config.getUrl('images/icons/famfamfam/application_view_detail.png'),
                        handler: function (grid, rowIndex, colIndex) {
                            var gradeRecord = grid.getStore().getAt(rowIndex);
                            self.GradeDetailsWindow(gradeRecord);
                        }
                    }
                ]
            },
            self.CreateActionColumn(self.State.Stores.School, {
                icon: config.getUrl('images/icons/famfamfam/book.png'),
                iconCls: 'assign-school',
                handler: function(grid, rowIndex, colIndex) {
                    var gradeRecord = grid.getStore().getAt(rowIndex);
                    self.AssignGradeWindow(gradeRecord);
                }
            })
        ],
        listeners: {
            render: function (grid) {
                self.State.Tooltips.AssignSchool = Ext.create('Ext.tip.ToolTip', {
                    target: grid.getEl(),
                    delegate: '.assign-school',
                    anchor: 'left',
                    html: self.getAssignTitle(self.Options.Titles.School),
                });

                self.State.Tooltips.DetailGrade = Ext.create('Ext.tip.ToolTip', {
                    target: grid.getEl(),
                    delegate: '.assign-grade-details',
                    anchor: 'left',
                    html: self.getDetailTitle(self.Options.Titles.Grade)
                });
            }
        }
    })

    self.State.Grids.Grade = Ext.create('Ext.grid.Panel', gridOptions);
}

UserFieldsComponent.prototype.CreatePager = function (store) {
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

UserFieldsComponent.prototype.CreateCellEditor = function () {
    var self = this;

    var cellEditor = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1
    });

    return cellEditor;
}

UserFieldsComponent.prototype.CreateSearchField = function (id, store, searchField) {
    var self = this;

    return {
        id: id + '-search-field',
        width: 300,
        xtype: 'textfield',
        emptyText: 'Keyword search ...',
        triggers: {
            clear: {
                cls: 'x-form-clear-trigger',
                handler: function () {
                    Ext.getCmp(id + '-search-field').setValue('');
                }
            }
        },
        checkChangeBuffer: 1000,
        listeners: {
            render: function (cmp) {
                cmp.triggerEl.hide();
            },
            change: function (cmp, value, oldValue) {
                if (value != '') {
                    cmp.triggerEl.show();
                } else {
                    cmp.triggerEl.hide();
                }
                store.clearFilter(true);
                if (value != '') {
                    store.filter([
                        {
                            property: searchField,
                            value: value
                        }
                    ]);
                } else {
                    store.loadPage(1);
                }
            }
        }
    };
}

UserFieldsComponent.prototype.CreateTitleField = function (id, currentTitle, getGrid, fieldName, callback) {
    var self = this;

    var cmpId = id + '-title-name-field';

    return {
        id: cmpId,
        width: 300,
        value: currentTitle,
        xtype: 'textfield',
        allowBlank: false,
        value: currentTitle,
        fieldLabel: 'Field label',
        labelWidth: 60,
        emptyText: 'Please enter value ...',
        triggers: {
            save: {
                cls: 'x-form-save-trigger',
                handler: function(){
                    var cmp = Ext.getCmp(cmpId);
                    cmp.triggerEl.hide();

                    if (!cmp.isValid()) {
                        return;
                    }

                    var value = cmp.getValue();
                    LAYOUT.setMasterinfoValue(1, fieldName, value, function () {
                        self.State.Panel.getActiveTab().setTitle(value);
                        var grid = getGrid();
                        var columns = grid.columns;
                        columns[0].setText(value);

                        if (Ext.isFunction(callback)) {
                            callback(value);
                        }
                    });
                }
            }
        },
        checkChangeBuffer: 500,
        listeners: {
            render: function (cmp) {

                var tip = Ext.create('Ext.tip.ToolTip', {
                    id: id + '-tip',
                    target: cmp.triggerEl.first(),
                    anchor: 'bottom',
                    anchorOffset: -10,
                    html: 'Press this button to save the value and update the display'
                });
                cmp.triggerEl.hide();
            },
            afterRender: function(cmp, options){
                this.keyNav = Ext.create('Ext.util.KeyNav', cmp.el, {
                    enter: cmp.onTriggerClick,
                    scope: cmp
                });
            },
            change: function (cmp, value, oldValue) {
                if (!cmp.isValid()) {
                    cmp.triggerEl.hide();
                    return;
                }
                cmp.triggerEl.show();
            }
        }
    };
}

UserFieldsComponent.prototype.CreateActionColumn = function (store, items, deleteCallback) {
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

            Ext.MessageBox.show({
                title: 'Confirm Delete',
                msg: 'If you click OK, the record you have selected will be deleted from the database.',
                buttons: Ext.MessageBox.OKCANCEL,
                icon: Ext.window.MessageBox.WARNING,
                fn: function (buttonId) {
                    if (buttonId == 'ok') {
                        var loadPreviousPage = store.count() == 1 && store.currentPage > 1;
                        var currentPage = store.currentPage;
                        store.removeAt(rowIndex);
                        if (loadPreviousPage) {
                            store.loadPage(currentPage - 1);
                        } else {
                            store.load();
                        }
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

UserFieldsComponent.prototype.CreateAddButton = function (getGrid, store, newItem, idProperty) {
    var self = this;

    var definition = {
        xtype: 'button',
        icon: config.getUrl('images/icons/famfamfam/add.png'),
        text: 'Add new',
        handler: function () {
            var grid = getGrid();
            var newRecord = newItem();

            store.sort([{
                property: idProperty,
                direction: 'DESC'
            }]);
            store.on('write', function () {
                store.loadPage(1, {
                    callback: function () {
                        grid.getSelectionModel().select(0);
                        var plugin = grid.findPlugin('cellediting');
                        var record = store.getAt(0);
                        var column = grid.columns[0];
                        var result = plugin.startEdit(record, column);

                        LAYOUT.notify('A new record has been addded and the list has been sorted so that the new record is on the top. It is ready for you to edit.');
                    }
                });
            }, this, { single: true });
            store.add(newRecord);
        }
    };

    return definition;
}

UserFieldsComponent.prototype.CreateExportButton = function (getGrid, entity) {
    var self = this;

    var definition = {
        xtype: 'button',
        icon: config.getUrl('images/icons/famfamfam/page_excel.png'),
        text: 'Export to Excel',
        handler: function () {

            var grid = getGrid();
            var store = grid.getStore();

            var comma = '';
            var columns = [];

            for (var index = 0; index < grid.columns.length; index++) {
                var column = grid.columns[index];
                if (column.dataIndex != null &
                        (typeof (column.hidden) == 'undefined' ||
                        (typeof (column.hidden) != 'undefined' && column.hidden == false))
                ) {
                    var text;
                    if (column.exportHeader) {
                        text = column.exportHeader;
                    } else {
                        text = column.text;
                    }

                    var colindx = column.dataIndex

                    columns.push({
                        column: colindx,
                        text: text
                    });
                }
            }

            var proxy = store.getProxy();
            var optn = store.lastOptions;
            var filter = [];
            var sort = []
            //console.log(optn.filters)

            if (typeof (optn.filters) != "undefined") {
                filter.push({
                    property: optn.filters[0].config.property,
                    value: optn.filters[0].config.value
                });
            }

            if (typeof (optn.sorters) != "undefined") {
                sort.push({
                    property: optn.sorters[0].config.property,
                    direction: optn.sorters[0].config.direction
                });

            }
            //console.log(sort)
            //console.log(filter)

            //console.log(store.lastOptions)

            var params =
            {
                sort: sort = Ext.encode(sort),
                filter: Ext.encode(filter),
                limit:1000000,
                columns: Ext.encode(columns)
            }


            //console.log(columns)
            var url = config.getUrl('adm/datastore/export?entity=' + entity);

                config.postData(url, params);

                return;


            return;

            var grid = getGrid();
            var store = grid.getStore();

            var comma = '';
            var columns = [];

            for (var index = 0; index < grid.columns.length; index++) {
                var column = grid.columns[index];
                if (column.dataIndex != null &
                        (typeof (column.hidden) == 'undefined' ||
                        (typeof (column.hidden) != 'undefined' && column.hidden == false))
                ) {
                    var text;
                    if (column.exportHeader) {
                        text = column.exportHeader;
                    } else {
                        text = column.text;
                    }
                    columns.push({
                        column: column.dataIndex,
                        text: text
                    });
                }
            }
            var proxy = store.getProxy();
            //console.log(proxy)
            //console.log(store.lastOptions)
            //var request = proxy.buildRequest(store.lastOptions);
            var request = proxy.buildRequest(store.model.fields);
            //console.log(request)
            var params = request.params;
            params['columns'] = Ext.encode(columns);

            var url = config.getUrl('adm/datastore/export?entity=' + entity);

            config.postData(url, params);

        }
    };

    return definition;
}

UserFieldsComponent.prototype.getAssignTitle = function (value) {
    var self = this;
    return 'Assign ' + self.prularize(value);
};

UserFieldsComponent.prototype.getDetailTitle = function (value) {
    var self = this;
    if (value == null) { return; }
    return 'Edit ' + value.toLowerCase() + ' additional information';
};

UserFieldsComponent.prototype.prularize = function (value) {
    var self = this;
    if (value == null) { return; }
    var multiplicator = '';
    if (!value.endsWith('s')) {
        multiplicator = '(s)';
    }
    return value + multiplicator;
}

UserFieldsComponent.prototype.AssignGradeWindow = function (gradeRecord) {
    var self = this;

    var gradeId = gradeRecord.get('GRADEID');
    var relStore = self.State.Stores.GradeToSchool.Relations;
    var schoolStore = self.State.Stores.GradeToSchool.School;

    LAYOUT.MaskLayout('Loading ...');
    var loadCounter = schoolStore.loaded ? 1 : 2;
    var init = function () {
        loadCounter--;
        if (loadCounter > 0) {
            return;
        }
        LAYOUT.UnmaskLayout();

        var view = Ext.getBody().getViewSize();

        var school = self.prularize(self.Options.Titles.School).toLowerCase();
        var grade = self.Options.Titles.Grade.toLowerCase();
        var title = 'Select ' + school + ' for ' + grade + ' ' + gradeRecord.get('GRADE');

        var buildPanel = function () {
            var schoolItems = [];
            schoolStore.each(function (schoolRecord) {
                var schoolId = schoolRecord.get('ID');
                var locationid = schoolRecord.get('locationid');
                var existingRecordIndex = relStore.find('SchoolsId', locationid);
                var hasSchool = existingRecordIndex > -1 ? true : false;

                var column = Ext.create('Ext.form.field.Checkbox', {
                    id: 'relation-school-checkbox-' + schoolId,
                    locationid: locationid,
                    boxLabel: schoolRecord.get('LOCATION'),
                    checked: hasSchool,
                    columnWidth: 0.33,
                    listeners: {
                        change: function (cmp, value, oldValue) {
                            win.mask('Updating database ...');
                            var schoolId = cmp.schoolId;
                            var existingRecordIndex = relStore.find('SchoolsId', locationid);
                            if (existingRecordIndex > -1) {
                                var record = relStore.getAt(existingRecordIndex);
                                relStore.remove(record);
                            } else {
                                relStore.add({
                                    SchoolsId: locationid,
                                    GradeId: gradeId
                                });
                            }
                            win.unmask();
                        }
                    }
                });
                schoolItems.push(column);
            });
            var schoolItemsPanel = Ext.create('Ext.panel.Panel', {
                border: 0,
                frame: 0,
                id: 'relation-school-checkbox-panel',
                layout: {
                    type: 'column'
                },
                items: schoolItems
            });
            return schoolItemsPanel;
        }

        var schoolItemsPanel = buildPanel();

        var win = new Ext.Window({
            title: title,
            bodyStyle: {
                backgroundColor: '#fff'
            },
            tbar: [
                {
                    id: 'school-relations-search',
                    xtype: 'trigger',
                    emptyText: 'Filter ' + self.prularize(self.Options.Titles.School) + ' ...',
                    triggerCls: 'x-form-clear-trigger',
                    onTriggerClick: function () {
                        Ext.getCmp('school-relations-search').setValue('');
                    },
                    checkChangeBuffer: 1000,
                    listeners: {
                        render: function (cmp) {
                            cmp.triggerEl.hide();
                        },
                        change: function (cmp, value, oldValue) {

                            if (value != '') {
                                cmp.triggerEl.show();
                            } else {
                                cmp.triggerEl.hide();
                            }
                            win.mask('Searching ...');

                            schoolStore.on('load', function () {
                                win.removeAll();
                                schoolItemsPanel.destroy();
                                schoolItemsPanel = buildPanel();
                                win.unmask();
                                win.add(schoolItemsPanel);
                            }, schoolStore, { single: true });

                            if (value != '') {
                                schoolStore.clearFilter(true);
                                schoolStore.filter([
                                    {
                                        property: 'LOCATION',
                                        value: value
                                    }
                                ]);
                            } else {
                                schoolStore.clearFilter();
                            }
                        }
                    }
                }
            ],
            autoScroll: true,
            width: view.width * 0.75,
            height: view.height * 0.75,
            modal: true,
            closeAction: 'destroy',
            items: [
                schoolItemsPanel
            ],
            listeners: {
                close: function () {
                    schoolStore.clearFilter();
                    win.destroy();
                }
            }
        });
        win.show();
    }

    if (!schoolStore.loaded) {
        schoolStore.load(function () {
            schoolStore.loaded = true;
            init();
        });
    }

    relStore.on('load', function () {
        init();
    },
    relStore,
    {
        single: true
    });
    relStore.clearFilter(true);
    relStore.filter([
        {
            property: 'GradeId',
            value: gradeId
        }
    ]);

}

UserFieldsComponent.prototype.AssignSchoolWindow = function (schoolRecord) {
    var self = this;

    var schoolLocationId = schoolRecord.get('locationid');
    var relStore = self.State.Stores.GradeToSchool.Relations;
    var gradeStore = self.State.Stores.GradeToSchool.Grade;

    LAYOUT.MaskLayout('Loading ...');

    var loadCounter = gradeStore.loaded ? 1 : 2;

    var init = function () {
        loadCounter--;
        if (loadCounter > 0) {
            return;
        }
        LAYOUT.UnmaskLayout();
        var view = Ext.getBody().getViewSize();

        var grade = self.prularize(self.Options.Titles.Grade).toLowerCase();
        var school = self.Options.Titles.School.toLowerCase();
        var title = 'Select ' + grade + ' for ' + school + ' ' + schoolRecord.get('LOCATION');

        var buildPanel = function () {
            var gradeItems = [];
            gradeStore.each(function (gradeRecord) {
                var gradeId = gradeRecord.get('GRADEID');
                var existingRecordIndex = relStore.find('GradeId', gradeId);
                var hasGrade = existingRecordIndex > -1 ? true : false;

                var column = Ext.create('Ext.form.field.Checkbox', {
                    id: 'relation-grade-checkbox-' + gradeId,
                    gradeId: gradeId,
                    boxLabel: gradeRecord.get('GRADE'),
                    checked: hasGrade,
                    columnWidth: 0.33,
                    listeners: {
                        change: function (cmp, value, oldValue) {
                            win.mask('Updating database ...');
                            var gradeId = cmp.gradeId;
                            var existingRecordIndex = relStore.find('GradeId', gradeId);
                            if (existingRecordIndex > -1) {
                                var record = relStore.getAt(existingRecordIndex);
                                relStore.remove(record);
                            } else {
                                relStore.add({
                                    SchoolsId: schoolLocationId,
                                    GradeId: gradeId
                                });
                            }
                            win.unmask();
                        }
                    }
                });
                gradeItems.push(column);
            });
            var gradeItemsPanel = Ext.create('Ext.panel.Panel', {
                border: 0,
                frame: 0,
                id: 'relation-grade-checkbox-panel',
                layout: {
                    type: 'column'
                },
                items: gradeItems
            });
            return gradeItemsPanel;
        }

        var gradeItemsPanel = buildPanel();

        var win = new Ext.Window({
            title: title,
            bodyStyle: {
                backgroundColor: '#fff'
            },
            tbar: [
                {
                    id: 'grade-relations-search',
                    xtype: 'trigger',
                    emptyText: 'Filter ' + self.prularize(self.Options.Titles.Grade) + ' ...',
                    triggerCls: 'x-form-clear-trigger',
                    onTriggerClick: function () {
                        Ext.getCmp('grade-relations-search').setValue('');
                    },
                    checkChangeBuffer: 1000,
                    listeners: {
                        render: function (cmp) {
                            cmp.triggerEl.hide();
                        },
                        change: function (cmp, value, oldValue) {

                            if (value != '') {
                                cmp.triggerEl.show();
                            } else {
                                cmp.triggerEl.hide();
                            }
                            win.mask('Searching ...');

                            gradeStore.on('load', function () {
                                win.removeAll();
                                gradeItemsPanel.destroy();
                                gradeItemsPanel = buildPanel();
                                win.unmask();
                                win.add(gradeItemsPanel);
                            }, gradeStore, { single: true });

                            if (value != '') {
                                gradeStore.clearFilter(true);
                                gradeStore.filter([
                                    {
                                        property: 'GRADE',
                                        value: value
                                    }
                                ]);
                            } else {
                                gradeStore.clearFilter();
                            }
                        }
                    }
                }
            ],
            autoScroll: true,
            width: view.width * 0.75,
            height: view.height * 0.75,
            modal: true,
            closeAction: 'destroy',
            items: [
                gradeItemsPanel
            ],
            listeners: {
                close: function () {
                    gradeStore.clearFilter();
                    win.destroy();
                }
            }
        });
        win.show();
    }

    if (!gradeStore.loaded) {
        gradeStore.load(function () {
            gradeStore.loaded = true;
            init();
        });
    }

    relStore.on('load', function () {
        init();
    },
    relStore,
    {
        single: true
    });
    relStore.clearFilter(true);
    relStore.filter([
        {
            property: 'SchoolsId',
            value: schoolLocationId
        }
    ]);

}

UserFieldsComponent.prototype.GetDistrictDetailsRecord = function (districtId, callback) {
    var self = this;

    var store = self.State.Stores.ExtraInfo.District;

    store.on('load', function () {
        var detail = store.getAt(0);
        if (!detail) {
            store.add({
                districtID: districtId
            });
            store.on('datachanged', function () {
                self.GetDistrictDetailsRecord(districtId, callback);
            }, store, { single: true });
        } else {
            callback(detail);
        }
    },
    store,
    {
        single: true
    });
    store.clearFilter(true);
    store.filter([
        {
            property: 'districtID',
            value: districtId
        }
    ]);
}

UserFieldsComponent.prototype.LoadDistrictDetailsColumn = function (record) {
    var self = this;

    var distid = record.get('DISTID');
    var id = 'district-additional-info-' + distid;
    var el = Ext.get(id);
    if (el == null) {
        return;
    }
    el.load({
        url: config.getUrl('adm/userfields/districtadditionalinfo?distid=' + distid),
        success: function () {
            if (self.State.AjaxRequestCount == 0) {
                self.State.Panel.doLayout();
            }
        }
    });
}

UserFieldsComponent.prototype.DistrictDetailsWindow = function (record) {
    var self = this;

    var init = function (detail) {
        LAYOUT.UnmaskLayout();

        var view = Ext.getBody().getViewSize();

        var layoutForm = Ext.create('Ext.form.Panel', {
            border: 0,
            fieldDefaults: {
                msgTarget: 'qtip',
                labelAlign: 'right',
                labelWidth: 65,
                anchor: '100%'
            },
            bodyPadding: 5,
            frame: false,
            defaultType: 'textfield',
            buttons: [
                {
                    text: 'Save',
                    icon: config.getUrl('images/icons/famfamfam/save.png'),
                    handler: function () {
                        var values = layoutForm.getValues();
                        detail.beginEdit();
                        for (var field in values) {
                            detail.set(field, values[field]);
                        }
                        detail.endEdit();
                        LAYOUT.notify('Additional information saved.');
                        win.destroy();
                    }
                }, {
                    text: 'Cancel',
                    icon: config.getUrl('images/icons/famfamfam/cancel.png'),
                    handler: function () {
                        win.destroy();
                    }
                }
            ],
            items: [
                {
                    name: 'shortdesc',
                    fieldLabel: 'Description',
                    maxLength: 250,
                    value: detail.get('shortdesc')
                }, {
                    name: 'daddress',
                    fieldLabel: 'Address',
                    maxLength: 250,
                    value: detail.get('daddress')
                }, {
                    name: 'dcity',
                    fieldLabel: 'City',
                    maxLength: 250,
                    value: detail.get('dcity')
                }, {
                    name: 'dstate',
                    fieldLabel: 'State',
                    maxLength: 50,
                    value: detail.get('dstate')
                }, {
                    name: 'dzip',
                    fieldLabel: 'Zip',
                    maxLength: 50,
                    value: detail.get('dzip')
                }, {
                    name: 'dphone',
                    fieldLabel: 'Phone',
                    maxLength: 100,
                    value: detail.get('dphone')
                }, {
                    name: 'dfax',
                    fieldLabel: 'Fax',
                    maxLength: 100,
                    value: detail.get('dfax')
                }, {
                    name: 'dcountry',
                    fieldLabel: 'Country',
                    maxLength: 100,
                    xtype: 'combobox',
                    store: Ext.create('CountryStore', {
                        pageSize: -1
                    }),
                    displayField: 'countryname',
                    valueField: 'countryname',
                    editable: true,
                    queryMode: 'local',
                    value: detail.get('dcountry')
                }
            ]
        })

        var win = new Ext.create('Ext.window.Window', {
            title: 'Additional information for ' + record.get('DISTRICT1') + ' ' + self.Options.Titles.District.toLowerCase(),
            modal: true,
            layout: 'fit',
            width: Math.min(400, view.width / 3 * 2),
            closeAction: 'destroy',
            items: [
                layoutForm
            ],
            listeners: {
                close: function () {
                    win.destroy();
                },
                destroy: function () {
                    self.LoadDistrictDetailsColumn(record);
                }
            }
        });
        win.show();
    };

    LAYOUT.MaskLayout('Processing ...');
    var districtId = record.get('DISTID');
    self.GetDistrictDetailsRecord(districtId, init);

}

// school id is location id
UserFieldsComponent.prototype.GetSchoolDetailsRecord = function (schoolId, callback) {
    var self = this

    var store = self.State.Stores.ExtraInfo.School;

    store.on('load', function () {
        var detail = store.getAt(0);
        if (!detail) {
            store.add({
                schoolid: schoolId
            });
            store.on('datachanged', function () {
                self.GetSchoolDetailsRecord(schoolId, callback);
            }, store, { single: true });
        } else {
            callback(detail);
        }
    },
    store,
    {
        single: true
    });
    store.clearFilter(true);
    store.filter([
        {
            property: 'schoolid',
            value: schoolId
        }
    ]);
}

UserFieldsComponent.prototype.LoadSchoolDetailsColumn = function (record) {
    var self = this;

    var locationid = record.get('locationid');
    var id = 'school-additional-info-' + locationid;
    var el = Ext.get(id);
    if (el == null) {
        return;
    }
    el.load({
        url: config.getUrl('adm/userfields/schooladditionalinfo?locationid=' + locationid),
        success: function() {
            if (self.State.AjaxRequestCount == 0) {
                self.State.Panel.doLayout();
            }
        }
    });
}

UserFieldsComponent.prototype.SchoolDetailsWindow = function (record) {
    var self = this;

    var init = function (detail) {
        LAYOUT.UnmaskLayout();

        var view = Ext.getBody().getViewSize();

        var layoutForm = Ext.create('Ext.form.Panel', {
            border: 0,
            fieldDefaults: {
                msgTarget: 'qtip',
                labelAlign: 'right',
                labelWidth: 75,
                anchor: '100%'
            },
            bodyPadding: 5,
            frame: false,
            defaultType: 'textfield',
            buttons: [
                {
                    text: 'Save',
                    icon: config.getUrl('images/icons/famfamfam/save.png'),
                    handler: function () {
                        var values = layoutForm.getValues();
                        detail.beginEdit();
                        for (var field in values) {
                            detail.set(field, values[field]);
                        }
                        detail.endEdit();
                        LAYOUT.notify('Additional information saved.');
                        win.destroy();
                    }
                }, {
                    text: 'Cancel',
                    icon: config.getUrl('images/icons/famfamfam/cancel.png'),
                    handler: function () {
                        win.destroy();
                    }
                }
            ],
            items: [
                {
                    name: 'schoolshortdesc',
                    fieldLabel: 'Description',
                    maxLength: 250,
                    value: detail.get('schoolshortdesc')
                }, {
                    name: 'sshipnumber',
                    fieldLabel: 'Ship number',
                    maxLength: 250,
                    value: detail.get('sshipnumber')
                }, {
                    name: 'saddress',
                    fieldLabel: 'Address',
                    maxLength: 250,
                    value: detail.get('saddress')
                }, {
                    name: 'scity',
                    fieldLabel: 'City',
                    maxLength: 250,
                    value: detail.get('scity')
                }, {
                    name: 'sstate',
                    fieldLabel: 'State',
                    maxLength: 250,
                    value: detail.get('sstate')
                }, {
                    name: 'szip',
                    fieldLabel: 'Zip',
                    maxLength: 250,
                    value: detail.get('szip')
                }, {
                    name: 'sphonenum',
                    fieldLabel: 'Phone',
                    maxLength: 250,
                    value: detail.get('sphonenum')
                }, {
                    name: 'sfaxnum',
                    fieldLabel: 'Fax',
                    maxLength: 250,
                    value: detail.get('sfaxnum')
                }, {
                    name: 'scountry',
                    fieldLabel: 'Country',
                    maxLength: 75,
                    xtype: 'combobox',
                    store: Ext.create('CountryStore', {
                        pageSize: -1
                    }),
                    displayField: 'countryname',
                    valueField: 'countryname',
                    editable: true,
                    queryMode: 'local',
                    value: detail.get('scountry')
                }
            ]
        })

        var win = new Ext.create('Ext.window.Window', {
            title: 'Additional information for ' + record.get('LOCATION') + ' ' + self.Options.Titles.School.toLowerCase(),
            modal: true,
            layout: 'fit',
            width: Math.min(400, view.width / 3 * 2),
            closeAction: 'destroy',
            items: [
                layoutForm
            ],
            listeners: {
                destroy: function () {
                    self.LoadSchoolDetailsColumn(record);
                }
            }
        });
        win.show();
    };

    LAYOUT.MaskLayout('Processing ...');
    var schoolId = record.get('locationid');
    self.GetSchoolDetailsRecord(schoolId, init);
}

UserFieldsComponent.prototype.GetGradeDetailsRecord = function (gradeId, callback) {
    var self = this;

    var store = self.State.Stores.ExtraInfo.Grade;
    store.on('load', function () {
        var detail = store.getAt(0);
        if (!detail) {
            store.add({
                gradeid: gradeId
            });
            store.on('datachanged', function () {
                self.GetGradeDetailsRecord(gradeId, callback);
            }, store, { single: true });
        } else {
            callback(detail);
        }
    },
    store,
    {
        single: true
    });
    store.clearFilter(true);
    store.filter([
        {
            property: 'gradeid',
            value: gradeId
        }
    ]);

}

UserFieldsComponent.prototype.LoadGradeDetailsColumn = function (record) {
    var self = this;

    var gradeId = record.get('GRADEID');
    var id = 'grade-additional-info-' + gradeId;
    var el = Ext.get(id);
    if (el == null) {
        return;
    }
    el.load({
        url: config.getUrl('adm/userfields/gradeadditionalinfo?gradeId=' + gradeId),
        success: function () {
            if (self.State.AjaxRequestCount == 0) {
                self.State.Panel.doLayout();
            }
        }
   });
}

UserFieldsComponent.prototype.GradeDetailsWindow = function (record) {
    var self = this;

    var init = function (detail) {
        LAYOUT.UnmaskLayout();

        var view = Ext.getBody().getViewSize();

        var layoutForm = Ext.create('Ext.form.Panel', {
            border: 0,
            fieldDefaults: {
                msgTarget: 'qtip',
                labelAlign: 'right',
                labelWidth: 75,
                anchor: '100%'
            },
            bodyPadding: 5,
            frame: false,
            defaultType: 'textfield',
            buttons: [
                {
                    text: 'Save',
                    icon: config.getUrl('images/icons/famfamfam/save.png'),
                    handler: function () {
                        var values = layoutForm.getValues();
                        detail.beginEdit();
                        for (var field in values) {
                            detail.set(field, values[field]);
                        }
                        detail.endEdit();
                        LAYOUT.notify('Additional information saved.');
                        win.destroy();
                    }
                }, {
                    text: 'Cancel',
                    icon: config.getUrl('images/icons/famfamfam/cancel.png'),
                    handler: function () {
                        win.destroy();
                    }
                }
            ],
            items: [
                {
                    name: 'gradeshortdesc',
                    fieldLabel: 'Description',
                    maxLength: 250,
                    value: detail.get('gradeshortdesc')
                }, {
                    name: 'manufacturer',
                    fieldLabel: 'Manufacturer',
                    maxLength: 250,
                    value: detail.get('manufacturer')
                }, {
                    name: 'gaddress',
                    fieldLabel: 'Address',
                    maxLength: 250,
                    value: detail.get('gaddress')
                }, {
                    name: 'gcity',
                    fieldLabel: 'City',
                    maxLength: 100,
                    value: detail.get('gcity')
                }, {
                    name: 'gstate',
                    fieldLabel: 'State',
                    maxLength: 50,
                    value: detail.get('gstate')
                }, {
                    name: 'gzip',
                    fieldLabel: 'Zip',
                    maxLength: 50,
                    value: detail.get('gzip')
                }
            ]
        })

        var win = new Ext.create('Ext.window.Window', {
            title: 'Additional information for ' + record.get('GRADE') + ' ' + self.Options.Titles.Grade.toLowerCase(),
            modal: true,
            layout: 'fit',
            width: Math.min(400, view.width / 3 * 2),
            closeAction: 'destroy',
            items: [
                layoutForm
            ],
            listeners: {
                destroy: function ()
                {
                    self.LoadGradeDetailsColumn(record);
                }
            }
        });
        win.show();
    };

    LAYOUT.MaskLayout('Processing ...');
    var gradeId = record.get('GRADEID');
    self.GetGradeDetailsRecord(gradeId, init);

}