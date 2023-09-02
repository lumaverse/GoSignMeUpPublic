Ext.define('GradeExtraInfo', {
    extend: 'Ext.data.Model',
    idProperty: 'gexId',
    fields: [
        { name: 'gexId', type: 'int' },
        { name: 'gradeshortdesc', type: 'string' },
        { name: 'manufacturer', type: 'string' },
        { name: 'gaddress', type: 'string' },
        { name: 'gcity', type: 'string' },
        { name: 'gstate', type: 'string' },
        { name: 'gzip', type: 'string' },
        { name: 'gradeid', type: 'int' }
    ]
});


