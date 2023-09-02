Ext.define('EmailRestriction', {
    extend: 'Ext.data.Store',
    model: 'EmailResriction',
    autoLoad: true,
    autoSync: false,
    pageSize: 10,
    remoteFilter: false,
    remoteGroup: false,
    remoteSort: false,
    sorters: [{
        property: 'id',
        direction: 'DESC'
    }],
    proxy: {
        type: 'ajax',
        api: {
            read: config.getUrl('adm/UserSettings/ReadEmailRestrictions'),
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