'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports Microsoft.VisualBasic
Imports System
Imports System.Globalization
Imports MsVsShell = Microsoft.VisualStudio.Shell

''' <summary>
''' This attribute registers the source control provider.
''' </summary>
<AttributeUsage(AttributeTargets.Class, AllowMultiple:=True, Inherited:=True)> _
Public NotInheritable Class ProvideSourceControlProvider
    Inherits MsVsShell.RegistrationAttribute
    Private _regName As String = Nothing
    Private _uiName As String = Nothing

    ''' <summary>
    ''' </summary>
    Public Sub New(ByVal regName As String, ByVal uiName As String)
        _regName = regName
        _uiName = uiName
    End Sub

    ''' <summary>
    ''' Get the friendly name of the provider (written in registry).
    ''' </summary>
    Public ReadOnly Property RegName() As String
        Get
            Return _regName
        End Get
    End Property

    ''' <summary>
    ''' Get the unique guid identifying the provider.
    ''' </summary>
    Public ReadOnly Property RegGuid() As Guid
        Get
            Return GuidList.guidSccProvider
        End Get
    End Property

    ''' <summary>
    ''' Get the UI name of the provider (string resource ID).
    ''' </summary>
    Public ReadOnly Property UIName() As String
        Get
            Return _uiName
        End Get
    End Property

    ''' <summary>
    ''' Get the package containing the UI name of the provider.
    ''' </summary>
    Public ReadOnly Property UINamePkg() As Guid
        Get
            Return GuidList.guidSccProviderPkg
        End Get
    End Property

    ''' <summary>
    ''' Get the guid of the provider's service.
    ''' </summary>
    Public ReadOnly Property SccProviderService() As Guid
        Get
            Return GuidList.guidSccProviderService
        End Get
    End Property

    ''' <summary>
    '''     Called to register this attribute with the given context.  The context
    '''     contains the location where the registration inforomation should be placed.
    '''     It also contains other information such as the type being registered and path information.
    ''' </summary>
    Public Overrides Sub Register(ByVal context As RegistrationContext)
        ' Write to the context's log what we are about to do.
        context.Log.WriteLine(String.Format(CultureInfo.CurrentCulture, "BasicSccProvider:" & Constants.vbTab + Constants.vbTab & "{0}" & Constants.vbLf, RegName))

        ' Declare the source control provider, its name, the provider's service 
        ' and aditionally the packages implementing this provider.
        Using sccProviders As Key = context.CreateKey("SourceControlProviders")
            Using sccProviderKey As Key = sccProviders.CreateSubkey(RegGuid.ToString("B"))
                sccProviderKey.SetValue("", RegName)
                sccProviderKey.SetValue("Service", SccProviderService.ToString("B"))

                Using sccProviderNameKey As Key = sccProviderKey.CreateSubkey("Name")
                    sccProviderNameKey.SetValue("", UIName)
                    sccProviderNameKey.SetValue("Package", UINamePkg.ToString("B"))

                    sccProviderNameKey.Close()
                End Using

                ' Additionally, you can create a "Packages" subkey where you can enumerate the dll
                ' that are used by the source control provider, something like "Package1"="BasicSccProvider.dll"
                ' but this is not a requirement.
                sccProviderKey.Close()
            End Using

            sccProviders.Close()
        End Using
    End Sub

    ''' <summary>
    ''' Unregister the source control provider.
    ''' </summary>
    ''' <param name="context"></param>
    Public Overrides Sub Unregister(ByVal context As RegistrationContext)
        context.RemoveKey("SourceControlProviders\" & GuidList.guidSccProviderPkg.ToString("B"))
    End Sub
End Class
