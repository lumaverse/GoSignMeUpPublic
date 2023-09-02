function CourseSearchDateFilter(options) {
    var self = this;

    Ext.merge(self.Options, options);

    self.CourseSearch = self.Options.courseSearch;
    self.Render();
}

CourseSearchDateFilter.constructor = CourseSearchDateFilter;

CourseSearchDateFilter.ConstructForLayout = function (composer, options) {
    var containerId = composer.getLayoutContainerComponentContainerId(options.container);
    var object = new CourseSearchDateFilter({
        containerId: containerId,
        courseSearch: composer.State.courseSearch
    });
    return object;
};

CourseSearchDateFilter.prototype.Options = {
    containerId: null,
    courseSearch: null
};

CourseSearchDateFilter.prototype.CourseSearch = null;

CourseSearchDateFilter.prototype.ElementContainer = null;

CourseSearchDateFilter.prototype.ClearFilterFrom = null;
CourseSearchDateFilter.prototype.ClearFilterUntil = null;
CourseSearchDateFilter.prototype.FilterFrom = null;
CourseSearchDateFilter.prototype.FilterUntil = null;
CourseSearchDateFilter.prototype.FilterFromContainer = null;
CourseSearchDateFilter.prototype.FilterUntilContainer = null;

CourseSearchDateFilter.prototype.Render = function () {
    var self = this;
    var FromEmptyText;
    FromEmptyText = window.LAYOUT.Options.buttonLabels.SearchFrom != "" ? window.LAYOUT.Options.buttonLabels.SearchFrom : "From";
    var UntilEmptyText;

    UntilEmptyText = window.LAYOUT.Options.buttonLabels.SearchTo != "" ? window.LAYOUT.Options.buttonLabels.SearchTo : "Until";
    self.ElementContainer = Ext.get(self.Options.containerId);

    // render from
    self.FilterFromContainer = Ext.create('Ext.container.Container', {
        renderTo: self.ElementContainer,
        cls: 'course-search-date-filter-container',
        layout: {
            type: 'hbox'
        }
    });
    self.FilterFrom = Ext.create('Ext.form.field.Date', {
        emptyText: FromEmptyText,
        float: true,
        fieldLabel: 'Input date from for course search',
        hideLabel:true,
        title: 'Input date from for course search',
        value: self.CourseSearch.DateFrom,
        invalidText :'not a valid date - it must be in the format {1}'
    });
    self.FilterFrom.on('change', function (that, newVal, oldVal, options) {

        if (newVal != null && newVal != '') {
            self.ClearFilterFrom.show();
        } else {
            self.ClearFilterFrom.hide();
        }

        if (self.FilterFrom.isValid()) {
            self.FilterUntil.setMinValue(newVal);
            self.CourseSearch.DateFrom = Ext.Date.format(newVal, 'n/j/Y');
            self.CourseSearch.Invoke();
        }
    }, self.FilterFrom, {
        buffer: 500
    });

    self.FilterFromContainer.add(self.FilterFrom);

    var clearImageStyle = 'margin-left: 2px; position: relative !important; top: 3px !important; cursor: pointer;'

    self.ClearFilterFrom = self.FilterFromContainer.add({
        xtype: 'box',
        autoEl: {
            tag: 'img',
            alt: 'Clear From Date',
            src: config.getUrl(self.CourseSearch.Defaults.ClearElementImage),
            title: 'Clear from date',
            style: clearImageStyle
        }
    });

    // render until
    self.FilterUntilContainer = Ext.create('Ext.container.Container', {
        renderTo: self.ElementContainer,
        cls: 'course-search-date-filter-container',
        layout: {
            type: 'hbox'
        }
    });
    self.FilterUntil = Ext.create('Ext.form.field.Date', {
        emptyText: UntilEmptyText,
        renderTo: self.ElementContainer,
        fieldLabel: 'Input date until for course search',
        hideLabel: true,
        title: 'Input date until for course search',
        value: self.CourseSearch.DateUntil,
        invalidText: 'not a valid date - it must be in the format {1}'
    });
    self.FilterUntil.on('change', function (that, newVal, oldVal, options) {

        if (newVal != null && newVal != '') {
            self.ClearFilterUntil.show();
        } else {
            self.ClearFilterUntil.hide();
        }

        if (self.FilterUntil.isValid()) {
            self.FilterFrom.setMaxValue(newVal);
            self.CourseSearch.DateUntil = Ext.Date.format(newVal, 'n/j/Y');

            self.CourseSearch.Invoke();
        }
    }, self.FilterUntil, {
        buffer: 500
    });
    self.FilterUntilContainer.add(self.FilterUntil);

    self.ClearFilterUntil = self.FilterUntilContainer.add({
        xtype: 'box',
        autoEl: {
            tag: 'img',
            alt: 'Clear Until Date',
            src: config.getUrl(self.CourseSearch.Defaults.ClearElementImage),
            title: 'Clear until date',
            style: clearImageStyle
        }
    });

    self.FilterFromContainer.setWidth(self.FilterFromContainer.getWidth() + 16);
    self.FilterUntilContainer.setWidth(self.FilterUntilContainer.getWidth() + 16);

    self.ClearFilterFrom.hide();
    self.ClearFilterUntil.hide();

    self.ClearFilterFrom.getEl().on('click', function () {
        self.FilterFrom.setValue('');
    });

    self.ClearFilterUntil.getEl().on('click', function () {
        self.FilterUntil.setValue('');
    });

    if (self.CourseSearch.DateFrom != '' || self.CourseSearch.DateUntil != '') {
        if (self.CourseSearch.DateFrom != '' && self.CourseSearch.DateFrom != null) {
            self.ClearFilterFrom.show();
        }
        if (self.CourseSearch.DateUntil != '' && self.CourseSearch.DateUntil != null) {
            self.ClearFilterUntil.show();
        }
    }
}
