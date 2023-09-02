var courseDateTimeModel = {};
var courseDateTimes = function () {
    //variables
    var dateCounter = 0;
    //private functions
    function loadData() {
        var courseId = UrlHelper.getUrlVars()["courseId"];
        var getCourseDateTimes = function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: gsmuObject.apiUrl + '/AdminCourseDash/GetCourseDateAndTimesById?courseId=' + courseId
            });
        }
        return getCourseDateTimes();
    }

    function deleteRecordsWhenOnline() {
        var courseId = UrlHelper.getUrlVars()["courseId"];
        $.ajax({
            type: 'POST',
            url: gsmuObject.apiUrl + '/AdminCourseDash/DeleteCourseDateWhenOnlineById?courseId=' + courseId,
            dataType: 'json',
            success: function (response) {
                $.jGrowl(response.Message, { theme: 'successGrowl', themeState: '' });
                courseDateEditor.initView();
                courseDateTimes.init();
                gsmuUIObject.unmask('#widget-course-date-times');
            }
        })
    }

    return {
        ui: {
            addBtn: $('#course-date-add-btn'),
            editBtn: $('#course-date-time-btns .edit'),
            saveBtn: $('#course-date-time-btns .save'),
            cancelBtn: $('#course-date-time-btns .cancel'),
            editors: $('.widget-times-panel input[type="text"], .widget-times-panel input[type="number"]')
        },
        init: function () {
            gsmuUIObject.mask('.widget-times-panel');
            var courseDateTimesData = loadData();
            courseDateTimesData.done(function (response) {
                if (response.Success === 1) {
                    courseDateTimes.initUI(response.Data);
                    dateCounter = response.Data.length;
                    courseDateEditor.initView();
                }
            });
            
        },
        initUI: function (data) {
            var selected = [];
            courseDateTimes.courseDateData = data;
            var dataTable = $('#course-date-time-grid').DataTable({
                responsive: true,
                autoWitdh: false,
                destroy: true,
                "searching": false,
                "bPaginate": false,
                'language': {
                    search: "",
                    searchPlaceholder: ""
                },
                "bInfo": false,
                select: {
                    style: 'single'
                },
                data: data,
                columns: [
                    {
                        title: "Date", data: 'CourseDate', render: function (data, type, row, rowIndex) {
                            var dateInput = '<input type="text" class="course-date date-input form-control input-sm" data-model="CourseDate" disabled value="' + moment(data).format('MM/DD/YYYY') + '" />'
                            return dateInput;
                        }
                    },
                    {
                        title: "Start Time", data: 'StartTime', render: function (data, type, row, rowIndex) {
                            var timeInput = '<input type="text" class="course-date time-input form-control input-sm" data-model="StartTime" disabled value="' + moment(data).format('hh:mm a') + '" />'
                            return timeInput;
                        }
                    },
                    {
                        title: "Finish Time", data: 'EndTime', render: function (data, type, row, rowIndex) {
                            var timeInput = '<input type="text" class="course-date time-input form-control input-sm" data-model="EndTime" disabled value="' + moment(data).format('hh:mm a') + '" />'
                            return timeInput;
                        }

                    },
                    {
                        title: "Action", data: 'ID', render: function (data, type, row, rowIndex) {
                            var saveButton = '<button class="btn btn-success btn-xs save action-save" onClick="courseDateEditor.saveRow(this)" disabled><span class="fa fa fa-floppy-o"></span></button> &nbsp;'
                            var deleteButton = '<button class="btn btn-danger btn-xs cancel action-delete" onClick="courseDateEditor.removeRow(this)" disabled><span class="fa fa-minus-circle"></span></button>';
                            return saveButton + deleteButton;
                        }

                    }
                ],
                "createdRow": function (row, data, dataIndex) {
                    //indicates that the rows after row 2 are candidate to be removed
                    //if course changes to online
                    if (dataIndex > 1) {
                        $(row).addClass('non-online');
                    }
                }
            });
            dataTable.on('click', 'tbody tr, tr button', function () {
                $(this).addClass('selected').siblings().removeClass('selected');
            });

            courseDateTimes.dateDataTableInstance = dataTable;

            $('#widget-course-date-times a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                var index = $(e.target).closest('li').index() + 1;
                if (index == 2) courseDateTimes.initEnrollmentOptionsUI();
                if (index == 3) courseDateTimes.initDisplayCommentsUI();
            });

            gsmuUIObject.unmask('.widget-times-panel');
            if (data.length == 0) return false;
            courseDateTimes.minDate = moment(data[0].CourseDate).format('MM/DD/YYYY');
            courseDateTimes.initDateTime();
        },
        initDateTime: function () {
            $('.date-input').datetimepicker({
                format: 'MM/DD/YYYY'
            }).on("dp.change", function (e) {
                var value = e.currentTarget.value;
                $(e.target).attr('value', value);

            });

            $('.time-input').datetimepicker({
                format: 'hh:mm a'
            }).on("dp.change", function (e) {
                var value = moment(e.date._d).format('hh:mm:ss');
                $(e.target).attr('value', value);
            });
        },
        initEnrollmentOptionsUI: function () {
            $('#course-datatimes-course-close-days').val(courseModel.CourseCloseDays);
            $('#course-datatimes-view-past-courses-days').val(courseModel.ViewPastCoursesDays);
        },
        initDisplayCommentsUI: function () {
            $('#course-datatimes-start-end-time-display').val(courseModel.StartEndTimeDisplay);
        },
        toggleOnlineCourse: function () {
            if (courseModel.OnlineCourse == 1) {
                $('#course-date-time-grid').find('tr.non-online').hide();
                //use this for final deleting
                //$('#course-date-time-grid').DataTable().rows('.non-online').remove().draw(false); 
                if (dateCounter > 2) {
                    bootbox.confirm({
                        title: 'Confirm',
                        message: 'There are more than 3 dates in this course, and these will be deleted if you choose to continue, Do you want to continue?',
                        buttons: {
                            cancel: {
                                label: '<i class="fa fa-times"></i> Cancel'
                            },
                            confirm: {
                                label: '<i class="fa fa-check"></i> Confirm'
                            }
                        },
                        callback: function (result) {
                            if (result)
                            {
                                deleteRecordsWhenOnline();
                                $.jGrowl('Succesfully Updated Course To Online', { theme: 'successGrowl', themeState: '', position: 'center' });
                            }
                            else
                            {
                                $.jGrowl('Succesfully Updated Course To Online', { theme: 'successGrowl', themeState: '', position : 'center' });
                            }
                        }
                    })
                    
                }
            }
            else {
                $('#course-date-time-grid').find('tr.non-online').show();

            }
            courseDateTimes.toggleAddBtn();
        },
        toggleAddBtn: function () {
            if (courseModel.OnlineCourse == 1) $(courseDateTimes.ui.addBtn).hide();
            else $(courseDateTimes.ui.addBtn).show();
        },
        overrideUI: function () {
            $('#course-date-add-container').detach().appendTo('#course-date-time-grid_wrapper .col-sm-6:eq(1)');
        },
        minDate: '',
        dateDataTableInstance: '',
        courseDateData: []
    }
}();

var courseDateEditor = {
    initEditor: function () {
        $(courseDateTimes.ui.editBtn).hide();
        $(courseDateTimes.ui.saveBtn).show();
        $(courseDateTimes.ui.cancelBtn).show();
        $(courseDateTimes.ui.addBtn).show();

        courseDateTimes.toggleAddBtn();
        courseDateTimes.initDateTime();

        $('.course-date, .action-delete, .action-save, .widget-times-panel').removeAttr('disabled');
        $(courseDateTimes.ui.editors).removeAttr('disabled');
    },
    initView: function () {
        $(courseDateTimes.ui.editBtn).show();
        $(courseDateTimes.ui.saveBtn).hide();
        $(courseDateTimes.ui.cancelBtn).hide();
        $(courseDateTimes.ui.addBtn).hide();
        $('.course-date, .action-delete, .action-save').attr('disabled', 'disabled');
        $(courseDateTimes.ui.editors).attr('disabled', 'disabled');
        courseDateTimes.toggleAddBtn();
        $(courseDateTimes.ui.editors).removeClass('focus-editor');
        courseDateTimes.overrideUI();
    },
    initDataBinding: function () {
        $(courseDateTimes.ui.editors).addClass('focus-editor');
    },
    clearAll: function () {

    },
    removeRow: function () {
        setTimeout(function () {
            courseDateTimes.dateDataTableInstance.row('.selected').remove().draw(false);
        }, 100);
       
        //do server request
    },
    addNewRow: function () {
        var date = '<input type="text" class="course-date time-input form-control input-sm" />';
        var startTime = '<input type="text" class="course-date time-input form-control input-sm" />';
        var endTime = '<input type="text" class="course-date time-input form-control input-sm" />';

        var table = $('#course-date-time-grid').DataTable();

        var rowNode = table
            .row.add([date, startTime, endTime])
            .draw()
            .node();

        $(rowNode)
            .addClass('selected')
            .css('color', 'green')
            .animate({ color: 'black' });

        courseDateEditor.initEditor();
    },
    saveRow: function () {
        gsmuUIObject.mask('#widget-course-date-times');
        setTimeout(function () {
            courseDateTimeModel = courseDateTimes.dateDataTableInstance.row('.selected').data();
            //@TODO: make a better implementation
            $('#course-date-time-grid .selected input').each(function (i, e) {
                if ($(e).attr('data-model') == 'CourseDate'){
                    courseDateTimeModel.CourseDate = $(e).val();
                }
                if ($(e).attr('data-model') == 'EndTime'){
                    courseDateTimeModel.EndTime = e.defaultValue;
                }
                if ($(e).attr('data-model') == 'StartTime'){
                    courseDateTimeModel.StartTime = e.defaultValue;
                }
                
            });
            if (!courseDateTimeModel) return false;
            courseDateTimeModel = {
                Id: courseDateTimeModel.Id,
                CourseId: courseModel.CourseId,
                CourseDateString: courseDateTimeModel.CourseDate,
                StartTimeString: courseDateTimeModel.StartTime,
                EndTimeString: courseDateTimeModel.EndTime
            }          
            $.ajax({
                type: "POST",
                dataType: 'json',
                data: courseDateTimeModel,
                url: gsmuObject.apiUrl + '/AdminCourseDash/SaveDateTime',
                success: function (response) {
                    $.jGrowl('Succesfully Updated Date Time Record', { theme: 'successGrowl', themeState: '' });
                    courseDateEditor.initView();
                    courseDateTimes.init();
                    gsmuUIObject.unmask('#widget-course-date-times');
                }
            });
        }, 500);
    },
    save: function () {
        courseEditor.save();
        courseDateEditor.initView();
    }
}

$(document).ready(function () {
    courseDateTimes.init();
});