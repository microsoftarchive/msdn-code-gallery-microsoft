Imports Microsoft.SharePoint

''' <summary>
''' When a user creates a new column in a List or Content Type, they see a list of 
''' Field Types, such as "Single Line of Text" and "Date and Time". By creating your
''' own Field Type, you can extend this list. You can also include your own logic in 
''' the new Field Type, such as custom code to set the default value or validate user
''' input. This code example shows how to create a custom Field Type
''' </summary>
''' <remarks>
''' To create a custom field type, create an empty SharePoint project and then add a 
''' new class. The new class should inherit from a SPField class. In this case, the 
''' field stores strings, so we inherit from SPFieldText. You must also create a 
''' fldtypes*.xml file and deploy it to TEMPLATES\XML in the 14 hive. If your field 
''' type doesn't appear immediately, try an iisreset.
''' </remarks>
Public Class ContosoProductCode
    Inherits SPFieldText

#Region "Constructors"

    'You must create both these constructors, passing the parameters to the base class
    Public Sub New(ByVal fields As SPFieldCollection, ByVal fName As String)
        MyBase.New(fields, fName)
    End Sub

    Public Sub New(ByVal fields As SPFieldCollection, ByVal tName As String, ByVal dName As String)
        MyBase.New(fields, tName, dName)
    End Sub

#End Region

    Public Overrides Property DefaultValue As String
        Get
            Return "CP0001"
        End Get
        Set(ByVal value As String)

        End Set
    End Property

    Public Overrides Function GetValidatedString(ByVal value As Object) As String

        'Check that it starts with CP for "Contoso Product"
        If Not value.ToString().StartsWith("CP", StringComparison.CurrentCultureIgnoreCase) Then

            'When you throw an SPFieldValidationException users see red text in the UI
            Throw New SPFieldValidationException("Product code must start with 'CP' for Contoso Products")

        End If

        'Check that it's 6 characters long
        If value.ToString().Length <> 6 Then
            Throw New SPFieldValidationException("Product code must be 6 characters long")
        End If

        'Always convert to uppercase before writing to Content DB.
        Return value.ToString().ToUpper()

    End Function

End Class
