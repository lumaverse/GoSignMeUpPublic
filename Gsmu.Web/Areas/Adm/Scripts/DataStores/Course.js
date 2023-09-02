Ext.define('CourseStore', {
    extend: 'Ext.data.Store',
    model: 'Course',
    autoLoad: true,
    autoSync: true,
    pageSize: 10,
    remoteFilter: true,
    remoteGroup: true,
    remoteSort: true,
    sorters: [{
        property: 'COURSENAME',
        direction: 'ASC'
    }],
    proxy: {
        type: 'ajax',
        api: {
            read: config.getUrl('adm/datastore/read?entity=Course'),
            create: config.getUrl('adm/datastore/create?entity=Course'),
            update: config.getUrl('adm/datastore/update?entity=Course'),
            destroy: config.getUrl('adm/datastore/destroy?entity=Course')
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