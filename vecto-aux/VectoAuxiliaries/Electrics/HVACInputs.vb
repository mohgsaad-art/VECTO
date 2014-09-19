Public Class HVACInputs
    Implements IHVACInputs

    Public Property Region As Integer Implements IHVACInputs.Region

    Public Property Season As Integer Implements IHVACInputs.Season

    Public Sub New()

    End Sub


    Public Sub New(Region As Integer, Season As Integer)

        Me.Region = Region
        Me.Season = Season

    End Sub


End Class
