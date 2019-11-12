//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************


static class Helpers
{

    #region Pointer helpers

    public enum PointerEventType
    {
        None,
        Ink,
        Select,
        Erase
    }

    public static PointerEventType GetPointerEventType(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        PointerEventType type = PointerEventType.None;

        if (EventIsErase(e))
        {
            type = PointerEventType.Erase;
        }
        else if (EventIsInk(e))
        {
            type = PointerEventType.Ink;
        }
        else if (EventIsSelect(e))
        {
            type = PointerEventType.Select;
        }

        return type;
    }

    public static bool EventIsErase(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var pointerProperties = e.GetCurrentPoint(null).Properties;

        bool rval = false
            || (pointerProperties.IsEraser)
            || (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Pen && !pointerProperties.IsBarrelButtonPressed && pointerProperties.IsRightButtonPressed)
            || (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse && pointerProperties.IsRightButtonPressed);

        return rval;
    }

    public static bool EventIsInk(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var pointerProperties = e.GetCurrentPoint(null).Properties;

        bool rval = false
            || (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Pen && !pointerProperties.IsBarrelButtonPressed && !pointerProperties.IsRightButtonPressed)
            || (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse && pointerProperties.IsLeftButtonPressed && e.KeyModifiers == Windows.System.VirtualKeyModifiers.None);


        return rval;
    }

    public static bool EventIsSelect(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var pointerProperties = e.GetCurrentPoint(null).Properties;

        bool rval = false
            || (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Pen && pointerProperties.IsBarrelButtonPressed)
            || (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse && pointerProperties.IsLeftButtonPressed && e.KeyModifiers == Windows.System.VirtualKeyModifiers.Control);

        return rval;
    }

    #endregion Pointer helpers

    #region InkManager extension methods

    public static bool AnySelected(this Windows.UI.Input.Inking.InkManager inkManager)
    {
        foreach (var stroke in inkManager.GetStrokes())
        {
            if (stroke.Selected)
            {
                return true;
            }
        }
        return false;
    }

    public static void ClearSelection(this Windows.UI.Input.Inking.InkManager inkManager)
    {
        foreach (var stroke in inkManager.GetStrokes())
        {
            stroke.Selected = false;
        }
    }

    public static void SelectAll(this Windows.UI.Input.Inking.InkManager inkManager)
    {
        foreach (var stroke in inkManager.GetStrokes())
        {
            stroke.Selected = true;
        }
    }

    public static void DeleteAll(this Windows.UI.Input.Inking.InkManager inkManager)
    {
        inkManager.SelectAll();
        inkManager.DeleteSelected();
    }

    #endregion
}
