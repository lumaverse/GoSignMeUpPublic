Ext.define('SchoolExtraInfo', {
    extend: 'Ext.data.Model',
    idProperty: 'sexid',
    fields: [
        { name: 'sexid', type: 'int' },
        { name: 'schoolshortdesc', type: 'string' },
        { name: 'sshipnumber', type: 'string' },
        { name: 'saddress', type: 'string' },
        { name: 'scity', type: 'string' },
        { name: 'sstate', type: 'string' },
        { name: 'szip', type: 'string' },
        { name: 'sphonenum', type: 'string' },
        { name: 'sfaxnum', type: 'string' },
        { name: 'schoolid', type: 'int' },
        { name: 'scountry', type: 'string' }
    ]
});


