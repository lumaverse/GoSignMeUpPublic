var courseTranscripts = {
    ui: {
        transcriptTable : null
    },
    init: function () {
        var transcriptPromise = courseTranscripts.initData();
        transcriptPromise.done(function (data) {
            courseTranscripts.initUI(data.Data);
        })
    },
    initUI: function (data) {
        if (data.length == 0) return false;
        $('.widget-transcript-info-panel').show();
        courseTranscripts.ui.transcriptTable = $('#course-transcript-grid').DataTable({
            data: data,
            responsive: true,
            autoWitdh: false,
            columns: [
                {
                    title: "Student Name", data: 'StudentFirstName', render: function (data, type, row) {
                        return row.StudentFirstName + " " + row.StudentLastName
                    }
                },
                {
                    title: "Dates", data: 'CourseDate'
                },
                {
                    title: "School", data: 'StudentsSchool'
                },
                {
                    title: "District", data: 'District'
                }
            ]
        });
    },
    initData: function () {
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
    transcriptClickButton: function (e) {
        if ($(e).text() == 'Show Transcript Records') {
            $('#transcript-grid-container').show('slow');
            $(e).text('Hide Transcript Records');
        }
        else {
            $('#transcript-grid-container').hide('slow');
            $(e).text('Show Transcript Records');
        }
    }
}
$(document).ready(function () {
    courseTranscripts.init();
})