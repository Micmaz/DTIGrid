Imports System
Imports System.Collections
Imports System.Text


Public Class DTIGridRowCollection
    Inherits CollectionBase

    Private _myParent As DTIGrid

    Sub New(ByRef grid As DTIGrid)
        _myParent = grid
    End Sub

    Public ReadOnly Property DataGrid() As DTIGrid
        Get
            Return _myParent
        End Get
    End Property

    Default Public Property Item(ByVal index As Integer) As DTIGridRow
        Get
            Return CType(List(index), DTIGridRow)
        End Get
        Set(ByVal value As DTIGridRow)
            List(index) = value
        End Set
    End Property

    Public Function FindRowById(ByVal Id As Integer) As DTIGridRow
        For Each row As DTIGridRow In List
            If row.Item("dtigridid") IsNot Nothing AndAlso row.Item("dtigridid") = Id Then
                Return row
            End If
        Next
        Return Nothing
    End Function

    'Public Function FindRowByPrimaryKey(ByVal val As String) As DTIGridRow
    '    If DataGrid.PrimaryKeyColumnName Is Nothing Then
    '        Throw New Exception("Need a Primary Key to bind with")
    '    End If
    '    For Each row As DTIGridRow In List
    '        If row.Item(DataGrid.PrimaryKeyColumnName) IsNot Nothing AndAlso row.Item(DataGrid.PrimaryKeyColumnName) = val Then
    '            Return row
    '        End If
    '    Next
    '    Return Nothing
    'End Function

    Public Function IndexOf(ByVal value As DTIGridRow) As Integer
        Return List.IndexOf(value)
    End Function 'IndexOf

    Public Function Add(ByVal value As DTIGridRow) As Integer
        Return List.Add(value)
    End Function 'Add

    Public Sub Remove(ByVal value As DTIGridRow)
        List.Remove(value)
    End Sub 'Remove

    Public Function Contains(ByVal value As DTIGridRow) As Boolean
        Return List.Contains(value)
    End Function 'Contains

    Protected Overrides Sub OnValidate(ByVal value As Object)
        If Not GetType(DTIGridRow).IsAssignableFrom(value.GetType()) Then
            Throw New ArgumentException("value must be of type DTIGridRow.", "value")
        End If
    End Sub 'OnValidate 

    Public Overrides Function ToString() As String
        Dim strbldr As New StringBuilder
        strbldr.AppendLine("[")
        For Each row As DTIGridRow In List
            strbldr.AppendLine(row.ToString & ",")
        Next
        If strbldr.Length > 3 Then _
            strbldr.Remove(strbldr.Length - 3, 1)
        strbldr.AppendLine("];")
        Return strbldr.ToString
    End Function

    Public Function Clone(ByRef grid As DTIGrid) As DTIGridRowCollection
        Dim NewRows As New DTIGridRowCollection(grid)
        For Each row As DTIGridRow In List
            Dim tmprow As New DTIGridRow(grid)
            tmprow.Selected = row.Selected
            tmprow.SetRowState(row.RowState)
            tmprow.isActive = row.isActive
            Dim i As Integer = 0
            For i = 0 To row.Items.Length - 1
                tmprow.Add(row.Keys(i), row.Items(i))
            Next
            NewRows.Add(tmprow)
        Next
        Return NewRows
    End Function
End Class

Public Class DTIGridRowNotFoundException
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
