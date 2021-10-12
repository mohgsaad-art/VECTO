
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Utils

Namespace Mocks

	Public Class AlternatorMapMock
		Implements IAlternatorMap



		Dim failing As Boolean

		Public Sub New(ByVal isFailing As Boolean)
			failing = isFailing
		End Sub


		Public Function GetEfficiency(rpm1 As PerSecond, amps1 As Ampere) As double Implements IAlternatorMap.GetEfficiency
			Return 0.0
		End Function

        public ReadOnly property Source As String Implements IAlternatorMap.Source
        get
            Return ""
                End Get
        End Property

		'Public Event IAuxiliaryEvent_AuxiliaryEvent As AuxiliaryEventEventHandler Implements IAuxiliaryEvent.AuxiliaryEvent
	End Class

End Namespace
