//jquery implementation
$(document).ready(function () {
    publicOptionsModule.initialize();
});
//global private objects goes here
var publicOptionsModuleObjects = (function ()
{
    var departmentItem = [];
    return {
        DepartmentItem: departmentItem
    }
})();
var publicOptionsModule = {
    initialize: function ()
    {
        //load data to depratment list
        publicOptionsModule.loadDepartmentData();
    },
    savePublicOptions: function (button)
    {
        button = $(button);
        var buttonText = button.html();
        var formData = $('#public-options-form').serialize()
        $.ajax({
            url: '/SystemConfig/SavePublicOptions',
            type: 'POST',
            data: formData,
            dataType: 'json',
            beforeSend: function () {
                button.html(buttonText.replace('Save','Saving...'));
                button.addClass("disabled");
                button.prop("disabled", true);
                $('.container-fluid').find('input,select,textarea,a,button').attr('disabled', true);
            },
            success: function (data)
            {
                if (data.success) {
                    $.jGrowl('Succesfully Updated Public Options!', { theme: 'successGrowl', themeState: '' });
                }
                else
                {
                    $.jGrowl('Something went wrong!' + data.error, { theme: 'errorGrowl', themeState: '' });
                }
            },
            error: function (data)
            {
                if (!data.success)
                {
                    $.jGrowl('Something went wrong!' + data.error, { theme: 'errorGrowl', themeState: '' });
                }
            },
            complete: function () {
                button.html(buttonText);
                button.removeClass("disabled");
                button.prop("disabled", false);
                $('.container-fluid').find('input,select,textarea,a,button').removeAttr('disabled');
            }
        })
    },
    backToIndex: function ()
    {
        window.location = '/Adm/SystemConfig/';
    },
    loadDepartmentData: function () {
        var departmentData = $('#CurrentShibbolethDepartmentAttribute').val();
        if (departmentData != "")
        {
            var departmentItems = departmentData.split(',');
            publicOptionsModuleObjects.DepartmentItem = departmentItems;
            for (var x = 0; x < departmentItems.length; x++)
            {
                publicOptionsModule.addDepartment(x, departmentItems[x]);
            }
        }
    },
    addDepartment: function (index, departmentItemData)
    {
        var departmentTextValue = $('#Department').val();
        var departmentItemValue = '';
        if (!departmentItemData) // check if it's from the load function or from add button click
        {
            if (departmentTextValue == '') {
                $.jGrowl('Department name cannot be empty!', { theme: 'errorGrowl', themeState: '' });
                return false;
            }
            publicOptionsModuleObjects.DepartmentItem.push(departmentTextValue);
            //set value
            publicOptionsModule.bindDepartmentListData();
            departmentItemValue = departmentTextValue;
        }
        else
        {
            departmentItemValue = departmentItemData;
        }
        
        if (!index)
        {
            var lastIndex = publicOptionsModuleObjects.DepartmentItem.length ? -1 : publicOptionsModuleObjects.DepartmentItem.length;
            index = lastIndex + 1;
        }

        //UI part - creating grid
        var departmentGrid = $('#department-grid tbody');
        var departmentItemDeleteAction = '<td class="text-right"><a class="delete-department" onclick="publicOptionsModule.deleteDepartment(this,' + index + ')">'
                                        + '<i class="glyphicon glyphicon-remove"></i></a></td>';
        var departmentItem = '<tr><td>' + departmentItemValue + '</td>' + departmentItemDeleteAction + '</tr>';
        departmentGrid.append(departmentItem).hide().fadeIn('slow');
    },
    deleteDepartment: function (control, index)
    {
        var controlTarget = $(control).parent().parent();
        $(controlTarget).hide('slow').remove('slow');
        publicOptionsModuleObjects.DepartmentItem.splice(index, 1);
        //set value
        publicOptionsModule.bindDepartmentListData();
    },
    bindDepartmentListData: function ()
    {
        $('#CurrentShibbolethDepartmentAttribute').val(publicOptionsModuleObjects.DepartmentItem.toString());
    }
}


//ext js implementation
function PublicOptionsComponent(options) {
    var self = this;
    self.Options.containerId = options.containerId;
    self.Options.Data = options.Data;

    Ext.onDocumentReady(function () {
        self.initialize();
        self.ExecuteFunction('load'); //loads data to the controls
    });
}
PublicOptionsComponent.constructor = PublicOptionsComponent;
PublicOptionsComponent.prototype.initialize = function (renderTo) {
    var self = this;
    self.Render();
}
PublicOptionsComponent.prototype.Options = {
    containerId: null,
    Data: []
}
PublicOptionsComponent.prototype.State = {
    AjaxRequestCount: 0,
    Panel: null,
    Stores: {
        TimeZone:  Ext.create('Ext.data.Store', {
            fields: ['name', 'value'],
            data:
                [
                    { "name": "None", "value": 9999 },
                    { "name": "Pacific (0)", "value": 0 },
                    { "name": "Mountain (1)", "value": 1 },
                    { "name": "Central (2)", "value": 2 },
                    { "name": "Eastern (3)", "value": 3 },
                    { "name": "Arizona (4)", "value": 4 }
                ]
        }),
        PolarAnswers: Ext.create('Ext.data.Store', {
            fields: ['name', 'value'],
            data:
                [
                    { "name": "No", "value": 0 },
                    { "name": "Yes", "value": 1 }
                ]
        }),
        PublicSignupAbilityOff: Ext.create('Ext.data.Store', {
            fields: ['name', 'value'],
            data:
                [
                    { "name": "No", "value": 0 },
                    { "name": "Yes", "value": 1 },
                    { "name": "Yes, Only Supervisor can signup after login.", "value": 2 }
                ]
        }),
        Forcelogin: Ext.create('Ext.data.Store', {
            fields: ['name', 'value'],
            data:
                [
                    { "name": "No", "value": 0 },
                    { "name": "Yes", "value": 1 },
                    { "name": "Yes, and show login screen first.", "value": 2 }
                ]
        }),
        PublicHideLinks: Ext.create('Ext.data.Store', {
            fields: ['name', 'value'],
            data:
                [
                    { "name": "Not Applicable", "value": 0 },
                    { "name": "Hide Manage Rooms Link (1)", "value": 1 },
                    { "name": "Hide Evaluations Area/Link (2)", "value": 2 },
                    { "name": "Hide 1 and 2 above", "value": 3 }
                ]
        }),
        NameDisplayStyle: Ext.create('Ext.data.Store', {
                fields: ['name', 'value'],
                data:
                    [
                        { "name": "Last, First", "value": 0 },
                        { "name": "First Last", "value": 1 }
                    ]
        }),
        SupervisorExcludeInactive: Ext.create('Ext.data.Store', {
            fields: ['name', 'value'],
            data:
                [
                    { "name": "Show Active and Inactive", "value": 0 },
                    { "name": "1. Exclude Inactive (Student Add/Edit Page Inactive? Field)", "value": 1 },
                    { "name": "2. Exclude by 'Inactive' (Student Registration Hidden Fields 1: Hidden Field Sample) ", "value": 2 },
                    { "name": "3. Exclude by 1 and 2 above", "value": 3 },
                ]
        })
    }
}
PublicOptionsComponent.prototype.Render = function () {

    var self = this;

    var commonLoginPanel = Ext.create('Ext.panel.Panel', {
        id: 'commonLoginPanel',
        cls: 'container-panel',
        title: 'Common Login Settings',
        collapsed: false,
        frame: false,
        bodyStyle: { background: 'transparent' },
        border: 0,
        padding: '2 15 2 0',
        bodyPadding: 20,
        header: {
            xtype: 'header',
            cls: 'header-panel-section',
            titlePosition: 0,
            defaults: {
                margin: '0 10px 0 10px'
            },
            items: [
                {
                    xtype:'image',
                    src: '/Images/Icons/FamFamFam/lock.png',
                    style: {
                        'margin': 'auto'
                    },
                    cls: 'header-panel-icon'

                }
            ]
        },
        layout: {
            type: 'vbox', pack: 'start', align: 'stretch'
        },
        items: [
                {
                    xtype: 'combobox',
                    id: 'systemTimeZoneOffset',
                    name: 'SystemTimeZoneHour',
                    fieldLabel: 'System Time Zone Hour Offset',
                    labelWidth: false,
                    labelStyle: 'width: auto',
                    store: self.State.Stores.TimeZone,
                    displayField: 'name',
                    valueField: 'value',
                    queryMode: 'remote',
                    remoteFilter: true,
                    forceSelection: true,
                    value: 1
                },
                {
                    xtype: 'radiogroup',
                    id: 'LoginAuthOption',
                    name: 'LoginAuthOption',
                    fieldLabel: 'Log-in Authentication Option',
                    columns:1,
                    vertical: true,
                    items: [
                        { boxLabel: 'Default', name: 'LoginAuthOption', inputValue: -1, },
                        { boxLabel: 'Email', name: 'LoginAuthOption', inputValue: 0 },
                        { boxLabel: 'Allow to use either Username or Email', name: 'LoginAuthOption', inputValue: 1 }
                    ],
                    labelWidth: false,
                    labelStyle: 'width: auto'
                },
                {
                    xtype: 'radiogroup',
                    id: 'StartupPage',
                    name: 'StartupPage',
                    fieldLabel: 'Log-in Startup Page',
                    columns: 1,
                    vertical: true,
                    items: [
                        {
                            boxLabel: 'Default', name: 'StartupPage', inputValue: 0,
                        },
                        {
                            boxLabel: 'Browse Courses', name: 'StartupPage', inputValue: 1
                        },
                        {
                            boxLabel: 'Other', name: 'StartupPage', inputValue: 2
                        }
                    ],
                    labelWidth: false,
                    labelStyle: 'width: auto'
                },
                {
                    xtype: 'combobox',
                    id: 'commonLoginForPublicAreas',
                    name: 'AllowPublicBreakCommonLogin',
                    fieldLabel: 'Common/Matching Login For Public Areas',
                    labelWidth: false,
                    labelStyle: 'width: auto',
                    queryMode: 'local',
                    store: self.State.Stores.PolarAnswers,
                    displayField: 'name',
                    valueField: 'value',
                    queryMode: 'remote',
                    remoteFilter: true,
                    forceSelection: true,
                    value: 1
                },
                {
                    xtype: 'combobox',
                    id: 'autoPopulatePassword',
                    name: 'AutoPopulatePassword4CommonLogin',
                    fieldLabel: 'Auto Populate Password for Instructor & Supervisor from student',
                    labelWidth: false,
                    labelStyle: 'width: auto',
                    queryMode: 'local',
                    store: self.State.Stores.PolarAnswers,
                    displayField: 'name',
                    valueField: 'value',
                    queryMode: 'remote',
                    remoteFilter: true,
                    forceSelection: true,
                    value: 1
                },
                {
                    xtype: 'combobox',
                    id: 'autoUpdateBasicData',
                    name: 'AllowCrossUserUpdate',
                    fieldLabel: 'Auto update basic data for Instructor & Supervisor from student',
                    labelWidth: false,
                    labelStyle: 'width: auto',
                    queryMode: 'local',
                    store: self.State.Stores.PolarAnswers,
                    displayField: 'name',
                    valueField: 'value',
                    queryMode: 'remote',
                    remoteFilter: true,
                    forceSelection: true,
                    value: 1
                },
                {
                    xtype: 'combobox',
                    id: 'preventUserSignUp',
                    name: 'PublicSignupAbilityOff',
                    fieldLabel: 'Prevent users from Sign up/check out',
                    labelWidth: false,
                    labelStyle: 'width: auto',
                    queryMode: 'local',
                    store: self.State.Stores.PublicSignupAbilityOff,
                    displayField: 'name',
                    valueField: 'value',
                    queryMode: 'remote',
                    remoteFilter: true,
                    forceSelection: true,
                    value: 1
                },
                {
                    xtype: 'textfield',
                    id: 'pretextUser',
                    name: 'StudentRegisterMaskFiveInitText',
                    fieldLabel: 'Pretext Username for Mask',
                    labelWidth: false,
                    labelStyle: 'width: auto'
                }
        ]
    });

    var requireLoginPanel = Ext.create('Ext.panel.Panel', {
        id: 'requireLoginPanel',
        cls: 'container-panel',
        title: 'Required Login Settings',
        plugins: 'responsive',
        collapsed: false,
        frame: false,
        bodyStyle: { background: 'transparent' },
        border: 0,
        padding: '2 15 2 0',
        bodyPadding: 20,
        header: {
            xtype: 'header',
            cls: 'header-panel-section',
            titlePosition: 0,
            defaults: {
                margin: '0 10px 0 10px'
            },
            items: [
                {
                    xtype:'image',
                    src: '/Images/Icons/FamFamFam/lock_break.png',
                    style: {
                        'margin': 'auto'
                    },
                    cls: 'header-panel-icon'

                }
            ]
        },
        layout: {
            type: 'vbox', pack: 'start', align: 'stretch'
        },
        items: [
                {
                    xtype: 'combobox',
                    id: 'requireStudentAccount',
                    fieldLabel: 'Require student accounts to have unique email addresses',
                    name: 'ForceUniqueStudentEmails',
                    labelWidth: false,
                    labelStyle: 'width: auto',
                    queryMode: 'local',
                    store: self.State.Stores.Forcelogin,
                    displayField: 'name',
                    valueField: 'value',
                    queryMode: 'remote',
                    remoteFilter: true,
                    forceSelection: true,
                    value: 0
                },
                {
                    xtype: 'combobox',
                    id: 'requirePublicLogin',
                    fieldLabel: 'Require Public to login before able to Browse or Search Classes',
                    name: 'ForceLogin',
                    labelWidth: false,
                    labelStyle: 'width: auto',
                    queryMode: 'local',
                    store: self.State.Stores.PolarAnswers,
                    displayField: 'name',
                    valueField: 'value',
                    queryMode: 'remote',
                    remoteFilter: true,
                    forceSelection: true,
                    value: 1
                },
                {
                    xtype: 'combobox',
                    id: 'forceUpdateAccount',
                    fieldLabel: 'Force Update account after login',
                    name: 'ForceAccountUpdate',
                    labelWidth: false,
                    labelStyle: 'width: auto',
                    queryMode: 'local',
                    store: self.State.Stores.PolarAnswers,
                    displayField: 'name',
                    valueField: 'value',
                    queryMode: 'remote',
                    remoteFilter: true,
                    forceSelection: true,
                    value: 1
                },
                {
                    xtype: 'combobox',
                    id: 'allowStuentMultipleSignUp',
                    fieldLabel: 'Allow Student Multiple Sign Up',
                    name: 'AllowStudentMultiEnroll',
                    labelWidth: false,
                    labelStyle: 'width: auto',
                    queryMode: 'local',
                    store: self.State.Stores.PolarAnswers,
                    displayField: 'name',
                    valueField: 'value',
                    queryMode: 'remote',
                    remoteFilter: true,
                    forceSelection: true,
                    value: 1
                },
                {
                    xtype: 'textfield',
                    id: 'multipleSignUpCustomText',
                    fieldLabel: 'Multiple Sign Up Custom Text',
                    name:'MultipleSignUpCustomText',
                    labelWidth: false,
                    labelStyle: 'width: auto'
                },
                {
                    xtype: 'combobox',
                    id: 'studentMultipleSignUp',
                    fieldLabel: 'Student Multiple Sign Up in Strict Mode',
                    name: 'RestrictStudentMultiSignup',
                    labelWidth: false,
                    labelStyle: 'width: auto',
                    queryMode: 'local',
                    store: self.State.Stores.PolarAnswers,
                    displayField: 'name',
                    valueField: 'value',
                    queryMode: 'remote',
                    remoteFilter: true,
                    forceSelection: true,
                    value: 1
                }
        ]
    });

    var payorPanel = Ext.create('Ext.panel.Panel', {
        id: 'payorPanel',
        cls: 'container-panel',
        title: 'Payor Settings',
        collapsed: false,
        frame: false,
        bodyStyle: { background: 'transparent' },
        border: 0,
        padding: '2 15 2 0',
        bodyPadding: 20,
        header: {
            xtype: 'header',
            cls: 'header-panel-section',
            titlePosition: 0,
            defaults: {
                margin: '0 10px 0 10px'
            },
            items: [
                {
                    xtype: 'image',
                    src: '/Images/Icons/FamFamFam/coins.png',
                    style: {
                        'margin': 'auto'
                    },
                    cls: 'header-panel-icon'

                }
            ]
        },
        layout: {
            type: 'vbox', pack: 'start', align: 'stretch'
        },
        items: [
                {
                    xtype: 'combobox',
                    id: 'allowPayorMultiple',
                    fieldLabel: 'Allow Payor (Multiple Students under one Payor)',
                    name: 'AllowParentLevel',
                    labelWidth: false,
                    labelStyle: 'width: auto',
                    store: self.State.Stores.PolarAnswers,
                    displayField: 'name',
                    valueField: 'value',
                    queryMode: 'remote',
                    remoteFilter: true,
                    forceSelection: true,
                    value: 1
                },
                {
                    xtype: 'textfield',
                    id: 'parentLevelTitle',
                    fieldLabel: 'Parent Level Title',
                    name: 'ParentLevelTitle',
                    labelWidth: false,
                    labelStyle: 'width: auto'
                },
                {
                    xtype: 'textfield',
                    id: 'childLevelTitle',
                    fieldLabel: 'Child Level Title',
                    name: 'ChildLevelTitle',
                    labelWidth: false,
                    labelStyle: 'width: auto'
                },
                {
                    xtype: 'combobox',
                    id: 'publicReleaseForm',
                    fieldLabel: 'Have Public Release Form (Must have Allow Payor set to Yes)',
                    name: 'AllowReleaseForm',
                    labelWidth: false,
                    labelStyle: 'width: auto',
                    store: self.State.Stores.PolarAnswers,
                    displayField: 'name',
                    valueField: 'value',
                    queryMode: 'remote',
                    remoteFilter: true,
                    forceSelection: true,
                    value: 1
                },
                {
                    xtype: 'textfield',
                    id: 'releaseFromTitle',
                    fieldLabel: 'Release Form Title',
                    name: 'ReleaseFormTitle',
                    labelWidth: false,
                    labelStyle: 'width: auto'
                },
                {
                    xtype: 'textareafield',
                    id: 'releaseFromText',
                    name: 'ReleaseFormText',
                    fieldLabel: 'Release Form Text',
                    name: '',
                    labelWidth: false,
                    labelStyle: 'width: auto'
                },
                {
                    xtype: 'combobox',
                    id: 'automaticallyMarkZeroCost',
                    fieldLabel: 'Automatically Mark Zero Cost Public Orders as Paid-In-Full So they are Auto-Approved',
                    name: 'AutoApproveZeroOrder',
                    labelWidth: false,
                    labelStyle: 'width: auto',
                    store: self.State.Stores.PolarAnswers,
                    displayField: 'name',
                    valueField: 'value',
                    queryMode: 'remote',
                    remoteFilter: true,
                    forceSelection: true,
                    value: 1
                },
                {
                    xtype: 'combobox',
                    id: 'hideSpecificLinksPublicSide',
                    fieldLabel: 'Hide Specific Links On The Public Side',
                    name: 'PublicHideLinks',
                    labelWidth: false,
                    labelStyle: 'width: auto',
                    store: self.State.Stores.PublicHideLinks,
                    displayField: 'name',
                    valueField: 'value',
                    queryMode: 'remote',
                    remoteFilter: true,
                    forceSelection: true,
                    value: 1
                },
                {
                    xtype: 'combobox',
                    id: 'reverseStudentsName',
                    fieldLabel: 'Reverse The Students Name in the Cart Area',
                    name: 'NameDisplayStyle',
                    labelWidth: false,
                    labelStyle: 'width: auto',
                    store: self.State.Stores.NameDisplayStyle,
                    displayField: 'name',
                    valueField: 'value',
                    queryMode: 'remote',
                    remoteFilter: true,
                    forceSelection: true,
                    value: 1
                },
                {
                    xtype: 'combobox',
                    id: 'studentsToBeShownPublic',
                    fieldLabel: 'Students To Be Shown In The Public Supervisor Area',
                    name: 'SupervisorExcludeInactive',
                    labelWidth: false,
                    labelStyle: 'width: auto',
                    store: self.State.Stores.SupervisorExcludeInactive,
                    displayField: 'name',
                    valueField: 'value',
                    queryMode: 'remote',
                    remoteFilter: true,
                    forceSelection: true,
                    value: 1
                }
        ]
    });

    var googleSSOPanel = Ext.create('Ext.panel.Panel', {
        id: 'googleSSOPanel',
        cls: 'container-panel',
        title: 'Google SSO Panel',
        collapsed: false,
        frame: false,
        bodyStyle: { background: 'transparent' },
        border: 0,
        padding: '2 15 2 0',
        bodyPadding: 20,
        header: {
            xtype: 'header',
            cls: 'header-panel-section',
            titlePosition: 0,
            defaults: {
                margin: '0 10px 0 10px'
            },
            items: [
                {
                    xtype: 'image',
                    src: '/Images/Icons/socialmediaicons/google-16x16.png',
                    style: {
                        'margin': 'auto'
                    },
                    cls: 'header-panel-icon'

                }
            ]
        },
        layout: {
            type: 'vbox', pack: 'start', align: 'stretch'
        },
        items: [
                {
                    xtype: 'box',
                    html: '<div style="text-align:center">Note: For information on how to enable integration contact GoSignMeUp! <br /> <a href="#">Google API Console</a></div>',
                    labelWidth: false,
                    labelStyle: 'width: auto'
                },
                {
                    xtype: 'box',
                    name: 'AspSiteRootUrl',
                    html: 'Google authentication redirect url : <div style="text-align:center;margin-left:205px"><a href="#" id="aspsite-google-redirect-url"></a></div>',
                    labelWidth: false,
                    labelStyle: 'width: auto',
                    afterRender: function (obj, val)
                    {
                        var urlVal = $('#aspsite-google-redirect-url-hidden').val();
                        $('#aspsite-google-redirect-url').attr('href', urlVal).text(urlVal);
                    }
                },
                {
                    xtype: 'radiogroup',
                    id: 'GoogleSSOEnabled',
                    fieldLabel: 'Google SSO Enabled',
                    name: 'GoogleSSOEnabled',
                    columns: 3,
                    vertical: true,
                    items: [
                        { boxLabel: 'On', name: 'GoogleSSOEnabled', inputValue: 1 },
                        { boxLabel: 'Off', name: 'GoogleSSOEnabled', inputValue: 0 },
                    ],
                    labelWidth: false,
                    labelStyle: 'width: auto'
                },
                {
                    xtype: 'textfield',
                    id: 'GoogleSSOClientId',
                    fieldLabel: 'Google SSO Client ID',
                    name: 'GoogleSSOClientId',
                    labelWidth: false,
                    labelStyle: 'width: auto'
                },
                {
                    xtype: 'textfield',
                    id: 'googleSSOClientSecret',
                    fieldLabel: 'Google SSO Client Secret',
                    name: 'GoogleSSOClientSecret',
                    labelWidth: false,
                    labelStyle: 'width: auto'
                }
        ]
    });

    var shibbolethSSOPanel = Ext.create('Ext.panel.Panel', {
        id: 'shibbolethSSOPanel',
        cls: 'container-panel',
        title: 'Shibboleth SSO Panel',
        collapsed: false,
        frame: false,
        bodyStyle: { background: 'transparent' },
        border: 0,
        padding: '2 15 2 0',
        bodyPadding: 20,
        header: {
            xtype: 'header',
            cls: 'header-panel-section',
            titlePosition: 0,
            defaults: {
                margin: '0 10px 0 10px'
            },
            items: [
                {
                    xtype: 'image',
                    src: '/Images/IntegrationPartners/shibbolethicon.png',
                    style: {
                        'margin': 'auto'
                    },
                    cls: 'header-panel-icon'

                }
            ]
        },
        layout: {
            type: 'vbox', pack: 'start', align: 'stretch'
        },
        items: [
                {
                    xtype: 'radiogroup',
                    id: 'ShibbolethSSOEnabled',
                    fieldLabel: 'Shibboleth SSO Enabled',
                    name: 'ShibbolethSSOEnabled',
                    columns: 3,
                    vertical: true,
                    items: [
                        { boxLabel: 'On', name: 'ShibbolethSSOEnabled', inputValue: 1 },
                        { boxLabel: 'Off', name: 'ShibbolethSSOEnabled', inputValue: 0 },
                    ],
                    labelWidth: false,
                    labelStyle: 'width: auto'
                },
                {
                    xtype: 'radiogroup',
                    id: 'ShibbolethAllowGSMULogin',
                    fieldLabel: 'Shibboleth Allow GSMU Authentication',
                    name: 'ShibbolethAllowGSMULogin',
                    columns: 3,
                    vertical: true,
                    items: [
                        { boxLabel: 'On', name: 'ShibbolethAllowGSMULogin', inputValue: 1 },
                        { boxLabel: 'Off', name: 'ShibbolethAllowGSMULogin', inputValue: 0, },
                    ],
                    labelWidth: false,
                    labelStyle: 'width: auto'
                },
                {
                    xtype: 'radiogroup',
                    id: 'ShibbolethRequiredLogin',
                    fieldLabel: 'Shibboleth SSO Required Log in',
                    name: 'ShibbolethRequiredLogin',
                    columns: 3,
                    vertical: true,
                    items: [
                        { boxLabel: 'On', name: 'ShibbolethRequiredLogin', inputValue: 1 },
                        { boxLabel: 'Off', name: 'ShibbolethRequiredLogin', inputValue: 0 },
                    ],
                    labelWidth: false,
                    labelStyle: 'width: auto'
                },
                {
                    xtype: 'radiogroup',
                    id: 'LoginShibSSOGSMUOnly',
                    fieldLabel: 'Only allow GSMU Users to Log in',
                    name: 'LoginShibSSOGSMUOnly',
                    columns: 3,
                    vertical: true,
                    items: [
                        { boxLabel: 'On', name: 'LoginShibSSOGSMUOnly', inputValue: 1 },
                        { boxLabel: 'Off', name: 'LoginShibSSOGSMUOnly', inputValue: 0 }
                    ],
                    labelWidth: false,
                    labelStyle: 'width: auto'
                },
                {
                    xtype: 'radiogroup',
                    id: 'LoginShipSSOGSMUActive',
                    fieldLabel: 'Only allow active Users to Log in',
                    name: 'LoginShipSSOGSMUActive',
                    columns: 3,
                    vertical: true,
                    items: [
                        { boxLabel: 'On', name: 'LoginShipSSOGSMUActive', inputValue: 1 },
                        { boxLabel: 'Off', name: 'LoginShipSSOGSMUActive', inputValue: 0 }
                    ],
                    labelWidth: false,
                    labelStyle: 'width: auto'
                },
                {
                    xtype: 'textfield',
                    id: 'ShibbolethLogoutLink',
                    fieldLabel: 'Shibboleth SSO Log out Link',
                    name: 'ShibbolethLogoutLink',
                    labelWidth: false,
                    labelStyle: 'width: auto'
                },
                {
                    xtype: 'textfield',
                    id: 'shibbolethSessionIdAttribute',
                    fieldLabel: 'Shibboleth Session ID Attribute',
                    labelWidth: false,
                    labelStyle: 'width: auto'
                },
                {
                    xtype: 'textfield',
                    id: 'ShibbolethDepartmentAttribute',
                    fieldLabel: 'Shibboleth Department Attribute',
                    name: 'ShibbolethDepartmentAttribute',
                    labelWidth: false,
                    labelStyle: 'width: auto'
                },
                {
                    xtype: 'fieldcontainer',
                    fieldLabel: 'Department',
                    labelWidth: false,
                    labelStyle: 'width: auto',
                    items: [
                        {
                            xtype: 'textfield',
                            flex: 1
                        },
                        {
                            xtype: 'splitter'
                        },
                        {
                            xtype: 'button',
                            flex: 1,
                            text: 'Add Department'
                        }
                    ]
                }
        ]
    });

    var publicOptionsPanel = Ext.create('Ext.panel.Panel', {
        id: 'publicOptionsPanel',
        layout: 'fit',
        //collapsible: true,
        frame: false,
        bodyStyle: { background: 'transparent' },
        border: 0,
        padding: '2 2 2 2',
        layout: {
            type: 'accordion',
            animate: true,
            multi: true,
        },
        items:
        [
            {
                xtype: 'panel', // << fake hidden panel
                hidden: true,
                collapsed: false
            },
            commonLoginPanel, requireLoginPanel , payorPanel, googleSSOPanel, shibbolethSSOPanel
        ]
    });

    self.State.Panel = Ext.create('Ext.Panel', {
        renderTo: Ext.get(self.Options.containerId),
        frame: true,
        autoScroll: false,
        title: 'Public Options',
        items: [
            publicOptionsPanel
        ]
    });

    Ext.on('resize', function () {
        self.State.Panel.doLayout();
    });

    Ext.QuickTips.init();
}
PublicOptionsComponent.prototype.ExecuteFunction = function (command)
{
    var self = this;
    switch (command)
    {
        case 'load':
            self.LoadData();
            break;
        case 'save':
            self.Save();
            break;
    }
}
PublicOptionsComponent.prototype.LoadData = function ()
{
    var self = this;
    var publicOptionModelData = self.Options.Data;
    var modelDataKeys = Object.keys(publicOptionModelData);
    var modelDataValues = Object.values(publicOptionModelData);

    for (var x = 0; x < modelDataKeys.length; x++)
    {
        var modelPropertyName = modelDataKeys[x];
        var modelPropertyValue = modelDataValues[x];
        var componentTarget = Ext.ComponentQuery.query('[name=' + modelPropertyName + ']')[0];
        
        if (componentTarget != null) //if target is found set data
        {
            if (componentTarget.xtype.indexOf(['textfield','textarea','combobox']) > -1)
            {
                componentTarget.setValue(modelPropertyValue);
            }
            else if (componentTarget.xtype == 'radiofield')
            {
                var item = Ext.getCmp(modelPropertyName).items.items;
                for (var y = 0; y < item.length; y++)
                {
                    if (item[y].inputValue == modelPropertyValue)
                    {
                        item[y].setValue(true);
                    }
                }
            }
            
        }
        
    }
}
PublicOptionsComponent.prototype.Save = function ()
{
    var self = this;
}