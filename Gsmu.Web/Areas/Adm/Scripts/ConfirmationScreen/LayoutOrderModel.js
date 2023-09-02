Ext.define('LayoutOrderModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'OrderDate', type: 'date', convert: ModelHelper.ConvertAspNetMvcDate, dateReadFormat: 'm/d/Y' },
        { name: 'OrderNumber', type: 'string' },
        { name: 'StudentFirst', type: 'string' },
        { name: 'StudentLast', type: 'string' }
    ]
});