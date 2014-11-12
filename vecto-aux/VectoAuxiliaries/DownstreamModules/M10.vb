Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules

Namespace DownstreamModules


Public Class M10
Implements IM10


Private m4 As IM4_AirCompressor
Private m9 As IM9

Private signals As ISignals




        Public ReadOnly Property InterFCNonSmartPneumatics As Single Implements IM10.InterFCNonSmartPneumatics
            Get

            End Get
        End Property

        Public ReadOnly Property InterFCSmartPneumatics As Single Implements IM10.InterFCSmartPneumatics
            Get

            End Get
        End Property


Public sub new( m4 As IM4_AirCompressor, m9 As IM9, fcMap As IFUELMAP ,  signals As ISignals)



End Sub


End Class



End Namespace






