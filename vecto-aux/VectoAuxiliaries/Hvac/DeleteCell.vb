Imports System.Windows.Forms
Imports System.Drawing




Public Class DeleteCell
 Inherits DataGridViewButtonCell

     Public property ToolTip As String = "Delete tech benefit line"
     Private del As Image = My.Resources.ResourceManager.GetObject("Delete")


     Protected Overrides Sub Paint(graphics As Graphics, clipBounds As Rectangle, cellBounds As Rectangle, rowIndex As Integer, elementState As DataGridViewElementStates, value As Object, formattedValue As Object, errorText As String, cellStyle As DataGridViewCellStyle, advancedBorderStyle As DataGridViewAdvancedBorderStyle, paintParts As DataGridViewPaintParts)

        advancedBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single

        Me.ToolTipText=ToolTip
    
        cellStyle.BackColor= Color.White
        MyBase.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts)
        graphics.DrawImage(del, cellBounds)

    End Sub
 

End Class

Public Class DeleteAlternatorCell
 Inherits DataGridViewButtonCell

     Public property ToolTip As String = "Delete alternator"
     Private del As Image = My.Resources.ResourceManager.GetObject("Delete")


     Protected Overrides Sub Paint(graphics As Graphics, clipBounds As Rectangle, cellBounds As Rectangle, rowIndex As Integer, elementState As DataGridViewElementStates, value As Object, formattedValue As Object, errorText As String, cellStyle As DataGridViewCellStyle, advancedBorderStyle As DataGridViewAdvancedBorderStyle, paintParts As DataGridViewPaintParts)

        advancedBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single

        Me.ToolTipText=ToolTip
    
        cellStyle.BackColor= Color.White
        MyBase.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts)
        graphics.DrawImage(del, cellBounds)

    End Sub
 

End Class