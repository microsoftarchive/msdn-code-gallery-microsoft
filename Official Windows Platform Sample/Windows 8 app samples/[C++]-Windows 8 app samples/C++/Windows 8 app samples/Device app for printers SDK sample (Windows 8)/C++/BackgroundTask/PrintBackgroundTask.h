#pragma once

namespace BackgroundTask
{

    public ref class PrintBackgroundTask sealed: public Windows::ApplicationModel::Background::IBackgroundTask
    {
    public:
        PrintBackgroundTask(void);
        virtual void Run(Windows::ApplicationModel::Background::IBackgroundTaskInstance^ taskInstance);

    private:
        Platform::String^ _keyPrinterName;
        Platform::String^ _keyAsyncUIXML;

        void ShowToast(Platform::String^ title, Platform::String^ body);
        void UpdateTile(Platform::String^ printerName, Platform::String^ bidiMessage);
        void UpdateBadge();
        ~PrintBackgroundTask(void);
    };

}