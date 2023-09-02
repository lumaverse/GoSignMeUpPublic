$(document).ready(function () {

    Networking.initialize();
});

var NetworkVariableValues = function () {
    return {
        SendEmailUsingPreview: true,
        EmailAddressTo: '',
        InitiateEmailSending: true,
        InitiateAuditLogging: true,
        Subject: '',
        Body: '',
        isOnEditing: true,
        reloadData: function () {
            NetworkVariableValues.Subject = $('#gsmu-email-subject-preview').val();
            NetworkVariableValues.Body = $('#gsmu-email-body-preview').html();
            //NetworkVariableValues.SendEmailUsingPreview = $('#gsmu-email-enable-send-preview').attr('data-value') == 'enable' ? false : true;
            //NetworkVariableValues.isOnEditing = $('#gsmu-email-enable-editing').attr('data-value') == 'disable' ? true : false;
        }
    }
}();

var Networking =
    {
        initialize: function () {
            Networking.initdOrderNumberAutoComplete();
            NetworkingUIFunctions.initControlEvents();
        },
        initdOrderNumberAutoComplete: function () {

            $('#order-number').autocomplete({
                source: function (request, response) {
                    $.ajax({
                        dataType: "json",
                        url: '/Adm/Networking/GetOrderInfoByOrderNumber',
                        method: 'get',
                        data:
                        {
                            query: request.term,
                            mailType: parseInt($('#mail-type').val())
                        },
                        classes: {
                            "ui-autocomplete": "highlight"
                        },
                        beforeSend: function () {

                        },
                        success: function (data) {
                            response($.map(data, function (item) {
                                return {
                                    value: item.name,
                                    label: item.label,
                                    id: item.id
                                };
                            }));

                        },
                        complete: function () {

                        }
                    });
                }, minLength: 3,

                select: function (event, ui) {
                    event.preventDefault();
                    $('#order-number').val(ui.item.value);
                    $('#email-networking-id').val(ui.item.id);
                    $('#gsmu-email-send-btn, #gsmu-email-preview-with-token, #gsmu-email-enable-send-preview').removeAttr('disabled');
                },
                focus: function (event, ui) {
                    event.preventDefault();
                    $('#order-number').val(ui.item.value);
                }
            });

        },
        previewEmailTemplate: function (tokenize) {
            var mailType = parseInt($('#mail-type').val());
            switch (mailType) {
                case 0:
                    data = { OrderNumber: $('#order-number').val() };
                    break;
                case 1:
                    data = { RosterId: parseInt($('#email-networking-id').val()) };
                    break;
                case 2:
                    data = { RosterId: parseInt($('#email-networking-id').val()) };
                    break;
            }
            var defaultData = { mailType: parseInt($('#mail-type').val()), replaceToken: tokenize };
            // if Mailtype is confirmation it return multiple Course//
            //MultipleReturn of Course//
            //The backend of this funtion is on the NetworkController.cs//
            if (mailType == 0) {
                var Ordernum = $('#order-number').val();
                $.ajax({
                    url: '/Adm/Networking/GetMultipleCourseListByOrderNum',
                    data: { OrderNumber: Ordernum },
                    type: 'GET',

                    success: function (data) {
                        var returnData = data;
                        var subject = returnData.Subject;
                        var body = returnData.Body;

                        $('#gsmu-email-subject-preview').val(subject);
                        $('#gsmu-email-body-preview').html(body);

                        $('.container-fluid').find('input,select,textarea,a,button').addClass('disabled');
                        $('#gsmu-email-send-btn').removeClass('disabled');
                    },

                })
            }
            else {
                //This is for Cancelation and Wait List To Confirmation//
                //The backend of this funtion is on the EmailFunction.cs//
                $.ajax({
                    dataType: "json",
                    url: '/Adm/Networking/GetEmailPreview',
                    method: 'get',
                    data: $.extend(data, defaultData),
                    beforeSend: function () {
                        $('.container-fluid').find('input,select,textarea,a,button').attr('disabled', true);
                    },
                    success: function (data) {
                        var returnData = data;
                        var subject = returnData.Subject;
                        var body = returnData.Body;

                        $('#gsmu-email-subject-preview').val(subject);
                        $('#gsmu-email-body-preview').html(body);

                        $.jGrowl('Succesfully Loaded Email Template ' + (tokenize ? "with tokenization" : ""), { theme: 'successGrowl', themeState: '' });
                    },
                    complete: function () {
                        $('.container-fluid').find('input,select,textarea,a,button').removeAttr('disabled');
                        if ($('#order-number').val() == '')
                            NetworkingUIFunctions.initControlEvents();
                        NetworkingUIFunctions.constantEmailBodyUIMod(true);
                    }
                });
            }
        },
        PreviewMultipleCourse: function () {
            var Ordernum = $('#order-number').val();
            $.ajax({
                url: '/Adm/Networking/GetMultipleCourseListByOrderNum',
                data: { OrderNumber: Ordernum },
                type: 'GET',
                dataSrc: "",
            }).done(function (data) {

                var jsondata = JSON.stringify(data);
                var EmailContent = JSON.parse(jsondata);

                $.each(EmailContent, function (i) {

                    $("#gsmu-email-body-preview").append("<br>Course Id" + EmailContent[i].Body + " <br>");
                    $("#gsmu-email-subject-preview").val("" + EmailContent[i].subject + "");
                })


            });


        },
        clearPreviewEmailTemplate: function () {
            $('#gsmu-email-preview').removeAttr('disabled');
            $('#gsmu-email-subject-preview').val('Subject');
            $('#gsmu-email-body-preview').html('Body');
            $('#mail-type').val(0);
            $('#order-number, #email-networking-id').val('');
            $('#gsmu-email-body-preview').attr('contenteditable', true);
            //Networking.editing($('#gsmu-email-enable-editing').attr('data-value', 'disable'));
            //Networking.editingSendPreview($('#gsmu-email-enable-send-preview').attr('data-value', 'disable'));
            NetworkingUIFunctions.constantEmailBodyUIMod(false);
            NetworkingUIFunctions.initControlEvents();
        },
        sendEmail: function (button, confirm) {
            if (confirm != true && NetworkingUIFunctions.validateSending() == false) {
                return false;
            }
            button = $(button);
            var buttonText = button.html();
            var data = {};
            var url = '/Adm/Networking/';
            var mailType = parseInt($('#mail-type').val());

            switch (mailType) {
                case 0:
                    data = { OrderNumber: $('#order-number').val() };
                    url = url + 'SendConfirmationEmail';
                    break;
                case 1:
                    data = { RosterId: parseInt($('#email-networking-id').val()) };
                    url = url + 'SendCancellationtionEmail';
                    break;
                case 2:
                    data = { RosterId: parseInt($('#email-networking-id').val()) };
                    url = url + 'SendWaitListEmail';
                    break;
            }

            if (NetworkVariableValues.SendEmailUsingPreview) {
                data = Networking.constructEmailResendParams(data);
            }
            $.post({
                url: url,
                method: 'post',
                dataType: "json",
                data: data,
                beforeSend: function () {
                    button.html(buttonText.replace('Send', 'Sending...'));
                    button.addClass("disabled");
                    button.prop("disabled", true);
                    $('.container-fluid').find('input,select,textarea,a,button').attr('disabled', true);
                },
                success: function (data) {
                    var returnData = data;
                    if (data.Status == 'failed') {
                        $.jGrowl('Something went wrong! Email not sent.', { theme: 'errorGrowl', themeState: '' });
                        console.log(returnData.ErrorDescription);
                        $('#gsmu-email-subject-preview').val('Subject');
                        $('#gsmu-email-body-preview').html('Body');
                        return false;
                    }
                    var subject = returnData.EmailSubject;
                    var body = returnData.EmailBody;
                    var to = returnData.EmailTo;
                    var cc = returnData.EmailCC;
                    var bcc = returnData.EmailBCC;

                    $('#gsmu-email-subject-preview').val(subject);
                    $('#gsmu-email-body-preview').html(body);

                    $.jGrowl('Succesfully Sent the ' + $('#mail-type :selected').text() + ' message to ' + to, { theme: 'successGrowl', themeState: '' });
                },
                error: function (data) {
                    $.jGrowl('Something went wrong!', { theme: 'errorGrowl', themeState: '' });
                },
                complete: function () {
                    button.html(buttonText);
                    button.removeClass("disabled");
                    button.prop("disabled", false);
                    $('.container-fluid').find('input,select,textarea,a,button').removeAttr('disabled');
                    //Networking.editing($('#gsmu-email-enable-editing').attr('data-value', 'disable'));
                    //Networking.editingSendPreview($('#gsmu-email-enable-send-preview').attr('data-value', 'disable'));
                }
            });
        },
        editing: function (button) {
            var button = $(button);
            var buttonText = button.html();
            var icon = $('#editing-icon')[0];
            var iconClass = icon.className;
            if (button.attr('data-value') == 'enable') {
                NetworkingUIFunctions.editMode(button, buttonText, iconClass);
            }
            else {
                NetworkingUIFunctions.viewMode(button, buttonText, iconClass);
            }
        },
        editingSendPreview: function (button) {
            var button = $(button);
            var buttonText = button.html();
            var icon = $('#editing-prev-icon')[0];
            var iconClass = icon.className;
            if (button.attr('data-value') == 'enable') {
                NetworkingUIFunctions.editModeSendPrev(button, buttonText, iconClass);
            }
            else {
                NetworkingUIFunctions.viewModeSendPrev(button, buttonText, iconClass);
            }
        },
        constructEmailResendParams: function (data) {
            NetworkVariableValues.Body = encodeURIComponent(NetworkVariableValues.Body);
            var resendingModel = NetworkVariableValues;
            data = $.extend(data, resendingModel);
            return data;
        }
    }
//UI/UX IMPLEMENTATION
var NetworkingUIFunctions =
    {
        initControlEvents: function () {
            $('#gsmu-email-send-btn, #gsmu-email-preview-with-token, #gsmu-email-enable-send-preview').attr('disabled', true);
        },
        constantEmailBodyUIMod: function (on) {
            if (on) {
                $('#gsmu-email-body-preview').addClass('email-resend-body-content').focus();
            }
            else {
                $('#gsmu-email-body-preview').removeClass('email-resend-body-content');
            }

        },
        viewMode: function (button, buttonText, iconClass) {
            button.html(buttonText.replace('Disable Editing', 'Enable Editing')).attr('data-value', 'enable').removeClass('default-button-enabled');
            $('#editing-icon')
                .attr('class', iconClass.replace('glyphicon-ban-circle', 'glyphicon-ok-circle'));
            $('#gsmu-email-subject-preview').addClass('disabled', 'disabled');
            $('#gsmu-email-body-preview').attr('contenteditable', false).addClass('disabled');
            NetworkingUIFunctions.constantEmailBodyUIMod(false);
        },
        editMode: function (button, buttonText, iconClass) {
            button.html(buttonText.replace('Enable Editing', 'Disable Editing')).attr('data-value', 'disable').addClass('default-button-enabled');
            $('#editing-icon')
                .attr('class', iconClass.replace('glyphicon-ok-circle', 'glyphicon-ban-circle'));
            $('#gsmu-email-subject-preview').removeClass('disabled');
            $('#gsmu-email-body-preview').attr('contenteditable', true).removeClass('disabled');
            NetworkingUIFunctions.constantEmailBodyUIMod(true);
        },
        viewModeSendPrev: function (button, buttonText, iconClass) {
            button.html(buttonText.replace('Disable', 'Enable')).attr('data-value', 'enable').removeClass('default-button-enabled');
            $('#editing-prev-icon')
                .attr('class', iconClass.replace('glyphicon-ban-circle', 'glyphicon-ok-circle'));
        },
        editModeSendPrev: function (button, buttonText, iconClass) {
            button.html(buttonText.replace('Enable', 'Disable')).attr('data-value', 'disable').addClass('default-button-enabled');
            $('#editing-prev-icon')
                .attr('class', iconClass.replace('glyphicon-ok-circle', 'glyphicon-ban-circle'));
        },
        validateSending: function () {
            NetworkVariableValues.reloadData();
            if (NetworkVariableValues.Subject.toString().toLowerCase() == 'subject' || NetworkVariableValues.Body.toString().toLowerCase() == 'body') {
                bootbox.confirm({
                    title: 'Confirmation',
                    size: "small",
                    message: "Are you sure you want to send this email without changes?",
                    callback: function (result) {
                        if (result) {
                            Networking.sendEmail($('#gsmu-email-send-btn'), true);
                            return true;
                        }
                        else {
                            NetworkingUIFunctions.modalClose();
                        }
                    }
                });
                return false;
            }
            if ((NetworkVariableValues.isOnEditing || NetworkVariableValues.SendEmailUsingPreview) && (NetworkVariableValues.Subject == '' || NetworkVariableValues.Body == '')) {
                $.jGrowl("Can't send if either of Subject and Body is empty!", { theme: 'warningGrowl', themeState: '' });
                return false;
            }
            return true;
        },
        modalClose: function () {
            $('#modal-dialog').modal('hide');
        }
    }


