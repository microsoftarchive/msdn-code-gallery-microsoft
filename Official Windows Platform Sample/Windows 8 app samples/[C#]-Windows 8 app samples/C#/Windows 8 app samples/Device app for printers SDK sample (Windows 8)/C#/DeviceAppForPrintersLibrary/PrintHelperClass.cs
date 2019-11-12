// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Data.Xml.Dom;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;

// Printer Extension types library
using Microsoft.Samples.Printing.PrinterExtension;
using Microsoft.Samples.Printing.PrinterExtension.Types;

namespace Microsoft.Samples.Printers.Extensions
{
    /// <summary>
    /// This WinMD library works with the print ticket from the printer extension context
    /// passed in from JavaScript.  It looks up available and
    /// currently selected options for print features, and surfaces this information
    /// to JavaScript as strings or arrays of strings.  It's also used for setting options.
    /// </summary>
    public sealed class PrintHelperClass
    {
        private enum OptionInfoType
        {
            DisplayName,
            Index
        }

        /// <summary>
        /// List of capabilities that this printer queue supports.
        /// It is very important that the print capabilities is retrieved only as many times as absolutely required because:
        ///     1. It is an expensive operation in terms of time.
        ///     2. The returning capabilities could be in a different order as before.
        /// </summary>
        private IPrintSchemaCapabilities capabilities;

        /// <summary>
        /// The printer extension context of this printer queue.
        /// </summary>
        private PrinterExtensionContext context;

        /// <summary>
        /// A dictionary of the IPrintSchema options.
        /// </summary>
        private Dictionary<string, List<IPrintSchemaOption>> featureOptions = new Dictionary<string, List<IPrintSchemaOption>>();

        /// <summary>
        /// The IPrinterQueue object of the printer.
        /// </summary>
        private IPrinterQueue printerQueue;

        /// <summary>
        /// The bidi response received from an ink query.
        /// </summary>
        private string bidiResponse;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">The PrinterExtensionContext from the app.</param>
        public PrintHelperClass(Object context)
        {
            this.context = new PrinterExtensionContext(context);
            printerQueue = this.context.Queue;
        }

        /// <summary>
        /// Get a list of options for a specified feature.
        /// </summary>
        /// <param name="feature">The feature whose options will be retrieved</param>
        /// <returns></returns>
        private List<IPrintSchemaOption> GetCachedFeatureOptions(string feature)
        {
            if (false == featureOptions.ContainsKey(feature))
            {
                // The first time this feature's options are retrieved, cache a copy of the list
                featureOptions[feature] = Capabilities.GetOptions(Capabilities.GetFeatureByKeyName(feature)).ToList();
            }
            return featureOptions[feature];
        }

        /// <summary>
        /// Returns the capabilities for the context's print ticket.  This method only looks up the
        /// capabilities the first time it's called, and it returns the cached capabilities on subsequent calls
        /// to avoid making a repeated expensive call to retrieve the capabilities from the print ticket.
        /// </summary>
        private IPrintSchemaCapabilities Capabilities
        {
            get
            {
                if (null != capabilities)
                {
                    return capabilities;
                }
                else
                {
                    if (null == context || null == context.Ticket)
                        return null;

                    capabilities = context.Ticket.GetCapabilities();
                    return capabilities;
                }
            }
        }

        /// <summary>
        /// This is a wrapper for the asynchronous operation of querying bidi.
        /// </summary>
        /// <returns>IAsyncOperation so that this method is:  
        ///     1) awaitable in C#.
        ///     2) consumed as a WinJS.promise on which we can then use .then() or .done().</returns>
        public IAsyncOperation<string> GetInkLevelAsync()
        {
            return AsyncInfo.Run(async (o) =>
            {
                // This allows the app to wait until a certain event is fired before continuing on.
                // To prevent the waiting from locking up the entire computer, this is used in an async task.
                AutoResetEvent waitHandler = new AutoResetEvent(false);

                string queryString = "\\Printer.Consumables";

                // Define the delegate for when OnBidiResponseReceived is invoked.
                printerQueue.OnBidiResponseReceived += ((sender, responseArguments) =>
                {
                    bidiResponse = ParseResponse(responseArguments);

                    // Set the waitHandler to signal that a response has been received.
                    waitHandler.Set();
                });

                // Send the query.
                printerQueue.SendBidiQuery(queryString);

                // Asynchronously wait for the the waitHandler to be set.
                await Task.Run(() =>
                {
                    waitHandler.WaitOne();
                });

                return bidiResponse;
            });
        }

        /// <summary>
        /// Parse the bidi response argument.
        /// </summary>
        /// <param name="responseArguments">The bidi response</param>
        /// <returns>A string that contains either the bidi response string or explains the invalid result.</returns>
        private string ParseResponse(PrinterQueueEventArgs responseArguments)
        {
            if (responseArguments.StatusHResult == (int)HRESULT.S_OK)
                return responseArguments.Response;
            else
                return InvalidHResult(responseArguments.StatusHResult);
        }

        /// <summary>
        /// Displays the invalid result returned by the bidi query as a string output statement.
        /// </summary>
        /// <param name="result">The HRESULT returned by the bidi query, assumed to be not HRESULT.S_OK.</param>
        /// <returns>A string that can be displayed to the user explaining the HRESULT.</returns>
        private string InvalidHResult(int result)
        {
            switch (result)
            {
                case unchecked((int)HRESULT.E_INVALIDARG):
                    return "Invalid Arguments";
                case unchecked((int)HRESULT.E_OUTOFMEMORY):
                    return "Out of Memory";
                case unchecked((int)HRESULT.ERROR_NOT_FOUND):
                    return "Not found";
                case (int)HRESULT.S_FALSE:
                    return "False";
                case (int)HRESULT.S_PT_NO_CONFLICT:
                    return "PT No Conflict";
                default:
                    return "Undefined status: " + (int)result;
            }
        }

        /// <summary>
        /// Determines whether the print capabilities contain the specified feature.
        /// </summary>
        /// <param name="feature">The feature to search the capabilities for</param>
        /// <returns>True if the capabilities and the ticket contain the specified feature, False if the feature was not found</returns>
        public bool FeatureExists(string feature)
        {
            if (string.IsNullOrWhiteSpace(feature))
                return false;

            if (null != Capabilities)
            {
                IPrintSchemaFeature capsFeature = Capabilities.GetFeatureByKeyName(feature);
                IPrintSchemaFeature ticketFeature = context.Ticket.GetFeatureByKeyName(feature);

                if (capsFeature != null && ticketFeature != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Set a specified feature's selected option to the specified option in the print ticket
        /// </summary>
        /// <param name="feature">The feature whose option will be set</param>
        /// <param name="optionIndex">The index of the option that will be selected in the list of options retrieved for the specified feature</param>
        public void SetFeatureOption(string feature, string optionIndex)
        {
            if (string.IsNullOrWhiteSpace(feature) || string.IsNullOrWhiteSpace(optionIndex))
            {
                return;
            }

            // convert the index from string to int
            var index = int.Parse(optionIndex);
            // Get the feature in the context's print ticket
            var ticketFeature = context.Ticket.GetFeatureByKeyName(feature);

            if (null != ticketFeature)
            {
                // Set the option only if the user has selected an option different from the original option.
                // Note that for options with user defined parameters, such as CustomMediaSize, extra information is needed
                // than what is present in the Print Capabilities.  For simplicity, we have chosen to use the default
                // parameters in this sample app. But developers should use specialized UI to prompt the user for the required parameters.
                if (index != int.Parse(GetSelectedOptionIndex(feature)))
                {
                    // Look up the specified option in the print capabilities, and set it as the feature's selected option
                    var option = GetCachedFeatureOptions(feature)[index];
                    ticketFeature.SelectedOption = option;
                }
            }
        }

        /// <summary>
        /// Get an array of a specified type of option information items for a specified feature in print capabilities.
        /// This function is called by Javascript to retrieve option display names and indeces
        /// </summary>
        /// <param name="feature">The feature whose options' information will be returned</param>
        /// <param name="infoTypeString">The type of information about the option to be looked up.  Valid strings include "DisplayName", and "Index"</param>
        /// <returns>An array of strings corresponding to each option's value for information of the specified type</returns>
        public string[] GetOptionInfo(string feature, string infoTypeString)
        {
            var options = new List<string>();

            if (string.IsNullOrWhiteSpace(infoTypeString))
                return null;

            // Parse infoTypeString to match it to an OptionInfoType
            OptionInfoType infoType;
            if (!Enum.TryParse(infoTypeString, out infoType) || !Enum.IsDefined(typeof(OptionInfoType), infoType))
                return null;

            var schemaOptions = GetCachedFeatureOptions(feature);

            if (infoType == OptionInfoType.DisplayName)
            {
                // Select just the DisplayName from each option
                options.AddRange(schemaOptions.Select(option => option.DisplayName));
            }
            else if (infoType == OptionInfoType.Index)
            {
                // Generate a range starting from 0 and equal in length to the number of options, and select each number in the range as a string
                options.AddRange(Enumerable.Range(0, schemaOptions.Count).Select(index => index.ToString()));
            }

            return options.ToArray();
        }

        /// <summary>
        /// Gets the index of the currently selected option in the list of options for a specified feature in the current print ticket
        /// </summary>
        /// <param name="feature">The feature whose currently selected option will be looked up</param>
        /// <returns>String-based representation of the index in the list of options of the specified feature's currently selected option</returns>
        public string GetSelectedOptionIndex(string feature)
        {
            List<IPrintSchemaOption> options = GetCachedFeatureOptions(feature);

            // Iterate through all the options, return the index of the selected option
            for (int i = 0; i < options.Count; i++)
            {
                if (options[i].Selected)
                {
                    return i.ToString();
                }
            }

            // It is possible for the PrintTicket object to not contain a current selection for this feature causing none 
            // of the options in the print capabilities to be marked as selected.  In this case, the developers should 
            // be familiar enough with the printer hardware to display and set the feature to the correct printer default option.
            // Because this is a generic sample app, the first option will be displayed and set when the user hits the back button.
            return "0";
        }

        /// <summary>
        /// Gets the display name for a specified feature in the print capabilities
        /// </summary>
        /// <param name="feature">The feature whose display name will be looked up</param>
        /// <returns>The display name of the specified feature</returns>
        public string GetFeatureDisplayName(string feature)
        {
            if (null == Capabilities)
                return null;

            IPrintSchemaFeature capsFeature = Capabilities.GetFeatureByKeyName(feature);
            if (null != capsFeature)
            {
                return capsFeature.DisplayName;
            }
            else
            {
                return "Feature \"" + feature + "\" was not found in the print capabilities";
            }
        }

        /// <summary>
        /// Checks each possible option for a particular feature for constraints.
        /// </summary>
        /// <param name="feature">The feature whose options will be looked up</param>
        /// <returns>A boolean array that is true for any option that is constrained under the last saved print ticket.</returns>
        public bool[] GetOptionConstraints(string feature)
        {
            List<IPrintSchemaOption> options = featureOptions[feature];
            bool[] constrainedList = new bool[options.Count];

            // Check the constrained value for each possible option of the feature
            foreach (IPrintSchemaOption option in options)
            {
                int optionIndex = options.IndexOf(option);
                constrainedList[optionIndex] = !options[optionIndex].Constrained.Equals(PrintSchemaConstrainedSetting.None);
            }

            return constrainedList;

        }

        /// <summary>
        /// This is a wrapper to make saving the ticket asynchronous.
        /// </summary>
        /// <returns>IAsyncOperation so that this method is:  
        ///     1) awaitable in C#.
        ///     2) consumed as a WinJS.promise on which we can then use .then() or .done().</returns>
        public IAsyncAction SaveTicketAsync(Windows.Devices.Printers.Extensions.PrintTaskConfigurationSaveRequest request, object printerExtensionContext)
        {
            return AsyncInfo.Run(async (o) =>
            {
                await Task.Run(() =>
                {
                    request.Save(printerExtensionContext);
                });
            });
        }

        /// <summary>
        /// Provides a friendlier way to use HRESULT error codes.
        /// </summary>
        enum HRESULT : int
        {
            S_OK = 0x0000,
            S_FALSE = 0x0001,
            S_PT_NO_CONFLICT = 0x40001,
            E_INVALID_DATA = unchecked((int)0x8007000D),
            E_INVALIDARG = unchecked((int)0x80070057),
            E_OUTOFMEMORY = unchecked((int)0x8007000E),
            ERROR_NOT_FOUND = unchecked((int)0x80070490)
        }

    }
}