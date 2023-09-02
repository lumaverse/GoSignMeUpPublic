// outdated!!!
// requires membership to be running
function CourseSearchLayoutComposer(options) {
    var self = this;

    self.Options = Ext.merge(self.Options, options);

    if (self.Options.selectedLayout != null) {
        self.Settings = self.Options.selectedLayout;
    }

    Ext.onDocumentReady(function () {
        self.initializeState();
        self.renderComponents();
        //        self.postMenuRender();
        if (window.LAYOUT.DecideAdminMode()) {
            config.showInfo('Default empty layout loaded. You may manage your layouts now.');
        }
    });

    window.LAYOUTCOMPOSER = self;
}

CourseSearchLayoutComposer.constructor = CourseSearchLayoutComposer;

CourseSearchLayoutComposer.prototype.Options = {
    selectedLayout: null,
    courseSearch: null,    
    displayMode: 'normal',
    containerId: 'composite-layout',
    contentContainerId: 'composite-layout-content-container',
    editModeClass: 'composite-layout-edit-mode',
    areaIds: {
        top: 'composite-layout-top',
        left: 'composite-layout-left',
        contentNavigation: 'composite-layout-content-navigation',
        content: 'composite-layout-content'
    },
    menuIds: {
        components: 'layout-composer-menu-component-list'
    },
    icons: {
        layoutMenuIcon: 'Images/Icons/FamFamFam/layout.png',
        layoutSaveMenuIcon: 'Images/Icons/FamFamFam/layout_add.png',
        layoutLoadMenuIcon: 'Images/Icons/FamFamFam/layout_edit.png',
        layoutSaveAsMenuIcon: 'Images/Icons/FamFamFam/layout_link.png',
        layoutDeleteMenuIcon: 'Images/Icons/FamFamFam/layout_delete.png',
        layoutEmptyMenuIcon: 'Images/Icons/FamFamFam/picture_empty.png',
        visitLayoutIcon: 'Images/Icons/FamFamFam/page_white_link.png',
        components: 'Images/Icons/FamFamFam/box.png'
    }
}

CourseSearchLayoutComposer.prototype.VirginSettings = {
    areas: {
        top: {
            components: new Array()
        },
        left: {
            components: new Array()
        },
        content: {
            components: new Array()
        },
        contentNavigation: {
            components: new Array()
        }
    }
};

// IMPORTANT!!! order of the 2 variables above and below IMPORTANT!!!
CourseSearchLayoutComposer.prototype.Settings = CourseSearchLayoutComposer.prototype.VirginSettings;

CourseSearchLayoutComposer.prototype.State = {
    container: null,
    areas: {
        top: null,
        left: null,
        content: null,
        contentNavigation: null
    },
    contentContainer: null,
    courseSearch: null,
    componentObjects: null,
    compositeLayoutButton: null,
    layouts: null
}

CourseSearchLayoutComposer.prototype.clearLayout = function () {
    var self = this;

    self.clearLayoutArea('content', self.Settings.areas.content.components);
    self.clearLayoutArea('contentNavigation', self.Settings.areas.contentNavigation.components);
    self.clearLayoutArea('left', self.Settings.areas.left.components);
    self.clearLayoutArea('top', self.Settings.areas.top.components);
}

CourseSearchLayoutComposer.prototype.clearLayoutArea = function (area,components) {
    var self = this;

    for (var index = 0; index < components.length; index++) {
        var cmp = components[index];
        self.componentCreator(area, cmp);
    }
    components.length = 0;
}

CourseSearchLayoutComposer.prototype.addComponent = function (area, item) {
    var self = this;

    var options = {
        container: self.getLayoutContainer(area, item)
    };

    Ext.get(options.container).set({
        'data-component-key': item.key
    });

    var constructorSpecs = item.className + '.ConstructForLayout;';
    var constructor = eval(constructorSpecs);
    var object = constructor(self, options);
    item.state.rendered = true;
    item.state.containerId = options.container.id;
    item.state.area = area;

    return object;
}

CourseSearchLayoutComposer.prototype.removeComponent = function (area, item) {
    var self = this;

    Ext.get(item.state.containerId).remove();
    item.state.rendered = false;
    item.state.containerId = null;
    item.state.area = null;
}

CourseSearchLayoutComposer.prototype.componentCreator = function (area, item) {
    var self = this;

    var object;
    if (item.state.rendered == true) {
        object = self.removeComponent(area, item);
    } else {
        object = self.addComponent(area, item);
    }

    CourseSearchComponentRegistry.components[item.key] = item;
    // the layout composer menu moved to the admin mode menu
    var menuItem = Ext.getCmp(
        CourseSearchComponentRegistry.methods.getKey(item.key)
    );

    if (menuItem) {
        menuItem.setChecked(item.state.rendered);
    }

    self.manageLayout();

    return object;
}

CourseSearchLayoutComposer.prototype.updateComponentMenuStateBySettings = function () {
    var self = this;

    for (var key in CourseSearchComponentRegistry.components) {
        var item = CourseSearchComponentRegistry.components[key];
        var menuId = CourseSearchComponentRegistry.methods.getKey(key);
        var menu = Ext.getCmp(menuId);
        if (menu) {
            menu.setChecked(item.state.rendered);
        } else {
            log('LayoutComposer menu missing: ' + menuId);
        }
    }
}


CourseSearchLayoutComposer.prototype.manageLayout = function () {
    var self = this;

    self.readSettingsFromLayout();

    if (self.Settings.areas.left.components.length > 0) {
        self.State.areas.left.addCls('has-left');
        self.State.contentContainer.addCls('has-left');
    } else {
        var hadLeft = self.State.areas.left.hasCls('has-left');
        self.State.areas.left.removeCls('has-left');
        self.State.contentContainer.removeCls('has-left');

        if (hadLeft) {
            self.State.courseSearch.Invoke();
        }
    }
}

CourseSearchLayoutComposer.prototype.renderComponents = function () {
    var self = this;

    // existing components
    var renderedOnes = [];

    // render each area
    for (var area in self.Settings.areas) {
        for (var key in self.Settings.areas[area].components) {
            var item = self.Settings.areas[area].components[key];
            item.state.area = area;
            item.state.rendered = false;
            var object = self.componentCreator(area, item);
            renderedOnes.push(item.key);
        }
    }

    
    for (var key in CourseSearchComponentRegistry.components) {
        var item = CourseSearchComponentRegistry.components[key];
        if (Ext.isBoolean(item.required) && item.required == true && !Ext.Array.contains(renderedOnes, item.key)) {
            item.state.rendered = false;
            item.state.area = item.defaultPlacement;
            var object = self.componentCreator(item.defaultPlacement, item);
            if (item.className == 'CourseSearch') {
                self.State.courseSearch = object;
            }
        }
    }
}

CourseSearchLayoutComposer.prototype.getLayoutContainer = function (area, item) {
    var self = this;

    var dh = Ext.DomHelper;
    var spec = {
        id: Ext.id(),
        tag: 'div',
        cls: 'composite-layout-component-container',
        children: [
            {
                id: Ext.id(),
                tag: 'div',
                cls: 'composite-layout-component-title',
                html: item.title
            },
            {
                id: Ext.id(),
                tag: 'div',
                cls: 'composite-layout-component',
            }
        ]
    };

    var container = dh.append(self.State.areas[area], spec, true);
    return container;
}

CourseSearchLayoutComposer.prototype.getLayoutContainerComponentContainerId = function (container) {
    var self = this;
    var renderContainer = Ext.dom.Query.selectNode('.composite-layout-component', container.dom);
    var id = renderContainer.id;
    return id;
}

CourseSearchLayoutComposer.prototype.setDisplayMode = function (mode) {
    var self = this;

    if (!Ext.isDefined(mode)) {
        mode = self.Options.displayMode;
    }

    $('.composite-layout-area').sortable({
        connectWith: '.composite-layout-area',
        placeholder: 'composite-layout-placeholder',
        appendTo: document.body,
        cursor: 'move',
        forceHelperSizeType: true,
        forcePlaceholderSize: true,
        handle: '.composite-layout-component-title',
        item: '> .composite-layout-component-container',
        opacity: 0.5,
        revert: true,
        scroll: true,
        update: function () {
// settings could be automatically saved here
        }
    });

    if (mode == 'edit') {
        $('.composite-layout-area').sortable('enable');
        $('.composite-layout-component-title').show();

        self.State.container.addCls(self.Options.editModeClass);
        self.Options.displayMode = "edit";
    } else {

        $('.composite-layout-component-title').hide();
        $('.composite-layout-area').sortable('disable');

        self.State.container.removeCls(self.Options.editModeClass);
        self.Options.displayMode = "normal";
    }

    var editMode = self.isEditMode();

    var compositeLayoutButton = self.State.compositeLayoutButton;

    if (editMode) {
        compositeLayoutButton.setText(CourseSearchComponentRegistry.variables.compositeSearchButtonTextOn);
        compositeLayoutButton.setTooltip(CourseSearchComponentRegistry.variables.compositeSearchEditModeInfo);
    } else {
        if (compositeLayoutButton != null) {
            compositeLayoutButton.setText(CourseSearchComponentRegistry.variables.compositeSearchButtonTextOff);
            compositeLayoutButton.setTooltip(CourseSearchComponentRegistry.variables.compositeSearchNonEditModeInfo);
        }
    };

    for (var key in CourseSearchComponentRegistry.components) {
        var item = CourseSearchComponentRegistry.components[key];
        var menuId = CourseSearchComponentRegistry.methods.getKey(key);
        var menu = Ext.getCmp(menuId);
        if (item.disabled) {
            menu.setDisabled(true);
        } else {
            // make it always enabled instead of based on edit mode
           // menu.setDisabled(!editMode);
        }
    }

    self.manageLayout();
}

CourseSearchLayoutComposer.prototype.switchDisplayMode = function () {
    var self = this;
    self.setDisplayMode(self.Options.displayMode == 'edit' ? 'normal' : 'edit');
}

CourseSearchLayoutComposer.prototype.isEditMode = function () {
    var self = this;
    return self.Options.displayMode == 'edit';
}

CourseSearchLayoutComposer.prototype.readSettingsFromLayout = function () {
    var self = this;

    self.Settings = self.VirginSettings;
    
    var components = null;
    var list = null;

    var getSettingsComponents = function (components) {
        var list = [];
        for (var index = 0; index < components.length; index++) {
            var el = Ext.get(components[index]);
            var componentKey = el.getAttribute('data-component-key');
            var component = CourseSearchComponentRegistry.components[componentKey];
            list.push(component);
        }
        return list;
    }

    //top
    components = Ext.DomQuery.select('#composite-layout-top .composite-layout-component-container');
    self.Settings.areas.top.components = getSettingsComponents(components);

    //left
    components = Ext.DomQuery.select('#composite-layout-left .composite-layout-component-container');
    self.Settings.areas.left.components = getSettingsComponents(components);

    //content navigation
    components = Ext.DomQuery.select('#composite-layout-content-navigation .composite-layout-component-container');
    self.Settings.areas.contentNavigation.components = getSettingsComponents(components);

    //content
    components = Ext.DomQuery.select('#composite-layout-content .composite-layout-component-container');
    self.Settings.areas.content.components = getSettingsComponents(components);

}

CourseSearchLayoutComposer.prototype.initializeState = function () {
    var self = this;

    self.State.courseSearch = new CourseSearch();

    self.State.container = Ext.get(self.Options.containerId);
    self.State.contentContainer = Ext.get(self.Options.contentContainerId);
    self.State.areas.content = Ext.get(self.Options.areaIds.content);
    self.State.areas.contentNavigation = Ext.get(self.Options.areaIds.contentNavigation);
    self.State.areas.left = Ext.get(self.Options.areaIds.left);
    self.State.areas.top = Ext.get(self.Options.areaIds.top);
}

CourseSearchLayoutComposer.prototype.setComponentDisplay = function (key) {
    var self = this;
    var item = self.findComponentInRegistryOrSettings(key);
    self.componentCreator(item.defaultPlacement, item);    
}

CourseSearchLayoutComposer.prototype.findComponentInRegistryOrSettings = function (key) {
    var self = this;

    var item = null;

    for (var area in self.Settings.areas) {
        for (var index in self.Settings.areas[area].components) {
            var tempItem = self.Settings.areas[area].components[index];
            if (tempItem.key == key) {
                item = tempItem;
                break;
            }
        }
        if (item != null) {
            break;
        }
    }

    if (item == null) {
        item = CourseSearchComponentRegistry.components[key];
    }
    return item;
}

CourseSearchLayoutComposer.prototype.getCompositeLayoutMenuItems = function () {
    var self = this;

    var isLayoutComposerEditMode = self.isEditMode();

    var componentItems = [];
    for (var key in CourseSearchComponentRegistry.components) {
        var item = CourseSearchComponentRegistry.components[key];

        var menu = {
            xtype: 'menucheckitem',
            text: item.title,
            tooltip: item.disabled ? item.disabledInfo : undefined,
            tooltipType: 'title',
            id: CourseSearchComponentRegistry.methods.getKey(key),
            layoutComponentId: key,
            handler: function (that) {
                self.setComponentDisplay(that.initialConfig.layoutComponentId);
            }
        };
        componentItems.push(menu)
    }

    var menu = {
        text: 'Search components',
        menu: componentItems,
        icon: config.getUrl(self.Options.icons.components)
    };

    return menu;
}

CourseSearchLayoutComposer.prototype.renderAdminModeMenu = function (asMenu) {
    var self = this;

    if (typeof (asMenu) == 'undefined') {
        asMenu = false;
    }

    var compositeLayoutItems = [];

    var compositeMenuItems = [
        {
            xtype: 'menuseparator'
        }
    ];

    var compositeLayoutMenu = {
        disabled: false,
        text: 'Layout actions',
        icon: config.getUrl(self.Options.icons.layoutMenuIcon),
        tooltip: 'You can manage the different layouts here',
        tooltipType: 'title',
        id: CourseSearchComponentRegistry.methods.getMainKey('layout-display'),
        menu: [
            {
                xtype: 'menuitem',
                disabled: false,
                icon: config.getUrl(self.Options.icons.layoutSaveMenuIcon),
                text: 'Save current layout as new',
                tooltip: 'Saves the current layout configuration as a new layout, after clicking this menu item, you may enter the name of the layout',
                tooltipType: 'title',
                id: CourseSearchComponentRegistry.methods.getMainKey('layout-new'),
                handler: function () {
                    Ext.MessageBox.prompt('Save current layout as new', 'Enter a name for this layout (can be an existing one also):', function(buttonId, value) {
                        value = Ext.String.trim(value);
                        if (buttonId == 'ok' && !Ext.isEmpty(value)) {
                            self.setLayoutState(value, self.Settings, function () {
                                self.updateAdminMenuComponentStates();
                                config.showInfo('Layout saved.');
                            });
                        }
                    })
                }
            },
            {
                xtype: 'menuitem',
                disabled: false,
                icon: config.getUrl(self.Options.icons.layoutLoadMenuIcon),
                text: 'Load an existing layout',
                tooltip: 'Loads an existing layout from the database',
                tooltipType: 'title',
                id: CourseSearchComponentRegistry.methods.getMainKey('layout-load')
            },
            {
                xtype: 'menuitem',
                disabled: false,
                icon: config.getUrl(self.Options.icons.layoutSaveAsMenuIcon),
                text: 'Save current as existing layout',
                tooltip: 'Saves the current layout to an existing layout in the database',
                tooltipType: 'title',
                id: CourseSearchComponentRegistry.methods.getMainKey('layout-save')
            },
            {
                xtype: 'menuitem',
                disabled: false,
                icon: config.getUrl(self.Options.icons.layoutDeleteMenuIcon),
                text: 'Delete an existing layout',
                tooltip: 'Deletes an existing layout from the database',
                tooltipType: 'title',
                id: CourseSearchComponentRegistry.methods.getMainKey('layout-delete')
            },
            {
                xtype: 'menuitem',
                disabled: false,
                icon: config.getUrl(self.Options.icons.layoutEmptyMenuIcon),
                text: 'Clear current layout',
                tooltip: 'Clears the current layout in the view and makes the areas empty',
                tooltipType: 'title',
                id: CourseSearchComponentRegistry.methods.getMainKey('layout-clear'),
                handler: function () {
                    self.clearLayout();
                }
            },
            {
                xtype: 'menuitem',
                disabled: false,
                icon: config.getUrl(self.Options.icons.visitLayoutIcon),
                text: 'Visit layout',
                tooltip: "Loads the layout in a new window via its own, specific url",
                tooltipType: 'title',
                id: CourseSearchComponentRegistry.methods.getMainKey('layout-visit')
            }
        ]
    };

//    compositeMenuItems.push(compositeLayoutMenu);

//    compositeMenuItems = compositeMenuItems.concat(self.getCompositeLayoutMenuItems());



    var dndType = asMenu ? 'Ext.menu.CheckItem' : 'Ext.Button.button'
    var compositeLayoutButton = Ext.create(dndType, {
        xtype: dndType,
        text: CourseSearchComponentRegistry.variables.compositeSearchButtonTextOff,
        enableToggle: true,
        pressed: self.isEditMode(),
        checked: self.isEditMode(),
        icon: config.getUrl('Images/Icons/glyph2/Icons16x16/move-2.png'),
        tooltip: CourseSearchComponentRegistry.variables.compositeSearchNonEditModeInfo,
        tooltipType: 'title',
        toggleHandler: function (button, isToggled) {
            window.LAYOUTCOMPOSER.switchDisplayMode();
        },
        checkHandler: function () {
            window.LAYOUTCOMPOSER.switchDisplayMode();
        }
        /*,
        menuAlign: 'tr-br',
        menu: new Ext.menu.Menu({
            items: compositeMenuItems
        })
        */
    });

    compositeLayoutItems.push(compositeLayoutButton);
    compositeLayoutItems.push(self.getCompositeLayoutMenuItems());
    compositeLayoutItems.push(compositeLayoutMenu);

    self.State.compositeLayoutButton = compositeLayoutButton;

    return compositeLayoutItems;
}

CourseSearchLayoutComposer.prototype.updateAdminMenuComponentStates = function () {
    var self = this;

    var layoutMenu = Ext.getCmp(
        CourseSearchComponentRegistry.methods.getMainKey('layout-display')
    );
    
    layoutMenu.setDisabled(true);

    self.listLayouts(function (layouts) {
        self.updateLayoutMenu(layouts, 'layout-load', function (key, value) {
            config.showInfo('Layout loaded.');
            self.clearLayout();
            self.Settings = Ext.decode(value);
            self.renderComponents();
        });
        self.updateLayoutMenu(layouts, 'layout-save', function (key, value) {
            self.setLayoutState(key, self.Settings, function () {
                config.showInfo('Layout saved.');
                self.updateAdminMenuComponentStates();
            });
        });
        self.updateLayoutMenu(layouts, 'layout-visit', function (key, value) {
            window.open(config.getUrl('public/course/layout/' + key), '_blank');;
        });
        self.updateLayoutMenu(layouts, 'layout-delete', function (key, value) {
            Ext.MessageBox.show({
                title: 'Confirm',
                msg: 'Are you sure to delete this layout?',
                buttons: Ext.Msg.OKCANCEL,
                buttonAlign: 'right',
                icon: Ext.MessageBox.QUESTION,
                fn: function (buttonId) {
                    if (buttonId == 'ok') {
                        self.deleteLayout(key, function () {
                            config.showInfo('Layout deleted.');
                            self.updateAdminMenuComponentStates();
                        });
                    }
                }
            });
        });

        layoutMenu.setDisabled(false);
    });
}

CourseSearchLayoutComposer.prototype.updateLayoutMenu = function (layouts, menuId, handler) {
    var self = this;

    var menu = Ext.getCmp(
        CourseSearchComponentRegistry.methods.getMainKey(menuId)
    );

    if (Object.keys(layouts).length == 0) {
        menu.setMenu([{
            xtype: 'menuitem',
            disabled: true,
            text: 'No layouts in the database'
        }], true);
        return;
    }

    var menus = [];
    for (var key in layouts) {
        var menuItem = {
            xtype: 'menuitem',
            text: key,
            handler: function (menu) {
                handler(menu.text, layouts[menu.text])
            }
        };
        menus.push(menuItem);
    };
    menu.setMenu(menus, true);
}

CourseSearchLayoutComposer.prototype.listLayouts = function (callback) {
    var self = this;
    Ext.Ajax.request({
        url: config.getUrl('public/layout/listlayouts'),
        success: function (response) {
            var layouts = Ext.decode(Ext.decode(response.responseText));
            self.State.layouts = layouts;
            if (Ext.isFunction(callback)) {
                callback(layouts);
            }
        }
    });
}

CourseSearchLayoutComposer.prototype.setLayoutState = function (id, settings, callback) {
    var self = this;

    var state = Ext.encode(settings);
    Ext.Ajax.request({
        url: config.getUrl('public/layout/setlayoutstate'),
        params: {
            id: id,
            state: state
        },
        success: function (response) {
            if (Ext.isFunction(callback)) {
                callback();
            }
        }
    });

}

CourseSearchLayoutComposer.prototype.deleteLayout = function (id, callback) {
    var self = this;

    Ext.Ajax.request({
        url: config.getUrl('public/layout/deletelayout'),
        params: {
            id: id
        },
        success: function (response) {
            if (Ext.isFunction(callback)) {
                callback();
            }
        }
    });

}

CourseSearchLayoutComposer.prototype.postMenuRender = function () {
    var self = this;

    self.updateComponentMenuStateBySettings();
    self.updateAdminMenuComponentStates();
    self.setDisplayMode();
}