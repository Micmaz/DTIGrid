var DataGrid = (function ($) {
var dataLength = 0;
	var dtiGridDelRow = function(table){
    var tblId='#'+table+'_Table';
    var hfId ='#'+table+'_DeletedRowsHidden';
	var lastSel = eval("window." + table+"_lastsel");
    $(tblId).jqGrid("saveRow", lastSel,false,'clientArray');
    if ($(tblId).getGridParam('multiselect')) {
        gridConfrim(tblId, hfId, lastSel, true);
    } else {
        gridConfrim(tblId, hfId, lastSel, false);
    }
}

	var deleteRow = function(tblId, hfId, lastSel) {
    var rowid = $(tblId).getGridParam('selrow')
    if(!rowid)
        rowid=lastSel;
    if ($(tblId).jqGrid('getCell', lastSel, 0).toString() != '') {
        $(tblId).jqGrid("setRowData",rowid,{dtigridstate:"deleted"});
    }            
    var delarr = $(tblId).getRowData(rowid);            
    if(delarr){
        var delstr = ',{';
        $.each(delarr,function(j,val1){
            delstr += j + "::" + htmlEncode(val1) + ";;";
        });
        if(delstr.length>0){
            delstr=delstr.substring(0,delstr.length-2);        
        }
        delstr += '}';
        $(hfId).val($(hfId).val() + delstr);  
        $(tblId).delRowData(rowid);
    }
}

	var deleteRows = function(tblId, hfId, lastSel) {
		var arr = $(tblId).getGridParam('selarrrow');
		var arrLen = arr.length;
		while (arrLen > 0) {
			if ($(tblId).jqGrid('getCell', lastSel, 0).toString() != '') {
				$(tblId).jqGrid("setRowData", arr[0], { dtigridstate: "deleted" });
			}
			var delarr = $(tblId).getRowData(arr[0]);
			var delstr = ',{';
			$.each(delarr, function (j, val1) {
				delstr += j + "::" + htmlEncode(val1) + ";;";
			});
			if (delstr.length > 0) {
				delstr = delstr.substring(0, delstr.length - 2);
			}
			delstr += '}';
			$(hfId).val($(hfId).val() + delstr);
			$(tblId).delRowData(arr[0]);
			arr = $(tblId).getGridParam('selarrrow');
		}
	}

	var gridConfrim = function(tblId, hfId, lastSel, isMulti) {
		var m = 'Are you sure you want to delete the selected row?';
		var t = 'Delete Row?';
		if (isMulti) {
			m = 'Are you sure you want to delete the selected rows?';
			t = 'Delete Rows?';
		}
		$('<div id="gridConfrim-dialog"><p>' + m + '</p></div>').dialog({
			title: t,
			modal: true,
			resizable: false,
			close: function (event, ui) { $('body').find('#gridConfrim-dialog').remove(); },
			buttons: {
				Ok: function () {
					if (isMulti)
						deleteRows(tblId, hfId, lastSel);
					else
						deleteRow(tblId, hfId, lastSel);
					$(this).dialog("close");
				},
				Cancel: function () {
					$(this).dialog("close");
				}
			}
		});
	}

	var dtiMakeGridAjax = function(gridid){
		//$("#"+gridid).find("input[type]='submit'").each(function() {
		$("#" + gridid).find(":submit").each(function () {
			if($(this).val()!= "Add Row" && $(this).val()!= "Delete Row")
				ajaxSubmitButtonSearchString("#"+gridid, $(this), "<!--##"+gridid+"##-->");
		});
	}
/*
function dtiSaveRow(table, rowid) {
    var tblId = '#' + table + '_Table';
    var lastSel = eval(table + "_lastsel");
    //dtiSaveRow1(table, rowid);
    if(lastSel) dtiSaveRow1(table, lastSel);
}

function dtiSaveRow1(table, rowid) {
    var tblId = '#' + table + '_Table';
    $(tblId).jqGrid("resetSelection");
    if ($(tblId).jqGrid('getCell', rowid, 0).toString() != '') {
        $(tblId).jqGrid("setRowData", rowid, { dtigridstate: "Updated" });
    }
    //$(tblId).jqGrid("saveRow", lastSel, false, 'clientArray');
    $(tblId).jqGrid("saveRow", rowid, function () { dtiGetGridData(table) });

}
*/
	var setGridCmd = function(table, value) {
		$('#' + table + '_PageCommand').val(value);
	}

	var dtiSelectRow = function(table, id, editEnabled) {
		var tblId = '#' + table + '_Table';
		var lastSel = eval("window." + table + "_lastsel");
		if (id && id !== lastSel) {
			eval("window." + table + "_lastsel='" + id +"'");
			if (lastSel) {
				$(tblId).saveRow(lastSel);

				if ($(tblId).jqGrid('getCell', lastSel, 0).toString() == '') {
					$(tblId).jqGrid("setRowData", lastSel, { dtigridstate: "added" })
				} else {
					$(tblId).jqGrid("setRowData", lastSel, { dtigridstate: "Updated" });
				}
				dtiGetGridData(table);
			}
		}
		$(tblId).editRow(id, true, window[table + "_datesHandle"]);
	}

	var dtiSaveGrid = function(table) {
		var tblId = '#' + table + '_Table';
		var lastSel = eval(table + "_lastsel");
		if (lastSel) {
			$(tblId).saveRow(lastSel);
			$(tblId).jqGrid("resetSelection");
			if (lastSel) {
				$(tblId).saveRow(lastSel);
				if ($(tblId).jqGrid('getCell', lastSel, 0).toString() == '') {
					$(tblId).jqGrid("setRowData", lastSel, { dtigridstate: "added" })
				} else {
					$(tblId).jqGrid("setRowData", lastSel, { dtigridstate: "Updated" });
				}
			}
			dtiGetGridData(table);
		}
	}

	var dtiGetGridData = function(table) {
		var tblId = '#' + table + '_Table';
    
		var dataStr = "";
		var rowStr = "";
		var cellStr = "";
		var rows = $(tblId).getRowData();
		var selectedRow = $(tblId).getGridParam('selrow');
		$(tblId).jqGrid("resetSelection");
		$.each(rows, function (j, val) {
			rowStr += "{"; cellStr = "";
			$.each(rows[j], function (k, val1) {
				cellStr += k + "::" + val1 + ";;";
			});
			if (cellStr.length > 0) {
				cellStr = cellStr.substring(0, cellStr.length - 2);
			}
			rowStr += cellStr + "},";
		});
		if (rowStr.length > 0) {
			dataStr = rowStr.substring(0, rowStr.length - 1);
		}
		dataStr += $('#' + table + '_DeletedRowsHidden').val();
		$('#' + table + '_Hidden').val(htmlEncode(dataStr));

		//populates hidden field with selected values
		var selectedRows = '';

		// if (selectedRow.length > 1) {
			// var arr = selectedRow //selectedRow.split(",");
			// var arLen = arr.length;
			// for (var i = 0, len = arLen; i < len; ++i) {
				// var rowid = arr[i];
				// var arid = $(tblId).getRowData(rowid)['dtigridid'];
				// selectedRows += arid + ','
			// }
			// selectedRows = selectedRows.slice(0, -1);
		// } else {
			//var rowid = $(tblId).getGridParam('selrow');
			if(selectedRow)
				selectedRows = $(tblId).getRowData(selectedRow)['dtigridid'];
		// }
		if(selectedRow)
			$('#' + table + '_SelectedRowsHidden').val(selectedRows);
		return true;
	}

	var dtiGetSearch = function(searchbtn){
    try{
        var ctrlId = searchbtn.id.replace("_Search", "");
        var $ddlSearch = $('#' + ctrlId + '_SearchDDL');
        var $searchTB = $('#' + ctrlId + '_SearchTB');
        var $searchCB = $('#' + ctrlId + '_SearchCB');
        var dataStr = $ddlSearch.val() + '|';
        if($ddlSearch.val().indexOf('Boolean') > -1){
            dataStr += $searchCB.is(':checked');
        }
        else{
            dataStr += $searchTB.val();
        }    
        $('#'+ctrlId+'_sHidden').val(dataStr);
    }catch(err){
        dataStr = '|';
    }
    return dataStr;
}

	var dtiSearchHandler = function(dropdown){
    var ctrlId = dropdown.id.replace("_SearchDDL", "");
    var $ddlSearch = $(dropdown);
    var $searchTB = $('#' + ctrlId + '_SearchTB');
    var $searchCB = $('#' + ctrlId + '_SearchCB');
    $searchTB.val('');
    $searchTB.removeAttr('checked');
    if($ddlSearch.val().indexOf('Boolean') > -1){
        $searchTB.hide();
        $searchCB.parent().show();
    }
    else{
        $searchTB.show();
        $searchCB.parent().hide();
    }
}

	var dtiSortHandler = function(name, index, sortorder){
    var ctrlId = this.id.replace("_Table", "");
    var dataStr = dtiGetSearch($('#' + ctrlId + '_Search')[0]);
    dataStr = name + '|' + sortorder + '|' + dataStr;
    $('#'+ctrlId+'_sHidden').val(dataStr);
    if ($('#' + ctrlId + '_PagerDiv').length > 0) {
        if ($('#' + ctrlId + '_PagerDiv').text().replace(' of ', '') != 1){
            $('#'+ctrlId+'_Sort')[0].click();
        }
    } //else{return 'stop';}
}

	var saveEdit = function(id){
    dtiGetGridData(id);
}

	var htmlEncode = function(value){
  return $('<div/>').text(value).html();
}

	var htmlDecode =function(value){
  return $('<div/>').html(value).text();
}


	var dtiAddRow = function(id,data){
    var tblId='#' + id + '_Table';
    if (dataLength == 0){
        dataLength = eval(id + '_data').length + 1;
    }
    
    $(tblId).jqGrid('addRowData',dataLength,data);
    dataLength += 1;
    var objDiv = $('#' + id + '_Table').parent().parent();
    objDiv.animate({ scrollTop: objDiv.prop("scrollHeight") - objDiv.height() }, 300);
}

	var LoadComplete = function(id)
{
    if ($('#' + id + '_Table').getGridParam('records') == 0) // are there any records?
        DisplayEmptyText(id,true);
    else
        DisplayEmptyText(id,false);
}

	var DisplayEmptyText = function(id, display)
	{
		var grid = $('#' + id + '_Table');
		var emptyText = grid.getGridParam('emptyDataText'); // get the empty text
		var container = grid.parents('.ui-jqgrid-view'); // find the grid's container
		if (display) {
			container.find('.ui-jqgrid-hdiv, .ui-jqgrid-bdiv').hide(); // hide the column headers and the cells below
			container.find('.ui-jqgrid-titlebar').after('' + emptyText + ''); // insert the empty data text
		}
		else {
			container.find('.ui-jqgrid-hdiv, .ui-jqgrid-bdiv').show(); // show the column headers
			container.find('#EmptyData' + dataObject).remove(); // remove the empty data text
		}
	}

	var CorrectColWidth = function(gridID) {
		var cols = $('#' + gridID).jqGrid("getGridParam", "colModel");
		var rows= $('#' + gridID).jqGrid('getRowData');
		if(rows.length == 0) return false;
		$.each(rows,function (ridx,row) {
			$.each(cols, function (cidx, col) {
				if (!col.hidden && col.autowidth) {
					if (!col.charlen) col.charlen = 4;
					if (col.charlen < row[col.name].trim().length)
						col.charlen = row[col.name].trim().length;
					if (col.charlen > 20) col.charlen = 20;
				}
			});
		});
		var gw = $('#' + gridID).jqGrid('getGridParam', 'width');
		var charlen = 0;

		$.each(cols, function (i, col) {
			if (col.charlen)
				charlen += col.charlen;
			else if (!col.hidden) gw -= col.width;
		});

		$.each(cols, function (i, col) {
			if(col.autowidth && !col.hidden) {
				col.width = (gw / charlen) * col.charlen;
				col.widthOrg = (gw / charlen) * col.charlen;
			}
		});
		$('#' + gridID).jqGrid('setGridWidth', $('#' + gridID).jqGrid('getGridParam', 'width'));
	}

	var CorrectHeight = function(gridID) {
    var height = $('#'+gridID).height();
    gridID = "gview_"+gridID+"_Table";
    var newheight = height;
    newheight = newheight - $('#'+gridID).children('.ui-jqgrid-titlebar').height();
    newheight = newheight - $('#'+gridID).children('.ui-jqgrid-titlebar').height();
    
    if(height==newheight) newheight = height-54;
    //$('#'+gridID).children('.ui-jqgrid-bdiv').height(newheight);
    //$('#" & Me.ClientID & "').children('ui-jqgrid-bdiv').height($('#" & Me.ClientID & "').height() + 100);
}

	var JqgridIntOnly = function(){
    var s = $(this).val();
    var reg = /[^0-9]/g;
    if (s.match(reg)){
        $(this).val(s.replace(reg,''));
    }
}

	var JqgridDubOnly =function(){
    var reg = /[^0-9\.]/g;
    var s = $(this).val();
    var sa = s.replace(/[^0-9\.]/g,'').split('.');
    var salen = sa.length
    if (s.match(reg) || salen > 2){
        s = s.replace(/[^0-9\.]/g,'');        
        if (salen > 2){
            s = sa[0] + '.';
            var l = salen - 1, i = 1;
            for (i = 1; i <= l; i++){
                s += sa[i];
            }        
        }        
        $(this).val(s);
    }
}

	var JqgridIntElem = function(value, options){
	var el = document.createElement('input');
    el.type='text';
    el.value = value;
    $(el).keyup(JqgridIntOnly);
    $(el).change(JqgridIntOnly);
    return el;
}

	var JqgridDubElem = function(value, options){
    var el = document.createElement('input');
    el.type='text';
    el.value = value;
    $(el).keyup(JqgridDubOnly);
    $(el).change(JqgridDubOnly);
    return el;
}

	var jqGridIntValue = function(elem, operation, value) {
    if(operation === 'get') {
		return $(elem).val();    
	} else if(operation === 'set') {
		$(elem).val(value);
	}
}

	window.dataLength = dataLength;
	window.dtiGridDelRow = dtiGridDelRow;
	window.deleteRow = deleteRow;
	window.deleteRows = deleteRows;
	window.gridConfrim = gridConfrim;
	window.dtiMakeGridAjax = dtiMakeGridAjax;
	window.setGridCmd = setGridCmd;
	window.dtiSelectRow = dtiSelectRow;
	window.dtiSaveGrid = dtiSaveGrid;
	window.dtiGetSearch = dtiGetSearch;
	window.dtiSearchHandler = dtiSearchHandler;
	window.dtiSortHandler = dtiSortHandler;
	window.saveEdit = saveEdit;
	window.htmlEncode = htmlEncode;
	window.htmlDecode = htmlDecode;
	window.dtiAddRow = dtiAddRow;
	window.LoadComplete = LoadComplete;
	window.DisplayEmptyText = DisplayEmptyText;
	window.CorrectColWidth = CorrectColWidth;
	window.CorrectHeight = CorrectHeight;
	window.JqgridIntOnly = JqgridIntOnly;
	window.JqgridIntElem = JqgridIntElem;
	window.JqgridDubElem = JqgridDubElem;
	window.jqGridIntValue = jqGridIntValue;



return {
	dataLength: dataLength,
	dtiGridDelRow: dtiGridDelRow,
	deleteRow: deleteRow,
	deleteRows: deleteRows,
	gridConfrim: gridConfrim,
	dtiMakeGridAjax: dtiMakeGridAjax,
	setGridCmd: setGridCmd,
	dtiSelectRow: dtiSelectRow,
	dtiSaveGrid: dtiSaveGrid,
	dtiGetSearch: dtiGetSearch,
	dtiSearchHandler: dtiSearchHandler,
	dtiSortHandler: dtiSortHandler,
	saveEdit: saveEdit,
	htmlEncode: htmlEncode,
	htmlDecode: htmlDecode,
	dtiAddRow: dtiAddRow,
	LoadComplete: LoadComplete,
	DisplayEmptyText: DisplayEmptyText,
	CorrectColWidth: CorrectColWidth,
	CorrectHeight: CorrectHeight,
	JqgridIntOnly: JqgridIntOnly,
	JqgridIntElem: JqgridIntElem,
	JqgridDubElem: JqgridDubElem,
	jqGridIntValue: jqGridIntValue
}



})(jQuery);
