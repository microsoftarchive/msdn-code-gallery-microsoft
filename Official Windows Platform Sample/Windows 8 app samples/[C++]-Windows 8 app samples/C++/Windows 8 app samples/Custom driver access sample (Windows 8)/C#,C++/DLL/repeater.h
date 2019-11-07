#include <strsafe.h>
#include <assert.h>

#include <functional>
#include <mutex>

#include <ppltasks.h>

#pragma once

namespace Samples {
    namespace Devices {

        // Implementation details
        namespace Details {
            // The type of task the StartTaskFunction() returns.  The bool indicates whether the next 
            // iteration should start.
            typedef concurrency::task<bool> RepeaterTaskType;

            // The type of function the creator provides to run each iteration.  It returns a task
            // which the repeater chains onto.
            typedef std::function<RepeaterTaskType (concurrency::cancellation_token, concurrency::task_continuation_context)> StartRepeaterTaskFunction;
            
            // The type of the function the creator calls once the repeater is stopped.
            typedef std::function<void(void)> RepeaterStoppedFunction;
        }

        // The repeater class performs a task over and over again.  It can be used 
        // to issue a device I/O repeatedly - for example reading events from the 
        // device.
        class Repeater {
        public:

            // The state of the repeater.  It is initialized in the stopped state.
            enum class RepeaterState {
                Stopped,
                Running,
                Stopping
            };

        private:
            // The function to call when starting each iteration of the repeater
            Details::StartRepeaterTaskFunction m_StartTaskFunction;

            // The source of cancellation tokens passed to the start task function
            concurrency::cancellation_token_source m_Canceller;

            // Whether the repeater is active, stopping or stopped.
            RepeaterState m_State;

            // Lock to protect the state.  The task chains are created under this 
            // lock, but they execute without the lock.
            std::mutex m_Lock;
            
            // shorthand for our mutex's lock type (mlt)
            typedef std::unique_lock<std::mutex> mlt;
            
            // An optional function to call once the repeater is stopped
            // TODO: should Stop return a task that the caller can .then?
            Details::RepeaterStoppedFunction m_RepeaterStoppedFunction;

        public:
            Repeater() : m_StartTaskFunction(nullptr), 
                         m_State(RepeaterState::Stopped), 
                         m_RepeaterStoppedFunction(nullptr)
            {
                return;
            }

            ~Repeater() 
            {
                // TODO: Move the core of the repeater to an allocated
                //       object that i can let run down on its own on destruction.  For now
                //       !! don't delete a repeater that's still running !!
                assert(m_State == RepeaterState::Stopped);
            }

            // Starts the repeater.  Each iteration calls StartTaskFunction to kick off a chain of tasks.
            // The repeater then adds a task to the end of the chain that fires off the next iteration
            // Caller can provide a context that plumbs through to the start functions, or use current
            void Start(
                Details::StartRepeaterTaskFunction StartTaskFunction, 
                concurrency::task_continuation_context Context = concurrency::task_continuation_context::use_current()
                )
            {
                // lock the mutex
                mlt lock(m_Lock);

                assert(m_State == RepeaterState::Stopped);

                if (m_State == RepeaterState::Stopped) {
                    m_StartTaskFunction = StartTaskFunction;
                    m_RepeaterStoppedFunction = nullptr;

                    // Get a new canceller before we start the tasks
                    auto c = concurrency::cancellation_token_source();
                    m_Canceller = std::move(c);

                    // Update task state before starting the task so that it can
                    // restart itself properly.
                    m_State = RepeaterState::Running;
                    StartTask(Context);
                }
            }

            // Stop the repeater.  This will cancel the current task (caller's task 
            // needs to have registered for cancellation).  Caller can optionally provide a 
            // callback that the last task will invoke.
            bool Stop(Details::RepeaterStoppedFunction* callback = nullptr)
            {
                mlt lock(m_Lock);
                bool active;

                if (m_State == RepeaterState::Running) {
                    m_State = RepeaterState::Stopping;
                    m_Canceller.cancel();

                    active = true;
                } else if (m_State == RepeaterState::Stopping) {
                    active = true;
                } else {
                    active = false;
                }

                if (callback) {
                    if (active == true) {
                        m_RepeaterStoppedFunction = *callback;
                        lock.release();
                    }
                }

                return active;
            }

            // The current state of the repeater.
            RepeaterState GetState(void)
            {
                return m_State;
            }

        private:

            // Creates the task chain, and appends the restart task to it
            void StartTask(concurrency::task_continuation_context context) {
                // start the caller's task and add a restart task onto 
                // the end.
                m_StartTaskFunction(m_Canceller.get_token(), context).
                    then([this, context](bool b) {
                        RestartTask(context, b);
                    }
                );
            }

            // The restart task mentioned above.  If the repeater is still running & 
            // the previous iteration didn't return false, it calls StartTask() to 
            // fire off the next iteration.  Otherwise it handles rundown.
            void RestartTask(concurrency::task_continuation_context context, bool Continue) {
                // Check and see if the task is still running
                mlt lock(m_Lock);

                if (Continue == false) {
                    m_State = RepeaterState::Stopping;
                }
                if (m_State == RepeaterState::Running) {
                    StartTask(context);
                } else if (m_State == RepeaterState::Stopping) {
                    m_State = RepeaterState::Stopped;
                    if (m_RepeaterStoppedFunction != nullptr) {
                        Details::RepeaterStoppedFunction callback = m_RepeaterStoppedFunction;
                        m_RepeaterStoppedFunction = nullptr;
                        lock.release();
                        callback();
                    }
                }
            }
        };


    }
}
