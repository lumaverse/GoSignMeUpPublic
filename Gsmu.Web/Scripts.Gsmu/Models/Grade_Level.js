Ext.define('Grade_Level', {
    extend: 'Ext.data.Model',
    idProperty: 'GRADEID',
    fields: [
        { name: 'GRADEID', type: 'int' },
        { name: 'GRADE', type: 'string' },
        { name: 'SortOrder', type: 'int' },
        { name: 'SchoolId', type: 'int' }
    ]
});


