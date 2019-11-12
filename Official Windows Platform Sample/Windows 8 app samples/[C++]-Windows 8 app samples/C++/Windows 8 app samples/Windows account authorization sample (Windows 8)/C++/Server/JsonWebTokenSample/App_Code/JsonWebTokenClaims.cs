using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.IO;

namespace MicrosoftAccount.Server.JsonWebTokenSample
{
    public class JsonWebToken
    {
        #region Helper Classes
        [DataContract]
        public class JsonWebTokenClaims
        {
            [DataMember(Name = "exp")]
            private int expUnixTime
            {
                get;
                set;
            }

            private DateTime? expiration = null;
            public DateTime Expiration
            {
                get
                {
                    if (this.expiration == null)
                    {
                        this.expiration = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(expUnixTime);
                    }

                    return (DateTime)this.expiration;
                }
            }

            [DataMember(Name = "iss")]
            public string Issuer
            {
                get;
                private set;
            }

            [DataMember(Name = "aud")]
            public string Audience
            {
                get;
                private set;
            }

            [DataMember(Name = "uid")]
            public string UserId
            {
                get;
                private set;
            }

            [DataMember(Name = "ver")]
            public int Version
            {
                get;
                private set;
            }

            [DataMember(Name = "urn:microsoft:appuri")]
            public string ClientIdentifier
            {
                get;
                private set;
            }

            [DataMember(Name = "urn:microsoft:appid")]
            public string AppId
            {
                get;
                private set;
            }
        }

        [DataContract]
        public class JsonWebTokenEnvelope
        {
            [DataMember(Name = "typ")]
            public string Type
            {
                get;
                private set;
            }

            [DataMember(Name = "alg")]
            public string Algorithm
            {
                get;
                private set;
            }

            [DataMember(Name = "kid")]
            public int KeyId
            {
                get;
                private set;
            }
        }
        #endregion

        #region Properties
        private static readonly DataContractJsonSerializer ClaimsJsonSerializer = new DataContractJsonSerializer(typeof(JsonWebTokenClaims));
        private static readonly DataContractJsonSerializer EnvelopeJsonSerializer = new DataContractJsonSerializer(typeof(JsonWebTokenEnvelope));
        private static readonly UTF8Encoding UTF8Encoder = new UTF8Encoding(true, true);
        private static readonly SHA256Managed SHA256Provider = new SHA256Managed();

        private string claimsTokenSegment;
        public JsonWebTokenClaims Claims
        {
            get;
            private set;
        }

        private string envelopeTokenSegment;
        public JsonWebTokenEnvelope Envelope
        {
            get;
            private set;
        }

        public string Signature
        {
            get;
            private set;
        }

        public bool IsExpired
        {
            get
            {
                return this.Claims.Expiration < DateTime.Now;
            }
        }
        #endregion

        #region Constructors
        public JsonWebToken(string token)
        {
            // Get the token segments & perform validation
            string[] tokenSegments = this.SplitToken(token);

            // Decode and deserialize the claims
            this.claimsTokenSegment = tokenSegments[1];
            this.Claims = this.GetClaimsFromTokenSegment(this.claimsTokenSegment);

            // Decode and deserialize the envelope
            this.envelopeTokenSegment = tokenSegments[0];
            this.Envelope = this.GetEnvelopeFromTokenSegment(this.envelopeTokenSegment);

            // Get the signature
            this.Signature = tokenSegments[2];

            this.ValidateEnvelope(this.Envelope);
        }

        private JsonWebToken()
        {
        }
        #endregion

        #region Parsing Methods
        private JsonWebTokenClaims GetClaimsFromTokenSegment(string claimsTokenSegment)
        {
            byte[] claimsData = this.Base64UrlDecode(claimsTokenSegment);
            using (MemoryStream memoryStream = new MemoryStream(claimsData))
            {
                return ClaimsJsonSerializer.ReadObject(memoryStream) as JsonWebTokenClaims;
            }
        }

        private JsonWebTokenEnvelope GetEnvelopeFromTokenSegment(string envelopeTokenSegment)
        {
            byte[] envelopeData = this.Base64UrlDecode(envelopeTokenSegment);
            using (MemoryStream memoryStream = new MemoryStream(envelopeData))
            {
                return EnvelopeJsonSerializer.ReadObject(memoryStream) as JsonWebTokenEnvelope;
            }
        }

        private string[] SplitToken(string token)
        {
            // Expected token format: Envelope.Claims.Signature

            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Token is empty or null.");
            }

            string[] segments = token.Split('.');

            if (segments.Length != 3)
            {
                throw new Exception("Invalid token format. Expected Envelope.Claims.Signature");
            }

            if (string.IsNullOrEmpty(segments[0]))
            {
                throw new Exception("Invalid token format. Envelope must not be empty");
            }

            if (string.IsNullOrEmpty(segments[1]))
            {
                throw new Exception("Invalid token format. Claims must not be empty");
            }

            if (string.IsNullOrEmpty(segments[2]))
            {
                throw new Exception("Invalid token format. Signature must not be empty");
            }

            return segments;
        }
        #endregion

        #region Validation Methods
        public void Validate(Dictionary<int, string> keyIdsKeys, string  appId, string audience)
        {
            // Validation
            if (this.Claims.Audience != audience)
            {
                throw new Exception(string.Format("Audiences do not match this.'{0}' != expected.'{1}'", this.Claims.Audience, audience));
            }

            if (this.Claims.AppId != appId)
            {
                throw new Exception(string.Format("AppId do not match this.'{0}' != expected.'{1}'", this.Claims.AppId, appId));
            }

            // Ensure that the tokens KeyId exists in the secret keys list
            if (!keyIdsKeys.ContainsKey(this.Envelope.KeyId))
            {
                throw new Exception(string.Format("Could not find key with id {0}", this.Envelope.KeyId));
            }

            this.ValidateSignature(keyIdsKeys[this.Envelope.KeyId]);     
        }

        private void ValidateEnvelope(JsonWebTokenEnvelope envelope)
        {
            if (envelope.Type != "JWT")
            {
                throw new Exception("Unsupported token type");
            }

            if (envelope.Algorithm != "HS256")
            {
                throw new Exception("Unsupported crypto algorithm");
            }
        }

        private void ValidateSignature(string key)
        {
            // Derive signing key, Signing key = SHA256(secret + "JWTSig")
            byte[] bytes = UTF8Encoder.GetBytes(key + "JWTSig");
            byte[] signingKey = SHA256Provider.ComputeHash(bytes);

            // To Validate:
            // 
            // 1. Take the bytes of the UTF-8 representation of the JWT Claim
            //  Segment and calculate an HMAC SHA-256 MAC on them using the
            //  shared key.
            //
            // 2. Base64url encode the previously generated HMAC as defined in this
            //  document.
            //
            // 3. If the JWT Crypto Segment and the previously calculated value
            //  exactly match in a character by character, case sensitive
            //  comparison, then one has confirmation that the key was used to
            //  generate the HMAC on the JWT and that the contents of the JWT
            //  Claim Segment have not be tampered with.
            //
            // 4. If the validation fails, the token MUST be rejected.

            // UFT-8 representation of the JWT envelope.claim segment
            byte[] input = UTF8Encoder.GetBytes(this.envelopeTokenSegment + "." + this.claimsTokenSegment);

            // calculate an HMAC SHA-256 MAC
            using (HMACSHA256 hashProvider = new HMACSHA256(signingKey))
            {
                byte[] myHashValue = hashProvider.ComputeHash(input);

                // Base64 url encode the hash
                string base64urlEncodedHash = this.Base64UrlEncode(myHashValue);

                // Now compare the two has values
                if (base64urlEncodedHash != this.Signature)
                {
                    throw new Exception("Signature does not match.");
                }
            }
        }
        #endregion

        #region Base64 Encode / Decode Functions
        // Reference: http://tools.ietf.org/search/draft-jones-json-web-token-00

        public byte[] Base64UrlDecode(string encodedSegment)
        {
            string s = encodedSegment;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding
            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: s += "=="; break; // Two pad chars
                case 3: s += "="; break; // One pad char
                default: throw new System.Exception("Illegal base64url string");
            }
            return Convert.FromBase64String(s); // Standard base64 decoder
        }

        public string Base64UrlEncode(byte[] arg)
        {
            string s = Convert.ToBase64String(arg); // Standard base64 encoder
            s = s.Split('=')[0]; // Remove any trailing '='s
            s = s.Replace('+', '-'); // 62nd char of encoding
            s = s.Replace('/', '_'); // 63rd char of encoding
            return s;
        }
        #endregion
    }
}
