function LayoutConfirmationScreen() {
    var self = this;

    Ext.onDocumentReady(function () {
        self.State.UrlVars = UrlHelper.getUrlVars();

        if (location.hash != '') {
            self.LoadConfirmation(location.hash.substring(1));
        }
        else if (Ext.isDefined(self.State.UrlVars['order'])) {
            self.LoadConfirmation(self.State.UrlVars['order']);
        } else {
            self.LoadConfirmation('');
        }
    });
    window.LAYOUTCONFIRMATIONSCREEN = this;
}

LayoutConfirmationScreen.prototype.Options = {
    PrintButtonContainerId1: 'confirmation-print-button-container',
    ToolbarContainerId: 'confirmation-toolbar-container',
    ConfirmationContainerId: 'confirmation-container',
    PrintButtonId1: 'confirmation-print-button',
    PrintButtonId2: 'confirmation-print-button2',
    ConfirmationHeaderContainerId: 'confirmation-header-container',
    ConfirmationHeaderEditorContainerId: 'confirmation-header-editor-container',
    ConfirmationFooterContainerId: 'confirmation-footer-container',
    ConfirmationFooterEditorContainerId: 'confirmation-footer-editor-container',

    ConfirmationIsWaitingInputId: 'confirmation-iswaiting',
    ConfirmationHeaderEditorId: 'layout-confirmation-header-editor',
    ConfirmationHeaderOnWaitingListEditorId: 'layout-confirmation-header-wiating-editor',
    ConfirmationFooterEditorId: 'layout-confirmation-footer-editor'
}

LayoutConfirmationScreen.prototype.State = {
    OrderNumber: null,
    UrlVars: {},
    PrintButton1: null,
    PrintButton2: null,
    AdminEditButton: null,
    OrderNumberCombo: null,
    EditButton: null,
    CancelButton: null,
    AdminComponentsRendered: false,
    Values: {
        Header: null,
        HeaderWhenOnWaitingList: null,
        Footer: null
    },
    IsEditing: false,
    ConfirmationHeader: null,
    ConfirmationFooter: null,
    ConfirmationHeaderEditor: null,
    ConfirmationFooterEditor: null,
    HtmlEditors: {
        Header: null,
        HeaderWhenOnWaitingList: null,
        Footer: null
    }
}

LayoutConfirmationScreen.prototype.IsPrinting = function () {
    var self = this;
    return Ext.isDefined(self.State.UrlVars['print']);
}

LayoutConfirmationScreen.prototype.LoadConfirmation = function (orderNumber) {
    var self = this;

    self.State.OrderNumber = orderNumber;
    location.hash = orderNumber;

    if (self.State.PrintButton1 == null) {

       var renderPrintButton = function (renderTo, id) {
            return Ext.create('Ext.button.Button', {
                renderTo: renderTo,
                icon: config.getUrl('images/icons/glyph2/icons16x16/bar-code.png'),
                id: id,
                text: 'Print confirmation',
                handler: function () {
                    var win = window.open(location.pathname + '?order=' + orderNumber + '&print=true');
                },
                listeners: {
                    render: function (button) {
                        var tip = Ext.create('Ext.tip.ToolTip', {
                            target: button.id,
                            anchor: 'left',
                            anchorToTarget: true,
                            showDelay: 0,
                            html: 'When you choose printing options, please enable printing background colors and images. You may also want to disable headers and footers.'
                        });
                    }
                }
            });        
        }

        self.State.PrintButton1 = renderPrintButton(self.Options.PrintButtonContainerId1, self.Options.PrintButtonId1);
        self.State.PrintButton2 = renderPrintButton(self.Options.ToolbarContainerId, self.Options.PrintButtonId2);
    }

    if (!self.IsPrinting()) {
        self.State.PrintButton1.show();
        self.State.PrintButton2.show();
    } else {
        self.State.PrintButton1.hide();
        self.State.PrintButton2.hide();
    }

    var confirmationContainer = Ext.get(self.Options.ConfirmationContainerId);
    confirmationContainer.mask('Loading confirmation #' + orderNumber);

    Ext.Ajax.request({
        url: config.getUrl('adm/confirmationscreen/ConfirmationPartial?order=' + orderNumber),
        success: function (response) {
            confirmationContainer.setHtml(response.responseText, true);
            confirmationContainer.unmask();

            self.State.ConfirmationHeader = Ext.get(self.Options.ConfirmationHeaderContainerId);
            self.State.ConfirmationFooter = Ext.get(self.Options.ConfirmationFooterContainerId);
            self.State.ConfirmationHeader.enableDisplayMode();
            self.State.ConfirmationFooter.enableDisplayMode();

            if (self.IsPrinting()) {
                self.State.AdminEditButton.hide();
                self.State.OrderNumberCombo.hide();
                window.print();
                return;
            }
            self.DisplayAdminComponents(self.State.IsEditing);
        }
    });
}


LayoutConfirmationScreen.prototype.getUploadData = function (area) {
    var self = this;

    if (area == 'HeaderWhenOnWaitingList') {
        area = 'HeaderOnWaitingList';
    }
    var editorId = self.Options['Confirmation' + area + 'EditorId'];
    return {
        url: 'adm/confirmationscreen/UploadConfirmationContentFile',
        editor: Ext.getCmp(editorId)
    };
}

LayoutConfirmationScreen.prototype.RenderAdminComponents = function (complete) {
    var self = this;

    var edit = Ext.getCmp('layout-confirmation-menu-edit');
    if (!edit)
    {
        return;
    }
    if (self.State.AdminComponentsRendered) {
        complete();
        return;
    }

    var renderComponents = function (values) {

        self.State.Values = values;

        self.State.ConfirmationHeaderEditor = Ext.create('Ext.tab.Panel', {
            renderTo: self.Options.ConfirmationHeaderEditorContainerId,
            height: 250,
            items: [
                {
                    xtype: 'gsmuhtmleditor',
                    id: self.Options.ConfirmationHeaderEditorId,
                    value: self.State.Values.Header,
                    title: 'Header'
                },
                {
                    xtype: 'gsmuhtmleditor',
                    id: self.Options.ConfirmationHeaderOnWaitingListEditorId,
                    value: self.State.Values.HeaderWhenOnWaitingList,
                    title: 'Header when on waiting list'
                }
            ]
        });

        self.State.ConfirmationFooterEditor = Ext.create('Ext.panel.Panel', {
            height: 200,
            title: 'Footer',
            renderTo: self.Options.ConfirmationFooterEditorContainerId,
            layout:{
                type:'fit',
                align:'stretch',
                pack:'start'
            },
            items: [
                {
                    xtype: 'gsmuhtmleditor',
                    id: self.Options.ConfirmationFooterEditorId,
                    value: self.State.Values.Footer
                }
            ]
        });

        self.State.HtmlEditors.Header = Ext.getCmp(self.Options.ConfirmationHeaderEditorId);
        self.State.HtmlEditors.Footer = Ext.getCmp(self.Options.ConfirmationFooterEditorId);
        self.State.HtmlEditors.HeaderWhenOnWaitingList = Ext.getCmp(self.Options.ConfirmationHeaderOnWaitingListEditorId);

        for (var key in self.State.HtmlEditors) {
            var editor = self.State.HtmlEditors[key];

            var editorToolbar = editor.getToolbar();
            editorToolbar.add({
                xtype: 'tbfill'
            });

            var undoAll = function () {
                for (var key in self.State.HtmlEditors) {
                    var editor = self.State.HtmlEditors[key];
                    editor.setValue(self.State.Values[key]);
                }
            }

            editorToolbar.add({
                tooltip: {
                    title: 'Insert file',
                    text: 'If the file you upload does not appear, make sure to place the cursor in the editor window to a valid location before you upload.'
                },
                editorKey: key,
                icon: config.getUrl(window.LAYOUT.Options.uploadIcon),
                handler: function (button) {
                    window.LAYOUT.EnsureUploadForm();
                    window.LAYOUT.State.uploadForm.getForm().reset();
                    window.LAYOUT.State.uploadFormHiddenField.setValue(button.editorKey);
                    window.LAYOUT.State.uploadFormSourceHiddenField.setValue('window.LAYOUTCONFIRMATIONSCREEN.getUploadData("'  + button.editorKey + '");');
                    window.LAYOUT.State.uploadWindow.show();
                    window.LAYOUT.State.uploadWindow.center();
                }
            });

            editorToolbar.add({
                tooltip: {
                    text: 'Undo all'
                },
                icon: config.getUrl('images/icons/famfamfam/arrow_rotate_clockwise.png'),
                handler: function () {
                    if (window.confirm('Are you sure to undo all changes in all editors?')) {
                        undoAll();
                    }
                }
            });
            editorToolbar.add({
                tooltip: {
                    text: 'Undo field'
                },
                editorKey: key,
                icon: config.getUrl('images/icons/famfamfam/arrow_undo.png'),
                handler: function (cmp) {
                    if (window.confirm('Are you sure to undo all changes in this editor?')) {
                        var editor = self.State.HtmlEditors[cmp.editorKey];
                        editor.setValue(self.State.Values[cmp.editorKey]);
                    }
                }
            });
            editorToolbar.add({
                tooltip: {
                    text: 'Save changes to all fields'
                },
                icon: config.getUrl('images/icons/famfamfam/disk.png'),
                handler: function () {
                    window.LAYOUT.MaskLayout('Saving confirmation screen values...');
                    self.State.Values.HeaderWhenOnWaitingList = self.State.HtmlEditors.HeaderWhenOnWaitingList.getValue();
                    self.State.Values.Footer = self.State.HtmlEditors.Footer.getValue();
                    self.State.Values.Header = self.State.HtmlEditors.Header.getValue();
                    Ext.Ajax.request({
                        method: 'POST',
                        url: config.getUrl('adm/confirmationscreen/confirmationscreendata'),
                        params: self.State.Values,
                        success: function () {
                            window.LAYOUT.UnmaskLayout();
                            self.DisplayAdminComponents(false);
                            if (self.LoadedOrderIsWaiting()) {
                                self.State.ConfirmationHeader.setHtml(self.State.Values.HeaderWhenOnWaitingList);
                            } else {
                                self.State.ConfirmationHeader.setHtml(self.State.Values.Header);
                            }
                            self.State.ConfirmationFooter.setHtml(self.State.Values.Footer);
                        }
                    });
                }
            });

            editorToolbar.add({
                tooltip: {
                    text: 'Cancel'
                },
                icon: config.getUrl('images/icons/famfamfam/delete.png'),
                handler: function () {
                    if (window.confirm('Are you sure to cancel all changes in all editors?')) {
                        undoAll();
                        self.DisplayAdminComponents(false);
                    }
                }
            });
        }

        complete();
        self.State.AdminComponentsRendered = true;
    }

    Ext.Ajax.request({
        method: 'GET',
        url: config.getUrl('adm/confirmationscreen/ConfirmationScreenData'),
        success: function (response) {
            var values = Ext.decode(response.responseText);
            renderComponents(values);
        }
    });

}

LayoutConfirmationScreen.prototype.DisplayAdminComponents = function (status) {
    var self = this;


    self.RenderAdminComponents(function () {
        self.State.IsEditing = status;
        if (status) {
            self.State.ConfirmationHeader.hide();
            self.State.ConfirmationFooter.hide();
            self.State.ConfirmationFooterEditor.show();
            self.State.ConfirmationHeaderEditor.show();

            self.State.AdminEditButton.hide();

            var activeTab = self.LoadedOrderIsWaiting() ? 1 : 0;
            self.State.ConfirmationHeaderEditor.setActiveTab(activeTab);
        } else {
            self.State.AdminEditButton.show();

            self.State.ConfirmationHeader.show();
            self.State.ConfirmationFooter.show();
            self.State.ConfirmationFooterEditor.hide();
            self.State.ConfirmationHeaderEditor.hide();
        }
    });
}

LayoutConfirmationScreen.prototype.LoadedOrderIsWaiting = function () {
    var self = this;

    return Ext.get(self.Options.ConfirmationIsWaitingInputId).getValue() == 'true';
}

LayoutConfirmationScreen.prototype.RenderAdmin = function () {
    var self = this;

    var layoutOrderDataStore = LayoutOrderDataStore.getStore();

    var self = this;

    self.State.OrderNumberCombo = Ext.create('Ext.form.field.ComboBox', {
        renderTo: 'confirmation-toolbar-admin-combo',
        xtype: 'combobox',
        id: 'layout-menu-confirmation-ordernumber',
        emptyText: 'Keyword or order number',
        minWidth: 180,
        store: layoutOrderDataStore,
        queryMode: 'remote',
        displayField: 'OrderNumber',
        hideLabel: true,
        anchor: '100%',
        typeAhead: false,
        forceSelection: true,
        matchFieldWidth: false,
        width: 200,
        listConfig: {
            loadingText: 'Searching...',
            emptyText: 'No matching orders found.',
            width: 200,

            // Custom rendering template for each item
            getInnerTpl: function () {
                return '<div style="font-weight: bold;">{OrderNumber}</div><div>{[Ext.Date.format(values.OrderDate, "m/d/Y")]}</div><div style="border-bottom: 1px solid #000000;">{StudentFirst} {StudentLast}</div>';
            }
        },
        listeners: {
            select: function (combo, values) {
               // var value = values[0];
                var orderNumber = values.get('OrderNumber');

                if (window.LAYOUTCONFIRMATIONSCREEN) {
                    window.LAYOUTCONFIRMATIONSCREEN.LoadConfirmation(orderNumber);
                } else {
                    document.location = config.getUrl('adm/confirmationscreen/confirmation?order=' + orderNumber);
                }
            },
            render: function (control) {
                control.popupTip = Ext.create('Ext.tip.ToolTip', {
                    target: control.id,
                    anchor: 'left',
                    left: '10px',
                    showDelay: 0,
                    hideDelay: 5000,
                    autoHide: true,
                    mouseOffset: [0, -60],
                    html: 'Enter the order number or student name fragment here and configure the confirmation screen for that order'
                });
            }
        }
    });

    self.State.AdminEditButton = Ext.create('Ext.Button', {
        renderTo: 'confirmation-toolbar-admin-edit',
        id: 'layout-confirmation-menu-edit',
        style: {
            marginLeft: '10px'
        },
        icon: config.getUrl('images/icons/famfamfam/pencil.png'),
        text: 'Edit headers and footers',
        handler: function (cmp, checked) {

            self.DisplayAdminComponents(true);
            cmp.hide();
        }
    });

}