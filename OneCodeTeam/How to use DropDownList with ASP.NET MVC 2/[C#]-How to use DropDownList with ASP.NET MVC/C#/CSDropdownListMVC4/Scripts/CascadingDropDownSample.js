//jQuery Ready function
//used to bind events for HTML controls
$(function () {
    var cascadingDropDownSample = new CascadingDropDownSample();

    //binding change event of the "make" select HTML control
    $('#make').on('change', function () {
        var selectedMake = $(this).val();

        //if selected other than default option, make a AJAX call to server
        if (selectedMake !== "-1") {
            $.post('/CascadingDropDownSample/GetSampleModels',
                { selectedMake: selectedMake },
                function (data) {
                    cascadingDropDownSample.resetCascadingDropDowns();
                    cascadingDropDownSample.getSampleModelsSuccess(data);
                });
        }
        else {
            //reset the cascading dropdown
            cascadingDropDownSample.resetCascadingDropDowns();
        }
    });

    //binding change event of the "model" select HTML control
    $('#model').on('change', function () {
        var selectedModel = $(this).val();

        //if selected other than default option, make a AJAX call to server
        if (selectedModel !== "-1") {
            $.post('/CascadingDropDownSample/GetSampleColors',
                { selectedModel: selectedModel },
                function (data) {
                    cascadingDropDownSample.resetColors();
                    cascadingDropDownSample.getSampleColorsSuccess(data);
                });
        }
        else {
            //reset the colors dropdown
            cascadingDropDownSample.resetColors();
        }
    });
});

// Module for CascadingDropDownSample containing JS helper functions
function CascadingDropDownSample() {
    this.resetCascadingDropDowns = function () {
        this.resetModels();
        this.resetColors();
    };

    this.getSampleModelsSuccess = function (data) {
        //binding JSON data received to HTML select control
        $.each(data, function (key, textValue) {
            $('#model').append($('<option />', { value: key, text: textValue }));
        });
        $('#model').attr('disabled', false);
    };

    this.getSampleColorsSuccess = function (data) {
        //binding JSON data received to HTML select control
        $.each(data, function (key, textValue) {
            $('#color').append($('<option />', { value: key, text: textValue }));
        });
        $('#color').attr('disabled', false);
    };

    this.resetModels = function () {
        $('#model option').remove();
        $('#model').append($('<option />', { value: '-1', text: 'Please select a model' }));
        $('#model').attr('disabled', 'disabled');
    };

    this.resetColors = function () {
        $('#color option').remove();
        $('#color').append($('<option />', { value: '-1', text: 'Please select a color' }));
        $('#color').attr('disabled', 'disabled');
    };
}