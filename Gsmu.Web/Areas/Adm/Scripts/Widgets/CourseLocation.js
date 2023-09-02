var courseLocation = {
    ui: {
        locations: $('#course-location-locations'),
        rooms: $('#course-location-rooms'),
        roomDirections: $('#course-location-room-directions'),
        countries: $('#course-location-country-input'),
        editor: $('#course-location-editor'),
        view: $('#course-location-info'),
        editBtn: $('#course-location-btns .edit'),
        saveBtn: $('#course-location-btns .save'),
        cancelBtn: $('#course-location-btns .cancel'),
    },
    init: function () {
        gsmuUIObject.mask('.widget-location-panel');
        var locationData = courseMain.courseLocationData;
        if (!$.isEmptyObject(locationData)) {
            courseLocation.initLocationUI(locationData);
        }
    },
    initDropdowns: function () {
        var getLocations = gsmuConfiguration.globalDropdown.getLocations();
        var getRoomsPromise = gsmuConfiguration.globalDropdown.getRooms();
        var getRoomDirectionsPromise = gsmuConfiguration.globalDropdown.getRoomDirections();
        var getCountries = gsmuConfiguration.globalDropdown.getCountries();

        getLocations.done(function (data) {
            if (data.Success === 1) {
                var locations = data.Data;
                locations.map(function (item) {
                    if (item.Value) {
                        $(courseLocation.ui.locations).append('<option value="' + item.Value + '" >' + item.DisplayText + '</option>');
                        if (item.Extra && JSON.parse(item.Extra))
                        {
                            var location = JSON.parse(item.Extra)
                            courseLocation.localtions.push(location);
                        }
                    }
                })
            }
            //$(courseLocation.ui.rooms).selectpicker();
        });
        if (gsmuConfiguration.masterSettings.RoomNumberOption > 0) {
            getRoomsPromise.done(function (data) {
                if (data.Success === 1) {
                    var rooms = data.Data;
                    rooms.map(function (item) {
                        if (item.Value) $(courseLocation.ui.rooms).append('<option value="' + item.Value + '" >' + item.DisplayText + '</option>');
                    })
                }
            });
        }
        else {
            $(courseLocation.ui.rooms).parent().parent().hide();
        }
        getRoomDirectionsPromise.done(function (data) {
            if (data.Success === 1) {
                var roomDirections = data.Data;
                roomDirections.map(function (item) {
                    if (item.Value) $(courseLocation.ui.roomDirections).append('<option value="' + item.Value + '" >' + item.DisplayText + '</option>');
                })
            }
        });

        getCountries.done(function (data) {
            if (data.Success === 1) {
                var countries = data.Data;
                countries.map(function (item) {
                    if (item.Value) $(courseLocation.ui.countries).append('<option value="' + item.Value + '" >' + item.DisplayText + '</option>');
                })
            }
        });


    },
    initLocationUI: function (locationData) {
        var gmapUrl = 'https://maps.google.com/?q=';
        var completeAddress = locationData.street + ', ' + locationData.city + ', ' + locationData.zip;
        $('#course-location-location').text(locationData.location);
        $('#course-location-street').text(locationData.street);
        $('#course-location-city').text(locationData.city);
        $('#course-location-state').text(locationData.state);
        $('#course-location-zip').text(locationData.zip);
        if (locationData.city != '' && locationData.state != '') {
            $('#map-preview-link').on('click', function () {
                window.open(gmapUrl + completeAddress, '_blank')
            });
        }

        courseLocation.initDropdowns();
        courseLocation.initLocationMap(locationData);
        courseLocationEditor.initView();

        $(courseLocation.ui.locations).change(function (e) {
            var locationId = $(this).val();

            var selectedLocation = courseLocation.localtions.filter(function (item) {
                return item.LocationID == locationId;
            });

            selectedLocation = selectedLocation[0];
            $('input[name="Street"]').val(selectedLocation.Street);
            $('input[name="City"]').val(selectedLocation.City);
            $('input[name="State"]').val(selectedLocation.State);
            $('input[name="Zip"]').val(selectedLocation.Zip);
            $('input[name="Country"]').val(selectedLocation.country);
            $('input[name="Locationurl"]').val(selectedLocation.LocationURL);
            $('input[name="LocationAdditionInfo"]').val(selectedLocation.City);

        })
    },
    initLocationMap: function (locationData) {
        var map;
        var address = new Address();
        var data = {
            street: locationData.street,
            city: locationData.city,
            state: locationData.state,
            zip: locationData.zip
        };
        address.verifyAddress(data, function (valid, results) {
            if (data.city.length < 2 && data.state.length < 2) {
                valid = false
            }
            if (valid) {
                $('#course-location-map').show();
                resultx = results[0].geometry.location;
                result = JSON.stringify(resultx)
                result = result.replace("(", "").replace(")", "")
                //custom styling
                var styledMapType = new google.maps.StyledMapType(
                    [
                        {
                            "elementType": "geometry",
                            "stylers": [
                                {
                                    "color": "#242f3e"
                                }
                            ]
                        },
                        {
                            "elementType": "labels.text.fill",
                            "stylers": [
                                {
                                    "color": "#746855"
                                }
                            ]
                        },
                        {
                            "elementType": "labels.text.stroke",
                            "stylers": [
                                {
                                    "color": "#242f3e"
                                }
                            ]
                        },
                        {
                            "featureType": "administrative.locality",
                            "elementType": "labels.text.fill",
                            "stylers": [
                                {
                                    "color": "#d59563"
                                }
                            ]
                        },
                        {
                            "featureType": "poi",
                            "elementType": "labels.text.fill",
                            "stylers": [
                                {
                                    "color": "#d59563"
                                }
                            ]
                        },
                        {
                            "featureType": "poi.park",
                            "elementType": "geometry",
                            "stylers": [
                                {
                                    "color": "#263c3f"
                                }
                            ]
                        },
                        {
                            "featureType": "poi.park",
                            "elementType": "labels.text.fill",
                            "stylers": [
                                {
                                    "color": "#6b9a76"
                                }
                            ]
                        },
                        {
                            "featureType": "road",
                            "elementType": "geometry",
                            "stylers": [
                                {
                                    "color": "#38414e"
                                }
                            ]
                        },
                        {
                            "featureType": "road",
                            "elementType": "geometry.stroke",
                            "stylers": [
                                {
                                    "color": "#212a37"
                                }
                            ]
                        },
                        {
                            "featureType": "road",
                            "elementType": "labels.text.fill",
                            "stylers": [
                                {
                                    "color": "#9ca5b3"
                                }
                            ]
                        },
                        {
                            "featureType": "road.highway",
                            "elementType": "geometry",
                            "stylers": [
                                {
                                    "color": "#746855"
                                }
                            ]
                        },
                        {
                            "featureType": "road.highway",
                            "elementType": "geometry.stroke",
                            "stylers": [
                                {
                                    "color": "#1f2835"
                                }
                            ]
                        },
                        {
                            "featureType": "road.highway",
                            "elementType": "labels.text.fill",
                            "stylers": [
                                {
                                    "color": "#f3d19c"
                                }
                            ]
                        },
                        {
                            "featureType": "transit",
                            "elementType": "geometry",
                            "stylers": [
                                {
                                    "color": "#2f3948"
                                }
                            ]
                        },
                        {
                            "featureType": "transit.station",
                            "elementType": "labels.text.fill",
                            "stylers": [
                                {
                                    "color": "#d59563"
                                }
                            ]
                        },
                        {
                            "featureType": "water",
                            "elementType": "geometry",
                            "stylers": [
                                {
                                    "color": "#17263c"
                                }
                            ]
                        },
                        {
                            "featureType": "water",
                            "elementType": "labels.text.fill",
                            "stylers": [
                                {
                                    "color": "#515c6d"
                                }
                            ]
                        },
                        {
                            "featureType": "water",
                            "elementType": "labels.text.stroke",
                            "stylers": [
                                {
                                    "color": "#17263c"
                                }
                            ]
                        }
                    ],
                    { name: 'Styled Map' });

                var mapOptions = {
                    zoom: 13,
                    center: new google.maps.LatLng(resultx.lat(), resultx.lng()),
                    mapTypeControlOptions: {
                        mapTypeIds: ['roadmap', 'satellite', 'hybrid', 'terrain',
                            'styled_map']
                    }
                };

                map = new google.maps.Map(document.getElementById('course-location-map'),
                    mapOptions);
                map.mapTypes.set('styled_map', styledMapType);
                map.setMapTypeId('styled_map');

                var marker = new google.maps.Marker({
                    map: map,
                    position: results[0].geometry.location,
                    title: locationData.location
                });
            } else {
                $('#course-location-map').hide();
                result = "Unable to find address: " + status;
            }
            gsmuUIObject.unmask('.widget-location-panel');
        });
    },
    localtions: []
}

var courseLocationEditor = {
    initEditor: function () {
        $(courseLocation.ui.editBtn).hide();
        $(courseLocation.ui.saveBtn).show();
        $(courseLocation.ui.cancelBtn).show();

        $(courseLocation.ui.view).hide();
        $(courseLocation.ui.editor).show();

        courseLocationEditor.initDataBinding();
    },
    initView: function () {
        $(courseLocation.ui.editBtn).show();
        $(courseLocation.ui.saveBtn).hide();
        $(courseLocation.ui.cancelBtn).hide();

        $(courseLocation.ui.view).show();
        $(courseLocation.ui.editor).hide();

        $(courseLocation.ui.editor).hide();

        $('#course-location-street-input').val(courseModel.Street);
        $('#course-location-city-input').val(courseModel.City);
        $('#course-location-state-input').val(courseModel.State);
        $('#course-location-zip-input').val(courseModel.Zip);
        $('#course-location-country-input').val(courseModel.Country);
        $('#course-location-url-input').val(courseModel.Locationurl);
        $('#course-location-info-input').val(courseModel.LocationAdditionInfo);
    },
    initDataBinding: function () {
        //code snippet should be the pattern for all
        $(courseLocation.ui.editor).find('input, select').map(function (e, el) {
            var prop = $(el).attr('name');
            $(el).addClass('focus-editor');
        });
    },
    clearAll: function () {

    },
    save: function () {
        courseEditor.save();
        courseLocationEditor.initView();
    }
}