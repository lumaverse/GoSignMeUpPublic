Ext.util.CSS.createStyleSheet = function (cssText, id) {
    var CSS = this,
        doc = document;


    var ss,
        head = doc.getElementsByTagName("head")[0],
        styleEl = doc.createElement("style");


    styleEl.setAttribute("type", "text/css");
    if (id) {
        styleEl.setAttribute("id", id);
    }


    if (Ext.isIE10m) {
        head.appendChild(styleEl);
        ss = styleEl.styleSheet;
        ss.cssText = cssText;
    } else {
        try {
            styleEl.appendChild(doc.createTextNode(cssText));
        } catch (e) {
            styleEl.cssText = cssText;
        }
        head.appendChild(styleEl);
        ss = styleEl.styleSheet ? styleEl.styleSheet : (styleEl.sheet || doc.styleSheets[doc.styleSheets.length - 1]);
    }
    CSS.cacheStyleSheet(ss);
    return ss;
};
