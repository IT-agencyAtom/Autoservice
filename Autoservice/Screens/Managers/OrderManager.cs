using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using Autoservice.Dialogs;
using Autoservice.Dialogs.Managers;
using ConstaSoft.Core.Controls.Managers;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Autoservice.ViewModel.Utils;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Office.Interop.Word;
using Application = System.Windows.Application;
using Task = System.Threading.Tasks.Task;

namespace Autoservice.Screens.Managers
{
    class OrderManager : PanelViewModelBase
    {
        private Order _selectedOrder;
        private string _ordersFilterString;
        private ICollectionView _ordersView { get; set; }
        public Order SelectedOrder { get { return _selectedOrder; }
            set
            {
                _selectedOrder = value;
                ChangeRightButtons();
                RaisePropertyChanged("SelectedOrder");
            }
        }
        public ObservableCollection<Order> Orders { get; set; }
        private PanelButtonManager _pbm;
        private PanelButtonManager _inExpectationpbm { get; set; }
        private Order _newOrder;
        public string OrdersFilterString
        {
            get { return _ordersFilterString; }
            set
            {
                if (_ordersFilterString == value)
                    return;

                _ordersFilterString = value.ToLower();

                _ordersView = CollectionViewSource.GetDefaultView(Orders)
                    ;
                _ordersView.Filter = OrdersFilter;
                _ordersView.MoveCurrentToFirst();

                RaisePropertyChanged("OrdersFilterString");

            }
        }
        private bool OrdersFilter(object item)
        {
            var order = item as Order;
            if (order == null)
                return false;
            if (OrdersFilterString != null)
                if (StringFilter(order) == false)
                    return false;
            return true;
        }
        private bool StringFilter(Order order)
        {
            return order.PersonalNumber.ToLower().Contains(OrdersFilterString) ||
                   order.Car.Client.Name.ToLower().Contains(OrdersFilterString) ||
                   order.Car.Client.Phone.ToLower().Contains(OrdersFilterString) ||
                   order.Car.ToString().ToLower().Contains(OrdersFilterString) ||
                   order.StringStatus.ToLower().Contains(OrdersFilterString) ||
                   order.Works.Select(w => w.Master).ToString().ToLower().Contains(OrdersFilterString);
        }
       


        public RelayCommand MouseDoubleClickCommand { get; set; }
        

        public OrderManager()
        {
            _pbm = new PanelButtonManager
            {
                OnButtonAction = o => ChangeActivity(),
                ButtonIcon = "appbar_checkmark_pencil",
                ButtonText = "Change"
            };
            _inExpectationpbm = new PanelButtonManager
            {
                OnButtonAction = o => ChangeActivityExpactaition(),
                ButtonIcon = "appbar_timer",
                ButtonText = "В ожидании запчастей",
                ButtonVisibility = Visibility.Hidden
            };
            Panel = new PanelManager
            {
                LeftButtons = new ObservableCollection<PanelButtonManager>
                {
                    new PanelButtonManager
                    {
                        OnButtonAction = o => AddHandler(),
                        ButtonIcon = "appbar_add",
                        ButtonText = "Добавить"
                    },
                    new PanelButtonManager
                    {
                        OnButtonAction = o => EditHandler(),
                        ButtonIcon = "appbar_edit",
                        ButtonText = "Изменить"
                    },
                    new PanelButtonManager
                    {
                        OnButtonAction = o => DeleteHandler(),
                        ButtonIcon = "appbar_delete",
                        ButtonText = "Удалить"
                    }
                },
                MiddleButtons = new ObservableCollection<PanelButtonManager>
                {
                    new PanelButtonManager
                    {
                        OnButtonAction = o => Refresh(),
                        ButtonIcon = "appbar_refresh",
                        ButtonText = "Обновить"
                    },
                    new PanelButtonManager
                    {
                        OnButtonAction = o =>
                        {
                            if(SelectedOrder != null)
                                GeneratePDFReport(SelectedOrder, @"Templates\Act.docx");
                        },
                        ButtonIcon = "appbar_clipboard_variant",
                        ButtonText = "  Акт"
                    }
                },
                RightButtons = new ObservableCollection<PanelButtonManager>
                {
                 _inExpectationpbm,
                 _pbm
                }
            };
            MouseDoubleClickCommand = new RelayCommand(EditHandler);
        }

        private void ChangeActivityExpactaition()
        {
            var oldActivity = SelectedOrder.Activities.Last();
            oldActivity.EndTime = DateTime.Now;
            var nextStatus = (oldActivity.Status == ActivityStatus.InOperation) ? ActivityStatus.InExpectationOfSpareParts : ActivityStatus.InOperation; 
            Activity activity = new Activity();
            activity.UniqueString = RandomStrings.GetRandomString(10);
            activity.StartTime = DateTime.Now;
            activity.OrderId = SelectedOrder.Id;
            activity.Status = nextStatus;
            activity.UserId = UserService.Instance.CurrentUser.Id;
            SelectedOrder.Activities.Add(activity);
            SaveActivity2DB(oldActivity, activity);
            ChangeRightButtons();
        }

        private async void ChangeActivity()
        {
            
            var oldActivity = SelectedOrder.Activities.Last();
            oldActivity.EndTime = DateTime.Now;
            var nextStatus = SelectedOrder.Activities.Last().GetNextStatus();
            if (nextStatus == null)
                return;
            Activity activity = new Activity();
            activity.UniqueString = RandomStrings.GetRandomString(10);
            activity.StartTime = DateTime.Now;
            activity.OrderId = SelectedOrder.Id;
            activity.Status = (ActivityStatus)nextStatus;
            activity.UserId = UserService.Instance.CurrentUser.Id;
            SelectedOrder.Activities.Add(activity);
            SaveActivity2DB(oldActivity, activity);
            ChangeRightButtons();

        }
        private void ChangeRightButtons()
        {
            if (_selectedOrder == null)
                return;
            if (_selectedOrder.Activities.Count == 0 || 
                _selectedOrder.Activities == null || 
                _selectedOrder.Activities.LastOrDefault().Status == ActivityStatus.Closed)
                _pbm.ButtonVisibility = Visibility.Hidden;
            else
            {
                _pbm.ButtonVisibility = Visibility.Visible;
                _pbm.ButtonText = _selectedOrder.Activities.LastOrDefault().GetNextStatus().ToDescriptionString();
                if (_selectedOrder.Activities.LastOrDefault().Status == ActivityStatus.InOperation)
                {
                    _inExpectationpbm.ButtonVisibility = Visibility.Visible;
                }
                else
                    _inExpectationpbm.ButtonVisibility = Visibility.Hidden;

            }
        }

        private void AddHandler()
        {
            _newOrder = new Order();
            _newOrder.StartDate = DateTime.Now;
            AddClient();
        }
        private async void EditHandler()
        {
            if (SelectedOrder == null)
                return;

            SetIsBusy(true);

            var addManager = new AddOrderManager { SetIsBusy = isBusy => SetIsBusy(isBusy) };
            await Task.Run(() => addManager.initializeEdit(SelectedOrder));

            var addDialog = new AddOrderDialog(addManager);

            addDialog.Closed += async (sender, args) =>
            {
                SetIsBusy(true);
                if (addManager.WasChanged)                
                    await Task.Run(() => addManager.Save2DB());               
                Refresh();
                SetIsBusy(false);
            };
            addDialog.Show();
        }
        
        private async void DeleteHandler()
        {
            if (SelectedOrder == null)
                return;

            var deleteDialogSettings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Да",
                NegativeButtonText = "Нет",
                FirstAuxiliaryButtonText = "Отмена"
            };

            var metroWindow = Application.Current.MainWindow as MetroWindow;
            if (metroWindow == null)
                return;

            SetIsBusy(true);

            var result =
                await
                    metroWindow.ShowMessageAsync("Подтвердите удаление заказа",
                        $"Вы уверен что хотите удалить заказ №{SelectedOrder.PersonalNumber}?",
                        MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, deleteDialogSettings);

            if (result == MessageDialogResult.Affirmative)
            {
                var generalService = Get<IGeneralService>();
                generalService.DeleteOrder(SelectedOrder);

                await
                    metroWindow.ShowMessageAsync("Успех", $"Заказ {SelectedOrder.PersonalNumber} был удалён");
            }
            Refresh();
            SetIsBusy(false);
        }

        private Client _client;
        private async void AddClient()
        {
            SetIsBusy(true);
            var addClientManager = new AddClientSelectorManager { SetIsBusy = IsBusy => SetIsBusy(IsBusy) };
            await Task.Run(() => addClientManager.initialize());
            var addClientDialog = new AddClientSelectorDialog(addClientManager);
            addClientDialog.Closed += (sender, args) =>
            {
                SetIsBusy(true);
                if (addClientManager.WasChanged)
                {
                    _client = addClientManager.Client;
                    AddCar();
                }
                SetIsBusy(false);
            };
            addClientDialog.Show();
        }
        private async void AddCar()
        {
            SetIsBusy(true);
            var addCarManager = new AddCarSelectorManager() { SetIsBusy = IsBusy => SetIsBusy(IsBusy) };
            addCarManager.initialize(_client);
            var addClientDialog = new AddCarSelectorDialog(addCarManager);
            addClientDialog.Closed += (sender, args) =>
            {
                SetIsBusy(true);
                if (addCarManager.WasChanged)
                {
                    _newOrder.Car = addCarManager.Car;
                    _newOrder.ClientCarId = addCarManager.Car.Id;                   
                    if (addCarManager.Template != null)
                    {
                        var works = addCarManager.Template.Works;
                        _newOrder.Works = new List<OrderWork>();
                        foreach (var work in works)
                        {
                            _newOrder.Works.Add(new OrderWork
                            {
                                IsNew = true,
                                OrderId = SelectedOrder.Id,
                                Price = work.Price,
                                WorkId = work.Id,
                                MasterId = UserService.Instance.DefaultMaster.Id
                            });
                        }
                    }
                    if (addCarManager.PreOrderIsChecked)
                        AddPreEntry();
                    else
                    {
                        Save2DB();
                        EditHandler();
                    }
                }
                SetIsBusy(false);
            };
            addClientDialog.Show();
        }
        private async void AddPreEntry()
        {
            SetIsBusy(true);
            var addPreEntryManager = new AddPreEntryManager() { SetIsBusy = IsBusy => SetIsBusy(IsBusy) };
            addPreEntryManager.Initialize();
            var addPreEntryDialog = new AddPreEntryDialog(addPreEntryManager);
            addPreEntryDialog.Closed += (sender, args) =>
            {
                SetIsBusy(true);
                if (addPreEntryManager.WasChanged)
                {                    
                    _newOrder.PreOrderDateTime = addPreEntryManager.SelectedDate;
                    Save2DB();
                    SelectedOrder = Orders.SingleOrDefault(o => o.Id == _newOrder.Id); 
                    EditHandler();
                }
                SetIsBusy(false);
            };
            addPreEntryDialog.Show();
        }


        private void Save2DB()
        {
            var generalService = Get<IGeneralService>();
            generalService.AddOrder(_newOrder);

            var activity = new Activity
            {
                StartTime = DateTime.Now,
                UniqueString = RandomStrings.GetRandomString(10),
                UserId = UserService.Instance.CurrentUser.Id,
                Status = ActivityStatus.New,
                OrderId = _newOrder.Id
            };
            generalService.AddActivity(activity);
            Refresh();        
        }            

        public void SaveActivity2DB(Activity oldActivity, Activity newActivity)
        {
            var generalService = Get<IGeneralService>();            
            generalService.UpdateOrder(SelectedOrder);
            generalService.UpdateActivity(oldActivity);
            generalService.AddActivity(newActivity);
            Refresh();
        }

        private void GeneratePDFReport(Order order, string templatePath)
        {
            string pdfsFolderPath = "PDFs";

            if (!Directory.Exists(pdfsFolderPath))
                Directory.CreateDirectory(pdfsFolderPath);

            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application { Visible = false };
            Microsoft.Office.Interop.Word.Document aDoc =
                wordApp.Documents.Open(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templatePath),
                    ReadOnly: false, Visible: false);

            try
            {
                aDoc.Activate();

                Microsoft.Office.Interop.Word.Find fnd = wordApp.ActiveWindow.Selection.Find;

                fnd.ClearFormatting();
                fnd.Replacement.ClearFormatting();
                fnd.Forward = true;

                fnd.Wrap = Microsoft.Office.Interop.Word.WdFindWrap.wdFindContinue;

                ReplaceLarge(wordApp, "Number", $"АГ-{order.Number}");
                //ReplaceLarge(wordApp, "Status", order.Status?.ToString() ?? "");
                ReplaceLarge(wordApp, "CarName", order.Car?.LocalName);
                ReplaceLarge(wordApp, "CarNumber", order.Car?.RegistrationNumber);
                //ReplaceLarge(wordApp, "CarBrand", order.Car.Car.Brand);
                //ReplaceLarge(wordApp, "CarModel", order.Car.Car.Model);
                ReplaceLarge(wordApp, "ClientName", order.Car?.Client?.Name);
                ReplaceLarge(wordApp, "ClientPhone", order.Car?.Client?.Phone);
                ReplaceLarge(wordApp, "ClientDiscount", order.Car?.Client?.Discount.ToString());
                ReplaceLarge(wordApp, "RepairZone", order.RepairZone);
                ReplaceLarge(wordApp, "PaymentMethod", order.PaymentMethod?.ToString() ?? "");
                ReplaceLarge(wordApp, "Notes", order.Notes);

                var ext = "pdf";

                var outputFilePath = string.Format("{0}{2}{1}.{3}", pdfsFolderPath, order.Number.ToString(),
                    System.IO.Path.DirectorySeparatorChar, ext);

                var subIndex = 0;
                while (File.Exists(outputFilePath))
                {
                    outputFilePath = string.Format("{0}{2}{1}-{4:00}.{3}", pdfsFolderPath, order.Number.ToString(),
                        System.IO.Path.DirectorySeparatorChar, ext, ++subIndex);
                }

                //aDoc.SaveAs(outputFilePath, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF);
                aDoc.ExportAsFixedFormat(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, outputFilePath), WdExportFormat.wdExportFormatPDF);
                Process.Start(outputFilePath);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                aDoc.Close(false);

                wordApp.Quit();
            }
        }

        private void ReplaceLarge(Microsoft.Office.Interop.Word.Application wordApp, string text, string replacement)
        {
            if (replacement == null)
                return;

            List<string> subs = new List<string>();
            int counter = 0;
            while (counter <= replacement.Length)
            {
                if (replacement.Length < counter + 250)
                {
                    subs.Add(replacement.ToString().Substring(counter, replacement.Length - counter));
                }
                else
                {
                    subs.Add(replacement.Substring(counter, 250) + "#r#");
                }
                counter += 250;
            }

            Microsoft.Office.Interop.Word.Find fnd = wordApp.ActiveWindow.Selection.Find;

            fnd.ClearFormatting();
            fnd.Replacement.ClearFormatting();
            fnd.Forward = true;

            fnd.Text = text;
            fnd.Wrap = Microsoft.Office.Interop.Word.WdFindWrap.wdFindStop;
            fnd.Replacement.Text = subs[0];
            fnd.Execute(Forward: true, Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);
            fnd.Text = "#r#";
            for (int i = 1; i < subs.Count; i++)
            {
                fnd.Replacement.Text = subs[i];
                fnd.Execute(Forward: true, Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);
            }
        }

        public override async void Refresh()
        {
            SetIsBusy(true);
            var service = Get<IGeneralService>();
            var notSortedOrders = service.GetAllOrders();
            //notSortedOrders.Sort(Order.CompareByPreOrderStartDate);
            notSortedOrders = notSortedOrders.OrderByDescending(o => o.IsPinned).ThenByDescending(o => o.StartDate).ToList();
            Orders = new ObservableCollection<Order>(notSortedOrders);
            
            var Users = new ObservableCollection<User>(service.GetAllUsers());

            foreach (var order in Orders)
            {
                order.Activities.Sort(Activity.CompareByStatus);
                order.Activities.ForEach(a => a.User = Users.First(u => u.Id == a.UserId));
            }
            _pbm.ButtonText = SelectedOrder?.Activities?.LastOrDefault()?.GetNextStatus()?.ToString() ?? "";
            RaisePropertyChanged("Orders");
            SetIsBusy(false);
        }

        public async Task<Order> GetOrder(Guid id)
        {
            SetIsBusy(true);
            var service = Get<IGeneralService>();
            var order = await Task.Run(() => service.GetOrderById(id));
            RaisePropertyChanged("Orders");
            SetIsBusy(false);
            return order;
        }
    }
}
