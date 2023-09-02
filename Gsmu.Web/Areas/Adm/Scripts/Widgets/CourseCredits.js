var courseCredits = {
    init: function () {
        courseCredits.initUI(); // this is just a moc call, initUI is called from CourseDescriptions
    },
    initUI: function () {
        //used the courseModel instead
        $('#course-credits-ceu').val(courseModel.CEUCredit);
        $('#course-credits-inservice').val(courseModel.InServiceHours);
        $('#course-credits-custom-credit').val(courseModel.CustomCreditHours);

        if (courseModel.GradingSystem == -1 || courseModel.GradingSystem == 1)
            $('#course-credits-grading-system').attr('checked', 'checked');
        if (courseModel.ShowCreditInPublic == -1 || courseModel.ShowCreditInPublic == 1)
            $('#course-credits-hide-public').attr('checked', 'checked');
        if (courseModel.AllowCreditRollOver == -1 || courseModel.AllowCreditRollOver == 1)
            $('#course-credits-allow-rollover').attr('checked', 'checked');

        $('#course-credits-grading-system').on('change',function () {
            var gradingSystem = $(this).prop('checked');
            courseModel.GradingSystem = gradingSystem ? 1 : 0;
            courseEditor.save();
        });
        $('#course-credits-hide-public').on('change',function () {
            var showCredit = $(this).prop('checked');
            courseModel.ShowCreditInPublic = showCredit ? 1 : 0;
            courseEditor.save();
        });
        $('#course-credits-allow-rollover').on('change',function () {
            var allowRollOver = $(this).prop('checked');
            courseModel.AllowCreditRollOver = allowRollOver ? 1 : 0;
            courseEditor.save();
        });
    }
}
var courseCreditsEditor = {
    save: function () {
        courseEditor.save();
    }
}
$(document).ready(function () {
    courseCredits.init();
});