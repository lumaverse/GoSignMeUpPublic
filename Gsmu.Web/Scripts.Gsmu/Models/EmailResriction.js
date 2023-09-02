Ext.define('EmailResriction', {
    extend: 'Ext.data.Model',
    idProperty: 'id',
    fields: [
        {name: 'id', type: 'int'},
        {name: 'email', type: 'string' },
        {name: 'grp', type: 'string'}
]
});

