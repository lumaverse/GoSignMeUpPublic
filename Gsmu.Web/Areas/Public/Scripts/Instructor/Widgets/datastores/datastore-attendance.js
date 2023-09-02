/// <reference path="../extjs/ext-all.js" />

Ext.define('PortalAttendance', {
	extend: 'Ext.data.Model',
	fields: [
	{
		name: 'RosterId'
	}, 
	{
		name: 'CourseId'
	}, 
	{
		name: 'CourseNum'
	},
	{
		name: 'CourseName'
	}, 
	{
		name: 'Addresss'
	}, 
	{
		name: 'StudentId'
	}, 
	{
		name: 'StudentFirstName'
	}, 
	{
	    name: 'StudentLast'
	},
	{
		name: 'StudentUsername'
	},
    {
        name: 'District'
    },
    {
        name: 'School'
    },
    {
        name: 'GradeLevel'
    },
    {
        name: 'StudRegField1'
    },
	{	
	    name: 'CourseDateStart'
	}, 
	{
		name: 'CourseDateEnd'
	}, 
	{
		name: 'AttendanceDate'
	},  
	{
		name: 'AttendanceDateString'
	}, 
	{
		name: 'Attended'
	},  
	{
		name: 'AttendedHours'
	},  
	{
		name: 'ClockHours'
	},
    {
        name: 'InserviceHours'
    },
    {
        name: 'CustomCreditHours'
    },
	{
	    name: 'CEUCredits'
	},  
	{
		name: 'GraduateCredits'
	},  
	{
		name: 'Grade'
	}, 
	{
		name: 'CourseLocation'
	},
    {
        name: 'TotalAttendedHours' //clockhours
    },
    {
        name: 'LastAttendanceRecord'
    },
	{
		name: 'MainCategories'
	},
	{
		name: 'SubCategories'
	}
	]
});
Ext.define('sturegfld1', {
	extend: 'Ext.data.Model',
	fields: [
	{
		name: 'studregfield1'
	}
	]
});

//*******************************************************************

var PortalAttendanceStore = {
    getStore: function (cid) {
        var store = Ext.create('Ext.data.Store', {
            autoLoad: true,
            autoSync: true,
            pageSize: 50,
            remoteFilter: true,
            remoteSort: true,
            remoteSort: true,
            model: 'PortalAttendance',
            proxy: {
                type: 'jsonp',
                url: config.getUrl('Public/Instructor/GenerateAttendanceReport'),
                reader: {
                    rootProperty: 'attendanceReportList',
                    totalProperty: 'recordCount',
                    listeners: {
                        exception: function (reader, response, error, opts) {
                            log(error);
                        }
                    }
                }
            },
            sorters: { id: 'AttendanceDate', property: 'AttendanceDate', direction: 'DESC' },
            groupField: 'CourseId',
            listeners: {
            load: function (store, records, successfull) {

                var json = store.proxy.reader.rawData;
                var param = store.proxy.extraParams.param;
				
                 if (param=="exportall")
                 {
                     window.location = "/Temp/" + json.exportFileName;
                 }
				
				
            }
    }

        });

        return store;
    }

}