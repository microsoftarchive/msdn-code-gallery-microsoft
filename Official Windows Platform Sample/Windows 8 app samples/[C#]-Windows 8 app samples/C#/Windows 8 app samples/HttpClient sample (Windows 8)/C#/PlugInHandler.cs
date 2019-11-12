using System.Net.Http;
using System.Threading;

namespace Microsoft.Samples.Networking.HttpClientSample
{
    public class PlugInHandler : MessageProcessingHandler
    {
        public PlugInHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        // Process the request before sending it
        protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Get)
            {
                request.Headers.Add("Custom-Header", "CustomRequestValue");
            }
            return request;
        }

        // Process the response before returning it to the user
        protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response.RequestMessage.Method == HttpMethod.Get)
            {
                response.Headers.Add("Custom-Header", "CustomResponseValue");
            }
            return response;
        }
    }
}
