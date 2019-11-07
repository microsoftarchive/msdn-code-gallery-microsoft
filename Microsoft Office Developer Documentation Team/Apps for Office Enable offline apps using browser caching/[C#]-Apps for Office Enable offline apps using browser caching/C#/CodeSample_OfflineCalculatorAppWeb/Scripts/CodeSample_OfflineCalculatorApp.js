// Global variables for the entered data area, the stored data area,
// the operation to evaluate, and a boolean indicating whether an evaluation
// has just occurred.
var storedData;
var enteredData;
var storedOperation = [];
var isResult;
var setDataDiv;
var getDataDiv;

// This function is run when the app is ready to start interacting with the host application
// It ensures the DOM is ready before adding click handlers to buttons
Office.initialize = function (reason) {
    $(document).ready(function () {

        // Set initial values for global variables.
        storedData = document.getElementById('stored-data');
        enteredData = document.getElementById('entered-data');
        storedOperation[0] = "undefined";
        storedOperation[1] = "undefined";
        isResult = false;

        // Checks if setSelectedDataAsync is supported and adds appropriate click handler
        if (Office.context.document.setSelectedDataAsync) {

            // Define an HTML element builder function.
            var buildNode = function (elementType, options, text) {
                var newElement = document.createElement(elementType);
                for (var i = 0; i < options.length; i++) {
                    newElement.setAttribute(options[i][0], options[i][1]);
                }
                if (text) { newElement.innerHTML = text; }
                return newElement;
            }

            // Create a button to set data in the document.
            setDataDiv = buildNode("input", [["style", "width:47%;margin:2px;"], ["type", "button"], ["value", "Insert Data"], ["onclick", "setData()"]]);
            document.getElementById("content-buttons").appendChild(setDataDiv);

            // Create a button to get data from the document.
            setDataDiv = buildNode("input", [["style", "width:47%;margin:2px;"], ["type", "button"], ["value", "Get Data"], ["onclick", "getData()"]]);
            document.getElementById("content-buttons").appendChild(setDataDiv);

        }

        // Wire up event handlers to buttons in the interface.
        initializeButtons();

    });
};

// Add event handlers to the buttons in the calculator.
function initializeButtons() {

    // Add event handlers to number buttons.
    for (var i = 0; i <= 9; i++) {
        document.getElementById(i.toString()).addEventListener("click",
            function (evt) {
                includeNumber(evt.target);
            });
    }

    // Add event handler to decimal button.
    document.getElementById("decimal").addEventListener("click",
        function (evt) {
            includeNumber(evt.target);
        });

    // Add event handlers to operator buttons.
    document.getElementById("divide").addEventListener("click",
        function (evt) {
            includeOperator(evt.target);
        });
    document.getElementById("multiply").addEventListener("click",
        function (evt) {
            includeOperator(evt.target);
        });
    document.getElementById("minus").addEventListener("click",
        function (evt) {
            includeOperator(evt.target);
        });
    document.getElementById("plus").addEventListener("click",
        function (evt) {
            includeOperator(evt.target);
        });
    document.getElementById("memoryplus").addEventListener("click",
        function (evt) {
            includeOperator(evt.target);
        });
    document.getElementById("memoryminus").addEventListener("click",
        function (evt) {
            includeOperator(evt.target);
        });
}

// Add numbers to the number display.
function includeNumber(node) {
    try {
        if (isResult) {
            enteredData.innerText = "";
            isResult = false;
        }

        var newEntry = node.id;
        var dataEntry = enteredData.innerText;
        dataEntry = String(dataEntry);

        // Check to see if the decimal button was clicked
        // and that there are no other decimals in the entry.
        if (newEntry == "decimal") {
            if (dataEntry.indexOf(".") == -1) {
                newEntry = ".";
            }
            else {
                newEntry = "";
            }
        }

        // Add the new entry to the entered data.
        if (enteredData.innerText == "0") {
            enteredData.innerText = newEntry;
        }
        else {
            enteredData.innerText += newEntry;
        }
    }
    catch (err) {
        Toast.showToast(err.name, err.message);
    }
}

// Convert the information in the display to a number
// and stores both operand and operator.
function includeOperator(node) {
    try {
        if (storedOperation[1] == "undefined") {

            // Parse the value and operator for the operation.
            var operator = node.id;
            var operand = enteredData.innerText;
            operand = parseFloat(operand);
            var memoryOperand = null;
            enteredData.innerText = "0";

            // Define the operation as an anonymous function 
            // and change the display.
            var operation;

            switch (operator) {
                case "divide":
                    operation = function (x, y) { return x / y };
                    storedData.innerText = operand + " / ";
                    break;

                case "multiply":
                    operation = function (x, y) { return x * y };
                    storedData.innerText = operand + " * ";
                    break;

                case "minus":
                    operation = function (x, y) { return x - y };
                    storedData.innerText = operand + " - ";
                    break;

                case "plus":
                    operation = function (x, y) { return x + y };
                    storedData.innerText = operand + " + ";
                    break;

                case "memoryminus":
                    operation = function (x, y) { return x - y };
                    memoryOperand = getMemory();
                    storedData.innerText = memoryOperand + " - ";
                    break;

                case "memoryplus":
                    operation = function (x, y) { return x + y };
                    memoryOperand = getMemory();
                    storedData.innerText = memoryOperand + " + ";
                    break;
            }

            // Store the entered data and the operator into a global variable.
            // Check to see whether this is a memory operation or not.
            if (memoryOperand != null) {
                storedOperation[0] = memoryOperand;
                enteredData.innerText = operand;
            }
            else {
                storedOperation[0] = operand;
            }
            storedOperation[1] = operation;
        }
    }
    catch (err) {
        Toast.showToast(err.name, err.message);
    }
}

// Removes all entries from the output and sets the value to '0'.
function clearEntries() {
    try {
        enteredData.innerText = "0";
        storedData.innerText = "";
    }
    catch (err) {
        Toast.showToast(err.name, err.message);
    }
}

// Gets the data and operation stored in the global array,
// evaluates the expression, and returns the results.
function evaluate() {

    try {
        // Make sure that some data is stored already before attempting to evaluate.
        if (storedOperation[0] != "undefined") {

            // Define the two operands and operation to perform.
            var x = storedOperation[0];
            var y = enteredData.innerText;
            y = parseFloat(y);
            var operation = storedOperation[1];
            var result = operation(x, y);

            // JavaScript has some troubles evaluating floating point
            // numnbers. If the result is has a decimal point,
            // clean up the results by fixing the value to five decimal points.
            if (String(result).indexOf(".") != -1) {
                result = result.toFixed(5);
                result = parseFloat(result);
            }

            // Display results and clear the stored data span.
            enteredData.innerText = result;
            storedData.innerText = "";
            isResult = true;

            // Clear the data out of the storedOperation array.
            storedOperation[0] = "undefined";
            storedOperation[1] = "undefined";
        }
    }
    catch (err) {
        Toast.showToast(err.name, err.message);
    }
}

// Inserts the current entered data/ result into 
// the currently selected place in the document.
function setData() {
    try {
        // Get the current value in the entereddata div.
        var value = enteredData.innerText;

        // Set the selected data into the document as text.
        Office.context.document.setSelectedDataAsync(
            value, function () { });
    }
    catch (err) {
        Toast.showToast(err.name, err.message);
    }
}

// Gets data from the current document and returns it as a number.
function getData() {

    var dataValue = 0;

    // Get the current selection from the word document.
    Office.context.document.getSelectedDataAsync(Office.CoercionType.Text,
        function (result) {
            try {
                if (result.status != "failed") {
                    var results = result.value;
                    results = parseFloat(results);

                    if (!isNaN(results)) {
                        dataValue = results;
                    }
                    else {
                        var NaN_Error;
                        NaN_Error = { name: "Data error", message: "Please select a number." };
                        throw NaN_Error;
                    }
                }
                enteredData.innerText = dataValue;
            }
            catch (err) {
                Toast.showToast(err.name, err.message);
            }
        });

}

// Set a value in the calculator's 'memory' 
// or clear the memory.
function setOrClearMemory() {
    try {
        var memoryIndicator = document.getElementById("memory-indicator");
        // Check to see whether there is a value 
        // in the apps for Office property bag.
        if (Office.context.document.settings.get("memory") != null) {

            // Remove the value from the property bag
            Office.context.document.settings.remove("memory");
            memoryIndicator.setAttribute("class", "hidden");
        }
        else {
            // Get the data that the user has entered and 
            // store it in the apps for Office property bag.
            var memoryData = enteredData.innerText;
            Office.context.document.settings.set("memory", memoryData);
            memoryIndicator.setAttribute("class", "visible");
            isResult = true;
        }
    }
    catch (err) {
        Toast.showToast(err.name, err.message);
    }
}

// Get a value from the calculator's 'memory'.
function getMemory() {

    var memoryData;

    // Check to see whether there is any data in the memory.
    if (Office.context.document.settings.get("memory") != null) {

        // Get the data from the apps for Office property bag.
        memoryData = Office.context.document.settings.get("memory");
        memoryData = parseFloat(memoryData);

    }
    else {

        // No data is stored in memory, return zero.
        memoryData = 0;
    }
    return memoryData;
}

