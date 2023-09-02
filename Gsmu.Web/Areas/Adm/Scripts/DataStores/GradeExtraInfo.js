Ext.define('GradeExtraInfoStore', {
    extend: 'Ext.data.Store',
    model: 'GradeExtraInfo',
    autoLoad: true,
    autoSync: true,
    pageSize: 10,
    remoteFilter: true,
    remoteGroup: true,
    remoteSort: true,
    sorters: [{
        property: 'gexId',
        direction: 'ASC'
    }],
    proxy: {
        type: 'ajax',
        api: {
            read: config.getUrl('adm/datastore/read?entity=GradeExtraInfo'),
            create: config.getUrl('adm/datastore/create?entity=GradeExtraInfo'),
            update: config.getUrl('adm/datastore/update?entity=GradeExtraInfo'),
            destroy: config.getUrl('adm/datastore/destroy?entity=GradeExtraInfo')
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