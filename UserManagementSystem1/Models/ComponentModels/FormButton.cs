// Models/ComponentModels/FormButton.cs
namespace UserManagementSystem.Models.ComponentModels
{
    public class FormButton
    {
        public string Type { get; set; }     // 按钮类型(submit/reset/button)
        public string Text { get; set; }     // 按钮文本
        public string Class { get; set; }    // CSS类名
        public string Icon { get; set; }     // 图标类名(如"fas fa-save")
        public string OnClick { get; set; }  // 点击事件处理函数名
    }
}