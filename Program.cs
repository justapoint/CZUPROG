using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

class CinemaHall
{
    public int Width { get; set; }
    public int Height { get; set; }
    public List<string> ReservedSeats { get; set; } = new List<string>();
}

class Program
{
    static string dataFile = "data.json";
    static Dictionary<string, CinemaHall> cinemaHalls = new Dictionary<string, CinemaHall>();

    static void Main()
    {
        LoadData();
        MainMenu();
    }

    static void LoadData()
    {
        if (File.Exists(dataFile))
        {
            var json = File.ReadAllText(dataFile);
            cinemaHalls = JsonConvert.DeserializeObject<Dictionary<string, CinemaHall>>(json) ?? new Dictionary<string, CinemaHall>();
        }
    }

    static void SaveData()
    {
        var json = JsonConvert.SerializeObject(cinemaHalls, Formatting.Indented);
        File.WriteAllText(dataFile, json);
    }

    static void MainMenu()
    {
        Console.Clear();
        Console.WriteLine("Welcome to the Cinema Reservation System");
        Console.WriteLine();
        if (cinemaHalls.Count == 0)
        {
            Console.WriteLine("1. Create new cinema hall");
            Console.WriteLine("2. Exit");
            string choice = Console.ReadLine();
            if (choice == "1") CreateNewHall();
            else Environment.Exit(0);
        }
        else
        {
            Console.WriteLine("1. Interact with existing cinema halls");
            Console.WriteLine("2. Create new cinema hall");
            Console.WriteLine("3. Exit");
            string choice = Console.ReadLine();
            if (choice == "1") InteractWithExistingHalls();
            else if (choice == "2") CreateNewHall();
            else Environment.Exit(0);
        }
    }

    static void CreateNewHall()
    {
        Console.Clear();
        Console.WriteLine("Enter cinema hall size (Width Max 26, Height Max 30)");

        int width = 0, height = 0;
        while (true)
        {
            Console.Write("Width (1-26): ");
            width = int.Parse(Console.ReadLine());
            if (width > 0 && width <= 26) break;
            Console.WriteLine("Invalid input. Try again.");
        }
        while (true)
        {
            Console.Write("Height (1-30): ");
            height = int.Parse(Console.ReadLine());
            if (height > 0 && height <= 30) break;
            Console.WriteLine("Invalid input. Try again.");
        }

        CinemaHall newHall = new CinemaHall { Width = width, Height = height };
        DrawHall(newHall);

        Console.WriteLine("\n1. Save hall");
        Console.WriteLine("2. Exit");
        string choice = Console.ReadLine();
        if (choice == "1")
        {
            Console.Write("Enter hall name: ");
            string hallName = Console.ReadLine();
            cinemaHalls[hallName] = newHall;
            SaveData();
        }
        else
        {
            Console.Write("Are you sure you want to exit without saving? (yes/no): ");
            string confirm = Console.ReadLine();
            if (confirm.ToLower() == "yes") Environment.Exit(0);
        }

        MainMenu();
    }

    static void DrawHall(CinemaHall hall)
    {
        Console.Clear();

        for (int i = 1; i <= hall.Height; i++)
        {
            Console.Write(i.ToString().PadRight(3));

            for (int j = 1; j <= hall.Width; j++)
            {
                string seatId = $"{(char)(j + 96)}{i}";
                char seat = (hall.ReservedSeats.Contains(seatId)) ? 'X' : '#';
                Console.ForegroundColor = (seat == 'X') ? ConsoleColor.Red : ConsoleColor.Green;
                Console.Write($"{seat} ");
                Console.ResetColor();

                if (j == hall.Width / 2)
                {
                    Console.Write("\t");
                }

            }
            Console.WriteLine();
        }
        Console.Write("   ");
        for (int i = 1; i <= hall.Width; i++)
        {
            Console.Write($"{(char)(i + 96)} ");

            if (i == hall.Width / 2)
            {
                Console.Write("\t");
            }
        }
        Console.WriteLine();
    }

    static void InteractWithExistingHalls()
    {
        Console.Clear();
        Console.WriteLine("Select a cinema hall:");
        int index = 1;
        foreach (var hall in cinemaHalls)
        {
            int totalSeats = hall.Value.Width * hall.Value.Height;
            int reservedSeats = hall.Value.ReservedSeats.Count;
            Console.WriteLine($"{index}. {hall.Key} [{hall.Value.Width}x{hall.Value.Height}] - {totalSeats} total seats, {reservedSeats} reserved");
            index++;
        }
        int choice = int.Parse(Console.ReadLine()) - 1;
        string hallName = new List<string>(cinemaHalls.Keys)[choice];
        CinemaHall selectedHall = cinemaHalls[hallName];
        
        DrawHall(selectedHall);
        Console.WriteLine("1. Reserve/Cancel reservation");
        Console.WriteLine("2. Delete hall");
        Console.WriteLine("3. Back to main menu");
        string option = Console.ReadLine();
        if (option == "1") ReserveOrCancel(selectedHall, hallName);
        else if (option == "2") DeleteHall(hallName);
        else MainMenu();
    }

    static void ReserveOrCancel(CinemaHall hall, string hallName)
    {
        Console.Clear();
        Console.Write("Enter seat to reserve or cancel (e.g., a1): ");
        string seat = Console.ReadLine();

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

        SaveData();
        DrawHall(hall);
        Console.WriteLine("1. Reserve/Cancel reservation");
        Console.WriteLine("2. Delete hall");
        Console.WriteLine("3. Back to main menu");
        string option = Console.ReadLine();
        if (option == "1") ReserveOrCancel(hall, hallName);
        else if (option == "2") DeleteHall(hallName);
        else MainMenu();
    }

    static void DeleteHall(string hallName)
    {
        Console.Clear();
        Console.Write($"Are you sure you want to delete the hall {hallName}? (yes/no): ");
        string confirm = Console.ReadLine();
        if (confirm.ToLower() == "yes")
        {
            cinemaHalls.Remove(hallName);
            SaveData();
            Console.WriteLine("Hall deleted.");
        }
        MainMenu();
    }
}
