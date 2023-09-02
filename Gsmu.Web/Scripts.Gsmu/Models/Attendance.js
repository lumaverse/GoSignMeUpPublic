Ext.define('Attendance', {
    extend: 'Ext.data.Model',
    idProperty: 'AttendanceId',
    fields: [
        { name: 'AttendanceId', type: 'int' },
        { name: 'COURSEID', type: 'int' },
        { name: 'DateEntered', type: 'date' },
        { name: 'InstructorFinalized', type: 'int' }
    ]
});


