Imports System
Imports System.Collections
Imports System.Text

Public Class DTIGridColumnCollection
    Inherits CollectionBase

    Default Public Property Item(ByVal index As Integer) As DTIGridColumn
        Get
            Return CType(List(index), DTIGridColumn)
        End Get
        Set(ByVal value As DTIGridColumn)
            List(index) = value
        End Set
    End Property

    Default Public Property Item(ByVal Name As String) As DTIGridColumn
        Get
            Return FindColumnByString(Name)
        End Get
        Set(ByVal value As DTIGridColumn)
            Dim col As DTIGridColumn = FindColumnByString(Name)
            col = value
        End Set
    End Property

    Public Function IndexOf(ByVal value As DTIGridColumn) As Integer
        Return List.IndexOf(value)
    End Function 'IndexOf

    Public Function IndexOf(ByVal name As String) As Integer
        Try
            Return FindIndexByString(name)
        Catch ex As DTIGridColumnNotFoundException
            Return -1
        End Try
    End Function 'IndexOf

    Public Function Add(ByVal value As DTIGridColumn) As Integer
        Return List.Add(value)
    End Function 'Add

    Public Function AddAt(ByVal index As Integer, ByVal value As DTIGridColumn) As Integer
        Dim tmpitem As DTIGridColumn = value
        For i As Integer = index To List.Count
            Dim saveitem As DTIGridColumn = List(i)
            List(i) = tmpitem
            tmpitem = saveitem
        Next
        List.Add(tmpitem)
    End Function 'AddAt


    Public Function Add(ByVal name As String, ByVal header As String, ByVal Datatype As Type, Optional ByVal visible As Boolean = True, Optional ByVal width As UShort = 0, _
                Optional ByVal sortable As Boolean = True, Optional ByVal editable As Boolean = True) As Integer
        Return List.Add(New DTIGridColumn(name, header, Datatype, visible, width, sortable, editable))
    End Function 'Add

    Public Sub Remove(ByVal value As DTIGridColumn)
        List.Remove(value)
    End Sub 'Remove

    Public Sub Remove(ByVal value As String)
        Dim col As DTIGridColumn = FindColumnByString(value)
        If Not col Is Nothing Then List.Remove(col)
    End Sub 'Remove

    Public Function Contains(ByVal value As DTIGridColumn) As Boolean
        Return List.Contains(value)
    End Function 'Contains

    Public Function Contains(ByVal value As String) As Boolean
        Try
            Return FindColumnByString(value) IsNot Nothing
        Catch ex As Exception
            Return Nothing
        End Try
    End Function 'Contains

    Protected Overrides Sub OnValidate(ByVal value As Object)
        If Not GetType(DTIGridColumn).IsAssignableFrom(value.GetType()) Then
            Throw New ArgumentException("value must be of type String.", "value")
        End If
    End Sub 'OnValidate 

    Private Function FindColumnByString(ByVal str As String) As DTIGridColumn
        For Each grid As DTIGridColumn In List
            If grid.Name.ToLower = str.ToLower Then
                Return grid
            End If
        Next
        Return Nothing
    End Function

    Private Function FindIndexByString(ByVal str As String) As Integer
        For Each grid As DTIGridColumn In List
            If grid.Name.ToLower = str.ToLower Then
                Return IndexOf(grid)
            End If
        Next
        Return Nothing
    End Function

    Public Overrides Function ToString() As String
        Dim strbldr As New StringBuilder
        ' strbldr.AppendLine("[{hidden:true,name:'dtigridstate', index:'dtigridstate', width:60, sortable:false, editable:false},")
        strbldr.AppendLine("[")
        For Each column As DTIGridColumn In List
            'If column.Name = DataGrid.PrimaryKeyColumnName OrElse Not column.Hidden Then _
            strbldr.AppendLine(column.ToString & ",")
        Next
        If strbldr.Length > 3 Then _
            strbldr.Remove(strbldr.Length - 3, 1)
        strbldr.Append("],")
        Return strbldr.ToString
    End Function

    Public Function ColumnHeadersString() As String
        Dim strbldr As New StringBuilder
        ' strbldr.Append("['dtigridstate',")
        strbldr.Append("[")
        For Each column As DTIGridColumn In List
            'If column.Name = DataGrid.PrimaryKeyColumnName OrElse Not column.Hidden Then _
            strbldr.Append("'" & BaseClasses.BaseSecurityPage.JavaScriptEncode(column.ColumnHeader) & "',")
        Next
        If strbldr.Length > 1 Then _
            strbldr.Remove(strbldr.Length - 1, 1)
        strbldr.Append("],")
        Return strbldr.ToString
    End Function

    Public Function Clone() As DTIGridColumnCollection
        Dim NewColls As New DTIGridColumnCollection()
        For Each col As DTIGridColumn In List
            NewColls.Add(col.Name, col.ColumnHeader, col.DataType, col.Visible, col.Width, col.Sortable, col.Editable)
        Next
        Return NewColls
    End Function
End Class

Public Class DTIGridColumnNotFoundException
    Inherits Exception
    Public Sub New()
    End Sub
    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub
    Public Sub New(ByVal message As String, ByVal inner As Exception)
        MyBase.New(message, inner)
    End Sub
End Class

