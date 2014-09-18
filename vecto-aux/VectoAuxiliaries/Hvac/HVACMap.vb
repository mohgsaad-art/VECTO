Imports System.IO
Imports VectoAuxiliaries.Electrics
Imports System.Windows.Forms

Namespace Hvac

    Public Class HVACMap
        Implements IHVACMap



#Region "Header Comments"

        'Some sort of multi-dimensional map implemented here
        'No interpolation - too expensive/complex to implement?
        'Set list of choices in each dimension of input

        'options
        '1. multi-dimension array
        '2. dictionary with a Tuple Key?
        '3. dictionary with struct as the key, struct would encapsulate the dimensions - this would map to the HVAC inputs
        'Need to test different choices for speed/ease of use

        'Initial Mock Implementation - 2 input parameters, 2 output values
        'probably easiest to implement the inputs and outputs as structs and then create a dictionary<input,output> ?

        'could define the list of inputs based on the supplied map


#End Region

        'Private Fields
        Public Property _mapHeaders As Dictionary(Of String, HVACMapParameter)
        Public Property MapHeaders As Dictionary(Of String, HVACMapParameter) Implements IHVACMap.MapHeaders
            Get
                Return Me._mapHeaders
            End Get
            Private Set(ByVal Value As Dictionary(Of String, HVACMapParameter))
                Me._mapHeaders = Value
            End Set
        End Property

        Private _map As List(Of String())
        Private _mapDimensions As Integer
        Private _mapPath As String
        Private _mechanicalDemandLookupKW As Single
        Private _electricalDemandLookupKW As Single


        'Constructor
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="iMapPath"></param>
        ''' <remarks></remarks>
        Public Sub New(iMapPath As String)

            _mapPath = iMapPath
            MapHeaders = New Dictionary(Of String, HVACMapParameter)()
            _map = New List(Of String())

            InitialiseMapHeaders()

            Initialise()

            Dim result As Object = (From line In _map Where line(0) = "2")


        End Sub

        'Initialisers and Map related Methods
        Public Function Initialise() As Boolean Implements IHVACMap.Initialise

            Dim myData As String
            Dim linesArray As String()

            Try

                myData = System.IO.File.ReadAllText(_mapPath, System.Text.Encoding.UTF8)
                linesArray = (From s As String In myData.Split(vbLf) Select s.Trim).ToArray



                'getValuesIntoMap
                For Each line As String In linesArray
                    _map.Add(line.Split(","c))
                Next

                _mapDimensions = _map(0).Length

                'Validate Map
                If validateMap() = False Then
                    Throw New Exception("Unable to complete Load of HVAC MAP")
                End If

                'Set Unique Values for headers which can be used as a selection device.


            Catch ffe As FileNotFoundException

                Throw ffe

            Catch ex As Exception

                Throw ex

            End Try

            Return True

        End Function
        Private Function validateMap() As Boolean

            'Lets make sure we have header and also data.
            If _map.Count < 2 Then
                MessageBox.Show("Map did not contain any data")
                Return False
            End If

            'Make sure we have matching headers for our list of HVACMapParameterList
            Dim i As Integer

            For i = 0 To _mapDimensions - 1

                If Not MapHeaders.ContainsKey(_map(0)(i)) Then
                    MessageBox.Show("Map header {0} was not found in the prefdefined and expected header list", _map(0)(i))
                    Return False
                Else
                    'Set ordinal position of respective header.
                    MapHeaders(_map(0)(i)).OrdinalPosition = i

                    'Get Unique Values associated with Headers.
                    MapHeaders(_map(0)(i)).UniqueDataValues = GetUniqueValuesByOrdinal(i)

                End If

            Next

            Return True

        End Function
        Public Sub InitialiseMapHeaders()

            'Region, CountryCode, Season, MechD, ElecD
            Dim region As New HVACMapParameter With {.Key = "Region",
                                                     .Description = "Region Code",
                                                     .Max = 0,
                                                     .Min = 0,
                                                     .Name = "Regional Code",
                                                     .Notes = "",
                                                     .SystemType = GetType(Integer),
                                                     .SearchControlType = GetType(System.Windows.Forms.ComboBox)}

            MapHeaders.Add("Region", region)

            Dim countryCode As New HVACMapParameter With {.Key = "CountryCode",
                                                     .Description = "Country Code",
                                                     .Max = 0,
                                                     .Min = 0,
                                                     .Name = "Country Code",
                                                     .Notes = "",
                                                     .SystemType = GetType(Integer),
                                                     .SearchControlType = GetType(System.Windows.Forms.ComboBox)}
            MapHeaders.Add("CountryCode", countryCode)

            Dim season As New HVACMapParameter With {.Key = "Season",
                                                     .Description = "Season Code",
                                                     .Max = 0,
                                                     .Min = 0,
                                                     .Name = "Season Code",
                                                     .Notes = "",
                                                     .SystemType = GetType(Integer),
                                                     .SearchControlType = GetType(System.Windows.Forms.ComboBox)}
            MapHeaders.Add("Season", season)


            Dim mechD As New HVACMapParameter With {.Key = "MechD",
                                                     .Description = "MechD",
                                                     .Max = 0,
                                                     .Min = 0,
                                                     .Name = "MechD",
                                                     .Notes = "",
                                                     .SystemType = GetType(Integer),
                                                     .IsOutput = True}

            MapHeaders.Add("MechD", mechD)

            Dim elecD As New HVACMapParameter With {.Key = "ElecD",
                                                     .Description = "ElecD",
                                                     .Max = 0,
                                                     .Min = 0,
                                                     .Name = "ElecD",
                                                     .Notes = "",
                                                     .SystemType = GetType(Integer),
                                                     .IsOutput = True}

            MapHeaders.Add("ElecD", elecD)


        End Sub


        'Map Enquiry Methods
        Public Function GetMapHeaders() As Dictionary(Of String, HVACMapParameter) Implements IHVACMap.GetMapHeaders

            Return MapHeaders

        End Function
        Public Function GetMapSubSet(search As String()) As List(Of String()) Implements IHVACMap.GetMapSubSet

            'Sanity Check
            If (search.Length <> _mapDimensions) Then
                Throw New Exception("The search array does not match the number elements in the map")
            End If

            Dim reducer As New List(Of String())
            Dim matched As Boolean
            Dim i As Integer ' For each data row

            For i = 1 To (_map.Count - 1)

                matched = True

                Dim o As Integer ' For each ordinal column
                For o = 0 To search.Length - 1
                    'Dont try and match it if it is an output or nothing, what would be the point?
                    If search(o) = Nothing Or search(o) = "" Or GetMapHeaders.ToArray()(o).Value.IsOutput Then
                        Continue For ' Get next ordinal
                    Else
                        'Try and match
                        If search(o) <> _map(i)(o) Then
                            matched = False
                        End If
                    End If

                Next o

                If matched Then
                    reducer.Add(_map(i))
                End If

            Next i

            Return reducer

        End Function
        Public Function GetUniqueValuesByOrdinal(o As Integer) As List(Of String) Implements IHVACMap.GetUniqueValuesByOrdinal

            'Sanity Check
            If (o < 0 Or o >= _mapDimensions) Then
                Throw New Exception(Format("Get Unique Values by ordinal ordinal passed {0} is outside the bounds of the array.", o))
            End If

            Dim results As List(Of String) =
             (From r In _map Select r(o) Distinct).ToList()

            'Remove Headers
            results.Remove(results(0))

            Return results

        End Function

        Public Function GetMechanicalDemand(region As Integer, season As Integer) As Integer Implements IHVACMap.GetMechanicalDemand
            Return 1
        End Function

        Public Function GetElectricalDemand(region As Integer, season As Integer) As Integer Implements IHVACMap.GetElectricalDemand
            Return 1
        End Function

    End Class

End Namespace