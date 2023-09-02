var combolayout;
var idcombo1;
var idcombo2;
var idcombo3;
var idcombo4;
var idcombo3;
var combonotes;
var coursename;
var thiscourseid;
var DisplayDate, addfivelineslabel, ShowSignInSheetNotes, addfivelinesvalue,papersize, multipledate, multipledatelabel, strInstructor, imagefile, optionalfieldvalue, DotNetSiteRootUrl, defaultPDForientation;
var SignupSheetDefaultColumn1Label,SignupSheetDefaultColumn2Label,SignupSheetDefaultColumn3Label,SignupSheetDefaultColumn4Label,SignupSheetDefaultColumn5Label;
var SignupSheetDefaultColumn1Value, SignupSheetDefaultColumn2Value, SignupSheetDefaultColumn3Value, SignupSheetDefaultColumn4Value, SignupSheetDefaultColumn5Value, configuration, 
Field1NameLabel, Field2NameLabel, Field3NameLabel;

//GET THE ASPNETROOTPORTAL INFORMATION (Cannot get this data from portal.js - gives issue
//THIS IS JUST A TEMP FIX
//@TO DO make a way to get the data from portal
Ext.onReady(function () {
    Ext.Ajax.request({
        url: '/admin/datastores/datastore-configuration.asp?' + 'rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
        params: {
            action: 'config'
        },
        success: function (response) {
            configuration = Ext.JSON.decode(response.responseText).configuration;
            DotNetSiteRootUrl = configuration.DotNetSiteRootUrl;
			Field1NameLabel = configuration.field1name; //grade
			Field2NameLabel = configuration.field2name; //school
			Field3NameLabel = configuration.field3name; //district
        }
    });
	

});
Ext.define('SiginSheet', {
    extend: 'Ext.data.Model',
    fields: [
        // fields
        { name: 'rosterid' },
        { name: 'studentlast', type: 'string', useNull: false },
        { name: 'studentfirst', type: 'string', useNull: false },
        { name: 'vdistrict', type: 'string', useNull: true },
        { name: 'vschool', type: 'string', useNull: false },
        { name: 'Notes', type: 'string', useNull: false },
        { name: 'gueststudentlast', type: 'string', useNull: false },
        { name: 'gradelevel', type: 'string', useNull: true }

    ]
});

Ext.define('combomodel', {
    extend: 'Ext.data.Model',
    fields: [

    // fields
    { name: 'textvalue', type: 'string', useNull: false },
    { name: 'combotextlabel', type: 'string', useNull: true }

    ]
});
Ext.define('schoolcombomodel', {
    extend: 'Ext.data.Model',
    fields: [

    // fields
    { name: 'textvalue', type: 'string', useNull: false },
    { name: 'combotextlabel', type: 'string', useNull: true }

    ]
});
Ext.define('imagescombomodel', {
    extend: 'Ext.data.Model',
    fields: [

    // fields
    { name: 'textvalue', type: 'string', useNull: false },
    { name: 'combotextlabel', type: 'string', useNull: true }

    ]
});
function SaveStudentNotes(noterosterid)
{
Notes = $("#CustomNotes" +noterosterid).val();

$.post("/admin/datastores/datastore-siginsheet.asp?action=savenotes&rosterid=" + noterosterid + '&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
			{
			Notes 	: Notes,
		},
		function(data){}
	);

}
function buildNewPortalSignInSheet(course_id)
{
	thiscourseid = course_id;

	$.post("/admin/datastores/datastore-siginsheet.asp?action=getcoursename&courseid=" + course_id + '&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
		{
			course_id 	: course_id,
		},
		function (data) {
			coursename = data.CourseName
			multipledate = data.SignInSheetMultipleDate
			addfivelinesvalue = data.SignInSheetShowBlankLine
			strInstructor = data.strInstructor
			ShowSignInSheetNotes = data.ShowSignInSheetNotes
			DisplayDate = data.DisplayDate
			defaultPDForientation = data.defaultPDForientation
			if (defaultPDForientation == "0") {
			    defaultPDForientationLabel = 'Portrait'
			  
			}
			else {
			    defaultPDForientationLabel = 'Landscape'
			    
			}

			if(ShowSignInSheetNotes=="1")
			{
				ShowSignInSheetNotesLabel = 'Yes'
				ShowSignInSheetNotesLabelHidden=false
			}
			else
			{
				ShowSignInSheetNotesLabel = 'No'
				ShowSignInSheetNotesLabelHidden=true
			}
			SignupSheetDefaultColumn1Label = data.SignupSheetDefaultColumn1Label
			SignupSheetDefaultColumn2Label = data.SignupSheetDefaultColumn2Label
			SignupSheetDefaultColumn3Label = data.SignupSheetDefaultColumn3Label
			SignupSheetDefaultColumn4Label = data.SignupSheetDefaultColumn4Label
			SignupSheetDefaultColumn5Label = data.SignupSheetDefaultColumn5Label
			SignupSheetDefaultColumn1Value = data.SignupSheetDefaultColumn1Value
			SignupSheetDefaultColumn2Value = data.SignupSheetDefaultColumn2Value
			SignupSheetDefaultColumn3Value = data.SignupSheetDefaultColumn3Value
			SignupSheetDefaultColumn4Value = data.SignupSheetDefaultColumn4Value
			SignupSheetDefaultColumn5Value = data.SignupSheetDefaultColumn5Value
			if (multipledate =="1")
			{
				multipledatelabel = "Yes"
			}
			else
			{
				multipledatelabel ="No"
			}

			buildPortalSignInSheet(course_id)
		}
	)

}
buildPortalSignInSheet = function (course_id) {
toolbar = {
    top: null,
    firstTool: null,
    secondTool: null,
    thirdTool: null,
    pager: null,
    pagerBottom: null,
	forthTool	:null
}

GetComboBoxValues = function()
{
	combolayout = Ext.getCmp('combolayout').value;
	idcombo1  = Ext.getCmp('idcombo1').value;
	idcombo2  = Ext.getCmp('idcombo2').value;
	idcombo3  = Ext.getCmp('idcombo3').value;
	idcombo4  = Ext.getCmp('idcombo4').value;
	idcombo5  = Ext.getCmp('idcombo5').value;
	notes = Ext.getCmp('combonote').value;
	imagefile =  Ext.getCmp('signinimage').value;
	multipledateold = multipledate;
	fiveblanlinesold = addfivelinesvalue;
	multipledate = Ext.getCmp('multipledates').value;
	papersize = Ext.getCmp('papersize').value;
	addfivelinesvalue = parseInt($.trim(Ext.getCmp('addfivelines').value));
	combonotes = $.trim(Ext.getCmp('combonote').value);
	signindatevalue = Ext.getCmp('signindate').value;
	signindatevalue1 = JSON.stringify(signindatevalue);
	optionalfieldvalue = Ext.getCmp('idoptionalfield').value;
	if (imagefile == null)
	{
		imagefile = "";
	}
	if (combolayout == null)
	{
	    combolayout = defaultPDForientation;
	}
	if ((combonotes == null)||(combonotes==""))
	{
		combonotes = ShowSignInSheetNotes;
	}
	if (multipledate == "Yes")
	{
		multipledate = 1;
	}
	else
	{
		if (multipledate == "No")
		{
			multipledate = 0;
		}
		else
		{
			multipledate = multipledateold;
		}
	}
	if (addfivelinesvalue > 0)
	{
		addfivelinesvalue = addfivelinesvalue;
	}
	else
	{
		addfivelinesvalue = fiveblanlinesold;
	}
	if (idcombo1 == null)
	{
		idcombo1 =SignupSheetDefaultColumn1Value
	}
	if (idcombo2 == null)
	{
		idcombo2 =SignupSheetDefaultColumn2Value
	}
	if (idcombo3 == null)
	{
		idcombo3 =SignupSheetDefaultColumn3Value
	}
	if (idcombo4 == null)
	{
		idcombo4 =SignupSheetDefaultColumn4Value
	}

	if (idcombo5 == null)
	{
		idcombo5 =SignupSheetDefaultColumn5Value
	}

}

var ComboItems = {
	getcomboitems: function(){
        var store = Ext.create('Ext.data.Store', {
            autoLoad: false,
            autoSync: true,
            pageSize: 10,
            remoteFilter: true,
            remoteGroup: true,
            remoteSort: true,
            model: 'combomodel',
            proxy: {
                type: 'ajax',
                url: '/admin/datastores/datastore-siginsheet.asp?action=comboitem' + '&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
                reader: {
                    type: 'json',
                    rootProperty: 'data',
                }
            }
        });

        return store;
    }
}
var ComboImages = {
	getcomboimagesitems: function(){
        var store = Ext.create('Ext.data.Store', {
            autoLoad: false,
            autoSync: true,
            pageSize: 10,
            remoteFilter: true,
            remoteGroup: true,
            remoteSort: true,
            model: 'imagescombomodel',
            proxy: {
                type: 'ajax',
                url: '/admin/datastores/datastore-siginsheet.asp?action=getimages' + '&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
                reader: {
                    type: 'json',
                    rootProperty: 'data',
                }
            }
        });

        return store;
    }
}

var ComboSigninDates = {
	getsignindates: function(){
        var store = Ext.create('Ext.data.Store', {
            autoLoad: false,
            autoSync: true,
            pageSize: 10,
            remoteFilter: true,
            remoteGroup: true,
            remoteSort: true,
            model: 'combomodel',
            proxy: {
                type: 'ajax',
                url: '/admin/datastores/datastore-siginsheet.asp?action=getdates&courseid=' + course_id + '&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
                reader: {
                    type: 'json',
                    rootProperty: 'data',
                }
            }
        });

        return store;
    }
}
var ComboBlankLines = {
	getblanklines: function(){
        var store = Ext.create('Ext.data.Store', {
            autoLoad: false,
            autoSync: true,
            pageSize: 10,
            remoteFilter: true,
            remoteGroup: true,
            remoteSort: true,
            model: 'combomodel',
            proxy: {
                type: 'ajax',
                url: '/admin/datastores/datastore-siginsheet.asp?action=getblanklines' + '&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
                reader: {
                    type: 'json',
                    rootProperty: 'data',
                }
            }
        });

        return store;
    }
}
var SchoolComboItems = {
	getschoolcomboitems: function(){
        var store = Ext.create('Ext.data.Store', {
            autoLoad: false,
            autoSync: true,
            pageSize: 10,
            remoteFilter: true,
            remoteGroup: true,
            remoteSort: true,
            model: 'schoolcombomodel',
            proxy: {
                type: 'ajax',
                url: '/admin/datastores/datastore-siginsheet.asp?action=schoolcomboitem' + '&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
                reader: {
                    type: 'json',
                    rootProperty: 'data',
                }
            }
        });

        return store;
    }
}

    var store = Ext.create('Ext.data.Store', {
            autoLoad: false,
            autoSync: true,
            pageSize: 10,
            remoteFilter: true,
            remoteGroup: true,
            remoteSort: true,
            model: 'SiginSheet',
			groupField: 'rosterid',
            proxy: {
                type: 'ajax',
                url: '/admin/datastores/datastore-siginsheet.asp?action=list&courseid=' + course_id + '&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
                reader: {
                    type: 'json',
                    rootProperty: 'signinsheet',
                    //totalProperty: 'recordCount'
                }
            }
    });
	
	
	
var groupmainHeaderTplDiv ="<div stlye=height:50px;><table><tr><td style='width:150px; text-align:left;'> Last Name</td><td style='width:150px;text-align:left;'>First Name</td><td style='width:250px;text-align:left;'>" + Field3NameLabel + "</td><td style='width:250px;text-align:left;'>" + Field2NameLabel + "</td><td style='width:150px;text-align:left;'>" +  Field1NameLabel + "</td></tr></table></div>"

	var columns = [
					{
						text: 'Last Name',
						id:'studentlast',
						dataIndex: 'studentlast',
						hidden: true,
						sortable: false,
						hideable: false,
						width: 100
					},{
						text: groupmainHeaderTplDiv,
						id:'Notes',
						dataIndex: 'Notes',
						hidden: false,
						sortable: false,
						hideable: false,
						width: 1150
					},{
						text: 'Sigininsheet',
						id:'gueststudentlast',
						dataIndex: 'gueststudentlast',
						hidden: true,
						sortable: false,
						hideable: false,
						width: 100
					},{
						id:'rosterid',
						dataIndex: 'rosterid',
						text: 'rosterid',
						hidden: true,
						sortable: false,
						hideable: false,
					},
					{
						text: 'First Name',
						id:'studentfirst',
						dataIndex: 'studentfirst',
						hidden: true,
						sortable: false,
						hideable: false,
						width: 100
					},{
						text: 'Company',
						id:'vdistrict',
						dataIndex: 'vdistrict',
						hidden: true,
						sortable: false,
						hideable: false,
						width: 100
					},{
						text: 'School',
						id:'vschool',
						dataIndex: 'vschool',
						hidden: true,
						sortable: false,
						hideable: false,
						width: 100

					},{
						text: 'Grade Level',
						id:'gradelevel',
						dataIndex: 'gradelevel',
						hidden: true,
						sortable: false,
						hideable: false,
						width: 200
					}
				];
	var groupHeaderTplDiv = "<div stlye=height:50px;><table><tr><td style='width:150px;'> {[values.rows[0].data.studentlast]} <br />{[values.rows[0].data.gueststudentlast]}</td><td style='width:150px;'>{[values.rows[0].data.studentfirst]}</td><td style='width:250px;'>{[values.rows[0].data.vdistrict]}</td><td style='width:250px;'>{[values.rows[0].data.vschool]}</td><td style='width:150px;'>{[values.rows[0].data.gradelevel]}</td></tr></table></div>"
	var style = 'float:left;width:200px; background-color:#cccccc;padding-left:10px;color:black;font-size:12px;'
	var groupHeaderTplDiv2 = '<div><div style=' + style + '>{[values.rows[0].data.studentlast]}</div><div style=' + style + '>{[values.rows[0].data.studentfirst]}</div><div style=' + style + '>{[values.rows[0].data.vdistrict]}</div><div style=' + style + '>{[values.rows[0].data.vschool]}</div><div style=' + style + '>{[values.rows[0].data.gradelevel]}</div></div>'
	var groupHeaderTplDiv23 = '<div>{[values.rows[0].data.studentlast]}</div>'
	var grid = Ext.create('Ext.grid.Panel', {
        border: false,
        store: store,
        columns: columns,
		layout: 'fit',
        loadMask: true,
		features: [{
            id: 'group',
            ftype: 'groupingsummary',
            groupHeaderTpl: groupHeaderTplDiv,
            hideGroupedHeader: true,
			collapsible: false,
			enableGroupingMenu: false,
			//enableNoGroups: false
			//showGroupsText: false
        }],
        emptyText: 'No Matching Records'
    });
    var stores = {
		schoolcombos:SchoolComboItems.getschoolcomboitems(true),
        combos: ComboItems.getcomboitems(true),
		imagescombo:ComboImages.getcomboimagesitems(true),
		blanklines:ComboBlankLines.getblanklines(true),
		signindates:ComboSigninDates.getsignindates(true),

    }
	toolbar.firstTool = Ext.create('Ext.toolbar.Toolbar', {
        layout:'hbox',
        layoutConfig: {
            pack:'center',
            //align:'stretch'
        },
        split: false,
        height: 30,
        border: false,
        dock: 'top',
            items: []
    });
	toolbar.secondTool = Ext.create('Ext.toolbar.Toolbar', {
        layout:'hbox',
        layoutConfig: {
            pack:'center',
        },
        split: false,
        height: 30,
        border: false,
        dock: 'top',
            items: [
				{   text: 'Add Image',
					icon:	'/admin/images/icons/image_add.png',
					listeners: {click: function (e) {
					    buildCommonUploadPanel('/admin/datastores/datastore-siginsheet.asp?action=upload' + '&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId).showAt(300, 300);
			}}}
        ]
    });
	toolbar.thirdTool = Ext.create('Ext.toolbar.Toolbar', {
        layout:'hbox',
        layoutConfig: {
            pack:'center',
            align:'stretch'
        },
        split: false,
        height: 30,
        border: false,
        dock: 'top',
            items: [
        ]
    });

	toolbar.forthTool = Ext.create('Ext.toolbar.Toolbar', {
        layout:'hbox',
        layoutConfig: {
            pack:'center',
            align:'stretch'
        },
        split: false,
        height: 30,
        border: false,
        dock: 'top',
            items: [
        ]
    });

	var orientation = Ext.create('Ext.form.field.ComboBox', {
	    emptyText: 'Portrait',
		queryMode: 'local',
		store : new Ext.data.JsonStore({
			fields : ['id', 'optionval'],
			data : [{id: 0, optionval: 'Portrait'},{id: 1, optionval: 'Landscape'}],
		}),
        flex: 1,
		id:'combolayout',
        hideEmptyLabel: true,
        fieldLabel: 'Orientation:',
        minChars: 1,
        typeAhead: true,
        remoteFilter: true,
        forceSelection: true,
		displayField: 'optionval',
		valueField:'id'
    });

		var notes = Ext.create('Ext.form.field.ComboBox', {
        emptyText: 'No',
		queryMode: 'local',
		hidden: false,
		store : new Ext.data.JsonStore({
			fields : ['id', 'optionval'],
			data : [{id: 0, optionval: 'No'},{id: 1, optionval: 'Yes'}],
		}),
        flex: 1,
		id:'combonote',
        hideEmptyLabel: true,
        fieldLabel: 'Print Note:',
        minChars: 1,
        typeAhead: true,
        remoteFilter: true,
        forceSelection: true,
		displayField: 'optionval',
		valueField:'id'
    });

	var ExtendedHeader = Ext.create('Ext.form.field.ComboBox', {
        emptyText: 'No',
        flex: 1,
		queryMode: 'local',
		store : new Ext.data.JsonStore({
			fields : ['id', 'optionval'],
			data : [{id: 0, optionval: 'No'},{id: 1, optionval: 'Yes'}],
		}),
        hideEmptyLabel: true,
        fieldLabel: 'Extended Header',
        minChars: 1,
        typeAhead: true,
        remoteFilter: true,
        forceSelection: true,
        displayField: 'optionval',
		valueField:'id'
	});

	var PaperSize = Ext.create('Ext.form.field.ComboBox', {
	    emptyText: 'Letter',
	    id: 'papersize',
	    flex: 1,
	    queryMode: 'local',
	    store: new Ext.data.JsonStore({
	        fields: ['id', 'optionval'],
	        data: [{ id: 'letter', optionval: 'Letter' }, { id: 'legal', optionval: 'Legal' }],
	    }),
	    hideEmptyLabel: true,
	    fieldLabel: 'Paper Size',
	    minChars: 1,
	    typeAhead: true,
	    remoteFilter: true,
	    forceSelection: true,
	    displayField: 'optionval',
	    valueField: 'id'
	});
	var MultipleDates =  Ext.create('Ext.form.field.ComboBox', {
        emptyText: multipledatelabel,
		id:'multipledates',
		queryMode: 'local',
		store : new Ext.data.JsonStore({
			fields : ['id', 'optionval'],
			data : [{id: 0, optionval: 'No'},{id: 1, optionval: 'Yes'}],
		}),
        flex: 1,
        hideEmptyLabel: true,
		fieldLabel: 'Multiple Dates',
        minChars: 1,
        remoteFilter: true,
        forceSelection: true,
        displayField: 'optionval'

    });

	var signinimage = Ext.create('Ext.form.field.ComboBox', {
        emptyText: 'No Image',
		id:'signinimage',
        flex: 1,
        hideEmptyLabel: true,
		fieldLabel: 'Image on File:',
		store: stores.imagescombo,
        minChars: 1,
        typeAhead: true,
        remoteFilter: true,
        forceSelection: true,
        displayField: 'combotextlabel',
		listeners : {
			expand : function () {

				var yourCombo = Ext.getCmp('signinimage');
				 yourCombo.store.reload();
			}
		}
    });
	var addfivelines = Ext.create('Ext.form.field.ComboBox', {
        emptyText: addfivelinesvalue,
		fieldLabel: 'Add Blank Line(s):',
		id:'addfivelines',
        flex: 1,
        hideEmptyLabel: true,
        hideLabel: false,
        minChars: 1,
        typeAhead: true,
        remoteFilter: true,
        forceSelection: true,
        store: stores.blanklines,
		valueField: 'textvalue',
        displayField: 'combotextlabel'
    });
	var signindate = Ext.create('Ext.form.field.ComboBox', {
        emptyText: DisplayDate,
		id:'signindate',
		multiSelect: true,
        flex: 1,
        hideEmptyLabel: true,
        hideLabel: true,
        minChars: 1,
        remoteFilter: true,
        forceSelection: true,
        store: stores.signindates,
		valueField: 'textvalue',
        displayField: 'combotextlabel',
        value:DisplayDate
	});
	var schoolcombo = Ext.create('Ext.form.field.ComboBox', {
        emptyText: 'School Name',
		fieldLabel: 'School Name:',
        flex: 1,
        hideEmptyLabel: true,
        hideLabel: true,
        minChars: 1,
        typeAhead: true,
        remoteFilter: true,
        store: stores.schoolcombos,
        displayField: 'combotextlabel'


    });

	var combo1 = Ext.create('Ext.form.field.ComboBox', {
		id:'idcombo1',
        emptyText: SignupSheetDefaultColumn1Label,
        flex: 1,
        hideEmptyLabel: true,
        hideLabel: true,
        minChars: 1,
        typeAhead: true,
        remoteFilter: true,
        forceSelection: true,
        store: stores.combos,
		valueField: 'textvalue',
        displayField: 'combotextlabel'
    });
	var combo2 = Ext.create('Ext.form.field.ComboBox', {
		id:'idcombo2',
        emptyText: SignupSheetDefaultColumn2Label,
        flex: 1,
        hideEmptyLabel: true,
        hideLabel: true,
        minChars: 1,
        typeAhead: true,
        remoteFilter: true,
        forceSelection: true,
        store: stores.combos,
		valueField: 'textvalue',
        displayField: 'combotextlabel'
    });
	var combo3 = Ext.create('Ext.form.field.ComboBox', {
		id:'idcombo3',
        emptyText: SignupSheetDefaultColumn3Label,
        flex: 1,
        hideEmptyLabel: true,
        hideLabel: true,
        minChars: 1,
        typeAhead: true,
        remoteFilter: true,
        forceSelection: true,
		store: stores.combos,
		valueField: 'textvalue',
        displayField: 'combotextlabel'
    });
	var combo4 = Ext.create('Ext.form.field.ComboBox', {
		id:'idcombo4',
        emptyText: SignupSheetDefaultColumn4Label,
        flex: 1,
        hideEmptyLabel: true,
        hideLabel: true,
        minChars: 1,
        typeAhead: true,
        remoteFilter: true,
        forceSelection: true,
		store: stores.combos,
		valueField: 'textvalue',
        displayField: 'combotextlabel'
    });
	var combo5 = Ext.create('Ext.form.field.ComboBox', {
		id:'idcombo5',
        emptyText: SignupSheetDefaultColumn5Label,
        flex: 1,
        hideEmptyLabel: true,
        hideLabel: true,
        minChars: 1,
        typeAhead: true,
        remoteFilter: true,
        forceSelection: true,
		store: stores.combos,
		valueField: 'textvalue',
        displayField: 'combotextlabel'
    });
	var optionalfield = Ext.create('Ext.form.field.Text', {
		id: 'idoptionalfield',
		fieldLabel: 'Optional Field:',
        emptyText: '',
		width:400,
    });

	var displaylabel1 = Ext.create('Ext.form.field.Text', {
	    id: 'displaylabel1',
        flex : 1,
		fieldLabel: 'Change column field:(Columns selected will only appear in the PDF report. ) ',
		labelWidth	: '600',
        emptyText: '',
        width: 0,
        labelStyle:{
            'font-size':'32px;'
            }
	});
	var includeWaitlist = Ext.create('Ext.form.field.Checkbox', {
	    id: 'includeWaitlist',
	    flex: 1,
	    boxLabel: 'Include Waitlist',
        inputVlaue : true
	});
var globalsettingmaintab = Ext.create('Ext.form.Panel', {
		frame		:false,
		id			:'maintab',
		height		:160,
		activeTab	:0,
		defaults	:{
						bodyPadding: 0
					},
		items		: [{
							title: ' ',
							items:[],
							dockedItems	:[,toolbar.firstTool,toolbar.secondTool,toolbar.forthTool,toolbar.thirdTool]
						}]
	});
var gridformonly = Ext.create('Ext.form.Panel', {
	border		:true,
	frame		:true,
	layout: 'fit',
	height		:300,
	width		: '100%',
	bodyPadding	: 0,
	items		: [grid]
});
var mainformgridset = Ext.create('Ext.form.Panel', {
	border		:true,
	frame		:true,
	height		:400,
	width		: '100%',
	bodyPadding	: 0,
	items		: [globalsettingmaintab,gridformonly]
});
	toolbar.secondTool.add(signinimage);
	toolbar.secondTool.add(addfivelines);
	toolbar.forthTool.add(displaylabel1);
	toolbar.forthTool.add(includeWaitlist);
	toolbar.firstTool.add(signindate);
	toolbar.firstTool.add(MultipleDates);
	toolbar.firstTool.add(ExtendedHeader);
	toolbar.firstTool.add(PaperSize);
	toolbar.firstTool.add(orientation);
	toolbar.secondTool.add(schoolcombo);
	toolbar.secondTool.add(notes);
	toolbar.thirdTool.add(combo1);
	toolbar.thirdTool.add(combo2);
	toolbar.thirdTool.add(combo3);
	toolbar.thirdTool.add(combo4);
	toolbar.thirdTool.add(combo5);
	toolbar.thirdTool.add(optionalfield);


    var win = Ext.create('widget.window', {
        title: course_id+":"+coursename + "<br /> " + Terminology.capital('instructor') + "(s):" + strInstructor,
		modal : true,
		closable: true,
		//closeAction: 'hide',
		items: [mainformgridset],
		width: 1000,
		height: 550,
		layout: 'fit',
				dockedItems : [{
                xtype: 'toolbar',
                title: 'Example',
                items: [{
                    text: 'Old PDF',
					icon: '/admin/images/icons/cog_edit.png',
					hidden:true,
					listeners: {
						click: function () {
							GetComboBoxValues();
							$.post("/admin/datastores/datastore-siginsheet.asp?action=createpdf&courseid=" + course_id + '&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
								{
									test 	: 'test',
									combo1	: idcombo1,
									combo2	: idcombo2,
									combo3	: idcombo3,
									combo4	: idcombo4,
									combo5	: idcombo5,
									RosterLayout: combolayout,
									Instructor : strInstructor,
									selectFilename : imagefile,
									optional : optionalfieldvalue,
									Print_all :'Yes',
									format	:'100',
									Multipledate: multipledate,
									addfivelinesvalue:addfivelinesvalue,
									signindatevalue:signindatevalue1,
								},
								function(data){
									//window.location = ".." + data.FileName
									window.open (".." + data.FileName)
								}
							)
						}
                    }
                },{
                    text: 'Print',
					icon: '/admin/images/icons/cog_edit.png',
					listeners: {
						click: function () {
							GetComboBoxValues();
							if (DotNetSiteRootUrl != null & DotNetSiteRootUrl != "") {
							    var url = DotNetSiteRootUrl + '/application/adminfunction?callback=?&call=portal-signinsheet&courseId=' + course_id
                                +"&combo1="+idcombo1
                                +"&combo2="+idcombo2
                                +"&combo3="+idcombo3
                                +"&combo4="+idcombo4
                                +"&combo5="+idcombo5
                                +"&selectFilename="+imagefile
                                +"&RosterLayout="+combolayout
                                +"&signindatevalue="+signindatevalue1
                                +"&addfivelinesvalue="+addfivelinesvalue
                                +"&shownotes="+combonotes
                                + "&multipledate=" + multipledate
                                + "&papersize=" + papersize
                                + "&optional=" + optionalfieldvalue
                                + "&includewaitinglist=" + Ext.getCmp('includeWaitlist').getValue();
							    $.getJSON(url.replace('#',''), function (result) {
							        if (result) {
							            if (result.Error != null) {
							                Ext.Msg.show({ title: 'Error Message', msg: 'An issue occured while doing the request. Contact Administrator', buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.ERROR });
							                console.log(result.Error.Message);
							                console.log(result.Error.StackTraceString);
							            } else {
							                window.open(DotNetSiteRootUrl + "temp/" + result)
							            }
							        }
							    });
							}
							else{
							    $.post("datastores/datastore-siginsheet.asp?action=createpdf&courseid=" + course_id + '&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
									{
										test 	: 'test',
										combo1	: idcombo1,
										combo2	: idcombo2,
										combo3	: idcombo3,
										RosterLayout: combolayout,
										Instructor : strInstructor,
										selectFilename : imagefile,
										optional : optionalfieldvalue,
										Print_all :'Yes',
										format	:'1',
										Multipledate: multipledate,
										addfivelinesvalue:addfivelinesvalue,
										signindatevalue:signindatevalue1,
									},
									function(data){
										//window.location = ".." + data.FileName
										window.open (".." + data.FileName)
									}
								)
							}
						}
                    }
                },{
                    text: 'Custom PDF',
					icon: '/admin/images/icons/cog_edit.png',
					hidden:true,
					listeners: {
						click: function () {
							GetComboBoxValues();
							$.post("/admin/datastores/datastore-siginsheet.asp?action=createpdf&courseid=" + course_id + '&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
								{
									test 	: 'test',
									combo1	: idcombo1,
									combo2	: idcombo2,
									combo3	: idcombo3,
									Instructor : strInstructor,
									optional : optionalfieldvalue,
									format	:'9',
									Print_all :'Yes',
									RosterLayout: combolayout,
									addfivelinesvalue:addfivelinesvalue,
									signindatevalue:signindatevalue1,

								},
								function(data){
									//window.location = ".." + data.FileName
									window.open (".." + data.FileName)
								}
							)
						}
                    }
                },{
                    text: 'CSV',
					icon: '/admin/images/icons/cog_edit.png',
					listeners: {
						click: function () {
								GetComboBoxValues();
								$.post("/admin/datastores/datastore-siginsheet.asp?action=createcsv&courseid=" + course_id + '&rubyrequest=1&uname=' + username + '&usersessionid=' + instructorsessionId,
								{
									test : 'test',
									Print_all :'Yes',
									combo1	: idcombo1,
									combo2	: idcombo2,
									combo3	: idcombo3,
									combo4	: idcombo4,
									combo5	: idcombo5,
									Instructor : strInstructor,
								},
								function(data){
									window.location = ".." + data.FileName
								}
							)
						}
                    }
                }]
            }]
		//items: [form]
    });

	store.loadPage();
	win.show();
}