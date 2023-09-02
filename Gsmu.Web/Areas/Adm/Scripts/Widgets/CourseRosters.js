var courseRosters = {
    init: function () {
        gsmuUIObject.mask('.widget-course-enrolllment-panel');
        var courseRostersData = courseRosters.loadData();
        courseRostersData.done(function (response) {
            courseRosters.loadTranscriptData().done(function (transcriptResponse) {
                if (response.Success === 1) {
                    //Transcript is Optional so if the Roster is okay it should still push through
                    courseRosters.initUI(response.Data, transcriptResponse.Success === 1 ? transcriptResponse.Data : null);
                }
            });
            
        });
    },
    loadData: function () {
        var courseId = UrlHelper.getUrlVars()["courseId"];
        var getCourseRosters = function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: gsmuObject.apiUrl + '/AdminCourseDash/GetCourseRostersById?courseId=' + courseId
            });
        }
        return getCourseRosters();
    },
    loadTranscriptData: function () {
        var courseId = UrlHelper.getUrlVars()["courseId"];;
        var getCourseTranscripts = function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: gsmuObject.apiUrl + '/AdminCourseDash/GetCourseTransciptsById?courseId=' + courseId
            });
        }
        return getCourseTranscripts();
    },
    initUI: function (data, transcriptData) {
        courseRosters.ActiveRecords = data.ActiveRosters;
        courseRosters.WaitingRecords = data.WaitingRecords;
        courseRosters.CancelledRecords = data.CancelledRecords;
        courseRosters.AllRecords = data.AllRosters;

        courseRosters.ActiveRecordsCount = data.ActiveRosters.length;
        courseRosters.WaitingRecordsCount = data.WaitingRosters.length;
        courseRosters.CancelledRecordsCount = data.CancelledRosters.length;
        courseRosters.TranscriptedRecords = [];
        if (transcriptData.length > 0)
        {
            courseRosters.TranscriptedRecords = transcriptData;
            courseRosters.TranscriptedCount = courseRosters.TranscriptedRecords.length;
            //push the transcript data
            courseRosters.TranscriptedRecords.map(function (item) {
                courseRosters.AllRecords.push({
                    Rosterid: item.studrosterid,
                    RegisteredDate: item.DateAdded,
                    RegisteredDateString: moment(item.DateAdded).format('MM-DD-YYYY'),
                    FirstName: item.StudentFirstName,
                    LastName: item.StudentLastName,
                    School: item.StudentsSchool,
                    Status: 'Transcripted',
                    Cancel: 0,
                    Waiting: 0
                });
            });

            
        }
        data = $.extend(data, { 'TranscriptedRecords': courseRosters.TranscriptedRecords })
        courseEnrollmentStatistics.loadGraph(data);

        $('#course-enrollment-data-active-grid').DataTable({
            responsive: true,
            autoWitdh: false,
            data: data.ActiveRosters,
            destroy: true,
            columns: [
                {
                    title: "First Name", data: 'FirstName'
                },
                {
                    title: "Last Name", data: 'LastName'
                },
                {
                    title: "Department", data: 'School'
                },
                {
                    title: "Transcripted Date", data: 'RegisteredDate', render: function (data, type, row) {
                        return moment(data).format('MM/DD/YYYY');
                    }
                }
            ]
        });
        //work around for grid not properly aligning on tabs
        $('#widget-course-enrolllment a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            var index = $(e.target).closest('li').index() + 1;
            switch (index)
            {
                case 2:
                    $('#course-enrollment-data-waiting-grid').DataTable({
                        responsive: true,
                        autoWitdh: false,
                        data: data.WaitingRosters,
                        destroy: true,
                        columns: [
                            {
                                title: "First Name", data: 'FirstName'
                            },
                            {
                                title: "Last Name", data: 'LastName'
                            },
                            {
                                title: "Department", data: 'School'
                            },
                            {
                                title: "Registered Date", data: 'RegisteredDate', render: function (data, type, row) {
                                    return moment(data).format('MM/DD/YYYY');
                                }
                            }
                        ]
                    });
                    break;

                case 3:
                    $('#course-enrollment-data-cancel-grid').DataTable({
                        responsive: true,
                        autoWitdh: false,
                        data: data.CancelledRosters,
                        destroy: true,
                        columns: [
                            {
                                title: "First Name", data: 'FirstName'
                            },
                            {
                                title: "Last Name", data: 'LastName'
                            },
                            {
                                title: "Department", data: 'School'
                            },
                            {
                                title: "Registered Date", data: 'RegisteredDate', render: function (data, type, row) {
                                    return moment(data).format('MM/DD/YYYY');
                                }
                            }
                        ]
                    });
                    break;
                case 4:
                    $('#course-enrollment-data-transcripted-grid').DataTable({
                        responsive: true,
                        autoWitdh: false,
                        data: courseRosters.TranscriptedRecords,
                        destroy: true,
                        columns: [
                            {
                                title: "First Name", data: 'StudentFirstName'
                            },
                            {
                                title: "Last Name", data: 'StudentLastName'
                            },
                            {
                                title: "School", data: 'StudentsSchool'
                            },
                            {
                                title: "Registered Date", data: 'DateAdded', render: function (data, type, row) {
                                    return moment(data).format('MM/DD/YYYY');
                                }
                            }
                        ]
                    });
                    break;;
            }
        });
        gsmuUIObject.unmask('.widget-course-enrolllment-panel');
    },
    exportToExcel: function () {
        util.jsonToCSV(courseRosters.AllRecords, "Roster Report", true);
    },
    exportToPDF: function () {
        var columns = [
            { title: "Roster ID", dataKey: "Rosterid" },
            { title: "First Name", dataKey: "FirstName" },
            { title: "Last Name", dataKey: "LastName" }, 
            { title: "Registered Date", dataKey: "RegisteredDateString" },
            { title: "School", dataKey: "School" },
            { title: "Status", dataKey: "Status" }
            ];
        var doc = new jsPDF('p', 'pt');
        doc.autoTable(columns, courseRosters.AllRecords, {
            addPageContent: function (data) {
                doc.text("Rosters", 40, 30);
            }
        });
        doc.save('All Rosters.pdf');
    },
    ActiveRecords: {},
    WaitingRecords: {},
    CancelledRecords: {},
    TranscriptedRecords: {},
    AllRecords: {},
    ActiveRecordsCount : 0,
    WaitingRecordsCount: 0,
    CancelledRecordsCount: 0,
    TranscriptedCount: 0
}
$(document).ready(function () {
    //courseRosters.init(); -- transferring this init on CourseMin (the main Course request should be finished first before loading this)
});