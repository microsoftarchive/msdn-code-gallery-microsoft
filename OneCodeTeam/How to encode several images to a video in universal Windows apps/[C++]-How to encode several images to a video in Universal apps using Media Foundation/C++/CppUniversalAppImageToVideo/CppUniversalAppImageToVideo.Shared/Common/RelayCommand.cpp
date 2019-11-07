//
// RelayCommand.cpp
// Implementation of the RelayCommand and associated classes
//

#include "pch.h"
#include "RelayCommand.h"
#include "NavigationHelper.h"

using namespace CppUniversalAppImageToVideo::Common;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::System;
using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Navigation;

/// <summary>
/// Determines whether this <see cref="RelayCommand"/> can execute in its current state.
/// </summary>
/// <param name="parameter">
/// Data used by the command. If the command does not require data to be passed, this object can be set to null.
/// </param>
/// <returns>true if this command can be executed; otherwise, false.</returns>
bool RelayCommand::CanExecute(Object^ parameter)
{
	return (_canExecuteCallback) (parameter);
}

/// <summary>
/// Executes the <see cref="RelayCommand"/> on the current command target.
/// </summary>
/// <param name="parameter">
/// Data used by the command. If the command does not require data to be passed, this object can be set to null.
/// </param>
void RelayCommand::Execute(Object^ parameter)
{
	(_executeCallback) (parameter);
}

/// <summary>
/// Method used to raise the <see cref="CanExecuteChanged"/> event
/// to indicate that the return value of the <see cref="CanExecute"/>
/// method has changed.
/// </summary>
void RelayCommand::RaiseCanExecuteChanged()
{
	CanExecuteChanged(this, nullptr);
}

/// <summary>
/// RelayCommand Class Destructor.
/// </summary>
RelayCommand::~RelayCommand()
{
	_canExecuteCallback = nullptr;
	_executeCallback = nullptr;
};

/// <summary>
/// Creates a new command that can always execute.
/// </summary>
/// <param name="canExecuteCallback">The execution status logic.</param>
/// <param name="executeCallback">The execution logic.</param>
RelayCommand::RelayCommand(std::function<bool(Platform::Object^)> canExecuteCallback,
	std::function<void(Platform::Object^)> executeCallback) :
	_canExecuteCallback(canExecuteCallback),
	_executeCallback(executeCallback)
	{
	}
