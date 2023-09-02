var CourseOptions = (function () {
    return {
        navigateToEditCredits: function () {
            window.open(gsmuObject.adminUrl + 'courses_edit.asp?cid='+ courseModel.CourseId + '&opensection=12', '_blank')
        },
        navigateToManageSystemCredits: function () {
            window.open(gsmuObject.adminUrl + 'systemconfig_courses_credithours.asp', '_blank')
        }
    }
})();

var CourseOptionsEditor = (function () {
    return {
        save: function () {
            courseMain.Save();
        }
    }
})();