using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Debugger.Interop;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Debugger.SampleEngine
{
    // This class implements IDebugThread2 which represents a thread running in a program.
    class AD7Thread : IDebugThread2, IDebugThread100
    {
        readonly AD7Engine m_engine;
        readonly DebuggedThread m_debuggedThread;
        const string ThreadNameString = "Sample Engine Thread";
        private string m_threadDisplayName;
        private uint m_threadFlags;

        public AD7Thread(AD7Engine engine, DebuggedThread debuggedThread)
        {
            m_engine = engine;
            m_debuggedThread = debuggedThread;
        }      
        
        X86ThreadContext GetThreadContext()
        {
            X86ThreadContext threadContext = m_engine.DebuggedProcess.GetThreadContext(m_debuggedThread.Handle);

            return threadContext;
        }

        string GetCurrentLocation(bool fIncludeModuleName)
        {
            uint ip = GetThreadContext().eip;
            string location = m_engine.GetAddressDescription(ip);

            return location;
        }

        internal DebuggedThread GetDebuggedThread()
        {
            return m_debuggedThread;
        }

        #region IDebugThread2 Members

        // Determines whether the next statement can be set to the given stack frame and code context.
        // The sample debug engine does not support set next statement, so S_FALSE is returned.
        int IDebugThread2.CanSetNextStatement(IDebugStackFrame2 stackFrame, IDebugCodeContext2 codeContext)
        {
            return Constants.S_FALSE; 
        }

        // Retrieves a list of the stack frames for this thread.
        // For the sample engine, enumerating the stack frames requires walking the callstack in the debuggee for this thread
        // and coverting that to an implementation of IEnumDebugFrameInfo2. 
        // Real engines will most likely want to cache this information to avoid recomputing it each time it is asked for,
        // and or construct it on demand instead of walking the entire stack.
        int IDebugThread2.EnumFrameInfo(enum_FRAMEINFO_FLAGS dwFieldSpec, uint nRadix, out IEnumDebugFrameInfo2 enumObject)
        {
            // Ask the lower-level to perform a stack walk on this thread
            m_engine.DebuggedProcess.DoStackWalk(this.m_debuggedThread);
            enumObject = null;

            try
            {
                System.Collections.Generic.List<X86ThreadContext> stackFrames = this.m_debuggedThread.StackFrames;
                int numStackFrames = stackFrames.Count;
                FRAMEINFO[] frameInfoArray;

                if (numStackFrames == 0)
                {
                    // failed to walk any frames. Only return the top frame.
                    frameInfoArray = new FRAMEINFO[1];
                    AD7StackFrame frame = new AD7StackFrame(m_engine, this, GetThreadContext());
                    frame.SetFrameInfo(dwFieldSpec, out frameInfoArray[0]);
                }
                else
                {
                    frameInfoArray = new FRAMEINFO[numStackFrames];

                    for (int i = 0; i < numStackFrames; i++)
                    {
                        AD7StackFrame frame = new AD7StackFrame(m_engine, this, stackFrames[i]);
                        frame.SetFrameInfo(dwFieldSpec, out frameInfoArray[i]);
                    }
                }

                enumObject = new AD7FrameInfoEnum(frameInfoArray);
                return Constants.S_OK;
            }
            catch (ComponentException e)
            {
                return e.HResult;
            }
            catch (Exception e)
            {
                return EngineUtils.UnexpectedException(e);
            } 
        }

        // Get the name of the thread. For the sample engine, the name of the thread is always "Sample Engine Thread"
        int IDebugThread2.GetName(out string threadName)
        {
            threadName = ThreadNameString;
            return Constants.S_OK;
        }

        // Return the program that this thread belongs to.
        int IDebugThread2.GetProgram(out IDebugProgram2 program)
        {
            program = m_engine;
            return Constants.S_OK;
        }

        // Gets the system thread identifier.
        int IDebugThread2.GetThreadId(out uint threadId)
        {
            threadId = (uint)m_debuggedThread.Id;
            return Constants.S_OK;
        }

        // Gets properties that describe a thread.
        int IDebugThread2.GetThreadProperties(enum_THREADPROPERTY_FIELDS dwFields, THREADPROPERTIES[] propertiesArray)
        {
            try
            {
                THREADPROPERTIES props = new THREADPROPERTIES();

                if ((dwFields & enum_THREADPROPERTY_FIELDS.TPF_ID) != 0)
                {
                    props.dwThreadId = (uint)m_debuggedThread.Id;
                    props.dwFields |= enum_THREADPROPERTY_FIELDS.TPF_ID;
                }
                if ((dwFields & enum_THREADPROPERTY_FIELDS.TPF_SUSPENDCOUNT) != 0) 
                {
                    // sample debug engine doesn't support suspending threads
                    props.dwFields |= enum_THREADPROPERTY_FIELDS.TPF_SUSPENDCOUNT;
                }
                if ((dwFields & enum_THREADPROPERTY_FIELDS.TPF_STATE) != 0) 
                {
                    props.dwThreadState = (uint)enum_THREADSTATE.THREADSTATE_RUNNING;
                    props.dwFields |= enum_THREADPROPERTY_FIELDS.TPF_STATE;
                }
                if ((dwFields & enum_THREADPROPERTY_FIELDS.TPF_PRIORITY) != 0) 
                {
                    props.bstrPriority = "Normal";
                    props.dwFields |= enum_THREADPROPERTY_FIELDS.TPF_PRIORITY;
                }
                if ((dwFields & enum_THREADPROPERTY_FIELDS.TPF_NAME) != 0)
                {
                    props.bstrName = ThreadNameString;
                    props.dwFields |= enum_THREADPROPERTY_FIELDS.TPF_NAME;
                }
                if ((dwFields & enum_THREADPROPERTY_FIELDS.TPF_LOCATION) != 0)
                {
                    props.bstrLocation = GetCurrentLocation(true);
                    props.dwFields |= enum_THREADPROPERTY_FIELDS.TPF_LOCATION;
                }

                return Constants.S_OK;
            }
            catch (ComponentException e)
            {
                return e.HResult;
            }
            catch (Exception e)
            {
                return EngineUtils.UnexpectedException(e);
            }
        }

        // Resume a thread.
        // This is called when the user chooses "Unfreeze" from the threads window when a thread has previously been frozen.
        int IDebugThread2.Resume(out uint suspendCount)
        {
            // The sample debug engine doesn't support suspending/resuming threads
            suspendCount = 0;
            return Constants.E_NOTIMPL;
        }

        // Sets the next statement to the given stack frame and code context.
        // The sample debug engine doesn't support set next statment
        int IDebugThread2.SetNextStatement(IDebugStackFrame2 stackFrame, IDebugCodeContext2 codeContext)
        {
            return Constants.E_NOTIMPL;
        }

        // suspend a thread.
        // This is called when the user chooses "Freeze" from the threads window
        int IDebugThread2.Suspend(out uint suspendCount)
        {
            // The sample debug engine doesn't support suspending/resuming threads
            suspendCount = 0;
            return Constants.E_NOTIMPL;
        }

        #endregion

        #region IDebugThread100 Members

        int IDebugThread100.SetThreadDisplayName(string name)
        {
            // Not necessary to implement in the debug engine. Instead
            // it is implemented in the SDM.
            return Constants.E_NOTIMPL;
        }

        int IDebugThread100.GetThreadDisplayName(out string name)
        {
            // Not necessary to implement in the debug engine. Instead
            // it is implemented in the SDM, which calls GetThreadProperties100()
            name = "";
            return Constants.E_NOTIMPL;
        }

        // Returns whether this thread can be used to do function/property evaluation.
        int IDebugThread100.CanDoFuncEval()
        {
            return Constants.S_FALSE;
        }

        int IDebugThread100.SetFlags(uint flags)
        {
            // Not necessary to implement in the debug engine. Instead
            // it is implemented in the SDM.
            return Constants.E_NOTIMPL;
        }

        int IDebugThread100.GetFlags(out uint flags)
        {
            // Not necessary to implement in the debug engine. Instead
            // it is implemented in the SDM.
            flags = 0;
            return Constants.E_NOTIMPL;
        }

        int IDebugThread100.GetThreadProperties100(uint dwFields, THREADPROPERTIES100[] props)
        {
            int hRes = Constants.S_OK;

            // Invoke GetThreadProperties to get the VS7/8/9 properties
            THREADPROPERTIES[] props90 = new THREADPROPERTIES[1];
            enum_THREADPROPERTY_FIELDS dwFields90 = (enum_THREADPROPERTY_FIELDS)(dwFields & 0x3f);
            hRes = ((IDebugThread2)this).GetThreadProperties(dwFields90, props90);
            props[0].bstrLocation = props90[0].bstrLocation;
            props[0].bstrName = props90[0].bstrName;
            props[0].bstrPriority = props90[0].bstrPriority;
            props[0].dwFields = (uint)props90[0].dwFields;
            props[0].dwSuspendCount = props90[0].dwSuspendCount;
            props[0].dwThreadId = props90[0].dwThreadId;
            props[0].dwThreadState = props90[0].dwThreadState;

            // Populate the new fields
            if (hRes == Constants.S_OK && dwFields != (uint)dwFields90)
            {
                if ((dwFields & (uint)enum_THREADPROPERTY_FIELDS100.TPF100_DISPLAY_NAME) != 0)
                {
                    // Thread display name is being requested
                    props[0].bstrDisplayName = "<thread name from sample engine>";
                    props[0].dwFields |= (uint)enum_THREADPROPERTY_FIELDS100.TPF100_DISPLAY_NAME;

                    // Give this display name a higher priority than the default (0)
                    // so that it will actually be displayed
                    props[0].DisplayNamePriority = 10;
                    props[0].dwFields |= (uint)enum_THREADPROPERTY_FIELDS100.TPF100_DISPLAY_NAME_PRIORITY;
                }

                if ((dwFields & (uint)enum_THREADPROPERTY_FIELDS100.TPF100_CATEGORY) != 0)
                {
                    // Thread category is being requested
                    props[0].dwThreadCategory = 0;
                    props[0].dwFields |= (uint)enum_THREADPROPERTY_FIELDS100.TPF100_CATEGORY;
                }

                if ((dwFields & (uint)enum_THREADPROPERTY_FIELDS100.TPF100_AFFINITY) != 0)
                {
                    // Thread cpu affinity is being requested
                    props[0].AffinityMask = 0;
                    props[0].dwFields |= (uint)enum_THREADPROPERTY_FIELDS100.TPF100_AFFINITY;
                }

                if ((dwFields & (uint)enum_THREADPROPERTY_FIELDS100.TPF100_PRIORITY_ID) != 0)
                {
                    // Thread display name is being requested
                    props[0].priorityId = 0;
                    props[0].dwFields |= (uint)enum_THREADPROPERTY_FIELDS100.TPF100_PRIORITY_ID;
                }

            }

            return hRes;
        }

        #endregion

        #region Uncalled interface methods
        // These methods are not currently called by the Visual Studio debugger, so they don't need to be implemented

        int IDebugThread2.GetLogicalThread(IDebugStackFrame2 stackFrame, out IDebugLogicalThread2 logicalThread)
        {
            Debug.Fail("This function is not called by the debugger");

            logicalThread = null;
            return Constants.E_NOTIMPL;
        }

        int IDebugThread2.SetThreadName(string name)
        {
            Debug.Fail("This function is not called by the debugger");

            return Constants.E_NOTIMPL;
        }

        #endregion
    }
}
