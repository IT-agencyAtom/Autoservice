using ConstaSoft.Core.Controls.Managers;
using Telerik.Reporting;

namespace Autoservice.Controls.Managers
{
    /// <summary>
    ///     This class contains properties that a View can data bind to.
    ///     <para>
    ///         See http://www.galasoft.ch/mvvm
    ///     </para>
    /// </summary>
    public class ReportManager : PanelViewModelBase
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;
                _name = value;

                RaisePropertyChanged("Name");
            }
        }

        private string _uri;

        public string Uri
        {
            get { return _uri; }
            set
            {
                if (_uri == value)
                    return;
                _uri = value;

                RaisePropertyChanged("Uri");
            }
        }

        private ReportSource _report;
        
        public ReportSource Report
        {
            get { return _report; }
            set
            {
                if (_report == value)
                    return;
                _report = value;
        
                RaisePropertyChanged("Report");
            }
        }

        public override void Refresh()
        {
        }
    }
}