Imports System
Imports System.Collections
Imports System.Text


Public Class DTIGridRow
    Inherits NameObjectCollectionBase
    Implements ICloneable

    Private _myParent As DTIGrid

    Sub New(ByRef grid As DTIGrid)
        _myParent = grid
    End Sub

    Public ReadOnly Property DataGrid() As DTIGrid
        Get
            Return _myParent
        End Get
    End Property

    Private _rowState As DTIRowState = DTIRowState.Unchanged
    Private _isActive As Boolean = False
    Private _selected As Boolean = False

    Public Enum DTIRowState
        Unchanged
        Updated
        Added
        Deleted
    End Enum

    ' Creates an empty collection.
    Public Sub New()
    End Sub 'New

    Default Public Property Item(ByVal index As Integer) As Object
        Get
            Return Me.BaseGet(index)
        End Get
        Set(ByVal value As Object)
            Me.BaseSet(index, value)
        End Set
    End Property

    Default Public Property Item(ByVal columnName As String) As Object
        Get
            Return Me.BaseGet(columnName)
        End Get
        Set(ByVal value As Object)
            Me.BaseSet(columnName, value)
        End Set
    End Property

    Public Property RowState() As DTIRowState
        Get
            Return _rowState
        End Get
        Set(ByVal value As DTIRowState)
            _rowState = value
        End Set
    End Property

    Public Property isActive() As Boolean
        Get
            Return _isActive
        End Get
        Set(ByVal value As Boolean)
            _isActive = value
        End Set
    End Property

    Public Property Selected() As Boolean
        Get
            Return _selected
        End Get
        Set(ByVal value As Boolean)
            _selected = value
        End Set
    End Property

    Public ReadOnly Property ColumnNames() As String()
        Get
            Return Me.BaseGetAllKeys()
        End Get
    End Property

    Public ReadOnly Property Items() As Array
        Get
            Return Me.BaseGetAllValues()
        End Get
    End Property

    ' Adds an entry to the collection.
    Public Sub Add(ByVal key As String, ByVal value As Object)
        Me.BaseAdd(key, value)
    End Sub 'Add

    ' Removes an entry with the specified key from the collection.
    Public Overloads Sub Remove(ByVal key As String)
        Me.BaseRemove(key)
    End Sub 'Remove

    ' Removes an entry in the specified index from the collection.
    Public Overloads Sub Remove(ByVal index As Integer)
        Me.BaseRemoveAt(index)
    End Sub 'Remove

    Friend Sub SetRowState(ByVal newState As DTIRowState)
        _rowState = newState
    End Sub

    Public Overrides Function ToString() As String
        Dim strbldr As New StringBuilder
        'strbldr.Append("{dtigridstate:""unchanged"",")
        strbldr.Append("{")
        If Me.Items.Length > 0 Then
            For i As Integer = 0 To Me.Items.Length - 1
                If Me.DataGrid.Columns(Me.Keys(i).ToString) Is Nothing Then
                    Me.DataGrid.Columns.Add(Me.Keys(i).ToString, Me.Keys(i).ToString, GetType(String))
                End If
                'If Me.DataGrid.PrimaryKeyColumnName = Me.Keys(i).ToString OrElse Not Me.DataGrid.Columns(Me.Keys(i).ToString).Hidden Then
                If Me.DataGrid.Columns(Me.Keys(i).ToString).DataType Is GetType(DateTime) Then
                    If Not Me.DataGrid.ShowDateAndTime Then
                        strbldr.Append(BaseClasses.BaseSecurityPage.RemoveSpecialCharacters(Me.Keys(i), "_") & ":""")
                        If Not Me.Items(i) Is DBNull.Value Then
                            strbldr.Append(CType(Me.Items(i), DateTime).ToString("MM/dd/yyyy") & """,")
                        Else
                            strbldr.Append(""",")
                        End If
                    Else
                        strbldr.Append(BaseClasses.BaseSecurityPage.RemoveSpecialCharacters(Me.Keys(i), "_") & ":""" & CType(IIf(Me.Items(i) Is DBNull.Value, Date.MinValue, Me.Items(i)), DateTime).ToString("MM/dd/yyyy hh:mm tt") & """,")
                    End If
                Else
                    Select Case Me.DataGrid.Columns(Me.Keys(i).ToString).EditType
                        Case DTIGridColumn.EditTypes.select
                            strbldr.Append(BaseClasses.BaseSecurityPage.RemoveSpecialCharacters(Me.Keys(i), "_") & ":""" & Me.DataGrid.Columns(Me.Keys(i).ToString).SelectValueToString(Me.Items(i).ToString) & """,")
                        Case DTIGridColumn.EditTypes.link
                            strbldr.Append(BaseClasses.BaseSecurityPage.RemoveSpecialCharacters(Me.Keys(i), "_") & ":""" & BaseClasses.BaseSecurityPage.JavaScriptEncode(Me.Items(i).ToString) & """,")
                        Case Else
                            strbldr.Append(BaseClasses.BaseSecurityPage.RemoveSpecialCharacters(Me.Keys(i), "_") & ":""" & BaseClasses.BaseSecurityPage.JavaScriptEncode(Me.Items(i).ToString) & """,")
                    End Select
                End If
				'End If

			Next
        End If
        If strbldr.Length > 1 Then _
            strbldr.Remove(strbldr.Length - 1, 1)
        strbldr.Append("}")
        Return strbldr.ToString
    End Function

    ''' <summary>
    ''' Add a link as a non-editable column to the grid. ColumnName need not exist before the call. 
    ''' This should happen after databind.
    ''' </summary>
    ''' <param name="ColumnName"></param>
    ''' <param name="linkText"></param>
    ''' <param name="linkURL"></param>
    ''' <param name="asDialog"></param>
    ''' <param name="dialogTitle"></param>
    ''' <param name="dialogHeight"></param>
    ''' <param name="dialogWidth"></param>
    ''' <param name="dialogModal"></param>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Add a link as a non-editable column to the grid. ColumnName need not exist before the call.    This should happen after databind.")> _
    Public Sub setLink(ByVal ColumnName As String, ByVal linkText As String, ByVal linkURL As String, Optional ByVal asDialog As Boolean = True, Optional ByVal dialogTitle As String = "", Optional ByVal dialogHeight As Integer = 400, Optional ByVal dialogWidth As Integer = 400, Optional ByVal dialogModal As Boolean = True)
        If Not Me.DataGrid.Columns.Contains(ColumnName) Then
            Me.DataGrid.Columns.Add(ColumnName, ColumnName, GetType(String), True, , False, False)
        End If
        Me.DataGrid.Columns(ColumnName).EditType = DTIGridColumn.EditTypes.link
        If asDialog Then
            JqueryUIControls.Dialog.registerControl(Me.DataGrid.Page)
            Me.Item(ColumnName) = String.Format("<a href=""javascript:void(0);"" onclick=""createDlg('{0}', '{2}', {3}, {4}, {5});"">{1}</a>", linkURL, linkText, dialogTitle, dialogHeight, dialogWidth, dialogModal.ToString.ToLower)
        Else
            Me.Item(ColumnName) = String.Format("<a href='{0}'>{1}</a>", linkURL, linkText)
        End If
    End Sub



    Public Function dataRow() As Data.DataRow
        Dim found As Boolean = False
        Dim index As Integer = Me("dtigridid")
        If index = 0 OrElse index > Me.DataGrid.dt.Rows.Count Then
            Return Nothing
        End If
        Return Me.DataGrid.dt.Rows(index - 1)
        'For Each row As DataRow In Me.DataGrid.dt.Rows
        '    For Each col As String In Me.Keys
        '        Try
        '            If row.Table.Columns.Contains(col) Then
        '                Dim column As DataColumn = row.Table.Columns(col)
        '                If Not (row.RowState = DataRowState.Added AndAlso (column.AutoIncrement Or Not column.DefaultValue Is DBNull.Value)) Then
        '                    If Not Me.Item(col) = row(col) Then
        '                        found = False
        '                        Exit For
        '                    End If
        '                End If
        '            End If

        '        Catch ex As Exception

        '        End Try
        '        found = True
        '    Next
        '    If found = True Then
        '        Return row
        '    End If
        'Next
        Return Nothing
    End Function

    Public Function Clone() As Object Implements System.ICloneable.Clone
        Return Me.MemberwiseClone()
    End Function
End Class
