function PopupHelper(options) {
    var self = this;

    self.documentClickAction = options.documentClickAction;
    self.popupElement = options.popupElement;
    //self.keepPopup = typeof (options.keepPopup) == 'undefined' ? false : options.keepPopup;


    Ext.onDocumentReady(function () {

        self.popupElement.on('click', function () {
            var event = arguments[0];
            event.stopPropagation();
            self.popupClicked = true;
        });

        Ext.get(document).on('click', function () {

            var FirstToRunOnLogInInit = Ext.util.Cookies.get('FirstToRunOnLogInInit');
            var FirstToRunOnLogInClick = Ext.util.Cookies.get('FirstToRunOnLogInClick');

            if (typeof (FirstToRunOnLogInInit) == 'null' || FirstToRunOnLogInInit == 'false') {
                Ext.util.Cookies.set('FirstToRunOnLogInClick', 'popuphelper');
                Ext.util.Cookies.set('FirstToRunOnLogInInit', true);
            }
            if (!self.popupClicked && !self.keepPopup) {
                    self.documentClickAction();
            }

            self.popupClicked = false;

        });

        var map = new Ext.util.KeyMap({
            target: document,
            binding: [{
                key: [27],
                fn: function () {
                    if (!self.popupClicked && !self.keepPopup) {
                        self.documentClickAction();
                    }
                    self.popupClicked = false;
                }
            }]
        });

    });
}

PopupHelper.prototype.constructor = PopupHelper

PopupHelper.prototype.documentClickAction = null;
PopupHelper.prototype.popupElement = null;
PopupHelper.prototype.keepPopup = false;
PopupHelper.prototype.popupClicked = false;

Ext.util.Cookies.set('FirstToRunOnLogInInit', 'false');


PopupHelper.prototype.configureForMessageBox = function (box, relative) {
    var self = this;
    self.keepPopup = true;
    box.on('hide', function () {
        self.keepPopup = false;
    });
    box.on('close', function () {
        self.keepPopup = false;
    });

    if (typeof (relative) != 'undefined') {
        var el = Ext.get(relative);
        var pos = el.getBox();
        var width = el.getWidth();
        var left = pos.left + width - box.getWidth();
        box.setPosition(left, pos.top);
    }
}