#include "pch.h"
#include<Windows.h>
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml::Controls;
using namespace Platform;

namespace StreamSocketTransportHelper
{
    namespace DiagnosticsHelper
    {
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Diag sealed
        {
            static CoreDispatcher^ coreDispatcher;
            static TextBlock^ debugOutputTextBlock;
        public:
            static void SetCoreDispatcher(CoreDispatcher^ dispatcher)
            {
                coreDispatcher=dispatcher;
            }
            static void SetDebugTextBlock(TextBlock^ debug)
            {
                debugOutputTextBlock=debug;
            }
            static void DebugPrint(String^ msg)
            {
                if (coreDispatcher != nullptr)
                {
                    coreDispatcher->RunAsync(CoreDispatcherPriority::Normal,ref new DispatchedHandler([msg](){
                        SYSTEMTIME systime;
                        GetLocalTime(&systime);
                        if(debugOutputTextBlock->Text!=nullptr)
                        {
                            debugOutputTextBlock->Text = debugOutputTextBlock->Text + systime.wMonth + "/" + 
                                systime.wDay + "/" + systime.wYear + " " + systime.wHour + ":" + systime.wMinute + ":" + " " + msg + "\r\n";
                        }
                        else
                        {
                            debugOutputTextBlock->Text = systime.wMonth + "/" + 
                                systime.wDay + "/" + systime.wYear + " " + systime.wHour + ":" + systime.wMinute + ":" + " " + msg + "\r\n";
                        }
                    }));
                }
            }
        };
    }
}