function GsmuTooltip() {
    var self = this;
    self.Init();
}

GsmuTooltip.constructor = GsmuTooltip;

GsmuTooltip.prototype.ActiveTip = null;
GsmuTooltip.prototype.ActiveTipShow = null;
GsmuTooltip.prototype.Tips = [];

GsmuTooltip.prototype.Init = function () {
    var self = this;
    Ext.get(document).on('click', function (ev, el, options) {
        Ext.Array.forEach(self.Tips, function (value) {
            value.hide();
        });
    });

    Ext.on('resize', function () {
        if (self.ActiveTip != null && self.ActiveTip.isVisible()) {
            self.ActiveTipShow();
            self.ActiveTipShow();
        }
    });
}

GsmuTooltip.prototype.CreateTooltip = function (options) {
    var self = this;

    options = Ext.merge({
        offset: [42, -8],
        extraCssClass: ''
    }, options);

    var target = options.target;
    var html = options.html;
    var trigger = options.trigger;

    var tip = Ext.create('Ext.container.Container', {
        html: html,
        floating: true,
        cls: 'gsmu-tooltip' + (options.extraCssClass != '' ? ' ' + options.extraCssClass : ''),
        shadow: false,
        renderTo: Ext.getBody(),
        hidden: true
    });

    tip.getEl().appendChild({
        tag: 'div',
        cls: 'gsmu-tooltip-arrow-border'
    });

    tip.getEl().appendChild({
        tag: 'div',
        cls: 'gsmu-tooltip-arrow'
    });

    var showTip = function () {
        if (tip.isVisible()) {
            return;
        }
        self.ActiveTip = tip;
        self.ActiveTipShow = function () {
            tip.showBy(target, 'br-tc', options.offset);
            setTimeout(function () {
                tip.setHidden(false);
            },100);
        }
        self.ActiveTipShow();

        Ext.Array.forEach(self.Tips, function (value) {
            if (self.ActiveTip != value) {
                value.hide();
            }
        });
    };

    /*
    trigger.on('mouseover', function() {
        clearTimeout(TileListJuly.ToolTipMouseOverTimeout);
        showTip();
    }, null, {
        delay: 500
    });
    */
    trigger.on('click', function () {
        if (tip.isVisible()) {
            tip.hide();
        } else {
            showTip();
        }
    }, tip, {
        stopPropagation: true
    });

    tip.getEl().on('click', function (ev, el, options) {
        ev.stopPropagation();
    });

    self.Tips.push(tip);

    return tip;
}
