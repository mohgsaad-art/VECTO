Namespace Hvac

    Public Class Bus
        Implements IBus

        'Private Property Backing
        Private _id As Integer
        Private _model As String
        Private _floorType As String
        Private _engineType As String
        Private _lengthInMetres As String
        Private _widthInMetres As String
        Private _heightInMetres As String
        Private _registeredPassengers As Integer
        Private _isDoubleDecker As Boolean

        Public ReadOnly Property Id As Integer Implements IBus.Id
            Get
                Return _id
            End Get
        End Property

        Public Property Model As String Implements IBus.Model
            Get
                Return _model
            End Get
            Set(value As String)
                If Not ModelOK(value) Then Throw New ArgumentException("Model argument is invalid")
                _model = value
            End Set
        End Property

        Public Property FloorType As String Implements IBus.FloorType
            Get
                Return _floorType
            End Get
            Set(value As String)
                If Not FloorTypeOK(value) Then Throw New ArgumentException("Model argument is invalid")
                _floorType = value
            End Set
        End Property

        Public Property EngineType As String Implements IBus.EngineType
            Get
                Return _engineType
            End Get
            Set(value As String)
                If Not EngineOK(value) Then Throw New ArgumentException("EngineType argument is invalid")
                _engineType = value
            End Set
        End Property

        Public Property LengthInMetres As Double Implements IBus.LengthInMetres
            Get
                Return _lengthInMetres
            End Get
            Set(value As Double)
                If Not DimensionOK(value) Then Throw New ArgumentException("Invalid Length")
                _lengthInMetres = value
            End Set
        End Property

        Public Property WidthInMetres As Double Implements IBus.WidthInMetres
            Get
                Return _widthInMetres
            End Get
            Set(value As Double)
                If Not DimensionOK(value) Then Throw New ArgumentException("Invalid Width")
                _widthInMetres = value
            End Set
        End Property

        Public Property HeightInMetres As Double Implements IBus.HeightInMetres
            Get
                Return _heightInMetres
            End Get
            Set(value As Double)
                If Not DimensionOK(value) Then Throw New ArgumentException("Invalid Height")
                _heightInMetres = value
            End Set
        End Property

        Public Property RegisteredPassengers As Integer Implements IBus.RegisteredPassengers
            Get
                Return _registeredPassengers
            End Get
            Set(value As Integer)
                If Not PassengersOK(value) Then Throw New ArgumentException("Invalid Number Of Passengers")
                _registeredPassengers = value
            End Set
        End Property

        Public Property IsDoubleDecker As Boolean Implements IBus.IsDoubleDecker
            Get
                Return _isDoubleDecker
            End Get
            Set(ByVal value As Boolean)
                _isDoubleDecker = value
            End Set
        End Property

        'Constructors
        Public Sub New(_id As Integer,
                        _model As String,
                        _floorType As String,
                        _engineType As String,
                        _lengthInMetres As String,
                        _widthInMetres As String,
                        _heightInMetres As String,
                        _registeredPassengers As Integer,
                        _doubleDecker As Boolean)


            'Validity checks.
            If Not ModelOK(_model) Then Throw New ArgumentException("Model argument is invalid")
            If Not FloorTypeOK(_floorType) Then Throw New ArgumentException("Model argument is invalid")
            If Not EngineOK(_engineType) Then Throw New ArgumentException("EngineType argument is invalid")
            If Not DimensionOK(_lengthInMetres) Then Throw New ArgumentException("Invalid Length")
            If Not DimensionOK(_widthInMetres) Then Throw New ArgumentException("Invalid Width")
            If Not DimensionOK(_heightInMetres) Then Throw New ArgumentException("Invalid Height")
            If Not PassengersOK(_registeredPassengers) Then Throw New ArgumentException("Invalid Number Of Passengers")


            'Set Private Members
            Me._id = _id
            Me._model = _model
            Me._floorType = _floorType
            Me._engineType = _engineType
            Me._lengthInMetres = _lengthInMetres
            Me._widthInMetres = _widthInMetres
            Me._heightInMetres = _heightInMetres
            Me._registeredPassengers = _registeredPassengers
            Me._isDoubleDecker = _doubleDecker

        End Sub

        'Construction Validators Helpers                                     
        Private Function ModelOK(ByVal model As String) As Boolean

            model = model.ToLower

            If model Is Nothing OrElse model.Trim.Length = 0 Then Return False

            Return True

        End Function

        Private Function FloorTypeOK(ByVal floorType As String) As Boolean

            floorType = floorType.ToLower

            If floorType Is Nothing OrElse floorType.Trim.Length = 0 Then Return False

            If floorType <> "raised floor" AndAlso floorType <> "low floor" AndAlso floorType <> "semi low floor" Then Return False

            Return True

        End Function

        Private Function EngineOK(ByVal engine As String) As Boolean

            engine = engine.ToLower

            If engine Is Nothing OrElse engine.Trim.Length = 0 Then Return False

            If engine <> "diesel" AndAlso engine <> "gas" AndAlso engine <> "hybrid" Then Return False

            Return True

        End Function

        Private Function DimensionOK(ByVal dimension As Double) As Boolean

            Return dimension > 0.5

        End Function

        Private Function PassengersOK(ByVal registeredPassengers As Integer) As Boolean

            Return registeredPassengers > 1


        End Function

        'To String function
        Public Overrides Function ToString() As String

            Return String.Format("{0},{1},{2},{3},{4},{5},{6},{7}", _model, _floorType, _engineType, _lengthInMetres, _widthInMetres, _heightInMetres, _registeredPassengers, _isDoubleDecker)

        End Function

    End Class


End Namespace



