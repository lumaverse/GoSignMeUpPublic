var ReportsOrderDetail = (function () {
    var template = $('#reports-order-detail-template').html();
    Mustache.parse(template); 

    function getReportsOrderDetail(orderNumber) {
        return $.ajax({
            type: "GET",
            dataType: 'json',
            url: gsmuObject.apiUrl + 'Reports/GetOrderDetailReport?orderNumber=' + orderNumber
        });
    }

    function saveOrder() {
        
    }

    return {
        loadOrderData: function (orderNumber) {
            return new Promise(resolve => {
                getReportsOrderDetail(orderNumber).done(function (data) {
                    var finalData = [];
                    var data = data.Data;
                    var rosterMaterials = data.RosterMaterialsModel;
                    var rosterCourse = data.RosterCoursesModel;
                    var revisedMaterialData = {};

                    if (rosterMaterials)
                    {
                        revisedMaterialData = rosterMaterials.map(function (item) {
                            return {
                                Id: item.ProductId,
                                Type: 'Material',
                                Name: item.ProductName,
                                Cost: '$ ' + item.ProductCost.toFixed(2),
                                QtyPurchase: item.QtyPurchase == 0 ? 1 : item.QtyPurchase
                            };
                        });
                    }
                    
                    var revisedCourseData = rosterCourse.map(function (item) {
                        return {
                            Id: item.CourseId,
                            Type: 'Course',
                            Name: item.CourseNumber + ' - ' + item.CourseName,
                            Cost: item.CourseCost,
                            QtyPurchase: 1
                        };
                    });
                    finalData = revisedMaterialData.concat(revisedCourseData);
                    ReportsOrderDetail.itemsData = finalData;
                    resolve(data, finalData);
                });
            });
        },
        loadOrderDetail: function (orderNumber) {
            ReportsOrderDetail.loadOrderData(orderNumber)
                .then(function (data) {
                    var color = 'transparent';
                    if (data.Status == 'Approved'){
                        color = 'green';
                    }
                    else if (data.Status == 'Pending') {
                        color = '#66ffff';
                    }
                    else {
                        color = 'pink';
                    }
                    var rendered = Mustache.render(template,
                        {
                            Color: color,
                            Status: data.Status,
                            OrderNumber: data.OrderNumber,
                            StudentName: data.FistName + ' ' + data.LastName,
                            HomePhone: data.StudentAddressModel.HomePhone,
                            WorkPhone: data.StudentAddressModel.WorkPhone,
                            Fax: data.StudentAddressModel.Fax,
                            Email: data.StudentAddressModel.Email,
                            Grade: data.GradeLevel,
                            District: data.District,
                            Gender: 'Male',
                            Race: 'Hispanic',
                            Department: 'Education',
                            OrderDate: moment(data.DateAdded).format('MM/DD/YYYY') + ' ' + moment(data.TimeAdded).format('hh:mm a')
                        });
                    $('.loader').html(rendered);
                    ReportsOrderDetail.loadRosterCourseAndMaterialsToGrid(ReportsOrderDetail.itemsData);
                });
        },
        loadRosterCourseAndMaterialsToGrid: function (data) {
            var dataTable = $('#order-detail-order-items').DataTable({
                destroy: true,
                paging: false,
                responsive: true,
                autoWitdh: false,
                'searching': false,
                "columnDefs": [
                    { "width": "20%", "targets": 0 },
                    { "width": "20%", "targets": 1 },
                    { "width": "20%", "targets": 2 },
                    { "width": "20%", "targets": 3 },
                    { "width": "20%", "targets": 4 },
                    { "width": "20%", "targets": 5 }
                ],
                select: {
                    style: 'single'
                },
                data: data,
                columns: [
                    {
                        data: 'Name', title: 'Course / Product', render: function (value, type, row, rowIndex) {
                            var itemId = row.Id;
                            var itemType = row.Type;
                            return '<a href="#"> ' + value + ' </a>'
                        }
                    },
                    { 
                        data: 'Type', title : 'Type'
                    },
                    {
                        data: 'Cost', title: 'Cost'
                    },
                    {
                        data: 'QtyPurchase', title: 'Quantity Purchased'
                    },
                    {
                        data: 'Cost', title: 'Refunded Amount', render: function (value, type, row, rowIndex) {
                            var itemId = row.Id;
                            var itemType = row.Type;
                            return '<a href="#"> ' + value + ' </a>'
                        }
                    },
                    {
                        data: 'Id', title: 'Partial Payment', render: function (value, type, row, rowIndex) {
                            var itemId = row.Id;
                            var itemType = row.Type;
                            return itemType == 'Course' ? '<input type="text" class="form-control input-sm"  />' : '';
                        }
                    }
                ]
            });

        },
        saveOrder: function () {
            
        },
        printToPDF: function () {

        },
        itemsData: []
    }
})();
//$(document).ready(function () {
//    ReportsOrderDetail.loadOrderDetail('C5YLBH2FR4743UO');
//})