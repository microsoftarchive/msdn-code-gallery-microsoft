/****************************** Module Header ******************************\
* Module Name:    Book.cpp
* Project:        CppUnvsAppDataTemplateDynamically
* Copyright (c) Microsoft Corporation.
*
* This class is used to initialize data in Book collection
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
#include "pch.h"
#include "Book.h"

using namespace CppUnvsAppDataTemplateDynamically;
Book::Book(Platform::String^ author, float32 price, Platform::String^ name) : _author(author), _price(price), _name(name)
{
}


Windows::Foundation::Collections::IObservableVector<Book^>^ Book::GetBooks()
{
	Windows::Foundation::Collections::IObservableVector<Book^>^  books = ref new Platform::Collections::Vector < Book^ >;
	books->Append(ref new Book("Allen", 29.99f, "War"));
	books->Append(ref new Book("Carter", 35.00f, "Fighting Like A Man"));
	books->Append(ref new Book("Rose", 39.99f, "Tomorrow"));
	books->Append(ref new Book("Daisy", 99.00f, "Last Three Days"));
	books->Append(ref new Book("Mary", 10.00f, "Fire Wall"));
	books->Append(ref new Book("Ray", 19.99f, "Lie"));
	books->Append(ref new Book("Sherry", 45.50f, "Three Wolves"));
	books->Append(ref new Book("Lisa", 36.99f, "Beauty"));
	books->Append(ref new Book("Judy", 12.00f, "The One"));
	books->Append(ref new Book("Jack", 88.99f, "Chosen by God"));
	books->Append(ref new Book("May", 130.00f, "The Magic"));
	books->Append(ref new Book("Vivian", 299.99f, "Who is the Murder"));

	return books;
}
