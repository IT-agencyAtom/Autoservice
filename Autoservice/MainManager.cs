using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using AutoUpdaterDotNET;
using ConstaSoft.Core.Controls.Managers;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using Autoservice.Controls.Managers;
using Autoservice.DAL.Common.Context;
using Autoservice.DAL.Services;
using Autoservice.Migrations;
using Autoservice.Screens.Managers;
using Autoservice.ViewModel.Utils;
using Autoservice.Screens.Managers;

namespace Autoservice
{
    /// <summary>
    /// Model for Main screen.</summary>
    public class MainManager : ManagerBase
    {
        public string ProgramVersion => $"Version: {Assembly.GetExecutingAssembly().GetName().Version}";
        public string UserName => $"{UserService.Instance.CurrentUser?.Login}";
        
        public string Title => Process.GetCurrentProcess().ProcessName;
        
        public RelayCommand ShowLogs { get; private set; }
        public RelayCommand ShowChangelog { get; private set; }
        
        
        private SettingsManager _settingsManager;
        private LoginManager _loginManager;
        private OrderManager _orderManager;
        private CarsManager _carsManager;
        private ClientsManager _clientsManager;
        private WorksManager _worksManager;
        private MastersManager _mastersManager;
        private WorkTemplateManager _workTemplateManager;
        private WarehouseManager _warehouseManager;
        private CarModelsManager _carModelsManager;

        private ObservableCollection<ScreenManager> _tabScreens;

        public ObservableCollection<ScreenManager> TabScreens
        {
            get { return _tabScreens; }
            set
            {
                if (_tabScreens == value)
                    return;

                _tabScreens = value;
                RaisePropertyChanged("TabScreens");
            }
        }

        private ScreenManager _tabSelectedItem;

        public ScreenManager TabSelectedItem
        {
            get { return _tabSelectedItem; }
            set
            {
                if (value == null || Equals(_tabSelectedItem, value))
                    return;

                _tabSelectedItem = value;

                switch (_tabSelectedItem.Label)
                {
                    case Constants.ExitScreenName:
                        LogOutHandler();
                        break;
                }

                RaisePropertyChanged("TabSelectedItem");
            }
        }


        /// <summary>
        ///     Initializes a new instance of the MainManager class.
        /// </summary>
        public MainManager()
        {
            printModuleVersions();
            _logger.Info($"Load application {Process.GetCurrentProcess().ProcessName} v{Assembly.GetExecutingAssembly().GetName().Version}");

            checkUpdates();

            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<AutoServiceDBContext, Configuration>());    
                     

            _settingsManager = new SettingsManager
            {
                SetIsBusy = isBusy => IsBusy = isBusy
            };

            _loginManager = new LoginManager
            {
                SetIsBusy = isBusy => IsBusy = isBusy,
                OnLogIn = () => RefreshTabs()
            };
            _orderManager = new OrderManager
            {
                SetIsBusy = isBusy => IsBusy = isBusy
            };
            _carsManager = new CarsManager
            {
                SetIsBusy = isBusy => IsBusy = isBusy
            };
            _clientsManager = new ClientsManager
            {
                SetIsBusy = isBusy => IsBusy = isBusy
            };
            _worksManager = new WorksManager
            {
                SetIsBusy = isBusy => IsBusy = isBusy
            };
            _mastersManager = new MastersManager
            {
                SetIsBusy = isBusy => IsBusy = isBusy
            };
            _workTemplateManager = new WorkTemplateManager
            {
                SetIsBusy = isBusy => IsBusy = isBusy
            };
            _warehouseManager = new WarehouseManager
            {
                SetIsBusy = isBusy => IsBusy = isBusy
            };
            _carModelsManager = new CarModelsManager
            {
                SetIsBusy = isBusy => IsBusy = isBusy
            };
            ShowChangelog = new RelayCommand(() => Process.Start("Changelog.docx"));
            ShowLogs = new RelayCommand(() => Process.Start(Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), @"ConstaSoft\" + Assembly.GetExecutingAssembly().GetName().Name + @"\Logs")));

            RefreshTabs();
        }

        private void RefreshTabs()
        {
            RaisePropertyChanged("UserName");

            TabScreens = new ObservableCollection<ScreenManager>();
            if (UserService.Instance.CurrentUser == null)
            {
                TabScreens.Add(new ScreenManager
                {
                    Label = "Log In",
                    ToolTip = "Log In",
                    Icon = new PackIconMaterial { Kind = PackIconMaterialKind.LoginVariant },
                    Tag = _loginManager
                });

                TabSelectedItem = TabScreens.FirstOrDefault();

                return;
            }

            TabScreens.Add(new ScreenManager
            {
                Label = "Заказы",
                ToolTip = "Заказы",
                Icon = new PackIconMaterial { Kind = PackIconMaterialKind.Tab },
                Tag = _orderManager
            });
            TabScreens.Add(new ScreenManager
            {
                Label = "Транспортные средства",
                ToolTip = "Транспортные средства",
                Icon = new PackIconMaterial { Kind = PackIconMaterialKind.Car },
                Tag = _carsManager
            });
            TabScreens.Add(new ScreenManager
            {
                Label = "Клиенты",
                ToolTip = "Клиенты",
                Icon = new PackIconMaterial { Kind = PackIconMaterialKind.HumanGreeting },
                Tag = _clientsManager
            });
            TabScreens.Add(new ScreenManager
            {
                Label = "Работы",
                ToolTip = "Работы",
                Icon = new PackIconMaterial { Kind = PackIconMaterialKind.Wrench },
                Tag = _worksManager
            });
            TabScreens.Add(new ScreenManager
            {
                Label = "Мастера",
                ToolTip = "Мастера",
                Icon = new PackIconMaterial { Kind = PackIconMaterialKind.Worker },
                Tag = _mastersManager
            });
            TabScreens.Add(new ScreenManager
            {
                Label = "Шаблоны работ",
                ToolTip = "Шаблоны",
                Icon = new PackIconMaterial { Kind = PackIconMaterialKind.Tablet },
                Tag = _workTemplateManager
            });
            TabScreens.Add(new ScreenManager
            {
                Label = "Модели ТС",
                ToolTip = "Модели ТС",
                Icon = new PackIconMaterial { Kind = PackIconMaterialKind.CarConnected },
                Tag = _carModelsManager
            });
            TabScreens.Add(new ScreenManager
            {
                Label = "Склад",
                ToolTip = "Список запчастей",
                Icon = new PackIconMaterial { Kind = PackIconMaterialKind.CarBattery },
                Tag = _warehouseManager
            });
            TabScreens.Add(new ScreenManager
            {
                Label = "Отчёты",
                ToolTip = "Отчёты",
                Icon = new PackIconMaterial { Kind = PackIconMaterialKind.FileChart },
                Tag = new ReportsManager { SetIsBusy = isBusy => IsBusy = isBusy }
            });
            if (UserService.Instance.IsAdmin)
            {                

                TabScreens.Add(new ScreenManager
                {
                    Label = "Настройки",
                    ToolTip = "Настройки",
                    Icon = new PackIconMaterial { Kind = PackIconMaterialKind.Settings },
                    Tag = _settingsManager
                });
            }           

            TabScreens.Add(new ScreenManager
            {
                Label = Constants.ExitScreenName,
                ToolTip = Constants.ExitScreenName,
                Icon = new PackIconMaterial { Kind = PackIconMaterialKind.ExitToApp },
            });

            TabSelectedItem = TabScreens.FirstOrDefault();
        }

        private void LogOutHandler()
        {
            UserService.Instance.CurrentUser = null;

            RaisePropertyChanged("UserName");

            RefreshTabs();
        }

        private void checkUpdates()
        {
            //AutoUpdater.CurrentCulture = CultureInfo.CreateSpecificCulture("en-EN");

            //If you want to open download page when user click on download button uncomment below line.

            //AutoUpdater.OpenDownloadPage = true;

            //Don't want user to select remind later time in AutoUpdater notification window then uncomment 3 lines below so default remind later time will be set to 2 days.

            AutoUpdater.LetUserSelectRemindLater = false;
            //AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Days;
            //AutoUpdater.RemindLaterAt = 2;

            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            AutoUpdater.Start($"http://constasoft.ru/product/Upwork/{Process.GetCurrentProcess().ProcessName}/{Process.GetCurrentProcess().ProcessName}.xml");
        }

        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args != null)
            {
                if (args.IsUpdateAvailable)
                {
                    var dialogResult =
                        MessageBox.Show(
                            $"New version available: {args.CurrentVersion}. Current version: {args.InstalledVersion}. Update now?", @"Available update",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Information);

                    if (dialogResult.Equals(MessageBoxResult.Yes))
                    {
                        try
                        {
                            //You can use Download Update dialog used by AutoUpdater.NET to download the update.

                            AutoUpdater.DownloadUpdate();
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    //MessageBox.Show(@"There is no update avilable please try again later.", @"No update available",
                    //  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                //MessageBox.Show(
                //       @"There is a problem reaching update server please check your internet connection and try again later.",
                //       @"Update check failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void printModuleVersions()
        {
            printLoad();

            _logger.Info("====== > Loading modules < =======");
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in loadedAssemblies)
            {
                if (assembly.GetName().Name.Equals(Process.GetCurrentProcess().ProcessName))
                    _logger.Info("=== > Client # Version: {0}, Destination: {1}", assembly.GetName().Version, assembly.Location);
                else if (assembly.GetName().Name.Equals("ConstaSoft.Core"))
                    _logger.Info("=== > ConstaSoft Core # Version: {0}, Destination: {1}", assembly.GetName().Version, assembly.Location);
                else if (assembly.GetName().Name.Equals("ConstaSoft.DAL.Core"))
                    _logger.Info("=== > ConstaSoft DAL Core # Version: {0}, Destination: {1}", assembly.GetName().Version, assembly.Location);
            }

            _logger.Info(""); _logger.Info(""); _logger.Info("");
        }

        private void printLoad()
        {
            _logger.Info(""); _logger.Info(""); _logger.Info("");

            _logger.Info("===================================");
            _logger.Info("===================================");
            _logger.Info("===================================");
            _logger.Info("==== > Load application < ======");
            _logger.Info("===================================");
            _logger.Info("===================================");
            _logger.Info("===================================");

            _logger.Info(""); _logger.Info(""); _logger.Info("");
        }
        public void RestartOrderScreen()
        {
            _orderManager = new OrderManager
            {
                SetIsBusy = isBusy => IsBusy = isBusy
            };
        }
    }
    

    public class SelectedItemToContentConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // first value is selected menu item, second value is selected option item
            if (values != null && values.Length > 1)
            {
                return values[0] ?? values[1];
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return targetTypes.Select(t => Binding.DoNothing).ToArray();
        }
    }

    public class CustomHamburgerMenuIconItem : HamburgerMenuIconItem
    {
        public static readonly DependencyProperty ToolTipProperty
            = DependencyProperty.Register("ToolTip",
                typeof(object),
                typeof(CustomHamburgerMenuIconItem),
                new PropertyMetadata(null));

        public object ToolTip
        {
            get { return (object)GetValue(ToolTipProperty); }
            set { SetValue(ToolTipProperty, value); }
        }
    }
}
