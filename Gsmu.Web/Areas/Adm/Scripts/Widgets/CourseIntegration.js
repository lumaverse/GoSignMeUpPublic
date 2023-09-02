var courseIntegration = {
    init: function () {
        courseIntegration.initUI();
    },
    haikuInit: function () {
        var haikuConfiguration = JSON.parse(globalObject.haikuConfiguration);
        var disableHaikuIntegration = courseIntegrationObject.disableHaikuIntegration; // 0 enabled, 1 disabled
        if (disableHaikuIntegration == 1) return false;

        var haikuResult = JSON.parse(courseIntegrationObject.haikuLastResult);
        $('.course-integration-lms-haiku').show();
        $('#lms-haiku-name').text(haikuResult && haikuResult.Name ? haikuResult.Name : 'PowerSchool Learning');
        $('#lms-haiku-class-id').text(courseIntegrationObject.haikuCourseId);
        $('#lms-haiku-import-id').text(courseIntegrationObject.haikuImportId);
        $('#lms-haiku-integration-enabled').text('Enabled');
        $('#lms-haiku-integrated-with').text(moment(courseIntegrationObject.haikuIntegrationDate).format('MM/DD/YYYY'));
        $('#lms-haiku-last-time-sync').text(moment(courseIntegration.haikuLastIntegration).format('MM/DD/YYYY'));
        $('#lms-haiku-last-result').html(haikuResult ? '<a onClick="courseIntegration.haikuShowResult();" class="btn btn-info btn-xs">Show</a>' : 'n / a');
        $('#lms-haiku-errors-integration').text(haikuResult.ContainsErrors ? haikuResult.Error : 'None');
    },
    canvasInit: function () {
        var canvasCourseId = courseIntegrationObject.canvasCourseId;
        var disableCanvasIntegration = courseIntegrationObject.disableCanvasIntegration;
    },
    bbInit: function () {
        //if (globalObject.bbSSOEnabled == 0) return false; seems to be always shown
        $('.course-integration-lms-blackboard, .black-board-course').show();
        var bbDuration = globalObject.bbCourseIntegrationDuration;
        $('#lms-blackboard-last-run').text(moment(globalObject.bbLastCourseIntegrationEnd).format('MM/DD/YYYY'));
        $('#lms-blackboard-duration').text(globalObject.bbCourseIntegrationDuration);

        $('#lms-blackboard-integration-state').text(courseIntegrationObject.bbLastIntegrationState == '' || courseIntegrationObject.bbLastIntegrationState == null ? 'n / a' : courseIntegrationObject.bbLastIntegrationState);
        $('#lms-blackboard-last-grade-integration').text(moment(courseIntegrationObject.bbLastUpdateGrade).format('MM/DD/YYYY'));
        $('#lms-blackboard-auto-enroll').text(courseIntegrationObject.bbAutoEnroll == 0 ? 'No' : 'Yes');
        $('#lms-blackboard-course-cloned').text(courseIntegrationObject.bbCourseCloned == 0 ? 'No' : 'Yes');
        $('#lms-blackboard-server-index').text(0);

    },
    initUI: function (courseIntegrationData) {
        courseIntegration.haikuInit(courseIntegrationData);
        courseIntegration.bbInit(courseIntegrationData);
    },
    haikuShowResult: function () {
        var haikuResult = JSON.parse(courseIntegrationObject.haikuLastResult);
        $('#haiku-result-course-id').text(haikuResult.GsmuCourseId);
        $('#haiku-result-id').text(haikuResult.Id);
        $('#haiku-result-import-id').text(haikuResult.ImportId);
        $('#haiku-result-code').text(haikuResult.Code);
        $('#haiku-result-name').text(haikuResult.Name);
        $('#haiku-result-short-name').text(haikuResult.ShortName);
        $('#haiku-result-description').text(haikuResult.Description);
        $('#haiku-result-active').text(haikuResult.Active);
        $('#haiku-result-end-date').text(moment(haikuResult.EndDate).format('MM/DD/YYYY'));
        $('#haiku-result-sync-status').text(haikuResult.GsmuSynchronizationStatus == 1 ? 'Synced' : 'Not Synced');
        $('#haiku-result-is-gsmu').text(haikuResult.IsGsmuCourse);
        $('#haiku-result-organization').text(haikuResult.OrganizationId);
        $('#haiku-result-teacher-id').text(haikuResult.TeacherId);
        $('#haiku-result-dynamic-roster').text(haikuResult.UseDymaicRoster);
        $('#haiku-result-year').text(haikuResult.Year);
        $("#haiku-modal").modal();
    }
}
