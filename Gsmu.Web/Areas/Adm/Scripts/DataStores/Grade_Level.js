Ext.define('GradeLevelStore', {
    extend: 'Ext.data.Store',
    model: 'Grade_Level',
    autoLoad: true,
    autoSync: true,
    pageSize: 10,
    remoteFilter: true,
    remoteGroup: true,
    remoteSort: true,
    sorters: [{
        property: 'GRADE',
        direction: 'ASC'
    }],
    proxy: {
        type: 'ajax',
        api: {
            read: config.getUrl('adm/datastore/read?entity=Grade_Level'),
            create: config.getUrl('adm/datastore/create?entity=Grade_Level'),
            update: config.getUrl('adm/datastore/update?entity=Grade_Level'),
            destroy: config.getUrl('adm/datastore/destroy?entity=Grade_Level')
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