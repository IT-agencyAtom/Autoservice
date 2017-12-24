using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Autoservice.Controls.Managers;
using ConstaSoft.Core.Controls.Managers;
using Telerik.Reporting;

namespace Autoservice.Screens.Managers
{
    /// <summary>
    ///     This class contains properties that a View can data bind to.
    ///     <para>
    ///         See http://www.galasoft.ch/mvvm
    ///     </para>
    /// </summary>
    public class ReportsManager : PanelViewModelBase
    {
        public ObservableCollection<ReportManager> Reports { get; set; }

        private ReportManager _selectedReport;

        public ReportManager SelectedReport
        {
            get { return _selectedReport; }
            set
            {
                if (_selectedReport == value)
                    return;
                _selectedReport = value;
                RaisePropertyChanged("SelectedReport");
            }
        }

        public override async void Refresh()
        {
            if (Reports == null)
                await Task.Run(() => LoadReports());

            SetIsBusy(false);
        }

        private void LoadReports()
        {
            Reports = new ObservableCollection<ReportManager>();
            Reports.Add(new ReportManager
            {
                Name = "По зарплатам",
                Report = DeserializeReport(@"Reports\SalariesOfMasters.trdp")
            });

            SelectedReport = Reports.FirstOrDefault();

            RaisePropertyChanged("Reports");
        }

        public static Report DeserializeReport(string path)
        {
            var reportPackager = new ReportPackager();
            using (var sourceStream = System.IO.File.OpenRead(path))
            {
                return reportPackager.Unpackage(sourceStream);
            }
        }
    }
}