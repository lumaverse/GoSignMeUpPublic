var CourseDashQuickLinks = (function () {
    return {
        navigateToSignInSheet: function () {

        },
        navigateToPrintNameCardsLabels: function () {
            window.open(gsmuObject.adminUrl + 'nametags/name_tags_classlist.asp?caller=admin&cid=' + courseid, '_blank')
        },
        navigateToDuplicateCourse: function () {
            window.open(gsmuObject.adminUrl + 'courses_hitlist.asp?func=dup&courseid=' + courseid, '_blank')
        },
        navigateToCoursePrintCertificate: function () {
            window.open(gsmuObject.adminUrl + 'reports_certificate_print.asp?cid=' + courseid, '_blank')
        },
        navigateToEmailCourse: function () {
            
        },
        navigateToCourseTakeAttendance: function () {
            window.open(gsmuObject.adminUrl + 'courses_movestudents.asp?cid=' + courseid, '_blank')
        },
        navigateToCourseEditClassList: function () {
            window.open(gsmuObject.adminUrl + 'systemconfig_pricingoptions.asp', '_blank')
        },
        navigateToCourseMoveStudents: function () {
            window.open(gsmuObject.adminUrl + 'courses_movestudents.asp?cid=' + courseid, '_blank')
        },
        navigateToCourseReplaceStudents: function () {
            window.open(gsmuObject.adminUrl + 'courses_replacestudents.asp?cid=' + courseid, '_blank')
        },
    }
})();