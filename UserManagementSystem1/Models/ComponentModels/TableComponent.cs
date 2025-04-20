using System.Collections.Generic;

namespace UserManagementSystem.Models.ComponentModels
{
    public class TableComponent : ComponentBase
    {
        public List<TableColumn> Columns { get; set; }
        public string DataSource { get; set; }
        public PaginationOptions Pagination { get; set; }
    }

    public class TableColumn
    {
        public string Name { get; set; }
        public string Display { get; set; }
        public string Type { get; set; }
        public bool Sortable { get; set; }
        public List<TableAction> Actions { get; set; }
    }

    public class TableAction
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Class { get; set; }
        public string Icon { get; set; }
        public string OnClick { get; set; }
    }

    public class PaginationOptions
    {
        public bool Enabled { get; set; }
        public int PageSize { get; set; }
    }
}
