<%@ Page Language="C#" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // Artificially inject 2 seconds of delay to enable testing
            // cancellation at the client.
            System.Threading.Thread.Sleep(2000);

            // Write back the request headers
            Response.Write("Request Headers:<br>");
            for (int i = 0; i < Request.Headers.Count; i++)
            {
                Response.Write(Request.Headers.Keys[i] + ": " + Server.HtmlEncode(Request.Headers[i]) + "<br>");
            }

            // Write back the request body
            Response.Write("<br>Request Body:<br>");

            System.IO.Stream inputStream = Request.InputStream;
            Int32 inputLength, bytesRead;

            using (System.IO.StreamReader reader = new System.IO.StreamReader(Request.InputStream))
            {
                string body = reader.ReadToEnd();
                Response.Write(Server.HtmlEncode(body));
                Response.Write("<br>");
            }

            if (Request.QueryString.Count > 0)
            {
                Response.Write("<br>Query Parameters:<br>");
                foreach (String key in Request.QueryString.AllKeys)
                {
                    Response.Write("Param: " + key + ";" + Server.HtmlEncode(Request.QueryString[key]) + "<br>");
                }
            }

            // Streaming Download, write extra data:
            if (Request.QueryString["extradata"] != null)
            {
                Response.Write("<br>Filler Data:<br>");
                int streamLength = Int32.Parse(Request.QueryString["extradata"]);
                for (int i = 0; i < streamLength; i++)
                {
                    Response.Write("ﬄ");
                }
                Response.Write("<br>");
            }

            Response.Write("<br>Default Response:");
        }
        catch (Exception ex)
        {
            Response.StatusCode = 500;
            Response.Write(Server.HtmlEncode(ex.ToString()));
        }
    }

</script>
<html>
 <body>
The server says hello.
 </body>
</html>
