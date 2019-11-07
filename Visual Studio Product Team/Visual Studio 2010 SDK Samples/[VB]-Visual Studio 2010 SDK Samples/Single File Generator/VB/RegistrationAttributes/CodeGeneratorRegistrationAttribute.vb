Imports System
Imports System.Globalization

Namespace Microsoft.VisualStudio.Shell
    ''' <summary>
    ''' This attribute adds a custom file generator registry entry for specific file 
    ''' type. 
    ''' For Example:
    '''   [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\9.0\Generators\
    '''		{fae04ec1-301f-11d3-bf4b-00c04f79efbc}\MyGenerator]
    '''			"CLSID"="{AAAA53CC-3D4F-40a2-BD4D-4F3419755476}"
    '''         "GeneratesDesignTimeSource" = d'1'
    ''' 
    ''' </summary>
    <AttributeUsage(AttributeTargets.Class, AllowMultiple:=True, Inherited:=True)>
    Public NotInheritable Class CodeGeneratorRegistrationAttribute
        Inherits RegistrationAttribute
        Private _contextGuid As String
        Private _generatorType As Type
        Private _generatorGuid As Guid
        Private _generatorName As String
        Private _generatorRegKeyName As String
        Private _generatesDesignTimeSource As Boolean = False
        Private _generatesSharedDesignTimeSource As Boolean = False
        ''' <summary>
        ''' Creates a new CodeGeneratorRegistrationAttribute attribute to register a custom
        ''' code generator for the provided context. 
        ''' </summary>
        ''' <param name="generatorType">The type of Code generator. Type that implements IVsSingleFileGenerator</param>
        ''' <param name="generatorName">The generator name</param>
        ''' <param name="contextGuid">The context GUID this code generator would appear under.</param>
        Public Sub New(ByVal generatorType As Type, ByVal generatorName As String, ByVal contextGuid As String)
            If generatorType Is Nothing Then
                Throw New ArgumentNullException("generatorType")
            End If
            If generatorName Is Nothing Then
                Throw New ArgumentNullException("generatorName")
            End If
            If contextGuid Is Nothing Then
                Throw New ArgumentNullException("contextGuid")
            End If

            _contextGuid = contextGuid
            _generatorType = generatorType
            _generatorName = generatorName
            _generatorRegKeyName = generatorType.Name
            _generatorGuid = generatorType.GUID
        End Sub

        ''' <summary>
        ''' Get the generator Type
        ''' </summary>
        Public ReadOnly Property GeneratorType() As Type
            Get
                Return _generatorType
            End Get
        End Property

        ''' <summary>
        ''' Get the Guid representing the project type
        ''' </summary>
        Public ReadOnly Property ContextGuid() As String
            Get
                Return _contextGuid
            End Get
        End Property

        ''' <summary>
        ''' Get the Guid representing the generator type
        ''' </summary>
        Public ReadOnly Property GeneratorGuid() As Guid
            Get
                Return _generatorGuid
            End Get
        End Property

        ''' <summary>
        ''' Get or Set the GeneratesDesignTimeSource value
        ''' </summary>
        Public Property GeneratesDesignTimeSource() As Boolean
            Get
                Return _generatesDesignTimeSource
            End Get
            Set(ByVal value As Boolean)
                _generatesDesignTimeSource = value
            End Set
        End Property

        ''' <summary>
        ''' Get or Set the GeneratesSharedDesignTimeSource value
        ''' </summary>
        Public Property GeneratesSharedDesignTimeSource() As Boolean
            Get
                Return _generatesSharedDesignTimeSource
            End Get
            Set(ByVal value As Boolean)
                _generatesSharedDesignTimeSource = value
            End Set
        End Property


        ''' <summary>
        ''' Gets the Generator name 
        ''' </summary>
        Public ReadOnly Property GeneratorName() As String
            Get
                Return _generatorName
            End Get
        End Property

        ''' <summary>
        ''' Gets the Generator reg key name under 
        ''' </summary>
        Public Property GeneratorRegKeyName() As String
            Get
                Return _generatorRegKeyName
            End Get
            Set(ByVal value As String)
                _generatorRegKeyName = value
            End Set
        End Property

        ''' <summary>
        ''' Property that gets the generator base key name
        ''' </summary>
        Private ReadOnly Property GeneratorRegKey() As String
            Get
                Return String.Format(CultureInfo.InvariantCulture, "Generators\{0}\{1}", ContextGuid, GeneratorRegKeyName)
            End Get
        End Property
        ''' <summary>
        '''     Called to register this attribute with the given context.  The context
        '''     contains the location where the registration inforomation should be placed.
        '''     It also contains other information such as the type being registered and path information.
        ''' </summary>
        Public Overrides Sub Register(ByVal context As RegistrationContext)
            Using childKey As Key = context.CreateKey(GeneratorRegKey)
                childKey.SetValue(String.Empty, GeneratorName)
                childKey.SetValue("CLSID", GeneratorGuid.ToString("B"))

                If GeneratesDesignTimeSource Then
                    childKey.SetValue("GeneratesDesignTimeSource", 1)
                End If

                If GeneratesSharedDesignTimeSource Then
                    childKey.SetValue("GeneratesSharedDesignTimeSource", 1)
                End If

            End Using

        End Sub

        ''' <summary>
        ''' Unregister this file extension.
        ''' </summary>
        ''' <param name="context"></param>
        Public Overrides Sub Unregister(ByVal context As RegistrationContext)
            context.RemoveKey(GeneratorRegKey)
        End Sub
    End Class
End Namespace