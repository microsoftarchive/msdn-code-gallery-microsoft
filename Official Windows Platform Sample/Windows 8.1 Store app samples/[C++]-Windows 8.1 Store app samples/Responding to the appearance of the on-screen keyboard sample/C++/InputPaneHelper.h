#include <set>
#include <collection.h>

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;

namespace KeyboardEventsSampleCPP
{
    public delegate void InputPaneShowingHandler(Object^ sender, InputPaneVisibilityEventArgs^ e);
    public delegate void InputPaneHidingHandler(InputPane^ sender, InputPaneVisibilityEventArgs^ e);

    
    ref class ElementInfo
    {
    public:
        property UIElement^ Element;
        property InputPaneShowingHandler^ ShowingHandler;
        property EventRegistrationToken GotFocusToken;
        property EventRegistrationToken LostFocusToken;
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class InputPaneHelper sealed
    {
    public:
        InputPaneHelper() 
        {
            _handlerMap = ref new Map<int, ElementInfo^>();  
			_gotFocusHandler = ref new RoutedEventHandler(this, &InputPaneHelper::GotFocus);
            _lostFocusHandler = ref new RoutedEventHandler(this, &InputPaneHelper::LostFocus);
		}

		void SubscribeToKeyboard(bool subscribe) 
		{
            auto input = InputPane::GetForCurrentView();  
 
			if (subscribe) 
			{
				_showingToken = input->Showing += ref new TypedEventHandler<InputPane^,InputPaneVisibilityEventArgs^>(this, &InputPaneHelper::ShowingHandler);  
				_hidingToken = input->Hiding += ref new TypedEventHandler<InputPane^,InputPaneVisibilityEventArgs^>(this, &InputPaneHelper::HidingHandler);
			}
			else
			{
				input->Showing -= _showingToken;
				input->Hiding -= _hidingToken;
			}
        }

        void SetHidingHandler(InputPaneHidingHandler^ handler)  
        {  
            _hidingHandlerDelegate = handler;  
        }

        void AddShowingHandler(UIElement^ element, InputPaneShowingHandler^ handler)
        {
            int hashCode = element->GetHashCode();
            if (!_handlerMap->HasKey(hashCode))
            {  
                auto info = ref new ElementInfo();
                
                info->Element = element;
                info->ShowingHandler = handler;
                info->GotFocusToken = element->GotFocus += _gotFocusHandler;
                info->LostFocusToken = element->LostFocus += _lostFocusHandler;
                
                _handlerMap->Insert(hashCode, info);
            }  
            else  
            {
                throw ref new InvalidArgumentException();  
            }  
        } 

        void RemoveShowingHandler(UIElement^ element)  
        {  
            int hashCode = element->GetHashCode();
            if (_handlerMap->HasKey(hashCode))
            {
                auto info = _handlerMap->Lookup(hashCode);
                _handlerMap->Remove(hashCode); 
                element->GotFocus -= info->GotFocusToken;  
                element->LostFocus -= info->LostFocusToken;
            }
            else
            {
                throw ref new InvalidArgumentException();
            }  
        }

        bool IsElementListening(UIElement^ element)  
        {  
            return _handlerMap->HasKey(element->GetHashCode());  
        }

    private:
        Map<int,ElementInfo^>^ _handlerMap;
        UIElement^ _lastFocusedElement;
        InputPaneHidingHandler^ _hidingHandlerDelegate;
        RoutedEventHandler^ _gotFocusHandler;
        RoutedEventHandler^ _lostFocusHandler;
        EventRegistrationToken _showingToken, _hidingToken;

        // It's important to tell which element got focus in order to decide if
        // the keyboard handler should be called          
        void GotFocus(Object^ sender, RoutedEventArgs^ e)  
        {  
            _lastFocusedElement = dynamic_cast<UIElement^>(sender);  
        }  
  
        void LostFocus(Object^ sender, RoutedEventArgs^ e)  
        {  
            if (_lastFocusedElement == dynamic_cast<UIElement^>(sender))  
            {  
                _lastFocusedElement = nullptr;  
            }  
        }  

        void ShowingHandler(InputPane^ sender, InputPaneVisibilityEventArgs^ e)  
        {  
            if (_lastFocusedElement != nullptr && _handlerMap->Size > 0)  
            {  
                _handlerMap->Lookup(_lastFocusedElement->GetHashCode())->ShowingHandler->Invoke(_lastFocusedElement, e);  
            }  
            _lastFocusedElement = nullptr;  
        }  

        void HidingHandler(InputPane^ sender, InputPaneVisibilityEventArgs^ e)  
        {  
            if (_hidingHandlerDelegate != nullptr)  
            {  
                _hidingHandlerDelegate->Invoke(sender, e);  
            }  
            _lastFocusedElement = nullptr;  
        }

        ~InputPaneHelper()
        {
			SubscribeToKeyboard(false);

            auto iter = _handlerMap->First();

            while (iter->HasCurrent)
            {
                auto elementInfo = iter->Current->Value;
                elementInfo->Element->GotFocus -= elementInfo->GotFocusToken;
                elementInfo->Element->LostFocus -= elementInfo->LostFocusToken;

                iter->MoveNext();
            }

			// Clearing the map frees the references to the UIElements, which
            // is necessary to break circular references in some cases
			_handlerMap->Clear();
        }
    };
}