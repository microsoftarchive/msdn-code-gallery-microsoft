Public Class Place
    ' Properties.
    Private _Name As String
    Private _State As String
    Private _PlaceType As PlaceType

    Public Property Name() As String
        Get
            Return _Name
        End Get
        Private Set(ByVal value As String)
            _Name = value
        End Set
    End Property

    Public Property State() As String
        Get
            Return _State
        End Get
        Private Set(ByVal value As String)
            _State = value
        End Set
    End Property

    Public Property PlaceType() As PlaceType
        Get
            Return _PlaceType
        End Get
        Private Set(ByVal value As PlaceType)
            _PlaceType = value
        End Set
    End Property

    ' Constructor.
    Friend Sub New(ByVal name As String, _
                   ByVal state As String, _
                   ByVal placeType As TerraServerReference.PlaceType)

        Me.Name = name
        Me.State = state
        Me.PlaceType = CType(placeType, PlaceType)
    End Sub
End Class

Public Enum PlaceType
    Unknown
    AirRailStation
    BayGulf
    CapePeninsula
    CityTown
    HillMountain
    Island
    Lake
    OtherLandFeature
    OtherWaterFeature
    ParkBeach
    PointOfInterest
    River
End Enum
