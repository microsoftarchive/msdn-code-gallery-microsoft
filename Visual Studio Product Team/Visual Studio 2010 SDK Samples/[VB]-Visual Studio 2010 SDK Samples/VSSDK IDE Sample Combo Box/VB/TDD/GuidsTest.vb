'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************


Imports Microsoft.VisualBasic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System
Imports System.Text
Imports System.Collections.Generic
Namespace ComboBox.UnitTest
	''' <summary>
    ''' This is a test class for Microsoft.Samples.VisualStudio.ComboBox.GuidList and is intended
    ''' to contain all Microsoft.Samples.VisualStudio.ComboBox.GuidList Unit Tests.
    ''' </summary>
	<TestClass()> _
	Public Class GuidListTest


		Private testContextInstance As TestContext

		''' <summary>
        ''' Gets or sets the test context which provides
        ''' information about and functionality for the current test run.
        ''' </summary>
		Public Property TestContext() As TestContext
			Get
				Return testContextInstance
			End Get
			Set(ByVal value As TestContext)
				testContextInstance = value
			End Set
		End Property
		#Region "Additional test attributes"
		' 
		'You can use the following additional attributes as you write your tests:
		'
		'Use ClassInitialize to run code before running the first test in the class
		'
		'[ClassInitialize()]
		'public static void MyClassInitialize(TestContext testContext)
		'{
		'}
		'
		'Use ClassCleanup to run code after all tests in a class have run
		'
		'[ClassCleanup()]
		'public static void MyClassCleanup()
		'{
		'}
		'
		'Use TestInitialize to run code before running each test
		'
		'[TestInitialize()]
		'public void MyTestInitialize()
		'{
		'}
		'
		'Use TestCleanup to run code after each test has run
		'
		'[TestCleanup()]
		'public void MyTestCleanup()
		'{
		'}
		'
		#End Region


	End Class


End Namespace
