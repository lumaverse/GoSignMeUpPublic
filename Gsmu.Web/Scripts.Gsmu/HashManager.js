function HashManager(options) {
    var self = this;
    self.options = Ext.merge(self.options, options);

    self.state.urlVars = UrlHelper.getUrlVars();
    self.state.useHistory = typeof (history.pushState) != 'undefined';

    if ((location.hash == '' || location.hash.length < 3) && self.options.enableCookies && self.options.enabled ) {
        var jsn = Ext.util.Cookies.get(self.options.cookie);
        self.state.viewState = Ext.decode(self.decompressString(jsn));
        self.saveState();
    }

    if (self.options.enabled) {
        if (self.state.useHistory && Ext.isDefined(self.state.urlVars.viewstate) &&
            (Ext.isDefined(self.state.urlVars['AllowDirectLoad']) || window.LAYOUT.Options.developmentMode)
        ) {
            self.state.viewState = Ext.decode(self.decompressString(self.state.urlVars.viewstate));
            self.state.stateIsValid = true;
        } else {
            var defaultInit = function () {
                self.state.viewState = {};
                self.state.stateIsValid = false;
            }

            try {
                var hash = location.hash.substring(1);
                if (hash != '') {
                    var json = self.decompressString(hash);
                    self.state.viewState = Ext.decode(json);
                    self.state.stateIsValid = true;
                } else {
                    defaultInit();
                }
            } catch (e) {
                defaultInit();
            }
        }
    }
    self.saveState();
}

HashManager.prototype.constructor = HashManager;

HashManager.prototype.options = {
    cookie: 'HashManager',
    enableCookies: true,
    enabled: true
}

HashManager.prototype.state = {
    hashIsValid: false,
    urlVars: {},
    viewState: {}
}

HashManager.prototype.setHashVar = function (id, value) {

    var self = this;

    if (self.options.enabled == false) {
        return value;
    }

    self.state.viewState[id] = value;
    self.state.stateIsValid = true;
}

HashManager.prototype.saveState = function () {
    var self = this;
    self.saveState();
}

HashManager.prototype.saveState = function () {
    var self = this;

    if (Ext.Object.getKeys(self.state.viewState).length < 1) {
        return;
    }

    var jsn = Ext.encode(self.state.viewState);
    jsn = self.compressString(jsn);

    var tempScrollTop = $(window).scrollTop();
    var tempScrollLeft = $(window).scrollLeft();


    // save state
    if (self.state.useHistory) {
        self.state.urlVars['viewstate'] = jsn;
        delete self.state.urlVars['message'];
        delete self.state.urlVars['adminmode'];
        var url = UrlHelper.buildUrl(self.state.urlVars);
        history.pushState(jsn, "hash-manager", url);
        location.hash = '';
    } else {
        location.hash = jsn;
    }

    //to mimimized the jumping of page
    $(window).scrollTop(tempScrollTop);
    $(window).scrollLeft(tempScrollLeft);
    $("html, body").animate({ scrollTop: tempScrollTop, scrollLeft: tempScrollLeft }, "slow");

    if (self.options.enableCookies) {
        var expires = new Date();
        expires.setFullYear(expires.getFullYear() + 1);
        Ext.util.Cookies.set(self.options.cookie, jsn, expires, '/', window.location.hostname);
    }
}

HashManager.prototype.getHashVar = function (id, defaultResult) {

    var self = this;
    if (self.options.enabled == false) {
        return defaultResult;
    }

    var obj = self.state.viewState;

    if ((typeof (obj) == 'undefined' || typeof (obj[id]) == 'undefined') && typeof (defaultResult) != 'undefined') {
        return defaultResult;
    }

    return obj[id];
}

HashManager.prototype.compressString = function (string) {
    var self = this;
    try {
        var result = window.btoa(string);
    } catch (e) {
        return string;
    }
    return result;
}

HashManager.prototype.decompressString = function (string) {
    var self = this;

    try {
        var result = window.atob(string);
    } catch (e) {
        return string;
    }
    return result;
}