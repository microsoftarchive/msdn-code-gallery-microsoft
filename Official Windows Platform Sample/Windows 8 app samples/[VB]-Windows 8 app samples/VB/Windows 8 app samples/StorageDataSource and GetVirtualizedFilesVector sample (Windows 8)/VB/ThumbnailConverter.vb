Imports System
Imports Windows.Storage.Streams
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Media.Imaging

Friend Class ThumbnailConverter
    Implements IValueConverter
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As String) As Object Implements IValueConverter.Convert
        If value IsNot Nothing Then
            Dim thumbnailStream = DirectCast(value, IRandomAccessStream)
            Dim image = New BitmapImage()
            image.SetSource(thumbnailStream)

            Return image
        End If

        Return DependencyProperty.UnsetValue
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As String) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class
