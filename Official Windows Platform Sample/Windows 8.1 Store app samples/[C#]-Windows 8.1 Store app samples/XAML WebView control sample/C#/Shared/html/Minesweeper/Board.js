function Board(rows, cols, squareSize, mines) {
    this.rows = rows;
    this.cols = cols;
    this.squareSize = squareSize;
    this.mines = mines;
    this.squaresRevealed = 0; //squares the user has uncovered.  The game ends when a mine is clicked or when ((rows * cols) - mines) == squaresRevealed
    this.minesFlagged = 0; //squares the user has flagged as a mine
}

Board.prototype.initialize = function () {
    this.map = new Array(this.rows);
    for (var i = 0; i < this.rows; i++) {
        this.map[i] = new Array(this.cols);
    }
    for (var i = 0; i < this.rows; i++) {
        for (var j = 0; j < this.cols; j++) {
            this.map[i][j] = new Square(i, j);
        }
    }
    this.squaresRevealed = 0;
}

Board.prototype.copyMinesFrom = function (mineLocations, rows, cols) {
    this.mines = 0;
    this.rows = rows;
    this.cols = cols;
    this.initialize();
    for (var iIndex = 0; iIndex < this.rows; iIndex++) {
        for (var jIndex = 0; jIndex < this.cols; jIndex++) {
            this.map[iIndex][jIndex].isMine = (mineLocations[iIndex][jIndex] === 1);
            if (this.map[iIndex][jIndex].isMine) {
                this.mines++;
            }
        }
    }

}

Board.prototype.generateMines = function () {
    var n = this.mines;
    do {
        var mineLocation = Math.floor(Math.random(5000) * ((this.rows * this.cols) - 1))
        var row = Math.floor(mineLocation / this.cols);
        var col = mineLocation % this.cols;
        if (!this.map[row][col].isMine) {
            this.map[row][col].isMine = true;
            n -= 1;
        }
    } while (n > 0);
}

Board.prototype.generateMinesBenchmark = function () {
    var n = this.mines;
    var mineIncrements = [7, 14, 12, 6, 18, 3, 13, 5, 2, 4, 5, 7, 12, 5, 4, 2, 14, 12, 3, 1, 9, 5, 2, 7, 9, 9];
    var squaresSinceLastMine = 0;
    var nextMine = 1;
    for (var i = 0; i < this.rows; i++) {
        for (var j = 0; j < this.cols; j++) {
            if (squaresSinceLastMine == nextMine) {
                this.map[i][j].isMine = true;
                squaresSinceLastMine = 0;
                nextMine = mineIncrements[(j++ % (i + 1)) + ((i + 1) % (j--))];
            }
            else {
                squaresSinceLastMine++;
            }
        }
    }
}

Board.prototype.regenerate = function (currentSquare) {
    var row = currentSquare.row;
    var col = currentSquare.col;

    while (currentSquare.getSurroundingMineCount() > 0 || currentSquare.isMine) {
        this.initialize();
        this.generateMines();
        currentSquare = this.map[row][col];
    }
}

Board.prototype.squaresRemaining = function () {
    return ((this.rows * this.cols) - this.mines) - this.squaresRevealed;
}
