var coursePreRequisite =
    {
        init: function () {

        },
        initUI: function (coursePreReqData) {
            if (courseModel.ShowPrerequisiteInfo == -1 || courseModel.ShowPrerequisiteInfo == 1)
            {
                $('#course-options-pre-req-show-info').attr('checked', 'checked');
            }
            $('#course-options-pre-req-show-info').change(function () {
                var showPreReq = $(this).prop('checked');
                courseModel.ShowPrerequisiteInfo = showPreReq ? 1 : 0;
                courseEditor.save();
            });
            $('#course-options-pre-req-info').val(courseModel.PrerequisiteInfo);
        }
    }