LayoutOrderDataStore = {
    getStore: function () {
        var store = Ext.create('Ext.data.Store', {
            pageSize: 100,
            model: 'LayoutOrderModel',
            proxy: {
                type: 'ajax',
                url: config.getUrl('adm/confirmationscreen/confirmationquery'),
                reader: {
                    type: 'json',
                    rootProperty: 'Result',
                    totalProperty: 'TotalCount',
                    listeners: {
                        exception: function (reader, response, error, opts) {
                            log(error);
                        }
                    }
                }
            }
        });
        return store;
    }
}