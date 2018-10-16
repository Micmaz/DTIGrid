Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports System.Web
Imports DTIMiniControls

Public Class DTIGrid
    Inherits WebControl

#Region "events"

    ''' <summary>
    ''' Occurs after the server control binds to a data source.
    ''' </summary>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Occurs after the server control binds to a data source.")> _
    Public Event DataBound()

    ''' <summary>
    ''' Occurs when a row is created in a GridView control.
    ''' </summary>
    ''' <param name="row"></param>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Occurs when a row is created in a GridView control.")> _
    Public Event RowAdded(ByRef row As DTIGridRow)

    ''' <summary>
    ''' Occurs when a data row is bound to data in a GridView control.
    ''' </summary>
    ''' <param name="row">DTIGridRow being bound</param>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Occurs when a data row is bound to data in a GridView control.")> _
    Public Event RowDataBound(ByRef row As DTIGridRow)

    ''' <summary>
    ''' Occurs on postback after the GridView control updates the row.
    ''' </summary>
    ''' <param name="row">Updated Row</param>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Occurs on postback after the GridView control updates the row.")> _
    Public Event RowUpdated(ByRef row As DTIGridRow)

    ''' <summary>
    ''' Occurs on postback after the GridView control deletes the row.
    ''' </summary>
    ''' <param name="row">Delted Row</param>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Occurs on postback after the GridView control deletes the row.")> _
    Public Event RowDeleted(ByRef row As DTIGridRow)

    ''' <summary>
    ''' Occurs after a new DTIGridColumn has be created.
    ''' </summary>
    ''' <param name="col">DTIGridColumn that has been created.</param>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Occurs after a new DTIGridColumn has be created.")> _
    Public Event ColumnCreated(ByRef col As DTIGridColumn)

    ''' <summary>
    ''' Occurs when a column is clicked only when sorting is enabled.
    ''' </summary>
    ''' <param name="SortColumn">Column clicked for sorting</param>
    ''' <param name="SortDirection">Direction column is to be sorted Asc or Desc</param>
    ''' <param name="SearchColumn">Column Searched on if applicable</param>
    ''' <param name="SearchValue">Value Searched for is applicable</param>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Occurs when a column is clicked only when sorting is enabled.")> _
    Public Event Sorting(ByVal SortColumn As String, ByVal SortDirection As String, ByVal SearchColumn As String, ByVal SearchValue As String)

    ''' <summary>
    ''' Occurs when a the search button is clicked on when searching is enabled.
    ''' </summary>
    ''' <param name="SearchColumn">Column to be searched</param>
    ''' <param name="SearchValue">Value to be searched for</param>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Occurs when a the search button is clicked on when searching is enabled.")> _
    Public Event Searching(ByVal SearchColumn As String, ByVal SearchValue As String)

    ''' <summary>
    ''' Occurs after all rows have been processed
    ''' </summary>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Occurs after all rows have been processed")> _
    Public Event UpdatesComplete As EventHandler

    ''' <summary>
    ''' Occurs when Autopostback is set to true and a row has been clicked.
    ''' </summary>
    ''' <param name="row">Rows Selected</param>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Occurs when Autopostback is set to true and a row has been clicked.")> _
    Public Event Click(ByRef row As DTIGridRow)

    ''' <summary>
    ''' Fired whenever the page changes
    ''' </summary>
    ''' <param name="index"></param>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Fired whenever the page changes")> _
    Public Event PageChanged(ByVal index As Integer)

    ''' <summary>
    ''' Occurs before the server control binds to a data source.
    ''' </summary>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Occurs before the server control binds to a data source.")> _
    Protected Shadows Event Databinding()

    Public Event gridBindingError(ByVal ex As Exception)

#End Region

#Region "Private Controls"
    Protected tbl As New Table
    Protected postbackData As New HiddenField
    Protected sPostbackData As New HiddenField
    Protected hfSelectedRows As New HiddenField
    Protected hfDeletedRows As New HiddenField
    Protected hfPageCommand As New HiddenField

	Private script As New LiteralControl

	Private _columns As DTIGridColumnCollection
    Private _rows As DTIGridRowCollection
    Private _selectedRows As DTIGridRowCollection
    Private _updatedRows As DTIGridRowCollection
    Private _deletedRows As DTIGridRowCollection
    Private _addedRows As DTIGridRowCollection
    Protected _title As String
    Private _multiSelect As Boolean = False
    'Private _sortName As String
    'Private _sortOrder As String = ""
    Private _clientSelect As String
    Private _enableSearching As Boolean = False
    Private _onRowSelectPostback As Boolean = False
    Private _enableSorting As Boolean = True
    'Private _ReadOnlyMode As Boolean = True
    Private _enableEditing As Boolean = False
    Private _enablePaging As Boolean = True
    Protected _hasdatabound = False
    'Private _rowsPerPage As Integer = 0
    'Private _PageCount As Integer = 0
    Private _showDateAndTime As Boolean
    Private _foriegnKeyDropDows As Boolean
    Private _enableAltRows As Boolean = True
    Private _altRowsCssClass As String = "ui-priority-secondary altrow"

    Private WithEvents btnRowSelect As New Button
    Private WithEvents btnSort As New Button

    'search stuff
    Private pnlSearch As New Panel
    Private WithEvents ddlSearch As New DropDownList
    Private WithEvents tbSearch As New TextBox
    Private WithEvents cbSearch As New CheckBox
    Private litSearchLabel As New Literal
    Private litSearchForLabel As New Literal
    Private WithEvents btnSearch As New Button
    'paging
    Private phgridarea As New PlaceHolder
    Private pnlPager As New Panel
    Private WithEvents btnNext As New Button
    Private WithEvents btnPrev As New Button
    Private WithEvents btnFirst As New Button
    Private WithEvents btnLast As New Button
    Private WithEvents btnExcel As New ImageButton
    Private WithEvents tbPageNum As New TextBox
    Private lblSeperator As New Literal
    Private lblPages As New Literal

    Private savedDt As DataTable 'Only used for paging datatable

    ''' <summary>
    ''' PlaceHolder directly next to the paging buttons for adding controls in inherited class
    ''' </summary>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("PlaceHolder directly next to the paging buttons for adding controls in inherited class")> _
    Protected ButtonPlaceHolder As New PlaceHolder
#End Region

#Region "Private Properties"

    Protected _returnSavedDt As Boolean = False
    Friend ReadOnly Property dt() As DataTable
        Get
            If DataSource IsNot Nothing AndAlso Not TypeOf DataSource Is DataTable Then
                If TypeOf DataSource Is DataView Then
                    DataSource = CType(DataSource, DataView).ToTable
                ElseIf TypeOf DataSource Is DataSet AndAlso DataMember <> "" Then
                    Dim ds As DataSet = CType(DataSource, DataSet)
                    DataSource = ds.Tables(DataMember)
                End If
            End If
            If Page Is Nothing Then Return Nothing
            If DataSource Is Nothing AndAlso Page.IsPostBack Then
				If _returnSavedDt AndAlso SavedGrid IsNot Nothing Then
					Return SavedGrid.DataSource
				End If
			End If
            Return DataSource
        End Get
    End Property

    Public Property Showerrors As Boolean
        Get
            Return errorDiv.Visible
        End Get
        Set(value As Boolean)
            errorDiv.Visible = value
        End Set
    End Property


    Protected Property SavedGrid() As DTIGrid
        Get
            If DesignMode Then Return Nothing
            If Not Page.IsPostBack Then Page.Session(HttpContext.Current.Request.RawUrl & Me.ClientID & "_SavedGrid") = New DTIGrid()
            If Page.Session(HttpContext.Current.Request.RawUrl & Me.ClientID & "_SavedGrid") Is Nothing Then

            End If
            Return Page.Session(HttpContext.Current.Request.RawUrl & Me.ClientID & "_SavedGrid")
        End Get
        Set(ByVal value As DTIGrid)
            If DesignMode Then Return
            Page.Session(HttpContext.Current.Request.RawUrl & Me.ClientID & "_SavedGrid") = value
        End Set
    End Property

    Friend ReadOnly Property SortString() As String
        Get
            If SortColumn.Trim = "" Then Return ""
            Return (SortColumn & " " & SortOrder).Trim
        End Get
    End Property

    Protected Property ReadonlyLastReq() As Boolean
        Get
            If DesignMode Then Return Nothing
            Return Page.Session(Me.UniqueID & "_readonlyflag")
        End Get
        Set(ByVal value As Boolean)
            If DesignMode Then Return
            Page.Session(Me.UniqueID & "_readonlyflag") = value
        End Set
    End Property

    Private cancelParse As Boolean = False
    Protected ReadOnly Property DataString() As String
        Get
            If DesignMode Then Return Nothing
            If cancelParse Then Return Nothing
            Return Page.Request.Form(postbackData.UniqueID) 'postbackData.Value
        End Get
    End Property

    Public Property ExcelExport() As Boolean
        Get
            Return btnExcel.Visible
        End Get
        Set(value As Boolean)
            btnExcel.Visible = value
        End Set
    End Property
#End Region

#Region "Properties"
    Private _datasource As Object

    ''' <summary>
    ''' Gets or sets the data source that the DTIGrid is displaying data for.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.ComponentModel.Description("Gets or sets the data source that the DTIGrid is displaying data for.")> _
    Public Property DataSource() As Object
        Get
            Return _datasource
        End Get
        Set(ByVal value As Object)
            _datasource = value
        End Set
    End Property

	Public ReadOnly Property rowCount As Integer
		Get
			If dt Is Nothing Then Return 0
			Return dt.Rows.Count
		End Get
	End Property


	Public Property ajaxEnable() As Boolean = False

	''' <summary>
	''' Comma delimited list of columns that are visible. By default all are visable unless this property is set. 
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Comma delimited list of columns that are visible. By default all are visable unless this property is set.")>
	Public Property visibleColumns() As String = ""

	''' <summary>
	''' Comma delimited list of columns to hide. Overides visibleColumns. 
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Comma delimited list of columns to hide. Overides visibleColumns.")>
	Public Property hiddenColumns() As String = ""

	''' <summary>
	''' Gets or sets the name of the list or table in the data source for which the DTIGrid is displaying data.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets or sets the name of the list or table in the data source for which the DTIGrid is displaying data.")>
	Public Property DataMember() As String = ""

	''' <summary>
	''' Gets a collection that contains all the columns in the control.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets a collection that contains all the columns in the control.")>
	Public ReadOnly Property Columns() As DTIGridColumnCollection
		Get
			EnsureChildControls()
			Return _columns
		End Get
	End Property

	''' <summary>
	''' Comma delimited list of column titles.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Comma delimited list of column titles.")>
	Public Property ColumnTitles As String = Nothing

	''' <summary>
	''' Comma delimited list of column widths
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Comma delimited list of column widths")>
	Public Property ColumnWidths As String = Nothing

	''' <summary>
	''' Gets a collection that contains all the rows in the DTIGrid control.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets a collection that contains all the rows in the DTIGrid control.")>
	Public ReadOnly Property Rows() As DTIGridRowCollection
		Get
			EnsureChildControls()
			Return _rows
		End Get
	End Property

	''' <summary>
	''' Gets the collection of rows selected by the user.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets the collection of rows selected by the user.")>
	Public ReadOnly Property SelectedRows() As DTIGridRowCollection
		Get
			Return _selectedRows
		End Get
	End Property

	''' <summary>
	''' Gets a collection that contains all the updated rows in the DTIGrid control.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets a collection that contains all the updated rows in the DTIGrid control.")>
	Public ReadOnly Property UpdatedRows() As DTIGridRowCollection
		Get
			Return _updatedRows
		End Get
	End Property

	''' <summary>
	''' Gets a collection that contains all the deleted rows in the DTIGrid control.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets a collection that contains all the deleted rows in the DTIGrid control.")>
	Public ReadOnly Property DeletedRows() As DTIGridRowCollection
		Get
			Return _deletedRows
		End Get
	End Property

	''' <summary>
	''' Gets a collection that contains all the added rows in the DTIGrid control.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets a collection that contains all the added rows in the DTIGrid control.")>
	Public ReadOnly Property AddedRows() As DTIGridRowCollection
		Get
			Return _addedRows
		End Get
	End Property

	Protected titleSet As Boolean = False

	''' <summary>
	''' Gets or sets the Title associated with the DTIGrid control.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets or sets the Title associated with the DTIGrid control.")>
	Public Property Title() As String
		Get
			Return _title
		End Get
		Set(ByVal value As String)
			_title = value
			titleSet = True
		End Set
	End Property

	''' <summary>
	''' Gets a value indicating whether the items in the DTIGrid control are 
	''' sorted in ascending or descending order, or are not sorted.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets a value indicating whether the items in the DTIGrid control are    sorted in ascending or descending order, or are not sorted.")>
	Public Property SortOrder() As String
		Get
			If ViewState(Me.ClientID & "_sortOrder") Is Nothing Then
				ViewState(Me.ClientID & "_sortOrder") = _SortOrder
			End If
			If Not (ViewState(Me.ClientID & "_sortOrder") = "asc" Or ViewState(Me.ClientID & "_sortOrder") = "desc") Then
				ViewState(Me.ClientID & "_sortOrder") = "asc"
			End If
			Return ViewState(Me.ClientID & "_sortOrder")
		End Get
		Set(ByVal value As String)
			_SortOrder = value.ToLower()
			ViewState(Me.ClientID & "_sortOrder") = value.ToLower()
		End Set
	End Property
	Private _SortOrder As String = "asc"

	''' <summary>
	''' Gets the column by which the DTIGrid contents are currently sorted.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets the column by which the DTIGrid contents are currently sorted.")>
	Public Property SortColumn() As String
		Get
			If ViewState(Me.ClientID & "_sortName") Is Nothing Then
				ViewState(Me.ClientID & "_sortName") = _SortColumn
			End If
			Return ViewState(Me.ClientID & "_sortName")
		End Get
		Set(ByVal value As String)
			_SortColumn = value
			ViewState(Me.ClientID & "_sortName") = value
		End Set
	End Property
	Private _SortColumn As String = ""

	''' <summary>
	''' Gets or sets a value indicating whether the user is allowed to select more 
	''' than one cell, row, or column of the DTIGrid at a time.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets or sets a value indicating whether the user is allowed to select more    than one cell, row, or column of the DTIGrid at a time.")>
	Public Property MultiSelect() As Boolean
		Get
			Return _multiSelect
		End Get
		Set(ByVal value As Boolean)
			_multiSelect = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the client-side script that executes when a row is selected.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets or sets the client-side script that executes when a row is selected.")>
	Public Property OnClientRowSelect() As String
		Get
			Return _clientSelect
		End Get
		Set(ByVal value As String)
			_clientSelect = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets a value indicating whether a postback to the server automatically 
	''' occurs when the user changes the row selection.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets or sets a value indicating whether a postback to the server automatically    occurs when the user changes the row selection.")>
	Public Property AutoPostBack() As Boolean
		Get
			Return _onRowSelectPostback
		End Get
		Set(ByVal value As Boolean)
			_onRowSelectPostback = value
			If value Then
				EnableEditing = False
			End If
		End Set
	End Property

	''' <summary>
	''' Gets or sets a value indicating whether a column can be sorted
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets or sets a value indicating whether a column can be sorted")>
	Public Property EnableSorting() As Boolean
		Get
			Return _enableSorting
		End Get
		Set(ByVal value As Boolean)
			_enableSorting = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets a value indicating whether the user can edit the cells of the DTIGrid control.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets or sets a value indicating whether the user can edit the cells of the DTIGrid control."), Obsolete("This Method is not very intutitive.  Use EnableEditing instead")>
	Public Property ReadOnlyMode() As Boolean
		Get
			Return Not EnableEditing
		End Get
		Set(ByVal value As Boolean)
			EnableEditing = Not value
		End Set
	End Property

	''' <summary>
	''' Gets or sets a value indicating whether the user can edit the cells of the DTIGrid control.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets or sets a value indicating whether the user can edit the cells of the DTIGrid control.")>
	Public Property EnableEditing() As Boolean
		Get
			Return _enableEditing
		End Get
		Set(ByVal value As Boolean)
			_enableEditing = value
			If value Then
				AutoPostBack = False
			End If
		End Set
	End Property

	''' <summary>
	''' Gets or sets a value indicating whether the search box is displayed
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets or sets a value indicating whether the search box is displayed")>
	Public Property EnableSearching() As Boolean
		Get
			Return _enableSearching
		End Get
		Set(ByVal value As Boolean)
			_enableSearching = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets a value indicating whether Paging is enabled.
	''' </summary>
	''' <value>
	'''   <c>true</c> if [enable paging]; otherwise, <c>false</c>.
	''' </value>
<System.ComponentModel.Description("Gets or sets a value indicating whether Paging is enabled.")> _
	Public Property EnablePaging() As Boolean
		Get
			Return _enablePaging
		End Get
		Set(ByVal value As Boolean)
			_enablePaging = value
		End Set
	End Property

	Private _pagesize As Integer = 15

	''' <summary>
	''' Gets or sets the number of rows to display on a page in a DTIGrid control.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets or sets the number of rows to display on a page in a DTIGrid control.")>
	Public Property PageSize() As Integer
		Get
			Return _pagesize
			'Dim num As Object = ViewState(Me.ClientID & "_pgSize")
			'If num Is Nothing Then
			'    Return 0
			'End If
			'Return CType(ViewState(Me.ClientID & "_pgSize"), Integer)
		End Get
		Set(ByVal value As Integer)
			_pagesize = value
			'If value > 0 Then
			'    ViewState(Me.ClientID & "_pgSize") = value
			'End If
		End Set
	End Property

	''' <summary>
	''' Gets or sets the index of the currently displayed page.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets or sets the index of the currently displayed page.")>
	Public Property PageIndex() As Integer
		Get
			Dim num As Object = ViewState(Me.ClientID & "_pgNum")
			If num Is Nothing Then
				Return 1
			End If
			Return CType(ViewState(Me.ClientID & "_pgNum"), Integer)
		End Get
		Set(ByVal value As Integer)
			ViewState(Me.ClientID & "_pgNum") = value
		End Set
	End Property

	''' <summary>
	''' Gets or Sets the total number of pages in the DTIGrid
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Gets or Sets the total number of pages in the DTIGrid")>
	Public Property PageCount() As Integer
		Get
			Dim num As Object = ViewState(Me.ClientID & "_pgCount")
			If num Is Nothing Then
				Return 0
			End If
			Return CType(ViewState(Me.ClientID & "_pgCount"), Integer)
		End Get
		Set(ByVal value As Integer)
			ViewState(Me.ClientID & "_pgCount") = value
		End Set
	End Property

	''' <summary>
	''' Puts a Date and time picker on columns whos datatypes are dates and makes the column
	''' display both date and time.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Puts a Date and time picker on columns whos datatypes are dates and makes the column   display both date and time.")>
	Public Property ShowDateAndTime() As Boolean
		Get
			Return _showDateAndTime
		End Get
		Set(ByVal value As Boolean)
			_showDateAndTime = value
		End Set
	End Property

	'Public Property ForiegnKeyDropDows() As Boolean
	'    Get
	'        Return _foriegnKeyDropDows
	'    End Get
	'    Set(ByVal value As Boolean)
	'        _foriegnKeyDropDows = value
	'    End Set
	'End Property

	''' <summary>
	''' Determines if page was changed within the DTIGrid
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Determines if page was changed within the DTIGrid")>
	Public ReadOnly Property isPageChangeClicked() As Boolean
		Get
			If DesignMode Then Return Nothing
			Return Page.Request.Params(btnNext.UniqueID) IsNot Nothing OrElse
				Page.Request.Params(btnPrev.UniqueID) IsNot Nothing OrElse
				Page.Request.Params(btnFirst.UniqueID) IsNot Nothing OrElse
				Page.Request.Params(btnLast.UniqueID) IsNot Nothing OrElse
				Page.Request.Params("__EVENTTARGET") = tbPageNum.UniqueID
		End Get
	End Property

	''' <summary>
	''' Determines if a built in search was clicked within the DTIGrid
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Determines if a built in search was clicked within the DTIGrid")>
	Public ReadOnly Property isSearchClicked() As Boolean
		Get
			If DesignMode Then Return False
			Return Page.Request.Params(btnSearch.UniqueID) IsNot Nothing
		End Get
	End Property

	''' <summary>
	''' Determines if a sort was made on the DTIGrid
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Determines if a sort was made on the DTIGrid")>
	Public ReadOnly Property isSortClicked() As Boolean
		Get
			If DesignMode Then Return False
			Return Page.Request.Params(btnSort.UniqueID) IsNot Nothing
		End Get
	End Property

	''' <summary>
	''' Determines if DTIGrid is performing an AutoPostBack
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Determines if DTIGrid is performing an AutoPostBack")>
	Public ReadOnly Property isGridClick() As Boolean
		Get
			If DesignMode Then Return False
			Return Page.Request.Params(btnRowSelect.UniqueID) IsNot Nothing
		End Get
	End Property

	''' <summary>
	''' Determines if a button (Sort, Search, Paging, AutoPostBack) was clicked
	''' within the DTIGrid
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Determines if a button (Sort, Search, Paging, AutoPostBack) was clicked   within the DTIGrid")>
	Public ReadOnly Property isGridButtonClicked() As Boolean
		Get
			Return isPageChangeClicked OrElse
				isSearchClicked OrElse
				isSortClicked OrElse
				isGridClick
		End Get
	End Property

	''' <summary>
	''' Set a zebra-striped grid
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Set a zebra-striped grid")>
	Public Property EnableAltRows() As Boolean
		Get
			Return _enableAltRows OrElse AltRowsCssClass <> ""
		End Get
		Set(ByVal value As Boolean)
			_enableAltRows = value
		End Set
	End Property

	''' <summary>
	''' The class that is used for alternate rows. Default is ui-priority-secondary
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("The class that is used for alternate rows. Default is ui-priority-secondary")>
	Public Property AltRowsCssClass() As String
		Get
			Return _altRowsCssClass
		End Get
		Set(ByVal value As String)
			_altRowsCssClass = value
		End Set
	End Property

	Private _shrinkToFit As Boolean = True

	''' <summary>
	''' This option describes the type of calculation of the initial width of each column 
	''' against with the width of the grid. If the value is true and the value in width 
	''' option is set then: Every column width is scaled according to the defined option 
	''' width. Example: if we define two columns with a width of 80 and 120 pixels, but 
	''' want the grid to have a 300 pixels - then the columns are recalculated as follow: 
	''' 1- column = 300(new width)/200(sum of all width)*80(column width) = 120 and 2 
	''' column = 300/200*120 = 180. The grid width is 300px. If the value is false and the 
	''' value in width option is set then: The width of the grid is the width set in option. 
	''' The column width are not recalculated and have the values defined in colModel. 
	''' If integer is set, the width is calculated according to it.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("This option describes the type of calculation of the initial width of each column    against with the width of the grid. If the value is true and the value in width    option is set then: Every column width is scaled according to the defined option    width. Example: if we define two columns with a width of 80 and 120 pixels, but    want the grid to have a 300 pixels - then the columns are recalculated as follow:    1- column = 300(new width)/200(sum of all width)*80(column width) = 120 and 2    column = 300/200*120 = 180. The grid width is 300px. If the value is false and the    value in width option is set then: The width of the grid is the width set in option.    The column width are not recalculated and have the values defined in colModel.    If integer is set, the width is calculated according to it.")>
	Public Property ShrinkToFit() As Boolean
		Get
			Return _shrinkToFit
		End Get
		Set(ByVal value As Boolean)
			_shrinkToFit = value
			shinkToFixExtrnallyChanged = True
		End Set
	End Property
	Private shinkToFixExtrnallyChanged As Boolean = False

	''' <summary>
	''' If true the control is rendered as an html table. Click event, sorting eding etc will not function (hey! it's a table!) Paging and searching still work fine.
	''' </summary>
	''' <value>
	'''   <c>true</c> if [render as table]; otherwise, <c>false</c>.
	''' </value>
<System.ComponentModel.Description("If true the control is rendered as an html table. Click event, sorting eding etc will not function (hey! it's a table!) Paging and searching still work fine.")> _
	Public Property renderAsTable() As Boolean = False

	''' <summary>
	''' If false grid cells will display html content as regular html. Set to true to see the "source view" for html cells.
	''' </summary>
	''' <value>
	'''   <c>true</c> if autoencode; otherwise, <c>false</c>.
	''' </value>
<System.ComponentModel.Description("If false grid cells will display html content as regular html. Set to true to see the ""source view"" for html cells.")> _
	Public Property autoencode() As Boolean = False


#End Region

	Sub New()
		_columns = New DTIGridColumnCollection()
		_rows = New DTIGridRowCollection(Me)
		_selectedRows = New DTIGridRowCollection(Me)
		_updatedRows = New DTIGridRowCollection(Me)
		_deletedRows = New DTIGridRowCollection(Me)
		_addedRows = New DTIGridRowCollection(Me)

        btnExcel.Visible = False
        sPostbackData.EnableViewState = False
		postbackData.EnableViewState = False
		hfSelectedRows.EnableViewState = False
		hfDeletedRows.EnableViewState = False

		pnlSearch.Controls.Add(litSearchLabel)
		pnlSearch.Controls.Add(ddlSearch)
		pnlSearch.Controls.Add(litSearchForLabel)
		pnlSearch.Controls.Add(tbSearch)
		pnlSearch.Controls.Add(cbSearch)
		pnlSearch.Controls.Add(btnSearch)
		pnlSearch.CssClass = "ui-widget"
		'pnlSearch.DefaultButton = btnSearch.ClientID

		Me.Controls.Add(pnlSearch)
		Me.Controls.Add(phgridarea)
		Me.Controls.Add(tbl)

		Dim bottomctrls As New Panel
		pnlPager.Controls.Add(btnFirst)
		pnlPager.Controls.Add(btnPrev)
		pnlPager.Controls.Add(tbPageNum)
		pnlPager.Controls.Add(lblSeperator)
		pnlPager.Controls.Add(lblPages)
		pnlPager.Controls.Add(btnNext)
		pnlPager.Controls.Add(btnLast)
		pnlPager.Style.Add("display", "inline")
		tbPageNum.AutoPostBack = True
		bottomctrls.Controls.Add(pnlPager)
		bottomctrls.Controls.Add(ButtonPlaceHolder)

		Me.Controls.Add(bottomctrls)

		Me.Controls.Add(postbackData)
		Me.Controls.Add(sPostbackData)
		Me.Controls.Add(hfSelectedRows)
		Me.Controls.Add(hfDeletedRows)
		Me.Controls.Add(hfPageCommand)
		Me.Controls.Add(script)
		Me.Controls.Add(btnRowSelect)
        Me.Controls.Add(btnSort)
        Me.Controls.Add(btnExcel)
        Me.CssClass = "DTIGrid"
	End Sub

	Protected Overridable Sub changeInnerControlIds()
		If String.IsNullOrEmpty(Me.ID) Then
			Me.ID = Me.ClientID.Substring(Me.ClientID.LastIndexOf("_") + 1)
		End If
		tbl.ID = Me.ID & "_Table"
		postbackData.ID = Me.ID & "_Hidden"
		sPostbackData.ID = Me.ID & "_sHidden"
		hfSelectedRows.ID = Me.ID & "_SelectedRowsHidden"
		hfDeletedRows.ID = Me.ID & "_DeletedRowsHidden"
		hfPageCommand.ID = Me.ID & "_PageCommand"
		'script.ID = Me.ID & "_Script"
		btnRowSelect.ID = Me.ID & "_RowSelect"
		btnSort.ID = Me.ID & "_Sort"
		pnlSearch.ID = Me.ID & "_SearchDiv"
		ddlSearch.ID = Me.ID & "_SearchDDL"
		tbSearch.ID = Me.ID & "_SearchTB"
		cbSearch.ID = Me.ID & "_SearchCB"
		litSearchLabel.ID = Me.ID & "_SearchLabel"
		litSearchForLabel.ID = Me.ID & "_SearchForLabel"
		btnSearch.ID = Me.ID & "_Search"
		pnlPager.ID = Me.ID & "_PagerDiv"
		btnFirst.ID = Me.ID & "_First"
		btnPrev.ID = Me.ID & "_Prev"
		tbPageNum.ID = Me.ID & "_PageNum"
		lblSeperator.ID = Me.ID & "_Sep"
		lblPages.ID = Me.ID & "_Pages"
		btnNext.ID = Me.ID & "_Next"
		btnLast.ID = Me.ID & "_Last"
		ButtonPlaceHolder.ID = Me.ID & "_Placeholder"
		setupButtons()
	End Sub

	Protected Overridable Sub setupButtons()
		litSearchLabel.Text = "Search "
		litSearchForLabel.Text = " for "
		btnRowSelect.Style("display") = "none"
		btnSort.Style("display") = "none"
		btnSearch.Text = "Search"
		ddlSearch.Attributes.Add("onChange", "dtiSearchHandler(this);")
		btnSearch.OnClientClick = "dtiGetSearch(this);"
		tbPageNum.Text = 1
		lblPages.Text = PageCount
		lblSeperator.Text = " of "
		btnFirst.Text = "|<"
		btnPrev.Text = "<"
		btnNext.Text = ">"
		btnLast.Text = ">|"
		tbPageNum.Width = 25
		btnFirst.Enabled = False
		btnPrev.Enabled = False
		btnNext.Enabled = False
		btnLast.Enabled = False
	End Sub

	Private Sub DTIGrid_Databinding() Handles Me.Databinding
		If DesignMode Then Return
		If Page.IsPostBack AndAlso dt IsNot Nothing Then

			SelectedRows.Clear()
			UpdatedRows.Clear()

			If Not ReadonlyLastReq Then
				parseJavascriptGrid()
			End If

			Dim selrows As String() = {}
			If Not Page.Request.Form(hfSelectedRows.UniqueID) Is Nothing Then
				selrows = Page.Request.Form(hfSelectedRows.UniqueID).Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries)
			End If

			For Each selrow As String In selrows
				Dim rowid As Integer = Integer.Parse(selrow)
				If rowCount > PageSize AndAlso PageIndex > 1 Then
					rowid = rowid + ((PageIndex - 1) * PageSize)
				End If
				Dim row As DTIGridRow = Me.Rows.FindRowById(rowid)
				If row Is Nothing Then
					If Me.SavedGrid IsNot Nothing Then
						row = Me.SavedGrid.Rows.FindRowById(rowid)
					End If
				End If
				If row IsNot Nothing Then
					row.Selected = True
					SelectedRows.Add(row)
				End If
			Next
			RaiseEvent UpdatesComplete(Me, New EventArgs)
		End If
	End Sub

	Private Sub DTIGrid_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
		'jQueryLibrary.jQueryInclude.RegisterJQueryUIThemed(Page)
		jQueryLibrary.jQueryInclude.RegisterJQueryUI(Page)
		'jQueryLibrary.jQueryInclude.addScriptFile(Page, "DTIGrid/jquery.jqGrid-3.8.2.min.js", , True)
		'jQueryLibrary.jQueryInclude.addScriptFile(Page, "DTIGrid/jquery.jqGrid-4.1.2.min.js", , True)
		'jQueryLibrary.jQueryInclude.addScriptFile(Page, "DTIGrid/jquery.jqGrid-4.5.2.min.js", , True)
		jQueryLibrary.jQueryInclude.addScriptFile(Page, "DTIGrid/jquery.jqgrid.min.js", , True)
		'jQueryLibrary.jQueryInclude.addScriptFile(Page, "DTIGrid/jquery.jqGrid-4.4.3.min.js", , True)
		jQueryLibrary.jQueryInclude.addScriptFile(Page, "DTIGrid/DTIDataGrid.js", , True)
		jQueryLibrary.jQueryInclude.addScriptFile(Page, "DTIGrid/ui.jqgrid.css", "text/css", True)
		If ShowDateAndTime Then
			jQueryLibrary.jQueryInclude.addScriptFile(Page, "DTIGrid/jquery.timepicker.js", , True)
			jQueryLibrary.jQueryInclude.addScriptFile(Page, "DTIGrid/jqueryTimepicker.css", "text/css", True)
		End If
		changeInnerControlIds()
	End Sub

#Region "button clicks"
	Private Sub btnSort_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSort.Click
		Dim btnClientID As String = CType(sender, Button).ClientID
		If btnClientID.IndexOf(Me.ClientID) > -1 Then
			Dim sorStr As String = sPostbackData.Value 'Me.Page.Request.Form(sPostbackData.ClientID)
			SortColumn = sorStr.Split(New Char() {"|"c})(0)
			SortOrder = sorStr.Split(New Char() {"|"c})(1)
			Dim searchCol As String = sorStr.Split(New Char() {"|"c})(2)
			If searchCol.IndexOf(".") > -1 Then
				searchCol = searchCol.Split(New Char() {"."c})(1)
			End If
			RaiseEvent Sorting(SortColumn, SortOrder, searchCol, sorStr.Split(New Char() {"|"c})(3))
		End If
	End Sub

	Private Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
		Dim btnClientID As String = CType(sender, Button).ClientID
		If btnClientID.IndexOf(Me.ClientID) > -1 Then
			Dim searchCol As String = sPostbackData.Value.Split(New Char() {"|"c})(0)
			If searchCol.IndexOf(".") > -1 Then
				searchCol = searchCol.Split(New Char() {"."c})(1)
			End If
			RaiseEvent Searching(searchCol, sPostbackData.Value.Split(New Char() {"|"c})(1))
		End If
	End Sub

#Region "paging"
	Private Sub btnFirst_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFirst.Click
		Dim btnClientID As String = CType(sender, Button).ClientID
		If btnClientID.IndexOf(Me.ClientID) > -1 Then
			PageIndex = 1
		End If
		DataBind()
		RaiseEvent PageChanged(PageIndex)
	End Sub

	Private Sub btnPrev_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrev.Click
		Dim btnClientID As String = CType(sender, Button).ClientID
		If btnClientID.IndexOf(Me.ClientID) > -1 Then
			PageIndex -= 1
		End If
		DataBind()
		RaiseEvent PageChanged(PageIndex)
	End Sub

	Private Sub tbPageNum_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tbPageNum.TextChanged
		If DesignMode Then Return
		Dim btnClientID As String = CType(sender, TextBox).ClientID
		If btnClientID.IndexOf(Me.ClientID) > -1 Then
			Try
				Dim newPage As Integer = Integer.Parse(Page.Request.Params(CType(sender, TextBox).UniqueID))
				If newPage > lblPages.Text Then
					PageIndex = lblPages.Text
				ElseIf newPage < 1 Then
					PageIndex = 1
				Else
					PageIndex = newPage
				End If
			Catch ex As Exception
				PageIndex = 1
			End Try
		End If
		DataBind()
		RaiseEvent PageChanged(PageIndex)
	End Sub

	Private Sub btnNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNext.Click
		Dim btnClientID As String = CType(sender, Button).ClientID
		If btnClientID.IndexOf(Me.ClientID) > -1 Then
			PageIndex += 1
		End If
		DataBind()
		RaiseEvent PageChanged(PageIndex)
	End Sub

	Private Sub btnLast_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLast.Click
		Dim btnClientID As String = CType(sender, Button).ClientID
		If btnClientID.IndexOf(Me.ClientID) > -1 Then
			PageIndex = lblPages.Text
		End If
		DataBind()
		RaiseEvent PageChanged(PageIndex)
	End Sub
#End Region
#End Region

	Private Sub DTIGrid_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
		If DesignMode Then Return
		If Not _hasdatabound Then Me.DataBind()
		If Not EnableSearching Then
			pnlSearch.Visible = False
		End If
		If Not AutoPostBack Then
			btnRowSelect.Visible = False
		End If
		If Not EnableSorting Then
			btnSort.Visible = False
		End If
		If EnablePaging Then
			If Not PageCount > 0 Then
				tbPageNum.Visible = False
				lblPages.Visible = False
				lblSeperator.Visible = False
				btnFirst.Visible = False
				btnPrev.Visible = False
				btnNext.Visible = False
				btnLast.Visible = False
			Else
				tbPageNum.Visible = True
				lblPages.Visible = True
				lblSeperator.Visible = True
				btnFirst.Visible = True
				btnPrev.Visible = True
				btnNext.Visible = True
				btnLast.Visible = True
			End If
		Else
			pnlPager.Visible = False
		End If

		AddButtonJquery(btnSearch, btnPrev, btnNext, btnFirst, btnLast)

		If EnableSearching Then
			Dim allCols As String = ""

			For Each col As DTIGridColumn In Me.Columns
				If col.Visible Then  'AndAlso col.DataType.ToString().Substring(7).StartsWith("String")  Searches only string cols
					allCols &= col.Name & ","
				End If
			Next
			allCols = allCols.Trim(",")
			ddlSearch.Items.Add(New ListItem("", allCols))
			For Each col As DTIGridColumn In Me.Columns
				With col
					If col.Visible Then
						ddlSearch.Items.Add(New ListItem(.ColumnHeader, .DataType.ToString.Substring(7) & "." & .Name))
					End If
				End With
				If Me.Page.Request.Params(ddlSearch.UniqueID) IsNot Nothing OrElse Me.Page.Request.Params(ddlSearch.UniqueID) <> "" Then
					Try
						ddlSearch.SelectedValue = Me.Page.Request.Params(ddlSearch.UniqueID)
					Catch ex As Exception
					End Try
				End If
			Next
		End If

		'This is only for paging a datatable
		If EnablePaging AndAlso isGridButtonClicked AndAlso savedDt IsNot Nothing Then
			Me.DataSource = pageDatatable(savedDt, PageSize, PageCount, PageIndex, (SortString).Trim)
			setupPageButtons()
			DataBind()
		End If
		errorDiv.Visible = errorDiv.isError
		If Not Me.renderAsTable Then
			setScript()
		End If
	End Sub

	Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
		If Not _hasdatabound Then Me.DataBind()

		'SavedGrid._columns = Me.Columns.Clone
		'SavedGrid.DataSource = Me.DataSource
		'SavedGrid._rows = Me.Rows.Clone(SavedGrid)
		Dim rowid As Integer = 1
		For Each rw As DTIGridRow In Me.Rows
			rw("dtigridstate") = "unchanged"
			rw("dtigridid") = rowid
			rowid += 1
		Next
		ReadonlyLastReq = Not EnableEditing
		'If dt IsNot Nothing OrElse Me.Rows.Count > 0 Then
		If Me.renderAsTable Then
			ButtonPlaceHolder.Visible = False
			Dim s As String = "<table class='gridtable'>" & vbCrLf
			s &= "<tr class='gridheader'>" & vbCrLf
			For Each col As DTIGridColumn In Me.Columns
				If col.Visible Then
					If col.Width > 0 Then
						s &= String.Format("<td width='{0}px'><b>{1}</b></td>", col.Width, col.ColumnHeader)
					Else
						s &= String.Format("<td><b>{0}</b></td>", col.ColumnHeader)
					End If
				End If
			Next
			s &= "</tr>"
			For Each row As DTIGridRow In Me.Rows
				s &= "<tr class='gridrow'>" & vbCrLf
				For Each col As DTIGridColumn In Me.Columns
					If col.Visible Then
						s &= String.Format("<td>{0}</td>", row(col.Name))
					End If
				Next
				s &= "</tr>" & vbCrLf
			Next
			s &= "</table>" & vbCrLf
			phgridarea.Controls.Add(New LiteralControl(s))
		Else
			'Page.ClientScript.RegisterOnSubmitStatement(Me.GetType, Me.ClientID & "SaveRowInformation", "    dtiGetGridData('" & Me.ClientID & "');")
			'setScript()
			'End If
		End If
		SavedGrid = Me
		writer.Write("<!--##" & Me.ClientID & "##-->")
		MyBase.Render(writer)
		writer.Write("<!--##" & Me.ClientID & "##-->")
	End Sub

	Private hasRaisedClickEvent As Boolean = False

	''' <summary>
	''' Binds a data source to the invoked server control and all its child controls.
	''' </summary>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Binds a data source to the invoked server control and all its child controls.")>
	Public Overrides Sub DataBind()
		If isGridButtonClicked AndAlso (dt Is Nothing OrElse rowCount = 0) AndAlso SavedGrid IsNot Nothing AndAlso SavedGrid.dt IsNot Nothing AndAlso SavedGrid.rowCount > 0 Then
			If isSearchClicked Then
				SavedGrid.dt.Clear()
			End If
			Me.DataSource = SavedGrid.dt
		End If
		If dt Is Nothing AndAlso SavedGrid IsNot Nothing AndAlso SavedGrid.dt IsNot Nothing Then
			Me.DataSource = SavedGrid.dt
		End If
		'If isGridClick AndAlso hfSelectedRows.Value <> "" Then
		'    RaiseEvent Click(SavedGrid.Rows(hfSelectedRows.Value - 1))
		'End If
		RaiseEvent Databinding()
		If EnablePaging Then
			If dt IsNot Nothing AndAlso rowCount - AddedRows.Count > PageSize Then
				Dim dtSaved As Boolean = False
				If Not dtSaved Then
					savedDt = dt
					dtSaved = True
				End If
				btnFirst.Enabled = False
				btnPrev.Enabled = False
				btnNext.Enabled = False
				btnLast.Enabled = False
				If Not isGridButtonClicked Then
					Me.DataSource = pageDatatable(savedDt, PageSize, PageCount, PageIndex, (SortString).Trim)
				End If
				setupPageButtons()
			End If

			If PageCount > 1 Then
				btnFirst.Enabled = False
				btnPrev.Enabled = False
				btnNext.Enabled = False
				btnLast.Enabled = False
				setupPageButtons()
			ElseIf PageCount = 1 Then
				lblPages.Text = 1
				tbPageNum.Text = 1
				PageIndex = 1
				btnFirst.Enabled = False
				btnPrev.Enabled = False
				btnNext.Enabled = False
				btnLast.Enabled = False
			End If
		Else
			pnlPager.Visible = False
		End If
		Me.Columns.Clear()
		Me.Rows.Clear()

		Dim i As Integer = 0
		Dim coltitles As String() = New String() {}
		If ColumnTitles IsNot Nothing Then coltitles = ColumnTitles.Split(",")
		Dim colWidths As String() = New String() {}
		If ColumnWidths IsNot Nothing Then colWidths = ColumnWidths.Split(",")
		If colWidths.Length > 0 And Not shinkToFixExtrnallyChanged Then
			If Not Width.IsEmpty AndAlso Width.ToString.EndsWith("%") Then
				_shrinkToFit = False
			End If
		End If

		If Not Me.DesignMode AndAlso dt IsNot Nothing Then
			Me.Columns.Add(New DTIGridColumn("dtigridid", "dtigridid", GetType(Integer), False, Nothing, EnableSorting, EnableEditing))
			'For Each relation As DataRelation In dt.ChildRelations

			'Next

			For Each column As DataColumn In dt.Columns
				With column
					'.ColumnName = BaseClasses.BaseSecurityPage.RemoveSpecialCharacters(.ColumnName)
					Dim visible As Boolean = True
					Dim width As UShort = 0
					Dim sortable As Boolean = EnableSorting
					Dim editable As Boolean = EnableEditing
					Dim align As DTIGridColumn.Alignment = DTIGridColumn.Alignment.left
					Dim header As String = .ColumnName
					If coltitles.Length > i Then
						If coltitles(i).Trim().Length > 0 Then header = coltitles(i).Trim
					End If
					If colWidths.Length > i Then
						Integer.TryParse(colWidths(i), width)
					End If

					Dim tmpcolumn As DTIGridColumn
					Try
						tmpcolumn = SavedGrid.Columns(.ColumnName)
					Catch ex As Exception

					End Try

					If tmpcolumn IsNot Nothing Then
						visible = tmpcolumn.Visible
						width = tmpcolumn.Width
						sortable = tmpcolumn.Sortable
						editable = tmpcolumn.Editable
						align = tmpcolumn.Align
						header = tmpcolumn.ColumnHeader
					End If

					If Array.IndexOf(dt.PrimaryKey, column) > -1 AndAlso column.AutoIncrement Then
						editable = False
					End If

					Dim col As New DTIGridColumn(.ColumnName, header, column.DataType, visible, width, sortable, editable, align)
					Me.Columns.Add(col)
					RaiseEvent ColumnCreated(col)
				End With
				i += 1
			Next
			Me.Columns.Add(New DTIGridColumn("dtigridstate", "dtigridstate", GetType(String), False, Nothing, False, False))

			Dim rowid As Integer = 1
			For Each row As DataRow In dt.Rows
				If Not row.RowState = DataRowState.Deleted Then
					Dim rw As New DTIGridRow(Me)
					rw.Add("dtigridid", rowid)
					For Each column As DataColumn In dt.Columns
						rw.Add(column.ColumnName, row.Item(column.ColumnName))
					Next
					rw.Add("dtigridstate", "unchanged")
					Me.Rows.Add(rw)
					RaiseEvent RowDataBound(rw)
					rowid += 1
				End If
			Next
		End If
		If EnableSearching Then
			ddlSearch.Items.Clear()
			'ddlSearch.Items.Add("")
			If ddlSearch.SelectedValue.IndexOf("Boolean") > -1 Then
				tbSearch.Style.Add("display", "none")
				cbSearch.Style.Remove("display")
			Else
				cbSearch.Style.Add("display", "none")
				tbSearch.Style.Remove("display")
			End If
		End If
		hfDeletedRows.Value = ""
		_hasdatabound = True
		If visibleColumns IsNot Nothing AndAlso Not visibleColumns.Trim = "" Then
			For Each gcol As DTIGridColumn In Me.Columns
				gcol.Visible = False
			Next
			i = 0
			For Each col As String In visibleColumns.Split(",")
				col = col.Trim.Trim(",").Trim
				If Not col = "" Then
					Dim gridcol As DTIGridColumn = Me.Columns(col)
					If Not gridcol Is Nothing Then
						Me.Columns.Remove(col)
						Me.Columns.Add(gridcol)
						Me.Columns(col).Visible = True
						If coltitles.Length > i Then
							If coltitles(i).Trim().Length > 0 Then Me.Columns(col).ColumnHeader = coltitles(i).Trim
						End If
						If colWidths.Length > i Then
							Dim w As Integer = 100
							If Integer.TryParse(colWidths(i), w) Then
								Me.Columns(col).Width = w
								Me.Columns(col).AutoWidth = False
							End If
						End If
					End If
				End If
				i += 1
			Next
		End If
		If hiddenColumns IsNot Nothing AndAlso Not hiddenColumns.Trim = "" Then
			For Each col As String In hiddenColumns.Split(",")
				col = col.Trim.Trim(",").Trim
				If Me.Columns(col) IsNot Nothing Then _
					Me.Columns(col).Visible = False
			Next
		End If
		If isGridClick AndAlso Page.Request.Params(hfSelectedRows.UniqueID) <> "" Then
			If Not hasRaisedClickEvent Then
				RaiseEvent Click(Me.Rows(Page.Request.Params(hfSelectedRows.UniqueID) - 1))
				hasRaisedClickEvent = True
			End If
		End If
		RaiseEvent DataBound()
	End Sub

	Private Function getScript() As String
		Dim ScriptText As String = ""
		ScriptText = ""
		ScriptText &= "window." & Me.ClientID & "_data = " & Me.Rows.ToString & vbCrLf
		ScriptText &= "window." & Me.ClientID & "_lastsel = null;"
		ScriptText &= "window." & Me.ID & " = $('#" & Me.tbl.ClientID & "').jqGrid({"
		ScriptText &= "datatype: 'local'," & vbCrLf
		ScriptText &= "data: " & Me.ClientID & "_data," & vbCrLf
		ScriptText &= "rowNum: " & rowCount & "," & vbCrLf
		If Height.Value > 0 AndAlso Not (Height.Value = 100.0 AndAlso Height.Type = UnitType.Percentage) Then
			ScriptText &= "height: " & Height.Value & ","
		Else
			ScriptText &= "height: 'auto',"
		End If
		If Width.Value > 0 Then
			ScriptText &= "width: " & Width.Value & ","
		End If
		If EnableAltRows Then
			ScriptText &= "altRows: true,"
			If AltRowsCssClass <> "" Then
				ScriptText &= "altclass: '" & AltRowsCssClass & "',"
			End If
		End If

		ScriptText &= "autoencode: " & autoencode.ToString().ToLower() & ","

		'.ScriptText &= "url:'local'," & vbCrLf
		ScriptText &= "editurl:''," & vbCrLf
		If Not ShrinkToFit Then
			ScriptText &= "shrinkToFit:false," & vbCrLf
		End If
		'If dt IsNot Nothing Then
		ScriptText &= "colNames:" & Me.Columns.ColumnHeadersString
		ScriptText &= "colModel:" & Me.Columns.ToString
		'End If
		'.ScriptText &= "gridComplete: loadCompleteFunction('" & Me.tbl.ClientID & "'),"
		'.ScriptText &= "emptyDataText:  'There are no records. If you would like to add one, click the ""Add New ..."" button below.',"
		If EnableEditing Then
			'.ScriptText &= "'cellEdit': true,'cellsubmit' : 'clientArray'," & vbCrLf
			ScriptText &= "editurl: 'clientArray'," & vbCrLf
			'cellEdit': true,
			'cellsubmit' : 'clientArray',
		End If
		ScriptText &= "onSelectRow: function(id){" & vbCrLf
		If EnableEditing Then
			ScriptText &= " DataGrid.dtiSelectRow('" & Me.ClientID & "',id,true);" & vbCrLf
		End If

		If AutoPostBack Then
			ScriptText &= "    DataGrid.dtiGetGridData('" & Me.ClientID & "');" & vbCrLf
		End If
		'.ScriptText &= "    if(id && id!==" & Me.ClientID & "_lastsel){ " & vbCrLf
		'If EnableEditing Then
		'    .ScriptText &= "        $('#" & Me.tbl.ClientID & "').jqGrid('editRow',id, true, " & Me.ClientID & "_datesHandle, false,'clientArray',false,function(){dtiSaveGrid('" & Me.ClientID & "');});" & vbCrLf
		'End If
		If Not String.IsNullOrEmpty(OnClientRowSelect) Then
			ScriptText &= OnClientRowSelect.Trim(";") & ";" & vbCrLf
		End If
		'.ScriptText &= "        " & Me.ClientID & "_lastsel=id;" & vbCrLf
		'.ScriptText &= "    }" & vbCrLf
		If AutoPostBack Then
			ScriptText &= "    $('#" & Me.btnRowSelect.ClientID & "').click();" & vbCrLf
		End If
		ScriptText &= "},"
		If EnableSorting Then
			ScriptText &= "onSortCol: DataGrid.dtiSortHandler," '{"
		End If
		If MultiSelect Then _
			ScriptText &= "multiselect: true,"
		If Not String.IsNullOrEmpty(SortColumn) Then _
			ScriptText &= "sortname: """ & SortColumn & ""","
		ScriptText &= "sortorder: """ & SortOrder & ""","
		If Title Is Nothing OrElse Title.ToLower = "table1" Then
			If dt IsNot Nothing Then
				ScriptText &= "caption: """ & dt.TableName & """"
			End If
		Else
			ScriptText &= "caption: """ & Title & """"
		End If
		ScriptText &= "});" & vbCrLf

		If Me.rowCount > Me.PageSize Then
			ScriptText &= "window." & Me.ClientID & "_startrow = " & Me.PageSize * (PageIndex - 1) & ";" & vbCrLf
			If Me.PageIndex = Me.PageCount Then
				ScriptText &= "window." & Me.ClientID & "_endrow = " & Me.ClientID & "_data.length;" & vbCrLf
			Else
				ScriptText &= "window." & Me.ClientID & "_endrow = " & (Me.PageSize * PageIndex) - 1 & ";" & vbCrLf
			End If
		Else
			ScriptText &= "window." & Me.ClientID & "_startrow = 0;" & vbCrLf
			ScriptText &= "window." & Me.ClientID & "_endrow = " & Me.ClientID & "_data.length;" & vbCrLf
		End If

		'ScriptText &= "for(var i=" & Me.ClientID & "_startrow;i<=" & Me.ClientID & "_endrow;i++){"
		'ScriptText &= "$('#" & Me.tbl.ClientID & "').jqGrid('addRowData',i+1," & Me.ClientID & "_data[i]);} "
		ScriptText &= "window." & Me.ClientID & "_datesHandle = function(id){ "
		For Each column As DTIGridColumn In Me.Columns
			If column.DataType Is GetType(DateTime) Then
				If Not ShowDateAndTime Then
					ScriptText &= "$('#'+id+'_" & column.Name & "','#" & Me.tbl.ClientID & "').datepicker({changeMonth: true,changeYear: true});"
				Else
					ScriptText &= "$('#'+id+'_" & column.Name & "','#" & Me.tbl.ClientID & "').datetimepicker({changeMonth: true,changeYear: true, ampm: true});"
				End If
			End If
		Next
		ScriptText &= "}" & vbCrLf
		'handle extra height added by search bar and/or pager
		'Dim pagebuttons As Boolean = ButtonPlaceHolder.Controls.Count > 0 OrElse PageSize > 0
		'If EnableSearching AndAlso Not pagebuttons Then
		'    ScriptText &= "try{$('#" & Me.ClientID & "').height($('#" & Me.ClientID & "').height() + 80);}catch(err){}" & vbCrLf
		'End If
		'If pagebuttons AndAlso Not EnableSearching Then
		'    ScriptText &= "try{$('#" & Me.ClientID & "').height($('#" & Me.ClientID & "').height() + 80);}catch(err){}"
		'End If
		'If pagebuttons AndAlso EnableSearching Then
		'    ScriptText &= "try{$('#" & Me.ClientID & "').height($('#" & Me.ClientID & "').height() + 100);}catch(err){}"
		'End If
		ScriptText &= "try{CorrectHeight('" & Me.ClientID & "');}catch(err){}"
		ScriptText &= vbCrLf & "$(document).ready(function() {"
		ScriptText &= "try{CorrectColWidth('" & Me.tbl.ClientID & "');}catch(err){}"
		If ajaxEnable Then
			ScriptText &= "dtiMakeGridAjax('" & Me.ClientID & "');"
		End If
		If Me.Width.Type = UnitType.Percentage Then
			ScriptText &= vbCrLf &
				" $(window).bind('resize', function() { " & vbCrLf &
				"    try{$('#" & Me.ClientID & "_Table').setGridWidth(50," & Me.ShrinkToFit.ToString.ToLower & "); }catch(e){}" & vbCrLf &
				"    try{$('#" & Me.ClientID & "_Table').setGridWidth($('#" & Me.ClientID & "').width()," & Me.ShrinkToFit.ToString.ToLower & "); }catch(e){}" & vbCrLf &
				"}).trigger('resize'); " & vbCrLf
			ScriptText &= vbCrLf &
				"    try{$('#" & Me.ClientID & "_Table').setGridWidth(50," & Me.ShrinkToFit.ToString.ToLower & "); }catch(e){}" & vbCrLf &
				"    try{$('#" & Me.ClientID & "_Table').setGridWidth($('#" & Me.ClientID & "').width()," & Me.ShrinkToFit.ToString.ToLower & "); }catch(e){}"
		End If
		ScriptText &= " });"
		Return ScriptText
	End Function
	Private Sub setScript()
		If Not Me.DesignMode Then
			script.Text = jQueryLibrary.jQueryInclude.isolateJquery(getScript(), True) '"<script type=""text/javascript"" language=""javascript"">(function($) {" & getScript() & "})($$);</script>"
			'jQueryLibrary.jQueryInclude.addScriptBlock(Me.Page, getScript(), id:=Me.ID)
		End If

	End Sub

	''' <summary>
	''' Returns a string that represents the current object.
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Returns a string that represents the current object.")>
	Public Overrides Function ToString() As String
		setScript()
		Return "DTIDataGrid: " & Me.ClientID & vbCrLf &
		"Columns Headers: " & Me.Columns.ColumnHeadersString & vbCrLf &
		"Columns: " & Me.Columns.ToString & vbCrLf &
		"Rows: " & Me.Rows.ToString & vbCrLf &
		 "" & vbCrLf &
		"Script: " & vbCrLf &
		Me.getScript()
	End Function

	''' <summary>
	''' Takes a Datatable and returns selection of the given table based on the number of rows to display
	''' per page.
	''' </summary>
	''' <param name="dt">Datatable to be paged</param>
	''' <param name="PageSize">Number of Rows per page</param>
	''' <param name="PageCount">Variable to store total number of pages</param>
	''' <param name="PageIndex">Index of selected page</param>
	''' <returns>a datatable with only a currentpages worth of data</returns>
	''' <remarks></remarks>
	<System.ComponentModel.Description("Takes a Datatable and returns selection of the given table based on the number of rows to display   per page.")>
	Public Shared Function pageDatatable(ByRef dt As DataTable, ByVal PageSize As Integer, ByRef PageCount As Integer, ByRef PageIndex As Integer, Optional ByVal Sort As String = Nothing) As DataTable
		Dim newdt As DataTable = dt.Clone
		Dim rowcount = 0
		If dt IsNot Nothing Then rowcount = dt.Rows.Count
		If PageSize > 0 And rowCount > 0 Then
			Dim tb As DataTable
			If Sort = Nothing Then
				tb = dt
			Else
				tb = New DataView(dt, "", Sort, DataViewRowState.CurrentRows).ToTable
			End If
			PageCount = Integer.Parse(Math.Ceiling(rowCount / PageSize))

			If PageIndex > PageCount Then PageIndex = PageCount

            Dim start As Integer = 0
            If PageIndex > 0 Then
                start = PageIndex - 1
            End If
            Dim startRow As Integer = (start) * PageSize
            For i As Integer = 0 To PageSize - 1
                Try
                    newdt.ImportRow(tb.Rows(startRow + i))
                Catch ex As Exception
                    Exit For
                End Try
            Next
            Return newdt
        Else
            Return dt
        End If
    End Function

    Protected Sub setupPageButtons()
        lblPages.Text = PageCount
        tbPageNum.Text = PageIndex
        If PageIndex > 1 Then
            btnFirst.Enabled = True
            btnPrev.Enabled = True
        End If
        If PageIndex < lblPages.Text Then
            btnNext.Enabled = True
            btnLast.Enabled = True
        End If
    End Sub

    Private Function getCellValues(ByVal rowString As String) As Hashtable
        Dim ht As New Hashtable

        For Each cell As String In rowString.Split(New String() {";;"}, StringSplitOptions.RemoveEmptyEntries)
            Dim columnName As String = cell.Split(New String() {"::"}, StringSplitOptions.None)(0)
            Dim columnVal As String = cell.Split(New String() {"::"}, StringSplitOptions.None)(1)
            ht.Add(columnName, columnVal)
        Next
        Return ht
    End Function

    Private hasparsed As Boolean = False
    Private Sub parseJavascriptGrid()
        If Not hasparsed AndAlso Not isGridButtonClicked Then
            Try
                Dim data As String = HttpUtility.HtmlDecode(DataString.Substring(1, DataString.Length - 2))
                postbackData.Value = ""
                Dim rows As String() = data.Split(New String() {"},{"}, StringSplitOptions.RemoveEmptyEntries)
                For Each row As String In rows
                    If row.StartsWith("{") Then row = row.Substring(1)
                    Dim rowVals As Hashtable = getCellValues(row)
                    'Dim cells As String() = row.Split(New String() {";;"}, StringSplitOptions.RemoveEmptyEntries)
                    Dim gridState As String = rowVals("dtigridstate") 'cells(cells.Length - 1).Split(New String() {"::"}, StringSplitOptions.None)(1)
                    Dim gridId As String = rowVals("dtigridid") 'cells(0).Split(New String() {"::"}, StringSplitOptions.None)(1)

                    Select Case gridState.ToLower
                        Case "updated"
                            Dim gRow As DTIGridRow = SavedGrid.Rows.FindRowById(gridId)
                            Dim isReallyEdited As Boolean = False
                            For Each columnName As String In rowVals.Keys
                                If setGridRow(gRow, columnName, rowVals(columnName)) Then
                                    isReallyEdited = True
                                End If
                            Next
                            Try
                                If isReallyEdited Then
                                    gRow.SetRowState(DTIGridRow.DTIRowState.Updated)
                                    'gRow.Remove("dtigridstate")
                                    'gRow.Remove("dtigridid")
                                    UpdatedRows.Add(gRow)
                                End If
                            Catch ex As Exception
                                dataError(ex)
                            End Try
                        Case "added"
                            Try
                                'Dim tmpRow As New DTIGridRow(SavedGrid)
                                Dim tmpRow As DTIGridRow
                                'If SavedGrid.Rows.Count = 0 Then
                                    tmpRow = New DTIGridRow(SavedGrid)
                                For Each columnName As String In rowVals.Keys
                                    tmpRow.Add(columnName, rowVals(columnName))
                                    setGridRow(tmpRow, columnName, rowVals(columnName))
                                Next
                                'Else
                                '    tmpRow = New DTIGridRow(SavedGrid)
                                '    For Each columnName As String In SavedGrid.Rows(0).ColumnNames
                                '        tmpRow.Add(columnName, SavedGrid.Rows(0)(columnName))
                                '    Next
                                '    For Each columnName As String In rowVals.Keys
                                '        If setGridRow(tmpRow, columnName, rowVals(columnName)) Then
                                '            tmpRow.Remove(columnName)
                                '            tmpRow.Add(columnName, rowVals(columnName))
                                '        End If
                                '    Next
                                'End If
                                tmpRow("dtigridid") = SavedGrid.Rows.Count + 1
                                'tmpRow.Remove("dtigridstate")
                                'tmpRow.Remove("dtigridid")
                                tmpRow.SetRowState(DTIGridRow.DTIRowState.Added)
                                SavedGrid.Rows.Add(tmpRow)
                                AddedRows.Add(tmpRow)
                            Catch ex As Exception
                                dataError(ex)
                            End Try
                        Case "deleted"
                            Try
                                Dim gRow As DTIGridRow = SavedGrid.Rows.FindRowById(gridId)
                                gRow("dtigridstate") = "deleted"
                                gRow.SetRowState(DTIGridRow.DTIRowState.Deleted)
                                'gRow.Remove("dtigridstate")
                                'gRow.Remove("dtigridid")
                                DeletedRows.Add(gRow)
                            Catch ex As Exception
                                dataError(ex)
                            End Try
                    End Select
                Next
            Catch ex As Exception
            End Try

            For Each row As DTIGridRow In UpdatedRows
                RaiseEvent RowUpdated(row)
            Next
            For Each row As DTIGridRow In AddedRows
                RaiseEvent RowAdded(row)
            Next
            For Each row As DTIGridRow In DeletedRows
                RaiseEvent RowDeleted(row)
            Next
        End If
    End Sub

    Private Function setGridRow(ByVal gRow As DTIGridRow, ByVal columnName As String, ByVal columnVal As String) As Boolean
        Try
            If columnName <> "dtigridstate" AndAlso columnName <> "dtigridid" Then
                Dim gCol As DTIGridColumn = SavedGrid.Columns(columnName)

                If gRow(columnName) IsNot Nothing AndAlso gCol IsNot Nothing Then
                    If gCol.Editable Then

                        Dim edittype As Type = gRow(columnName).GetType
                        If gCol.EditType = DTIGridColumn.EditTypes.select Then
                            columnVal = gCol.SelectStringToValue(columnVal)
                            edittype = columnVal.GetType
                        End If
                        If edittype Is GetType(DateTime) Then
                            If gRow(columnName) IsNot DBNull.Value AndAlso columnVal = "" Then
                                gRow(columnName) = DBNull.Value
                                Return True
                            ElseIf gRow(columnName) Is DBNull.Value OrElse gRow(columnName) <> columnVal Then
                                gRow(columnName) = columnVal
                                Return True
                            End If
                        ElseIf edittype Is GetType(Integer) OrElse edittype Is GetType(Decimal) Then
                            If gRow(columnName) IsNot DBNull.Value AndAlso columnVal = "" Then
                                gRow(columnName) = DBNull.Value
                                Return True
                            ElseIf gRow(columnName) Is DBNull.Value OrElse gRow(columnName) <> columnVal Then
                                gRow(columnName) = columnVal
                                Return True
                            End If
                        Else
                            If gRow(columnName) IsNot DBNull.Value AndAlso columnVal.ToLower = "null" Then
                                gRow(columnName) = DBNull.Value
                                Return True
                            ElseIf gRow(columnName) Is DBNull.Value OrElse gRow(columnName) <> columnVal Then
                                gRow(columnName) = columnVal
                                Return True
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            dataError(ex, columnName)
        End Try
        Return False
    End Function

    Public Property errorDiv As JqueryUIControls.InfoDiv = New JqueryUIControls.InfoDiv

    Protected Sub dataError(ByVal ex As Exception, Optional ByVal columnName As String = Nothing)

        If columnName IsNot Nothing Then
            If visibleColumns.Contains(columnName) Then
                errorDiv.isError = True
                errorDiv.Controls.Add(New LiteralControl("Column:" & columnName & "  " & ex.Message & "<br>"))
                RaiseEvent gridBindingError(ex)
            End If
        Else
            errorDiv.isError = True
            errorDiv.Controls.Add(New LiteralControl(ex.Message & "<br>"))
            RaiseEvent gridBindingError(ex)
        End If

    End Sub

    Protected Sub AddButtonJquery(ByVal ParamArray Buttons() As Control)
        Dim s As String = "$(function(){"
        For Each btn As Control In Buttons
            If btn.Visible Then
                s &= "$('#" & btn.ClientID & "').button();"
            End If
        Next
        s &= "});"
        jQueryLibrary.jQueryInclude.addScriptBlock(Page, s)
    End Sub

    Private Sub btnRowSelect_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRowSelect.Click
        If SelectedRows.Count > 0 Then
            If Not hasRaisedClickEvent Then
                RaiseEvent Click(SelectedRows(0))
                hasRaisedClickEvent = True
            End If
        End If
    End Sub

    Public Sub cancelDataEvents()
        postbackData.Value = ""
        sPostbackData.Value = ""
        cancelParse = True
    End Sub

    Private Sub btnExcel_Click(sender As Object, e As ImageClickEventArgs) Handles btnExcel.Click
        ExporttoExcel()

    End Sub
    Public Overridable Sub ExporttoExcel(Optional colnames As String() = Nothing, Optional filename As String = Nothing)
        BaseClasses.BaseSecurityPage.excelExport(dt, colnames, True, filename)
    End Sub
End Class
