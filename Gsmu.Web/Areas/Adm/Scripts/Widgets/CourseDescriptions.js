var courseDescriptions = {
    ui: {
        longEditor: $('#course-description-long-content'),
        longValContainer: $('#course-description-long'),
        shortEditor: $('#course-description-short-content'),
        shortValContainer: $('#course-description-short'),
        longInputHandler: $('#course-description-long-input'),
        shortInputHandler: $('#course-description-short-input'),
        editBtn: $('#course-descriptions-editor-btns .edit'),
        saveBtn: $('#course-descriptions-editor-btns .save'),
        cancelBtn: $('#course-descriptions-editor-btns .cancel'),
    },
    init: function () {
        courseDescriptionsEditor.initView();
    }
}

var courseDescriptionsEditor = {
    initEditor: function () {
        $(courseDescriptions.ui.longEditor).summernote({
            dialogsFade: true,
            height: 85,
            minHeight: 85,
            maxHeight: 85,
        });
        $(courseDescriptions.ui.shortEditor).summernote({
            dialogsFade: true,
            height: 85,
            minHeight: 85,
            maxHeight: 85,
        });
        $(courseDescriptions.ui.editBtn).hide();
        $(courseDescriptions.ui.saveBtn).show();
        $(courseDescriptions.ui.cancelBtn).show();
    },
    initView: function () {
        $(courseDescriptions.ui.longEditor).summernote('destroy');
        $(courseDescriptions.ui.shortEditor).summernote('destroy');
        $(courseDescriptions.ui.editBtn).show();
        $(courseDescriptions.ui.saveBtn).hide();
        $(courseDescriptions.ui.cancelBtn).hide();

        //set default values
        $(courseDescriptions.ui.longInputHandler).val(courseModel.Description);
        $(courseDescriptions.ui.shortInputHandler).val(courseModel.ShortDescription);
    },
    initDataBinding: function () { 
        var long = $(courseDescriptions.ui.longValContainer).find('.note-editable').html();
        var short = $(courseDescriptions.ui.shortValContainer).find('.note-editable').html();
        $(courseDescriptions.ui.longInputHandler).val(long);
        $(courseDescriptions.ui.shortInputHandler).val(short);
    },
    clearAll: function () { 
        $(courseDescriptionsEditor.ui.edit).find('input').map(function (e, el) {
            $(el).removeClass('focus-editor').val('');
        })
    },
    save: function () {
        courseDescriptionsEditor.initDataBinding();
        courseEditor.save();
        courseDescriptionsEditor.initView();
    }
}
$(document).ready(function () {
    courseDescriptions.init();
});
