﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CLI.DAO;
using CLI.Model;

namespace CLI.Console
{
    class MainMenu
    {
        public MainMenu() 
        {
            
        }

        public void RunMenu()
        {

            while (true)
            {
                ShowMenu();
                string userInput = System.Console.ReadLine() ?? "0";
                if (userInput == "0") break;
                HandleMenu(userInput);
            }
        }
        private void ShowMenu()
        {
            System.Console.WriteLine("\nChoose an option: ");
            System.Console.WriteLine("1: Studenti");
            System.Console.WriteLine("2: Predmeti");
            System.Console.WriteLine("3: Profesori");
            System.Console.WriteLine("4: Katedre");
            System.Console.WriteLine("5: Ocene");
            
            System.Console.WriteLine("0: Close");
        }



        private void HandleMenu(string input)
        {
            StudentDAO student = new StudentDAO();
            PredmetDAO predmet = new PredmetDAO();
            ProfesorDAO profesor = new ProfesorDAO();
            KatedraDAO katedra = new KatedraDAO();
            AdresaDAO adresa = new AdresaDAO();
            IndeksDAO indeks = new IndeksDAO();
            OcenaNaUpisuDAO ocena = new OcenaNaUpisuDAO();
            StudentPredmetDAO studentpredmet = new StudentPredmetDAO();

            switch (input)
            {
                case "1":
                    StudentConsoleView studentView = new StudentConsoleView(student, studentpredmet, adresa, indeks);
                    studentView.RunMenu();
                    break;
                case "2":
                    PredmetConsoleView predmetView = new PredmetConsoleView(predmet);
                    predmetView.RunMenu();
                    break;
                case "3":
                    ProfesorConsoleView profesorView = new ProfesorConsoleView(profesor, adresa);
                    profesorView.RunMenu();
                    break;
                case "4":
                    KatedraConsoleView katedraView = new KatedraConsoleView(katedra);
                    katedraView.RunMenu();
                    break;
                case "5":
                    OcenaNaUpisuConsoleView ocenaView = new OcenaNaUpisuConsoleView(ocena);
                    ocenaView.RunMenu();
                    break;
                case "0":
                    break;
            }

        }
           

    }
}
