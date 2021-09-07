using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeopleManagementSystem;
using System.Collections.Generic;

namespace PeopleManagementSystemTest
{
    [TestClass]
    public class PeopleManagerTest
    {
        [TestMethod]
        public void Main()
        {
            CreatePerson();

            string[] args = new string[] { };
            PeopleManager.Main(args);

        }

        [TestMethod]
        public void GetPeople()
        {
            var people = PeopleManager.GetPeople();
            
        }

        [TestMethod]
        public void CreatePerson()
        {
            Person firstPerson = new Person()
            {
                FirstName = "Campilax",
                LastName = "Dum",
                AddressInfo = new List<AddressInfo>
                {
                    new AddressInfo()
            {
                Address = "",
                City = new City()
                {
                    CountryRegion = "Lagos, Nigeria",
                    Name = "Ikeja"
                }
            }
                }
            };
            PeopleManager.CreatePerson(firstPerson);
        }

        [TestMethod]
        public void OnosuDynasty()
        {
            PeopleManager.OnosuDynasty();
        }

    }
}
