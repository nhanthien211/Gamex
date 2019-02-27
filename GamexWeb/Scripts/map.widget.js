var autoComplete, placeSearch;

function initAutocomplete() {
    autocomplete = new google.maps.places.Autocomplete(
        document.getElementById('Address'), { types: ['geocode'] });

    // Avoid paying for data that you don't need by restricting the set of
    // place fields that are returned to just the address components.
    autocomplete.setFields('formatted_address');

    // When the user selects an address from the drop-down, populate the
    // address fields in the form.
//    autocomplete.addListener('place_changed', fillInAddress);
}