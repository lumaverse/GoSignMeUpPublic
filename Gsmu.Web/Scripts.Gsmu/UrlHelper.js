var UrlHelper = {
    getUrlVars: function() {
        var vars = {}, hash;
        if (window.location.search == '') {
            return vars;
        }
        var hashes = window.location.search.substr(1);
        hashes = hashes.split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars[hash[0]] = unescape(hash[1]);
        }
        return vars;
    },

    getUrlWithoutVariable: function (removable) {

        var vars = this.getUrlVars();
        if (removable instanceof Array) {
            for (var index = 0; index < removable.length; index++) {
                delete vars[removable[index]];
            }
        } else {
            delete vars[removable];
        }
        return this.buildUrl(vars);
    },

    buildUrl: function(vars) {
        var url = location.protocol + '//' + location.hostname;
        if (location.port != 80 && location.port != 443) {
            url += ':' + location.port;
        }
        url += location.pathname;
        var search = '';
        for (var key in vars) {
            var value = vars[key];
            search += search == '' ? '?' : '&';
            search += key + '=' + escape(value);
        }

        url += search + location.hash;

        return url;
    },

    getUrl: function() {
        var url = location.protocol + '//' + location.hostname + location.pathname;
        url += location.search + location.hash;
        return url;
    }
}