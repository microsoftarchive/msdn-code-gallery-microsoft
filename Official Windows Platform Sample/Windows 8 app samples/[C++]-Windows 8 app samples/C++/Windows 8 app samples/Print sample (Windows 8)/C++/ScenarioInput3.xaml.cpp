// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario3Input.xaml.cpp
// Implementation of the Scenario3Input class.
//

#include "pch.h"
#include "ScenarioInput3.xaml.h"
#include "ScenarioOutput3.xaml.h"
#include "MainPage.xaml.h"

using namespace PrintSample;

using namespace Windows::Foundation::Collections;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::Graphics::Printing;
using namespace Windows::UI;
using namespace Windows::UI::Text;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Documents;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::UI::Xaml::Navigation;

ScenarioInput3::ScenarioInput3()
{
    InitializeComponent();
}

/// <summary>
/// Provide print content for scenario 3 first page
/// </summary>
void ScenarioInput3::PreparetPrintContent()
{
    if (FirstPage == nullptr)
    {
        FirstPage = ref new ScenarioOutput3();
        StackPanel^ header = safe_cast<StackPanel^>(FirstPage->FindName("header"));
        header->Visibility = Windows::UI::Xaml::Visibility::Visible;
    }

    // Add the (newley created) page to the printing root which is part of the visual tree and force it to go
    // through layout so that the linked containers correctly distribute the content inside them.
    PrintingRoot->Children->Append(FirstPage);
    PrintingRoot->InvalidateMeasure();
    PrintingRoot->UpdateLayout();
}

/// <summary>
/// This is the event handler for PrintManager.PrintTaskRequested.
/// In order to ensure a good user experience, the system requires that the app handle the PrintTaskRequested event within the time specified by PrintTaskRequestedEventArgs->Request->Deadline.
/// Therefore, we use this handler to only create the print task.
/// The print settings customization can be done when the print document source is requested.
/// </summary>
/// <param name="sender">PrintManager</param>
/// <param name="e">PrintTaskRequestedEventArgs</param>
void ScenarioInput3::PrintTaskRequested(PrintManager^ sender, PrintTaskRequestedEventArgs^ e)
{
    auto printTaskRef = std::make_shared<PrintTask^>(nullptr);
    *printTaskRef = e->Request->CreatePrintTask("C++ Printing SDK Sample",
                                                ref new Windows::Graphics::Printing::PrintTaskSourceRequestedHandler([this, printTaskRef](PrintTaskSourceRequestedArgs^ args)
                                                {
                                                PrintTask^ printTask = *printTaskRef;

                                                IVector<String^>^ displayedOptions = printTask->Options->DisplayedOptions;

                                                // clear the print options that are displayed beacuse adding the same one multiple times will throw an exception
                                                displayedOptions->Clear();
                                            
                                                // Choose the printer options to be shown.
                                                // The order in which the options are appended determines the order in which they appear in the UI
                                                displayedOptions->Append(StandardPrintTaskOptions::Copies);
                                                displayedOptions->Append(StandardPrintTaskOptions::Orientation);
                                                displayedOptions->Append(StandardPrintTaskOptions::MediaSize);
                                                displayedOptions->Append(StandardPrintTaskOptions::Collation);
                                                displayedOptions->Append(StandardPrintTaskOptions::Duplex);

                                                // Preset the default value of the printer option
                                                printTask->Options->MediaSize = PrintMediaSize::NorthAmericaLegal;

                                                // Print Task event handler is invoked when the print job is completed.
                                                printTask->Completed += ref new TypedEventHandler<PrintTask^, PrintTaskCompletedEventArgs^>(
                                                [=](PrintTask^ sender, PrintTaskCompletedEventArgs^ e)
                                                {
                                                    // Notify the user when the print operation fails.
                                                    if (e->Completion == Windows::Graphics::Printing::PrintTaskCompletion::Failed)
                                                    {
                                                        auto callback = ref new Windows::UI::Core::DispatchedHandler(
                                                        [=]()
                                                        {
                                                            RootPage->NotifyUser(ref new String(L"Failed to print."), NotifyType::ErrorMessage);
                                                        });

                                                        Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, callback);
                                                    }
                                                });

                                                args->SetSource(PrintDocumentSource);
                                                }));
}

void ScenarioInput3::OnNavigatedTo(NavigationEventArgs^ e)
{
    BasePrintPage::OnNavigatedTo(e);

    RegisterForPrinting();
}

void ScenarioInput3::OnNavigatedFrom(NavigationEventArgs^ e)
{
    UnregisterForPrinting() ;
}