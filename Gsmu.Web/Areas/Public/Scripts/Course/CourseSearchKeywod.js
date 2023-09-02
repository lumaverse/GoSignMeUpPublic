// <input type="text" id="course-search-keyword" value="" data-form-label="Course keyword">
//<img id="course-search-keyword-clear" title="Clear keyword filter" src="@Url.Content("~/Areas/Public/Images/Layout/btn_close_off.jpg")" />

$(function () {
    $("#searchinput").keyup(function () {
        var invoke = true;
        if (invoke) {
            if ($(this).val() != window.CourseSearchInstance.Text) {
                window.COURSESEARCHLEFTCATEGORY.Reset(false);
            }
            window.CourseSearchInstance.ResetPage();
            window.CourseSearchInstance.Text = $(this).val();
            window.CourseSearchInstance.Invoke();

        }

    })
    ;

    $('[data-toggle="tooltip"]').tooltip();

});

function CourseSearchKeyword(courseSearch, keywordContainerId, options) {
    var self = this;

    Ext.merge(self.Options, options);

    self.ClearElementImage = 'Images/Icons/glyph2/Icons16x16/Delete.png';
    self.CourseSearch = courseSearch;
    self.KeywordContainerId = keywordContainerId;

    self.KeywordContainer = Ext.get(keywordContainerId);

    //var keywordEmptyText = 'Course name, number, id';
    var keywordEmptyText = window.LAYOUT.Options.buttonLabels.Search;

    var searchImage = config.getUrl('Images/Icons/glyph2/Icons24x24/Search 1.png');
    var clearImage = config.getUrl('Images/Icons/glyph2/Icons24x24/Delete.png');

    var width = 400;
    var backgroundWidth = width;

    self.Keyword = Ext.create('Ext.form.field.Text', {
        id: 'SearchCourseText',
        title: 'Enter keyword here to search',
        fieldLabel: 'Search must match the text of the course exactly as it appears. Text must be in same order with no additional words. It is recommended to try words in different orders.',
        hideLabel: true,
        renderTo: self.KeywordContainer,
        emptyText: keywordEmptyText,
        width: width,
        /*        grow: self.Options.grow,
                growMin: 200,
                growMax: 620, */
        style: {
            float: 'left'
        },
        fieldStyle: {
            lineHeight: '26px',
            fontSize: '18px',
            paddingRight: '26px'
        },
        value: self.CourseSearch.Text,
        listeners: {
            blur: function (p) {
                //setTimeout(function () {
                //if (typeof (Ext.getCmp('SearchCourseTextToolTip')) != 'undefined') {
                //    Ext.getCmp('SearchCourseTextToolTip').destroy();
                //    Ext.util.Cookies.set('closeSearchtooltip', 'closedbyclick')
                //}
                //}, 500);

            },
            focus: function (p) {
                var closeSearchtooltip = Ext.util.Cookies.get('closeSearchtooltip');
                if (closeSearchtooltip != 'closedbyx' && ToolTipSearchCourseText == 'enable') {
                    var theTip = Ext.create('Ext.tip.ToolTip', {
                        id: 'SearchCourseTextToolTip',
                        //title: '<div stlye="background-color: lightgray;"></div>',
                        html: '<div stlye="background-color: red;">Search must match the text of the course exactly as it appears. Text must be in same order with no additional words. It is recommended to try words in different orders.</div>',
                        margin: '0 0 0 0',
                        anchor: 'bottom',
                        trackMouse: false,
                        width: 800,
                        target: 'SearchCourseText',
                        cls: 'x-tipcustom',
                        bodyStyle: 'background-color: lightgray; color: black  !important;',
                        autoHide: true,
                        closable: false,
                        shadow: true,
                        listeners: {
                            //hide: function () {
                            //    if (typeof (theTip) != 'undefined') {
                            //        Ext.util.Cookies.set('closeSearchtooltip', 'closedbyx')
                            //        theTip.destroy();
                            //        theTip = null;
                            //    }
                            //}
                        }
                    });
                    setTimeout(function () {
                        theTip.show();
                    }, 1000);
                }

            }
        }
    });

    var dh = Ext.DomHelper;

    var clickPlaceholder = dh.append(self.KeywordContainerId, {
        tag: 'div',
        style: 'z-index: 10; cursor: pointer; width: 26px; height: 25px; position: absolute; left: ' + (backgroundWidth - 26) + 'px; background-size: 70%; background-position: 0px 4px; background-repeat: no-repeat; background-image: url(' + searchImage + ') ',
        id: 'course-search-keyword-click-placeholder'
    }, true);

    clickPlaceholder.on('click', function () {
        var value = Ext.String.trim(self.Keyword.getValue());
        if (value.length > 0) {
            self.Keyword.setValue(' ');
        } else {
            keywordOnChange(true);
        }
    });

    var keywordOnChange = function (invoke) {
        if (typeof (invoke) == 'undefined') {
            invoke = false;
        }
        var value = Ext.String.trim(self.Keyword.getValue());

        if (value.length > 0) {
            clickPlaceholder.setStyle({
                backgroundImage: "url('" + clearImage + "')",
            });
        } else {
            clickPlaceholder.setStyle({
                backgroundImage: "url('" + searchImage + "')",
            });
        }

        if (invoke) {
            if (value != self.CourseSearch.Text) {
                window.COURSESEARCHLEFTCATEGORY.Reset(false);
            }
            self.CourseSearch.ResetPage();
            self.CourseSearch.Text = value;
            self.CourseSearch.Invoke();

        }
    };

    keywordOnChange(false);

    self.Keyword.on('change', keywordOnChange, self, {
        buffer: 500
    });

}

CourseSearchKeyword.constructor = CourseSearchKeyword;

CourseSearchKeyword.ConstructForLayout = function (composer, options) {
    var containerId = composer.getLayoutContainerComponentContainerId(options.container);
    var object = new CourseSearchKeyword(composer.State.courseSearch, containerId);
    return object;
};


// the options of the component
CourseSearchKeyword.prototype.Options = {
};

// reference to the course search component
CourseSearchKeyword.prototype.CourseSearch = null;

// the input of the keyword
CourseSearchKeyword.prototype.KeywordContainerId = null;

// the container
CourseSearchKeyword.prototype.KeywordContainer = null;

// the container
CourseSearchKeyword.prototype.Keyword = null;

// the container
CourseSearchKeyword.prototype.KeywordClearElement = null;
