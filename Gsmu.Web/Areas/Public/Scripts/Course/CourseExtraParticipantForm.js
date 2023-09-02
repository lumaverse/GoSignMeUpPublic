
function CourseExtraParticipantForm(courseId, courseModifier, cartOptions, stats, ExtraParticipantRequiredFields) {
    var self = this;

    self.Options.CourseId = courseId;
    self.Options.ExtraParticipantRequiredFieldsText = ExtraParticipantRequiredFields;
    self.Options.CourseDisplayModifier = courseModifier;
    self.Options.EnrollmentStatistics = stats;
    self.Options.ExtraParticipantCustomFieldRequired = extraParticipantCustomFieldLabelRequired;

    Ext.merge(self.Options, cartOptions);
    if ((ExtraParticipantRequiredFields != undefined && ExtraParticipantRequiredFields !=null) && ExtraParticipantRequiredFields != "all" && ExtraParticipantRequiredFields != "") {
        if (ExtraParticipantRequiredFields.toLowerCase().indexOf("first") > -1) {
            self.Options.ExtraParticipantRequiredFields.firstname = true;
        }
        else {
            self.Options.ExtraParticipantRequiredFields.firstname = false;
        }
        if (ExtraParticipantRequiredFields.toLowerCase().indexOf("last") > -1) {
            self.Options.ExtraParticipantRequiredFields.lastname = true;
        }
        else {
            self.Options.ExtraParticipantRequiredFields.lastname = false;
        }
        if (ExtraParticipantRequiredFields.toLowerCase().indexOf("email") > -1) {
            self.Options.ExtraParticipantRequiredFields.email = true;
        }
        else {
            self.Options.ExtraParticipantRequiredFields.email = false;
        }
        if (ExtraParticipantRequiredFields.toLowerCase().indexOf("custom") > -1) {
            self.Options.ExtraParticipantRequiredFields.customfield = extraParticipantCustomFieldLabelRequired;
        }
        else {
            self.Options.ExtraParticipantRequiredFields.customfield = extraParticipantCustomFieldLabelRequired;
        }
    }
    if (self.Options.EnrollmentStatistics.SpaceAvailable < 1 && self.Options.EnrollmentStatistics.WaitSpaceAvailable < 1) {
        config.showWarning('We apologize for the inconvenience but the course you are trying to add with the pricing option does not have at least one available spaces.', 'Not enough space available in course');
        //and besides yourself you do need to add at least one ' + self.Options.ExtraParticipantLabel + ' member in this setting. -- THIS IS THE REMOVED PART OF THE MESSAGE
        window.LAYOUT.UnmaskLayout(); //Removes the loading mask issue on the waitspace dialog and closing the course detail
        return;
    }

    Ext.onDocumentReady(function () {
        self.ShowForm();
    });
}

CourseExtraParticipantForm.prototype.constructor = CourseExtraParticipantForm;

CourseExtraParticipantForm.prototype.Options = {
    ExtraParticipantCollectionEnabled: false,
    ExtraParticipantRequiredFieldsText:'all',
    ExtraParticipantLabel: null,
    ExtraParticipantCollectCustomField: false,
    ExtraParticipantCustomFieldLabel: null,
    CourseId: null,
    CourseDisplayModifier: null,
    EnrollmentStatistics: {
        SpaceAvailable: null,
        WaitSpaceAvailable: null,
        AvailableRosterCount: null,
        EnrolledRosterCount: null,
        EnrollmentStatus: null,
        MaxEnrolledRosterCount: null,
        MaxWaitingRosterCount: null,
        Status: null,
        TotalRosterCount: null,
        WaitingRosterCount: null
    },
    ExtraParticipantRequiredFields: {
        firstname: true,
        lastname: true,
        email: true,
        customfield: extraParticipantCustomFieldLabelRequired

    }
};

CourseExtraParticipantForm.prototype.ShowForm = function () {
    var self = this;

    var currentModel =null;
    if (self.Options.ExtraParticipantRequiredFieldsText != "all" && self.Options.ExtraParticipantRequiredFieldsText != "") {
        currentModel = 'CourseExtraParticipantValidatedConfigs'
    }
    else {
        currentModel = self.Options.ExtraParticipantCollectCustomField ? 'CourseExtraParticipantValidatedCustomField' : 'CourseExtraParticipantValidatedRegular';
    }

    var selector = '[data-course-id=\'' + self.Options.CourseId + '\']';
    var courseInfo = Ext.select(selector).first();
    var courseName = courseInfo.getAttribute('data-course-name');

    var title = Ext.String.capitalize(self.Options.ExtraParticipantLabel) + ' members for ' + courseName + ' </br>* PLEASE DO NOT INCLUDE YOURSELF ON THIS FORM';

    var addNewButton = Ext.create('Ext.Button', {
        text: '<div style="font-size:14px;">Add new ' + self.Options.ExtraParticipantLabel + ' member</div>',
        icon: config.getUrl('images/icons/famfamfam/user_add.png'),
        handler: function () {
            if (self.Options.ExtraParticipantRequiredFieldsText != "all" && self.Options.ExtraParticipantRequiredFieldsText != "") {
                var newRecord = CourseExtraParticipantValidatedConfigs.create({
                    validators: [
                        { type: 'length', field: 'StudentFirst', min: 0, max: 50 },
                        { type: 'length', field: 'StudentLast', min: 1, max: 50 },
                        { type: 'email', field: 'StudentEmail' },
                        { type: 'length', field: 'StudentEmail', min: 5, max: 100 },
                    ]

                });
                store.add(newRecord);
            }
            else {
                var newRecord = self.Options.ExtraParticipantCollectCustomField ? CourseExtraParticipantValidatedCustomField.create({}) : CourseExtraParticipantValidatedRegular.create({});
                store.add(newRecord);
            }
        }
    });

    var spaceAvailable = self.Options.EnrollmentStatistics.SpaceAvailable;
    var spaceWait = self.Options.EnrollmentStatistics.WaitSpaceAvailable;

    var infoDisplayTextTemplateSpace = '<img style="vertical-align: top;" src="' + config.getUrl('images/icons/famfamfam/information.png') + '"/> Space available: {0}'
    var infoDisplayTextTemplateWaitSpace = '<img style="vertical-align: top;" src="' + config.getUrl('images/icons/famfamfam/information.png') + '"/> Wait space available: {0}'

    var getCurrentStatus = function (available, type) {
		if(type)
		{
			if (type == 'spaceAvailable')
				return infoDisplayTextTemplateSpace.replace('{0}', available);
			else
				return infoDisplayTextTemplateWaitSpace.replace('{0}', available);
		}
    }

	var infoDisplaySpaceAvailable = Ext.create('Ext.toolbar.TextItem', {
	    text: '<div style="font-size:14px;">' + getCurrentStatus(spaceAvailable, 'spaceAvailable') + '</div>'
    });

    var infoDisplaySpaceWait = Ext.create('Ext.toolbar.TextItem', {
        text: '<div style="font-size:14px;">'+ getCurrentStatus(spaceWait, 'spaceWait') +'</div>',
        hidden: spaceWait == 0 ? true : false
    });



    var toolbar = Ext.create('Ext.toolbar.Toolbar', {
        frame: false,
        border: 0,
        region: 'north',
        height: 28,
        items: [
            infoDisplaySpaceAvailable,
            {
                xtype: 'tbfill'
            },
            infoDisplaySpaceWait,
            {
                xtype: 'tbfill'
            },
            addNewButton
        ]
    });
    var windowButtons = [];

    var saveButton = Ext.create('Ext.Button', {
        height:30,
        text: '<div style="font-size:14px;">'+'Add course with members to cart'+'</div>',
        handler: function () {
            var count = store.count();
            if (count < 1) {
                Ext.MessageBox.show({
                    title: 'Missing participant!',
                    msg: 'Please enter at least one participant!',
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.window.MessageBox.WARNING,
                });
                return;
            }
            var errorShown = false;
            var extraParticipants = [];
            var missingFields = "";
            store.each(function (record) {
                var errors = record.validate();
                if (!errors.isValid()) {
                    errorShown = true;
                    return;
                }
                if (self.Options.ExtraParticipantRequiredFields.firstname) {
                    if (record.data.StudentFirst == "") {
                        errorShown = true;
                        missingFields = "First name";
                        return;
                    }
                }
                if (self.Options.ExtraParticipantRequiredFields.lastname) {
                    if (record.data.StudentLast == "") {
						missingFields = " Last name";
                        errorShown = true;
                        return;
                    }
                }
                if (self.Options.ExtraParticipantRequiredFields.email) {
                    if (record.data.StudentEmail == "") {
                        errorShown = true;
                        return;
                    }
                }
                if (self.Options.ExtraParticipantCollectCustomField) {
                    if (self.Options.ExtraParticipantRequiredFields.customfield) {
                        if (record.data.CustomField2 == "") {
							missingFields = self.Options.ExtraParticipantCustomFieldLabel;
                            errorShown = true;
                            return;
                        }
                    }
                }
                extraParticipants.push(record.data);
            });
            if (errorShown) {
                Ext.MessageBox.show({
                    title: 'Validation error!',
                    msg: 'Field (' + missingFields + ') is missing! Please make sure all member fields are filled out with valid data!',
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.window.MessageBox.WARNING,
                });
            } else {
                window.destroy();
                cart.AddCourse(self.Options.CourseId, self.Options.CourseDisplayModifier, extraParticipants);
            }
        },
        icon: config.getUrl('images/icons/famfamfam/user_go.png'),
        disabled: true
    });
    var cancelButton = Ext.create('Ext.Button', {
        height: 30,
        text: '<div style="font-size:14px;">' + 'Cancel' + '</div>',
        handler: function () {
            window.destroy();
        },
        icon: config.getUrl('images/icons/glyph2/icons16x16/cancel.png')
    });

    windowButtons.push(saveButton);
    windowButtons.push(cancelButton);

    var store = Ext.create('Ext.data.Store', {
        xtype: 'store',
        autoLoad: true,
        model: currentModel,
        proxy: {
            type: 'memory',
            reader: {
                type: 'json',
                rootProperty: 'CourseExtraParticipant'
            }
        }
    });


    store.on('datachanged', function () {
        var count = store.count();
        if (count < 1) {
            saveButton.setDisabled(true);
        } else {
            saveButton.setDisabled(false);
        }

        var currentAvailable = spaceAvailable > 0 ? spaceAvailable - count : spaceWait - count;
        currentAvailable = currentAvailable - 1;
        addNewButton.setDisabled( currentAvailable < 1);
        var status = '';
        if (spaceAvailable > 0)
        {
			status = getCurrentStatus(Math.max(0, currentAvailable), 'spaceAvailable');
			infoDisplaySpaceAvailable.setText('<div style="font-size:14px;">' + status +'</div>');
        }
        else if (spaceAvailable == 0 && spaceWait > 0)
        {
			status = getCurrentStatus(Math.max(0, currentAvailable), 'spaceWait');
			infoDisplaySpaceWait.setText('<div style="font-size:14px;">' + status + '</div>');
        }

    })
    var gridColumns = [
            {
                text: '<div style="font-size:14px;">'+'First name'+'</div>',
                dataIndex: 'StudentFirst',
                flex: 1,
                editor: {
                    xtype: 'textfield',
                    minLength: 1,
                    maxLength: 50,
                    allowBlank: !self.Options.ExtraParticipantRequiredFields.firstname
                }
            },
            {
                text: '<div style="font-size:14px;">' + 'Last name' + '</div>',
                dataIndex: 'StudentLast',
                flex: 1,
                editor: {
                    xtype: 'textfield',
                    minLength: 1,
                    maxLength: 50,
                    allowBlank: !self.Options.ExtraParticipantRequiredFields.lastname
                }
            },
            {
                text: '<div style="font-size:14px;">' + 'E-mail'+'</div>',
                dataIndex: 'StudentEmail',
                flex: 1,
                editor: {
                    xtype: 'textfield',
                    minLength: 5,
                    maxLength: 100,
                    allowBlank: !self.Options.ExtraParticipantRequiredFields.email,
                    vtype: 'email'
                }
            }
    ];

    if (self.Options.ExtraParticipantCollectCustomField) {
        gridColumns.push({
            text: '<div style="font-size:14px;">' + self.Options.ExtraParticipantCustomFieldLabel + '</div>',
            flex: 1,
            dataIndex: 'CustomField2',
            editor: {
                xtype: 'textfield',
                minLength: 1,
                maxLength: 50,
                allowBlank: extraParticipantCustomFieldLabelRequired == true ? false : true
            }
        })
    }

    gridColumns.push({
        xtype: 'actioncolumn',
        width: 25 * 1,
        items: [
            {
                icon: config.getUrl('images/icons/famfamfam/delete.png'),
                tooltip: 'Delete',
                handler: function (grid, rowIndex, colIndex) {

                    Ext.MessageBox.show({
                        title: 'Confirm Delete',
                        msg: 'If you click OK, the record you have selected will be deleted.',
                        buttons: Ext.MessageBox.OKCANCEL,
                        icon: Ext.window.MessageBox.WARNING,
                        fn: function (buttonId) {
                            if (buttonId == 'ok') {
                                store.removeAt(rowIndex);
                            }
                        }
                    });
                }
            }
        ]
    });


    var grid = Ext.create('Ext.grid.Panel', {
        layout: 'fit',
        selType: 'cellmodel',
        plugins: [
            {
                ptype: 'cellediting',
                clicksToEdit: 1
            }
        ],
        viewConfig: {
            emptyText: '<div style="font-size:14px;">' + 'There are no ' + self.Options.ExtraParticipantLabel + ' members assigned.' + '</div>',
            deferEmptyText: false
        },
        listeners: {
            edit: function (grid, e) {
                e.record.commit();
            }
        },
        frame: false,
        border: 0,
        region: 'center',
        cls: 'extrprtCSS',
        store: store,
        columns: gridColumns
    });

    var window = new Ext.Window({
        icon: config.getUrl('images/icons/famfamfam/user_suit.png'),
        title: '<div style="font-size:14px;">'+ title +'</div>',
        listeners : {
            afterrender : function(panel) {
                var header = panel.header;
                header.setHeight(40);
            }
        },
        width: 800,
        height: 500,
        layout: 'fit',
        frame: false,
        modal: true,
        items: [
            {
                xtype: 'panel',
                frame: false,
                border: 0,
                layout: 'border',
                items: [
                    toolbar,
                    grid
                ]
            }
        ],
        buttons: windowButtons
    });

    window.show();
}

