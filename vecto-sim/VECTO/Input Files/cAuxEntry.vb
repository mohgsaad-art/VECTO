Imports System.Collections.Generic

    Public Class cAuxEntry
        Public Type As String
        Public Path As cSubPath
        Public TechStr As String = ""

        Public PulleyGearEfficiencyES As Single
        Public PulleyGearRatioES As Single
        Public PulleyGearEfficiencyPS As Single
        Public PulleyGearRatioPS As Single
        Public PulleyGearEfficiencyHVAC As Single
        Public PulleyGearRatioHVAC As Single


        Public ConsumerListES As List(Of VectoAuxiliaries.Electrics.ElectricalConsumer)
        Public ConsumerListPS As List(Of VectoAuxiliaries.Pneumatics.PneumaticConsumer)
        Public HVACMapInputs As Dictionary(Of String, Single)

        Public Sub New()
            Path = New cSubPath
        End Sub

    End Class