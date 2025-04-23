namespace DataManagementApp.ViewModels
{
    public abstract class BaseViewModel<T> where T : class
    {
        public T Entity { get; set; }
        public string PageTitle { get; set; }
        public string FormAction { get; set; }
        public string FormController { get; set; }
    }
}
