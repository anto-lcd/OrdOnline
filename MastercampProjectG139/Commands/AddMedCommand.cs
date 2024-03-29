﻿using MastercampProjectG139.Models;
using MastercampProjectG139.Services;
using MastercampProjectG139.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MastercampProjectG139.Commands
{
    internal class AddMedCommand : CommandBase
    {
        private readonly AddMedViewModel _addMedViewModel;
        private readonly ModelOrdonnance _ordonnance;
        private readonly NavigationService _medicamentViewNavigationService;

        public AddMedCommand(AddMedViewModel addMedViewModel, ModelOrdonnance ordonnance, NavigationService medicamentViewNavigationService)
        {
            _addMedViewModel = addMedViewModel;
            _ordonnance = ordonnance;
            _medicamentViewNavigationService = medicamentViewNavigationService;
            //Update le changement de variable
            _addMedViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(AddMedViewModel.Name) || e.PropertyName == nameof(AddMedViewModel.Frequence) || e.PropertyName == nameof(AddMedViewModel.Duration))
            {
                OnCanExecutedChanged();  
            }
        }
        //Vérifie si le champ est vide pour savoir si le le bouton est valide
        public override bool CanExecute(object parameter)
        {
            return !string.IsNullOrEmpty(_addMedViewModel.Name) && !string.IsNullOrEmpty(_addMedViewModel.Duration) && !string.IsNullOrEmpty(_addMedViewModel.Frequence) &&  base.CanExecute(parameter);
        }

        //Commande d'exécution lorsqu'on appuie sur le bouton ajouter dans la vue AddMed
        public override void Execute(object parameter)
        {
            Config conf = new Config();
            String connectionString = conf.DbConnectionString;
            MySqlConnection connection = new MySqlConnection(connectionString);
            int idMed = -1;
            bool medExist = true;
            {
                try
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                        connection.Open();
                    String query = "Select idMedic from Medicament where nom = @n";
                    MySqlCommand mySqlCmd = new MySqlCommand(query, connection);
                    
                    mySqlCmd.CommandType = System.Data.CommandType.Text;
                    mySqlCmd.Parameters.AddWithValue("@n", _addMedViewModel.Name);
                    MySqlDataReader reader = mySqlCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        idMed = (int)reader["idMedic"];
                    }
                    if(idMed == -1)
                    {
                        medExist = false;
                    }
                    
                    
                    reader.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
            List<string> names = new List<string>();
            foreach(ModelMedicament medicament1 in _ordonnance.GetAllMedicaments())
            {
                names.Add(medicament1.Name);
            }
            bool medInList = names.AsQueryable().Contains(_addMedViewModel.Name);
            
            if (medExist && !medInList)
            {
                ModelMedicament medicament = new ModelMedicament(idMed, _addMedViewModel.Name, _addMedViewModel.Frequence, _addMedViewModel.Duration, _addMedViewModel.Status, _addMedViewModel.IdOrdo);
                try
                {
                    //Le nouveau "medicament" est ajouté à la liste "_ordonnance"
                    _ordonnance.AddMed(medicament);

                    //Supprimer la MessageBox si jamais cela vous gêne durant les tests
                    MessageBox.Show("Le médicament a été ajouté", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _medicamentViewNavigationService.Navigate();

                }
                catch (Exception)
                {
                    MessageBox.Show("Erreur", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if(!medExist)
            {
                MessageBox.Show("Ce médicament n'existe pas dans la base de données", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (medInList)
            {
                MessageBox.Show("Vous avez déjà ajouté ce médicament à l'ordonnance", "Error",
                       MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Une erreur s'est produite", "Error",
                       MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }
}
