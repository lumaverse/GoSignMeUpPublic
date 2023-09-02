function Config(options) {
    var self = this;

    self.Options = Ext.merge(self.Options, options);

    self.initializeSiteUrls();

    self.initializeAjaxErrorHandling();

    var cp = Ext.create('Ext.state.CookieProvider', {
        expires: new Date(new Date().getTime() + (1000 * 60 * 60 * 24 * 90 /* xx days */ ))
    });
    Ext.state.Manager.setProvider(cp);

    Ext.onReady(function() {
        Ext.on('beforeunload', function () {
            self.Options.DisableAjaxErrorHandling = true;
        });

        Ext.on('unload', function () {
            self.Options.DisableAjaxErrorHandling = true;
        });
    });
}

Config.prototype.constructor = Config;

Config.prototype.Options = {
    DisableAjaxErrorHandling: false,
    root: null,
    dotNetSiteUrl: null,
    aspSiteUrl: null,
    sessionCookie: null
}

Config.prototype.hasSessionCookie = function () {
    var self = this;

    var cookie = Ext.util.Cookies.get(self.Options.sessionCookie);
    return cookie != null;
}

Config.prototype.initializeJqueryAjaxErrorHandling = function () {
    var self = this;

    if (Ext.ieVersion < 9) {
        $(document).ajaxComplete(function (event, xhr, settings) {
            self.runPlaceholderPlugin();
        });
    }

}

Config.prototype.requestComplete = null;

Config.prototype.runPlaceholderPlugin = function () {
    var self = this;

    clearTimeout(self.requestComplete);
    self.requestComplete = setTimeout(function () {
        //  $('input, textarea').placeholder();
        $('form').find("input[type=textarea], input, textarea").each(function (ev) {
            if (!$(this).val()) {
               // $(this).attr("placeholder", "");
            }
        });
    }, 500);
}

Config.prototype.initializeAjaxErrorHandling = function () {
    var self = this;

    var notify = function (text, popup) {
        if (typeof (popup) == 'undefined') {
            popup = true;
        }
        if (window.LAYOUT && window.LAYOUT.Options.developmentMode && popup) {
            window.LAYOUT.notify('Ajax Application error occured, please check the developer console for details.');
        }
        var div = document.createElement("div");
        div.innerHTML = text;
        var style = div.getElementsByTagName('style')[0];
       // div.removeChild(style);
        var text = div.textContent || div.innerText || "";
        text = text.replace(/\s{2,}$/g, ' ');
        log(text);
    }

    var errorHandler = function (text, suspendException) {

        log('---------------------------------------')
        log('--------------ERROR START--------------')
        log('---------------------------------------')

        if (suspendException) {
            notify(text);
        } else if (window.LAYOUT && window.LAYOUT.Options.developmentMode) {
            var win = new Ext.Window({
                title: 'AJAX Application Error Occured, see details in Window',
                width: Math.max(document.documentElement.clientWidth - 300, 100),
                height: Math.max(document.documentElement.clientHeight - 300, 100),
                layout: 'fit',
                modal: true,
                items: [{
                    xtype: "component",
                    autoEl: {
                        tag: "iframe",
                        src: "data:text/html;charset=utf-8," + escape(text),
                        style: {
                            border: '0'
                        }
                    }
                }]
            }).show();
            notify(text, false);
        } else {
            notify(text);
        };

        log('---------------------------------------')
        log('--------------ERROR END----------------')
        log('---------------------------------------')

    }

    jQuery.ajaxSetup({
        error: function (xhr, status, error) {
            if (self.Options.DisableAjaxErrorHandling) {
                return;
            }
            errorHandler(xhr.responseText, xhr.suspendException);
        }
    });


    Ext.Ajax.on('requestexception', function (conn, response, options, eopts) {
        if (self.Options.DisableAjaxErrorHandling) {
            return;
        }
        var text;
        if (typeof (response.responseText) == 'undefined') {
            text = response.statusText;
        } else {
            text = response.responseText;
        }
        errorHandler(text, options.suspendException);
    });

    if (Ext.ieVersion < 9) {
        Ext.Ajax.on('requestcomplete', function (conn, response, options, eopts) {
            self.runPlaceholderPlugin();
        });
    }

}

Config.prototype.initializeSiteUrls = function () {
    var self = this;

    var dotNetSiteUrl = location.protocol + '//' + location.hostname + self.Options.root;

    if (dotNetSiteUrl != self.Options.dotNetSiteUrl) {
        self.Options.dotNetSiteUrl = dotNetSiteUrl;
        Ext.Ajax.request({
            method: 'POST',
            url: self.getUrl('Application/SaveApplicationUrl'),
            params: {
                url: dotNetSiteUrl
            }
        });
    }
}

Config.prototype.getUrl = function (relative, isAbsolute) {
    var self = this;

    if (typeof (relative) != 'undefined' && relative != null && relative != '' && relative.charAt(0) == '/') {
        relative = relative.substr(1);
    }

    if (typeof (isAbsolute) != 'undefined' && isAbsolute == true) {
        return self.Options.dotNetSiteUrl + relative;
    }

    return self.Options.root + relative;
}

Config.prototype.showError = function (text) {
    var self = this;

    Ext.MessageBox.show({
        title: 'Error',
        msg: text,
        buttons: Ext.Msg.OK,
        buttonAlign: 'right',
        icon: Ext.MessageBox.ERROR
    });
}

Config.prototype.underConstruction = function () {
    var self = this;

    self.showDevelopmentInfo('This feature is under construction so please be patient until we complete it for you.', 'Under construction');
}

Config.prototype.showDevelopmentInfo = function(msg, title) {

    if (typeof(title) == 'undefined') {
        title = 'Under construction';
    }

    var self = this;
    self.showInfo(msg, title);
}

Config.prototype.showInfo = function (msg, title) {
    var self = this;

    self.showMessageBox(msg, title, Ext.MessageBox.INFO);

}

Config.prototype.showWarning = function (msg, title) {
    var self = this;

    self.showMessageBox(msg, title, Ext.MessageBox.WARNING);

}

Config.prototype.showMessageBox = function (msg, title, icon) {
    var self = this;

    Ext.MessageBox.show({
        title: title,
        msg: msg,
        buttons: Ext.Msg.OK,
        buttonAlign: 'right',
        icon: icon
    });

}

Config.prototype.postData = function (url, params) {
    var self = this;

    var form = document.createElement("form");
    form.setAttribute("method", 'post');
    form.setAttribute("action", url);

    for (var key in params) {
        if (params.hasOwnProperty(key)) {
            var hiddenField = document.createElement("input");
            hiddenField.setAttribute("type", "hidden");
            hiddenField.setAttribute("name", key);
            hiddenField.setAttribute("value", params[key]);
            form.appendChild(hiddenField);
        }
    }
    document.body.appendChild(form);
    form.submit();
}
