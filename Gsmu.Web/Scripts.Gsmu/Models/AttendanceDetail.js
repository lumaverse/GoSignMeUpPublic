Ext.define('AttendanceDetail', {
    extend: 'Ext.data.Model',
    idProperty: 'AttendanceDetailId',
    fields: [
        { name: 'AttendanceDetailId', type: 'int' },
        { name: 'CourseID', type: 'int' },
        { name: 'RosterId', type: 'int' },
        { name: 'CourseDate', type: 'date' },
        { name: 'Attended', type: 'int' },
        { name: 'AttendedHours', type: 'int' }
    ]
});


