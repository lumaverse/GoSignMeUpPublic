var ModelHelper = {
    ConvertVbScriptBooelan: function (v, record) {
        if (Ext.isDefined(v)) {
            return v == -1 || v == 1 || v == '1' || v == '-1';
        }
        return false;
    },

    ConvertAspNetMvcDate: function (v, record) {
        if (v == null) {
            return null;
        }
        v = v.replace('/Date(', '');
        v = parseInt(v.replace(')/', ''));
        var date = new Date(v);
        return date;
    },

    ConvertDotNetDate: function (v, record) {
        if (v == null) {
            return null;
        }
        var date = new Date(v);
        return date;
    }

}