var courseModel = {};
var courseMain = {
    init: function () {
        gsmuUIObject.mask('body');
        var courseDescriptionData = courseMain.loadData();
        courseDescriptionData.done(function (response) {
            if (response.Success === 1) {
                courseMain.initData(response.Data);
            }
            courseEditor.initView();
            courseMain.initValidateGoogleSync();
        });
    },
    loadData: function () {
        var courseId = UrlHelper.getUrlVars()["courseId"];
        var getCourseDesciption = function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: gsmuObject.apiUrl + '/AdminCourseDash/GetCourseDescriptionById?courseId=' + courseId
            });
        }
        return getCourseDesciption();
    },
    initData: function (data) {
        //@TODO: Work on the data from course and model should be used instead of the objects created
        courseModel = data;
        //misc info
        //etc

        var maxEnroll = data.MaxEnroll;
        var maxWait = data.MaxWait;
        var numberOfDays = data.Days;
        var Notes = data.Notes;

        var courseInfoData = {
            courseId: data.CourseId,
            courseName: data.CourseName,
            courseNumber: data.CourseNumber,
            courseMasterField: data.MasterCourseId,
            cancelCourse: data.CancelCourse,
            internalClass: data.InternalClass
        }

        var categoriesData = {
            mainCategory: data.MainCategory,
            mainCategory2: data.MainCategory2,
            mainCategory3: data.MainCategory3,
            subCategory1: data.SubCategory1,
            subCategory2: data.Subcategory2
        }

        var courseDescriptionsData = {
            longDescription: data.Description,
            shortDescription: data.ShortDescription
        }

        var courseContactInfoData = {
            contactNumber: data.ContactPhone,
            contactName: data.ContactName
        }

        var instructorInfoData = {
            instructor1: data.InstructorId,
            instructor2: data.InstructorId2,
            instructor3: data.InstructorId3
        }

        var locationData = {
            location: data.Location,
            street: data.Street,
            city: data.City,
            state: data.State,
            zip: data.Zip,
            room: data.Room
        }

        var courseCreditsData = {
            cEUCredit: data.CEUCredit,
            inServiceHours: data.InServiceHours,
            customCreditHours: data.CustomCreditHours,
            gradingSystem: data.GradingSystem,
            showCreditInPublic: data.ShowCreditInPublic,
            allowCreditRollover: data.AllowCreditRollOver
        }

        var coursePreReqData = {
            showPrerequisiteInfo: data.ShowPrerequisiteInfo,
            prerequisiteInfo: data.PrerequisiteInfo
        }

        var courseAccessCode = {
            accessCode: data.AccessCode
        }
        //expose objects publicly

        courseMain.courseNumber = data.CourseNumber;
        courseMain.courseName = data.CourseName;
        courseMain.courseDescription = data.description;
        courseMain.isOnline = data.OnlineCourse;

        courseMain.courseShortLongDescriptionData = courseDescriptionsData;
        courseMain.courseContactInfoData = courseContactInfoData;
        courseMain.courseLocationData = locationData;

        courseMain.courseMaxEnroll = maxEnroll;
        courseMain.courseMaxWait = maxWait;

        courseMain.audienceId = data.AudienceId;
        courseMain.departmentId = data.DepartmentId;
        courseMain.icons = data.Icons;
        courseMain.courseColorGrouping = data.CourseColorGrouping;
        courseMain.courseCertificationsId = data.CourseCertificationsId;

        courseMain.courseCloseDays = data.CourseCloseDays;
        courseMain.viewPastCoursesDays = data.ViewPastCoursesDays;
        courseMain.startEndTimeDisplay = data.StartEndTimeDisplay;
        courseMain.allowSendSurvey = data.AllowSendSurvey;
        courseMain.tileImageUrl = data.TileImageUrl;

        //integration objects
        courseIntegrationObject.haikuLastResult = data.HaikuLastResult;
        courseIntegrationObject.disableHaikuIntegration = data.DisableHaikuIntegration;
        courseIntegrationObject.haikuCourseId = data.HaikuCourseId;
        courseIntegrationObject.haikuImportId = data.HaikuImportId;
        courseIntegrationObject.haikuIntegrationDate = data.HaikuIntegrationDate;
        courseIntegrationObject.haikuLastIntegration = data.HaikuLastIntegrationDate;
        courseIntegrationObject.canvasCourseId = data.CanvasCourseId;
        courseIntegrationObject.disableCanvasIntegration = data.DisableCanvasIntegration;
        courseIntegrationObject.bbServer = data.BBServer;
        courseIntegrationObject.bbAutoEnroll = data.BBAutoEnroll;
        courseIntegrationObject.bbCourseCloned = data.BBCourseCloned;
        courseIntegrationObject.bbDescription = data.BBDescription;
        courseIntegrationObject.bbLastIntegrationDate = data.BBLastIntegrationDate;
        courseIntegrationObject.bbLastUpdateGrade = data.BBLastUpdateGrade;
        courseIntegrationObject.bbLastIntegrationState = data.BBLastIntegrationState;
        //call ui implementation
        courseMain.initCourseInfoUI(courseInfoData);
        courseMain.initCourseDescriptionsUI(courseDescriptionsData);
        courseMain.initCourseLocation(locationData);
        courseMain.initCourseContactInformation(courseContactInfoData);
        courseMain.initCourseConfirmationEmail();
        courseMain.initCourseEnrollmentStatistics();
        courseMain.initCourseOptionsCourseCredits();
        courseMain.initCourseOptionsCoursePreRequisite(coursePreReqData);
        courseMain.initCourseOptionAccessCode(courseAccessCode);
        courseMain.initCourseIntegration();
        courseMain.initCourseRoster();
        courseMain.initCourseSurveyUI();
        courseMain.initCoursePricing();
        gsmuUIObject.unmask('body');
    },
    initCourseInfoUI: function (courseInfoData) {
        courseEditor.initDataBinding();
        $('#course-cancel, #course-internal, #course-isonline, .bootstrap-toggle').bootstrapToggle({
            on: 'Yes',
            off: 'No'
        })
        .unbind('change');

        $('#course-cancel').on('change', function () {
            var cancelCourse = $(this).prop('checked');
            courseModel.CancelCourse = cancelCourse ? 1 : 0;
            courseEditor.save();
        });

        $('#course-internal').on('change', function () {
            var internalCourse = $(this).prop('checked');
            courseModel.InternalClass = internalCourse ? 1 : 0;
            courseEditor.save();
        });

        $('#course-isonline').on('change', function () {
            var isOnline = $(this).prop('checked');
            courseModel.OnlineCourse = isOnline ? 1 : 0;
            courseDateTimes.toggleOnlineCourse();
            courseEditor.save();
        });

        $('#course-sync-google').on('change', function () {
            var googleSync = $(this).prop('checked');
            courseModel.GoogleCalendarSyncEnabled = googleSync ? 1 : 0;
            courseEditor.save();
        });
        //initialize binding for course description controls
        
        courseMain.initCourseActionLinks();

    },
    initCourseDescriptionsUI: function (courseDescriptionsData) {
        $('#course-description-long-content').html(courseModel.Description);
        $('#course-description-short-content').html(courseModel.ShortDescription);
        $('#course-description-long-input').val(courseModel.Description);
        $('#course-description-short-input').val(courseModel.ShortDescription);
    },
    initCourseConfirmationEmail: function () {
        CourseConfirmationEmail.init();
    },
    initCourseEnrollmentStatistics: function () {
        courseEnrollmentStatistics.init();
    },
    initCourseLocation: function (locationData) {
        courseLocation.initLocationUI(locationData);
    },
    initCourseContactInformation: function (courseContactInfoData) {
        $('#contact-name').text(courseContactInfoData.contactName);
        $('#contact-number').text(courseContactInfoData.contactNumber);
    },
    initCourseOptionsCourseCredits: function (courseCreditsData) {
        courseCredits.initUI(courseCreditsData);
    },
    initCourseSurveyUI: function () {
        courseSurvey.initUI();
    },
    initCourseOptionsCoursePreRequisite: function (coursePreReqData) {
        coursePreRequisite.initUI(coursePreReqData);
    },
    initCourseOptionAccessCode: function (courseAccessCodeData) {
        courseAccessCode.initUI(courseAccessCodeData);
    },
    initCourseActionLinks: function () {
        gsmuCourseDash.wireActionLinks.init(courseMain.courseId);
    },
    initCourseIntegration: function () {
        courseIntegration.init();
    },
    initCourseRoster: function () {
        courseRosters.init();
    },
    initCoursePricing: function () {
        coursePricing.init();
    },
    initValidateGoogleSync: function () {
        var url = gsmuObject.baseUrl + 'SSO/ValidateGoogleCalendarForSync';
        $.ajax({
            url: url,
            type: "GET",
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                var data = response;
                if (data.ResponseStatus === "ok" && data.Valid) {
                    $('#google-integration-container').show();
                }
                else {
                    $('#google-integration-container').hide();
                }
            }
        })
        
    },
    courseId: UrlHelper.getUrlVars()["courseId"],
    courseNumber: '',
    courseName: '',
    description: '',
    isOnline: 0,
    courseMaxEnroll: 0,
    courseMaxWait: 0,
    courseLocationData: {},
    courseShortLongDescriptionData: {},
    courseContactInfoData: {},
    courseCreditsData: {},
    noDistPrice: 0,
    distPrice: 0,
    specialPrice: 0,
    audienceId: 0,
    departmentId: 0,
    icons: '',
    courseColorGrouping: 0,
    courseCertificationsId: 0,
    startEndTimeDisplay: '',
    allowSendSurvey: 0,
    tileImageUrl: ''
}
//separating these properties so the courseDescriptions object wont clutter
var courseIntegrationObject = {
    haikuLastResult: '',
    disableHaikuIntegration: '',
    haikuCourseId: '',
    haikuImportId: '',
    haikuIntegrationDate: '',
    haikuLastIntegration: '',
    canvasCourseId: '',
    disableCanvasIntegration: '',
    bbServer: '',
    bbAutoEnroll: '',
    bbCourseCloned: '',
    bbDescription: '',
    bbLastIntegrationDate: '',
    bbLastIntegrationState: '',
    bbLastUpdateGrade: '',
    syncCourseToGoogle: function () {
        //@TODO : make this an ajax request instead of a pop up
        var url = gsmuObject.baseUrl  + 'sso/google?begin=&end=&courseId=' + courseModel.CourseId;
        var left = (screen.width / 2) - (500 / 2);
        var top = (screen.height / 2) - (300 / 2);
        dialog = window.open(url, "google synch", 'toolbar=no, titlebar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=no, copyhistory=no, width=500, height=300, top=' + top + ', left=' + left);
        return;
    }
}

var courseEditor = {
    ui: {
        parent: $('#course-dashboard-widget-container'),
        view: $('#course-desc-content'),
        edit: $('#course-desc-content-editor')
    },
    initEditor: function () {
        $(courseEditor.ui.view).hide();
        $(courseEditor.ui.edit).show();
    },
    initView: function () {
        $(courseEditor.ui.view).show();
        $(courseEditor.ui.edit).hide();

        if (courseModel.GoogleCalendarSyncEnabled === 1) {
            $('#sync-course-btn').show();
        }
        else {
            $('#sync-course-btn').hide();
        }
        
    },
    initDataBinding: function () { //@TODO: Look for databinding plugin or make this as your own databinding library
        $('#course-name').text(courseModel.CourseName);
        $('#course-number').text(courseModel.CourseNumber);
        $('#master-course-id').text(courseModel.MasterCourseId);
        $('#course-id').text(courseModel.CourseId);

        $('#course-cancel').attr('checked', courseModel.CancelCourse !== 0 ? true : false);
        $('#course-internal').attr('checked', courseModel.InternalClass !== 0 ? true : false);
        $('#course-isonline').attr('checked', courseModel.OnlineCourse !== 0 ? true : false);
        $('#course-display-price').attr('checked', courseModel.DisplayPrice !== 0 ? true : false);
        $('#course-sync-google').attr('checked', courseModel.GoogleCalendarSyncEnabled !== 0 ? true : false);
        
        $('#course-copy-confirmation-to-instructor').attr('checked', courseModel.SendConfirmationEmailtoInstructor !== 0 ? true : false);

        $(courseEditor.ui.edit).find('input').map(function (e, el) {
            var prop = $(el).attr('name');
            var value = courseModel[prop];
            $(el).val(value).addClass('focus-editor');
        });
    },
    clearAll: function () { //@TODO: Improve clear all, should consider select and checkboxes etc
        $(courseEditor.ui.edit).find('input').map(function (e, el) {
            $(el).removeClass('focus-editor').val('');
        })
    },
    saveRequest: function () {
        return $.ajax({
            type: "POST",
            dataType: 'json',
            data: courseModel,
            url: gsmuObject.apiUrl + '/AdminCourseDash/SaveCourse'
        });
    },
    save: function () {
        gsmuUIObject.mask('body');
        var newCourseModel = {};
        $(courseEditor.ui.parent).find('input, select').serializeArray().map(function (x) {
            if (x.value === null) x.value = ""; 
            newCourseModel[x.name] = x.value; 
        });
        courseModel = $.extend(courseModel, newCourseModel);
        //console.log(courseModel);
        courseEditor.saveRequest().done(function (response) {
            if (response.Success === 1)
            $.jGrowl('Succesfully Saved Course Data', { theme: 'successGrowl', themeState: '' });
           else
               $.jGrowl('Something went wrong. ' + response.Message, { theme: 'errorGrowl', themeState: '' });
            //courseMain.init();
            //courseEditor.initView();
            gsmuUIObject.unmask('body');
        })
       
    }
}

$(document).ready(function () {
    courseMain.init();
});
