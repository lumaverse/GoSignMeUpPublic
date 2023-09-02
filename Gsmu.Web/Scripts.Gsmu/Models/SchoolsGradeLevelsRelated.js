Ext.define('SchoolsGradeLevelsRelated', {
    extend: 'Ext.data.Model',
    idProperty: 'SchoolsGradeLevelsRelatedId',
    fields: [
        { name: 'SchoolsGradeLevelsRelatedId', type: 'int' },
        { name: 'SchoolsId', type: 'int' },
        { name: 'GradeId', type: 'int' }
    ]
});
