Ext.define('SchoolsGradeLevelsRelatedStore', {
    extend: 'Ext.data.Store',
    model: 'SchoolsGradeLevelsRelated',
    autoLoad: true,
    autoSync: true,
    pageSize: -1,
    remoteFilter: true,
    remoteSort: true,
    remoteSort: true,
    sorters: [{
        property: 'SchoolsGradeLevelsRelatedId',
        direction: 'ASC'
    }],
    proxy: {
        type: 'ajax',
        api: {
            read: config.getUrl('adm/datastore/read?entity=SchoolsGradeLevelsRelated'),
            create: config.getUrl('adm/datastore/create?entity=SchoolsGradeLevelsRelated'),
            update: config.getUrl('adm/datastore/update?entity=SchoolsGradeLevelsRelated'),
            destroy: config.getUrl('adm/datastore/destroy?entity=SchoolsGradeLevelsRelated')
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