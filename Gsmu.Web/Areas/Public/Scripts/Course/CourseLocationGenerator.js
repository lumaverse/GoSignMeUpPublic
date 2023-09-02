// requires addressgenerator class

function CourseLocationGenerator() {
}

CourseLocationGenerator.constructor = CourseLocationGenerator;

CourseLocationGenerator.prototype.generate = function (data, value, field, options) {
    var self = this;

    options = Ext.merge({
        offset: [42, -8],
        extraCssClass: ''
    }, options);

    for (var key in data) {
        if (!Ext.isObject(data[key])) {
            data[key] = Ext.String.trim(data[key] + "");
        }
    }
    var addressGenerator = new Address();
    var addressString = addressGenerator.getAddressString(data);
    var mapid = 'course-location-generator-map-' + Math.floor((Math.random() * 10000) + 1) +'-'+ Math.floor((Math.random() * 10000) + 1);

    if ((data.location || addressString) || (addressString != '' || data.location!='')) {
        var displayString = data.location;

        if (Ext.isEmpty(displayString)) {
            displayString = addressString;
        }
        var display = '';
        if (!Ext.isEmpty(addressString)) {
            display += '<a href="javascript:window.CourseSearchInstance.ShowMapPanel(\''+ mapid +'\');">';
        }
        display += displayString;
        if (!Ext.isEmpty(addressString)) {
            display += '</a>';
            display += '<input type="hidden" id="cslocation' + mapid + '" value="' + data.location + '" />';
            display += '<input type="hidden" id="csstreet' + mapid + '" value="' + data.street + '" />';
            display += '<input type="hidden" id="cscity' + mapid + '" value="' + data.city + '" />';
            display += '<input type="hidden" id="csstate' + mapid + '" value="' + data.state + '" />';
            display += '<input type="hidden" id="cszip' + mapid + '" value="' + data.zip + '" />';
            display += '<input type="hidden" id="cscountry' + mapid + '" value="' + data.country + '" />';
            display += '<input type="hidden" id="addressString' + mapid + '" value="' + addressString + '" />';
        }

        if (display == 'null') {
            return;
        }
        value.setHtml(display);


        var tipDisplay = addressString;
        if (!Ext.isEmpty(data.locationadditionalinfo) && data.locationadditionalinfo != 'null') {
            tipDisplay += '<br/>' + data.locationadditionalinfo;
        }
        if (!Ext.isEmpty(data.locationurl) && data.locationurl != 'null') {
            tipDisplay += '<br/><a target="_blank" href="' + data.locationurl + '">Location Url</a>';
        }
        if (!Ext.isEmpty(data.room) && data.room != 'null') {
            tipDisplay += '<br/>Room: ' + data.room;
        }
        if (Ext.isObject(data.roomdirection)) {
            tipDisplay += '<br/>' + data.roomdirection.RoomDirectionsTitle + ': ' + data.roomdirection.RoomDirectionsInfo;
        }

        if (tipDisplay == '') {
            return;
        }

        tipDisplay = '<div style="width:300px">' + tipDisplay + '<br/><br/></div>';
        field.setStyle('cursor', 'pointer');
        var container = GSMUTOOLTIP.CreateTooltip({
            target: value,
            trigger: field,
            offset: options.offset,
            extraCssClass: options.extraCssClass
        });

        container.add({
            xtype: 'container',
            html: tipDisplay
        });
        container.getEl().appendChild({
            tag: 'div',
            style: 'height:200px; width:300px;',
            html: 'loading...',
            id: mapid
        });

        //Transfered to CourseSearch.js, it failed in some sites beacause of the Google map query limits  
        //addressGenerator.verifyAddress(data, function (valid) {
        //    if (!valid) {
        //        return;
        //    }
            //var component = addressGenerator.getGoogleMapPanel(
            //    data,
            //    !data.location ? addressString : data.location,
            //    {
            //        width: 300,
            //        height: 200
            //    }
            //);
            //container.insert(1, component);
       // });

    } else {
        field.hide();
    }
}