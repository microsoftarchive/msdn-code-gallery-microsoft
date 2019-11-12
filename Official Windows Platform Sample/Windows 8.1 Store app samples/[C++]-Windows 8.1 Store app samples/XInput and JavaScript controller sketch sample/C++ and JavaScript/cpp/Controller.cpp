//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "Controller.h"

using namespace Platform;
using namespace std;
using namespace GameController;

namespace
{
    const uint64 EnumerateTimeout = 2000;  // 2 seconds
}

Controller::Controller(uint32 index)
{
    m_index = index;
    m_lastEnumTime = ::GetTickCount64() - EnumerateTimeout;
}

Controller::~Controller()
{
}

void Controller::SetState(uint16 leftSpeed, uint16 rightSpeed)
{
    XINPUT_VIBRATION xinputVibration;
    xinputVibration.wLeftMotorSpeed = leftSpeed;
    xinputVibration.wRightMotorSpeed = rightSpeed;
    XInputSetState(m_index, &xinputVibration);
}

State Controller::GetState()
{
    // defaults to return controllerState that indicates controller is not connected
    State controllerState;
    controllerState.connected = false;

    // An app should avoid calling XInput functions every frame if there are no known devices connected
    // as initial device enumeration can slow down application performance.
    uint64 currentTime = ::GetTickCount64();
    if (!m_isControllerConnected && currentTime - m_lastEnumTime < EnumerateTimeout)
    {
        return controllerState;
    }
    m_lastEnumTime = currentTime;

    auto stateResult = XInputGetState(m_index, &m_xinputState);

    if (stateResult == ERROR_SUCCESS)
    {
        m_isControllerConnected = true;

        controllerState.connected = true;
        controllerState.controllerId = m_index;
        controllerState.packetNumber = m_xinputState.dwPacketNumber;
        controllerState.LeftTrigger = m_xinputState.Gamepad.bLeftTrigger;
        controllerState.RightTrigger = m_xinputState.Gamepad.bRightTrigger;
        controllerState.LeftThumbX = m_xinputState.Gamepad.sThumbLX;
        controllerState.LeftThumbY = m_xinputState.Gamepad.sThumbLY;
        controllerState.RightThumbX = m_xinputState.Gamepad.sThumbRX;
        controllerState.RightThumbY = m_xinputState.Gamepad.sThumbRY;

        auto buttons = m_xinputState.Gamepad.wButtons;
        controllerState.a = (buttons & XINPUT_GAMEPAD_A) != 0;
        controllerState.b = (buttons & XINPUT_GAMEPAD_B) != 0;
        controllerState.x = (buttons & XINPUT_GAMEPAD_X) != 0;
        controllerState.y = (buttons & XINPUT_GAMEPAD_Y) != 0;
        controllerState.dpad_up = (buttons & XINPUT_GAMEPAD_DPAD_UP) != 0;
        controllerState.dpad_down = (buttons & XINPUT_GAMEPAD_DPAD_DOWN) != 0;
        controllerState.dpad_left = (buttons & XINPUT_GAMEPAD_DPAD_LEFT) != 0;
        controllerState.dpad_right = (buttons & XINPUT_GAMEPAD_DPAD_RIGHT) != 0;
        controllerState.left_thumb = (buttons & XINPUT_GAMEPAD_LEFT_THUMB) != 0;
        controllerState.right_thumb = (buttons & XINPUT_GAMEPAD_RIGHT_THUMB) != 0;
        controllerState.left_shoulder = (buttons & XINPUT_GAMEPAD_LEFT_SHOULDER) != 0;
        controllerState.right_shoulder = (buttons & XINPUT_GAMEPAD_RIGHT_SHOULDER) != 0;
        controllerState.start = (buttons & XINPUT_GAMEPAD_START) != 0;
        controllerState.back = (buttons & XINPUT_GAMEPAD_BACK) != 0;
    }
    else
    {
        m_isControllerConnected = false;
    }

    return controllerState;
}