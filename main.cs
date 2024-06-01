using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

[Serializable]
class Company
{
    public string Name { get; set; }
    public List<Employee> Employees { get; set; } = new List<Employee>();
    public List<Customer> Customers { get; set; } = new List<Customer>();
}

[Serializable]
class Employee
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public decimal Salary { get; set; }
}

[Serializable]
class Customer
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public List<Sale> OrderHistory { get; set; } = new List<Sale>();
}

[Serializable]
class Sale
{
    public string Item { get; set; }
    public int Quantity { get; set; }
    public decimal Cost { get; set; }
    public decimal TotalCost => Cost * Quantity;
}

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Enter the company name (without extension): ");
        string fileName = Console.ReadLine();

        if (!fileName.EndsWith(".dat"))
        {
            fileName += ".dat";
        }

        Company company = LoadCompanyData(fileName);

        bool quit = false;
        while (!quit)
        {
            Console.WriteLine("\n    MAIN MENU");
            Console.WriteLine("1.) Employees");
            Console.WriteLine("2.) Sales");
            Console.WriteLine("3.) Quit");
            Console.Write("\nChoice? ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ManageEmployees(company);
                    break;
                case "2":
                    ManageSales(company);
                    break;
                case "3":
                    quit = true;
                    SaveCompanyData(fileName, company);
                    break;
                default:
                    Console.WriteLine("Invalid choice, try again.");
                    break;
            }
        }
    }

    static Company LoadCompanyData(string fileName)
    {
        if (File.Exists(fileName))
        {
            try
            {
                string jsonString = File.ReadAllText(fileName);
                return JsonSerializer.Deserialize<Company>(jsonString) ?? new Company { Name = Path.GetFileNameWithoutExtension(fileName) };
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to load data: {e.Message}");
                return new Company { Name = Path.GetFileNameWithoutExtension(fileName) };
            }
        }
        else
        {
            Console.WriteLine("File does not exist. Creating a new company data file.");
            return new Company { Name = Path.GetFileNameWithoutExtension(fileName) };
        }
    }

    static void SaveCompanyData(string fileName, Company company)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(company, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fileName, jsonString);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to save data: {e.Message}");
        }
    }

    static void ManageEmployees(Company company)
    {
        bool backToMainMenu = false;
        while (!backToMainMenu)
        {
            Console.WriteLine("\nCurrent Employees:");
            foreach (Employee employee in company.Employees)
            {
                Console.WriteLine($"{employee.Name} <{employee.Email}>  Phone: {employee.Phone} Salary: {employee.Salary:C}");
            }

            Console.Write("(A)dd Employee or (M)ain Menu? ");
            string choice = Console.ReadLine();

            if (choice.ToUpper() == "A")
            {
                Employee newEmployee = new Employee();

                Console.Write("Name: ");
                newEmployee.Name = Console.ReadLine();
                Console.Write("Email: ");
                newEmployee.Email = Console.ReadLine();
                Console.Write("Phone: ");
                newEmployee.Phone = Console.ReadLine();
                Console.Write("Salary: ");
                newEmployee.Salary = decimal.Parse(Console.ReadLine());

                company.Employees.Add(newEmployee);
            }
            else if (choice.ToUpper() == "M")
            {
                backToMainMenu = true;
            }
            else
            {
                Console.WriteLine("Invalid choice, try again.");
            }
        }
    }

    static void ManageSales(Company company)
    {
        bool backToMainMenu = false;
        while (!backToMainMenu)
        {
            Console.Write("(A)dd Customer, Enter a (S)ale, (V)iew Customer, or (M)ain Menu? ");
            string choice = Console.ReadLine();

            switch (choice.ToUpper())
            {
                case "A":
                    Customer newCustomer = new Customer();

                    Console.Write("Name: ");
                    newCustomer.Name = Console.ReadLine();
                    Console.Write("Email: ");
                    newCustomer.Email = Console.ReadLine();
                    Console.Write("Phone: ");
                    newCustomer.Phone = Console.ReadLine();

                    company.Customers.Add(newCustomer);
                    break;
                case "S":
                    if (company.Customers.Count == 0)
                    {
                        Console.WriteLine("Error: No Customers.");
                        break;
                    }

                    for (int i = 0; i < company.Customers.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}.) {company.Customers[i].Name}");
                    }

                    Console.Write("Choice? ");
                    int customerChoice = int.Parse(Console.ReadLine()) - 1;

                    if (customerChoice < 0 || customerChoice >= company.Customers.Count)
                    {
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                    }

                    Sale newSale = new Sale();

                    Console.Write("Item: ");
                    newSale.Item = Console.ReadLine();
                    Console.Write("Quantity: ");
                    newSale.Quantity = int.Parse(Console.ReadLine());
                    Console.Write("Cost: ");
                    newSale.Cost = decimal.Parse(Console.ReadLine());

                    company.Customers[customerChoice].OrderHistory.Add(newSale);
                    break;
                case "V":
                    if (company.Customers.Count == 0)
                    {
                        Console.WriteLine("Error: No Customers.");
                        break;
                    }

                    for (int i = 0; i < company.Customers.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}.) {company.Customers[i].Name}");
                    }

                    Console.Write("Choice? ");
                    int viewChoice = int.Parse(Console.ReadLine()) - 1;

                    if (viewChoice < 0 || viewChoice >= company.Customers.Count)
                    {
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                    }

                    Customer selectedCustomer = company.Customers[viewChoice];
                    Console.WriteLine($"{selectedCustomer.Name} <{selectedCustomer.Email}>  Phone: {selectedCustomer.Phone}");

                    Console.WriteLine("\nOrder History");
                    Console.WriteLine("Item                   Price  Quantity   Total");

                    foreach (Sale sale in selectedCustomer.OrderHistory)
                    {
                        Console.WriteLine($"{sale.Item.PadRight(20)} {sale.Cost.ToString("F2").PadRight(6)} {sale.Quantity.ToString().PadRight(10)} {sale.TotalCost.ToString("F2")}");
                    }
                    break;
                case "M":
                    backToMainMenu = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice, try again.");
                    break;
            }
        }
    }
}
