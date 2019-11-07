//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3_oAuthFlickr.xaml.h"
#include <string>
#include <sstream>

using namespace SDKSample::WebAuthentication;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

using namespace Concurrency;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Security::Authentication::Web;
using namespace Windows::UI::ApplicationSettings;

using namespace Windows::UI::Popups;
using namespace Windows::Security::Credentials;
using namespace Windows::Storage;
using namespace Windows::Web::Http;
using namespace Windows::Data::Json;


std::vector<std::wstring> &Split(const std::wstring &s, char delim, std::vector<std::wstring> &elems) {
    std::wstringstream ss(s);
    std::wstring item;
    while (std::getline<wchar_t>(ss, item, delim)) {
        elems.push_back(item);
    }
    return elems;
}


std::vector<std::wstring> Split(const std::wstring &s, char delim) {
    std::vector<std::wstring> elems;
    Split(s, delim, elems);
    return elems;
}

Scenario3::Scenario3()
{
    InitializeComponent();

	roamingSettings = ApplicationData::Current->RoamingSettings;
	InitializeWebAccountProviders();
    InitializeWebAccounts();

	this->commandsRequestedEventRegistrationToken = SettingsPane::GetForCurrentView()->CommandsRequested += 
		ref new TypedEventHandler<SettingsPane ^, SettingsPaneCommandsRequestedEventArgs ^>(this, &Scenario3::OnCommandsRequested);
	this->accountCommandsRequestedEventRegistrationToken =  AccountsSettingsPane::GetForCurrentView()->AccountCommandsRequested += 
		ref new TypedEventHandler<AccountsSettingsPane ^, AccountsSettingsPaneCommandsRequestedEventArgs ^>(this, &Scenario3::OnAccountCommandsRequested);

}

void Scenario3::InitializeWebAccountProviders()
{

    facebookProvider = ref new WebAccountProvider(
        "Facebook.com",
        "Facebook",
        ref new Uri("ms-appx:///icons/Facebook.png"));

    googleProvider = ref new WebAccountProvider(
                            "Google.com",
							"YouTube",
                            ref new Uri("ms-appx:///icons/youtube.png"));
}

void Scenario3::InitializeWebAccounts()
{
    auto values = roamingSettings->Values;
	String^ facebookToken = safe_cast<String^>(roamingSettings->Values->Lookup("FACEBOOK_OAUTH_TOKEN"));
	if (!facebookToken)
    {
        isFacebookUserLoggedIn = false;
    }
    else
    {
		String^ facebookUser = safe_cast<String^>(roamingSettings->Values->Lookup("FACEBOOK_USER_NAME"));
        if (facebookUser)
        {
            this->facebookUserName = facebookUser;
            isFacebookUserLoggedIn = true;
        }
    }

    String^ youtubeRefreshToken = safe_cast<String^>(roamingSettings->Values->Lookup("YOUTUBE_REFRESH_TOKEN"));
	if ( !youtubeRefreshToken)
    {
        isGoogleUserLoggedIn = false;
    }
    else
    {
		String^ youtubeUser = safe_cast<String^>(roamingSettings->Values->Lookup("YOUTUBE_USER_NAME"));
        if (youtubeUser)
        {
            this->googleUserName = youtubeUser;
            isGoogleUserLoggedIn = true;
        }
    }
	
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario3::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void Scenario3::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // Added to make sure the event handler for CommandsRequested and OnAccountCommandsRequestedin cleaned up before other scenarios
    SettingsPane::GetForCurrentView()->CommandsRequested -= this->commandsRequestedEventRegistrationToken;
	AccountsSettingsPane::GetForCurrentView()->AccountCommandsRequested -= this->accountCommandsRequestedEventRegistrationToken;
}

/// <summary>
/// This event is generated when the user opens the settings pane. During this event, append your
/// SettingsCommand objects to the available ApplicationCommands vector to make them available to the
/// SettingsPange UI.
/// </summary>
/// <param name="settingsPane">Instance that triggered the event.</param>
/// <param name="eventArgs">Event data describing the conditions that led to the event.</param>
void Scenario3::OnCommandsRequested(Windows::UI::ApplicationSettings::SettingsPane ^settingsPane, Windows::UI::ApplicationSettings::SettingsPaneCommandsRequestedEventArgs ^eventArgs)
{
	eventArgs->Request->ApplicationCommands->Append(SettingsCommand::AccountsCommand);
}

/// <summary>
/// This event is generated when the user clicks on Accounts command in settings pane. During this event, add your
/// WebAccountProviderCommand, WebAccountCommand, CredentialCommand and  SettingsCommand objects to make them available to the
/// AccountsSettingsPane UI.
/// </summary>
/// <param name="accountsSettingsPane">Instance that triggered the event.</param>
/// <param name="eventArgs">Event data describing the conditions that led to the event.</param>
void Scenario3::OnAccountCommandsRequested(Windows::UI::ApplicationSettings::AccountsSettingsPane ^accountsSettingsPane, Windows::UI::ApplicationSettings::AccountsSettingsPaneCommandsRequestedEventArgs ^eventArgs)
{
	auto  deferral = eventArgs->GetDeferral();
	
	eventArgs->HeaderText = "This is sample text. You can put a message here to give context to user. This section is optional.";

	// add account provider commands 
	WebAccountProviderCommandInvokedHandler^ providerHandler = ref new WebAccountProviderCommandInvokedHandler(this, &Scenario3::WebAccountProviderInvokedHandler); 
	WebAccountProviderCommand^ facebookProviderCmd = ref new WebAccountProviderCommand(this->facebookProvider, providerHandler); 
	eventArgs->WebAccountProviderCommands->Append(facebookProviderCmd); 
	WebAccountProviderCommand^ twitterProviderCmd = ref new WebAccountProviderCommand(this->googleProvider, providerHandler); 
	eventArgs->WebAccountProviderCommands->Append(twitterProviderCmd); 
	
	// add account commands 
	WebAccountCommandInvokedHandler^ accountHandler = ref new WebAccountCommandInvokedHandler(this, &Scenario3::WebAccountInvokedHandler); 
	if(isFacebookUserLoggedIn)
	{
		this->facebookAccount = ref new WebAccount(this->facebookProvider, this->facebookUserName, WebAccountState::Connected);
		WebAccountCommand^ facebookAccountCmd = ref new WebAccountCommand( facebookAccount, accountHandler, SupportedWebAccountActions::Manage | SupportedWebAccountActions::Remove); 
		eventArgs->WebAccountCommands->Append(facebookAccountCmd); 
	}

	if(isGoogleUserLoggedIn)
	{
		this->googleAccount = ref new WebAccount(this->googleProvider, this->googleUserName, WebAccountState::Connected);
		WebAccountCommand^ twitterAccountCmd = ref new WebAccountCommand( this->googleAccount, accountHandler, SupportedWebAccountActions::Manage | SupportedWebAccountActions::Remove); 
		eventArgs->WebAccountCommands->Append(twitterAccountCmd); 
	}
	
	// add app specific command links. 
	UICommandInvokedHandler^ handler = ref new UICommandInvokedHandler( this, &Scenario3::HandleAppSpecificCmd); 
	SettingsCommand^ appCmd = ref new SettingsCommand( 1, // id 
		"Privacy policy", // Label 
		handler); 
	eventArgs->Commands->Append(appCmd); 
			 
	deferral->Complete();
}

/// <summary>
/// This event is generated when the user clicks on Account provider tile. This method is 
/// responsible for deciding what to do further.
/// </summary>
/// <param name="providerCmd">WebAccountProviderCommand instance that triggered the event.</param>
void Scenario3::WebAccountProviderInvokedHandler(WebAccountProviderCommand^ providerCmd) 
{ 
	if(providerCmd->WebAccountProvider->Id->Equals("Facebook.com"))
    {
        if(!isFacebookUserLoggedIn)
		{
			AuthenticateToFacebook();
		}
		else
		{
			DebugArea->Text += "User is already logged in. If you support multiple accounts from the same provider then do something here to connect new user.\r\n";
		}
    }
    else if (providerCmd->WebAccountProvider->Id->Equals("Google.com"))
    {
        if(!isGoogleUserLoggedIn)
		{
			AuthenticateToGoogle();
		}
		else
		{
			DebugArea->Text += "User is already logged in. If you support multiple accounts from the same provider then do something here to connect new user.\r\n";
		}
    }
} 

/// <summary>
/// This event is generated when the user clicks on action button on account details pane. This method is 
/// responsible for handling what to do with selected action.
/// </summary>
/// <param name="accountCmd">Instance that triggered the event.</param>
/// <param name="args">Event data describing the conditions that led to the event.</param>
void  Scenario3::WebAccountInvokedHandler(WebAccountCommand^ accountCmd, WebAccountInvokedArgs^ args) 
{ 
	DebugArea->Text += "Account State = " + accountCmd->WebAccount->State.ToString() + " and Selected Action = " + args->Action.ToString() + "\r\n";
            
    if (args->Action == WebAccountAction::Remove)
    {
        //Remove user logon information since user requested to remove account.
        if (accountCmd->WebAccount->WebAccountProvider->Id->Equals("Facebook.com"))
        {
            roamingSettings->Values->Remove("FACEBOOK_USER_NAME");
            roamingSettings->Values->Remove("FACEBOOK_OAUTH_TOKEN");
            isFacebookUserLoggedIn = false;
        }
        else if (accountCmd->WebAccount->WebAccountProvider->Id->Equals("Google.com"))
        {
            roamingSettings->Values->Remove("YOUTUBE_USER_NAME");
            roamingSettings->Values->Remove("YOUTUBE_REFRESH_TOKEN");
            isGoogleUserLoggedIn = false;
        }
    }
} 

/// <summary>
/// This is the event handler for links added to Accounts Settings pane. This method can do more work based on selected link.
/// </summary>
/// <param name="command">Link instance that triggered the event.</param>
void Scenario3::HandleAppSpecificCmd(IUICommand^ command) 
{ 
	DebugArea->Text = "Link clicked: " + command->Label + "\r\n";
} 



/// <summary>
/// Event handler for Show button. This method demonstrates how to show AccountsSettings pane programatically.
/// </summary>
/// <param name="sender">Instance that triggered the event.</param>
/// <param name="e">Event data describing the conditions that led to the event.</param>
void Scenario3::Show_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{

	AccountsSettingsPane::Show();

}


void Scenario3::AuthenticateToFacebook()
{
    DebugArea->Text = "";
    ReturnedToken->Text = "";

    String^ facebookURL = "https://www.facebook.com/dialog/oauth?client_id=";

    auto clientID = FacebookClientID->Text;
    if (clientID == nullptr || clientID->IsEmpty())
    {
        DebugArea->Text = "Enter a ClientID \r\n";
        return;
    }

    facebookURL += clientID + "&redirect_uri=https%3A%2F%2Fwww.facebook.com%2Fconnect%2Flogin_success.html&scope=read_stream&display=popup&response_type=token";

    try
    {
        auto facebookOutput = DebugArea;
        auto facebookToken = ReturnedToken;

        auto startURI = ref new Uri(facebookURL);
        auto endURI = ref new Uri("https://www.facebook.com/connect/login_success.html");
        DebugArea->Text += "Navigating to: " + facebookURL + "\r\n";
        
        create_task(WebAuthenticationBroker::AuthenticateAsync(WebAuthenticationOptions::None, startURI, endURI)).then([this](WebAuthenticationResult^ result)
        {
            String^ statusString;
            switch (result->ResponseStatus)
            {
            case WebAuthenticationStatus::ErrorHttp:
                statusString = "ErrorHttp: " + result->ResponseErrorDetail;
                break;
            case WebAuthenticationStatus::Success:
                statusString = "Success";				
                ReturnedToken->Text += result->ResponseData;
				GetFacebookUserName(result->ResponseData);
				isFacebookUserLoggedIn = true;
                break;
            case WebAuthenticationStatus::UserCancel:
                statusString = "UserCancel";
                break;
            }

            DebugArea->Text += "Status returned by WebAuth broker: " + statusString + "\r\n";						

         });
    }
    catch (Exception^ ex)
    {
        DebugArea->Text += "Error launching WebAuth " + ex->Message + "\r\n";
        return;
    }
}

/// <summary>
/// This function extracts access_token from the response returned from web authentication broker
/// and uses that token to get user information using facebook graph api. 
/// </summary>
/// <param name="webAuthResultResponseData">responseData returned from AuthenticateAsync result.</param>
void Scenario3::GetFacebookUserName(String^ webAuthResultResponseData)
{
    //Get Access Token first
	std::wstring responseData(webAuthResultResponseData->Data());
	responseData = responseData.substr(responseData.find(L"access_token"));
    
	std::vector<std::wstring> keyValPairs = Split(responseData, '&');
    std::wstring access_token;
    std::wstring expires_in;

	for(std::vector<int>::size_type i = 0; i != keyValPairs.size(); i++) 
	{
		std::vector<std::wstring>  splits = Split(keyValPairs[i], '=');
	
		if(splits[0] == L"access_token")
		{
			access_token = splits[1];
		}
		else if (splits[0] == L"expires_in")
		{
			expires_in = splits[1];
		}
	}

	//store access token locally for further use.
	String^ access_token_s = (ref new String(access_token.c_str()));
	auto values = this->roamingSettings->Values;
	values->Insert("FACEBOOK_OAUTH_TOKEN", dynamic_cast<PropertyValue^>(PropertyValue::CreateString(access_token_s)));
	DebugArea->Text += "\r\naccess_token = " + access_token_s + "\r\n";		

    //Request User info.
    HttpClient^ httpClient = ref new HttpClient();
    create_task(httpClient->GetStringAsync(ref new Uri("https://graph.facebook.com/me?access_token=" + access_token_s))).then([this](String^ response)
	{

		JsonObject^ value = JsonValue::Parse(response)->GetObject();
		this->facebookUserName = value->GetNamedString("name");
		auto values = this->roamingSettings->Values;
		values->Insert("FACEBOOK_USER_NAME", dynamic_cast<PropertyValue^>(PropertyValue::CreateString(facebookUserName)));
		DebugArea->Text += this->facebookUserName + " is connected!!\r\n";		
    });

}


void Scenario3::AuthenticateToGoogle()
{
    auto clientID = GoogleClientID->Text;
    if (clientID == nullptr || clientID->IsEmpty())
    {
        DebugArea->Text = "Enter a ClientID for Google";
        return;
    }

	String^ googleURL = "https://accounts.google.com/o/oauth2/auth?client_id=" + clientID + "&redirect_uri=urn:ietf:wg:oauth:2.0:oob" + "&response_type=code&scope=" + Uri::EscapeComponent("https://gdata.youtube.com") + " " + Uri::EscapeComponent("https://www.googleapis.com/auth/userinfo.profile");

    try
    {        
        auto startURI = ref new Uri(googleURL);
        auto endURI = ref new Uri("https://accounts.google.com/o/oauth2/approval?");
        DebugArea->Text = "Navigating to: " + googleURL + "\r\n";

        create_task(WebAuthenticationBroker::AuthenticateAsync(WebAuthenticationOptions::UseTitle, startURI, endURI)).then([this](WebAuthenticationResult^ result)
        {
            String^ statusString;
            switch (result->ResponseStatus)
            {
            case WebAuthenticationStatus::ErrorHttp:
                statusString = "ErrorHttp: " + result->ResponseErrorDetail;
                break;
            case WebAuthenticationStatus::Success:
                statusString = "Success";
				DebugArea->Text += result->ResponseData + "\r\n";
				GetYouTubeUserName(result->ResponseData);
				isGoogleUserLoggedIn = true;
                break;
            case WebAuthenticationStatus::UserCancel:
                statusString = "UserCancel";
                break;
            }

            DebugArea->Text += "Status returned by WebAuth broker: " + statusString + "\r\n";			
        });
    }
    catch (Exception^ ex)
    {
        DebugArea->Text += "Error launching WebAuth " + ex->Message + "\r\n";
        return;
    }
}
/// <summary>
/// This function extracts authorization code from response data and make a post request to get access_token
/// and refresh_token. It then uses access_token to get user information. Please refer to google developer documentation
/// for more information.
/// </summary>
/// <param name="webAuthResultResponseData">responseData returned from AuthenticateAsync result.</param>
void Scenario3::GetYouTubeUserName(String^ webAuthResultResponseData)
{
	std::wstring responseData(webAuthResultResponseData->Data());
	std::wstring authorizationCode = responseData.substr(responseData.find(L"code=") + 5);
    

	//lets get youtube access_token and  refresh_token from authorization code.
    String^ url = "https://accounts.google.com/o/oauth2/token";

    HttpClient^ httpClient = ref new HttpClient();
    String^ s = "";
    s += "client_id=" + Uri::EscapeComponent(GoogleClientID->Text) + "&";
    s += "client_secret=" + Uri::EscapeComponent(GoogleClientSecret->Text) + "&";
    s += "code=" + Uri::EscapeComponent(ref new String(authorizationCode.c_str())) + "&";
    s += "redirect_uri=" + Uri::EscapeComponent("urn:ietf:wg:oauth:2.0:oob") + "&";
    s += "grant_type=authorization_code";
	
	auto httpContent = ref new HttpStringContent(s);
	httpContent->Headers->ContentType = Headers::HttpMediaTypeHeaderValue::Parse("application/x-www-form-urlencoded");
	auto postOp = httpClient->PostAsync(ref new Uri(url), httpContent);
	create_task(postOp).then([this](HttpResponseMessage^ responseMessage)
	{
		if (responseMessage->StatusCode == HttpStatusCode::Ok)
	    {
			create_task(responseMessage->Content->ReadAsStringAsync()).then([this](String^ strResponse)
			{
				JsonValue^ jsonValue = JsonValue::Parse(strResponse);
				String^ YouTubeAccessToken =  jsonValue->GetObject()->GetNamedString("access_token");
				String^ YouTubeRefreshToken =  jsonValue->GetObject()->GetNamedString("refresh_token");
				//Store refresh token locally for further use.
				auto values = this->roamingSettings->Values;
				values->Insert("YOUTUBE_REFRESH_TOKEN", dynamic_cast<PropertyValue^>(PropertyValue::CreateString(YouTubeRefreshToken)));
				DebugArea->Text += "\r\n access_token = " + YouTubeAccessToken + "\r\n";
				DebugArea->Text += "refresh_token = " + YouTubeRefreshToken + "\r\n";

				//Now get user information usign access_token
				HttpClient^ httpClient = ref new HttpClient();
				Uri^ youtubeUserfeedUri = ref new Uri("https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + YouTubeAccessToken);
				create_task(httpClient->GetStringAsync(youtubeUserfeedUri)).then([this](String^ response)
				{
					JsonObject^ value = JsonValue::Parse(response)->GetObject();
					this->googleUserName = value->GetNamedString("name");
					auto values = this->roamingSettings->Values;
					values->Insert("YOUTUBE_USER_NAME", dynamic_cast<PropertyValue^>(PropertyValue::CreateString(this->googleUserName)));
					DebugArea->Text += this->googleUserName + " is connected!! \r\n";
				});
			});
		}
	});

	
}

