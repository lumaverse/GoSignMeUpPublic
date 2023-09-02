function InstructorDashboard(options) {
    var self = this;

    self.Options = Ext.merge(self.Options, options);

    Ext.onReady(function () {
        self.State.containers.left = Ext.get(self.Options.containers.left);
        self.State.containers.right = Ext.get(self.Options.containers.right);
        self.State.containers.center = Ext.get(self.Options.containers.center);
    });

    Ext.Ajax.request({
        method: 'post',
        url: config.getUrl('public/instructor/getdata'),
        success: function (response) {
            self.State.instructorData = Ext.decode(response.responseText);

            Ext.onReady(function () {
                self.Render();
            })

        },
        failure: function () {
            Ext.onReady(function () {
                Ext.Msg.show({
                    title: 'Error',
                    message: 'Error during loading supervisor data. Click OK to reload page or contact customer support',
                    buttons: Ext.Msg.OK,
                    icon: Ext.Msg.WARNING,
                    fn: function (btn) {
                        location.reload();
                    }
                });
            });
        }
    });
}

InstructorDashboard.constructor = InstructorDashboard;

InstructorDashboard.prototype.Options = {
    containers: {
        left: 'supervisor-left',
        center: 'supervisor-center'

    },
    margins: {
        left: '5px',
        right: '5px',
        top: '5px',
        bottom: '5px'
    },
    noProfileImage: null,
    profileImagePath: null,
    profileImageMaxWidth: null,
    profileImageMaxHeight: null,
    widgets: {
        left: [],
        center: [],
        right: []
    }
};

InstructorDashboard.prototype.State = {
    instructorData: null,
    containers: {
        left: null,
        center: null,
        right: null
    },
    widgets: {}
};

InstructorDashboard.prototype.Render = function () {
    var self = this;

    Object.keys(self.Options.widgets).forEach(function (value, index, array) {
        var container = self.State.containers[value];
        Ext.Array.forEach(self.Options.widgets[value], function (widgetName, arrayIndex, array) {
            var widget = null;
            eval("widget = new " + widgetName + "(InstructorDashboard.prototype);");
            //widget = new SupervisorIdentityWidget(self);
            self.State.widgets[widgetName] = widget;
            widget.Render(container);
            if (arrayIndex < array.length - 1) {
                container.appendChild({
                    tag: 'div',
                    style: 'height: ' + self.Options.margins.top + ';'
                });
            }
        });
    });
}
