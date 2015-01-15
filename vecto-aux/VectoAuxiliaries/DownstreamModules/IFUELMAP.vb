' Copyright 2015 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.

Public Interface IFUELMAP


   Function ReadFile(Optional ByVal ShowMsg As Boolean = True) As Boolean


   Function fFCdelaunay_Intp(ByVal nU As Single, ByVal Tq As Single) As Single

   Function Triangulate() As Boolean

   Property FilePath As String
   ReadOnly Property MapDim As Integer 
   ReadOnly Property Tq As List(Of Single)
   ReadOnly Property FC As List(Of Single)
   ReadOnly Property nU As List(Of Single)



End Interface
