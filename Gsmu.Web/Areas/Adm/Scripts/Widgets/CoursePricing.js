var coursePricingModel = {}
var coursePricing = {
    ui: {
        nonDistPrice: $('#non-member-price'),
        distPrice: $('#member-price'),
        specialPrice: $('#special-member-price'),
        nonFirstPayment: $('#non-member-first-payment'),
        firstPayment: $('#member-first-payment'),
        spFirstPayment: $('#special-member-first-payment')
    },
    init: function () {
        gsmuUIObject.mask('.widget-course-pricing-panel');
        var pricingOptionsData = gsmuConfiguration.globalData.getAllPricingOptions();
        pricingOptionsData.done(function (data) {
            if (data.Success === 1) {
                coursePricing.initUI(data.Data)
            }
        });
    },
    initUI: function (pricingOptionsData) {
        var nonMemberTable = $('#course-pricing-membership-non-member-grid').DataTable({
            responsive: true,
            autoWitdh: false,
            scrollY: "300px",
            destroy: true,
            "searching": false,
            data: pricingOptionsData,
            columns: [
                {
                    data: 'PricingOptionId', 'visible': false
                },
                {
                    title: "Description", data: 'PriceTypeDescription'
                },
                {
                    title: "Price", data: 'Price', render: function (data, type, row) {
                        var pricingOptionId = row.PricingOptionId;
                        var input = '$ <input type="number" id="pricing-option-non-' + pricingOptionId +'" class="form-control input-sm price" step="0.01" value="'+ parseFloat(data).toFixed(2) +'" />'
                        return input;
                    }
                },
                {
                    title: "Controls", data: 'PricingOptionId', render: function (data, type, row) {
                        var hSpacer = '<span class="h-spacer-sm"> &nbsp </span>'
                        var inputOne = '<input type="number" id="pricing-non-range-start-' + row.PricingOptionId + '" class="form-control input-sm price pad-x-5" step="1" value="0" />';
                        var inputTwo = '<input type="number"  id="pricing-non-range-end-' + row.PricingOptionId + '" class="form-control input-sm price pad-x-5" step="1" value="0" />';
                        var calcBtn =  '<button type="button" class="btn btn-default btn-xs pricing-grid-calc-btn">&nbsp</button>';
                        var checkBox = '<input type="checkbox" class="label-middle" />'
                        return checkBox + hSpacer + calcBtn + hSpacer + inputOne + hSpacer + inputTwo;
                    }
                }
            ]
        });

        $('#widget-course-pricing a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            var index = $(e.target).closest('li').index() + 1;
            switch (index){
                case 1:
                    //do nothing
                    break;
                case 2:
                    var memberTable = $('#course-pricing-membership-member-grid').DataTable({
                        responsive: true,
                        autoWitdh: false,
                        destroy: true,
                        scrollY: "300px",
                        data: pricingOptionsData,
                        paging: true,
                        columns: [
                            {
                                data: 'PricingOptionId', 'visible': false
                            },
                            {
                                title: "Description", data: 'PriceTypeDescription'
                            },
                            {
                                title: "Price", data: 'Price', render: function (data, type, row) {
                                    var pricingOptionId = row.PricingOptionId;
                                    var input = '$ <input type="number" id="pricing-option-' + pricingOptionId + '" class="form-control input-sm price" step="0.01" value="' + parseFloat(data).toFixed(2) + '" />'
                                    return input;
                                }
                            },
                            {
                                title: "Controls", data: 'PricingOptionId', render: function (data, type, row) {
                                    var hSpacer = '<span class="h-spacer-sm"> &nbsp </span>'
                                    var inputOne = '<input type="number" id="pricing-range-start-' + row.PricingOptionId + '" class="form-control input-sm price pad-x-5" step="1" value="0" />';
                                    var inputTwo = '<input type="number" id="pricing-range-start-' + row.PricingOptionId + '" class="form-control input-sm price pad-x-5" step="1" value="0" />';
                                    var calcBtn = '<button type="button" class="btn btn-default btn-xs pricing-grid-calc-btn">&nbsp</button>';
                                    var checkBox = '<input type="checkbox" class="label-middle" />'
                                    return checkBox + hSpacer + calcBtn + hSpacer + inputOne + hSpacer + inputTwo;
                                }
                            }
                        ]
                    });
                    coursePricing.MemberDataTableInstance = memberTable;
                    coursePricing.MembershipPricing.map(function (item) {
                        var pricingOptionId = item.Id;
                        var type = item.Type;
                        var priceValue = item.Price > 0 ? item.Price.toFixed(2) : 0.00;
                        var membershipPriceInput = $('#pricing-option-' + pricingOptionId + '');
                        $(membershipPriceInput).val(priceValue).addClass('border-with-data');
                    })
                    break;
                case 3:
                    var specialTable = $('#course-pricing-membership-special-grid').DataTable({
                        responsive: true,
                        autoWitdh: false,
                        destroy: true,
                        scrollY: "300px",
                        data: pricingOptionsData,
                        paging: true,
                        columns: [
                            {
                                data: 'PricingOptionId', 'visible': false
                            },
                            {
                                title: "Description", data: 'PriceTypeDescription'
                            },
                            {
                                title: "Price", data: 'Price', render: function (data, type, row) {
                                    var pricingOptionId = row.PricingOptionId;
                                    var input = '$ <input type="number" id="pricing-option-special-' + pricingOptionId + '" class="form-control input-sm price" step="0.01" value="' + parseFloat(data).toFixed(2) + '" />'
                                    return input;
                                }
                            },
                            {
                                title: "Controls", data: 'PricingOptionId', render: function (data, type, row) {
                                    var hSpacer = '<span class="h-spacer-sm"> &nbsp </span>'
                                    var inputOne = '<input type="number" id="pricing-special-range-start-' + row.PricingOptionId + '" class="form-control input-sm price pad-x-5" step="1" value="0" />';
                                    var inputTwo = '<input type="number" id="pricing-special-range-start-' + row.PricingOptionId + '" class="form-control input-sm price pad-x-5" step="1" value="0" />';
                                    var calcBtn = '<button type="button" class="btn btn-default btn-xs pricing-grid-calc-btn">&nbsp</button>';
                                    var checkBox = '<input type="checkbox" class="label-middle" />'
                                    return checkBox + hSpacer + calcBtn + hSpacer + inputOne + hSpacer + inputTwo;
                                }
                            }
                        ]
                    });
                    coursePricing.SpecalDataTableInstance = specialTable;
                    coursePricing.SpecialPricing.map(function (item) {
                        var pricingOptionId = item.Id;
                        var type = item.Type;
                        var priceValue = item.Price > 0 ? item.Price.toFixed(2) : 0.00;
                        var specialPriceInput = $('#pricing-option-special-' + pricingOptionId + '');
                        $(specialPriceInput).val(priceValue).addClass('border-with-data');
                    });
                    break;
                case 4:
                    //do nothing for now
                    break;
            };
        });

        var nonDistPrice = parseFloat(courseModel.NoDistPrice).toFixed(2);
        var distPrice = parseFloat(courseModel.DistPrice).toFixed(2);
        var specialPrice = parseFloat(courseModel.SpecialPrice).toFixed(2);

        var nonFirstPayment = parseFloat(courseModel.PartialPaymentNon).toFixed(2);
        var firstPayment = parseFloat(courseModel.PartialPaymentAmount).toFixed(2);
        var spFirstPayment = parseFloat(courseModel.PartialPaymentSP).toFixed(2);

        $(coursePricing.ui.nonDistPrice).val(nonDistPrice);
        $(coursePricing.ui.distPrice).val(distPrice);
        $(coursePricing.ui.specialPrice).val(specialPrice);

        $(coursePricing.ui.nonFirstPayment).val(nonFirstPayment);
        $(coursePricing.ui.firstPayment).val(firstPayment);
        $(coursePricing.ui.spFirstPayment).val(spFirstPayment);

        $('#course-display-price').on('change', function () {
            var displayPrice = $(this).prop('checked');
            courseModel.DisplayPrice = displayPrice ? 1 : 0;
            courseEditor.save();
        });

        $('#course-pricing-membership-non-member-grid input[type="checkbox"]').on('change', function (e) {
            if ($(this).prop('checked')) {
                $(this).parent().parent().addClass('selected');
            }
            else {
                $(this).parent().parent().removeClass('selected');
            }
        });

        coursePricing.NonMemberDataTableInstance = nonMemberTable;

        coursePricing.loadCoursePricingDataToGrid();
    },
    loadCoursePricingOptionData: function () {
        var courseId = UrlHelper.getUrlVars()["courseId"];
        var getCoursePricing = function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: gsmuObject.apiUrl + '/AdminCourseDash/GetCoursePricingById?courseId=' + courseId
            });
        }
        return getCoursePricing();
    },
    loadCoursePricingDataToGrid: function () {
        var coursePricingData = coursePricing.loadCoursePricingOptionData();
        coursePricingData.done(function (data) {

            var publicPricing = data.Data.PublicCoursePricing;
            var membershipPricing = data.Data.MemberCoursePricing;
            var specialPricing = data.Data.SpecialCoursePricing;

            coursePricing.PublicPricing = publicPricing;
            coursePricing.MembershipPricing = membershipPricing;
            coursePricing.SpecialPricing = specialPricing;

            publicPricing.map(function (item) {
                var pricingOptionId = item.Id;
                var type = item.Type;
                var priceValue = item.Price > 0 ? item.Price.toFixed(2) : '0.00';
                var rangeStartValue = item.RangeStart > 0 ? item.RangeStart.toFixed(2) : '0.00';
                var rangeEndValue = item.RangeEnd > 0 ? item.RangeEnd.toFixed(2) : '0.00';

                var publicPriceInput = $('#pricing-option-non-' + pricingOptionId);
                var rangeStartInput = $('#pricing-non-range-start-' + pricingOptionId);
                var rangeEndInput = $('#pricing-non-range-end-' + pricingOptionId);
 
                $(publicPriceInput).val(priceValue).addClass('border-with-data');
                $(rangeStartInput).val(rangeStartValue).addClass('border-with-data');
                $(rangeEndInput).val(rangeEndValue).addClass('border-with-data');
            });
            gsmuUIObject.unmask('.widget-course-pricing-panel');
        });
    },
    getCoursePricingValues: function () {
        coursePricing.NonMemberDataTableInstance.rows('.selected').data().map(function (item) {
            coursePricing.NonMemberData.push({
                Id : item.PricingOptionId,
                Price: $('#pricing-option-non-' + item.PricingOptionId).val(),
                Type: 1,
                RangeStart: $('#pricing-non-range-start-' + item.PricingOptionId).val(),
                RangeEnd: $('#pricing-non-range-end-' + item.PricingOptionId).val(),
                CourseId: courseModel.CourseId
            });
        });
        if (coursePricing.MemberDataTableInstance != '')
        {
            coursePricing.MemberDataTableInstance.rows('.selected').data().map(function (item) {
                coursePricing.MemberData.push({
                    Id: item.PricingOptionId,
                    Price: $('#pricing-option-' + item.PricingOptionId).val(),
                    Type: 0,
                    RangeStart: $('#pricing-non-range-start-' + item.PricingOptionId).val(),
                    RangeEnd: $('#pricing-non-range-end-' + item.PricingOptionId).val(),
                    CourseId: courseModel.CourseId
                });
            });
        }
        
        if (coursePricing.SpecalDataTableInstance != '') {
            coursePricing.SpecalDataTableInstance.rows('.selected').data().map(function (item) {
                coursePricing.SpecialMemberData.push({
                    Id: item.PricingOptionId,
                    Price: $('#pricing-option-special-' + item.PricingOptionId).val(),
                    Type: 3,
                    RangeStart: $('#pricing-non-range-start-' + item.PricingOptionId).val(),
                    RangeEnd: $('#pricing-non-range-end-' + item.PricingOptionId).val(),
                    CourseId: courseModel.CourseId
                });
            });
        }
        return $.when(0);
    },
    PublicPricing: [],
    MembershipPricing: [],
    SpecialPricing: [],
    NonMemberDataTableInstance: '',
    MemberDataTableInstance: '',
    SpecalDataTableInstance: '',
    NonMemberData: [],
    MemberData: [],
    SpecialMemberData: [],
    Data: []
}
var coursePricingEditor = {
    selectRow: function (e) {

    },
    saveRequest: function () {
       return $.ajax({
            type: "POST",
            dataType: 'json',
            data: coursePricingModel,
            url: gsmuObject.apiUrl + '/AdminCourseDash/SaveCoursePrice'
        });
    },
    save: function () {
        gsmuUIObject.mask('.widget-course-pricing-panel');
        courseModel.NoDistPrice = $(coursePricing.ui.nonDistPrice).val();
        courseModel.DistPrice = $(coursePricing.ui.distPrice).val();
        courseModel.SpecialPrice = $(coursePricing.ui.specialPrice).val();

        courseModel.PartialPaymentNon = $(coursePricing.ui.nonFirstPayment).val();
        courseModel.PartialPaymentAmount = $(coursePricing.ui.firstPayment).val();
        courseModel.PartialPaymentSP = $(coursePricing.ui.spFirstPayment).val();

        coursePricing.getCoursePricingValues()
            .then(function () {
                coursePricingModel =
                    {
                            MemberCoursePricing: coursePricing.MemberData,
                            PublicCoursePricing: coursePricing.NonMemberData,
                            SpecialCoursePricing: coursePricing.SpecialMemberData,
                            CourseId: courseModel.CourseId
                    }
                coursePricingEditor.saveRequest().done(function (response) {
                    if (response.Success === 1)
                        $.jGrowl('Succesfully Saved Course Pricing data', { theme: 'successGrowl', themeState: '' });
                    else
                        $.jGrowl('Something went wrong. ' + response.Message, { theme: 'errorGrowl', themeState: '' });

                    coursePricing.init();
                });
            });
        courseEditor.save();
    },
    navigateToMange: function () {
        window.open(gsmuObject.adminUrl + 'systemconfig_pricingoptions.asp', '_blank')
    }
}

//$(document).ready(function () {
//    coursePricing.init();
//})
