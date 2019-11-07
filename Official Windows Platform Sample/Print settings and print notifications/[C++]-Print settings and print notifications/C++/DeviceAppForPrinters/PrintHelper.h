#pragma once

#include <PrinterExtension.h>
#include <OCIdl.h>
#include <wrl\client.h>
#include <wrl\implements.h>

namespace DeviceAppForPrinters
{
    public delegate void InkLevelReceivedHandler(uint32 result, Platform::String^ response);

    ref class PrintHelper;

    class PrinterQueueEventHandler : public Microsoft::WRL::RuntimeClass<Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::RuntimeClassType::ClassicCom>, IPrinterQueueEvent, IDispatch>
    {
    public:
        PrinterQueueEventHandler(PrintHelper^ currentHelper);
        ~PrinterQueueEventHandler(void);

        Platform::String^ WaitForBidiResponse(void);

        // IPrinterQueueEvent
        virtual HRESULT STDMETHODCALLTYPE OnBidiResponseReceived(BSTR bstrResponse, HRESULT hrStatus);

        // IDispatch
        virtual HRESULT STDMETHODCALLTYPE GetTypeInfoCount(__RPC__out UINT *) { return E_NOTIMPL; }
        virtual HRESULT STDMETHODCALLTYPE GetTypeInfo(UINT, LCID, __RPC__deref_out_opt ITypeInfo **) { return E_NOTIMPL; }        
        virtual HRESULT STDMETHODCALLTYPE GetIDsOfNames(__RPC__in REFIID, __RPC__in_ecount_full(cNames) LPOLESTR *, __RPC__in_range(0,16384) UINT cNames, LCID, __RPC__out_ecount_full(cNames) DISPID *) { return E_NOTIMPL; }      
        virtual HRESULT STDMETHODCALLTYPE Invoke(_In_  DISPID, _In_  REFIID, _In_  LCID, _In_  WORD, _In_  DISPPARAMS *, _Out_opt_  VARIANT *, _Out_opt_  EXCEPINFO *, _Out_opt_  UINT *); 

    private:
        PrintHelper^ _currentPrintHelperInstance;
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class PrintHelper sealed
    {
    public:
        PrintHelper();
        bool Initialize(Platform::Object^ contextObject);
        bool FeatureExists(Platform::String^ feature);
        Platform::String^ GetFeatureDisplayName(Platform::String^ feature);
        bool GetOptionInfo(Platform::String^ feature, Windows::UI::Xaml::Controls::ComboBox^ collection);
        bool IsOptionConstrained(Platform::String^ feature, int optionIndex);
        bool SetFeatureOption(Platform::String^ feature, int optionIndex);

        void RaiseInkLevelReceivedEvent(uint32 hresult, Platform::String^ response)
        {
            OnInkLevelReceived(hresult, response);
        }

        Platform::String^ SendInkLevelQuery();

        event InkLevelReceivedHandler^ OnInkLevelReceived;

    private:
        Microsoft::WRL::ComPtr<IPrinterExtensionContext> _spContext;
        Microsoft::WRL::ComPtr<IPrinterQueue> _spQueue;
        Microsoft::WRL::ComPtr<IPrintSchemaTicket> _spTicket;
        Microsoft::WRL::ComPtr<IPrintSchemaCapabilities> _spCapabilities;

        Microsoft::WRL::ComPtr<PrinterQueueEventHandler> _spHandler;

        Microsoft::WRL::ComPtr<IConnectionPoint> _spConnection;
        DWORD _cookie;

        ~PrintHelper(void);
    };
}