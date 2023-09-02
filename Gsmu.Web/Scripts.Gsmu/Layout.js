
/*
all: 'images/icons/famfamfam/users_all.png',
students: 'images/icons/famfamfam/user_gray.png',
supervisors: 'images/icons/famfamfam/user_orange.png',
instructors: 'images/icons/famfamfam/user.png',
subadmin: 'images/icons/famfamfam/user_gray.png',

*/
function Layout(options) {
    var self = this;

    Ext.merge(self.Options, options);
    window.LAYOUT = self;

    if (self.Options.staticAdmin) {
        self.Options.noLayout = true;
        self.State.layoutLess = true;
    }

    self.State.queryString = UrlHelper.getUrlVars();

    var embedQuery = self.State.queryString["embed"];
    if (embedQuery) {
        embedQuery = embedQuery.toLowerCase();
        embedQuery = embedQuery == 'true' || embedQuery == '1' || embedQuery == 'yes' ? true : false;
        self.Options.isSiteEmbedded = embedQuery;
        self.setEmbeddedState(embedQuery);
    }

    if (window.top != window.self && self.State.queryString["embed"] && Ext.isSafari) {
        var tested = sessionStorage.getItem('iframetest');
        if (!tested) {
            var url = config.getUrl('landing/iframefix?redirect=' + encodeURIComponent(document.referrer), true);
            top.location = url;
            sessionStorage.setItem('iframetest', 1);
            return;
        }
    }

    self.InitializeStateManager();

    if (Ext.isIE && (Ext.ieVersion < 8 && Ext.ieVersion > 0) && self.Options.currentAction != 'browser' && self.Options.currentController != 'support') {
        document.location = config.getUrl('public/support/browser');
        return;
    }

    if (self.Options.noLayout == false) {
        self.State.showWelcomeMessage = true;

        self.State.layoutLess = typeof (self.State.queryString["layoutless"]) != 'undefined' && self.State.queryString["layoutless"].toLowerCase() == 'true';

        if (self.State.layoutLess) {
            self.Options.isSiteEmbedded = true;
        }

        if (self.Options.enableComposer == true) {
            self.State.layoutComposer = new CourseSearchLayoutComposer({
                displayMode: self.Options.displayMode,
                selectedLayout: self.Options.selectedLayout
            });
        }

    }

    Ext.onDocumentReady(function () {

        self.enableSessionTimer();

        Ext.QuickTips.init();
        var body = Ext.dom.Query.selectNode('#layout-container-top');
        if (body) {
            self.State.body = Ext.get(body);
        } else {
            self.State.body = Ext.getBody();
        }
        self.showMessages();

        if (window.top != window.self) {

            var task = Ext.TaskManager.start({
                interval: 1000,
                run: function () {
                    var height = Ext.dom.Element.getDocumentHeight();
                    if (Ext.isIE && Ext.ieVersion < 9) {
                        top.postMessage(height, "*");
                    } else {
                        top.postMessage({
                            height: height
                        }, "*");
                    }
                }
            });
        }

        if (self.Options.noLayout) {
            self.fireLayoutComplete();
            return;
        }

        if (!(self.Options.currentController == 'course' && self.Options.currentAction == 'browse')) {
            self.State.showWelcomeMessage = false;
        }

        if (self.State.layoutLess) {
            self.Options.Settings.PublicHeaderVisible = false;
            self.Options.Settings.PublicFooterVisible = false;
            $('.grad_stud_top_info').hide();
            $('#layout-welcomeuser').hide();
            $('#layout-top-info-middle').hide();
            $('#grad_stud_title').css({
                float: 'none',
                textAlign: 'left'
            });
            /*
            $('#grad_stud_title').hide();
            var topInfo = Ext.dom.Query.selectNode('.grad_stud_top_info');
            if (topInfo) {
                topInfo = Ext.get(topInfo);
                topInfo.enableDisplayMode();
                topInfo.hide();
            }
            */
        }

        self.InitLayoutEditor();
        self.RenderAdminMenu();
        self.fireLayoutComplete();
    });

}

Layout.prototype.MaskLayout = function (status) {
    var self = this;
    //self.State.body.mask(status);
    Ext.getBody().mask(status);
}

Layout.prototype.UnmaskLayout = function () {
    var self = this;
    //self.State.body.unmask();
    Ext.getBody().unmask();
}

Layout.prototype.constructor = Layout;

Layout.prototype.showMessages = function () {
    var self = this;

    if (Ext.isDefined(self.State.queryString['message'])) {
        self.notify((self.State.queryString['message']).replace(/<[^>]*>/g, ''));

        if (typeof (history.pushState) != 'undefined') {
            history.pushState(null, null, UrlHelper.getUrlWithoutVariable("message"));
        }
    }

    if (!Ext.isArray(self.Options.messages) || self.Options.messages.length < 1) {
        return;
    }

    var messages = self.Options.messages;
    var msg = '';
    for (var index = 0; index < messages.length; index++) {
        var newMessage = messages[index];
        if (msg != '') {
            msg += '<br/>';
        }
        msg += newMessage;
    }

    self.notify(msg);

}

Layout.prototype.setEmbeddedState = function (isEmbedded, success) {
    var self = this;

    Ext.Ajax.request({
        url: config.getUrl('public/layout/SetEmbeddedState'),
        params: {
            enabled: isEmbedded
        },
        success: function () {
            self.Options.isSiteEmbedded = isEmbedded;
            if (Ext.isFunction(success)) {
                success();
            }
        }
    });

    Ext.onDocumentReady(function () {
        if (isEmbedded) {
            Ext.getBody().addCls('embedded');
        } else {
            Ext.getBody().removeCls('embedded');
        }
    });
}

Layout.prototype.InitLayoutEditor = function () {
    var self = this;

    self.State.header.container = Ext.get(self.Options.headerContainerId);
    self.State.header.content = Ext.get(self.Options.headerContentId);
    self.State.header.editor = Ext.get(self.Options.headerEditorId);
    self.State.header.content.enableDisplayMode();
    self.State.header.visible = self.Options.Settings.PublicHeaderVisible && !self.Options.isSiteEmbedded;
    self.State.header.content.setVisible(self.Options.Settings.PublicHeaderVisible && !self.Options.isSiteEmbedded);

    self.State.footer.container = Ext.get(self.Options.footerContainerId);
    self.State.footer.content = Ext.get(self.Options.footerContentId);
    self.State.footer.editor = Ext.get(self.Options.footerEditorId);
    if (self.State.footer.content != null) {
        self.State.footer.content.enableDisplayMode();
        self.State.footer.content.setVisible(self.Options.Settings.PublicFooterVisible);
    }
    self.State.footer.visible = self.Options.Settings.PublicFooterVisible;    

    if (self.State.showWelcomeMessage) {
            self.State.welcome.container = Ext.get(self.Options.welcomeContainerId);
            self.State.welcome.content = Ext.get(self.Options.welcomeContentId);
            self.State.welcome.editor = Ext.get(self.Options.welcomeEditorId);
            self.State.welcome.content.enableDisplayMode();
            self.State.welcome.visible = self.Options.Settings.PublicWelcomeMessageVisible;
            self.State.welcome.content.setVisible(self.Options.Settings.PublicWelcomeMessageVisible);
            self.State.welcome.container.enableDisplayMode();
            self.State.welcome.container.show();
    }

    if (self.State.queryString["editlayout"]) {
        self.setlayoutEditMode(true);
    }
    if (self.State.queryString["colorScheme"]) {
        self.loadColorScheme();
    }

}

Layout.prototype.Options = {
    noLayout: false,
    staticAdmin: false,
    plainMenuItemStyle: {
        padding: '2px',
        marginBottom: '2px',
        marginRight: '2px'
    },
    showPastOnlineCourses: false,
    allowViewPastCourses: false,
    messages: [],
    developmentMode: false,
    isGsmuDevelopmentMachine: false,
    isSiteEmbedded: false,
    courseSearchSingleView: true,
    currentController: null,
    currentAction: null,
    selectedLayout: null,
    displayMode: 'normal',
    enableComposer: false,
    editIcon: 'Images/Icons/FamFamfam/pencil.png',
    saveIcon: 'Images/Icons/FamFamfam/disk.png',
    cancelIcon: 'Images/Icons/FamFamfam/cancel.png',
    uploadIcon: 'Images/Icons/FamFamfam/attach.png',
    exitIcon: 'Images/Icons/famfamfam/cross.png',
    layoutEditIcon: 'Images/Icons/FamFamfam/layout_edit.png',
    compositeLayoutButtonIcon: 'Images/Icons/FamFamfam/layout_content.png',
    componentsIcon: 'Images/Icons/FamFamFam/box.png',
    browserLinkId: null,
    adminMenuId: null,
    adminMenuContainerId: null,
    headerContainerId: null,
    headerContentId: null,
    headerEditorId: null,
    welcomeContainerId: null,
    welcomeContentId: null,
    welcomeEditorId: null,
    footerContainerId: null,
    footerContentId: null,
    footerEditorId: null,
    Settings: {
        PublicHeaderVisible: false,
        PublicFooterVisible: false,
        PublicWelcomeMessageVisible: false
    },
    layoutEditMode: false,
    adminMode: false,
    buttonLabels: {
        ClassFull: "Class full",
        WaitSpaceAvailable: "Wait space available",
        EmptyCart: "Empty cart",
        GoToCart: "Go to Cart",
        Login: "Login",
        CreateAccount: "Create account",
        Search: "Search",
        ClosedEnrollment: "Closed Enrollment",
        Cas_Login: "CAS Login",
        Canvas_Login : "Canvas Login"
    },
    layoutConfiguration: null,
    timeoutSeconds: 60 * 10,
    sessionStart: null,
    fieldNames: {
        Field1Name: null,
        Field2Name: null,
        Field3Name: null,
        Field4Name: null,
        Field5Name: null
    }
};

Layout.prototype.State = {
    layoutLess: false,
    adminMode: false,
    isButtonPage: false,
    saveLayoutConfigurationTimer: null,
    body: null,
    showWelcomeMessage: false,
    layoutComposer: null,
    queryString: {},
    editModeCancelActions: [],
    uploadForm: null,
    uploadFormHiddenField: null,
    uploadFormSourceHiddenField: null,
    uploadWindow: null,
    isEditMode: false,
    layoutEditorComponentsInitialized: false,
    buttonEditorInitialiezd: false,
    buttonEditing: {
        button: null,
        editing: false,
        textField: null
    },
    toolbar: null,
    enableEditingMenu: null,
    header: {
        content: null,
        container: null,
        editor: null,
        layoutEditIcon: null,
        visibleIcon: null,
        hiddenIcon: null,
        toolbox: null,
        visible: false
    },
    welcome: {
        content: null,
        container: null,
        editor: null,
        editIcon: null,
        visibleIcon: null,
        hiddenIcon: null,
        toolbox: null,
        visible: false
    },
    footer: {
        content: null,
        container: null,
        editor: null,
        layoutEditIcon: null,
        visibleIcon: null,
        hiddenIcon: null,
        toolbox: null,
        visible: false
    },
    layoutComplete: [],
    layoutCompleteFired: false
}

Layout.prototype.InitializeStateManager = function () {
    var self = this;

    Ext.onDocumentReady(function () {
        var stateProvider = Ext.create('Ext.state.CookieProvider', {
            //30 days
            expires: new Date(new Date().getTime() + (1000 * 60 * 60 * 24 * 30))
        });

        Ext.state.Manager.setProvider(stateProvider);
    });
}

Layout.prototype.startCheckout = function () {
    var self = this;
    if (self.State.welcome.container) {
        self.State.welcome.container.hide();
    }
    Ext.select('#course-search-keyword-container, #grad_stud_title').hide();
};

Layout.prototype.endCheckout = function() {
    var self = this;
    if (self.State.welcome.container) {
        self.State.welcome.container.show();
    }
    Ext.select('#course-search-keyword-container, #grad_stud_title').show();
};

Layout.prototype.saveLayoutConfiguration = function (viewType) {
    var self = this;

    clearTimeout(self.State.saveLayoutConfigurationTimer);
    self.State.saveLayoutConfigurationTimer = setTimeout(function () {
        Ext.Ajax.request({
            url: config.getUrl('public/layout/layoutconfiguration'),
            method: 'POST',
            params: {
                layoutConfiguration: Ext.encode(self.Options.layoutConfiguration)
            },
            success: function (response) {
                if (window.COURSESEARCH) {
                    window.COURSESEARCH.Invoke();
                }
            }
        });
    }, 1000);
};

Layout.prototype.DecideAdminMode = function() {
    var self = this;

    if (self.Options.staticAdmin) {
        self.State.adminMode = true;
        return true;
    }

    var nonQueryAdminMode = Ext.isDefined(self.State.queryString['adminmode']) && self.State.queryString['adminmode'].toLowerCase() == 'false';

    if (nonQueryAdminMode) {
        self.State.adminMode = false;
    } else {
        var queryAdminMode = Ext.isDefined(self.State.queryString['adminmode']) && self.State.queryString['adminmode'].toLowerCase() == 'true';

        if (queryAdminMode) {
            Ext.util.Cookies.set('adminmode', 'true');
        }

        var cookieAdminMode = false;
        if (self.Options.developmentMode) {
            var cookie = Ext.util.Cookies.get('adminmode');
            if (cookie != null && cookie == 'true') {
                cookieAdminMode = true;
            }
            self.State.adminMode = queryAdminMode || cookieAdminMode;
        } else {
            self.State.adminMode = queryAdminMode && self.Options.adminMode;
        }
    }
    return self.State.adminMode;
}

Layout.prototype.RenderAdminMenuSearchLayoutItems = function () {
    var self = this;

    var configMenus = {
        Grid: [],
        TileJuly: []
    };

    var menuIcons = {
        Grid: {
            Icon: 'images/icons/famfamfam/application_view_list.png',
            Title: 'Grid',
            Id: 'layout-view-button-grid'
        },
        TileJuly: {
            Icon: 'images/icons/famfamfam/application_view_tile.png',
            Title: 'Tiles',
            Id: 'layout-view-button-tilejuly'
        }
    };

    var columnLabels = {
        CourseDescription: 'Course description',
        Sessions: 'Session(s)',
        Duration: 'Duration',
        Location: 'Location',
        Status: 'Status',
        ClassSize: 'Class size',
        AccessCode: 'Access code',
        CourseId: 'Course ID',
        CourseNumber: 'Course #',
        CourseStart: 'Start date/time',
        CourseEnd: 'End date/time',
        Credit: 'Credit(s)',
        Material: 'Material(s)',
        Price: 'Prices(s)',
        PreRequisite: 'Pre-requisite',
        Icons: 'Icon(s)',
        AdjustLayoutComponents: '<strong>Align components horizontally</strong>',
        Image: 'Image'
    };

    var tileJulyItems = [];

    for (var area in self.Options.layoutConfiguration.SearchColumns) {
        if (area == 'DefaultView') {
            continue;
        }
        for (var key in self.Options.layoutConfiguration.SearchColumns[area]) {
            var menuItem = null;
            
            if (key == 'TileWidth' || key == 'TileImageWidth' || key == 'TileImageHeight') {

                var labels = {
                    TileWidth: 'Tile width',
                    TileImageWidth: 'Tile image width',
                    TileImageHeight: 'Tile image height'
                }

                tileJulyItems.push({ text: labels[key], disabled: true });
                tileJulyItems.push({
                    layoutArea: area,
                    layoutKey: key,
                    xtype: 'numberfield',
                    minValue: 10,
                    maxValue: 1000,
                    value: self.Options.layoutConfiguration.SearchColumns[area][key],
                    listeners: {
                        change: function (item, value) {
                            clearTimeout(self.State.saveLayoutConfigurationTimer);
                            self.Options.layoutConfiguration.SearchColumns[item.layoutArea][item.layoutKey] = value;
                            self.saveLayoutConfiguration(item.layoutArea);
                        }
                    }
                });
            } else {
                menuItem = {
                    layoutArea: area,
                    layoutKey: key,
                    xtype: 'menucheckitem',
                    text: columnLabels[key],
                    checked: self.Options.layoutConfiguration.SearchColumns[area][key],
                    checkHandler: function (item, checked) {
                        clearTimeout(self.State.saveLayoutConfigurationTimer);
                        self.Options.layoutConfiguration.SearchColumns[item.layoutArea][item.layoutKey] = checked;
                        self.saveLayoutConfiguration(item.layoutArea);
                    }
                };

                if (key == 'AdjustLayoutComponents') {
                    tileJulyItems.push(menuItem);
                } else {
                    if (area == 'Grid' && key == 'CourseEnd') {
                    } else {
                        configMenus[area].push(menuItem);
                    }
                }
            }
        }
    }

    var items = [];

    var configHasViewEnabled = function () {
        var hasEnabled = false;
        for (var key in self.Options.layoutConfiguration.SearchColumns) {
            if (typeof(key) != 'undefined' && key.endsWith('ViewEnabled') && self.Options.layoutConfiguration.SearchColumns[key] == true) {
                hasEnabled = true;
                break;
            }
        }
        return hasEnabled;
    }

    for (var key in configMenus) {

        var menuConfig = menuIcons[key];
        var menuItems = configMenus[key];
        var onbutton = {
            id: menuConfig.Id + 'on',
            xtype: 'splitbutton',
            ui: 'default-toolbar',
            gsmuMenuConfig: menuConfig,
            gsmuArea: key,
            text: menuConfig.Title + '<input type="checkbox" checked="true" style=" vertical-align: -10%;">',
            icon: config.getUrl(menuConfig.Icon),
            menu: menuItems,
            tooltip: menuConfig.Title + ' is currently enabled.',
            tooltipType: 'title',
            hidden: !self.Options.layoutConfiguration.SearchColumns[key + 'ViewEnabled'],
            handler: function (button, state) {
                if (configHasViewEnabled()) {
                    clearTimeout(self.State.saveLayoutConfigurationTimer);
                    self.Options.layoutConfiguration.SearchColumns[button.gsmuArea + 'ViewEnabled'] = false;
                    button.setVisible(false);
                    Ext.getCmp(button.gsmuMenuConfig.Id + 'off').setVisible(true);
                    window.COURSELISTINGTYPE.Render();
                    self.saveLayoutConfiguration(button.gsmuArea);
                    Ext.getCmp('layout-default-view-menu').setVisible(false);
                } else {
                    self.notify('Sorry, but at least one view needs to be enabled!');
                }
            }
        };

        var offbutton = {
            id: menuConfig.Id + 'off',
            xtype: 'button',
            ui: 'default-toolbar',
            gsmuMenuConfig: menuConfig,
            gsmuArea: key,
            text: menuConfig.Title + '<input type="checkbox" style=" vertical-align: -10%;">',
            icon: config.getUrl(menuConfig.Icon),
            tooltip: menuConfig.Title + ' is currently disabled.',
            tooltipType: 'title',
            hidden: self.Options.layoutConfiguration.SearchColumns[key + 'ViewEnabled'],
            handler: function (button, state) {
                clearTimeout(self.State.saveLayoutConfigurationTimer);
                Ext.getCmp('layout-default-view-menu').setVisible(true);
                button.setVisible(false);
                Ext.getCmp(button.gsmuMenuConfig.Id + 'on').setVisible(true);
                self.Options.layoutConfiguration.SearchColumns[button.gsmuArea + 'ViewEnabled'] = true;
                window.COURSELISTINGTYPE.Render();
                self.saveLayoutConfiguration(button.gsmuArea);
            }
        };

        if (key == 'TileJuly') {
            var menus = onbutton.menu;
            onbutton.menu = tileJulyItems.concat(menus);
        }
        items.push(onbutton);
        items.push(offbutton);

    };

    items.push({
        xtype: 'cycle',
        id: 'layout-default-view-menu',
        hidden: !self.Options.layoutConfiguration.SearchColumns.GridViewEnabled || !self.Options.layoutConfiguration.SearchColumns.TileJulyViewEnabled,
        showText: true,
        prependText: 'Default view: ',
        icon: config.getUrl(menuIcons[self.Options.layoutConfiguration.SearchColumns.DefaultView].Icon),
        menu: {
            items: [
                {
                    text: 'Grid',
                    viewCode: 'Grid',
                    checked: self.Options.layoutConfiguration.SearchColumns.DefaultView == 'Grid',
                    icon: config.getUrl('images/icons/famfamfam/application_view_list.png')
                }, {
                    text: 'Tile',
                    viewCode: 'TileJuly',
                    checked: self.Options.layoutConfiguration.SearchColumns.DefaultView == 'TileJuly',
                    icon: config.getUrl('images/icons/famfamfam/application_view_tile.png')
                }
            ]
        },
        changeHandler: function (cmp, item) {
            clearTimeout(self.State.saveLayoutConfigurationTimer);
            self.Options.layoutConfiguration.SearchColumns.DefaultView = item.viewCode;
            cmp.setIcon(
                config.getUrl(menuIcons[item.viewCode].Icon)
            );
            self.saveLayoutConfiguration();
        }
    });

    return items;

}

Layout.prototype.RenderAdminMenuLayoutItems = function () {
    var self = this;

    var items = [];

    var tipInfo = 'When this mode is on, you are able to edit the header, the welcome message and the footer. So to get the most out of this function, you may visit multiple pages. Such as course search, the home page and etc..';
    var editModeInfo = 'Edit mode is on. \n\n' + tipInfo;
    var nonEditModeInfo = 'Edit mode is off. \n\n' + tipInfo;

    var headerButton;
    if (self.Options.isSiteEmbedded) {
        headerButton = Ext.create('Ext.menu.Item', {
            text: 'Header component',
            tooltip: 'The home page, has a header component, which in embedded mode is turned off, to be able to edit it, please get out of embedded mode',
            tooltipType: 'title',
            icon: config.getUrl(self.Options.hiddenIcon),
            handler: function (that) {
                alert(that.tooltip);
            }
        });
    } else {
        headerButton = self.createContentButton('header');
    }

    var footerButton = self.createContentButton('footer');

    var welcomeButton;
    if (self.State.showWelcomeMessage) {
        welcomeButton = self.createContentButton('welcome');
    } else {
        welcomeButton = Ext.create('Ext.menu.Item', {
            text: 'Welcome component',
            tooltip: 'The home page, has a welcome component, you can configure it after clicking this button, and coming to this same menu item after',
            tooltipType: 'title',
            icon: config.getUrl(self.Options.welcomeComponentIcon),
            handler: function () {
                document.location = config.getUrl('public/home/index');
            }
        });
    }

    var checkLayoutEditMode = function (button, isToggled) {
        self.setlayoutEditMode(isToggled);
        var edit = Ext.getCmp('layout-toolbar-layout-items');
        if (self.State.isEditMode) {
            edit.show();
            layoutModeButton.setTooltip(editModeInfo);
            layoutModeButton.setText('Turn off layout edit mode');
        } else {
            edit.hide();
            layoutModeButton.setTooltip(nonEditModeInfo);
            layoutModeButton.setText('Turn on layout edit mode');
        }
    };

    var layoutModeButton = Ext.create('Ext.button.Button', {
        id:'layoutModeButton',
        enableToggle: true,
        text: self.State.isEditMode ? 'Turn off layout edit mode' : 'Turn on layout edit mode',
        pressed: self.State.isEditMode,
        icon: config.getUrl('images/icons/famfamfam/layout_edit.png'),
        tooltip: self.State.isEditMode ? editModeInfo : nonEditModeInfo,
        tooltipType: 'title',
        toggleHandler: checkLayoutEditMode
    });

    items.push(headerButton);
    items.push(footerButton);
    items.push(welcomeButton);

    return {
        button: layoutModeButton,
        menu: {
            xtype: 'button',
            ui: 'default-toolbar',
            text: 'Component',
            icon: config.getUrl('images/icons/famfamfam/layout_content.png'),
            menu: items
        }
    };
}

Layout.prototype.RenderAdminMenuEmbedMenu = function (container) {
    var self = this;

    var embedItems = [];

    var embedIcon = config.getUrl('Images/Icons/FamFamFam/script_code.png');
    embedItems.push({
        xtype: 'menuitem',
        text: 'Get embed script',
        icon: embedIcon,
        tooltip: 'Gives you the script to place the web site into an IFrame',
        tooltipType: 'title',
        listeners: {
            click: function (button) {
                /*
        <iframe id="gsmu-embedded" scrolling="no" frameborder="none" height="500" width="920" style="border: none; margin: 0;"></iframe>
        <script type="text/javascript">  
            (function () {
                var el = document.getElementById('gsmu-embedded');
                var address = (location.protocol.toLowerCase() == 'file:' ? 'http:' : location.protocol) + '//' + '" + location.host + "/landing/embedded';
                el.src = address;
                if (!window.publicnetComponentReceiver) {
                    window.publicnetComponentReceiver = function (event) {
                        var height;
                        if (typeof(event.data) != 'undefined' && typeof(event.data.height) != 'undefined') {
                            height = event.data.height;
                        } else {
                            height = event.data;
                        }
                        document.getElementById('gsmu-embedded').style.height = height + "px";
                    };
                }
                if (window.removeEventListener) {
                    window.removeEventListener('message', window.publicnetComponentReceiver);
                } else if (window.detachEvent) {
                    window.detachEvent('onmessage', window.publicnetComponentReceiver);
                }
                if (window.addEventListener) {
                    window.addEventListener('message', window.publicnetComponentReceiver);
                } else if (window.attachEvent) {
                    window.attachEvent('onmessage', window.publicnetComponentReceiver);
                }
            })();
        </script>
                */
                var scriptCode = '';
                scriptCode += '<iframe id="gsmu-embedded" scrolling="no" frameborder="none" height="500" width="920" style="border: none; margin: 0;"></iframe>\n';
                scriptCode += '<script type="text/javascript">';
                scriptCode += "(function(){var e=document.getElementById('gsmu-embedded');var t=(location.protocol.toLowerCase()=='file:'?'http:':location.protocol)+'//" + location.host + "/landing/embedded';e.src=t;if(!window.publicnetComponentReceiver){window.publicnetComponentReceiver=function(e){var t;if(typeof e.data!='undefined'&&typeof e.data.height!='undefined'){t=e.data.height}else{t=e.data}document.getElementById('gsmu-embedded').style.height=t+'px'}}if(window.removeEventListener){window.removeEventListener('message',window.publicnetComponentReceiver)}else if(window.detachEvent){window.detachEvent('onmessage',window.publicnetComponentReceiver)}if(window.addEventListener){window.addEventListener('message',window.publicnetComponentReceiver)}else if(window.attachEvent){window.attachEvent('onmessage',window.publicnetComponentReceiver)}})()";
                scriptCode += '</script>';

                Ext.create('Ext.window.Window', {
                    title: 'Embed window code',
                    icon: embedIcon,
                    height: 200,
                    width: 400,
                    layout: 'fit',
                    modal: true,
                    items: [
                        {
                            id: 'embedscript',
                            xtype: 'textareafield',
                            selectOnFocus: true,
                            grow: true,
                            name: 'message',
                            anchor: '100%',
                            value: scriptCode
                        }
                    ]
                }).show();
            }
        }
    });

    embedItems.push({
        xtype: 'menucheckitem',
        text: 'Embedded mode',
        checked: self.Options.isSiteEmbedded,
        icon: config.getUrl('Images/Icons/FamFamFam/arrow_in.png'),
        tooltip: 'Enable/disable the embedded mode',
        tooltipType: 'title',
        checkHandler: function (button, isPressed) {
            self.setEmbeddedState(isPressed, function () {                
                self.InitLayoutEditor();
                self.RenderAdminMenu();
                self.setlayoutEditMode(self.State.isEditMode);
            });
        }
    });

    container.push({
        xtype: 'button',
        ui: 'default-toolbar',
        text: 'Embed',
        icon: config.getUrl('Images/icons/famfamfam/arrow_inout.png'),
        menu: embedItems
    });


    container.push({
        xtype: 'button',
        ui: 'default-toolbar',
        icon: config.getUrl('Images/icons/famfamfam/help.png'),
        style: {
            marginRight: '10px'
        },
        listeners: {
            render: function (button) {
                var tip = Ext.create('Ext.tip.ToolTip', {
                    target: button.id,
                    anchor: 'bottom',
                    anchorToTarget: true,
                    showDelay: 0,
                    hideDelay: 10000,
                    anchorOffset: -8,
                    html: 'When the "Embed Mode" is turned on you can test and setup your site in embed mode. To actually use embed mode on a company web page managed by content management systems like Drupal or WordPress, use the "Get embed script" option. If your company builds their own  website it could also be inserted into the existing code.<br/><br/>In embed mode, the maximum Tile width is 219 pixels.<br/><br/>In normal mode, the maximum Tile width is 232 pixels.<br/><br/>Please, make sure you change the Tile width according to your mode, otherwise your layout will be off.<br/><br/>FYI: In embed mode the header is automatically disabled.'
                });
            }
        }
    });
    
}

Layout.prototype.RenderAdminMenuCompositeLayoutItems = function () {
    var self = this;

    var compositeLayoutItems = [];
    if (self.State.layoutComposer != null) {
        compositeLayoutItems = self.State.layoutComposer.renderAdminModeMenu(true);
    } else {
        compositeLayoutItems = [{
            text: CourseSearchComponentRegistry.variables.compositeSearchButtonTextEntrance,
            icon: config.getUrl(self.Options.compositeLayoutButtonIcon),
            tooltip: CourseSearchComponentRegistry.variables.compositeSearchNonEditModeInfo,
            tooltipType: 'title',
            listeners: {
                click: function () {
                    document.location = config.getUrl('public/course/browse/composite/edit');
                }
            }
        }];
    }

    return {
        icon: config.getUrl('images/icons/famfamfam/layout.png'),
        text: 'Course search layout',
        menu: compositeLayoutItems
    };
}

Layout.prototype.RenderAdminMenuSsoItems = function () {
    var self = this;

    var ssoItems = [];

    ssoItems.push({
        text: 'Blackboard settings',
        icon: config.getUrl('Images/IntegrationPartners/lti_blackboard.png'),
        handler: function () {
            document.location = config.getUrl('Adm/blackboard/settings');
        }
    });

    ssoItems.push({
        text: 'Canvas settings',
        icon: config.getUrl('Images/IntegrationPartners/lti_canvas.png'),
        handler: function () {
            document.location = config.getUrl('Adm/canvas/settings');
        }
    });

    ssoItems.push({
        text: 'Haiku settings',
        icon: config.getUrl('Images/IntegrationPartners/lti_haiku.png'),
        handler: function () {
            document.location = config.getUrl('Adm/haiku/settings');
        }
    });

    ssoItems.push({
        text: 'LTI settings',
        icon: config.getUrl('Images/IntegrationPartners/lti_lti.png'),
        handler: function () {
            document.location = config.getUrl('Adm/lti/settings');
        }
    });

    ssoItems.push('-');

    ssoItems.push({
        text: 'SSO/redirect URLs',
        disabled: true
    });

    ssoItems.push({
        text: 'SSO Urls page',
        icon: config.getUrl('Images/Icons/FamFamFam/lock_edit.png'),
        handler: function () {
            document.location = config.getUrl('Adm/home/sso');
        }
    });


    ssoItems.push({
        text: 'Blackboard SSO Url',
        icon: config.getUrl('Images/IntegrationPartners/lti_blackboard.png'),
        handler: function () {
            var url = location.protocol + '//' + location.host + config.getUrl('SSO/Blackboard');
            config.showInfo(url, 'The GSMU SSO URL for the Blackboard Plugin');
        }
    });

    ssoItems.push({
        text: 'Google SSO Redirect Url',
        icon: config.getUrl('Images/Icons/socialmediaicons/google-16x16.png'),
        handler: function () {
            var url = location.protocol + '//' + location.host + config.getUrl('SSO/Google');
            config.showInfo(url, 'The GSMU SSO redirect URL for the Google integration');
        }
    });

    ssoItems.push({
        text: 'LTI SSO Url',
        icon: config.getUrl('Images/IntegrationPartners/lti_lti.png'),
        handler: function () {
            var url = location.protocol + '//' + location.host + config.getUrl('SSO/Lti');
            config.showInfo(url, 'The GSMU SSO URL for the LTI Tool Consumer');
        }
    });

    ssoItems.push({
        text: 'Canvas SSO Url',
        icon: config.getUrl('Images/IntegrationPartners/lti_canvas.png'),
        handler: function () {
            var url = location.protocol + '//' + location.host + config.getUrl('SSO/Canvas');
            config.showInfo(url, 'The Canvas SSO redirect URL');
        }
    });


    return {
        ui: 'default-toolbar',
        text: 'SSO/Integration',
        icon: config.getUrl('Images/Icons/FamFamFam/lock_open.png'),
        menu: ssoItems
    };

}

Layout.prototype.RenderAdminMenuDevelopment = function () {

    var self = this;

    var scriptLoaded = false;
    var development;

    Ext.Loader.loadScript({
        url: config.getUrl('Areas/Public/Scripts/Development.js'),
        onLoad: function () {
            scriptLoaded = true;
            development = new Development();
        },
        onError: function (msg) {
            alert('Script loading error! Please contact admin or refresh browser.\r\n' + msg);
        }
    });

    var waitForScript = function (callback) {
        if (!scriptLoaded) {
            setTimeout(function () {
                waitForScript(callback);
            }, 500);
        } else {
            callback();
        }
    }

    var menu = [];





    menu.push({
        ui: 'default-toolbar',
        text: 'Public site settings',
        icon: config.getUrl('Images/Icons/FamFamFam/settings.png'),
        menu: [
            {
                xtype: 'menucheckitem',
                text: 'Show past online courses',
                icon: config.getUrl('Images/Icons/FamFamFam/timeline_marker.png'),
                checked: self.Options.showPastOnlineCourses,
                tooltip: 'Show online courses that have a Start Date in the past but an End Date in the future',
                tooltipType: 'title',
                checkHandler: function (control, value) {
                    self.setMasterinfoValue(1, "ShowPastOnlineCourses", value ? 1 : 0, function () {
                        self.Options.showPastOnlineCourses = value;
                        if (window.COURSESEARCH) {
                            window.COURSESEARCH.Invoke();
                        }
                    });
                }
            },
            {
                xtype: 'menucheckitem',
                text: 'Allow to view past courses',
                icon: config.getUrl('Images/Icons/FamFamFam/calendar_view_day.png'),
                checked: self.Options.allowViewPastCourses,
                tooltip: 'Allows to view past courses that have the setting enabled',
                tooltipType: 'title',
                checkHandler: function (control, value) {
                    self.setMasterinfoValue(3, "allowviewpastcoursesdays", value ? 1 : 0, function () {
                        self.Options.allowViewPastCourses = value;
                        if (window.COURSESEARCH) {
                            window.COURSESEARCH.Invoke();
                        }
                    });
                }
            },
            {
                xtype: 'menucheckitem',
                text: 'Fast loading for public course listing',
                icon: config.getUrl('Images/Icons/FamFamFam/lightning.png'),
                checked: self.Options.publicCourseListingFastLoad,
                tooltip: 'By default, the course listing page loads the categories and course list via Ajax after the page is loaded. If this option is enabled the initial load is done all from the server at high speed. The cost is that at initial development we implemented hash variables in the url which the server can not see. In order to load the last view state, it will need some development work.',
                tooltipType: 'title',
                checkHandler: function (control, value) {
                    self.setMasterinfoValue(4, "public_course_listing_fast_load", value ? 1 : 0, function () {
                        self.Options.publicCourseListingFastLoad = value;
                    });
                }
            }, {
                xtype: 'menuseparator'
            },
            {
                text: 'Buttons labels',
                icon: config.getUrl('Images/Icons/famfamfam/bullet_green.png'),
                handler: function (control, value) {
                    document.location = config.getUrl('public/layout/button-labels-and-coloring?adminmode=true');
                }
            }
        ]
    });


    menu.push({
        ui: 'default-toolbar',
        text: 'Developer settings',
        icon: config.getUrl('Images/Icons/FamFamFam/wrench.png'),
        menu: [
            {
                disabled: self.Options.isGsmuDevelopmentMachine,
                text: 'Development mode',
                xtype: 'menucheckitem',
                icon: config.getUrl('Images/Icons/FamFamFam/wrench_orange.png'),
                checked: self.Options.developmentMode,
                tooltip: self.Options.isGsmuDevelopmentMachine ? 'This server is configured to be in permament development mode in the App_Data file DevelopmentMachineList configuration key. If you wish to be able to change this setting, remove localhost or your domain from that config value.' :'Warning! This wil reload the whole page!',
                //tooltipType: 'title',
                checkHandler: function (control, value) {
                    self.MaskLayout('Setting development mode: ' + (value ? 'on' : 'off'));
                    Ext.Ajax.request({
                        method: 'POST',
                        url: config.getUrl('Adm/development/setdevelopmentmode'),
                        params: {
                            state: value
                        },
                        success: function () {
                            location.reload();
                        }
                    });
                }
            },
            {
                text: 'Send test e-mail',
                icon: config.getUrl('Images/Icons/FamFamFam/email.png'),
                handler: function () {
                    waitForScript(development.SendEmail);
                }
            }

        ]
    });

    menu.push(
        self.RenderAdminMenuSsoItems()
    );

    menu.push({
        ui: 'default-toolbar',
        text: 'Courses',
        icon: config.getUrl('Images/Icons/FamFamFam/bell.png'),
        menu: [
            {
                text: 'Course E-mail Confirmation',
                icon: config.getUrl('Images/Icons/glyph2/Icons16x16/button-sale.png'),
                handler: function (control, value) {
                    document.location = config.getUrl('Adm/confirmationscreen/confirmation');
                }
            },
            {
                text: 'User fields',
                icon: config.getUrl('Images/Icons/famfamfam/building.png'),
                handler: function (control, value) {
                    document.location = config.getUrl('Adm/userfields');
                }
            },
            {
                text: 'Attendance taking',
                icon: config.getUrl('Images/Icons/famfamfam/clock_edit.png'),
                handler: function (control, value) {
                    document.location = config.getUrl('Adm/attendance');
                }
            }
        ]
    });


    if (!self.Options.courseSearchSingleView) {
        menu.push(
            self.RenderAdminMenuCompositeLayoutItems()
        );
    }


    return {
        xtype: 'button',
        ui: 'default-toolbar',
        text: 'Admin',
        icon: config.getUrl('Images/Icons/FamFamFam/ruby.png'),
        menu: menu
    }



}

Layout.prototype.RenderAdminMenu = function () {
    var self = this;

    if (!self.DecideAdminMode()) {
        return;
    }

    var centeringContainer = Ext.get(self.Options.adminMenuContainerId);
    centeringContainer.show();

    var container = Ext.get(self.Options.adminMenuId);
    if (!container.setHtml) {
        container.update('');
    } else {
        container.setHtml('');
    }
    if (Ext.dom.Query.selectNode('.grad_stud_top_all')) {
        Ext.get(Ext.dom.Query.selectNode('.grad_stud_top_all')).addCls('layout-editing-spacer');
    }


    var toggle = Ext.get('layout-admin-toggle');
    if (toggle) {
        toggle.hide();
    }


    var toolbarItems = [];

    var layoutToolbarRegularItems = [];

    if (self.Options.staticAdmin) {
        toolbarItems.push({
            text: 'GSMU Public Home',
            icon: config.getUrl('Images/Icons/glyph2/icons16x16/home.png'),
            handler: function () {
                document.location = config.getUrl('public/course/browse?adminmode=true');
            }
        });
    }


    layoutToolbarRegularItems.push(
        self.RenderAdminMenuDevelopment()
    );

    toolbarItems.push({
        xtype: 'container',
        id: 'layout-toolbar-regular-items',
        items: layoutToolbarRegularItems
    });

    //toolbarItems.push({
    //    xtype: 'button',
    //    id: 'colorSchemeLayoutBtn',
    //    ui: 'default-toolbar',
    //    text: 'Color Schemes',
    //    icon: config.getUrl('Images/Icons/famfamfam/color_wheel.png'),
    //    handler: function (control, value) {
    //        self.loadColorScheme();
    //    }
    //});


    var redirectHandler = function (redirect) {
        return function (cmp, ev) {
            var url = config.getUrl('application/AdminFunction?call=go-admin&redirect=' + redirect);
            if (ev.ctrlKey || ev.shiftKey) {
                window.open(url, "_blank");
            } else {
                top.location = url;
            }
        }
    }

    if (window.location == top.location) {
        toolbarItems.push({
            xtype: 'button',
            text: 'Portal',
            tooltip: 'Visit the GSMU V3 Admin Portal',
            icon: config.getUrl('images/icons/famfamfam/application_side_list.png'),
            handler: redirectHandler('portal.asp')
        });

        toolbarItems.push({
            xtype: 'button',
            text: 'Dash',
            tooltip: 'Visit the GSMU V3 Course Dashboard',
            icon: config.getUrl('images/icons/famfamfam/coursedash.png'),
            handler: redirectHandler('course-center.asp')
        });
    }





    toolbarItems.push({
        xtype: 'tbfill'
    });


    if (!self.State.layoutLess) {
        var layoutItems = self.RenderAdminMenuLayoutItems();
        var layoutContainerItems = self.RenderAdminMenuSearchLayoutItems();
        layoutContainerItems.unshift(layoutItems.menu);
        var embedItems = [];
        self.RenderAdminMenuEmbedMenu(embedItems);

        layoutContainerItems = embedItems.concat(layoutContainerItems);

        toolbarItems.push({
            xtype: 'container',
            id: 'layout-toolbar-layout-items',
            items: layoutContainerItems,
            hidden: !self.State.isEditMode
        });

        toolbarItems.push(layoutItems.button);
    }

    if (!self.Options.staticAdmin) {
        toolbarItems.push({
            xtype: 'tbseparator'
        });

        toolbarItems.push({
            xtype: 'button',
            icon: config.getUrl(self.Options.exitIcon),
            tooltip: 'Exit admin mode',
            tooltipType: 'title',
            listeners: {
                click: function () {
                    self.exitAdminMode();
                }
            }
        });

    }

    var toolbar = Ext.create('Ext.toolbar.Toolbar', {
        renderTo: container,
        width: container.getWidth(),
        height: 36,
        items: toolbarItems,
        id: 'layout-toolbar-main'
    });

    if (self.Options.staticAdmin) {
        var setToolbarWidth = function () {
            var view = Ext.getBody().getViewSize();
            toolbar.setWidth(view.width - 10);
        }
        Ext.on('resize',setToolbarWidth);
        setTimeout(setToolbarWidth, 200);
    }

    if (self.Options.LayoutEditMode) {
        self.State.enableEditingMenu.setChecked(true);
    }

    if (self.State.layoutComposer != null) {
        self.State.layoutComposer.postMenuRender();
    }

}

Layout.prototype.exitAdminMode = function () {
    var self = this;

    self.InitLayoutEditor();
    self.setlayoutEditMode(false);
    Ext.util.Cookies.set('adminmode', null);
    var oldToolbar = Ext.getCmp('layout-toolbar-main');
    if (oldToolbar) {
        oldToolbar.destroy();
    }
    if (typeof (history.pushState) != 'undefined') {
        history.pushState(null, null, UrlHelper.getUrlWithoutVariable("adminmode"));
    }
    var toggle = Ext.get('layout-admin-toggle');
    if (toggle) {
        toggle.show();
    }
}

Layout.prototype.enterAdminMode = function () {
    var self = this;

    var oldToolbar = Ext.getCmp('layout-toolbar-main');
    if (oldToolbar) {
        oldToolbar.destroy();
    }

    var toggle = Ext.get('layout-admin-toggle');
    if (toggle) {
        toggle.hide();
    }
    if (typeof (history.pushState) != 'undefined') {
        var url = UrlHelper.getUrlWithoutVariable("admin");
        url += url.indexOf('?') > -1 ? '&' : '?';
        url += 'adminmode=true';
        history.pushState(null, null, url);
    }

    Ext.util.Cookies.set('adminmode', 'true');

    self.InitLayoutEditor();
    self.RenderAdminMenu();
    self.setlayoutEditMode(false);

}

Layout.prototype.createContentButton = function (area) {
    var self = this;

    var showText = 'Show ' + area + ' component';
    var hideText = 'Hide ' + area + ' component';

    var icon;
    switch (area) {
        case 'header':
            icon = config.getUrl('images/icons/famfamfam/arrow_up.png');
            break;

        case 'welcome':
            icon = config.getUrl('images/icons/famfamfam/arrow_right.png');
            break;

        case 'footer':
            icon = config.getUrl('images/icons/famfamfam/arrow_down.png');
            break;

    }

    var button = Ext.create('Ext.menu.CheckItem', {
        text: !self.State[area].visible ? showText : hideText,
        icon: icon,
        handler: function (button) {
            var pressed = !self.State[area].visible;
            self.State[area].visible = pressed;
            if (pressed) {
                button.setText(hideText);
            } else {
                button.setText(showText);
            }
            self.saveContentVisibilitySetting(area);
            self.showContentBySetting(area);
        }
    });

    button.setChecked(self.State[area].visible);
    return button;
};

Layout.prototype.showContentBySetting = function (area) {
    var self = this;

    self.State[area].content.setVisible(self.State[area].visible);
}

Layout.prototype.loadColorScheme = function () {
    var self = this;
    self.setlayoutEditMode(false);
    if(typeof(Ext.getCmp("layoutModeButton"))!="undefined"){
        Ext.getCmp("layoutModeButton").toggle(false);
    }
    Ext.Ajax.request({
        method: 'POST',
        url: config.getUrl('public/layout/GetHistoryBGColor?adminmode=true'),
        success: function (response) {
            var dta = response.responseText;
            $('#BackGroundColorThemeHistory')
                .empty()
                .append('<option value="-">(Previously Applied)</option>')
            ;

            if (dta.length > 10) {
                var arr = dta.split("|");
                var arrcnt = arr.length - 1
                for (i = 0; i <= arrcnt; i++) {
                    var orgdta = arr[i]
                    var arrdta = Ext.decode(JSON.parse(arr[i]))


                    $('#BackGroundColorThemeHistory').append($("<option/>", {
                        value: orgdta,
                        text: arrdta.ThemeDate
                    }));

                }
            }

            $("#BackGroundColorTheme").dialog({
                title: "Color Scheme",
                modal: false,
                draggable: true,
                height: 600,
                width: 200,
                resizable: true,
                beforeClose: function (event, ui) {
                    //Ext.getCmp("layoutModeButton").toggle(true);
                },
                position: [28, 28]
            });

        }
    });

}


Layout.prototype.setlayoutEditMode = function (isEdit) {
    var self = this;
    self.State.isEditMode = isEdit;
    if (isEdit) {
        self.EnsureLayoutEditorComponents();
        //self.EnsureButtonEditing();

        if (!self.Options.isSiteEmbedded) {
            self.State.header.container.addCls('layout-item-edit-mode');
        }
        self.State.footer.container.addCls('layout-item-edit-mode');

        if (self.State.showWelcomeMessage) {
            self.State.welcome.container.addCls('layout-item-edit-mode');
        }
    } else {
        if (!self.Options.isSiteEmbedded) {
            self.State.header.container.removeCls('layout-item-edit-mode');
        }
        self.State.footer.container.removeCls('layout-item-edit-mode');
        
        if (self.State.showWelcomeMessage) {
            self.State.welcome.container.removeCls('layout-item-edit-mode');
        }

        for (var index = 0 ; index < self.State.editModeCancelActions.length; index++) {
            var func = self.State.editModeCancelActions[index];
            func();
        }
    }
}

Layout.prototype.EnsureUploadForm = function () {
    var self = this;

    if (self.State.uploadFormHiddenField != null) {
        return;
    }
    
    self.State.uploadFormHiddenField = Ext.create('Ext.form.field.Hidden', {
        name: 'area'
    });

    self.State.uploadFormSourceHiddenField = Ext.create('Ext.form.field.Hidden', {
        name: 'source'
    });

    self.State.uploadForm = Ext.create('Ext.form.Panel', {
        frame: true,
        items: [{
            xtype: 'fileuploadfield',
            name: 'file',
            fieldLabel: 'File',
            labelWidth: 50,
            msgTarget: 'side',
            allowBlank: false,
            anchor: '100%',
            text: 'Select File...',
            buttonText: 'Select File...',
            listeners: {
                change: function (control, value) {
                    value = new String(value);
                    var lastIndex = value.lastIndexOf('/')
                    if (lastIndex == -1) {
                        lastIndex = value.lastIndexOf('\\');
                    }
                    var fileName = value.substring(lastIndex + 1);
                    control.inputEl.dom.value = fileName;
                }
            }
        },
            self.State.uploadFormHiddenField
        ],
        buttons: [{
            text: 'Upload',
            icon: config.getUrl(self.Options.uploadIcon),
            handler: function () {
                var form = this.up('form').getForm();

                area = self.State.uploadFormHiddenField.getValue();

                if (form.isValid()) {

                    var source = self.State.uploadFormSourceHiddenField.getValue();
                    var data = null;
                    if (source != '') {
                        data = eval(source);
                    } else {
                        data = {
                            url: 'public/layout/uploadcontentfile',
                            editor: Ext.getCmp('layout-area-editor-' + area)
                        };
                    }

                    form.submit({
                        url: config.getUrl(data.url),
                        waitMsg: 'Uploading your file...',
                        success: function (fp, o) {
                            var mime = o.result.mime;
                            var file = o.result.file;
                            var area = self.State.uploadFormHiddenField.getValue();
                            data.editor.focus();
                            if (mime.startsWith('image')) {
                                data.editor.insertAtCursor('<img src="' + config.getUrl(file) + '"/>');
                            } else {
                                var url = config.getUrl(file);
                                data.editor.insertAtCursor('<a href="' + url + '">' + url + '</a>');
                            }
                            self.State.uploadWindow.hide();
                        }
                    });
                }
            }
        },
        {
            text: 'Cancel',
            icon: config.getUrl(self.Options.cancelIcon),
            handler: function () {
                self.State.uploadWindow.hide();
            }
        }]
    });

    self.State.uploadWindow = Ext.create('Ext.window.Window', {
        title: 'Upload a file',
        modal: true,
        resizable: false,
        closeAction: 'hide',
        items: [
            self.State.uploadForm
        ]
    });
}

Layout.prototype.EnsureLayoutEditorComponents = function () {
    var self = this;

    if (self.State.layoutEditorComponentsInitialized) {
        return;
    }

    self.createLayoutComponent(self.State.header.container, self.State.header.content, self.State.header.editor, 'Edit the header content', 'header', self.Options.Settings.PublicHeaderVisible);

    if (self.State.showWelcomeMessage) {
        self.createLayoutComponent(self.State.welcome.container, self.State.welcome.content, self.State.welcome.editor, 'Edit the welcome content', 'welcome', self.Options.Settings.PublicWelcomeMessageVisible);
    }

    self.createLayoutComponent(self.State.footer.container, self.State.footer.content, self.State.footer.editor, 'Edit the footer content', 'footer', self.Options.Settings.PublicFooterVisible);

    self.EnsureUploadForm();

    self.State.layoutEditorComponentsInitialized = true;
}

Layout.prototype.createLayoutComponent = function (container, content, editor, title, area) {
    var self = this;

    var editIconUrl = config.getUrl(self.Options.layoutEditIcon);
    var dh = Ext.DomHelper;
    var saveIcon = config.getUrl(self.Options.saveIcon);
    var cancelIcon = config.getUrl(self.Options.cancelIcon);

    var toolbox = dh.append(document.body, {
        tag: 'div',
        cls: 'layout-toolbox'
    }, true);

    var editIcon = dh.append(toolbox, {
        tag: 'img',
        src: editIconUrl,
        cls: 'layout-edit-icon',
        title: title
    }, true);


    self.State[area].toolbox = toolbox;
    self.State[area].editIcon = editIcon;

    var editorCmp = Ext.create('Gsmu.HtmlEditor', {
        id: 'layout-area-editor-' + area,
        renderTo: editor,
        width: editor.getWidth() - 8,
        height: 200
    });
    var editorToolbar = editorCmp.getToolbar();
    editorToolbar.add({
        xtype: 'tbseparator'            
    });
    editorToolbar.add({
        tooltip: {
            title: 'Insert file',
            text: 'If the file you upload does not appear, make sure to place the cursor in the editor window to a valid location before you upload.'
        },
        tooltipType: 'title',
        icon: config.getUrl(self.Options.uploadIcon),
        handler: function () {
            self.State.uploadForm.getForm().reset();
            self.State.uploadFormHiddenField.setValue(area);
            self.State.uploadFormSourceHiddenField.setValue('');
            self.State.uploadWindow.show();
        }
    });

    editorToolbar.add({
        xtype: 'tbfill'
    });
    editorToolbar.add({
        tooltip: {
            text: 'Save'
        },
        icon: saveIcon,
        handler: function () {
            var html = editorCmp.getValue();
            content.setHtml(html);
            self.showContentBySetting(area);
            editor.hide();

            Ext.Ajax.request({
                url: config.getUrl('public/layout/savecontent'),
                params: {
                    area: area,
                    html: editorCmp.getValue()
                }
            });

        }
    });
    editorToolbar.add({
        tooltip: {
            text: 'Cancel'
        },
        icon: cancelIcon,
        handler: function () {
            self.showContentBySetting(area);
            editor.hide();
        }
    });
    editor.enableDisplayMode();
    editor.hide();

    toolbox.hide();

    editIcon.on('click', function () {
        content.hide();
        editor.show();
        editorCmp.setValue(content.getHtml());
        toolbox.hide();
    });

    container.on('mouseover', function () {
        if (self.State.isEditMode && !editor.isVisible()) {
            toolbox.show();
            var position = container.getBox();
            toolbox.setX(position.x + position.width - toolbox.getWidth());
            toolbox.setY(position.y);
            container.addCls('layout-content-hover');
        }
    }).on('mouseout', function() {
        if (self.State.isEditMode) {
            toolbox.hide();
            container.removeCls('layout-content-hover');
        }
    });

    toolbox.on('mouseover', function () {
        if (self.State.isEditMode) {
            toolbox.show();
            container.addCls('layout-content-hover');
        }
    }).on('mouseout', function() {
        if (self.State.isEditMode) {
            toolbox.hide();
            container.removeCls('layout-content-hover');
        }
    });
}

Layout.prototype.saveContentVisibilitySetting = function (area, setting) {
    var self = this;

    Ext.Ajax.request({
        url: config.getUrl('public/layout/SaveContentVisibilitySetting'),
        params: {
            area: area,
            visible: self.State[area].visible
        }
    });
}

Layout.prototype.EnsurePixelLabel = function () {
    var self = this;

    var ajaxTimeout = null;

    var render = Ext.get('component-pixel-size-renderer');

    var numbers = Ext.create({
        xtype: 'numberfield',
        name: 'increaseWordTopRow',
        width: 200,
        labelWidth: 140,
        labelAlign: 'right',
        fieldLabel: 'Increase word pixels',
        value: self.Options.layoutConfiguration.IncreaseWordTopRow,
        maxValue: 25,
        minValue: 5,
        renderTo: render,
        listeners: {
            change: function (item, value) {
                self.Options.layoutConfiguration.IncreaseWordTopRow = value;
                var update = Ext.get(Ext.query('.component-pixel-top-row')[0]);
                update.setStyle('fontSize', value + 'px');
                           
                clearTimeout(ajaxTimeout);
                ajaxTimeout = setTimeout(function () {
                    render.mask();
                    Ext.Ajax.request({
                        url: config.getUrl('public/layout/SetIncreaseWordTopRow'),
                        params: {
                            increaseWordTopRow: value
                        },
                        success: function () {
                            render.unmask();
                            Ext.get('word-top-increase').setStyle('fontSize', value + 'px');
                        }
                    });
                }, 1000);
            }
        }
    });
    numbers.hide();

    var component = Ext.get('word-top-increase-component');
    var clearComponent = null;
    component.on('mouseover', function () {
        clearTimeout(clearComponent);
        numbers.show();
    }).on('mouseout', function () {
        clearComponent = setTimeout(function () {
            numbers.hide();
        }, 3000)
    });

    Ext.get(document).on('click', function () {
        clearTimeout(clearComponent);
        numbers.hide();
    });
}

Layout.prototype.EnsureButtonEditing = function () {

    var self = this;

    if (self.State.buttonEditorInitialiezd) {
        return;
    }

    var dh = Ext.DomHelper;
    var toolbox = dh.append(document.body, {
        tag: 'div',
        cls: 'layout-toolbox'
    }, true);
    var pencil = dh.append(toolbox, {
        tag: 'img',
        src: config.getUrl(self.Options.editIcon),
        cls: 'layout-edit-icon',
        title: 'Click to edit button text'
    }, true);
    var saveIcon = dh.append(toolbox, {
        tag: 'img',
        src: config.getUrl(self.Options.saveIcon),
        cls: 'layout-edit-icon',
        title: 'Click to save edited button text'
    }, true);
    var cancelIcon = dh.append(toolbox, {
        tag: 'img',
        src: config.getUrl(self.Options.cancelIcon),
        cls: 'layout-edit-icon',
        title: 'Click to cancel editing button text'
    }, true);
    var toolboxHideTimeout = null;
    pencil.enableDisplayMode();
    saveIcon.enableDisplayMode();
    cancelIcon.enableDisplayMode();

    var resetEditing = function () {
        saveIcon.hide();
        pencil.show();
        cancelIcon.hide();
        toolbox.hide();
        self.State.buttonEditing.editing = false;
        clearTimeout(toolboxHideTimeout);
    };
    resetEditing();

    var isEditMode = function () {
        if (!(self.State.isEditMode || self.State.isButtonPage) || self.State.buttonEditing.editing == true) {
            return true;
        }
        return false;
    }

    var enableEditableButtons = function () {

        if (window.CourseSearchInstance != null && window.CourseSearchInstance.IsLoading) {
            setTimeout(enableEditableButtons, 100);
            return;
        }

        var buttons = Ext.query('.button-component');

        Ext.Array.each(buttons, function (value, key) {
            var element = Ext.get(value);

            if (element.getAttribute('data-editable') == 'true') {
                return;
            }
            element.set({
                'data-editable': 'true'
            });

            element.on('mouseover', function (evt, target, options) {
                clearTimeout(toolboxHideTimeout);
                if (isEditMode()) {
                    return;
                }
                var element = Ext.get(target);
                placeToolbox(element);
                self.State.buttonEditing.button = element;
            }).on('mouseout', function () {
                if (isEditMode()) {
                    return;
                }
                toolboxHideTimeout = setTimeout(function () {
                    toolbox.hide();
                }, 500);
            }).on('dblclick', function (evt, target, options) {
                if (!self.State.isButtonPage) {
                    return;
                }
                if (isEditMode()) {
                    cancelAction();
                }
                var element = Ext.get(target);
                placeToolbox(element);
                self.State.buttonEditing.button = element;
                pencil.dom.click();
            });
        });
    };

    toolbox.on('mouseover', function () {
        if (isEditMode()) {
            return;
        }
        clearTimeout(toolboxHideTimeout);
    }).on('mouseout', function () {
        if (isEditMode()) {
            return;
        }
        toolboxHideTimeout = setTimeout(function () {
            toolbox.hide();
        }, 500);
    });

    var placeToolbox = function(el) {
        toolbox.show();
        var position = el.getBox();
        toolbox.setX(position.x + position.width + 3);
        toolbox.setY(position.y);
    }

    var cancelAction = function () {
        var button = self.State.buttonEditing.button;        
        if (button != null) {
            var text = button.getAttribute('data-label');
            if (text != null) {
                button.set({
                    onclick: button.getAttribute('data-onclick'),
                    "data-onclick": null,
                    "data-label": null
                });
                button.setHtml(text);
            }
            self.State.buttonEditing.button = null;
        }
        resetEditing();
    };
    self.State.editModeCancelActions.push(cancelAction);

    pencil.on('click', function () {
        self.State.buttonEditing.editing = true;
        var button = self.State.buttonEditing.button;
        var onclick = button.getAttribute('onclick');
        pencil.hide();
        cancelIcon.show();
        saveIcon.show();
        placeToolbox(button);
        var text = button.getHtml();
        button.set({
            onclick: null,
            "data-onclick": onclick,
            "data-label": text
        });
        button.setHtml('');
        var text = Ext.create('Ext.form.field.Text', {
            value: text.trim(),
            renderTo: button,
            grow: true,
            growMin: 10,
            growMax: 200
        });
        text.setFieldStyle({
            'border': 'none',
            'background-color': 'transparent',
            'background': 'none',
            'color': 'inherit',
            'font-family': 'inherit',
            'font-weight': 'inherit',
            'font-size': 'inherit'
        });
        text.focus(true);
        self.State.buttonEditing.textField = text;
    });


    cancelIcon.on('click', cancelAction);
    saveIcon.on('click', function () {
        var button = self.State.buttonEditing.button;
        cancelAction();
        self.State.buttonEditing.button = null;
        var type = button.getAttribute('data-button-type');
        var text = self.State.buttonEditing.textField.getValue();
        self.saveButtonLabel(type, text);
        var buttons = Ext.query('[data-button-type=' + type + ']');
        Ext.Array.each(buttons, function (value, key) {
            var element = Ext.get(value);
            element.setHtml(text);
        });
    });

    enableEditableButtons();

    Ext.Ajax.on('beforerequest', function () {
        if (self.State.isEditMode) {
            cancelAction();
        }
    });
    Ext.Ajax.on('requestcomplete', function () {
        if (self.State.isEditMode) {
            self.MaskLayout('Setting buttons ...');
            setTimeout(function () {
                self.UnmaskLayout();
                enableEditableButtons();
            }, 500);
        }
    });
    


    self.State.buttonEditorInitialiezd = true;
}

Layout.prototype.saveButtonLabel= function (button, label) {
    var self = this;

    Ext.Ajax.request({
        url: config.getUrl('public/layout/SaveButtonLabel'),
        params: {
            button: button,
            label: label
        }
    });
}

var BgfldCounter = 0;
var BgfldpropTotal = 0;

Layout.prototype.SaveApplyAllBGColors = function () {
    var self = this;

    var r = confirm("Are you sure to apply this scheme colors to default?");
    if (r == true) {

        Ext.Ajax.request({
            url: config.getUrl('public/layout/AllBGColors?adminmode=true'),
            success: function (response) {
                var model = Ext.decode(response.responseText);
                var Bgfldprop = model.Data;
                BgfldpropTotal = Bgfldprop.length;

                for (var ifld = 0; ifld < Bgfldprop.length; ifld++) {
                    var field = Bgfldprop[ifld].field;
                    var color = $("#BGColor" + field).val();
                    Bgfldprop[ifld].color = color;
                }
                Layout.prototype.saveBGColorSeq(Bgfldprop)
            }
        });
    }
}

Layout.prototype.saveBGColorSeq = function (Bgfldprop) {
    var self = this;
    var field = Bgfldprop[BgfldCounter].field;
    var color = Bgfldprop[BgfldCounter].color;
    Ext.Ajax.request({
        url: config.getUrl('public/layout/SaveBGColor?adminmode=true'),
        params: {
            field: field,
            color: color
        },
        success: function () {
            if (BgfldCounter < (BgfldpropTotal-1)) {
                BgfldCounter += 1;
                Layout.prototype.saveBGColorSeq(Bgfldprop)
            } else {
                Layout.prototype.FinalizeBgfldprop(Bgfldprop)
            }
        }
    });
}

Layout.prototype.FinalizeBgfldprop = function (Bgfldprop) {
    var self = this;

    var todayDate = new Date();
    var format = "AM";
    var hour = todayDate.getHours();
    var min = todayDate.getMinutes();
    if (hour > 11) { format = "PM"; }
    if (hour > 12) { hour = hour - 12; }
    if (hour == 0) { hour = 12; }
    if (min < 10) { min = "0" + min; }
    var formatdatetoday = todayDate.getMonth() + 1 + "/" + todayDate.getDate() + "/" + todayDate.getFullYear() + " " + hour + ":" + min + " " + format;

    var jsondata = JSON.stringify({ ThemeTitle: formatdatetoday, ThemeDate: formatdatetoday, Data: Ext.encode(Bgfldprop) });

    Ext.Ajax.request({
        url: config.getUrl('public/layout/AppendHistoryBGColor?adminmode=true'),
        params: {
            jsondata: jsondata
        },
        success: function () {
            $("#BackGroundColorTheme").dialog('close');
            //Ext.getCmp("layoutModeButton").toggle(true);
            alert('successfully saved.');
        }
    });
}

Layout.prototype.saveBGColor = function (field) {
    var self = this;
    var color = $("#BGColor" + field).val();
    Ext.Ajax.request({
        url: config.getUrl('public/layout/SaveBGColor?adminmode=true'),
        params: {
            field: field,
            color: color
        },
        success: function () {
            alert('successfully saved.');

        }
    });
}

Layout.prototype.resetAllBGColors = function () {
    var self = this;

    var r = confirm("Are you sure to reset all background colors to default?");
    if (r == true) {

        Ext.Ajax.request({
            url: config.getUrl('public/layout/resetAllBGColors'),
            success: function () {
                window.location.reload();

            }
        });
    }

}

Layout.prototype.notify = function (text, once) {
    var self = this;

    if (typeof (once) == 'undefined') {
        once = false;
    }

    if (once) {
        var id = 'layout-notify{' + text + '}';
        if (window.sessionStorage.getItem(id) != null) {
            return;
        }
        window.sessionStorage.setItem(id, true);
    }

    $.jGrowl(text, {
        themeState: '',
        position: 'top-right',
        life: 10000,
        afterOpen: function ($e, message, options, jgrowl) {
            var $message = $e.find('.jGrowl-message');
            $message.css('cursor', 'pointer');
            $message.click(function () {
                $e.find('.jGrowl-close').trigger('click');
            });
        }
    });
}

Layout.prototype.setSameHeight = function (selector, parent, itemContainerSelector) {
    var self = this;

    var containers = Ext.query(selector);

    if (containers.length < 2) {
        return;
    }

    var columns, rows;
    if (parent != undefined) {

        var itemContainers = containers;
        if (itemContainerSelector != undefined) {
            itemContainers = Ext.query(itemContainerSelector);
        }

        var parentWidth = parent.getWidth();
        var elementWidth = Ext.get(itemContainers[0]).getWidth();
        columns = Math.floor(parentWidth / elementWidth);
        rows = Math.floor(containers.length / columns);
    }

    var maxHeights = [];

    var maxHeight = -1;
    var counter = 0;
    Ext.Array.forEach(containers, function (value) {
        maxHeight = Math.max(maxHeight, Ext.get(value).getHeight());
        counter++;
        if (counter == columns) {
            counter = 0;
            maxHeights.push(maxHeight);
            maxHeight = -1;
        }
    });
    if (counter < columns) {
        maxHeights.push(maxHeight);
    }

    if (maxHeight > -1 || maxHeights.length > 0) {
        counter = 0;
        var column = 0;
        Ext.Array.forEach(containers, function (value) {
            if (maxHeights.length > 0) {
                maxHeight = maxHeights[counter];
            }
            Ext.get(value).setHeight(maxHeight);

            column++;

            if (column == columns) {
                counter++;
                column = 0;
            }
        });
    }

}

Layout.prototype.setMasterinfoValue = function (id, key, value, callback) {
    var self = this;

    self.MaskLayout('Saving configuration...');
    Ext.Ajax.request({
        url: config.getUrl('Adm/datastore/SetMasterinfoValue'),
        params: {
            id: id,
            key: key,
            value: value
        },
        success: function (response) {
            if (Ext.isFunction(callback)) {
                callback();
            }
            self.UnmaskLayout();
        }
    });
}

Layout.prototype.FooterBrowserCompatibility = function () {
    var self = this;

    var window = Ext.create('Ext.container.Container', {
        floating: true,
        componentCls: 'login-popup',
        shadow: true,
        html: self.Options.FooterBrowserCompatibility,
        listeners: {
            show: function () {
                Ext.getDoc().addListener('click', function () {
                    window.hide();
                });
            }
        }
    });
    window.show();

    var position = $("#link-BrowserCompatibility").position();
    var left = position.left
    var top = position.top - window.getHeight() - 2;
    window.setPosition(left, top, false);
}


Layout.prototype.enableSessionTimer = function () {
    var self = this;

    var el = Ext.get('session-timeout');
    var start = new Date();
    var timeout = self.Options.timeoutSeconds - 10;

    $(document).ajaxComplete(function (event, xhr, settings) {
        start = new Date();
    });

    Ext.Ajax.on('requestcomplete', function (conn, response, options, eopts) {
        start = new Date();
    });

    var notified = false;

    var updateDisplay = function () {

        var now = new Date();
        var end = Ext.Date.add(start, Ext.Date.SECOND, timeout);

        var secondsLeft = Math.round((end - now) / 1000);
        var timeLeft = new String(secondsLeft);
        if (secondsLeft >= 0) {

            if (secondsLeft < 60 && !notified) {
                LAYOUT.notify('Your session is about to expire. When it expires, you will receive a new session and if you were logged in, you will be automatically logged out.');
                notified = true;
            }
            
            if (el != null) {
                el.setHtml(timeLeft.toHHMMSS());
            }
            setTimeout(updateDisplay, 1000);
        } else {
            self.logout(true);
        }
    }
    updateDisplay();
}

Layout.prototype.logout = function (sessionExpiry) {
    var self = this;

    if (typeof (sessionExpiry) == 'undefined') {
        sessionExpiry = false;
    }

    config.Options.DisableAjaxErrorHandling = true;

    if (sessionExpiry) {
        window.LAYOUT.MaskLayout('Your session has expired, you are getting a new session for security reasons');
    } else {
        window.LAYOUT.MaskLayout('Logging out');
    }
    Ext.Ajax.request({
        url: config.getUrl('public/membership/Logout'),
        autoAbort: true,
        success: function (response) {
            var result = Ext.decode(response.responseText);
            var originalMsg = 'You have been successfully logged out!';
            var msg = self.getResultMessage(originalMsg, result.messages);

            if (originalMsg == msg && typeof(MEMBERSHIP) != 'undefined') {
                document.location = config.getUrl('public/course/browse');
                return;
            }
            window.LAYOUT.UnmaskLayout();

            Ext.MessageBox.show({
                //animateTarget: 'link-logout',
                title: 'Logout',
                msg: msg,
                buttons: Ext.MessageBox.OK,
                buttonAlign: 'right',
                icon: Ext.MessageBox.INFO,
                fn: function () {
                    location.reload();
                }
            });

        }
    });
}

Layout.prototype.getResultMessage = function (msg, messages) {
    if (!Ext.isArray(messages) || messages.length < 1) {
        return msg;
    }

    msg += '<br/><br/><strong>During the process the system has also provided the following information:<strong><br/>';
    for (var index = 0; index < messages.length; index++) {
        var newMessage = messages[index];
        msg += '<br/>';
        msg += newMessage;
    }
    return msg;
}



Layout.prototype.setPageTitle = function (title) {
    var self = this;

    document.title = title;
    Ext.get('page-title').setHtml(title);
}

Layout.prototype.fireLayoutComplete = function () {
    var self = this;

    self.State.layoutCompleteFired = true;
    for (var index = 0; index < self.State.layoutComplete.length; index++) {
        var callback = self.State.layoutComplete[index];
        callback();
    }
}

Layout.prototype.onLayoutComplete = function(callback) {
    var self = this;

    if (self.State.layoutCompleteFired) {
        callback();
    } else {
        self.State.layoutComplete.push(callback);
    }
}

Layout.prototype.PrevwColor = function (me,fieldname) {
    var self = this;
    var color = me.value;
    if (fieldname.indexOf("text") > -1) {
        $(".BG" + fieldname).css("color", "#" + color);
    } else {
        $(".BG" + fieldname).css("background-color", "#" + color);
    }
}


Layout.prototype.ApplyHistoryBGColors = function (me) {
    var self = this;
    var arrdta = Ext.decode(JSON.parse(me.value))
    var fldprop = Ext.decode(arrdta.Data);
    for (var ifld = 0; ifld < fldprop.length; ifld++) {
        var fieldname = fldprop[ifld].field;
        var color = fldprop[ifld].color;


        $("#BGColor" + fieldname).val(color);
        $("#BGColor" + fieldname).css("background-color", "#" + color);
        if (fieldname.indexOf("text") > -1) {
            $(".BG" + fieldname).css("color", "#" + color);
        } else {
            $(".BG" + fieldname).css("background-color", "#" + color);
        }
    }
}
