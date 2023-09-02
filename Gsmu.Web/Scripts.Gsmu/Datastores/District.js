Ext.define('DistrictStore', {
    extend: 'Ext.data.Store',
    model: 'District',
    autoLoad: true,
    autoSync: true,
    pageSize: 10,
    remoteFilter: true,
    remoteGroup: true,
    remoteSort: true,
    sorters: [{
        property: 'DISTRICT1',
        direction: 'ASC'
    }],
    proxy: {
        type: 'ajax',
        api: {
                read: config.getUrl('datastore/read?entity=District')
        },
        reader: {
                type: 'json',
                rootProperty: 'Data',
                totalProperty: 'Count'
        }
    }
});