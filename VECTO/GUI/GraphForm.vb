' Copyright 2017 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.

Imports System.Collections.Generic
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Linq
Imports System.Windows.Forms.DataVisualization.Charting
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Utils

Public Class GraphForm
	Private _filepath As String
	Private ReadOnly _channels As List(Of Channel)
	Private _distanceList As List(Of Single)
	Private _timeList As List(Of Single)

	Private _xMin As Double
	Private _xMax As Double

	Private _xMax0 As Double


	Public Sub New()

		' Dieser Aufruf ist für den Designer erforderlich.
		InitializeComponent()

		' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
		_channels = New List(Of Channel)

		Clear()

		CbXaxis.SelectedIndex = 0
	End Sub

	Private Sub ToolStripBtOpen_Click(sender As Object, e As EventArgs) Handles ToolStripBtOpen.Click

		If ModalResultsFileBrowser.OpenDialog(_filepath) Then

			LoadNewFile(ModalResultsFileBrowser.Files(0))

		End If
	End Sub


	Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
		LoadFile()
	End Sub

	Public Sub LoadNewFile(ByVal path As String)
		Dim lv0 As ListViewItem
		Dim i As Integer = 0

		Clear()

		_filepath = path

		LoadFile()

		For Each channel As Channel In _channels
			If (channel.Name = "v_act [km/h]" OrElse channel.Name = "v_targ [km/h]") Then
				lv0 = New ListViewItem
				lv0.Text = channel.Name
				lv0.SubItems.Add("Left")
				lv0.Tag = i
				lv0.Checked = True
				ListView1.Items.Add(lv0)
			End If
			i += 1
		Next
	End Sub

	Private Sub LoadFile()

		Try

			_channels.Clear()

			Dim data As TableData = VectoCSVFile.Read(_filepath)
			For Each column As DataColumn In data.Columns
				Dim values As List(Of String) = New List(Of String)
				For Each row As DataRow In data.Rows
					values.Add(row(column).ToString())
				Next
				_channels.Add(New Channel() With {
								.Name = column.ColumnName,
								.Values = values})
			Next


			_timeList = Nothing
			_distanceList = Nothing

			For Each channel As Channel In _channels
				If (channel.Name = "time" AndAlso _timeList Is Nothing) Then
					_timeList = channel.Values.Select(Function(x) CSng(x)).ToList()
				End If
				If (channel.Name = "dist" AndAlso _distanceList Is Nothing) Then
					_distanceList = channel.Values.Select(Function(x) CSng(x)).ToList()
				End If
			Next

			SetxMax0()

			TbXmin.Text = 0.ToGUIFormat()
			TbXmax.Text = _xMax0.ToGUIFormat()

			Text = GetFilenameWithoutPath(_filepath, True)

		Catch ex As Exception

			Exit Sub
		End Try


		UpdateGraph()
	End Sub

	Private Sub SetxMax0()

		If _channels.Count = 0 Then Exit Sub

		If CbXaxis.SelectedIndex = 0 Then
			_xMax0 = _distanceList(_distanceList.Count - 1)
		Else
			_xMax0 = _timeList(_timeList.Count - 1)
		End If
	End Sub

	Private Sub UpdateGraph()
		Dim listViewItem As ListViewItem
		Dim leftaxis As New List(Of String)
		Dim rightaxis As New List(Of String)
		Dim txt As String
		Dim i As Integer

		If WindowState = FormWindowState.Minimized Then Exit Sub

		If ListView1.CheckedItems.Count = 0 Then
			PictureBox1.Image = Nothing
			Exit Sub
		End If

		Dim overDist As Boolean = (CbXaxis.SelectedIndex = 0)

		SetxMax0()


		Dim chart As Chart = New Chart
		chart.Width = PictureBox1.Width
		chart.Height = PictureBox1.Height

		Dim chartArea As ChartArea = New ChartArea


		For Each listViewItem In ListView1.CheckedItems

			Dim isLeft As Boolean = (listViewItem.SubItems(1).Text = "Left")

			Dim chartSeries As Series = New Series

			If overDist Then
				chartSeries.Points.DataBindXY(_distanceList, _channels(CType(listViewItem.Tag, Integer)).Values)
			Else
				chartSeries.Points.DataBindXY(_timeList, _channels(CType(listViewItem.Tag, Integer)).Values)
			End If

			chartSeries.ChartType = SeriesChartType.FastLine
			chartSeries.Name = listViewItem.Text
			chartSeries.BorderWidth = 2

			If isLeft Then
				If Not leftaxis.Contains(listViewItem.SubItems(0).Text) Then leftaxis.Add(listViewItem.SubItems(0).Text)
			Else
				If Not rightaxis.Contains(listViewItem.SubItems(0).Text) Then rightaxis.Add(listViewItem.SubItems(0).Text)
				chartSeries.YAxisType = AxisType.Secondary
			End If

			chart.Series.Add(chartSeries)

		Next


		chartArea.Name = "main"

		If overDist Then
			chartArea.AxisX.Title = "distance [m]"
		Else
			chartArea.AxisX.Title = "time [s]"
		End If
		chartArea.AxisX.TitleFont = New Font("Helvetica", 10)
		chartArea.AxisX.LabelStyle.Font = New Font("Helvetica", 8)
		chartArea.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.None
		chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot

		If _xMax > _xMin Then
			chartArea.AxisX.Minimum = _xMin
			chartArea.AxisX.Maximum = _xMax
			chartArea.AxisX.Interval = AutoIntervalXAxis()
		Else
			chartArea.AxisX.Minimum = 0
			chartArea.AxisX.Maximum = _xMax0
		End If


		If leftaxis.Count > 0 Then

			txt = leftaxis(0)
			For i = 1 To leftaxis.Count - 1
				txt &= ", " & leftaxis(i)
			Next

			chartArea.AxisY.Title = txt
			chartArea.AxisY.TitleFont = New Font("Helvetica", 10)
			chartArea.AxisY.LabelStyle.Font = New Font("Helvetica", 8)
			chartArea.AxisY.LabelAutoFitStyle = LabelAutoFitStyles.None

		End If

		If rightaxis.Count > 0 Then

			txt = rightaxis(0)
			For i = 1 To rightaxis.Count - 1
				txt &= ", " & rightaxis(i)
			Next

			chartArea.AxisY2.Title = txt
			chartArea.AxisY2.TitleFont = New Font("Helvetica", 10)
			chartArea.AxisY2.LabelStyle.Font = New Font("Helvetica", 8)
			chartArea.AxisY2.LabelAutoFitStyle = LabelAutoFitStyles.None
			chartArea.AxisY2.MinorGrid.Enabled = False
			chartArea.AxisY2.MajorGrid.Enabled = False

		End If

		chartArea.BackColor = Color.GhostWhite

		chartArea.BorderDashStyle = ChartDashStyle.Solid
		chartArea.BorderWidth = 1

		chart.ChartAreas.Add(chartArea)

		With chart.ChartAreas(0)
			.Position.X = 0
			.Position.Y = 0
			.Position.Width = 85
			.Position.Height = 100
		End With

		chart.Legends.Add("main")
		chart.Legends(0).Font = New Font("Helvetica", 8)
		chart.Legends(0).BorderColor = Color.Black
		chart.Legends(0).BorderWidth = 1
		chart.Legends(0).Position.X = 86
		chart.Legends(0).Position.Y = 3
		chart.Legends(0).Position.Width = 13
		chart.Legends(0).Position.Height = 40

		chart.Update()

		Dim img As Bitmap = New Bitmap(chart.Width, chart.Height, PixelFormat.Format32bppArgb)
		chart.DrawToBitmap(img, New Rectangle(0, 0, PictureBox1.Width, PictureBox1.Height))

		PictureBox1.Image = img
	End Sub

	Private Function AutoIntervalXAxis() As Double
		Dim xyd(3) As Double
		Dim xya(3) As Double
		Dim i As Int16

		Dim inv As Double = (_xMax - _xMin)/10

		Dim grx As Long = 20
		Do While 10^grx > inv
			grx = grx - 1
		Loop

		xyd(0) = 1*10^grx
		xyd(1) = 2.5*10^grx
		xyd(2) = 5*10^grx
		xyd(3) = 10*10^grx
		For i = 0 To 3
			xya(i) = Math.Abs(inv - xyd(i))
		Next

		Dim xyamin As Double = xya(0)
		Dim xydmin As Double = xyd(0)
		For i = 1 To 3
			If xya(i) < xyamin Then
				xyamin = xya(i)
				xydmin = xyd(i)
			End If
		Next

		'Intervall speichern
		Return xydmin
	End Function

	Private Sub Clear()

		_filepath = ""

		ListView1.Items.Clear()

		TbXmin.Text = ""
		TbXmax.Text = ""

		PictureBox1.Image = Nothing
	End Sub


	Private Sub BtAddCh_Click(sender As Object, e As EventArgs) Handles BtAddCh.Click
		Dim dlog As New GraphEditChannelDialog
		Dim i As Integer
		Dim lv0 As ListViewItem

		If _channels.Count = 0 Then Exit Sub

		For i = 0 To _channels.Count - 1
			dlog.ComboBox1.Items.Add(_channels(i).Name)
		Next

		dlog.RbLeft.Checked = True

		dlog.ComboBox1.SelectedIndex = 0

		If dlog.ShowDialog = DialogResult.OK Then
			lv0 = New ListViewItem
			i = dlog.ComboBox1.SelectedIndex
			lv0.Text = _channels(i).Name
			lv0.Tag = i
			lv0.Checked = True
			If dlog.RbLeft.Checked Then
				lv0.SubItems.Add("Left")
			Else
				lv0.SubItems.Add("Right")
			End If

			ListView1.Items.Add(lv0)

			UpdateGraph()

		End If
	End Sub

	Private Sub EditChannel()
		Dim dlog As New GraphEditChannelDialog
		Dim i As Integer
		Dim lv0 As ListViewItem

		If ListView1.SelectedItems.Count = 0 Or _channels.Count = 0 Then Exit Sub

		lv0 = ListView1.SelectedItems(0)

		For i = 0 To _channels.Count - 1
			dlog.ComboBox1.Items.Add(_channels(i).Name)
		Next

		If lv0.SubItems(1).Text = "Left" Then
			dlog.RbLeft.Checked = True
		Else
			dlog.RbRight.Checked = True
		End If

		dlog.ComboBox1.SelectedIndex = CType(lv0.Tag, Integer)

		If dlog.ShowDialog = DialogResult.OK Then
			i = dlog.ComboBox1.SelectedIndex
			lv0.Text = _channels(i).Name
			lv0.Tag = i
			lv0.Checked = True
			If dlog.RbLeft.Checked Then
				lv0.SubItems(1).Text = "Left"
			Else
				lv0.SubItems(1).Text = "Right"
			End If

			UpdateGraph()

		End If
	End Sub

	Private Sub RemoveChannel()
		Dim i0 As Integer

		If ListView1.Items.Count = 0 Then Exit Sub

		If ListView1.SelectedItems.Count = 0 Then ListView1.Items(ListView1.Items.Count - 1).Selected = True

		i0 = ListView1.SelectedItems(0).Index

		ListView1.SelectedItems(0).Remove()

		If i0 < ListView1.Items.Count Then
			ListView1.Items(i0).Selected = True
			ListView1.Items(i0).EnsureVisible()
		End If

		UpdateGraph()
	End Sub

	Private Sub ListView1_DoubleClick(sender As Object, e As EventArgs) Handles ListView1.DoubleClick
		If ListView1.SelectedItems.Count > 0 Then
			ListView1.SelectedItems(0).Checked = Not ListView1.SelectedItems(0).Checked
			EditChannel()
		End If
	End Sub

	Private Sub BtRemCh_Click(sender As Object, e As EventArgs) Handles BtRemCh.Click
		RemoveChannel()
	End Sub

	Private Sub ListView1_ItemChecked(sender As Object, e As ItemCheckedEventArgs) _
		Handles ListView1.ItemChecked
		UpdateGraph()
	End Sub

	Private Sub ListView1_KeyDown(sender As Object, e As KeyEventArgs) Handles ListView1.KeyDown
		Select Case e.KeyCode
			Case Keys.Delete, Keys.Back
				RemoveChannel()
			Case Keys.Enter
				EditChannel()
		End Select
	End Sub

	Private Sub CbXaxis_SelectedIndexChanged(sender As Object, e As EventArgs) _
		Handles CbXaxis.SelectedIndexChanged
		SetxMax0()
		TbXmin.Text = 0.ToGUIFormat()
		TbXmax.Text = _xMax0.ToGUIFormat()
		UpdateGraph()
	End Sub

	Private Sub BtReset_Click(sender As Object, e As EventArgs) Handles BtReset.Click
		_xMin = 0
		_xMax = _xMax0
		TbXmin.Text = 0.ToGUIFormat()
		TbXmax.Text = _xMax0.ToGUIFormat()
	End Sub

	Private Sub TbXmin_TextChanged(sender As Object, e As EventArgs) Handles TbXmin.TextChanged
		If IsNumeric(TbXmin.Text) Then _xMin = TbXmin.Text.ToDouble()
		UpdateGraph()
	End Sub

	Private Sub TbXmax_TextChanged(sender As Object, e As EventArgs) Handles TbXmax.TextChanged
		If IsNumeric(TbXmax.Text) Then _xMax = TbXmax.Text.ToDouble()
		UpdateGraph()
	End Sub

	Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
		Dim graph As New GraphForm
		graph.Show()
	End Sub

	Private Sub F_Graph_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
		UpdateGraph()
	End Sub

	Private Sub BtZoomIn_Click(sender As Object, e As EventArgs) Handles BtZoomIn.Click
		Dim d As Double

		d = (_xMax - _xMin)/10

		_xMin += 2*0.5*d
		_xMax -= 2*(1 - 0.5)*d

		If _xMin > 1000 Then
			_xMin = Math.Round(_xMin/100, 0)*100
		Else
			_xMin = Math.Round(_xMin, 0)
		End If

		TbXmin.Text = _xMin.ToGUIFormat()
		TbXmax.Text = _xMax.ToGUIFormat()
	End Sub

	Private Sub BtZoomOut_Click(sender As Object, e As EventArgs) Handles BtZoomOut.Click
		Dim d As Double

		d = (_xMax - _xMin)/10

		_xMin -= 2*0.5*d
		_xMax += 2*(1 - 0.5)*d

		If _xMin > 1000 Then
			_xMin = Math.Round(_xMin/100, 0)*100
		Else
			_xMin = Math.Round(_xMin, 0)
		End If

		TbXmin.Text = _xMin.ToGUIFormat()
		TbXmax.Text = _xMax.ToGUIFormat()
	End Sub

	Private Sub BtMoveL_Click(sender As Object, e As EventArgs) Handles BtMoveL.Click
		Dim d As Double

		If _xMin <= 0 Then Exit Sub

		d = (_xMax - _xMin)/3
		_xMin -= d
		_xMax -= d

		If _xMin > 1000 Then
			_xMin = Math.Round(_xMin/100, 0)*100
		Else
			_xMin = Math.Round(_xMin, 0)
		End If

		TbXmin.Text = _xMin.ToGUIFormat()
		TbXmax.Text = _xMax.ToGUIFormat()
	End Sub

	Private Sub BtMoveR_Click(sender As Object, e As EventArgs) Handles BtMoveR.Click
		Dim d As Double

		If _xMax >= _xMax0 Then Exit Sub

		d = (_xMax - _xMin)/3
		_xMin += d
		_xMax += d

		If _xMin > 1000 Then
			_xMin = Math.Round(_xMin/100, 0)*100
		Else
			_xMin = Math.Round(_xMin, 0)
		End If

		TbXmin.Text = _xMin.ToGUIFormat()
		TbXmax.Text = _xMax.ToGUIFormat()
	End Sub

	Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
		If File.Exists(Path.Combine(MyAppPath, "User Manual\help.html")) Then
			Dim defaultBrowserPath As String = BrowserUtils.GetDefaultBrowserPath()
			Process.Start(defaultBrowserPath,
						$"""file://{Path.Combine(MyAppPath, "User Manual\help.html#graph-window")}""")
		Else
			MsgBox("User Manual not found!", MsgBoxStyle.Critical)
		End If
	End Sub

	Private Class Channel
		Public Name As String
		Public Values As List(Of String)
	End Class
End Class
