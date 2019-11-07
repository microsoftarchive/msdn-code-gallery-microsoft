// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Team.cpp
// Implementation of the Team class
//

#include "pch.h"
#include "Team.h"

using namespace SDKSample::DataBinding;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization::NumberFormatting;
using namespace Windows::UI::Xaml::Media;

Team::Team()
{
	_propBag = ref new PropertySet();
}

Teams::Teams()
{
	_propBag = ref new PropertySet();
	_items = ref new Platform::Collections::Vector<Object^>();

	auto team0 = ref new Team();
	team0->Name = "The Reds";
	team0->City = "Liverpool";
	team0->Color = ref new SolidColorBrush(Windows::UI::Colors::Green);
	this->Insert("0", team0);
	_items->Append(team0);

	auto team1 = ref new Team();
	team1->Name = "The Red Devils";
	team1->City = "Manchester";
	team1->Color = ref new SolidColorBrush(Windows::UI::Colors::Yellow);
	this->Insert("1", team1);
	_items->Append(team1);
	
	auto team2 = ref new Team();
	team2->Name = "The Blues";
	team2->City = "London";
	team2->Color = ref new SolidColorBrush(Windows::UI::Colors::Orange);
	this->Insert("2", team2);
	_items->Append(team2);

	auto team3 = ref new Team();
	team3->Name = "The Gunners";
	team3->City = "London";
	team3->Color = ref new SolidColorBrush(Windows::UI::Colors::Red);
	team3->Insert("Gaffer","le Professeur");
	team3->Insert("Skipper","Mr Gooner");
	this->Insert("3", team3);
	_items->Append(team3);
}

TeamGroup::TeamGroup(String^ name)
{
	_name = name;
	_items = ref new Platform::Collections::Vector<Object^>();
}

TeamDataSource::TeamDataSource()
{
	_itemGroups = ref new Platform::Collections::Vector<Object^>();

	auto liverpoolVector = ref new TeamGroup("Liverpool");
	auto manchesterVector = ref new TeamGroup("Manchester");
	auto londonVector = ref new TeamGroup("London");

	auto team0 = ref new Team();
	team0->Name = "The Reds";
	team0->City = "Liverpool";
	team0->Color = ref new SolidColorBrush(Windows::UI::Colors::Green);
	liverpoolVector->Items->Append(team0);

	auto team1 = ref new Team();
	team1->Name = "The Red Devils";
	team1->City = "Manchester";
	team1->Color = ref new SolidColorBrush(Windows::UI::Colors::Yellow);
	manchesterVector->Items->Append(team1);
	
	auto team2 = ref new Team();
	team2->Name = "The Blues";
	team2->City = "London";
	team2->Color = ref new SolidColorBrush(Windows::UI::Colors::Orange);
	londonVector->Items->Append(team2);

	auto team3 = ref new Team();
	team3->Name = "The Gunners";
	team3->City = "London";
	team3->Color = ref new SolidColorBrush(Windows::UI::Colors::Red);
	team3->Insert("Gaffer","le Professeur");
	team3->Insert("Skipper","Mr Gooner");
	londonVector->Items->Append(team3);

	_itemGroups->Append(liverpoolVector);
	_itemGroups->Append(manchesterVector);
	_itemGroups->Append(londonVector);
}



