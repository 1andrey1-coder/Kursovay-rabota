﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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

namespace schedule
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public TblGroup SelectedGroup { get; set; }

        public TblGroup SelectedGroup2 { get => selectedGroup2;
            set { selectedGroup2 = value;
                var db = new ScheduleDbContext();
                TblScheduleDb = db.TblScheduleDbs.Where(s => s.Groupid == SelectedGroup2.GroupId).ToList();
            }  }

        public TblGroup grud { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        //таблица 1
        public List<TblGroup> Group { get; set; }

        public List<TblReplacement> Replacement { get => replacement1; set { replacement1 = value; Fill();  } }
        private List<TblReplacement> replacement1;


        //таблица 2
        public List<TblGroup> Group2 { get; set; }
        public List<TblScheduleDb> TblScheduleDb { get => TblScheduleDb1; set { TblScheduleDb1 = value; Fill(); } }
        private List<TblScheduleDb> TblScheduleDb1;



        public MainWindow()
        {

            InitializeComponent();
            DataContext = this;

            Search();

            Replacement = DB.GetInstance().TblReplacements.ToList();
            TblScheduleDb = DB.GetInstance().TblScheduleDbs.ToList();
            //listschedule2.ItemsSource = Replacement;


            FillGroup();
            var user = DB.GetInstance().TblGroups.Include(s => s.SemestrNuber).Include(s => s.SemestrWeek);
            Group = DB.GetInstance().TblGroups.ToList();
            Group2 = DB.GetInstance().TblGroups.ToList();
            Combobox.ItemsSource = Group;
            Combobox2.ItemsSource = Group2;

          

            grud = new TblGroup();
            grud = SelectedGroup;
        }
        //поиск
        void Signal(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        private string searchText = "";
        private TblGroup selectedGroup2;

        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                Search();
            }
        }
        private void Search()
        {
            var db = new ScheduleDbContext();
            var result = db.TblReplacements.Where(s =>
                    s.Name.Contains(searchText) /*||*/
                );
            Replacement = result.ToList();
            Signal(nameof(Replacement));

        }







        private void replacement(object sender, RoutedEventArgs e)
        {
            Replacement replacements = new Replacement();
            replacements.ShowDialog();
            Obnov(sender, e);
        }


        private void FillGroup()
        {
            Group = new List<TblGroup>();
            Group2 = new List<TblGroup>();

            Group.AddRange(DB.GetInstance().TblGroups
                .Include(S => S.Speciality)
                .Include(S => S.SemestrNuber)
                .Include(S => S.Course)
                .Include(S => S.SemestrWeek).ToList());

           

            SelectedGroup = Group.FirstOrDefault();
        }


        

        private void Fill([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        //private void perexod(object sender, MouseButtonEventArgs e)
        //{


        //}


        private void Obnov(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Обновить таблицу?", "Потверждение", MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                using (var db = new ScheduleDbContext())
                {
                    Replacement = DB.GetInstance().TblReplacements.ToList();
                    db.SaveChanges();
                }
            }


        }

        
        private void perexod(object sender, RoutedEventArgs e)
        {

            var result = MessageBox.Show("Это та группа?", "Потверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                RedactorSivo redactorSivos = new RedactorSivo(SelectedGroup);
                redactorSivos.ShowDialog();
                
            }
        }
    }
}
