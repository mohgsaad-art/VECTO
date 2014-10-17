Imports VectoAuxiliaries.Hvac
Imports System.Windows.Forms

Namespace UI

    Public Class F_HVAC

        'Private Fields
        Private _mapPath As String
        Private _cfgPath As String
        Private _map As HVACMap
        Private _mapFilter As List(Of String) = New List(Of String)()
        Private _selectedInputs As New Dictionary(Of String, String)


        'Properties
        Public Property MapPath As String

            Get
                Return _mapPath
            End Get
            Set(value As String)
                _mapPath = value
                txtMapFile.Text = value
            End Set


        End Property

        Public Property Inputs As Dictionary(Of String, String)

            Get
                Return _selectedInputs
            End Get

            Private Set(value As Dictionary(Of String, String))
                _selectedInputs = value
            End Set

        End Property




        'Helpers
        Private Sub BuildSearchBar()

            'Clear out anything which may be resident
            pnlSearchBar.Controls.Clear()

            If _map Is Nothing Then
                MessageBox.Show("Map not yet loaded")
                Return
            End If

            'Ok Showtime


            'Get all the columns who's header is an input
            Dim results As Dictionary(Of String, HVACMapParameter) = (From p In _map.GetMapHeaders() Where Not p.Value.IsOutput Select p).ToDictionary(Function(p) p.Key, Function(p) p.Value)

            'Add Layuout Table inside the pnlSearchBar
            Dim filterPanelTable As TableLayoutPanel = New TableLayoutPanel()
            filterPanelTable.RowCount = 2
            filterPanelTable.ColumnCount = _map.GetMapHeaders().Count
            filterPanelTable.Dock = DockStyle.Fill

            pnlSearchBar.Controls.Add(filterPanelTable)

            For Each parameter As HVACMapParameter In results.Values

                'TB : 'Dies ist, wo die Magie passiert 
                Dim sp1 As Control = Convert.ChangeType(Activator.CreateInstance(parameter.SearchControlType), parameter.SearchControlType)

                sp1.Name = "FilterControl_" + parameter.OrdinalPosition.ToString()

                FillControl(sp1, parameter.UniqueDataValues, parameter)

                'Set Label
                filterPanelTable.Controls.Add((New Label() With {.Text = parameter.Name}), parameter.OrdinalPosition, 0)

                'Set Control
                filterPanelTable.Controls.Add(sp1, parameter.OrdinalPosition, 1)

            Next


        End Sub
        Private Sub FillControl(ByRef control As Control, values As List(Of String), param As HVACMapParameter)

            'ComboBox Filter Control
            If TypeOf control Is ComboBox Then

                Dim cb As ComboBox = CType(control, ComboBox)
                AddHandler cb.SelectionChangeCommitted, AddressOf Me.FilterComboHandler
                cb.Items.Add("<Select>")
                For Each item As String In values
                    cb.Items.Add(item)
                Next

                cb.SelectedIndex = 0

            End If

        End Sub
        Private Sub PopulateResultsTable(subsetResults As List(Of String()))

            Dim table As New DataTable
            For Each item As KeyValuePair(Of String, HVACMapParameter) In _map.GetMapHeaders
                Dim col As New DataColumn(item.Value.Name)
                table.Columns.Add(col)
            Next

            For Each row As String() In subsetResults
                Dim cell As Integer
                Dim newTableRow As DataRow = table.NewRow
                For cell = 0 To table.Columns.Count - 1
                    newTableRow.SetField(cell, row(cell))
                Next
                table.Rows.Add(newTableRow)
            Next

            dgMapResults.DataSource = table

            dgMapResults.ClearSelection()
            txtElectricalDemand.Text = String.Empty
            txtMechanicalDemand.Text = String.Empty


        End Sub


        'Constructors
        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.

        End Sub

        Public Sub New(ByVal iMapPath As String)

            Me.New()

            MapPath = iMapPath

        End Sub

        'Event Handlers
        '**************
        'Programatically attached when filer is built
        Private Sub FilterComboHandler(sender As Object, e As EventArgs)


            'Determine where in the filer we need to be
            Dim combo As ComboBox = CType(sender, ComboBox)
            Dim oridnal As Integer = combo.Name.Split("_")(1)

            If (combo.SelectedIndex > 0) Then
                _mapFilter(oridnal) = combo.Items(combo.SelectedIndex).ToString()

            Else
                _mapFilter(oridnal) = String.Empty
            End If

            Dim results as List(Of String()) = _map.GetMapSubSet(_mapFilter.ToArray())

            PopulateResultsTable(results)

        End Sub

        'Delcaratively attached
        Private Sub dgMapResults_SelectionChanged(sender As Object, e As EventArgs) Handles dgMapResults.SelectionChanged

            If (CType(sender, DataGridView).SelectedRows.Count = 1) Then
                txtMechanicalDemand.Text = CType(sender, DataGridView).SelectedRows(0).Cells("MechD").Value
                txtElectricalDemand.Text = CType(sender, DataGridView).SelectedRows(0).Cells("ElecD").Value
            End If

        End Sub
        Private Sub F_HVAC_Load(sender As Object, e As EventArgs) Handles MyBase.Load

            'If we have a map on load lets try and get the information
            If (MapPath.Length <> 0) Then

                GetMapData()

            End If


        End Sub


        Private Sub GetMapData()

            'Dim openFileDialog1 = New OpenFileDialog()

            'openFileDialog1.InitialDirectory = "."
            'openFileDialog1.Filter = "Map Files (*.vaux)|*.vaux"
            'openFileDialog1.FilterIndex = 1
            'openFileDialog1.RestoreDirectory = True

            'openFileDialog1.ShowDialog(Me)
            'MapPath = openFileDialog1.FileName

            'openFileDialog1.Dispose()

            Try

                _map = New HVACMap(_mapPath)
                _map.Initialise()

                _mapFilter.Clear()
                For Each item As KeyValuePair(Of String, HVACMapParameter) In _map.GetMapHeaders
                    _mapFilter.Add("")
                Next

                BuildSearchBar()

            Catch ex As Exception

                MessageBox.Show("An error has occured while trying to read the map file selected ")
                'TODO:Log this error message.
                DialogResult = Windows.Forms.DialogResult.Abort
                Me.Close()

            End Try





        End Sub



        Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

            DialogResult = Windows.Forms.DialogResult.OK

        End Sub
        Private Sub btnBrowseMap_Click(sender As Object, e As EventArgs) Handles btnBrowseMap.Click



        End Sub


        Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click

            DialogResult = Windows.Forms.DialogResult.Cancel
            Me.Close()

        End Sub


        Private Sub F_HVAC_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

            'If closing as a result of OK being pressed
            Select DialogResult


                Case Windows.Forms.DialogResult.OK

                    If (Me.dgMapResults.Rows.Count > 0 AndAlso Me.dgMapResults.SelectedRows.Count = 1) Then

                        Inputs.Clear()

                        'Build a list of Inputs
                        For Each p As HVACMapParameter In _map.MapHeaders.OrderBy(Function(x) x.Value.OrdinalPosition).Select(Function(x) x.Value)
                            Dim val As String = Me.dgMapResults.SelectedRows(0).Cells(p.OrdinalPosition).Value.ToString()
                            If Not p.IsOutput Then
                                Inputs.Add(p.Name, val)
                            End If
                        Next
                    Else
                        'Cancel the event as nothing has been selected althoug the users has pressed the ok button indicating a submit.
                        MessageBox.Show("You do not have a selected row.")
                        e.Cancel = True
                    End If

                Case Else
                    Inputs.Clear()
                    txtMapFile.Text = String.Empty



            End Select







        End Sub

    End Class


End Namespace
