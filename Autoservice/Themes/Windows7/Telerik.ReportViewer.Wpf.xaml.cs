using System.Linq;
using System.Windows.Controls;
using Telerik.ReportViewer.Wpf;

namespace JobScheduler.Themes.Windows7
{
    /// <summary>
    /// Interaction logic for Telerik.xaml
    /// </summary>
    public partial class Telerik
    {
        public Telerik()
        {
            InitializeComponent();
        }

        private void CbPartners_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null)
                return;

            var dataContext = comboBox.DataContext as MultivalueAvailableValuesParameterModel;

            comboBox.ItemsSource = string.IsNullOrWhiteSpace(comboBox.Text)
                ? dataContext.AvailableValues
                : dataContext.AvailableValues.Where(
                    p => p.Name.ToUpper().Contains(comboBox.Text.ToUpper()));
        }
    }
}
