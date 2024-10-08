﻿using Autoservice.DAL.Entities;
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
using System.Globalization;
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
        public Order SelectedOrder
        {
            get { return _selectedOrder; }
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
                    },
                    new PanelButtonManager
                    {
                        OnButtonAction = o =>
                        {
                            if (SelectedOrder != null)
                            {
                                if(SelectedOrder.Car.Client==null)
                                {
                                    MessageBox.Show("Непредвиденная ошибка");
                                    return;
                                }
                                if (SelectedOrder.Car.Client.IsIndinidual)
                                    GeneratePDFReport(SelectedOrder,@"Templates\ActOfPerfWorks_Ind.docx");
                                else
                                    GeneratePDFReport(SelectedOrder,@"Templates\ActOfPerfWorks_LE.docx");
                            }
                        },
                        ButtonIcon = "appbar_clipboard_paper_check",
                        ButtonText = "  Акт вып. работ"
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
                {
                    await Task.Run(() => addManager.Save2DB());

                    var service = Get<IGeneralService>();
                    SelectedOrder = service.GetOrderById(SelectedOrder.Id);
                }

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
                    if (_client.Cars.Count > 0)
                        AddCar();
                    else AddNewCar();
                }
                SetIsBusy(false);
            };
            addClientDialog.Show();
        }

        private async void AddNewCar()
        {
            SetIsBusy(true);
            var addCarManager = new AddCarManager { SetIsBusy = IsBusy => SetIsBusy(IsBusy) };
            await Task.Run(() => addCarManager.initializeAdd());
            var addCarDialog = new AddCarDialog(addCarManager);
            addCarDialog.Closed += (sender, args) =>
            {
                SetIsBusy(true);
                if (addCarManager.WasChanged)
                {
                    _client.Cars.Add(addCarManager.ClientCar);
                    var car = addCarManager.ClientCar;
                    car.ClientId = _client.Id;
                    var generalService = Get<IGeneralService>();
                    generalService.AddClientCar(car);
                    AddCar();
                }
                SetIsBusy(false);
            };
            addCarDialog.Show();
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
                    _newOrder.Works = new List<OrderWork>();
                    if (addCarManager.SelectedTemplate != null)
                    {                        
                        var works = addCarManager.Works?.Where(w => w.IsChecked);
                        foreach (var work in works)
                        {
                            _newOrder.Works.Add(new OrderWork
                            {
                                IsNew = true,
                                OrderId = _newOrder.Id,
                                Price = work.Price,
                                WorkId = work.Id,
                                MasterId = UserService.Instance.DefaultMaster.Id
                            });
                            if (addCarManager.SelectedTemplate.Works.FirstOrDefault(w => w.WorkId == work.Id) == null)
                            {
                                addCarManager.SelectedTemplate.Works.Add(new WorkTemplateWork(addCarManager.SelectedTemplate,work));
                            }
                        }
                        UpdateTemplate(addCarManager.SelectedTemplate);
                        SaveWorks2DB(works.Select(w => new Work(w)).ToList());
                    }

                    if (addCarManager.PreOrderIsChecked)
                        AddPreEntry();
                    else
                    {
                        Save2DB();
                        var service = Get<IGeneralService>();
                        SelectedOrder = service.GetOrderById(_newOrder.Id);
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
                    var service = Get<IGeneralService>();
                    SelectedOrder = service.GetOrderById(_newOrder.Id);
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
            //Refresh();
        }
        private void UpdateTemplate(WorkTemplate template)
        {
            var generalService = Get<IGeneralService>();
            generalService.UpdateWorkTemplate(template);
        }

        private void SaveWorks2DB(List<Work> works)
        {
            var generalService = Get<IGeneralService>();
            foreach (var work in works)
            {
                generalService.UpdateWork(work);
            }
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

            Microsoft.Office.Interop.Word.Application wordApp = null;
            Microsoft.Office.Interop.Word.Document aDoc = null;

            try
            {
                wordApp = new Microsoft.Office.Interop.Word.Application { Visible = false };
                aDoc =
                    wordApp.Documents.Open(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templatePath),
                        ReadOnly: false, Visible: false);


                aDoc.Activate();

                Microsoft.Office.Interop.Word.Find fnd = wordApp.ActiveWindow.Selection.Find;

                fnd.ClearFormatting();
                fnd.Replacement.ClearFormatting();
                fnd.Forward = true;

                fnd.Wrap = Microsoft.Office.Interop.Word.WdFindWrap.wdFindContinue;

                WordTemplateHelper.ReplaceLarge(wordApp, "#number", $"АГ-{order.Number}");
                //WordTemplateHelper.ReplaceLarge(wordApp, "Status", order.Status?.ToString() ?? "");
                WordTemplateHelper.ReplaceLarge(wordApp, "#car_name", order.Car?.Car?.LocalName);
                WordTemplateHelper.ReplaceLarge(wordApp, "#car_number", order.Car?.RegistrationNumber);
                //WordTemplateHelper.ReplaceLarge(wordApp, "CarBrand", order.Car.Car.Brand);
                //WordTemplateHelper.ReplaceLarge(wordApp, "CarModel", order.Car.Car.Model);
                WordTemplateHelper.ReplaceLarge(wordApp, "#client_name", order.Car?.Client?.Name);
                WordTemplateHelper.ReplaceLarge(wordApp, "#client_phone", order.Car?.Client?.Phone);
                WordTemplateHelper.ReplaceLarge(wordApp, "#client_discount", $"{order.Car?.Client?.Discount.ToString()} %");
                WordTemplateHelper.ReplaceLarge(wordApp, "#repair_zone", order.RepairZone);
                WordTemplateHelper.ReplaceLarge(wordApp, "#payment_method", order.PaymentMethod?.ToDescriptionString() ?? "");
                WordTemplateHelper.ReplaceLarge(wordApp, "#notes", order.Notes);
                WordTemplateHelper.ReplaceLarge(wordApp, "#total_price", order.CalculatingTotalPrice.ToString());

                WriteSpareParts(wordApp, order);
                WriteWorks(wordApp, order);
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
                _logger.Error(ex, "Error while generate pdf!");
            }
            finally
            {
                aDoc?.Close(false);

                wordApp?.Quit();
            }
        }       

        private void WriteSpareParts(Microsoft.Office.Interop.Word.Application wordApp, Order order)
        {
            int COUNT_OF_ROWS = 30;

            List<OrderSparePart> list = order.SpareParts;
            if (list == null)
                return;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                    continue;
                WordTemplateHelper.ReplaceLarge(wordApp, $"#spare_part{i}", list[i].SparePart?.Name);
                WordTemplateHelper.ReplaceLarge(wordApp, $"#sp_price{i}", list[i].SparePart?.Price.ToString());
                WordTemplateHelper.ReplaceLarge(wordApp, $"#sp_count{i}", list[i].Number.ToString());
            }
            for (int i = list.Count; i < COUNT_OF_ROWS; i++)
            {
                WordTemplateHelper.ReplaceLarge(wordApp, $"#spare_part{i}", "");
                WordTemplateHelper.ReplaceLarge(wordApp, $"#sp_price{i}", "");
                WordTemplateHelper.ReplaceLarge(wordApp, $"#sp_count{i}", "");
            }
        }
        private void WriteWorks(Microsoft.Office.Interop.Word.Application wordApp, Order order)
        {
            int COUNT_OF_ROWS = 8;
            List<OrderWork> list = order.Works;
            if (list == null)
                return;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                    continue;
                WordTemplateHelper.ReplaceLarge(wordApp, $"#work{i}", list[i].Work?.Name);
                WordTemplateHelper.ReplaceLarge(wordApp, $"#w_price{i}", list[i].Price.ToString());
            }
            for (int i = list.Count; i < COUNT_OF_ROWS; i++)
            {
                WordTemplateHelper.ReplaceLarge(wordApp, $"#work{i}", "");
                WordTemplateHelper.ReplaceLarge(wordApp, $"#w_price{i}", "");
            }
        }

        public override async void Refresh()
        {
            SetIsBusy(true);
            var service = Get<IGeneralService>();
            var notSortedOrders = await Task.Run(() => service.GetAllOrders());
            //notSortedOrders.Sort(Order.CompareByPreOrderStartDate);
            notSortedOrders = notSortedOrders.OrderByDescending(o => o.IsPinned).ThenByDescending(o => o.StartDate).ToList();
            Orders = new ObservableCollection<Order>(notSortedOrders);

            var Users = new ObservableCollection<User>(await Task.Run(() => service.GetAllUsers()));

            foreach (var order in Orders)
            {
                order.Activities.Sort(Activity.CompareByStatus);
                order.Activities.ForEach(a => a.User = Users.First(u => u.Id == a.UserId));
            }
            _pbm.ButtonText = SelectedOrder?.Activities?.LastOrDefault()?.GetNextStatus()?.ToString() ?? "";
            RaisePropertyChanged("Orders");
            SetIsBusy(false);
        }
    }
}
