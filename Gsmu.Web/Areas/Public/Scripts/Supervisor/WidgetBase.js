function WidgetBase() {

}

WidgetBase.constructor = WidgetBase;

WidgetBase.prototype.emptyText = 'This field is empty ...';
WidgetBase.prototype.requiredIndicator = '<font color="red">*</font>&nbsp;';

WidgetBase.prototype.getWidgetDefaults = function (options) {

    var self = this;

    return Ext.merge({
        defaults: {
            anchor: '100%',
            style: {
                marginTop: self.State.dashboard.Options.margins.top,
                marginLeft: self.State.dashboard.Options.margins.left,
                marginRight: self.State.dashboard.Options.margins.right
            }
        },
        fieldDefaults: {
            labelAlign: 'right',
            labelWidth: 120,
            labelClsExtra: 'widget-field'
        },
        userMembers: {
            editMode: function (panel) {
                this.setViewVisibility(panel, false);
                this.setEditVisibility(panel, true);

                if (panel.userMembers.editModeComplete) {
                    panel.userMembers.editModeComplete(panel);
                }
            },
            viewMode: function (panel) {
                this.setViewVisibility(panel, true);
                this.setEditVisibility(panel, false);

                if (panel.userMembers.viewModeComplete) {
                    panel.userMembers.viewModeComplete(panel);
                }
            },
            setViewVisibility: function (panel, visible) {
                var edit = panel.down('#tool-edit');
                edit.setVisible(visible);
                Ext.Array.each(panel.userMembers.viewItems, function (value) {
                    var item = panel.getComponent(value);
                    if (item != undefined) {
                        item.setVisible(visible);
                    }
                });

                if (panel.userMembers.setViewVisibilityComplete) {
                    panel.userMembers.setViewVisibilityComplete(panel);
                }
            },
            setEditVisibility: function (panel, visible) {
                var save = panel.down('#tool-save');
                save.setVisible(visible);
                var cancel = panel.down('#tool-cancel');
                cancel.setVisible(visible);

                Ext.Array.each(panel.userMembers.editItems, function (value) {
                    var item = panel.getComponent(value);
                    if (item != undefined) {
                        item.setVisible(visible);
                    }
                });

                if (panel.userMembers.setEditVisibilityComplete) {
                    panel.userMembers.setEditVisibilityComplete(panel);
                }
            }
        },
        defaultType: 'textfield',
        renderTo: self.State.container,
        tools: [
            {
                itemId: 'tool-edit',
                type: 'edit',
                tooltip: 'Edit',
                callback: function (panel) {
                    panel.userMembers.editMode(panel);
                }
            },
            {
                itemId: 'tool-save',
                type: 'save',
                tooltip: 'Save',
                hidden: true,
                callback: function (panel) {
                    var form = panel.getForm();
                    if (form.isValid()) {
                        form.submit({
                            submitEmptyText: false,
                            waitMsg: 'Saving data  ...',
                            success: function (form, action) {
                                panel.unmask();
                                self.State.dashboard.State.supervisorData = action.result.supervisor;
                                LAYOUT.notify(action.result.message);
                                self.State.widget.destroy();
                                self.Render();
                            },
                            failure: function (form, action) {
                                panel.unmask();
                                LAYOUT.notify(action.result.message);
                            }
                        });
                    } else {
                        LAYOUT.notify('Please check the form. The field(s) marked with red contain invalid data. To see the error message, just move your mouse over the highlighted field.');
                    }
                }
            },
            {
                itemId: 'tool-cancel',
                type: 'cross',
                tooltip: 'Cancel',
                hidden: true,
                callback: function (panel) {
                    var form = panel.getForm();
                    if (form.isValid()) {
                        panel.userMembers.viewMode(panel);
                    } else {
                        LAYOUT.notify('Please fix the invalid field first.');
                    }
                }
            }
        ]
    }, options);
};

WidgetBase.prototype.getDisplayField = function (options) {
    var self = this;

    return Ext.merge({
        xtype: 'displayfield',
        submitValue: false
    }, options);
}

WidgetBase.prototype.getField = function (options) {
    var self = this;

    return Ext.merge({
        emptyText: self.emptyText
    }, options);
}

WidgetBase.prototype.Render = function (container) {

    var self = this;

    if (typeof (container) == 'undefined') {
        self.RenderImplementation();
        return;
    }

    self.State.container = container.appendChild({
        tag: 'div'
    }, true);

    self.RenderImplementation();
}