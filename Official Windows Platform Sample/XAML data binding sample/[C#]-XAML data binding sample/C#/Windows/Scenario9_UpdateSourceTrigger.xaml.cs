using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace DataBinding
{
    public sealed partial class Scenario9
    {

        Employee _employee;

        public Scenario9()
        {
            this.InitializeComponent();
            _employee = new Employee();
            _employee.PropertyChanged += employeeChanged;
            Output.DataContext = _employee;
            ScenarioReset(null, null);
        }

        private void ScenarioReset(object sender, RoutedEventArgs e)
        {
            _employee.Name = "Jane Doe";
            _employee.Organization = "Contoso";
            BoundDataModelStatus.Text = "";
        }

        private void employeeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Name"))
            {
                BoundDataModelStatus.Text = "The property '" + e.PropertyName + "' was changed." + "\n\nNew value: " + _employee.Name;
            }
        }

        private void UpdateDataBtnClick(object sender, RoutedEventArgs e)
        {
            var expression = NameTxtBox.GetBindingExpression(TextBox.TextProperty);
            expression.UpdateSource();
        }
    }
}
