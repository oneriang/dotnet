// Models/ComponentModels/FormField.cs
using System.Collections.Generic;

namespace UserManagementSystem.Models.ComponentModels
{
    public abstract class FormField
    {
        public string Name { get; set; }          // 字段名称(对应模型属性名)
        public string Label { get; set; }         // 显示标签
        public string Type { get; set; }          // 字段类型(text/email/password/select等)
        public bool Required { get; set; }        // 是否必填
        public string HelpText { get; set; }      // 帮助文本
        public string Placeholder { get; set; }   // 占位文本
        public string Class { get; set; }         // CSS类名
        public object DefaultValue { get; set; }  // 默认值
        public string Condition { get; set; }     // 显示条件(如"!Id"表示当Id不存在时显示)
        public bool Hidden { get; set; }          // 是否隐藏
    }

    // 文本输入字段
    public class TextField : FormField
    {
        public int? MaxLength { get; set; }      // 最大长度
        public int? MinLength { get; set; }       // 最小长度
        public string Pattern { get; set; }       // 正则验证规则
    }

    // 数字输入字段
    public class NumberField : FormField
    {
        public double? Min { get; set; }          // 最小值
        public double? Max { get; set; }          // 最大值
        public double? Step { get; set; }         // 步长
    }

    // 下拉选择字段
    public class SelectField : FormField
    {
        public List<SelectOption> Options { get; set; } // 选项列表
        public bool Multiple { get; set; }              // 是否多选
        public bool Searchable { get; set; }            // 是否可搜索
    }

    // 下拉选项
    public class SelectOption
    {
        public string Value { get; set; }      // 选项值
        public string Text { get; set; }       // 显示文本
        public bool Disabled { get; set; }     // 是否禁用
    }

    // 单选按钮组
    public class RadioField : FormField
    {
        public List<RadioOption> Options { get; set; } // 单选选项
        public bool Inline { get; set; }              // 是否水平排列
    }

    // 单选选项
    public class RadioOption
    {
        public string Value { get; set; }      // 选项值
        public string Text { get; set; }       // 显示文本
    }

    // 复选框组
    public class CheckboxField : FormField
    {
        public List<CheckboxOption> Options { get; set; } // 复选选项
        public bool Inline { get; set; }                  // 是否水平排列
    }

    // 复选选项
    public class CheckboxOption
    {
        public string Value { get; set; }      // 选项值
        public string Text { get; set; }       // 显示文本
        public bool Checked { get; set; }      // 是否默认选中
    }

    // 开关字段
    public class SwitchField : FormField
    {
        public string OnText { get; set; }     // 开启状态文本
        public string OffText { get; set; }     // 关闭状态文本
    }

    // 文件上传字段
    public class FileField : FormField
    {
        public string Accept { get; set; }      // 接受的文件类型(如"image/*")
        public bool Multiple { get; set; }      // 是否多文件
        public string MaxSize { get; set; }     // 最大文件大小(如"2MB")
    }
}