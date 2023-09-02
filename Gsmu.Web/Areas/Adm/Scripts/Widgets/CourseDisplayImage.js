var courseDisplayImage = {
    ui: {
        imageInput: $('#course-display-image-input'),
        image: $('#course-display-image')
    },
    init: function () {
        courseDisplayImage.initUI();
    },
    openFileInput: function () {
        $(courseDisplayImage.ui.imageInput).click();
    },
    changeFileInput: function (input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#course-display-image').attr('src', e.target.result);
            }
            reader.readAsDataURL(input.files[0]);
            courseDisplayImage.saveImage();
        }
    },
    saveImage: function () {
        setTimeout(function () {
            gsmuUIObject.mask('#widget-course-display-image');
            var data = new FormData();
            var files = $('#course-display-image-input').get(0).files;
            if (files.length > 0) {
                data.append('UploadedImage', files[0]);
                data.append('courseId', UrlHelper.getUrlVars()["courseId"]);
            }
            var saveCourse = $.ajax({
                type: 'POST',
                url: gsmuObject.apiUrl + 'AdminCourseDash/SaveCourseImage',
                contentType: false,
                processData: false,
                data: data
            });
            saveCourse.done(function (xhr, response) {
                if (response === 'success') {
                    $.jGrowl('Succesfully Added Course Image', { theme: 'successGrowl', themeState: '' });
                }
                else {
                    $.jGrowl('Something went wrong on your photo upload', { theme: 'errorGrowl', themeState: '' });
                }
                gsmuUIObject.unmask('#widget-course-display-image');
            })
        }, 500);
    },
    deleteImage: function () {
        courseDisplayImage.changeUrl(); // replace the image with the empty
        setTimeout(function () {
            gsmuUIObject.mask('#widget-course-display-image');
            var data = new FormData();
            data.append('courseId', UrlHelper.getUrlVars()["courseId"]);

            var deleteCourse = $.ajax({
                type: 'POST',
                url: gsmuObject.apiUrl + 'AdminCourseDash/DeleteCourseImage',
                contentType: false,
                processData: false,
                data: data
            });
            deleteCourse.done(function (xhr, response) {
                if (response === 'success') {
                    $.jGrowl('Succesfully Deleted Course Image', { theme: 'successGrowl', themeState: '' });
                }
                else {
                    $.jGrowl('Something went wrong on your photo deletion', { theme: 'errorGrowl', themeState: '' });
                }
                gsmuUIObject.unmask('#widget-course-display-image');
            })
        }, 500);
    },
    changeUrl: function () {
        $(courseDisplayImage.ui.image).attr('src', '../../Images/NoProfileImage.png');
    },
    initUI: function () {
        var courseId = UrlHelper.getUrlVars()["courseId"];
        $(courseDisplayImage.ui.image).attr('src', '../../../../admin/CourseTiles/' + courseId + '.jpg')
    }
}
$(document).ready(function () {
    courseDisplayImage.init();
})