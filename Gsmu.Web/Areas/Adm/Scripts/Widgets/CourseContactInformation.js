var courseContact = {
    ui: {
        view: $('#course-contact-content'),
        edit: $('#course-contact-editor'),
        editBtn: $('#course-contact-editor-btns .edit'),
        saveBtn: $('#course-contact-editor-btns .save'),
        cancelBtn: $('#course-contact-editor-btns .cancel')
    },
    init: function () {
        courseContactEditor.initView();
    }
}
var courseContactEditor = {
    initEditor: function () {
        $(courseContact.ui.view).hide();
        $(courseContact.ui.edit).show();

        $(courseContact.ui.editBtn).hide();
        $(courseContact.ui.saveBtn).show();
        $(courseContact.ui.cancelBtn).show();

        courseContactEditor.initDataBinding();
    },
    initView: function () {
        $(courseContact.ui.view).show();
        $(courseContact.ui.edit).hide();

        $(courseContact.ui.editBtn).show();
        $(courseContact.ui.saveBtn).hide();
        $(courseContact.ui.cancelBtn).hide();
    },
    initDataBinding: function () {
        $(courseContact.ui.edit).find('input').map(function (e, el) {
            var prop = $(el).attr('name');
            var value = courseModel[prop];
            $(el).val(value).addClass('focus-editor');
        });
    },
    save: function () {
        courseContactEditor.initDataBinding();
        courseEditor.save();
        courseEditor.initView();
    }
}
$(document).ready(function () {
    courseContact.init();
})