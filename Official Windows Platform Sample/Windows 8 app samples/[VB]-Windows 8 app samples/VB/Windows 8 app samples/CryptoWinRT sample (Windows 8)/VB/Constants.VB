'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        Public Const FEATURE_NAME As String = "Cryptography WinRT Sample"

        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "CryptographicBuffer", .ClassType = GetType(Global.CryptoWinRT.Scenario1)},
                                                               New Scenario With {.Title = "Hash Algorithms", .ClassType = GetType(Global.CryptoWinRT.Scenario2)},
                                                               New Scenario With {.Title = "Hmac Algorithms", .ClassType = GetType(Global.CryptoWinRT.Scenario3)},
                                                               New Scenario With {.Title = "Key Derivation", .ClassType = GetType(Global.CryptoWinRT.Scenario4)},
                                                               New Scenario With {.Title = "Cipher Algorithms", .ClassType = GetType(Global.CryptoWinRT.Scenario5)},
                                                               New Scenario With {.Title = "Authenticated Encryption Algorithms", .ClassType = GetType(Global.CryptoWinRT.Scenario6)},
                                                               New Scenario With {.Title = "Encrypt and Decrypt", .ClassType = GetType(Global.CryptoWinRT.Scenario7)},
                                                               New Scenario With {.Title = "Sign and Verify Signature", .ClassType = GetType(Global.CryptoWinRT.Scenario8)},
                                                               New Scenario With {.Title = "Asynchronous Data Protection", .ClassType = GetType(Global.CryptoWinRT.Scenario9)},
                                                               New Scenario With {.Title = "Asynchronous Data Protection with Streams", .ClassType = GetType(Global.CryptoWinRT.Scenario10)}
                                                          }
    End Class

    Public Class Scenario
        Public Property Title As String
            Get
                Return m_Title
            End Get
            Set(value As String)
                m_Title = value
            End Set
        End Property
        Private m_Title As String

        Public Property ClassType As Type
            Get
                Return m_ClassType
            End Get
            Set(value As Type)
                m_ClassType = value
            End Set
        End Property
        Private m_ClassType As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace

