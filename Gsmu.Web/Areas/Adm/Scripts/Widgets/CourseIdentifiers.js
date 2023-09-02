var courseIdentifiers = {
    ui: {
        audiencesSelect: $('#course-identifiers-target-audience'),
        departmentsSelect: $('#course-identifiers-dept-name'),
        icons1Select: $('#course-identifiers-icon-1'),
        icons2Select: $('#course-identifiers-icon-2'),
        icons3Select: $('#course-identifiers-icon-3'),
        icons4Select: $('#course-identifiers-icon-4'),
        courseGroupSelect: $('#course-identifiers-course-grouping')
    },
    iconsData:[0, 0, 0, 0],
    init: function () {
        courseIdentifiers.initUI();
    },
    initUI: function () {
        var audiences = gsmuConfiguration.audiences;
        var departments = gsmuConfiguration.departments;
        var icons = gsmuConfiguration.icons;
        var courseGroupig = gsmuConfiguration.courseColorGrouping;

        audiences.map(function (item) {
            $(courseIdentifiers.ui.audiencesSelect).append('<option value=' + item.AudienceId + '>' + item.AudienceName + '</option>');
        });

        departments.map(function (item) {
            $(courseIdentifiers.ui.departmentsSelect).append('<option value=' + item.DepartmentId + '>' + item.DepartmentName + '</option>');
        });

        courseGroupig.map(function (item) {
            var color = item.CourseCategoryColor;
            $(courseIdentifiers.ui.courseGroupSelect).append('<option style="background: #' + item.CourseCategoryColor + '; color: #0000;" value=' + item.CourseCategoryID + ' data-style="background-color:red">' + item.CourseCategoryName + '</option>');
        });

        icons.map(function (item) {
            $(courseIdentifiers.ui.icons1Select).append('<option value=' + item.IconId + '>' + item.IconTitle + '</option>');
            $(courseIdentifiers.ui.icons2Select).append('<option value=' + item.IconId + '>' + item.IconTitle + '</option>');
            $(courseIdentifiers.ui.icons3Select).append('<option value=' + item.IconId + '>' + item.IconTitle + '</option>');
            $(courseIdentifiers.ui.icons4Select).append('<option value=' + item.IconId + '>' + item.IconTitle + '</option>');
        });

        $(courseIdentifiers.ui.audiencesSelect).val(courseModel.AudienceId)
            .unbind('change')

        $(courseIdentifiers.ui.departmentsSelect).val(courseModel.DepartmentId)
            .unbind('change')

        $(courseIdentifiers.ui.courseGroupSelect).val(courseModel.CourseColorGrouping)
            .unbind('change')

        $(courseIdentifiers.ui.icons1Select).val(icons[0])
            .unbind('change')

        $(courseIdentifiers.ui.icons2Select).val(icons[1])
            .unbind('change')

        $(courseIdentifiers.ui.icons3Select).val(icons[2])
            .unbind('change')

        $(courseIdentifiers.ui.icons4Select).val(icons[3])
            .unbind('change')

        $(courseIdentifiers.ui.audiencesSelect).on('change', function (e, clickedIndex, newValue, oldValue) {
            var selected = $(e.currentTarget).val();
            courseModel.AudienceId = selected;
            courseEditor.save();
        });

        $(courseIdentifiers.ui.departmentsSelect).on('change', function (e, clickedIndex, newValue, oldValue) {
            var selected = $(e.currentTarget).val();
            courseModel.DepartmentId = selected;
            courseEditor.save();
        });

        $(courseIdentifiers.ui.courseGroupSelect).on('change', function (e, clickedIndex, newValue, oldValue) {
            var selected = $(e.currentTarget).val();
            courseModel.CourseColorGrouping = selected;
            courseEditor.save();
        });

        $(courseIdentifiers.ui.icons1Select).on('change', function (e, clickedIndex, newValue, oldValue) {
            var selected = $(e.currentTarget).val();
            courseIdentifiers.iconsData[0] = selected;
            courseModel.Icons = courseIdentifiers.iconsData.join('~|~');
        });

        $(courseIdentifiers.ui.icons2Select).on('change', function (e, clickedIndex, newValue, oldValue) {
            var selected = $(e.currentTarget).val();
            courseIdentifiers.iconsData[1] = selected;
            courseModel.Icons = courseIdentifiers.iconsData.join('~|~');
        });

        $(courseIdentifiers.ui.icons3Select).on('change', function (e, clickedIndex, newValue, oldValue) {
            var selected = $(e.currentTarget).val();
            courseIdentifiers.iconsData[2] = selected;
            courseModel.Icons = courseIdentifiers.iconsData.join('~|~');
        });

        $(courseIdentifiers.ui.icons4Select).on('change', function (e, clickedIndex, newValue, oldValue) {
            var selected = $(e.currentTarget).val();
            courseIdentifiers.iconsData[3] = selected;
            courseModel.Icons = courseIdentifiers.iconsData.join('~|~');
        });

        $('#widget-course-options a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            var index = $(e.target).closest('li').index() + 1;
            courseIdentifiers.iconsData = courseModel.Icons == '' ? courseIdentifiers.iconsData : courseModel.Icons.split('~|~');
            if (index != 5) return false;

            setTimeout(function () {
                courseIdentifiers.initSelectBox();
            }, 500)
            
        });
        
    },
    initSelectBox: function () {
        $(courseIdentifiers.ui.audiencesSelect).selectpicker();
        $(courseIdentifiers.ui.departmentsSelect).selectpicker();
        $(courseIdentifiers.ui.icons1Select).selectpicker();
        $(courseIdentifiers.ui.icons2Select).selectpicker();
        $(courseIdentifiers.ui.icons3Select).selectpicker();
        $(courseIdentifiers.ui.icons4Select).selectpicker();
        $(courseIdentifiers.ui.courseGroupSelect).selectpicker();
    },
    initUIData: function () {

    },
    loadCourseData: function () {
       
    }
}
$(document).ready(function () {
    courseIdentifiers.init();
});