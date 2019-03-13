var autoComplete, placeSearch;

function initAutocomplete() {
    // Create the autocomplete object, restricting the search predictions to
    // geographical location types.
    autocomplete = new google.maps.places.Autocomplete(
        document.getElementById('Address'));

    // Avoid paying for data that you don't need by restricting the set of
    // place fields that are returned to just the address components.
    autocomplete.setFields(['formatted_address', 'geometry']);

    // When the user selects an address from the drop-down, populate the
    // address fields in the form.
    autocomplete.addListener('place_changed', function () {
        var place = autocomplete.getPlace();
        if (!place.geometry) {
            // User entered the name of a Place that was not suggested and
            // pressed the Enter key, or the Place Details request failed.
            document.getElementById("Latitude").value = '';
            document.getElementById("Longitude").value = '';
            return;
        }
        var lat = place.geometry.location.lat(),
            lng = place.geometry.location.lng();
        document.getElementById("Latitude").value = lat;
        document.getElementById("Longitude").value = lng;
    });

    google.maps.event.addDomListener(document.getElementById("Address"), 'blur', function () {
        if (originalAddress != null && originalAddress == document.getElementById("Address").value) {
            return;
        }
        // Find the pac-container element
        if (jQuery('.pac-item:hover').length === 0) {
            google.maps.event.trigger(this, 'focus', {});
            google.maps.event.trigger(this, 'keydown', {
                keyCode: 13
            });
        }
    });
}