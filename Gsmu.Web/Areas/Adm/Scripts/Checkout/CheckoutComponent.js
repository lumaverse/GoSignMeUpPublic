function CheckoutComponent(options) {
    var self = this;

    Ext.onDocumentReady(function () {
        self.initialize();
    });
}


CheckoutComponent.constructor = CheckoutComponent;
CheckoutComponent.prototype.ActiveId = null;
CheckoutComponent.prototype.GobalVar = null;

CheckoutComponent.prototype.initialize = function (renderTo) {
    var self = this;

    Ext.Ajax.request({
        url: "/adm/CheckoutSettings/GetCheckoutSettings",
        success: function (response) {
            self.GobalVar = Ext.decode(response.responseText);
            self.Render();
        }
    });
}


CheckoutComponent.prototype.Render = function () {

    var self = this;
    self.buildSettingsTab();
    
    var UserCoursesTab = Ext.create('Ext.tab.Panel', {
        layout: 'fit',
        items: [{
            title: 'Settings' ,
            listeners: {
                activate: function () {
                    Ext.getCmp('UserSettingsPanel').setVisible(true)
                }
            }
        }]
    });


    self.State.Panel = Ext.create('Ext.Panel', {
        title: 'Checkout Settings Manager',
        renderTo: Ext.get("checkout-component-container"),
        frame: true,
        autoScroll: false,
        items: [
            //UserCoursesTab,
            self.settingsPanel
        ]
    });


    Ext.on('resize', function () {
        self.State.Panel.doLayout();
    });

    Ext.QuickTips.init();
}


CheckoutComponent.prototype.buildSettingsTab = function () {
    var self = this;
	
    self.settingsPanel = Ext.create('Ext.form.Panel', {
        bodyPadding: '30 10 0 20',
        height: 400,
        items: [
            {
                id: 'AllowPartialPayment',
                xtype: 'checkbox',
                boxLabel: 'Allow Partial Payment',
                checked: self.GobalVar.AllowPartialPayment,
            },
            {
                xtype: 'button',
                text: 'Save Settings',
                handler: function () {
                    Ext.Ajax.request({
                        url: '/adm/CheckoutSettings/UpdateCheckoutSettings',
                        params: {
                            PartialPayment: Ext.getCmp("AllowPartialPayment").getValue() ? 1 : 0
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




