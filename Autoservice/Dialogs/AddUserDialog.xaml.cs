using Autoservice.Dialogs.Managers;

namespace Autoservice.Dialogs
{
    /// <summary>
    ///     Interaction logic for AddUserDialog.xaml
    /// </summary>
    public partial class AddUserDialog
    {
        public AddUserDialog(AddUserManager addUserManager)
        {
            InitializeComponent();

            DataContext = addUserManager;
            addUserManager.OnExit += Close;
        }
    }
}