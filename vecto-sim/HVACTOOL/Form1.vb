
Imports VectoAuxiliaries.Hvac
Imports System.ComponentModel




Public Class Form1

Private Const ALLONLIST As String = "SSMTechBenefitsALLON.CSV"
Private genInputs As ISSMGenInputs = New SSMGenInputs(True)
Private ssmTechList As ISSMTechList = New SSMTechList(ALLONLIST, genInputs)


Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    If ssmTechList.Initialise() = False Then MsgBox("Failed to initalise list")

    SetUpControls
    SetUpBindings

End Sub

Public Sub SetUpControls()

      Dim cIndex As Integer = 0

      gvTechBenefits.AutoGenerateColumns = False


 'BenefitName As String
''LowFloor As BusFloorLow


     'ElectricalConsumerGrid 
     'Columns
     cIndex = gvTechBenefits.Columns.Add("BenefitName", "BenefitName")
     gvTechBenefits.Columns(cIndex).DataPropertyName = "BenefitName"
     gvTechBenefits.Columns(cIndex).MinimumWidth = 150
     gvTechBenefits.Columns(cIndex).ReadOnly = false
     gvTechBenefits.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvTechBenefits.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)

     cIndex = gvTechBenefits.Columns.Add("LowFloorV" ,"LowFloorV")

     gvTechBenefits.Columns(cIndex).DataPropertyName = "LowFloorV"
     gvTechBenefits.Columns(cIndex).MinimumWidth = 70
     gvTechBenefits.Columns(cIndex).ReadOnly = false
     gvTechBenefits.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvTechBenefits.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)



End Sub

Public Sub SetUpBindings()

     'Dim electricalConsumerBinding As New BindingList(Of IElectricalConsumer)(auxConfig.ElectricalUserInputsConfig.ElectricalConsumers.Items)
     'gvElectricalConsumables.DataSource = electricalConsumerBinding


     'Electrical ConsumablesGrid
     Dim techListBinding As New BindingList(Of ITechListBenefitLine)(ssmTechList.TechLines)

     gvTechBenefits.DataSource = techListBinding


End Sub


End Class
