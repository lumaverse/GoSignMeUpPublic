Ext.define('District', {
    extend: 'Ext.data.Model',
    idProperty: 'DISTID',
    fields: [
        { name: 'DISTID', type: 'int' },
        { name: 'DISTRICT1', type: 'string' },
        { name: 'SortOrder', type: 'int' },
        { name: 'NoTaxShipping', type: 'int' },
        { name: 'ShowAlternateConfirmation', type: 'int' },
        { name: 'AltEmailConfirmation', type: 'int' },
        { name: 'MembershipFlag', type: 'int' },
        { name: 'HideInPublicArea', type: 'int' }
    ]
});

