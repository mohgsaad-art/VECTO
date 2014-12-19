Imports System.Collections.Generic
Imports VectoAuxiliaries
Imports System.IO

Module AAUX_Gobal

  'AA-TB
  ''' <summary>
  ''' Discovers Advanced Auxiliaries Assemblies in 'targetDirectory' Directory
  ''' </summary>
  ''' <returns>List(Of cAdvancedAuxiliary)</returns>
  ''' <remarks>Target Directory would normally be the executing directory, but can be in another location.</remarks>
  Public Function DiscoverAdvancedAuxiliaries() As List(Of cAdvancedAuxiliary)

     Dim returnList As List(Of cAdvancedAuxiliary) = New List(Of cAdvancedAuxiliary)
     Dim fileNameWoPath As String
     Dim fileNameWoExtentsion As String
     Dim advancedAuxiliary As cAdvancedAuxiliary
     Dim o As System.Runtime.Remoting.ObjectHandle
     Dim iAdvancedAux As IAdvancedAuxiliaries


     'Create Default
     returnList.Add(New cAdvancedAuxiliary("Classic Vecto Auxiliary", "CLASSIC", "CLASSIC", "CLASSIC"))



     Try
     Dim fileEntries As String() = Directory.GetFiles(GetAAUXSourceDirectory)
        ' Process the list of files found in the directory. 
        Dim fileName As String

        For Each fileName In fileEntries

         If fileName.Contains("Auxiliaries.dll") Then

         'Get filenamewith
         fileNameWoPath = fFILE(fileName, True)
         fileNameWoExtentsion = fFILE(fileName, False)

         o = Activator.CreateInstance(fileNameWoExtentsion, "VectoAuxiliaries.AdvancedAuxiliaries")

         iAdvancedAux = DirectCast(o.Unwrap, IAdvancedAuxiliaries)

         advancedAuxiliary = New cAdvancedAuxiliary(iAdvancedAux.AuxiliaryName, iAdvancedAux.AuxiliaryVersion, fileNameWoPath, fileNameWoExtentsion)

         returnList.Add(advancedAuxiliary)


         End If

        Next fileName

        Catch ex As Exception

            MessageBox.Show("Unable to obtain Advanced Auxiliary Assemblies" )

        End Try


        Return returnList

    End Function

  'AA-TB
  ''' <summary>
  ''' Invokes Advanced Auxiliaries Configuration Screen
  ''' </summary>
  ''' <param name="vectoFilePath">String : Contains the path of the vecto file.</param>
  ''' <returns>Boolean. True if aauxFile is valid after operation , false of not.</returns>
  ''' <remarks></remarks>
  Public Function ConfigureAdvancedAuxiliaries( ByVal assemblyName As String, byval version As string, filePath As String, vectoFilePath As string) As Boolean


      Dim auxList As List(of cAdvancedAuxiliary ) = DiscoverAdvancedAuxiliaries()
      Dim chosenAssembly  As String 
      Dim o As System.Runtime.Remoting.ObjectHandle
      Dim iAdvancedAux As IAdvancedAuxiliaries
      Dim result As Boolean

      chosenAssembly = auxList.Find( Function(x) x.AssemblyName= assemblyName ANDalso x.AuxiliaryVersion= version).AssemblyName
      If String.IsNullOrEmpty( chosenAssembly) then Return False


      'Open Assembly and invoke the configuration using the paths supplied.

      Try
               o = Activator.CreateInstance(chosenAssembly, "VectoAuxiliaries.AdvancedAuxiliaries")
               iAdvancedAux = DirectCast(o.Unwrap, IAdvancedAuxiliaries)

               iAdvancedAux.Configure(filePath, vectoFilePath)

      Catch ex As Exception

       result = false

      End Try

      Return result



  End Function


''' <summary>
''' Gets location of Advanced Auxiliaries Directory which contains all the assemblies available.
''' </summary>
''' <returns>Path where Auxiliaries can be found : String</returns>
''' <remarks></remarks>
Public Function GetAAUXSourceDirectory() As String


     Return Path.GetDirectoryName(Application.ExecutablePath)


  End Function





End Module
