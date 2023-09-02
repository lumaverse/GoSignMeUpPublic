Ext.define('SchoolExtraInfoStore', {
    extend: 'Ext.data.Store',
    model: 'SchoolExtraInfo',
    autoLoad: true,
    autoSync: true,
    pageSize: 10,
    remoteFilter: true,
    remoteGroup: true,
    remoteSort: true,
    sorters: [{
        property: 'sexid',
        direction: 'ASC'
    }],
    proxy: {
        type: 'ajax',
        api: {
            read: config.getUrl('adm/datastore/read?entity=SchoolExtraInfo'),
            create: config.getUrl('adm/datastore/create?entity=SchoolExtraInfo'),
            update: config.getUrl('adm/datastore/update?entity=SchoolExtraInfo'),
            destroy: config.getUrl('adm/datastore/destroy?entity=SchoolExtraInfo')
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