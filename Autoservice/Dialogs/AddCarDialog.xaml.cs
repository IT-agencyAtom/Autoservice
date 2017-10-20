﻿using Autoservice.Dialogs.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Autoservice.Dialogs
{
    /// <summary>
    /// Interaction logic for AddWorkDialog.xaml
    /// </summary>
    public partial class AddCarDialog
    {
        public AddCarDialog(AddCarManager acm)
        {
            InitializeComponent();

            DataContext = acm;
            acm.OnExit = Close;
        }
    }
}
