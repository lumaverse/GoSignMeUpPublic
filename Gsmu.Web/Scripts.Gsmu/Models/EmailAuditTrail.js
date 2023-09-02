/*
Requires!!!!
@Scripts.Render("~/Scripts.Gsmu/ModelHelper.js")
*/
if (typeof (ModelHelper) != 'object') {
    alert('Scripts.Gsmu/ModelHelper.js is required!');
}

Ext.define('EmailAuditTrail', {
    extend: 'Ext.data.Model',
    idProperty: 'AuditID',
    fields: [

        // fields
        { name: 'AuditID', type: 'int', useNull: false },
        { name: 'AuditDate', type: 'date', useNull: true, convert: ModelHelper.ConvertAspNetMvcDate },
        { name: 'EmailSubject', type: 'string', useNull: true },
        { name: 'AuditProcess', type: 'string', useNull: true },
        { name: 'EmailBody', type: 'string', useNull: true },
        { name: 'EmailTo', type: 'string', useNull: true },
        { name: 'EmailFrom', type: 'string', useNull: true },
        { name: 'EmailCC', type: 'string', useNull: true },
        { name: 'EmailBCC', type: 'string', useNull: true },
        { name: 'LoggedInUser', type: 'string', useNull: true },
        { name: 'Pending', type: 'int', useNull: true },
        { name: 'AttachmentName', type: 'string', useNull: true },
        { name: 'ErrorInfo', type: 'string', useNull: true },
        { name: 'RetryDateTime', type: 'date', useNull: true, convert: ModelHelper.ConvertAspNetMvcDate },
        { name: 'AttachmentNameMemo', type: 'string', useNull: true },
        { name: 'ResendDateTime', type: 'date', useNull: true, convert: ModelHelper.ConvertAspNetMvcDate }
    ]
});