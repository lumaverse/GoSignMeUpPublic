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
            read: config.getUrl('adm/datastore/read?entity=Country'),
            create: config.getUrl('adm/datastore/create?entity=Country'),
            update: config.getUrl('adm/datastore/update?entity=Country'),
            destroy: config.getUrl('adm/datastore/destroy?entity=Country')
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