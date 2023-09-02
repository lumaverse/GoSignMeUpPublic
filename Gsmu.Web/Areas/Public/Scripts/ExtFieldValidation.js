var emailvalue = 'E-mail already exist or invalid format "user@example.com"';
var emailinput = "onload";
var validEmailWhitlist = true;
var EmailNotification = "";
var currentValueForUsernameValidation = "";
Ext.apply(Ext.form.field.VTypes, {
    empty: function (val, field) {
        return true;
    },

    daterange: function (val, field) {
        var date = field.parseDate(val);

        if (!date) {
            return false;
        }
        if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime()))) {
            var start = field.up('form').down('#' + field.startDateField);
            start.setMaxValue(date);
            start.validate();
            this.dateRangeMax = date;
        }
        else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime()))) {
            var end = field.up('form').down('#' + field.endDateField);
            end.setMinValue(date);
            end.validate();
            this.dateRangeMin = date;
        }
        return true;
    },

    daterangeText: 'Start date must be less than end date',

    emailcon: function (val, field) {
        if (field.initialPassField) {
            var pwd = field.up('form').down('#' + field.initialPassField);
            return (val == pwd.getValue());
        }
        return true;
    },
    emailconText: 'Emails do not match',
    emailvalText: 'Error value',
    checkEmailResctrict: function (val, field) {
        var self = this;
        var valarr = val.split("@");
        var valdomain = "@" + valarr[1];
        if ((emailinput != val)) {
            Ext.Ajax.request({
                async: false,
                method: 'POST',
                url: config.getUrl('public/user/CheckEmailRestriction'),
                params: {
                    domain: valdomain,
                    email: val
                },
                success: function (response) {
                    var model = Ext.decode(response.responseText);
                    if (model.OnOff == 1) {
                        if (model.valid) {
                            if (field.vtypeclass == "emailcon2") {
                                validEmailWhitlist = self.checkUsername(val, field);
                            } else {
                                field.invalidText = '';
                                validEmailWhitlist = true;
                            }
                            return validEmailWhitlist;
                        } else {
                            validEmailWhitlist = false;
                            //"This email is not listed in the allowed email postfix."
                             EmailNotification = model.EmailNotification
                            field.invalidText = EmailNotification;
                            return false;
                        }
                    } else {
                        if (field.vtypeclass == "emailcon2") {
                            validEmailWhitlist = self.checkUsername(val, field);
                        } else {
                            field.invalidText = '';
                            validEmailWhitlist = true;
                        }
                        return validEmailWhitlist;
                    }
                },
                failure: function () {
                    alert("Error in connection. Please contact system administrator. EVError501")
                }
            });
        }
        emailinput = val;
        if (!validEmailWhitlist && EmailNotification !="") {
            field.invalidText = EmailNotification;
        }
        return validEmailWhitlist;

    },

    checkUsername: function (val, field) {

        if (field.originalValue == val) {
            field.invalidText = '';
            validfield = true;
            return validfield;
        }
        Ext.Ajax.request({
            async: false,
            method: 'POST',
            url: config.getUrl('public/user/CheckStudentUsernameAvailable'),
            params: {
                Username: val
            },
            success: function (response) {
                if (response.responseText == "available") {
                    validfield = true;
                    field.invalidText = '';
                } else if (response.responseText == "notavailableBB") {
                    validfield = false;
                    field.invalidText = 'Username already exist in either GSMU or Blackboard. Please login with the existing account.';
                } else if (response.responseText == "notavailableCanvas") {
                    validfield = false;
                    field.invalidText = 'Username already exist in Canvas system. Please login with the existing account.';
                } else if (response.responseText == "notavailableGSMUorCanvas") {
                    validfield = false;
                    field.invalidText = 'Username already exist in Gosignmeup or Canvas system. Please login with the existing account.';
                } else if (response.responseText == "invalidhaiku") {
                    validfield = false;
                    field.invalidText = 'Username contains invalid charater(s).';
                } else {
                    validfield = false;
                    field.invalidText = 'Username already exist. Please try another...';
                }
                return validfield;
            },
            failure: function () {
                alert("Error in connection. Please contact administrator. EVError502")
            }
        });
        return validfield;
    },

    emailval: function (val, field) {
        field.invalidText = '';

        if (val == "") {
            return true;
        }

        // email format
        var validEmailFormat = checkEmailFormat(val);
        if (!validEmailFormat) {
            var validEmailFormatError = 'Invalid e-mail address format "user@example.com"';
            field.invalidText = validEmailFormatError;
            return false;
            
        }

        //whitelist
        if (validEmailFormat) {
           var retval = this.checkEmailResctrict(val, field);
            return retval;
        }
    },

    emailcon2X: function (val, field) {
        var validfield = true;
        var orgnalusername = field.originalValue;
        var filter = /^([a-zA-Z0-9_\.\-\w-'])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;

        if (!filter.test(val)) {
            validfield = false;
            var texterror = 'Invalid e-mail address format "user@example.com"';
            //$('#' + field.errorWrapEl.id).html('<div role="alert" aria-live="polite" data-ref="errorEl" class="x-form-error-msg x-form-invalid-under x-form-invalid-under-default"><ul class="x-list-plain"><li>' + texterror + '</li></ul></div>');
            return;
        } else {
            //$('#' + field.errorWrapEl.id).html('<div role="alert" aria-live="polite" data-ref="errorEl" class="x-form-error-msg x-form-invalid-under x-form-invalid-under-default"><ul class="x-list-plain"><li></li></ul></div>');

            Ext.Ajax.request({
                async: false,
                method: 'POST',
                url: config.getUrl('public/user/CheckStudentUsernameAvailable'),
                params: {
                    Username: val
                },
                success: function (response) {
                    if (orgnalusername == val) {
                        validfield = true;
                        return;

                    }
                    if (response.responseText == "available" || response.responseText == "invalidhaiku") {
                        field.clearInvalid();
                        validfield = true;
                    }else {
                        validfield = false;
                        texterror = 'E-mail address already exist. Please try another.';
                        //$('#' + field.errorWrapEl.id).html('<div role="alert" aria-live="polite" data-ref="errorEl" class="x-form-error-msg x-form-invalid-under x-form-invalid-under-default"><ul class="x-list-plain"><li>' + texterror + '</li></ul></div>');
                        return;
                    }
                },
                failure: function () {
                    alert("Error in connection. Please contact system administrator. EVError503")
                }
            });

        }

        return validfield;
    },

    emailcon2Text: emailvalue,

    password: function (val, field) {
        if (field.initialPassField) {
            var pwd = field.up('form').down('#' + field.initialPassField);
            var result = (val == pwd.getValue());
            if (!result) {
                Ext.getCmp('errdisp_psscnfmid').setValue('Passwords do not match');
                Ext.getCmp('errdisp_psscnfmid').show();
            }
            else {
                Ext.getCmp('errdisp_psscnfmid').setValue('');
            }
            return result;
        }
        return true;
    },

    passwordText: 'Passwords do not match',

    chkusername: function (val, field) {
        var validfield = false;
        if (/\s/g.test(val)) {
            val = val.replace(/\s+/g, '');
            Ext.getCmp(field.id).setValue(val);
        }
        var orgnalusername = field.originalValue;
        Ext.Ajax.request({
            async: false,
            method: 'POST',
            url: config.getUrl('public/user/CheckStudentUsernameAvailable'),
            params: {
                Username: val
            },
            success: function (response) {
                if (response.responseText == "available") {
                    field.clearInvalid();
                    validfield = true;
                } else if (response.responseText == "invalidhaiku") {
                    validfield = false;
                    field.invalidText = 'Username contains invalid charater(s).';
                } else {
                    validfield = false;
                    if (orgnalusername == val) {
                        validfield = true;
                    }
                    else {
                        field.invalidText = 'Username already exist. Please try another .'
                    }
                }
            },
            failure: function () {
                alert("Error in connection. Please contact system administrator. EVError504")
            }
        });
        return validfield;
    },

    //chkusernameText: ['Username already exist. Please try another.', 'Spaces not allowed.'],
    chkusernameText: 'Username already exist. Please try another .',

    chkusernameIT: function (val, field) {
        var validfield = false;
        var orgnalusername = field.originalValue;
        Ext.Ajax.request({
            async: false,
            method: 'POST',
            url: config.getUrl('public/user/CheckInstructorUsernameAvailable'),
            params: {
                Username: val
            },
            success: function (response) {
                if (response.responseText == "available") {
                    field.clearInvalid();
                    validfield = true;
                } else {
                    validfield = false;
                    if (orgnalusername == val) {
                        validfield = true;
                    }
                }
            },
            failure: function () {
                alert("Error in connection. Please contact system administrator. EVError505")
            }
        });
        return validfield;
    },

    chkusernameITText: 'Username already exist. Please try another..',


    phoneText: 'Not a valid phone number. Must be in the format (123) 456-7890.',
    phoneMask: /[\-\+0-9\(\)\s\.Ext]/,
    //phoneRe: /^(\({1}[0-9]{3}\){1}\s{1})([0-9]{3}[-]{1}[0-9]{4})$|^(((\+44)? ?(\(0\))? ?)|(0))( ?[0-9]{3,4}){3}$|^Ext. [0-9]+$/,
    phoneRe: /^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$|^Ext. [0-9]+$/,
    phone: function (v) {
        return this.phoneRe.test(v);
    },

    SS1Text: 'Not a valid SS number. Must be in the format 123-45-6789.',
    SS1Mask: /[\-\+0-9\s\.Ext]/,
    SS1Re: /^\d{3}-\d{2}-\d{4}$|^Ext. [0-9]+$/,
    SS1: function (v) {
        return this.SS1Re.test(v)||v=="************";
    },

    SS2Text: 'Not a valid SS number. Must be in the format 1234.',
    SS2Mask: /[\-\+0-9\s\.Ext]/,
    SS2Re: /^\d{4}$|^Ext. [0-9]+$/,
    SS2: function (v) {
        return this.SS2Re.test(v) || v == "************";
    },
    emailCommaSeparated: function (val, field)
    {
        var validEmailFormatError = 'Invalid e-mail address format "user@example.com". Use semi-colon to separate emails (for multiple emails)';
        var isValid = true;
        val = val.trim();
        field.invalidText = '';
        
        if (val == "") {
            isValid = true;
        }
        if (val.length > 200)
        {
            validEmailFormatError = 'Invalid length of email/s you entered. Make sure it only has 200 characters all, including semi-colon.'
            field.invalidText = validEmailFormatError;
            isValid = false;
        }
        if (val.indexOf(';') > -1) // contains multiple emails
        {
            var multipleEmails = val.split(';');
            multipleEmails.map(function (email) {
                var validEmailFormat = checkEmailFormat(email);
                if (!validEmailFormat) {
                    field.invalidText = validEmailFormatError;
                    isValid = false;
                }
            });
        }
        else // single email
        {
            // email format
            var validEmailFormat = checkEmailFormat(val);
            if (!validEmailFormat) {
                field.invalidText = validEmailFormatError;
                isValid = false;

            }
        }
        return isValid;
    }


});


Ext.apply(Ext.util.Format, {
    phoneNumber: function (value) {
        var phoneNumber = value.replace(/\./g, '').replace(/-/g, '').replace(/[^0-9]/g, '');

        if (phoneNumber != '' && phoneNumber.length >= 10) {
            return '(' + phoneNumber.substr(0, 3) + ') ' + phoneNumber.substr(3, 3) + '-' + phoneNumber.substr(6, 4);
        } else {
            return value;
        }
    },
    // ###-##-####
    SS1: function (value) {
        var SS1 = value.replace(/\./g, '').replace(/-/g, '').replace(/[^0-9]/g, '');

        if (SS1 != '' && SS1.length >= 9) {
            return SS1.substr(0, 3) + '-' + SS1.substr(3, 2) + '-' + SS1.substr(5, 4);
        } else {
            return value;
        }
    },

    // ####
    SS2: function (value) {
        var SS2 = value.replace(/\./g, '').replace(/-/g, '').replace(/[^0-9]/g, '');

        if (SS2 != '' && SS2.length >= 4) {
            return SS2.substr(0, 4);
        } else {
            return value;
        }
    }

});

Ext.namespace('Ext.ux.plugin');

// Plugin to format a phone number on value change
Ext.ux.plugin.FormatPhoneNumber = Ext.extend(Ext.form.TextField, {
    init: function (c) {
        c.on('change', this.onChange, this);
    },
    onChange: function (c) {
        c.setValue(Ext.util.Format.phoneNumber(c.getValue()));
    }
});

Ext.ux.plugin.FormatSS1 = Ext.extend(Ext.form.TextField, {
    init: function (c) {
        c.on('change', this.onChange, this);
    },
    onChange: function (c) {
        c.setValue(Ext.util.Format.SS1(c.getValue()));
    }
});

Ext.ux.plugin.FormatSS2 = Ext.extend(Ext.form.TextField, {
    init: function (c) {
        c.on('change', this.onChange, this);
    },
    onChange: function (c) {
        c.setValue(Ext.util.Format.SS2(c.getValue()));
    }
});
