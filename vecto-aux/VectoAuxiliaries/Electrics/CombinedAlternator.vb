
imports System
imports System.Collections.Generic
imports System.Linq
imports System.Text
imports System.IO
imports DocumentFormat.OpenXml
imports DocumentFormat.OpenXml.Spreadsheet
imports SpreadsheetLight

Namespace Electrics

Public Class CombinedAlternator



'Simulation Spreadsheet constants.
Private const powerCalculationSheetName          As string = "Power Calculation"
Private const combinedAlternatorETAMAPSheetName  As string = "Combined Alternator ETA Map"
Private const alt1SheetName                      As string = "Alt 1"
Private const aLt2SheetName                      As string = "Alt 2"
Private const alt3SheetName                      As string = "Alt 3"
Private const aLt4SheetName                      As string = "Alt 4"
Private const alternator1Name                    As string = "Alternator#1"
Private const alternator2Name                    As string = "Alternotor#2"
Private const alternator3Name                    As string = "Alternator#3"
Private const alternator4Name                    As string = "Alternator#4"


'Simulation Spreadsheet variables
Private excelPath As String  = "alt.xlsx"

'SSLight Declarations
Private combinedSheet    As SLDocument
Private powerCalculation As SLDocument
Private Alt1             As SLDocument
Private ALt2             As SLDocument
Private Alt3             As SLDocument
Private ALt4             As SLDocument
Private alternator1      As SLDocument
Private alternator2      As SLDocument
Private alternator3      As SLDocument
Private alternator4      As SLDocument


'User Inputs
Private Property NumAlternators As Integer
Private Property Alt1PulleyRatio As Single 
Private Property Alt2PulleyRatio As Single 
Private Property Alt3PulleyRatio As Single 
Private Property Alt4PulleyRatio As Single 

'Constructor
Public Sub new( simulationExcelBookPath As string, alt1PulleyRatio As Single, alt2PulleyRatio As Single, alt3PulleyRatio As Single, alt4PulleyRatio As single, numAlternators As Integer)





   'Sanity Checks
   If( alt1PulleyRatio <0.1 orElse alt1PulleyRatio >10)  then throw new ArgumentException("Please enter a sensible Alternator Pulley Ratio")
   If( alt2PulleyRatio <0.1 orElse alt2PulleyRatio >10)  then throw new ArgumentException("Please enter a sensible Alternator Pulley Ratio")
   If( alt3PulleyRatio <0.1 orElse alt3PulleyRatio >10)  then throw new ArgumentException("Please enter a sensible Alternator Pulley Ratio")
   If( alt4PulleyRatio <0.1 orElse alt4PulleyRatio >10)  then throw new ArgumentException("Please enter a sensible Alternator Pulley Ratio")
   If( numAlternators<1     orElse numAlternators  >4 )  then Throw New ArgumentException("Number of alternators must be between 1 and 4, these are the limits supported by this tool")

   If NOT simulationExcelBookPath is Nothing AndAlso simulationExcelBookPath.Length>6 then

      excelPath = simulationExcelBookPath

   End If

   'if we get here lets try and initialise the spreadsheet.

    me.NumAlternators   = numAlternators
    me.Alt1PulleyRatio  = alt1PulleyRatio
    me.Alt2PulleyRatio  = alt2PulleyRatio
    me.Alt3PulleyRatio  = alt3PulleyRatio
    me.Alt4PulleyRatio  = alt4PulleyRatio

   SetUserConfigurables()

End sub

Public Sub SetUserConfigurables()



        powerCalculation = New SLDocument(excelPath, powerCalculationSheetName)

        'Set Number of Alternators
        powerCalculation.SetCellValue(4,2, NumAlternators)
      
        'Set Pulley Efficiencies
        powerCalculation.SetCellValue(5,2, Alt1PulleyRatio)
        powerCalculation.SetCellValue(6,2, Alt2PulleyRatio)
        powerCalculation.SetCellValue(7,2, Alt3PulleyRatio)
        powerCalculation.SetCellValue(8,2, Alt4PulleyRatio)       
        powerCalculation.Save()

        powerCalculation = New SLDocument(excelPath, powerCalculationSheetName)
        Alt1 = New SLDocument(excelPath, alt1SheetName)
        Calculate(Alt1,39,2)
        Alt1.Save()

        Alt2 = New SLDocument(excelPath, alt2SheetName)
        Calculate(Alt2,39,7)
        ALt2.Save()

        Alt3 = New SLDocument(excelPath, alt3SheetName)
        Calculate(Alt3,52,2)
        Alt3.Save()

        Alt4 = New SLDocument(excelPath, alt4SheetName)
        Calculate(Alt4,52,7)
        ALt4.Save()

        powerCalculation.Save()


        'Try an make it calculate

        'powerCalculation.SelectWorksheet(combinedAlternatorETAMAPSheetName)
        'powerCalculation.SelectWorksheet(alt1SheetName)
        'powerCalculation.SelectWorksheet(alt2SheetName)
        'powerCalculation.SelectWorksheet(alt3SheetName)
        'powerCalculation.SelectWorksheet(alt4SheetName) 

        ' powerCalculation.SelectWorksheet(alternator1Name)
        ' powerCalculation.SelectWorksheet(alternator2Name)
        ' powerCalculation.SelectWorksheet(alternator3Name)
        ' powerCalculation.SelectWorksheet(alternator4Name)      
                     
     


End Sub



'Runs Code on the sheets to allow resulting map to be recalculated.
Private Sub Calculate( alt As SLDocument, startRow As Integer, startColumn As integer)

'This macro builds the base for the calculation of the "Output Matrix" in sheet "Power Calculation". Therefore it collects the values
'out of the user inteface in sheet "Power Calculation" (Input Alt 1) and writes it into the table P4:U19 in sheet "Alt 1". This program
'checks out if the values are increasing or decreasing and splits it into these two cases.

'If there are changes in row or column number in sheet "Power Calculation", following steps will have to be made:
'Change in Row number:
'1. Change the start value of a (line 25, 70, 113) into the row number of the first amps in the "Input Altenator" table.
'2. Change the start value of the Loop Counter i (line 29, 73, 116) into the same value as a
'3. Change the end value of the Loop Counter i (line 29, 73, 116) into its start value plus 6
'4. Change value of a in the If-query (line 46, 89, 132 and line 55, 97, 141) into the end value of (3.) plus 1
'5. Change the end value of the Loop Counter j (line 47, 90, 133) into the end value of (3.) plus 1
'Change in column:
'1. Change the number (not the letter) in all commands similar to 'Sheets("Power Calculation").'.
'   ATTENTION: different numbers for different rpm



    Dim i, j, k, l, m, n, a         'Definition of the used variables
    
'Calculation for 2000 rpm
    j = 5       'Start-values
    k = 0
    a = startRow'39
    n = 1
        
    'Increasing values
    For i = startRow To  (startRow + 6 )'45     'Loop over the rows 5 to 11 in sheet "Power Calculation"
        If powerCalculation.GetCells(New SLCellPoint(i, startColumn)).NumericValue <> 0 Then 'Query whether column B, row i in sheet "Power Calculation" contains a value
            If powerCalculation.GetCells(New SLCellPoint(i, startColumn)).NumericValue > k Then           'Query if the value of row i is greater than the value of row i-1
                alt.SetCellValue(j, 17,powerCalculation.GetCells(New SLCellPoint(i, startColumn)).NumericValue)        'Write the value of row i into the table P4:U19 row j in sheet "Alt 1"
                j = j + 1           'Counter for row j in table P4:U19 (sheet "Alt 1")
                k = powerCalculation.GetCells(New SLCellPoint(i, startColumn) ).NumericValue         'Compare value for the next iteration cycle
            Else: a = j - 1 + a - 5    'Saving row-number with the greatest value of degree of efficiency
            End If
        Else
            For l = j To 11         'Loop to fill empty cells in Q6 to Q8 with the existing values
                alt.SetCellValue(l, 17,alt.GetCells(New SLCellPoint(l - j + 5, 17)).NumericValue)
            Next
            alt.SetCellValue(13, 17, k  )     'Write last value in the first row of the decreasing valus
        End If
    Next
    


    'Decreasing values
    If a < (startRow+7) Then          'Query: if a>=11: no decreasing values exist
        For j = a To (startRow+7)
            If powerCalculation.GetCells(New SLCellPoint(j + 1, startColumn)).NumericValue < k And powerCalculation.GetCells(New SLCellPoint(j + 1, startColumn)).NumericValue <> 0 Then
                alt.SetCellValue(13 + n, 17, powerCalculation.GetCells(New SLCellPoint(j + 1, startColumn)).NumericValue) 'Write decreasing values into row 13+n
                k = powerCalculation.GetCells(New SLCellPoint(j + 1, startColumn)).NumericValue      'Compare value for the next iteration cycle
                n = n + 1           'Counter for row number
                a = a + 1           'Counter
            End If
        Next
        If a < (startRow+7) Then
            For m = 13 + n To 19            'Loop to fill empty cells in Q13 to Q19 with the existing values
                alt.SetCellValue(m, 17,alt.GetCells(New SLCellPoint(m - n, 17)).NumericValue)
            Next
        End If
    Else
        For m = 13 To 19
             alt.SetCellValue(m, 17, 0 )    'If decreasing values do not exist, fill Q13 to Q19 (sheet "Alt 1") with 0
        Next
    End If
    

'Calculation for 4000 rpm
    j = 5
    k = 0
    a = startRow '39
    n = 1
    
    For i = startRow To (startRow + 6 ) '45
        If powerCalculation.GetCells(New SLCellPoint(i, startColumn+1)).NumericValue <> 0 Then
            If powerCalculation.GetCells(New SLCellPoint(i, startColumn+1)).NumericValue > k Then
                alt.SetCellValue(j, 19, powerCalculation.GetCells(New SLCellPoint(i, startColumn+1)).NumericValue)
                j = j + 1
                k = powerCalculation.GetCells(New SLCellPoint(i, startColumn+1)).NumericValue
            Else: a = j - 1 + a - 5
            End If
        Else
            For l = j To 11
                alt.SetCellValue(l, 19,  alt.GetCells( New SLCellPoint(l - j + 5, 19)).NumericValue)
            Next
             alt.SetCellValue(13, 19, k)
        End If
    Next
        
    If a < (startRow + 7) Then
        For j = startRow To startRow+7
            If powerCalculation.GetCells(New SLCellPoint(j + 1, startColumn+1)).NumericValue < k And powerCalculation.GetCells(New SLCellPoint(j + 1, startColumn+1)).NumericValue <> 0 Then
                alt.SetCellValue(13 + n, 19, powerCalculation.GetCells(New SLCellPoint(j + 1, startColumn+1)).NumericValue)
                k = powerCalculation.GetCells(New SLCellPoint(j + 1, startColumn+1)).NumericValue
                n = n + 1
                a = a + 1
            End If
        Next
        If a < (startRow + 7 ) Then
            For m = 13 + n To 19
                alt.SetCellValue(m, 19,  alt.GetCells( New SLCellPoint(m - n, 19)).NumericValue)
            Next
        End If
    Else
        For m = 13 To 19
            alt.SetCellValue(m, 19, 0)
        Next
    End If
    
    
'Calculation for 6000 rpm
    j = 5
    k = 0
    a = startRow
    n = 1

    For i = startRow To ( startRow + 6 )
        If powerCalculation.GetCells(New SLCellPoint(i, startColumn+2)).NumericValue <> 0 Then
            If powerCalculation.GetCells(New SLCellPoint(i, startColumn+2)).NumericValue > k Then
                alt.SetCellValue(j, 21,  powerCalculation.GetCells(New SLCellPoint(i, startColumn+2)).NumericValue)
                j = j + 1
                k = powerCalculation.GetCells(New SLCellPoint(i, startColumn+2)).NumericValue
            Else: a = j - 1 + a - 5
            End If
        Else
            For l = j To 11
                alt.SetCellValue(l, 21,  alt.GetCells( New SLCellPoint(l - j + 5, 21)).NumericValue)
            Next
            alt.SetCellValue(13, 21, k )
        End If
    Next
    
    If a < (startRow + 7 ) Then
        For j = a To startRow+7
            If powerCalculation.GetCells(New SLCellPoint(j + 1, startColumn+2)).NumericValue < k And powerCalculation.GetCells(New SLCellPoint(j + 1, startColumn+2)).NumericValue <> 0 Then
                alt.SetCellValue(13 + n, 21, powerCalculation.GetCells(New SLCellPoint(j + 1, startColumn+2)).NumericValue)
                k = powerCalculation.GetCells(New SLCellPoint(j + 1, startColumn+2)).NumericValue
                n = n + 1
                a = a + 1
            End If
        Next
        If a < (startRow + 7 ) Then
            For m = 13 + n To 19
                alt.SetCellValue(m, 21, alt.GetCells( New SLCellPoint(m - n, 21)).NumericValue)
            Next
        End If
    Else
        For m = 13 To 19
            alt.SetCellValue(m, 21, 0)
        Next
    End If
    
    
End Sub

'Gives the Resulting Map to the caller.
Public function GetCombinedMap() As List(Of CombinedAltEntry)

  Dim  firstColumn As integer = 10
  Dim  firstRow    As Integer = 4
  Dim  lastRow     As integer = 38
  Dim  resultMap   As New List(Of CombinedAltEntry)
  Dim valueAmps As Single
  Dim valueEngineSpeed As Single
  Dim valueEfficiency As Single
  Dim currentColumn As integer

 
   
  combinedSheet = New SLDocument(excelPath,  combinedAlternatorETAMAPSheetName)

      


  For currentRow As Integer = firstRow to lastRow

      currentColumn = firstColumn

      valueAmps        = Math.Round(combinedSheet.GetCells( New SLCellPoint(currentRow,currentColumn+0)).NumericValue,2)

      valueEngineSpeed = Math.Round(combinedSheet.GetCells( New SLCellPoint(currentRow,currentColumn+1)).NumericValue,0)

      valueEfficiency  = Math.Round(combinedSheet.GetCells( New SLCellPoint(currentRow,currentColumn+2)).NumericValue,2)


      resultMap.Add( New CombinedAltEntry With {.Amps=valueAmps, .EngineSpeed=valueEngineSpeed, .Efficiency=valueEfficiency })


  next

  combinedSheet.CloseWithoutSaving()

  Return resultMap


End function






End Class


End Namespace



