
CourseSearch.prototype.CourseVariables = {
    CourseID: 0,
    CourseName: '',
    CourseNum: '',
    CourseDescription: ''
}

function CourseSearch(options) {
    var self = this;
    window.CourseSearchInstance = this;
    window.COURSESEARCH = this;

    self.ViewListType = window.LAYOUT.Options.layoutConfiguration.SearchColumns.DefaultView;

    var urlVars = UrlHelper.getUrlVars()
    if (!isNaN(urlVars['courseid'])) {
        var courseid = urlVars['courseid'];
        var newurl = UrlHelper.getUrlWithoutVariable('courseid');
        if (history.pushState) {
            history.pushState(null, null, newurl);
        };
        Ext.onDocumentReady(function () {
            self.ShowCourseDetails(courseid);
        });
    }

    if (!isNaN(urlVars['showMembership'])) {
        var Membership = urlVars['showMembership'];
        var newurl = UrlHelper.getUrlWithoutVariable('showMembership');
        if (history.pushState) {
            history.pushState(null, null, newurl);
        };
        Ext.onDocumentReady(function () {
            self.ShowMembershipDetails();
        });
    }

    self.Hash = new HashManager({
        cookie: 'CourseSearch',
        enableCookies: false
    });

    if (typeof (options) != 'undefined') {
        self.PageSize = typeof (options["defaultPageSize"]) == 'undefined' ? 10 : options.defaultPageSize;
        self.showPublicAnncmnt = typeof (options["showPublicAnncmnt"]) == 'undefined' ? false : options.showPublicAnncmnt;
    }
    if (Ext.isDefined(options) && Ext.isDefined(options.resultElementId)) {
        self.ResultElement = Ext.get(options.resultElementId);
    }

    window.onpopstate = Ext.bind(function (ev) {
        if (adminmode_call != "true") {
            if (ev.state != null) {
                var state = self.Hash.decompressString(ev.state);
                self.Hash.state.viewState = Ext.decode(state);
                self.loadSerializedData();
                self.Invoke();
                cslc.Init();
            }
        }
    });

    self.loadSerializedData();

    var internalUrl = location.pathname.toLowerCase().indexOf('/browseinternal') > -1;
    self.CourseInternal = internalUrl ? true : false;

}

CourseSearch.constructor = CourseSearch;

CourseSearch.ConstructForLayout = function (composer, options) {

    var id = composer.getLayoutContainerComponentContainerId(options.container);
    composer.State.courseSearch.SetResultElementId(id);
    composer.State.courseSearch.Invoke();
    return composer.State.courseSearch;
};

// defaults
CourseSearch.prototype.Defaults = {
    ClearElementImage: 'Images/Icons/glyph2/Icons16x16/delete.png'
};

// indicates the course search is loading
CourseSearch.prototype.IsLoading = false;

// The element that shows the search results
CourseSearch.prototype.ResultElement = null;

// See ViewListType enum for values
CourseSearch.prototype.ViewListType = 'Grid';

// indexing starts from 1, to be human, so page 1 is page 1, there is no page 0
CourseSearch.prototype.Hash = null;

// indexing starts from 1, to be human, so page 1 is page 1, there is no page 0
CourseSearch.prototype.Page = 1;

// number of items per page
CourseSearch.prototype.PageSize = 20;

// See CourseOrderByField enum
CourseSearch.prototype.OrderByField = 'SystemDefault';

// See OrderByDirection enum
CourseSearch.prototype.OrderByDirection = 'Ascending';

// See CourseActiveState enum for values
CourseSearch.prototype.CourseActiveState = 'Current';

// main category - text value
CourseSearch.prototype.MainCategory = '';

// subcategory - text value
CourseSearch.prototype.SubCategory = '';

//sECOND sub category
CourseSearch.prototype.SubSubCategory = '';

CourseSearch.prototype.SubCategoryIsSubSub = false;

// keyword search
CourseSearch.prototype.Text = '';

// mm/dd/yyyt date value for from
CourseSearch.prototype.DateFrom = null;

// mm/dd/yyyt date value for until
CourseSearch.prototype.DateUntil = null;

// see Gsmu.Api.Data.School.Course.CourseInternalState for string values
CourseSearch.prototype.CourseInternal = false;

// see Gsmu.Api.Data.School.Course.CourseCancelState for string values
CourseSearch.prototype.CancelState = 'NotCancelled';

// course detail opout
CourseSearch.prototype.CoursePopout = 0;

// initial run
CourseSearch.prototype.InitialRun = true;
CourseSearch.prototype.ActvViewListType = null;
CourseSearch.prototype.showPublicAnncmnt = false;

CourseSearch.prototype.ShowCourseDetails = null;

CourseSearch.prototype.ShowMembershipDetails = null;

CourseSearch.prototype.docScrollTop = 0;
CourseSearch.prototype.adminSearchShowInternal = false;
CourseSearch.prototype.adminSearchShowClosedPast = false;
CourseSearch.prototype.adminSearchShowMembership = false;

CourseSearch.prototype.loadSerializedData = function () {
    var self = this;

    self.EnsureViewListType();

    self.CancelState = self.Hash.getHashVar('CancelState', self.CancelState);
    self.MainCategory = self.Hash.getHashVar('MainCategory', self.MainCategory);
    self.SubCategory = self.Hash.getHashVar('SubCategory', self.SubCategory);

    self.SubCategoryIsSubSub = self.SubSubCategory;

    self.DateFrom = self.Hash.getHashVar('DateFrom', self.DateFrom);
    self.DateUntil = self.Hash.getHashVar('DateUntil', self.DateUntil);
    self.ViewListType = self.Hash.getHashVar('ViewListType', self.ViewListType);
    self.Page = self.Hash.getHashVar('Page', self.Page);
    self.PageSize = self.Hash.getHashVar('PageSize', self.PageSize);
    self.OrderByField = self.Hash.getHashVar('OrderByField', self.OrderByField);
    self.OrderByDirection = self.Hash.getHashVar('OrderByDirection', self.OrderByDirection);
    self.CourseActiveState = self.Hash.getHashVar('CourseActiveState', self.CourseActiveState);
    self.Text = self.Hash.getHashVar('Text', self.Text);
    self.CoursePopout = self.Hash.getHashVar('CoursePopout', self.CoursePopout);
}

CourseSearch.prototype.EnsureViewListType = function () {
    var self = this;

    if (!window.LAYOUT.Options.layoutConfiguration.SearchColumns[self.ViewListType + 'ViewEnabled']) {
        if (window.LAYOUT.Options.layoutConfiguration.SearchColumns['GridViewEnabled']) {
            self.ViewListType = 'Grid';
        } else if (window.LAYOUT.Options.layoutConfiguration.SearchColumns['TileJulyViewEnabled']) {
            self.ViewListType = 'TileJuly';
        }
    }
}

CourseSearch.prototype.serializeData = function () {
    var self = this;

    if (self.ViewListType == 'Anncmnt') {
        self.ViewListType = self.ActvViewListType;
    }

    self.EnsureViewListType();

    if (self.showPublicAnncmnt) {
        if (self.InitialRun) {
            if (!window.LAYOUT.Options.publicCourseListingFastLoad && !self.Hash.state.stateIsValid) {
                self.ActvViewListType = self.ViewListType
                self.ViewListType = 'Anncmnt';
            }
            self.InitialRun = false;
        }
    }
    self.Hash.setHashVar('ViewListType', self.ViewListType);

    self.Hash.setHashVar('Page', self.Page);
    self.Hash.setHashVar('PageSize', self.PageSize);
    self.Hash.setHashVar('OrderByField', self.OrderByField);
    self.Hash.setHashVar('OrderByDirection', self.OrderByDirection);
    self.Hash.setHashVar('CourseActiveState', self.CourseActiveState);
    self.Hash.setHashVar('MainCategory', self.MainCategory);
    self.Hash.setHashVar('SubCategory', self.SubCategory);
    self.Hash.setHashVar('subsubcattext', self.SubSubCategory);
    self.Hash.setHashVar('SubCategoryIsSubSub', self.SubCategoryIsSubSub);
    self.Hash.setHashVar('Text', self.Text);
    self.Hash.setHashVar('DateFrom', self.DateFrom);
    self.Hash.setHashVar('DateUntil', self.DateUntil);
    self.Hash.setHashVar('CancelState', self.CancelState);
    self.Hash.setHashVar('CoursePopout', self.CoursePopout);
    self.Hash.saveState();

}

CourseSearch.prototype.SetResultElementId = function (id) {
    var self = this;

    self.ResultElement = Ext.get(id);
}
CourseSearch.prototype.SetAdminShowInternal = function () {
    var self = this;
    if (self.adminSearchShowInternal) {
        $(".internalcoursesshowed").attr("src", "/Images/icons/gsmu/Keyoff.png");
        self.adminSearchShowInternal = false;
        $('#course-search-left-category-container').load('Public/Category/LeftCategories?ShowPastCourses=' + self.adminSearchShowClosedPast);
        self.Invoke();
    }
    else {
        self.adminSearchShowInternal = true;
        $(".internalcoursesshowed").attr("src", "/Images/icons/gsmu/Keyon.png");
        $('#course-search-left-category-container').load('Public/Category/LeftCategories?courseInternal=true&ShowPastCourses='+self.adminSearchShowClosedPast );
        self.Invoke();
    }
    
}

CourseSearch.prototype.SetAdminShowMembership = function () {
    var self = this;
    if (self.adminSearchShowMembership) {
        $(".membershipcoursesshowed").attr("src", "/Images/icons/glyph2/icons24x24/User-2.png");
        self.adminSearchShowMembership = false;
        $('#course-search-left-category-container').load('Public/Category/LeftCategories?ShowPastCourses=' + self.adminSearchShowClosedPast);
        self.Invoke();
    }
    else {
        self.adminSearchShowMembership = true;
        $(".membershipcoursesshowed").attr("src", "/Images/icons/glyph2/icons24x24/User-2.png");
        document.location =('/public/Course/Browse?showMembership=1');
        //self.Invoke();
    }
    
}
CourseSearch.prototype.SetAdminShowPastClosed = function () {
    var self = this;
    if (self.adminSearchShowClosedPast == true) {
        $(".pastcoursesshowed").attr("src", "/Images/icons/gsmu/Calendaroff.png");
        $('#course-search-left-category-container').load('Public/Category/LeftCategories?courseInternal=' + self.adminSearchShowInternal);
        self.adminSearchShowClosedPast = false;
        self.Invoke();
    }
    else {
        self.adminSearchShowClosedPast = true;
        $(".pastcoursesshowed").attr("src", "/Images/icons/gsmu/Calendaron.png");
        $('#course-search-left-category-container').load('Public/Category/LeftCategories?ShowPastCourses=true&courseInternal=' + self.adminSearchShowInternal);
        self.Invoke();
    }
    
}
// invokes the search
CourseSearch.prototype.Invoke = function (fade) {
    var self = this;
    self.IsLoading = true;

    if (typeof (fade) == 'undefined') {
        fade = true;
    }
    console.log("Date until")
    console.log(self.DateUntil == null)
    self.serializeData();

    self.CloseCourseDetailsWindow();
    self.ResultElement.mask('Loading');
    var url = config.getUrl('public/course/list' +
                '?page=' + self.Page +
                '&pageSize=' + self.PageSize +
                '&orderByField=' + encodeURIComponent(self.OrderByField) +
                '&orderByDirection=' + self.OrderByDirection +
                '&text=' + encodeURIComponent(self.Text) +
                '&mainCategory=' + encodeURIComponent(self.MainCategory) +
                '&subCategory=' + encodeURIComponent(self.SubCategory) +
                '&subsubcattext=' + encodeURIComponent(self.SubSubCategory) +
                '&subCategoryIsSubSub=' + self.SubCategoryIsSubSub +
                '&state=' + self.CourseActiveState +
                '&view=' + self.ViewListType +
                '&from=' + self.DateFrom +
                '&until=' + self.DateUntil +
                '&courseInternal=' + self.CourseInternal +
                '&CoursePopout=' + self.CoursePopout +
                '&cancelState=' + self.CancelState +
                '&showinternal=' + self.adminSearchShowInternal +
                '&showclosedpast=' + self.adminSearchShowClosedPast+
                '&showmembership=' + self.adminSearchShowMembership
               
            );

    Ext.Ajax.request({
        url: url,
        success: function (response) {
            if (Ext.isDefined(cart)) {
                cart.resetMaterialData();
            }
            self.ResultElement.unmask();
            if (fade) {
                self.ResultElement.hide();
            }
            self.ResultElement.setHtml(response.responseText, true);
            if (fade) {
                self.ResultElement.fadeIn();
            }
            self.IsLoading = false;
            Ext.select('.gsmu-tooltip-removable').remove();
            self.ShowCourseDetails(self.CoursePopout);
        }
    });

}

CourseSearch.prototype.ResetSearch = function () {
    var self = this;
    self.MainCategory = '';
    self.SubCategory = '';
    self.SubCategoryIsSubSub = false;
    self.Page = 1;
    self.Text = '';
    self.DateFrom = null;
    self.DateUntil = null;
    self.CoursePopout = 0;
}

CourseSearch.prototype.ResetPage = function () {
    var self = this;
    self.Page = 1;
}

CourseSearch.prototype.Paginate = function (page, pageSize) {
    var self = this;

    self.Page = page;
    if (typeof (pageSize) != 'undefined') {
        self.PageSize = pageSize;
    }
    self.Invoke();
}

CourseSearch.prototype.Sort = function (field, direction) {
    var self = this;

    if (Ext.isDefined(direction)) {
        if (direction != 'Ascending' || direction != 'Descending') {
            direction = self.OrderByDirection;
        }
    }

    if (typeof (direction) != 'undefined') {
        self.OrderByDirection = direction;
    } else if (self.OrderByField == field) {
        self.OrderByDirection = self.OrderByDirection == 'Ascending' ? 'Descending' : 'Ascending';
    }
    self.OrderByField = field;
    self.Invoke();

}

CourseSearch.prototype.ShowMapPanel = function (mapid) {
    var self = this;

            //_location= $("#cslocation" + mapid).val(),
            //street= $("#csstreet" + mapid).val(),
            //city= $("#cscity" + mapid).val(),
            //state= $("#csstate" + mapid).val(),
            //zip= $("#cszip" + mapid).val(),
            //country = $("#cscountry" + mapid).val()

            //document.getElementById(mapid).innerHTML = '<iframe frameborder="0" style="border:0" src="https://www.google.com/maps/embed/v1/place?q=+' + _location + ',+' + street + ',+' + city + ',+' + state + ',+,+' + zip + '&amp;key=AIzaSyCyVJTUsuif-nIOC7AMZLpo623hhsE9adw" allowfullscreen=""></iframe>'
       
        loc = $("#cslocation" + mapid).val(),
        street = $("#csstreet" + mapid).val(),
        city = $("#cscity" + mapid).val(),
        state = $("#csstate" + mapid).val(),
        zip = $("#cszip" + mapid).val(),
        country = $("#cscountry" + mapid).val()

    if (loc.search(/online/) > -1 || loc.search(/virtual/) > -1) {
        //console.log("world")
        document.getElementById(mapid).innerHTML = '<iframe frameborder="0" style="border:0;width:100%;height:100%;" src="https://www.google.com/maps/embed/v1/view?zoom=0&center=37.0902%2C-95.7129&amp;key=AIzaSyC0xAyystvOBYRuOrX8N_lRCOEkMqLEJkw" allowfullscreen=""></iframe>'

    } else {
        //console.log("specific")
        document.getElementById(mapid).innerHTML = '<iframe frameborder="0" style="border:0;width:100%;height:100%;" src="https://www.google.com/maps/embed/v1/search?q=+' + street + ',+' + city + ',+' + state + ',+' + country + ',+' + zip + '&amp;key=AIzaSyC0xAyystvOBYRuOrX8N_lRCOEkMqLEJkw" allowfullscreen=""></iframe>'
    }

}

CourseSearch.prototype.ShowMembershipDetails = function () {
    var self = this;

    document.title = "Membership";
    $(".page-title").html("Membership");

    //var cntrpanel = Ext.create('Ext.form.Panel', {
    //    border: false,
    //    frame: false,
    //    bodyPadding: 0,
    //    html: '',
    //    anchor: '100% 100%'
    //});

    //var window = Ext.getCmp('MembershipDetailsWindow');
    //if (window != null) {
    //    window.destroy();
    //}

    if (history.pushState) {
        var query = UrlHelper.getUrlVars();
        history.pushState(null, null, config.getUrl('membership'));
    }

    //window = Ext.create('Ext.window.Window', {
    //    id: 'MembershipDetailsWindow',
    //    modal: true,
    //    border: false,
    //    frame: false,
    //    header: false,
    //    layout: 'anchor',
    //    y: 10,
    //    tbar: false,
    //    title: 'Course details',
    //    width: 100,
    //    //should always autoScroll=false for the swipe function in mobiles devices
    //    autoScroll: false,
    //    listeners: {
    //        close: function () {
    //            self.CoursePopout = 0;
    //            if (history.pushState) {
    //                var query = UrlHelper.getUrlVars();
    //                history.pushState(null, null, config.getUrl('public/course/browse'));
    //            }
    //            if (modifier != 'calendar') {
    //                self.serializeData();
    //            }
    //        }
    //    },
    //    items: cntrpanel
    //});
    //window.show();
    //cntrpanel.setLoading(true);


    Ext.Ajax.request({
        url: config.getUrl('public/Course/MembershipDetails'),
        //params: {
        //    intMembershipID: intCourseId
        //},
        success: function (response) {

            //cntrpanel.setLoading(false);
            //cntrpanel.hide()
            //window.setPosition(25, 10);
            //window.setWidth(document.documentElement.clientWidth - 50);
            //self.docScrollTop = $(document).scrollTop();
            //$("html, body").animate({ scrollTop: 0 }, "slow");
            //window.update(response.responseText, true, function () {

            cart.elementCheckoutContainer.setHtml(response.responseText, true);
            cart.ShowCheckoutContainerDisplay();

            //});
        }
    });
}

CourseSearch.prototype.CloseMembershipDetailsWindow = function () {
    var self = this;

    if (history.pushState) {
        var query = UrlHelper.getUrlVars();
        history.pushState(null, null, config.getUrl('public/course/browse'));
    }

}


CourseSearch.prototype.ShowCourseDetails = function (intCourseId, modifier, showNotification, cmd, subcmd) {
    var self = this;

    if (typeof (showNotification) == 'undefined') {
        showNotification = false;
    }

    if (isNaN(intCourseId) || intCourseId == 0) { return; }

    self.CoursePopout = intCourseId;
    if ((modifier != 'calendar') && (modifier != 'instructor') && (modifier != 'childevent')) {
        self.serializeData();
    }

    var heardertreebrowse = function () {
        var hdrtxt = "&nbsp;>&nbsp;<a href='javascript:window.CourseSearchInstance.CloseCourseDetailsWindow()'>" + self.MainCategory + "</a>&nbsp;>&nbsp;"
        hdrtxt += "<a href='javascript:window.CourseSearchInstance.CloseCourseDetailsWindow()'>" + self.SubCategory + "</a>&nbsp;>&nbsp;"
        if (self.MainCategory.length < 1) {
            return "&nbsp;>&nbsp;<a href='javascript:window.CourseSearchInstance.CloseCourseDetailsWindow()'>All</a>&nbsp;>&nbsp;";
        }
        return hdrtxt;
    }

    var cntrpanel = Ext.create('Ext.form.Panel', {
        border: false,
        frame: false,
        bodyPadding: 0,
        html: '',
        anchor: '100% 100%'
    });

    var window = Ext.getCmp('CourseDetailsWindow');
    if (window != null) {
        window.destroy();
    }

    if (history.pushState) {
        var query = UrlHelper.getUrlVars();
        history.pushState(null, null, config.getUrl('public/Course/browse?courseid=' + intCourseId));
    }

    window = Ext.create('Ext.window.Window', {
        id: 'CourseDetailsWindow',
        modal: true,
        border: false,
        frame: false,
        header: false,
        layout: 'anchor',
        y: 10,
        tbar: false,
        title: 'Course details',
        width: 100,
        //should always autoScroll=false for the swipe function in mobiles devices
        autoScroll: false,
        listeners: {
            close: function () {
                self.CoursePopout = 0;
                if ((modifier != 'calendar') && (modifier != 'instructor')) {
                    self.serializeData();
                }
            }
        },
        items: cntrpanel
    });
    window.show();
    cntrpanel.setLoading(true);


    Ext.Ajax.request({
        url: config.getUrl('public/Course/CourseDetails'),
        params: {
            intCourseId: intCourseId
        },
        success: function (response) {

            cntrpanel.setLoading(false);
            cntrpanel.hide()
            window.setPosition(25, 10);
            window.setWidth(document.documentElement.clientWidth - 50);
            self.docScrollTop = $(document).scrollTop();
            $("html, body").animate({ scrollTop: 0 }, "slow");
            window.update(response.responseText, true, function () {

                // //Prerequisite
                    $('#bootstrap-header-course').hide();
                    $('#none-bootstrap-header-course').show();
                    $('#bootstrap-header-prereq').hide();
                    $('#none-bootstrap-header-prereq').show();

                //$("#s6").accordion();
                $("#coursetreebrowse").html(heardertreebrowse);

			//auto adjust height to longest widget
			setTimeout(function() {
				//var maxHeight = 0;				
				var maxHeight = $("#CourseDetailsWindow-innerCt").height();				
				$(".course-widgetbox").each(function(){
				   var thisH = $(this).height();
				   var thisT = $(this).position().top;
				   var thisTH = thisH + thisT;
				   if (thisTH > maxHeight) { maxHeight = thisTH; }
				});
				maxHeight = maxHeight + 20;
				console.log(maxHeight)
				window.setHeight(maxHeight);
			}, 1000);				
				
                var map;
                var address = new Address();
                var data = {
                    street: $("#hdstreet").val().trim(),
                    city: $("#hdcity").val().trim(),
                    state: $("#hdstate").val().trim(),
                    zip: $("#hdzip").val().trim(),
                    country: $("#hdcountry").val().trim()
                };

                address.verifyAddress(data, function (valid, results) {
                    if ($("#hdcity").val().trim().length < 2 && $("#hdstate").val().trim().length < 2) {
                        valid = false
                    }
                    if (valid) {
                        $('#map-canvas').show();
                        resultx = results[0].geometry.location;
                        result = JSON.stringify(resultx)
                        result = result.replace("(", "")
                        result = result.replace(")", "")
                        var mapOptions = {
                            zoom: 10,
                            center: new google.maps.LatLng(resultx.lat(), resultx.lng()),
                            mapTypeId: google.maps.MapTypeId.ROADMAP
                        };

                        map = new google.maps.Map(document.getElementById('map-canvas'),
                        mapOptions);

                        var marker = new google.maps.Marker({
                            map: map,
                            position: results[0].geometry.location,
                            title: $("#hdlocation").val()
                        });


                    } else {
                        $('#map-canvas').hide();
                        result = "Unable to find address: " + status;
                    }


                    if (showNotification) {
                        top.window.LAYOUT.notify('There are options available for the course that you are required to make before you can add it to your cart. Please make your selection on the right and proceed by clicking the green button.');
                    }

                });
            });
        }
    });
}

CourseSearch.prototype.CloseCourseDetailsWindow = function () {
    var self = this;

    var window = Ext.getCmp('CourseDetailsWindow');
    if (window) {
        window.close();
        $("html, body").animate({ scrollTop: self.docScrollTop }, "slow");
    }
    else {
        var calendarwindow = Ext.getCmp('CalendarCourseDetail');
        if (calendarwindow) {
            calendarwindow.close();
            $("html, body").animate({ scrollTop: self.docScrollTop }, "slow");
        }
    }

}
CourseSearch.prototype.ViewCourseDetailsInEvent = function (courseid) {
    Ext.Ajax.request({
        url: config.getUrl('public/Course/CourseDetails'),
        params: {
            intCourseId: courseid
        },
        success: function (response) {
            window_course_details_event = Ext.create('Ext.window.Window', {
                id: 'CourseDetailsWindowInEvent',
                modal: true,
                border: false,
                frame: false,
                header: true,
                layout: 'anchor',
                width: '95%',
                html: response.responseText.replace('" responseXML="">', ''),
                y: 10,
                tbar: false,
                title: 'Course details',
                //should always autoScroll=false for the swipe function in mobiles devices
                autoScroll: false,
                listeners: {

                },
            });
            window_course_details_event.show();
        }
    });
}
CourseSearch.prototype.CheckRequisite = function () {
    if ($('#chkRequisite').is(':checked')) {
        $("#CourseRequisiteContainer").removeClass("course-widgetbox_error").addClass('course-widgetbox');
    }
    else {
        $("#CourseRequisiteContainer").removeClass("course-widgetbox").addClass('course-widgetbox_error');
    }
}

CourseSearch.prototype.GetSpecificUrl = function () {

    Ext.MessageBox.show({
        title: 'Copy the URL below:',
        msg: location.href,
        buttons: Ext.MessageBox.INFO
    });
}

CourseSearch.prototype.ShowCourseShareLinks = function (intCourseId, cmd) {
    var self = this;

    var window = Ext.getCmp('CourseDetailsWindow');
    if ($("#divCourseLinkAction").css("display") == "block") {

        $('#divCourseLinkAction').slideUp('fast', function () {
            $("#divCourseLinkAction").css("display", "none");
            if (window) {
                var curHT = window.getHeight();
                window.setHeight(curHT - 250);
            }
        });
        if ($("#CourseLinkActioncmd").val() == cmd) {
            return;
        }
    }

    if (window) {
        var curHT = window.getHeight();
        window.setHeight(curHT + 250);
    }

    Ext.Ajax.request({
        url: config.getUrl('public/Course/CourseLinkAction'),
        params: {
            intCourseId: intCourseId,
            cmd: cmd,
            urllnk: location.href
        },
        success: function (response) {
            $("#divCourseLinkAction").html(response.responseText);
            $('#divCourseLinkAction').slideDown('fast', function () {
                if (cmd == "lk") {
                    $("#lnkinputbox").focus(function () { $(this).select(); });
                    $("#lnkinputbox").focus();
                }
                try {
                    FB.XFBML.parse();
                } catch (ex) { }

                if (cmd == "ml") {
                    grecaptcha.ready(function () {
                        grecaptcha.execute('6LcPeLEUAAAAAMoYQ3qMUz2MMdUswmt-MDUYICjK',
                            {
                                action: grecaptcha.focus_response_field

                            }).then(function () {
                            });
                    });
                    //Recaptcha.create("6Lekt-MSAAAAAKR9JpvuXaae8IBNm4zeP0XOf1Sf", 'captchadiv', {
                    //    tabindex: 1,
                    //    theme: "white",
                    //    callback: Recaptcha.focus_response_field
                    //});

                }


            });
        }
    });
}
