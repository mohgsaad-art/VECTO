Imports VectoAuxiliaries.Hvac
Imports System.Windows.Forms

Namespace UI

    Public Class F_HVAC

        'Private Fields
        Private _mapPath As String
        Private _cfgPath As String
        Private _map As HVACMap
        Private _mapFilter As List(Of String) = New List(Of String)()

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

            Dim results = _map.GetMapSubSet(_mapFilter.ToArray())

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


        End Sub
        Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click


        End Sub
        Private Sub btnBrowseMap_Click(sender As Object, e As EventArgs) Handles btnBrowseMap.Click

            Dim openFileDialog1 = New OpenFileDialog()

            openFileDialog1.InitialDirectory = "."
            openFileDialog1.Filter = "Map Files (*.vaux)|*.vaux"
            openFileDialog1.FilterIndex = 1
            openFileDialog1.RestoreDirectory = True

            openFileDialog1.ShowDialog(Me)
            MapPath = openFileDialog1.FileName

            openFileDialog1.Dispose()

            _map = New HVACMap(_mapPath)

            _map.Initialise()

            _mapFilter.Clear()
            For Each item As KeyValuePair(Of String, HVACMapParameter) In _map.GetMapHeaders
                _mapFilter.Add("")
            Next

            BuildSearchBar()

        End Sub


        Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
            Me.Close()
        End Sub
    End Class


End Namespace
