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
                read: config.getUrl('adm/datastore/read?entity=District'),
                create: config.getUrl('adm/datastore/create?entity=District'),
                update: config.getUrl('adm/datastore/update?entity=District'),
                destroy: config.getUrl('adm/datastore/destroy?entity=District')
        },
        reader: {
                type: 'json',
                rootProperty: 'Data',
                totalProperty: 'Count'
        },
        writer: {
                type: 'json',
                writeAllFields: false
        }
    }
});