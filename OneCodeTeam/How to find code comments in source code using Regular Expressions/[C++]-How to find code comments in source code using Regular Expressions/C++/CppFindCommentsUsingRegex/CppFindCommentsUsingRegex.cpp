// CppFindCommentsUsingRegex.cpp : Defines the entry point for the console application.
//
/*
****************************** Module Header ******************************\
* Module Name:  CppFindCommentsUsingRegex.cpp
* Project:      CppFindCommentsUsingRegex
* Copyright (c) Microsoft Corporation.
*
* The Main program.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************
*/
#include "stdafx.h"
#include <regex>
#include <iostream>
#include <fstream>
#include <string>
using namespace std;

int _tmain(int argc, _TCHAR* argv[])
{
	//Read file to string
	ifstream in("SourceCode.cpp", ios::in);
	istreambuf_iterator<char> beg(in), end;
	string str(beg, end);
	in.close();

	string pattern("(/\\*([^*]|[\r\n]|(\\*+([^*/]|[\r\n])))*\\*+/)|(//.*)");
	regex r(pattern, regex_constants::egrep);
	for (sregex_iterator it(str.begin(), str.end(), r), end;
		it != end;
		++it)
	{
		cout << it->str() << endl;
	}

	getchar();
	return 0;
}

