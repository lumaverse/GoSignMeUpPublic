var courseSurvey = {
    ui: {
        entity: $('#course-survey-entity'),
        preCourse: $('#course-survey-pre-course-surveys'),
        postCourse: $('#course-survey-post-course-surveys'),
        allowSendYes: $('#send-after-course-survey-yes'),
        allowSendNo: $('#send-after-course-survey-no'),
        allowSend: $('input[name="AllowSendSurveyRadio"]')

    },
    init: function () {
        courseSurvey.initUI();
    },
    initUI: function () {
        var surveyPromise = gsmuConfiguration.globalDropdown.getSurveys();
        surveyPromise.done(function (data) {
            if (data.Success === 1) {
                data.Data.map(function (item) {
                    if (item.Value) $(courseSurvey.ui.preCourse).append('<option data-subtext="' + item.Extra +'" value="' + item.Value + '" >' + item.DisplayText + '</option>');
                    if (item.Value) $(courseSurvey.ui.postCourse).append('<option data-subtext="' + item.Extra +'" value="' + item.Value + '" >' + item.DisplayText + '</option>');
                });

                $(courseSurvey.ui.preCourse).selectpicker();
                $(courseSurvey.ui.postCourse).selectpicker();
                $(courseSurvey.ui.entity).selectpicker();
            }

            var allowSend = courseModel.AllowSendSurvey;
            if (allowSend == 1 || allowSend == -1) $(courseSurvey.ui.allowSendYes).attr('checked', 'checked')
            else $(courseSurvey.ui.allowSendNo).attr('checked', 'checked')

            $(courseSurvey.ui.allowSend).change(function (e) {
                courseModel.AllowSendSurvey = $(this).filter(':checked').val();
                courseEditor.save();
            })

        });
    }
}
