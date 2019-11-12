Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace PortfolioReportGenerator
    Class Portfolio

        Public Sub New(ByVal name As String)
            If name = "Steve" Then
                Me.Name = name
                Me.AccountNumber = 443221
                Me.BeginningValueQTR = 10000
                Me.BeginningValueYTD = 5800
                Me.ContributionsQTR = 2000
                Me.ContributionsYTD = 6000
                Me.DistributionsQTR = -100
                Me.DistributionsYTD = -300
                Me.FeesQTR = -50
                Me.FeesYTD = -150
                Me.GainLossQTR = 700
                Me.GainLossYTD = 1200
                Me.Performance1Yr = 0.1
                Me.Performance2Yr = 0.06
                Me.Performance3Yr = 0.085
                Me.Performance5Yr = 0.09
                Me.Performance10Yr = 0.0825
                Me.WithdrawalsQTR = 500
                Me.WithdrawalsYTD = 500

                Me.Holdings = New PortfolioItem(1) {}
            ElseIf name = "Kelly" Then
                Me.Name = name
                Me.AccountNumber = 443699
                Me.BeginningValueQTR = 11000
                Me.BeginningValueYTD = 5300
                Me.ContributionsQTR = 0
                Me.ContributionsYTD = 5000
                Me.DistributionsQTR = 0
                Me.DistributionsYTD = 0
                Me.FeesQTR = -75
                Me.FeesYTD = -150
                Me.GainLossQTR = 575
                Me.GainLossYTD = 1350
                Me.Performance1Yr = 0.12
                Me.Performance2Yr = 0.05
                Me.Performance3Yr = 0.09
                Me.Performance5Yr = 0.0927
                Me.Performance10Yr = 0.084
                Me.WithdrawalsQTR = 0
                Me.WithdrawalsYTD = 0
                Me.Holdings = New PortfolioItem(2) {}
            End If
            InitializeHoldings()
        End Sub

        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(ByVal value As String)
                m_Name = Value
            End Set
        End Property
        Private m_Name As String
        Public Property AccountNumber() As Int32
            Get
                Return m_AccountNumber
            End Get
            Set(ByVal value As Int32)
                m_AccountNumber = Value
            End Set
        End Property
        Private m_AccountNumber As Int32
        Public Property BeginningValueQTR() As Double
            Get
                Return m_BeginningValueQTR
            End Get
            Set(ByVal value As Double)
                m_BeginningValueQTR = Value
            End Set
        End Property
        Private m_BeginningValueQTR As Double
        Public Property BeginningValueYTD() As Double
            Get
                Return m_BeginningValueYTD
            End Get
            Set(ByVal value As Double)
                m_BeginningValueYTD = Value
            End Set
        End Property
        Private m_BeginningValueYTD As Double
        Public Property ContributionsQTR() As Double
            Get
                Return m_ContributionsQTR
            End Get
            Set(ByVal value As Double)
                m_ContributionsQTR = Value
            End Set
        End Property
        Private m_ContributionsQTR As Double
        Public Property ContributionsYTD() As Double
            Get
                Return m_ContributionsYTD
            End Get
            Set(ByVal value As Double)
                m_ContributionsYTD = Value
            End Set
        End Property
        Private m_ContributionsYTD As Double
        Public Property WithdrawalsQTR() As Double
            Get
                Return m_WithdrawalsQTR
            End Get
            Set(ByVal value As Double)
                m_WithdrawalsQTR = Value
            End Set
        End Property
        Private m_WithdrawalsQTR As Double
        Public Property WithdrawalsYTD() As Double
            Get
                Return m_WithdrawalsYTD
            End Get
            Set(ByVal value As Double)
                m_WithdrawalsYTD = Value
            End Set
        End Property
        Private m_WithdrawalsYTD As Double
        Public Property DistributionsQTR() As Double
            Get
                Return m_DistributionsQTR
            End Get
            Set(ByVal value As Double)
                m_DistributionsQTR = Value
            End Set
        End Property
        Private m_DistributionsQTR As Double
        Public Property DistributionsYTD() As Double
            Get
                Return m_DistributionsYTD
            End Get
            Set(ByVal value As Double)
                m_DistributionsYTD = Value
            End Set
        End Property
        Private m_DistributionsYTD As Double
        Public Property FeesQTR() As Double
            Get
                Return m_FeesQTR
            End Get
            Set(ByVal value As Double)
                m_FeesQTR = Value
            End Set
        End Property
        Private m_FeesQTR As Double
        Public Property FeesYTD() As Double
            Get
                Return m_FeesYTD
            End Get
            Set(ByVal value As Double)
                m_FeesYTD = Value
            End Set
        End Property
        Private m_FeesYTD As Double
        Public Property GainLossQTR() As Double
            Get
                Return m_GainLossQTR
            End Get
            Set(ByVal value As Double)
                m_GainLossQTR = Value
            End Set
        End Property
        Private m_GainLossQTR As Double
        Public Property GainLossYTD() As Double
            Get
                Return m_GainLossYTD
            End Get
            Set(ByVal value As Double)
                m_GainLossYTD = Value
            End Set
        End Property
        Private m_GainLossYTD As Double
        Public Property PerformanceQTR() As Double
            Get
                Return m_PerformanceQTR
            End Get
            Set(ByVal value As Double)
                m_PerformanceQTR = Value
            End Set
        End Property
        Private m_PerformanceQTR As Double
        Public Property PerformanceYTD() As Double
            Get
                Return m_PerformanceYTD
            End Get
            Set(ByVal value As Double)
                m_PerformanceYTD = Value
            End Set
        End Property
        Private m_PerformanceYTD As Double
        Public Property Performance1Yr() As Double
            Get
                Return m_Performance1Yr
            End Get
            Set(ByVal value As Double)
                m_Performance1Yr = Value
            End Set
        End Property
        Private m_Performance1Yr As Double
        Public Property Performance2Yr() As Double
            Get
                Return m_Performance2Yr
            End Get
            Set(ByVal value As Double)
                m_Performance2Yr = Value
            End Set
        End Property
        Private m_Performance2Yr As Double
        Public Property Performance3Yr() As Double
            Get
                Return m_Performance3Yr
            End Get
            Set(ByVal value As Double)
                m_Performance3Yr = Value
            End Set
        End Property
        Private m_Performance3Yr As Double
        Public Property Performance5Yr() As Double
            Get
                Return m_Performance5Yr
            End Get
            Set(ByVal value As Double)
                m_Performance5Yr = Value
            End Set
        End Property
        Private m_Performance5Yr As Double
        Public Property Performance10Yr() As Double
            Get
                Return m_Performance10Yr
            End Get
            Set(ByVal value As Double)
                m_Performance10Yr = Value
            End Set
        End Property
        Private m_Performance10Yr As Double
        Public Property Holdings() As PortfolioItem()
            Get
                Return m_Holdings
            End Get
            Set(ByVal value As PortfolioItem())
                m_Holdings = Value
            End Set
        End Property
        Private m_Holdings As PortfolioItem()

        Private Sub InitializeHoldings()
            If Me.Name = "Steve" Then
                Me.Holdings(0) = New PortfolioItem()
                Me.Holdings(0).Cost = 5000
                Me.Holdings(0).CurrentPrice = 14.25
                Me.Holdings(0).Description = "Adventure Works"
                Me.Holdings(0).High52Week = 17.5
                Me.Holdings(0).Low52Week = 10.25
                Me.Holdings(0).SharesHeld = 500
                Me.Holdings(0).MarketValue = Me.Holdings(0).SharesHeld * Me.Holdings(0).CurrentPrice
                Me.Holdings(0).Ticker = "AW"

                Me.Holdings(1) = New PortfolioItem()
                Me.Holdings(1).Cost = 7590
                Me.Holdings(1).CurrentPrice = 18.0
                Me.Holdings(1).Description = "Contoso"
                Me.Holdings(1).High52Week = 19.5
                Me.Holdings(1).Low52Week = 12.8
                Me.Holdings(1).SharesHeld = 500
                Me.Holdings(1).MarketValue = Me.Holdings(1).SharesHeld * Me.Holdings(1).CurrentPrice

                Me.Holdings(1).Ticker = "CTSO"
            ElseIf Me.Name = "Kelly" Then
                Me.Holdings(0) = New PortfolioItem()
                Me.Holdings(0).Cost = 4900
                Me.Holdings(0).CurrentPrice = 14.25
                Me.Holdings(0).Description = "Adventure Works"
                Me.Holdings(0).High52Week = 17.5
                Me.Holdings(0).Low52Week = 10.25
                Me.Holdings(0).SharesHeld = 300
                Me.Holdings(0).MarketValue = Me.Holdings(0).SharesHeld * Me.Holdings(0).CurrentPrice
                Me.Holdings(0).Ticker = "AW"

                Me.Holdings(1) = New PortfolioItem()
                Me.Holdings(1).Cost = 7790
                Me.Holdings(1).CurrentPrice = 18.0
                Me.Holdings(1).Description = "Contoso"
                Me.Holdings(1).High52Week = 19.5
                Me.Holdings(1).Low52Week = 12.8
                Me.Holdings(1).SharesHeld = 700
                Me.Holdings(1).MarketValue = Me.Holdings(1).SharesHeld * Me.Holdings(1).CurrentPrice
                Me.Holdings(1).Ticker = "CTSO"

                Me.Holdings(2) = New PortfolioItem()
                Me.Holdings(2).Cost = 10900
                Me.Holdings(2).CurrentPrice = 10.0
                Me.Holdings(2).Description = "Wingtip Bank"
                Me.Holdings(2).High52Week = 11.5
                Me.Holdings(2).Low52Week = 8.1
                Me.Holdings(2).SharesHeld = 1000
                Me.Holdings(2).MarketValue = Me.Holdings(2).SharesHeld * Me.Holdings(2).CurrentPrice
                Me.Holdings(2).Ticker = "WTIP"
            End If
        End Sub

    End Class

    Class PortfolioItem
        Public Property Ticker() As String
            Get
                Return m_Ticker
            End Get
            Set(ByVal value As String)
                m_Ticker = Value
            End Set
        End Property
        Private m_Ticker As String
        Public Property Description() As String
            Get
                Return m_Description
            End Get
            Set(ByVal value As String)
                m_Description = Value
            End Set
        End Property
        Private m_Description As String
        Public Property CurrentPrice() As Double
            Get
                Return m_CurrentPrice
            End Get
            Set(ByVal value As Double)
                m_CurrentPrice = Value
            End Set
        End Property
        Private m_CurrentPrice As Double
        Public Property SharesHeld() As Double
            Get
                Return m_SharesHeld
            End Get
            Set(ByVal value As Double)
                m_SharesHeld = Value
            End Set
        End Property
        Private m_SharesHeld As Double
        Public Property MarketValue() As Double
            Get
                Return m_MarketValue
            End Get
            Set(ByVal value As Double)
                m_MarketValue = Value
            End Set
        End Property
        Private m_MarketValue As Double
        Public Property Cost() As Double
            Get
                Return m_Cost
            End Get
            Set(ByVal value As Double)
                m_Cost = Value
            End Set
        End Property
        Private m_Cost As Double
        Public Property High52Week() As Double
            Get
                Return m_High52Week
            End Get
            Set(ByVal value As Double)
                m_High52Week = Value
            End Set
        End Property
        Private m_High52Week As Double
        Public Property Low52Week() As Double
            Get
                Return m_Low52Week
            End Get
            Set(ByVal value As Double)
                m_Low52Week = Value
            End Set
        End Property
        Private m_Low52Week As Double
    End Class
End Namespace