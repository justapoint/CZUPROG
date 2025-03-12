using System;            // Základní funkce jazyka C# (vstup/výstup, konzole, výjimky atd.).
using System.Collections.Generic; // Práce se seznamy, množinami, slovníky a dalšími kolekcemi.
using System.IO;         // Práce se soubory a streamy.
using System.Text.RegularExpressions; // Používá se pro práci s regulárními výrazy.
using Newtonsoft.Json;   // Serializace a deserializace JSON dat.


class CinemaHall
{
    public int Width { get; set; }
    public int Height { get; set; }
    public List <string> ReservedSeats { get; set; } = new List <string> ();
}

class Program
{
    static string dataFile = "data.json";
    static Dictionary <string, CinemaHall> cinemaHalls = new Dictionary <string, CinemaHall> ();

    static void Main()
    {
        LoadData();
        MainMenu();
    }

    static void LoadData() // Načte data ze souboru JSON a deserializuje je do slovníku cinemaHalls.
    {
        if (File.Exists(dataFile))
        {
            var json = File.ReadAllText(dataFile);
            cinemaHalls = JsonConvert.DeserializeObject <Dictionary <string, CinemaHall >> (json) ?? new Dictionary <string, CinemaHall> ();
        }
    }

    static void SaveData() // Serializuje slovník cinemaHalls do formátu JSON a uloží ho do souboru.

    {
        var json = JsonConvert.SerializeObject(cinemaHalls, Formatting.Indented);
        File.WriteAllText(dataFile, json);
    }

    static void MainMenu() // Hlavní menu programu, které umožňuje uživateli interakci s aplikací.
    {
        Console.Clear();
        Console.WriteLine("Welcome to the Cinema Reservation System");
        Console.WriteLine();

        if (cinemaHalls.Count == 0) // Pokud nejsou nalezeny žádné existující sály, nebude možné je otevřít.
        {
            Console.WriteLine("1. Create new cinema hall");
            Console.WriteLine("2. Exit");

            string choice = Console.ReadLine();

            if (choice == "1") CreateNewHall();
            else Environment.Exit(0); // Vstupy jakýchkoli symbolů, kromě povolených čísel, program považuje za ne.
        }
        else
        {
            Console.WriteLine("1. Interact with existing cinema halls");
            Console.WriteLine("2. Create new cinema hall");
            Console.WriteLine("3. Exit");

            Console.WriteLine();
            Console.Write("Select an option: ");
            string choice = Console.ReadLine();

            if (choice == "1") InteractWithExistingHalls();
            else if (choice == "2") CreateNewHall();
            else Environment.Exit(0);
        }
    }

    static void CreateNewHall() // Vytvoření nového sála a přidání do seznamu.
    {
        Console.Clear();
        Console.WriteLine("Enter cinema hall size (Width Max 26, Height Max 30)");

        int width = 0, height = 0;

        while (true)
        {
            Console.Write("Width (1-26): "); // Maximální počet míst je roven počtu písmen v latinské abecedě.

            string input = Console.ReadLine();
            if (int.TryParse(input, out width) && width > 0 && width <= 26) // int.TryParse zkontroluje, zda je vstup číslo, a zabrání pádu programu při zadání písmen.
            {
                break;
            }

            Console.WriteLine("Invalid input. Try again.");
        }

        while (true)
        {
            Console.Write("Height (1-30): ");

            string input = Console.ReadLine();
            if (int.TryParse(input, out height) && height > 0 && height <= 30)
            {
                break;
            }

            Console.WriteLine("Invalid input. Try again.");
        }


        CinemaHall newHall = new CinemaHall { Width = width, Height = height };

        while (true)
        {
            DrawHall(newHall);

            Console.WriteLine("\n1. Save hall");
            Console.WriteLine("2. Exit");

            Console.WriteLine();
            Console.Write("Select an option: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Enter hall name: ");

                string hallName = Console.ReadLine();
                cinemaHalls[hallName] = newHall;
                SaveData();
                break;
            }
            else
            {
                Console.Write("Are you sure you want to exit without saving? (yes/no): ");

                string confirm = Console.ReadLine();

                if (confirm.ToLower() == "yes")
                {
                    MainMenu();
                    break;
                }
                else
                {
                    continue;
                }
            }

            Console.Clear();
        }

    }

    static void DrawHall(CinemaHall hall) // Vykreslení sálů.
    {
        Console.Clear();

        for (int i = 1; i <= hall.Height; i++)
        {
            Console.Write(i.ToString().PadRight(3)); // Výstup čísla s minimální šířkou 3 znaky, zarovnaný doleva.

            for (int j = 1; j <= hall.Width; j++)
            {
                string seatId = $"{(char)(j + 96)}{i}"; // Vytvoření ID sedadla ve formátu "a1".
                char seat = (hall.ReservedSeats.Contains(seatId)) ? 'X' : '#'; // Kontrola rezervace sedadla.

                Console.ForegroundColor = (seat == 'X') ? ConsoleColor.Red : ConsoleColor.Green; // Nastavení barvy textu.
                Console.Write($"{seat} ");
                Console.ResetColor();

                if (j == hall.Width / 2) // Vytvoření chodby uprostřed sálů pro rozdělení sedadel.
                {
                    Console.Write("\t"); 
                }

            }
            Console.WriteLine();
        }

        Console.Write("   "); // Posun písmen pro sladění s rozložením sedadel  

        for (int i = 1; i <= hall.Width; i++)  // Vykreslení sedadel 
        {
            Console.Write($"{(char)(i + 96)} ");

            if (i == hall.Width / 2)
            {
                Console.Write("\t");
            }
        }

        Console.WriteLine();
    }

    static void InteractWithExistingHalls() // Práce s existujícími sály.
    {
        int choice = -1;
        string hallName;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Select a cinema hall:");
            Console.WriteLine();

            int index = 1;

            foreach (var hall in cinemaHalls) // Výpis všech existujících sálů.
            {
                int totalSeats = hall.Value.Width * hall.Value.Height;
                int reservedSeats = hall.Value.ReservedSeats.Count;

                Console.WriteLine($"{index}. {hall.Key} [{hall.Value.Width}x{hall.Value.Height}] - {totalSeats} total seats, {reservedSeats} reserved");
                index++;
            }

            Console.WriteLine();
            Console.Write("Enter number between (1 and " + cinemaHalls.Count + "): ");
            
            string input = Console.ReadLine();

            if (int.TryParse(input, out choice))
            {
                choice--;

                if (choice >= 0 && choice < cinemaHalls.Count)
                {
                    hallName = new List<string>(cinemaHalls.Keys)[choice];
                    break;
                }
            }
        }


        CinemaHall selectedHall = cinemaHalls[hallName];
        
        DrawHall(selectedHall);

        Console.WriteLine();
        Console.WriteLine("1. Reserve/Cancel reservation");
        Console.WriteLine("2. Delete hall");
        Console.WriteLine("3. Back to main menu");

        Console.WriteLine();
        Console.Write("Select an option: ");
        string option = Console.ReadLine();

        if (option == "1") ReserveOrCancel(selectedHall, hallName);
        else if (option == "2") DeleteHall(hallName);
        else MainMenu();
    }

    static void ReserveOrCancel(CinemaHall hall, string hallName)
    {
        while (true)
        {
            Console.Clear();

            DrawHall(hall);

            Console.WriteLine();
            Console.Write("Enter seat to reserve or cancel (e.g., a1): ");
            string seat = Console.ReadLine();

            if (!Regex.IsMatch(seat, "^[a-zA-Z]{1}[1-9]$|^[a-zA-Z]{1}[1-2][0-9]$|^[a-zA-Z]{1}30$")) // Kontrola správného formátu místa.
            {
                continue;
            }

            char seatLetter = char.ToUpper(seat[0]); // Pro zjednodušení převádíme všechna písmena na stejný registr.
            int seatRow = int.Parse(seat.Substring(1));

            if (seatLetter - 'A' >= hall.Width || seatRow > hall.Height) // Kontrola, zda místo existuje.
            {
                continue;
            }

            if (hall.ReservedSeats.Contains(seat))
            {
                hall.ReservedSeats.Remove(seat);
                Console.WriteLine("Reservation canceled.");
            }
            else
            {
                hall.ReservedSeats.Add(seat);
                Console.WriteLine("Seat reserved.");
            }

            break;
        }


        SaveData();
        DrawHall(hall);

        Console.WriteLine();

        Console.WriteLine("1. Reserve/Cancel reservation");
        Console.WriteLine("2. Delete hall");
        Console.WriteLine("3. Back to main menu");

        Console.WriteLine();
        Console.Write("Select an option: ");
        string option = Console.ReadLine();

        if (option == "1") ReserveOrCancel(hall, hallName);
        else if (option == "2") DeleteHall(hallName);
        else MainMenu();
    }

    static void DeleteHall(string hallName) // Smazání existujícího sálu.
    {
        Console.Clear();
        Console.Write($"Are you sure you want to delete the hall {hallName}? (yes/no): ");

        string confirm = Console.ReadLine();

        if (confirm.ToLower() == "yes")
        {
            cinemaHalls.Remove(hallName);
            SaveData();
        }
        else {
            MainMenu();
        }
    }
} // Všechny komentáře v kódu byly napsány ChatGPT.
