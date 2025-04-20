// Models/ComponentModels/LayoutConfig.cs
using System.Collections.Generic;
using UserManagementSystem.Models.ComponentModels;

namespace UserManagementSystem.Models.ComponentModels
{
    public class LayoutConfig
    {
        public string Title { get; set; }
        public List<Section> Sections { get; set; }
    }


    public class Section : ComponentBase
    {
        // 从 TableComponent 或其他具体组件继承的属性
        public List<TableColumn> Columns { get; set; }  // 表格列定义
        public string DataSource { get; set; }         // 数据源URL
        public PaginationOptions Pagination { get; set; } // 分页配置
        
        // 表单相关属性
        public List<FormField> Fields { get; set; }    // 表单字段
        public List<FormButton> Buttons { get; set; }  // 表单按钮
        public string SubmitUrl { get; set; }          // 提交URL
        public string Method { get; set; } = "post";   // 提交方法
    }

/*
    // 基础组件类
    public abstract class ComponentBase
    {
        public string Type { get; set; }  // 组件类型(table/form等)
        public string Id { get; set; }    // 组件ID
        public string Title { get; set; } // 组件标题
    }
*/

}