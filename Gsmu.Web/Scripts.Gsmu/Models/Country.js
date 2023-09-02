Ext.define('Country', {
    extend: 'Ext.data.Model',
    idProperty: 'ccodeid',
    fields: [
        { name: 'ccodeid', type: 'int' },
        { name: 'countrycode', type: 'string' },
        { name: 'countryname', type: 'string' },
        { name: 'disabled', type: 'int' },
        { name: 'countryorder', type: 'int' }
    ]
});


