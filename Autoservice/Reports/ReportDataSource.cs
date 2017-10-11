using System.ComponentModel;
using Autoservice.ViewModel;

namespace Autoservice.Reports
{
    [DataObject]
    public class ReportDataSource
    {
        public ReportDataSource()
        {
            new ViewModelLocator();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public void TestFunction()
        {
        }
    }
}
