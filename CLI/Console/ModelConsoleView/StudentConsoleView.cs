﻿using CLI.DAO;
using CLI.Model;

namespace CLI.Console
{
class StudentConsoleView
{
        private readonly StudentDAO _studentsDao;

        public StudentConsoleView(StudentDAO studentsDao)
        {
            _studentsDao = studentsDao;
        }

        private void PrintStudents(List<Student> students)
        {
            foreach (Student student in students)
            {
                System.Console.WriteLine(student.ToString());
            }
        }

        private Student InputStudent()
        {
            System.Console.WriteLine("Unesite prezime studenta: ");
            string prezime = System.Console.ReadLine() ?? string.Empty;

            System.Console.WriteLine("Unesite ime studenta: ");
            string ime = System.Console.ReadLine() ?? string.Empty;

            DateOnly datumRodjenja;
            while (true)
            {
                System.Console.WriteLine("Unesite datum rođenja (yyyy-MM-dd): ");
                if (DateOnly.TryParse(System.Console.ReadLine(), out datumRodjenja))
                {
                    break; 
                }
                else
                {
                    System.Console.WriteLine("Pogrešan format datuma. Ponovite unos.");
                }
            }

            System.Console.WriteLine("Unesite kontakt telefon: ");
            string kontaktTelefon = System.Console.ReadLine() ?? string.Empty;

            System.Console.WriteLine("Unesite email adresu: ");
            string emailAdresa = System.Console.ReadLine() ?? string.Empty;

            System.Console.WriteLine("Unesite trenutnu godinu studija: ");
            int trenutnaGodinaStudija = ConsoleViewUtils.SafeInputInt();

            Status status;
            while (true)
            {
                System.Console.WriteLine("Unesite status studenta (B/S): ");
                if (Enum.TryParse(System.Console.ReadLine(), out status))
                {
                    break; 
                }
                else
                {
                    System.Console.WriteLine("Pogrešan unos statusa. Ponovite unos.");
                }
            }

            AdresaDAO _adresa = new AdresaDAO();
            AdresaConsoleView _adresaView = new AdresaConsoleView(_adresa);
            Adresa a = _adresaView.InputAdresa();
            _adresa.AddAdresa(a);

            int idAdrese = a.idAdrese;


            IndeksDAO _indeks = new IndeksDAO();
            IndeksConsoleView _indeksConsoleView = new IndeksConsoleView(_indeks);
            Indeks i = _indeksConsoleView.InputIndeks();
            _indeks.AddIndeks(i);

            int idIndeksa = i.idIndeksa;

            System.Console.WriteLine("Unesite prosečnu ocenu: ");
            if (double.TryParse(System.Console.ReadLine(), out double prosecnaOcena))
            {
            
            }


            return new Student(prezime, ime, datumRodjenja, idAdrese, kontaktTelefon, emailAdresa, trenutnaGodinaStudija, status, prosecnaOcena, a, idIndeksa, i);
        }

        private int InputStudentId()
        {
            System.Console.WriteLine("Enter student id: ");
            return ConsoleViewUtils.SafeInputInt();
        }

        public void RunMenu()
        {
            while (true)
            {
                ShowMenu();
                string userInput = System.Console.ReadLine() ?? "0";
                if (userInput == "0") break;
                HandleMenuInput(userInput);
            }
        }

        private void ShowMenu()
        {
            System.Console.WriteLine("\nChoose an option: ");
            System.Console.WriteLine("1: Show All students");
            System.Console.WriteLine("2: Add student");
            System.Console.WriteLine("3: Update student");
            System.Console.WriteLine("4: Remove student");
            System.Console.WriteLine("5: Show and sort students");
            System.Console.WriteLine("0: Close");
        }

        private void HandleMenuInput(string input)
        {
            switch (input)
            {
                case "1":
                    ShowAllStudents();
                    break;
                case "2":
                    AddStudent();
                    break;
                case "3":
                    UpdateStudent();
                    break;
                case "4":
                    RemoveStudent();
                    break;
                case "5":
                    ShowAndSortStudents();
                    break;
            }
        }

        private void ShowAllStudents()
        {
            PrintStudents(_studentsDao.GetAllStudents());
        }


        private void RemoveStudent()
        {
            int id = InputStudentId();
            Student? removedStudent = _studentsDao.RemoveStudent(id);
            if (removedStudent is null)
            {
                System.Console.WriteLine("Student nije pronadjen");
                return;
            }

            System.Console.WriteLine("Student izbrisan");
        }

        private void UpdateStudent()
        {
            int id = InputStudentId();
            Student student = InputStudent();
            student.idStudent = id;
            Student? updatedStudent = _studentsDao.UpdateStudent(student);
            if (updatedStudent is null)
            {
                System.Console.WriteLine("Student not found");
                return;
            }

            System.Console.WriteLine("Student updated");
        }

        private void AddStudent()
        {
            Student student = InputStudent();
            _studentsDao.AddStudent(student);
            System.Console.WriteLine("Student added");
        }

        private void ShowAndSortStudents()
        {
            System.Console.WriteLine("\nEnter page: ");
            int page = ConsoleViewUtils.SafeInputInt();
            System.Console.WriteLine("\nEnter page size: ");
            int pageSize = ConsoleViewUtils.SafeInputInt();
            System.Console.WriteLine("\nEnter sort criteria: ");
            string sortCriteria = System.Console.ReadLine() ?? string.Empty;
            System.Console.WriteLine("\nEnter 0 for ascending, any key for descending: ");
            int sortDirectionInput = ConsoleViewUtils.SafeInputInt();
            SortDirection sortDirection = sortDirectionInput == 0 ? SortDirection.Ascending : SortDirection.Descending;

            PrintStudents(_studentsDao.GetAllStudents(page, pageSize, sortCriteria, sortDirection));
        }

    }
}
