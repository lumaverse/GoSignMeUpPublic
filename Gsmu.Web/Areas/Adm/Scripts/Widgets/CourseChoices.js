var courseChoices = {
    init: function () {
        gsmuUIObject.mask('#widget-course-options');
        var courseChoice = courseChoices.loadCourseChoicesData();
        courseChoice.done(function (item) {
            courseChoices.initUI(item.Data);
        });
    },
    selectedCourseChoice:0,
    initUI: function (data) {
        var courseChoicesData = data;
        courseChoicesData.map(function (item) {
            var id = item.CourseChoiceId;
            var desc = item.CourseChoiceName;
            var labelTemplate = '<div class="col-lg-6">' + desc + '&nbsp;</div>';
            var itemTemplate = '<div class="col-lg-2 pad-bot-10"><input type="checkbox" id="' + id + '" data-size="mini" />' + '</div>';
            $('#course-choices-list').append('<div class="row">' + labelTemplate + itemTemplate + '</div>');


            $('#' + id).on('change', function () {
                var selected = $(this).prop('checked');
                if (selected) courseChoiceEditor.addToCourseChoice(id);
                else courseChoiceEditor.removeToCourseChoice(id);
            });

        });
        //work around to make sure the values are loaded first before triggering the toggle - for dynamic controls
        $('#widget-course-options a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            var index = $(e.target).closest('li').index() + 1;
            if (index === 4) {
                var courseChoicesById = courseChoices.loadCourseChoiceByIdData();
                courseChoicesById.done(function (item) {
                    item.Data.map(function (item) {
                        var id = item.CourseChoiceId;
                        $('#' + id).prop('checked', true);
                    });

                    $('#course-choices-list input[type="checkbox"]').addClass('bootstrap-toggle');
                    //setTimeout(function () {
                    $('.bootstrap-toggle').bootstrapToggle({
                        on: 'Yes',
                        off: 'No'
                    });
                    //}, 500);
                });
            }
        });
        gsmuUIObject.unmask('#widget-course-options');
    },
    loadCourseChoicesData: function () {
        return $.ajax({
            type: "GET",
            dataType: 'json',
            url: gsmuObject.apiUrl + '/MasterSettings/CourseChoices'
        });
    },
    loadCourseChoiceByIdData: function () {
        return $.ajax({
            type: "GET",
            dataType: 'json',
            url: gsmuObject.apiUrl + '/AdminCourseDash/GetCourseChoiceById?courseId=' + courseModel.CourseId
        });
    },
    saveCourseChoiceRequest: function (choiceId) {
        return $.ajax({
            type: "POST",
            dataType: 'json',
            url: gsmuObject.apiUrl + '/AdminCourseDash/SaveCourseChoice?courseId=' + courseModel.CourseId + '&choiceId=' + choiceId
        });
    },
    deleteCourseChoiceRequest: function () {
        return $.ajax({
            type: "POST",
            dataType: 'json',
            url: gsmuObject.apiUrl + '/AdminCourseDash/SaveCourseChoice?courseId=' + courseModel.CourseId + '&choiceId=' + choiceId
        });
    }
}

var courseChoiceEditor = {
    addToCourseChoice: function (courseChoiceid) {
        gsmuUIObject.mask('#widget-course-options');
        courseChoices.saveCourseChoiceRequest(courseChoiceid)
            .done(function (xhr, response) {
                $.jGrowl('Succesfully Added Cource Choice', { theme: 'successGrowl', themeState: '' });
                gsmuUIObject.unmask('#widget-course-options');
            })
    },
    removeToCourseChoice: function (courseChoiceid) {
        gsmuUIObject.mask('#widget-course-options');
        courseChoices.saveCourseChoiceRequest(courseChoiceid)
            .done(function (xhr, response) {
                $.jGrowl('Succesfully Deleted Course Choice', { theme: 'successGrowl', themeState: '' });
                gsmuUIObject.unmask('#widget-course-options');
            })
    }
}

$(document).ready(function () {
    courseChoices.init();
})