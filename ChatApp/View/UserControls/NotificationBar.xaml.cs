﻿using System;
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
using ChatApp.ViewModel;
using ChatApp.Model;

namespace ChatApp.View.UserControls
{
    /// <summary>
    /// Interaction logic for NotificationBar.xaml
    /// </summary>
    public partial class NotificationBar : UserControl
    {
        public NotificationBar()
        {
            InitializeComponent();
            this.DataContext = new NotificationBarViewModel(new NotificationManager());
        }
    }
}
