using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Samples.Networking.HttpClientSample
{
    internal static class Helpers
    {
        internal static async Task DisplayTextResult(HttpResponseMessage response, TextBox output)
        {
            string responseBodyAsText;

            // We cast the StatusCode to an int so we display the numeric value (e.g., "200") rather than the
            // name of the enum (e.g., "OK") which would often be redundant with the ReasonPhrase.
            output.Text += ((int)response.StatusCode) + " " + response.ReasonPhrase + Environment.NewLine;

            responseBodyAsText = await response.Content.ReadAsStringAsync();
            responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine); // Insert new lines
            output.Text += responseBodyAsText;
        }

        internal static void CreateHttpClient(ref HttpClient httpClient)
        {
            if (httpClient != null)
            {
                httpClient.Dispose();
            }

            // HttpClient functionality can be extended by plugging multiple handlers together and providing
            // HttpClient with the configured handler pipeline.
            HttpMessageHandler handler = new HttpClientHandler();
            handler = new PlugInHandler(handler); // Adds a custom header to every request and response message.            
            httpClient = new HttpClient(handler);

            // The following line sets a "User-Agent" request header as a default header on the HttpClient instance.
            // Default headers will be sent with every request sent from this HttpClient instance.
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Sample", "v8"));
        }

        internal static void ScenarioStarted(Button startButton, Button cancelButton)
        {
            startButton.IsEnabled = false;
            cancelButton.IsEnabled = true;
        }

        internal static void ScenarioCompleted(Button startButton, Button cancelButton)
        {
            startButton.IsEnabled = true;
            cancelButton.IsEnabled = false;
        }
    }
}
