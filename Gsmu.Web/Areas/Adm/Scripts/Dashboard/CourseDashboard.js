var gsmuObject = {
    apiUrl: location.href.indexOf('.com') > -1 ? '/api/' : 'http://localhost:8090/', //change this to adapt the environment
    adminUrl: location.href.indexOf('.com') > -1 ? location.origin + '/admin/' : 'http://localhost/admin/',
    baseUrl: location.href.indexOf('.com') > -1 ? location.origin + '/' : 'http://localhost:56149/',
}
//this call here should only be used for global scope data configs
var gsmuConfiguration = {
    masterSettings: {},
    audiences: [],
    departments: [],
    icons: [],
    courseColorGrouping: [],
    customCertificates: [],
    requestMasterSettings: function () {
        return $.ajax({
            type: "GET",
            dataType: 'json',
            url: gsmuObject.apiUrl + '/MasterSettings/GetMasterSettings'
        });
    }, //MASTER SETTINGS (MASTERINFO)
    requestConfiguration: function () {
        return $.ajax({
            type: "GET",
            dataType: 'json',
            url: gsmuObject.apiUrl + '/AdminCourseDash/GetCourseConfiguration'
        });
    },
    globalDropdown: {
        getMainCategories: function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: gsmuObject.apiUrl + '/MasterSettings/GetMainCategories'
            });
        },
        getSubCategories: function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: gsmuObject.apiUrl + '/MasterSettings/GetSubCategories'
            });
        },
        getSubSubCategories: function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: gsmuObject.apiUrl + '/MasterSettings/GetSubSubCategories'
            });
        },
        getSurveys: function () {
            return $.ajax({
                type: 'GET',
                dataType: 'json',
                url: gsmuObject.apiUrl + '/MasterSettings/GetSurveys'
            })
        },
        getLocations: function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: gsmuObject.apiUrl + '/MasterSettings/GetLocations'
            });
        },
        getRooms: function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: gsmuObject.apiUrl + '/MasterSettings/GetRooms'
            });
        },
        getRoomDirections: function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: gsmuObject.apiUrl + '/MasterSettings/GetRoomDirections'
            });
        },
        getCountries: function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: gsmuObject.apiUrl + '/MasterSettings/GetCountries'
            });
        },
        getInstructors: function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: gsmuObject.apiUrl + '/MasterSettings/GetInstructors'
            });
        }
    }, //TODO : remove this soon and replace with separate calls and dont call everyone at once
    globalData: {
        getAllPricingOptions: function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: gsmuObject.apiUrl + '/MasterSettings/GetAllPricingOptions'
            });
        }
    },
    getMasterSettingsData: function () {
        var promise = gsmuConfiguration.requestMasterSettings();
        promise.done(function (response) {
            if (response.Success === 1)
            {
                var settingsData = response.Data;
                gsmuConfiguration.masterSettings.RoomNumberOption = settingsData.RoomNumberOption;
                gsmuConfiguration.masterSettings.RoomDirectionsOption = settingsData.RoomDirectionsOption;
            }
        })
    },
    getConfigurationData: function () { //lets call this soon, for now let's call it separately, this is not in use for now
        var promise = gsmuConfiguration.requestConfiguration();
        promise.done(function (response) {
            if (response.Success === 1) {
                var configData = response.Data;
                gsmuConfiguration.audiences = configData.Audiences;
                gsmuConfiguration.departments = configData.Departments;
                gsmuConfiguration.icons = configData.Icons;
                gsmuConfiguration.courseColorGrouping = configData.CourseColorGrouping;
                gsmuConfiguration.customCertificates = configData.CustomCertificates;

                courseIdentifiers.initUI();
                courseCompletionCertificate.initUI();
            }
        });

    },
    initConfiguration: function () {
        gsmuConfiguration.getMasterSettingsData();
        gsmuConfiguration.getConfigurationData();
        gsmuConfiguration.initGlobalUI();
    },
    initGlobalUI: function () {
        $('.date-input').datetimepicker({
            format: 'MM/DD/YYYY'
        });
        $('.time-input').datetimepicker({
            format: 'hh:mm a'
        });
    }
}

var gsmuCourseDash = {
    wireActionLinks:
    {
        init: function (courseid) {
            $('#action-widget-signin-sheet').attr('href', 'javascript:parent.buildNewPortalSignInSheet(' + courseid + ')');
            $('#action-widget-edit-class-list').attr('href', 'javascript:parent.buildNewPortalClassList(' + courseid + ')');
            $('#action-widget-email-course').attr('href', 'javascript:parent.PortalEmailCourse.prototype.executeEmailCourseAction("email", ' + courseid + ')');
            
            $('#action-widget-print-name-cards')
                .click(function () {
                    window.open(gsmuObject.adminUrl + 'nametags/name_tags_classlist.asp?caller=admin&cid=' + courseid, '_blank');
                })
                //.attr('href', '/admin/nametags/name_tags_classlist.asp?caller=admin&cid=' + courseid);
            $('#action-widget-duplicate-course')
                .click(function () {
                    window.open(gsmuObject.adminUrl + 'courses_hitlist.asp?func=dup&courseid=' + courseid, '_blank')
                })
                //.attr('href', '/admin/courses_hitlist.asp?func=dup&courseid=' + courseid);
            $('#action-widget-print-certificates')
                .click(function () {
                    window.open(gsmuObject.adminUrl + 'reports_certificate_print.asp?cid=' + courseid, '_blank')
                })
                //.attr('href', '/admin/reports_certificate_print.asp?cid=' + courseid);
            $('#action-widget-take-attendance')
                .click(function () {
                    window.open(gsmuObject.adminUrl + 'courses_movestudents.asp?cid=' + courseid, '_blank')
                })
                //.attr('href', '/admin/courses_attendance_detail.asp?cid=' + courseid);
            $('#action-widget-move-students')
                .click(function () {
                    window.open(gsmuObject.adminUrl + 'courses_movestudents.asp?cid=' + courseid, '_blank')
                })
                //.attr('href', '/admin/courses_movestudents.asp?cid=' + courseid);
            $('#action-widget-replace-students')
                .click(function () {
                    window.open(gsmuObject.adminUrl + 'courses_replacestudents.asp?cid=' + courseid, '_blank')
                })
                //.attr('href', '/admin/courses_replacestudents.asp?cid=' + courseid);
            $('.reports-link').click(function () {
                var courseId = UrlHelper.getUrlVars()["courseId"];
                gsmuCourseDash.callModal('/Adm/Reports/ReportsPaymentClassList?courseId=' + courseId);
            })

          

        }
    },
    callModal: function (src) {
        $.ajax({
            url: src,
            dataType: "html",
            success: function (data) {
                var htmlResponse = data;
                var box = bootbox.dialog({
                    show: true,
                    message: htmlResponse,
                    title: "Reports",
                    buttons: {
                        ok: {
                            label: "OK",
                            className: "btn-primary btn-sm",
                            callback: function () {
                                console.log('OK Button');
                            }
                        },
                        cancel: {
                            label: "Cancel",
                            className: "btn-default btn-sm"
                        }
                    }
                });
                var dialogHolder = $('.modal-dialog');
                dialogHolder.css({ marginRight: (($(window).width() - dialogHolder.width()) / 2) + 300 });
            }
        });


    }
}

$(document).ready(function () {
    if (location.href.indexOf('Dashboard') > -1) {
        CourseSearch.init();
        var courseId = UrlHelper.getUrlVars()["courseId"];
        if (courseId > 0){
            gsmuConfiguration.initConfiguration();
        }
    }

    setTimeout(function () {
        $('#admin-header, #admin-menu').hide('fast');
    }, 300)
    
});
