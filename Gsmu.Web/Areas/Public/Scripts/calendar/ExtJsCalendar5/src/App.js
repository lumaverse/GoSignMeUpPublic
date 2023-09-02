/*
 * This calendar application was forked from Ext Calendar Pro
 * and contributed to Ext JS as an advanced example of what can 
 * be built using and customizing Ext components and templates.
 * 
 * If you find this example to be useful you should take a look at
 * the original project, which has more features, more examples and
 * is maintained on a regular basis:
 * 
 *  http://ext.ensible.com/products/calendar
 */
Ext.define('Ext.calendar.App', {
    
    requires: [
        'Ext.Viewport',
        'Ext.layout.container.Border',
        'Ext.picker.Date',
        'Ext.calendar.util.Date',
        'Ext.calendar.CalendarPanel',
        'Ext.calendar.data.MemoryCalendarStore',
        'Ext.calendar.data.MemoryEventStore',
        'Ext.calendar.data.Events',
        'Ext.calendar.data.Calendars',
        'Ext.calendar.form.EventWindow'
    ],

    constructor: function () {
        var me = this;

        // Minor workaround for OSX Lion scrollbars
        this.checkScrollOffset();

        // This is an example calendar store that enables event color-coding
        this.calendarStore = Ext.create('Ext.calendar.data.MemoryCalendarStore', {
            data: Ext.calendar.data.Calendars.getData()
        });

        var querycourses = function (callback) {

            window.LAYOUT.MaskLayout('Loading event data...');

            var element = document.getElementById('list');
            var subelement = document.getElementById('sublist');
            var mainc = getURLParameter('main');
            var subcat = getURLParameter('sub');
            if (mainc != null && mainc != '') {
                element.value = mainc;
            }
            if (subcat != null && subcat != '') {
                subelement.value = subcat;
            }
            var result = [];
            $.ajax({
                url: config.getUrl('public/calendar/getcalendarcourses?maincategory='+mainc+'&subcategory='+subcat),
                dataType: 'json',
                success: function (response) {
                    for (x = 0; x < response.length; x++) {
                        var jobj = {
                            id:response[x].id,
                            cid:response[x].cid,
                            title: response[x].title,
                            online: response[x].OnlineCourse - response[x].ctype,
                            onlinedatestartend: response[x].OnlineDateStartEnd,
                            StartEndTimeDisplay: response[x].StartEndTimeDisplay,
                            //start: makeDate(response[x].stringstartdate, 0, 0, 0),
                            start: new Date(response[x].stringstartdate),
                            //end: makeDate(response[x].startdate, 0, 0, 0)
                            //end: (response[x].OnlineCourse != undefined && response[x].OnlineCourse != null && response[x].OnlineCourse != 0 ? new Date(response[x].stringstartdate) : new Date(response[x].stringstartdate)),
                            end: new Date(response[x].stringenddate),
                            loc: response[x].loc
                        }
                        //console.log(response[x].stringenddate)
                        result.push(jobj);
                    }
                    var data = { "evts": result };
                    window.LAYOUT.UnmaskLayout();
                    callback.call(me, data);
                }
            });
        }
        
        var loadComplete = function (eventData) {

            // A sample event store that loads static JSON from a local file. Obviously a real
            // implementation would likely be loading remote data via an HttpProxy, but the
            // underlying store functionality is the same.
            this.eventStore = Ext.create('Ext.calendar.data.MemoryEventStore', {
                data: eventData
            });


            // This is the app UI layout code.  All of the calendar views are subcomponents of
            // CalendarPanel, but the app title bar and sidebar/navigation calendar are separate
            // pieces that are composed in app-specific layout code since they could be omitted
            // or placed elsewhere within the application.
            Ext.create('Ext.panel.Panel', {
                frame: false,
                border: 0,
                layout: 'border',
                height: 500,
                width: '100%',
                renderTo: 'gsmu-calendar-container',
                items: [{
                    id: 'app-center',
                    title: '...', // will be updated to the current view's date range
                    region: 'center',
                    layout: 'border',
                    listeners: {
                        'afterrender': function () {
                            Ext.getCmp('app-center').header.addCls('app-center-header');
                        }
                    },
                    items: [{
                        xtype: 'calendarpanel',
                        eventStore: this.eventStore,
                        calendarStore: this.calendarStore,
                        border: false,
                        id: 'app-calendar',
                        region: 'center',
                        activeItem: 3, // month view

                        monthViewCfg: {
                            showHeader: true,
                            showWeekLinks: true,
                            showWeekNumbers: true
                        },

                        listeners: {
                            'eventclick': {
                                fn: function (vw, rec, el) {
                                    var calendarcourseid = rec.data.CalendarId;
                                    if (rec.data.Title.indexOf("&nbsp;&nbsp;") >= 0) {
                                        ShowEventDetails(calendarcourseid);
                                    }
                                    else {
                                        CourseSearch.prototype.ShowCourseDetails(calendarcourseid, 'calendar');

                                    }
                                },
                                scope: this
                            },
                            'eventover': function (vw, rec, el, ev) {
                                //console.log('Entered evt rec='+rec.data.Title+', view='+ vw.id +', el='+el.id);
                                startdate = new Date(rec.data.StartDate).toLocaleDateString();
                                enddate = new Date(rec.data.EndDate).toLocaleDateString();
                                starttime = new Date(rec.data.StartDate).toLocaleTimeString();
                                endtime = new Date(rec.data.EndDate).toLocaleTimeString();
                                online = rec.data.online;
                                onlinedate = rec.data.onlinedatestartend;
                                var title = rec.data.Title.replace('(MST)', '').replace('((CST))', '').replace('(EST)', '').replace('(PST)', '')
                                var datetimedisplay = startdate;
                                if (online != undefined && online != null && online != 0) {
                                    datetimedisplay = onlinedate;
                                }
                                if (online == 1 && rec.data.StartEndTimeDisplay != null) { datetimedisplay = rec.data.StartEndTimeDisplay }
                                wintip = Ext.create('Ext.container.Container', {
                                    layout: 'fit',
                                    modal: false,
                                    floating: true,
                                    border: false,
                                    width: 250,
                                    height: 80,
                                    bodyStyle: 'background-color:white;border:1px solid blue;',
                                    items: [{
                                        bodyStyle: "background-color:white; color:black; padding-top:5px;padding-left:10px;",
                                        html: '<div style="bacground-color:red; color:blue;">' + '<div style="font-weight:bold;">' + title + "</div><div> " + datetimedisplay + '</div><div>' + rec.data.Location + '</div></div>',
                                        height: 1,
                                    }]

                                });
                                wintip.showAt(el.getLeft(), el.getTop() - 85);
                            },
                            'eventout': function (vw, rec, el) {
                                //console.log('Leaving evt rec='+rec.data.Title+', view='+ vw.id +', el='+el.id);
                                wintip.destroy();
                            },
                            'eventadd': {
                                fn: function (cp, rec) {
                                    this.showMsg('Event ' + rec.data.Title + ' was added');
                                },
                                scope: this
                            },
                            'eventupdate': {
                                fn: function (cp, rec) {
                                    this.showMsg('Event ' + rec.data.Title + ' was updated');
                                },
                                scope: this
                            },
                            'eventcancel': {
                                fn: function (cp, rec) {
                                    // edit canceled
                                },
                                scope: this
                            },
                            'viewchange': {
                                fn: function (p, vw, dateInfo) {
                                    if (this.editWin) {
                                        this.editWin.hide();
                                    }
                                    if (dateInfo) {
                                        // will be null when switching to the event edit form so ignore
                                        Ext.getCmp('app-nav-picker').setValue(dateInfo.activeDate);
                                        this.updateTitle(dateInfo.viewStart, dateInfo.viewEnd);
                                    }
                                },
                                scope: this
                            },
                            'dayclick': {
                                fn: function (vw, dt, ad, el) {
                                    return;
                                    /*
                                    this.showEditWindow({
                                        StartDate: dt,
                                        IsAllDay: ad
                                    }, el);
                                    this.clearMsg();
                                    */
                                },
                                scope: this
                            },
                            'rangeselect': {
                                fn: function (win, dates, onComplete) {
                                    return;
                                    /*
                                    this.showEditWindow(dates);
                                    this.editWin.on('hide', onComplete, this, { single: true });
                                    this.clearMsg();
                                    */
                                },
                                scope: this
                            },
                            'eventmove': {
                                fn: function (vw, rec) {
                                    return;
                                    /*
                                    var mappings = Ext.calendar.data.EventMappings,
                                        time = rec.data[mappings.IsAllDay.name] ? '' : ' \\a\\t g:i a';

                                    rec.commit();

                                    this.showMsg('Event ' + rec.data[mappings.Title.name] + ' was moved to ' +
                                        Ext.Date.format(rec.data[mappings.StartDate.name], ('F jS' + time)));
                                        */
                                },
                                scope: this
                            },
                            'eventresize': {
                                fn: function (vw, rec) {
                                    return;
                                    /*
                                    rec.commit();
                                    this.showMsg('Event ' + rec.data.Title + ' was updated');
                                    */
                                },
                                scope: this
                            },
                            'eventdelete': {
                                fn: function (win, rec) {
                                    return;
                                    /*
                                    this.eventStore.remove(rec);
                                    this.showMsg('Event ' + rec.data.Title + ' was deleted');
                                    */
                                },
                                scope: this
                            },
                            'initdrag': {
                                fn: function (vw) {
                                    if (this.editWin && this.editWin.isVisible()) {
                                        this.editWin.hide();
                                    }
                                },
                                scope: this
                            }
                        }
                    }]
                }]
            });
        };

        querycourses(loadComplete);
    },
        
    // The edit popup window is not part of the CalendarPanel itself -- it is a separate component.
    // This makes it very easy to swap it out with a different type of window or custom view, or omit
    // it altogether. Because of this, it's up to the application code to tie the pieces together.
    // Note that this function is called from various event handlers in the CalendarPanel above.
    showEditWindow : function(rec, animateTarget){
        if(!this.editWin){
            this.editWin = Ext.create('Ext.calendar.form.EventWindow', {
                calendarStore: this.calendarStore,
                listeners: {
                    'eventadd': {
                        fn: function(win, rec){
                            win.hide();
                            rec.data.IsNew = false;
                            this.eventStore.add(rec);
                            this.eventStore.sync();
                            this.showMsg('Event '+ rec.data.Title +' was added');
                        },
                        scope: this
                    },
                    'eventupdate': {
                        fn: function(win, rec){
                            win.hide();
                            rec.commit();
                            this.eventStore.sync();
                            this.showMsg('Event '+ rec.data.Title +' was updated');
                        },
                        scope: this
                    },
                    'eventdelete': {
                        fn: function(win, rec){
                            this.eventStore.remove(rec);
                            this.eventStore.sync();
                            win.hide();
                            this.showMsg('Event '+ rec.data.Title +' was deleted');
                        },
                        scope: this
                    },
                    'editdetails': {
                        fn: function(win, rec){
                            win.hide();
                            Ext.getCmp('app-calendar').showEditForm(rec);
                        }
                    }
                }
            });
        }
        this.editWin.show(rec, animateTarget);
    },
        
    // The CalendarPanel itself supports the standard Panel title config, but that title
    // only spans the calendar views.  For a title that spans the entire width of the app
    // we added a title to the layout's outer center region that is app-specific. This code
    // updates that outer title based on the currently-selected view range anytime the view changes.
    updateTitle: function(startDt, endDt){
        var p = Ext.getCmp('app-center'),
            fmt = Ext.Date.format;
        
        if(Ext.Date.clearTime(startDt).getTime() === Ext.Date.clearTime(endDt).getTime()){
            p.setTitle(fmt(startDt, 'F j, Y'));
        }
        else if(startDt.getFullYear() === endDt.getFullYear()){
            if(startDt.getMonth() === endDt.getMonth()){
                p.setTitle(fmt(startDt, 'F j') + ' - ' + fmt(endDt, 'j, Y'));
            }
            else{
                p.setTitle(fmt(startDt, 'F j') + ' - ' + fmt(endDt, 'F j, Y'));
            }
        }
        else{
            p.setTitle(fmt(startDt, 'F j, Y') + ' - ' + fmt(endDt, 'F j, Y'));
        }
    },
    
    // This is an application-specific way to communicate CalendarPanel event messages back to the user.
    // This could be replaced with a function to do "toast" style messages, growl messages, etc. This will
    // vary based on application requirements, which is why it's not baked into the CalendarPanel.
    showMsg: function(msg){
        Ext.fly('app-msg').update(msg).removeCls('x-hidden');
    },
    clearMsg: function(){
        Ext.fly('app-msg').update('').addCls('x-hidden');
    },
    
    // OSX Lion introduced dynamic scrollbars that do not take up space in the
    // body. Since certain aspects of the layout are calculated and rely on
    // scrollbar width, we add a special class if needed so that we can apply
    // static style rules rather than recalculate sizes on each resize.
    checkScrollOffset: function() {
        var scrollbarWidth = Ext.getScrollbarSize ? Ext.getScrollbarSize().width : Ext.getScrollBarWidth();
        
        // We check for less than 3 because the Ext scrollbar measurement gets
        // slightly padded (not sure the reason), so it's never returned as 0.
        if (scrollbarWidth < 3) {
            Ext.getBody().addCls('x-no-scrollbar');
        }
        if (Ext.isWindows) {
            Ext.getBody().addCls('x-win');
        }
    }
},
function() {
    /*
     * A few Ext overrides needed to work around issues in the calendar
     */
    
    Ext.form.Basic.override({
        reset: function() {
            var me = this;
            // This causes field events to be ignored. This is a problem for the
            // DateTimeField since it relies on handling the all-day checkbox state
            // changes to refresh its layout. In general, this batching is really not
            // needed -- it was an artifact of pre-4.0 performance issues and can be removed.
            //me.batchLayouts(function() {
                me.getFields().each(function(f) {
                    f.reset();
                });
            //});
            return me;
        }
    });
    
    // Currently MemoryProxy really only functions for read-only data. Since we want
    // to simulate CRUD transactions we have to at the very least allow them to be
    // marked as completed and successful, otherwise they will never filter back to the
    // UI components correctly.
    Ext.data.MemoryProxy.override({
        updateOperation: function(operation, callback, scope) {
            operation.setCompleted();
            operation.setSuccessful();
            Ext.callback(callback, scope || this, [operation]);
        },
        create: function() {
            this.updateOperation.apply(this, arguments);
        },
        update: function() {
            this.updateOperation.apply(this, arguments);
        },
        destroy: function() {
            this.updateOperation.apply(this, arguments);
        }
    });
});

function openDescSlider() {
    var open_height = $(".slider").attr("box_h") + "px";
    $(".slider").animate({ "height": open_height }, { duration: "slow" });

    $(".slider_menu").html(slideupbtn);
    $(".slider_menu a").click(function () { closeDesSlider() })
    $(".slider_menu a").attr("title", "click to hide");
}

function closeDesSlider() {
    $(".slider").animate({ "height": sliderHeight }, { duration: "slow" });
    //expand
    $(".slider_menu").html(slidedownbtn);
    $(".slider_menu a").click(function () { openDescSlider() })
    $(".slider_menu a").attr("title", "click to expand");
}

var sliderHeightval = 98;
var sliderHeight = sliderHeightval + "px";
var curht = 0;
var slidedownbtn = "<a><img src='../../Areas/Public/Images/Layout/slidedown.png' /></a>";
var slideupbtn = "<a><img src='../../Areas/Public/Images/Layout/slideup.png' /></a>";
function ShowEventDetails(eventid) {
    $.ajax({
        url: config.getUrl('public/calendar/courseeventdetails?intEventId='+eventid),
        dataType: 'json',
        async: false,
        success: function (response) {
            if (response.timevalue == null) {
                timevalue = "";
            }
            else {
                timevalue = response.timevalue;
            }

            if ((response.locationvalue != null) && (response.locationvalue != "")) {
                addressinfo = '<br /><div class="course-widgetbox" style="min-height:100px; padding:5px;">'
                addressinfo = addressinfo + '<div class="course-widgetbox-smheader">Location Info </div>';



                ShowMap(response.locationvalue);
                addressinfo = addressinfo + '<div id="map-canvas" style="width: 100%; height: 120px;"></div>';
                addressinfo = addressinfo + '<br />' + response.locationvalue;

                addressinfo = addressinfo + ' </div>';
                winh = 620;
            }
            else {
                addressinfo = "";
                winh = 400;
            }

            hideinfo = 0;
            additionalinfo = '<br /><div class="course-widgetbox" style="min-height:100px; padding:5px;">';
            additionalinfo = additionalinfo + '<div class="course-widgetbox-smheader">Additional Info </div>';
            if ((response.speakervalue != null) &&(response.speakervalue != "")) {
                additionalinfo = additionalinfo + '<b>Featured Speaker: </b>' + response.speakervalue;
                hideinfo = hideinfo+1;
            }
            if ((response.contactvalue != null) && (response.contactvalue != "")) {
                additionalinfo = additionalinfo + '<br /><b>Contact Info: </b>' + response.contactvalue +'&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;';
                hideinfo =hideinfo+ 1;
            }
            if ((response.emailvalue != null) && (response.emailvalue != "")) {
                additionalinfo = additionalinfo + '<b>Contact Email: </b>' + response.emailvalue;
                hideinfo = hideinfo + 1;
            }
            if ((response.websitevalue != null) && (response.websitevalue != "")) {
                additionalinfo = additionalinfo + '<br /><b>Website: </b><a href="' + response.websitevalue+'" />'+response.websitevalue+'</a>';
                hideinfo = hideinfo + 1;
            }
            if ((response.feevalue != null) && (response.feevalue != "") && (response.feevalue != "0")) {
                additionalinfo = additionalinfo + '<br /><b>Fees: </b>' + response.feevalue;
                hideinfo = hideinfo + 1;
            }
            additionalinfo = additionalinfo + "</div>";

            if (hideinfo < 1) {
                additionalinfo = "";
                winh = winh - 100;
            }


            slidemenu ='    <div style="height: 20px" class="contslider_menu"><div style="float: right; width: 20px; text-align: center; margin-right: 10px; cursor: pointer" class="slider_menu"><a>&#8744;</a> </div></div>'

            win = Ext.create('widget.window', {
                title: response.title,
                modal:true,
                html: '<div style="background-color:#E0E0E0; padding:10px;"><div class="course-widgetbox slider" style="overflow:hidden;"><div class="course-widgetbox-smheader">' + response.title + "</div><div style='margin:5px;' >" + response.description + '</div></div>' + slidemenu + ' <br /> <div class="course-widgetbox"><div class="course-widgetbox-smheader">Date and Times </div><div style="margin:5px;"><b> Start Date: </b>' + response.startdatevalue + ' &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;<b>End Date: </b>' + response.enddatevalue + '<br /> <b>Event Time Note: </b>' + timevalue + '</div></div> ' + addressinfo + additionalinfo + '</div>',


                header: {
                    //  titlePosition: 2,
                    titleAlign: 'left'
                },
                closable: true,
                // closeAction: 'hide',
                width: 600,
                minWidth: 350,
                height: winh,
                layout: {
                    type: 'border',
                    padding: 5
                },
            });

            if (win.isVisible()) {
                win.hide(this, function () {
                });
            } else {
                win.show(this, function () {
                    $('.slider').each(function () {
                        var current = $(this);
                        current.attr("box_h", current.height() + 10);
                        curht = current.height();

                    });
                    //$(".slider").css("height", sliderHeight);
                    $(".slider").css("max-height", sliderHeight);
                    if (curht <= sliderHeightval) {
                        $(".contslider_menu").css("display", "none");
                    } else {
                        win.setHeight(winh + (curht/2));
                        $('.slider').css('max-height', '');
                        $(".slider").css("height", sliderHeight);
                        $(".slider_menu").html(slidedownbtn);
                        $(".slider_menu a").attr("title", "click to expand");
                        $(".slider_menu a").click(function () { openDescSlider() })

                    }
                });
            }
        }
    })
}

function ShowMap(location) {
    var map;
    var address = new Address();
    var data = {
        street: location,
        city: location,
        state: location,
        zip: location,
        country: location
    };
    var geocoder = new google.maps.Geocoder();
    geocoder.geocode({ 'address': location }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            var myOptions = {
                zoom: 13,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            }
            var map = new google.maps.Map(document.getElementById('map-canvas'), myOptions);
            map.setCenter(results[0].geometry.location);
            $('#map-canvas').show();
        }
    });
}