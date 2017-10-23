using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ConstaSoft.Core.Controls.Managers;
using GalaSoft.MvvmLight.Command;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using Autoservice.ViewModel.Utils;
using MessageBox = System.Windows.MessageBox;

namespace Autoservice.Screens.Managers
{
    /// <summary>
    /// Model for Login screen.</summary>
    public class LoginManager : PanelViewModelBase
    {
        public Action OnLogIn;
        public ObservableCollection<User> Users { get; set; }

        private User _user;

        public User User
        {
            get { return _user; }
            set
            {
                if (_user == value)
                    return;
                _user = value;

                RaisePropertyChanged("User");
            }
        }

        public RelayCommand LogIn { get; private set; }

        public LoginManager()
        {
            LogIn = new RelayCommand(LogInHandler);
            User = new User();
        }

        private void LogInHandler()
        {
            var user = Users?.FirstOrDefault(u => u.Login == User.Login && u.Password == User.Password);
            if (user == null)
            {
                MessageBox.Show($"Не удалось выполнить вход. Неправильная комбинация логина и пароля", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            UserService.Instance.CurrentUser = user;
            User = new User();
            OnLogIn();
        }

        public override async void Refresh()
        {
            SetIsBusy(true);

            var service = Get<IGeneralService>();
            Users = new ObservableCollection<User>(await Task.Run(() => service.GetAllUsers()));

            RaisePropertyChanged("Users");

            SetIsBusy(false);
        }
    }
}