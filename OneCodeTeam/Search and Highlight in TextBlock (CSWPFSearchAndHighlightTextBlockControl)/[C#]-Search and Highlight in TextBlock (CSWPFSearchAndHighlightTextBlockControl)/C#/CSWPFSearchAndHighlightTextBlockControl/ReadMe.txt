================================================================================
       WPF APPLICATION: CSWPFSearchAndHighlightTextBlockControl Overview                        
================================================================================


/////////////////////////////////////////////////////////////////////////////
Summary:

The WPF code sample demonstrates how to search and highlight keywords in a 
TextBlock control. 

The sample includes a custom user control "SearchableTextControl" and its search 
method is used to demonstrate how to highlight keywords using 
System.Windows.Documents.Run and System.Windows.Documents.Incline.


////////////////////////////////////////////////////////////////////////////////
Demo:

Step1. Build this project in VS2010. 

Step2. Run CSWPFSearchAndHighlightTextBlockControl.exe.

Step3. Input the source string after the "Source Text" TextBlock.

Step4. Type the keyword which you want to search in the source string after the "Search" TextBlock.

Step5. You can change the Background/Foreground colors by selecting the specific color 
       in drop-list of combobox. 
       

/////////////////////////////////////////////////////////////////////////////
Code Logic:

Step 1. Create a Visual C# WPF Application in Visual Studio 2010 and name it 
        "CSWPFSearchAndHighlightTextBlockControl".

Step 2. Create a class "SearchableTextControl". Make sure it inherits from the "Control" class.

Step 3. The "SearchableTextControl" class uses DependencyProperty.Register to implement
        a render event bound to custom user control such as:

		/// <summary>
		/// SearchText sandbox which is used to get or set the value from a dependency property,
		/// if it gets a value,it should be forced to bind to a string type.
		/// </summary>
		public string SearchText
		{
		  get { return (string)GetValue(SearchTextProperty); }
		  set { SetValue(SearchTextProperty, value); }
		}

		/// <summary>
		/// Real implementation about SearchTextProperty which registers a dependency property with 
		/// the specified property name, property type, owner type, and property metadata. 
		/// </summary>
		public static readonly DependencyProperty SearchTextProperty =
			DependencyProperty.Register("SearchText", typeof(string), typeof(SearchableTextControl), 
			new UIPropertyMetadata(string.Empty,
			  UpdateControlCallBack));

		/// <summary>
		/// Create a call back function which is used to invalidate the rendering of the element, 
		/// and force a complete new layout pass.
		/// One such advanced scenario is if you are creating a PropertyChangedCallback for a 
		/// dependency property that is not  on a Freezable or FrameworkElement derived class that 
		/// still influences the layout when it changes.
		/// </summary>
		private static void UpdateControlCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SearchableTextControl obj = d as SearchableTextControl;
			obj.InvalidateVisual();
		}

Step 4. In SearchableTextControl class, there is a override method "OnRender", this method uses 
		String.IndexOf and String.Substring methods to match the target string, and we associate
		those methods with TextBlock.Inlines.Add method in this sample. And in order to implement 
		the behavior which looks like capture of regular expression with several times.

Step 5. The method "GenerateRun" which in class SearchableTextControl is used to alternate the 
		colors of all matched strings. And it add some features by adding the FontStyle property and 
		the FontWeight property.

		private Run GenerateRun(string searchedString, bool isHighlight)
		{
		   if (!string.IsNullOrEmpty(searchedString))
            {
                Run run = new Run(searchedString)
                {
                    Background = isHighlight ? this.HighlightBackground : this.Background,
                    Foreground = isHighlight ? this.HighlightForeground : this.Foreground,

                    // Set the source text with the style which is Italic.
                    FontStyle = isHighlight ? FontStyles.Italic : FontStyles.Normal,

                    // Set the source text with the style which is Bold.
                    FontWeight = isHighlight ? FontWeights.Bold : FontWeights.Normal,
                };
                return run;
            }
		  return null;
		}


/////////////////////////////////////////////////////////////////////////////
References:

Run Class 
http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&l=EN-US&k=k(SYSTEM.WINDOWS.DOCUMENTS.RUN);k(RUN);k(DevLang-CSHARP)&rd=true

DependencyProperty.OverrideMetadata Method
http://msdn.microsoft.com/en-us/library/system.windows.dependencyproperty.overridemetadata.aspx

DependencyProperty.Register Method 
http://msdn.microsoft.com/en-us/library/system.windows.dependencyproperty.register.aspx

FrameworkTemplate.FindName Method 
http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&l=EN-US&k=k(SYSTEM.WINDOWS.FRAMEWORKTEMPLATE.FINDNAME);k(TargetFrameworkMoniker-%22.NETFRAMEWORK%2cVERSION%3dV4.0%22);k(DevLang-CSHARP)&rd=true

UIElement.InvalidateVisual Method 
http://msdn.microsoft.com/en-us/library/system.windows.uielement.invalidatevisual.aspx

/////////////////////////////////////////////////////////////////////////////
