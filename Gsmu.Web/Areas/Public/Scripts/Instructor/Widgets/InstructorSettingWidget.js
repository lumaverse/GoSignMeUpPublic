function SupervisorSettingWidget(dashboard) {
    var self = this;

    self.State.dashboard = dashboard;
}

SupervisorSettingWidget.prototype = new WidgetBase();

SupervisorSettingWidget.constructor = SupervisorSettingWidget;

SupervisorSettingWidget.prototype.Options = {
};

SupervisorSettingWidget.prototype.State = {
    container: null,
    dashboard: null,
    widget: null
};

SupervisorSettingWidget.prototype.RenderImplementation = function () {
    var self = this;

    var advnaceoptionstip = " data-qtip='Students will be filtered based on the " + LAYOUT.Options.fieldNames.Field3Name + " or " + LAYOUT.Options.fieldNames.Field2Name + " selections made.'";

    var supervisorData = self.State.dashboard.State.supervisorData;

    self.State.widget = Ext.create('Ext.form.Panel', self.getWidgetDefaults({
        fieldDefaults: {
            labelAlign: 'right',
            labelWidth: 200,
            labelClsExtra: 'widget-field'
        },
        userMembers: {
            viewItems: ['advanceoptions-display'],
            editItems: ['advanceoptions']
        },
        listeners: {
            render: function (panel) {
                panel.userMembers.viewMode(panel);
            }
        },
        title: 'Settings',
        url: config.getUrl('public/supervisor/savesettings'),
        icon: config.getUrl('Images/Icons/Famfamfam/cog.png'),
        items: [
            self.getDisplayField({
                itemId: 'advanceoptions-display',
                fieldLabel: 'Students enrollment/edit enabled',
                labelAttrTpl: advnaceoptionstip,
                inputAttrTpl: advnaceoptionstip,
                value: '<img style="position:relative; top: -2px;" src="' + (supervisorData.AdvanceOptions == -1 ? config.getUrl('images/icons/famfamfam/accept.png') : config.getUrl('images/icons/famfamfam/cross.png')) + '"/>'
            }),
            self.getField({
                itemId: 'advanceoptions',
                name: 'advanceoptions',
                xtype: 'checkbox',
                fieldLabel: 'Students enrollment/edit enabled',
                labelAttrTpl: advnaceoptionstip,
                inputAttrTpl: advnaceoptionstip,
                value: supervisorData.AdvanceOptions,
                inputValue: -1
            })
        ]
    }));
};
