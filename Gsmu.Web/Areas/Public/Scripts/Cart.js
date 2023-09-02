function Cart(options) {
    var self = this;

    self.CartImageFull = options.CartImageFull;
    self.CartImageEmpty = options.CartImageEmpty;

    Ext.merge(self.Options, options);

    window.CART = self;

    Ext.onDocumentReady(function () {
        Ext.Ajax.timeout = 120000; // 120 seconds
        self.elementInfo = Ext.get('cart-info');
        self.elementContainer = Ext.get('cart-info-container');
        self.elementPopup = Ext.get('cart-popup');
        self.elementPopupItems = Ext.get('cart-popup-items');
        self.elementCheckoutContainer = Ext.get('cart-checkout-container');
        self.elementCheckoutContainer.enableDisplayMode();
        self.elementMainCenter = Ext.get('grad_stud_main2');
        self.elementBodyTitle = Ext.get('grad_stud_title');
        self.activeCheckoutStep = 'cart';

        self.setCartImage();

        self.popupHelper = new PopupHelper({
            popupElement: self.elementContainer,
            documentClickAction: function () {
                if (!window.LAYOUT.State.isEditMode) {
                    self.hideMiniDisplay();
                }
            }
        });

        self.executeReloadCallback();
    });
}
Cart.prototype.CheckoutSort = "0";
Cart.prototype.CartImageFull = null;
Cart.prototype.CartImageEmpty = null;
Cart.prototype.CancelCartShowImage = 'Images/Icons/glyph2/Icons24x24/Cancel.png';

Cart.prototype.Options = {
    ExtraParticipantCollectionEnabled: false,
    ExtraParticipantLabel: null,
    ExtraParticipantCollectCustomField: false,
    ExtraParticipantCustomFieldLabel: null
};

Cart.prototype.constructor = Cart;

Cart.prototype.popupHelper = null;

Cart.prototype.elementContainer = null;
Cart.prototype.elementInfo = null;
Cart.prototype.elementPopup = null;
Cart.prototype.elementPopupItems = null;
Cart.prototype.elementCheckoutContainer = null;
Cart.prototype.elementMainCenter = null;
Cart.prototype.elementBodyTitle = null;

Cart.prototype.courseMaterialData = {};
Cart.prototype.courseSelectedCredits = {};

Cart.prototype.activeCheckoutStep = null;
Cart.prototype.checkMaterial = null;
Cart.prototype.ShowCourseDetails = null;
Cart.prototype.ShowRegisterUser = null;

Cart.prototype.calculateMaterialPerQty = null;
Cart.prototype.ComputePriceTotal = null;

Cart.prototype.mask = null;
Cart.prototype.delay = null;
Cart.prototype.setCartImage = function () {
    var self = this;

    var currentImage = self.CartImageEmpty;
    var cartstatus = $("#cart-info").html();
    if (cartstatus.toLowerCase().indexOf('empty') == -1) {
        currentImage = self.CartImageFull;
    }
    if(self.elementContainer!=null){
        self.elementContainer.setStyle('backgroundImage', "url('" + config.getUrl(currentImage) + "')");
    }
};

Cart.prototype.resetMaterialData = function () {
    var self = this;

    self.courseMaterialData = {};
}

Cart.prototype.setCourseSelectedMaterials = function (courseIdWithModifier, selectedMaterials) {
    var self = this;
    self.courseMaterialData[courseIdWithModifier]['selectedMaterials'] = selectedMaterials;
}

Cart.prototype.setCourseMaterialData = function (courseIdWithModifier, materialsRequired, materials) {
    var self = this;

    self.courseMaterialData[courseIdWithModifier] = {};
    self.courseMaterialData[courseIdWithModifier]['materialsRequired'] = materialsRequired;
    self.courseMaterialData[courseIdWithModifier]['materials'] = materials;
}

Cart.prototype.miniDisplay = function (cmd) {

    var self = this;

    if (self.elementPopup.isVisible() && cmd != 'refresh') {
        self.hideMiniDisplay();
        return;
    }

    self.elementContainer.mask();
    Ext.Ajax.request({
        url: config.getUrl('public/cart/minidisplay'),
        success: function (response) {
            //notsure why this is not working
            //if (response.responseText.indexOf("Empty") >= 0){
            //    $('#reservationtimer').hide();
            //}
            self.elementPopupItems.setHtml(response.responseText);
            self.showMiniDisplay();
            self.elementContainer.unmask();
            //var url = config.getUrl('public/cart/ThankYou');
            //var win = window.open(url, '_blank');
            //$(win.document.body).html('Redirecting...');
            //win.focus();
        }
    });
}

Cart.prototype.refreshDisplay = function () {
    var self = this;

    if (self.elementPopup.isVisible()) {
        self.miniDisplay('refresh');
    }
}

Cart.prototype.showMiniDisplay = function () {
    var self = this;
    self.elementPopup.show();
    self.elementContainer.setStyle('backgroundImage', "url('" + config.getUrl(self.CancelCartShowImage) + "')");
}

Cart.prototype.hideMiniDisplay = function () {
    var self = this;
    self.elementPopup.setStyle({
        display: 'none'
    });
    self.setCartImage();
}

var isOperationInProgress = 'No';

Cart.prototype.AddMembership = function (MembershipID) {
    var self = this;
    var params = {
        courseId: MembershipID,
        coursePricingOptionId:"0"
    };

    Ext.Ajax.request({
        url: config.getUrl('public/cart/addcourse'),
        headers: { 'Content-Type': 'application/json' },
        jsonData: params,
        success: function (response) {
            var result = Ext.decode(response.responseText);
            if (result.success) {

                self.updateCartStatus(response.responseText);
            }

            CourseSearch.prototype.CloseMembershipDetailsWindow();
            cart.HideReviewCheckout();
        }
    });

}

Cart.prototype.RemoveEvent = function (courseId,cmd) {
    var self = this;
    var selectedEvents = [];

    var params = {
        eventParent: courseId
    };

    Ext.Ajax.request({
        url: config.getUrl('public/cart/RemoveEvents'),
        headers: { 'Content-Type': 'application/json' },
        jsonData: params,
        success: function (response) {
            var result = Ext.decode(response.responseText);
            if (result.success) {
                if (cmd == "checkout") {
                    Ext.util.Cookies.set('cmdonload', 'ShowReviewCheckout');
                    Ext.getBody().mask('Please wait...')
                    location.reload();

                } else {
                    self.miniDisplay('refresh');
                    self.elementInfo.setHtml(result.status);
                    self.updateCartStatus(response.responseText);
                }
            }

        }
    });
}

Cart.prototype.AddEvent = function (courseId, modifier) {
    var self = this;
    var selectedEvents = [];
    //Ext.getCmp('CourseDetailsWindow').mask('Please wait...')
    window.LAYOUT.MaskLayout('Loading');
    $("#AddEvent").prop("display", "none");
    var params = {
        eventParent: courseId
    };

    $('.SessionOptn').each(function () {
        var SessionCourseId = $(this).val();
        if ($(this).is(':checked')) {
            selectedEvents.push(SessionCourseId);
        }
    });
    if (selectedEvents.length == 0) {
        // Ext.getCmp('CourseDetailsWindow').unmask();
        config.showWarning('Choose the sessions you want then click the Add Event button.', 'Cart error');
        return;
    }
    selectedEvents.push(courseId);


    Ext.Ajax.request({
        url: config.getUrl('public/cart/RemoveEvents'),
        headers: { 'Content-Type': 'application/json' },
        jsonData: params,
        success: function (response) {
            window.LAYOUT.UnmaskLayout();
            var result = Ext.decode(response.responseText);
            if (result.success) {

                Cart.prototype.AddEventLoop(selectedEvents, selectedEvents.length - 1, courseId)

            }
            $("#AddEvent").prop("display", "block");

        }
    });
}

Cart.prototype.AddEventLoop = function (selectedEvents, eventIndex, eventParent) {
    var self = this;
    window.LAYOUT.MaskLayout('Loading');
    var courseId = selectedEvents[eventIndex];
    var coursePricingOptionId = null;
    var courseChoiceId = null;
    var selectedMaterials = [];
    //var selectedEvents = [];
    //var eventParent = null;
    var selected_credits = [];
    var accessCode = null;
    var qty = [];

    $('.EventMaterial.' + courseId).each(function () {
        var selmat = $(this).val();
        if ($(this).is(':checked')) {
            selectedMaterials.push(selmat);
            var selmatqty = $('.EventMaterialQty.' + courseId).val();
            qty.push(selmatqty);
        }
    });

    if (courseId == eventParent) {
        var accessCode = $("#accesscode").val();
    }

    coursePricingOptionId = $('.EventPrice.' + courseId).val();
    if (typeof (coursePricingOptionId) == "undefined") { coursePricingOptionId = null }

    var courseChoiceRadioGroup = $('input[name=ccradio' + courseId + ']:checked');
    if ($('input[name=ccradio' + courseId + ']').length > 0) {
        if (courseChoiceRadioGroup != null) {
            var courseChoiceSelect = parseInt(courseChoiceRadioGroup.val());
            var courseChoiceId = null;
            if (courseChoiceSelect != null) {
                courseChoiceId = courseChoiceSelect;
                if (isNaN(courseChoiceId) || courseChoiceId == null || courseChoiceId == '' || courseChoiceId == 0) {
                    if (courseId != eventParent) {
                        config.showWarning('Please select a session choice in selected event!', 'Session choice not selected.');
                    } else {
                        config.showWarning('Please select a course choice!', 'Course choice not selected.');
                    }
                    return;
                }
            }
        }
    }



    var params = {
        courseId: courseId,
        coursePricingOptionId: coursePricingOptionId,
        courseChoiceId: courseChoiceId,
        materialsIds: selectedMaterials,
        eventIds: selectedEvents,
        eventParent: eventParent,
        selectedcredits: selected_credits,
        accessCode: accessCode,
        extraParticipants: null,
        studentId: null,
        qty: qty,
        enrollStudentOnHouseholdRequired: null
    };


    Ext.Ajax.request({
        url: config.getUrl('public/cart/addcourse'),
        headers: { 'Content-Type': 'application/json' },
        jsonData: params,
        success: function (response) {
            isOperationInProgress = 'No'
            var result = Ext.decode(response.responseText);
            //if (result.status == 'household pending') {
            //    Ext.MessageBox.show({
            //        title: 'Confirmation',
            //        msg: 'There is only one spot left and you can not add any additional members. Would you like to continue only enrolling yourself?',
            //        buttons: Ext.Msg.YESNO,
            //        icon: Ext.Msg.QUESTION,
            //        fn: function (btn) {
            //            if (btn == "yes") {
            //                cart.AddCourse(courseId, modifier, extraParticipants, multipleEnroll, true);
            //            }
            //            if (btn == "no") {
            //                //NO
            //            }
            //        }
            //    });
            //    window.LAYOUT.UnmaskLayout();
            //    return;
            //}
            //if (result.extraParticipantRequired) {
            //    var ExtraParticipantRequiredFields = result.ExtraParticipantRequiredFields;
            //    var form = new CourseExtraParticipantForm(courseId, modifier, self.Options, result.statistics, ExtraParticipantRequiredFields);
            //    window.LAYOUT.UnmaskLayout();
            //    return;
            //}
            window.LAYOUT.UnmaskLayout();
            if (result.success) {
                self.elementInfo = Ext.get('cart-info');
                var nextIndex = eventIndex - 1;
                if (nextIndex > -1) {
                    Cart.prototype.AddEventLoop(selectedEvents, nextIndex, eventParent)
                } else {


                    CourseSearch.prototype.CloseCourseDetailsWindow();
                    self.updateCartStatus(response.responseText)

                }
                //self.updateCartStatus(response.responseText);
                //if (multipleEnroll) {
                //    cart.DisplayAllStudents_ForEnrollment(courseId);
                //}
                //else {
                //    animateButton();
                //}


            }
            else {
                //console.log("Error")
                //console.log(result.error)
                //if ($("#CourseNameDescContainer").length > 0) {
                //    self.setInterimWindow(result.error);

                //}
                //else {
                var e_message = result.error + result.message
                config.showWarning(e_message.replace('undefined',''), 'Cart error');
                Ext.getCmp('CourseDetailsWindow').unmask();
                //    cartButton.fadeIn();
                //}
            }

            window.LAYOUT.UnmaskLayout();
        }
    });

}




// if you change the courseid, modifier, please add it to the courseextraparticipantform.js as well
Cart.prototype.AddCourse = function (courseId, modifier, extraParticipants, multipleEnroll, enrollStudentOnHouseholdRequired) {
    //check prereq config
    //let preReqConfigPromise = cart.GetPreReqConfig();
    //let overrideCheck = preReqConfigPromise.done((response) => {
    //var isAdvance = (response);
    // if (isAdvance == "TrueX" && modifier !== 'CourseDetails Prerequisite') {
    if (1 == 2) {
    }
    else {
        if (isOperationInProgress == 'No') {
            isOperationInProgress = 'Yes'
            var self = this;
            var error = "";
            if (typeof (modifier) == 'undefined') {
                modifier = '';
            }

            //check requisite
            if ($('#chkRequisite').length > 0) {
                if ($('#chkRequisite').is(':checked')) {
                    console.log('checked');
                }
                else {
                    self.setInterimWindow('Requisite');
                    error = "Requisite";
                }
            }
            // check price
            var coursePricingOptionSelect = Ext.getCmp('course-price-' + courseId + modifier);
            var coursePricingOptionId = null;

            if (coursePricingOptionSelect != null && modifier != "CourseDetails") {
            }
            else {
                if ($('#course-price-' + courseId + modifier).val() != undefined) {
                    coursePricingOptionId = $('#course-price-' + courseId + modifier).val();
                }
            }

            if (modifier == "CourseDetails") {
                coursePricingOptionSelect = $("input[name='coursepriceitem']:checked").val();
            }
            if (coursePricingOptionSelect != null && modifier == "CourseDetails") {
                coursePricingOptionId = $("input[name='coursepriceitem']:checked").val()
            }

            var courseChoiceRadioGroup = $('input[name=ccradio]:checked');
            if (courseChoiceRadioGroup != null) {
                var courseChoiceSelect = parseInt(courseChoiceRadioGroup.val());
                var courseChoiceId = null;
                if (courseChoiceSelect != null) {
                    courseChoiceId = courseChoiceSelect;
                    if (isNaN(courseChoiceId) || courseChoiceId == null || courseChoiceId == '' || courseChoiceId == 0) {
                        if ($("#CourseChoiceContainerDet").length > 0) {
                            if (isNaN(courseChoiceId)) {
                                //TODO: set up interim window
                                self.setInterimWindow('CourseChoice');
                                error = error + " Course Choice";
                            } else {
                                config.showWarning('Please select a course choice!', 'Course choice not selected...');
                                return;
                            }
                        }
                    }
                }
            }

            //Event Selected
            var eventParent = parseInt(Ext.util.Cookies.get('eventParent'));
            var selectedEvents = [];
            $('.SessionOptn').each(function () {
                var SessionCourseId = $(this).val();
                if ($(this).is(':checked')) {
                    selectedEvents.push(SessionCourseId);
                }
            });

            //Event Materials
            var selectedEvents = [];
            $('.SessionOptn').each(function () {
                var SessionCourseId = $(this).val();
                if ($(this).is(':checked')) {
                    selectedEvents.push(SessionCourseId);
                }
            });


            // check materials
            function CountObj(obj) {
                // replacement for getOwnPropertyNames length to be compatible in IE8
                var count = 0;
                for (var x in obj) {
                    ++count;
                }
                return count;
            }

            var courseMaterialData = self.courseMaterialData[courseId + modifier];
            if (courseMaterialData != undefined) {
                if (courseMaterialData['materialsRequired']) {
                    //if (!Ext.isDefined(courseMaterialData['selectedMaterials']) || !Ext.isObject(courseMaterialData['selectedMaterials']) || Object.getOwnPropertyNames(courseMaterialData['selectedMaterials']).length < 1 CountObj(courseMaterialData['selectedMaterials']) < 1) {
                    if (!Ext.isDefined(courseMaterialData['selectedMaterials']) || !Ext.isObject(courseMaterialData['selectedMaterials']) || CountObj(courseMaterialData['selectedMaterials']) < 1) {
                        if ($("#CourseNameDescContainer").length > 0) {
                            self.setInterimWindow('material');
                            error = error + " material";
                        }
                        else {
                            config.showWarning('Please select at least 1 material for this course...', 'Material selection required');
                            return;
                        }
                    }
                }
            }
            var selectedMaterials = [];
            if (courseMaterialData != undefined) {
                if (Ext.isObject(courseMaterialData['selectedMaterials'])) {
                    for (var key in courseMaterialData['selectedMaterials']) {
                        var material = courseMaterialData['selectedMaterials'][key];
                        selectedMaterials.push(material.get('productID'));
                    }
                }
            }
            var qty = [];
            for (var materialid in selectedMaterials) {
                qty_comp = $('input[name=qtyNoField' + selectedMaterials[materialid] + "]").val();
                qty.push(qty_comp);
            }

            var cartButton = Ext.get(Ext.DomQuery.selectNode('[data-cart-button=' + courseId + modifier + ']'));
            var selected_credits = [];
            var courseselected_credits = self.courseSelectedCredits[courseId];
            if (courseselected_credits != undefined) {
                for (var key in courseselected_credits['selectedCredits']) {
                    var credit = courseselected_credits['selectedCredits'][key];
                    selected_credits.push(credit);
                }
            }

            var completeAddToCart = function (accessCode, passrequirements, accesscodecancelled) {
                window.LAYOUT.MaskLayout('Loading');
                //VALIDATE ENROLLMENTCHECKING
                Ext.Ajax.request({
                    url: config.getUrl('public/cart/EnrollmentCheckSettings'),
                    headers: { 'Content-Type': 'application/json' },
                    jsonData: { course_id: courseId },
                    asynch: false,
                    success: function (response) {
                        var data = Ext.decode(response.responseText);
                        if (data.allowed == false) {
                            var message = '';
                            if (data.enrollmentCheckSetting == 1)
                                message = 'Name';
                            else if (data.enrollmentCheckSetting == 2)
                                message = 'Number';
                            else if (data.enrollmentCheckSetting == 3)
                                message = 'Name and Number';
                            Ext.MessageBox.show({
                                buttons: Ext.MessageBox.OK,
                                title: 'Stop!',
                                msg: 'You cannot add this course because you have already enrolled in a course with the same ' + message,
                                icon: Ext.MessageBox.ERROR
                            });
                            isOperationInProgress = 'No'
                            window.LAYOUT.UnmaskLayout();
                            return false;
                        }
                        else {
                   
                            if (!Ext.isDefined(accessCode)) {
                                accessCode = null;
                            }

                            var animateButton = function () {
                                cartButton.fadeOut({
                                    duration: 1000
                                });
                                var courseItem = Ext.get(Ext.DomQuery.selectNode('[data-course-id=' + courseId + ']'));
                                var position = self.elementContainer.getXY();
                                var x = position[0] + (self.elementContainer.getWidth() / 2);
                                var y = position[1] + (self.elementContainer.getHeight() / 2);

                                var dh = Ext.DomHelper;
                                var currentUrl = document.location + " ";
                                if (courseItem != null) {
                                  
                                }
                                
                                CourseSearch.prototype.CloseCourseDetailsWindow();
                            }

                            if (error.length > 0) {
                                window.LAYOUT.UnmaskLayout();
                                return;

                            }
                            var params = {
                                courseId: courseId,
                                coursePricingOptionId: coursePricingOptionId,
                                courseChoiceId: courseChoiceId,
                                materialsIds: selectedMaterials,
                                eventIds: selectedEvents,
                                eventParent: eventParent,
                                selectedcredits: selected_credits,
                                accessCode: accessCode,
                                extraParticipants: null,
                                studentId: null,
                                qty: qty,
                                enrollStudentOnHouseholdRequired: enrollStudentOnHouseholdRequired,
                                passrequirements: passrequirements
                            };
                            if (extraParticipants instanceof Array) {
                                params.extraParticipants = extraParticipants;
                            }
                            Ext.Ajax.request({
                                url: config.getUrl('public/cart/addcourse'),
                                headers: { 'Content-Type': 'application/json' },
                                jsonData: params,
                                success: function (response) {
                                    isOperationInProgress = 'No'
                                    var result = Ext.decode(response.responseText);
                                    if (result.status == 'household pending') {
                                        Ext.MessageBox.show({
                                            title: 'Confirmation',
                                            msg: 'There is only one spot left and you can not add any additional members. Would you like to continue only enrolling yourself?',
                                            buttons: Ext.Msg.YESNO,
                                            icon: Ext.Msg.QUESTION,
                                            fn: function (btn) {
                                                if (btn == "yes") {
                                                    cart.AddCourse(courseId, modifier, extraParticipants, multipleEnroll, true);
                                                }
                                                if (btn == "no") {
                                                    //NO
                                                }
                                            }
                                        });
                                        window.LAYOUT.UnmaskLayout();
                                        return;
                                    }

                                    if (result.status == "Color_Groupings_Duplicate") {
                                        message="test"
                                        if (result.message != "" && result.message != null) {
                                            message = result.message;
                                        }
                                        
                                        Ext.Msg.alert('Error', message);
                                        window.LAYOUT.UnmaskLayout();
                                        return;
                                    }

                                    if (result.AdminCheckAccessCode) {
                                        Ext.MessageBox.show({
                                            title: 'Confirmation',
                                            msg: 'This class uses an access code in the public area. Make sure that selected students can access it. Proceed with enrollment?',
                                            buttons: Ext.Msg.YESNO,
                                            icon: Ext.Msg.QUESTION,
                                            fn: function (btn) {
                                                if (btn == "yes") {
                                                    completeAddToCart();
                                                    window.LAYOUT.UnmaskLayout();
                                                    return;
                                                }
                                                if (btn == "no") {
                                                    Ext.Ajax.request({
                                                        url: config.getUrl('public/cart/removecourse?courseId=' + courseId),
                                                        success: function (response) {
                                                        }
                                                    });
                                                }
                                            }
                                        });
                                        window.LAYOUT.UnmaskLayout();
                                        return;
                                    }
                                    if (result.AdminCheckCourseRequirements) {
                                        Ext.MessageBox.show({
                                            title: 'Confirmation',
                                            msg: 'This course has restricted access. Are you sure you want to by pass the enrollment requirement?',
                                            buttons: Ext.Msg.YESNO,
                                            icon: Ext.Msg.QUESTION,
                                            fn: function (btn) {
                                                if (btn == "yes") {
                                                    completeAddToCart('', true);
                                                    window.LAYOUT.UnmaskLayout();
                                                    return;
                                                }
                                                if (btn == "no") {
                                                    Ext.Ajax.request({
                                                        url: config.getUrl('public/cart/removecourse?courseId=' + courseId),
                                                        success: function (response) {
                                                        }
                                                    });
                                                }
                                            }
                                        });
                                        window.LAYOUT.UnmaskLayout();
                                        return;
                                    }
                                    if (result.extraParticipantRequired) {
                                        var ExtraParticipantRequiredFields = result.ExtraParticipantRequiredFields;
                                        var form = new CourseExtraParticipantForm(courseId, modifier, self.Options, result.statistics, ExtraParticipantRequiredFields);
                                        window.LAYOUT.UnmaskLayout();
                                        return;
                                    }
                                    if (result.success) {

                                        self.updateCartStatus(response.responseText);
                                        if (multipleEnroll) {
                                            cart.DisplayAllStudents_ForEnrollment(courseId);
                                        }
                                        else {
                                            animateButton();
                                        }
                                    }
                                    else {
                                        if ($("#CourseNameDescContainer").length > 0) {
                                            self.setInterimWindow(result.error);

                                        }
                                        else {
                                            if (accesscodecancelled != "Cancelled") {
                                                config.showWarning(result.error, 'Cart error');
                                                cartButton.fadeIn();
                                            }
                                        }
                                    }

                                    if (eventParent > 0) {
                                        Ext.util.Cookies.set('eventParent', 0);
                                        window.CourseSearchInstance.ShowCourseDetails(eventParent, 'childevent')
                                    }

                                    window.LAYOUT.UnmaskLayout();
                                }
                            });

                            window.LAYOUT.UnmaskLayout();
                        }
                    }
                });

            } //completeAddToCart

            var validaccess = $("#accessvalid").val();
            var accesscodedetail = $("#accesscode").val();
            if (accesscodedetail == "undefined" || accesscodedetail==undefined) {
                accesscodedetail = "";

            }
            if ((cartButton != null) && (cartButton.getAttribute('data-access-code-required') == 'true' && validaccess != "1")) {
                Ext.MessageBox.prompt('Access code required', 'This course requires an access code. Please enter it here.',
            function (button, accessCode) {
                if (button == 'ok') {
                    completeAddToCart(accessCode);
                }
                else {
                    completeAddToCart(accessCode,"","Cancelled");
                }
            }
        );
            } else {
                if (accesscodedetail == "") {
                    completeAddToCart();
                }
                else {
                    completeAddToCart(accesscodedetail);
                }
            }
        } //isOperationInProgress
    } //isAdvance
    //});
}
Cart.prototype.setInterimWindow = function (returnederror) {

    isOperationInProgress = 'No'
    $("#CourseInstructorsContainer").removeClass("course-widgetbox").addClass('course-widgetbox_disabled');
    $("#CourseMediaContainer").removeClass("course-widgetbox").addClass('course-widgetbox_disabled');
    // $("#CourseCreditsContainer").removeClass("course-widgetbox").addClass('course-widgetbox_disabled');
    $("#CourseContactContainer").removeClass("course-widgetbox").addClass('course-widgetbox_disabled');
    //$("#CoursePriceContainer").removeClass("course-widgetbox").addClass('course-widgetbox_disabled');
    $("#CourseNameDescContainer").removeClass("course-widgetbox").addClass('course-widgetbox_disabled');
    $("#CourseDates_and_TimesContainer").removeClass("course-widgetbox").addClass('course-widgetbox_disabled');
    $("#CourseLocationContainer").removeClass("course-widgetbox").addClass('course-widgetbox_disabled');
    $("#CourseAddtional_Similar_NameContainer").removeClass("course-widgetbox").addClass('course-widgetbox_disabled');
    $("#CourseAvailabilityContainer").removeClass("course-widgetbox").addClass('course-widgetbox_disabled');
    $("#CourseCreditsContainer").removeClass("course-widgetbox").addClass('course-widgetbox_disabled');

    $("#coursedetailserror").css('display', 'block');
    $("#coursedetailserrorspacer").css('display', 'none');


    if (returnederror.indexOf("invalid") !== -1 || returnederror.indexOf("Please select") !== -1 || returnederror.indexOf("already") !== -1) {
        $("#coursedetailserror").html(returnederror);
    }
    if (returnederror.indexOf("Pricing") !== -1) {
        $("#CoursePriceContainer").removeClass("course-widgetbox").addClass('course-widgetbox_error');
    }
    if (returnederror.indexOf("Requisite") !== -1) {
        $("#CourseRequisiteContainer").removeClass("course-widgetbox").addClass('course-widgetbox_error');
    }
    if (returnederror.indexOf("material") !== -1) {
        $("#CourseMaterialsContainer").removeClass("course-widgetbox").addClass('course-widgetbox_error');
    }
    if (returnederror.indexOf("CourseChoice") !== -1) {
        $("#CourseChoiceContainer").removeClass("course-widgetbox").addClass('course-widgetbox_error');
    }
    if (returnederror.indexOf("This course has restricted access") !== -1) {
        $("#coursedetailserror").html(returnederror);
    }
    if (returnederror.indexOf("no space available. Please refresh") !== -1) {
        $("#coursedetailserror").html(returnederror);
    }
    else if (returnederror.indexOf("access") !== -1) {
        $("#accesscodeerror").html('*');
        var accesscodeval = $("#accesscode").val();
        $("#verificationresult").html("<div> Access Code: " + "****" + "<img src='/images/share/redx.png' /></div>");

    }

    if (returnederror.indexOf("credit") !== -1) {
        $("#CourseCreditsContainer").removeClass("course-widgetbox").addClass('course-widgetbox_error');
    }
    if ($("#instructor1").length > 0) {
        $("#instructor1").css('background', 'none');
    }
    if ($("#instructor2").length > 0) {
        $("#instructor2").css('background', 'none');
    }
    if ($("#instructor3").length > 0) {
        $("#instructor3").css('background', 'none');
    }


    //"#coursedetailserrorinfo").html(returnederror);
}
Cart.prototype.showExpandedDisplay = function () {
    var self = this;
    self.ShowReviewCheckout();
    //config.showDevelopmentInfo('Expanded display will come here');
}

Cart.prototype.empty = function () {
    var self = this;
    selectedCredit = [];
    var box = Ext.MessageBox.show({
        title: 'Empty cart',
        msg: 'Are you sure you want to remove all items from the cart?',
        buttons: Ext.MessageBox.YESNO,
        icon: Ext.MessageBox.QUESTION,
        buttonAlign: 'right',
        fn: function (buttonId, text, opt) {
            if (buttonId != 'yes') {
                self.miniDisplay('refresh');
                return;
            }

            Ext.Ajax.request({
                url: config.getUrl('public/cart/empty'),
                success: function (response) {
                    var info = Ext.decode(response.responseText);
                    self.elementInfo.setHtml(info.status);
                    self.updateCartStatus(response.responseText);
                    if (self.activeCheckoutStep == 'checkout') {
                        cart.HideReviewCheckout();
                    }
                    self.miniDisplay('refresh');
                    $('#reservationtimer').hide();
                }
            });
        }
    });
    self.popupHelper.configureForMessageBox(box, self.elementPopup);
}

Cart.prototype.refreshCourseListing = function () {
    var self = this;

    if (typeof (window.CourseSearchInstance) != 'undefined') {
        window.CourseSearchInstance.Invoke(false);
    }
}

Cart.prototype.backToBrowse = function (browse) {
    var self = this;

    if (typeof (browse) == 'undefined') {
        browse = false;
    }

    if (browse && !(LAYOUT.Options.currentController == 'course' && LAYOUT.Options.currentAction == 'browse')) {
        document.location = config.getUrl('public/course/browse');
        return;
    }

    LAYOUT.endCheckout();
    self.HideReviewCheckout();
    self.refreshDisplay();
    self.refreshCourseListing();
}


Cart.prototype.remove = function (courseId, cmd, subcmd) {
    var self = this;
    var alertBody;
    var e = Ext.get("remove-" + courseId);
    if (e != null && e.getAttribute("data-message") != null) {
        alertBody = decodeURIComponent(e.getAttribute("data-message"));
    }
    else {
        alertBody = "Are you sure you want to remove this course from the cart?";
    }

    var box = Ext.MessageBox.show({
        // animateTarget: 'cart-item-' + courseId,
        title: 'Remove course from cart',
        msg: alertBody,
        buttons: Ext.MessageBox.YESNO,
        icon: Ext.MessageBox.QUESTION,
        buttonAlign: 'right',
        /*
                listeners: {
                    close: function () {
                        self.keepPopup = false;
                    }
                },
        */
        fn: function (buttonId, text, opt) {
            if (buttonId != 'yes' && cmd != 'event') {
                self.miniDisplay('refresh');
                return;
            }
            if (!isNaN(cmd)) {
                Ext.Ajax.request({
                    url: config.getUrl('public/cart/RemoveStudentinCheckoutMultiple'),
                    params: {
                        studentId: cmd,
                        courseId: courseId
                    },
                    success: function (response) {
                        var result = Ext.decode(response.responseText);
                        self.updateCartStatus(response.responseText);
                        if (result.status == "Empty") {
                            cart.HideReviewCheckout();
                            $('#reservationtimer').hide();
                        }
                        else {
                            cart.checkout()
                        }
                        self.miniDisplay('refresh');
                        return;


                    }
                });
            }
            else {
                Ext.Ajax.request({

                    url: config.getUrl('public/cart/removecourse?courseId=' + courseId),
                    success: function (response) {
                        if (cmd == 'event') {
                            window.CourseSearchInstance.ShowCourseDetails(subcmd, 'childevent')
                            return;
                        }

                        self.updateCartStatus(response.responseText);

                        if (self.activeCheckoutStep == 'checkout') {
                            //self.hideDeletedCourse(courseId);
                            cart.checkout()
                            var result = Ext.decode(response.responseText);
                            if (result.status == "Empty") {
                                cart.HideReviewCheckout();
                                $('#reservationtimer').hide();
                            }
                        }
                        else if (self.activeCheckoutStep == 'payment') {
                            window.LAYOUT.MaskLayout('Loading');
                            self.checkout();
                        }
                        self.miniDisplay('refresh');

                        /* whoever put it here it was a bug i removed it, as after removing item from cart hides the search
                        var result = Ext.decode(response.responseText);

                        if (result.status == "Empty") {
                            cart.HideReviewCheckout();
                        }
                        */
                    }
                });
            }
        }
    });
    self.popupHelper.configureForMessageBox(box, 'cart-info-container');

}
var intervalhandler = null;
Cart.prototype.ReservationTimerHandler = function (countDownDate) {
    var self = this;
    clearInterval(intervalhandler);
    intervalhandler = setInterval(function () {
        $('#checkoutreservationtimer').css("visibility", "visible");
        // Get todays date and time
        var now = new Date().getTime();
        // Find the distance between now and the count down date
        var distance = countDownDate - now;

        // Time calculations for days, hours, minutes and seconds

        var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
        var seconds = Math.floor((distance % (1000 * 60)) / 1000);

        // Output the result in an element with id="demo"
        $('#reservationtimer').html("You now have " + minutes + "m " + seconds + "s " + "to complete your order.");
        $('#checkoutreservationtimer').html("You now have " + minutes + "m " + seconds + "s " + "to complete your order.");

        // If the count down is over, write some text 
        if (distance < 0) {
            clearInterval(intervalhandler);
            Ext.Ajax.request({
                url: config.getUrl('public/cart/empty'),
                success: function (response) {
                    var info = Ext.decode(response.responseText);
                    self.updateCartStatus(response.responseText);
                    if (self.activeCheckoutStep == 'checkout') {
                        cart.HideReviewCheckout();
                    }
                    self.miniDisplay('refresh');
                    $('#reservationtimer').hide();
                }
            });
            $('#reservationtimer').hide();
            alert("Reservation time has ran out. Please refresh for availability and add to cart again.");
        }
    }, 1000);
}
Cart.prototype.updateCartStatus = function (json) {
    var self = this;
    var result = Ext.decode(json);
    if (AllowAutoReserveCourseOrder == "true") {
        $('#reservationtimer').show();
        var countDownDate = new Date().getTime() + (ReserveOrderExpiry * 60000);
        Ext.Ajax.request({

            url: config.getUrl('public/cart/SetReservationExpiredTime?reservationexpiredtime=' + countDownDate),
            success: function (response) {
            }
        });
        this.ReservationTimerHandler(countDownDate);
        if (result.status !="Empty") {
            alert("You now have 30 minutes to complete your order");
        }
    }
    if (!result.success) {
        config.showWarning(result.error, 'Cart error');
        return;
    }

    self.elementInfo.setHtml(result.status);

    self.refreshCourseListing();
    self.refreshDisplay();
    self.setCartImage();

}

Cart.prototype.checkout = function (sort) {
    var self = this;
    self.CheckoutSort = sort;
    window.LAYOUT.MaskLayout('Loading');
    Ext.Ajax.request({
        url: config.getUrl('cart/validatecart'),
        success: function (response) {
            window.LAYOUT.UnmaskLayout();
            var messages = Ext.decode(response.responseText);
            if (messages.length > 0) {
                var message = messages.join('<br/>');
                Ext.MessageBox.show({
                    buttons: Ext.MessageBox.OK,
                    title: 'Your cart has been checked and changes occured',
                    msg: message,
                    icon: Ext.MessageBox.INFO
                });
            } else {
                self.ShowReviewCheckout();
            }
        }
    });
    //config.showDevelopmentInfo('Checkout will go here');
}




Cart.prototype.ShowRegisterUser = function () {
    var self = this;
    Ext.Ajax.request({
        url: config.getUrl('public/user/RegisterUser'),
        method: 'POST',
        success: function (response) {
            $("#dialogPopout").html(response.responseText);
            $("#dialogPopout").dialog({
                title: 'New Account',
                width: 840,
                position: { my: "center", at: "top", of: window },
                modal: true,
                show: {
                    duration: 500
                }


            });

            //var userdb = new PortalUserDashboard();
            //userdb.registration('addnew');

            //userdb.districtfieldlabel = '@ViewBag.districtfieldlabel';
            //userdb.schoolfieldlabel = '@ViewBag.schoolfieldlabel';
            //userdb.gradelevellabel = '@ViewBag.gradelevellabel';


        }
    });

}

Cart.prototype.ShowRegisterUserDisclaimer = function () {
    var self = this;
    Ext.Ajax.request({
        url: config.getUrl('public/user/Disclaimer'),
        success: function (response) {
            $("#dialogPopout").html(response.responseText);
            $("#dialogPopout").dialog({
                title: 'New Account',
                width: 840,
                position: { my: "center", at: "center", of: window },
                modal: true,
                show: {
                    duration: 500
                }


            });

        }
    });

}



Cart.prototype.CloseRegisterUserDisclaimer = function (redirectpage) {
    var self = this;
    $("#dialogPopout").dialog("close");

    self.setReloadCallback(self.getPostRegistrationAction());
    if (redirectpage != "" && redirectpage != "undefined" && redirectpage != undefined ) {
        window.location.href = config.getUrl(redirectpage);
    }
    else {
        window.location.href = config.getUrl('');
    }
}

Cart.prototype.SubmitRegistration = function (intCourseId) {
    var self = this;
    var userdb = new PortalUserDashboard();
    userdb.saveNewRegistration();

}


//*************************************************************
//                       checkout
//*************************************************************

Cart.prototype.ShowReviewCheckout = function (cmd) {
    var self = this;
    self.activeCheckoutStep = 'checkout';
    self.hideMiniDisplay();
    window.LAYOUT.startCheckout();
    CourseSearch.prototype.CloseCourseDetailsWindow();
    window.LAYOUT.MaskLayout('Loading');

    Ext.Ajax.request({
        url: config.getUrl('public/cart/CheckAuthorization'),
        success: function (response) {
            window.LAYOUT.UnmaskLayout();
            var result = Ext.decode(response.responseText);

            if (result.IsLoggedIn && (result.UserType != 'Student') && (result.UserType != 'Supervisor') && (result.UserType != 'Instructor') && (result.UserType != 'Admin') && (result.UserType != 'SubAdmin')) {
                Ext.MessageBox.show({
                    msg: 'We apologize for the inconvenience but checkout is only implemented for ' + Terminology.lower('students') + ' at the moment. For other user types, the feature is in development.',
                    title: 'Checkout information',
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.MessageBox.WARNING,
                    fn: function () {
                        self.backToBrowse();
                    }
                });
            }
            else if(result.IsCanvasSelectUser){

                return;
            } else if (result.IsLoggedIn) {
                window.LAYOUT.MaskLayout('Loading');
                Ext.Ajax.request({
                    url: config.getUrl('public/cart/checkout'),
                    params: { sort: self.CheckoutSort },
                    asynch: false,
                    success: function (response) {
                        if ((result.UserType != 'Supervisor') && (result.UserType != 'Instructor') && (result.UserType != 'Admin') && (result.UserType != 'SubAdmin')) {
                            Ext.Ajax.request({
                                url: config.getUrl('public/user/CheckReqMissingFields'),
                                success: function (missingfield) {
                                    if (missingfield.responseText == "NoMissingReqFields") {

                                        //window.LAYOUT.UnmaskLayout();  //Transfer the Unmasking inside ComputePriceCheckout(), to make sure that Price is computed before it allow student to submit checkout.


                                        self.elementCheckoutContainer.setHtml(response.responseText, true);
                                        //$('input, textarea').placeholder();
                                        self.activeCheckoutStep = 'checkout';
                                        self.ShowCheckoutContainerDisplay();
                                        self.ComputePriceCheckout();
                                        self.RefreshCourseStatus();
                                        try{
                                            if (window.top.location.toString().indexOf('PaypalRedirectConfirmation') > -1 && history.pushState) {
                                                history.pushState('', 'Paypal Confirmation', window.location.toString().replace("PaypalRedirectConfirmation", "PaypalCheckout") + "&Checkout=true");
                                            }
                                        }
                                        catch (e) { }


                                    } else {
                                        document.location = config.getUrl('public/user/dashboard?MissingReqFields=1');
                                    }
                                }
                            });
                        }

                        else {
                            self.elementCheckoutContainer.setHtml(response.responseText, true);
                            //$('input, textarea').placeholder();
                            self.activeCheckoutStep = 'checkout';
                            self.ShowCheckoutContainerDisplay();
                            self.ComputePriceCheckout();
                            self.RefreshCourseStatus();
                            try{
                                if (window.top.location.toString().indexOf('PaypalRedirectConfirmation') > -1 && history.pushState) {
                                    history.pushState('', 'Paypal Confirmation', window.location.toString().replace("PaypalRedirectConfirmation", "PaypalCheckout") + "&Checkout=true");
                                }
                            }
                            catch (e) { }
                        }


                    }
                });
            } else {


                Ext.Ajax.request({
                    url: config.getUrl('public/cart/login'),
                    success: function (response) {
                        self.elementCheckoutContainer.setHtml(response.responseText, true);
                        self.elementBodyTitle.setHtml('');
                        self.activeCheckoutStep = 'cartlogin';
                        self.ShowCheckoutContainerDisplay();

                    }
                });
                // Ext.MessageBox.show({
                // msg: 'Please log in to complete your checkout.',
                ////title: 'Cart Info',
                //buttons: Ext.MessageBox.OK,
                // icon: Ext.MessageBox.INFO,
                // });

                //membership.showLoginForm();
                //cart.HideReviewCheckout();

            }
        }
    });
}

Cart.prototype.HideReviewCheckout = function (cmd) {
    var self = this;
    window.LAYOUT.endCheckout();
    self.activeCheckoutStep = 'cart';
    self.elementCheckoutContainer.hide();
    self.elementMainCenter.setStyle('display', 'none');
    self.elementCheckoutContainer.setHtml('');
    self.elementBodyTitle.setHtml('Browse Courses');
    self.elementMainCenter.show();
}

Cart.prototype.ShowCheckoutContainerDisplay = function () {
    var self = this;
    self.elementMainCenter.hide();
    self.elementMainCenter.setStyle('display', 'none');
    //self.elementBodyTitle.setHtml('Checkout');
    self.elementCheckoutContainer.show();

}
var tempMateriallist = "";
var lastSelectedMaterial = "";
Cart.prototype.setOrignalSelectedMaterial = function () {
    lastSelectedMaterial = $('input[name=selectMaterialCheckout]:checked').val()
}
Cart.prototype.checkMaterial = function (cID, pID, Amt, qty, originalSelectedMat) {
    var self = this;
    var cAmt = parseFloat(Ext.util.Format.number(parseFloat(Amt), '0.00'));
    var elemid = cID + "-" + pID;
    var qty_element = $('#qty_' + cID + '-' + pID);
    var element_type = $('#chk' + elemid).attr('type');
    if ($('#chk' + elemid).is(':checked')) {
        //checked


        //if radio - if selection 1
        if (element_type == 'radio')
        {

         
            if (lastSelectedMaterial != '')
            {
                originalSelectedMat = lastSelectedMaterial;
            }
            if (originalSelectedMat != '' && originalSelectedMat != undefined && originalSelectedMat != null)
            {
                var originalPID = originalSelectedMat.split('|')[0];
                var originalPamount = originalSelectedMat.split('|')[1];
                var originalPqty = originalSelectedMat.split('|')[2];
                // alert($('#chk' + elemid).val())
                if (pID != originalPID) {

                    var originalElemid = cID + "-" + originalPID;
                    var originalCamt = parseFloat(Ext.util.Format.number(parseFloat(originalPamount), '0.00'));
                    qty_element = $('#qty_' + cID + '-' + originalPID);

                    //$('#tr' + originalElemid).fadeTo('slow', 0.4);
                    $('#price' + originalElemid).css("text-decoration", "line-through");
                 
                    var orginalSubtot = parseFloat(cAmt);
                    $('#matsubtot' + cID).val(Ext.util.Format.number(orginalSubtot, '0.00'));
                    //qty_element.attr('disabled', 'disabled');
                    qty_element.val(0)
                    tempMateriallist = tempMateriallist + ",-" + cID + "|" + originalPID + "|" + originalPqty;
                }
                else {
                    return;
                }
            }
        }

        //$('#tr' + elemid).fadeTo('slow', 1);
        $('#price' + elemid).css("text-decoration", "none");
        if (element_type == 'radio') {
            var subtot =  cAmt;
            $('#matsubtot' + cID).val(Ext.util.Format.number(subtot, '0.00'));
        }
        else {
            var subtot = parseFloat($('#matsubtot' + cID).val()) + cAmt;
            $('#matsubtot' + cID).val(Ext.util.Format.number(subtot, '0.00'));
        }
        //qty_element.removeAttr('disabled', 'disabled');
        qty_element.val(qty);
        //selectedmaterials
        tempMateriallist = tempMateriallist + "," + cID + "|" + pID + "|" + qty;
        lastSelectedMaterial = pID + "|" + cAmt + "|" + qty; // load the last selected one
    } else {
        //
        //$('#tr' + elemid).fadeTo('slow', 0.4);
        $('#price' + elemid).css("text-decoration", "line-through");
        var subtot = parseFloat($('#matsubtot' + cID).val()) - cAmt
        $('#matsubtot' + cID).val(Ext.util.Format.number(subtot, '0.00'));
        qty_element.attr('disabled', 'disabled');
        qty_element.val(0)
        //remove materials
        tempMateriallist = tempMateriallist + ",-" + cID + "|" + pID + "|" + qty;
    }
    self.ComputePriceCheckout();
    Ext.Ajax.request({
        url: config.getUrl('public/cart/GetAllMultipleStudentIdinCart'),
        params: {
            cid: cID
        },
        success: function (response) {
            var studentlist = response.responseText;
            var a = studentlist.split(","), i;

            for (i = 0; i < a.length; i++) {
                if ($('#chk' + elemid).is(':checked')) {
                    $('#' + a[i] + 'chk' + elemid).prop('checked', true);
                    //$('#' + +a[i] + 'tr' + elemid).fadeTo('slow', 1);
                }
                else {
                    $('#' + a[i] + 'chk' + elemid).prop('checked', false);
                    //$('#' + +a[i] + 'tr' + elemid).fadeTo('slow', 0.4);
                }
            }
        }
    });
}

Cart.prototype.calculateMaterialPerQty = function (cID, pID, qty_control, price_control, orig_price) {
    e = qty_control;
    if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            (e.keyCode == 65 && e.ctrlKey === true) || //Ctrl+A
            (e.keyCode == 67 && e.ctrlKey === true) || //Ctrl+C
            (e.keyCode == 88 && e.ctrlKey === true) || //Ctrl+X
            (e.keyCode >= 35 && e.keyCode <= 39)) { // Allow: home, end, left, right
        return;
    }
    if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
        e.preventDefault();
    }
    if (parseInt($(qty_control).val()) < 0 || isNaN($(qty_control).val()) || $(qty_control).val() == '') {
        $(qty_control).val(1);
        return;
    }
    Cart.prototype.delay(function () {
        if (qty_control != '' && price_control != '') {

            var cAmt = parseFloat(Ext.util.Format.number(parseFloat(orig_price), '0.00'));
            var qty_val = $(qty_control).val();
            var total = qty_val * orig_price;
            var current_total_line_price = parseFloat($('#' + price_control).text().replace('$', ''));
            var total_diff_curr_line_price = total - current_total_line_price;
            $('#' + price_control).text('$' + total.toFixed(2)); // CALCULATE THE TOTAL




            //SUB TOTAL AND TOTAL
            var sub_total = $('#CheckoutSubtotal').text().replace('$', '');
            var grand_total = $('#CheckoutTotal').text().replace('$', '');

            var sub_total_final = parseFloat(Ext.util.Format.number((total_diff_curr_line_price), '0.00')) + parseFloat(Ext.util.Format.number(sub_total, '0.00'));
            $('#CheckoutSubtotal').text('$' + parseFloat(sub_total_final).toFixed(2));
            $('#CheckoutTotal').text('$' + parseFloat(sub_total_final).toFixed(2));
            $('#hiddenPaymentTotal').val(parseFloat(sub_total_final).toFixed(2));
            //temp material list
            tempMateriallist = tempMateriallist + "," + cID + "|" + pID + "|" + qty_val;
        }
    }, 100);

}
Cart.prototype.delay = (function () {
    var timer = 0;
    return function (callback, ms) {
        clearTimeout(timer);
        timer = setTimeout(callback, ms);
    };
})();

Cart.prototype.updatePartialPayment = function (cID) {
    var self = this;
    Ext.Ajax.request({
        url: config.getUrl('public/cart/UpdatePartialPayment'),
        params: {
            intCourseId: cID,
            ppval: $('#PartlPaymnt' + cID).is(':checked')
        },

        success: function (response) {
            var model = Ext.decode(response.responseText);
            var courses = model.items;
            self.ComputePriceTotal(courses,"",0);
            cart.ApplyAutomaticCouponDiscount();
        }
    });

}

Cart.prototype.updateCRExtraParticipant = function (cID) {
    var self = this;
    Ext.Ajax.request({
        url: config.getUrl('public/cart/UpdateCRExtraParticipant'),
        params: {
            intCourseId: cID,
            val: $('#CRExtraParticipant' + cID).val()
        }

        //success: function (response) {
        //}
    });

}

var totalmatprice = 0;
var totalcourseprice = 0;
var totalpayment = 0;
var materialtotal =0;
Cart.prototype.ComputePriceCheckout = function () {
    console.log("dfgf")
    var self = this;
    Ext.Ajax.request({
        url: config.getUrl('public/cart/GetCartList'),
        success: function (response) {
            var model = Ext.decode(response.responseText);
            var courses = model.items;
            var multiplestudent = model.mutiplestudent;
            var taxpercent = model.taxpercent;
            self.ComputePriceTotal(courses, multiplestudent, taxpercent);
            cart.ApplyAutomaticCouponDiscount();
        },
        failure: function () {
            alert("There's no more space in one of your class.Class were removed from the Cart.")
            Ext.Ajax.request({
                url: config.getUrl('public/cart/ValidateMaxEnrollment'),
                success: function (response) {
                    var model = Ext.decode(response.responseText);
                    var noofcourse = model.noofcourse;
                    if (noofcourse > 0) {
                        cart.checkout();
                    }
                    else {

                        document.location = "";
                    }
                },
                failure: function () {
                    alert("There's no more space in one of your class.Class were removed from the Cart.")
                    cart.checkout();
                }
            });
        }
    });
}

Cart.prototype.ComputePriceCheckoutAfterAutoCouponDiscount = function () {
    var self = this;
    Ext.Ajax.request({
        url: config.getUrl('public/cart/GetCartList'),
        success: function (response) {
            var model = Ext.decode(response.responseText);
            var courses = model.items;
            var multiplestudent = model.mutiplestudent;
            var taxpercent = model.taxpercent;

            self.ComputePriceTotal(courses, multiplestudent, taxpercent, taxpercent);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
        }
    });
}

Cart.prototype.ComputePriceTotal = function (courses, multiplestudent,taxpercent) {
    var self = this;
    totalmatprice = 0;
    totalcourseprice = 0;
    totalpayment = 0;
    nontaxablemat = 0;
    var remainingAmountonCourses = 0;
    var totaldisc = $('#hiddenOrderDiscountTotal').val() - 0;
    for (dtai in courses) {
        if (multiplestudent.length > 0) { //Supervisor Enrollment for multiple student
            for (stud in multiplestudent) {
                var course = courses[dtai];
                if (multiplestudent[stud].CourseId == course.CourseId) {

                    //******** Note: Below line can't be transfer to another Function without adding delay on succeeding actions. Assync property of js will give an error on displaying the actual cost.
                    // alert(multiplestudent[stud].PricingModel.EffectivePrice);
                    var effectiveprice = 0;
                    if (multiplestudent[stud].CourseTotal == null) {
                        effectiveprice = 0;
                    }
                    else {
                        effectiveprice = multiplestudent[stud].CourseTotal;
         
                    }
                    var courseprice = effectiveprice + course.MateriaTotal;
                    var nontaxablemat_partial = course.NonTaxableMateriaTotal;
                    var matprice = parseFloat($('#matsubtot' + course.CourseId).val());
                    var coursematprice = courseprice + matprice;
                    if (course.IsPartialPayment) {

                        if (course.PartialPayment > coursematprice) {
                            totalpayment += coursematprice;
                        } else {
                            totalpayment += course.PartialPayment;
                            remainingAmountonCourses += (coursematprice - course.PartialPayment); // compute the remaining Amount after Partial Payment
                        }
                    } else {
                        totalpayment += coursematprice;
                    }

                    totalcourseprice += courseprice;
                    totalmatprice += matprice;
                    nontaxablemat += nontaxablemat_partial;
                    //*******End
                }
            }
        }
        else { // Regular Student Enrollment
            // ******* Below Line is almost a copy of above code computation for supervisor. But making it one inside one function will gives a computation error. "Assync Js Property"
            //******** Note: Below line can't be transfer to another Function without adding delay on succeeding actions. Assync property of js will give an error on displaying the actual cost.

            var course = courses[dtai];
            var courseprice = course.CourseTotal;
            var nontaxablemat_partial = course.NonTaxableMateriaTotal;
            var matprice = parseFloat($('#matsubtot' + course.CourseId).val());
            var coursematprice = courseprice + matprice;
            if (matprice == 0) {
                matprice = course.MateriaTotal
            }
            if (course.IsPartialPayment) {

                if (course.PartialPayment > coursematprice) {
                    totalpayment += coursematprice;
                } else {
                    totalpayment += course.PartialPayment;
                    remainingAmountonCourses += (coursematprice - course.PartialPayment); // compute the remaining Amount after Partial Payment
                }
            } else {
                totalpayment += coursematprice;
            }

            totalcourseprice += courseprice;
            totalmatprice += matprice;
            materialtotal += course.MateriaTotal;
            nontaxablemat += nontaxablemat_partial;

            //End
        }

    }
    var CheckoutSubtotal = totalmatprice + totalcourseprice;
    var CheckoutGrandTotal = CheckoutSubtotal - totaldisc;
    var PaymentTotal = totalpayment - totaldisc;
    var SaletaxTotal = ((PaymentTotal - nontaxablemat) * taxpercent) / 100;
    PaymentTotal = PaymentTotal + SaletaxTotal;
    CheckoutGrandTotal = CheckoutGrandTotal + SaletaxTotal;
    if (PaymentTotal < 0) { //this is for negative payment total(After deducting the discount). Discount Amoumt is greater than Payment Amount.
        remainingAmountonCourses = remainingAmountonCourses + PaymentTotal; //the excess discount will be deducted to Remaining Amount.
        PaymentTotal = 0; // Payment Amount will be zero.

    }
    var RemainingAmount = remainingAmountonCourses;
    $('#CheckoutDiscount').html(Ext.util.Format.currency(totaldisc));
    $('#CheckoutSubtotal').html(Ext.util.Format.currency(CheckoutSubtotal));
    $('#CheckoutTotal').html(Ext.util.Format.currency(CheckoutGrandTotal));
    $('#SalesTaxTotal').html(Ext.util.Format.currency(SaletaxTotal));
    $('#PartialPaymentTD').html(Ext.util.Format.currency(PaymentTotal));
    $('#RemainingAmountTD').html(Ext.util.Format.currency(RemainingAmount));

    if (RemainingAmount <= 0) {
        $('#PartialPaymentTR').hide();
        $('#RemainingAmountTR').hide();
    } else {
        $('#PartialPaymentTR').show('slow');
        $('#RemainingAmountTR').show('slow');
    }

    $('#hiddenPaymentTotal').val(PaymentTotal);
    $('#hiddenOrderTotal').val(CheckoutGrandTotal);
    $('#hiddenRemainingAmount').val(RemainingAmount);
    $('#hiddenSalesTaxTotal').val(SaletaxTotal);

    if (PaymentTotal == 0) {
        $('#ConfirmCheckoutButton').html('Proceed to Next Step');
    } else {
        $('#ConfirmCheckoutButton').html('Proceed to Payment');
    }

}

Cart.prototype.ComputePriceCheckoutXX = function () {
    totalmatprice = 0;
    totaldisc = $('#hiddenOrderDiscountTotal').val() - 0;
    Ext.Ajax.request({
        url: config.getUrl('public/cart/GetCartCourseSubTotal'),
        success: function (response) {
            totalcourseprice = parseFloat(response.responseText);


            Ext.Ajax.request({
                url: config.getUrl('public/cart/GetCartCourseIDList'),
                success: function (response) {
                    var CourseIDs = response.responseText.split(",");
                    for (var dtai = 0; dtai < CourseIDs.length; dtai++) {
                        totalmatprice += parseFloat($('#matsubtot' + CourseIDs[dtai]).val());
                    }
                    $('#CheckoutSubtotal').html(Ext.util.Format.currency(totalmatprice + totalcourseprice));
                    $('#CheckoutTotal').html(Ext.util.Format.currency((totalmatprice + totalcourseprice) - totaldisc));
                    $('#hiddenShowConfirmationPage').val((totalmatprice + totalcourseprice) - totaldisc);
                    if ((totalmatprice + totalcourseprice) == 0) {
                        $('#ConfirmCheckoutButton').html('Proceed to Next Step');
                    } else {
                        $('#ConfirmCheckoutButton').html('Proceed to Payment');
                    }

                    cart.ApplyAutomaticCouponDiscount();
                }
            });
        }
    });

    window.LAYOUT.UnmaskLayout();
}
Cart.prototype.hideDeletedCourse = function (cID) {
    var self = this;
    $('.tr1' + cID).fadeOut('slow');
    $('#matsubtot' + cID).val(Ext.util.Format.number(0, '0.00'));
    self.ComputePriceCheckout();
}

Cart.prototype.RefreshCourseStatus = function () {
}
//Cart.prototype.RefreshCourseStatus = function () {
//    var self = this;
//    Ext.Ajax.request({
//        url: config.getUrl('public/cart/GetCartCourseIDList'),
//        success: function (response) {
//            var CourseIDs = response.responseText.split(",");
//            if (response.responseText.length < 2) {

//                Ext.MessageBox.show({
//                    msg: 'Your cart is empty. This will now close checkout and redirect to course browsing.',
//                    title: 'Checkout',
//                    buttons: Ext.MessageBox.OK,
//                    icon: Ext.MessageBox.INFO
//                });


//                cart.HideReviewCheckout();
//                cart.refreshDisplay();
//                cart.refreshCourseListing();
//                self.elementInfo.setHtml('Empty');
//            } else {

//                if (CourseIDs.length == 1) {
//                    self.elementInfo.setHtml('1 item');
//                }
//                else {
//                    self.elementInfo.setHtml(CourseIDs.length + ' items');
//                }


//            }
//        }
//    });
//}

Cart.prototype.GetPreReqConfig = function () {
    return $.ajax({
        url: config.getUrl('public/cart/GetPreRequisiteConfig'),
        type: 'json',
        method: 'GET'
    });
};

Cart.prototype.GetPreRequistes = function (courseId) {
    return $.ajax({
        url: config.getUrl('public/Course/GetCoursePrerequisites'),
        type: 'json',
        method: 'POST',
        data: {
            courseId : courseId
        }
    });
};

Cart.prototype.ConfirmCheckout = function (cmd, roomatealert) {
    if (roomatealert == 1) {
        var roomatehasvalue = false;
        $(".roomatechecker").each(function () {
            var element = $(this);
            if (element.val().length > 0) {
                $(".roomatesavedyet").each(function () {
                    var savedelement = $(this);
                    if ('saved' + element.attr('id') == savedelement.attr('id')) {
                        if (savedelement.val() == "1") {
                            roomatehasvalue = false;
                        } else {
                            roomatehasvalue = true;
                        }
                    }
                });                
            }
        });
        if (roomatehasvalue == true) {
            alert("You have unsaved roommate request information. Please save or clear out before continuing.");
            return;
        }
    }

    var self = this;
    var currentUserStudentId = !isNaN(cmd) ? cmd : 0;
    //final recheck before saving
    if ($.validator)
    {
        $.validator.messages.required = "";
    }

    //do course prereq checking
    var courseIdsRemoved = [];
    var coursePrereqIsValid = true;
    var prerequisiteInvalidMessage = '';
    var prerequisiteOk = true;
    Ext.Ajax.request({
        url: config.getUrl('public/cart/GetPreRequisiteConfig'),
        success: function (response) {
            var isAdvance = response.responseText;
            try{
                var adminIframe = top.document.getElementById("enrollment-window-iframe");
            }

            catch (e) { }
            if (isAdvance == "True" && !adminIframe) {
                var asyncPrereqValidate = cart.CoursePrereqValidate(currentUserStudentId).then(function (result) {
                    return new Promise(function (resolve, reject) {
                        result.map(function (result) {
                            var validationResult = result;
                            if(validationResult.isValid==false){
                                prerequisiteOk = false;
                            }
                            prerequisiteMessage = validationResult.responseMessage;
                            if (validationResult && validationResult.isValid === false) {

                                // remove the course from cart course list
                                var asynRemoveCourse = new Promise(function (resolve) {
                                    var counter = 0;
                                    if (validationResult.data.length > 0) {
                                        validationResult.data.map(function (item) {
                                            var courseId = item.CourseId;
                                            cart.RemoveCourseFromCartList(courseId).then(function () {
                                                if (courseIdsRemoved.indexOf(courseId) === -1) {
                                                    courseIdsRemoved.push(courseId);
                                                }
                                                counter++;

                                                //only resolve when all of the results have been checked
                                                if (counter === (validationResult.data.length)) {
                                                    resolve(true);
                                                }
                                            });
                                        });
                                    }
                                    resolve(true);
                                });
                                //wait until the asynRemoveCourse is done before executing the response data
                                asynRemoveCourse.then(function () {
                                    if (courseIdsRemoved.length > 0) {

                                        Ext.MessageBox.show({
                                            msg: 'Courses [ ' + courseIdsRemoved.join(',') + ' ] were removed from the Cart.',
                                            title: 'Info',
                                            icon: Ext.MessageBox.INFO
                                        });
                                    }

                                    setTimeout(function () {
                                        //pause for 3 second to ease transition
                                        resolve(false);
                                    }, 3000);

                                });
                            }
                            else {
                                resolve(true);
                            }
                        });
                    });
                });
                //wait until all of the requests above are done before prompting this message
                asyncPrereqValidate.then(function (result) {

                    window.LAYOUT.UnmaskLayout();

                    if (result === false) {

                        Ext.MessageBox.show({
                            msg: prerequisiteMessage,
                            title: 'Error',
                            buttons: Ext.MessageBox.OK,
                            icon: Ext.MessageBox.ERROR
                        });
                        cart.HideReviewCheckout();
                        return;
                    }
                    try{
                        $("#checkoutform").validate({
                            debug: true,
                            rules: {
                                Prerequisite: { required: true },
                                alternatecheckoutradio: { required: true, equalTo: "#chkYes" },
                                CheckOutQuestion: { required: true }
                            },
                            ignore: '.ignore, :hidden',
                            messages: {
                                alternatecheckoutradio: { equalTo: '' }
                            }

                        });
                    }
                    catch(e){
                    }

                    var CRExtraParticipant = true;
                    var CRExtraParticipantInfo = $('.CRExtraParticipant-required');
                    var ExtraParticipantLabel = $("#ExtraParticipantLabel").val();
                    var ErrInfoElement = null;
                    CRExtraParticipantInfo.each(function (index, element) {
                        var infoElement = $(element);
                        if (infoElement.val().length == 0) {
                            ErrInfoElement = infoElement;
                            CRExtraParticipant = false;
                            return;
                        }
                    });

                    if (CRExtraParticipant == false) {
                        ErrInfoElement.focus();
                        Ext.MessageBox.show({
                            msg: 'It is required to fill the ' + ExtraParticipantLabel + '.',
                            title: 'Error',
                            buttons: Ext.MessageBox.OK,
                            icon: Ext.MessageBox.ERROR
                        });
                        return;
                    }


                    var materialsOk = true;

                    var materialInfo = $('.material-required');
                    materialInfo.each(function (index, element) {
                        var infoElement = $(element);
                        var required = infoElement.val() == 'true';
                        if (!required) {
                            return;
                        }

                        var courseId = infoElement.data('course-id');

                        var materials = $('[data-material-course=' + courseId + ']');
                        var hasMaterialChecked = false;
                        materials.each(function (index, element) {
                            var materialElement = $(element);
                            if (materialElement.is(':checked')) {
                                hasMaterialChecked = true;
                            }
                        });
                        if (!hasMaterialChecked) {
                            materialsOk = false;
                        }
                    });
                    try{
                        if ($("#checkoutform").valid() && materialsOk) {
                            if(prerequisiteOk){
                                Ext.Ajax.request({
                                    url: config.getUrl('public/cart/ValidateMaxEnrollment'),
                                    success: function (response) {
                                        var model = Ext.decode(response.responseText);
                                        var noofcourse = model.noofcourse;
                                        if (noofcourse > 0) {
                                            self.SubmitCheckout();
                                        }
                                        else {
                                            alert("There's no more space in one of your class.Class were removed from the Cart.")
                                            document.location = "";
                                        }
                                    },
                                    failure: function () {
                                        alert("There's no more space in one of your class.Class were removed from the Cart.")
                                        cart.checkout();
                                    }
                                });

                            }
                            else
                            {
                                Ext.MessageBox.show({
                                    msg: 'Course(s) were removed from your cart because you did not meet the pre-requisite.'  ,
                                    title: 'Error',
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.ERROR
                                });
                            }
                        } else {

                            Ext.MessageBox.show({
                                msg: 'Please check the fields which are marked in red and correct these.',
                                title: 'Error',
                                buttons: Ext.MessageBox.OK,
                                icon: Ext.MessageBox.ERROR
                            });
                        }
                    }
                        catch(e){}
                });
            }
            else {
                try{
                    $("#checkoutform").validate({
                        debug: true,
                        rules: {
                            Prerequisite: { required: true },
                            alternatecheckoutradio: { required: true, equalTo: "#chkYes" },
                            CheckOutQuestion: { required: true }
                        },
                        ignore: '.ignore, :hidden',
                        messages: {
                            alternatecheckoutradio: { equalTo: '' }
                        }

                    });
                }
                catch(e){}

                var CRExtraParticipant = true;
                var CRExtraParticipantInfo = $('.CRExtraParticipant-required');
                var ExtraParticipantLabel = $("#ExtraParticipantLabel").val();
                var ErrInfoElement = null;
                CRExtraParticipantInfo.each(function (index, element) {
                    var infoElement = $(element);
                    if (infoElement.val().length == 0) {
                        ErrInfoElement = infoElement;
                        CRExtraParticipant = false;
                        return;
                    }
                });

                if (CRExtraParticipant == false) {
                    ErrInfoElement.focus();
                    Ext.MessageBox.show({
                        msg: 'It is required to fill the ' + ExtraParticipantLabel + '.',
                        title: 'Error',
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.ERROR
                    });
                    return;
                }


                var materialsOk = true;

                var materialInfo = $('.material-required');
                materialInfo.each(function (index, element) {
                    var infoElement = $(element);
                    var required = infoElement.val() == 'true';
                    if (!required) {
                        return;
                    }

                    var courseId = infoElement.data('course-id');

                    var materials = $('[data-material-course=' + courseId + ']');
                    var hasMaterialChecked = false;
                    materials.each(function (index, element) {
                        var materialElement = $(element);
                        if (materialElement.is(':checked')) {
                            hasMaterialChecked = true;
                        }
                    });
                    if (!hasMaterialChecked) {
                        materialsOk = false;
                    }
                });

                if ($("#checkoutform").valid() && materialsOk) {
                    Ext.Ajax.request({
                        url: config.getUrl('public/cart/ValidateMaxEnrollment'),
                        success: function (response) {
                            var model = Ext.decode(response.responseText);
                            var noofcourse = model.noofcourse;
                            if (noofcourse > 0) {
                                self.SubmitCheckout();
                            }
                            else {
                                alert("There's no more space in one of your class.Class were removed from the Cart.")
                                document.location = "";
                            }
                        },
                        failure: function () {
                            alert("There's no more space in one of your class.Class were removed from the Cart.")
                            cart.checkout();
                        }
                    });
                } else {

                    Ext.MessageBox.show({
                        msg: 'Please check the fields which are marked in red and correct these.',
                        title: 'Error',
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.ERROR
                    });
                }
            }
        }
    });

    window.LAYOUT.UnmaskLayout();
}

Cart.prototype.GetCartList = function () {
    var self = this;
    var cartList = [];

    window.LAYOUT.MaskLayout('Getting courses from Cart...');

    var getCartListrequest = $.ajax({
        url: config.getUrl('public/Cart/GetCartList'),
        type: 'json',
        method: 'GET'
    });
    return new Promise(function (resolve) {
        getCartListrequest.done(function (response) {
            if (response.success)
            {
                var items = response.items;
                cartList = response;
            }
            resolve(cartList);
        });
    });
}

Cart.prototype.CoursePrereqValidate = function (currentUserStudentId) {
    var self = this;

    window.LAYOUT.MaskLayout('Checking course prerequisite(s)...');

    return new Promise(function (resolve) {
        cart.GetCartList().then(function (response) {
            var validatedItems = [];
            var items = response.items;
            var multipleStudents = response.mutiplestudent;
            var counter = 0;
            if (items.length > 0 && currentUserStudentId > 0) { //if the student is accessing
                items.map(function (item) {
                    var studentId = item.StudentId > 0 ? item.StudentId : currentUserStudentId;
                    var validateRequest = $.ajax({
                        url: config.getUrl('public/Course/UnattendedCoursesForCourseByPreReq') + '?courseId=' + item.CourseId + '&studentId=' + studentId,
                        type: 'json',
                        method: 'GET'
                    });

                    window.LAYOUT.MaskLayout('Checking course prerequisite(s)...');

                    validateRequest.done(function (response) {
                        window.LAYOUT.UnmaskLayout();
                        var data = response;
                        validatedItems.push(data);
                        counter++;
                        //only resolve when all of the results have been checked
                        if (counter === (items.length)) {
                            resolve(validatedItems);
                        }
                    });
                });
            }
            else if (multipleStudents.length > 0){ //if supervisor is accessing and doing multiple enroll
                multipleStudents.map(function (item) {
                    var studentId = item.StudentId;
                    var validateRequest = $.ajax({
                        url: config.getUrl('public/Course/UnattendedCoursesForCourseByPreReq') + '?courseId=' + item.CourseId + '&studentId=' + studentId,
                        type: 'json',
                        method: 'GET'
                    });


                    window.LAYOUT.MaskLayout('Checking course prerequisite(s)...');

                    validateRequest.done(function (response) {
                        window.LAYOUT.UnmaskLayout();
                        var data = response;
                        validatedItems.push(data);
                        counter++;

                        //only resolve when all of the results have been checked
                        if (counter === (items.length)) {
                            resolve(validatedItems);
                        }
                    });
                });

            }
        });

    });

}

Cart.prototype.RemoveCourseFromCartList = function (courseid) {
    var self = this;

    return new Promise(function (resolve, reject) {
        var removeRequest = $.ajax({
            url: config.getUrl('public/cart/removecourse?courseId=' + courseid),
            type: 'json',
            method: 'GET'
        });

        window.LAYOUT.MaskLayout('Removing Course : ' + courseid + '...');

        removeRequest.done(function (response) {
            self.updateCartStatus(Ext.encode(response));
            if (self.activeCheckoutStep == 'checkout') {
               // cart.checkout()
                var result = response;
                if (result.status == "Empty") {
                    cart.HideReviewCheckout();
                }
                else {
                    cart.checkout()
                }
            }
            else if (self.activeCheckoutStep == 'payment') {
                window.LAYOUT.MaskLayout('Loading');
                self.checkout();
            }
            //self.miniDisplay('refresh');

            resolve(true);
        });
    });
}

Cart.prototype.SubmitCheckout = function (cmd) {
    var self = this;
    var mat = tempMateriallist;
    var discountcode = $("#txtCoupon").val();
    var coupondiscount = $('#hiddenOrderDiscountTotal').val();
    var strOrderTotal = $('#hiddenOrderTotal').val();
    var strPaymentTotal = $('#hiddenPaymentTotal').val();
    var strSalestaxTotal = $('#hiddenSalesTaxTotal').val();
    var markedaspaidinfull = 0;
    window.LAYOUT.MaskLayout('Loading');

    if ($('#paidinfull').is(':checked')) {

        markedaspaidinfull = 1;

    }
    else {
        markedaspaidinfull = 0;

    }
    Ext.Ajax.request({
        url: config.getUrl('public/cart/SubmitCheckout'),
        params: {
            CheckoutComments: $("#CheckoutComments").val(),
            CheckOutQuestion: $("#CheckOutQuestion").val(),
            MaterialList: tempMateriallist,
            discountcode: discountcode,
            coupondiscount: coupondiscount,
            SalesTaxTotal: strSalestaxTotal,
            MarkedAsPaidInFull: markedaspaidinfull
        },
        success: function (response) {
            window.LAYOUT.UnmaskLayout();
            var OrderNumber = response.responseText;
            var responseintext = response.responseText;
            if (responseintext.indexOf("timed out") != -1) {
                alert(responseintext);
                window.location = "";
            };
            tempMateriallist="";
            self.ShowPaymentPage(strPaymentTotal, OrderNumber, "", "checkout");
        }
    });
}



var discountdetails = "No discount.";
var totalDiscount = "$0.00";
Cart.prototype.ApplyCouponDiscount = function (cmd, clix) {
    var self = this;
    if (cmd.which == 13 || clix == 1) {
        coupon = $("#txtCoupon").val();
        Ext.Ajax.request({
            url: config.getUrl('public/cart/applycoupondiscount'),
            params: {
                couponcode: coupon
            },
            success: function (response) {
                var resultdiscount = Ext.decode(response.responseText);
                var result = resultdiscount.status;
                var dollardiscount = resultdiscount.dollardiscount;
                discountdetails = Ext.util.Format.currency(dollardiscount) + " and " + resultdiscount.percentdiscount + "% Off";
                var materialdiscounted = resultdiscount.materialdicounted;
                if (result == 'approved') {
                    if (materialdiscounted == "0") {

                        var percentdiscount = (totalcourseprice-materialtotal) * (resultdiscount.percentdiscount / 100);
                        materialtotal = 0;
                        //$('#CheckoutTotal').html(Ext.util.Format.currency(totalmatprice + ((totalcourseprice - dollardiscount) - percentdiscount)));
                        //$('#CheckoutDiscount').html(Ext.util.Format.currency(dollardiscount - 0 + percentdiscount - 0));
                        //$('#hiddenOrderTotal').val(totalmatprice + ((totalcourseprice - dollardiscount) - percentdiscount));
                        //$('#hiddenOrderDiscountTotal').val(dollardiscount - 0 + percentdiscount - 0)
                        //totalDiscount = Ext.util.Format.currency((dollardiscount - 0) + (percentdiscount - 0));
                    }
                    else {
                        var percentdiscount = (totalmatprice + totalcourseprice) * (resultdiscount.percentdiscount / 100);
                        //$('#CheckoutTotal').html(Ext.util.Format.currency((((totalmatprice + totalcourseprice) - dollardiscount) - percentdiscount)));
                        //$('#CheckoutDiscount').html(Ext.util.Format.currency(dollardiscount - 0 + percentdiscount - 0));
                        //$('#hiddenOrderTotal').val((((totalmatprice + totalcourseprice) - dollardiscount) - percentdiscount));
                        //$('#hiddenOrderDiscountTotal').val(dollardiscount - 0 + percentdiscount - 0)
                        //totalDiscount = Ext.util.Format.currency((dollardiscount - 0) + (percentdiscount - 0));
                    }

                    $('#hiddenOrderDiscountTotal').val(dollardiscount - 0 + percentdiscount - 0)
                    totalDiscount = Ext.util.Format.currency((dollardiscount - 0) + (percentdiscount - 0));
                    self.ComputePriceCheckout();


                }
                else {
                    discountdetails = resultdiscount.status;
                    Ext.MessageBox.show({
                        msg: resultdiscount.status,
                        title: 'Error',
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.ERROR
                    });

                    //$('#CheckoutTotal').html(Ext.util.Format.currency(totalmatprice + totalcourseprice));
                    //$('#CheckoutDiscount').html(Ext.util.Format.currency(0));
                    // $('#hiddenOrderTotal').val(totalmatprice + totalcourseprice);
                    $('#hiddenOrderDiscountTotal').val(0);
                    self.ComputePriceCheckout();
                }
            }
        });
    }
}

Cart.prototype.ApplyAutomaticCouponDiscount = function () {
    var self = this;
    coupon = $("#txtCoupon").val();
    var isAdmin = $("#hiddenOrderDiscountTotal").attr("data-isadmin");
    console.log("isAdmin")
    console.log(isAdmin)
    if (coupon != "" && typeof coupon !== "undefined" && isAdmin != true) {
        Ext.Ajax.request({
            url: config.getUrl('public/cart/applycoupondiscount'),
            params: {
                couponcode: coupon
            },
            success: function (response) {
                var resultdiscount = Ext.decode(response.responseText);
                var result = resultdiscount.status;
                if (result == 'Stack') {
                    var ids = resultdiscount.courseidlist;
                    var coupons = resultdiscount.couponidlist;
                    var couponsplit = coupons.split("|");
                    var idsplit = ids.split("|");
                    for (i = 0; i < idsplit.length; i++) {
                        if (idsplit[i] != "0") {
                            $('#txtcoupon' + idsplit[i]).val(couponsplit[i]);

                            self.ApplyCouponDiscountPerOrder('event', idsplit[i], 1, 0);
                        }
                    }

                }
                var dollardiscount = resultdiscount.dollardiscount;
                discountdetails = Ext.util.Format.currency(dollardiscount) + " and " + resultdiscount.percentdiscount + "% Off";
                var materialdiscounted = resultdiscount.materialdicounted;
                if (result == 'approved') {
                    if (materialdiscounted == "0") {

                        var percentdiscount = (totalcourseprice-materialtotal) * (resultdiscount.percentdiscount / 100);
                        materialtotal = 0;
                        //$('#CheckoutTotal').html(Ext.util.Format.currency(totalmatprice + ((totalcourseprice - dollardiscount) - percentdiscount)));
                        //$('#CheckoutDiscount').html(Ext.util.Format.currency(dollardiscount - 0 + percentdiscount - 0));
                        //$('#hiddenOrderTotal').val(totalmatprice + ((totalcourseprice - dollardiscount) - percentdiscount));
                        //$('#hiddenOrderDiscountTotal').val(dollardiscount - 0 + percentdiscount - 0)

                        totalDiscount = Ext.util.Format.currency((dollardiscount - 0) + (percentdiscount - 0));
                    }
                    else {
                        var percentdiscount = (totalmatprice + totalcourseprice) * (resultdiscount.percentdiscount / 100);
                        //$('#CheckoutTotal').html(Ext.util.Format.currency((((totalmatprice + totalcourseprice) - dollardiscount) - percentdiscount)));
                        //$('#CheckoutDiscount').html(Ext.util.Format.currency(dollardiscount - 0 + percentdiscount - 0));
                        //$('#hiddenOrderTotal').val((((totalmatprice + totalcourseprice) - dollardiscount) - percentdiscount));
                        //$('#hiddenOrderDiscountTotal').val(dollardiscount - 0 + percentdiscount - 0)
                        totalDiscount = Ext.util.Format.currency((dollardiscount - 0) + (percentdiscount - 0));
                    }

                    $('#hiddenOrderDiscountTotal').val(dollardiscount - 0 + percentdiscount - 0)
                    self.ComputePriceCheckoutAfterAutoCouponDiscount();

                }
                else {
                    discountdetails = resultdiscount.status;
                    //$('#CheckoutTotal').html(Ext.util.Format.currency(totalmatprice + totalcourseprice));
                    //$('#CheckoutDiscount').html(Ext.util.Format.currency(0));
                    //$('#hiddenOrderTotal').val(totalmatprice + totalcourseprice);
                    $('#hiddenOrderDiscountTotal').val(0);
                    self.ComputePriceCheckoutAfterAutoCouponDiscount();
                }

            }
        });
    }

    window.LAYOUT.UnmaskLayout();
}

Cart.prototype.ApplyRoomRequest = function (cid) {
    var self = this;
    var roommatename = $("#roommatename" + cid).val();
    var roommategender = $("#roommategender" + cid).val();
    var roommatecommute = $("#roommatecommute" + cid).val();
    Ext.Ajax.request({
        url: config.getUrl('public/cart/ApplyRoomMateRequest'),
        params: {
            roommatename: roommatename,
            roommategender: roommategender,
            roommatecommute: roommatecommute,
            cid: cid
        },
        success: function (response) {
            $("#roommatename" + cid).prop('disabled', true);
            $("#roommategender" + cid).prop('disabled', true);
            $("#roommatecommute" + cid).prop('disabled', true);
            $("#savedroommatename" + cid).val('1');
        }
    });
}
Cart.prototype.CancelRoomRequest = function (cid) {
    var self = this;
    var roommatename = "";
    var roommategender = "";
    var roommatecommute ="";
    Ext.Ajax.request({
        url: config.getUrl('public/cart/ApplyRoomMateRequest'),
        params: {
            roommatename: roommatename,
            roommategender: roommategender,
            roommatecommute: roommatecommute,
            cid: cid
        },
        success: function (response) {
            $("#roommatename" + cid).prop('disabled', false);
            $("#roommategender" + cid).prop('disabled', false);
            $("#roommatecommute" + cid).prop('disabled', false);
            $("#roommatename" + cid).val('');
            $("#roommategender" + cid).val('');
            $("#roommatecommute" + cid).val('');
            $("#savedroommatename" + cid).val('');

        }
    });
}

Cart.prototype.UpdateCoursePrice = function (courseid, studentid) {
    var self = this;
    var newprice = $("#txtadminpriceedit" + courseid + studentid).val() - 0;
    Ext.Ajax.request({
        url: config.getUrl('public/cart/UpdateAdminPricing'),
        params: {
            newprice: newprice,
            courseid: courseid,
            studentid: studentid
        },
        success: function (response) {
            self.checkout();
        }
    });
}



Cart.prototype.ApplyCouponDiscountPerOrder = function (cmd, courseid, coursecost, preselectedmaterialamount, couponid,studentid) {
    var self = this;
    if (cmd != 'event') {
        totalcourseprice = $('#hiddenOrderTotal').val() - 0;
        totalnOrderDiscountTotal = $('#hiddenOrderDiscountTotal').val() - 0;
        coupon = $("#" + couponid).val();
        discountvalue = $("#txtdiscountvalue" + courseid).val() - 0;
        Ext.Ajax.request({
            url: config.getUrl('public/cart/applycoupondiscountpercourse'),
            params: {
                couponcode: coupon,
                courseid: courseid,
                studentid: studentid
            },
            success: function (response) {
                var resultdiscount = Ext.decode(response.responseText);
                var result = resultdiscount.status;
                var dollardiscount = resultdiscount.dollardiscount;
                discountdetails = Ext.util.Format.currency(dollardiscount) + " and " + resultdiscount.percentdiscount + "% Off";
                var materialdiscounted = resultdiscount.materialdicounted;
                if (result == 'approved') {

                    if (coupon.trim() != '') {
                        var element = document.getElementById(couponid);
                        if (element && !element.disabled) {
                            var tip = Ext.create('Ext.tip.ToolTip', {
                                anchor: 'bottom',
                                anchorToTarget: 'true',
                                showDelay: 0,
                                target: couponid + '-container',
                                html: 'If you wish to change the coupons around, please go to cart and restart the checkout process...'
                            });
                            element.disabled = true;
                        }
                        $('#' + couponid + '-href').hide();
                        $('input[data-material-course="' + courseid + '"]').prop("disabled", true);
                    }

                    if (materialdiscounted == "0") {

                        var percentdiscount = coursecost * (resultdiscount.percentdiscount / 100);
                        //$('#CheckoutTotal').html(Ext.util.Format.currency(((totalcourseprice - dollardiscount) - percentdiscount)));
                        //$('#CheckoutDiscount').html(Ext.util.Format.currency(dollardiscount - 0 + percentdiscount - 0 + (totalnOrderDiscountTotal - 0)));
                        //$('#hiddenOrderTotal').val(((totalcourseprice - dollardiscount) - percentdiscount));
                        //$('#hiddenOrderDiscountTotal').val(dollardiscount - 0 + percentdiscount - 0 + (totalnOrderDiscountTotal - 0))
                        totalDiscount = Ext.util.Format.currency((dollardiscount - 0) + (percentdiscount - 0) + (totalnOrderDiscountTotal - 0));
                        //$("#txtdiscountvalue" + courseid).val(((dollardiscount - 0) + (percentdiscount - 0)));

                    }
                    else {
                        var percentdiscount = ((totalmatprice - 0) + (coursecost - 0)) * (resultdiscount.percentdiscount / 100);
                        //$('#CheckoutTotal').html(Ext.util.Format.currency((((totalcourseprice) - dollardiscount) - percentdiscount)));
                        //$('#CheckoutDiscount').html(Ext.util.Format.currency(dollardiscount - 0 + percentdiscount - 0 + (totalnOrderDiscountTotal - 0)));
                        //$('#hiddenOrderTotal').val((((totalcourseprice) - dollardiscount) - percentdiscount));
                        //$('#hiddenOrderDiscountTotal').val(dollardiscount - 0 + percentdiscount - 0 + (totalnOrderDiscountTotal - 0))
                        totalDiscount = Ext.util.Format.currency((dollardiscount - 0) + (percentdiscount - 0) + (totalnOrderDiscountTotal - 0));
                        //$("#txtdiscountvalue" + courseid).val(((dollardiscount - 0) + (percentdiscount - 0)));
                    }
                    $('#hiddenOrderDiscountTotal').val(dollardiscount - 0 + percentdiscount - 0 + (totalnOrderDiscountTotal - 0))
                    $("#txtdiscountvalue" + courseid).val(((dollardiscount - 0) + (percentdiscount - 0)));
                    //self.ComputePriceCheckoutAfterAutoCouponDiscount;

                    if (totalDiscount != 0) {
                        Ext.Ajax.request({
                            url: config.getUrl('public/cart/setdiscountamountpercourse'),
                            params: {
                                discountamount: parseFloat(percentdiscount) + parseFloat(dollardiscount),
                                courseid: courseid,
                                studentid: studentid
                            },
                            success: function (response) {
                                self.ComputePriceCheckoutAfterAutoCouponDiscount();
                            }
                        });
                    }
                    else {
                        Ext.Ajax.request({
                            url: config.getUrl('public/cart/setdiscountamountpercourse'),
                            params: {
                                discountamount: 0,
                                courseid: courseid,
                                studentid: studentid
                            },
                            success: function (response) {
                                self.ComputePriceCheckoutAfterAutoCouponDiscount();
                            }
                        });
                    }

                }
                else {
                    if (coupon != "") {
                        discountdetails = resultdiscount.status;
                        iscouponerror = "yes"
                        Ext.MessageBox.show({
                            msg: resultdiscount.status,
                            title: 'Error',
                            buttons: Ext.MessageBox.OK,
                            icon: Ext.MessageBox.ERROR
                        });
                    }
                    Ext.Ajax.request({
                        url: config.getUrl('public/cart/setdiscountamountpercourse'),
                        params: {
                            discountamount: 0,
                            courseid: courseid,
                            studentid: studentid
                        },
                        success: function (response) {
                        }
                    });
                    //$('#CheckoutTotal').html(Ext.util.Format.currency((totalcourseprice - 0) + (discountvalue - 0)));
                    //$('#CheckoutDiscount').html(Ext.util.Format.currency((totalnOrderDiscountTotal - 0) - (discountvalue - 0)));
                    //$('#hiddenOrderTotal').val((totalcourseprice - 0) + (discountvalue - 0));
                    if (iscouponerror != "yes") {
                        $('#hiddenOrderDiscountTotal').val((totalnOrderDiscountTotal - 0) - (discountvalue - 0));
                        $("#txtdiscountvalue" + courseid).val(0);
                        self.ComputePriceCheckoutAfterAutoCouponDiscount();
                    }
                }

            }
        });
    }
}
Cart.prototype.AlertDiscountDetails = function (cmd) {
    Ext.MessageBox.show({
        msg: discountdetails,
        title: 'Discount Details',
        buttons: Ext.MessageBox.OK,
        icon: Ext.MessageBox.INFO
    });
}

//*************************************************************
//                       payment
//*************************************************************
var cartstepLocation = "";
Cart.prototype.ShowPaymentPage = function (strPaymentTotal, strOrderNumber, strTranscriptID, PaymentCaller) {
    // use for checkout (full-partial payment and usedash PayNow)
    var self = this;
    window.LAYOUT.MaskLayout('Please wait..');
    self.activeCheckoutStep = 'payment';
    self.hideMiniDisplay();
    Ext.Ajax.request({
        url: config.getUrl('public/cart/payment'),
        timeout: 1200000,
        asynch: false,
        params: {
            OrderNumber: strOrderNumber,
            TranscriptID: strTranscriptID,
            PaymentTotal: String(strPaymentTotal).replace(/,/g, ''),
            PaymentCaller: PaymentCaller,
            MaterialList: tempMateriallist
        },
        success: function (response) {
            window.LAYOUT.UnmaskLayout();
            var responseintext = response.responseText;

            if (responseintext.indexOf("Default.aspx") == -1) {
                self.elementCheckoutContainer.setHtml(response.responseText, true);
                self.ShowCheckoutContainerDisplay();
            }
            else {
                window.location = response.responseText;
            }

            if (strPaymentTotal == "0") {
                $('#reservationtimer').hide();
                Ext.Ajax.request({
                    url: config.getUrl('public/cart/minidisplay'),
                    success: function (response) {
                        if (response.responseText.indexOf('emptydiv') > -1) {
                            self.elementInfo.setHtml('Empty');
                            Ext.Ajax.request({
                                url: config.getUrl('public/cart/CheckGoogleAnalyticsUsed'),
                                headers: { 'Content-Type': 'application/json' },
                                asynch: false,
                                success: function (response) {
                                    var data = Ext.decode(response.responseText);
                                    if (data.ga_used == true) {
                                        ga('send', 'pageview', {
                                            'page': config.getUrl('public/cart/ThankYou'),
                                            'title': 'Google Analytics Tracking (Thank You ActionResult)'
                                        });
                                        ga('send', 'pageview', {
                                            'page': config.getUrl('thankyou.asp?callfrom=ruby'),
                                            'title': 'Google Analytics Tracking (Thank You ASP)'
                                        });
                                    }
                                }
                            });
                        }
                    }
                });
            }
            try{
                if (window.top.location.toString().indexOf('PaypalRedirectConfirmation') > -1 && history.pushState) {
                    history.pushState('', 'Paypal Confirmation', window.location.toString().replace("PaypalRedirectConfirmation", "PaypalCheckout") + "&Payment=true");
                }
            }
            catch(e){}
            $(document).ready(function () {
                var showCreditCard = $('#hiddenShowCredit').val() != undefined ? $('#hiddenShowCredit').val() : false;
                var showIsAuthorized = $('#hiddenIsAuthorized').val() != undefined ? $('#hiddenIsAuthorized').val() : false;
                var showIsPaypal = $('#hiddenIsPaypal').val() != undefined ? $('#hiddenIsPaypal').val() : false;
                var chasePayment = $('#hiddenIsChasePayment').val() != undefined ? $('#hiddenIsChasePayment').val() : false;
                var payGov = $('#hiddenIsPaygov').val() != undefined ? $('#hiddenIsPaygov').val() : false;
                var ipay = $('#hiddenIsiPay').val() != undefined ? $('#hiddenIsiPay').val() : false;
                var nelnet = $("#hiddenIsNelNet").val() != undefined ? $('#hiddenIsNelNet').val() : false;
                var showTouchnet = $("#hiddenIsTouchnet").val() != undefined ? $('#hiddenIsTouchnet').val() : false;
                var payUserDash = $('#hiddenIsPayNowUserDash').val() != undefined ? $('#hiddenIsPayNowUserDash').val() : false;
                var showSquarePayment = $('#hiddenIsSquare').val() != undefined ? $('#hiddenIsSquare').val() : false;
                //console.log(showCreditCard + ' ' + showIsAuthorized + ' ' + showIsPaypal + ' ' + chasePayment + ' ' + payGov);
                if (new String(showCreditCard).toLowerCase() == 'true' || new String(showIsAuthorized).toLowerCase() == 'true'
                    || new String(showIsPaypal).toLowerCase() == 'true' || new String(chasePayment).toLowerCase() == 'true'
                    || new String(ipay).toLowerCase() == 'true'
                    || new String(payGov).toLowerCase() == 'true'
                    || new String(showTouchnet).toLowerCase() == 'true'
                    || new String(nelnet).toLowerCase() == 'true'
                    || new String(showSquarePayment).toLowerCase() == 'true'
                    || payUserDash == 1)
                {
                    $("#itemOtherPayments option[value='Select Payment Type']").remove();
                    $("#itemOtherPayments").prepend(new Option("Credit/Debit Card", "Credit Card"));
                    $("#itemOtherPayments").val('Credit Card')
                }

            });

        }

    });

    var _gaq = _gaq || [];
    _gaq.push(['_setAccount', 'UA-48929136-1']);
    _gaq.push(['_trackPageview']);
    _gaq.push(['_addTrans',
      strOrderNumber,           // transaction ID - required
      'Go Sign Me UP',  // affiliation or store name
      strPaymentTotal,          // total - required
      '0.00',           // tax
      '0.00',              // shipping
      'San Jose',       // city
      'California',     // state or province
      'USA'             // country
    ]);

    // add item might be called for every item in the shopping cart
    // where your ecommerce engine loops through each item in the cart and
    // prints out _addItem for each
    _gaq.push(['_addItem',
      strOrderNumber,           // transaction ID - required
      strOrderNumber,           // SKU/code - required
      'GSMU Course',        // product name
      'Course',   // category or variation
      strPaymentTotal,          // unit price - required
      '1'               // quantity - required
    ]);
    _gaq.push(['_trackTrans']); //submits transaction to the Analytics servers

    (function () {
        var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
        ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
        var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
    })();
}
function googleanalyticsecommerce() {

}

//*************************************************************
//                       confirmation
//*************************************************************
var isrequiredpaynumber = false;
Cart.prototype.ShowConfirmationPageforPaygov = function (cmd) {
    self.activeCheckoutStep = 'confirmation';
    Ext.Ajax.request({
        url: config.getUrl('public/cart/confirmation'),
        timeout: 1200000,
        params: {
            PaymentType: 'CC',
            Paynumber: $("#transactionid").val(),
            OrderNo: $("#OrderNumber").val(),
            OrderTotal: $("#OrderTotal").val().replace(/,/g, ''),
            OrderAmount: $("#OrderTotal").val().replace(/,/g, ''),
            pgAuthNum: $('#pgAuthNum').val(),
            pgCardName: $('#pgCardName').val()
        },

            success: function (response) {
                $('#confirmationprint').html(response.responseText);
                self.elementInfo.setHtml('Empty');
                window.LAYOUT.MaskLayout('Loading');
                self.elementCheckoutContainer.setHtml(response.responseText, true);
                self.ShowCheckoutContainerDisplay();
                if (response.responseText.indexOf("Ipay") != -1) {
                }

                Ext.Ajax.request({
                    url: config.getUrl('public/cart/minidisplay'),
                    success: function (response) {
                        if (response.responseText.indexOf('emptydiv') > -1) {
                            self.elementInfo.setHtml('Empty');
                            Ext.Ajax.request({
                                url: config.getUrl('public/cart/CheckGoogleAnalyticsUsed'),
                                headers: { 'Content-Type': 'application/json' },
                                asynch: false,
                                success: function (response) {
                                    var data = Ext.decode(response.responseText);
                                    if (data.ga_used == true) {
                                        ga('send', 'pageview', {
                                            'page': config.getUrl('public/cart/ThankYou'),
                                            'title': 'Google Analytics Tracking (Thank You ActionResult)'
                                        });
                                        ga('send', 'pageview', {
                                            'page': config.getUrl('thankyou.asp?callfrom=ruby'),
                                            'title': 'Google Analytics Tracking (Thank You ASP)'
                                        });
                                    }
                                }
                            });
                        }
                    }
                });
            }
    });
}

Cart.prototype.ShowConfirmationReceipt = function (OrderNo) {
    //will only show Confirmation receipt no processing will be done.
    $('#reservationtimer').hide();
    var self = this;
    Ext.Ajax.request({
        url: config.getUrl('public/cart/ConfirmationReceipt'),
        timeout: 1200000,
        params: {
            OrderNumber: OrderNo,
        },
        success: function (response) {
            Ext.Ajax.request({
                url: config.getUrl('public/cart/CheckGoogleAnalyticsUsed'),
                headers: { 'Content-Type': 'application/json' },
                asynch: false,
                success: function (response) {
                    var data = Ext.decode(response.responseText);
                    if (data.ga_used == true) {
                        ga('send', 'pageview', {
                            'page': config.getUrl('public/cart/ThankYou'),
                            'title': 'Google Analytics Tracking (Thank You ActionResult)'
                        });
                        ga('send', 'pageview', {
                            'page': config.getUrl('thankyou.asp?callfrom=ruby'),
                            'title': 'Google Analytics Tracking (Thank You ASP)'
                        });
                    }
                }
            });
            $('#confirmationprint').html(response.responseText);
            if (self.elementCheckoutContainer != null) {
                window.LAYOUT.MaskLayout('Loading');
                self.elementCheckoutContainer.setHtml(response.responseText, true);
                self.ShowCheckoutContainerDisplay();
            }
        }
    });
}

Cart.prototype.ShowConfirmationPage = function (cmd) {
    if ($("#itemOtherPayments").val() == "Select Payment Type") {

        alert("Please select a payment  type.")
        return false;
    }
    else{
        Ext.Ajax.request({
            url: config.getUrl('public/cart/ValidateMaxEnrollment'),
            success: function (response) {
                var model = Ext.decode(response.responseText);
                var noofcourse = model.noofcourse;
                var noofremovecourse = model.noofremovecourse;
                if (noofremovecourse > 0) {
                    if (noofcourse > 0) {
                        cart.checkout();
                    }
                    else {
                        alert("There's no more space in one of your course. The course has been removed from the Cart.")
                        document.location = "";
                    }
                }
                else if (noofcourse > 0) {
                    cart.ShowValidatedConfirmationPage();
                }
                else {
                    if ($("#hiddenIsPayNowUserDash").val() == "1") {
                        cart.ShowValidatedConfirmationPage();
                    } else {
                        document.location = "";
                    }
                }
            },
            failure: function () {
                alert("There's no more space in one of your course. The course has been removed from the Cart.")
                cart.checkout();
            }
        });
    }
}

Cart.prototype.ShowValidatedConfirmationPage = function (cmd) {
    var self = this;
    self.activeCheckoutStep = 'confirmation';
    $('#reservationtimer').hide();
    self.hideMiniDisplay();
    var requiredFieldSetUp = $("#RequiredFieldSetUp").val();
    countryRequired = false;
    addressRequired = false;
    zipRequired = false;
    stateRequired = false;
    cityRequired = false;
    if (requiredFieldSetUp == "1") {
        countryRequired = false;
        addressRequired = true;
        zipRequired = true;
        stateRequired = true;
        cityRequired = true;
    } else if (requiredFieldSetUp == "4") {
        countryRequired = true;
        addressRequired = true;
        zipRequired = true;
        stateRequired = true;
        cityRequired = true;
    } else {
        if (requiredFieldSetUp == "0") {
            addressRequired = true;
        }
        else {
            addressRequired = false;
        }
        countryRequired = false;
        zipRequired = false;
        stateRequired = false;
        cityRequired = false;
    }


    if (($("#itemOtherPayments").val() == "CC"
        || $("#itemOtherPayments").val() == "Credit Card"
            || $("#itemOtherPayments").val() == "SquarePayment"
                || typeof ($("#itemOtherPayments").val()) == "undefined")
        && ($("#hiddenIsSquare").val() == "true" || $("#hiddenIsSquare").val() == "True")) {
        window.location = config.getUrl('public/cart/squarepayment');
        return;
    }

    if (($("#itemOtherPayments").val() == "CC"
        || $("#itemOtherPayments").val() == "Credit Card"
        || $("#itemOtherPayments").val() == "PaygovTCS"
                || typeof ($("#itemOtherPayments").val()) == "undefined")
        && ($("#hiddenIsPaygovTCS").val() == "true" || $("#hiddenIsPaygovTCS").val() == "True")) {
        window.location = config.getUrl('public/cart/PaygovTCSPayment');
        return;
    }

    if (($("#itemOtherPayments").val() == "CC"
        || $("#itemOtherPayments").val() == "Credit Card"
            || $("#itemOtherPayments").val() == "CybersourcePayment"
                || typeof ($("#itemOtherPayments").val()) == "undefined")
        && ($("#hiddenIsCybersource").val() == "true" || $("#hiddenIsCybersource").val() == "True")) {
        window.location = config.getUrl('public/cart/CybersourcePayment');
        return;
    }

    if (($("#itemOtherPayments").val() == "CC"
        || $("#itemOtherPayments").val() == "Credit Card"
            || $("#itemOtherPayments").val() == "FirstDataPayment"
                || typeof ($("#itemOtherPayments").val()) == "undefined"
        )
        && ($("#hiddenIsFirstData").val() == "true" || $("#hiddenIsFirstData").val() == "True")) {
        window.location = config.getUrl('public/cart/FirstDataPayment');
        return;
    }

    if (($("#itemOtherPayments").val() == "CC"
        || $("#itemOtherPayments").val() == "Credit Card"
            || $("#itemOtherPayments").val() == "AdyenPayment"
                || typeof ($("#itemOtherPayments").val()) == "undefined"
        )
        && ($("#hiddenIsAdyen").val() == "true" || $("#hiddenIsAdyen").val() == "True")) {
        window.location = config.getUrl('public/cart/AdyenPayment');
        return;
    }

    if (($("#itemOtherPayments").val() == "CC"
    || $("#itemOtherPayments").val() == "Credit Card"
        || $("#itemOtherPayments").val() == "ChasePayment"
            || typeof ($("#itemOtherPayments").val()) == "undefined"
    )
    && ($("#hiddenIsChasePayment").val() == "true" || $("#hiddenIsChasePayment").val() == "True")) {
        window.location = config.getUrl('public/cart/ChaseHPPPayment');
        return;
    }

    if (((
        $("#itemOtherPayments").val() == "Select Payment Type")
        || ($("#itemOtherPayments").val() == "Credit Card")
            || $("#itemOtherPayments").val() == "PayPal"
                || ($("#itemOtherPayments").val() == undefined))) {

        $.validator.messages.required = "*";
        $("#creditcardform").validate({
            rules: {
                CardNumber: {
                    required: true
                },
                item1List: "required",
                FirstName: "required",
                LastName: "required",
                Address: { required: addressRequired },
                City: { required: cityRequired },
                State: { required: stateRequired },
                Zip: { required: zipRequired },
                itemCountryList: { required: countryRequired /*, notEqual: "Select Country"*/ }
            }
        });
        var otherpayment = "";
        if ($('#itemOtherPayments').val() == undefined) {
            otherpayment = "Select Payment Type";
        }
        else {
            var otherpayment = $('#itemOtherPayments').val().replace(/,/g, '');
        }
        if ($("#creditcardform").valid()) {
            window.LAYOUT.MaskLayout('Processing...');
            Ext.Ajax.request({
                url: config.getUrl('public/cart/confirmation'),
                timeout: 1200000,
                params: {
                    CardNumber: $("#CardNumber").val(),
                    ExpiryDate: $("#expirymonth").val() + $("#expiryyear").val(),
                    ExpiryMonth: $("#expirymonth").val(),
                    ExpiryYear: $("#expiryyear").val(),
                    Address: $("#Address").val(),
                    State: $("#State").val(),
                    City: $("#City").val(),
                    Zip: $("#Zip").val(),
                    Country: $("#itemCountryList").val(),
                    FirstName: $("#FirstName").val(),
                    LastName: $("#LastName").val(),
                    PaymentType: "Credit Card",
                    OrderNo: $("#OrderNumber").val(),
                    CardType: $("#item1List").val(),
                    Paynumber: $("#PaymentNumber").val(),
                    OrderTotal: $("#OrderTotal").val().replace(/,/g, ''),
                    OtherPayment: otherpayment,
                    ccv: $("#CCV").val(),
                    MaterialList: tempMateriallist,
                    totalDiscount: totalDiscount,
                    Email: $("#Email").val()
                },
                success: function (response) {
                    window.LAYOUT.UnmaskLayout();
                    var responseintext = response.responseText;
                    if (responseintext.indexOf("timed out")!=-1) {
                        alert(responseintext);
                        window.location = "";
                    };
                    if (responseintext.indexOf("Default.aspx") == -1) {
                        self.elementCheckoutContainer.setHtml(response.responseText, true);
                    }
                    else {
                        window.location = response.responseText;
                    }
                    if ((responseintext.indexOf("dll") != -1) || (responseintext.indexOf("chasepayment") != -1) || (responseintext.indexOf("upay") != -1)) {
                        Ext.Ajax.request({
                            url: config.getUrl('public/cart/empty'),
                            success: function (response) {
                                document.frmSubmit.submit();
                            }
                        });

                    }
                    else {
                        self.ShowCheckoutContainerDisplay();
                        if (response.responseText.indexOf("Ipay") != -1) {
                        }else{
                            Ext.Ajax.request({
                                url: config.getUrl('public/cart/minidisplay'),
                                success: function (response) {
                                    if (response.responseText.indexOf('emptydiv') > -1) {
                                        self.elementInfo.setHtml('Empty');
                                        Ext.Ajax.request({
                                            url: config.getUrl('public/cart/CheckGoogleAnalyticsUsed'),
                                            headers: { 'Content-Type': 'application/json' },
                                            asynch: false,
                                            success: function (response) {
                                                var data = Ext.decode(response.responseText);
                                                if (data.ga_used == true) {
                                                    ga('send', 'pageview', {
                                                        'page': config.getUrl('public/cart/ThankYou'),
                                                        'title': 'Google Analytics Tracking (Thank You ActionResult)'
                                                    });
                                                    ga('send', 'pageview', {
                                                        'page': config.getUrl('thankyou.asp?callfrom=ruby'),
                                                        'title': 'Google Analytics Tracking (Thank You ASP)'
                                                    });
                                                }
                                            }
                                        });
                                    }
                                }
                            });
                        }
                    }
                }
            });
        }
        else {
            alert("Please Complete Required Fields");
        }
    }
    else {
        jQuery('#creditcardform').validate().currentForm = '';
        var formisvalid = true;
        if (isrequiredpaynumber) {

            if ($("#PaymentNumber").val() == "") {
                $("#validatorpaynumber").css('display', 'block');
                formisvalid = false;
            }
        }
        if (formisvalid) {
            window.LAYOUT.MaskLayout('Processing...');
            Ext.Ajax.request({
                url: config.getUrl('public/cart/confirmation'),
                timeout: 1200000,
                params: {
                    PaymentType: $("#itemOtherPayments").val(),
                    Paynumber: $("#PaymentNumber").val(),
                    OrderNo: $("#OrderNumber").val(),
                    OrderTotal: $("#OrderTotal").val().replace(/,/g, ''),
                    MaterialList: tempMateriallist,
                    totalDiscount: totalDiscount
                },
                success: function (response) {
                    window.LAYOUT.UnmaskLayout();
                    self.elementCheckoutContainer.setHtml(response.responseText, true);
                    self.ShowCheckoutContainerDisplay();

                    Ext.Ajax.request({
                        url: config.getUrl('public/cart/minidisplay'),
                        success: function (response) {
                            $('#reservationtimer').hide();
                            if (response.responseText.indexOf('emptydiv') > -1) {
                                Ext.Ajax.request({
                                    url: config.getUrl('public/cart/CheckGoogleAnalyticsUsed'),
                                    headers: { 'Content-Type': 'application/json' },
                                    asynch: false,
                                    success: function (response) {
                                        var data = Ext.decode(response.responseText);
                                        if (data.ga_used == true) {
                                            ga('send', 'pageview', {
                                                'page': config.getUrl('public/cart/ThankYou'),
                                                'title': 'Google Analytics Tracking (Thank You ActionResult)'
                                            });
                                            ga('send', 'pageview', {
                                                'page': config.getUrl('thankyou.asp?callfrom=ruby'),
                                                'title': 'Google Analytics Tracking (Thank You ASP)'
                                            });
                                        }
                                    }
                                });
                            }
                        }
                    });
                }
            });
        }
    }
}


Cart.prototype.ProcessCreditHoursPayment = function (cmd) {
    var self = this;
    if ((cmd == 'anetredirect') && (($("#itemOtherPayments").val() == "Select Payment Type") || ($("#itemOtherPayments").val() == "Credit Card") || $("#itemOtherPayments").val() == "PayPal" || ($("#itemOtherPayments").val() == undefined))) {
        document.frmSubmit.submit();
    }
    else {
        self.activeCheckoutStep = 'confirmation';
        //self.hideMiniDisplay();
        var requiredFieldSetUp = $("#RequiredFieldSetUp").val();
        countryRequired = false;
        addressRequired = false;
        zipRequired = false;
        stateRequired = false;
        cityRequired = false;
        if (requiredFieldSetUp == "1") {
            countryRequired = false;
            addressRequired = true;
            zipRequired = true;
            stateRequired = true;
            cityRequired = true;
        } else if (requiredFieldSetUp == "4") {
            countryRequired = true;
            addressRequired = true;
            zipRequired = true;
            stateRequired = true;
            cityRequired = true;
        } else {
            if (requiredFieldSetUp == "0") {
                addressRequired = true;
            }
            else {
                addressRequired = false;
            }
            countryRequired = false;
            zipRequired = false;
            stateRequired = false;
            cityRequired = false;
        }

        if ((($("#itemOtherPayments").val() == "Select Payment Type") || ($("#itemOtherPayments").val() == "Credit Card") || $("#itemOtherPayments").val() == "PayPal" || ($("#itemOtherPayments").val() == undefined))) {
            var otherpaymentValue = "";
            if ($('#itemOtherPayments').val() != undefined) {
                otherpaymentValue = $('#itemOtherPayments').val().replace(/,/g, '');
            }
            $.validator.messages.required = "*";
            $("#creditcardform").validate({
                rules: {
                    CardNumber: {
                        required: true
                    },
                    item1List: "required",
                    FirstName: "required",
                    LastName: "required",
                    Address: { required: addressRequired },
                    City: { required: cityRequired },
                    State: { required: stateRequired },
                    Zip: { required: zipRequired },
                    itemCountryList: { required: countryRequired /*, notEqual: "Select Country"*/ }
                }
            });

            if ($("#creditcardform").valid()) {
                window.LAYOUT.MaskLayout('Processing...');
                Ext.Ajax.request({
                    url: config.getUrl('public/cart/ProcessCreditHoursPayment'),
                    timeout: 1200000,
                    params: {
                        CardNumber: $("#CardNumber").val(),
                        ExpiryDate: $("#expirymonth").val() + $("#expiryyear").val(),
                        ExpiryMonth: $("#expirymonth").val(),
                        ExpiryYear: $("#expiryyear").val(),
                        Address: $("#Address").val(),
                        State: $("#State").val(),
                        City: $("#City").val(),
                        Zip: $("#Zip").val(),
                        Country: $("#itemCountryList").val(),
                        FirstName: $("#FirstName").val(),
                        LastName: $("#LastName").val(),
                        PaymentType: "Credit Card",
                        OrderNo: $("#OrderNumber").val(),
                        CardType: $("#item1List").val(),
                        Paynumber: $("#PaymentNumber").val(),
                        OrderTotal: $("#OrderTotal").val().replace(/,/g, ''),
                        OtherPayment: otherpaymentValue,
                        ccv: $("#CCV").val(),
                        MaterialList: tempMateriallist,
                        totalDiscount: totalDiscount,
                        Email: $("#Email").val(),
                        TranscriptID: $("#TranscriptId").val()
                    },
                    success: function (response) {
                        var result = Ext.decode(response.responseText);
                        window.LAYOUT.UnmaskLayout();
                        if (result.success) {
                            alert("Payment Successful. Your payment has been processed.");
                            popWindow_paymenthours.close();
                            document.location = config.getUrl('/public/user/dashboard');
                            Ext.Ajax.request({
                                url: config.getUrl('public/cart/CheckGoogleAnalyticsUsed'),
                                headers: { 'Content-Type': 'application/json' },
                                asynch: false,
                                success: function (response) {
                                    var data = Ext.decode(response.responseText);
                                    if (data.ga_used == true) {
                                        ga('send', 'pageview', {
                                            'page': config.getUrl('public/cart/ThankYou'),
                                            'title': 'Google Analytics Tracking (Thank You ActionResult)'
                                        });
                                        ga('send', 'pageview', {
                                            'page': config.getUrl('thankyou.asp?callfrom=ruby'),
                                            'title': 'Google Analytics Tracking (Thank You ASP)'
                                        });
                                    }
                                }
                            });
                        }
                        else {
                            alert("Error:" + result.messages);
                            popWindow_paymenthours.close();
                        }
                    }
                });
            }
            else {
                alert("Please Complete Required Fields");
            }
        }
        else {
            var otherpaymentValue = "";
            if ($('#itemOtherPayments').val() != undefined) {
                otherpaymentValue = $('#itemOtherPayments').val().replace(/,/g, '');
            }
            window.LAYOUT.MaskLayout('Processing...');
            Ext.Ajax.request({
                url: config.getUrl('public/cart/ProcessCreditHoursPayment'),
                timeout: 1200000,
                params: {
                    OrderNo: $("#OrderNumber").val(),
                    OtherPayment: otherpaymentValue,
                    MaterialList: tempMateriallist,
                    totalDiscount: totalDiscount,
                    OrderTotal: $("#PaymentTotal").val().replace(/,/g, ''),
                    TranscriptID: $("#TranscriptId").val()
                },
                success: function (response) {
                    var result = Ext.decode(response.responseText);
                    window.LAYOUT.UnmaskLayout();
                    if (result.success) {
                        alert("Payment Successful. Your payment has been processed.");
                        if (typeof popWindow_paymenthours != 'undefined' && popWindow_paymenthours) {

                            popWindow_paymenthours.close();
                        }
                        else {
                            document.location = "/public/user/dashboard";

                        }

                    }
                    else {
                        alert("Error:" + result.messages);
                        popWindow_paymenthours.close();
                    }
                }
            });
        }
    }

}
Cart.prototype.printreceipt = function (orderNumber) {
    var win = window.open(config.getUrl('public/user/orderconfirmation?order=' + orderNumber + '&print=1'));
    win.print();
}

Cart.prototype.SelectPayOption = function (PayOtion) {
    var self = this;

    $("#itemOtherPayments").val(PayOtion);
    self.ShowPaymentNumber()
}


Cart.prototype.ShowPaymentNumber = function (cmd) {

    //jQuery('#creditcardform').validate().currentForm = jQuery('#creditcardform')[0];
    $("#validatorpaynumber").css('display', 'none');

    var paymenttype = $("#itemOtherPayments").val()
    if (paymenttype == 'Credit Card') {
        $(".trCC").css("display", "block")
    } else {
        $(".trCC").css("display", "none")
    }

    Ext.Ajax.request({
        url: config.getUrl('public/cart/checkrequiredpaynumber'),
        async: false,
        params: {
            paymenttype: paymenttype
        },
        success: function (data) {
            if (data.responseText == "0") {
                $("#PaymentNumber").css('display', 'block');
                isrequiredpaynumber = true;
            }
            else {
                $("#PaymentNumber").css('display', 'none');
                isrequiredpaynumber = false;
            }
        }
    })
    //UI

    if ($('#itemOtherPayments').val() === "Select Payment Type" || $('#itemOtherPayments').val() === "Credit Card" || $('#itemOtherPayments').val() === "PayPal") {

        $('#place_order_now').text('Continue');
        if ($('#place_order_now').text() == 'Continue') {
            $('#PaymentNumber').hide();
            $("#PaymentNumber").css('display', 'none');
            isrequiredpaynumber = false;
        }
    }
    else {
        $('#place_order_now').text('Place Order Now');
    }
    if ($('#itemOtherPayments').val() === "PayPal") {
        $("#creditcardform tr:eq(0),#creditcardform tr:eq(1),#creditcardform tr:eq(2)").hide();
        //$('#itemOtherPayments').parent().parent().remove().insertBefore('#creditcardform tr:first');
        //$('#paymentLabel').parent().parent().remove().insertBefore('#creditcardform tr:first');
    }
    else {
        $("#creditcardform tr:eq(0),#creditcardform tr:eq(1),#creditcardform tr:eq(2)").show();
        //$('#paymentLabel').parent().parent().remove().insertAfter('#creditcardform tr:last');
        //$('#itemOtherPayments').parent().parent().remove().insertAfter('#creditcardform tr:last');
    }

    if ($('#itemOtherPayments').val() === "Select Payment Type") {
        $('#place_order_now').hide();
        $('#place_order_now_info').hide();
    } else {
        $('#place_order_now').show();
        $('#place_order_now_info').show();

    }
}

Cart.prototype.VerifyAccess = function (cmd) {
    var accesscodeval = $("#accesscode").val();
    var cid = $("#cid").val();
    Ext.Ajax.request({
        url: config.getUrl('public/cart/ValidateAccessCode'),
        params: {
            courseId: cid,
            accessCode: accesscodeval
        },
        success: function (response) {
            var result = Ext.decode(response.responseText);
            if (result.success) {
                if (result.resulttext) {
                    $("#accesscodeinputcontainer").css('display', 'none');
                    $("#accesscodebuttoncontainer").css('display', 'none');
                    $("#verificationresult").html("<div> Access Code: " + "****" + "<img src='/images/share/greencheck.png' /></div>");

                }
                else {
                    $("#verificationresult").html("<div> Access Code: " + "****" + "<img src='/images/share/redx.png' /></div>");

                }
            }
            else {
                config.showWarning(response.error, 'Cart error');
            }
        }
    });
}

Cart.prototype.setReloadCallback = function (code) {
    var self = this;
    Ext.util.Cookies.set('cart-reload-callback', code);
}

Cart.prototype.executeReloadCallback = function () {
    var self = this;
    var code = Ext.util.Cookies.get('cart-reload-callback');
    //tweak for ie8 to identify typeof null and null (empty)
    var txtcode = '>>' + code + '<<';
    if (txtcode != '>>null<<' && txtcode != '>><<') {
        Ext.util.Cookies.clear('cart-reload-callback');
        setTimeout(function () {
            self[code]();
        }, 1000);
    }
}

Cart.prototype.setPostRegistrationAction = function (action) {
    var self = this;

    Ext.util.Cookies.set('post-registration-action', action);
}

Cart.prototype.getPostRegistrationAction = function () {
    var self = this;
    var result = Ext.util.Cookies.get('post-registration-action');
    Ext.util.Cookies.clear('post-registration-action');
    return result;
}

//$(document).ready(function () {
//$('#itemOtherPayments').on('change',function () {
//    if ($("#itemOtherPayments").val() == "PayPal") {
//        $("#creditcardform tr:eq(0),#creditcardform tr:eq(1),#creditcardform tr:eq(2)").hide();
//    }
//});

//});
var selectedCredit = [];
Cart.prototype.SetSelectedCredit = function (courseIdWithModifier,value,selection) {
    if ($('#' + value).prop('checked')) {
        selectedCredit.push(value);
    }
    else {
        var i = selectedCredit.indexOf(value);
        selectedCredit.splice(i, 1);
    }
    if (selection == 'single') {
        selectedCredit = [];
        selectedCredit.push(value);
    }
    this.courseSelectedCredits[courseIdWithModifier] = {};
    this.courseSelectedCredits[courseIdWithModifier]['selectedCredits'] = selectedCredit;
}
// =======================================Functions Use for Multiple Enrollment================================================

Cart.prototype.Multiple_Enrollment = function (cid, modifier) {
    cart.AddCourse(cid, modifier, '', true)
}
Cart.prototype.DisplayAllStudents_ForEnrollment = function (cid,editonly) {
    var self = this;
    var studenttoenroll = "";
    var studenttoenroll_allpages = "";
    var availableseat = 0;
    var counter_getclass = 0; //Check getclass counts of call. To avoid duplicate.
    var store = Ext.create('Ext.data.Store', {
        autoLoad: true,
        autoSync: true,
        pageSize: 10,
        remoteFilter: true,
        remoteSort: true,
        remoteSort: true,
        model: 'EmailAuditTrail',
        proxy: {
            type: 'ajax',
            url: config.getUrl('public/Supervisor/Students?courseId=' + cid),
            jsonData: {
                courseId: cid
            },
            reader: {
                type: 'json',
                rootProperty: 'Result',
                totalProperty: 'TotalCount',

                listeners: {
                    exception: function (reader, response, error, opts) {
                        log(error);
                    },
                    load: function (sender, node, records) {
                        studenttoenroll = "";
                    }
                }
            }
        }
    });
    var searchField = Ext.create('Ext.form.field.Text', {
        region: 'north',
        emptyText: 'Search',
        anchor: '100%'
    });

    searchField.on('change', function (that, value, oldValue, options) {
        store.filter(
            [{ id: 'keyword', property: 'keyword', value: value }]
        );
    }, searchField, {
        buffer: 500
    });
    Ext.Ajax.request({
        url: config.getUrl('public/Supervisor/GetPrincipalStudent?cid=' + cid),
        success: function (response) {
            studenttoenroll = response.responseText +"<br /> ";
            studenttoenroll_allpages = response.responseText + "<br /> ";



            grid = Ext.create('Ext.grid.Panel', {
                region: 'center',
                id: 'studentlistgridid',
                dockedItems: [
                    {
                        xtype: 'pagingtoolbar',
                        store: store,
                        dock: 'top',
                        displayInfo: true,
                        listeners: {
                            beforechange: function () {
                                studenttoenroll = "";


                            }
                        }
                    }
                ],
                store: store,
                emptyText: 'No Students found',
                columns: [
                    {
                        text: 'User Name',
                        dataIndex: 'UserName',
                        width: 120,
                        renderer: function (myValue, val, myRecord) {
                            var cls = '';
                            var inActive = myRecord.get('InActive')
                            if (inActive === 1) {
                                cls = 'inactive';
                                val.tdCls = cls;
                                val.style = "color:lightgray";
                            }
                            return myValue;
                        }
                    },
                    {
                        text: 'First Name',
                        dataIndex: 'StudentFirstName',
                        width: 120,
                        renderer: function (myValue, val, myRecord) {
                            var cls = '';
                            var inActive = myRecord.get('InActive')
                            if (inActive === 1) {
                                cls = 'inactive';
                                val.tdCls = cls;
                                val.style = "color:lightgray";
                            }
                            return myValue;
                        }
                    },
                    {
                        text: 'Last Name',
                        dataIndex: 'StudentLastName',
                        flex: 1,
                        renderer: function (myValue, val, myRecord) {
                            var cls = '';
                            var inActive = myRecord.get('InActive')
                            if (inActive === 1) {
                                cls = 'inactive';
                                val.tdCls = cls;
                                val.style = "color:lightgray";
                            }
                            return myValue;
                        }
                    },
                    {
                        xtype: 'actioncolumn',
                        width: 22,
                        items: [
                            {
                                text: 'Enroll',
                                tooltip: '',
                                id: function (value, meta, record) {
                                    return record.getb().get('StudentId');
                                },
                                getClass: function (v, metadata, r, rowIndex, colIndex, store) {

                                    if (r.data.Isenrolled > 0) {
                                        // this.items[0].icon = ''

                                        if (cid == 0) {
                                            if (editonly == 1) {
                                                return 'x-edit-button';
                                            }
                                            return 'x-add-button';
                                        }
                                        else {
                                            return 'x-enrolled-button';
                                        }
                                    }
                                    else if ((r.data.IsErroriNRequirements == "true")|| (r.data.IsErroriNRequirements == true)) {
                                        return 'x-enrolled-button';
                                    }
                                        // hide this action if row data flag indicates it is not deletable
                                    else if (r.data.inCheckout > 0) {
                                        // this.items[0].icon = '/images/icons/famfamfam/delete.png';
                                        if (r.data.StudentFirstName == null) {
                                            r.data.StudentFirstName = "&nbsp;";
                                        }
                                        if (r.data.StudentLastName == null) {
                                            r.data.StudentLastName = "&nbsp;";
                                        }
                                        studenttoenroll = studenttoenroll +" "+ r.data.UserName + " " + r.data.StudentFirstName + " " + r.data.StudentLastName + " <br />";
                                        studenttoenroll_allpages = studenttoenroll_allpages.replace(r.data.UserName + " " + r.data.StudentFirstName + " " + r.data.StudentLastName + "<br />", " ");
                                        studenttoenroll_allpages = studenttoenroll_allpages.replace(" "+r.data.UserName + " " + r.data.StudentFirstName + " " + r.data.StudentLastName + " <br />", " ") +" "+ r.data.UserName + " " +r.data.StudentFirstName + " " + r.data.StudentLastName + " <br />";

                                        Ext.getCmp('rosterlist').update('<div style="overflow:scroll; height:290px; margin:10px; 10px; 10px; 10px;">' + studenttoenroll_allpages + '<div id="lastperson">'+""+'</div></div></div><div style="background-color:rgb(68, 147, 61);; color:white; margin-top:-35px; padding:5px;  text-weight:bold;">Available Seat(s): ' + r.data.AvailableSeats + '</div>');
                                        return 'x-delete-button';
                                    }
                                    else {
                                        if (r.data.StudentFirstName == null) {
                                            r.data.StudentFirstName = "&nbsp;";
                                        }
                                        if (r.data.StudentLastName == null) {
                                            r.data.StudentLastName = "&nbsp;";
                                        }
                                        studenttoenroll_allpages = studenttoenroll_allpages.replace(r.data.UserName + " " + r.data.StudentFirstName + " " + r.data.StudentLastName + "<br />", " ")
                                        studenttoenroll_allpages = studenttoenroll_allpages.replace(" " + r.data.UserName + " " + r.data.StudentFirstName + " " + r.data.StudentLastName + " <br />", " ");
                                        Ext.getCmp('rosterlist').update('<div style="overflow:scroll; height:290px; margin:10px; 10px; 10px; 10px;">' + studenttoenroll_allpages + '<div id="lastperson">' + "" + '</div></div><div style="background-color:rgb(68, 147, 61);; color:white; margin-top:-35px; padding:5px; text-weight:bold;">Available Seat(s): ' + r.data.AvailableSeats + '</div>');

                                        return 'x-add-button';
                                    }
                                },
                                handler: function (grid, rowIndex, colIndex) {
                                    var rec = grid.getStore().getAt(rowIndex);
                                    var sid = rec.get('StudentId')
                                    var sname = rec.get('StudentFirstName') + " " + rec.get('StudentLastName') + "<br />";
                                    var isincheckout = rec.get('inCheckout');
                                    var isenrolled = rec.get('Isenrolled') - 0;
                                    var AvailableSeats = rec.get('AvailableSeats') - 0;

                                    if ((isincheckout <= 0) && (isenrolled <= 0)) {
                                        self.elementContainer.mask();
                                        Ext.Ajax.request({
                                            url: config.getUrl('public/cart/addcourse'),
                                            params: {
                                                studentId: sid,
                                                courseId: cid,
                                                passrequirements:true
                                            },
                                            success: function (response) {
                                                //grid.getStore().removeAt(rowIndex);
                                                Ext.getCmp('studentlistgridid').getStore().load();
                                                window.LAYOUT.UnmaskLayout();
                                                studenttoenroll = '';

                                            }
                                        });
                                    }

                                    else {
                                        if (isincheckout == 1) {
                                            self.elementContainer.mask();
                                            Ext.Ajax.request({
                                                url: config.getUrl('public/cart/RemoveStudentinCheckoutMultiple'),
                                                params: {
                                                    studentId: sid,
                                                    courseId: cid
                                                },
                                                success: function (data) {
                                                    Ext.getCmp('studentlistgridid').getStore().load();
                                                    window.LAYOUT.UnmaskLayout();
                                                    studenttoenroll = '';
                                                    self.miniDisplay('refresh');
                                                    var response = JSON.parse(data.responseText)
                                                    if (response.courseenrolleecount == 0) {
                                                        alert('Last student has been removed. This course is no longer in cart.');
                                                        winstudent.close();
                                                    }
                                                    self.updateCartStatus(data.responseText);



                                                }
                                            });
                                        }
                                        else {
                                            if (cid == 0) {
                                                if (editonly == 1) {
                                                    document.location = "public/supervisor/editstudentinfo?sid="+sid
                                                }
                                                else {
                                                    Ext.Ajax.request({
                                                        url: config.getUrl('public/supervisor/SetPrincipalStudentonCart'),
                                                        params: {
                                                            studentId: sid,
                                                        },
                                                        success: function (response) {
                                                            document.location = "public/course/browse"


                                                        }
                                                    });
                                                }
                                            }
                                            else {
                                                if (AvailableSeats > 0) {
                                                    alert('Selected student is already registered for this course.');
                                                }
                                                else {
                                                    alert('Class is already full.');
                                                }
                                            }
                                        }
                                    }
                                }

                            }]
                    }
                ]
            });
            var winstudent;
            var isHiddenRoster = false;
            var textLabelTop = "MULTIPLE ENROLL";
            var topTitle = "Select Student to Enroll";
            if ((cid == 0) &&(editonly==1)) {
                isHiddenRoster = true;
                textLabelTop = " Edit "+studentTerm+" Information";
                topTitle = "Select " + studentTerm;
            }
            if (typeof showAddstudentbutton === 'undefined') {
                showAddstudentbutton = true;
            }
            var panel = Ext.create('Ext.form.Panel', {
                bodyPadding: 0,
                title: topTitle,
                region: 'center',
                items: [searchField, grid
                ],
                tools: [{
                    xtype: 'button',
                    tooltip: 'Add New',
                    margin: '0 0 0 15',
                    hidden: showAddstudentbutton,
                    html: '<div style="color:#fff">Add New '+studentTerm+ '</div>',
                    cls: 'hudbtn primary button-component button-checkout',
                    style: {
                        color: 'white'
                    },
                    listeners: {
                        click: function () {
                            self.AddNewStudents_ForEnrollment(cid);
                        }
                    }
                },{
                    xtype: 'button',
                    tooltip: 'Checkout',
                    margin: '0 0 0 15',
                    hidden:isHiddenRoster,
                    html: '<div style="color:#fff">Click When Finished</div>',
                    cls: 'hudbtn primary button-component button-checkout',
                    style: {
                        color: 'white'
                    },
                    listeners: {
                        click: function () {
                            winstudent.close();
                        }
                    }
                }]
            });
            var ctitlesection_name = "";

            Ext.Ajax.request({
                url: config.getUrl('public/course/GetCourseDetails'),
                params: {
                    cid: cid
                },
                success: function (response) {
                    var result = Ext.decode(response.responseText);
                    ctitlesection_name = result.coursenumber + " " + result.coursename
                    winstudent = Ext.create('Ext.window.Window', {
                        title: ctitlesection_name + " ",
                        height: 400,
                        width: 700,
                        modal: true,
                        layout: 'border',
                        items: [panel,
                            {

                                region: 'south',
                                height: 30,
                                minSize: 75,
                                maxSize: 250,
                                cmargins: '5 0 0 0',
                                bodyStyle: { "background-color": "#44933d" },
                                html: '<div style="color:white; padding:4px 0px 0px 20px; font-weight:bolder; font-size:15px;">'+textLabelTop+' </div>'
                            },
                            {
                                title: 'Current Roster:',
                                region: 'west',
                                margins: '5 0 0 0',
                                cmargins: '5 5 0 0',
                                width: 205,
                                minSize: 100,
                                maxSize: 250,
                                hidden:isHiddenRoster,
                                id: 'rosterlist',
                                html: '<div></div>',
                                tools: [{
                                    xtype: 'button',
                                    tooltip: 'Checkout',
                                    margin: '0 0 0 15',
                                    html: '<div style="color:#fff">Checkout</div>',
                                    cls: 'hudbtn primary button-component button-checkout',
                                    listeners: {
                                        click: function () {
                                            self.checkout();
                                            winstudent.close();
                                        }
                                    }
                                }]
                            }
                        ],
                        listeners: {
                            close: function () {
                                // top.window.LAYOUT.notify("Enrolling Student(s)<br />"+studenttoenroll);

                            }
                        }
                    }).show();
                    store.reload();
                }
            });
        }
    });


}
var NewLyAddedStudent = "";
Cart.prototype.AddNewStudents_ForEnrollment = function (cid) {


    var txtfirstName = Ext.create('Ext.form.field.Text', {
        region: 'north',
        emptyText: 'First Name',
        anchor: '100%'
    });
    var txtlastName = Ext.create('Ext.form.field.Text', {
        region: 'north',
        emptyText: 'Last  Name',
        anchor: '100%'
    });
    var txtEmail = Ext.create('Ext.form.field.Text', {
        region: 'north',
        emptyText: 'Email',
        anchor: '100%'
    });

    Ext.Ajax.request({
        url: config.getUrl('public/user/GetCustomRegistrationField'),
        success: function (response) {
            var result = Ext.decode(response.responseText);
            var AddnewStudent_panel = Ext.create('Ext.form.Panel', {
                bodyPadding: 0,
                title: '',
                region: 'center',
                items: [txtfirstName, txtlastName, txtEmail
                ],
                tools: [{
                    xtype: 'button',
                    tooltip: 'Add New',
                    margin: '0 0 0 15',
                    html: '<div style="color:#fff">Save</div>',
                    cls: 'hudbtn primary button-component button-checkout',
                    id:'AddStudent_enroll',
                    style: {
                        color: 'white'
                    },
                    listeners: {
                        click: function () {
                            var fieldcountloop = 0;
                            var studentaddparams = {};
                            studentaddparams['FirstName'] = txtfirstName.getValue();
                            studentaddparams['LastName'] = txtlastName.getValue();
                            studentaddparams['Email'] = txtEmail.getValue();
                            studentaddparams['cid'] = cid;
                            Ext.getCmp("AddStudent_enroll").setDisabled(true);
                            Ext.getCmp("AddStudent_enroll").setText("Saving...");
                            NewLyAddedStudent = studentaddparams['Email'] + " " + studentaddparams['FirstName'] + " " + studentaddparams['LastName'];
                            for (s = 1; s <= 20;s++) {
                                if (typeof(Ext.getCmp('dynamicfieldstudregfield' + s)) != "undefined") {
                                    studentaddparams['dynamicfieldstudregfield' + s] = Ext.getCmp('dynamicfieldstudregfield' + s).getValue();
                                }
                            }
                            Ext.Ajax.request({
                                url: config.getUrl('public/cart/AddNewStudentForEnrollment'),
                                params: studentaddparams,
                                success: function (response) {
                                    
                                    alert(response.responseText);
                                    if (response.responseText == 'Student has been added successfully.') {
                                        winstudentaddnew.close();
                                        Ext.getCmp('studentlistgridid').getStore().load();
                                        try {
                                            Ext.getCmp("AddStudent_enroll").setDisabled(false);
                                            Ext.getCmp("AddStudent_enroll").setText("<div style='color:white'>Save</div>");
                                        }
                                        catch (e) { }
                                      
                                    }
                                    else {
                                        NewLyAddedStudent = "";
                                    }

                                }


                            });
                        }
                    }
                }, {
                    xtype: 'button',
                    tooltip: 'Checkout',
                    margin: '0 0 0 15',
                    html: '<div style="color:#fff">Cancel</div>',
                    cls: 'hudbtn primary button-component button-checkout',
                    style: {
                        color: 'white'
                    },
                    listeners: {
                        click: function () {
                            winstudentaddnew.close();
                        }
                    }
                }],
                dockedItems: [

				{
				    xtype: 'panel',
				    html: '<div style="color:red; font-size:10px;">Note: Your Email will be your Username, password is randomly created and will be included in the confirmation email to each student individually.</div>',
				    margin: '0 0 0 0',
				    dock: 'bottom'
				}]
            });
            var fieldcountloop = 0;
            var tempfieldholder = null;
            var windowheight = 185;
            for (s in result) {
                if (result[fieldcountloop].ShowinMultipleEnroll == 1) {
                    tempfieldholder = Ext.create('Ext.form.field.Text', {
                        region: 'north',
                        emptyText:result[fieldcountloop].FieldLabel,
                        id:'dynamicfield'+result[fieldcountloop].FieldName,
                        anchor: '100%'
                    });
                    AddnewStudent_panel.add(tempfieldholder);
                    windowheight = windowheight + 30;

                }
                fieldcountloop = fieldcountloop + 1;
            }
            var winstudentaddnew = Ext.create('Ext.window.Window', {
                title: "Add New " + studentTerm,
                height: windowheight,
                width: 300,
                modal: true,
                layout: 'border',
                items: [AddnewStudent_panel],

            }).show();
        }
    });
}
//=============================================================================================================================

