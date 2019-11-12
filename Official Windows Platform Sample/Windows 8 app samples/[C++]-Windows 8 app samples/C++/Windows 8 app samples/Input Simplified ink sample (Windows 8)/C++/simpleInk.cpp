// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

#include "pch.h"
#include <ppltasks.h>
#include "simpleInk.h"

#pragma region Helper functions

typedef enum 
{
    None,
    Ink,
    Select,
    Erase
} PointerEventType;

bool EventIsErase(_In_ Windows::UI::Core::PointerEventArgs^ e)
{
    auto pointerDevice = e->CurrentPoint->PointerDevice;
    auto pointerProperties = e->CurrentPoint->Properties;

    bool rval = false
        || (pointerProperties->IsEraser)
        || (pointerDevice->PointerDeviceType == Windows::Devices::Input::PointerDeviceType::Pen && !pointerProperties->IsBarrelButtonPressed && pointerProperties->IsRightButtonPressed)
        || (pointerDevice->PointerDeviceType == Windows::Devices::Input::PointerDeviceType::Mouse && pointerProperties->IsRightButtonPressed);

    return rval;
}

bool EventIsInk(_In_ Windows::UI::Core::PointerEventArgs^ e)
{
    auto pointerDevice = e->CurrentPoint->PointerDevice;
    auto pointerProperties = e->CurrentPoint->Properties;

    bool rval = false
        || (pointerDevice->PointerDeviceType == Windows::Devices::Input::PointerDeviceType::Pen && !pointerProperties->IsBarrelButtonPressed && !pointerProperties->IsRightButtonPressed)
        || (pointerDevice->PointerDeviceType == Windows::Devices::Input::PointerDeviceType::Mouse && pointerProperties->IsLeftButtonPressed && e->KeyModifiers == Windows::System::VirtualKeyModifiers::None);

    return rval;
}

bool EventIsSelect(_In_ Windows::UI::Core::PointerEventArgs^ e)
{
    auto pointerDevice = e->CurrentPoint->PointerDevice;
    auto pointerProperties = e->CurrentPoint->Properties;

    bool rval = false
        || (pointerDevice->PointerDeviceType == Windows::Devices::Input::PointerDeviceType::Pen && pointerProperties->IsBarrelButtonPressed)
        || (pointerDevice->PointerDeviceType == Windows::Devices::Input::PointerDeviceType::Mouse && pointerProperties->IsLeftButtonPressed && e->KeyModifiers == Windows::System::VirtualKeyModifiers::Control);

    return rval;
}

PointerEventType GetPointerEventType(_In_ Windows::UI::Core::PointerEventArgs^ e)
{
    PointerEventType type = PointerEventType::None;

    if (EventIsErase(e))
    {
        type = PointerEventType::Erase;
    }
    else if (EventIsInk(e))
    {
        type = PointerEventType::Ink;
    }
    else if (EventIsSelect(e))
    {
        type = PointerEventType::Select;
    }

    return type;
}

void CreateBrush(
    _In_ ID2D1DeviceContext* d2dContext, 
    _In_ Windows::UI::Color color, 
    _Outptr_ ID2D1SolidColorBrush** brush)
{
    D2D1::ColorF d2dColor(color.R/255.0f, color.G/255.0f, color.B/255.0f);
    *brush = nullptr;
    DX::ThrowIfFailed(
        d2dContext->CreateSolidColorBrush(
            d2dColor,
            brush
            )
        );
}

bool AnySelected(_In_ Windows::Foundation::Collections::IVectorView<Windows::UI::Input::Inking::InkStroke^>^ strokes)
{
    for (unsigned int i = 0; i < strokes->Size; i++)
    {
        if (strokes->GetAt(i)->Selected)
        {
            return true;
        }
    }
    return false;
}

void SelectAll(_In_ Windows::Foundation::Collections::IVectorView<Windows::UI::Input::Inking::InkStroke^>^ strokes, _In_ bool val)
{
    for (unsigned int i = 0; i < strokes->Size; i++)
    {
        strokes->GetAt(i)->Selected = val;
    }
}

#pragma endregion

//-----------------------------------------------------------------------------
// simpleInk implementation
//-----------------------------------------------------------------------------

simpleInk::simpleInk() :
    _backgroundColor(Windows::UI::Colors::White),
    _backgroundBrush(nullptr),
    _inkBrush(nullptr),
    _messageBrush(nullptr),
    _selectionBrush(nullptr),
    _currentBuffer(nullptr),
    _previousBuffer(nullptr),
    _inkStyle(nullptr),
    _selectionStyle(nullptr),
    _pointerId(-1),
    _recognitionText(""),
    _recognizerId(0),
    _renderingMode(Bezier),
    _manipulationMode(Windows::UI::Input::Inking::InkManipulationMode::Inking),
    _statusMessage("")
{
    // Change the number of rendering buffers (the constructor of DirectXBase sets it to 2)
    // to account for the special needs of apps that must support live rendering of ink.
    // Notice that buffers are created by CreateWindowSizeDependentResources, so it is safe
    // to change the number here.
    //
    // We must deviate from the recommended practice of using 2 buffers because calling
    // Present more than m_numBuffers-1 times within one VSync cycle will block the app.
    // We use 4 buffers because in live rendering mode we will be calling Present every time
    // we receive input updates, and we estimate the ratio between the frequency of updates 
    // and that of VSync to be between 2 and 3.
    m_numBuffers = 4;

    _manipulationPoints = ref new Platform::Collections::Vector<Windows::Foundation::Point>();

    _strokeBuilder = ref new Windows::UI::Input::Inking::InkStrokeBuilder();
    _strokeContainer = ref new Windows::UI::Input::Inking::InkStrokeContainer();
    _recognizerContainer = ref new Windows::UI::Input::Inking::InkRecognizerContainer();

    // take the first recognizer from the recognizer collection and set it as default
    _recognizer = _recognizerContainer->GetRecognizers()->GetAt(_recognizerId);
    _recognizerContainer->SetDefaultRecognizer(_recognizer);

    _drawingAttributes = ref new Windows::UI::Input::Inking::InkDrawingAttributes();
    _drawingAttributes->Color = Windows::UI::Colors::Black;
    _drawingAttributes->PenTip = Windows::UI::Input::Inking::PenTipShape::Circle;
    _drawingAttributes->Size = Windows::Foundation::Size(3.0f, 3.0f);
    _drawingAttributes->IgnorePressure = true;
    _drawingAttributes->FitToCurve = true;

    _strokeBuilder->SetDefaultDrawingAttributes(_drawingAttributes);
}

#pragma region Pointer event handlers (live rendering)

void simpleInk::OnPointerPressed(
    _In_ Windows::UI::Core::CoreWindow^,
    _In_ Windows::UI::Core::PointerEventArgs^ args)
{
    Windows::UI::Input::PointerPoint^ pointerPoint = args->CurrentPoint;

    // Make sure no pointer is already inking (we allow only one 'active' pointer at a time) 
    // Make sure pointer is in inking mode (pen and no button, or mouse and left button down)
    if (_pointerId == -1)
    {
        _statusMessage = "";
        _manipulationPoints->Clear();
        SelectAll(_strokeContainer->GetStrokes(), false);
       
        switch (GetPointerEventType(args))
        {
        case PointerEventType::Erase:
            _manipulationMode = Windows::UI::Input::Inking::InkManipulationMode::Erasing;
            _statusMessage = "Erase mode: scribble across the ink you want to erase.";
            _manipulationPoints->Append(pointerPoint->Position);
            break;
        case PointerEventType::Ink:
             _manipulationMode = Windows::UI::Input::Inking::InkManipulationMode::Inking;
            _renderingMode = Live; // enter live rendering mode
            // this is the first point of the stroke, begin the stroke with the stroke builder
            _strokeBuilder->BeginStroke(pointerPoint);
            break;
        case PointerEventType::Select:
            _manipulationMode = Windows::UI::Input::Inking::InkManipulationMode::Selecting;
            _statusMessage = "Select mode: draw a contour around the ink you want to select.";
            _manipulationPoints->Append(pointerPoint->Position);
            break;
        default:
            return; // pointer is neither inking nor erasing nor selecting: do nothing
        }

        Render();

        _pointerId = pointerPoint->PointerId; // save pointer id so that no other pointer can ink until this one it is released      
    }
}

void simpleInk::OnPointerMoved(
    _In_ Windows::UI::Core::CoreWindow^,
    _In_ Windows::UI::Core::PointerEventArgs^ args)
{ 
    Windows::UI::Input::PointerPoint^ pointerPoint = args->CurrentPoint;

    // Make sure the event belongs to the pointer that is currently inking
    if (_pointerId == (int) pointerPoint->PointerId)
    {
        if (_manipulationMode == Windows::UI::Input::Inking::InkManipulationMode::Erasing)
        {
            // Erase ink that intersects line from last point to current point
            Windows::Foundation::Rect invalidateRect = _strokeContainer->SelectWithLine(_manipulationPoints->GetAt(0), pointerPoint->Position);
            if (invalidateRect.Height != 0 || invalidateRect.Width != 0)
            {
                _strokeContainer->DeleteSelected();
                Render();
            }

            // Store current point: it will be the starting point at next pointer update
            _manipulationPoints->Clear();
            _manipulationPoints->Append(pointerPoint->Position);
        }
        else
        {
            Windows::Foundation::Point previousPoint;
            ID2D1StrokeStyle* strokeStyle;
            ID2D1SolidColorBrush* brush;
            float width;

            if (_manipulationMode == Windows::UI::Input::Inking::InkManipulationMode::Inking)
            {
                // Obtain intermediate points (including the last/current one)
                Windows::Foundation::Collections::IVector<Windows::UI::Input::PointerPoint^>^ intermediatePoints = args->GetIntermediatePoints();

                // Update ink manager with all intermediate points
                int i = intermediatePoints->Size - 1;
                // AppendToStroke returns the last point that was added to the stroke builder.
                // We need to save it because it is the initial point of the new line we want to render.
                previousPoint = _strokeBuilder->AppendToStroke(intermediatePoints->GetAt(i))->Position;
                for (i = i - 1; i >= 0; i--)
                {
                    _strokeBuilder->AppendToStroke(intermediatePoints->GetAt(i));
                }

                // Setup drawing attributes for live rendering
                strokeStyle = _inkStyle.Get();
                brush = _inkBrush.Get();
                width = _drawingAttributes->Size.Width;
            }
            else // _manipulationMode == Windows::UI::Input::Inking::InkManipulationMode::Selecting
            {
                previousPoint = _manipulationPoints->GetAt(_manipulationPoints->Size - 1);
                _manipulationPoints->Append(pointerPoint->Position);

                // Setup drawing attributes for live rendering
                strokeStyle = _selectionStyle.Get();
                brush = _selectionBrush.Get();
                width = 1.0f;
            }

            // Live rendering
            // First we need to copy the content of the last presented buffer: it contains
            // the Beziers and the lines we rendered at previous pointer moves.
            m_d3dContext->CopyResource(_currentBuffer.Get(), _previousBuffer.Get());
            // Then we draw a new line, from the last position of the pointer to its current one.
            m_d2dContext->BeginDraw();
            m_d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());
            m_d2dContext->DrawLine(
                D2D1::Point2F(previousPoint.X, previousPoint.Y), 
                D2D1::Point2F(pointerPoint->Position.X, pointerPoint->Position.Y), 
                brush, 
                width, 
                strokeStyle);

            // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
            // is lost. It will be handled during the next call to Present.
            HRESULT hr = m_d2dContext->EndDraw();
            if (hr != D2DERR_RECREATE_TARGET)
            {
                DX::ThrowIfFailed(hr);
            }

            Present();
        }
    }
}

void simpleInk::OnPointerReleased(
    _In_ Windows::UI::Core::CoreWindow^,
    _In_ Windows::UI::Core::PointerEventArgs^ args)
{
    Windows::UI::Input::Inking::InkStroke^ stroke = nullptr;
    Windows::UI::Input::PointerPoint^ pointerPoint = args->CurrentPoint;

    // Make sure the event belongs to the pointer that is currently inking
    if (_pointerId == (int) pointerPoint->PointerId)
    {
        _renderingMode = Bezier;    // exit live rendering mode
        _pointerId = -1;            // release 'active' pointer so that a new pointer may start inking again
        _statusMessage = "";

        switch (_manipulationMode)
        {
        case Windows::UI::Input::Inking::InkManipulationMode::Erasing:
            _strokeContainer->SelectWithLine(_manipulationPoints->GetAt(0), pointerPoint->Position);
            _strokeContainer->DeleteSelected();
            break;
        case Windows::UI::Input::Inking::InkManipulationMode::Inking:
            // this is the last point in the stroke
            stroke = _strokeBuilder->EndStroke(pointerPoint);
    
            // add stroke to the stoke container
            _strokeContainer->AddStroke(stroke);

            // out of curiosity we shall change the color and the width of the next stroke
            OnChangeDrawingAttributes();
            break;
        case Windows::UI::Input::Inking::InkManipulationMode::Selecting:
            _manipulationPoints->Append(pointerPoint->Position);
            _strokeContainer->SelectWithPolyLine(_manipulationPoints);
            break;
        }

         _manipulationPoints->Clear();

        // re-render the entire scene
        Render();
    }
}

#pragma endregion

#pragma region Bezier rendering

// convert bezier control points in each segment to a bezier curve in the path geometry
void simpleInk::ConvertStrokeToGeometry(_In_ Windows::UI::Input::Inking::InkStroke^ stroke, _Outptr_ ID2D1PathGeometry** geometry)
{
    // create a geometry path
    DX::ThrowIfFailed(m_d2dFactory->CreatePathGeometry(geometry));

    // create and initialize a geometry sink
    Microsoft::WRL::ComPtr<ID2D1GeometrySink> sink;
    DX::ThrowIfFailed((*geometry)->Open(&sink));
    sink->SetSegmentFlags(D2D1_PATH_SEGMENT_FORCE_ROUND_LINE_JOIN);
    sink->SetFillMode(D2D1_FILL_MODE_ALTERNATE);

    // obtain rendering segments for this stroke
    Windows::Foundation::Collections::IVectorView<Windows::UI::Input::Inking::InkStrokeRenderingSegment^>^ renderingSegments = stroke->GetRenderingSegments();

    // obtain first rendering segment, set as a starting point
    Windows::UI::Input::Inking::InkStrokeRenderingSegment^ firstSegment = renderingSegments->GetAt(0);
    Windows::Foundation::Point first = firstSegment->Position;
    sink->BeginFigure(D2D1::Point2F(first.X, first.Y), D2D1_FIGURE_BEGIN_FILLED);

    // process all remaining rendering segments, add bezier segment to the geometry path sink
    for (unsigned int j = 1; j < renderingSegments->Size; j++)
    {
        // obtain j-th rendering segment for the given stroke
        Windows::UI::Input::Inking::InkStrokeRenderingSegment^ renderingSegment = renderingSegments->GetAt(j);

        // add bezier segment to the geometry path sink
        sink->AddBezier(
            D2D1::BezierSegment(
                D2D1::Point2F(renderingSegment->BezierControlPoint1.X, renderingSegment->BezierControlPoint1.Y),
                D2D1::Point2F(renderingSegment->BezierControlPoint2.X, renderingSegment->BezierControlPoint2.Y),
                D2D1::Point2F(renderingSegment->Position.X, renderingSegment->Position.Y)
                )
            );
    }

    // done with given stroke, end and close geometry sink
    // sink will be automatically released as it is a smart pointer
    sink->EndFigure(D2D1_FIGURE_END_OPEN);
    DX::ThrowIfFailed(sink->Close());
}

// creates Bezier render, called by Render()
void simpleInk::BezierRender()
{
    // render all strokes
    Windows::Foundation::Collections::IVectorView<Windows::UI::Input::Inking::InkStroke^>^ inkStrokes = _strokeContainer->GetStrokes();
    for (unsigned int i = 0; i < inkStrokes->Size; i++)
    {
        // obtain the stroke
        Windows::UI::Input::Inking::InkStroke^ stroke = inkStrokes->GetAt(i);

        // convert stroke to geometry
        // geometry is a smart pointer and it is released right after it is used
        // for performance reasons one may consider storing geometry paths instead of auto-releasing them once they are used
        Microsoft::WRL::ComPtr<ID2D1PathGeometry> strokeGeometry;
        ConvertStrokeToGeometry(stroke, &strokeGeometry);

        // stroke's width and color are retrieved from the stroke's drawing attributes
        float width = stroke->DrawingAttributes->Size.Width;
        // create brush from current stroke
        Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> strokeBrush;
        CreateBrush(m_d2dContext.Get(), stroke->DrawingAttributes->Color, &strokeBrush);
        
        // render stroke geometry
        // if stroke is selected, we render its contour with the stroke's color and we fill it with white
        if (stroke->Selected)
        {
            m_d2dContext->DrawGeometry(strokeGeometry.Get(), strokeBrush.Get(), width + 2, _inkStyle.Get());
            m_d2dContext->DrawGeometry(strokeGeometry.Get(), _backgroundBrush.Get(), width, _inkStyle.Get());
        }
        else
        {
            m_d2dContext->DrawGeometry(strokeGeometry.Get(), strokeBrush.Get(), width, _inkStyle.Get());
        }        
    }
}

#pragma endregion

#pragma region Event handlers

void simpleInk::OnCharacterReceived(
    _In_ Windows::UI::Core::CoreWindow^, 
    _In_ Windows::UI::Core::CharacterReceivedEventArgs^ args)
{
    if (_pointerId == -1) // While in live rendering mode everything else is disabled
    {
        Platform::String^ oldStatusMessage = _statusMessage;

        // Reset status message, it will be updated by the functio that will be called
        _statusMessage = ""; // reset status message
        switch (args->KeyCode)
        {
        case 3: // ctrl+c, copy handler
            OnCopyToClipboard();
            break;
        case 20: // ctrl+t, copy text handler
            OnCopyTextToClipboard();
            break;
        case 22: // ctrl+v, paste handler
            SelectAll(_strokeContainer->GetStrokes(), false);
            OnPasteFromClipboard();
            break;
        case 19: // ctrl+s, save handler
            OnSave();
            break;
        case 15: // ctrl+o, open handler
            OnLoad();
            break;
        case 1: // ctrl+a, select all handler
            OnSelectAll();
            break;
        case 8: // backspace, delete handler
            OnDelete();
            break;
        case 4: // crtl+d, change drawing attributes
            OnChangeDrawingAttributes();
            break;
        case 18: // crtl+r, change recognizer
            OnChangeRecognizer();
            break;
        case 32: // space, recognize handler
            OnRecognize();
            break;
        default:
            // invalid command: ignore and restore old status message
            _statusMessage = oldStatusMessage;
        }
    }
}

void simpleInk::OnCopyToClipboard()
{
    // for most app scenarios it is sufficient to just call _strokeContainer->CopySelectedToClipboard();
    // for this sample we shall implement a slightly different scenario:
    //  - in case some strokes are selected we shall copy just the selected strokes
    //  - in case no strokes are selected we shall copy all strokes

    // obtain the strokes in the stroke container
    Windows::Foundation::Collections::IVectorView<Windows::UI::Input::Inking::InkStroke^>^ inkStrokes = _strokeContainer->GetStrokes();

    // check if any strokes are selected
    if (AnySelected(inkStrokes))
    {
        // some strokes are selected, copy only those
        _strokeContainer->CopySelectedToClipboard();
    }
    else
    {
        // no strokes are selected, copy all strokes to clipboard

        // select all strokes
        SelectAll(inkStrokes, true);

        // copy all selected strokes to clipboard
        _strokeContainer->CopySelectedToClipboard();

        // deselect all strokes
        SelectAll(inkStrokes, false);
    }
}

void simpleInk::OnCopyTextToClipboard()
{
    // we use data package to set text
    Windows::ApplicationModel::DataTransfer::DataPackage^ dataPackage = ref new Windows::ApplicationModel::DataTransfer::DataPackage();

    // set recognition text to data package
    dataPackage->SetText(_recognitionText);

    // copy text to clipboard
    Windows::ApplicationModel::DataTransfer::Clipboard::SetContent(dataPackage);
}

void simpleInk::OnPasteFromClipboard()
{
    // make sure that clipboard object is compatible with ink
    bool canPaste = _strokeContainer->CanPasteFromClipboard();
    if (canPaste)
    {
        // clipboard does contain ink compatible object

        // this is the position where ink will be pasted to
        Windows::Foundation::Point position;
        position.X = 100.0f;
        position.Y = 100.0f;

        _strokeContainer->PasteFromClipboard(position);
    }
    else 
    {
        _statusMessage = "Clipboard does not contain ink compatible objects. Cannot paste from clipboard.";
    }

    Render();
}

// how to use StrokeContainer::SaveAsync()
void simpleInk::OnSave()
{
    // NOTE: check that we have some ink to save before calling StrokeContainer::SaveAsync()
    if (_strokeContainer->GetStrokes()->Size > 0)
    {
        // File pickers don't work in snapped state. Make sure we are not in snapped state, or that we successfully unsnapped
        if (Windows::UI::ViewManagement::ApplicationView::Value != Windows::UI::ViewManagement::ApplicationViewState::Snapped || Windows::UI::ViewManagement::ApplicationView::TryUnsnap())
        {
            Windows::Storage::Pickers::FileSavePicker^ savePicker = ref new Windows::Storage::Pickers::FileSavePicker();
            savePicker->SuggestedStartLocation = Windows::Storage::Pickers::PickerLocationId::PicturesLibrary;

            auto extension = ref new Platform::Collections::Vector<Platform::String^>;
            extension->Append(".gif");
            savePicker->FileTypeChoices->Insert("Gif with embedded ISF", extension);

            savePicker->DefaultFileExtension = ".gif";
           
            concurrency::task<Windows::Storage::StorageFile^> savePickerTask(savePicker->PickSaveFileAsync());
            savePickerTask.then([this] (Windows::Storage::StorageFile^ file)
            {
                // file may be nullptr if end user clicked on cancel button
                if (nullptr != file)
                {
                    return file->OpenAsync(Windows::Storage::FileAccessMode::ReadWrite);
                }
                else
                {
                    // user clicked cancel, cancel all value based continuations
                    concurrency::cancel_current_task();
                }

            }).then([this] (Windows::Storage::Streams::IRandomAccessStream^ stream)
            {
                // IOutputStream is required by IRandomAccessStream, hence we can safely use the latter
                return _strokeContainer->SaveAsync(stream);

            });
        }
    }
    else
    {
        _statusMessage = "There is no ink to save.";
        Render();
    }
}

// how to use StrokeContainer::LoadAsync()
void simpleInk::OnLoad()
{
    // File pickers don't work in snapped state. Make sure we are not in snapped state, or that we successfully unsnapped
    if (Windows::UI::ViewManagement::ApplicationView::Value != Windows::UI::ViewManagement::ApplicationViewState::Snapped || Windows::UI::ViewManagement::ApplicationView::TryUnsnap())
    {
        Windows::Storage::Pickers::FileOpenPicker^ openPicker = ref new Windows::Storage::Pickers::FileOpenPicker();
        openPicker->SuggestedStartLocation = Windows::Storage::Pickers::PickerLocationId::PicturesLibrary;
        openPicker->FileTypeFilter->Append(".gif");

        concurrency::task<Windows::Storage::StorageFile^> openPickerTask(openPicker->PickSingleFileAsync());
        openPickerTask.then([this] (Windows::Storage::StorageFile^ file)
        {
            // file may be nullptr if end user clicked on cancel button
            if (nullptr != file)
            {
                return file->OpenAsync(Windows::Storage::FileAccessMode::Read);
            }
            else
            {
                // user clicked cancel, cancel all value based continuations
                concurrency::cancel_current_task();
            }

        }).then([this] (Windows::Storage::Streams::IRandomAccessStream^ stream)
        {
            return _strokeContainer->LoadAsync(stream);

        }).then([this] ()
        {
            Render();

        });
    }
}

void simpleInk::OnSelectAll()
{
    SelectAll(_strokeContainer->GetStrokes(), true);
    Render();
}

void simpleInk::OnDelete()
{
    // for most app scenarios it is sufficient to just call _strokeContainer->DeleteSelected();
    // for this sample we shall implement a slightly different scenario:
    //  - in case some strokes are selected we shall delete just the selected strokes
    //  - in case no strokes are selected we shall delete all strokes

    // obtain the strokes in the stroke container
    Windows::Foundation::Collections::IVectorView<Windows::UI::Input::Inking::InkStroke^>^ inkStrokes = _strokeContainer->GetStrokes();

    // check if any strokes are selected
    if (AnySelected(inkStrokes))
    {
        // some strokes are selected, delete only those
        _strokeContainer->DeleteSelected();
    }
    else
    {
        // no strokes are selected, delete all strokes

        // select all strokes
        SelectAll(inkStrokes, true);

        // delete all selected strokes
        _strokeContainer->DeleteSelected();
    }

    _recognitionText = ""; // ink has been erased, reset recognition text

    Render();
}

void simpleInk::OnChangeDrawingAttributes()
{
    // change color randomly
    // manipulate bitwise & 0xf0 to ensure we do not have entirely (or close to) white stroke
    _drawingAttributes->Color = Windows::UI::ColorHelper::FromArgb(255, rand() & 0xf0, rand() & 0xf0, rand() & 0xf0);

    // change size randomly
    float val = (rand() & 0x07) * 1.0f + 1.0f; // make sure we do not have 0.0f stroke width/height
    _drawingAttributes->Size = Windows::Foundation::Size(val, val);

    // update default drawing attributes
    _strokeBuilder->SetDefaultDrawingAttributes(_drawingAttributes);

    // update _inkBrush to reflect drawing color change
    CreateBrush(m_d2dContext.Get(), _drawingAttributes->Color, &_inkBrush);
}

void simpleInk::OnChangeRecognizer()
{
    // select next recognizer
    _recognizerId++;
    
    // make sure we are still in range
    if (_recognizerId >= _recognizerContainer->GetRecognizers()->Size)
    {
        _recognizerId = 0;
    }

    // set as a new default recognizer
    _recognizerContainer->SetDefaultRecognizer(_recognizerContainer->GetRecognizers()->GetAt(_recognizerId));

    // update the name of the recognizer on the screen
    // name is stored in _recognizerContainer->GetRecognizers()->GetAt(_recognizerId)->Name
    Render();
}

void simpleInk::OnRecognize()
{
    // there are three ways to do handwriting recognition, as defined
    // by the Windows::UI::Input::Inking::InkRecognitionTarget
    //  - (All) against all strokes
    //  - (Selected) against selected strokes
    //  - (Recent) against recent strokes, not recognized (InkStroke::Recognized == false)
    //
    // in this sample we shall demonstrate recognition against all and selected strokes
    // in case some strokes as selected we shall recognize only those
    // in case no strokes are selected we shall recognize against all strokes

    // obtain the strokes in the stroke container
    Windows::Foundation::Collections::IVectorView<Windows::UI::Input::Inking::InkStroke^>^ inkStrokes = _strokeContainer->GetStrokes();

    // NOTE: check that we have some ink to recognize before calling RecognizerContainer::RecognizeAsync()
    if (inkStrokes->Size > 0)
    {
        // loop through all strokes, check for selected flag
        Windows::UI::Input::Inking::InkRecognitionTarget target;
        if (AnySelected(inkStrokes))
        {
            _statusMessage = "Selected ink recognized as: ";
            target = Windows::UI::Input::Inking::InkRecognitionTarget::Selected;
        }
        else
        {
            _statusMessage = "Ink recognized as: ";
            target = Windows::UI::Input::Inking::InkRecognitionTarget::All;
        }
        
        // do handwriting recognition
        concurrency::task<Windows::Foundation::Collections::IVectorView<Windows::UI::Input::Inking::InkRecognitionResult^>^> recognizeTask(_recognizerContainer->RecognizeAsync(_strokeContainer, target));
        recognizeTask.then([this] (Windows::Foundation::Collections::IVectorView<Windows::UI::Input::Inking::InkRecognitionResult^>^ recognitionResults)
        {
            // update results in stroke container
            _strokeContainer->UpdateRecognitionResults(recognitionResults);
        
            // display recognition result
            _recognitionText = "";
            for (unsigned int i = 0; i < recognitionResults->Size; i++)
            {
                // obtain recognition results
                Windows::UI::Input::Inking::InkRecognitionResult^ recognitionResult = recognitionResults->GetAt(i);

                // obtain text candidates
                Windows::Foundation::Collections::IVectorView<Platform::String^>^ textCandidates = recognitionResult->GetTextCandidates();

                // we only need top candidate
                Platform::String^ topCandidate = textCandidates->GetAt(0);

                // update global recognition text with the top candidate
                _recognitionText += topCandidate + " ";
            }

            _statusMessage += _recognitionText;

            // render everything including the updated text
            Render();
        });
    }
}

void simpleInk::OnWindowSizeChanged(
    _In_ Windows::UI::Core::CoreWindow^ /*sender*/, 
    _In_ Windows::UI::Core::WindowSizeChangedEventArgs^ /*args*/)
{
    UpdateForWindowSizeChange();
    Render();
}

void simpleInk::OnLogicalDpiChanged(_In_ Platform::Object^ /*sender*/)
{
    SetDpi(static_cast<float>(Windows::Graphics::Display::DisplayProperties::LogicalDpi));
    Render();
}

void simpleInk::OnDisplayContentsInvalidated(_In_ Platform::Object^ /*sender*/)
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    Render();
}

void simpleInk::OnActivated(
    _In_ Windows::ApplicationModel::Core::CoreApplicationView^ /*applicationView*/,
    _In_ Windows::ApplicationModel::Activation::IActivatedEventArgs^ /*args*/)
{
    Windows::UI::Core::CoreWindow::GetForCurrentThread()->Activate();
}

#pragma endregion

#pragma region DirectXBase overrides

void simpleInk::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    DX::ThrowIfFailed(
        m_d2dFactory->CreateStrokeStyle(
            D2D1::StrokeStyleProperties(
                D2D1_CAP_STYLE_ROUND,
                D2D1_CAP_STYLE_ROUND,
                D2D1_CAP_STYLE_ROUND,
                D2D1_LINE_JOIN_ROUND,
                1.0f,
                D2D1_DASH_STYLE_SOLID,
                0.0f),
            nullptr,
            0,
            &_inkStyle));

    DX::ThrowIfFailed(
        m_d2dFactory->CreateStrokeStyle(
            D2D1::StrokeStyleProperties(
                D2D1_CAP_STYLE_ROUND,
                D2D1_CAP_STYLE_ROUND,
                D2D1_CAP_STYLE_ROUND,
                D2D1_LINE_JOIN_ROUND,
                1.0f,
                D2D1_DASH_STYLE_DASH,
                0.0f),
            nullptr,
            0,
            &_selectionStyle));
}

void simpleInk::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    // Create a DirectWrite text format object.
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(L"Verdana", nullptr, DWRITE_FONT_WEIGHT_NORMAL,
            DWRITE_FONT_STYLE_NORMAL, DWRITE_FONT_STRETCH_NORMAL, 20.0f, L"",
            &_eventTextFormat));
    // Center the text horizontally and vertically.
    _eventTextFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_LEADING);
    _eventTextFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_NEAR);

    CreateBrush(m_d2dContext.Get(), _backgroundColor, &_backgroundBrush);
    CreateBrush(m_d2dContext.Get(), _drawingAttributes->Color, &_inkBrush);
    CreateBrush(m_d2dContext.Get(), Windows::UI::Colors::Black, &_messageBrush);
    CreateBrush(m_d2dContext.Get(), Windows::UI::Colors::Goldenrod, &_selectionBrush);
}

void simpleInk::CreateWindowSizeDependentResources()
{
    _previousBuffer = nullptr;
    _currentBuffer = nullptr;

    DirectXBase::CreateWindowSizeDependentResources();

    // Retrieve buffers 0 and m_numBuffers-1: we will need them in live rendering mode.
    //
    // In live rendering mode we need to copy the content of the last buffer we
    // presented into the new buffer we are going to present. This is more
    // efficient than redoing the whole bezier render and the previous parts
    // of the live render.
    // The next buffer that will be presented is always buffer 0, and the last
    // buffer presented is always buffer m_numBuffers - 1.
    DX::ThrowIfFailed(
        m_swapChain->GetBuffer(
            0,
            __uuidof(ID3D11Texture2D),
            &_currentBuffer
            )
        );
    DX::ThrowIfFailed(
        m_swapChain->GetBuffer(
            m_numBuffers - 1,
            __uuidof(ID3D11Texture2D),
            &_previousBuffer
            )
        );
}

void simpleInk::Present()
{
    HRESULT hr = S_OK;

    DXGI_PRESENT_PARAMETERS parameters = {0};
    parameters.DirtyRectsCount = 0;
    parameters.pDirtyRects = nullptr;
    parameters.pScrollRect = nullptr;
    parameters.pScrollOffset = nullptr;

    // In the interest of minimizing lagging between the rendered stroke and the
    // position of the inking device, we shall use immediate-mode presentation
    // (Present(0, ...)) while in live rendering. This way we can always present
    // the most up to date inking sample.
    // Since immediate mode presentation drains the battery we use non-immediate
    // mode presentation (Present(1, ...)) when we are not in live rendering mode.
    switch (_renderingMode)
    {
    case Bezier:
        hr = m_swapChain->Present(1, 0);
        break;
    case Live:
        hr = m_swapChain->Present(0, 0);
        break;
    }

    // If the device was removed either by a disconnect or a driver upgrade, we 
    // must recreate all device resources.
    if (hr == DXGI_ERROR_DEVICE_REMOVED)
    {
        HandleDeviceLost();
    }
    else
    {
        DX::ThrowIfFailed(hr);
    }
}

void simpleInk::Render()
{
    m_d2dContext->BeginDraw();
    m_d2dContext->Clear(D2D1::ColorF(_backgroundColor.R/255.0f, _backgroundColor.G/255.0f, _backgroundColor.B/255.0f));
    m_d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());

    // Render recognizer name
    D2D1_RECT_F recoNameRect = {0, 0, 1000, 500};
    Platform::String^ name = _recognizerContainer->GetRecognizers()->GetAt(_recognizerId)->Name;
    m_d2dContext->DrawText(name->Begin(), name->Length(), _eventTextFormat.Get(), &recoNameRect, _messageBrush.Get());

    // Render status message, if any
    D2D1_RECT_F statusMessageRect = {0, 50, 1000, 500};
    m_d2dContext->DrawText(_statusMessage->Begin(), _statusMessage->Length(), _eventTextFormat.Get(), &statusMessageRect, _messageBrush.Get());

    // Render Beziers
    BezierRender();

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    Present();
}

#pragma endregion

#pragma region IFrameworkView overrides

void simpleInk::Initialize(
    _In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView)
{
    applicationView->Activated +=
        ref new Windows::Foundation::TypedEventHandler<
            Windows::ApplicationModel::Core::CoreApplicationView^, 
            Windows::ApplicationModel::Activation::IActivatedEventArgs^>(
                this, &simpleInk::OnActivated);
}

void simpleInk::SetWindow(_In_ Windows::UI::Core::CoreWindow^ window)
{
    // Setup event handlers
    Windows::Graphics::Display::DisplayProperties::LogicalDpiChanged += 
        ref new Windows::Graphics::Display::DisplayPropertiesEventHandler(
            this, &simpleInk::OnLogicalDpiChanged);

    Windows::Graphics::Display::DisplayProperties::DisplayContentsInvalidated += 
        ref new Windows::Graphics::Display::DisplayPropertiesEventHandler(
            this, &simpleInk::OnDisplayContentsInvalidated);

    window->SizeChanged += 
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Core::CoreWindow^, 
            Windows::UI::Core::WindowSizeChangedEventArgs^>(
                this, &simpleInk::OnWindowSizeChanged);

    window->CharacterReceived +=
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Core::CoreWindow^, 
            Windows::UI::Core::CharacterReceivedEventArgs^>(
                this, &simpleInk::OnCharacterReceived);

    window->PointerPressed +=
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Core::CoreWindow^, Windows::UI::Core::PointerEventArgs^>(
                this, &simpleInk::OnPointerPressed);

    window->PointerMoved +=
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Core::CoreWindow^, Windows::UI::Core::PointerEventArgs^>(
                this, &simpleInk::OnPointerMoved);

    window->PointerReleased +=
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Core::CoreWindow^, Windows::UI::Core::PointerEventArgs^>(
                this, &simpleInk::OnPointerReleased);

    // Set mouse cursor
    window->PointerCursor = ref new Windows::UI::Core::CoreCursor(
        Windows::UI::Core::CoreCursorType::Arrow, 0);

    DirectXBase::Initialize(
        Windows::UI::Core::CoreWindow::GetForCurrentThread(),
        Windows::Graphics::Display::DisplayProperties::LogicalDpi);
}

void simpleInk::Load(_In_ Platform::String^ /*entryPoint*/)
{
}

void simpleInk::Run()
{
    Render();
    Windows::UI::Core::CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(
        Windows::UI::Core::CoreProcessEventsOption::ProcessUntilQuit);
}

void simpleInk::Uninitialize()
{
}

#pragma endregion

//-----------------------------------------------------------------------------
// entry point
//-----------------------------------------------------------------------------

Windows::ApplicationModel::Core::IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new simpleInk();
}

// The main entry point for the sample program.
[Platform::MTAThread]
int main(Platform::Array<_In_ Platform::String^>^ /*args*/)
{
    auto directXAppSource = ref new DirectXAppSource();
    Windows::ApplicationModel::Core::CoreApplication::Run(directXAppSource);
    return 0;
}
