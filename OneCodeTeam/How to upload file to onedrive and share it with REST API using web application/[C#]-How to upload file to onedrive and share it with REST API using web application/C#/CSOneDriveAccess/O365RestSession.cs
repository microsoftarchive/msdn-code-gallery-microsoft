using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace CSOneDriveAccess
{
    public class O365RestSession : O365RestSessionBase
    {
        /// <summary>
        /// The oneDrive REST API root address
        /// </summary>
        public string OneDriveApiRoot { get; set; } = "https://api.onedrive.com/v1.0/";

        /// <summary>
        /// Init a O365RestSessionBase object
        /// </summary>
        /// <param name="clientId">clientId of you office 365 application, you can find it in https://apps.dev.microsoft.com/</param>
        /// <param name="clientSecret">Password/Public Key of you office 365 application, you can find it in https://apps.dev.microsoft.com/</param>
        /// <param name="redirectURI">Authentication callback url, you can set it in https://apps.dev.microsoft.com/</param>
        public O365RestSession(string clientId, string clientSecret, string redirectURI) : base(clientId, clientSecret, redirectURI)
        {
        }

        /// <summary>
        /// Upload file to onedrive
        /// </summary>
        /// <param name="filePath">file path in local dick</param>
        /// <param name="oneDrivePath">path of one dirve</param>
        /// <returns>uploaded file info with json format</returns>
public async Task<string> UploadFileAsync(string filePath, string oneDrivePath)
{
    #region create upload session
            string uploadUri = await GetUploadSession(oneDrivePath);
            #endregion

    #region upload the file fragment
    string result = string.Empty;
    using (FileStream stream = File.OpenRead(filePath))
    {
        long position = 0;
        long totalLength = stream.Length;
        int length = 10 * 1024 * 1024;

        while (true)
        {
            byte[] bytes = await ReadFileFragmentAsync(stream, position, length);
            if (position >= totalLength)
            {
                break;
            }

            result = await UploadFileFragmentAsync(bytes, uploadUri, position, totalLength);

            position += bytes.Length;
        }
    }
    #endregion

    return result;
}

/// <summary>
        /// Get ond drive share link by fileID
        /// </summary>
        /// <param name="fileID">id on office 365 of the file</param>
        /// <param name="type">the link type, about more:https://dev.onedrive.com/items/sharing_createLink.htm#link-types</param>
        /// <param name="scope">the scope of share, about more:https://dev.onedrive.com/items/sharing_createLink.htm#scope-types</param>
        /// <returns>Share Uri</returns>
public async Task<string> GetShareLinkAsync(string fileID, OneDriveShareLinkType type, OneDrevShareScopeType scope)
        {
            string param = "{type:'" + type + "',scope:'" + scope + "'}";

            string result = await AuthRequestToStringAsync(
                uri: $"{OneDriveApiRoot}drive/items/{fileID}/action.createLink",
                httpMethod: HTTPMethod.Post,
                data: Encoding.UTF8.GetBytes(param),
                contentType: "application/json");

            return JObject.Parse(result).SelectToken("link.webUrl").Value<string>();
        }

/// <summary>
        /// create a upload session for upload
        /// </summary>
        /// <param name="oneDriveFilePath">path of one dirve</param>
        /// <returns>upload Uri</returns>
private async Task<string> GetUploadSession(string oneDriveFilePath)
        {
            var uploadSession = await AuthRequestToStringAsync(
                uri: $"{OneDriveApiRoot}drive/root:/{oneDriveFilePath}:/upload.createSession",
                httpMethod: HTTPMethod.Post,
                contentType: "application/x-www-form-urlencoded");

            //get the uplaod uri
            JObject jo = JObject.Parse(uploadSession);

            return jo.SelectToken("uploadUrl").Value<string>();
        }

/// <summary>
        /// upload file fragment
        /// </summary>
        /// <param name="datas">file fragment</param>
        /// <param name="uploadUri">upload uri</param>
        /// <param name="position">postion of the file bytes</param>
        /// <param name="totalLength">the file bytes lenght</param>
        /// <returns>expire time with json format</returns>
private async Task<string> UploadFileFragmentAsync(byte[] datas, string uploadUri, long position, long totalLength)
{
    var request = await InitAuthRequest(uploadUri, HTTPMethod.Put, datas, null);
    request.Request.Headers.Add("Content-Range", $"bytes {position}-{position + datas.Length - 1}/{totalLength}");

    return await request.GetResponseStringAsync();
}

/// <summary>
        /// read file fragment
        /// </summary>
        /// <param name="stream">file stream</param>
        /// <param name="startPos">start position</param>
        /// <param name="count">take count</param>
        /// <returns>the fragment of file with byte[]</returns>
private async Task<byte[]> ReadFileFragmentAsync(FileStream stream, long startPos, int count)
        {
            if (startPos >= stream.Length || startPos < 0 || count <= 0)
                return null;

            long trimCount = startPos + count > stream.Length ? stream.Length - startPos : count;

            byte[] retBytes = new byte[trimCount];
            stream.Seek(startPos, SeekOrigin.Begin);
            await stream.ReadAsync(retBytes, 0, (int)trimCount);
            return retBytes;
        }
    }
}
