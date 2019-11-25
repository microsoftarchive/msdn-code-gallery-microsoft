#region "Directives"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

#endregion

namespace CSDropdownListMVC4.Models
{
    /// <summary>
    /// This model is used to bind the CascadingDropDownSample Controller's Index action
    /// </summary>
    public class CascadingDropDownSampleModel
    {
        /// <summary>
        /// Dictionary holding the sample Makes values
        /// </summary>
        public IDictionary<string, string> Makes { get; set; }
    }
}