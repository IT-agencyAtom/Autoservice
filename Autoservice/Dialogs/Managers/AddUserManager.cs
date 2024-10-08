﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using ConstaSoft.Core.Controls.Managers;
using GalaSoft.MvvmLight.Command;
using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;

namespace Autoservice.Dialogs.Managers
{
    /// <summary>
    /// Model for Add User dialog.</summary>
    public class AddUserManager : PanelViewModelBase
    {
        public Action OnExit { get; set; }

        public string Title { get; set; }

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

        private ObservableCollection<string> _roles;

        public ObservableCollection<string> Roles
        {
            get { return _roles; }
            set
            {
                if (_roles == value)
                    return;

                _roles = value;
                RaisePropertyChanged("Roles");
            }
        }

        [Required(ErrorMessage = "Укажите роль пользователя")]
        public string Role
        {
            get { return User.Role; }
            set
            {
                if (User.Role == value)
                    return;

                User.Role = value;
                RaisePropertyChanged("Role");
            }
        }

        //Комманды
        public RelayCommand Save { get; private set; }

        public RelayCommand Cancel { get; private set; }

        private bool _isEdit { get; set; }

        /// <summary>
        ///     Initializes a new instance of the SettingsScreen class.
        /// </summary>
        public void initializeEdit(User user)
        {
            _isEdit = true;

            initialize();

            User = user;

            Title = "Изменить пользователя";
        }

        public void initializeAdd()
        {
            initialize();

            User = new User();

            Title = "Добавить пользователя";
        }

        private void initialize()
        {
            Panel = new PanelManager
            {
                RightButtons = new ObservableCollection<PanelButtonManager>
                {
                    new PanelButtonManager
                    {
                        OnButtonAction = (obj) => CancelHandler(),
                        ButtonIcon = "appbar_undo",
                        ButtonText = "Отмена"
                    },
                    new PanelButtonManager
                    {
                        OnButtonAction = (obj) => SaveHandler(),
                        ButtonIcon = "appbar_disk",
                        ButtonText = "Сохранить"
                    }
                }
            };

            Roles = new ObservableCollection<string>
            {
                "Admin",
                "Manager"
            };
        }

        private void CancelHandler()
        {
            OnExit();
        }

        private void SaveHandler()
        {
            Validate();
            if (HasErrors)
                return;

            WasChanged = true;

            OnExit();
        }

        public void Save2DB()
        {
            var generalService = Get<IGeneralService>();

            if (_isEdit)
                generalService.UpdateUser(User);
            else
                generalService.AddUser(User);
        }

        public override void Refresh()
        {
            SetIsBusy(false);
        }
    }
}