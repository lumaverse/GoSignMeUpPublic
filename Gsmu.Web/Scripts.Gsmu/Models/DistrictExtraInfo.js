Ext.define('DistrictExtraInfo', {
    extend: 'Ext.data.Model',
    idProperty: 'dexid',
    fields: [
        { name: 'dexid', type: 'int' },
        { name: 'shortdesc', type: 'string' },
        { name: 'daddress', type: 'string' },
        { name: 'dcity', type: 'string' },
        { name: 'dstate', type: 'string' },
        { name: 'dzip', type: 'string' },
        { name: 'dphone', type: 'string' },
        { name: 'dfax', type: 'string' },
        { name: 'districtID', type: 'int' },
        { name: 'dcountry', type: 'string' }
    ]
});


