﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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


using CLI.DAO;
using CLI.Model;
using GUI.DTO;
using GUI.View.DialogWindows;

namespace GUI.View.Student
{
    /// <summary>
    /// Interaction logic for EditStudent.xaml
    /// </summary>
    public partial class EditStudent : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public StudentDTO Student { get; set; }
        private StudentDAO studentDAO;

        public ObservableCollection<PredmetDTO> NotPassedSubjects { get; set; }  // lista nepolozenih
        public ObservableCollection<OcenaDTO> Ocene { get; set; }

        public ObservableCollection<ProfesorPredmetDTO> Profesori { get; set; }

        public ProfesorPredmetDTO SelectedProfesorPredmet { get; set; } 

        private ProfesorDAO profesorDAO;
        public OcenaDTO SelectedOcena { get; set; }
        private OcenaNaUpisuDAO ocenaDAO;

        public PredmetDTO SelectedSubject { get; set; } // selektovan nepolozen
        private PredmetDAO predmetDAO;

        private StudentPredmetDAO studentPredmetDAO;

        List<int> Godine;
        List<string> Statusi;

        public EditStudent(StudentDAO studentDAO, StudentDTO selectedStudent)
        {
            InitializeComponent();
            DataContext = this;
            this.studentDAO = studentDAO;
            Student = selectedStudent;

            NotPassedSubjects = new ObservableCollection<PredmetDTO>();
            predmetDAO = new PredmetDAO();
            SelectedSubject = new PredmetDTO();

            Ocene = new ObservableCollection<OcenaDTO>();
            ocenaDAO = new OcenaNaUpisuDAO();
            SelectedOcena = new OcenaDTO();

            studentPredmetDAO = new StudentPredmetDAO();

            Godine = new List<int> { 1, 2, 3, 4 };
            cmbGodinaStudija.ItemsSource = Godine;

            Statusi = new List<string> { "samofinasiranje", "budzet" };
            cmbStatusStudenta.ItemsSource = Statusi;

            profesorDAO = new ProfesorDAO();
            Profesori = new ObservableCollection<ProfesorPredmetDTO>();

            SelectedProfesorPredmet = new ProfesorPredmetDTO();

            Update();
        }

        public void Update()
        {
            studentDAO.MakeStudent();

            if (Student.NotPassedIds != null)
            {
                NotPassedSubjects.Clear();
                foreach (int i in Student.NotPassedIds)
                {
                    NotPassedSubjects.Add(new PredmetDTO(predmetDAO.GetPredmetById(i)));
                }
            }


            //if (Student.gradesIds == null || Student.gradesIds.Count == 0)
            //{
             //   MessageBox.Show("Nema ID-ova ocena za proveru.", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
           // }

            if (Student.GradesIds != null)
            {
                Ocene.Clear();
                foreach (int i in Student.GradesIds)
                {

                    if (ocenaDAO.GetOcenaById(i) != null)
                    {
                       // MessageBox.Show("nasao je ocenuuu");
                        Ocene.Add(new OcenaDTO(ocenaDAO.GetOcenaById(i)));
                    }
                }

            }

            //ProveriOcene();

            Student.UkupnoEspb = izracunajEspb();
            Student.ProsecnaOcena = izracunajProsecnuOcenu();


            foreach(int ids in Student.NotPassedIds)
            {
                CLI.Model.Predmet pr = predmetDAO.GetPredmetById(ids);
                int idProf = pr.IdProfesora;

                CLI.Model.Profesor prof = profesorDAO.GetProfesorById(idProf);

                if(prof != null)
                    Profesori.Add(new ProfesorPredmetDTO(prof, pr));
            }

        }

        public int izracunajEspb()
        {
            int espb = 0;
            foreach (int i in Student.GradesIds)
            {
                espb += ocenaDAO.GetOcenaById(i).Predmet.BrojESPB;
            }


            return espb;
        }


        public double izracunajProsecnuOcenu()
        {
            double suma = 0;
            int count = 0;
            foreach (int i in Student.GradesIds)
            {
                suma += ocenaDAO.GetOcenaById(i).Ocena;
                ++count;
            }
            if (count == 0)
            {
                return 5;
            }
            else return Math.Round(suma / count, 2);
        }

        protected virtual void OnPrortyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }


        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
                CLI.Model.Student sr = Student.toStudent();
                sr.IdStudent = Student.StudentId;

                sr.IdAdrese = Student.idAdrese;
                sr.AdresaStanovanja.IdAdrese = Student.idAdrese;
                
                sr.IdIndeksa = Student.idIndeksa;
                sr.Indeks.idIndeksa = Student.idIndeksa;

                

                studentDAO.UpdateStudent(sr);
                MessageBox.Show("Student je uspesno promenjen!", "Uspesno", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Popunite sva polja pre potvrde", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool ValidateFields()
        {
            return !string.IsNullOrWhiteSpace(txtBoxIme.Text) &&
                   !string.IsNullOrWhiteSpace(txtBoxPrezime.Text) &&
                   !string.IsNullOrWhiteSpace(datpDatumRodjenja.Text) &&
                   !string.IsNullOrWhiteSpace(txtBoxUlica.Text) &&
                   !string.IsNullOrWhiteSpace(txtBoxBroj.Text) &&
                   !string.IsNullOrWhiteSpace(txtBoxGrad.Text) &&
                   !string.IsNullOrWhiteSpace(txtBoxDrzava.Text) &&
                   !string.IsNullOrWhiteSpace(txtBoxKontakt.Text) &&
                   !string.IsNullOrWhiteSpace(txtBoxEmail.Text) &&
                   !string.IsNullOrWhiteSpace(txtBoxOznakaSmera.Text) &&
                   !string.IsNullOrWhiteSpace(txtBoxBrojIndeksa.Text) &&
                   !string.IsNullOrWhiteSpace(txtBoxGodinaUpisa.Text) &&
                   cmbGodinaStudija.SelectedItem != null &&
                   cmbStatusStudenta.SelectedItem != null;
                   //!string.IsNullOrWhiteSpace(txtBoxProsecnaOcena.Text);
        }

        private void AddSubject_Click(object sender, RoutedEventArgs e)
        {
            var selectSubjectWindow  = new SelectSubject(predmetDAO, Student, studentPredmetDAO);
            selectSubjectWindow.Owner = this;
            selectSubjectWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            selectSubjectWindow.ShowDialog();
            Update();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedSubject == null)
            {
                MessageBox.Show(this, "Izaberite predmet za brisanje!");
            }
            else
            {
                var confirmationDialog = new ConfirmationWindow("predmet");
                confirmationDialog.Owner = this;
                confirmationDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                confirmationDialog.ShowDialog();

                if (confirmationDialog.UserConfirmed)
                {
                    studentPredmetDAO.RemovePredmetFromStudent(studentPredmetDAO.GetByIds(Student.StudentId, SelectedSubject.predmetId));
                }

                // da bi se izmene odmah prikazale:
                Student.NotPassedIds.Remove(SelectedSubject.predmetId);
            }

            Update();
        }
    
        private void AddGrade_Click(object sender, RoutedEventArgs e) 
        {
            if (SelectedSubject == null)
            {
                MessageBox.Show(this, "Izaberite predmet!");
            }
            else
            {
                var addGrade = new AddGrade(SelectedSubject, Student, ocenaDAO);
                addGrade.Owner = this;
                addGrade.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                addGrade.ShowDialog();
            }

            Update();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void PonistiOcenu_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedSubject == null)
            {
                MessageBox.Show(this, "Izaberite predmet za brisanje!");
            }
            else
            {
                var confirmationDialog = new ConfirmationWindow("ocena");
                confirmationDialog.Owner = this;
                confirmationDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                confirmationDialog.ShowDialog();

                if (confirmationDialog.UserConfirmed)
                {
                    ocenaDAO.RemoveOcena(SelectedOcena.idOcene);
                }

                Student.NotPassedIds.Add(SelectedOcena.IdPredmeta);
                Student.GradesIds.Remove(SelectedOcena.idOcene);
                Student.PassedIds.Remove(SelectedOcena.IdPredmeta);

            }

            Update();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
           // Implementirati !
        }

    }
}