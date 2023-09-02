var ReportsPaymentClassList = (function () {
    var courseId = UrlHelper.getUrlVars()["courseId"];
    var classListObjects = {
        asPage : false
    }
    function getReportsPaymentClassListRequest() {
        return $.ajax({
            type: "GET",
            dataType: 'json',
            url: gsmuObject.apiUrl + '/Reports/GetPaymentClassListReports?courseId=' + courseId
        });
    }

    return {
        modal: $('#reports-payment-classlist-modal'),
        modalContent: $('#reports-payment-classlist-modal .modal-content'),
        init: function () {
            gsmuUIObject.mask('#reports-payment-classlist');
            getReportsPaymentClassListRequest().done(function (response) {
                if (response.Success === 1) {
                    var data = response.Data;
                    ReportsPaymentClassList.Data = data;
                    var dataTable = $('#reports-payment-classlist-grid').DataTable({
                        destroy: true,
                        select: {
                            style: 'single'
                        },
                        data: data,
                        columns: [
                            {
                                data: 'StudentId', 'visible': false
                            },
                            {
                                data: 'RosterId', 'visible': false
                            },
                            {
                                data: 'OrderNumber', 'visible': false
                            },
                            {
                                title: "First Name", data: 'FirstName'
                            },
                            { title: "Last Name", data: 'LastName' },
                            {
                                title: "Paid &nbsp;&nbsp;<input type='checkbox' data-size='mini' id='report-paid-all'>", data: 'PaidInFull', render: function (value, type, row, rowIndex) {
                                    var paymentMade = row.CRPartialPaymentMade == 1 ? 'Partial' : row.CRPartialPaymentMade == 2 ? 'Full' : 'N/A';
                                    var paymentMethod = row.CRPartialPaymentPaidMethod;
                                    var paymentAmount = row.CRPartialPaymentPaidAmount;
                                    var paymentNotes = row.CRPartialPaymentPaidNote;

                                    var paidInFull = row.PaidInFull;
                                    var orderNumber = row.OrderNumber;

                                    var paymentTemplate = '<br /><div class="reports-payment-partial-detail-section">Payment : ' + paymentMade + '<br />' + 'Method : ' + (paymentMethod ? paymentMethod : 'N/A') + '<br />';
                                    paymentTemplate += 'Amount Paid : $ ' + paymentAmount.toFixed(2);
                                    paymentTemplate += paymentNotes ? '<br /> Notes : ' + paymentNotes + '' : '';
                                    paymentTemplate += '<br /> <button type="button" onClick=ReportsPaymentClassList.openEditPartialPage("' + orderNumber + '") class="btn btn-warning btn-xs">Edit Partial &nbsp;<span class="fa fa-pencil"></span></button> </div>' ;
                                    var template = '<div><input type="checkbox" data-size="mini" class="toggle" ' + (value !== 0 ? 'checked="checked"' : '') + ' /></div>'
                                    return template + (value === 0 ? paymentTemplate : '<br />Paid In Full');
                                }
                            },
                            {
                                title: "Course Cost", data: 'CourseCost', 'visible': false, render: function (value, type, row, rowIndex) {
                                    value = value ? value.toFixed(2) : '0.00';
                                    var orderNumber = row.OrderNumber;
                                    var template = '<a href="#" onClick=onClick=ReportsPaymentClassList.openEditPartialPage("' + orderNumber + '")>$ ' + value + '</a>';
                                    return template;
                                }
                            },
                            {
                                title: "Materials Cost", data: 'MaterialCost', render: function (value, type, row, rowIndex) {
                                    value = value ? value.toFixed(2) : '0.00';
                                    var orderNumber = row.OrderNumber;
                                    var template = '<a href="#" onClick=onClick=ReportsPaymentClassList.openEditPartialPage("' + orderNumber + '")>$ ' + value + '</a>';
                                    return template;
                                }
                            },
                            {
                                title: "Course Total", data: 'CourseTotal', render: function (value, type, row, rowIndex) {
                                    value = value ? value.toFixed(2) : '0.00';
                                    var orderNumber = row.OrderNumber;
                                    var template = '<a href="#" onClick=onClick=ReportsPaymentClassList.openEditPartialPage("' + orderNumber + '")>$ ' + value + '</a>';
                                    return template;
                                }
                            },
                            {
                                title: "Order Total", data: 'CourseTotal', render: function (value, type, row, rowIndex) {
                                    value = value ? value.toFixed(2) : '0.00';
                                    var orderNumber = row.OrderNumber;
                                    var template = '<a href="#" onClick=onClick=ReportsPaymentClassList.openEditPartialPage("' + orderNumber + '")>$ ' + value + '</a>';
                                    return template;
                                }
                            },
                            {
                                title: "Amount Paid", data: 'TotalPaid', render: function (value, type, row, rowIndex) {
                                    value = value ? value.toFixed(2) : '0.00';
                                    var orderNumber = row.OrderNumber;
                                    var template = '<a href="#" onClick=onClick=ReportsPaymentClassList.openEditPartialPage("' + orderNumber + '")>$ ' + value + '</a>';
                                    return template;
                                }
                            },
                            {
                                title: "Payment Method", data: 'PayMethod', render: function (value, type, row, rowIndex) {
                                    var courseId = row.CourseId;
                                    var studentId = row.StudentId;
                                    var rosterId = row.RosterId;

                                    var orderNumber = row.OrderNumber;
                                    var template = value !== '' && value ? '<div><b>' + value + '</b>' : 'N / A';
                                    template += '<button type="button" onClick="ReportsPaymentClassList.openPaymentMethodDetails(' + courseId + ',' + studentId + ',' + rosterId + ')" class="btn btn-success btn-xs pull-right">Details</button></div>';
                                    template += '<div>Order # : </div><div><a href="#" onClick=onClick=ReportsPaymentClassList.openEditPartialPage("' + orderNumber + '")><b>' + row.OrderNumber + '<b/></a></div>';
                                    return template;
                                }
                            },
                            {
                                title: "Notes", data: 'Notes', render: function (value, type, row, rowIndex) {
                                    value = value == null ? '' : '<div class="pad-top-10 gsmu-grid-notes">' + value + '</div>';
                                    var studentId = row.StudentId;
                                    var template = '<button type="button" onclick="ReportsPaymentClassList.openEditNotes(' + studentId + ')" class="btn btn-warning btn-xs"> Edit &nbsp;<span class="fa fa-pencil"></span></button>';
                                    return template + value;
                                }
                            }
                        ],
                        "order": [[1, 'asc']],
                        paging: false,
                        //"scrollY": "500px",
                        //"scrollCollapse": true,
                    });
                    gsmuUIObject.unmask('#reports-payment-classlist');
                    setTimeout(function () {
                        ReportsPaymentClassList.initGridToggle();
                        gsmuUIObject.unmask('#reports-payment-classlist');
                    }, 1000);
                    ReportsPaymentClassList.DataTable = dataTable;
                }
            });
            ReportsPaymentClassList.CourseId = courseId;
            ReportsPaymentClassList.initSettings();
        },
        initGridToggle: function () {
            $('.toggle').bootstrapToggle({
                on: 'Yes',
                off: 'No',
                size: 'mini'
            }).unbind('change');

            $('#report-paid-all').bootstrapToggle({
                on: 'Paid All',
                off: 'Unpaid All'
            }).unbind('change');
        },
        openStudentPage: function (studentId) {
            window.open(gsmuObject.adminUrl + 'students_edit.asp.asp?sid=' + studentId, '_blank');
        },
        openEditPartialPage: function (orderId) {
            if (classListObjects.asPage) {
                window.open(gsmuObject.adminUrl + 'reports_editOrder.asp?id=' + orderId, '_blank');
            }
            else {
                gsmuUIObject.mask('#reports-payment-classlist');
                $.ajax({
                    url: '/Adm/Reports/ReportsOrderDetail?orderNumber=' + orderId,
                    dataType: 'html',
                    success: function (html) {
                        $(ReportsPaymentClassList.modalContent).html(html);
                        $(ReportsPaymentClassList.modal).modal('show');
                        $(ReportsPaymentClassList.modalContent).resizable({
                            minHeight: 300,
                            minWidth: 400
                        }).draggable();
                        $.when(ReportsOrderDetail.loadOrderDetail(orderId)).then(function () {
                            gsmuUIObject.unmask('#reports-payment-classlist');
                        });
                    }
                });
            }
            
            
        },
        openEditNotes: function (studentid) {
            gsmuUIObject.mask('#reports-payment-classlist');
            $.ajax({
                url: '/Adm/Students/StudentNotes',
                dataType: 'html',
                success: function (html) {
                    $('#reports-payment-classlist-modal .modal-content').html(html);
                    $('#reports-payment-classlist-modal').modal('show');
                    $.when(StudentsNotes.loadNote(studentid)).then(function () {
                        gsmuUIObject.unmask('#reports-payment-classlist');
                    });
                    
                }
                
            })
        },
        openPaymentMethodDetails: function (courseId, studentId, rosterid) {
            window.open(gsmuObject.adminUrl + 'payment_edit.asp?cid=' + courseId + '&sid=' + studentId + '&rosterid=' + rosterid + '', '_blank');
        },
        exportToExcel: function () {
            util.jsonToCSV(ReportsPaymentClassList.Data, "Reports Classlist - Course : ( " + ReportsPaymentClassList.CourseId + " )", true);
        },
        exportToPDF: function () {
            var columns = [
                //{ title: "Roster ID", dataKey: "Rosterid" },
                { title: "Name", dataKey: "FullName" },
                { title: "Paid", dataKey: "PaidInFull" },
                { title: "Course Cost", dataKey: "CourseCost" },
                { title: "Material Cost", dataKey: "MaterialCost" },
                { title: "Total Paid", dataKey: "TotalPaid" },
                { title: "Pay Method", dataKey: "PayMethod" },
                { title: "Order#", dataKey: "OrderNumber" },
            ];
            var finalData = ReportsPaymentClassList.Data.map(function (item) {
                return {
                    FullName: item.FirstName + ' ' + item.LastName,
                    PaidInFull: item.PaidInFull === 0 ? 'No' : 'Yes',
                    CourseCost: '$ ' + item.CourseCost.toFixed(2),
                    MaterialCost: '$ ' + item.MaterialCost.toFixed(2),
                    TotalPaid: '$ ' + item.MaterialCost.toFixed(2),
                    PayMethod: item.PayMethod,
                    OrderNumber: item.OrderNumber
                }
            })
            var doc = new jsPDF('p', 'pt');
            doc.autoTable(columns, finalData, {
                addPageContent: function (data) {
                    doc.text("Reports Classlist - Course : ( " + ReportsPaymentClassList.CourseId + " )", 40, 30);
                }
            });
            doc.save('Reports Payment Class-list ' + ReportsPaymentClassList.CourseId  + '.pdf');
        },
        closeStudentNotes: function () {
            $('#reports-payment-classlist-modal').modal('hide');
        },
        initSettings: function () {
            var openDetailPage = localStorage.getItem('gsmuOpenOrderDetailAsPage');
            if (openDetailPage) {
                classListObjects.asPage = JSON.parse(openDetailPage); 
                $('.btn.open')
                    .attr('data-value', JSON.parse(openDetailPage))
                    .addClass(JSON.parse(openDetailPage) ? 'toggledDefaultButton' : '');
            }
            $('<a name="top"/>').insertBefore($('body').children().eq(0));
            window.location.hash = 'top';
        },
        openAsPage: function (btn) {
            var btnValue = $(btn).attr('data-value');
            if (btnValue === 'false') {
                classListObjects.asPage = true;
                localStorage.setItem('gsmuOpenOrderDetailAsPage', true);
                $(btn).addClass('toggledDefaultButton').attr('data-value', true);
            }
            else {
                classListObjects.asPage = false;
                localStorage.setItem('gsmuOpenOrderDetailAsPage', false);
                $(btn).removeClass('toggledDefaultButton').attr('data-value', false);
            }
        },
        CourseId: 0,
        DataTable: null,
        Data: []
    };
})();

$(document).ready(function () {
    ReportsPaymentClassList.init();
});