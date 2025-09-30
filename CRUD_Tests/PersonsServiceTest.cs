using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System.Diagnostics.Contracts;
using System.Security.Cryptography.X509Certificates;
using Xunit.Abstractions;

namespace CRUD_Tests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        private readonly ITestOutputHelper _testOutputHelper;       //Used to show output in the Test window in VS

        public PersonsServiceTest(ITestOutputHelper testOutput)
        {
            _countriesService = new CountriesService(new DeepDbContext(new DbContextOptionsBuilder<DeepDbContext>().Options));
            _personsService = new PersonsService(new DeepDbContext(new DbContextOptionsBuilder<DeepDbContext>().Options), _countriesService);
            _testOutputHelper = testOutput;
        }

        #region TEST AddPerson()

        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async() => await _personsService.AddPerson(personAddRequest));
        }

        //When we supply null value as PersonName, it should throw an ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            // Arrange
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = null, Email = "alpha@deepz.com", Gender = GenderOptions.Female
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async() => await _personsService.AddPerson(personAddRequest));
        }

        //When we supply proper person details, it should insert the person into the list and return the PersonResponse with the newly generated PersonID
        [Fact]
        public async Task AddPerson_ProperPersonsDetails()
        {
            // Arrange
            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                PersonName = "John Doe",
                Email = "john.doe@example.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = GenderOptions.Male,
                Address = "123 Main St",
                ReceiveNewsLetters = true,
                CountryID = Guid.NewGuid()
            };

            // Act
            PersonResponse personResponse_from_add = await _personsService.AddPerson(personAddRequest);
            List<PersonResponse> persons_list = await _personsService.GetAllPersons();

            //Assert
            Assert.True(personResponse_from_add.PersonID != Guid.Empty);
            Assert.Contains(personResponse_from_add, persons_list);
        }

        #endregion

        #region TEST GetPersonByPersonID()
        //If we supply null s PersonID, it should return null as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(personID);

            //Assert
            Assert.Null(personID);
        }

        //If we supply a valid person id, it should return the valid person details as PersonResponse object
        [Fact]
        public async Task GetPersonByPersonID_WithPersonID()
        {
            //Arrange
            CountryAddRequest country_request = new CountryAddRequest { CountryName = "Mexico" };
            CountryResponse country_response = await _countriesService.AddCountry(country_request);

            //Act
            PersonAddRequest person_request = new PersonAddRequest
            {
                PersonName = "Beethoven",
                CountryID = country_response.CountryID,
                Gender = GenderOptions.Male,
                Address = "Bonn",
                ReceiveNewsLetters = false,
                DateOfBirth = new DateTime(1770, 1, 1),
                Email = "beethoven@music.com"
            };
            PersonResponse person_response_from_add = await _personsService.AddPerson(person_request);

            PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(person_response_from_add.PersonID);

            //Assert
            Assert.Equal(person_response_from_add, person_response_from_get);
        }


        #endregion

        #region TEST GetAllPersons()
        //The GetALlPersons() should return empty list by default

        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            //Arrange
            //Act
            List<PersonResponse> persons_from_get = await _personsService.GetAllPersons();

            //Assert
            Assert.Empty(persons_from_get);
        }



        //First, we will add a few persons, and then when we call GetAllPersons(), it should return the same persons that were added 
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            CountryAddRequest country_request_1 = new CountryAddRequest { CountryName = "Japan" };
            CountryAddRequest country_request_2 = new CountryAddRequest { CountryName = "China" };

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest
            {
                PersonName = "Sakura",
                Email = "sakura@japan.com",
                DateOfBirth = new DateTime(1985, 3, 21),
                Gender = GenderOptions.Female,
                Address = "Tokyo",
                ReceiveNewsLetters = true,
                CountryID = country_response_1.CountryID
            };

            PersonAddRequest person_request_2 = new PersonAddRequest
            {
                PersonName = "Li",
                Email = "li@china.com",
                DateOfBirth = new DateTime(1990, 5, 15),
                Gender = GenderOptions.Male,
                Address = "Beijing",
                ReceiveNewsLetters = false,
                CountryID = country_response_2.CountryID
            };

            PersonAddRequest person_request_3 = new PersonAddRequest
            {
                PersonName = "Manu",
                Email = "sakura@japan.com",
                DateOfBirth = new DateTime(1988, 3, 8),
                Gender = GenderOptions.Male,
                Address = "Djakarta",
                ReceiveNewsLetters = true,
                CountryID = country_response_1.CountryID
            };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest> { person_request_1, person_request_2, person_request_3 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            List<PersonResponse> persons_list_from_get = await _personsService.GetAllPersons();


            //print persons_response_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, persons_list_from_get);

            }



        }

        #endregion

        #region TEST GetFilteredPersons
        //If search text is empty and search by is 'PersonName', it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            //Arrange
            CountryAddRequest country_request_1 = new CountryAddRequest { CountryName = "Japan" };
            CountryAddRequest country_request_2 = new CountryAddRequest { CountryName = "China" };

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest
            {
                PersonName = "Sakura",
                Email = "sakura@japan.com",
                DateOfBirth = new DateTime(1985, 3, 21),
                Gender = GenderOptions.Female,
                Address = "Tokyo",
                ReceiveNewsLetters = true,
                CountryID = country_response_1.CountryID
            };

            PersonAddRequest person_request_2 = new PersonAddRequest
            {
                PersonName = "Li",
                Email = "li@china.com",
                DateOfBirth = new DateTime(1990, 5, 15),
                Gender = GenderOptions.Male,
                Address = "Beijing",
                ReceiveNewsLetters = false,
                CountryID = country_response_2.CountryID
            };

            PersonAddRequest person_request_3 = new PersonAddRequest
            {
                PersonName = "Manu",
                Email = "sakura@japan.com",
                DateOfBirth = new DateTime(1988, 3, 8),
                Gender = GenderOptions.Male,
                Address = "Djakarta",
                ReceiveNewsLetters = true,
                CountryID = country_response_1.CountryID
            };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest> { person_request_1, person_request_2, person_request_3 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            List<PersonResponse> persons_list_from_search =
                await _personsService.GetFilteredPersons(nameof(Person.PersonName), "");


            //print persons_response_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, persons_list_from_search);

            }
        }


        //Add a few persons and then search based on person name with a search string. It should return the matching persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            //Arrange
            CountryAddRequest country_request_1 = new CountryAddRequest { CountryName = "Japan" };
            CountryAddRequest country_request_2 = new CountryAddRequest { CountryName = "China" };

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest
            {
                PersonName = "Mary",
                Email = "sakura@japan.com",
                DateOfBirth = new DateTime(1985, 3, 21),
                Gender = GenderOptions.Female,
                Address = "Tokyo",
                ReceiveNewsLetters = true,
                CountryID = country_response_1.CountryID
            };

            PersonAddRequest person_request_2 = new PersonAddRequest
            {
                PersonName = "Rahman",
                Email = "li@china.com",
                DateOfBirth = new DateTime(1990, 5, 15),
                Gender = GenderOptions.Male,
                Address = "Beijing",
                ReceiveNewsLetters = false,
                CountryID = country_response_2.CountryID
            };

            PersonAddRequest person_request_3 = new PersonAddRequest
            {
                PersonName = "Manu",
                Email = "sakura@japan.com",
                DateOfBirth = new DateTime(1988, 3, 8),
                Gender = GenderOptions.Male,
                Address = "Djakarta",
                ReceiveNewsLetters = true,
                CountryID = country_response_1.CountryID
            };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest> { person_request_1, person_request_2, person_request_3 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            List<PersonResponse> persons_list_from_search =
                await _personsService.GetFilteredPersons(nameof(Person.PersonName), "ma");


            //print persons_response_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                if (person_response_from_add.PersonName!=null && person_response_from_add.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                {
                    Assert.Contains(person_response_from_add, persons_list_from_search);
                }

            }



        }
        #endregion

        #region GetSortedPersons

        //When we sort based on PersonName in DESC, it should return person list in descending on PersonName
        [Fact]
        public async Task GetSortedPersons()
        {
            //Arrange
            CountryAddRequest country_request_1 = new CountryAddRequest { CountryName = "Japan" };
            CountryAddRequest country_request_2 = new CountryAddRequest { CountryName = "China" };

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest
            {
                PersonName = "Mary",
                Email = "sakura@japan.com",
                DateOfBirth = new DateTime(1985, 3, 21),
                Gender = GenderOptions.Female,
                Address = "Tokyo",
                ReceiveNewsLetters = true,
                CountryID = country_response_1.CountryID
            };

            PersonAddRequest person_request_2 = new PersonAddRequest
            {
                PersonName = "Rahman",
                Email = "li@china.com",
                DateOfBirth = new DateTime(1990, 5, 15),
                Gender = GenderOptions.Male,
                Address = "Beijing",
                ReceiveNewsLetters = false,
                CountryID = country_response_2.CountryID
            };

            PersonAddRequest person_request_3 = new PersonAddRequest
            {
                PersonName = "Manu",
                Email = "sakura@japan.com",
                DateOfBirth = new DateTime(1988, 3, 8),
                Gender = GenderOptions.Male,
                Address = "Djakarta",
                ReceiveNewsLetters = true,
                CountryID = country_response_1.CountryID
            };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest> { person_request_1, person_request_2, person_request_3 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            List<PersonResponse> allPersons = await _personsService.GetAllPersons();
            //Act
            List<PersonResponse> persons_list_from_sort =
                await _personsService.GetSortedPersons(allPersons,nameof(Person.PersonName), SortOrderOptions.DESC);


            //print persons_response_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }
            person_response_list_from_add = person_response_list_from_add.OrderByDescending(p => p.PersonName).ToList();

            //Assert
            for (int i = 0; i < person_response_list_from_add.Count; i++)
            {
                Assert.Equal(person_response_list_from_add[i], persons_list_from_sort[i]);
            }
        }
        #endregion

        #region TEST UpdatePerson()

        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {
                //Act
                await _personsService.UpdatePerson(person_update_request);
            });
        }

        //When we supply invalid personid, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonId()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = new PersonUpdateRequest { PersonID = Guid.NewGuid()};

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                //Act
                await _personsService.UpdatePerson(person_update_request);
            });
        }

        //When person name is null or empty, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest { CountryName = "France"};
            CountryResponse country_response_from_add= await _countriesService.AddCountry(country_add_request);


            PersonAddRequest person_add_request = new PersonAddRequest
                { PersonName = "Aloha", CountryID = country_response_from_add.CountryID, Email = "mindy@yahoo.com", Gender = GenderOptions.Other};
            PersonResponse person_response_from_add= await _personsService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = null;


            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                //Act
                await _personsService.UpdatePerson(person_update_request);
            });
        }

        //First, add a new person and then update the name & email
        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdate()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest { CountryName = "Germany" };
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);


            PersonAddRequest person_add_request = new PersonAddRequest
                { PersonName = "Sunil", CountryID = country_response_from_add.CountryID, Address = "N-22", DateOfBirth = DateTime.Parse("2000-01-01"), Email = "abc@xyz.com", Gender = GenderOptions.Female, ReceiveNewsLetters = false};
            PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = "Amit";
            person_update_request.Email = "newmail@gmail.com";


            //Act
            PersonResponse person_response_from_update= await _personsService.UpdatePerson(person_update_request);
            PersonResponse person_response_from_get= await _personsService.GetPersonByPersonID(person_response_from_update.PersonID);

            //Assert
            Assert.Equal(person_response_from_get, person_response_from_update);
            
        }
        #endregion

        #region TEST DeletePerson()

        //If you supply valid PersonID, it returns true
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest { CountryName = "USA" };
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = new PersonAddRequest
            {
                PersonName = "Mat",
                CountryID = country_response_from_add.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("1999-12-12"),
                Address = "Nayantara",
                Email = "donotmail@yahoo.com",
                ReceiveNewsLetters = true
            };
            PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);
            

            //Act
            bool isDeleted = await _personsService.DeletePerson(person_response_from_add.PersonID);

            //Assert
            Assert.True(isDeleted);

        }


        //If you supply invalid PersonID, it returns false
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest { CountryName = "USA" };
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = new PersonAddRequest
            {
                PersonName = "Mat",
                CountryID = country_response_from_add.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("1999-12-12"),
                Address = "Nayantara",
                Email = "donotmail@yahoo.com",
                ReceiveNewsLetters = true
            };
            PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);


            //Act
            bool isDeleted = await _personsService.DeletePerson(Guid.NewGuid());

            //Assert
            Assert.False(isDeleted);

        }

        #endregion
    }
}
