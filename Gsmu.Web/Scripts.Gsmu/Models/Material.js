/*
Requires!!!!
@Scripts.Render("~/Scripts.Gsmu/ModelHelper.js")

public int productID { get; set; }
public string product_num { get; set; }
public string product_name { get; set; }
public Nullable<float> price { get; set; }
public Nullable<float> shipping_cost { get; set; }
public short priceincluded { get; set; }
public short taxable { get; set; }
public Nullable<float> shipping_weight { get; set; }
public Nullable<int> quantity { get; set; }
public Nullable<int> use_qty_from_materialid { get; set; }
*/
if (typeof (ModelHelper) != 'object') {
    alert('Scripts.Gsmu/ModelHelper.js is required!');
}

Ext.define('Models.Material', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'productID', type: 'int', useNull: true },
        { name: 'product_num', type: 'string', useNull: true },
        { name: 'product_name', type: 'string', useNull: true },
        { name: 'price', type: 'float', useNull: true },
        { name: 'shipping_cost', type: 'float', useNull: true },
        { name: 'priceincluded', type: 'boolean', useNull: true, convert: ModelHelper.ConvertVbScriptBooelan },
        { name: 'taxable', type: 'boolean', useNull: true, convert: ModelHelper.ConvertVbScriptBooelan },
        { name: 'shipping_weight', type: 'float', useNull: true },
        { name: 'quantity', type: 'int', useNull: true },
        { name: 'ActualPriceTotal', type: 'float', useNull: true },
        { name: 'PriceIncludedAsBoolean', type: 'boolean', useNull: true },
        { name: 'use_qty_from_materialid', type: 'int', useNull: true }
    
    ]
});