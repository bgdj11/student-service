﻿using CLI.DAO;
using GUI.DTO;
using CLI.Observer;
using CLI.Model;
using GUI.View.Student;
using GUI.View.DialogWindows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using GUI.View.Profesor;
using GUI.View.Predmet;
using System.Xml.Serialization;
using GUI.View.Katedra;
using CLI.Controller;
using System.ComponentModel;
using System.DirectoryServices;
using System.Windows.Controls.Primitives;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IObserver, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public App app;
        private const string SRB = "sr-Latn-RS";
        private const string ENG = "en-US";
        public ObservableCollection<StudentDTO> Students { get; set; }
        public ObservableCollection<ProfesorDTO> Profesors { get; set; }
        public ObservableCollection<PredmetDTO> Subjects { get; set; }

        public ObservableCollection<KatedraDTO> Katedre { get; set; }

        public StudentDTO SelectedStudent { get; set; }
        public ProfesorDTO SelectedProfesor { get; set; }
        public PredmetDTO SelectedPredmet { get; set; }

        public KatedraDTO SelectedKatedra { get; set; }

        //private ProfesorDAO profesorDAO { get; set; }
        //private StudentDAO studentDAO { get; set; }
        //private PredmetDAO predmetDAO { get; set; }
        private KatedraDAO katedraDAO { get; set; }

        private StudentController studentController;
        private ProfesorController profesorController;
        private PredmetController predmetController;
        private AdresaController adresaController;
        private KatedraController katedraController;
        private OcenaController ocenaController;
        private StudentPredmetController studentPredmetController;

        public static RoutedCommand NewCommand = new RoutedCommand();
        public static RoutedCommand SaveCommand = new RoutedCommand();
        public static RoutedCommand CloseCommand = new RoutedCommand();
        public static RoutedCommand EditCommand = new RoutedCommand();
        public static RoutedCommand HelpCommand = new RoutedCommand();
        public static RoutedCommand DeleteCommand = new RoutedCommand();

        private ObservableCollection<StudentDTO> currentFilteredStudents;
        private ObservableCollection<ProfesorDTO> currentFilteredProfesors;
        private ObservableCollection<PredmetDTO> currentFilteredSubjects;



        public int TrenutnaStranica { get; set; } = 1;
        public int UkupnoStranica { get; set; }
        public int StavkiPoStranici { get; set; } = 16;
        private int SelectedPageIndex { get; set; } = 1;



        private object SelectedEntity { get; set; }


        private Dictionary<string, ListSortDirection> columnSortDirections = new Dictionary<string, ListSortDirection>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeStatusBar();

            DataContext = this;


            Students = new ObservableCollection<StudentDTO>();
            studentController = new StudentController();
            studentController.Subscribe(this);

            Profesors = new ObservableCollection<ProfesorDTO>();
            //profesorDAO = new ProfesorDAO();
            profesorController = new ProfesorController();

            Subjects = new ObservableCollection<PredmetDTO>();
            predmetController = new PredmetController();
            Tab.Text = "Studenti";

            Katedre = new ObservableCollection<KatedraDTO>();
            katedraController = new KatedraController();

            studentPredmetController = new StudentPredmetController();

            app = (App)Application.Current;
            app.ChangeLanguage(SRB);

            Update();

            CommandBindings.Add(new CommandBinding(NewCommand, CreateEntityButton_Click));
            CommandBindings.Add(new CommandBinding(SaveCommand, SaveApp));
            CommandBindings.Add(new CommandBinding(CloseCommand, CloseApp_Execution));
            CommandBindings.Add(new CommandBinding(EditCommand, EditEntityButton_Click));
            CommandBindings.Add(new CommandBinding(HelpCommand, OpenAboutWindow));
            CommandBindings.Add(new CommandBinding(DeleteCommand, DeleteEntityButton_Click));

            // Postavljanje Input Gestures
            NewCommand.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            SaveCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            CloseCommand.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control));
            EditCommand.InputGestures.Add(new KeyGesture(Key.E, ModifierKeys.Control));
            HelpCommand.InputGestures.Add(new KeyGesture(Key.H, ModifierKeys.Control));
            DeleteCommand.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Control));

        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void MenuItem_Predmeti_Click()
        {
            MainTabControl.SelectedItem = MainTabControl.Items.Cast<TabItem>().First(tab => tab.Header.Equals("Predmeti"));
            if (app.trenutniJezik == SRB)
                Tab.Text = "Predmeti";
            else
                Tab.Text = "Subjects";
        }
        private void MenuItem_Predmeti_Click(object sender, RoutedEventArgs e)
        {
            MenuItem_Predmeti_Click();
        }
        private void MenuItem_Katedre_Click()
        {
            MainTabControl.SelectedItem = MainTabControl.Items.Cast<TabItem>().First(tab => tab.Header.Equals("Katedre"));
            if (app.trenutniJezik == SRB)
                Tab.Text = "Katedre";
            else
                Tab.Text = "Departments";
        }
        private void MenuItem_Katedre_Click(object sender, RoutedEventArgs e)
        {
            MenuItem_Katedre_Click();
        }
        private void MenuItem_Profesori_Click()
        {
            MainTabControl.SelectedItem = MainTabControl.Items.Cast<TabItem>().First(tab => tab.Header.Equals("Profesori"));
            if (app.trenutniJezik == SRB)
                Tab.Text = " Profesori";
            else
                Tab.Text = "Professors";
        }
        private void MenuItem_Profesori_Click(object sender, RoutedEventArgs e)
        {
            MenuItem_Profesori_Click();
        }
        private void MenuItem_Studenti_Click()
        {
            MainTabControl.SelectedItem = MainTabControl.Items.Cast<TabItem>().First(tab => tab.Header.Equals("Studenti"));
            if (app.trenutniJezik == SRB)
                Tab.Text = "Studenti";
            else
                Tab.Text = "Students";
        }
        private void MenuItem_Studenti_Click(object sender, RoutedEventArgs e)
        {
            MenuItem_Studenti_Click();
        }
        private void AddNewEntity(object sender, RoutedEventArgs e)
        {

        }
        private void SaveApp(object sender, RoutedEventArgs e)
        {
        }
        private void CloseApp_Execution(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void OpenAboutWindow()
        {
            MessageBox.Show("Bogdan Djukic RA98/2021 i Mateja Jovanovic RA160/2021", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void OpenAboutWindow(object sender, RoutedEventArgs e)
        {
            OpenAboutWindow();
        }
        public void Update()
        {
            Profesors.Clear();
            foreach (Profesor profesor in profesorController.GetAllProfesor())
                Profesors.Add(new ProfesorDTO(profesor));

            Students.Clear();
            foreach (Student student in studentController.GetAllStudents())
                Students.Add(new StudentDTO(student));

            Subjects.Clear();
            foreach (Predmet predmet in predmetController.GetAllPredmet())
                Subjects.Add(new PredmetDTO(predmet));

            Katedre.Clear();
            foreach (Katedra k in katedraController.GetAllKatedra())
            {
                Katedre.Add(new KatedraDTO(k));
            }
        }
        private void InitializeStatusBar()
        {
            var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (sender, e) =>
            {
                UpdateDate();
                UpdateTime();
            };
            timer.Start();
        }
        private void UpdateDate()
        {
            if (app.trenutniJezik == SRB)
                statusDate.Text = $"Datum: {DateTime.Now.ToString("yyyy-MM-dd")}";
            else
                statusDate.Text = $"Date: {DateTime.Now.ToString("yyyy-MM-dd")}";
        }

        private void UpdateTime()
        {
            if (app.trenutniJezik == SRB)
                statusTime.Text = $"Vreme: {DateTime.Now.ToString("HH:mm:ss")}";
            else
                statusTime.Text = $"Time: {DateTime.Now.ToString("HH:mm:ss")}";
        }
        private void CreateEntityButton_Click()
        {
            if (MainTabControl.SelectedItem == StudentsTab)
            {
                var addStudentWindow = new AddStudent(studentController);
                addStudentWindow.Owner = this;
                addStudentWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                addStudentWindow.ShowDialog();
                Update();
            }
            else if (MainTabControl.SelectedItem == ProfesorsTab)
            {
                var addProfesorWindow = new AddProfesor(profesorController);
                addProfesorWindow.Owner = this;
                addProfesorWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                addProfesorWindow.ShowDialog();
                Update();
            }
            else if (MainTabControl.SelectedItem == SubjectsTab)
            {
                var addPredmetWindow = new AddPredmet(predmetController);
                addPredmetWindow.Owner = this;
                addPredmetWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                addPredmetWindow.ShowDialog();
                Update();
            }
            else if (MainTabControl.SelectedItem == KatedreTab)
            {
                var addKatedraWindow = new AddKatedra(katedraController);
                addKatedraWindow.Owner = this;
                addKatedraWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                addKatedraWindow.ShowDialog();
                Update();
            }
        }
        private void EditEntityButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainTabControl.SelectedItem == ProfesorsTab)
            {
                if (SelectedProfesor == null)
                {
                    if (app.trenutniJezik == SRB)
                        MessageBox.Show("Niste izabrali nijednog profesora!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show("You did not choose any professor!", "Mistake", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    var editsProfesorWindow = new EditProfesor(profesorController, predmetController, SelectedProfesor.Clone());
                    editsProfesorWindow.Owner = this;
                    editsProfesorWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    editsProfesorWindow.ShowDialog();
                }
            }
            else if (MainTabControl.SelectedItem == SubjectsTab)
            {
                if (SelectedPredmet == null)
                {
                    if (app.trenutniJezik == SRB)
                        MessageBox.Show("Niste izabrali nijedan predmet!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show("You did not choose any subject!", "Mistake", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    var editPredmetWindow = new EditPredmet(predmetController, SelectedPredmet.clone());
                    editPredmetWindow.Owner = this;
                    editPredmetWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    editPredmetWindow.ShowDialog();
                }
            }
            else if (MainTabControl.SelectedItem == StudentsTab)
            {
                if (SelectedStudent == null)
                {
                    if (app.trenutniJezik == SRB)
                        MessageBox.Show("Niste izabrali nijednog studenta!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show("You did not choose any student!", "Mistake", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    var editStudentWIndow = new EditStudent(studentController, SelectedStudent.Clone());
                    editStudentWIndow.Owner = this;
                    editStudentWIndow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    editStudentWIndow.ShowDialog();
                }

            }
            else
            {
                if (SelectedKatedra == null)
                {
                    if (app.trenutniJezik == SRB)
                        MessageBox.Show("Niste izabrali nijednu katedru!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show("You did not choose any department!", "Mistake", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    var editKatedraWindow = new EditKatedra(katedraController, SelectedKatedra.clone(), profesorController);
                    editKatedraWindow.Owner = this;
                    editKatedraWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    editKatedraWindow.ShowDialog();
                }
            }

            Update();
        }
        private void DeleteEntityButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainTabControl.SelectedItem == StudentsTab)
            {
                if (SelectedStudent == null)
                {
                    if (app.trenutniJezik == SRB)
                        MessageBox.Show("Niste izabrali nijednog studenta!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show("You did not choose any student!", "Mistake", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                   
                    var confirmationDialog = new ConfirmationWindow("studenta");
                    confirmationDialog.Owner = this;
                    confirmationDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    confirmationDialog.ShowDialog();

                    if (confirmationDialog.UserConfirmed)
                    {

                        studentController.DeleteStudent(SelectedStudent.StudentId);
                        Students.Remove(SelectedStudent);
                    }
                }
            }
            else if (MainTabControl.SelectedItem == ProfesorsTab)
            {
                if (SelectedProfesor == null)
                {
                    if (app.trenutniJezik == SRB)
                        MessageBox.Show("Niste izabrali nijednog profesora!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show("You did not choose any professor!", "Mistake", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {

                    var confirmationDialog = new ConfirmationWindow("profesora");
                    confirmationDialog.Owner = this;
                    confirmationDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    confirmationDialog.ShowDialog();

                    if (confirmationDialog.UserConfirmed)
                    {
                        profesorController.DeleteProfesor(SelectedProfesor.IdProfesor);
                        Profesors.Remove(SelectedProfesor);
                    }
                }
            }
            else if (MainTabControl.SelectedItem == SubjectsTab)
            {
                if (SelectedPredmet == null)
                {
                    if (app.trenutniJezik == SRB)
                        MessageBox.Show("Niste izabrali nijedan predmet!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show("You did not choose any subject!", "Mistake", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {

                    var confirmationDialog = new ConfirmationWindow("predmet");
                    confirmationDialog.Owner = this;
                    confirmationDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    confirmationDialog.ShowDialog();

                    if (confirmationDialog.UserConfirmed)
                    {
                        predmetController.RemovePredmet(SelectedPredmet.predmetId);
                        Subjects.Remove(SelectedPredmet);
                    }
                }
            }
            else if (MainTabControl.SelectedItem == KatedreTab)
            {
                if (SelectedKatedra == null)
                {
                    if (app.trenutniJezik == SRB)
                        MessageBox.Show("Niste izabrali nijednu katedru!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show("You did not choose any department!", "Mistake", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {

                    var confirmationDialog = new ConfirmationWindow("katedru");
                    confirmationDialog.Owner = this;
                    confirmationDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    confirmationDialog.ShowDialog();

                    if (confirmationDialog.UserConfirmed)
                    {
                        katedraController.RemoveKatedra(SelectedKatedra.katedraId);
                        Katedre.Remove(SelectedKatedra);
                    }
                }
            }

            AzurirajPaginacijuPosleBrisanja();
            Update();
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = searchTextBox.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                currentFilteredStudents = null;
                currentFilteredProfesors = null;
                currentFilteredSubjects = null;
            }

            if (MainTabControl.SelectedItem == StudentsTab)
            {
                currentFilteredStudents = FilterStudent(Students, searchTerm);
                UkupnoStranica = (int)Math.Ceiling(currentFilteredStudents.Count / (double)StavkiPoStranici);
                TrenutnaStranica = 1;
                PostaviEntiteteZaTrenutnuStranicu(currentFilteredStudents);
            }
            else if (MainTabControl.SelectedItem == ProfesorsTab)
            {
                currentFilteredProfesors = FilterProfesor(Profesors, searchTerm);
                UkupnoStranica = (int)Math.Ceiling(currentFilteredProfesors.Count / (double)StavkiPoStranici);
                TrenutnaStranica = 1;
                PostaviEntiteteZaTrenutnuStranicu(currentFilteredProfesors);
            }
            else if (MainTabControl.SelectedItem == SubjectsTab)
            {
                currentFilteredSubjects = FilterSubject(Subjects, searchTerm);
                UkupnoStranica = (int)Math.Ceiling(currentFilteredSubjects.Count / (double)StavkiPoStranici);
                TrenutnaStranica = 1;
                PostaviEntiteteZaTrenutnuStranicu(currentFilteredSubjects);
            }




        }
        private ObservableCollection<StudentDTO> FilterStudent(ObservableCollection<StudentDTO> originalCollection, string searchTerm)
        {
            // Razdvajanje unetog upita na reči i konverzija u mala slova
            var terms = searchTerm.ToLower().Split(',').Select(s => s.Trim()).ToList();

            // Filtriranje na osnovu broja unetih reči
            switch (terms.Count)
            {
                case 1: // Samo prezime
                    return new ObservableCollection<StudentDTO>(
                        originalCollection.Where(studentDto =>
                            studentDto.Prezime.ToLower().Contains(terms[0]))
                    );

                case 2: // Prezime i ime
                    return new ObservableCollection<StudentDTO>(
                        originalCollection.Where(studentDto =>
                            studentDto.Prezime.ToLower().Contains(terms[0]) &&
                            studentDto.Ime.ToLower().Contains(terms[1]))
                    );

                case 3: // Indeks, ime i prezime
                    return new ObservableCollection<StudentDTO>(
                        originalCollection.Where(studentDto =>
                            studentDto.Indeks.ToLower().Contains(terms[0]) &&
                            studentDto.Ime.ToLower().Contains(terms[1]) &&
                            studentDto.Prezime.ToLower().Contains(terms[2]))
                    );

                default:
                    return originalCollection;
            }
        }
        private ObservableCollection<ProfesorDTO> FilterProfesor(ObservableCollection<ProfesorDTO> originalCollection, string searchTerm)
        {
            var terms = searchTerm.ToLower().Split(',').Select(s => s.Trim()).ToList();


            switch (terms.Count)
            {
                case 1: // Samo prezime
                    return new ObservableCollection<ProfesorDTO>(
                        originalCollection.Where(profesorDTO =>
                            profesorDTO.Prezime.ToLower().Contains(terms[0]))
                    );

                case 2: // Prezime i ime
                    return new ObservableCollection<ProfesorDTO>(
                        originalCollection.Where(profesorDTO =>
                            profesorDTO.Prezime.ToLower().Contains(terms[0]) &&
                            profesorDTO.Ime.ToLower().Contains(terms[1]))
                    );

                default:
                    return originalCollection;
            }
        }
        private ObservableCollection<PredmetDTO> FilterSubject(ObservableCollection<PredmetDTO> originalCollection, string searchTerm)
        {
            var terms = searchTerm.ToLower().Split(',').Select(s => s.Trim()).ToList();

            switch (terms.Count())
            {
                case 1: // Samo sifra predmeta
                    return new ObservableCollection<PredmetDTO>(
                        originalCollection.Where(s => s.SifraPredmeta.ToLower().Contains(terms[0]))
                        );

                case 2: // Sifra i naziv predmeta
                    return new ObservableCollection<PredmetDTO>(
                        originalCollection.Where(s => s.SifraPredmeta.ToLower().Contains(terms[0]) &&
                        s.NazivPredmeta.ToLower().Contains(terms[1]))
                        );

                default:
                    return originalCollection;
            }
        }
        private void CreateEntityButton_Click(object sender, RoutedEventArgs e)
        {
            CreateEntityButton_Click();
        }

        private void StudentsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Tab.Text = "Status: Studenti";
        }
        private void ProfesorsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Tab.Text = "Status: Profesori";
        }
        private void SubjectsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Tab.Text = "Status: Predmeti";
        }

        private void KatedreDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Tab.Text = "Status: Katedre";
        }

        private void TabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedItem == StudentsTab)
            {
                if (app.trenutniJezik == SRB)
                    Tab.Text = "Studenti";
                else
                    Tab.Text = "Students";
            }
            else if (MainTabControl.SelectedItem == ProfesorsTab)
            {
                if (app.trenutniJezik == SRB)
                    Tab.Text = " Profesori";
                else
                    Tab.Text = "Professors";
            }
            else if (MainTabControl.SelectedItem == SubjectsTab)
            {
                if (app.trenutniJezik == SRB)
                    Tab.Text = "Predmeti";
                else
                    Tab.Text = "Subjects";
            }
            else if (MainTabControl.SelectedItem == KatedreTab)
                if (app.trenutniJezik == SRB)
                    Tab.Text = "Katedre";
                else
                    Tab.Text = "Departments";

            IzracunajPaginaciju();
        }

        private void IzracunajPaginaciju()
        {
            //TrenutnaStranica = 1;

            int ukupnoEntiteta = 0;

            if (MainTabControl.SelectedItem == StudentsTab)
            {
                ukupnoEntiteta = Students.Count;
                UkupnoStranica = (int)Math.Ceiling(ukupnoEntiteta / (double)StavkiPoStranici);
                PostaviEntiteteZaTrenutnuStranicu(Students);
            }
            else if (MainTabControl.SelectedItem == ProfesorsTab)
            {
                ukupnoEntiteta = Profesors.Count;
                UkupnoStranica = (int)Math.Ceiling(ukupnoEntiteta / (double)StavkiPoStranici);
                PostaviEntiteteZaTrenutnuStranicu(Profesors);
            }
            else
            {
                ukupnoEntiteta = Subjects.Count;
                UkupnoStranica = (int)Math.Ceiling(ukupnoEntiteta / (double)StavkiPoStranici);
                PostaviEntiteteZaTrenutnuStranicu(Subjects);
            }


        }

        private void PostaviEntiteteZaTrenutnuStranicu<T>(ObservableCollection<T> entiteti)
        {
            int startIndex = (TrenutnaStranica - 1) * StavkiPoStranici;
            var filtriraniEntiteti = entiteti.Skip(startIndex).Take(StavkiPoStranici);

            if (MainTabControl.SelectedItem == StudentsTab)
            {
                StudentsDataGrid.ItemsSource = new ObservableCollection<StudentDTO>(filtriraniEntiteti.Cast<StudentDTO>());
            }
            else if (MainTabControl.SelectedItem == ProfesorsTab)
            {
                ProfesorsDataGrid.ItemsSource = new ObservableCollection<ProfesorDTO>(filtriraniEntiteti.Cast<ProfesorDTO>());
            }
            else if (MainTabControl.SelectedItem == SubjectsTab)
            {
                SubjectsDataGrid.ItemsSource = new ObservableCollection<PredmetDTO>(filtriraniEntiteti.Cast<PredmetDTO>());
            }
        }

        public void SledecaStranica()
        {
            if (TrenutnaStranica < UkupnoStranica)
            {
                TrenutnaStranica++;
                ResetSelectedEntities();
                OsveziTrenutnuStranicu();
            }
        }

        public void PrethodnaStranica()
        {
            if (TrenutnaStranica > 1)
            {
                TrenutnaStranica--;
                ResetSelectedEntities();
                OsveziTrenutnuStranicu();
            }
        }
        private void OsveziTrenutnuStranicu()

        {
            ResetSelectedEntities();

            if (MainTabControl.SelectedItem == StudentsTab)
            {
                PostaviEntiteteZaTrenutnuStranicu(Students);
            }
            else if (MainTabControl.SelectedItem == ProfesorsTab)
            {
                PostaviEntiteteZaTrenutnuStranicu(Profesors);
            }
            else if (MainTabControl.SelectedItem == SubjectsTab)
            {
                PostaviEntiteteZaTrenutnuStranicu(Subjects);
            }
        }
        private void PrethodnaStranica_Click(object sender, RoutedEventArgs e)
        {
            PrethodnaStranica();
        }

        private void NarednaStranica_Click(object sender, RoutedEventArgs e)
        {
            SledecaStranica();
        }

        private void ResetSelectedEntities()
        {
            SelectedStudent = null;
            SelectedProfesor = null;
            SelectedPredmet = null;
            SelectedKatedra = null;

        }
        private void AzurirajPaginacijuPosleBrisanja()
        {
            UkupnoStranica = (int)Math.Ceiling(VratiTrenutniBrojElemenata() / (double)StavkiPoStranici);

            if (TrenutnaStranica > UkupnoStranica)
            {
                TrenutnaStranica = Math.Max(1, UkupnoStranica);
            }

            OsveziTrenutnuStranicu();
        }
        private int VratiTrenutniBrojElemenata()
        {
            if (MainTabControl.SelectedItem == StudentsTab)
            {
                return Students.Count;
            }
            // Dodajte logiku za ostale tabove...

            return 0;
        }
        private void Serbian_Click(object sender, RoutedEventArgs e)
        {
            app.ChangeLanguage(SRB);
            Serbian_Tabs();
        }

        private void English_Click(object sender, RoutedEventArgs e)
        {
            app.ChangeLanguage(ENG);
            English_Tabs();
        }
        public void English_Tabs()
        {
            if (MainTabControl.SelectedItem == StudentsTab)
                Tab.Text = "Students";
            else if (MainTabControl.SelectedItem == SubjectsTab)
                Tab.Text = "Subjects";
            else if (MainTabControl.SelectedItem == ProfesorsTab)
                Tab.Text = "Professors";
            else
                Tab.Text = "Departments";
        }
        public void Serbian_Tabs()
        {
            if (MainTabControl.SelectedItem == StudentsTab)
                Tab.Text = "Studenti";
            else if (MainTabControl.SelectedItem == SubjectsTab)
                Tab.Text = "Predmeti";
            else if (MainTabControl.SelectedItem == ProfesorsTab)
                Tab.Text = "Profesori";
            else
                Tab.Text = "Katedre";
        }

        private void StudentsDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null || e.Column == null) return;

            e.Handled = true;

            var propertyName = e.Column.SortMemberPath;

            // Provera da li je kolona već sortirana i menjanje smera sortiranja
            if (!columnSortDirections.TryGetValue(propertyName, out var currentDirection))
            {
                currentDirection = ListSortDirection.Ascending;
            }
            else
            {
                currentDirection = currentDirection == ListSortDirection.Ascending ?
                                   ListSortDirection.Descending : ListSortDirection.Ascending;
            }

            // Ažuriranje rečnika sa novim smerom sortiranja
            columnSortDirections[propertyName] = currentDirection;

            e.Column.SortDirection = currentDirection;

            SortStudentsWithPagination(propertyName, currentDirection);
        }

        private void SortStudentsWithPagination(string sortBy, ListSortDirection direction)
        {
            var collectionToSort = currentFilteredStudents != null && currentFilteredStudents.Any()
                ? currentFilteredStudents
                : Students;

            var sortedStudents = direction == ListSortDirection.Ascending
                ? collectionToSort.OrderBy(x => GetPropertyValue(x, sortBy))
                : collectionToSort.OrderByDescending(x => GetPropertyValue(x, sortBy));

            // Apply sorted data to either filtered collection or main collection
            if (currentFilteredStudents != null)
            {
                currentFilteredStudents = new ObservableCollection<StudentDTO>(sortedStudents);
                PostaviEntiteteZaTrenutnuStranicu(currentFilteredStudents);
            }
            else
            {
                Students = new ObservableCollection<StudentDTO>(sortedStudents);
                PostaviEntiteteZaTrenutnuStranicu(Students);
            }
        }

        private object GetPropertyValue(object obj, string propName)
        {
            return obj.GetType().GetProperty(propName)?.GetValue(obj, null);
        }

        private void SubjectsDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null || e.Column == null) return;

            e.Handled = true;

            var propertyName = e.Column.SortMemberPath;

            if (!columnSortDirections.TryGetValue(propertyName, out var currentDirection))
            {
                currentDirection = ListSortDirection.Ascending;
            }
            else
            {
                currentDirection = currentDirection == ListSortDirection.Ascending ?
                                   ListSortDirection.Descending : ListSortDirection.Ascending;
            }

            columnSortDirections[propertyName] = currentDirection;

            e.Column.SortDirection = currentDirection;

            SortSubjectsWithPagination(propertyName, currentDirection);
        }

        private void SortSubjectsWithPagination(string sortBy, ListSortDirection direction)
        {
            var collectionToSort = currentFilteredSubjects != null && currentFilteredSubjects.Any()
                ? currentFilteredSubjects
                : Subjects;

            var sortedSubjects = direction == ListSortDirection.Ascending
                ? collectionToSort.OrderBy(x => GetPropertyValue(x, sortBy))
                : collectionToSort.OrderByDescending(x => GetPropertyValue(x, sortBy));

            // Apply sorted data to either filtered collection or main collection
            if (currentFilteredSubjects != null)
            {
                currentFilteredSubjects = new ObservableCollection<PredmetDTO>(sortedSubjects);
                PostaviEntiteteZaTrenutnuStranicu(currentFilteredSubjects);
            }
            else
            {
                Subjects = new ObservableCollection<PredmetDTO>(sortedSubjects);
                PostaviEntiteteZaTrenutnuStranicu(Subjects);
            }
        }


        private void ProfesorsDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null || e.Column == null) return;

            e.Handled = true;

            var propertyName = e.Column.SortMemberPath;

            if (!columnSortDirections.TryGetValue(propertyName, out var currentDirection))
            {
                currentDirection = ListSortDirection.Ascending;
            }
            else
            {
                currentDirection = currentDirection == ListSortDirection.Ascending ?
                                   ListSortDirection.Descending : ListSortDirection.Ascending;
            }

            columnSortDirections[propertyName] = currentDirection;

            e.Column.SortDirection = currentDirection;

            SortProfessorsWithPagination(propertyName, currentDirection);
        }

        private void SortProfessorsWithPagination(string sortBy, ListSortDirection direction)
        {
            var collectionToSort = currentFilteredProfesors != null && currentFilteredProfesors.Any()
                ? currentFilteredProfesors
                : Profesors;

            var sortedProfessors = direction == ListSortDirection.Ascending
                ? collectionToSort.OrderBy(x => GetPropertyValue(x, sortBy))
                : collectionToSort.OrderByDescending(x => GetPropertyValue(x, sortBy));

            // Apply sorted data to either filtered collection or main collection
            if (currentFilteredProfesors != null)
            {
                currentFilteredProfesors = new ObservableCollection<ProfesorDTO>(sortedProfessors);
                PostaviEntiteteZaTrenutnuStranicu(currentFilteredProfesors);
            }
            else
            {
                Profesors = new ObservableCollection<ProfesorDTO>(sortedProfessors);
                PostaviEntiteteZaTrenutnuStranicu(Profesors);
            }
        }
    }
}
