var StudentsNotes = (function () {
    var template = $('#student-notes-template').html();
    Mustache.parse(template); 
    function getStudentNotesData(studentId) {
        return $.ajax({
            type: "GET",
            dataType: 'json',
            url: gsmuObject.apiUrl + 'Students/GetStudentNotes?studentId=' + studentId
        });
    }

    function saveStudentNotes() {
        var studentNotesModel = {
            StudentId: StudentsNotes.studentId,
            Notes: $('#student-notes-input').val()
        }
        return $.ajax({
            type: "POST",
            data: studentNotesModel,
            dataType: 'json',
            url: gsmuObject.apiUrl + 'Students/SaveStudentNotes'
        });
    }

    return {
        loadNote: function (studentId) {
            getStudentNotesData(studentId)
                .done(function (data) {
                    var data = data.Data;
                    var rendered = Mustache.render(template,
                    {
                        StudentNotesFor: '(' + data.StudentNumber + ') ' + data.FirstName + ' ' + data.LastName,
                        Notes: data.Notes
                    });
                    $('.loader').html(rendered);
                    StudentsNotes.studentId = studentId;
                    return $.when(0);
                });
        },
        saveNote: function () {
            saveStudentNotes()
                .done(function (data, response) {
                    if (data.Success === 1) {
                        $.jGrowl('Succesfully Saved Student Notes', { theme: 'successGrowl', themeState: '' });
                        ReportsPaymentClassList.init();
                        ReportsPaymentClassList.closeStudentNotes();
                    }
                });
        },
        clearNote: function () {
            $('#student-notes-input').html('');
        },
        studentId: 0
    }
})();