var supervisorStudentsWaitList = function () {
    function loadData() {
        var courseId = UrlHelper.getUrlVars()["courseId"];
        var getStudents = function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: '/public/Supervisor/WaitingStudentsData',
            });
        }
        return getStudents();
    }

    function loadApprovedData() {
        var courseId = UrlHelper.getUrlVars()["courseId"];
        var getStudents = function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: '/public/Supervisor/ApprovedListStudentsData',
            });
        }
        return getStudents();
    }

    function approveEnrollment(rosterid, send) {
        return $.ajax({
            type: 'POST',
            dataType: 'json',
            data: { 'rosterid': rosterid, 'sendEmail': send },
            url: '/public/Supervisor/ApproveWaitingStudent'
        })
    }

    function moveToApproveToWait(rosterid) {
        return $.ajax({
            type: 'POST',
            dataType: 'json',
            data: { 'rosterid': rosterid },
            url: '/public/Supervisor/MoveToApproveToWaitStudent'
        })
    }

    function cancelRoster(rosterId) {
        return $.ajax({
            type: 'POST',
            dataType: 'json',
            data: { 'rosterid': rosterId },
            url: '/public/Course/CancelRoster'
        })
    }
    return {
        allButtons: $('button'),
        exportButton: null,
        approvedExportButton: null,
        approveToWaitlistData: [],
        approvedListData: [],
        initUI: function () {
            loadData().done(function (data) {
                supervisorStudentsWaitList.initGrid(data);
                supervisorStudentsWaitList.approveToWaitlistData = data;
            });
            loadApprovedData().done(function (data) {
                supervisorStudentsWaitList.initApprovedGrid(data);
                supervisorStudentsWaitList.approvedListData = data;
            });

        },
        initGrid: function (data) {
            $('#supervisor-student-waitlist-grid').DataTable({
                responsive: true,
                autoWitdh: false,
                data: data,
                destroy: true,
                "columnDefs": [
                    { "width": "15%", "targets": 6 }, { "width": "20%", "targets": 1 }
                ],
                "order": [],
                fixedHeader: true,
                scrollY: "300px",
                columns: [
                    {
                        title: "Course #", data: 'CourseNumber'
                    },
                    {
                        title: "Course Name", data: 'CourseName', render: function (data, type, row) {
                            return data;
                        },
                    },
                    {
                        title: "Start Date", data: 'CourseStartDate', render: function (data, type, row) {
                            return moment(data).format('MM/DD/YYYY');
                        }
                    },
                    {
                        title: "Statistics", data: 'CourseId', render: function (data, type, row) {
                            var maxEnroll = row.MaxEnroll;
                            var maxWait = row.MaxWait;
                            var enrollCount = row.EnrolledCount;
                            var waitCount = row.WaitingCount;
                            var remainingSlots = row.RemainingSlots;
                            var remainingWaitingSlots = row.RemainingWaitSlots;
                            var template = '<div class="row">Available Seats : <span class="badge">' + remainingSlots
                                + '</span> <br /> <a href="javascript:void(0)" onClick="supervisorStudentsWaitList.showStatDetails(' + maxEnroll + ',' + maxWait + ',' + enrollCount + ',' + waitCount + ',' + remainingSlots + ',' + remainingWaitingSlots + ')">Details</button>'
                            return template;
                        }
                    },
                    {
                        title: "Name", data: 'StudentFirstName', render: function (data, type, row) {
                            return data + ' ' + row.StudentLastName
                        }
                    },
                    {
                        title: "Date Added", data: 'DateAdded', render: function (data, type, row) {
                            return moment(data).format('MM/DD/YYYY');
                        }
                    },
                    {
                        title: "Action", data: 'RosterId', className: 'center', render: function (data, type, row) {
                            var remainingSlots = row.RemainingSlots;
                            var remainingWaitingSlots = row.RemainingWaitSlots;
                            var actionTemplate = '';
                            var rosterId = row.RosterId;
                            var cancellable = row.Cancellable;

                            var cancelTemplate = cancellable ? '<div class="or or-sm"></div>' + '<button type="button" data-toggle="tooltip" data-placement="top" title="Click to Deny" class="btn btn-sm btn-danger" onClick="supervisorStudentsWaitList.cancelRoster(' + data + ')">&nbsp;<i class="glyphicon glyphicon-thumbs-down"></i> </button>' : '';

                            if (remainingSlots > 0) {
                                actionTemplate = '<button type="button" data-toggle="tooltip" data-placement="top" title="Click to Approve" class="btn btn-sm btn-success" onClick="supervisorStudentsWaitList.enrollRoster(' + data + ')">&nbsp;<i class="glyphicon glyphicon-thumbs-up"></i> </button>';
                            }
                            else {
                                actionTemplate = '<button type="button" data-toggle="tooltip" data-placement="top" title="Click to Approve" class="btn btn-sm btn-success" onClick="supervisorStudentsWaitList.moveToWait(' + data + ')">&nbsp;<i class="glyphicon glyphicon-thumbs-up"></i> </button>';
                            }
                            var finalTemp = '<div class="ui-group-buttons">' + actionTemplate + cancelTemplate + '</div>';
                            return finalTemp;
                        }
                    }
                ],
                "dom": '<"top"f>rt<"bottom"ilp><"clear">',
                "fnDrawCallback": function () {
                    $('#students-waitlist-grid-container').find('.dataTables_info, .dataTables_length, .dataTables_paginate').addClass('col-sm-12');
                    //exchange positions
                    var exportButton = $('#waitlist-export').detach();
                    if (exportButton.length > 0) {
                        supervisorStudentsWaitList.exportButton = exportButton;
                    }
                    else {
                        exportButton = supervisorStudentsWaitList.exportButton
                    }


                    var gridLength = $('#students-waitlist-grid-container').find('.dataTables_length').detach();
                    var gridPagination = $('#students-waitlist-grid-container').find('.dataTables_paginate').detach();
                    $('#students-waitlist-grid-container').find('.bottom').append(gridPagination);
                    $('#students-waitlist-grid-container').find('.bottom').append(gridLength);

                    $('#students-waitlist-grid-container').find('.dataTables_filter').append(exportButton);
                }
            });
        },
        initApprovedGrid: function (data) {
            $('#supervisor-student-approved-grid').DataTable({
                responsive: true,
                autoWitdh: false,
                data: data,
                destroy: true,
                "order": [],
                columns: [
                    {
                        title: "Course #", data: 'CourseNumber'
                    },
                    {
                        title: "Course Name", data: 'CourseName', render: function (data, type, row) {
                            return data;
                        },
                    },
                    {
                        title: "Start Date", data: 'CourseStartDate', render: function (data, type, row) {
                            return moment(data).format('MM/DD/YYYY');
                        }
                    },
                    {
                        title: "Statistics", data: 'CourseId', render: function (data, type, row) {
                            var maxEnroll = row.MaxEnroll;
                            var maxWait = row.MaxWait;
                            var enrollCount = row.EnrolledCount;
                            var waitCount = row.WaitingCount;
                            var remainingSlots = row.RemainingSlots;
                            var remainingWaitingSlots = row.RemainingWaitSlots;
                            var template = '<div class="row">Available Seats : <span class="badge">' + remainingSlots
                                + '</span> <br /> <a href="javascript:void(0)" onClick="supervisorStudentsWaitList.showStatDetails(' + maxEnroll + ',' + maxWait + ',' + enrollCount + ',' + waitCount + ',' + remainingSlots + ',' + remainingWaitingSlots + ')">Details</button>'
                            return template;
                        }
                    },
                    {
                        title: "Name", data: 'StudentFirstName', render: function (data, type, row) {
                            return data + ' ' + row.StudentLastName
                        }
                    },
                    {
                        title: "Date Added", data: 'DateAdded', render: function (data, type, row) {
                            return moment(data).format('MM/DD/YYYY');
                        }
                    }
                ],
                "dom": '<"top"f>rt<"bottom"ilp><"clear">',
                "fnDrawCallback": function () {
                    $('#students-approved-grid-container').find('.dataTables_info, .dataTables_length, .dataTables_paginate').addClass('col-sm-12');
                    //exchange positions
                    //var exportButton = $('#approved-export').detach();
                    //if (exportButton.length > 0) {
                    //    supervisorStudentsWaitList.approvedExportButton = exportButton;
                    //}
                    //else {
                    //    exportButton = supervisorStudentsWaitList.approvedExportButton;
                    //}

                    var gridLength = $('#students-approved-grid-container').find('.dataTables_length').detach();
                    var gridPagination = $('#students-approved-grid-container').find('.dataTables_paginate').detach();
                    $('#students-approved-grid-container').find('.bottom').append(gridPagination);
                    $('#students-approved-grid-container').find('.bottom').append(gridLength);

                    //$('#students-approved-grid-container').find('.dataTables_filter').append(exportButton);
                }
            });
        },
        showStatDetails: function (maxEnroll, maxWait, enrollCount, waitCount, remainingSlots, remainingWaitingSlots) {
            var template =
                '<table style="font-size: 12px;"><tr><td>Max Enroll Allowed</td><td style="text-align:right;width:55%;"><span class="badge">' + maxEnroll + '</span></td><tr>';
            template += '<tr><td>Currently Enrolled</td><td style="text-align:right;width:55%;"><span class="badge">' + enrollCount + '</span></td><tr>';
            template += '<tr><td>Available Seats</td><td style="text-align:right;width:55%;"><span class="badge">' + remainingSlots + '</span></td><tr>';
            template += '<tr><td>Max Waitlist Allowed</td><td style="text-align:right;width:55%;"><span class="badge">' + maxWait + '</span></td><tr>';
            template += '<tr><td>Currently on Wait List</td><td style="text-align:right;width:55%;"><span class="badge">' + waitCount + '</span></td><tr>';
            template += '<tr><td>Available Wait Seats</td><td style="text-align:right;width:55%;"><span class="badge">' + remainingWaitingSlots + '</span></td><tr>';

            bootbox.dialog({
                title: 'Information',
                size: 'small',
                message: template
            });
        },
        enrollRoster: function (rosterid) {
            bootbox.dialog({
                title: 'Confirmation',
                size: "medium",
                message: "Are you sure you wanted to enroll this student?",
                buttons: {
                    enrollEmail: {
                        label: "Enroll and Email",
                        className: 'btn-success btn-sm',
                        callback: function () {
                            $($('#supervisor-student-waitlist-grid_filter')[0]).append('<h2 id="loading-text"  style="text-align:center">Loading....</h2>')
                            approveEnrollment(rosterid, true).done(function (response) {
                                var data = JSON.parse(response);
                                if (data.Success) {
                                    parent.$.jGrowl('You have Approved the student and they are now enrolled in the course.', { theme: 'successGrowl', themeState: '' });
                                }
                                else {
                                    parent.$.jGrowl('Something went wrong', { theme: 'errorGrowl', themeState: '' });
                                }
                                supervisorStudentsWaitList.initUI();
                            });
                        }
                    },
                    enroll: {
                        label: "Enroll",
                        className: 'btn-primary btn-sm',
                        callback: function () {
                            $($('#supervisor-student-waitlist-grid_filter')[0]).append('<h2 id="loading-text" style="text-align:center">Loading....</h2>')
                            approveEnrollment(rosterid, false).done(function (response) {
                                var data = JSON.parse(response);
                                if (data.Success) {
                                    parent.$.jGrowl('You have approved the student and they are now on the waiting list.', { theme: 'successGrowl', themeState: '' });
                                }
                                else {
                                    parent.$.jGrowl('Something went wrong', { theme: 'errorGrowl', themeState: '' });
                                }
                                supervisorStudentsWaitList.initUI();
                            });

                        }
                    },
                    close: {
                        label: "Close",
                        className: 'btn-default btn-sm',
                        callback: function () {

                        }
                    }
                }

            });
        },
        cancelRoster: function (rosterid) {
            bootbox.dialog({
                title: 'Confirmation',
                size: "medium",
                message: "Are you sure you want to deny this registration?",
                buttons: {
                    cancel: {
                        label: "Cancel Roster",
                        className: 'btn-sm btn-danger',
                        callback: function () {
                            $($('#supervisor-student-waitlist-grid_filter')[0]).append('<h2 id="loading-text"  style="text-align:center">Loading....</h2>')
                            cancelRoster(rosterid).done(function (response) {
                                var data = JSON.parse(response);
                                if (data) {
                                    parent.$.jGrowl('You have Cancelled the student.', { theme: 'successGrowl', themeState: '' });
                                }
                                else {
                                    parent.$.jGrowl('Something went wrong', { theme: 'errorGrowl', themeState: '' });
                                }
                                supervisorStudentsWaitList.initUI();
                            });
                        }
                    },
                    close: {
                        label: "Close",
                        className: 'btn-default btn-sm',
                        callback: function () {

                        }
                    }
                }
            });
        },
        moveToWait: function (rosterid) {
            bootbox.dialog({
                title: 'Confirmation',
                size: "medium",
                message: "There are currently no available seats for enrollment but you can approve the student and they will be moved when space is available. Are you sure you want to approve this student?",
                buttons: {
                    move: {
                        label: "Move",
                        className: 'btn-primary btn-sm',
                        callback: function () {
                            $($('#supervisor-student-waitlist-grid_filter')[0]).append('<h2 id="loading-text"  style="text-align:center">Loading....</h2>')
                            moveToApproveToWait(rosterid, false).done(function (response) {
                                var data = JSON.parse(response);
                                if (data.Success) {
                                    parent.$.jGrowl('Successfully Moved Student To Wait for Approval List.', { theme: 'successGrowl', themeState: '' });
                                }
                                else {
                                    parent.$.jGrowl('Something went wrong', { theme: 'errorGrowl', themeState: '' });
                                }
                                supervisorStudentsWaitList.initUI();
                            });

                        }
                    },
                    close: {
                        label: "Close",
                        className: 'btn-default btn-sm',
                        callback: function () {

                        }
                    }
                }

            });
        },
        exportApproveToWaitlistToExcel: function () {
            var data = supervisorStudentsWaitList.approveToWaitlistData.map(function (item) {
                return {
                    //'Student Id': item.StudentId,
                    'First Name': item.StudentFirstName,
                    'Last Name': item.StudentLastName,
                    'Email': item.Email,
                    //'Roster Id': item.RosterId,
                    'Course Id': item.CourseId,
                    'Course Name': item.CourseName,
                    'Course Number': item.CourseNumber,
                    'Course Start Date': moment(item.CourseStartDate).format('MM/DD/YYYY'),
                    'Date Added': moment(item.DateAdded).format('MM/DD/YYYY')
                }
            });
            util.jsonToCSV(data, "Waitlist to Approve Report", true);
        },
        exportApprovedlistToExcel: function () {
            var data = supervisorStudentsWaitList.approvedListData.map(function (item) {
                return {
                    //'Student Id': item.StudentId,
                    'First Name': item.StudentFirstName,
                    'Last Name': item.StudentLastName,
                    'Email': item.Email,
                    //'Roster Id': item.RosterId,
                    'Course Id': item.CourseId,
                    'Course Name': item.CourseName,
                    'Course Number': item.CourseNumber,
                    'Course Start Date': moment(item.CourseStartDate).format('MM/DD/YYYY'),
                    'Date Added': moment(item.DateAdded).format('MM/DD/YYYY')
                }
            });
            util.jsonToCSV(data, "Approved Records Report", true);
        },
    }
}();

$(document).ready(function () {
    supervisorStudentsWaitList.initUI();
})