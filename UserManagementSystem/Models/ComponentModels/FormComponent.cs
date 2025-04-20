using System.Collections.Generic;

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

    public abstract class FormField
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

    public class FormButton
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Class { get; set; }
    }
}
