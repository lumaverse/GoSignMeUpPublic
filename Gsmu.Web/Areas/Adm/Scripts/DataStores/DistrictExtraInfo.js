Ext.define('DistrictExtraInfoStore', {
    extend: 'Ext.data.Store',
    model: 'DistrictExtraInfo',
    autoLoad: true,
    autoSync: true,
    pageSize: 10,
    remoteFilter: true,
    remoteGroup: true,
    remoteSort: true,
    sorters: [{
        property: 'dexid',
        direction: 'ASC'
    }],
    proxy: {
        type: 'ajax',
        api: {
            read: config.getUrl('adm/datastore/read?entity=DistrictExtraInfo'),
            create: config.getUrl('adm/datastore/create?entity=DistrictExtraInfo'),
            update: config.getUrl('adm/datastore/update?entity=DistrictExtraInfo'),
            destroy: config.getUrl('adm/datastore/destroy?entity=DistrictExtraInfo')
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