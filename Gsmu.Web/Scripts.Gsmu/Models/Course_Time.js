Ext.define('Course_Time', {
    extend: 'Ext.data.Model',
    idProperty: 'ID',
    fields: [
        { name: 'ID', type: 'int' },
        { name: 'COURSEID', type: 'int' },
        { name: 'COURSEDATE', type: 'date' },
        { name: 'STARTTIME', type: 'date' },
        { name: 'FINISHTIME', type: 'date' }
    ]
});


