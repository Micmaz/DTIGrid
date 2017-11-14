Imports BaseClasses
Imports DTIMiniControls

Public Class DTIDataGrid
    Inherits DTIGrid

#Region "Private Variables"
    Private btnAdd As New ClientButton
    Private btnDel As New ClientButton
    Private btnCancel As New ClientButton
    Private WithEvents btnSave As New Button
    'Private pk As String = ""
#End Region

#Region "Private Properties"

    Public Property DataTableSearch() As String = ""

    Private Property searchString() As String
        Get

            If ViewState(Me.ClientID & "_SearchString") Is Nothing Then
                ViewState(Me.ClientID & "_SearchString") = ""
            End If
            If DataTableSearch.Trim = "" Then
                Return ViewState(Me.ClientID & "_SearchString")
            Else
				if ViewState(Me.ClientID & "_SearchString") = "" then Return DataTableSearch
				Return DataTableSearch & " AND " & ViewState(Me.ClientID & "_SearchString")
            End If
        End Get
        Set(ByVal value As String)
            ViewState(Me.ClientID & "_SearchString") = value
        End Set
    End Property

	Private Property searchVal() As Object
		Get

			If ViewState(Me.ClientID & "_SearchVal") Is Nothing Then
				ViewState(Me.ClientID & "_SearchVal") = ""
			End If
			Return ViewState(Me.ClientID & "_SearchVal")
		End Get
		Set(ByVal value As Object)
			ViewState(Me.ClientID & "_SearchVal") = value
		End Set
	End Property

	Private ReadOnly Property sqlhelper() As BaseClasses.BaseHelper
        Get
            Return DataBase.getHelper
        End Get
    End Property

    Private _Gridhelper As BaseClasses.BaseHelper = Nothing
    Private ReadOnly Property GridHelper() As BaseClasses.BaseHelper
        Get
            If _Gridhelper Is Nothing Then
                If gridConnection IsNot Nothing Then
                    _Gridhelper = BaseClasses.DataBase.createHelper(gridConnection)
                Else
                    _Gridhelper = BaseClasses.DataBase.createHelper(sqlhelper.defaultConnection)
                    _Gridhelper.createAdaptorsWithoutPrimaryKeys = sqlhelper.createAdaptorsWithoutPrimaryKeys
                End If
            End If
            Return _Gridhelper
        End Get
    End Property

    'Private ReadOnly Property isCancelClicked() As Boolean
    '    Get
    '        Return Page.Request.Params(btnCancel.UniqueID) IsNot Nothing
    '    End Get
    'End Property

    'Private ReadOnly Property isSaveClicked() As Boolean
    '    Get
    '        Return Page.Request.Params(btnSave.UniqueID) IsNot Nothing
    '    End Get
    'End Property
#End Region

#Region "Properties"

    Private gridConnectionValue As System.Data.Common.DbConnection
    Public Property gridConnection() As System.Data.Common.DbConnection
        Get
            Return gridConnectionValue
        End Get
        Set(ByVal value As System.Data.Common.DbConnection)
            gridConnectionValue = value
            _Gridhelper = Nothing
        End Set
    End Property

    Private _dataTableName As String = ""
    Private originalTableName As String = ""

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("")> _
    Public Property DataTableName() As String
        Get
            Return _dataTableName
        End Get
        Set(ByVal value As String)
            originalTableName = value
            If isSelect(value) Then
                Dim r1 As Regex = New Regex( _
                      "order\s+by\s+(?<sortcol>\[?\w+\]?)\s*(?<sortorder>(?:asc|des" + _
                      "c))?\s*\z", _
                    RegexOptions.IgnoreCase _
                    Or RegexOptions.Multiline _
                    Or RegexOptions.CultureInvariant _
                    Or RegexOptions.Compiled)
                If r1.IsMatch(value) Then
                    Dim m1 As Match = r1.Matches(value)(0)
                    Me.SortColumn = m1.Groups("sortcol").Value
                    Me.SortOrder = m1.Groups("sortorder").Value
                    value = value.Replace(m1.Value, "")
                End If
                If Not _dataTableName.EndsWith("datatable1") Then _
                    _dataTableName = "(" & value & ") as datatable1"
            Else
                _dataTableName = value
            End If
        End Set
    End Property

    Private _datatableKey As String = ""

    ''' <summary>
    ''' When setting DataSource to a Table Name or a Query that Tables Primary Key will need to be
    ''' defined.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("When setting DataSource to a Table Name or a Query that Tables Primary Key will need to be   defined.")> _
    Public Property DataTableKey() As String
        Get
            Return _datatableKey
        End Get
        Set(ByVal value As String)
            _datatableKey = value
        End Set
    End Property


	Private _paramArr As List(Of Object) = New List(Of Object)

	''' <summary>
	''' When Setting DataSource to a Table name or a query and addition params object can be passed
	''' for safe filtering
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("When Setting DataSource to a Table name or a query and addition params object can be passed   for safe filtering")>
	Public Property DataTableParamArray() As List(Of Object)
		Get
			Return _paramArr
		End Get
		Set(ByVal value As List(Of Object))
			_paramArr = value
		End Set
	End Property

	Private _customEvents As Boolean = False

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("")> _
    Public Property CustomEditEvents() As Boolean
        Get
            Return _customEvents
        End Get
        Set(ByVal value As Boolean)
            _customEvents = value
        End Set
    End Property

    Private _customSearch As Boolean = False
    Public Property CustomSearchEvents() As Boolean
        Get
            Return _customSearch
        End Get
        Set(ByVal value As Boolean)
            _customSearch = value
        End Set
    End Property

    Private _customSort As Boolean = False
    Public Property CustomSortEvents() As Boolean
        Get
            Return _customSort Or _customSearch
        End Get
        Set(ByVal value As Boolean)
            _customSort = value
        End Set
    End Property

    Private _enableAdding As Boolean = True

    ''' <summary>
    ''' Determines if Rows can be added when EnableEditing is true
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Determines if Rows can be added when EnableEditing is true")> _
    Public Property EnableAdding() As Boolean
        Get
            Return _enableAdding
        End Get
        Set(ByVal value As Boolean)
            _enableAdding = value
        End Set
    End Property

    Private _enableDeleting As Boolean = True
    Public Property EnableDeleting() As Boolean
        Get
            Return _enableDeleting
        End Get
        Set(ByVal value As Boolean)
            _enableDeleting = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or Sets the DataSource of the DTIDataGrid object.  Can be either a DataTable, a 
    ''' name of a datatable, or a query for a dataTable (set DataTableKey if the datasource is 
    ''' one of the latter two).  Editing will not work on a table that
    ''' cannnot be mapped to the database.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Gets or Sets the DataSource of the DTIDataGrid object. Can be either a DataTable, a    name of a datatable, or a query for a dataTable (set DataTableKey if the datasource is    one of the latter two). Editing will not work on a table that   cannnot be mapped to the database.")> _
    Public Shadows Property DataSource() As Object
        Get
            Return MyBase.DataSource
        End Get
        Set(ByVal value As Object)
            If value is Nothing OrElse value.GetType Is GetType(String) Then
                DataTableName = value
                'If CType(value, String).Contains("select") Then
                '    _dataTableName = value '"(" & value & ") as datatable1"
                'Else
                '    _dataTableName = value
                'End If
                MyBase.DataSource = Nothing
            Else
                MyBase.DataSource = value
            End If
        End Set
    End Property

    ''' <summary>
    ''' Determines if a built in save was clicked within the DTIDataGrid
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Determines if a built in save was clicked within the DTIDataGrid")> _
    Public ReadOnly Property isSaveButtonClicked() As Boolean
        Get
            If DesignMode Then Return False
            If Me.hfPageCommand.Value = "save" Then Return True
            Return Page.Request.Params(btnSave.UniqueID) IsNot Nothing
        End Get
    End Property

    Public Property DefaultConnection() As Data.Common.DbConnection
        Get
            Return GridHelper.defaultConnection
        End Get
        Set(ByVal value As System.Data.Common.DbConnection)
            If value IsNot Nothing Then
                GridHelper.defaultConnection = value
            End If
        End Set
    End Property
#End Region

#Region "Events"

    'Important note on events: In the dataGrid RowAdded, RowUpdated and rowDeleted happens after the change has been applied to the underlying datatable.
    'This overrides the data change events from the base class, which throws the events after the data has been parsed.
    Public Event BeforeRowAdded(ByRef row As DTIGridRow)
    Public Shadows Event RowAdded(ByRef row As DTIGridRow)
    Public Event BeforeRowUpdated(ByRef row As DTIGridRow)
    Public Shadows Event RowUpdated(ByRef row As DTIGridRow)
    Public Event BeforeRowDeleted(ByRef row As DTIGridRow)
    Public Shadows Event RowDeleted(ByRef row As DTIGridRow)
    Public Event SearchRequest(ByVal columnName As String, ByVal SearchValue As String)
    Public Event SortRequest(ByVal columnName As String, ByVal sortOrder As String, ByVal SearchColumn As String, ByVal SearchValue As String)
    Public Shadows Event UpdatesComplete()
    Public Event BeforeSaveClicked(ByRef dt As DataTable)
    Public Event AfterSaveClicked(ByRef dt As DataTable)
    Public Event DataFetched(ByRef dt As DataTable)
#End Region

    Sub New()
        'Me.Controls.Add(pnlPager)
        ButtonPlaceHolder.Controls.Add(btnAdd)
        ButtonPlaceHolder.Controls.Add(btnDel)
        ButtonPlaceHolder.Controls.Add(btnSave)
        ButtonPlaceHolder.Controls.Add(btnCancel)
    End Sub

    Private Sub DTIDataGrid_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        changeInnerControlIds()
        changeInnerControlIds2()
        'If Not Me.DataTableName = "" Then
        '    If dt Is Nothing Then Me.databind(False)
        'End If
        If DesignMode Then Return
        If Page.IsPostBack AndAlso Page.Request.Params(Me.btnSave.UniqueID) IsNot Nothing Then
            _returnSavedDt = True
            'setpk()
            'Binding()
        End If
    End Sub

    Public Overrides Sub databind()
        'fix paging errors
        If PageSize <= 0 Then
            PageSize = 15
        ElseIf PageCount > 0 AndAlso PageIndex > PageCount Then
            PageIndex = 1
        End If
        'This is so I don't go through my binding method on load but only after the 
        'button in the grid was clicked
        If Not isGridButtonClicked AndAlso Not isSaveButtonClicked Then
            Binding()
        Else
            MyBase.DataBind()
        End If
        If doSave Then doSaveMethods()
    End Sub

    Protected Function isSelect(ByVal cmd As String) As Boolean
        Dim r0 As Regex = New Regex("select", RegexOptions.IgnoreCase Or RegexOptions.Multiline Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Return r0.IsMatch(cmd)
    End Function

    Public Sub Binding(Optional ByVal allowRefetch = False)
        If DesignMode Then Return
        fetchdata(allowRefetch)
        MyBase.DataBind()
        setupPageButtons()
    End Sub

    Public Function getTableName() As String
        Dim name As String = ""
        If _dataTableName <> "" Then
            name = _dataTableName
        Else
            name = dt.TableName
        End If
        If name.ToLower.Contains("from") Then
            Dim r As Regex = New Regex( _
                  "from\s*(?<table>\x5B?\w*\x5D?)", _
                RegexOptions.IgnoreCase _
                Or RegexOptions.Multiline _
                Or RegexOptions.CultureInvariant _
                Or RegexOptions.Compiled _
                )
            For Each singleMatch As Match In r.Matches(name)
                If Not singleMatch.ToString = "" Then
                    name = singleMatch.Groups("table").Value
                    Exit For
                End If
            Next
        End If
        Return name
    End Function

    Private Sub FillTablePasser(ByVal sql As String, ByVal dt As DataTable, ByVal ParamArray parms() As Object)
        Dim l As New ArrayList       
        l.Add(dt)
        Try
            For Each itm As Object In DataTableParamArray
                l.Add(itm)
            Next
        Catch ex As Exception

        End Try
        Array.Resize(parms, l.Count)
        l.ToArray.CopyTo(parms, 0)
        GridHelper.FillDataTable(sql, parms)
    End Sub

    Private firstFetched As Boolean = True
	Public Function fetchdata(Optional ByVal allowRefetch As Boolean = True) As Boolean
		Dim retval As Boolean = True
		If allowRefetch OrElse firstFetched Then
			If _dataTableName <> "" Then
				DataSource = New DataTable
			ElseIf DataSource Is Nothing Then
				'Nothing was set
				DataSource = New DataTable
				Exit Function
			End If
			Try
				If EnableEditing OrElse EnableSorting OrElse EnablePaging Then
					'Dim n As String = getTableName()
					'Dim da As System.Data.Common.DbDataAdapter = GridHelper.Adaptor(n)
					'da.FillSchema(dt, SchemaType.Mapped)
				End If
			Catch ex As Exception
				If EnableEditing Then
					'dataError(New Exception("Could not enable editing.<br>" & ex.Message))
				End If
				retval = False
				Me.EnableEditing = False
			End Try

			'setpk()

			'if Datatable name is set handling any sorting errors is up to the user
			'Sorting errors only occur with switching out grid object with different datatables.
			'If dt.Columns(SortColumn) Is Nothing AndAlso Not _dataTableName.StartsWith("(") AndAlso retval Then
			'    SortColumn = ""
			'    SortOrder = ""
			'End If
			If firstFetched Then
				If Not String.IsNullOrEmpty(searchVal) Then DataTableParamArray.Add(searchVal)
			End If
			If Me._dataTableName <> "" AndAlso Not _returnSavedDt Then
				Try
					If dt Is Nothing Then DataSource = New DataTable
					dt.Clear()
					If Not EnablePaging Then
						Try
							If isSelect(_dataTableName) Then
								FillTablePasser(originalTableName, dt)
								Me.PageSize = dt.Rows.Count
							Else
								GridHelper.FillDataTable("Select top 100 * from " & _dataTableName, dt, DataTableParamArray.ToArray())
								If PageSize = 100 Then
									dataError(New Exception("Only the first 100 rows from the table are displayed."))
								End If
							End If
						Catch ex As Exception
							dataError(ex)
						End Try
					Else
						PageCount = GridHelper.GetSortedPage(dt, _dataTableName, PageSize, PageIndex, pk, SortString, searchString, DataTableParamArray.ToArray())
						If dt.Rows.Count = 0 AndAlso PageCount < PageIndex Then
							PageIndex = 1
							PageCount = GridHelper.GetSortedPage(dt, _dataTableName, PageSize, PageIndex, pk, SortString, searchString, DataTableParamArray.ToArray())
						End If
					End If
				Catch ex As Exception
					dataError(ex)
					'EnablePaging = False
					retval = False
					Try
						If isSelect(_dataTableName) Then
							FillTablePasser(originalTableName, dt)
							Me.PageSize = dt.Rows.Count
							'GridHelper.FillDataTable(originalTableName, l.ToArray)
						Else
							GridHelper.FillDataTable("Select top 100 * from " & _dataTableName, dt, DataTableParamArray.ToArray())
							Me.PageSize = dt.Rows.Count
						End If
						If EnableEditing AndAlso (Me.DataTableKey = "" OrElse dt.Columns(Me.DataTableKey) Is Nothing) Then
							dataError(ex)
							dataError(New Exception("The datatable key: " & DataTableKey & " was not found in the returned table. Editing and paging are disabled."))
							'Me.EnableEditing = False
							'Me.EnablePaging = False
							'Me.EnableSearching = False
						End If
					Catch ex1 As Exception
						dataError(ex)
						dataError(ex1)
					End Try
				End Try
			End If
			DataSource = dt
			Try
				If Not titleSet Then _title = dt.TableName
			Catch ex As Exception

			End Try
			firstFetched = False
			RaiseEvent DataFetched(dt)
		End If

		Return retval
	End Function

	Private ReadOnly Property pk() As String
        Get
            If DataTableKey <> "" Then
                Return DataTableKey
            End If
            If dt.PrimaryKey.Length > 0 Then
                Return dt.PrimaryKey(0).ToString
            ElseIf DataTableName <> "" Then
                'If datatable is a select statement get the pri key for the first listed table only.
                If DataTableName.ToLower.Contains("select") Then
                    Dim r As Regex = New Regex("from\s*\(?\[?(?<TableName>\w*)", RegexOptions.IgnoreCase Or RegexOptions.Multiline Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
                    Dim tblname As String = r.Match(DataTableName).Groups("TableName").Value
                    Dim dtmp As New DataTable
                    sqlhelper.Adaptor(tblname).FillSchema(dtmp, SchemaType.Mapped)

                    If dtmp.PrimaryKey.Length > 0 Then
                        Return dtmp.PrimaryKey(0).ToString
                    End If
                    If dtmp.Columns.Contains("id") Then Return "id"
                Else

                    Dim dtmp As New DataTable
                    sqlhelper.Adaptor(DataTableName).FillSchema(dtmp, SchemaType.Mapped)

                    If dtmp.PrimaryKey.Length > 0 Then
                        Return dtmp.PrimaryKey(0).ToString
                    End If
                    If dtmp.Columns.Contains("id") Then Return "id"
                End If
            End If
            Return ""
        End Get
    End Property

    Protected Sub changeInnerControlIds2()
        Me.btnSave.ID = Me.ClientID & "_Save"
        Me.btnCancel.ID = Me.ClientID & "_Cancel"
        Me.btnAdd.ID = Me.ClientID & "_Add"
        setupButtons2()
    End Sub

    Protected Sub setupButtons2()
        Me.btnSave.Text = "Save"
        Me.btnSave.OnClientClick = "dtiSaveGrid('" & Me.ClientID & "');"
        Me.btnCancel.Text = "Cancel"
        Me.btnAdd.Text = "Add Row"
        If MultiSelect Then
            Me.btnDel.Text = "Delete Rows"
        Else
            Me.btnDel.Text = "Delete Row"
        End If
        Me.btnDel.OnClick = "dtiGridDelRow('" & Me.ClientID & "');"
        Me.btnCancel.OnClick = "$('#" & Me.ClientID & "_Table').jqGrid('restoreRow', '' + $('#" & Me.ClientID & "_Table').getGridParam('selrow'));"
    End Sub

    Private Sub DTIDataGrid_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        Dim data As String = "{"
        For Each c As DTIGridColumn In Me.Columns
            If c.Name = "dtigridstate" Then
                data &= c.Name & ":'Added',"
            Else
                data &= c.Name & ":'',"
            End If
        Next
        data = data.TrimEnd(",") & "}"
        If EnableEditing Then
            ButtonPlaceHolder.Visible = True
            btnAdd.Visible = EnableAdding
            btnDel.Visible = EnableDeleting
            btnSave.Visible = EnableEditing
        Else
            ButtonPlaceHolder.Visible = False
        End If
        Me.btnAdd.OnClick = "dtiAddRow('" & Me.ClientID & "'," & data & ");"
        'Me.btnAdd.OnClick = "$('#" & Me.ClientID & "_Table').jqGrid('editGridRow', 'new',{addCaption:'Add Record',bSubmit:'Add',bCancel: 'Cancel'} )"
        AddButtonJquery(btnAdd, btnSave, btnDel, btnCancel)
        If Not errorDiv Is Nothing Then Me.Controls.Add(errorDiv)
        If Title Is Nothing OrElse Title.ToLower = "table1" Then
            Title = getTableName().Replace("[", "").Replace("]", "")
        End If
    End Sub

    Private Function setColumnValue(ByVal rw As DataRow, ByVal column As DataColumn, ByVal value As Object) As Boolean
        Try
            If column.AutoIncrement Then
            ElseIf Not column.ReadOnly Then
                If column.DataType Is GetType(DateTime) Then
                    Try
                        If value IsNot DBNull.Value Then
                            rw.Item(column.ColumnName) = Date.Parse(value)
                        Else
                            rw.Item(column.ColumnName) = column.DefaultValue
                        End If
                    Catch ex As Exception
                        dataError(ex, column.ColumnName)
                        rw.Item(column.ColumnName) = DBNull.Value
                        Return False
                    End Try
                    'Date.TryParse(row.Item(column.ColumnName), rw.Item(column.ColumnName))
                Else
                    Try
                        If value IsNot DBNull.Value Then
                            rw(column.ColumnName) = value
                        Else
                            rw.Item(column.ColumnName) = column.DefaultValue
                        End If
                    Catch ex As Exception
                        If Not value = "" Then
                            dataError(ex, column.ColumnName)
                        End If
                        If column.AllowDBNull Then
                            rw(column.ColumnName) = DBNull.Value
                        Else
                            rw(column.ColumnName) = 0
                        End If
                        Return False
                    End Try
                End If
            End If
            Return True
        Catch ex As Exception
            dataError(ex, column.ColumnName)
            Return False
        End Try

    End Function

    Private Sub RaiseRowevent(ByVal row As DTIGridRow)
        If row.RowState = DTIGridRow.DTIRowState.Added Then
            DTIDataGrid_RowAdded(row)
        ElseIf row.RowState = DTIGridRow.DTIRowState.Deleted Then
            DTIDataGrid_RowDeleted(row)
        ElseIf row.RowState = DTIGridRow.DTIRowState.Updated Then
            DTIDataGrid_RowUpdated(row)
        End If
    End Sub

    Private Sub DTIDataGrid_RowDeleted(ByRef row As DTIGridRow) Handles MyBase.RowDeleted
        If EnableEditing AndAlso EnableDeleting Then
            RaiseEvent BeforeRowDeleted(row)
            If row.RowState = DTIGridRow.DTIRowState.Deleted Then
                If Not CustomEditEvents Then
                    Dim row1 As DataRow = row.dataRow
                    'For Each row1 As DataRow In dt.Rows
                    If row1.RowState <> DataRowState.Deleted AndAlso pk IsNot Nothing AndAlso row1.Item(pk) = row.Item(pk) Then
                        row1.Delete()
                        '    Exit For
                    End If
                    'Next
                End If
                RaiseEvent RowDeleted(row)
            Else
                RaiseRowevent(row)
            End If
        End If
    End Sub

    Private Sub DTIDataGrid_RowUpdated(ByRef row As DTIGridRow) Handles MyBase.RowUpdated
        If EnableEditing Then
            RaiseEvent BeforeRowUpdated(row)
            If row.RowState = DTIGridRow.DTIRowState.Updated Then
                If Not CustomEditEvents Then
                    Dim row1 As DataRow = row.dataRow
                    'For Each row1 As DataRow In dt.Rows
                    Try
                        '        If Not pk = "" AndAlso row1.Table.Columns.Contains(pk) Then
                        '            If row1.Item(pk) = row.Item(pk) Then
                        For Each column As DataColumn In dt.Columns
                            setColumnValue(row1, column, row(column.ColumnName))
                        Next
                        '            End If
                        '        Else
                        '            dataError(New Exception("No primary key data could be found for this table. Please specify one manually."))
                        '        End If
                    Catch ex As Exception
                        dataError(ex)
                    End Try
                    'Next
                End If
                RaiseEvent RowUpdated(row)
            Else
                RaiseRowevent(row)
            End If
        End If
    End Sub

    Private Sub DTIDataGrid_RowAdded(ByRef row As DTIGridRow) Handles MyBase.RowAdded
        If EnableEditing AndAlso EnableAdding Then
            RaiseEvent BeforeRowAdded(row)
            If row.RowState = DTIGridRow.DTIRowState.Added Then
                Dim reversedPK As Boolean = False
                'If dt.DataSet Is Nothing Then
                '    Dim ds As New DataSet
                '    ds.EnforceConstraints = False
                '    ds.Tables.Add(dt)
                'End If
                'Dim enforceConst As Boolean = dt.DataSet.EnforceConstraints

                If Not dt.PrimaryKey Is Nothing AndAlso dt.PrimaryKey.Length = 1 Then
                    If dt.PrimaryKey(0).AutoIncrement AndAlso Not dt.PrimaryKey(0).AutoIncrementStep = -1 Then
                        reversedPK = True
                        dt.PrimaryKey(0).AutoIncrementStep = -1
                        dt.PrimaryKey(0).AutoIncrementSeed = -10000
                    End If
                End If
                Try
                    If Not CustomEditEvents Then
                        Dim rw As DataRow = dt.NewRow
                        Dim noerror As Boolean = True
                        For Each column As DataColumn In dt.Columns
                            noerror = setColumnValue(rw, column, row(column.ColumnName))
                            'If Not noerror Then Exit For
                        Next
                        'If noerror Then _
                        dt.Rows.Add(rw)
                    End If
                Catch ex As Exception
                    dataError(ex)
                End Try
                'If reversedPK Then
                '    dt.PrimaryKey(0).AutoIncrementSeed = (dt.PrimaryKey(0).AutoIncrementSeed * -1) - 1
                '    dt.PrimaryKey(0).AutoIncrementStep = dt.PrimaryKey(0).AutoIncrementStep * -1
                'End If
                RaiseEvent RowAdded(row)
            Else
                RaiseRowevent(row)
            End If
        End If

    End Sub

    Private Sub DTIDataGrid_PageChanged(ByVal index As Integer) Handles Me.PageChanged
        If DataTableName = "" Then
            DataTableName = CType(Me.SavedGrid, DTIDataGrid).DataTableName
        End If
        Binding(True)
    End Sub

	Private Sub DTIGrid_SearchData(ByVal columnName As String, ByVal mySearchValue As String) Handles Me.Searching
		If DataTableName = "" Then
			DataTableName = CType(Me.SavedGrid, DTIDataGrid).DataTableName
		End If
		If Not CustomSearchEvents Then 'AndAlso Not isCancelClicked Then
			searchVal = mySearchValue
			searchString = ""
			If Not (searchVal.StartsWith("""") AndAlso searchVal.EndsWith("""")) Then
				searchVal = "%" & searchVal & "%"
			End If
			If searchVal.Length = 0 Then
				searchString = ""
			Else
				If columnName.Contains(",") Then
					For Each val As String In columnName.Split(",")
						searchString &= val & " LIKE @searchval OR "
					Next
					If searchString.EndsWith("OR ") Then
						searchString = searchString.Substring(0, searchString.LastIndexOf("OR "))
					End If
				Else
					searchString = columnName & " LIKE @searchval "
				End If
			End If
			Me.Binding(True)
		End If
		RaiseEvent SearchRequest(columnName, searchVal)
	End Sub

	Private Sub DTIGrid_SortData(ByVal columnName As String, ByVal sortOrder As String, ByVal SearchColumn As String, ByVal SearchValue As String) Handles MyBase.Sorting
        If Not CustomSortEvents Then 'AndAlso Not isCancelClicked Then
            Me.SortOrder = sortOrder
            Me.SortColumn = columnName
            Me.Binding(True)
        End If
        RaiseEvent SortRequest(columnName, sortOrder, SearchColumn, SearchValue)
    End Sub

    Private doSave As Boolean = False
    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If _hasdatabound Then
            doSaveMethods()
        Else : doSave = True
        End If

    End Sub

    Private Sub doSaveMethods()
        RaiseEvent BeforeSaveClicked(dt)
        Try
            If dt.PrimaryKey.Length = 0 AndAlso GridHelper.createAdaptorsWithoutPrimaryKeys Then
                If DataTableKey IsNot Nothing AndAlso DataTableKey <> "" Then
                    dt.BeginLoadData()
                    dt.PrimaryKey = New DataColumn() {dt.Columns(DataTableKey)}
                    'If dt.PrimaryKey.GetType().ToString().StartsWith("Int") Then

                    '    Dim setAutoInc As Boolean = False
                    '    For Each row As DataRow In dt.Rows
                    '        If row(DataTableKey) Is DBNull.Value Then
                    '            setAutoInc = True
                    '            Exit For
                    '        End If
                    '    Next
                    '    If setAutoInc Then
                    '        dt.Columns(DataTableKey).AutoIncrement = True
                    '        dt.Columns(DataTableKey).AutoIncrementSeed = -1
                    '        dt.Columns(DataTableKey).AutoIncrementStep = -1
                    '    End If
                    'End If

                    GridHelper.removeCachedAdaptor(dt.TableName)
                    Try
                        dt.EndLoadData()
                    Catch ex As Exception

                    End Try

                End If
            End If
            If Not dt.TableName = getTableName() Then
                dt.TableName = getTableName()
            End If
            GridHelper.Update(dt)
        Catch ex As Exception
            dataError(New Exception("Error Saving to database:<br>" & ex.Message & "<br>", ex))
        End Try
        _returnSavedDt = False
        If Not ReadonlyLastReq Then
            ReadonlyLastReq = True
            Me.Binding(True)
            ReadonlyLastReq = False
        End If
        RaiseEvent AfterSaveClicked(dt)
    End Sub

End Class
