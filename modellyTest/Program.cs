using Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modellyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var nnn = new List<int> { 1, 2, 3, 4 };
            var nns = String.Join("Id,", nnn);
            Console.WriteLine(nns);
            Console.WriteLine("Welcome to Modelly test App.\r\nPlease choose what you would like to do from the list below:");
            Console.WriteLine("***********************************************************************************");

            int choice;
            string id;
            do
            {
                choice = -1;
                Console.WriteLine("\n***********************************************************************************\n");
                Console.WriteLine("1. Retrieve a person by ID");
                Console.WriteLine("2. List all persons in the db");
                Console.WriteLine("3. Add a new person in the db");
                Console.WriteLine("4. Add several persons");
                Console.WriteLine("5. Edit a person.");
                Console.WriteLine("6. Edit several persons.");
                Console.WriteLine("7. Delete a person.");
                Console.WriteLine("8. Delete several persons.");

                Console.WriteLine("9. Quit.");

                try { choice = int.Parse(Console.ReadLine()); }
                catch (Exception ex) { Console.WriteLine("\nOops... ! An exception occurs.\r\n{0}\r\n\r\n{1}\n", ex.Message, ex.StackTrace); }
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Please type the person's ID:");
                        id = Console.ReadLine();
                        get(long.Parse(id));
                        break;
                    case 2:
                        getMultiple();
                        break;
                    case 3:
                        add();
                        break;
                    case 4:
                        addMultiple();
                        break;
                    case 5:
                        Console.WriteLine("Please type the person's ID:");
                        id = Console.ReadLine();
                        edit(long.Parse(id));
                        break;
                    case 6:
                        editMultiple();
                        break;
                    case 7:
                        Console.WriteLine("Please type the person's ID:");
                        id = Console.ReadLine();
                        delete(long.Parse(id));
                        break;
                    case 8:
                        deleteMultiple();
                        break;
                    case 9:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("\n\nBeg you pardon, I did not understand your choice.\r\nPlease retry.");
                        break;
                }
            } while (true);

        }
        public static void add()
        {
            try
            {
                Moperson p = new Moperson();
                Console.WriteLine("Type the person's first name :");
                p.first_name = Console.ReadLine();
                Console.WriteLine("Type the person's last name :");
                p.last_name = Console.ReadLine();
                Console.WriteLine("Type the person's age :");
                p.age = decimal.Parse(Console.ReadLine());
                Console.WriteLine("Type the person's dob :");
                p.date_of_birth = DateTime.Parse(Console.ReadLine());
                Console.WriteLine("Type the person's address :");
                p.address = Console.ReadLine();

                Moperson.Access.Add(p);
                Console.WriteLine("{0} {1} successfully added to the db.", p.first_name, p.last_name);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Oops... ! An exception occurs.\r\n{0}\r\n\r\n{1}", ex.Message, ex.StackTrace);
            }
        }
        public static void addMultiple()
        {
            List<Moperson> pers = new List<Moperson>();
            pers.Add(new Moperson { address = "Nowhere", age = 15, date_of_birth = new DateTime(1990, 01, 01), first_name = "fname", last_name = "lname" });
            pers.Add(new Moperson { address = "Nowhere", age = 15, date_of_birth = new DateTime(1990, 01, 01), first_name = "fname", last_name = "lname" });
            pers.Add(new Moperson { address = "Nowhere", age = 15, date_of_birth = new DateTime(1990, 01, 01), first_name = "fname", last_name = "lname" });
            pers.Add(new Moperson { address = "Nowhere", age = 15, date_of_birth = new DateTime(1990, 01, 01), first_name = "fname", last_name = "lname" });
            pers.Add(new Moperson { address = "Nowhere", age = 15, date_of_birth = new DateTime(1990, 01, 01), first_name = "fname", last_name = "lname" });
            pers.Add(new Moperson { address = "Nowhere", age = 15, date_of_birth = new DateTime(1990, 01, 01), first_name = "fname", last_name = "lname" });
            pers.Add(new Moperson { address = "Nowhere", age = 15, date_of_birth = new DateTime(1990, 01, 01), first_name = "fname", last_name = "lname" });

            int r = Moperson.Access.Add(pers);
            Console.WriteLine("{0} persons added successfully", r);
        }
        public static void get(long id)
        {
            var p = Moperson.Access.Get(id);
            if (p != null)
            {
                Console.WriteLine("{0} found in db: Id:{1}, First name{2}, Last name:{3} ", id, p.Id, p.first_name, p.last_name);
            }
            else
            {
                Console.WriteLine("I am sorry, {0} is not found in db.", id);
            }
        }
        public static void getMultiple()
        {
            var pers = Moperson.Access.Get();
            if (pers != null && pers.Count > 0)
            {
                Console.WriteLine("I find {0} persons in the db, here's a list:", pers.Count);
                foreach (var p in pers)
                {
                    Console.WriteLine("\tId:{0}, First name:{1}, Last name:{2} ", p.Id, p.first_name, p.last_name);
                }
            }
            else
            {
                Console.WriteLine("I am sorry, there's no one found in the db.");
            }
        }
        public static void edit(long id)
        {
            try
            {
                var pp = Moperson.Access.Get(id);
                if (pp != null)
                {

                    Moperson p = new Moperson();
                    p.Id = id;
                    Console.WriteLine("Type the person's new first name :");
                    p.first_name = Console.ReadLine();
                    Console.WriteLine("Type the person's new last name :");
                    p.last_name = Console.ReadLine();
                    Console.WriteLine("Type the person's new age :");
                    p.age = int.Parse(Console.ReadLine());
                    Console.WriteLine("Type the person's new  dob :");
                    p.date_of_birth = DateTime.Parse(Console.ReadLine());
                    Console.WriteLine("Type the person's new address :");
                    p.address = Console.ReadLine();

                    Moperson.Access.Edit(p);
                    Console.WriteLine("{0} {1} successfully edited.", p.first_name, p.last_name);
                }
                else
                {
                    Console.WriteLine("I am sorry, {0} is not found in db.\nWe can not edit him", id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Oops... ! An exception occurs.\r\n{0}\r\n\r\n{1}", ex.Message, ex.StackTrace);
            }
        }
        public static void editMultiple()
        {

            List<Moperson> pers = new List<Moperson>();
            pers.Add(new Moperson { Id = 6, address = "6Nowhere", age = 15, date_of_birth = new DateTime(1990, 01, 01), first_name = "fname6", last_name = "lname" });
            pers.Add(new Moperson { Id = 7, address = "7Nowhere", age = 15, date_of_birth = new DateTime(1990, 01, 01), first_name = "fname7", last_name = "lname" });
            pers.Add(new Moperson { Id = 8, address = "8Nowhere", age = 15, date_of_birth = new DateTime(1990, 01, 01), first_name = "fname8", last_name = "lname" });
            pers.Add(new Moperson { Id = 9, address = "9Nowhere", age = 15, date_of_birth = new DateTime(1990, 01, 01), first_name = "fname9", last_name = "lname" });
            pers.Add(new Moperson { Id = 10, address = "10Nowhere", age = 15, date_of_birth = new DateTime(1990, 01, 01), first_name = "fname10", last_name = "lname" });
            pers.Add(new Moperson { Id = 11, address = "11Nowhere", age = 15, date_of_birth = new DateTime(1990, 01, 01), first_name = "fname11", last_name = "lname" });
            pers.Add(new Moperson { Id = 12, address = "12Nowhere", age = 15, date_of_birth = new DateTime(1990, 01, 01), first_name = "fname12", last_name = "lname" });

            int r = Moperson.Access.Edit(pers);
            Console.WriteLine("{0} persons edited successfully", r);
        }
        public static void delete(long id)
        {
            var p = Moperson.Access.Get(id);
            if (p != null)
            {
                Moperson.Access.Delete(id);
                Console.WriteLine("{0} successfully delete from db.", id);
            }
            else
            {
                Console.WriteLine("I am sorry, {0} is not found in db.\nSo we can not delete", id);
            }
        }
        public static void deleteMultiple()
        {
            List<long> pers = new List<long>();
            pers.Add(9);
            pers.Add(6);
            pers.Add(12);

            int r = Moperson.Access.Delete(pers);
            Console.WriteLine("{0} persons deleted successfully", r);
        }
    }
}
