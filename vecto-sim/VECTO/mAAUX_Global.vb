Imports System.Collections.Generic
Imports VectoAuxiliaries
Imports System.IO

Module mAAUX_Global


    Public ClutchEngaged As Boolean
    Public EngineDrivelinePower As Single
    Public EngineDrivelineTorque As Single
    Public EngineMotoringPower As Single
    Public EngineSpeed As Integer
    Public PreExistingAuxPower As Single
    Public Idle As Boolean
    Public InNeutral As Boolean
    Public WithEvents advancedAuxModel As IAdvancedAuxiliaries

    Public RunningCalc As Boolean = False

    'This must be set in the main loop and will be used to determin
    'the name of the file which would be offered to the model which is used
    'by it internally. In Bus Auxiliaries, it is used for Actuations of the
    'Doors during particular cycle types.
    Public CurrentCycleFile As String = String.Empty

    'This is a default setting of 3114 this will be set when the cycle begins.
    Public CycleTimeInSeconds As Integer = 3114

    Public Sub AAEventAuxiliaryEvent(ByRef sender As Object, ByVal message As String, ByVal messageType As AdvancedAuxiliaryMessageType) Handles advancedAuxModel.AuxiliaryEvent


        WorkerMsg(messageType, message, "Advanced Auxiliaries")


    End Sub

    'AA-TB
    Public Function InitialiseAdvancedAuxModel(aauxFile As String) As Boolean

        Dim o As System.Runtime.Remoting.ObjectHandle
        Dim result As Boolean = True

        If VECTO_Global.VEC.AuxiliaryAssembly <> "CLASSIC" Then

            Try


                'Open Assembly and invoke the validation using the paths supplied.
                Try
                    o = Activator.CreateInstance(VEC.AuxiliaryAssembly, "VectoAuxiliaries.AdvancedAuxiliaries")
                    advancedAuxModel = DirectCast(o.Unwrap, IAdvancedAuxiliaries)

                    Dim message As String = String.Empty

                    'Set Statics
                    advancedAuxModel.VectoInputs.Cycle = DetermineCycleNameFromCurrentFile()
                    advancedAuxModel.VectoInputs.VehicleWeightKG = VEH.Mass
                    advancedAuxModel.VectoInputs.FuelMap = ENG.FuelMapFullPath
                    advancedAuxModel.VectoInputs.FuelDensity = Cfg.FuelDens

                    'Set Signals
                    advancedAuxModel.Signals.TotalCycleTimeSeconds = CycleTimeInSeconds
                    advancedAuxModel.Signals.EngineIdleSpeed = ENG.Nidle
                    advancedAuxModel.RunStart(aauxFile, VEC.FilePath, message)



                Catch Ex As Exception

                    result = False

                End Try

                Return result


            Catch ex As Exception



            End Try


        End If

        Return False

    End Function

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
        returnList.Add(New cAdvancedAuxiliary())



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

            MessageBox.Show("Unable to obtain Advanced Auxiliary Assemblies")

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
    Public Function ConfigureAdvancedAuxiliaries(ByVal assemblyName As String, ByVal version As String, filePath As String, vectoFilePath As String) As Boolean


        Dim auxList As List(Of cAdvancedAuxiliary) = DiscoverAdvancedAuxiliaries()
        Dim chosenAssembly As String
        Dim o As System.Runtime.Remoting.ObjectHandle
        Dim iAdvancedAux As IAdvancedAuxiliaries
        Dim result As Boolean

        chosenAssembly = auxList.Find(Function(x) x.AssemblyName = assemblyName AndAlso x.AuxiliaryVersion = version).AssemblyName
        If String.IsNullOrEmpty(chosenAssembly) Then Return False


        'Open Assembly and invoke the configuration using the paths supplied.

        Try
            o = Activator.CreateInstance(chosenAssembly, "VectoAuxiliaries.AdvancedAuxiliaries")
            iAdvancedAux = DirectCast(o.Unwrap, IAdvancedAuxiliaries)

            iAdvancedAux.Configure(filePath, vectoFilePath)

        Catch ex As Exception

            result = False

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


    Public Function ResolveAAUXFilePath(vectoPath As String, filename As String) As String

        'No Vecto Path supplied
        If vectoPath = "" Then Return filename

        'This is not relative
        If filename.Contains(":\") Then

            'Filepath is already absolute
            Return filename
        Else
            Return vectoPath & filename
        End If

    End Function


    Public Function ValidateAAUXFile(ByVal absoluteAAuxPath As String,
                                    ByVal assemblyName As String,
                                    ByVal version As String,
                                    ByRef message As String) As Boolean

        Dim auxList As List(Of cAdvancedAuxiliary) = DiscoverAdvancedAuxiliaries()
        Dim chosenAssembly As String
        Dim o As System.Runtime.Remoting.ObjectHandle
        Dim iAdvancedAux As IAdvancedAuxiliaries
        Dim result As Boolean


        chosenAssembly = auxList.Find(Function(x) x.AssemblyName = assemblyName AndAlso x.AuxiliaryVersion = version).AssemblyName
        If String.IsNullOrEmpty(chosenAssembly) Then Return False


        'Open Assembly and invoke the validation using the paths supplied.
        Try
            o = Activator.CreateInstance(chosenAssembly, "VectoAuxiliaries.AdvancedAuxiliaries")
            iAdvancedAux = DirectCast(o.Unwrap, IAdvancedAuxiliaries)

            result = iAdvancedAux.ValidateAAUXFile(absoluteAAuxPath, message)

        Catch ex As Exception

            result = False

        End Try

        Return result

    End Function


    ''' <summary>
    ''' Will Apply an algorithm to the DRI cycle file being used and attempt to return a consitant name
    ''' </summary>
    ''' <returns>String : Cylename IE, Bus_Interurban, Bus_Urban,etc</returns>
    ''' <remarks></remarks>
    Public Function DetermineCycleNameFromCurrentFile() As String

        'Get DriveFile without path and without extension
        Dim driveFile As String = fFILE(CurrentCycleFile, False)

        Select Case (True)

            'DJN - update to make contains test case insensitive
            Case driveFile.ToLower().Contains("heavy_urban") AndAlso driveFile.ToLower().Contains("bus")
                Return "Heavy urban"

            Case driveFile.ToLower().Contains("suburban") AndAlso driveFile.ToLower().Contains("bus")
                Return "Suburban"

            Case driveFile.ToLower().Contains("urban") AndAlso driveFile.ToLower().Contains("bus")
                Return "Urban"

            Case driveFile.ToLower().Contains("interurban") AndAlso driveFile.ToLower().Contains("bus")
                Return "Interurban"

            Case driveFile.ToLower().Contains("coach")
                Return "Coach"

            Case Else
                WorkerMsg(tMsgID.Warn, String.Format("UnServiced Cycle Name '{0}' in Pneumatics Actuations Map 0 Actuations returned", driveFile), "Advanced Auxiliaries")
                Return "UnknownCycleName"

        End Select




        Return "Urban"

    End Function




End Module
