using System.Collections.Generic;

namespace UserManagementSystem.Models.ComponentModels
{
    public class TextField : FormField
    {
        public string Placeholder { get; set; }
        public int? MaxLength { get; set; }
        public int? MinLength { get; set; }
        public string Pattern { get; set; }
    }

    public class SelectField : FormField
    {
        public List<SelectOption> Options { get; set; }
        public bool Multiple { get; set; }
        public bool Searchable { get; set; }
    }

    public class SelectOption
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public bool Disabled { get; set; }
    }

    public class RadioField : FormField
    {
        public List<RadioOption> Options { get; set; }
        public bool Inline { get; set; }
    }

    public class RadioOption
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public class CheckboxField : FormField
    {
        public List<CheckboxOption> Options { get; set; }
        public bool Inline { get; set; }
    }

    public class CheckboxOption
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public bool Checked { get; set; }
    }

    public class SwitchField : FormField
    {
        public string OnText { get; set; }
        public string OffText { get; set; }
    }

    public class DateField : FormField
    {
        public string Format { get; set; }
        public string MinDate { get; set; }
        public string MaxDate { get; set; }
    }

    public class FileField : FormField
    {
        public string Accept { get; set; }
        public bool Multiple { get; set; }
        public string MaxSize { get; set; }
    }

    public class RichTextField : FormField
    {
        public List<string> Toolbar { get; set; }
        public string Height { get; set; }
    }

    public class HiddenField : FormField
    {
        // 仅包含基类属性
    }
}
