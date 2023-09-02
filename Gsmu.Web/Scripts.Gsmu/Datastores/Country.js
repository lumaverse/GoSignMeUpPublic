Ext.define('CountryStore', {
    extend: 'Ext.data.Store',
    model: 'Country',
    autoLoad: true,
    autoSync: true,
    pageSize: 10,
    remoteFilter: true,
    remoteGroup: true,
    remoteSort: true,
    sorters: [{
        property: 'countryorder',
        direction: 'ASC'
    }],
    proxy: {
        type: 'ajax',
        api: {
            read: config.getUrl('datastore/read?entity=Country')
        },
        reader: {
            type: 'json',
            rootProperty: 'Data',
            totalProperty: 'Count'
        }
    }
});