var courseBundle = {
    ui: {
        isCourseBundleLabel: $('#is-course-bundle-label'),
        courseBundleGridContainer: $('#course-bundle-child-courses-container'),
        courseBundleGrid: $('#course-bundle-child-courses-grid-grid')
    },
    init: function () {
        var courseBundleData = courseBundle.loadData();
        courseBundleData.done(function (response) {
            courseBundle.initUI(response);
        });
    },
    loadData: function () {
        var courseId = UrlHelper.getUrlVars()["courseId"];;
        var getCourseBundle = function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: '/Application/AdminFunction?call=portal-course-dashboard-bundle-data&courseId=' + courseId
            });
        }
        return getCourseBundle();
    },
    initUI: function (data) {
        $(courseBundle.ui.isCourseBundleLabel).text(data.length > 0 ? 'Yes' : 'No');
        $(courseBundle.ui.courseBundleGridContainer).css('display', data.length > 0 ? 'block' : 'none')
        $(courseBundle.ui.courseBundleGrid).DataTable({
            searching: false,
            "lengthChange": false,
            data: data,
            columns: [
                {
                    title: "Course ID", data: 'CourseID', render: function (data, type, row) {
                        return data;
                    }
                },
                {
                    title: "Course Name", data: 'CourseName'
                },
                {
                    title: "Course #", data: 'CourseNumber'

                }
            ]

        });
        $('#admin-header, #admin-menu, #page-footer').hide();
    }
}
$(document).ready(function () {
    courseBundle.init();
})