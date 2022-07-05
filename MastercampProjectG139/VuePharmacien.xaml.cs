﻿using MastercampProjectG139.Commands;
using MastercampProjectG139.Models;
using MastercampProjectG139.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace MastercampProjectG139
{
    /// <summary>
    /// Interaction logic for VuePharmacien.xaml
    /// </summary>
    public partial class VuePharmacien : Window
    {
        private Pharmacien pharmacien;
        private string numSS;
        private string code;
        private readonly ModelOrdonnance _ordoP;
        public ObservableCollection<ModelMedicament> _medlist;


        public VuePharmacien() => InitializeComponent();

        public VuePharmacien(Pharmacien pharmacien)
        {
            InitializeComponent();
            this.pharmacien = pharmacien;
            txtBlock_nomPrenom.Text = pharmacien.getNom().ToUpper() + " " + pharmacien.getPrenom().ToUpper();
            _ordoP = new ModelOrdonnance("Ordonnance Pharmacien");

        }

        public ObservableCollection<ModelMedicament> Medlist()
        {
            IEnumerable<ModelMedicament> lal = _ordoP.GetAllMedicaments();
            if (lal!=null) {
                lal = Enumerable.Empty<ModelMedicament>();
                lal = _ordoP.GetAllMedicaments();
                _medlist = new ObservableCollection<ModelMedicament>(lal);
                _ordoP.RemoveAllMedicaments();
                return _medlist;

            }
            else
            {
                _medlist = new ObservableCollection<ModelMedicament>(lal);
                return _medlist;
            }
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            LoginScreen loginScreen = new LoginScreen();
            loginScreen.Show();
            this.Close();
        }



        private void GetOrdo(object sender, RoutedEventArgs e)
        {
            DatabaseCommand databaseCommand = new DatabaseCommand();
            numSS = txtBox_numSSPatient.Text;
            code = txtBox_codePatient.Text;
            databaseCommand.getOrdonnance(pharmacien, numSS, code, _ordoP);
            _medlist = Medlist();
            pharatio.ItemsSource = _medlist;
            numSS = "";
            code = "";

        }


        private void About_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.Topmost = true;
            about.Show();
        }
    }
}
