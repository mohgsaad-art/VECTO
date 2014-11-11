Public Interface IFUELMAP


   Function ReadFile(Optional ByVal ShowMsg As Boolean = True) As Boolean


   Function fFCdelaunay_Intp(ByVal nU As Single, ByVal Tq As Single) As Single

   Function Triangulate() As Boolean

   Property FilePath As String
   ReadOnly Property MapDim As Integer 
   ReadOnly Property Tq As List(Of Single)
   ReadOnly Property FC As List(Of Single)
   ReadOnly Property nU As List(Of Single)



End Interface
