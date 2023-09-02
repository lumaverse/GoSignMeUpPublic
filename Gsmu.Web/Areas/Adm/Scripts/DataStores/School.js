Ext.define('SchoolStore', {
    extend: 'Ext.data.Store',
    model: 'School',
    autoLoad: true,
    autoSync: true,
    pageSize: 10,
    remoteFilter: true,
    remoteGroup: true,
    remoteSort: true,
    sorters: [{
        property: 'LOCATION',
        direction: 'ASC'
    }],
    proxy: {
        type: 'ajax',
        api: {
            read: config.getUrl('adm/datastore/read?entity=School'),
            create: config.getUrl('adm/datastore/create?entity=School'),
            update: config.getUrl('adm/datastore/update?entity=School'),
            destroy: config.getUrl('adm/datastore/destroy?entity=School')
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