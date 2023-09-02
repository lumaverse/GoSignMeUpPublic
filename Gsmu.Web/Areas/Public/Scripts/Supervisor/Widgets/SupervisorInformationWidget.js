function SupervisorInformationWidget(dashboard) {
    var self = this;

    self.State.dashboard = dashboard;
}

SupervisorInformationWidget.prototype = new WidgetBase();

SupervisorInformationWidget.constructor = SupervisorInformationWidget;

SupervisorInformationWidget.prototype.Options = {
};

SupervisorInformationWidget.prototype.State = {
    container: null,
    dashboard: null,
    widget: null
};

SupervisorInformationWidget.prototype.RenderImplementation = function () {
    var self = this;
    
    var supervisorData = self.State.dashboard.State.supervisorData;
    var supervisorDistrict = self.State.dashboard.State.supervisorDistrict;
    var supervisorSchool = self.State.dashboard.State.supervisorSchool;
    var hideSupDistrict = false;
    if (supervisorDistrict.DISTRICT1 == null) { hideSupDistrict = true; }
    //console.log(supervisorDistrict)
    var SchoolList = "";
    var hideSupSchool = true;
    if (supervisorSchool.length > 0) {
        for (sch in supervisorSchool) {
            SchoolList += supervisorSchool[sch].LOCATION + "</br>"
        }
        hideSupSchool = false;
    }

    self.State.widget = Ext.create('Ext.form.Panel', self.getWidgetDefaults({
        userMembers: {
            //viewItems: ['first-display', 'last-display', 'title-display', 'address-display', 'city-display', 'state-display', 'zip-display', 'phone-display', 'fax-display', 'supervisornum-display', 'email-display', 'additionalemailaddresses-display'],
            //editItems: ['first', 'last', 'title', 'address', 'city', 'state', 'zip', 'phone', 'fax', 'supervisornum', 'email', 'additionalemailaddresses']
            viewItems: ['first-display', 'last-display', 'address-display', 'city-display', 'state-display', 'zip-display', 'phone-display', 'fax-display', 'supervisornum-display', 'email-display', 'additionalemailaddresses-display'],
            editItems: ['first', 'last', 'address', 'city', 'state', 'zip', 'phone', 'fax', 'supervisornum', 'email', 'additionalemailaddresses']
        },
        listeners: {
            render: function (panel) {
                panel.userMembers.viewMode(panel);
            }
        },
        frame: true,
        title: 'Information',
        url: config.getUrl('public/supervisor/saveinformation'),
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
            //self.getDisplayField({
            //    itemId: 'title-display',
            //    fieldLabel: 'Title',
            //    value: supervisorData.TITLE
            //}),
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
                value: supervisorData.PHONE
            }),
            self.getDisplayField({
                itemId: 'fax-display',
                fieldLabel: 'Fax',
                value: supervisorData.FAX
            }),
            self.getDisplayField({
                itemId: 'supervisornum-display',
                fieldLabel: 'Supervisor number',
                value: supervisorData.SUPERVISORNUM
            }),
            self.getDisplayField({
                itemId: 'additionalemailaddresses-display',
                fieldLabel: 'Additional e-mail(s)',
                value: supervisorData.AdditionalEmailAddresses
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
            //self.getField({
            //    itemId: 'title',
            //    name: 'title',
            //    maxLength: 50,
            //    fieldLabel: 'Title',
            //    value: supervisorData.TITLE
            //}),
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
                fieldLabel: 'Phone',
                maxLength: 50,
                value: supervisorData.PHONE
            }),
            self.getField({
                itemId: 'fax',
                name: 'fax',
                maxLength: 50,
                fieldLabel: 'Fax',
                value: supervisorData.FAX
            }),
            self.getField({
                itemId: 'supervisornum',
                name: 'supervisornum',
                fieldLabel: 'Supervisor number',
                maxLength: 50,
                value: supervisorData.SUPERVISORNUM
            }),
            self.getField({
                xtype: 'textarea',
                itemId: 'additionalemailaddresses',
                name: 'additionalemailaddresses',
                maxLength: 245,
                inputAttrTpl: " data-qtip='Please divide e-mail addresses with semicolon.'",
                fieldLabel: 'Additional e-mail(s)',
                value: supervisorData.AdditionalEmailAddresses
            }),
            self.getDisplayField({
                itemId: 'supervisordistrict',
                fieldLabel: window.LAYOUT.Options.fieldNames.Field3Name,
                hidden: hideSupDistrict,
                value: supervisorDistrict.DISTRICT1
            }),
            self.getDisplayField({
                itemId: 'supervisorschool',
                fieldLabel: window.LAYOUT.Options.fieldNames.Field2Name,
                hidden: hideSupSchool,
                value: SchoolList
            })
        ]
    }));
};
