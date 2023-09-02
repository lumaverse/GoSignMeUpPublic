



var coursesList = function () {
    var selectedCourses = [];
    var courseId = UrlHelper.getUrlVars()["courseid"];
    var admin = parent && location.href.indexOf('browse') <= -1 && (parent.location.href.indexOf('.asp') > -1 || parent.location.href.indexOf('localhost') > -1) ? 'admin' : '';

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
        })
    }

    return {
        initUI: function (caller) {
            return new Promise(function (resolve, reject) {
                if (caller === 'admin') {
                   // $('#course-prereq-btn').hide();
                    coursesList.initGrid().then(function (response) {
                        if (response) {
                            coursesList.initGetCheckedCourses();
                            resolve(true);
                        }
                    });
                }
                else {
                    $('#current-courselist-container').hide();
                    $('#course-prerequisitelist-container').removeClass('col-sm-6').addClass('col-sm-12');
                    resolve(true);
                }
            });
        },
        initGrid: function (data) {
            if (courseId == 0) {
                $('#save-button-container').hide();
            }
            return new Promise(function (resolve, reject) {
                $('#current-courselist-table').DataTable({
                    dom: 'tip',
                    ajax: {
                        url: '/public/Course/GetCourseList',
                        type: 'POST',
                        data: {
                            courseId: courseId,
                            showPast: data,
                            keyword: $('#courselist-search').val()
                        }
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
                                var template = '<input type="checkbox" class="prereq-course-idx" onclick="coursesList.selectCourseinList('+data+')" value=' + data + ' />'
                                return template + ' ' + data;
                            },
                        },
                        {
                            title: "Course #", data: 'CourseNumber', render: function (data, type, row) {
                                return data;
                            },
                        },
                        {
                            title: "Course Name", data: 'CourseName', render: function (data, type, row) {
                                return data;
                            },
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
                    //$('#course-prereq-btn').hide();
                }
            })
        },
        getCheckedCourses: function () {
            return selectedCourses.join(',');
        },
        saveCoursePreRequisites: function () {
            saveData({
                courseId: courseId,
                preReqIds: coursesList.getCheckedCourses()
            }).done(function (e) {
                coursesList.initUI(admin).then(function () {
                    coursePrerequisites.initUI(admin);
                });
            });
        },
        selectCourseinList: function (courseid) {
            var coursePrereqIndex = selectedCourses.indexOf(courseid);
            if (coursePrereqIndex > -1) {
                selectedCourses.splice(coursePrereqIndex, 1);
            }
            else {
                selectedCourses.push(courseid);
            }

        }
    }
}();




var coursePrerequisites = function () {
    var courseId = UrlHelper.getUrlVars()["courseid"];
    function loadData() {
        return $.ajax({
            type: "GET",
            dataType: 'json',
            url: '/public/Course/GetCoursePrerequisites?courseId=' + courseId
        });
    } 
    return {
        initUI: function (caller) {
            //always call the admin grid - otherwise if logged in call the public
            if (caller === 'admin') {
                coursePrerequisites.initAdminGrid();
            }
            else {
                coursePrerequisites.initPublicGrid();
            }
        },
        initAdminGrid: function () {
            return new Promise(function (resolve, reject) {
                var table = $('#course-prerequisite-list').DataTable({
                    dom: '<"pull-left"l><"pull-right"f>tip',
                    ajax: {
                        url: '/public/Course/GetCoursePrerequisites',
                        type: 'POST',
                        data: {
                            courseId: courseId,
                        }
                    },
                    responsive: true,
                    autoWitdh: false,
                    destroy: true,
                    "ordering": false,
                    fixedHeader: true,
                    "searching": false,
                    "bLengthChange": false,
                    'iDisplayLength': 5,
                    columns: [
                        {
                            title: "Course #", data: 'CourseNumber', render: function (data, type, row) {
                                return data;
                            },
                        },
                        {
                            title: "Name", data: 'CourseName', visible: (HideCourseNameOnPreReqList=="true"? false : true),  render: function (data, type, row) {
                                return data;
                            },
                        },
                        {
                            title: "", data: 'CoursePreReqId', render: function (data, type, row) {
                                return '<i class="fa fa-times-circle" style="color:red; cursor:pointer;" onclick="coursePrerequisites.removeCourseRequisite(' + data + ')"></i>';
                            },
                       }
                    ],
                    "initComplete": function (settings, json) {
                        var isValid = json.isValid;
                        if (isValid) {
                            //do something - enable the add cart
                        }
                        resolve(true);
                    }
                });
            });
        },
        initPublicGrid: function () {
            return new Promise(function (resolve, reject) {
				
				
                var table = $('#course-prerequisite-list').DataTable({
                    dom: '<"pull-left"l><"pull-right"f>tip',
                    ajax: {
                        url: '/public/Course/GetCoursePrerequisites',
                        type: 'POST',
                        data: {
                            courseId: courseId,
                        }
                    },
                    responsive: true,
                    autoWitdh: false,
                    destroy: true,
                    "ordering": false,
                    fixedHeader: true,
                    "searching": false,
                    "bLengthChange": false,
                    'iDisplayLength': 5,
                    columns: [
                        {
                            title: "Course Number", data: 'CourseNumber', render: function (data, type, row) {
                                return data;
                            },
                        },
                        {
                            title: "Name", data: 'CourseName', visible: (HideCourseNameOnPreReqList=="true"? false : true), render: function (data, type, row) {
                                return data;
                            },
                        },
                        {
                            title: "", data: 'Attended', render: function (data, type, row) {
                                var isLoggedIn = row.IsStudentLoggedIn > 0;
                                if (!isLoggedIn)
                                {
                                    var column = table.column(3);
                                    //column.visible(false);
                                }
                                var template = data > 0 ? '<i class="fa fa-check-circle" style="color:green"></i>' : '';
                                return template;
                            }
                        }
                    ],
                    "initComplete": function (settings, json) {
                        var isValid = json.isValid;
                        var displayPrompt = json.displayPrompt;
                        if (isValid === false && !displayPrompt)
                        {   
                            $("#PreRegMsg").show();
                            //do something - enable the add cart
                            $('.hudbtn').css({ 'cursor': 'not-allowed', 'pointer-events': 'none' }).removeClass('primary').addClass('info');
                            if (isSupervisorLogin != 1) {
                                $('#CourseAccessCodeContainer').height(100);
                            } else { }
                            $('#CourseAccessCodeContainer').height(100);
                        }

                        if (displayPrompt === true && !isValid)
                        {
                            $('.hudbtn').click(function (e) {
                                $('.coursedetcloseicon').click();
                                e.preventDefault();
                                bootbox.dialog({
                                    title: 'Confirmation',
                                    size: "medium",
                                    message: "This student has not fulfilled it's prerequisites in this course, Would you like to enroll him/her anyway?",
                                    buttons: {
                                        continueEnroll: {
                                            label: "Enroll",
                                            className: 'btn-success btn-sm',
                                            callback: function () {
                                                cart.AddCourse(courseId, 'CourseDetails Prerequisite')
                                            }
                                        },
                                        close: {
                                            label: "Cancel",
                                            className: 'btn-default btn-sm',
                                            callback: function () {

                                            }
                                        }
                                    }

                                });
                            });
                            return false;
                        }
                        
                        resolve(true);
                    }
                });


            });
        },
        removeCourseRequisite: function (id) {
            var r = confirm("Are you sure you want to delete this course?");
            if (r == true) {
                return $.ajax({
                    type: 'POST',
                    dataType: 'json',
                    url: '/public/Course/SaveCoursePreReq',
                    data: { courseId: courseId, preReqIds: id, process: 'delete' },
                    success: function (data) {
                        admin = parent && (parent.location.href.indexOf('.asp') > -1 || (parent.location.href.indexOf('localhost') > -1)
                         && parent.location.href.indexOf('public') === -1) ? 'admin' : '';
                        coursePrerequisites.initUI(admin);
                    }
                })
            }

        },
    }
}();

$(document).ready(function () {
    //IFRAME PUBLIC SITE DOESNT LOAD PROPERLY when using parent.location. PUBLIC SITE DIRECTLY HIDE SHOW COMPONENTS. DEFAULT IS FOR ADMIN.
    var adminIframe = top.document.getElementById("enrollment-window-iframe");
    var admin = parent && (parent.location.href.indexOf('.asp') > -1 || (parent.location.href.indexOf('localhost') > -1)
        && parent.location.href.indexOf('public') === -1) && !adminIframe ? 'admin' : '';

    coursePrerequisites.initUI(admin);
    coursesList.initUI(admin);
    
    if ((parent.location.href.indexOf('.asp') != -1)&& (location.href.indexOf('browse') <= -1))
    {
        $('#bootstrap-header-course').show();
        $('#none-bootstrap-header-course').hide();
        $('#bootstrap-header-prereq').show();
        $('#none-bootstrap-header-prereq').hide();
    }
    else
    {
        $('#bootstrap-header-course').hide();
        $('#none-bootstrap-header-course').show();
        $('#bootstrap-header-prereq').hide();
        $('#none-bootstrap-header-prereq').show();
    }


        
})