using System;
using System.Collections.Generic;
using static System.Console;
using System.Threading;
using System.Threading.Tasks;
using Simple.OData.Client;
using System.Text;
using System.Linq;
using ConsoleTableExt;

namespace PeopleManagementSystem
{
    public class Person
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string[] Emails { get; set; }
        public List<AddressInfo> AddressInfo { get; set; }
        public string Gender { get; set; }
        public long Concurrency { get; set; }
    }

    public class AddressInfo
    {
        public string Address { get; set; }
        public City City { get; set; }
    }

    public class City
    {
        public string CountryRegion { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
    }

    //public class ChildOfOnosu
    //{
    //    private string firstName;
    //    public string FirstName { get; set; }
    //    public string ReadOnlyFirstName { get; }
    //    public string SecondReadOnlyFirstName { get { return firstName; } }
    //    public string SetOnlyFirstName
    //    {
    //        set
    //        {
    //            firstName = value;
    //        }
    //    }
    //}

    public class PeopleManager
    {
        //public static void OnosuDynasty()
        //{
        //    ChildOfOnosu oc = new ChildOfOnosu();
        //    oc.FirstName = "Dafe Great";
        //    oc.SetOnlyFirstName = "Onosu Great";

        //    WriteLine(oc.FirstName);
        //    WriteLine(oc.ReadOnlyFirstName);
        //    WriteLine(oc.SecondReadOnlyFirstName);
        //}

        public static ODataClient GetODataClient()
        {
            return new ODataClient("http://services.odata.org/v4/TripPinServiceRW/");
        }

        public static async void CreatePerson(Person person)
        {
            var oDataClient = GetODataClient();
            await oDataClient.For<Person>().Set(person).InsertEntryAsync();
        }

        public static async Task<IEnumerable<Person>> GetPeople()
        {
            try
            {
                var oDataClient = GetODataClient();
                IEnumerable<Person> people = await oDataClient.For<Person>().FindEntriesAsync();
                return people;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static async Task<IEnumerable<Person>> FilterPeople(string queryString)
        {
            try
            {
                var oDataClient = GetODataClient();
                IEnumerable<Person> people = await oDataClient.For<Person>()
                                                              .Filter(x => x.UserName.Contains(queryString) ||
                                                                           x.FirstName.Contains(queryString) ||
                                                                           x.LastName.Contains(queryString))
                                                              .FindEntriesAsync();
                return people;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static async Task<Person> ViewPersonDetails(string queryString)
        {
            try
            {
                var oDataClient = GetODataClient();
                IEnumerable<Person> people = await oDataClient.For<Person>()
                                                              .Filter(x => (x.UserName == queryString))
                                                              .FindEntriesAsync();
                return (people != null) ? people.ToList()[0] : null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static void ScreenHeader()
        {
            // render screen header
            string AppTitle = "People Management Solution";
            string AppTitleSeparator = "============================";

            WriteLine();
            WriteLine();
            WriteLine();
            WriteLine(String.Format("{0," + ((AppTitle.Length / 2)) + "}", AppTitle));
            WriteLine(String.Format("{0," + ((AppTitleSeparator.Length / 2)) + "}", AppTitleSeparator));
            WriteLine();
            //
        }

        public static void MainMenuScreen()
        {
            // clear screen
            Clear();

            // render screen header
            ScreenHeader();

            // render menu screen
            string MenuTitle = ":: Main Menu ::";
            WriteLine(String.Format("{0," + MenuTitle.Length + "}", MenuTitle));
            WriteLine();

            string[] MenuItems = new string[] { "1.\tList People", "2.\tSearch/Filter People", "3.\tShow Details on a Specific Person", "4.\tExit" };
            foreach (var item in MenuItems)
            {
                WriteLine(String.Format("{0," + item.Length + "}", item));
            }
            WriteLine();

            string MenuSelectorHint = "Choose a menu option to continue: ";
            Write(String.Format("{0, " + MenuSelectorHint.Length + "}", MenuSelectorHint));

            while (true)
            {
                var menuSelection = ReadKey(true);
                switch (menuSelection.Key)
                {
                    case ConsoleKey.D1:
                        ListPeopleScreen();
                        break;
                    case ConsoleKey.D2:
                        FilterPeopleScreen();
                        break;
                    case ConsoleKey.D3:
                        ViewPersonDetailsScreen();
                        break;
                    case ConsoleKey.D4:
                        ExitConfirmationScreen();
                        break;
                    default:
                        break;
                }
            }
        }

        public static void ListPeopleScreen()
        {
            try
            {
                // clear screen
                Clear();

                // render screen header
                ScreenHeader();

                // render list people screen
                string MenuTitle = ":: List People ::";
                WriteLine(String.Format("{0," + MenuTitle.Length + "}", MenuTitle));
                WriteLine();

                var people = GetPeople().Result;
                if (people.Count() > 0)
                {
                    ConsoleTableBuilder
                    .From<Person>(people.Select(p => new Person
                    {
                        UserName = p.UserName,
                        FirstName = p.FirstName,
                        LastName = p.LastName
                    }).ToList())
                    .WithColumn("Username", "First Name", "Last Name")
                    .WithFormat(ConsoleTableBuilderFormat.Default)
                    .ExportAndWriteLine();
                }
                else
                {
                    string EmptyMessage = "No records to show";
                    WriteLine(String.Format("{0, " + EmptyMessage.Length + "}", EmptyMessage));
                }

                // add a new line
                WriteLine();

                string MenuSelectorHint = "Press the <-- to return to the main menu: ";
                Write(String.Format("{0, " + MenuSelectorHint.Length + "}", MenuSelectorHint));

                while (true)
                {
                    var menuSelection = ReadKey(true);
                    if (menuSelection.Key == ConsoleKey.LeftArrow)
                    {
                        MainMenuScreen();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenuScreen();
            }
        }

        public static void FilterPeopleScreen()
        {
            try
            {
                // clear screen
                Clear();

                // render screen header
                ScreenHeader();

                // render filter people view
                string MenuTitle = ":: Search/Filter People ::";
                WriteLine(String.Format("{0," + MenuTitle.Length + "}", MenuTitle));
                WriteLine();

                string searchQueryTitle = "Enter name to search: ";
                Write(String.Format("{0, " + searchQueryTitle.Length + "}", searchQueryTitle));

                var searchQuery = ReadLine();
                var filteredPeople = FilterPeople(searchQuery).Result;
                if (filteredPeople.Count() > 0)
                {
                    ConsoleTableBuilder
                        .From<Person>(filteredPeople.Select(p => new Person
                        {
                            UserName = p.UserName,
                            FirstName = p.FirstName,
                            LastName = p.LastName
                        }).ToList())
                        .WithColumn("Username", "First Name", "Last Name")
                        .WithFormat(ConsoleTableBuilderFormat.Default)
                        .ExportAndWriteLine();
                }
                else
                {
                    string EmptyMessage = "No records to show";
                    WriteLine(String.Format("{0, " + EmptyMessage.Length + "}", EmptyMessage));
                }

                // add a new line
                WriteLine();

                string MenuSelectorHint = "Press the <-- to return to the main menu or F to search again: ";
                Write(String.Format("{0, " + MenuSelectorHint.Length + "}", MenuSelectorHint));

                while (true)
                {
                    var menuSelection = ReadKey(true);
                    switch (menuSelection.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            MainMenuScreen();
                            break;
                        case ConsoleKey.F:
                            FilterPeopleScreen();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenuScreen();
            }
        }

        public static void ViewPersonDetailsScreen()
        {
            try
            {
                // clear screen
                Clear();

                // render screen header
                ScreenHeader();

                // render person details view
                string MenuTitle = ":: Show Details on a Specific Person ::";
                WriteLine(String.Format("{0," + MenuTitle.Length + "}", MenuTitle));
                WriteLine();

                string searchQueryTitle = "Enter unique username: ";
                Write(String.Format("{0, " + searchQueryTitle.Length + "}", searchQueryTitle));

                var searchQuery = ReadLine();
                var person = ViewPersonDetails(searchQuery).Result;
                if (person != null)
                {
                    WriteLine();
                    var FirstName = "First Name: " + person.FirstName;
                    WriteLine(String.Format("{0, " + FirstName.Length + "}", FirstName));
                    WriteLine();

                    var LastName = "Last Name: " + person.LastName;
                    WriteLine(String.Format("{0, " + LastName.Length + "}", LastName));
                    WriteLine();

                    var Gender = "Gender: " + string.Join(',', person.Gender);
                    WriteLine(String.Format("{0, " + Gender.Length + "}", Gender));
                    WriteLine();

                    var EmailAddresses = "Email: " + string.Join(',', person.Emails);
                    WriteLine(String.Format("{0, " + EmailAddresses.Length + "}", EmailAddresses));
                    WriteLine();

                    int index = 1;
                    var ContactAddressTitle = "Contact Address" + index + ": ";
                    WriteLine(String.Format("{0, " + ContactAddressTitle.Length + "}", ContactAddressTitle));
                    WriteLine("**********************");
                    foreach (var item in person.AddressInfo)
                    {
                        var ContactAddress = item.Address;
                        var City = item.City;
                        var Location = City.Name + ", " + City.CountryRegion;
                        WriteLine(String.Format("{0, " + ContactAddress.Length + "}", ContactAddress));
                        WriteLine(String.Format("{0, " + Location.Length + "}", Location));
                        WriteLine();
                        index += 1;
                    }
                }
                else
                {
                    string EmptyMessage = "No record found";
                    WriteLine(String.Format("{0, " + EmptyMessage.Length + "}", EmptyMessage));
                }


                // add a new line
                WriteLine();

                string MenuSelectorHint = "Press the <-- to return to the main menu or F to search again: ";
                Write(String.Format("{0, " + MenuSelectorHint.Length + "}", MenuSelectorHint));

                while (true)
                {
                    var menuSelection = ReadKey(true);
                    switch (menuSelection.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            MainMenuScreen();
                            break;
                        case ConsoleKey.F:
                            ViewPersonDetailsScreen();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLine();
                string EmptyMessage = "No record found";
                WriteLine(String.Format("{0, " + EmptyMessage.Length + "}", EmptyMessage));
                WriteLine();

                string MenuSelectorHint = "Press the <-- to return to the main menu or F to search again: ";
                Write(String.Format("{0, " + MenuSelectorHint.Length + "}", MenuSelectorHint));

                while (true)
                {
                    var menuSelection = ReadKey(true);
                    switch (menuSelection.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            MainMenuScreen();
                            break;
                        case ConsoleKey.F:
                            ViewPersonDetailsScreen();
                            break;
                    }
                }
            }
        }

        public static void ExitConfirmationScreen()
        {
            try
            {
                // clear screen
                Clear();

                // render screen header
                ScreenHeader();

                // render person details view
                string MenuTitle = ":: Confirmation ::";
                WriteLine(String.Format("\t\t{0," + MenuTitle.Length + "}", MenuTitle));
                WriteLine();

                // add a new line
                WriteLine();

                string ConfirmationMessage = "Are you sure you want exit this program?";
                WriteLine(String.Format("\t\t{0, " + ConfirmationMessage.Length + "}", ConfirmationMessage));
                WriteLine();
                string MenuSelectorHint = "Press ESC key to continue or <-- to return to the main menu: ";
                Write(String.Format("\t\t{0, " + MenuSelectorHint.Length + "}", MenuSelectorHint));
                WriteLine();
                WriteLine();

                while (true)
                {
                    var menuSelection = ReadKey(true);
                    switch (menuSelection.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            MainMenuScreen();
                            break;
                        case ConsoleKey.Escape:
                            Environment.Exit(0);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenuScreen();
            }
        }

        public static void Main(string[] args)
        {
            MainMenuScreen();
        }
    }
}
