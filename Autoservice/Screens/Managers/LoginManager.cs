using System;
using System.Collections.Generic;
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
using Excel = Microsoft.Office.Interop.Excel;

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
            //ParseCars();

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

        private void ParseCars()
        {
            //Create COM Objects. Create a COM object for everything that is referenced
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@"c:\_work\_projects\_free\autoservice\Autoservice\bin\Debug\1_brands (2).csv");
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            Excel.Workbook xlWorkbook1 = xlApp.Workbooks.Open(@"c:\_work\_projects\_free\autoservice\Autoservice\bin\Debug\2_models (2).csv");
            Excel._Worksheet xlWorksheet1 = xlWorkbook1.Sheets[1];
            Excel.Range xlRange1 = xlWorksheet1.UsedRange;

            Dictionary<int, string> brands = new Dictionary<int, string>();
            List<Car> cars = new List<Car>();
            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!
            for (int i = 2; i <= 213; i++)
            {

                //write the value to the console
                //if (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
                //    Console.Write(xlRange.Cells[i, j].Value2.ToString() + "\t");

                //add useful things here!   
                brands.Add(int.Parse(xlRange.Cells[i, 1].Value2.ToString()), xlRange.Cells[i, 2].Value2.ToString());
            }

            for (int i = 2; i <= 2268; i++)
            {
                var brandVal = int.Parse(xlRange1.Cells[i, 2].Value2.ToString());
                if (brands.ContainsKey(brandVal))
                {
                    var brand = brands[brandVal];

                    cars.Add(new Car
                    {
                        Brand = brand,
                        Model = xlRange1.Cells[i, 3].Value2.ToString(),
                        Type = Car.CarType.Automobile
                    });
                }
            }

            var service = Get<IGeneralService>();
            service.AddCars(cars);
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