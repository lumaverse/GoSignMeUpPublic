var courseCategoryModel = {};
var courseCategories = {
    selectedMainCategory : null,
    selectedSubCategory : null,
    selectedSubSubCategory : null,
    ui: {
        mainCategory: $('#main-course-categories1'),
        subCategory: $('#subcategory-course-categories1'),
        subSubCategory: $('#subsubcategory-course-categories1'),
        mainCategory2: $('#main-course-categories2'),
        subCategory2: $('#subcategory-course-categories2'),
        subSubCategory2: $('#subsubcategory-course-categories2'),
        mainCategory3: $('#main-course-categories3'),
        subCategory3: $('#subcategory-course-categories3'),
        subSubCategory3: $('#subsubcategory-course-categories3'),
        saveBtn: $('.course-categories-btns .save')
    },
    init: function () {
        courseCategories.loadDropDownDataSource().then(function () {
                var courseCategoriesData = courseCategories.loadData();
                courseCategoriesData.done(function (response) {
                    if (response.Success === 1) {
                        courseCategories.initUI(response.Data);
                    }
                });
            });
        
    },
    loadDropDownDataSource: function () {
        return $.when(0)
            .then(function () {
                var mainCategoriesPromise = gsmuConfiguration.globalDropdown.getMainCategories();
                mainCategoriesPromise.done(function (data) {
                    if (data.Success === 1) {
                        data.Data.map(function (item) {
                            if (item.Value) $(courseCategories.ui.mainCategory).append('<option value="' + item.DisplayText + '" >' + item.DisplayText + '</option>');
                            if (item.Value) $(courseCategories.ui.mainCategory2).append('<option value="' + item.DisplayText + '" >' + item.DisplayText + '</option>');
                            if (item.Value) $(courseCategories.ui.mainCategory3).append('<option value="' + item.DisplayText + '" >' + item.DisplayText + '</option>');
                        });
                    }
                });
                return gsmuConfiguration.globalDropdown.getSubCategories();
            })
            .then(function (data) {
                if (data.Success === 1) {
                    data.Data.map(function (item) {
                        if (item.Value) $(courseCategories.ui.subCategory).append('<option value="' + item.DisplayText + '" >' + item.DisplayText + '</option>');
                        if (item.Value) $(courseCategories.ui.subCategory2).append('<option value="' + item.DisplayText + '" >' + item.DisplayText + '</option>');
                        if (item.Value) $(courseCategories.ui.subCategory3).append('<option value="' + item.DisplayText + '" >' + item.DisplayText + '</option>');
                    });
                    return gsmuConfiguration.globalDropdown.getSubSubCategories();
                }
            })
            .then(function (data) {
                if (data.Success === 1) {
                    data.Data.map(function (item) {
                        if (item.Value) $(courseCategories.ui.subSubCategory).append('<option value="' + item.DisplayText + '" >' + item.DisplayText + '</option>');
                        if (item.Value) $(courseCategories.ui.subSubCategory2).append('<option value="' + item.DisplayText + '" >' + item.DisplayText + '</option>');
                        if (item.Value) $(courseCategories.ui.subSubCategory3).append('<option value="' + item.DisplayText + '" >' + item.DisplayText + '</option>');
                    });
                }
                return $.when(0); // return the promise
            })

    },
    loadData: function () {
        var courseId = UrlHelper.getUrlVars()["courseId"];
        var getCourseCategories = function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: gsmuObject.apiUrl + '/AdminCourseDash/GetCourseCategoriesById?courseId=' + courseId
            });
        }
        return getCourseCategories();
    },
    initUI: function (data) {
        $('#category-container-2, #category-container-3').hide();
        courseCategoriesEditor.initView();

        if (!data || data.length == 0) return false;

        $(courseCategories.ui.mainCategory,
        courseCategories.ui.subCategory,
        courseCategories.ui.subSubCategory).unbind('change');

        $('select').on('change', function () {
            courseCategoriesEditor.initEditor();
        })

        $(courseCategories.ui.mainCategory).on('change', function (e) {
            var selected = $(e.currentTarget).val();
            courseCategoryModel.MainCategoryName = selected;
            courseCategoriesEditor.save();
        });

        $(courseCategories.ui.subCategory).on('change', function (e) {
            var selected = $(e.currentTarget).val();
            courseCategoryModel.SubCategoryName = selected;
            courseCategoriesEditor.save();
        });

        $(courseCategories.ui.subSubCategory).on('change', function (e) {
            var selected = $(e.currentTarget).val();
            courseCategoryModel.SubSubCategoryName = selected;
            courseCategoriesEditor.save();
        });

        $(courseCategories.ui.mainCategory2).on('change', function (e) {
            var selected = $(e.currentTarget).val();
            courseCategoryModel.MainCategory2 = selected;
            courseCategoriesEditor.save();
        });

        $(courseCategories.ui.subCategory2).on('change', function (e) {
            var selected = $(e.currentTarget).val();
            courseCategoryModel.SubCategory2 = selected;
            courseCategoriesEditor.save();
        });

        $(courseCategories.ui.subSubCategory2).on('change', function (e) {
            var selected = $(e.currentTarget).val();
            courseCategoryModel.SubSubCategory2 = selected;
            courseCategoriesEditor.save();
        });

        $(courseCategories.ui.mainCategory3).on('change', function (e) {
            var selected = $(e.currentTarget).val();
            courseCategoryModel.MainCategory3 = selected;
            courseCategoriesEditor.save();
        });

        $(courseCategories.ui.subCategory3).on('change', function (e) {
            var selected = $(e.currentTarget).val();
            courseCategoryModel.SubCategory3 = selected;
            courseCategoriesEditor.save();
        });

        $(courseCategories.ui.subSubCategory3).on('change', function (e) {
            var selected = $(e.currentTarget).val();
            courseCategoryModel.SubSubCategory3 = selected;
            courseCategoriesEditor.save();
        });

        $('input[name=categoriesRadio]').on('change', function () {
            courseCategories.categoryDropdownsVisibilitySetter();
        });

        if (globalObject.Categories.NoOfCategories > 1) {
            $('input[name=categoriesRadio][value=' + globalObject.Categories.NoOfCategories + ']')
                .prop('checked', true)
                .trigger('change')
        }

        data.map(function (item, index) {
            $('#main-course-categories' + (index + 1)).val(item.MainCategoryName);
            $('#subcategory-course-categories' + (index + 1)).val(item.SubCategoryName);
            $('#subsubcategory-course-categories' + (index + 1)).val(item.subSubCategory);
        })

    },
    categoryDropdownsVisibilitySetter: function () {
        var selected = $('input[name=categoriesRadio]:checked').val();
        if (selected == 2) {
            $('#category-container-2').show();
            $('#category-container-3').hide();
        }
        else if (selected == 3) {
            $('#category-container-2').show();
            $('#category-container-3').show();
        }
        else {
            $('#category-container-2, #category-container-3').hide();
        }
    }
}

var courseCategoriesEditor = {
    initView: function () {
        $(courseCategories.ui.saveBtn).hide('fast');
    },
    initEditor: function () {
        //$(courseCategories.ui.saveBtn).show('fast');
    },
    saveRequest: function () {
        return $.ajax({
            type: "POST",
            dataType: 'json',
            data: courseCategoryModel,
            url: gsmuObject.apiUrl + '/AdminCourseDash/SaveCategories'
        });
    },
    save: function () {
        console.log(courseCategoryModel);
        courseCategoryModel.CourseId = courseModel.CourseId;
        courseCategoriesEditor.saveRequest().
            done(function (response) {
                $.jGrowl('Succesfully Updated Course Categories Record', { theme: 'successGrowl', themeState: '', position : 'top-left' });
            });
    }
}

$(document).ready(function () {
    courseCategories.init();
})