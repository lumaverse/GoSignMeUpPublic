Ext.define('CourseExtraParticipant', {
    extend: 'Ext.data.Model',
    idProperty: 'CourseExtraParticipantId',
    fields: [
        { name: 'CourseExtraParticipantId', type: 'int' },
        { name: 'RosterId', type: 'int' },
        { name: 'StudentFirst', type: 'string' },
        { name: 'StudentLast', type: 'string' },
        { name: 'StudentEmail', type: 'string' },
        { name: 'CustomField2', type: 'string' }
    ]
});

Ext.define('CourseExtraParticipantValidatedRegular', {
    extend: 'CourseExtraParticipant',

    validators: [
         { type: 'length', field: 'StudentFirst', min: 1, max: 50 },
         { type: 'length', field: 'StudentLast', min: 1, max: 50 },
         { type: 'email', field: 'StudentEmail' },
         { type: 'length', field: 'StudentEmail', min: 5, max: 100 },
    ]
});

Ext.define('CourseExtraParticipantValidatedCustomField', {
    extend: 'CourseExtraParticipantValidatedRegular',
    validators: [
         { type: 'length', field: 'CustomField2', min: 1, max: 50 }
    ]
});
Ext.define('CourseExtraParticipantValidatedConfigs', {
    extend: 'CourseExtraParticipant'
});


