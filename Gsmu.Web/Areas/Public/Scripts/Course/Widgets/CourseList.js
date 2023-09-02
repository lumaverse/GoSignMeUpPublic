var courseList = function () {
    var selectedCourses = [];
    var courseId = UrlHelper.getUrlVars()["courseid"];
    function loadData() {
        return $.ajax({
            type: "GET",
            dataType: 'json',
            url: '/public/Course/GetCourseList'
        });
    }  

    function saveData(data) {
        return $.ajax({
            type: 'POST',
            dataType: 'json',
            url: '/public/Course/SaveCoursePreReq',
            data: data
        });
    }

    return {
        initUI: function () {
            if (courseId === 0) {
                $('#save-button-container').hide();
            }
            courseList.initGrid().then(function (response) {
                if (response) {
                    courseList.initGetCheckedCourses();
                    coursePrerequisites.initUI();
                }
            });
        },
        initGrid: function (showPast) {
            return new Promise(function (resolve, reject) {
                let data = {
                    courseId: courseId
                };

                if (showPast) {
                    data.showPast = true;
                }

                $('#current-courselist-table').DataTable({
                    ajax: {
                        url: '/public/Course/GetCourseList',
                        type: 'POST',
                        data: data
                    },
                    responsive: true,
                    autoWitdh: false,
                    destroy: true,
                    "order": [],
                    fixedHeader: true,
                    scrollY: "300px",
                    columns: [
                        {
                            title: "Course Id", data: 'CourseId', render: function (data, type, row) {
                                var template = '<input type="checkbox" class="prereq-course-id" value=' + data + ' />';
                                return template + ' ' + data;
                            }
                        },
                        {
                            title: "Course #", data: 'CourseName', render: function (data, type, row) {
                                return data;
                            }
                        },
                        {
                            title: "Course Name", data: 'CourseNumber', render: function (data, type, row) {
                                return data;
                            }
                        }
                    ],
                    "initComplete": function (settings, json) {
                        resolve(true);
                    }
                });
            });
        },
        initGetCheckedCourses: function () {
            $('.prereq-course-id').on('click', function () {
                var self = $(this);
                if (self.is(':checked')) {
                    selectedCourses.push(self.val());
                }
                else {
                    var coursePrereqIndex = selectedCourses.indexOf(self.val());
                    if (coursePrereqIndex > -1) {
                        selectedCourses.splice(coursePrereqIndex, 1);
                    }

                }
                if (selectedCourses.length > 0) {
                    $('#course-prereq-btn').show();
                }
                else {
                    // $('#course-prereq-btn').hide();
                }
            });
        },
        getCheckedCourses: function () {
            return selectedCourses.join(',');
        },
        saveCoursePreRequisites: function (cid) {
            if (courseList.getCheckedCourses().length <= 0) {
                return;
            }
            if (courseId === 0) {
                courseId = cid;
            }
            saveData({
                courseId: courseId,
                preReqIds: courseList.getCheckedCourses()
            }).done(function (e) {
                courseList.initUI();
            });
        },
        showPastCourse: function () {
            courseList.initGrid(true).then(function (response) {
                if (response) {
                    courseList.initGetCheckedCourses();
                    coursePrerequisites.initUI();
                }
            });
        }
    };
}();

$(document).ready(function () {
    courseList.initUI();
});