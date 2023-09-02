var courseCompletionCertificate = {
    ui: {
        customCertificateSelect: $('#course-completion-certicate-select')
    },
    init: function () {
        courseCompletionCertificate.initUI();
    },
    initUI: function () {
        var customCertificates = gsmuConfiguration.customCertificates;
        $(courseCompletionCertificate.ui.customCertificateSelect).unbind('change');

        $(courseCompletionCertificate.ui.customCertificateSelect).on('change', function (e, clickedIndex, newValue, oldValue) {
            var selected = $(e.currentTarget).val();
            courseModel.CourseCertificationsId = selected;
            courseEditor.save();
        });

        $('#widget-course-options a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            var index = $(e.target).closest('li').index() + 1;
            if (index != 7) return false;
            var customCertificateSelect = $(courseCompletionCertificate.ui.customCertificateSelect);
            customCertificates.map(function (item) {
                $(customCertificateSelect).append('<option value=' + item.CustomCertificateId + '>' + item.CertificateTitle + '</option>');
            });

            $(courseCompletionCertificate.ui.customCertificateSelect).val(courseModel.CourseCertificationsId )

            setTimeout(function () {
                $(courseCompletionCertificate.ui.customCertificateSelect).selectpicker('refresh');
            }, 500)
        });

        
    }
}
$(document).ready(function () {
    courseCompletionCertificate.init();
})