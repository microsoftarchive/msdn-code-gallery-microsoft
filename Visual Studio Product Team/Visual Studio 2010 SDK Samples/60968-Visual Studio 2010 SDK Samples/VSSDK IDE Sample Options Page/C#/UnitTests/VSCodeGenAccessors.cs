/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage.UnitTests
{
[System.Diagnostics.DebuggerStepThrough()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")]
internal class BaseAccessor {
    
    protected Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject m_privateObject;
    
    protected BaseAccessor(object target, Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType type) {
        m_privateObject = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(target, type);
    }
    
    protected BaseAccessor(Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType type) : 
            this(null, type) {
    }
    
    internal virtual object Target {
        get {
            return m_privateObject.Target;
        }
    }
    
    public override string ToString() {
        return this.Target.ToString();
    }
    
    public override bool Equals(object obj) {
        if (typeof(BaseAccessor).IsInstanceOfType(obj)) {
            obj = ((BaseAccessor)(obj)).Target;
        }
        return this.Target.Equals(obj);
    }
    
    public override int GetHashCode() {
        return this.Target.GetHashCode();
    }
}


[System.Diagnostics.DebuggerStepThrough()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")]
internal class Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPagePackageAccessor : BaseAccessor {
    
    protected static Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType m_privateType = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(typeof(global::Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPagePackageCS));
    
    internal Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPagePackageAccessor(global::Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPagePackageCS target) : 
            base(target, m_privateType) {
    }
    
    internal void Initialize() {
        object[] args = new object[0];
        m_privateObject.Invoke("Initialize", new System.Type[0], args);
    }
}
[System.Diagnostics.DebuggerStepThrough()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")]
internal class Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPageCustomAccessor : BaseAccessor {
    
    protected static Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType m_privateType = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(typeof(global::Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPageCustom));
    
    internal Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPageCustomAccessor(global::Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPageCustom target) : 
            base(target, m_privateType) {
    }
    
    internal string selectedImagePath {
        get {
            string ret = ((string)(m_privateObject.GetField("selectedImagePath")));
            return ret;
        }
        set {
            m_privateObject.SetField("selectedImagePath", value);
        }
    }
    
    internal global::System.Windows.Forms.IWin32Window Window {
        get {
            global::System.Windows.Forms.IWin32Window ret = ((global::System.Windows.Forms.IWin32Window)(m_privateObject.GetProperty("Window")));
            return ret;
        }
    }
}
[System.Diagnostics.DebuggerStepThrough()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")]
internal class Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsCompositeControlAccessor : BaseAccessor {
    
    protected static Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType m_privateType = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(typeof(global::Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsCompositeControl));
    
    internal Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsCompositeControlAccessor(global::Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsCompositeControl target) : 
            base(target, m_privateType) {
    }
    
    internal global::System.Windows.Forms.PictureBox pictureBox {
        get {
            global::System.Windows.Forms.PictureBox ret = ((global::System.Windows.Forms.PictureBox)(m_privateObject.GetField("pictureBox")));
            return ret;
        }
        set {
            m_privateObject.SetField("pictureBox", value);
        }
    }
    
    internal global::System.Windows.Forms.OpenFileDialog openImageFileDialog {
        get {
            global::System.Windows.Forms.OpenFileDialog ret = ((global::System.Windows.Forms.OpenFileDialog)(m_privateObject.GetField("openImageFileDialog")));
            return ret;
        }
        set {
            m_privateObject.SetField("openImageFileDialog", value);
        }
    }
    
    internal global::System.Windows.Forms.Button buttonChooseImage {
        get {
            global::System.Windows.Forms.Button ret = ((global::System.Windows.Forms.Button)(m_privateObject.GetField("buttonChooseImage")));
            return ret;
        }
        set {
            m_privateObject.SetField("buttonChooseImage", value);
        }
    }
    
    internal global::System.Windows.Forms.Button buttonClearImage {
        get {
            global::System.Windows.Forms.Button ret = ((global::System.Windows.Forms.Button)(m_privateObject.GetField("buttonClearImage")));
            return ret;
        }
        set {
            m_privateObject.SetField("buttonClearImage", value);
        }
    }
    
    internal global::Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPageCustom customOptionsPage {
        get {
            global::Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPageCustom ret = ((global::Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPageCustom)(m_privateObject.GetField("customOptionsPage")));
            return ret;
        }
        set {
            m_privateObject.SetField("customOptionsPage", value);
        }
    }
    
    internal global::System.ComponentModel.Container components {
        get {
            global::System.ComponentModel.Container ret = ((global::System.ComponentModel.Container)(m_privateObject.GetField("components")));
            return ret;
        }
        set {
            m_privateObject.SetField("components", value);
        }
    }
    
    internal void Dispose(bool disposing) {
        object[] args = new object[] {
                disposing};
        m_privateObject.Invoke("Dispose", new System.Type[] {
                    typeof(bool)}, args);
    }
    
    internal void InitializeComponent() {
        object[] args = new object[0];
        m_privateObject.Invoke("InitializeComponent", new System.Type[0], args);
    }
    
    internal void OnChooseImage(object sender, global::System.EventArgs e) {
        object[] args = new object[] {
                sender,
                e};
        m_privateObject.Invoke("OnChooseImage", new System.Type[] {
                    typeof(object),
                    typeof(global::System.EventArgs)}, args);
    }
    
    internal void OnClearImage(object sender, global::System.EventArgs e) {
        object[] args = new object[] {
                sender,
                e};
        m_privateObject.Invoke("OnClearImage", new System.Type[] {
                    typeof(object),
                    typeof(global::System.EventArgs)}, args);
    }
    
    internal void RefreshImage() {
        object[] args = new object[0];
        m_privateObject.Invoke("RefreshImage", new System.Type[0], args);
    }
}
[System.Diagnostics.DebuggerStepThrough()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")]
internal class Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPageGeneralAccessor : BaseAccessor {
    
    protected static Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType m_privateType = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(typeof(global::Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPageGeneral));
    
    internal Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPageGeneralAccessor(global::Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPageGeneral target) : 
            base(target, m_privateType) {
    }
    
    internal int optionCustomInteger {
        get {
            int ret = ((int)(m_privateObject.GetField("optionCustomInteger")));
            return ret;
        }
        set {
            m_privateObject.SetField("optionCustomInteger", value);
        }
    }
    
    internal global::System.Drawing.Size optionCustomSize {
        get {
            global::System.Drawing.Size ret = ((global::System.Drawing.Size)(m_privateObject.GetField("optionCustomSize")));
            return ret;
        }
        set {
            m_privateObject.SetField("optionCustomSize", value);
        }
    }
    
    internal string optionCustomString {
        get {
            string ret = ((string)(m_privateObject.GetField("optionCustomString")));
            return ret;
        }
        set {
            m_privateObject.SetField("optionCustomString", value);
        }
    }
    
    internal void OnActivate(global::System.ComponentModel.CancelEventArgs e) {
        object[] args = new object[] {
                e};
        m_privateObject.Invoke("OnActivate", new System.Type[] {
                    typeof(global::System.ComponentModel.CancelEventArgs)}, args);
    }
    
    internal void OnClosed(global::System.EventArgs e) {
        object[] args = new object[] {
                e};
        m_privateObject.Invoke("OnClosed", new System.Type[] {
                    typeof(global::System.EventArgs)}, args);
    }
    
    internal void OnDeactivate(global::System.ComponentModel.CancelEventArgs e) {
        object[] args = new object[] {
                e};
        m_privateObject.Invoke("OnDeactivate", new System.Type[] {
                    typeof(global::System.ComponentModel.CancelEventArgs)}, args);
    }
    
    internal void OnApply(Microsoft_VisualStudio_Shell_DialogPage_PageApplyEventArgsAccessor e) {
        object e_val_target = null;
        if ((e != null)) {
            e_val_target = e.Target;
        }
        object[] args = new object[] {
                e_val_target};
        Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType target = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType("Microsoft.VisualStudio.Shell", "Microsoft.VisualStudio.Shell.DialogPage+PageApplyEventArgs");
        m_privateObject.Invoke("OnApply", new System.Type[] {
                    target.ReferencedType}, args);
    }
}
[System.Diagnostics.DebuggerStepThrough()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")]
internal class Microsoft_VisualStudio_Shell_DialogPage_PageApplyEventArgsAccessor : BaseAccessor {
    
    protected static Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType m_privateType = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType("Microsoft.VisualStudio.Shell", "Microsoft.VisualStudio.Shell.DialogPage+PageApplyEventArgs");
    
    internal Microsoft_VisualStudio_Shell_DialogPage_PageApplyEventArgsAccessor(object target) : 
            base(target, m_privateType) {
    }
    
    internal global::Microsoft.VisualStudio.Shell.DialogPage.ApplyKind ApplyBehavior {
        get {
            global::Microsoft.VisualStudio.Shell.DialogPage.ApplyKind ret = ((global::Microsoft.VisualStudio.Shell.DialogPage.ApplyKind)(m_privateObject.GetProperty("ApplyBehavior")));
            return ret;
        }
        set {
            m_privateObject.SetProperty("ApplyBehavior", value);
        }
    }
    
    internal static global::System.EventArgs CreatePrivate() {
        object[] args = new object[0];
        Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject priv_obj = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject("Microsoft.VisualStudio.Shell", "Microsoft.VisualStudio.Shell.DialogPage+PageApplyEventArgs", new System.Type[0], args);
        return ((global::System.EventArgs)(priv_obj.Target));
    }
}
}
