//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;

namespace PasteXmlAsLinq {
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : Object, IDTExtensibility2, IDTCommandTarget {
        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
        public Connect() {
        }

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
        /// <param term='Application'>Root object of the host application.</param>
        /// <param term='ConnectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='AddInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object Application, ext_ConnectMode ConnectMode, object AddInInst, ref Array custom) {
            applicationObject = (DTE2)Application;
            addInInstance = (AddIn)AddInInst;
            if (ConnectMode == ext_ConnectMode.ext_cm_UISetup) {
                object[] contextGUIDS = new object[] { };
                Commands2 commands = (Commands2)applicationObject.Commands;

                try {
                    CommandBar menuBarCommandBar;
                    CommandBarControl toolsControl;
                    CommandBarPopup toolsPopup;
                    CommandBarControl commandBarControl;

                    //Add a command to the Commands collection:
                    Command command = commands.AddNamedCommand2(addInInstance, "PasteXmlAsLinq", "Paste XML as XElement", "Pastes the XML on the clipboard as C# Linq To Xml code", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

                    String editMenuName;

                    //Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
                    menuBarCommandBar = ((CommandBars)applicationObject.CommandBars)["MenuBar"];

                    try {

                        //  This code will take the culture, append on the name of the menuitem,
                        //  then add the command to the menu. You can find a list of all the top-level menus in the file
                        //  CommandBar.resx.
                        System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("PasteXmlAsLinq.CommandBar", System.Reflection.Assembly.GetExecutingAssembly());
                        System.Threading.Thread thread = System.Threading.Thread.CurrentThread;
                        System.Globalization.CultureInfo cultureInfo = thread.CurrentUICulture;
                        editMenuName = resourceManager.GetString(String.Concat(cultureInfo.TwoLetterISOLanguageName, "Edit"));
                        toolsControl = menuBarCommandBar.Controls[editMenuName];
                    }
                    catch (Exception) {
                        //  We tried to find a localized version of the word Edit, but one was not found.
                        //  Default to the en-US word, which may work for the current culture.
                        toolsControl = menuBarCommandBar.Controls["Edit"];
                    }

                    //Place the command on the edit menu.
                    toolsPopup = (CommandBarPopup)toolsControl;
                    int pasteControlIndex = 1;

                    //Find the paste control so that the new element can be added after it.
                    foreach (CommandBarControl commandBar in toolsPopup.CommandBar.Controls) {
                        if (String.Compare(commandBar.Caption, "&Paste", StringComparison.OrdinalIgnoreCase) == 0) {
                            pasteControlIndex = commandBar.Index + 1;
                            break;
                        }
                    }

                    //Find the appropriate command bar on the MenuBar command bar:
                    commandBarControl = (CommandBarControl)command.AddControl(toolsPopup.CommandBar, pasteControlIndex);
                }
                catch (Exception) {
                }
            }
        }

        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='RemoveMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode RemoveMode, ref Array custom) {
        }

        /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />		
        public void OnAddInsUpdate(ref Array custom) {
        }

        /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref Array custom) {
        }

        /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref Array custom) {
        }

        /// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
        /// <param term='CmdName'>The name of the command to determine state for.</param>
        /// <param term='NeededText'>Text that is needed for the command.</param>
        /// <param term='StatusOption'>The state of the command in the user interface.</param>
        /// <param term='CommandText'>Text requested by the NeededText parameter.</param>
        /// <seealso class='Exec' />
        public void QueryStatus(string CmdName, vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText) {
            if (NeededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone) {
                if (CmdName == "PasteXmlAsLinq.Connect.PasteXmlAsLinq") {
                    StatusOption = (vsCommandStatus)vsCommandStatus.vsCommandStatusUnsupported;
                    if (applicationObject.ActiveDocument != null) {
                        string xml = (string)Clipboard.GetDataObject().GetData(typeof(string));
                        if (xml != null && Converter.CanConvert(xml.Trim())) {
                            StatusOption = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                        }
                    }
                }
            }
        }

        /// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
        /// <param term='CmdName'>The name of the command to execute.</param>
        /// <param term='ExecuteOption'>Describes how the command should be run.</param>
        /// <param term='VariantIn'>Parameters passed from the caller to the command handler.</param>
        /// <param term='VariantOut'>Parameters passed from the command handler to the caller.</param>
        /// <param term='Handled'>Informs the caller if the command was handled or not.</param>
        /// <seealso class='Exec' />
        public void Exec(string CmdName, vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled) {
            Handled = false;
            if (ExecuteOption == vsCommandExecOption.vsCommandExecOptionDoDefault) {
                if (CmdName == "PasteXmlAsLinq.Connect.PasteXmlAsLinq") {
                    Document doc = applicationObject.ActiveDocument;
                    if (doc != null) {
                        string xml = (string)Clipboard.GetDataObject().GetData(typeof(string));
                        if (xml != null) {
                            try {
                                string code = Converter.Convert(xml);
                                TextSelection s = (TextSelection)doc.Selection;
                                s.Insert(code, (int)vsInsertFlags.vsInsertFlagsContainNewText);
                                applicationObject.ExecuteCommand("Edit.FormatSelection", "");
                            }
                            catch (Exception e) {
                                MessageBox.Show("Clipboard does not contain valid XML.\r\n" + e.Message, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    Handled = true;
                    return;
                }
            }
        }
        private DTE2 applicationObject;
        private AddIn addInInstance;
    }
}