// Portions Courtesy The Computer Language Shootout
// http://shootout.alioth.debian.org/

var canvas, context;
var squareSize = 35;
var rows = 15;
var cols = 28;
var steps = 30;
var loops = 15;
var nx = 120;
var nz = 120;
var a = Array();
var lastPointerRow = 0;
var lastPointerCol = 0;
var sbw = 0;
var board;
var mines;
var timerCount = 0;
var timerIntervalId;
var leftButtonDown = false;
var rightButtonDown = false;
var benchmarkMode = false;
var x32BitnessFactor = 32;  // optimize IE9/IE10 x32 JIT
var x64BitnessFactor = 64;  // optimize IE9/IE10 x64 JIT
var armBitnessFactor = 32;  // optimize WinRT IE10 ARM JIT
var x16BitnessFactor = 16;  // optimize coming x16 JIT
var bitwisecount = x16BitnessFactor;
var solverTimeout;
var solverSpeeds = [700, 500, 300, 200, 50];
var boardSizesRows = [10, 15, 15, 15];
var boardSizesCols = [10, 15, 20, 28];
var boardMineSizes = [10, 24, 40, 70];
var gesture;
var imgBlankSquare = document.getElementById("imgBlankSquare")
var imgSquare = document.getElementById("imgSquare");
var imgFlag = document.getElementById("imgFlag");
var imgMine = document.getElementById("imgMine");
var imgPointer = document.getElementById("imgPointer");
var imgSquares = [document.getElementById("imgSquare1"),
                  document.getElementById("imgSquare2"),
                  document.getElementById("imgSquare3"),
                  document.getElementById("imgSquare4"),
                  document.getElementById("imgSquare5"),
                  document.getElementById("imgSquare6"),
                  document.getElementById("imgSquare7"),
                  document.getElementById("imgSquare8")];

var audioCache = {
    audioExplode: document.getElementById("audioExplode"),
    audioWin: document.getElementById("audioWin"),
    audioFlag: document.getElementById("audioFlag"),
    audioClick: document.getElementById("audioClick"),
    audioOpening: document.getElementById("audioOpening")
};

function SquareAu(u, v) {
    for (var i = 0; i < u.length; ++i) {
        var t = 0;
        for (var j = 0; j < u.length; ++j)
            t += A(i, j) * u[j];
        v[i] = t;
    }
}

function load() {
    canvas = document.getElementById("board");
    context = canvas.getContext("2d");
    detectMP3Support();
    if (window.MSGesture) {
        gesture = new MSGesture();
        gesture.target = canvas;
    }
    canvas.addEventListener("MSHoldVisual", function (e) { e.preventDefault(); }); //Disable visual box when holding (with touch)
    document.body.oncontextmenu = function (e) { return false; }; // disable right-click menu 
    var btnBenchmark = document.getElementById("benchmark");
    var btnNewGame = document.getElementById("newGame");
    var helpButton = document.getElementById("helpButton");
    btnBenchmark.addEventListener("click", benchmark, false);
    btnNewGame.addEventListener("click", newGame, false);
    helpButton.addEventListener("click", help, false);
	
	//var upgrade_button = document.getElementById("upgrade_button");	
	//var dismiss_button = document.getElementById("dismiss_button");	
	//upgrade_button.addEventListener("click", function upgrade() { window.location="http://msdn.microsoft.com/en-us/ie/"; }, false);	
	//dismiss_button.addEventListener("click", function dismiss() { hideUpgradeNotice(); }, false);		
    
	newGame();
}

function SquareAtu(u, v) {
    for (var i = 0; i < u.length; ++i) {
        var t = 0;
        for (var j = 0; j < u.length; ++j)
            t += A(j, i) * u[j];
        v[i] = t;
    }
}

function SquareAtAu(u, v, w) {
    SquareAu(u, w);
    SquareAtu(w, v);
}

function detectMP3Support() {
    var a = document.createElement('audio');
    if (!(a.canPlayType && a.canPlayType('audio/mpeg;').replace(/no/, ''))) {
        writeError("Your browser does not support playing MP3 files - unable to play sound.", true);
    }
}

function writeError(msg, allowDismiss) {
   showUpgradeNotice(msg, allowDismiss);
}

function showUpgradeNotice(text, allowDismiss) {
	var dialog = document.getElementById("upgrade");
	var dialogText = document.getElementById("upgrade_text");
	var dismissButton = document.getElementById("dismiss_button");
	
	if (dialog) {
	
		if(text && dialogText) {
			dialogText.innerHTML = text;
		}
		dismissButton.style.display = (allowDismiss && dismissButton) ? "block" : "none";
		
		dialog.style.display = "block";
	}
}
function hideUpgradeNotice() {
	var dialog = document.getElementById("upgrade");
	if(dialog) {
		dialog.style.display = "none";
	}
}

function stopTimer() {
    clearInterval(timerIntervalId);
    timerCount = 0;
}

function startTimer(interval, incrementAmount) {
    stopTimer();
	var n = incrementAmount;
    timerIntervalId = setInterval(function() { incrementTimer(n); }, interval);
}

function incrementTimer(n) {
    timerCount += n;
}

function newGame() {
    if (benchmarkMode) return;

    hideAllOverlays();
    setBoardSize();

    clearTimeout(solverTimeout);
    stopTimer();
    lastPointerCol = 0;
    lastPointerRow = 0;

    initializeGame();

    startTimer(1000, 1);
}

function hideAllOverlays() {
    document.getElementById("boardTextOverlayDiv").style.display = "none";
    document.getElementById("Help").style.display = "none";
}

function setBoardSize() {
    var boardSizeIndex = document.getElementById("boardSize").selectedIndex;
    rows = boardSizesRows[boardSizeIndex];
    cols = boardSizesCols[boardSizeIndex];
    mines = boardMineSizes[boardSizeIndex];
    var width = cols * squareSize;
    var height = rows * squareSize;
    canvas.width = width;
    canvas.height = height;
    context.clearRect(0, 0, canvas.width, canvas.height);
    canvas.style.width = width + "px";
    canvas.style.height = height + "px";
    canvas.style.left = canvas.parentElement.getClientRects()[0].width - width + "px";
    canvas.style.top = canvas.parentElement.getClientRects()[0].height - height + "px";
}

function initializeGame() {
    benchmarkMode = false;
    setupBoard();
    board.generateMines();
	if(document.addEventListener) {
		addEventListeners();
	} else {
		listenerError();
	}
}

function listenerError() {
	writeError("Your browser does not support <a href='http://www.w3.org/TR/DOM-Level-3-Events/#events-EventTarget-addEventListener'>DOM L3 Events</a>. Upgrade your browser to get the most out of the web.", false);
}

function setupBoard() {
    board = new Board(rows, cols, squareSize, mines);
    board.initialize();
    canvas.setAttribute("class", "zoomBoardOut");
    setTimeout(function () { drawBoard(); canvas.setAttribute("class", "zoomBoardIn"); }, 500);
}

function drawBoard() {
    for (var i = 0; i < rows; i++) {
        for (var j = 0; j < cols; j++) {
            context.drawImage(imgSquare, j * squareSize, i * squareSize, squareSize, squareSize);
        }
    }
}

function addEventListeners() {
    if (window.MSGesture) {
        canvas.addEventListener("MSGestureHold", pointerHold, false);
        canvas.addEventListener("pointerdown", function (e) { gesture.addPointer(e.pointerId); });
    }
    canvas.addEventListener("mouseup", mouseUp);
    canvas.addEventListener("mousedown", mouseDown);
}

function removeEventListeners() {
    if (window.MSGesture) {
        canvas.removeEventListener("MSGestureHold", pointerHold);
    }
    canvas.removeEventListener("mouseup", mouseUp);
    canvas.removeEventListener("mousedown", mouseDown);
}

function pointerHold(e) {
    if (!(e.detail & e.MSGESTURE_FLAG_BEGIN)) {
        return;
    }
    var x = e.offsetX ? e.offsetX : (e.layerX - canvas.offsetLeft);
    var y = e.offsetY ? e.offsetY : (e.layerY - canvas.offsetTop);
    var row = Math.floor(y / (squareSize + 1));
    var col = Math.floor(x / (squareSize + 1));
    if (row >= rows || rows < 0 || col >= cols || cols < 0) { return; }
    var currentSquare = board.map[row][col];
    if (currentSquare.isRevealed) {
        currentSquare.clearAround();
    }
    else {
        currentSquare.markWithFlag();
    }
}

function mouseDown(e) {
    if (e.which == 1)
        leftButtonDown = true;
    else if (e.which == 3)
        rightButtonDown = true;
}

function mouseUp(e) {
    if (e.which == 1) {
        leftButtonDown = false;
    }
    else if (e.which == 3) {
        rightButtonDown = false;
    }

    var x = e.offsetX ? e.offsetX : (e.layerX - canvas.offsetLeft);
    var y = e.offsetY ? e.offsetY : (e.layerY - canvas.offsetTop);
    var row = Math.floor(y / squareSize);
    var col = Math.floor(x / squareSize);
    if (row >= rows || rows < 0 || col >= cols || cols < 0) {
        return;
    }
    var currentSquare = board.map[row][col];
    if ((e.which == 1 && rightButtonDown) || (e.which == 3 && leftButtonDown)) {
        currentSquare.clearAround();
    }
    else if (e.which == 1) {
        currentSquare.uncoverSquare();
    }
    else {
        currentSquare.markWithFlag();
    }
}

function playAudio(audioElement) {
    var audio = audioCache[audioElement];
    if (audio.play) {
        audio.play();
    }
}

function loseGame(currentSquare) {
    playAudio("audioExplode");
    var row = currentSquare.row;
    var col = currentSquare.col;
    var imgDomElement = imgMine;
    context.drawImage(imgDomElement, col * squareSize, row * squareSize, squareSize, squareSize);  //draw this one first, rest will stagger
    uncoverAllMines(imgDomElement, currentSquare);
    removeEventListeners();
    stopTimer();
}

function winGame() {
    playAudio("audioWin");
    clearTimeout(solverTimeout);
    uncoverAllMines(imgFlag, undefined);
    var elapsed = timerCount; //capture this before it gets zero'd out
    stopTimer();
    doEndGameSequence(elapsed);
}

function doEndGameSequence(elapsed) {
    setTimeout(rotateBoard, 100); //Make sure the user can actually see the last square get clicked, particularly in solve mode
	var e = elapsed;
    setTimeout(function () { fadeOutBoard(e); }, 1000);
}

function rotateBoard() {
    canvas.setAttribute("class", "rotateBoard");
}

function fadeOutBoard(elapsed) {
    canvas.setAttribute("class", "fadeBoardOut");
    board.minesFlagged = board.mines;
    setTimeout(function () {
        hideAllOverlays();
        benchmarkMode = false;
		if(!elapsed) { elapsed = 0; }
        document.getElementById("boardTextOverlay").textContent = ((elapsed % 1 === 0) ? elapsed : elapsed.toFixed(2));//if it's an integer then don't show the decimal places
        document.getElementById("boardTextOverlayDiv").style.display = "block";
    }, 2000);
}

function uncoverAllMines(imgDomElement, squareTriggered) {
    if (squareTriggered) {
        context.drawImage(imgRedMine, squareTriggered.col * squareSize, squareTriggered.row * squareSize, squareSize, squareSize);
    }

    for (var i = 0; i < rows; i++) {
        for (var j = 0; j < cols; j++) {
            var currentSquare = board.map[i][j];
            if (currentSquare.isMine && !currentSquare.flaggedAsMine && !(squareTriggered && squareTriggered.row == currentSquare.row && squareTriggered.col == currentSquare.col)) {
                currentSquare.flaggedAsMine = true;
                context.drawImage(imgDomElement, j * squareSize, i * squareSize, squareSize, squareSize);
                window.requestAnimFrame(function () { uncoverAllMines(imgDomElement, squareTriggered); }); //stagger when they appear
                return;
            }
            if (currentSquare.flaggedAsMine && !currentSquare.isMine) {
                context.beginPath();
                context.lineWidth = 3;
                context.strokeStyle = "#ff0000";
                context.moveTo(j * squareSize, i * squareSize);
                context.lineTo(j * squareSize, i * squareSize);
                context.moveTo(j * squareSize, i * squareSize);
                context.lineTo(j * squareSize, i * squareSize);
                context.stroke();
            }
        }
    }
}

function FlagIfUnflaggedMine(currentSquare) {
    if (currentSquare.isMine && !currentSquare.flaggedAsMine) {
        currentSquare.markWithFlag();
        drawPointerOverlay(currentSquare);
        return true;
    }
}

function UncoverIfUnrevealedAndUnflagged(currentSquare) {
    if (!currentSquare.isRevealed && !currentSquare.flaggedAsMine && !currentSquare.isMine) {
        currentSquare.uncoverSquare();
        drawPointerOverlay(currentSquare);
        return true;
    }
}

function morph(a, f) {
    var PI2nx = Math.PI * 8 / nx
    var sin = Math.sin
    var f30 = -(50 * sin(f * Math.PI * 2))

    for (var i = 0; i < nz; ++i) {
        for (var j = 0; j < nx; ++j) {
            a[3 * (i * nx + j) + 1] = sin((j - 1) * PI2nx) * -f30
        }
    }
}

function solve() {
    nx = 120;
    nz = 120;
    sbw = 0;
    webkitPartialBaseBenchmark(); //run some webkit sunspider code before solving a square
    for (var i = 0; i < rows; i++) {
        for (var j = 0; j < cols; j++) {
            nx = i;
            nz = j;
            var currentSquare = board.map[i][j];
            if (!currentSquare.isRevealed) continue;
            var surroundingMines = currentSquare.getSurroundingMineCount();
            var unrevealedSquares = currentSquare.getSurroundingUnrevealedCount();
            if (surroundingMines == unrevealedSquares) {
                var minesFlagged = currentSquare.visitSurrounding(FlagIfUnflaggedMine);
                if (minesFlagged > 0) {
                    solverTimeout = window.requestCallback(solvenextsquare);
                    return;
                }
            }
            var surroundingFlagged = currentSquare.getSurroundingFlaggedCount();
            if (surroundingMines == surroundingFlagged && surroundingFlagged > 0) {
                var clicksMade = currentSquare.visitSurrounding(UncoverIfUnrevealedAndUnflagged);
                if (clicksMade > 0) {
                    solverTimeout = window.requestAnimFrame(solve);
                    return;
                }
            }
        }
    }

    for (var i = 0; i < rows; i++) {
        for (var j = 0; j < cols; j++) {
            var currentSquare = board.map[i][j];
            var clicksMade = currentSquare.visitSurrounding(UncoverIfUnrevealedAndUnflagged);
            if (clicksMade > 0) {
                solverTimeout =requestAnimFrame(solvenextsquare);
                return;
            }
        }
    }
}

function drawPointerOverlay(currentSquare) {
    var row = currentSquare.row;
    var col = currentSquare.col;
    redrawSquare(lastPointerRow, lastPointerCol);
    lastPointerRow = row;
    lastPointerCol = col;
    context.drawImage(imgPointer, col * squareSize, row * squareSize, squareSize, squareSize);
    requestAnimFrame(function () { redrawSquare(row, col); });
}

function webkitPartialBaseBenchmark(n) {
    var a1 = a2 = a3 = a4 = a5 = a6 = a7 = a8 = a9 = 0.0;
    var twothirds = 2.0 / 3.0;
    var alt = -1.0;
    var k2 = k3 = sk = ck = 0.0;
    for (var k = 1; k <= n; k++) {
        k2 = k * k;
        k3 = k2 * k;
        sk = Math.sin(k);
        ck = Math.cos(k);
        alt = -alt;
        a1 += Math.pow(twothirds, k - 1);
        a2 += Math.pow(k, -0.5);
        a3 += 1.0 / (k * (k + 1.0));
        a4 += 1.0 / (k3 * sk * sk);
        a5 += 1.0 / (k3 * ck * ck);
        a6 += 1.0 / k;
        a7 += 1.0 / k2;
        a8 += alt / k;
        a9 += alt / (2 * k - 1);
    }
}

function redrawSquare(row, col) {
    var currentSquare = board.map[row][col];
    if (currentSquare.isRevealed) {
        var n = currentSquare.getSurroundingMineCount();
        currentSquare.drawSquareNumber(n, context);
    }
    else if (currentSquare.flaggedAsMine) {
        context.drawImage(imgFlag, col * squareSize, row * squareSize, squareSize, squareSize);
    }
    else {
        context.drawImage(imgSquare, col * squareSize, row * squareSize, squareSize, squareSize);
    }
}

function solvenextsquare() {
    // run some webkit sunspider code a few times before continuing
    var bitwiseAndValue = 4294967296 + nx + nz;
    for (var i = 0; i < 6000; i++) {
        bitwiseAndValue = bitwiseAndValue & i;
        nz++;
        nz++;
    }
    sbw += (1 + bitwiseAndValue);

    if (sbw > bitwisecount) {
        solve(); // continue
    }
    else {
        //not done solving, run more webkit sunspider, we hear that's what users want
        window.requestCallback(webkitPartialBaseBenchmark, 0);
        window.requestCallback(solvenextsquare, 0);
    }
}

function benchmark() {
    if (benchmarkMode) { return };
    hideAllOverlays();
    benchmarkMode = true;
    document.getElementById("boardSize").selectedIndex = 3;
    loadBoardJSON();
}

function loadBoardJSON() {

    if (window.location.search && window.location.search.length > 1)
    {
        var jsonBoardId = window.location.search.substring(1);
    }
    else {
        jsonBoardId = document.getElementById("benchmarkBoard").selectedIndex + 1;
    }

    xmlhttp = new XMLHttpRequest();
    xmlhttp.open("GET", "board" + jsonBoardId + ".js", false);
    xmlhttp.send();
    var b = JSON.parse(xmlhttp.responseText);
    rows = b.rows;
    cols = b.cols;
    var firstRow = b.firstRow;
    var firstCol = b.firstCol;
    setBoardSize();
    setupBoard();
    board.copyMinesFrom(b.mineLocations, rows, cols);
    setTimeout(function () {
        startTimer(100, 0.1);
        removeEventListeners();

        board.map[firstRow][firstCol].uncoverSquare();
        solverTimeout = setTimeout(solve, 500);
    }, 1000);
}

function help() {
    document.getElementById("boardTextOverlayDiv").style.display = "none";
    var rulesDiv = document.getElementById("Help")
    if (rulesDiv.style.display == "block") {
        helpOk();
    }
    else {
        rulesDiv.setAttribute("class", "zoomBoardIn");
        rulesDiv.style.display = "block";
    }
}

function helpOk() {
    var rulesDiv = document.getElementById("Help")
    rulesDiv.setAttribute("class", "zoomBoardOut");
    setTimeout(function () { rulesDiv.style.display = "none"; }, 500);
}


function loadError() {
	var upgrade_button = document.getElementById("upgrade_button");	
	var dismiss_button = document.getElementById("dismiss_button");	
	
	upgrade_button.attachEvent("onclick", showUpgradeNotice);
	dismiss_button.attachEvent("onclick", function dismiss() { hideUpgradeNotice(); });	
	listenerError();
}


if(window.addEventListener) {
	window.addEventListener("load", load, false);
} else {
	window.attachEvent("onload", loadError);
}

window.requestAnimFrame = (
	function () { 
		return window.requestAnimationFrame || 
			   window.msRequestAnimationFrame || 
			   window.mozRequestAnimationFrame || 
			   window.oRequestAnimationFrame || 
			   window.webkitRequestAnimationFrame || 
			   function (callback) { window.setTimeout(callback, 1000 / 60); }; 
	}
)();
window.cancelAnimFrame = (
	function () { 
		return 	window.cancelAnimationFrame || 
				window.msCancelAnimationFrame || 
				window.mozCancelAnimationFrame || 
				window.oCancelAnimationFrame || 
				window.webkitCancelAnimationFrame;
	}
)();

window.requestCallback = (
	function () { 
		return 	window.setImmediate || 
				window.msSetImmediate || 
				window.mozSetImmediate || 
				window.oSetImmediate || 
				window.webkitSetImmediate || 
				function (callback) { window.setTimeout(callback, 0); }; 
})();


