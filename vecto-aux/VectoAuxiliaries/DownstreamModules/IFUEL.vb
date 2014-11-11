Public Interface IFUELMAP

     Function ReadFile(Optional ByVal ShowMsg As Boolean = True) As Boolean
     Function Pdrag(ByVal nU As Single) As Single
     Function Pfull(ByVal nU As Single, ByVal LastPe As Single) As Single
     Function Pfull(ByVal nU As Single) As Single
     Function Tq(ByVal nU As Single) As Single
     Function fNpref(ByVal Nidle As Single) As Single
     Function fnUrated() As Single
     Function fnUofPfull(ByVal PeTarget As Single, ByVal FromLeft As Boolean) As Single
     Function Tmax() As Single

     Sub Init(ByVal Nidle As Single)
     Sub DeclInit()

     Property FilePath() As String


End Interface
