using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.ComponentModelHost; //scomponentmodel

using Microsoft.VisualStudio.Editor; //editor
using Microsoft.VisualStudio.Text;  //text
using Microsoft.VisualStudio.Text.Operations;       //EditorOperations
using Microsoft.VisualStudio.Text.Editor; //Text
using Microsoft.VisualStudio.Utilities; // for content type
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.TextManager.Interop; //IVsTextView
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Microsoft.VisualStudio.OLE.Interop;


namespace Samples.EditorToolwindow
{
    /// <summary>
    /// This toolwindow hosts an editor inside. In order to successfully route commands and events to the editor, we have to implement
    /// IOleCommandTarget ourselves as our base implementation will not forward commands to the hosted editor.
    /// </summary>
    [Guid("e3e490c6-e7b5-425a-a44d-191d5c4a9e5b")]
    public class MyToolWindow : ToolWindowPane, IOleCommandTarget
    {
        IVsTextView viewAdapter;
        IVsTextBuffer bufferAdapter;

        IVsEditorAdaptersFactoryService m_EditorAdapterFactory;
        IOleServiceProvider oleServiceProvider;
        ITextBufferFactoryService bufferFactory;

        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public MyToolWindow() :
            base(null)
        {

            // Set the window title reading it from the resources.
            this.Caption = Resources.ToolWindowTitle;
            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;
        }

        /// <summary>
        /// This property returns the control that should be hosted in the Tool Window.
        /// In this case we return the wpf Text View Host for the editor
        /// </summary>
        override public object Content
        {
            get
            {
                return TextViewHost;
            }
        }

        private void InitializeEditor()
        {
            IComponentModel m_componentModel = (IComponentModel)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel));
            oleServiceProvider = (IOleServiceProvider)GetService(typeof(IOleServiceProvider));
            bufferFactory = m_componentModel.GetService<ITextBufferFactoryService>();

            m_EditorAdapterFactory = m_componentModel.GetService<IVsEditorAdaptersFactoryService>();
            bufferAdapter = m_EditorAdapterFactory.CreateVsTextBufferAdapter(oleServiceProvider, bufferFactory.TextContentType);
            int result = bufferAdapter.InitializeContent("ed", 2);
            viewAdapter = m_EditorAdapterFactory.CreateVsTextViewAdapter(oleServiceProvider);
            ((IVsWindowPane)viewAdapter).SetSite(oleServiceProvider);

            INITVIEW[] _initView = new INITVIEW[] { new INITVIEW() };
            _initView[0].fSelectionMargin = 0;
            _initView[0].fWidgetMargin = 0;
            _initView[0].fVirtualSpace = 0;
            _initView[0].fDragDropMove = 1;
            _initView[0].fVirtualSpace = 0;

            viewAdapter.Initialize(bufferAdapter as IVsTextLines, IntPtr.Zero, (uint)TextViewInitFlags.VIF_HSCROLL | (uint)TextViewInitFlags3.VIF_NO_HWND_SUPPORT, _initView);
        }

        private IWpfTextViewHost textViewHost;

        /// <summary>
        /// Gets the editor wpf host that we can use as the tool windows content.
        /// </summary>
        public IWpfTextViewHost TextViewHost
        {
            get
            {
                if (textViewHost == null)
                {
                    InitializeEditor();
                    IVsUserData data = viewAdapter as IVsUserData;
                    if (data != null)
                    {
                        Guid guid = Microsoft.VisualStudio.Editor.DefGuidList.guidIWpfTextViewHost;
                        object obj;
                        int hr = data.GetData(ref guid, out obj);
                        if ((hr == Microsoft.VisualStudio.VSConstants.S_OK) && obj != null && obj is IWpfTextViewHost)
                        {
                            textViewHost = obj as IWpfTextViewHost;
                        }

                    }
                }
                return textViewHost;
            }
        }

        public override void OnToolWindowCreated()
        {
            // Register key bindings to use in the editor
            var windowFrame = (IVsWindowFrame)Frame;
            Guid cmdUi = Microsoft.VisualStudio.VSConstants.GUID_TextEditorFactory;
            int hr = windowFrame.SetGuidProperty((int)__VSFPROPID.VSFPROPID_InheritKeyBindings, ref cmdUi);
            base.OnToolWindowCreated();
        }

        /// <summary>
        /// This override allows us to forward these messages to the editor instance as well
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected override bool PreProcessMessage(ref System.Windows.Forms.Message m)
        {
            if (this.TextViewHost != null)
            {
                // copy the Message into a MSG[] array, so we can pass
                // it along to the active core editor's IVsWindowPane.TranslateAccelerator
                MSG[] pMsg = new MSG[1];
                pMsg[0].hwnd = m.HWnd;
                pMsg[0].message = (uint)m.Msg;
                pMsg[0].wParam = m.WParam;
                pMsg[0].lParam = m.LParam;

                IVsWindowPane vsWindowPane = (IVsWindowPane)this.viewAdapter;
                if (0 == vsWindowPane.TranslateAccelerator(pMsg))
                    return true;
                else
                    return false;
            }

            return base.PreProcessMessage(ref m);
        }

        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            int hr = (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;

            // if a core editor object has the focus defer call to it's
            // IOleCommandTarget.QueryStatus implementation.
            if (this.viewAdapter != null)
            {
                IOleCommandTarget cmdTarget = (IOleCommandTarget)viewAdapter;
                hr = cmdTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            }

            return hr;
        }

        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            int hr = (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;

            // if a core editor object has the focus defer call to it's
            // IOleCommandTarget.QueryStatus implementation.
            if (this.viewAdapter != null)
            {
                IOleCommandTarget cmdTarget = (IOleCommandTarget)viewAdapter;
                hr = cmdTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
            }

            return hr;
        }
    }
}


