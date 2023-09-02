var courseInstructors = function () {
    function loadData() {
        var courseId = UrlHelper.getUrlVars()["courseId"];
        var getInstructors = function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: gsmuObject.apiUrl + '/AdminCourseDash/GetCourseInstructorsById?courseId=' + courseId
            });
        }
        return getInstructors();
    }


    function saveInstructorBio(instructorId, bio) {
        return $.ajax({
            type: "POST",
            dataType: 'json',
            url: gsmuObject.apiUrl + '/AdminCourseDash/SaveInstructorBio?instructorId=' + instructorId + '&bio=' + bio
        });
    }
    return {
        selectedInstructor: 0,
        currentInstructorIndex: 0,
        instructorPerCourseLimit: 3,
        instructorSelectedIndex: 0,
        ui: {
            view: $('#course-location-info'),
            editBtn: $('#course-instructors-editor-btns .edit'),
            saveBtn: $('#course-instructors-editor-btns .save'),
            cancelBtn: $('#course-instructors-editor-btns .cancel'),
            instructorDropdown: $('#course-instructors-dropdown')
        },
        init: function () {
            gsmuUIObject.mask('.widget-instructor-panel');
            var courseInstructorsData = loadData();
            courseInstructorsData.done(function (response) {
                if (response.Success === 1) {
                    courseInstructors.initUI(response.Data);
                }
            });
        },
        loadInstructorsDropdown: function () {
            gsmuConfiguration.globalDropdown.getInstructors().done(function (data) {
                var instructors = data.Data;
                instructors.map(function (data) {
                    var itemTemplate = '<option onClick="courseInstructors.setInstructor(' + data.Value + ')" value=' + data.Value + '>' + data.DisplayText + '</option>';
                    $(courseInstructors.ui.instructorDropdown).append(itemTemplate);
                });
                $(courseInstructors.ui.instructorDropdown).selectpicker();
            })
        },
        loadInstructorDataById: function (instructorId) {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: gsmuObject.apiUrl + '/AdminCourseDash/GetInstructorById?instructorId=' + instructorId
            });
        },
        loadInstructorDataToTab: function (saveToCourse) {
            gsmuUIObject.mask('#widget-course-options');
            var instructorId = parseInt(courseInstructors.selectedInstructor);
            var instructorPromise = courseInstructors.loadInstructorDataById(instructorId);
            instructorPromise.done(function (data) {
                var instructorData = data.Data;
                courseInstructors.initTabsTemplate(instructorData, courseInstructors.currentInstructorIndex, saveToCourse);
                courseInstructors.checkInstructorCount();
                gsmuUIObject.unmask('#widget-course-options');
            });
        },
        checkInstructorCount: function () {
            if (courseInstructors.instructorPerCourseLimit <= courseInstructors.currentInstructorIndex) {
                $('#course-instructors-editor-btns .add, #course-instructors-editor-btns .btn-group').hide();
                return false;
            }
            else {
                $('#course-instructors-editor-btns .add, #course-instructors-editor-btns .btn-group').show();
                return true;
            }
        },
        initUI: function (data) {
            var instructors = data;
            instructors.map(function (instructor, index) {
                courseInstructors.initTabsTemplate(instructor, index);
            });
            courseInstructorsEditor.initView();
            courseInstructors.loadInstructorsDropdown();
            $(courseInstructors.ui.instructorDropdown).on('changed.bs.select', function (e, clickedIndex, newValue, oldValue) {
                var selected = $(e.currentTarget).val();
                courseInstructors.selectedInstructor = selected;
            });
            $('#course-copy-confirmation-to-instructor').on('change', function () {
                var sendConfimration = $(this).prop('checked');
                courseModel.SendConfirmationEmailtoInstructor = sendConfimration ? -1 : 0;
                courseEditor.save();
            });

            $('#widget-course-instructors a[data-toggle="tab"]').on('shown.bs.tab', function (e, i) {
                var index = $(e.target).attr("data-index") // activated tab
                courseInstructors.instructorSelectedIndex = parseInt(index);
            });
            if (courseInstructors.currentInstructorIndex == 0)
            {
                courseInstructors.setInstructorToNone();
            }
        },
        setInstructor: function () {
            if (courseInstructors.checkInstructorCount()) courseInstructors.loadInstructorDataToTab(true);
        },
        setInstructorToNone: function () {
            var contentTarget = $('#course-intstructors-tabs-content');
            var contentTemplate = '<div><b>No Instructor/s Selected.</b></div>'
            $(contentTemplate).appendTo(contentTarget);
        },
        initTabsTemplate: function (instructorData, index, saveToCourse) {
            index = index + 1;
            var headerTarget = $('#course-intstructors-tabs-header');
            var contentTarget = $('#course-intstructors-tabs-content');
            var photoRoot = widgetConfig.profileImagePath;
            var photoURL = instructorData.PhotoImage;
            var instructorFullName = instructorData.FirstName + ' ' + instructorData.LastName;
            //@TODO : Make a plugin to create tabs out data
            var noImage = '../../Images/NoProfileImage.png';
            var headerTemplate =
                '<li role="presentation" ' + (index == 1 ? 'class="active"' : '') + '>'
                + '<a href="#course-instructors-' + index + '" data-toggle="tab" data-index="' + index + '">'
                + instructorFullName
                + '<span class="close" data-name="' + instructorFullName + '" onClick=\"courseInstructorsEditor.removeByIndex(' + index + ', this)" id="course-instructor-' + index + '-remove">×</span></a>';

            var contentTemplate =
                '<div id="course-instructors-' + index + '" class="tab-pane fade in ' + (index == 1 ? 'active' : '') + '">'
                + '<div class="col-lg-6 pad-x-0 course-instructors-image-thumb"><img onerror="courseInstructors.changeUrl(this)" src="' + (photoURL ? photoRoot + photoURL : noImage) + '" class="img-thumbnail" style="height:' + widgetConfig.profileImageMaxHeight + 'px; width: ' + widgetConfig.profileImageMaxWidth + 'px;" /></div>'
                + '<div class="col-lg-6 pad-x-0 course-instructors-fees pull-right text-right">'
                + '<div><b>Fixed fee</b> : $ ' + 0.00 + '</div>'
                + '<div><b>Per/stud fee</b> : $ ' + 0.00 + '</div>'
                + '<div><b>Revenue fee</b> : ' + 0 + ' % </div>'
                + '</div>'
                + '<div style="clear: both;"></div>'
                + '<div class="course-instructors-contact">'
                + '<div>' + instructorData.Email + '</div>'
                + '<div>' + instructorData.WorkPhone + ' | ' + instructorData.HomePhone + '</div>'
                + '</div><br />'
                + ' <b>BIO</b>  :  <br />'
                + '<div class="course-instructors-bio">' + (instructorData.Bio ? instructorData.Bio : 'There is no bio added') + '</div>'
                + '</div>';

            courseInstructors.currentInstructorIndex = index;
            $(headerTemplate).appendTo(headerTarget);
            $(contentTemplate).appendTo(contentTarget);
            //$('.course-instructors-bio').html($('.course-instructors-bio').html())
            //do saving here
            if (index === 1 && parseInt(courseInstructors.selectedInstructor) > 0) {
                courseModel.InstructorId = parseInt(courseInstructors.selectedInstructor);
            }
           
            if (index === 2 && parseInt(courseInstructors.selectedInstructor) > 0) {
                courseModel.InstructorId2 = parseInt(courseInstructors.selectedInstructor);
            }
            
            if (index === 3 && parseInt(courseInstructors.selectedInstructor) > 0) {
                courseModel.InstructorId3 = parseInt(courseInstructors.selectedInstructor);
            }
            
            if (saveToCourse) courseEditor.save();
            gsmuUIObject.unmask('.widget-instructor-panel');
        },
        changeUrl: function (e) {
            $(e).attr('src', '../../Images/NoProfileImage.png');
        },
        saveInstructorBio: function () {
            var instructorId = 0;
            var value = ''
            if (courseInstructors.instructorSelectedIndex === 0 || courseInstructors.instructorSelectedIndex === 1) {
                instructorId = courseModel.InstructorId;
                value = $($('.course-instructors-bio')[0]).summernote('code');

            }
            else if (courseInstructors.instructorSelectedIndex === 2) {
                instructorId = courseModel.InstructorId2;
                value = $($('.course-instructors-bio')[1]).summernote('code');
            }
            else if (courseInstructors.instructorSelectedIndex === 3)
            {
                instructorId = courseModel.InstructorId3;
                value = $($('.course-instructors-bio')[2]).summernote('code');
            }
            return saveInstructorBio(instructorId, value);
        }
    }
}();

var courseInstructorsEditor = {
    initEditor: function () {
        $(courseInstructors.ui.editBtn).hide();
        $(courseInstructors.ui.saveBtn).show();
        $(courseInstructors.ui.cancelBtn).show();

        $(courseInstructors.ui.view).hide();
        $('.course-instructors-bio').summernote({
            height: 150,
            minHeight: 150,
            maxHeight: 150,
        });

        courseInstructorsEditor.initDataBinding();
    },
    initView: function () {
        $(courseInstructors.ui.editBtn).show();
        $(courseInstructors.ui.saveBtn).hide();
        $(courseInstructors.ui.cancelBtn).hide();

        $(courseInstructors.ui.view).show();
        $(courseInstructors.ui.editor).hide();

        $('.course-instructors-bio').each(function (i, e) {
            $(e).summernote('destroy');
        })
    },
    initDataBinding: function () {
        //code snippet should be the pattern for all
        $(courseInstructors.ui.editor).find('input, select').map(function (e, el) {
            var prop = $(el).attr('name');
            var value = courseModel[prop];
            $(el).val(value).addClass('focus-editor');
        });
    },
    clearAll: function () {

    },
    removeByIndex: function (index, e) {
        var name = $(e).attr('data-name')
        bootbox.confirm({
            title: 'Confirmation',
            size: "small",
            message: "Are you sure you want to remove <b>" + name + " </b> in this course?",
            callback: function (result) {
                if (result) {
                    if (index == 1) courseModel.InstructorId = "";
                    if (index == 2) courseModel.InstructorId2 = "";
                    if (index == 3) courseModel.InstructorId3 = "";
                    var tabId = $(e).parents('a').attr('href');
                    $(e).parents('li').remove();
                    $(tabId).remove();
                    courseInstructors.currentInstructorIndex--;
                    courseInstructors.checkInstructorCount();
                    courseEditor.save();
                }
            }
        });
    },
    save: function () {
        courseEditor.save();
        courseInstructorsEditor.saveBio();
        courseInstructorsEditor.initView();
    },
    saveBio: function () {
        courseInstructors.saveInstructorBio().done(function (e) {
            $.jGrowl('Succesfully Saved Instructor Bio', { theme: 'successGrowl', themeState: '' });
            courseInstructorsEditor.initView();
        })
        
    }
}
$(document).ready(function () {
    courseInstructors.init();
})