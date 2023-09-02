// this requires the following javascripts loaded
// https://maps.googleapis.com/maps/api/js?v=3&sensor=false
// extjs/examples/ux/GMapPanel.js

function Address() {
}

Address.constructor = Address;

Address.addressValidationCache = {
}

Address.addressValidationCacheResult = {
}

Address.prototype.getAddressString = function (data) {
    var self = this;

    var address = '';
    var comma = '';
    var commaString = ', ';
    if (!Ext.isEmpty(data.street) && data.street != null && data.street != 'null') {
		if(data.street == '3130 holen ave')
		{
			data.street = '';
		}
        address += comma + data.street;
        comma = commaString;
    }
    if (!Ext.isEmpty(data.city) && data.city != null && data.city != 'null') {
        address += comma + data.city;
        comma = commaString;
    }
    if (!Ext.isEmpty(data.state) && data.state != null && data.state != 'null') {
        address += comma + data.state;
        comma = commaString;
    }
    if (!Ext.isEmpty(data.zip) && data.zip != null && data.zip != 'null') {
        address += comma + data.zip;
        comma = commaString;
    }
    if (!Ext.isEmpty(data.country) && data.country != null && data.country != 'null') {
        address += comma + data.country;
        comma = commaString;
    }
    return address;
}

Address.prototype.generateUrl = function (data, callback) {
    var self = this;

    var createAddress = function (type) {
        var url;
        switch (type) {
            case 'mapquest':
                url = 'http://www.mapquest.com/?q=';
                break;

            case 'google':
                url = 'http://maps.google.com/?q=';
                break;
        }

        var address = self.getAddressString(data);
        url += address;
        win.close();
        if (Ext.isFunction(callback)) {
            callback(url);
        }
    }

    /*
    city: "fullerton"
    country: ""
    location: "fullerton jc college"
    locationadditionalinfo: ""
    locationid: 1008
    locationurl: "test"
    sortorder: 6
    state: "CA"
    street: "michealson st."
    zip: "92618"
    */

    var items = [];

    var mapquestButton = {
        xtype: 'button',
        text: 'Generate MapQuest link',
        flex: 1,
        handler: function () {
            createAddress('mapquest');
        }
    };

    var googleButton = {
        xtype: 'button',
        text: 'Generate Google Map link',
        flex: 1,
        handler: function () {
            createAddress('google');
        }
    }

    var getWin = function (items) {
        if (items.length == 0) {
            alert('Sorry, this function is disabled at the moment.');
            return null;
        }
        var win = Ext.create('Ext.window.Window', {
            modal: true,
            title: 'Generate MAP Url for location',
            width: Math.max(items.length * 150, 200),
            resizable: false,
            layout: {
                type: 'hbox',
                pack: 'stretch'
            },
            items: items
        });
        win.show();
        return win;
    }


    if (Ext.isDefined(LiteApi)) {
        var liteapi = LiteApi.getInstance();
        liteapi.onReady(function () {
            if (liteapi.isFeatureEnabled(liteapi.options.featureNames.googlemap)) {
                items.push(googleButton);
            }
            if (liteapi.isFeatureEnabled(liteapi.options.featureNames.mapquestmap)) {
                items.push(mapquestButton);
            }
            win = getWin(items);
        });
    } else {
        items.push(googleButton);
        items.push(mapquestButton);
        win = getWin(items);
    }
}

Address.prototype.verifyAddress = function (data, callback) {
    var self = this;

    var addressString = self.getAddressString(data);

    if (Ext.isDefined(Address.addressValidationCache[addressString])) {
        callback(Address.addressValidationCache[addressString], Address.addressValidationCacheResult[addressString]);
        return;
    }
    var geocoder = new google.maps.Geocoder();
    geocoder.geocode({
        address: addressString
    }, function (results, status) {
        if (Ext.isFunction(callback)) {
            var valid = status == 'OK' && status != 'ZERO_RESULTS';
            Address.addressValidationCache[addressString] = valid;
            Address.addressValidationCacheResult[addressString] = results;
            callback(valid, results);
        }
    });
}

Address.prototype.showGoogleMapWindow = function (data, title, options, centerCallback) {
    var self = this;

    var cmp = self.getGoogleMapPanel(data, title);
    var win = Ext.create('Ext.window.Window', {
        modal: true,
        layout: 'fit',
        title: title,
        width: 400,
        height: 400,
        resizable: true,
        items: [
            cmp
        ]
    });
    win.show();
}

Address.prototype.getGoogleMapPanel = function (data, title, options, centerCallback) {
    var self = this;

    if (!Ext.isFunction(centerCallback)) {
        centerCallback = function () {
        };
    }

    var address = self.getAddressString(data);
    options = Ext.merge({
        center: {
            geoCodeAddr: address,
            marker: {
                title: title,
                listeners: {
                    click: centerCallback
                }
            }
        }
    }, options);

    var component = Ext.create('Ext.ux.GMapPanel', options);
    return component;
}