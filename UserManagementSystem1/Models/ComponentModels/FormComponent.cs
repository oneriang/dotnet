using System.Collections.Generic;
using UserManagementSystem.Models.ComponentModels;

namespace UserManagementSystem.Models.ComponentModels
{
    public class FormComponent : ComponentBase
    {
        public string SubmitUrl { get; set; }
        public string Method { get; set; } = "post";
        public string Layout { get; set; } = "vertical";
        public List<FormField> Fields { get; set; }
        public List<FormButton> Buttons { get; set; }
    }
    
/*
    public abstract class FormFieldBase
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
        public string HelpText { get; set; }
        public string Class { get; set; }
        public string DefaultValue { get; set; }
        public string Condition { get; set; }
        public bool Hidden { get; set; }
    }

    public class TextField : FormFieldBase
    {
        public string Placeholder { get; set; }
        public int? MaxLength { get; set; }
        public int? MinLength { get; set; }
        public string Pattern { get; set; }
    }

    public class SelectField : FormFieldBase
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

    public class FormButton
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Class { get; set; }
        public string OnClick { get; set; }
    }
*/
}
