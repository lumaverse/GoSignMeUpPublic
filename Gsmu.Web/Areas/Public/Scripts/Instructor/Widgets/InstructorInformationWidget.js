function InstructorInformationWidget(dashboard) {
    var self = this;

    self.State.dashboard = dashboard;
}

InstructorInformationWidget.prototype = new WidgetBase();

InstructorInformationWidget.constructor = InstructorInformationWidget;

InstructorInformationWidget.prototype.Options = {
};

InstructorInformationWidget.prototype.State = {
    container: null,
    dashboard: null,
    widget: null
};
function ArrayFilterData(Data, id, type) {
    
    Data.forEach(function (element) {
        if (element.vlu == id) {
            if (type == "dist") {
                district_Label = element.labl;
            }
            if (type == "sch") {
                school_Label = element.labl;
            }
            if (type == "grd") {
                grade_label = element.labl;
            }
        }
    });
}

InstructorInformationWidget.prototype.RenderImplementation = function () {
    var self = this;  
    var supervisorData = self.State.dashboard.State.instructorData;
    ArrayFilterData(ListOfDistrict, supervisorData.DISTRICT, "dist")
    ArrayFilterData(ListOfSchool, supervisorData.SCHOOL, "sch")
    ArrayFilterData(ListOfGrade, supervisorData.GRADELEVEL, "grd")
    self.State.widget = Ext.create('Ext.form.Panel', self.getWidgetDefaults({
        userMembers: {
            viewItems: ['first-display', 'last-display', 'address-display', 'city-display', 'state-display', 'zip-display', 'phone-display', 'fax-display', 'email-display', 'experienece-display', 'workphone-display','district-display','school-display','grade-display'],
            editItems: ['first', 'last', 'address', 'city', 'state', 'zip', 'phone', 'fax', 'email','experience','workphone','district','school','grade']
        },
        listeners: {
            render: function (panel) {
                panel.userMembers.viewMode(panel);
            }
        },
        frame: true,
        title: 'Information',
        url: config.getUrl('public/instructor/saveinformation'),
        icon: config.getUrl('Images/Icons/Famfamfam/information.png'),
        items: [
            self.getDisplayField({
                itemId: 'first-display',
                fieldLabel: self.requiredIndicator + 'First name',
                value: supervisorData.FIRST
            }),
            self.getDisplayField({
                itemId: 'last-display',
                fieldLabel: self.requiredIndicator + 'Last name',
                value: supervisorData.LAST
            }),
            self.getDisplayField({
                itemId: 'email-display',
                fieldLabel: self.requiredIndicator + 'E-mail',
                value: supervisorData.EMAIL
            }),
            self.getDisplayField({
                itemId: 'address-display',
                fieldLabel: 'Address',
                value: supervisorData.ADDRESS
            }),
            self.getDisplayField({
                itemId: 'city-display',
                fieldLabel: 'City',
                value: supervisorData.CITY
            }),
            self.getDisplayField({
                itemId: 'state-display',
                fieldLabel: 'State',
                value: supervisorData.STATE
            }),
            self.getDisplayField({
                itemId: 'zip-display',
                fieldLabel: 'Zip',
                value: supervisorData.ZIP
            }),
            self.getDisplayField({
                itemId: 'phone-display',
                fieldLabel: 'Phone',
                value: supervisorData.HOMEPHONE
            }),
            self.getDisplayField({
                itemId: 'workphone-display',
                fieldLabel: 'Work Phone',
                value: supervisorData.WORKPHONE
            }),
            self.getDisplayField({
                itemId: 'fax-display',
                fieldLabel: 'Fax',
                value: supervisorData.FAX
            }),
                        self.getDisplayField({
                            itemId: 'experienece-display',
                            fieldLabel: 'Years Teaching Experience',
                            value: supervisorData.EXPERIENCE
                        }),

                                                self.getDisplayField({
                                                    id: 'district-display',
                                                    fieldLabel: 'District',
                                                    value: district_Label,
                                                    listeners: {
                                                        show: function () {
                                                            if (hideDist) {
                                                                Ext.getCmp('district-display').hide();
                                                            }
                                                        },
                                                        render: function () {
                                                            if (hideDist) {
                                                                Ext.getCmp('district-display').hide();
                                                            }
                                                        }

                                                    }
                                                    
                                                }),
                                                                        self.getDisplayField({
                                                                            id: 'school-display',
                                                                            fieldLabel: 'School',
                                                                            value: school_Label,
                                                                            listeners: {
                                                                                show: function () {
                                                                                    if (hideSch) {
                                                                                        Ext.getCmp('school-display').hide();
                                                                                    }
                                                                                },
                                                                                render: function () {
                                                                                    if (hideSch) {
                                                                                        Ext.getCmp('school-display').hide();
                                                                                    }
                                                                                }
                                                                            }
                                                                            
                                                                        }),
                                                                                                self.getDisplayField({
                                                                                                    id: 'grade-display',
                                                                                                    fieldLabel: 'Grade',
                                                                                                    value: grade_label,
                                                                                                    listeners: {
                                                                                                        render: function () {
                                                                                                            if (hideGrade) {
                                                                                                                Ext.getCmp('grade-display').hide();
                                                                                                            }
                                                                                                        },
                                                                                                        show: function () {
                                                                                                            if (hideGrade) {
                                                                                                                Ext.getCmp('grade-display').hide();
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }),

            self.getField({
                itemId: 'first',
                name: 'first',
                maxLength: 50,
                allowBlank: false,
                fieldLabel: self.requiredIndicator + 'First name',
                value: supervisorData.FIRST
            }),
            self.getField({
                itemId: 'last',
                name: 'last',
                allowBlank: false,
                maxLength: 50,
                fieldLabel: self.requiredIndicator + 'Last name',
                value: supervisorData.LAST
            }),
            self.getField({
                itemId: 'email',
                name: 'email',
                fieldLabel: self.requiredIndicator + 'E-mail',
                maxLength: 50,
                allowBlank: false,
                value: supervisorData.EMAIL
            }),
            self.getField({
                itemId: 'address',
                name: 'address',
                maxLength: 50,
                fieldLabel: 'Address',
                value: supervisorData.ADDRESS
            }),
            self.getField({
                itemId: 'city',
                name: 'city',
                fieldLabel: 'City',
                maxLength: 50,
                value: supervisorData.CITY
            }),
            self.getField({
                itemId: 'state',
                name: 'state',
                fieldLabel: 'State',
                maxLength: 50,
                value: supervisorData.STATE
            }),
            self.getField({
                itemId: 'zip',
                name: 'zip',
                maxLength: 50,
                fieldLabel: 'Zip',
                value: supervisorData.ZIP
            }),
            self.getField({
                itemId: 'phone',
                name: 'phone',
                fieldLabel: 'Home Phone',
                maxLength: 50,
                value: supervisorData.HOMEPHONE
            }),
                        self.getField({
                            itemId: 'workphone',
                            name: 'workphone',
                            fieldLabel: 'Work Phone',
                            maxLength: 50,
                            value: supervisorData.WORKPHONE
                        }),
            self.getField({
                itemId: 'fax',
                name: 'fax',
                maxLength: 50,
                fieldLabel: 'Fax',
                value: supervisorData.FAX
            }),
                        self.getField({
                            itemId: 'experience',
                            name: 'experience',
                            maxLength: 50,
                            fieldLabel: 'Years Teaching Experience',
                            value: supervisorData.EXPERIENCE
                        }),
                                                self.getField({
                                                    id: 'district',
                                                    xtype: 'combobox',
                                                    name: 'district',
                                                    fieldLabel: 'District',
                                                    store: DistrictStore,
                                                    displayField: 'labl',
                                                    valueField: 'vlu',
                                                    queryMode: 'local',
                                                    value: district_Label,
                                                    
                                                    
                                                    listeners: {
                                                        change: function (frm, newValue, oldValue, eOpts) {
                                                            if (newValue != null) {
                                                                var cmbxfld = Ext.getCmp('school');
                                                                var store = cmbxfld.getStore();
                                                                store.filter([
                                                                    { id: 'district', property: 'district', value: newValue, exactMatch: true }
                                                                ]);
                                                                store.reload();
                                                                cmbxfld.Text = '';
                                                                if (store.data.length == 0) {
                                                                    cmbxfld.emptyText = 'No record available';
                                                                } else {
                                                                    cmbxfld.emptyText = '';
                                                                }
                                                                cmbxfld.applyEmptyText();
                                                                cmbxfld.setValue(0);
                                                                districtnewvalue = newValue;
                                                            }
                                                        },
                                                        show: function () {
                                                            if (hideDist) {
                                                                Ext.getCmp('district').hide();
                                                            }
                                                        }

                                                    }
                                                   
                                                }), self.getField({
                                                    id: 'school',
                                                    xtype: 'combobox',
                                                    name: 'school',
                                                    maxLength: 50,
                                                    fieldLabel: 'School',
                                                    store: SchoolStore,
                                                    displayField: 'labl',
                                                    valueField: 'vlu',
                                                    queryMode: 'local',
                                                    value: school_Label,
                                                    

                                                    listeners: {
                                                        change: function (frm, newValue, oldValue, eOpts) {
                                                            if (newValue != null) {
                                                                var cmbxfld = Ext.getCmp('grade');
                                                                var store = cmbxfld.getStore();
                                                                store.filter([
                                                                    { id: 'school', property: 'school', value: newValue, exactMatch: true }
                                                                ]);
                                                                store.reload();
                                                                cmbxfld.Text = '';
                                                                if (store.data.length == 0) {
                                                                    cmbxfld.emptyText = 'No record available';
                                                                } else {
                                                                    cmbxfld.emptyText = '';
                                                                }
                                                                cmbxfld.applyEmptyText();
                                                                cmbxfld.setValue(0);
                                                                schoolnewvalue = newValue;
                                                            }
                                                        },
                                                        show: function () {
                                                            if (hideSch) {
                                                                Ext.getCmp('school').hide();
                                                            }
                                                        }
                                                    }
                                                    
                                                }), self.getField({
                                                    id: 'grade',
                                                    xtype: 'combobox',
                                                    name: 'grade',
                                                    maxLength: 50,
                                                    fieldLabel: 'Grade',
                                                    store: GradeLevelStore,
                                                    displayField: 'labl',
                                                    valueField: 'vlu',
                                                    queryMode: 'local',
                                                    value: grade_label,
                                                    listeners: {
                                                        show: function () {
                                                            if (hideGrade) {
                                                                Ext.getCmp('grade').hide();
                                                            }
                                                        }
                                                    }
                                                   
                                                })

        ]
    }));

    
};
