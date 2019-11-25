Imports System.Runtime.Serialization
Imports System.Threading
Imports System.Net
Imports System.Runtime.Serialization.Json
Imports System.IO
Imports System.ServiceModel.Channels
Imports System.ServiceModel

Public Class WebForm1
    Inherits System.Web.UI.Page
    Public detectedLangCode As String, toCode As String
    Public allLang As String(), languagesForTranslate As String()


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim admToken As AdmAccessToken
        Dim headerValue As String
        'Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
        'Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx)            
        Dim admAuth As New AdmAuthentication("24b731c4-d565-4311-8642-0a1ce6318456", "iBjdc2HmbL3fHDFJrFb5nwE/Dowe6T5w48V21MGR95Q=")
        Try
            admToken = admAuth.GetAccessToken()
            Dim tokenReceived As DateTime = DateTime.Now
            ' Create a header with the access_token property of the returned
            headerValue = "Bearer " + admToken.access_token
            DetectMethod(headerValue)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub DetectMethod(authToken As String)
        Dim client As New ServiceReference1.LanguageServiceClient()

        Dim httpRequestProperty As New HttpRequestMessageProperty()
        httpRequestProperty.Method = "POST"
        httpRequestProperty.Headers.Add("Authorization", authToken)
        Using scope As New OperationContextScope(client.InnerChannel)
            OperationContext.Current.OutgoingMessageProperties(HttpRequestMessageProperty.Name) = httpRequestProperty
            'Below line will return the code of the detected language.
            Dim strDetectedLangCode As String() = {client.Detect("", txtUser.Text)}
            detectedLangCode = strDetectedLangCode(0)
            'Fetch the name of the detected language using the code.
            Dim strDetectedLang As String() = client.GetLanguageNames("", "en", strDetectedLangCode, True)
            lblDetectedText.Text = strDetectedLang(0)

            'Fetching the list of supported languages and binding to the dropdown list.
            languagesForTranslate = client.GetLanguagesForTranslate("")
            allLang = client.GetLanguageNames("", "en", languagesForTranslate, True)
            drpAllLang.DataSource = allLang
            drpAllLang.DataBind()
        End Using

    End Sub

    Protected Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim admToken As AdmAccessToken
        Dim headerValue As String
        'Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
        'Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx)            
        Dim admAuth As New AdmAuthentication("24b731c4-d565-4311-8642-0a1ce6318456", "iBjdc2HmbL3fHDFJrFb5nwE/Dowe6T5w48V21MGR95Q=")
        Try
            admToken = admAuth.GetAccessToken()
            Dim tokenReceived As DateTime = DateTime.Now
            ' Create a header with the access_token property of the returned
            headerValue = "Bearer " + admToken.access_token
            TranslateMethod(headerValue)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub TranslateMethod(auToken As String)
        Dim client As New ServiceReference1.LanguageServiceClient()
        'Set Authorization header before sending the request
        Dim httpRequestProperty As New HttpRequestMessageProperty()
        httpRequestProperty.Method = "POST"
        httpRequestProperty.Headers.Add("Authorization", auToken)
        ' Creates a block within which an OperationContext object is in scope.
        Using scope As New OperationContextScope(client.InnerChannel)
            OperationContext.Current.OutgoingMessageProperties(HttpRequestMessageProperty.Name) = httpRequestProperty
            Dim translationResult As String
            languagesForTranslate = client.GetLanguagesForTranslate("")
            detectedLangCode = client.Detect("", txtUser.Text)
            translationResult = client.Translate("", txtUser.Text, detectedLangCode, languagesForTranslate(drpAllLang.SelectedIndex), "text/html", "general", _
                "")
            txtTranslated.Text = translationResult
        End Using
    End Sub
End Class

<DataContract> _
Public Class AdmAccessToken
    <DataMember> _
    Public Property access_token() As String
        Get
            Return m_access_token
        End Get
        Set(value As String)
            m_access_token = value
        End Set
    End Property
    Private m_access_token As String
    <DataMember> _
    Public Property token_type() As String
        Get
            Return m_token_type
        End Get
        Set(value As String)
            m_token_type = value
        End Set
    End Property
    Private m_token_type As String
    <DataMember> _
    Public Property expires_in() As String
        Get
            Return m_expires_in
        End Get
        Set(value As String)
            m_expires_in = value
        End Set
    End Property
    Private m_expires_in As String
    <DataMember> _
    Public Property scope() As String
        Get
            Return m_scope
        End Get
        Set(value As String)
            m_scope = value
        End Set
    End Property
    Private m_scope As String
End Class

Public Class AdmAuthentication
    Public Shared ReadOnly DatamarketAccessUri As String = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13"
    Private clientId As String
    Private clientSecret As String
    Private request As String
    Private token As AdmAccessToken

    Public Sub New(clientId As String, clientSecret As String)
        Me.clientId = clientId
        Me.clientSecret = clientSecret
        'If clientid or client secret has special characters, encode before sending request
        Me.request = String.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret))
        Me.token = HttpPost(DatamarketAccessUri, Me.request)
    End Sub

    Public Function GetAccessToken() As AdmAccessToken
        Return Me.token
    End Function
   

    Private Function HttpPost(DatamarketAccessUri As String, requestDetails As String) As AdmAccessToken
        'Prepare OAuth request 
        Dim webRequest__1 As WebRequest = WebRequest.Create(DatamarketAccessUri)
        webRequest__1.ContentType = "application/x-www-form-urlencoded"
        webRequest__1.Method = "POST"
        Dim bytes As Byte() = Encoding.ASCII.GetBytes(requestDetails)
        webRequest__1.ContentLength = bytes.Length
        Using outputStream As Stream = webRequest__1.GetRequestStream()
            outputStream.Write(bytes, 0, bytes.Length)
        End Using
        Using webResponse As WebResponse = webRequest__1.GetResponse()
            Dim serializer As New DataContractJsonSerializer(GetType(AdmAccessToken))
            'Get deserialized object from JSON stream
            Dim token As AdmAccessToken = DirectCast(serializer.ReadObject(webResponse.GetResponseStream()), AdmAccessToken)
            Return token
        End Using
    End Function
End Class