Ext.define('Gsmu.HtmlEditor', {
    extend: 'Ext.form.field.HtmlEditor',
    alias: 'widget.gsmuhtmleditor',


    constructor: function () {
        var self = this;

        Gsmu.HtmlEditor.superclass.constructor.apply(this, arguments);

        var addListener = function () {
            Ext.EventManager.addListener(
                self.iframeEl.dom.contentWindow.document.body
                , 'paste'
                , function (e, elem) {

                    if (window.confirm('Do you wish to clean the pasted Html? (In case it looks weird, it is worth trying it.')) {
                        self.on('sync', function () {
                            self.fixWordPasteHtml();
                        }, self, {
                            single: true
                        });
                    }
                }
            );

        }

        var editorToolbar = self.getToolbar();

        editorToolbar.add({
            tooltip:'<strong>Fix Html Pasted from Word</strong><br/><br/>If you pasted the Html from Microsoft Word and the result looks weird, you can try cleaning up the Html with this button, altough it is not guaranteed.',
            icon: config.getUrl('Images/Icons/Famfamfam/paste_word.png'),
            handler: function () {
                self.fixWordPasteHtml();
            }
        });


        self.on('initialize', function () {
            addListener();
        });

    },

    fixWordPasteHtml: function () {
        var self = this;
        var wordPaste = self.getValue();

        if (!wordPaste) {
            return;
        }

        var removals = [
            /&nbsp;/ig, /[\r\n]/g,
            /<(xml|style)[^>]*>.*?<\/\1>/ig,
            /<\/?(meta|object)[^>]*>/ig,
//			/<\/?[A-Z0-9]*:[A-Z]*[^>]*>/ig,
            /(lang|class|type|href|name|title|id|clear)=\"[^\"]*\"/ig,
//            /style=(\'\'|\"\")/ig,
            /<![\[-].*?-*>/g,
			/MsoNormal/g,
            /<\\?\?xml[^>]*>/g,
            /<\/?o:p[^>]*>/g,
            /<\/?v:[^>]*>/g,
            /<\/?o:[^>]*>/g,
            /<\/?st1:[^>]*>/g,
            /&nbsp;/g,
//            /<\/?SPAN[^>]*>/g,
//            /<\/?FONT[^>]*>/g,
//            /<\/?STRONG[^>]*>/g,
//            /<\/?H1[^>]*>/g,
//            /<\/?H2[^>]*>/g,
//            /<\/?H3[^>]*>/g,
//            /<\/?H4[^>]*>/g,
//            /<\/?H5[^>]*>/g,
//            /<\/?H6[^>]*>/g,
//            /<\/?P[^>]*><\/P>/g,
            /<!--(.*)-->/g,
            /<!--(.*)>/g,
            /<!(.*)-->/g,
            /<\\?\?xml[^>]*>/g,
            /<\/?o:p[^>]*>/g,
            /<\/?v:[^>]*>/g,
            /<\/?o:[^>]*>/g,
            /<\/?st1:[^>]*>/g,
//            /style=\"[^\"]*\"/g,
//            /style=\'[^\"]*\'/g,
            /lang=\"[^\"]*\"/g,
            /lang=\'[^\"]*\'/g,
            /class=\"[^\"]*\"/g,
            /class=\'[^\"]*\'/g,
            /type=\"[^\"]*\"/g,
            /type=\'[^\"]*\'/g,
            /href=\'#[^\"]*\'/g,
            /href=\"#[^\"]*\"/g,
            /name=\"[^\"]*\"/g,
            /name=\'[^\"]*\'/g,
            /clear=\"all\"/g,
            /id=\"[^\"]*\"/g,
            /title=\"[^\"]*\"/g,
//            /<span[^>]*>/g,
//            /<\/?span[^>]*>/g,
            /<title>(.*)<\/title>/g,
            /class=/g,
            /<meta[^>]*>/g,
            /<link[^>]*>/g,
            /<style>(.*)<\/style>/g,
            /<w:[^>]*>(.*)<\/w:[^>]*>/g
        ];

        Ext.each(removals, function (s) {
            wordPaste = wordPaste.replace(s, "");
        });

        // keep the divs in paragraphs
        wordPaste = wordPaste.replace(/<div[^>]*>/g, "<p>");
        wordPaste = wordPaste.replace(/<\/?div[^>]*>/g, "</p>");

        self.setValue(wordPaste);
    }

    /*
    cleanHtml: function (html) {

        html = this.fixWordPasteHtml(html);

        return Gsmu.HtmlEditor.superclass.cleanHtml.call(this, html);
    }
    */

});