Public Class DTIGridColumn

#Region "Private Members"
    Private _sortable As Boolean = True
    Private _fixed As Boolean = False
    Private _editable As Boolean = True
    Private _hidden As Boolean = False
    Private _keyedColumn As Boolean = False
    Private _width As UInt16 = 0
    Private _index As String
    Private _name As String
    Private _columnHeader As String
    Private _sortType As SortTypes = SortTypes.text
    Private _dataType As Type
    Private _align As Alignment = Alignment.left
    'Private _myParent As DTIGrid
    Private _editType As EditTypes = EditTypes.text
    Private _selectDT As DataTable
    Private _SelectTableValueColumn As String
    Private _SelectTableTextColumn As String
#End Region

#Region "Enums"

    Public Enum SortTypes
        int
        [date]
        float
        text
    End Enum

    Public Enum EditTypes
        text
        textarea
        [select]
        checkbox
        password
        [integer]
        [double]
        link
    End Enum

    Public Enum Alignment
        left
        center
        right
    End Enum

#End Region

#Region "Properties"
    Public ReadOnly Property DataType() As System.Type
        Get
            Return _dataType
        End Get
    End Property

    Private ReadOnly Property KeyedColumn() As Boolean
        Get
            Return _keyedColumn
        End Get
    End Property

    'Public ReadOnly Property DataGrid() As DTIGrid
    '    Get
    '        Return _myParent
    '    End Get
    'End Property

    Public Property Sortable() As Boolean
        Get
            Return _sortable
        End Get
        Set(ByVal value As Boolean)
            _sortable = value
        End Set
    End Property

    Public Property Fixed() As Boolean
        Get
            Return _fixed
        End Get
        Set(ByVal value As Boolean)
            _fixed = value
        End Set
    End Property

    Public Property Editable() As Boolean
        Get
            Return _editable
        End Get
        Set(ByVal value As Boolean)
            _editable = value
        End Set
    End Property

    Public Property Hidden() As Boolean
        Get
            Return _hidden
        End Get
        Set(ByVal value As Boolean)
            _hidden = value
        End Set
    End Property

    Public Property AutoWidth() As Boolean = True

    Public Property Visible() As Boolean
        Get
            Return Not _hidden
        End Get
        Set(ByVal value As Boolean)
            _hidden = Not value
        End Set
    End Property

    Public Property Width() As UInt16
        Get
            Return _width
        End Get
        Set(ByVal value As UInt16)
            _width = value
        End Set
    End Property

    Public Property Index() As String
        Get
            Return _index
        End Get
        Set(ByVal value As String)
            _index = value
        End Set
    End Property

    Public Property EditType() As EditTypes
        Get
            Return _editType
        End Get
        Set(ByVal value As EditTypes)
            _editType = value
        End Set
    End Property

    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Public Property SortType() As SortTypes
        Get
            Return _sortType
        End Get
        Set(ByVal value As SortTypes)
            _sortType = value
        End Set
    End Property

    Public Property Align() As Alignment
        Get
            Return _align
        End Get
        Set(ByVal value As Alignment)
            _align = value
        End Set
    End Property

    Public Property ColumnHeader() As String
        Get
            Return HttpUtility.HtmlEncode(_columnHeader.Replace(vbCr, "").Replace(vbLf, ""))
        End Get
        Set(ByVal value As String)
            _columnHeader = value
        End Set
    End Property

    Public Property SelectTable() As DataTable
        Get
            Return _selectDT
        End Get
        Set(ByVal value As DataTable)
            Me.EditType = EditTypes.select
            _selectDT = value
        End Set
    End Property

    Public Property SelectTableValueColumn() As String
        Get
            Return _SelectTableValueColumn
        End Get
        Set(ByVal value As String)
            _SelectTableValueColumn = value
        End Set
    End Property

    Public Property SelectTableTextColumn() As String
        Get
            Return _SelectTableTextColumn
        End Get
        Set(ByVal value As String)
            _SelectTableTextColumn = value
        End Set
    End Property

#End Region

#Region "Select List Methods"

    Private Sub SelectColumnFix()
        If SelectTable Is Nothing Then Return
        If SelectTableValueColumn Is Nothing OrElse SelectTable.Columns(SelectTableValueColumn) Is Nothing Then
            SelectTableValueColumn = SelectTable.Columns(0).ColumnName
        End If
        If SelectTableTextColumn Is Nothing OrElse SelectTable.Columns(SelectTableTextColumn) Is Nothing Then
            SelectTableTextColumn = SelectTableValueColumn
        End If
    End Sub

    Public Function SelectStringToValue(ByVal str As String) As String
        SelectColumnFix()
        For Each row As DataRow In SelectTable.Rows
            If row.Item(SelectTableTextColumn).ToString = str Then Return row.Item(SelectTableValueColumn).ToString
        Next
        Return Nothing
    End Function

    Public Function SelectValueToString(ByVal val As String) As String
        SelectColumnFix()
        For Each row As DataRow In SelectTable.Rows
            If row.Item(SelectTableValueColumn).ToString = val Then Return row.Item(SelectTableTextColumn).ToString
        Next
        Return Nothing
    End Function

    Private Function SelectTableToOptions()
        If SelectTable Is Nothing Then _
            Throw New Exception("Select Table not defined. Please change edittype or define the table.")
        'If SelectTable.Columns.Count < 2 Then _
        '    Throw New Exception("Select Table does not contain enough columns to bind to.  It needs a value column and a text column.")
        'If SelectTable.Columns(0).DataType IsNot GetType(Integer) And SelectTable.Columns(0).DataType IsNot GetType(String) Then _
        '    Throw New Exception("Select Table first column must be the column for the value of the select")
        'If SelectTable.Columns(1).DataType IsNot GetType(Integer) And SelectTable.Columns(1).DataType IsNot GetType(String) Then _
        '    Throw New Exception("Select Table second column must be the column for the string of the select")
        If SelectTable.Rows.Count < 1 Then _
            Throw New Exception("SelectTable has no rows.")
        Dim strbldr As New StringBuilder
        strbldr.Append("{value:""")
        SelectColumnFix()

        For Each row As DataRow In SelectTable.Rows
            strbldr.Append(row.Item(SelectTableValueColumn).ToString.Replace("""", "\""") & ":" & row.Item(SelectTableTextColumn).ToString.Replace("""", "\""") & ";")
        Next
        If strbldr.Length > 1 Then _
            strbldr.Remove(strbldr.Length - 1, 1)
        strbldr.Append("""}")
        Return strbldr.ToString
    End Function

    ''' <summary>
    ''' Sets the column to a select list type and adds an item to that list. 
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="DisplayString"></param>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Sets the column to a select list type and adds an item to that list.")>
    Public Sub addItemToSelectList(ByVal value As Object, Optional ByVal DisplayString As String = Nothing)
        If value Is Nothing Then value = DBNull.Value
        Me.EditType = EditTypes.select
        If Me.SelectTableTextColumn Is Nothing Then
            SelectTableTextColumn = "display"
        End If
        If Me.SelectTableValueColumn Is Nothing Then
            SelectTableValueColumn = "value"
        End If
        If Me.SelectTable Is Nothing Then
            SelectTable = New DataTable
        End If
        If Not SelectTable.Columns.Contains(SelectTableTextColumn) Then
            SelectTable.Columns.Add(SelectTableTextColumn)
        End If
        If Not SelectTable.Columns.Contains(SelectTableValueColumn) Then
            SelectTable.Columns.Add(SelectTableValueColumn)
        End If
        If DisplayString Is Nothing Then DisplayString = value.ToString()
        Dim row As DataRow = SelectTable.NewRow
        row(SelectTableValueColumn) = value
        row(SelectTableTextColumn) = DisplayString
        SelectTable.Rows.Add(row)
        'Me.EditType = EditTypes.select
        'If SelectTable Is Nothing Then
        '    SelectTable = New DataTable
        '    SelectTable.Columns.Add("value")
        '    SelectTable.Columns.Add("text")
        'End If
        'Dim row As DataRow = SelectTable.NewRow()
        'row(0) = itemValue
        'If itemText = Nothing Then itemText = itemValue.ToString
        'row(1) = itemText
        'SelectTable.Rows.Add(row)
    End Sub

    ''' <summary>
    ''' Sets the column to a select list type and sets a datatable as the select item list.
    ''' </summary>
    ''' <param name="table"></param>
    ''' <param name="valueColumn"></param>
    ''' <param name="textColumn"></param>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Sets the column to a select list type and sets a datatable as the select item list.")>
    Public Sub setSelectTable(ByVal table As DataTable, Optional ByVal valueColumn As String = Nothing, Optional ByVal textColumn As String = Nothing, Optional NullHeader As String = Nothing)
        Me.EditType = EditTypes.select
        SelectTable = table

        If valueColumn Is Nothing Then
            valueColumn = table.Columns(0).ColumnName
        End If
        Me.SelectTableValueColumn = valueColumn
        If Not textColumn Is Nothing Then
            SelectTableTextColumn = textColumn
        End If
        If NullHeader IsNot Nothing Then
            Dim newRow As DataRow = SelectTable.NewRow()
            newRow(valueColumn) = DBNull.Value
            If textColumn IsNot Nothing Then newRow(textColumn) = NullHeader
            SelectTable.Rows.InsertAt(newRow, 0)
        End If

    End Sub

#End Region

    Public Overrides Function ToString() As String
        'If Me.Name = DataGrid.PrimaryKeyColumnName OrElse Not Hidden Then
        Dim str As String = _
        "{" & _
        "hidden:" & Hidden.ToString.ToLower() & ", " & _
        "name:'" & BaseClasses.BaseSecurityPage.RemoveSpecialCharacters(Name, "_") & "', " & _
        "index:'" & Index & "', "
        If Width > 0 Then
            str &= "width:" & Width & ", "
        Else
            If (AutoWidth) Then str &= "autowidth: true,"
        End If
        If Fixed Then
            str &= "fixed: true,resizable:false, "
        End If
        str &= "sortable:" & Sortable.ToString.ToLower & ", " & _
        "align:""" & System.Enum.GetName(GetType(Alignment), Align) & """, "
        Select Case EditType
            Case EditTypes.checkbox
                str &= "edittype:'checkbox', editoptions:{value:""True:False""}, "
            Case EditTypes.select
                If SelectTable IsNot Nothing Then
                    str &= "edittype:'select', "
                    str &= "editoptions:" & SelectTableToOptions() & ", "
                Else
                    str &= "edittype:'text', "
                End If
            Case EditTypes.integer
                str &= "edittype:'custom', "
                str &= "editoptions:{custom_element: JqgridIntElem, custom_value:jqGridIntValue} , "
            Case EditTypes.double
                str &= "edittype:'custom', "
                str &= "editoptions:{custom_element: JqgridDubElem, custom_value:jqGridIntValue} , "
            Case EditTypes.text, EditTypes.password, EditTypes.textarea
                str &= "edittype:'" & System.Enum.GetName(GetType(EditTypes), EditType) & "', "
        End Select
        str &= _
        "editable:" & Editable.ToString.ToLower & ", " & _
        "sorttype:""" & System.Enum.GetName(GetType(SortTypes), SortType) & """" & _
        "}"
        Return str
        'Else : Return ""
        'End If
    End Function

    Public Sub New(ByVal name As String, ByVal header As String, ByVal Datatype As Type, Optional ByVal visible As Boolean = True, Optional ByVal width As UShort = 0, _
                Optional ByVal sortable As Boolean = True, Optional ByVal editable As Boolean = True, Optional ByVal align As Alignment = Alignment.left)
        'Me._myParent = parent
        Me.Name = name
        Me.Index = name
        Me.ColumnHeader = header
        Me.Visible = visible
        Me.Width = width
        Me.Sortable = sortable
        Me.Editable = editable
        Me.Align = align
        If Datatype Is GetType(Boolean) Then
            Me._dataType = GetType(Boolean)
            Me.SortType = SortTypes.text
            Me.EditType = EditTypes.checkbox
        ElseIf Datatype Is GetType(Byte) Then
            Me._dataType = GetType(Integer)
            Me.Align = Alignment.right
        ElseIf Datatype Is GetType(Byte()) Then
            Me._dataType = GetType(Integer)
            Me.Align = Alignment.right
            Me.Hidden = True
        ElseIf Datatype Is GetType(Char) Then
            Me._dataType = GetType(String)
        ElseIf Datatype Is GetType(DateTime) Then
            Me._dataType = GetType(DateTime)
            Me.Align = Alignment.right
        ElseIf Datatype Is GetType(Decimal) Then
            Me._dataType = GetType(Double)
            Me.EditType = EditTypes.double
            Me.Align = Alignment.right
        ElseIf Datatype Is GetType(Double) Then
            Me._dataType = GetType(Double)
            Me.EditType = EditTypes.double
            Me.Align = Alignment.right
        ElseIf Datatype Is GetType(Int16) Then
            Me._dataType = GetType(Integer)
            Me.EditType = EditTypes.integer
            Me.Align = Alignment.right
        ElseIf Datatype Is GetType(Int32) Then
            Me._dataType = GetType(Integer)
            Me.EditType = EditTypes.integer
            Me.Align = Alignment.right
        ElseIf Datatype Is GetType(Int64) Then
            Me._dataType = GetType(Integer)
            Me.EditType = EditTypes.integer
            Me.Align = Alignment.right
        ElseIf Datatype Is GetType(SByte) Then
            Me._dataType = GetType(Integer)
            Me.Align = Alignment.right
        ElseIf Datatype Is GetType(Single) Then
            Me._dataType = GetType(Integer)
            Me.EditType = EditTypes.integer
            Me.Align = Alignment.right
        ElseIf Datatype Is GetType(String) Then
            Me._dataType = GetType(String)
        ElseIf Datatype Is GetType(TimeSpan) Then
            Me._dataType = GetType(DateTime)
            Me.Align = Alignment.right
        ElseIf Datatype Is GetType(UInt16) Then
            Me._dataType = GetType(Integer)
            Me.Align = Alignment.right
        ElseIf Datatype Is GetType(UInt32) Then
            Me._dataType = GetType(Integer)
            Me.Align = Alignment.right
        ElseIf Datatype Is GetType(UInt64) Then
            Me._dataType = GetType(Integer)
            Me.Align = Alignment.right
        Else
            Me._dataType = GetType(String)
        End If
    End Sub

End Class

Public Class ColumnUnbindableDataType
    Inherits Exception
    Public Sub New(ByVal Datatype As Type)
        MyBase.New("Binding for this " & Datatype.ToString & " undefined. Please add the rule.")
    End Sub
End Class


