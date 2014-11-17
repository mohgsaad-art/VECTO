Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports VectoAuxiliaries

public class MockFuel50PC
Implements IFUELMAP


   Public Function fFCdelaunay_Intp(nU As Single, Tq As Single) As Single Implements IFUELMAP.fFCdelaunay_Intp

       Return (nU + Tq ) * 0.5

   End Function

   Public Property FilePath As String Implements IFUELMAP.FilePath

   Public Function ReadFile(Optional ShowMsg As Boolean = True) As Boolean Implements IFUELMAP.ReadFile
   Return true
   End Function

            Public ReadOnly Property FC As List(Of Single) Implements IFUELMAP.FC
                Get

                End Get
            End Property

            Public ReadOnly Property MapDim As Integer Implements IFUELMAP.MapDim
                Get

                End Get
            End Property

            Public ReadOnly Property nU As List(Of Single) Implements IFUELMAP.nU
                Get

                End Get
            End Property

            Public ReadOnly Property Tq As List(Of Single) Implements IFUELMAP.Tq
                Get

                End Get
            End Property

            Public Function Triangulate() As Boolean Implements IFUELMAP.Triangulate

            End Function
End Class

