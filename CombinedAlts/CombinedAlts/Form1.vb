
imports System
imports System.Collections.Generic
imports System.Linq
imports System.Text
imports System.IO
imports DocumentFormat.OpenXml
imports DocumentFormat.OpenXml.Spreadsheet
imports SpreadsheetLight

Public Class Form1

Private powerCalculationSheetName = "Power Calculation"
Private combinedAlternatorETAMAPSheetName = "Combined Alternator ETA Map"
Private alt1SheetName  = "Alt 1"
Private aLt2SheetName  = "Alt 2"
Private alt3SheetName  = "Alt 3"
Private aLt4SheetName  = "Alt 4"


private combinedSheet As  SLDocument
Private powerCalculation As SLDocument
Private Alt1 As SLDocument
Private ALt2 As SLDocument
Private Alt3 As SLDocument
Private ALt4 As SLDocument

Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click


ABC("c:\alt.xlsx")

End Sub


public sub calculate()



End Sub


Public  Sub ABC(excelPath As string)

Dim fs As   FileStream
Dim msFirstPass As  MemoryStream
Dim combinedSheet As  SLDocument

'run calculation



Try

  fs = New FileStream(excelPath, FileMode.Open) 

  msFirstPass = New MemoryStream()
  combinedSheet = New SLDocument(fs,  combinedAlternatorETAMAPSheetName)
  powerCalculation = New SLDocument(fs, powerCalculationSheetName)
  Alt1 = New SLDocument(fs, alt1SheetName)
  Alt2 = New SLDocument(fs, alt2SheetName)
  Alt3 = New SLDocument(fs, alt3SheetName)
  Alt4 = New SLDocument(fs, alt4SheetName)




  'Invoke Macro
  ValuesAlt1()
  fs.Close

  Alt1.SaveAs(excelPath)

  'Dim cell1  = combinedSheet.GetCells( New SLCellPoint(4,11))

  'debug.WriteLine(String.Format("Values are {0}",cell1.NumericValue))

Catch ex As Exception

  Console.WriteLine( ex.Message)

Finally

 

End Try



End Sub

Sub ValuesAlt1()

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
    a = 39
    n = 1
        
    'Increasing values
    For i = 39 To 45     'Loop over the rows 5 to 11 in sheet "Power Calculation"
        If powerCalculation.GetCells(New SLCellPoint(i, 2)).NumericValue <> 0 Then 'Query whether column B, row i in sheet "Power Calculation" contains a value
            If powerCalculation.GetCells(New SLCellPoint(i, 2)).NumericValue > k Then           'Query if the value of row i is greater than the value of row i-1
                alt1.SetCellValue(j, 17,powerCalculation.GetCells(New SLCellPoint(i, 2)).NumericValue)        'Write the value of row i into the table P4:U19 row j in sheet "Alt 1"
                j = j + 1           'Counter for row j in table P4:U19 (sheet "Alt 1")
                k = powerCalculation.GetCells(New SLCellPoint(i, 2) ).NumericValue         'Compare value for the next iteration cycle
            Else: a = j - 1 + a - 5    'Saving row-number with the greatest value of degree of efficiency
            End If
        Else
            For l = j To 11         'Loop to fill empty cells in Q6 to Q8 with the existing values
                alt1.SetCellValue(l, 17,Alt1.GetCells(New SLCellPoint(l - j + 5, 17)).NumericValue)
            Next
            alt1.SetCellValue(13, 17, k  )     'Write last value in the first row of the decreasing valus
        End If
    Next
    
    'Decreasing values
    If a < 46 Then          'Query: if a>=11: no decreasing values exist
        For j = a To 46
            If powerCalculation.GetCells(New SLCellPoint(j + 1, 2)).NumericValue < k And powerCalculation.GetCells(New SLCellPoint(j + 1, 2)).NumericValue <> 0 Then
                alt1.SetCellValue(13 + n, 17, powerCalculation.GetCells(New SLCellPoint(j + 1, 2)).NumericValue) 'Write decreasing values into row 13+n
                k = powerCalculation.GetCells(New SLCellPoint(j + 1, 2)).NumericValue      'Compare value for the next iteration cycle
                n = n + 1           'Counter for row number
                a = a + 1           'Counter
            End If
        Next
        If a < 46 Then
            For m = 13 + n To 19            'Loop to fill empty cells in Q13 to Q19 with the existing values
                alt1.SetCellValue(m, 17,Alt1.GetCells(New SLCellPoint(m - n, 17)).NumericValue)
            Next
        End If
    Else
        For m = 13 To 19
             alt1.SetCellValue(m, 17, 0 )    'If decreasing values do not exist, fill Q13 to Q19 (sheet "Alt 1") with 0
        Next
    End If
    

'Calculation for 4000 rpm
    j = 5
    k = 0
    a = 39
    n = 1
    
    For i = 39 To 45
        If powerCalculation.GetCells(New SLCellPoint(i, 3)).NumericValue <> 0 Then
            If powerCalculation.GetCells(New SLCellPoint(i, 3)).NumericValue > k Then
                alt1.SetCellValue(j, 19, powerCalculation.GetCells(New SLCellPoint(i, 3)).NumericValue)
                j = j + 1
                k = powerCalculation.GetCells(New SLCellPoint(i, 3)).NumericValue
            Else: a = j - 1 + a - 5
            End If
        Else
            For l = j To 11
                alt1.SetCellValue(l, 19,  alt1.GetCells( New SLCellPoint(l - j + 5, 19)).NumericValue)
            Next
             alt1.SetCellValue(13, 19, k)
        End If
    Next
        
    If a < 46 Then
        For j = a To 46
            If powerCalculation.GetCells(New SLCellPoint(j + 1, 3)).NumericValue < k And powerCalculation.GetCells(New SLCellPoint(j + 1, 3)).NumericValue <> 0 Then
                alt1.SetCellValue(13 + n, 19, powerCalculation.GetCells(New SLCellPoint(j + 1, 3)).NumericValue)
                k = powerCalculation.GetCells(New SLCellPoint(j + 1, 3)).NumericValue
                n = n + 1
                a = a + 1
            End If
        Next
        If a < 46 Then
            For m = 13 + n To 19
                alt1.SetCellValue(m, 19,  alt1.GetCells( New SLCellPoint(m - n, 19)).NumericValue)
            Next
        End If
    Else
        For m = 13 To 19
            alt1.SetCellValue(m, 19, 0)
        Next
    End If
    
    
'Calculation for 6000 rpm
    j = 5
    k = 0
    a = 39
    n = 1

    For i = 39 To 45
        If powerCalculation.GetCells(New SLCellPoint(i, 4)).NumericValue <> 0 Then
            If powerCalculation.GetCells(New SLCellPoint(i, 4)).NumericValue > k Then
                alt1.SetCellValue(j, 21,  powerCalculation.GetCells(New SLCellPoint(i, 4)).NumericValue)
                j = j + 1
                k = powerCalculation.GetCells(New SLCellPoint(i, 4)).NumericValue
            Else: a = j - 1 + a - 5
            End If
        Else
            For l = j To 11
                alt1.SetCellValue(l, 21,  alt1.GetCells( New SLCellPoint(l - j + 5, 21)).NumericValue)
            Next
            alt1.SetCellValue(13, 21, k )
        End If
    Next
    
    If a < 46 Then
        For j = a To 46
            If powerCalculation.GetCells(New SLCellPoint(j + 1, 4)).NumericValue < k And powerCalculation.GetCells(New SLCellPoint(j + 1, 4)).NumericValue <> 0 Then
                alt1.SetCellValue(13 + n, 21, powerCalculation.GetCells(New SLCellPoint(j + 1, 4)).NumericValue)
                k = powerCalculation.GetCells(New SLCellPoint(j + 1, 4)).NumericValue
                n = n + 1
                a = a + 1
            End If
        Next
        If a < 46 Then
            For m = 13 + n To 19
                alt1.SetCellValue(m, 21, alt1.GetCells( New SLCellPoint(m - n, 21)).NumericValue)
            Next
        End If
    Else
        For m = 13 To 19
            alt1.SetCellValue(m, 21, 0)
        Next
    End If
    
    
End Sub





End Class
