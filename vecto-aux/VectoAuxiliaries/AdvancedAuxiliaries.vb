Imports AdvancedAuxiliaryInterfaces

Public Class AdvancedAuxiliaries
Implements  IAdvancedAuxiliaries



     Private auxConfig As AuxiliaryConfig
   
    Public Sub new( )

           'VectoInputs = New VectoInputs() 
           'Signals     = New Signals()

    End Sub


    Public Function Configure(filePath As String, vectoFilePath As String ) As Boolean Implements AdvancedAuxiliaryInterfaces.IAdvancedAuxiliaries.Configure
    
    try

             Dim frmAuxiliaryConfig As New frmAuxiliaryConfig( filePath, vectoFilePath)

             frmAuxiliaryConfig.Show()

    Catch ex As Exception

     Return False

    Return false

    End Try


    Return true

    End Function

    Public Function CycleStep(seconds As Integer, ByRef message As String) As Boolean Implements AdvancedAuxiliaryInterfaces.IAdvancedAuxiliaries.CycleStep
          throw new NotImplementedException
    End Function

    Public Event Message(Message As String, messageType As AdvancedAuxiliaryInterfaces.AdvancedAuxiliaryMessageType) Implements AdvancedAuxiliaryInterfaces.IAdvancedAuxiliaries.Message

    Public ReadOnly Property Running As Boolean Implements AdvancedAuxiliaryInterfaces.IAdvancedAuxiliaries.Running
        Get
              throw new NotImplementedException
        End Get
    End Property

    Public Function RunStart( ByVal auxFilePath As String, ByRef message As String) As Boolean Implements AdvancedAuxiliaryInterfaces.IAdvancedAuxiliaries.RunStart
          throw new NotImplementedException
    End Function

    Public Function RunStop(ByRef message As String) As Boolean Implements AdvancedAuxiliaryInterfaces.IAdvancedAuxiliaries.RunStop
          throw new NotImplementedException
    End Function

'

    Public ReadOnly Property TotalFuelGRAMS As Single Implements AdvancedAuxiliaryInterfaces.IAdvancedAuxiliaries.TotalFuelGRAMS
        Get
              throw new NotImplementedException
        End Get
    End Property

    Public ReadOnly Property TotalFuelLITRES As Single Implements AdvancedAuxiliaryInterfaces.IAdvancedAuxiliaries.TotalFuelLITRES
        Get
              throw new NotImplementedException
        End Get
    End Property

    Public ReadOnly Property AuxiliaryName As String Implements AdvancedAuxiliaryInterfaces.IAdvancedAuxiliaries.AuxiliaryName
        Get
          Return "BusAuxiliaries"
        End Get
    End Property

    Public ReadOnly Property AuxiliaryVersion As String Implements AdvancedAuxiliaryInterfaces.IAdvancedAuxiliaries.AuxiliaryVersion
        Get
           Return "Version 1.0 Beta"
        End Get
    End Property


    Public Property Signals As AdvancedAuxiliaryInterfaces.ISignals Implements IAdvancedAuxiliaries.Signals

    Public Property VectoInputs As AdvancedAuxiliaryInterfaces.IVectoInputs Implements IAdvancedAuxiliaries.VectoInputs


End Class
