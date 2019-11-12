<%@ Page Language="C#" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // Artificially inject a delay to enable the ControlChannelTrigger client app 
            // to be suspended after sending the HttpRequest.
            // We should not delay for a case where a canary request is sent by the client.
            if (Request.HttpMethod != "HEAD")
            {
                System.Threading.Thread.Sleep(25000);
            }
        }
        catch (Exception ex)
        {
            Response.StatusCode = 500;
            Response.Write(Server.HtmlEncode(ex.ToString()));
        }
    }

</script>
The server says hello.