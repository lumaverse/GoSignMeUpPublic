var courseMaterials = {
    ui: {
        materialsDropdown: $('#course-materials-dropdown')
    },
    init: function () {
        gsmuUIObject.mask('#widget-course-options');
        var courseDateTimesData = courseMaterials.loadData();
        courseDateTimesData.done(function (response) {
            if (response.Success === 1) {
                courseMaterials.initUI(response.Data);
                gsmuUIObject.unmask('#widget-course-options');
            }
        });
    },
    loadData: function () {
        var courseId = UrlHelper.getUrlVars()["courseId"];;
        var getCourseMaterials = function () {
            return $.ajax({
                type: "GET",
                dataType: 'json',
                url: gsmuObject.apiUrl + '/AdminCourseDash/GetCourseMaterialsById?courseId=' + courseId
            });
        }
        return getCourseMaterials();
    },
    loadDropdownData: function () {
        return $.ajax({
            type: "GET",
            dataType: 'json',
            url: gsmuObject.apiUrl + '/MasterSettings/GetMaterials'
        });
    },
    initUI: function (materialsData) {
        $('#widget-course-options a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            var index = $(e.target).closest('li').index() + 1;
            if (index !== 2) return false;
            $('#course-material-grid').DataTable({
                responsive: true,
                autoWitdh: false,
                data: materialsData,
                destroy: true,
                columns: [
                    {
                        title: "Product Name", data: 'ProductName'
                    },
                    {
                        title: "Price", data: 'Price'
                    },
                    {
                        title: "Shipping Cost", data: 'ShippingCost'
                    },
                    {
                        title: "Shipping Weight", data: 'ShippingWeight'
                    },
                    {
                        title: "Quantity", data: 'Quantity'
                    }
                ]
            });
        });
        
        gsmuUIObject.mask('#widget-course-options');
        courseMaterials.loadDropdownData()
            .done(function (data) {
                var materials = data.Data;
                materials.map(function (item) {
                    var itemTemplate = '<option value=' + item.Value + '>' + item.DisplayText + ' ( $ ' + parseFloat(item.Extra).toFixed(2) + ' ) ' + '</option>';
                    $(courseMaterials.ui.materialsDropdown).append(itemTemplate);
                });
                $(courseMaterials.ui.materialsDropdown).selectpicker();
                $(courseMaterials.ui.materialsDropdown).on('changed.bs.select', function (e, clickedIndex, newValue, oldValue) {
                    var selected = $(e.currentTarget).val();
                    courseMaterials.selectedMaterial = selected;
                });

                if (courseModel.MaterialsRequired == -1 || courseModel.MaterialsRequired == 1)
                    $('#course-require-material').attr('checked', 'checked');

                $('#course-require-material').on('change', function () {
                    var materialRequired = $(this).prop('checked');
                    courseModel.MaterialsRequired = materialRequired ? 1 : 0;
                    courseEditor.save();
                });

                gsmuUIObject.unmask('#widget-course-options');
            });
    },
    addMaterial: function () {
        gsmuUIObject.mask('#widget-course-options');
        var saveMaterial = $.ajax({
            type: "POST",
            dataType: 'json',
            url: gsmuObject.apiUrl + '/AdminCourseDash/SaveMaterials?courseId=' + courseModel.CourseId + '&materialId=' + courseMaterials.selectedMaterial
        });
        saveMaterial.done(function (xhr, response) {
            if (response === 'success') {
                courseMaterials.refreshMaterialsGrid();
                gsmuUIObject.unmask('#widget-course-options');
            }
        });
    },
    refreshMaterialsGrid: function () {
        courseMaterials.init();
    },
    selectedMaterial: 0
}
$(document).ready(function () {
    courseMaterials.init();
})