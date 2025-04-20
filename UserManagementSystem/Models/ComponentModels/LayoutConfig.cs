using System.Collections.Generic;

namespace UserManagementSystem.Models.ComponentModels
{
    public class LayoutConfig
    {
        public string Title { get; set; }
        public List<Section> Sections { get; set; }
    }

    public class Section : ComponentBase
    {
        // 共用基类属性
    }
}
