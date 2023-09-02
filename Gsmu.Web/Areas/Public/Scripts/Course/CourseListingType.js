function CourseListingType(courseSearch, containerId, renderTitles) {
    var self = this;

    if (Ext.isDefined(renderTitles)) {
        self.renderTitles = renderTitles;
    }

    self.CourseSearch = courseSearch;
    self.ContainerId = containerId;

    self.Container = Ext.get(containerId);

    self.DevelopmentMode = window.LAYOUT.Options.developmentMode;

    self.Render();

    window.COURSELISTINGTYPE = self;
}

CourseListingType.constructor = CourseListingType;

CourseListingType.ConstructForLayout = function (composer, options) {
    var containerId = composer.getLayoutContainerComponentContainerId(options.container);
    var object = new CourseListingType(composer.State.courseSearch, containerId);
    return object;
};


// reference to the course search component
CourseListingType.prototype.CourseSearch = null;

// the container
CourseListingType.prototype.ContainerId = null;

// the container
CourseListingType.prototype.Container = null;

// development mode
CourseListingType.prototype.DevelopmentMode = false;

// tile july view image
CourseListingType.prototype.TileJulyViewImage = 'Images/Icons/glyph2/Icons24x24/square.png';

// grid view image
CourseListingType.prototype.GridViewImage = 'Images/Icons/glyph2/Icons24x24/column-1.png';

// grid view element
CourseListingType.prototype.GridElement = null;

// tile view element
CourseListingType.prototype.TileJulyElement = null;

CourseListingType.prototype.renderTitles = true;

CourseListingType.prototype.Render = function () {
    var self = this;

    self.Container.setHtml('');

    var enabledViewCount = 0;
    if (window.LAYOUT.Options.layoutConfiguration.SearchColumns['GridViewEnabled']) {
        enabledViewCount++;
    }
    if (window.LAYOUT.Options.layoutConfiguration.SearchColumns['TileJulyViewEnabled']) {
        enabledViewCount++;
    }
    if (enabledViewCount < 2) {
        self.Container.enableDisplayMode();
        self.Container.hide();
        return;
    }
    self.Container.show();

    /*
    <div id="view-type-grid" class="view-type-item active" title="View the course list as a grid">
        <img src="@Url.Content("~/Images/Icons/Famfamfam/application_view_columns.png")" />
        Grid
    </div>
    <div id="view-type-tile" class="view-type-item" title="View the course list as tile">
        <img src="@Url.Content("~/Images/Icons/Famfamfam/application_view_tile.png")" />             
        Tile
    </div>
    */
    var dh = Ext.DomHelper;

    // GRID
    // GRID
    // GRID
    if (window.LAYOUT.Options.layoutConfiguration.SearchColumns['GridViewEnabled']) {
        var gridChildren = [
                {
                    //tag: 'img',
                    //alt:'Grid View',
                    //src: config.getUrl(self.GridViewImage)
                    tag: 'i',
                    cls: 'fa fa-list SubBarTextDefault BGclogsubbartext',
                    style: 'font-size:25px'

                }
        ];

        if (self.renderTitles) {
            gridChildren.push(
                {
                    tag: 'span',
                    html: 'Grid'
                }
            );
        }

        var grid = {
            tag: 'a',
            id: 'view-type-grid',
            //cls: 'view-type-item' + (self.CourseSearch.ViewListType == 'Grid' ? '' : ''),
            title: 'View the course list as a grid',
            style: 'margin: 10px; cursor: pointer;',
            children: gridChildren
        };

        self.GridElement = dh.append(self.Container, grid, true);


        self.GridElement.on('click', function () {
            //self.TileJulyElement.removeCls('active');
            //self.GridElement.addCls('active');
            self.CourseSearch.ViewListType = 'Grid';
            self.CourseSearch.Invoke();
        });

    }

    // TILE JULY
    // TILE JULY 
    // TILE JULY
    if (window.LAYOUT.Options.layoutConfiguration.SearchColumns['TileJulyViewEnabled']) {
        var tileJulyChildren = [
                {
                    //tag: 'img',
                    //alt: 'Tile View',
                    //src: config.getUrl(self.TileJulyViewImage)
                    tag: 'i',
                    cls: 'fa fa-th-large SubBarTextDefault BGclogsubbartext',
                    style: 'font-size:25px'
            }
        ];

        if (self.renderTitles) {
            tileJulyChildren.push({
                tag: 'span',
                html: 'Tile'
            });
        }

        var tileJuly = {
            tag: 'a',
            id: 'view-type-tile-july',
            //cls: 'view-type-item' + (self.CourseSearch.ViewListType == 'TileJuly' ? '' : ''),
            style: 'margin: 10px; cursor: pointer',
            title: 'View the course list as tile',
            children: tileJulyChildren
        }

        self.TileJulyElement = dh.append(self.Container, tileJuly, true);

        self.TileJulyElement.on('click', function () {
            //self.GridElement.removeCls('active');
            //self.TileJulyElement.addCls('active');
            self.CourseSearch.ViewListType = 'TileJuly';
            self.CourseSearch.Invoke();
        });
    }
}