Ext.define('AttendanceStatus', {
    extend: 'Ext.data.Model',
    idProperty: 'AttendanceStatusId',
    fields: [
        { name: 'AttendanceStatusId', type: 'int' },
        { name: 'AttendanceStatus', type: 'string' }
    ]
});
 
 
