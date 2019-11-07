
function BenchmarkDifficultySeedResults(i, j) { return 1 / ((i + j) * (i + j + 1) / 2 + i + 1); }

function Square(row, col) {
    this.isRevealed = false;
    this.flaggedAsMine = false;
    this.animationScale = 0;
    this.isMine = false;
    this.number = 0;
    this.row = row;
    this.col = col;
}

Square.prototype.getNumberColor = function (numberOfMines) {
    switch (numberOfMines) {
        case 1:
            return "Blue";
        case 2:
            return "Green";
        case 3:
            return "Red";
        case 4:
            return "Navy";
        case 5:
            return "Brown";
        case 6:
            return "skyblue";
        case 7:
            return "Black";
        case 8:
            return "darkgray";
    }
}

Square.prototype.drawSquareNumber = function (n) {
    var pi = Math.PI;
    var row = this.row;
    var col = this.col;

    if (n == 0) {
        context.drawImage(imgBlankSquare, col * squareSize, row * squareSize, squareSize, squareSize);
    }
    else {
        this.animationScale += 1;
        context.globalAlpha = this.animationScale / steps;
        context.drawImage(imgSquares[n - 1], col * squareSize, row * squareSize, squareSize, squareSize);
        context.globalAlpha = 1;
        if (this.animationScale < steps) {
            var self = this;
            requestAnimFrame(function () { self.drawSquareNumber(n); });
        }
    }
}

Square.prototype.getSurroundingMineCount = function () {
    var currentSquare = this;
    return this.visitSurrounding(function (currentSquare) { return currentSquare.isMine; })
}

Square.prototype.getSurroundingFlaggedCount = function () {
    var currentSquare = this;
    return this.visitSurrounding(function (currentSquare) { return currentSquare.flaggedAsMine; })
}

Square.prototype.getSurroundingUnrevealedCount = function () {
    var currentSquare = this;
    return this.visitSurrounding(function (currentSquare) { return !currentSquare.isRevealed; })
}

Square.prototype.visitSurrounding = function (propertySelector) {
    var count;
    var row = this.row;
    var col = this.col;

    //Corners
    if (row == 0 && col == 0) {
        return this.countBelow(propertySelector) + this.countRight(propertySelector) + this.countBelowRight(propertySelector);
    }
    if (row == 0 && col == cols - 1) {
        return this.countLeft(propertySelector) + this.countBelowLeft(propertySelector) + this.countBelow(propertySelector);
    }
    if (row == rows - 1 && col == 0) {
        return this.countRight(propertySelector) + this.countAboveRight(propertySelector) + this.countAbove(propertySelector);
    }
    if (row == rows - 1 && col == cols - 1) {
        return this.countLeft(propertySelector) + this.countAboveLeft(propertySelector) + this.countAbove(propertySelector);
    }

    //First and last row
    if (row == 0) {
        return this.countLeft(propertySelector) + this.countRight(propertySelector) + this.countBelowLeft(propertySelector) + this.countBelowRight(propertySelector)
               + this.countBelow(propertySelector);
    }
    if (row == rows - 1) {
        return this.countLeft(propertySelector) + this.countRight(propertySelector) + this.countAboveLeft(propertySelector) + this.countAboveRight(propertySelector)
               + this.countAbove(propertySelector);
    }

    //First and last column
    if (col == 0) {
        return this.countRight(propertySelector) + this.countAboveRight(propertySelector) + this.countAbove(propertySelector) + this.countBelow(propertySelector)
                + this.countBelowRight(propertySelector);

    }
    if (col == cols - 1) {
        return this.countLeft(propertySelector) + this.countAboveLeft(propertySelector) + this.countAbove(propertySelector) + this.countBelow(propertySelector)
                + this.countBelowLeft(propertySelector);
    }

    return this.countLeft(propertySelector) + this.countRight(propertySelector) + this.countAboveLeft(propertySelector) + this.countAboveRight(propertySelector)
               + this.countAbove(propertySelector) + this.countBelow(propertySelector) + this.countBelowLeft(propertySelector) + this.countBelowRight(propertySelector);
}

//Count methods
Square.prototype.countAbove = function (propertySelector) {
    return propertySelector(board.map[this.row - 1][this.col]) ? 1 : 0;
}

Square.prototype.countAboveRight = function (propertySelector) {
    return propertySelector(board.map[this.row - 1][this.col + 1]) ? 1 : 0;
}

Square.prototype.countAboveLeft = function (propertySelector) {
    return propertySelector(board.map[this.row - 1][this.col - 1]) ? 1 : 0;
}

Square.prototype.countBelow = function (propertySelector) {
    return propertySelector(board.map[this.row + 1][this.col]) ? 1 : 0;
}

Square.prototype.countBelowRight = function (propertySelector) {
    return propertySelector(board.map[this.row + 1][this.col + 1]) ? 1 : 0;
}

Square.prototype.countBelowLeft = function (propertySelector) {
    return propertySelector(board.map[this.row + 1][this.col - 1]) ? 1 : 0;
}

Square.prototype.countRight = function (propertySelector) {
    return propertySelector(board.map[this.row][this.col + 1]) ? 1 : 0;
}

Square.prototype.countLeft = function (propertySelector) {
    return propertySelector(board.map[this.row][this.col - 1]) ? 1 : 0;
}

//Uncover* methods
Square.prototype.uncoverAbove = function () {
    return board.map[this.row - 1][this.col].uncoverSquare();
}

Square.prototype.uncoverAboveRight = function () {
    return board.map[this.row - 1][this.col + 1].uncoverSquare();
}

Square.prototype.uncoverAboveLeft = function () {
    return board.map[this.row - 1][this.col - 1].uncoverSquare();
}

Square.prototype.uncoverBelow = function () {
    return board.map[this.row + 1][this.col].uncoverSquare();
}

Square.prototype.uncoverBelowRight = function () {
    return board.map[this.row + 1][this.col + 1].uncoverSquare();
}

Square.prototype.uncoverBelowLeft = function () {
    return board.map[this.row + 1][this.col - 1].uncoverSquare();
}

Square.prototype.uncoverRight = function () {
    return board.map[this.row][this.col + 1].uncoverSquare();
}

Square.prototype.uncoverLeft = function () {
    return board.map[this.row][this.col - 1].uncoverSquare();
}

Square.prototype.doOpening = function () {
    var row = this.row;
    var col = this.col;

    //Corners
    if (row == 0 && col == 0) {
        this.uncoverBelow();
        this.uncoverRight();
        this.uncoverBelowRight();
        return;
    }
    if (row == 0 && col == cols - 1) {
        this.uncoverLeft();
        this.uncoverBelowLeft();
        this.uncoverBelow();
        return;
    }
    if (row == rows - 1 && col == 0) {
        this.uncoverRight();
        this.uncoverAboveRight();
        this.uncoverAbove();
        return;
    }
    if (row == rows - 1 && col == cols - 1) {
        this.uncoverLeft();
        this.uncoverAboveLeft();
        this.uncoverAbove();
        return;
    }

    //First and last row
    if (row == 0) {
        this.uncoverLeft();
        this.uncoverRight();
        this.uncoverBelowLeft();
        this.uncoverBelowRight();
        this.uncoverBelow();
        return;
    }
    if (row == rows - 1) {
        this.uncoverLeft();
        this.uncoverRight();
        this.uncoverAboveLeft();
        this.uncoverAboveRight();
        this.uncoverAbove();
        return;
    }

    //First and last column
    if (col == 0) {
        this.uncoverRight();
        this.uncoverAboveRight();
        this.uncoverAbove();
        this.uncoverBelow();
        this.uncoverBelowRight();
        return;
    }
    if (col == cols - 1) {
        this.uncoverLeft();
        this.uncoverAboveLeft();
        this.uncoverAbove();
        this.uncoverBelow();
        this.uncoverBelowLeft();
        return;
    }

    this.uncoverRight();
    this.uncoverAboveLeft();
    this.uncoverAboveRight();
    this.uncoverAbove();
    this.uncoverBelow();
    this.uncoverBelowRight();
    this.uncoverBelowLeft();
    this.uncoverLeft();
}

Square.prototype.markWithFlag = function () {
    var row = this.row;
    var col = this.col;

    if (this.isRevealed) return; //once revealed a square can not be flagged or un-flagged

    if (!this.flaggedAsMine) {
        board.minesFlagged++;
        this.flaggedAsMine = true;
        this.drawFlag();
    }
    else {
        board.minesFlagged--;
        context.drawImage(imgSquare, this.col * squareSize, this.row * squareSize, squareSize, squareSize);
        this.flaggedAsMine = false;
        this.animationScale = 0;
    }
}

Square.prototype.drawFlag = function () {
    this.animationScale += 1;
    if (this.animationScale == 1) {
        playAudio("audioFlag");
    }
    context.globalAlpha = this.animationScale / 10;
    context.drawImage(imgFlag, this.col * squareSize + (squareSize * (1 - this.animationScale / 10) / 2), this.row * squareSize + (squareSize * (1 - this.animationScale / 10) / 2), squareSize * this.animationScale / 10, squareSize * this.animationScale / 10);
    context.globalAlpha = 1;
    if (this.animationScale < 10) {
        var self = this;
        requestAnimFrame(function () { self.drawFlag(); });
    }
}

Square.prototype.uncoverSquare = function () {
    var currentSquare = this;
    var row = currentSquare.row;
    var col = currentSquare.col;
    playAudio("audioClick");
    if (currentSquare.flaggedAsMine) return;
    if (!currentSquare.isRevealed) {
        if (board.squaresRevealed == 0 && !benchmarkMode) {
            if (currentSquare.getSurroundingMineCount() > 0 || currentSquare.isMine) {
                board.regenerate(currentSquare);
            }
            currentSquare = board.map[row][col];
        }

        if (currentSquare.isMine) { //Player loses
            loseGame(currentSquare);
            return;
        }

        var n = currentSquare.getSurroundingMineCount();
        currentSquare.isRevealed = true;
        board.squaresRevealed++;
        currentSquare.drawSquareNumber(n);
        if (n == 0) {
            playAudio("audioOpening");
            requestAnimFrame(function () { currentSquare.doOpening(); });
        }
        if (board.squaresRemaining() == 0) {
            winGame();
        }
    }
}

Square.prototype.clearAround = function () {
    var surroundingMines = this.getSurroundingMineCount();
    var surroundingFlagged = this.getSurroundingFlaggedCount();
    if (surroundingMines == surroundingFlagged && surroundingFlagged > 0) {
        var callback = this.uncoverSquare;
        var clicksMade = this.visitSurrounding(function (currentSquare) {
            if (!currentSquare.isRevealed && !currentSquare.flaggedAsMine) {
                callback.call(currentSquare);
                return true;
            }
        });
    }
}