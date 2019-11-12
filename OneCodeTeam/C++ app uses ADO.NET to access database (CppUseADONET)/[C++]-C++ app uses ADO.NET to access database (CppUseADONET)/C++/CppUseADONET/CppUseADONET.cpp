/****************************** Module Header ******************************\
* Module Name:  CppUseADONET.cpp
* Project:      CppUseADONET
* Copyright (c) Microsoft Corporation.
* 
* The CppUseADONET example demonstrates the Microsoft ADO.NET technology to 
* access databases using Visual C++ in both managed code and unmanaged code. 
* It shows the basic structure of connecting to a data source, issuing SQL 
* commands, using DataSet object and performing the cleanup. 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\****************************************************************************/

#include "stdafx.h"

#include <comdef.h>
#include <gcroot.h>
#include <iostream>
using namespace std;

#using <System.dll>
#using <System.Data.dll>
using namespace System;
using namespace System::Data;
using namespace System::Data::SqlClient;
using namespace System::Runtime::InteropServices;

// Max row number of the DataSet
#define MAXROWS 100

// Managed codes to connect database via ADO.NET
#pragma managed
class DatabaseClass
{	
public:
    DatabaseClass()
	{
		// Create the database connection string
		//conStr = "Data Source=localhost;Initial Catalog=SQLServer2005DB;" 
		//	+ "User Id=HelloWorld;Password=111111;";

			conStr = "Data Source=.\\sqlexpress;Initial Catalog=SQLServer2005DB;" 
			+ "Integrated Security=SSPI;";
	}

	/*!
    * \brief
	* Create and open database connection.
	*/
	void CreateConnection()
	{
		con = gcnew SqlConnection(conStr);
		con->Open();
	}

	/*!
	* \brief
	* Add one new data row to data source using SQL command.
	* 
	* \param lastName
	* Last Name value for data table LastName column.
	* 
	* \param firstName
	* First Name value for data table FirstName column.
	* 
	* \param image
	* SAFEARRAY holding the array of bytes which represents an image
	*/
	void AddRow(wchar_t *lastName, wchar_t *firstName, SAFEARRAY *image)
	{
		// 1. Inialize the SqlCommand object.
		cmd = gcnew SqlCommand();

		// 2. Assign the connection to the SqlCommand.
		cmd->Connection = con;

		// 3. Set the SQL command text.
		// SQL statement or the name of the stored procedure.
		cmd->CommandText = "INSERT INTO Person(LastName, FirstName, " + 
			"HireDate, EnrollmentDate, Picture) VALUES (@LastName, " + 
			"@FirstName, @HireDate, @EnrollmentDate, @Picture)";

		// 4. Set the command type.
		// CommandType::Text for ordinary SQL statements;
		// CommandType::StoredProcedure for stored procedures.
		cmd->CommandType = CommandType::Text;

		// 5. Append the parameters.
		// DBNull::Value for SQL-Nullable fields.
		cmd->Parameters->Add("@LastName", SqlDbType::NVarChar, 50)->Value = 
			Marshal::PtrToStringUni((IntPtr)lastName);
		cmd->Parameters->Add("@FirstName", SqlDbType::NVarChar, 50)->Value = 
			Marshal::PtrToStringUni((IntPtr)firstName);
		cmd->Parameters->Add("@HireDate", SqlDbType::DateTime)->Value = 
			DBNull::Value;
		cmd->Parameters->Add("@EnrollmentDate", SqlDbType::DateTime)->Value
			= DateTime::Now;
		if (image == NULL)
		{
			cmd->Parameters->Add("@Picture", SqlDbType::Image)->Value = 
				DBNull::Value;
		}
		else
		{
			// Convert the SAFEARRAY to an array of bytes.
			int len = image->rgsabound[0].cElements;
			array<byte> ^arr = gcnew array<byte>(len);
			int *pData;
			SafeArrayAccessData(image, (void **)&pData);
			Marshal::Copy(IntPtr(pData), arr, 0, len);
			SafeArrayUnaccessData(image);
			cmd->Parameters->Add("@Picture", SqlDbType::Image)->Value = arr;
		}

		// 6. Execute the command.
		cmd->ExecuteNonQuery();
	}

	/*!
	* \brief
	* Inialize the DataSet object and the SqlDataAdapter object, 
	* then fill DataSet object with the query data table.
	* 
	* \param command
	* SQL command to query the data.
	*/
	void FillDataSet(wchar_t *command)
	{
		// 1. Inialize the DataSet object.
		ds = gcnew DataSet();

		// 2. Create a SELECT SQL command.
		String ^ strSelectCmd = Marshal::PtrToStringUni((IntPtr)command);
		
		// 3. Inialize the SqlDataAdapter object.
		// SqlDataAdapter represents a set of data commands and a 
        // database connection that are used to fill the DataSet and 
        // update a SQL Server database. 
		da = gcnew SqlDataAdapter(strSelectCmd, con);

		// 4. Fill the DataSet object.
		// Fill the DataTable in DataSet with the rows selected by the SQL 
		// command.
		da->Fill(ds);
	}

	/*!
	* \brief
	* Retrieve data under one DataTable column into an array of VARIANT
	* 
	* \param dataColumn
	* The DataTable column name.
	*
	* \param values
	* The array of VARIANT holding the query results.
	*
	* \param valuesLength
	* The max row number of the query results.
	*
	* \returns
	* The row number of the query results.
	*/
	int GetValueForColumn(wchar_t *dataColumn, VARIANT *values, 
		int valuesLength)
	{
		// Convert the column name to managed String object.
		String ^columnStr = Marshal::PtrToStringUni((IntPtr)dataColumn);

		// Create an array of DataRow which holds all the DataTable rows.
		array<DataRow ^> ^rows = ds->Tables[0]->Select();

		// Get the row number.
		int len = rows->Length;
		len = (len > valuesLength) ? valuesLength : len;

		// Copy each row of data under the column to the array of VARIANT.
		for (int i = 0; i < len; i++)
		{
			Marshal::GetNativeVariantForObject(rows[i][columnStr], 
				IntPtr(&values[i]));
		}

		return len;
	}

	/*!
	* \brief
	* Close the database connection.
	*/
	void CloseConnection()
	{
		con->Close();
	}

private:
    // Using gcroot, you can use a managed type in
    // a native class.
	gcroot<DataSet ^> ds;
	gcroot<String ^> conStr;
	gcroot<SqlConnection ^> con;
	gcroot<SqlCommand ^> cmd;
	gcroot<SqlDataAdapter ^> da;
};

// Unmanaged codes
#pragma unmanaged
DWORD ReadImage(PCTSTR pszImage, SAFEARRAY FAR **psaChunk);

int _tmain(int argc, _TCHAR* argv[])
{
	DatabaseClass *db = NULL;

	try
	{
		/////////////////////////////////////////////////////////////////////
		// Connect to the data source.
		// 

		_tprintf(L"Connecting to the database...\n");

		// Initialize the managed DatabaseClass object.
		db = new DatabaseClass();

		// Create and open the database connection.
		// (The data source is created in the sample SQLServer2005DB)
		db->CreateConnection();


		/////////////////////////////////////////////////////////////////////
		// Build and Execute and ADO.NET Command.
		// It can be a SQL statement (SELECT/UPDATE/INSERT/DELETE), or a
		// stored procedure call.  Here is the sample of an INSERT command.
		//

		_tprintf(L"Inserting a record to the Person table.\n");

		// Create a SAFEARRAY to hold the array of bytes.
        SAFEARRAY FAR *psaChunk = NULL;

		// Read the image file into an array of bytes.
		int cbChunkBytes = ReadImage(_T("MSDN.jpg"), &psaChunk);

		// Set the SAFEARRAY to NULL if the image loading fails.  
		if (cbChunkBytes == 0)
		{
			psaChunk = NULL;
		}

		// Add one row of new data to the data source.
		db->AddRow(L"Sun", L"Lingzhi", psaChunk);


		/////////////////////////////////////////////////////////////////////
		// Use the DataSet Object.
        // The DataSet, which is an in-memory cache of data retrieved from a 
		// data source, is a major component of the ADO.NET architecture. The 
		// DataSet consists of a collection of DataTable objects that you can 
		// relate to each other with DataRelation objects.
        // 

		_tprintf(L"Using DataSet to store the data query result.\n");

		// Create a SELECT SQL command.
		wchar_t *strSelectedCmd = L"SELECT * FROM Person";

		// Fill the DataTable in the DataSet with the rows selected by the
		// SQL command.
		db->FillDataSet(strSelectedCmd);

		// Create an array of VARIANT to hold the PersonID column values.
		VARIANT valuesID[MAXROWS];

		// Fill the array of VARIANT with the PersonID column values.
		int len = db->GetValueForColumn(L"PersonID", valuesID, MAXROWS);

		// Create an array of VARIANT to hold the LastName column values.
		VARIANT valuesLastName[MAXROWS];

		// Fill the array of VARIANT with the PersonID column values. 
		db->GetValueForColumn(L"LastName", valuesLastName, MAXROWS);

		// Create an array of VARIANT to hold the FirstName column values.
		VARIANT valuesFirstName[MAXROWS];

		// Fill the array of VARIANT with the FirstName column values. 
		db->GetValueForColumn(L"FirstName", valuesFirstName, MAXROWS);
		
		// Display the values in the three VARIANT arrays.
		for (int i = 0; i < len; i++)
		{
			// When dumping a SQL-Nullable field, you need to test it for
			// VT_NULL.
			_tprintf(L"%d\t%s %s\n", valuesID[i].intVal, 
				(valuesFirstName[i].vt == VT_NULL) ? L"(DBNULL)" : valuesFirstName[i].bstrVal,
				(valuesLastName[i].vt == VT_NULL) ? L"(DBNULL)" : valuesLastName[i].bstrVal);
		}
	}
	catch (_com_error &err)
	{
		_tprintf(_T("The application throws the error: %s\n"), 
			err.ErrorMessage());
		_tprintf(_T("Description = %s\n"), (LPCTSTR) err.Description());
	}


	/////////////////////////////////////////////////////////////////////////
	// Clean up objects before exit.
	//

	// Close the database connection.
	db->CloseConnection();

	// Delete the managed DatabaseClass object.
    delete db;

	return 0;
}

/*!
 * \brief
 * Read an image file to a safe array of bytes.
 * 
 * \param pszImage
 * The path of the image file.
 * 
 * \param ppsaChunk
 * The output of the safe array.
 * 
 * \returns
 * The number of bytes read from the image file. 0 means failure.
 */
DWORD ReadImage(PCTSTR pszImage, SAFEARRAY FAR **ppsaChunk)
{
	// Open the image file
	HANDLE hImage = CreateFile(pszImage, GENERIC_READ, FILE_SHARE_READ, NULL,
		OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);

	if (hImage == INVALID_HANDLE_VALUE) 
	{
		_tprintf(_T("Could not open the image %s\n"), pszImage);
		return 0;
	}

	// Get the size of the file in bytes
	LARGE_INTEGER liSize;
	if (!GetFileSizeEx(hImage, &liSize))
	{
		_tprintf(_T("Could not get the image size w/err 0x%08lx\n"), 
			GetLastError());
		CloseHandle(hImage);
		return 0;
	}
	if (liSize.HighPart != 0)
	{
		_tprintf(_T("The image file is too big\n"));
		CloseHandle(hImage);
		return 0;
	}
	DWORD dwSize = liSize.LowPart, dwBytesRead;

	// Create a safe array with cbChunkBytes elements
	*ppsaChunk = SafeArrayCreateVector(VT_UI1, 1, dwSize);

	// Initialize the content of the safe array
	BYTE *pbChunk;
	SafeArrayAccessData(*ppsaChunk, (void **)&pbChunk);

	// Read the image file
	if (!ReadFile(hImage, pbChunk, dwSize, &dwBytesRead, NULL) 
		|| dwBytesRead == 0 || dwBytesRead != dwSize)
	{
		_tprintf(_T("Could not read from file w/err 0x%08lx\n"),
			GetLastError());
		CloseHandle(hImage);
		// Destroy the safe array
		SafeArrayUnaccessData(*ppsaChunk);
		SafeArrayDestroy(*ppsaChunk);
        return 0;
	}

	SafeArrayUnaccessData(*ppsaChunk);

	CloseHandle(hImage);

	return dwSize;
}