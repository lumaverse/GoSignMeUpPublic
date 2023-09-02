/// <reference path="../extjs/ext-all.js" />

Ext.define('PortalRosters', {
	extend: 'Ext.data.Model',
	fields: [{
		name: 'rosterid'
	}, {
		name: 'count'
	}, {
		name: 'coursenameid'
	}, {
		name: 'coursedateid'
	}, {
		name: 'coursename'
	}, {
		name: 'coursenum'
	}, {
	    name: 'courseid'
	}, {
	    name: 'dateadded'
	}, {
		name: 'startdate'
	}, {
		name: 'enddate'
	}, {
		name: 'instructor'
	}, {
		name: 'instructor2'
	}, {
		name: 'instructor3'
	}, {
		name: 'studentid'
	}, {
		name: 'last',
	}, {
		name: 'first'
	}, {
		name: 'state'
	}, {
		name: 'address'
	}, {
		name: 'city'
	}, {
		name: 'zip'
	}, {
		name: 'country'
	}, {
		name: 'completeaddress'
	}, {
		name: 'courselocation'
	}, {
		name: 'homephone'
	}, {
		name: 'workphone'
	}, {
		name: 'fax'
	}, {
		name: 'email'
	}, {
		name: 'district'
	}, {
		name: 'cancelled'
	}, {
		name: 'attended'
	}, {
		name: 'cancelledtxt'
	}, {
		name: 'waitingtxt'
	}, {
		name: 'attendedtxt'
	}, {
		name: 'paidfulltxt'
	}, {
		name: 'paid'
	}, {
		name: 'course', type: 'float'
	}, {
		name: 'material', type: 'float'
	}, {
		name: 'coursetotal', type: 'float'
	}, {
		name: 'txtotal', type: 'float'
	}, {
		name: 'amountpaid', type: 'float'
	}, {
		name: 'materialnames'
	}, {
		name: 'studentschool'
	}, {
		name: 'studentgradelevel'
	}, {
		name: 'credited', type: 'float'
	}, {
		name: 'paymethod'
	}, {
		name: 'paynumber'
	}, {
		name: 'authnum'
	}, {
		name: 'refnumber'
	}, {
		name: 'ordernum'
	}, {
		name: 'accountnum'
	}, {
		name: 'invoicenumber'
	}, {
		name: 'invoicedate'
	}, {
		name: 'studentgrade'			
	}, {
		name: 'internalnote'
	}, {
		name: 'enrollmentnote'
	}, {
		name: 'coursechoice'
	}, {
		name: 'studregfield1'
	}, {
		name: 'studregfield2'
	}, {
		name: 'studregfield3'
	}, {
		name: 'studregfield4'
	}, {
		name: 'studregfield5'
	}, {
		name: 'studregfield6'
	}, {
		name: 'studregfield7'
	}, {
		name: 'studregfield8'
	}, {
		name: 'studregfield9'
	}, {
		name: 'studregfield10'
	}, {
		name: 'studregfield11'
	}, {
		name: 'studregfield12'
	}, {
		name: 'studregfield13'
	}, {
		name: 'studregfield14'
	}, {
		name: 'studregfield15'
	}, {
		name: 'studregfield16'
	}, {
		name: 'studregfield17'
	}, {
		name: 'studregfield18'
	}, {
		name: 'studregfield19'
	}, {
		name: 'studregfield20'
	}, {
		name: 'hiddenstudregfield1'
	}, {
		name: 'hiddenstudregfield2'
	}, {
		name: 'readonlystudregfield1'
	}, {
		name: 'readonlystudregfield2'
	}, {
		name: 'readonlystudregfield3'
	}, {
	    name: 'readonlystudregfield4'
	}, {
	    name: 'crinitialauditinfo'
	}, {
	    name: 'daddress'
	},
	{
		name: 'maincategory'
	}, 
	{
		name: 'subcategory'
	}
	]
});

//*******************************************************************

var PortalRostersStore = {
    getStore: function (cid) {	
	var store = Ext.create('Ext.data.Store', {
	    autoLoad: true,
	    autoSync: true,
	    pageSize: 10,
	    remoteFilter: true,
	    remoteSort: true,
	    remoteSort: true,
	    model: 'PortalRosters',
	    proxy: {
	        type: 'jsonp',
	        url: config.getUrl('Public/Instructor/GenerateRosterReport?cid=' + cid),
	        reader: {
	            rootProperty: 'rosters',
	            totalProperty: 'recordCount',
	            listeners: {
	                exception: function (reader, response, error, opts) {
	                    log(error);
	                }
	            }
	        }
	    },
	    sorters: {id: 'coursedateid', property: 'coursedateid', direction: 'DESC'},
        groupField: 'coursedateid'
	});

        return store;
    }

}