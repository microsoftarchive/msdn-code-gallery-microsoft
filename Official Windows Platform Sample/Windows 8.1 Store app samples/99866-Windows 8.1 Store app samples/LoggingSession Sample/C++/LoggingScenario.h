//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// LoggingScenario.h
// Declaration of the LoggingScenario class
//

#include <agents.h>
#include <ppltasks.h>

#pragma once
using namespace std;
using namespace concurrency;
using namespace Windows::Storage;
using namespace Windows::Foundation::Collections;
using namespace Windows::Foundation::Diagnostics;

namespace SDKSample
{
	namespace LoggingSession
	{
        // LoggingScenario tells the UI what's happening by 
        // using the following enums. 
        enum LoggingScenarioEventType
        {
            BusyStatusChanged,
            LogFileGenerated,
            LoggingEnabledDisabled
        };

        ref class LoggingScenarioEventArgs sealed
        {
        private:

            LoggingScenarioEventType _type;
            std::wstring _logFileFullPath;
            bool _enabled;

        internal:

            LoggingScenarioEventArgs(LoggingScenarioEventType type)
            {
                _type = type;
            }

            LoggingScenarioEventArgs(LoggingScenarioEventType type, const std::wstring& logFileFullPath)
            {
                _type = type;
                _logFileFullPath = logFileFullPath;
            }

            LoggingScenarioEventArgs(bool enabled)
            {
                _type = LoggingEnabledDisabled;
                _enabled = enabled;
            }

            property LoggingScenarioEventType Type
            {
                LoggingScenarioEventType get()
                {
                    return _type;
                }
            }

            property std::wstring LogFileFullPath
            {
                std::wstring get()
                {
                    return _logFileFullPath;
                }
            }

            property bool Enabled
            {
                bool get()
                {
                    return _enabled;
                }
            }
        };

        ref class LoggingScenario;
        delegate void StatusChangedHandler(LoggingScenario^ sender, LoggingScenarioEventArgs^ args);

		ref class LoggingScenario sealed
		{

        internal:

            LoggingScenario();
            event StatusChangedHandler^ StatusChanged;

            static property Platform::String^ DEFAULT_SESSION_NAME
            {
                Platform::String^ get()
                {
                    return ref new Platform::String(L"AppSession1");
                }
            }

            static property Platform::String^ DEFAULT_CHANNEL_NAME
            {
                Platform::String^ get()
                {
                    return ref new Platform::String(L"AppChannel1");
                }
            }

            static property Platform::String^ OUR_SAMPLE_APP_LOG_FILE_FOLDER_NAME
            {
                Platform::String^ get()
                {
                    return ref new Platform::String(L"OurAppLogFiles");
                }
            }

            static property Platform::String^ LOG_FILE_BASE_FILE_NAME
            {
                Platform::String^ get()
                {
                    return ref new Platform::String(L"OurAppLog");
                }
            }

            static property Platform::String^ LOGGING_ENABLED_SETTING_KEY_NAME
            {
                Platform::String^ get()
                {
                    return ref new Platform::String(L"LoggingEnabled");
                }
            }
           
            static void SetAppLocalSettingsValue(Platform::String^ key, Platform::Object^ value)
            {
                if (ApplicationData::Current->LocalSettings->Values->HasKey(key))
                {
                    ApplicationData::Current->LocalSettings->Values->Remove(key);
                }
                ApplicationData::Current->LocalSettings->Values->Insert(key, value);
            }

            static bool IsAppLocalSettingsValue(Platform::String^ key)
            {
                return ApplicationData::Current->LocalSettings->Values->HasKey(key);
            }

            static Platform::Object^ GetAppLocalSettingsValueAsObject(Platform::String^ key)
            {
                return ApplicationData::Current->LocalSettings->Values->Lookup(key);
            }

            static bool GetAppLocalSettingsValueAsBool(Platform::String^ key)
            {
                Platform::Object^ obj = GetAppLocalSettingsValueAsObject(key);
                return safe_cast<bool>(obj);
            }

            static Platform::String^ GetAppLocalSettingsValueAsString(Platform::String^ key)
            {
                Platform::Object^ obj = GetAppLocalSettingsValueAsObject(key);
                return safe_cast<Platform::String^>(obj);
            }

            static LoggingScenario^ GetLoggingScenarioSingleton()
            {
                if (loggingScenario == nullptr)
                {
                    loggingScenario = ref new LoggingScenario();
                }
                return loggingScenario;
            }

            property bool IsBusy
            {
                bool get() { return _isBusy; }
            private: 
                void set(bool value)
                {
                    _isBusy = value;
                    StatusChanged(this, ref new LoggingScenarioEventArgs(LoggingScenarioEventType::BusyStatusChanged));
                }
            }

            property int LogFileGeneratedCount
            {
                int get() { return _logFileGeneratedCount; }
            }            

            property bool LoggingEnabled
            {
                bool get() { return _session != nullptr; }
            }

            bool ToggleLoggingEnabledDisabled();
            void StartLogging();

            void PrepareToSuspend();
            void ResumeLoggingIfApplicable();

            task<void> DoScenarioAsync();

        private:

            ~LoggingScenario();

		private:

			static LoggingScenario^ loggingScenario;
			Windows::Foundation::Diagnostics::LoggingSession^ _session;
			Windows::Foundation::Diagnostics::LoggingChannel^ _channel;

            long _logFileGeneratedCount;
            bool _isBusy;

			// The following are set when a LoggingChannel 
			// calls OnChannelLoggingEnabled:
			bool _isChannelEnabled;
			LoggingLevel _channelLoggingLevel;
			Windows::Foundation::EventRegistrationToken _onChannelLoggingEnabledToken;
            void OnChannelLoggingEnabled(ILoggingChannel ^sender, Platform::Object ^args);

            task<StorageFile^> SaveLogInMemoryToFileAsync();
            void OnAppSuspending(Platform::Object ^sender, Windows::ApplicationModel::SuspendingEventArgs ^e);
            void OnAppResuming(Platform::Object ^sender, Platform::Object ^args);
        };

        // Create a time stamp to append to log file names so 
        // each file has a unique name. The format is:
        //     YYMMDD-hhmmssMMM 
        // where
        //     YY == year
        //     MM == month
        //     DD == day
        //     hh == hours
        //     mm == minutes
        //     ss == seconds
        //     MMM == milliseconds
        inline std::wstring GetTimeStamp()
        {
            std::wstring result;
            SYSTEMTIME timeNow;
            GetLocalTime(&timeNow);
            result.resize(100);
            HRESULT hr = StringCchPrintf(&result[0],
                result.size(),
                L"%02u%02u%02u-%02u%02u%02u%03u",
                timeNow.wYear - 2000,
                timeNow.wMonth,
                timeNow.wDay,
                timeNow.wHour,
                timeNow.wMinute,
                timeNow.wSecond,
                timeNow.wMilliseconds);
            if (FAILED(hr))
            {
                result = L"GetTimeStamp-FAILURE";
            }
            return result;
        }

        // Create a task which delays for a specified duration before executing a function.
        template <typename Func>
        auto create_delayed_task(std::chrono::milliseconds delay, Func func, concurrency::cancellation_token token = concurrency::cancellation_token::none()) -> decltype(create_task(func))
        {
            concurrency::task_completion_event<void> tce;

            auto pTimer = new concurrency::timer<int>(static_cast<int>(delay.count()), 0, NULL, false);
            auto pCallback = new concurrency::call<int>([tce](int) {
                tce.set();
            });

            pTimer->link_target(pCallback);
            pTimer->start();

            return create_task(tce).then([pCallback, pTimer]() {
                delete pCallback;
                delete pTimer;
            }).then(func, token);
        }
	}
}
