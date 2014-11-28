
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports VectoAuxiliaries
Imports Moq

Namespace UnitTests

<TestFixture()>
Public Class FilePathUtilityTests

Private Const  GOODFILEPATH_WithExt           as string="c:\myfilePath\MyFileName.ext"
Private Const  GOODFILEPATH_WithoutExt        as string="c:\myfilePath\MyFileName"

Private Const  noFileFILEPATH                 as string="c:\myfilePath\"
Private Const  IlliegalFILEPATH_WithExt       as string="c:\myfilePath\MyFile|Name.ext"

<Test()>
Public Sub GetPathOnly()
  
  Dim expected As String =  "c:\myfilePath\"

  Dim actual  As String = FilePathUtils.filePathOnly( GOODFILEPATH_WithExt)

  Assert.AreEqual( expected.ToLower(), actual.ToLower())
    
End Sub

<Test()>
Public Sub GetExtOnly()

  Dim expected As String =  ".ext"

  Dim actual  As String = FilePathUtils.fileExtentionOnly( GOODFILEPATH_WithExt)

  Assert.AreEqual( expected.ToLower(), actual.ToLower())

End Sub


<Test()>
Public Sub NoFileNameExpectOne()

  Dim expected As String =  ""

  Dim actual  As String = FilePathUtils.fileNameOnly( noFileFILEPATH, True)

  Assert.AreEqual( expected.ToLower(), actual.ToLower())


End Sub


<Test()>
Public Sub TestForIllegalCharacters()

  Dim expected As boolean = false

  Dim actual  As Boolean  = FilePathUtils.fileNameLegal( IlliegalFILEPATH_WithExt)

  Assert.AreEqual( expected, actual)


End Sub


<Test()> _
<TestCase( "filename.ext",".ext",true)> _
<TestCase( "filename.ext",".",false)> _
<TestCase( "filename",".ext",false)> _
Public Sub ValidFilePath( filePath As String, expectedExtension As String, expected As Boolean )

  Dim actual As Boolean
  Dim message As string = String.Empty
  actual = FilePathUtils.ValidateFilePath( filePath, expectedExtension, message )
  Assert.AreEqual( expected, actual )

End Sub

 
End Class




End Namespace


