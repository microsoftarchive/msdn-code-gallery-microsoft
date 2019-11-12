''' <summary>
''' This model is used to bind the CascadingDropDownSample Controller's Index action
''' </summary>
Public Class CascadingDropDownSampleModel
    ''' <summary>
    ''' Dictionary holding the sample Makes values
    ''' </summary>
    Public Property Makes() As IDictionary(Of String, String)
        Get
            Return m_Makes
        End Get
        Set(value As IDictionary(Of String, String))
            m_Makes = Value
        End Set
    End Property

    Private m_Makes As IDictionary(Of String, String)
End Class
