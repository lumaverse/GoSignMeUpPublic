function Membership(options) {
    var self = this;

    self.PwLabel = options.PwLabel;
    self.UsernameLabel = options.UsernameLabel;
    self.EmailLabel = options.EmailLabel;
    self.HideForgotPassword = options.HideForgotPassword;
    self.PublicSignupAbilityOff = options.PublicSignupAbilityOff;
    self.disallownewuser = options.disallownewuser;
    self.HideStudentLogin = options.HideStudentLogin;
    self.HideSupervisorLogin = options.HideSupervisorLogin;
    self.HideInstructorLogin = options.HideInstructorLogin;
    self.AspSiteRootUrl = options.AspSiteRootUrl;
    self.CurrentLogin = null;
    if (options.LoginAuthOption == 1) {
        self.UsernameEmptyText = self.EmailLabel;
    } else if (options.LoginAuthOption == 2) {
        self.UsernameEmptyText = self.UsernameLabel + " / " + self.EmailLabel;
    } else {
        self.UsernameEmptyText = self.UsernameLabel;
    }

    self.defaultAcnt = "student";
    var hideStudent = (self.HideInstructorLogin == 1 && (self.HideSupervisorLogin == 1 || self.HideSupervisorLogin == 2) ? true : (self.HideStudentLogin == 1 ? true : false))
    var hideInstructor = (self.HideInstructorLogin == 1 ? true : false)
    var hideSupervisor = ((self.HideSupervisorLogin == 1 || self.HideSupervisorLogin == 2) ? true : false)
    if (hideStudent && hideSupervisor) { self.defaultAcnt = "instructor" }
    if (hideInstructor && hideStudent) { self.defaultAcnt = "supervisor" }
    if (hideInstructor && !hideStudent) { self.defaultAcnt = "student" }
    if (hideInstructor && hideSupervisor) { self.defaultAcnt = "student" }

    self.options = options;
    window.MEMBERSHIP = self;
    Ext.onDocumentReady(function () {
		Ext.Ajax.timeout = 120000; // 120 seconds
        if (!self.options.isLoggedIn) {
            self.renderLoginForm();

            if (self.options.showLoginUi) {
                self.showLoginForm();
                window.LAYOUT.notify('The page you have requested is either a restricted area or your session has expired and you have been redirected to the default page.');
            }
        }

        //reload and checkout
        if (Ext.util.Cookies.get('cmdonload') == 'ShowReviewCheckout') {
            Ext.util.Cookies.set('cmdonload', '');
            Ext.Ajax.request({
                url: config.getUrl('public/cart/CheckAuthorization'),
                success: function (response) {
                    var result = Ext.decode(response.responseText);

                    if (result.IsLoggedIn){
                        cart.ShowReviewCheckout();
                    }
                }
            });
        }
        //reload and checkout
        if (Ext.util.Cookies.get('cmdonload') == 'PayNowFromUserDash') {
            var remainderamount = Ext.util.Cookies.get('PayNowFromUserDash-remainderamount')
            var orderno = Ext.util.Cookies.get('PayNowFromUserDash-orderno')
            //console.log('PayNowFromUserDash')
            cart.ShowPaymentPage(remainderamount, orderno, '', 'paynowuserdash');
        }
    });
}

Membership.prototype.constructor = Membership;

Membership.prototype.popupHelper = null;

Membership.prototype.options = {
    loginLabel: 'Login',
    showLoginUi: false,
    isLoggedIn: null,
    loginLinkId: null,
    logoutLinkId: null,
    googleSsoEnabled: false,
    canvasSsoEnabled: false,
    DocumentsFolder: null,
};

Membership.prototype.elementLoginLink = null;

Membership.prototype.container = null;

Membership.prototype.username = null;
Membership.prototype.password = null;
Membership.prototype.loginTypeStudent = null;
Membership.prototype.loginTypeInstructor = null;
Membership.prototype.loginTypeSupervisor = null;

Membership.prototype.UsernameEmptyText = null;
Membership.prototype.UsernameLabel = null;
Membership.prototype.EmailLabel = null;
Membership.prototype.PwLabel = null;

Membership.prototype.HideForgotPassword = 1;
Membership.prototype.PublicSignupAbilityOff = 1;

Membership.prototype.storeUsername = function (username) {
    var self = this;
    var exp = new Date();
    exp = Ext.Date.add(exp, Ext.Date.DAY, 30);
    Ext.util.Cookies.set('membership-username', username, exp,'','', true);
}

Membership.prototype.storeLastLogonType = function (type) {
    var self = this;
    var exp = Ext.Date.add(new Date(), Ext.Date.DAY, 30);
    Ext.util.Cookies.set('membership-type', type, exp, '', '', true);
}

Membership.prototype.getLastLogonType = function () {
    var self = this;
    return Ext.util.Cookies.get('membership-type');
}

Membership.prototype.getStoredUsername = function () {
    var self = this;
    return Ext.util.Cookies.get('membership-username');
}

Membership.prototype.renderLoginForm = function () {
    var self = this;
    self.elementLoginLink = Ext.get(self.options.loginLinkId);

    var enterHandler = function (that, e, opts) {
        if (e.getKey() == e.ENTER) {
            loginAction();
        }
    };

    self.username = Ext.widget({
        xtype: 'textfield',
        emptyText: self.UsernameEmptyText,
        fieldLabel: 'Input ' + self.UsernameEmptyText,
        hideLabel: true,
        //fix for ie9 compatibilty issue
        fieldStyle: 'width:200px;',

        allowBlank: false,
        name: 'username',
        validateOnBlur: false,
        validateOnChange: false,
        value: self.getStoredUsername(),
        enableKeyEvents: true,
        msgTarget: 'under',
        listeners: {
            change: function () {
                self.storeUsername(self.username.getValue());
            },
            keyUp: enterHandler
        }
    });

    self.password = Ext.widget({
        xtype: 'textfield',
        msgTarget: 'under',
        emptyText: self.PwLabel,
        fieldLabel: 'Input ' + self.PwLabel,
        hideLabel: true,

        //fix for ie9 compatibilty issue
        fieldStyle: 'width:200px;',

        allowBlank: false,
        inputType: 'password',
        name: 'password',
        validateOnBlur: false,
        validateOnChange: false,
        enableKeyEvents: true,
        listeners: {
            keyUp: enterHandler
        }
    });

    self.RecoverAccount = Ext.widget({
        xtype: 'component',
        hidden: (self.HideForgotPassword != 1 ? false : true),
        autoEl: {
            tag: 'div',
            style: 'width:100%; text-align:right; height:20px',
            html: '<a id="forgot_password_link" style="text-decoration:none; font-size:9px; color:gray; display:block; margin-right:5px;" href="' + config.getUrl('Public/User/AccountRecovery?usertype=' + self.defaultAcnt) + '">Forgot your ' + self.UsernameLabel.toLowerCase() + ' or password?</a>'
        }
    });

    self.ErrorMessage = Ext.widget({
        xtype: 'box',
        name: 'errormessage',
        id: 'errormessage'
    });

    self.loginTypeStudent = Ext.widget({
        hidden: (self.HideInstructorLogin == 1 && (self.HideSupervisorLogin == 1 || self.HideSupervisorLogin == 2) ? true : (self.HideStudentLogin == 1 ? true : false)),
        xtype: 'radiofield',
        name: 'loginType',
        boxLabel: Terminology.capital('student'),
        fieldLabel: 'Select ' + Terminology.capital('student'),
        hideLabel: true,
        checked: true,
        listeners: {
            change: function (fn, nw, old) {
                if (nw) {
                    if (self.disallownewuser != -1) {
                        $('#forgot_password_link').show();
                    }// SHOW THIS WHEN SELECTED
                    else {
                        $('#forgot_password_link').hide();
                    }
                    $('#forgot_password_link').attr("href", config.getUrl('Public/User/AccountRecovery?usertype=student'))
                    self.storeLastLogonType('student');
                    var login2Radio = Ext.get('student-radio');
                    if (login2Radio != null) {
                        login2Radio.dom.checked = true;
                        login2Radio.fireEvent('change');
                    }

                    Ext.getCmp('login-items-for-hiding').show();
                    Ext.getCmp('membershiploginbutton').show();
                    if (self.options.show_shibb_button) {
                        Ext.getCmp('membershipshibbutton').show();
                    }
                    if (self.options.show_cas_button) {
                        Ext.getCmp('membershipcasbutton').show();
                    }
                   // if (self.options.googleSsoEnabled) {
                     //   Ext.getCmp('membershipgooglebutton').show();
                    //}
                    //if (self.options.canvasSsoEnabled) {
                     //   Ext.getCmp('membershipcanvasbutton').show();
                   // }

                    var membershipinstructorredirect = Ext.getCmp('membershipinstructorredirect');
                    var membershipinstructorcanvasbutton = Ext.getCmp('membershipinstructorcanvasbutton');
                    if (membershipinstructorredirect != null) {
                        membershipinstructorredirect.setVisible(false);
                    }
                    if (membershipinstructorcanvasbutton != null) {
                        membershipinstructorcanvasbutton.setVisible(false);
                    }

                }
            }
        }
    });

    self.loginTypeInstructor = Ext.widget({
        hidden: (self.HideInstructorLogin == 1 ? true : false),
        xtype: 'radiofield',
        name: 'loginType',
        checked: (self.HideStudentLogin == 1 ? true : false),
        boxLabel: Terminology.capital('instructor'),
        fieldLabel: 'Select ' + Terminology.capital('instructor'),
        hideLabel: true,
        listeners: {
            change: function (fn, nw, old) {

                if (nw) {
                    $('#forgot_password_link').show(); // SHOW THIS WHEN SELECTED
                    $('#forgot_password_link').attr("href", config.getUrl('Public/User/AccountRecovery?usertype=instructor'))
                    self.storeLastLogonType('instructor');
                    var login2Radio = Ext.get('instructor-radio');
                    if (login2Radio != null) {
                        login2Radio.dom.checked = true;
                        login2Radio.fireEvent('change');
                    }

                    Ext.getCmp('login-items-for-hiding').show();
                    Ext.getCmp('membershiploginbutton').show();
                   // if (self.options.googleSsoEnabled) {
                      //  Ext.getCmp('membershipgooglebutton').hide();
                   // }
                   // if (self.options.canvasSsoEnabled) {
                     //   Ext.getCmp('membershipcanvasbutton').hide();
                   // }
                    if (self.options.show_shibb_button) {
                        Ext.getCmp('membershipshibbutton').hide();
                    }
                    if (self.options.show_cas_button) {
                        Ext.getCmp('membershipcasbutton').hide();
                    }
                    var membershipinstructorredirect = Ext.getCmp('membershipinstructorredirect');
                    var membershipinstructorcanvasbutton = Ext.getCmp('membershipinstructorcanvasbutton');
                    if (membershipinstructorredirect != null) {
                        membershipinstructorredirect.setVisible(false);
                    }
                    if (membershipinstructorcanvasbutton != null) {
                        membershipinstructorcanvasbutton.setVisible(false);
                    }

                }

            }
        }
    });

    self.loginTypeSupervisor = Ext.widget({
        hidden: (self.HideSupervisorLogin == 1 || self.HideSupervisorLogin == 2 ? true : false),
        xtype: 'radiofield',
        name: 'loginType',
        checked: (self.HideInstructorLogin == 1 && self.HideStudentLogin == 1 ? true : false),
        boxLabel: Terminology.capital('supervisor'),
        fieldLabel: 'Select ' + Terminology.capital('supervisor'),
        hideLabel: true,
        listeners: {
            change: function (fn, nw, old) {
                if (nw) {
                    
                    $('#forgot_password_link').attr("href", config.getUrl('Public/User/AccountRecovery?usertype=supervisor'))
                    self.storeLastLogonType('supervisor');
                    var login2Radio = Ext.get('supervisor-radio');
                    if (login2Radio != null) {
                        login2Radio.dom.checked = true;
                        login2Radio.fireEvent('change');
                    }


                    Ext.getCmp('login-items-for-hiding').show();
                    Ext.getCmp('membershiploginbutton').show();
                   // if (self.options.googleSsoEnabled) {
                    ////    Ext.getCmp('membershipgooglebutton').hide();
                    //}
                    //if (self.options.canvasSsoEnabled) {
                        //Ext.getCmp('membershipcanvasbutton').hide();
                  //  }
                    if (self.options.show_shibb_button) {
                        Ext.getCmp('membershipshibbutton').hide();
                    }
                    if (self.options.show_cas_button) {
                        Ext.getCmp('membershipcasbutton').hide();
                    }
                    var membershipinstructorredirect = Ext.getCmp('membershipinstructorredirect');
                    var membershipinstructorcanvasbutton = Ext.getCmp('membershipinstructorcanvasbutton');
                    if (membershipinstructorredirect != null) {
                        membershipinstructorredirect.setVisible(false);
                    }
                    if (membershipinstructorcanvasbutton != null) {
                        membershipinstructorcanvasbutton.setVisible(false);
                    }
                    $('#forgot_password_link').show(); // HIDE THIS WHEN SELECTED

                }

            }
        }
    });

    var loginAction = function () {
        self.login('login1');
    };

    var loginItems = [
            {
                xtpye: 'container',
                id: 'login-items-for-hiding',
                bodyStyle: 'background:transparent;',
                width: 205,
                frame: false,
                border: false,
                items: [
                    self.username,
                    self.password,
                    self.RecoverAccount,
                    self.ErrorMessage
                ]
            },
            {
                xtype: 'container',
                width: 200,
                layout: {
                    type: 'hbox',
                    align: 'top',
                    pack: 'left'
                },
                items: [
                    {
                        xtype: 'container',
                        layout: 'vbox',
                        items: [
                            self.loginTypeStudent,
                            self.loginTypeInstructor,
                            self.loginTypeSupervisor
                        ]
                    },
                ]
            }
    ];
    var buttonWidth = 200;
    var buttonShibWith = 120;
    divider = 1;
    if (self.options.googleSsoEnabled) {
     //   divider++;
    }
    if (self.options.canvasSsoEnabled) {
      //  divider++;
    }
    buttonWidth = buttonWidth / divider;

    if (self.options.show_shibb_button) {
        buttonWidth = 80;
        divider++;
    }
    if (self.options.show_cas_button) {
        buttonWidth = 80;
        divider++;
    }
    loginItems.push({
        id:'membershiploginbutton',
        xtype: 'button',
        style: {
            marginTop: '5px'
        },
        width: buttonWidth,
        text: self.options.loginLabel,
        icon: config.getUrl('Images/Icons/famfamfam/lock.png'),
        handler: loginAction
    });
   // if (self.options.googleSsoEnabled) {
       // loginItems.push({
          //  id: 'membershipgooglebutton',
          //  xtype: 'button',
          //  text: 'Google',
          //  tooltip: 'Google' + self.options.loginLabel,
           // style: {
              //  marginTop: '5px'
           // },
           // width: buttonWidth,
           // icon: config.getUrl('Images/Icons/socialmediaicons/google-16x16.png'),
           // handler: function () {
            //    self.GoogleLogin();
           // }
        //});
    //}
   // if (self.options.canvasSsoEnabled) {
        //loginItems.push({
          //  id: 'membershipcanvasbutton',
           // xtype: 'button',
           // text: self.options.Canvas_LoginLabel,
           // tooltip: self.options.Canvas_LoginLabel,
           // style: {
               // marginTop: '5px'
           // },
           // width: buttonWidth + 30,
           // icon: config.getUrl('Images/IntegrationPartners/lti_canvas.png'),
            //handler: function () {
              //  self.CanvasLogin(false, 'student');
          //  }
     //   });
   // }

    if (self.options.show_shibb_button) {
        loginItems.push({
            id: 'membershipshibbutton',
            xtype: 'button',
            text: self.options.Shibb_LoginLabel,
            tooltip: self.options.Shibb_LoginLabel,
            style: {
                marginTop: '5px'
            },
            width: buttonShibWith,
            icon: config.getUrl('Images/IntegrationPartners/shibbolethicon.png'),
            handler: function () {
                self.ShibbolethLogin('login1');
            }
        });
    }

    if (self.options.show_cas_button) {
        loginItems.push({
            id: 'membershipcasbutton',
            xtype: 'button',
            text: self.options.Cas_LoginLabel,
            tooltip: self.options.Cas_LoginLabel,
            style: {
                marginTop: '5px'
            },
            width: buttonShibWith,
            icon: config.getUrl('Images/IntegrationPartners/cas_icon.png'),
            handler: function () {
                self.CasLogin(self.options.casurl);
            }
        });
    }

    var instructorButtonWidth = 200;
    //if (self.options.canvasSsoEnabled) {
      //  instructorButtonWidth = 150;
  //  }
    loginItems.push({
        id: 'membershipinstructorredirect',
        hidden:true,
        xtype: 'button',
        style: {
            marginTop: '5px'
        },
        width: instructorButtonWidth,
        text: 'Proceed',
        tooltip: 'Proceed to ' + Terminology.capital('instructors') + ' ' + self.options.loginLabel,
        icon: config.getUrl('Images/Icons/famfamfam/lock.png'),
        handler: function() {
            self.instructorredirect();
        }
    });
    /*if (self.options.canvasSsoEnabled) {
        loginItems.push({
            hidden: true,
            id: 'membershipinstructorcanvasbutton',
            xtype: 'button',
            text: self.options.Canvas_LoginLabel,
            tooltip: 'Canvas ' + Terminology.capital('instructors') + ' ' + self.options.Canvas_LoginLabel,
            style: {
                marginTop: '5px'
            },
            width: instructorButtonWidth,
            icon: config.getUrl('Images/IntegrationPartners/lti_canvas.png'),
            handler: function () {
                self.CanvasLogin(false, 'instructor');
            }
        });
    }
    */

    /*
    This can be removed, 10/23/2014 - started implementing supervisor login

    loginItems.push({
        id: 'membershipsupervisorredirect',
        hidden: true,
        xtype: 'button',
        style: {
            marginTop: '5px'
        },
        width: 200,
        text: 'Proceed to ' + Terminology.capital('supervisors') + ' Login',
        icon: config.getUrl('Images/Icons/famfamfam/lock.png'),
        handler: function () {
            document.location = self.AspSiteRootUrl + 'dev_supervisors.asp?action=login';
        }
    });
    */

    self.container = Ext.create('Ext.container.Container', {
        renderTo: Ext.getBody(),
        floating: true,
        hidden: true,
        componentCls: 'login-popup',
        shadow: true,
        items: loginItems
    });

    self.popupHelper = new PopupHelper({
        popupElement: Ext.get(self.container.getId()),
        documentClickAction: function () {
            self.hideLoginForm();
        }
    });

    Ext.on('resize', function () {
        if (self.container.isVisible()) {
            self.setContainerPosition(false);
        }
    });
}

Membership.prototype.loginShibboleth= function(userType)
{


    url = 'AuthMe/LoginShibbolethUser';
    
    Ext.Ajax.request({
        url: url,
        params: {
            type: userType
        },
        success: function (response) {
            var result = response.responseText;
            result = result.replace("Public", "/Public")
            document.location = result;
        }
    });
}


Membership.prototype.loginBB = function (userType) {


    url = 'AuthMe/LoginBBUser';

    Ext.Ajax.request({
        url: url,
        params: {
            type: userType
        },
        success: function (response) {
            var result = response.responseText;
            result = result.replace("Public", "/Public")
            document.location = result;
        }
    });
}

Membership.prototype.loginCanvas = function (userType) {


    url = 'SSO/LoginSelectedCanvasUser';

    Ext.Ajax.request({
        url: url,
        params: {
            type: userType
        },
        success: function (response) {
            var result = response.responseText;
            result = result.replace("Public", "/Public")
            document.location = result;
        }
    });
}

Membership.prototype.login = function (loginsrc) {
    var self = this;
    var url;
    var notimplmeneted = false;
    var otherForm = false;

    var loginTypeStudent = false;
    var loginTypeInstructor = false;
    var loginTypeSupervisor = false;
    var username;
    var password;
    var containerEl;
    if ((loginsrc == 'login2')  || (loginsrc == 'login3')) {
        username = Ext.getCmp('membershiploginformusername');
        password = Ext.getCmp('membershiploginformpassword');
        containerEl = Ext.get('loginmaincnt');
        var loginTypeValue = Ext.dom.Query.selectValue('input[name=logintype]:checked/@value');
        switch (loginTypeValue) {
            case '1':
                loginTypeStudent = true;
                break;

            case '2':
                loginTypeInstructor = true;
                break;

            case '3':
                loginTypeSupervisor = true;
                break;

            default:
                loginTypeStudent = true;
        }

        otherForm = true;
    } else {
        loginTypeStudent = self.loginTypeStudent.getValue();
        loginTypeInstructor = self.loginTypeInstructor.getValue();
        loginTypeSupervisor = self.loginTypeSupervisor.getValue();
        username = self.username;
        password = self.password;
        containerEl = self.container.getEl();
    }

    if (loginTypeInstructor) {
        url = config.getUrl('public/membership/LoginInstructor');
        loginTypeStudent = 0
        loginTypeSupervisor = 0
    } else if (loginTypeSupervisor) {
        url = config.getUrl('public/membership/LoginSupervisor');
    } else {
		// for loginTypeStudent or hidden login type selection
        url = config.getUrl('public/membership/LoginStudent');
	}
	
    if (self.notImplementedLogin(otherForm)) {
        return;
    };

    if (username.isValid() && password.isValid()) {
        LAYOUT.MaskLayout('Logging in...');
        if (loginsrc == 'login1') {
            self.hideLoginForm();
        }

        Ext.Ajax.request({
            url: url,
            params: {
                username: username.getValue(),
                password: password.getValue()
            },
            success: function (response) {
                var result = Ext.decode(response.responseText);
                LAYOUT.UnmaskLayout();

                if (!result.success) {
                    if (loginsrc == 'login1') {
                        self.showLoginForm(true);
                    }
                    // var box = Ext.MessageBox.show({
                    //animateTarget: self.container,
                    //   title: 'Login',
                    // msg: result.error,
                    //icon: Ext.MessageBox.WARNING,
                    //buttons: Ext.MessageBox.OK
                    //});
                    if ((loginsrc == 'login2') || (loginsrc == 'login3')) {
                        $("#errormsg").html(result.error);
                    }
                    else {
                        username.markInvalid(result.error);
                        //Ext.getCmp('errormessage').update('<div style="color:red; font-weight: bold;">' + result.error + '</div>')
                        //self.popupHelper.configureForMessageBox(box);
                    }
                    return;
                }

                // login type student
                if (loginTypeStudent) {
                    if (result.studentkey) {
                        var studentSessionId = result.studentkey;
                        if (studentSessionId.indexOf('studentguid') > -1) {
                            //SERVICE API ACCESS
                            var sessionId = studentSessionId.split(':')[1].trim();
                            var tokenRaw = encodeURIComponent(btoa(username.getValue() + ':' + sessionId));
                            var roleRaw = encodeURIComponent(btoa('student'));
                            localStorage.setItem('sessionId', sessionId);
                            localStorage.setItem('token', JSON.stringify(tokenRaw));
                            localStorage.setItem('role', JSON.stringify(roleRaw));
                        }
                    }
                    self.CheckReqMissingFields("login", result.messages, true, result, loginsrc);
                    
                } else if (loginTypeSupervisor) {
                    document.location = config.getUrl('public/supervisor');
                    self.CurrentLogin = "supervisor";
                    
                } else if (loginTypeInstructor) {
                    if (loginsrc == 'login3') {
                        document.location = config.getUrl(surveypageredirect);
                        self.CurrentLogin = "instructor";
                    }
                    else {
                        document.location = config.getUrl('public/instructor');
                        self.CurrentLogin = "instructor";
                    }
                }
            }
        });
    }
};
Membership.prototype.ShibbolethLogin = function (loginfrom) {
    var self = this;
    if (shibbRedirectPopup == "true ") {
        var r = confirm("You will be redirected to another page to login. Click here to continue.");
        if (r == true) {
            if (loginfrom != 'login1') {
                var params = new window.URLSearchParams(window.location.search);
                var returnURL = (params.get('returnUrl'));
                if (returnURL == "" || returnURL == null) {
                    Ext.util.Cookies.set('cmdonload', 'ShowReviewCheckout');
                }
            }
            if (shib_stud_for_survey == '') {
                document.location = "/Shibboleth.sso/Login";
            }
            else {
                shib_stud_for_survey = "";
            }
        }
        else {
            document.location = "/Public/Course/Browse";
            return;
        }
    }
    else {
        if (loginfrom != 'login1') {
            var params = new window.URLSearchParams(window.location.search);
            var returnURL = (params.get('returnUrl'));
            if (returnURL == "" || returnURL == null) {
                Ext.util.Cookies.set('cmdonload', 'ShowReviewCheckout');
            }
        }
        if (shib_stud_for_survey == '') {
            document.location = "/Shibboleth.sso/Login";
        }
        else {
            shib_stud_for_survey = "";
        }
    }
};
Membership.prototype.CasLogin = function (casUrl) {
    var self = this;
    var r = confirm("You will be redirected to another page to login. Click here to continue.");
    if (r == true) {
        serviceurl =window.location.protocol+"//"+ window.location.hostname;
        document.location = casUrl + "Login/?service=" + serviceurl;
    }
    else {
        document.location = "/Public/Course/Browse";
        return;
    }
};

Membership.prototype.instructorredirect = function () {
    var self = this;
    document.location = self.AspSiteRootUrl + 'dev_instructors.asp?action=login';
};

Membership.prototype.CheckReqMissingFields = function (cmd, param1, bool, result, loginsrc) {
    var self = this;
    window.LAYOUT.MaskLayout('Loading ...');
    Ext.Ajax.request({
        url: config.getUrl('public/user/CheckReqMissingFields'),
        success: function (response) {
            if (response.responseText == "NoMissingReqFields") {
                window.LAYOUT.UnmaskLayout();
                if (cmd == "login") {
                    self.showLoginInfo(param1, bool, result, loginsrc);
                    return;
                }
                if (cmd == "checkout") {
                    cart.ShowReviewCheckoutExec(param1);
                    return;
                }
                
                Ext.MessageBox.show({
                    title: 'Error Message',
                    msg: 'Command not implemented yet',
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.MessageBox.ERROR
                });
        } else {
                document.location = config.getUrl('public/user/dashboard?MissingReqFields=1');
            }
        }
    });
};


Membership.prototype.notImplementedLogin = function (otherForm) {
    var self = this;
    var notImplemented = false;

    if (!Ext.isDefined(otherForm)) {
        otherForm = false;
    }
    if (!otherForm) {
        if (self.loginTypeInstructor.getValue()) {
            notImplemented = false;
        } else if (self.loginTypeSupervisor.getValue()) {
            notImplemented = false;
        } else {
			// for loginTypeStudent or hidden login type selection
			notImplemented = false;
		}
    } else {
        var student = (self.HideStudentLogin == 1 ? false : Ext.get('student-radio').dom.checked);
        var instructor = (self.HideInstructorLogin == 1 || !Ext.get('instructor-radio') ? false : Ext.get('instructor-radio').dom.checked);
        var supervisor = (self.HideSupervisorLogin == 1 || self.HideSupervisorLogin == 2 ? false : Ext.get('supervisor-radio').dom.checked);

        if (instructor) {
            notImplemented = false;
        } else if (supervisor) {
            notImplemented = false;
        } else {
			// for loginTypeStudent or hidden login type selection
			notImplemented = false;
		}
    }

    if (notImplemented) {

        var box = Ext.MessageBox.show({
            //animateTarget: self.container,
            title: 'Login',
            msg: 'This login type is not implemented yet',
            icon: Ext.MessageBox.WARNING,
            buttons: Ext.MessageBox.OK
        });
        self.popupHelper.configureForMessageBox(box);

    }

    return notImplemented;
}

Membership.prototype.setContainerPosition = function (animate) {
    var self = this;

    var position = self.elementLoginLink.getBox();
    var left = position.left - self.container.getWidth() + position.width;
    var top = position.top + position.height;
    self.container.setPosition(left, top, animate);
};

Ext.util.Cookies.set('FirstToRunOnLogInInit', 'false');

Membership.prototype.showLoginForm = function (noFocus, firstShow) {
    var self = this;
    if (typeof (noFocus) == 'undefined') {
        noFocus = false;
    }

    if (typeof (firstShow) == 'undefined') {
        firstShow = true;
    }

    if (firstShow) {
        self.popupHelper.firstShow = true;
    }

    var FirstToRunOnLogInInit = Ext.util.Cookies.get('FirstToRunOnLogInInit');
    var FirstToRunOnLogInClick = Ext.util.Cookies.get('FirstToRunOnLogInClick');

    if (FirstToRunOnLogInClick != "popuphelper") {
        self.popupHelper.popupClicked = true;
    }

    if (typeof (FirstToRunOnLogInInit) == 'null' || FirstToRunOnLogInInit == 'false') {
        Ext.util.Cookies.set('FirstToRunOnLogInClick', 'membership');
        Ext.util.Cookies.set('FirstToRunOnLogInInit', true);
    }

    switch (self.getLastLogonType()) {
        case 'student':
            self.loginTypeStudent.setValue(true);
            self.loginTypeSupervisor.setValue(false);
            self.loginTypeInstructor.setValue(false);
            break;

        case 'instructor':
            self.loginTypeInstructor.setValue(true);
            self.loginTypeStudent.setValue(false);
            self.loginTypeSupervisor.setValue(false);
            break;

        case 'supervisor':
            self.loginTypeSupervisor.setValue(true);
            self.loginTypeInstructor.setValue(false);
            self.loginTypeStudent.setValue(false);
            break;
    }

    self.elementLoginLink.addCls('login-popup-link');
    self.container.show();
    self.setContainerPosition(false);
    self.username.focus();

}

Membership.prototype.hideLoginForm = function () {
    var self = this;
    self.elementLoginLink.removeCls('login-popup-link');
    self.container.hide();
    self.popupHelper.popupClicked = false;
}

Membership.prototype.logoutShibboleth = function (logoutlink) {
    var self = this;
    window.LAYOUT.MaskLayout('Logging out');
    Ext.Ajax.request({
        url: config.getUrl('public/membership/Logout'),
        success: function (response) {
            if ((logoutlink != "") || (logoutlink != null) || (logoutlink != undefined)) {
                setTimeout(function () { document.location = logoutlink }, 500);
            }
            else {
                setTimeout(function () { document.location = '/Shibboleth.sso/Logout' }, 500);
            }
        }
    });
}
Membership.prototype.logoutCas = function (logoutlink) {
    var self = this;
    window.LAYOUT.MaskLayout('Logging out');
    Ext.Ajax.request({
        url: config.getUrl('public/membership/Logout'),
        success: function (response) {
            if ((logoutlink == "") || (logoutlink == null) || (logoutlink == undefined)) {
                setTimeout(function () { document.location = '/Public/Course/Browse' }, 500);
            }
            else {
                setTimeout(function () { window.open(logoutlink + "/logout") }, 500);
                setTimeout(function () { document.location = '/Public/Course/Browse' }, 500);
            }
        }
    });
}

Membership.prototype.logout = function () {
    var self = this;
    //LAYOUT.logout();
    window.LAYOUT.MaskLayout('Logging out');
    Ext.Ajax.request({
        url: config.getUrl('public/membership/Logout'),
        success: function (response) {
            var data = Ext.decode(response.responseText);
            if (data.activeUser == "supervisor") {
                var ifra = document.createElement('iframe');
                ifra.src = "/admin/logoff.asp?type=supervisor&misc=559";
                ifra.setAttribute("height", "230");
                ifra.setAttribute("width", "360");
                void (document.body.appendChild(ifra));
                setTimeout(function () { document.location = '/Public' }, 500);
            }
            else if (data.activeUser == "instructor") {
                var ifra = document.createElement('iframe');
                ifra.src = "/admin/logoff.asp?type=instructor&misc=559";
                ifra.setAttribute("height", "230");
                ifra.setAttribute("width", "360");
                void (document.body.appendChild(ifra));
                setTimeout(function () { document.location = '/Public' }, 500);
            }
            else {
                setTimeout(function () { document.location = '/Public' }, 500);
            }

            localStorage.removeItem('token');
            localStorage.removeItem('role');
        }
    });
}

Membership.prototype.enrollmentInfo = function (dateAdded) {
    Ext.MessageBox.show({
        msg: 'You enrolled in this course on ' + dateAdded + '.',
        title: 'Enrollment Info',
        buttons: Ext.MessageBox.OK,
        buttonAlign: 'right',
        icon: Ext.MessageBox.INFO
    });
}

Membership.prototype.showLoginInfo = function (messages, reload, result, loginsrc, completeAction, requireCartContents) {
    var self = this;
    if (self.PublicSignupAbilityOff > 0) {
        loginsrc = "";
    }
    if (typeof (requireCartContents) == 'undefined') {
        requireCartContents = false;
    }

    var originalCompleteAction = function () {

        if (window.LAYOUT.State.queryString["loginRedirectUrl"]) {
            document.location = window.LAYOUT.State.queryString["loginRedirectUrl"];
            return;
        }

        if (!Ext.isDefined(reload) || reload == true) {

            var returnUrl = $('#LoginreturnUrl').val();
            var hash = $('#LoginreturnUrlhash').val();
            if (self.PublicSignupAbilityOff > 0) {
                returnUrl = "";
            }
            if (returnUrl == undefined) { returnUrl = ""; }

            var redirect = function (url) {
                LAYOUT.MaskLayout('Please hold on, you are being redirected to your home page...');
                location = url;
            };

            if (returnUrl.length > 5) {
                redirect(returnUrl + hash);
            } else if (result.ForceUpdate == 1) {
                if ((loginsrc == 'login2') || (loginsrc == 'login3')) {
                    if (loginsrc == 'login2'){
                        //cart.ShowReviewCheckout();
                        //reload and checkout
                        Ext.util.Cookies.set('cmdonload', 'ShowReviewCheckout');
                        location.reload();

                    } else
                    if (loginsrc == 'login3') {
                        redirect(surveypageredirect);
                    } else {
                        location.reload();
                    }
                }
                else {
                    redirect(config.getUrl('public/user/dashboard'));
                }
            } else {
                if ((loginsrc == 'login2') || (loginsrc == 'login3')) {
                    if (loginsrc == 'login2') {
                        cart.setReloadCallback('ShowReviewCheckout');
                        redirect('/public/course/Browse');
                    } else
                    if (loginsrc == 'login3') {
                        window.location = surveypageredirect;
                    } else {
                        location.reload();

                    }
                } else if (result.StrtupPage == 0) {
                    redirect(config.getUrl('public/user/dashboard'));
                } else if (result.StrtupPage == 1) {
                    if ((loginsrc == 'login2') || (loginsrc == 'login3'))  {
                        if (loginsrc == 'login2') {
                            cart.setReloadCallback('ShowReviewCheckout');
                            redirect('/public/course/browse');
                        } else
                        if (loginsrc == 'login3') {
                            redirect(surveypageredirect);
                        } else {
                            location.reload();
                        }
                    }
                    else {
                        redirect(config.getUrl('public/course/browse'));
                    }
                } else if (result.StrtupPage == 2) {
                    redirect(config.getUrl('public/' + result.OtherStrtupPage));
                } else {
                    location.reload();
                }
            }
        }
    };
    if (!Ext.isFunction(completeAction)) {
        completeAction = originalCompleteAction;
    }

    var resultCall = function () {
        completeAction(result.cartItemCount > 0, originalCompleteAction);
    }

    if (result.cartItemCount == 0 && requireCartContents) {
        if (!Ext.isArray(messages)) {
            messages = [];
        }
        messages.push('<br/>Your cart is empty now , please fill your cart again to be able to execute this step...');
    }

    if (Ext.isArray(messages) && messages.length > 0) {
        var msg = 'You have been successfully logged in!';
        msg = LAYOUT.getResultMessage(msg, messages);

        Ext.MessageBox.show({
            animateTarget: self.container,
            title: 'Login',
            msg: msg,
            buttons: Ext.MessageBox.OK,
            icon: Ext.MessageBox.INFO,
            fn: function () {
                resultCall();
            }
        });
    } else {
        resultCall();
    }
}

Membership.prototype.GoogleLogin = function (otherForm) {
    var self = this;

    var notImplemented = self.notImplementedLogin(otherForm);
    if (notImplemented) {
        return false;
    }

    if (window.CART.activeCheckoutStep == 'cartlogin') {
        window.CART.setReloadCallback('ShowReviewCheckout');
    }
    document.location = config.getUrl('SSO/Google');
}

Membership.prototype.CanvasLogin = function (otherForm, reason) {
    var self = this;

    var notImplemented = self.notImplementedLogin(otherForm);
    if (notImplemented) {
        return false;
    }

    if (window.CART.activeCheckoutStep == 'cartlogin') {
        window.CART.setReloadCallback('ShowReviewCheckout');
    }
    document.location = config.getUrl('SSO/Canvas?reason=' + reason);
}


Membership.prototype.UserAction = function (cid, orderno, control, cmd, rosterid, remainderamount, thiscontrol) {
    var self = this;
    if (cmd == "0") {
        return;
    }
    if (cmd == "printreciept") {
        //var win = window.open(location.href.replace('dashboard', 'orderconfirmation' + '?order=' + orderno).replace('/Supervisor/EditStudentInfo', '/user/orderconfirmation' + '?order=' + orderno).replace('/DashboardViewAdmin', '/orderconfirmation' + '?order=' + orderno +'&'));
        var link = location.href.replace('dashboard', 'orderconfirmation' + '?order=' + orderno).replace('/Supervisor/EditStudentInfo', '/user/orderconfirmation' + '?order=' + orderno).replace('/DashboardViewAdmin', '/orderconfirmation' + '?order=' + orderno + '&');
        link = link.indexOf('/orderconfirmation?order=' + orderno + '?sid') > -1 ? link.replace('?sid', '&sid') : link;
        link = link.indexOf('/orderconfirmation?order=' + orderno + '?MissingReqFields') > -1 ? link.replace('??MissingReqFields', '&?MissingReqFields') : link;

        var win = window.open(link);
    }
    else if (cmd == "viewcoursework") {
        ViewCourseWork(cid);
        control.value = "0";
    }
    else if (cmd == "cancelcourse") {
        self.CancelRoster(rosterid);
    }
    else if (cmd == "movetoenroll") {
        self.MoveToEnrollRoster(rosterid);
    }
    else if (cmd == "paybalance") {
        cart.ShowPaymentPage(remainderamount, orderno,'', 'paynowuserdash');
    }
    else {
        Ext.Msg.alert("User Dashboard", "cid:" + cid + " cmd:" + cmd + "orderno" + orderno);
    }

    thiscontrol.value = 0;
}

Membership.prototype.CancelRoster = function (rosterid) {
    var self = this;
    var modal = Ext.getCmp('UserDashboardCourses');
    var modal_others = Ext.getCmp('OtherUserDashCourse');
    Ext.Msg.confirm('Confirm Action', 'Are you sure you want to cancel this course? If this is a fast track course, it will remove all other included courses.', function (btn) {
        if (btn == 'yes') {
            modal.mask('Loading...');
            if (typeof (modal_others) != 'undefined') { modal_others.mask('Loading...'); }
            Ext.Ajax.request({
                url: config.getUrl('public/Course/CancelRoster'),
                timeout: 60000,
                params: {
                    rosterid: rosterid
                },
                success: function (response) {
                    window.location.reload();
                    modal.unmask();
                    if (typeof (modal_others) != 'undefined') { modal_others.unmask(); }
                },
                failure: function (response)
                {
                    Ext.MessageBox.show({
                        title: 'Error Message',
                        msg: 'There was an issue encountered',
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.ERROR
                    });
                    modal.unmask();
                }
            });
            
        }
    });
}

Membership.prototype.MoveToEnrollRoster = function (rosterid) {
    var self = this;
    var modal = Ext.getCmp('UserDashboardCourses');
    Ext.Msg.confirm('Confirm Action', 'Are you sure you want to move the student off the waiting list?', function (btn) {
        if (btn == 'yes') {
            modal.mask('Loading...');
            Ext.Ajax.request({
                url: config.getUrl('public/Course/MoveToEnrollRoster'),
                timeout: 60000,
                params: {
                    rosterid: rosterid
                },
                success: function (response) {
                    window.location.reload();
                    modal.unmask();
                },
                failure: function (response) {
                    Ext.MessageBox.show({
                        title: 'Error Message',
                        msg: 'There was an issue encountered',
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.ERROR
                    });
                    modal.unmask();
                }
            });

        }
    });
}

Membership.prototype.CourseWorkAction = function (cmd, url, count) {
    var self = this;
    var tblworkheight = $('#tblCourseWorks').css('height');
    if (cmd == "print") {
        var win = window.open(self.options.DocumentsFolder + url);
        win.print();
    }

    else if (cmd == "download") {
        window.open(self.options.DocumentsFolder + url, 'Download');
    }
    else if (cmd == "email") {

        $('#tblCourseWorks').slideUp('fast', function () {
            $("#tblCourseWorks").css("display", "none");
        });

        $('#tblEmailFunction').slideUp('fast', function () {
            $("#tblEmailFunction").css('height', tblworkheight);
            $("#tblEmailFunction").css("display", "block");
            $("#divattached").html(url);

        });
    }
    else if (cmd == "back") {
        $('#tblCourseWorks').slideUp('fast', function () {
            $("#tblCourseWorks").css("display", "block");
        });

        $('#tblEmailFunction').slideUp('fast', function () {
            $("#tblEmailFunction").css("display", "none");
        });
    }
}


function ViewCourseWork(cid) {
    var cntrpanel = Ext.create('Ext.form.Panel', {
        border: false,
        frame: false,
        bodyPadding: 0,
        html: '',
        anchor: '100% 100%'
    });

    var window = Ext.create('Ext.window.Window', {
        id: 'CourseWork',
        modal: true,
        closeAction: 'destroy',
        closable: true,
        border: false,
        frame: false,
        header: true,
        layout: 'anchor',
        //y:10,
        tbar: false,
        width: 630,
        height: 180,
        autoScroll: true,
        items: cntrpanel
    });
    window.show();
    cntrpanel.setLoading(true);
    Ext.Ajax.request({
        url: config.getUrl('public/Course/CourseWork'),
        params: {
            courseid: cid
        },
        success: function (response) {
            cntrpanel.setLoading(false);
            cntrpanel.hide();
            $("html, body").animate({ scrollTop: 0 }, "slow");
            window.update(response.responseText, true, function () {

            });
        }
    });
}
