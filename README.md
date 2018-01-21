# DTIGrid
.NET Data grid for fast editing. Add the grid give it a table name and add/update/delete.  Includes searching and paging.  Uses free-jqgrid.

Usage: 
(Editing a table called "Locations". The project connection string is used if it is named "ConnectionString")

    <%@ Register Assembly="DTIGrid" Namespace="DTIGrid" TagPrefix="DTIGrid" %>
    <DTIGrid:DTIDataGrid ID="DTIDataGrid1" runat="server" DataTableName="Locations" EnableEditing="True"
    Width="100%" EnablePaging="True" EnableSorting="True" EnableSearching="True" />
