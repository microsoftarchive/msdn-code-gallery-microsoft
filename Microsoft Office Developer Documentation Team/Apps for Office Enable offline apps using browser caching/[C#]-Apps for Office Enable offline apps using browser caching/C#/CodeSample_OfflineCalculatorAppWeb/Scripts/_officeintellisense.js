
var Office = new function() {
    this._appContext = 23;
}

// 1 Excel appContext = 001
// 2 Word appContext = 010
// 3 Word + Excel appContext = 011
// 4 Project appContext = 100
// 5 Project + Excel appContext = 101
// 6 Project + Word appContext = 110
// 7 Project + Word + Excel appContext = 111
// 8 Outlook appContext = 1000
// 16 PowerPoint appContext = 10000 
// 17 PowerPoint + Excel appContext = 10001 
// 18 PowerPoint + Word appContext = 10010 
// 19 PowerPoint + Word + Excel appContext = 10011 
// 20 PowerPoint + Project appContext = 10100 
// 21 PowerPoint + Project + Excel appContext = 10101 
// 22 PowerPoint + Project + Word appContext = 10110 
// 23 PowerPoint + Project + Word + Excel appContext = 10111

