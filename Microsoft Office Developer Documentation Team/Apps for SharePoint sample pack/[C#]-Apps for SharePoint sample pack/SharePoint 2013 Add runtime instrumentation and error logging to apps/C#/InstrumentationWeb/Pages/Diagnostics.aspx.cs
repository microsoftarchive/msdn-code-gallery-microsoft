using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace InstrumentationWeb.Pages
{
    public partial class Diagnostics : System.Web.UI.Page
    {
        Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");

        protected void Page_Load(object sender, EventArgs e)
        {
            TraceSection section = (TraceSection)configuration.GetSection("system.web/trace");
            if (section.Enabled)
            {
                // Tracing is enabled, so make sure the link to the trace log works.
                lnkToTraceLog.Enabled = true;

                // Add a custom warning to the trace log.
                Trace.Warn("This trace warning was thrown when the diagnostics page loaded.");
                btnToggleTracing.Text = "TURN OFF TRACING";
            }
            else
            {
                // Tracing is not enabled, so make sure the link to the trace log does not work.
                lnkToTraceLog.Enabled = false;
                btnToggleTracing.Text = "TURN ON TRACING";
            }
        }

        protected void btnToggleTracing_Click(object sender, EventArgs e)
        {
            TraceSection section = (TraceSection)configuration.GetSection("system.web/trace");
            if (!section.Enabled)
            {
                section.Enabled = true;
                configuration.Save();
                lnkToTraceLog.Enabled = true;
                btnToggleTracing.Text = "TURN OFF TRACING";
            }
            else
            {
                section.Enabled = false;
                configuration.Save();
                lnkToTraceLog.Enabled = false;
                btnToggleTracing.Text = "TURN ON TRACING";

            }
        }
    }
}