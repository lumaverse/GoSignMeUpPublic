var control = {
    yearDropDown: $('#course-enrollment-stat-date-filter') //@TODO : transfer to ui section
} 
var courseEnrollmentStatistics = {
    ui: {
        editor: $('#course-enrollment-statistics-editor'),
        view: $('#course-enrollment-statistics-info'),
        editBtn: $('#course-enrollment-stat-editor-btns .edit'),
        saveBtn: $('#course-enrollment-stat-editor-btns .save'),
        cancelBtn: $('#course-enrollment-stat-editor-btns .cancel'),
    },
    init: function () {
        //courseEnrollmentStatistics.loadGraph();
        courseEnrollmentStatEditor.initView();
    },
    loadData: function () {
        var courseId = UrlHelper.getUrlVars()["courseId"];
    },
    initUI: function (data) {
        var yearDropdown = $(control.yearDropDown);
        var currentYear = new Date().getFullYear();
        for (var x = 1990; x <= 2020; x++){
            yearDropdown.append('<option value="' + x + '" ' + x +' >' + x + '</option>');
        }
        $.when(yearDropdown.val(currentYear)).then(function () {
            courseEnrollmentStatistics.initFilterDateGraphUI(data);
        });

        yearDropdown.selectpicker({
            size: 4
        })
        .change(function () {
            courseEnrollmentStatistics.initFilterDateGraphUI(data);
        });
    },
    initFilterDateGraphUI: function (data) {
        var yearValue = $(control.yearDropDown).val();

        var activeDataYear = data.ActiveRosters.filter(function (rosterItem) {
            var activeRosterItem = $.extend(rosterItem, { RegisteredDate: new Date(rosterItem.RegisteredDate) });
            return rosterItem.RegisteredDate.getFullYear() == yearValue;
        });

        var waitingDataYear = data.WaitingRosters.filter(function (rosterItem) {
            var waitingRosterItem = $.extend(rosterItem, { RegisteredDate: new Date(rosterItem.RegisteredDate) });
            return rosterItem.RegisteredDate.getFullYear() == yearValue;
        });

        var cancelDataYear = data.CancelledRosters.filter(function (rosterItem) {
            var cancelRosterItem = $.extend(rosterItem, { RegisteredDate: new Date(rosterItem.RegisteredDate) });
            return rosterItem.RegisteredDate.getFullYear() == yearValue;
        });

        let finalActiveCount = [];
        let finalWaitingCount = [];
        let finalCancelCount = [];

        for (var x = 1; x <= 12; x++) {
            finalActiveCount.push(courseEnrollmentStatistics.filteredRostersPerMonth(activeDataYear, x))
            finalWaitingCount.push(courseEnrollmentStatistics.filteredRostersPerMonth(waitingDataYear, x));
            finalCancelCount.push(courseEnrollmentStatistics.filteredRostersPerMonth(cancelDataYear, x));
        }
        //add a bit of delay
        setTimeout(function () {
            var filteredData = {
                activeData: finalActiveCount,
                waitingData: finalWaitingCount,
                cancelData: finalCancelCount,
            }
            courseEnrollmentStatistics.loadEnrollmentDateGraph(filteredData);
        }, 1000)
    },
    loadGraph: function (data) {
        gsmuUIObject.mask('.widget-enrollment-statistics-panel');
        var activeRecordsCount = data.ActiveRosters.length;
        var waitingRecordsCount = data.WaitingRosters.length;
        var cancelledRecordsCount = data.CancelledRosters.length;
        var transcriptedRecordsCount = data.TranscriptedRecords.length;

        var maxEnroll = courseMain.courseMaxEnroll;
        var maxWait = courseMain.courseMaxWait;
        var openSeatsLeft = maxEnroll - activeRecordsCount;
        var waitListLeft = maxWait - waitingRecordsCount;

        $('#enrolled-count').text(activeRecordsCount);
        $('#enrolled-max').text(maxEnroll);
        $('#waiting-count').text(waitingRecordsCount);
        $('#waiting-max').text(maxWait);
        $('#cancelled-count').text(cancelledRecordsCount);
        $('#transcripted-count').text(transcriptedRecordsCount);

        $('#open-seats-left').text(openSeatsLeft);
        $('#wait-list-left').text(waitListLeft);

        //chartjs
        var ctx = document.getElementById("course-enrollment-statistics-graph").getContext('2d');
        var myChart = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: ["Enrolled", "Waiting", "Cancelled", 'Transcripted'],
                datasets: [{
                    label: '# of Rosters',
                    data: [activeRecordsCount, waitingRecordsCount, cancelledRecordsCount, transcriptedRecordsCount],
                    backgroundColor: [
                        '#1BC98E',
                        '#E4D836',
                        '#E64759',
                        '#5ba4cf'
                    ],
                    borderWidth: 3,
                    options: [
                        {
                            animation:
                            {
                                animateScale: true
                            }
                        }
                    ]
                }]
            }
        });
        courseEnrollmentStatistics.loadEnrollmentDateGraph({});
        courseEnrollmentStatistics.initUI(data);
    },
    loadEnrollmentDateGraph: function (data) {
        var activeCtx = document.getElementById("course-enrollment-statistics-enrollment-by-date").getContext('2d');

        var activeData = data.activeData;
        var waitingData = data.waitingData;
        var cancelData = data.cancelData;

        var enrollmentActiveByDate = new Chart(activeCtx, {
            type: 'line',
            data: {
                labels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
                datasets: [
                    {
                        label: 'Cancelled Students',
                        pointStyle: 'circle',
                        data: cancelData,
                        backgroundColor: 'rgba(235, 109, 123, 1)'
                    },
                    {
                        label: 'Waiting Students',
                        pointStyle: 'circle',
                        data: waitingData,
                        backgroundColor: 'rgba(225,212,32, 0.8)'
                    },
                    {
                        label: 'Active Students',
                        pointStyle: 'circle',
                        data: activeData,
                        backgroundColor: 'rgba(27,201,142, 0.8)'
                    }
                ]
            }
        });
        gsmuUIObject.unmask('.widget-enrollment-statistics-panel');
    },
    filteredRostersPerMonth: function (rosterItems, month) {
        var count = 0;
        rosterItems.map(function (data) {
            if (data.RegisteredDate.getMonth() == month) {
                count++;
            }
        });
        return count;
    }
}

var courseEnrollmentStatEditor = {
    initEditor: function () {
        $(courseEnrollmentStatistics.ui.editBtn).hide();
        $(courseEnrollmentStatistics.ui.saveBtn).show();
        $(courseEnrollmentStatistics.ui.cancelBtn).show();

        $(courseEnrollmentStatistics.ui.view).hide();
        $(courseEnrollmentStatistics.ui.editor).show();

        courseEnrollmentStatEditor.initDataBinding();
    },
    initView: function () {
        $(courseEnrollmentStatistics.ui.editBtn).show();
        $(courseEnrollmentStatistics.ui.saveBtn).hide();
        $(courseEnrollmentStatistics.ui.cancelBtn).hide();

        $(courseEnrollmentStatistics.ui.view).show();
        $(courseEnrollmentStatistics.ui.editor).hide();

        $(courseEnrollmentStatistics.ui.editor).hide();

        $('#course-enrollment-max-enroll').val(courseModel.MaxEnroll);
        $('#course-enrollment-max-wait').val(courseModel.MaxWait);
    },
    initDataBinding: function () {
        //code snippet should be the pattern for all
        $(courseEnrollmentStatistics.ui.editor).find('input').map(function (e, el) {
            $(el).addClass('focus-editor');
        });
    },
    clearAll: function () {

    },
    save: function () {
        //courseEnrollmentStatEditor.initDataBinding();
        courseEditor.save();
        courseEnrollmentStatEditor.initView();
    }
}

//$(document).ready(function () {
//    courseEnrollmentStatistics.init();
//});
