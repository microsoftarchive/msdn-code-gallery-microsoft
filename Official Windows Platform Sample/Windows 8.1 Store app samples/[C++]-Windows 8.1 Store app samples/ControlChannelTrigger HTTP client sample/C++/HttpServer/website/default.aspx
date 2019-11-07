<%@ Page Language="C#" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Response.ContentType = "text/plain";

            // Use chunked encoding.
            Response.BufferOutput = false;

            // Artificially inject delays to allow the ControlChannelTrigger client app 
            // to be suspended before completing the response.
            // We should not delay for a case where a canary request is sent by the client.
            if (Request.HttpMethod != "HEAD")
            {
                // Send a chunk every 10 seconds for the next 15 minutes.
                for (int i = 0; i < 90; i++)
                {
                    System.Threading.Thread.Sleep(10000);
                    Response.Write("Message " + i + "\r\n");
                }
            }
        }
        catch (Exception ex)
        {
            Response.StatusCode = 500;
            Response.Write(Server.HtmlEncode(ex.ToString()));
        }
    }

</script>
