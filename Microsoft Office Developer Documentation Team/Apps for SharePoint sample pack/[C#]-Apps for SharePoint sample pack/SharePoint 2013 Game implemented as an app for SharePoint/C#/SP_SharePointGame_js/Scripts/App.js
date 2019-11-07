// Variables used in various functions and callbacks
var context;
var web;
var list;
var listItems;
var listItem;
var user;
var existingPlayer = false; 
var playerListId;
var currentWins = 0;
var currentLosses = 0;

// The following code runs when the DOM is ready and creates a context object which is needed 
// to use the SharePoint object model. We have encapsulated that operation in the start()
// function to allow us to call start() when the user clicks the Restart Game button.
$(document).ready(function () {
    start();
});

// Attempt to retrieve the list items in the Score list. This list is deployed as part of the App, 
// and will be empty to start with. It has a default column of Title (as provided by SharePoint for all lists)
// and a column called Wins and a column called Losses. The Title field will hold the current user's name
// so in this function we are using a CAML query to attempt to retrieve only those rows for the current user.
function start() {
    context = SP.ClientContext.get_current();
    web = context.get_web();
    list = web.get_lists().getByTitle('Score');
    user = web.get_currentUser();
    context.load(user);

    // Make an asynchronous call, which will load our list, and more importantly the user object
    // (otherwise we couldn't access the user.get_title() property that we need in the CAML query)
    // The first function you see is the success callback
    context.executeQueryAsync(function () {

        // Set up the query to retrieve only rows where the Title field contains the current user's name
        var camlQuery = new SP.CamlQuery();
        camlQuery.set_viewXml("<View><Where><Eq><FieldRef Name='Title' /><Value Type='Text'>"
            + user.get_title()
            + "</Value></Eq></Where></View>");
        listItems = list.getItems(camlQuery);
        context.load(listItems, 'Include(Title, Wins, Losses)');
        // Now we need to ensure the list data is accessible to our code, so we need to 
        // call executeQueryAsync again.
        // NOTE: We are still in the success callback, so the following two function calls
        // are the success and failure callbacks for the executeQueryAsync we are about to call
        context.executeQueryAsync(
            function () {
                // The showScore function access the data that we have just retrieved
                showScore();
            },

            // This is the failure callback for the second call to executeQueryAsync() 
            function (sender, args) {
                var dataArea = document.getElementById("currentScores");
                // Remove all nodes from the chart <DIV> so we have a clean space to write to
                while (dataArea.hasChildNodes()) {
                    dataArea.removeChild(dataArea.lastChild);
                }
                dataArea.appendChild(document.createTextNode("Failed to get score data. Error: "
                    + args.get_message()));
            });
    },
     // This  function is the failure callback for the first call to executeQueryAsync()
    function () {
        var dataArea = document.getElementById("currentScores");
        // Remove all nodes from the chart <DIV> so we have a clean space to write to
        while (dataArea.hasChildNodes()) {
            dataArea.removeChild(dataArea.lastChild);
        }
        dataArea.appendChild(document.createTextNode("Failed to get user name. Error: "
            + args.get_message()));
    });
}

function showScore() {
    var tableArea = document.getElementById("currentScores");
    // Remove all nodes from the chart <DIV> so we have a clean space to write to
    while (tableArea.hasChildNodes()) {
        tableArea.removeChild(tableArea.lastChild);
    }

    // We actually expect zero or one rows to have been returned in the ListItemCollection
    // The reason for iterating through the list is that it might be possible for a user to have 
    // two or more rows (especially if list data is edited manually by a user),
    // in which case we'll simply take the last row as the correct one.
    var listItemEnumerator = listItems.getEnumerator();
    while (listItemEnumerator.moveNext()) {
        var listItem = listItemEnumerator.get_current();
        var playerName = listItem.get_fieldValues()["Title"];
        var playerWins = listItem.get_fieldValues()["Wins"];
        var playerLosses = listItem.get_fieldValues()["Losses"];
        if ((isNaN(playerWins))||(IsNullOrUndefined(playerWins))) {
            playerWins = 0;
        }
        if ((isNaN(playerLosses)) || (IsNullOrUndefined(playerLosses))) {
            playerLosses = 0;
        }
        // Tell the user what their score is in terms of wins and losses.
        tableArea.appendChild(document.createTextNode("Welcome " + user.get_title() + "! You have won " + playerWins + " times, and lost " + playerLosses + " times!"));
        // Set the class variable to true to indicate that we have a row for the current player.
        existingPlayer = true;
    }
    // If they don't exist in the list yet, tell the user their wins and losses are zero
    if (!existingPlayer) {
        tableArea.appendChild(document.createTextNode("Welcome " + user.get_title() + "! You have won 0 times, and lost 0 times!"));
    }
}

// Each tile in the game tells us what number it is (between 0 and 9)
// when it's clicked. If the tile has a class of 'blank' then it's available.
function square_click(number) {
    var square = document.getElementById("square" + number);
    if (square.getAttribute("class") == "blank") {

        // User is always noughts, and SharePoint is always crosses
        square.setAttribute("class", "nought")

        // A tile has just been changed, so we want to check whether it was a winning move by the user.
        // If not, we'll let SharePoint take a turn.
        // NOTE: See the checkWin function below for how we know if it's a winning move by the user!
        // ALSO: See the SharePointTurn function below for how we play as SharePoint
        if (checkWin('User')) {
            recordScore('Wins'); 
        }
        else {
            SharePointTurn();
        }
    }
}


function SharePointTurn() {
    //The user always plays first, so that means they will also take 
    // the 3rd, 5th, 7th, and last moves (if the game gets that far). So we first need to 
    // check whether the game has ended by calling the hasGameEnded function. 
    // That function checks whether all the tiles have been used, and if we've got that far
    // without a win being registered for either SharePoint or the user, then we know it's a draw.
    if (hasGameEnded()) {
        recordScore('Draw');
    }
    else {
        // Make it appear that SharePoint is thinking for quarter of a second
        // before making a move 
        setTimeout(function () { SharePointPlay(); }, 250);
    }
}


function hasGameEnded() {
    var gameEnded = true;
    //if at least one tile is still blank, SharePoint can make a move, so return false
    for (var squareCounter = 1; squareCounter <= 9; squareCounter++) {
        var square = document.getElementById("square" + squareCounter);
        if (square.getAttribute("class") == "blank") {
            gameEnded = false;
        }
    }
    return (gameEnded);
}

// This function is called after the quarter second delay described above
// It keeps generating random numbers between 1 and 9 until the tile with that number
// in its ID is found to be available
function SharePointPlay() {
    var randomNumber;
    var availableSquare = false;
    while (!availableSquare) {
        randomNumber = Math.floor(Math.random() * 9) + 1;
        var square = document.getElementById("square" + randomNumber);
        if (square.getAttribute("class") == "blank") {
            square.setAttribute("class", "cross")
            availableSquare = true;
        }
    }
    //SharePoint has made a move, so we need to check if it was a winning move!
    // NOTE: See the checkWin function below for how we know if it's a winning move by SharePoint!
    if (checkWin('SharePoint')) {
        // A win by SharePoint is a loss by the user
        recordScore('Losses');
    }
}

function checkWin(player) {
    // Winning Combinations are:
    // 1,2,3 - Top Row
    // 1,4,7 - Left Column
    // 1,5,9 - Diaganol Down from Top Left
    // 2,5,8 - Middle Column
    // 3,6,9 - Right Column
    // 3,5,7 - Diaganol Down from Top Right
    // 4,5,6 - Middle Row
    // 7,8,9 - Bottom Row
    // So we will simply check if those combinations of tiles all have the same class
    // (which indicates whether they were clicked by the user or taken by SharePoint)
    var retVal = false;
    var square1 = document.getElementById("square1").getAttribute("class");
    var square2 = document.getElementById("square2").getAttribute("class");
    var square3 = document.getElementById("square3").getAttribute("class");
    var square4 = document.getElementById("square4").getAttribute("class");
    var square5 = document.getElementById("square5").getAttribute("class");
    var square6 = document.getElementById("square6").getAttribute("class");
    var square7 = document.getElementById("square7").getAttribute("class");
    var square8 = document.getElementById("square8").getAttribute("class");
    var square9 = document.getElementById("square9").getAttribute("class");
    if (player == "User") {
        // Check the combinations for noughts
        if ((square1 == "nought") && (square2 == "nought") && (square3 == "nought")) {
            retVal = true;
            return (retVal);
        }
        if ((square1 == "nought") && (square4 == "nought") && (square7 == "nought")) {
            retVal = true;
            return (retVal);
        }
        if ((square1 == "nought") && (square5 == "nought") && (square9 == "nought")) {
            retVal = true;
            return (retVal);
        }
        if ((square2 == "nought") && (square5 == "nought") && (square8 == "nought")) {
            retVal = true;
            return (retVal);
        }
        if ((square3 == "nought") && (square6 == "nought") && (square9 == "nought")) {
            retVal = true;
            return (retVal);
        }
        if ((square3 == "nought") && (square5 == "nought") && (square7 == "nought")) {
            retVal = true;
            return (retVal);
        }
        if ((square4 == "nought") && (square5 == "nought") && (square6 == "nought")) {
            retVal = true;
            return (retVal);
        }
        if ((square7 == "nought") && (square8 == "nought") && (square9 == "nought")) {
            retVal = true;
            return (retVal);
        }
    }
    else {
        // Check the combinations for crosses (i.e. SharePoint's tiles)
        if ((square1 == "cross") && (square2 == "cross") && (square3 == "cross")) {
            retVal = true;
            return (retVal);
        }
        if ((square1 == "cross") && (square4 == "cross") && (square7 == "cross")) {
            retVal = true;
            return (retVal);
        }
        if ((square1 == "cross") && (square5 == "cross") && (square9 == "cross")) {
            retVal = true;
            return (retVal);
        }
        if ((square2 == "cross") && (square5 == "cross") && (square8 == "cross")) {
            retVal = true;
            return (retVal);
        }
        if ((square3 == "cross") && (square6 == "cross") && (square9 == "cross")) {
            retVal = true;
            return (retVal);
        }
        if ((square3 == "cross") && (square5 == "cross") && (square7 == "cross")) {
            retVal = true;
            return (retVal);
        }
        if ((square4 == "cross") && (square5 == "cross") && (square6 == "cross")) {
            retVal = true;
            return (retVal);
        }
        if ((square7 == "cross") && (square8 == "cross") && (square9 == "cross")) {
            retVal = true;
            return (retVal);
        }
    }
    return (retVal);
}

// If either SharePoint or the user has won (or indeed if there has been a draw)
// tell the user and store wins/losses in the Score list
function recordScore(state) {
    // Game must have ended for the code to get here,
    // so we'll disable each tile, just in case there are remaining available tiles
    for (var squareCounter = 1; squareCounter <= 9; squareCounter++) {
        var square = document.getElementById("square" + squareCounter);
        square.disabled = true;
    }
    // The existingPlayer variable will have been set to true if we've previously
    // retrieved a row from the Score list. If that's the case, then we know that we
    // also have stored the row id for that user in the playerListId variable
    if(existingPlayer)
    {
        // If the game was won, call incrementWins (which updates the Score table appropriately)
        // and tell the user
        if (state == "Wins") {
            alert("Hurrah! You are smarter than SharePoint!");
            listItem = list.getItemById(playerListId);
            context.load(listItem);
            context.executeQueryAsync(function () { incrementWins(); }, function(){ alert("Current score could not be retrieved");});
        }

        // If the game was tied, just tell the user
        if (state == "Draw") {
            alert("Not Bad: You and SharePoint are evenly matched...");
        }

        // If the game was lost, call incrementLosses (which updates the Score table appropriately)
        // and tell the user
        if (state == "Losses") {
            alert("Erm, I don't know how to break this to you, but you've been beaten by a random number generator...");
            listItem = list.getItemById(playerListId);
            context.load(listItem);
            context.executeQueryAsync(function () { incrementLosses(); }, function () { alert("Current score could not be retrieved"); });
        }
    }
    else
    {
        // The user does not have a row in the Score list, so we'll create one for them and set the Wins/Losses data
        if (state == "Wins") {
            alert("Hurrah! You are smarter than SharePoint!");
            var itemCreateInfo = new SP.ListItemCreationInformation();
            listItem = list.addItem(itemCreateInfo);
            listItem.set_item("Title", user.get_title());
            listItem.set_item("Wins", 1);
            listItem.set_item("Losses", 0);
            listItem.update();
            context.load(listItem);
            context.executeQueryAsync(function() { playerListId = listItem.get_id();}, 
                function() { alert('Score could not be stored');});
        }
        if (state == "Draw") {
            alert("Not Bad: You and SharePoint are evenly matched...");
        }
        if (state == "Losses") {
            alert("Erm, I don't know how to break this to you, but you've been beaten by a random number generator...");
            var itemCreateInfo = new SP.ListItemCreationInformation();
            listItem = list.addItem(itemCreateInfo);
            listItem.set_item("Title", user.get_title());
            listItem.set_item("Wins", 0);
            listItem.set_item("Losses", 1);
            listItem.update();
            context.load(listItem);
            context.executeQueryAsync(function() { playerListId = listItem.get_id();}, 
                function() { alert('Score could not be stored');});

        }
    }
}


function incrementWins() {
    currentWins = listItem.get_fieldValues()["Wins"];
    listItem.set_item("Wins", parseInt(currentWins) + 1);
    listItem.update();
    context.executeQueryAsync(null, function () { alert("Score could not be updated"); });

}
function incrementLosses() {
    currentLosses = listItem.get_fieldValues()["Losses"];
    listItem.set_item("Losses", parseInt(currentLosses) + 1);
    listItem.update();
    context.executeQueryAsync(null, function () { alert("Score could not be updated"); });

}

// This function is called when the user clicks the Restart Game button.
// We simply reset the tiles, and call the start function, which 
// ensures the scores shown to the user are up to date. The user can then try their
// skills against SharePoint again.
function resetNow() {
    for (var squareCounter = 1; squareCounter <= 9; squareCounter++) {
        var square = document.getElementById("square" + squareCounter);
        square.setAttribute("class", "blank");
        square.disabled = false;
    }
    start();
}