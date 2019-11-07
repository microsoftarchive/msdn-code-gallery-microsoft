//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Security.Authentication.Web;
using Windows.Storage;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace Geofencing4SqSample
{
    /// <summary>
    /// Extends IHttpFilter for making OAuth2 calls to Foursquare
    /// </summary>
    public class AuthFilter : IHttpFilter
    {
        private IHttpFilter _innerFilter;
        private string _token;

        public AuthFilter(IHttpFilter innerFilter)
        {
            if (null == innerFilter)
            {
                throw new ArgumentException("innerFilter cannot be null.");
            }
            _innerFilter = innerFilter;
        }

        public void Dispose()
        {
            _innerFilter.Dispose();
            GC.SuppressFinalize(this);
        }

        public IAsyncOperationWithProgress<HttpResponseMessage, HttpProgress> SendRequestAsync(HttpRequestMessage request)
        {
            return AsyncInfo.Run<HttpResponseMessage, HttpProgress>(async (cancellationToken, progress) =>
            {
                // Get the auth token
                if (null == _token)
                {
                    await AuthenticateAsync();
                }
                request.Headers.Add("Authorization", "OAuth " + _token);
                var response = await _innerFilter.SendRequestAsync(request).AsTask(cancellationToken, progress);

                cancellationToken.ThrowIfCancellationRequested();
                return response;
            });
        }

        /// <summary>
        /// Authenticates with FourSquare and populates the OAuth on success
        /// Note: WebAuthenticationBroker does not work with Facebook sign-in, so users
        /// need to enter their FourSquare credentials.
        /// </summary>
        async private Task AuthenticateAsync()
        {
            var startUri = new Uri("https://foursquare.com/oauth2/authenticate?client_id="
                                    + Constants.ClientId
                                    + "&response_type=token&display=touch&redirect_uri=ms-app%3A%2F%2F"
                                    + Uri.EscapeDataString(Constants.AppSid));
            var endUri = new Uri("ms-app://" + Uri.EscapeDataString(Constants.AppSid));

            WebAuthenticationResult authResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                    WebAuthenticationOptions.None,
                                                    startUri,
                                                    endUri);

            if (authResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                // Extract the token from the response
                var response = authResult.ResponseData.ToString().TrimEnd();
                if (response.Contains("error"))
                {
                    var errorString = response.Split('=').Last<string>();
                    if (errorString == "access_denied")
                    {
                        Logger.Trace(TraceLevel.Error, "User denied access to Foursquare");
                        throw new UnauthorizedAccessException("User denied access to Foursquare");
                    }
                    else
                    {
                        Logger.Trace(TraceLevel.Error, "Authentication returned an error response: " + errorString);
                        throw new UnauthorizedAccessException("Authentication returned an error response: " + errorString);
                    }
                }
                else
                {
                    _token = response.Split('=').Last<string>();
                    ApplicationData.Current.LocalSettings.Values[Constants.OAuthTokenKey] = _token;
                }
            }
            else if (authResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                Logger.Trace(TraceLevel.Error, "HTTP Error returned by AuthenticateAsync() : " + authResult.ResponseErrorDetail.ToString());
                throw new UnauthorizedAccessException("Authentication failed due to HTTP error : " + authResult.ResponseErrorDetail.ToString());
            }
            else
            {
                Logger.Trace(TraceLevel.Error, "Error returned by AuthenticateAsync() : " + authResult.ResponseStatus.ToString());
                throw new UnauthorizedAccessException("Authentication failed: " + authResult.ResponseStatus.ToString());
            }
        }
    }
}
