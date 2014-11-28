Imports System.IO
Imports AdvancedAuxiliaryInterfaces.Electrics
Imports System.Windows.Forms

Namespace Hvac

    Public Interface IHVACMap

        Property MapHeaders As Dictionary(Of String, HVACMapParameter)

        Function Initialise() As Boolean

        'Map Enquiry Methods
        Function GetMapHeaders() As Dictionary(Of String, HVACMapParameter)
        Function GetMapSubSet(search As String()) As List(Of String())
        Function GetUniqueValuesByOrdinal(o As Integer) As List(Of String)

        Function GetMechanicalDemand(ByVal region As Integer, ByVal season As Integer) As Integer
        Function GetElectricalDemand(ByVal region As Integer, ByVal season As Integer) As Integer


    End Interface



End Namespace

