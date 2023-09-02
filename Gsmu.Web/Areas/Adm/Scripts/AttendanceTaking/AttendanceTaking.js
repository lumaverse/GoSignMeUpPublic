var gsmuObject = {
    apiUrl: location.href.indexOf('.com') > -1 ? '/api/' : 'http://localhost:8090/', //change this to adapt the environment
    adminUrl: location.href.indexOf('.com') > -1 ? location.origin + '/admin/' : 'http://localhost/admin/',
    baseUrl: location.href.indexOf('.com') > -1 ? location.origin + '/' : 'http://localhost:56149/',
}
var AttendanceActions = {
    SetSingleDateAttendance: function (rosterid, date, Isattended) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: gsmuObject.apiUrl + 'CourseAttendanceTaking/SaveDateAttendance?rosterid=' + rosterid +'&date='+date+'&status='+Isattended,
        });
    },
    SaveStatusAttendance: function (rosterid, control) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: gsmuObject.apiUrl + 'CourseAttendanceTaking/SaveStatusAttendance?rosterid=' + rosterid + '&status='+control.value,
        });
    },
    SaveAttendanceGrade: function (rosterid, control) {
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: gsmuObject.apiUrl + 'CourseAttendanceTaking/SaveAttendanceGrade?rosterid=' + rosterid + '&grade='+control.value,
        });
    },
    TranscribeAllStudents: function (courseid) {
        gsmuUIObject.mask("#layout-content-container");
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: gsmuObject.apiUrl + 'CourseAttendanceTaking/TranscribeAllStudents?courseid=' + courseid,
            success: function (data) {
                location.reload();
            }
        });
    },
    TranscribeSingleStudent: function (rosterid) {
        gsmuUIObject.mask("#layout-content-container");
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: gsmuObject.apiUrl + 'CourseAttendanceTaking/TranscribeSingleStudent?rosterid=' + rosterid,
            success: function (data) {
                location.reload();
            }
        });
    },
    SetAllAttendedValue: function (status) {
        var courseid = 0;
        var sThisVal = $("#courseidvalue").val();
        if (sThisVal != "") {
            courseid = sThisVal;
        }
        var selectedactionvalue = $("input[name=AllAttended]:checked").val();
        var confirmmessage = "Are you sure you want to set All date(s) attended?";
        if (status == 0) {
            selectedactionvalue = $("input[name=AllNotAttended]:checked").val();
            var confirmmessage = "Are you sure you want to set All date(s) not attended?";
        }
        if (selectedactionvalue == "ALL") {
            var r = confirm(confirmmessage);
            if (r == true) {
                gsmuUIObject.mask("#layout-content-container");
                $.ajax({
                    type: "POST",
                    dataType: 'json',
                    url: gsmuObject.apiUrl + 'CourseAttendanceTaking/SetAllAttendance?courseid=' + courseid + '&status=' + status,
                    success: function (data) {
                        location.reload();
                    }
                });
            }
        }
        else if (selectedactionvalue == "None") {
            if (status == 1) {
                $('.attended-radio-item').each(function () {
                    $(this).prop('checked', false);

                });
            }
            else {
                $('.notattended-radio-item').each(function () {
                    $(this).prop('checked', false);

                });
            }
        }
        else {
            if (status == 0) {
                $('.head-notattended-radio').each(function () {
                    var sThisVal = (this.checked ? $(this).val() : "");
                    if (sThisVal != "") {
                        gsmuUIObject.mask("#layout-content-container");
                        $.ajax({
                            type: "POST",
                            dataType: 'json',
                            url: gsmuObject.apiUrl + 'CourseAttendanceTaking/SetAllAttendance?courseid=' + courseid + '&status=' + status + '&requesteddate=' + sThisVal,
                            success: function (data) {
                                location.reload();
                            }
                        });
                    }
                });
            }
            else {
                $('.head-attended-radio').each(function () {
                    var sThisVal = (this.checked ? $(this).val() : "");
                    if (sThisVal != "") {
                        gsmuUIObject.mask("#layout-content-container");
                        $.ajax({
                            type: "POST",
                            dataType: 'json',
                            url: gsmuObject.apiUrl + 'CourseAttendanceTaking/SetAllAttendance?courseid=' + courseid + '&status=' + status + '&requesteddate=' + sThisVal,
                            success: function (data) {
                                location.reload();
                            }
                        });
                    }
                });
            }
        }
    },
    SetClearAttendedValue: function () {
    }


}
var AttendanceConfiguration = {
    ui: {
        AttendanceHeader: $('.attendance-header-title'),
        AttendanceGrid: $('#attendance-grid'),
        AttendaceDatatable:null
    },
    initUI: function (header, data,config) {
        $('.attendance-header-title').text(header);
        var hiddenfields = [24];
        var selectedcredittypes = [];
        var showoptionalcredits = true;
        var hiddenfieldsstring = config.Data.HiddenCreditFields;
        if (hiddenfieldsstring.indexOf("credithours") >= 0) {
            hiddenfields.push(7);
        }
        else {
            selectedcredittypes.push("credithours");
        }
        if (hiddenfieldsstring.indexOf("customcredit") >= 0) {
            hiddenfields.push(9);
        }
        else {
            selectedcredittypes.push("customcredit");
        }
        if (hiddenfieldsstring.indexOf("inservice") >= 0) {
            hiddenfields.push(10);
        }
        if (hiddenfieldsstring.indexOf("parking") >= 0) {
            hiddenfields.push(12);
        }

        hiddenfields.push(13);
        if (hiddenfieldsstring.indexOf("ceu") >= 0) {
            hiddenfields.push(14);
            hiddenfields.push(15);
        }
        else {
            selectedcredittypes.push("ceu");
        }
        if (hiddenfieldsstring.indexOf("optionalcredits") >= 0) {
            hiddenfields.push(16);
            hiddenfields.push(17);
            hiddenfields.push(18);
            hiddenfields.push(19);
            hiddenfields.push(20);
            hiddenfields.push(21);
            hiddenfields.push(22);
            hiddenfields.push(23);
            showoptionalcredits = false;

        }
        if (config.Data.FieldLabel.LabelOptionalCredit1 == "") {
            hiddenfields.push(16);
        }
        else {
            if (showoptionalcredits) {
                selectedcredittypes.push("optionalcredit1");
            }
        }
        if (config.Data.FieldLabel.LabelOptionalCredit2 == "") {
            hiddenfields.push(17);
        }
        else {
            if (showoptionalcredits) {
                selectedcredittypes.push("optionalcredit2");
            }
        }
        if (config.Data.FieldLabel.LabelOptionalCredit3 == "") {
            hiddenfields.push(18);
        }
        else {
            if (showoptionalcredits) {
                selectedcredittypes.push("optionalcredit3");
            }
        }
        if (config.Data.FieldLabel.LabelOptionalCredit4 == "") {
            hiddenfields.push(19);
        }
        else {
            if (showoptionalcredits) {
                selectedcredittypes.push("optionalcredit4");
            }
        }
        if (config.Data.FieldLabel.LabelOptionalCredit5 == "") {
            hiddenfields.push(20);
        }
        else {
            if (showoptionalcredits) {
                selectedcredittypes.push("optionalcredit5");
            }
        }
        if (config.Data.FieldLabel.LabelOptionalCredit6 == "") {
            hiddenfields.push(21);
        }
        else {
            if (showoptionalcredits) {
                selectedcredittypes.push("optionalcredit6");
            }
        }
        if (config.Data.FieldLabel.LabelOptionalCredit7 == "") {
            hiddenfields.push(22);
        }
        else {
            if (showoptionalcredits) {
                selectedcredittypes.push("optionalcredit7");
            }
        }
        if (config.Data.FieldLabel.LabelOptionalCredit8 == "") {
            hiddenfields.push(23);
        }
        else {
            if (showoptionalcredits) {
                selectedcredittypes.push("optionalcredit8");
            }
        }
        AttendanceConfiguration.initAttendanceDataTable(hiddenfields, selectedcredittypes, header, data, config);
    },
    initAttendanceDataTable: function (hiddenfields, selectedcredittypes, header, data, config) {
        AttendanceConfiguration.ui.AttendaceDatatable = $('#attendance-grid').DataTable({
            searching: false,
            "lengthChange": false,
            "paging": false,
            "aaSorting": [[24, 'asc'], [2, 'asc']],
            data: data.Data,
            'columnDefs': [{
                "targets": [0, 3, 4, 5, 6, 7, 8, 9, , 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21], // your case first column
                "className": "text-center",
                "width": "4%",
                "orderable": false
            },
            {
                "targets": hiddenfields,
                "visible": false
            }
            ],
            columns: [
                {
                    title: "Transcribe ", data: 'RosterId', render: function (data, type, row) {
                        if (row.IsTranscribed == 0) {
                            return '<input type="checkbox" value="' + data + '" class="Transcribe-checkbox">';
                        }
                        else {
                            return '<input type="checkbox" value="' + data + '" class="Transcribe-checkbox" checked=checked disabled>';
                        }
                    }
                },
                {
                    title: "Last Name", data: 'LastName'
                }, {
                    title: "First Name", data: 'FirstName'

                },
                {
                    title: "Attendance Status", data: 'AttendanceStatus', render: function (data, type, row) {
                        var attendancestatus = '';
                        $.each(config.Data.AttendanceStatus, function (i, item) {
                            
                            attendancestatus = attendancestatus + '<option value="' + config.Data.AttendanceStatus[i].Id + '"' + AttendanceConfiguration.getAttendanceDetails(data, config.Data.AttendanceStatus[i].Id) + '>'+config.Data.AttendanceStatus[i].Status+'</option>'
                        });
                        return '<select onchange="AttendanceActions.SaveStatusAttendance(' + row.RosterId + ',' + 'this) ">'
                               + '<option ></option>'
                               + attendancestatus
                               + '  </select>';
                    }

                },
                {
                    title: "Attended<div class='attendance-header-dropdown'>"
                            + '<div class="btn-group">'
                             + '<button type="button" class="btn-custom-attendance btn btn-secondary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">'
                                    + '<span class="dropdown-selected-label">SELECT</span><span class="dropdown-selected-item"></span>'
                             + '</button>'
                             + '</button>'
                             + '<div class="dropdown-menu">'
                                   + '<div style="margin-left:5px;"><input type="radio" name="AllAttended" value="None"  class="attended-radio"> NONE </br>'
                                   + '<input type="radio" name="AllAttended" value="ALL"  class="attended-radio"> ALL  </div>'
                                   
                                   + ' <div class="dropdown-divider"></div>'
                                   + ' <div class="custom-dropdown-item">Individual Date(s)</div>'
                                    + AttendanceConfiguration.getListOfDatesForHeader('Yes', config)
                                        + '<div class="custom-dropdown-item-group"><input type="button" value="CLEAR" class="attendance-custom-button-red" />&nbsp;<input type="button" value="SET" class="attendance-custom-button-green" onclick="AttendanceActions.SetAllAttendedValue(1)" /></div>'

                           + ' </div>'
                       + ' </div>'

                        + "</div>", data: 'AttendanceDateList', data: 'AttendanceDateList', render: function (data, type, row) {
                            if (row.IsWaitListed == 0) {
                                var AttendedDates = "";
                                var ischecked = "";
                                $.each(config.Data.CourseDates, function (i, item) {
                                    $.each(row.AttendanceDateList, function (index, value) {
                                        if (config.Data.CourseDates[i].CourseDateItem == row.AttendanceDateList[index].Coursedate && row.AttendanceDateList[index].IsAttended == 1) {
                                            ischecked = "checked";
                                        }
                                    })
                                    AttendedDates = AttendedDates + "<div  style='margin-top:3px'>" + '<input onclick="AttendanceActions.SetSingleDateAttendance(' + row.RosterId + ',\'' + config.Data.CourseDates[i].CourseDateItem + '\',1)" type="radio" name="' + row.RosterId + "_" + i + '" value="' + config.Data.CourseDates[i].CourseDateItem + '"  class="attended-radio-item attended-radio" ' + ischecked + '>' + "</div>";
                                    ischecked = "";
                                })
                                return AttendedDates;
                            }
                            else {

                                return "<div style='margin:6px; color: grey; font-weight:bold;'>WL</div>";
                            }


                        }

                },
                {
                    title: "Did Not Attend <div class='attendance-header-dropdown'>"
                        + '<div class="btn-group">'
                             + '<button type="button" class="btn-custom-attendance btn btn-secondary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">'
                                      + '<span class="dropdown-selected-label">SELECT</span><span class="dropdown-selected-item"></span>'
                             + '</button>'
                             + '</button>'
                             + '<div class="dropdown-menu">'
                                   + '<div style="margin-left:5px;"><input type="radio" name="AllNotAttended" value="None"  class="attended-radio"> NONE <br />'
                                            + '<input type="radio" name="AllNotAttended" value="ALL"  class="attended-radio"> ALL  </div>'
                                            + ' <div class="dropdown-divider"></div>'
                                            + ' <div class="custom-dropdown-item ">Individual Date(s)</div>'
                                            + AttendanceConfiguration.getListOfDatesForHeader('Not', config)
                                   + '<div class="custom-dropdown-item-group"><input type="button" value="CLEAR" class="attendance-custom-button-red" />&nbsp;<input type="button" value="SET" class="attendance-custom-button-green" onclick="AttendanceActions.SetAllAttendedValue(0)" /></div>'
                           + ' </div>'
                       + ' </div>'
                        + '</div>', data: 'AttendanceDateList', render: function (data, type, row) {
                            if (row.IsWaitListed == 0) {
                                var notAttendedDates = "";
                                var ischecked = "";
                                $.each(config.Data.CourseDates, function (i, item) {
                                    $.each(row.AttendanceDateList, function (index, value) {
                                        if (config.Data.CourseDates[i].CourseDateItem == row.AttendanceDateList[index].Coursedate && row.AttendanceDateList[index].IsAttended == 0) {
                                            ischecked = "checked";
                                        }
                                    })
                                    notAttendedDates = notAttendedDates + "<div style='margin-top:3px'>" + '<input type="radio" onclick="AttendanceActions.SetSingleDateAttendance(' + row.RosterId + ',\'' + config.Data.CourseDates[i].CourseDateItem + '\',0)" name="' + row.RosterId + "_" + i + '" value="' + config.Data.CourseDates[i].CourseDateItem + '"  class="notattended-radio-item notattended-radio" ' + ischecked + '>' + "</div>";
                                    ischecked = "";
                                })
                                return notAttendedDates;
                            } else {

                                return "<div style='margin:6px; color: grey; font-weight:bold;'>WL</div>";
                            }
                        }

                },
                {
                    title: "Dates", data: 'AttendanceDateList', render: function (data, type, row) {
                        var listDate = "";

                        $.each(config.Data.CourseDates, function (i, item) {

                            listDate = listDate + "<div style='margin:6px'>" + (config.Data.CourseDates[i].CourseDateItem) + "</div>";
                        })
                        return listDate;
                    }

                },
                {
                    title: config.Data.FieldLabel.LabelCreditHours, data: 'CreditHours', render: function (data, type, row) {
                        var listtextfiled = "";
                        var indvalue = 0;
                        $.each(config.Data.CourseDates, function (i, item) {
                            $.each(row.AttendanceDateList, function (index, value) {
                                if (config.Data.CourseDates[i].CourseDateItem == row.AttendanceDateList[index].Coursedate) {
                                    indvalue = value.AttendedHours
                                }
                            })
                            listtextfiled = listtextfiled + '<div><input type="text" id=CreditHours"' + row.RosterId + "+" + i + '" value="' + indvalue + '" style="width:50px"></div>'
                            indvalue = 0;
                        });
                        listtextfiled = listtextfiled + '<div style="font-weight:bold"><hr><input type="text" id=CreditHours"' + row.RosterId + "+" + 'total' + '" value="' + data + '" style="width:50px"><br/>Total</div>';
                        return listtextfiled;
                    }

                },
                {
                    title: "Grade", data: 'Grade', render: function (data, type, row) {
                        if (data == "null" || data == null) {
                            data = 0;
                        }
                        return '<input type="text" id=Grade"' + row.RosterId + '" value="' + data + '" style="width:50px" onchange="AttendanceActions.SaveAttendanceGrade(' + row.RosterId + ',' + 'this) ">';
                    }

                },
                {
                    title: config.Data.FieldLabel.LabelCustomCredit 
                        + '<br /><div class="btn-group">'
                             + '<button type="button" class="btn-custom-attendance btn btn-secondary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">'
                                      + '<span class="dropdown-selected-label">0</span><span class="dropdown-selected-item"></span>'
                             + '</button>'
                             + '</button>'
                             + '<div class="attendance-custom-dropdown-menu dropdown-menu">'
                                    + '<div class="div-dropdown-menu-value">0</div>'
                                    + '<div class="div-dropdown-menu-value">1</div>'
                                    + '<div class="div-dropdown-menu-value">2</div>'
                             + ' </div>'
                        + '</div>'
                        , data: 'CustomCredit', render: function (data, type, row) {
                        return '<input type="text" id=CustomCredit"' + row.RosterId + '" value="' + data + '" style="width:50px">';
                    }

                },
                {
                    title: config.Data.FieldLabel.LabelInservice
                       + '<br /><div class="btn-group">'
                             + '<button type="button" class="btn-custom-attendance btn btn-secondary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">'
                                      + '<span class="dropdown-selected-label">0</span><span class="dropdown-selected-item"></span>'
                             + '</button>'
                             + '</button>'
                             + '<div class="attendance-custom-dropdown-menu dropdown-menu">'
                                    + '<div class="div-dropdown-menu-value">0</div>'
                                    + '<div class="div-dropdown-menu-value">1</div>'
                                    + '<div class="div-dropdown-menu-value">2</div>'
                             + ' </div>'
                        + '</div>'
                    , data: 'Inservice', render: function (data, type, row) {
                        return '<input type="text" id=Inservice"' + row.RosterId + '" value="' + data + '" style="width:50px">';
                    }

                },
                {
                    title: "Credit Type"+'<br /><div class="btn-group">'
                                + '<button type="button" class="btn-custom-attendance btn btn-secondary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">'
                                            + '<span class="dropdown-selected-label">NONE</span><span class="dropdown-selected-item"></span>'
                                + '</button>'
                                + '<div class="attendance-custom-dropdown-menu dropdown-menu">'
                                        + '<div class="div-dropdown-menu-marginated-left-head  div-dropdown-menu-value" onclick="' + AttendanceConfiguration.getCreditTypeDetails(data, 1) + '">NONE</div>'
                                        + '<div class="div-dropdown-menu-marginated-left-head  div-dropdown-menu-value" onclick="' + AttendanceConfiguration.getCreditTypeDetails(data, 4) + '">ALL</div>'
                                        + ' <div class="dropdown-divider"></div>'
                                        + ' <div class="div-dropdown-menu-marginated-left-head  custom-dropdown-item">CUSTOM</div>'
                                        + AttendanceConfiguration.getListOfSelectedCredits(data,0, selectedcredittypes, config)
                                + ' </div>'
                        + '</div>', data: 'SelectedCreditType', render: function (data, type, row) {

                        return '<div class="btn-group">'
                                + '<button type="button" class="btn-custom-attendance btn btn-secondary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">'
                                            + '<span class="dropdown-selected-label">NONE</span><span class="dropdown-selected-item"></span>'
                                + '</button>'
                                + '<div class="dropdown-menu">'
                                        + '<div class="div-dropdown-menu-marginated-left  div-dropdown-menu-value" onclick="' + AttendanceConfiguration.getCreditTypeDetails(data, 1) + '">NONE</div>'
                                        + '<div class="div-dropdown-menu-marginated-left  div-dropdown-menu-value" onclick="' + AttendanceConfiguration.getCreditTypeDetails(data, 4) + '">ALL</div>'
                                        + ' <div class="dropdown-divider"></div>'
                                        + ' <div class="div-dropdown-menu-marginated-left  custom-dropdown-item">CUSTOM</div>'
                                        + AttendanceConfiguration.getListOfSelectedCredits(data, row.Rosterid, selectedcredittypes, config)
                                + ' </div>'
                        + '</div>'
                    }

                },
                {
                    title: "Parking", data: 'Parking', render: function (data, type, row) {
                        return data;
                    }

                },
                {
                    title: " Additional Credit Type", render: function (data, type, row) {
                        return '<select>'
                               + '<option value="1"' + AttendanceConfiguration.getCreditTypeDetails(data, 1) + '>None</option>'
                               + ' <option value="2"' + AttendanceConfiguration.getAttendanceDetails(data, 2) + '>Custom</option>'
                               + '<option value="4"' + AttendanceConfiguration.getAttendanceDetails(data, 4) + '>ALL</option>'
                               + '<option value="5"' + AttendanceConfiguration.getAttendanceDetails(data, 5) + '>CEU CH</option>'
                               + '<option value="5"' + AttendanceConfiguration.getAttendanceDetails(data, 5) + '>Clock Hours</option>'
                               + '<option value="5"' + AttendanceConfiguration.getAttendanceDetails(data, 5) + '>Inservice</option>'
                               + '  </select>';
                    }

                },
                {
                    title: "Graduate"
                      + '<br /><div class="btn-group">'
                             + '<button type="button" class="btn-custom-attendance btn btn-secondary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">'
                                      + '<span class="dropdown-selected-label">0</span><span class="dropdown-selected-item"></span>'
                             + '</button>'
                             + '</button>'
                             + '<div class="attendance-custom-dropdown-menu dropdown-menu">'
                                    + '<div class="div-dropdown-menu-value">0</div>'
                                    + '<div class="div-dropdown-menu-value">1</div>'
                                    + '<div class="div-dropdown-menu-value">2</div>'
                             + ' </div>'
                        + '</div>'
                    , data: 'GraduateCredit', render: function (data, type, row) {
                        return '<input type="text" id=CreditHours"' + row.RosterId + '" value="' + data + '" style="width:50px">';
                    }

                },
                {
                    title: config.Data.FieldLabel.LabelCEUCredit
                        + '<br /><div class="btn-group">'
                             + '<button type="button" class="btn-custom-attendance btn btn-secondary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">'
                                      + '<span class="dropdown-selected-label">0</span><span class="dropdown-selected-item"></span>'
                             + '</button>'
                             + '</button>'
                             + '<div class="attendance-custom-dropdown-menu dropdown-menu">'
                                    + '<div class="div-dropdown-menu-value">0</div>'
                                    + '<div class="div-dropdown-menu-value">1</div>'
                                    + '<div class="div-dropdown-menu-value">2</div>'
                             + ' </div>'
                        + '</div>'
                    , data: 'CEUCredit', render: function (data, type, row) {
                        return '<input type="text" id=CreditHours"' + row.RosterId + '" value="' + data + '" style="width:50px">';
                    }

                },
                {
                    title: config.Data.FieldLabel.LabelOptionalCredit1
                        + '<br /><div class="btn-group">'
                             + '<button type="button" class="btn-custom-attendance btn btn-secondary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">'
                                      + '<span class="dropdown-selected-label">0</span><span class="dropdown-selected-item"></span>'
                             + '</button>'
                             + '</button>'
                             + '<div class="attendance-custom-dropdown-menu dropdown-menu">'
                                    + '<div class="div-dropdown-menu-value">0</div>'
                                    + '<div class="div-dropdown-menu-value">1</div>'
                                    + '<div class="div-dropdown-menu-value">2</div>'
                             + ' </div>'
                        + '</div>'
                   , data: 'OptionalCredit1', render: function (data, type, row) {
                        return '<input type="text" id=CreditHours"' + row.RosterId + '" value="' + data + '" style="width:50px">';
                    }

                },
                {
                    title: config.Data.FieldLabel.LabelOptionalCredit2
                        + '<br /><div class="btn-group">'
                             + '<button type="button" class="btn-custom-attendance btn btn-secondary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">'
                                      + '<span class="dropdown-selected-label">0</span><span class="dropdown-selected-item"></span>'
                             + '</button>'
                             + '</button>'
                             + '<div class="attendance-custom-dropdown-menu dropdown-menu">'
                                    + '<div class="div-dropdown-menu-value">0</div>'
                                    + '<div class="div-dropdown-menu-value">1</div>'
                                    + '<div class="div-dropdown-menu-value">2</div>'
                             + ' </div>'
                        + '</div>'
                    , data: 'OptionalCredit2', render: function (data, type, row) {
                        return '<input type="text" id=CreditHours"' + row.RosterId + '" value="' + data + '" style="width:50px">';
                    }

                },
                {
                    title: config.Data.FieldLabel.LabelOptionalCredit3 + '<br /><div class="btn-group">'
                             + '<button type="button" class="btn-custom-attendance btn btn-secondary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">'
                                      + '<span class="dropdown-selected-label">0</span><span class="dropdown-selected-item"></span>'
                             + '</button>'
                             + '</button>'
                             + '<div class="attendance-custom-dropdown-menu dropdown-menu">'
                                    + '<div class="div-dropdown-menu-value">0</div>'
                                    + '<div class="div-dropdown-menu-value">1</div>'
                                    + '<div class="div-dropdown-menu-value">2</div>'
                             + ' </div>'
                        + '</div>', data: 'OptionalCredit3', render: function (data, type, row) {
                        return '<input type="text" id=CreditHours"' + row.RosterId + '" value="' + data + '" style="width:50px">';
                    }

                },
                {
                    title: config.Data.FieldLabel.LabelOptionalCredit4 + '<br /><div class="btn-group">'
                             + '<button type="button" class="btn-custom-attendance btn btn-secondary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">'
                                      + '<span class="dropdown-selected-label">0</span><span class="dropdown-selected-item"></span>'
                             + '</button>'
                             + '</button>'
                             + '<div class="attendance-custom-dropdown-menu dropdown-menu">'
                                    + '<div class="div-dropdown-menu-value">0</div>'
                                    + '<div class="div-dropdown-menu-value">1</div>'
                                    + '<div class="div-dropdown-menu-value">2</div>'
                             + ' </div>'
                        + '</div>', data: 'OptionalCredit4', render: function (data, type, row) {
                        return '<input type="text" id=CreditHours"' + row.RosterId + '" value="' + data + '" style="width:50px">';
                    }

                },
                {
                    title: config.Data.FieldLabel.LabelOptionalCredit5 + '<br /><div class="btn-group">'
                             + '<button type="button" class="btn-custom-attendance btn btn-secondary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">'
                                      + '<span class="dropdown-selected-label">0</span><span class="dropdown-selected-item"></span>'
                             + '</button>'
                             + '</button>'
                             + '<div class="attendance-custom-dropdown-menu dropdown-menu">'
                                    + '<div class="div-dropdown-menu-value">0</div>'
                                    + '<div class="div-dropdown-menu-value">1</div>'
                                    + '<div class="div-dropdown-menu-value">2</div>'
                             + ' </div>'
                        + '</div>', data: 'OptionalCredit5', render: function (data, type, row) {
                        return '<input type="text" id=CreditHours"' + row.RosterId + '" value="' + data + '" style="width:50px">';
                    }

                },
                {
                    title: config.Data.FieldLabel.LabelOptionalCredit6 + '<br /><div class="btn-group">'
                             + '<button type="button" class="btn-custom-attendance btn btn-secondary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">'
                                      + '<span class="dropdown-selected-label">0</span><span class="dropdown-selected-item"></span>'
                             + '</button>'
                             + '</button>'
                             + '<div class="attendance-custom-dropdown-menu dropdown-menu">'
                                    + '<div class="div-dropdown-menu-value">0</div>'
                                    + '<div class="div-dropdown-menu-value">1</div>'
                                    + '<div class="div-dropdown-menu-value">2</div>'
                             + ' </div>'
                        + '</div>', data: 'OptionalCredit6', render: function (data, type, row) {
                        return '<input type="text" id=CreditHours"' + row.RosterId + '" value="' + data + '" style="width:50px">';
                    }

                },
                {
                    title: config.Data.FieldLabel.LabelOptionalCredit7 + '<br /><div class="btn-group">'
                             + '<button type="button" class="btn-custom-attendance btn btn-secondary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">'
                                      + '<span class="dropdown-selected-label">0</span><span class="dropdown-selected-item"></span>'
                             + '</button>'
                             + '</button>'
                             + '<div class="attendance-custom-dropdown-menu dropdown-menu">'
                                    + '<div class="div-dropdown-menu-value">0</div>'
                                    + '<div class="div-dropdown-menu-value">1</div>'
                                    + '<div class="div-dropdown-menu-value">2</div>'
                             + ' </div>'
                        + '</div>', data: 'OptionalCredit7', render: function (data, type, row) {
                        return '<input type="text" id=CreditHours"' + row.RosterId + '" value="' + data + '" style="width:50px">';
                    }

                },
                {
                    title: config.Data.FieldLabel.LabelOptionalCredit8 + '<br /><div class="btn-group">'
                             + '<button type="button" class="btn-custom-attendance btn btn-secondary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">'
                                      + '<span class="dropdown-selected-label">0</span><span class="dropdown-selected-item"></span>'
                             + '</button>'
                             + '</button>'
                             + '<div class="attendance-custom-dropdown-menu dropdown-menu">'
                                    + '<div class="div-dropdown-menu-value">0</div>'
                                    + '<div class="div-dropdown-menu-value">1</div>'
                                    + '<div class="div-dropdown-menu-value">2</div>'
                             + ' </div>'
                        + '</div>', data: 'OptionalCredit8', render: function (data, type, row) {
                        return '<input type="text" id=CreditHours"' + row.RosterId + '" value="' + data + '" style="width:50px">';
                    }

                },
                {
                    title: '', data: 'IsWaitListed'

                }
            ]

        });
    },
    getAttendanceDetails: function(value,selection){
        if(value == selection)
        {
            return "selected";
        }
    },
    getCreditTypeDetails: function (value, selection) {
        if (value == selection) {
            return "selected";
        }
    },
    getListOfSelectedCredits: function (data, Rosterid, selectedcredittypes, config) {
        result = "";
        $.each(selectedcredittypes, function (i, item) {
            if (item == "credithours") {
                result = result + '<div class="custom-dropdown-item-group"><input type="radio" name="" value="' + item + '"  class="selcted-customcredittype-radio"> ' + config.Data.FieldLabel.LabelCreditHours + '</div>';

            }
            else if (item == "customcredit") {
                result = result + '<div class="custom-dropdown-item-group"><input type="radio" name="" value="' + item + '"  class="selcted-customcredittype-radio"> ' + config.Data.FieldLabel.LabelCustomCredit + '</div>';

            }
            else if (item == "inservice") {
                result = result + '<div class="custom-dropdown-item-group"><input type="radio" name="" value="' + item + '"  class="selcted-customcredittype-radio"> ' + config.Data.FieldLabel.LabelInservice + '</div>';

            }
            else if (item == "ceu") {
                result = result + '<div class="custom-dropdown-item-group"><input type="radio" name="" value="' + item + '"  class="selcted-customcredittype-radio"> ' + config.Data.FieldLabel.LabelCEUCredit + '</div>';
                result = result + '<div class="custom-dropdown-item-group"><input type="radio" name="" value="' + item + '"  class="selcted-customcredittype-radio"> ' + 'Graduate' + '</div>';

            }
            else if (item == "optionalcredit1") {
                result = result + '<div class="custom-dropdown-item-group"><input type="radio" name="" value="' + item + '"  class="selcted-customcredittype-radio"> ' + config.Data.FieldLabel.LabelOptionalCredit1 + '</div>';

            }
            else if (item == "optionalcredit2") {
                result = result + '<div class="custom-dropdown-item-group"><input type="radio" name="" value="' + item + '"  class="selcted-customcredittype-radio"> ' + config.Data.FieldLabel.LabelOptionalCredit2 + '</div>';

            }
            else if (item == "optionalcredit3") {
                result = result + '<div class="custom-dropdown-item-group"><input type="radio" name="" value="' + item + '"  class="selcted-customcredittype-radio"> ' + config.Data.FieldLabel.LabelOptionalCredit3 + '</div>';

            }
            else if (item == "optionalcredit4") {
                result = result + '<div class="custom-dropdown-item-group"><input type="radio" name="" value="' + item + '"  class="selcted-customcredittype-radio"> ' + config.Data.FieldLabel.LabelOptionalCredit4 + '</div>';

            }
            else if (item == "optionalcredit5") {
                result = result + '<div class="custom-dropdown-item-group"><input type="radio" name="" value="' + item + '"  class="selcted-customcredittype-radio"> ' + config.Data.FieldLabel.LabelOptionalCredit5 + '</div>';

            }
            else if (item == "optionalcredit6") {
                result = result + '<div class="custom-dropdown-item-group"><input type="radio" name="" value="' + item + '"  class="selcted-customcredittype-radio"> ' + config.Data.FieldLabel.LabelOptionalCredit6 + '</div>';

            }
            else if (item == "optionalcredit7") {
                result = result + '<div class="custom-dropdown-item-group"><input type="radio" name="" value="' + item + '"  class="selcted-customcredittype-radio"> ' + config.Data.FieldLabel.LabelOptionalCredit7 + '</div>';

            }
            else if (item == "optionalcredit8") {
                result = result + '<div class="custom-dropdown-item-group"><input type="radio" name="" value="' + item + '"  class="selcted-customcredittype-radio"> ' + config.Data.FieldLabel.LabelOptionalCredit8 + '</div>';

            }
        });
        return result;
    },
    getListOfDatesForHeader: function (append,config) {
        var result = "";
        if (append == "Not") {
            $.each(config.Data.CourseDates, function (index, value) {
                result = result + '<div class="custom-dropdown-item-group"><input type="radio" name="" value="' + append + '_' + config.Data.CourseDates[index].CourseDateItem + '"  class="head-notattended-radio"> ' + config.Data.CourseDates[index].CourseDateItem + '</div>';
            })
        }
        else {
            $.each(config.Data.CourseDates, function (index, value) {
                result = result + '<div class="custom-dropdown-item-group"><input type="radio" name="" value="' + config.Data.CourseDates[index].CourseDateItem + '"  class="head-notattended-radio"> ' + config.Data.CourseDates[index].CourseDateItem + '</div>';
            })
        }
        return result;
    },

    Actions: function () {
        var selectedactionvalue = $("input[name=attendance-action]:checked").val();
        if (selectedactionvalue == "checkedonly") {
            $('input:checkbox.Transcribe-checkbox').each(function () {
                var sThisVal = (this.checked ? $(this).val() : "");
                if (sThisVal != "") {
                    AttendanceActions.TranscribeSingleStudent(sThisVal);
                }

            });
        }
        else if (selectedactionvalue == "all") {
            var sThisVal = $("#courseidvalue").val();
            if (sThisVal != "") {
                AttendanceActions.TranscribeAllStudents(sThisVal);
            }
        }

    },
    requestCourseDetails: function () {
        gsmuUIObject.mask("#layout-content-container");
        var courseId = UrlHelper.getUrlVars()["courseId"];
         $.ajax({
            type: "GET",
            dataType: 'json',
            url: gsmuObject.apiUrl + '/CourseAttendanceTaking/GetCourseDetails?courseid=' + courseId,
            success: function (data) {
                var header = data.Data.CourseBasicDetails.CourseId + " - " + data.Data.CourseBasicDetails.CourseNumber + " - " + data.Data.CourseBasicDetails.CourseName + " " + data.Data.CourseDates[0].CourseDateItem
                var config = data;
                $("#courseidvalue").val(data.Data.CourseBasicDetails.CourseId);
                var AttendanceList = AttendanceConfiguration.requestRostersList();
                AttendanceList.done(function (response) {
                    AttendanceConfiguration.initUI(header, response, config);
                    gsmuUIObject.unmask("#layout-content-container");
                });
            }
        });
    },
    requestRostersList: function () {
        var courseId = UrlHelper.getUrlVars()["courseId"];
        return $.ajax({
            type: "GET",
            dataType: 'json',
            url: gsmuObject.apiUrl + '/CourseAttendanceTaking/GetRosterList?courseid=' + courseId
        });
    },
    initConfiguration: function () {
        AttendanceConfiguration.requestCourseDetails();
    },
}

$(document).ready(function () {
    var courseId = UrlHelper.getUrlVars()["courseId"];
        if (courseId > 0) {
            AttendanceConfiguration.initConfiguration();
        }
    setTimeout(function () {
        $('#admin-header, #admin-menu').hide('fast');
    }, 300)

});
