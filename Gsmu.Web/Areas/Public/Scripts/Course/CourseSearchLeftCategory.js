function CourseSearchLeftCategory(courseSearch, container, delayInit) {
    var self = this;

    window.cslc = self;
    window.COURSESEARCHLEFTCATEGORY = self;

    self.CourseSearch = courseSearch;

    self.MainContainer = container;

    if (Ext.isDefined(delayInit) && delayInit == false) {
        self.construct();
    }
}

CourseSearchLeftCategory.constructor = CourseSearchLeftCategory;

CourseSearchLeftCategory.ConstructForLayout = function (composer, options) {

    Ext.Ajax.request({
        url: config.getUrl('public/category/leftcategories'),
        params: {
            cancelState: self.CourseSearch.CancelState,
            courseInternal: self.CourseSearch.CourseInternal
        },
        success: function (response, ajaxOptions) {
            var container = options.container.down('.composite-layout-component');
            container.setHtml(response.responseText);
            object.Init();
        }
    });
    options.container.addCls('grad_stud_left');
    var object = new CourseSearchLeftCategory(composer.State.courseSearch, options.container, true);
    return object;
};

CourseSearchLeftCategory.prototype.construct = function () {

    var self = this;

    if (window.LAYOUT.Options.publicCourseListingFastLoad) {
        self.Init();
    } else {
        self.MainContainer.mask('Loading categories...');
        Ext.Ajax.request({
            url: config.getUrl('public/category/leftcategories'),
            params: {
                cancelState: self.CourseSearch.CancelState,
                courseInternal: self.CourseSearch.CourseInternal
            },
            success: function (response, ajaxOptions) {
                self.MainContainer.unmask();
                var container = self.MainContainer;
                container.setHtml(response.responseText);
                self.Init();
            }
        });
    }
};

CourseSearchLeftCategory.prototype.ClearImage = 'Images/Icons/famfamfam/cross.png';

// the main container
CourseSearchLeftCategory.prototype.MainContainer = null;

// the current tool tip for the category
CourseSearchLeftCategory.prototype.CurrentTip = null;

CourseSearchLeftCategory.prototype.useTip = false;

// the open main categories
CourseSearchLeftCategory.prototype.OpenMainCategories = {
};

// the index in the ui of the active sub category for highlighting and such
CourseSearchLeftCategory.prototype.ActiveCourseCategoryIndex = null;

CourseSearchLeftCategory.prototype.ActiveCourseSubSubCategoryIndex = null;


// if the active course category is sub sub
CourseSearchLeftCategory.prototype.ActiveCourseCategoryIsSubSub = null;

// reference to the course search component
CourseSearchLeftCategory.prototype.CourseSearch = null;

// the variable holding the timout counter for the tooltip
CourseSearchLeftCategory.prototype.TipHideTimeout = null;


// the variable holding scroll top
CourseSearchLeftCategory.prototype.ScrollTop = 0;




// shows the tooltip
CourseSearchLeftCategory.prototype.TipHide = function () {
    var self = this;

    if (self.CurrentTip != null) {
        self.TipHideTimeout = setTimeout(function () {
            self.CurrentTip.hide();
        }, 500);
    }
};


// hides the tooltip
CourseSearchLeftCategory.prototype.TipShow = function () {
    var self = this;

    clearTimeout(self.TipHideTimeout);
    if (self.CurrentTip != null) {
        self.CurrentTip.show();
    }
};


// initializes the category in case we are on a saved screen
CourseSearchLeftCategory.prototype.Init = function () {
    var self = this;

    if (self.useTip) {
        self.MainContainer.on('mouseover', function () {
            self.TipShow();
        })
        .on('mouseout', function () {
            self.TipHide();
        })
        .on('mousemove', function () {
            self.TipShow();
        });
    }

    self.OpenMaincategories = self.CourseSearch.Hash.getHashVar('OpenMainCategories', {});
    self.ActiveCourseCategoryIndex = self.CourseSearch.Hash.getHashVar('ActiveCourseCategoryIndex', null);
    self.ActiveCourseSubSubCategoryIndex = self.CourseSearch.Hash.getHashVar('ActiveCourseSubSubCategoryIndex', null);
    self.ActiveCourseCategoryIsSubSub = self.CourseSearch.Hash.getHashVar('ActiveCourseCategoryIsSubSub', false);
    for (var id in self.OpenMaincategories) {
        self.ShowCategory(id);
    }

    if (window.LAYOUT.Options.publicCourseListingFastLoad) {
        return;
    }


    if (self.CourseSearch.MainCategory != '' && self.CourseSearch.SubCategory != '' && self.ActiveCourseCategoryIndex != null) {
        self.FilterByCategory(self.CourseSearch.MainCategory, self.CourseSearch.SubCategory, self.ActiveCourseCategoryIndex, self.ActiveCourseCategoryIsSubSub ? self.ActiveCourseCategoryIndex : -1, true, true);
    }

}

CourseSearchLeftCategory.prototype.ShowCategory = function (id) {
    var self = this;

    var mainMenu = Ext.get('grad_stud_left_' + id);

    var subMenu = $('#grad_stud_left_menu_' + id);
    if (!mainMenu.hasCls('grad_stud_left_li_open')) {
        mainMenu.addCls('grad_stud_left_li_open');
        if (subMenu != null) {
            subMenu.slideDown();
            //var plus = $('#grad_stud_left_menu_' + id + ' .sub-plus');
            //var minus = $('#grad_stud_left_menu_' + id + ' .sub-minus');
            var subsub = $('#grad_stud_left_menu_' + id + ' .subsub');
            subsub.hide();
            //plus.show();
            //minus.hide();
            $('#main_sub_plusminus_' + id).html('&ndash;')
        }
        self.OpenMainCategories[id] = true;
    } else {
        mainMenu.removeCls('grad_stud_left_li_open');
        $('#main_sub_plusminus_' + id).html('+')

        if (subMenu != null) {
            subMenu.slideUp();
        }
        delete self.OpenMainCategories[id];
    }

    self.CourseSearch.Hash.setHashVar('OpenMainCategories', self.OpenMainCategories);
}

CourseSearchLeftCategory.prototype.ShowAll = function () {
    window.cslc.CatNavHide()
    window.cslc.Reset()

}


CourseSearchLeftCategory.prototype.CatNavHide = function () {
    var self = this;
    if ($(window).width() < 768) {
        self.ScrollTop = $(window).scrollTop();
        $('.sidebar').hide("slide", { direction: "left" }, 500);
        $('#btnCatShow').toggle("slide");
    }
}

CourseSearchLeftCategory.prototype.CatNavShow = function () {
    var self = this;
    $('#btnCatShow').toggle("slide");
    $('.sidebar').show("slide", { direction: "left" }, 500);
    $("html").scrollTop(self.ScrollTop);
}



CourseSearchLeftCategory.prototype.FilterByCategory = function (mainCategory, subCategory, subCategoryIndex, subsubIndex, noInvoke, startup,subsubcategory) {
    var self = this;
    if (typeof (startup) == 'undefined') {
        startup = false;
    }

    if (typeof (noInvoke) == 'undefined') {
        noInvoke = false;
    }
    
    if (typeof (subsubIndex) == 'undefined') {
        subsubIndex = -1;
    }

    var isSubSub = null;
    if (subsubIndex < 0) {
        isSubSub = false;
    } else {
        isSubSub = true;
    }
    self.CourseSearch.Hash.setHashVar('ActiveCourseCategoryIndex', subCategoryIndex);
    self.CourseSearch.Hash.setHashVar('ActiveCourseSubSubCategoryIndex', subsubIndex);
    self.CourseSearch.Hash.setHashVar('ActiveCourseCategoryIsSubSub', isSubSub);
    self.CourseSearch.Hash.saveState();
    self.ActiveCourseCategoryIndex = subCategoryIndex;
    self.ActiveCourseCategoryIsSubSub = isSubSub;
    $('.BGclogsubcatactive').removeClass('BGclogsubcatactive');
    $('.BGclogsub2catactive').removeClass('BGclogsub2catactive');
    $('.course-search-leftcategory-showall').removeClass('course-search-leftcategory-showall-active');

    var plus = $('#SubCatID' + subCategoryIndex + '-plus');
    var minus = $('#SubCatID' + subCategoryIndex + '-minus');
    var item = $('#SubCatID' + subCategoryIndex);
    if (!isSubSub) {
        item.addClass('BGclogsubcatactive');

        window.cslc.CatNavHide()

        if (item.hasClass('left-category-subcategory-parent')) {
            var subItems = $('[id^="grad_stud_left_menu_subsub_' + subCategoryIndex + '_"]');
            if (startup) {
                plus.show();
                minus.hide();
            }
            $.each(subItems, function (i) {
                //cannot assume i == _subsubIndex in case multiple sub categories have sub sub categories
                var subsub_i = $(this).data('subsubcategory-index');
                var subItem = $('[id="grad_stud_left_menu_subsub_' + subCategoryIndex + '_' + subsub_i + '"]');
                if (subItem.is(':visible')) {
                    if (!startup) {
                        subItem.slideUp();
                        minus.hide();
                        plus.show();
                    }
                } else {
                    if (!startup) {
                        subItem.slideDown();
                        plus.hide();
                        minus.show();
                    }
                }
            });
        }
    } else {
        minus.show();
        plus.hide();
        item.show();
        //gets the subsubIndex of the selected category and add the active class to that li only.
        var subItem = $('[id="grad_stud_left_menu_subsub_' + subCategoryIndex + '_' + subsubIndex + '"]');
        subItem.show();
        subItem.addClass('BGclogsub2catactive');
        window.cslc.CatNavHide()
    }
    if (self.CurrentCategoryElement == null) {

        if (self.useTip) {
            var html = '<span class="course-search-leftcategory-title">' + mainCategory + '/' + subCategory + '</span> <span class="course-search-leftcategory-close" onclick="window.cslc.Reset();"><img src="' + config.getUrl(self.ClearImage) + '" title="Clear category filter"></span>';

            if (self.CurrentTip != null) {
                self.CurrentTip.destroy();
            }
            self.CurrentTip = Ext.create('Ext.tip.ToolTip', {
                target: self.MainContainer,
                html: html,
                anchor: 'left',
                autoHide: false,
                autoShow: true,
                title: 'Selected category',
                style: {
                    'whiteSpace': 'nowrap'
                }
            });
            self.CurrentTip.hide();
            var elementTip = self.CurrentTip.getEl();
            elementTip.on('mouseover', function () {
                self.TipShow();
            });
            elementTip.on('mouseout', function () {
                self.TipHide();
            });
        }

    } else {
        self.CurrentCategoryElement.html(html);
    }
    self.CourseSearch.MainCategory = mainCategory;
    self.CourseSearch.SubCategory = subCategory;
    self.CourseSearch.SubCategoryIsSubSub = isSubSub;
    self.CourseSearch.SubSubCategory = subsubcategory;
    self.CourseSearch.Page = 1;
    if (!noInvoke) {
        self.Invoke();
    }
}

CourseSearchLeftCategory.prototype.Invoke = function () {
    var self = this;
    self.CourseSearch.Invoke();
}

CourseSearchLeftCategory.prototype.Reset = function (invoke) {
    var self = this;

    if (typeof (invoke) == 'undefined') {
        invoke = true;
    }

    $('.course-search-leftcategory-showall').addClass('course-search-leftcategory-showall-active');

    $('.BGclogsubcatactive').removeClass('BGclogsubcatactive');
    $('.BGclogsub2catactive').removeClass('BGclogsub2catactive');
    if (self.CurrentCategoryElement != null) {
        self.CurrentCategoryElement.html('');
    }
    if (self.CurrentTip != null) {
        self.CurrentTip.destroy();
        self.CurrentTip = null;
    }
    self.CourseSearch.MainCategory = '';
    self.CourseSearch.SubCategory = '';
    self.CourseSearch.SubCategoryIsSubSub = false;

    if (invoke) {
        self.Invoke();
    }
}