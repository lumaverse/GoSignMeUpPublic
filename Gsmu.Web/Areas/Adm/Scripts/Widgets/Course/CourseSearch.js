CourseSearch = (function () {
    return {
        init: function () {
            //FOR INIT DEFAULT
            var termTemplate = "<span class='ui-autocomplete-term'>%s</span>";
            var isOpened = false;
            $('.course-search-input')
                .bind('focus', function () {
                    if (isOpened && !$("ul.ui-autocomplete").is(":visible")) {
                        $("ul.ui-autocomplete").show();
                    }
                })
                .autocomplete({
                source: function (request, response) {
                    gsmuUIObject.mask('body');
                    $.ajax({
                        url: gsmuObject.apiUrl + 'AdminCourseDash/GetCourseByFilter?filter=' + request.term,
                        dataType: "json",
                        success: function (data) {
                            response($.map(data.results.splice(0, 10), function (course) {
                                gsmuUIObject.unmask('body');
                                return course;
                            }));
                        }
                    });
                },
                minLength: 3,
                select: function (event, ui) {
                    gsmuUIObject.mask('body');
                    window.location.replace('/Adm/Dashboard/AdminCourseDashboard?courseId=' + ui.item.CourseId);
                },
                open: function () {
                    isOpened = true;
                    var acData = $('.course-search-input').data('ui-autocomplete');
                    var styledTerm = termTemplate.replace('%s', acData.term.toUpperCase());

                    acData
                        .menu
                        .element
                        .find('.val')
                        .each(function () {
                            var me = $(this);
                            var template = $(me[0]).text().toLowerCase().replace(acData.term.toLowerCase(), styledTerm)
                            me.html(template);
                        });
                        
                    acData
                        .menu
                        .element
                        .find('.name')
                        .each(function () {
                            var me = $(this);
                            var template = $(me[0]).text().toLowerCase().replace(acData.term.toLowerCase(), styledTerm)
                            me.html(template);
                        });
                },
                close: function () {
                    //if (!$("ul.ui-autocomplete").is(":visible")) {
                    //    $("ul.ui-autocomplete").show();
                    //}
                }
            })
            .data('ui-autocomplete')._renderItem = function (ul, data) {
                var url = gsmuUIObject.handleImages(data.TileImage, data.CourseId);
                var template = '<div class="item">' +
                    '<img class="img-thumbnail" src="' + url + '" />' +
                    '<div class="name"><span>' + data.CourseName + '</span></div>' +
                    '<div style="clear:both;"></div>' +
                    '<div class="prop">' +
                    '<div class="lbl"><span>Course ID : </span></div>' +
                    '<div class="val"><span>' + data.CourseId + ' - From ' + moment(data.CourseStartDate).format('MMMM Do YYYY') + ' To ' + moment(data.CourseEndDate).format('MMMM Do YYYY') + '</span></div>' +
                    '</div>' +
                    '<div class="prop">' +
                    '<div class="lbl"><span>Course Number : </span></div>' +
                    '<div class="val"><span>' + data.CourseNumber + '</span></div>' +
                    '</div>' +
                    '<div style="clear:both;"></div>' +
                    '</div>';
                return $("<li>")
                    .data("item.autocomplete", data)
                    .append(template)
                    .appendTo(ul);
            };
   
        },

    }
})();
