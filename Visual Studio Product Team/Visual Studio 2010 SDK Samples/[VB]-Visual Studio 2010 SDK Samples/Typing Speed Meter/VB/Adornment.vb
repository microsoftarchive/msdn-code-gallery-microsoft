Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Text.Tagging
Imports Microsoft.VisualStudio.Text
Imports System.Timers
Imports Microsoft.VisualStudio.Editor
Imports Microsoft.VisualStudio.TextManager.Interop
Imports Microsoft.VisualStudio.OLE.Interop

Namespace TypingSpeed

	''' <summary>
	''' Adornment class that draws a square box in the top right hand corner of the viewport
	''' </summary>
	Friend Class TypingSpeedMeter

		Private _root As TypingSpeedControl
		Private _view As IWpfTextView
		Private _adornmentLayer As IAdornmentLayer
		Private _curMax As Integer
		Private _start As Date

		Public Sub New(ByVal view As IWpfTextView)
			_view = view
			_root = New TypingSpeedControl
			_curMax = 0
			_start = Date.UtcNow

			'Grab a reference to the adornment layer that this adornment should be added to
			_adornmentLayer = view.GetAdornmentLayer("TypingSpeed")

			AddHandler _view.ViewportHeightChanged, Sub() Me.onSizeChange()
			AddHandler _view.ViewportWidthChanged, Sub() Me.onSizeChange()
		End Sub

		Public Sub updateBar(ByVal typedChars As Integer)
			Dim max As Integer = 1000
			Dim curLevel As Double = 0
			Dim now As Date = Date.UtcNow
			Dim interval = now.Subtract(_start).TotalMinutes
			Dim speed As Integer = CInt(Fix(typedChars / interval))

			'speed
			_root.val.Content = speed
			If speed > _curMax Then
				_curMax = speed
				_root.MaxVal.Content = "Max: " & _curMax.ToString()
			End If

			If speed >= max Then
				curLevel = 1
			Else
				curLevel = CDbl(speed)/max
            End If

			_root.fill.Height = _root.bar.Height * curLevel
		End Sub

		Public Sub onSizeChange()
			'clear the adornment layer of previous adornments
			_adornmentLayer.RemoveAdornment(_root)

			'Place the image in the top right hand corner of the Viewport
			Canvas.SetLeft(_root, _view.ViewportRight - 80)
			Canvas.SetTop(_root, _view.ViewportTop + 15)

			'add the image to the adornment layer and make it relative to the viewports
			_adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, Nothing, Nothing, _root, Nothing)
		End Sub

	End Class
End Namespace
