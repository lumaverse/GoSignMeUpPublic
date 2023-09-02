Ext.define('Supervisors', {
    extend: 'Ext.data.Model',
    idProperty: 'SUPERVISORID',
    fields: [
        { name: 'SUPERVISORID', type: 'int' },
        { name: 'FIRST', type: 'string' },
        { name: 'LAST', type: 'string' },
        { name: 'TITLE', type: 'string' },
        { name: 'ADDRESS', type: 'string' },
        { name: 'CITY', type: 'string' },
        { name: 'STATE', type: 'string' },
        { name: 'ZIP', type: 'string' },
        { name: 'PHONE', type: 'string' },
        { name: 'FAX', type: 'string' },
        { name: 'SUPERVISORNUM', type: 'string' },
        { name: 'UserName', type: 'string' },
        { name: 'PASSWORD', type: 'string' },
        { name: 'SCHOOL', type: 'int' },
        { name: 'DISTRICT', type: 'int' },
        { name: 'EMAIL', type: 'string' },
        { name: 'ACTIVE', type: 'int' },
        { name: 'GRADE', type: 'int' },
        { name: 'NOTIFY', type: 'int' },
        { name: 'AdvanceOptions', type: 'int' },
        { name: 'AdditionalEmailAddresses', type: 'string' },
        { name: 'date_modified', type: 'date' },
        { name: 'ProfileImageFile', type: 'string' }
    ]
});
