Imports System.Windows.Forms
Imports System.Drawing




Public Class DeleteCell
 Inherits DataGridViewButtonCell


        Dim del As Image = My.Resources.ResourceManager.GetObject("Delete")
        'Image.("..\\..\\images\\delete.png")

 
         Protected Overrides Sub Paint(graphics As Graphics, clipBounds As Rectangle, cellBounds As Rectangle, rowIndex As Integer, elementState As DataGridViewElementStates, value As Object, formattedValue As Object, errorText As String, cellStyle As DataGridViewCellStyle, advancedBorderStyle As DataGridViewAdvancedBorderStyle, paintParts As DataGridViewPaintParts)

        advancedBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single

        Me.ToolTipText="Delete this Tech benefit line."
    
        cellStyle.BackColor= Color.White
        MyBase.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts)
        graphics.DrawImage(del, cellBounds)

    End Sub
 

End Class

