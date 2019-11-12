'********************************** Module Header ***********************************\
'Module Name:  CascadingDropDownSampleController.cs
'Project:      VBDropdownListMVC4
'Copyright (c) Microsoft Corporation.
'
'ASP.NET MVC 4 is a framework for building scalable, standards-based web applications using well-established design patterns and the power of ASP.NET and the .NET Framework. 
'This article and the attached code samples demonstrate demonstrates how to use cascading dropdown list with ASP.NET MVC 4. 
'
'This source is subject to the Microsoft Public License.
'See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'All other rights reserved.
'
'THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
'EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
'MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\************************************************************************************

Namespace VBDropdownListMVC4.Controllers
    ''' <summary>
    ''' Controller class for sample
    ''' </summary>
    Public Class CascadingDropDownSampleController
        Inherits Controller
#Region "Public Actions"

        ''' <summary>
        ''' Default Action for the web-page handling HTTP GET requests
        ''' </summary>
        ''' <returns></returns>
        <HttpGet> _
        Public Function Index() As ActionResult
            Dim viewModel As New CascadingDropDownSampleModel()

            viewModel.Makes = GetSampleMakes()

            Return View(viewModel)
        End Function

        ''' <summary>
        ''' AJAX Action to send sample Models in JSON format based on the selected make
        ''' </summary>
        ''' <param name="selectedMake"></param>
        ''' <returns></returns>
        Public Function GetSampleModels(selectedMake As String) As ActionResult
            Dim models As IDictionary(Of String, String) = GetSampleModelsFromSelectedMake(selectedMake)
            Return Json(models)
        End Function

        ''' <summary>
        ''' AJAX Action to send sample colors in JSON format based on the selected model
        ''' </summary>
        ''' <param name="selectedModel"></param>
        ''' <returns></returns>
        Public Function GetSampleColors(selectedModel As String) As ActionResult
            Dim colors As IDictionary(Of String, String) = GetSampleColorsFromSelectedModel(selectedModel)
            Return Json(colors)
        End Function

#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Method to generate sample makes
        ''' </summary>
        ''' <returns></returns>
        Private Function GetSampleMakes() As IDictionary(Of String, String)
            Dim makes As IDictionary(Of String, String) = New Dictionary(Of String, String)()

            makes.Add("1", "Acura")
            makes.Add("2", "Audi")
            makes.Add("3", "BMW")

            Return makes
        End Function

        ''' <summary>
        ''' Method to generate sample models based on selected make
        ''' </summary>
        ''' <param name="selectedMake"></param>
        ''' <returns></returns>
        Private Function GetSampleModelsFromSelectedMake(selectedMake As String) As IDictionary(Of String, String)
            Dim models As IDictionary(Of String, String) = New Dictionary(Of String, String)()

            Select Case selectedMake
                Case "1"
                    models.Add("1", "Integra")
                    models.Add("2", "RL")
                    models.Add("3", "TL")
                    Exit Select
                Case "2"
                    models.Add("4", "A4")
                    models.Add("5", "S4")
                    models.Add("6", "A6")
                    Exit Select
                Case "3"
                    models.Add("7", "3 series")
                    models.Add("8", "5 series")
                    models.Add("9", "7 series")
                    Exit Select
                Case Else
                    Throw New NotImplementedException()

            End Select

            Return models
        End Function

        ''' <summary>
        ''' Method to generate sample colors based on selected model
        ''' </summary>
        ''' <param name="selectedModel"></param>
        ''' <returns></returns>
        Private Function GetSampleColorsFromSelectedModel(selectedModel As String) As IDictionary(Of String, String)
            Dim colors As IDictionary(Of String, String) = New Dictionary(Of String, String)()

            Select Case selectedModel
                Case "1"
                    colors.Add("1", "Green")
                    colors.Add("2", "Sea Green")
                    colors.Add("3", "Pale Green")
                    Exit Select
                Case "2"
                    colors.Add("4", "Red")
                    colors.Add("5", "Bright Red")
                    Exit Select
                Case "3"
                    colors.Add("6", "Teal")
                    colors.Add("7", "Dark Teal")
                    Exit Select
                Case "4"
                    colors.Add("8", "Azure")
                    colors.Add("9", "Light Azure")
                    colors.Add("10", "Dark Azure")
                    Exit Select
                Case "5"
                    colors.Add("11", "Silver")
                    colors.Add("12", "Metallic")
                    Exit Select
                Case "6"
                    colors.Add("13", "Cyan")
                    Exit Select
                Case "7"
                    colors.Add("14", "Blue")
                    colors.Add("15", "Sky Blue")
                    colors.Add("16", "Racing Blue")
                    Exit Select
                Case "8"
                    colors.Add("17", "Yellow")
                    colors.Add("18", "Red")
                    Exit Select
                Case "9"
                    colors.Add("17", "Brown")
                    Exit Select
                Case Else
                    Throw New NotImplementedException()

            End Select

            Return colors
        End Function

#End Region

    End Class
End Namespace
