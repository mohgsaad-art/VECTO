'AA-TB

Public Class cAdvancedAuxiliary

'Private properties. Set on Constructor
private _AuxiliaryName     As string 
private _AuxiliaryVersion  As String
private _FileName          As String
private _AssemblyName      As String


'Public Readonly properties
Public readonly property AuxiliaryName     As string 
    Get
      Return _AuxiliaryName
    End Get
End Property
Public readonly Property AuxiliaryVersion  As String
    Get
     Return _AuxiliaryVersion
    End Get
End Property
Public readonly Property FileName          As String
    Get
     Return _FileName
    End Get
End Property
Public readonly Property AssemblyName      As String
    Get
     Return _AssemblyName
    End Get
End Property


'Constructor(s)

Public Sub new()

   _AuxiliaryName     ="Classic Vecto Auxiliary"
  _AuxiliaryVersion   =  "CLASSIC"
  _FileName           =  "CLASSIC"
  _AssemblyName       =  "CLASSIC"


End Sub

Public Sub new ( AuxiliaryName   As String, AuxiliaryVersion  As String, FileName As string, AssemblyName  As string  )

    _AuxiliaryName      = AuxiliaryName
    _AuxiliaryVersion   = AuxiliaryVersion
    _FileName           = FileName
    _AssemblyName       = AssemblyName    
                                                                                 
End Sub
                 
                 
                 
End Class    
