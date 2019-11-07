//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// This script reads a compiled HLSL shader header from stdin and
// outputs a header file that includes an instruction count constant.

var stdin = WScript.StdIn;
var stdout = WScript.StdOut;
var shaderID = WScript.Arguments(0)

var fFound = false;
var uInstructionCount = 0;

// Iterate over each line of the input.
while (!stdin.AtEndOfStream) {
    var str = stdin.ReadLine();

    // Use a regular expression to extract the number of used instruction slots.
    var reg = /(\d*) instruction slots used/;
    var match = str.match(reg);

    if (match) {
        // The integral value can be found in the first submatch,
        // corresponding to the bracketed part of the regular expression.
        uInstructionCount = match[1];
        fFound = true;
        break;
    }
}

// Write out the instruction count as a code-referenceable constant integer.
stdout.WriteLine("const UINT32 " + shaderID + "_InstructionCount = " + uInstructionCount + ";");

if (!fFound) {
    WScript.StdErr.WriteLine("ERROR: Instruction count not found");
    exit(1);
}
