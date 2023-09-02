Ext.define('School', {
    extend: 'Ext.data.Model',
    idProperty: 'ID',
    fields: [
        { name: 'ID', type: 'int' },
        { name: 'locationid', type: 'int' },
        { name: 'LOCATION', type: 'string' },
        { name: 'URL', type: 'string' },
        { name: 'District', type: 'int' },
        { name: 'SortOrder', type: 'int' },
        { name: 'MembershipFlag', type: 'int' },
        { name: 'LocationAlias', type: 'string' },
        { name: 'certid', type: 'int' }
    ]
});


