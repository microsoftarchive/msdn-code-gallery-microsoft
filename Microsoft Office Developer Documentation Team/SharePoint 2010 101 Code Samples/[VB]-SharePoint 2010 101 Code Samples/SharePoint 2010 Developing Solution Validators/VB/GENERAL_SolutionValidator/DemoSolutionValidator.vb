'We need this namespace for the GUID that identifies the validator
Imports System.Runtime.InteropServices
Imports Microsoft.SharePoint
'We need this namespace for the SPUserCode class
Imports Microsoft.SharePoint.Administration
'We need this namespace for the SPSolutionValidationProperties class
Imports Microsoft.SharePoint.UserCode

''' <summary>
''' A solution validator is a class that can check sandboxed user solutions when they
''' are activated. Your code can test the solution to increase your confidence in
''' user solutions. For example, you could allow only Web Part solutions or check for
''' a particular certificate. In this case, we'll simply log all user solution activations
''' </summary>
''' <remarks>
''' This GIUD is essential and used to remove the Solution Validator when the feature
''' is deactivated. You can use Tools/Create GUID to generate a unique one. 
''' </remarks>
<Guid("93DA2F64-4438-4547-B969-226834DB9AB5")> _
Public Class DemoSolutionValidator
    Inherits SPSolutionValidator

    Private Const VALIDATOR_NAME As String = "Demo Solution Validator"

    'Two Constructors
    Public Sub New(ByVal userCodeService As SPUserCodeService)
        MyBase.New(VALIDATOR_NAME, userCodeService)

        'SharePoint uses this value to determine if the validator has changed
        'Alter it for every version. 
        Me.Signature = 5678
    End Sub

    Public Sub New()

    End Sub

    'This method is called once for every Solution as it activates
    Public Overrides Sub ValidateSolution(ByVal properties As Microsoft.SharePoint.UserCode.SPSolutionValidationProperties)
        MyBase.ValidateSolution(properties)

        'We will mark every solution as valid, then log its activation to a SharePoint list
        'The Valid property is false by default. We must set it to true or else the 
        'Solution activation will fail. Usually you'd do this only after making
        'your checks.
        properties.Valid = True
        'NOTE: If the user solution fails your tests, you should use the 
        'ValidationErrorMessage and ValidationErrorUrl properties to tell the
        'user why.

        'Get the SPSite where the solution was activated, ensuring correct disposal
        Using site As SPSite = properties.Site

            'Get the top level SPWeb
            Using topWeb As SPWeb = site.RootWeb

                'Get the Annoucements list
                Dim announcementsList As SPList = topWeb.Lists("Announcements")

                'Create a new announcement
                Dim newAnnouncement As SPListItem = announcementsList.Items.Add()
                newAnnouncement("Title") = "A user solution has been activated"
                newAnnouncement("Body") = "The user solution name was: " + properties.Name
                newAnnouncement.Update()

            End Using

        End Using
    End Sub

    'This method is called once for every Assembly in the solution
    Public Overrides Sub ValidateAssembly(ByVal properties As Microsoft.SharePoint.UserCode.SPSolutionValidationProperties, ByVal assembly As Microsoft.SharePoint.UserCode.SPSolutionFile)
        MyBase.ValidateAssembly(properties, assembly)
        'In this example, we run no special check on assemblies
        'But we must set Valid = true or the solution activation will not complete
        properties.Valid = True
    End Sub
End Class
