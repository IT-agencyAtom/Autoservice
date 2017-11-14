using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using ConstaSoft.Core.Controls.Managers;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using Autoservice.Dialogs;
using Autoservice.Dialogs.Managers;
using Autoservice.ViewModel.Utils;
using Application = System.Windows.Application;

namespace Autoservice.Screens.Managers
{
    /// <summary>
    /// Model for Settings screen.</summary>
    public class SettingsManager : PanelViewModelBase
    {
        public ObservableCollection<User> Users { get; set; }

        private User _selectedUser;

        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (_selectedUser == value)
                    return;
                _selectedUser = value;

                RaisePropertyChanged();
            }
        }

        private string _accountDomain;

        public string AccountDomain
        {
            get { return _accountDomain; }
            set
            {
                if (_accountDomain == value)
                    return;
                _accountDomain = value;

                RaisePropertyChanged();
            }
        }

        private string _accountUsername;

        public string AccountUsername
        {
            get { return _accountUsername; }
            set
            {
                if (_accountUsername == value)
                    return;
                _accountUsername = value;

                RaisePropertyChanged();
            }
        }

        private string _accountPassword;

        public string AccountPassword
        {
            get { return _accountPassword; }
            set
            {
                if (_accountPassword == value)
                    return;
                _accountPassword = value;

                RaisePropertyChanged();
            }
        }

        public RelayCommand EditUser { get; private set; }
        public RelayCommand DeleteUser { get; private set; }
        public RelayCommand AddUser { get; private set; }
        public RelayCommand SaveAccountSettings { get; private set; }

        public SettingsManager()
        {
            Panel = new PanelManager
            {
                MiddleButtons = new ObservableCollection<PanelButtonManager>
                {
                    new PanelButtonManager
                    {
                        OnButtonAction = o => Refresh(),
                        ButtonIcon = "appbar_refresh",
                        ButtonText = "Refresh"
                    }
                },

                RightButtons = new ObservableCollection<PanelButtonManager>
                {

                }
            };

            AddUser = new RelayCommand(AddUserHandler);
            EditUser = new RelayCommand(EditUserHandler);
            DeleteUser = new RelayCommand(DeleteUserHandler);
            SaveAccountSettings = new RelayCommand(SaveAccountSettingsHandler);

            AccountDomain = Properties.Settings.Default.AccountDomain;
            AccountUsername = Properties.Settings.Default.AccountUsername;
            AccountPassword = Properties.Settings.Default.AccountPassword;
        }

        private async void AddUserHandler()
        {
            SetIsBusy(true);

            var addUserManager = new AddUserManager {SetIsBusy = isBusy => SetIsBusy(isBusy)};
            await Task.Run(() => addUserManager.initializeAdd());

            var addUserDialog = new AddUserDialog(addUserManager);

            addUserDialog.Closed += async (sender, args) =>
            {
                SetIsBusy(true);

                if (addUserManager.WasChanged)
                {
                    await Task.Run(() => addUserManager.Save2DB());
                    
                    Refresh();
                }

                SetIsBusy(false);
            };

            addUserDialog.Show();
        }

        private async void EditUserHandler()
        {
            if (SelectedUser == null)
                return;

            SetIsBusy(true);

            var addUserManager = new AddUserManager {SetIsBusy = isBusy => SetIsBusy(isBusy)};
            await Task.Run(() => addUserManager.initializeEdit(SelectedUser));

            var addUserDialog = new AddUserDialog(addUserManager);

            addUserDialog.Closed += async (sender, args) =>
            {
                SetIsBusy(true);

                if (addUserManager.WasChanged)
                {
                    await Task.Run(() => addUserManager.Save2DB());

                    Refresh();
                }

                SetIsBusy(false);
            };
            addUserDialog.Show();
        }

        private async void DeleteUserHandler()
        {
            if (SelectedUser == null)
                return;

            var deleteDialogSettings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No",
                FirstAuxiliaryButtonText = "Cancel"
            };

            var metroWindow = Application.Current.MainWindow as MetroWindow;
            if (metroWindow == null)
                return;

            SetIsBusy(true);

            var result =
                await
                    metroWindow.ShowMessageAsync("Confirm user delete",
                        $"Are you sure to delete user {SelectedUser.Login}",
                        MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, deleteDialogSettings);

            if (result == MessageDialogResult.Affirmative)
            {
                var generalService = Get<IGeneralService>();
                generalService.DeleteUser(SelectedUser);

                await
                    metroWindow.ShowMessageAsync("Success", $"User {SelectedUser.Login} was deleted");

                Refresh();
            }

            SetIsBusy(false);
        }

        private void SaveAccountSettingsHandler()
        {
            if (string.IsNullOrWhiteSpace(AccountDomain) ||
                string.IsNullOrWhiteSpace(AccountUsername) ||
                string.IsNullOrWhiteSpace(AccountPassword))
            {
                MessageBox.Show("Please fill all account fields before saving!", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }            

            //Save to settings
            Properties.Settings.Default.AccountDomain = AccountDomain;
            Properties.Settings.Default.AccountUsername = AccountUsername;
            Properties.Settings.Default.AccountPassword = AccountPassword;           
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