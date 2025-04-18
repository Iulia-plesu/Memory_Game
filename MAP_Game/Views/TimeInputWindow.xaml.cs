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
using System.Windows.Shapes;

namespace MAP_Game.View
{
    public partial class TimeInputWindow : Window
    {
        public int TimeLimit { get; private set; }

        public TimeInputWindow()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TimeInputTextBox.Text, out int time) && time > 0)
            {
                TimeLimit = time; 
            }
            else
            {
                MessageBox.Show("Invalid input. Using default time limit of 60 seconds.");
                TimeLimit = 60;
            }

            DialogResult = true;
        }
    }

}
