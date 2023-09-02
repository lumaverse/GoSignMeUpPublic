var CourseConfirmationEmail = (function () {
    //private functions
    //databinding and ajax requests should be put to private functions to avoid exposure
    var getCourseDocumentsRequest = function () {
        return $.ajax({
            type: "GET",
            dataType: 'json',
            url: gsmuObject.apiUrl + '/AdminCourseDash/GetDocumentFilesById?courseId=' + courseModel.CourseId
        });
    };

    var loadAttachmentsData = function () {
        getCourseDocumentsRequest().done(function (response) {
            if (response.Success === 1) {
                var courseDocumentsGrid = $('#course-attachements-grid').DataTable({
                    data: response.Data,
                    destroy: true,
                    scrollY: "80",
                    scrollX: false,
                    height: 80,
                    "bInfo": false,
                    "searching": false,
                    //scrollCollapse: true,
                    paging: false,
                    columns: [
                        {
                            title: 'File Name', data: 'Name'
                        },
                        {
                            title: 'Extension', data: 'Extension'
                        },
                        {
                            title: 'Size', data: 'Size', render: function (data) {
                                return (data / 1000) + ' KB';
                            }
                        },
                        {
                            title: 'Last Updated Date', data: 'LastDateModified', render: function (data) {
                                return moment(data).format('MM/DD/YYYY')
                            }
                        }
                    ]
                });
            }
        });
    }

    var uploadFile = function () {
        setTimeout(function () {
            gsmuUIObject.mask('#widget-course-confirmation-email');
            var data = new FormData();
            var files = $('#course-email-attachment').get(0).files;
            if (files.length > 0) {
                data.append('UploadedFile', files[0]);
                data.append('courseId', UrlHelper.getUrlVars()["courseId"]);
            }
            var saveFile = $.ajax({
                type: 'POST',
                url: gsmuObject.apiUrl + 'AdminCourseDash/SaveFile',
                contentType: false,
                processData: false,
                data: data
            });
            saveFile.done(function (xhr, response) {
                if (response === 'success') {
                    $.jGrowl('Succesfully Added Course Image', { theme: 'successGrowl', themeState: '' });
                    loadAttachmentsData();
                }
                else {
                    $.jGrowl('Something went wrong on your photo upload', { theme: 'errorGrowl', themeState: '' });
                }
                gsmuUIObject.unmask('#widget-course-confirmation-email');
            })
        }, 500);
    }

    return {
        init: function () {
            CourseConfirmationEditor.initView();
            $('#course-confirmation-additional-text').html(courseModel.CourseConfirmationEmailExtraText);
            $('#widget-course-confirmation-email a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                var index = $(e.target).closest('li').index() + 1;
                switch (index) {
                    case 2:
                        loadAttachmentsData();
                        $(CourseConfirmationEmail.ui.attachmentBtn).show();
                        break;
                    default:
                        $(CourseConfirmationEmail.ui.attachmentBtn).hide();
                        break;
                }
            });
            $(CourseConfirmationEmail.ui.courseConfirmationNoregEmail).val(courseModel.NoRegEmail).unbind('change');

            $(CourseConfirmationEmail.ui.courseConfirmationNoregEmail).on('change', function (e) {
                var selected = $(e.currentTarget).val();
                courseModel.NoRegEmail = selected;
                CourseConfirmationEditor.save();
            });

            setTimeout(function () {
                $(CourseConfirmationEmail.ui.courseConfirmationNoregEmail).selectpicker();
            }, 500);  
        },
        ui: {
            editor: $('#course-confirmation-additional-text'),
            valContainer: $('#course-confirmation-emeail-additional-text'),
            inputHandler: $('#course-confirmation-input'),
            editBtn: $('#course-confirmation-editor-btns .edit'),
            saveBtn: $('#course-confirmation-editor-btns .save'),
            cancelBtn: $('#course-confirmation-editor-btns .cancel'),
            attachmentBtn: $('#course-confirmation-editor-btns .attach'),
            fileInput: $('#course-email-attachment'),
            courseConfirmationNoregEmail: $('#course-confirmation-noreg-email-dropdown')
        },
        uploadFile: function () {
            uploadFile()
        } 
    }
})();

var CourseConfirmationEditor = (function () {
    return {
        initEditor: function () {
            $(CourseConfirmationEmail.ui.editor).summernote();
            $(CourseConfirmationEmail.ui.editBtn).hide();
            $(CourseConfirmationEmail.ui.saveBtn).show();
            $(CourseConfirmationEmail.ui.cancelBtn).show();
        },
        initView: function () {
            $(CourseConfirmationEmail.ui.editor).summernote('destroy');
            $(CourseConfirmationEmail.ui.editBtn).show();
            $(CourseConfirmationEmail.ui.saveBtn).hide();
            $(CourseConfirmationEmail.ui.cancelBtn).hide();
            $(CourseConfirmationEmail.ui.attachmentBtn).hide();
        },
        initDataBinding: function () {
            var additionalText = $(CourseConfirmationEmail.ui.valContainer).find('.note-editable').html();
            $(CourseConfirmationEmail.ui.inputHandler).val(additionalText);
        },
        openFileInput: function () {
            $(CourseConfirmationEmail.ui.fileInput).click();
        },
        changeFileInput: function (input) {
            if (input.files && input.files[0]) {
                CourseConfirmationEmail.uploadFile();
            }
        },
        clearAll: function () {

        },
        save: function () {
            CourseConfirmationEditor.initDataBinding();
            courseEditor.save();
            CourseConfirmationEditor.initView();

        }
    }
})();
