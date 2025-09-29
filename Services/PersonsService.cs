using System.Diagnostics;
using System.Runtime.InteropServices.Marshalling;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly DeepDbContext _db;
        private readonly ICountriesService _countriesService;

        public PersonsService(DeepDbContext deepDbContext, ICountriesService countriesService)
        {
            _db = deepDbContext;
            _countriesService = countriesService;

        }

        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            //Check if argument is null
            if (personAddRequest is null)
                throw new ArgumentNullException(nameof(personAddRequest));

            //check PersonName (this is more validations are now being done with model validations)
            //if (string.IsNullOrEmpty(personAddRequest.PersonName))
            //    throw new ArgumentException(nameof(personAddRequest));

            ValidationHelper.ModelValidation(personAddRequest);

            //convert personAddRequest to Person type
            Person person = personAddRequest.ToPerson();
            person.PersonID = Guid.NewGuid();

            //add Person to persons lisst
            _db.Persons.Add(person);
            _db.SaveChanges();

            //Convert Person object to PersonResponse type
            return ConvertPersonToPersonResponse(person);

        }

        public List<PersonResponse> GetAllPersons()
        {
            return _db.Persons.ToList().Select(p => ConvertPersonToPersonResponse(p)).ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            if (personID == null) return null;

            Person? person = _db.Persons.FirstOrDefault(p => p.PersonID == personID);
            if (person == null) return null;

            return ConvertPersonToPersonResponse(person);
        }

        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            //Convert Person object to PersonResponse type
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesService.GetCountryByCountryID(person.CountryID)?.CountryName;
            return personResponse;
        }

        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;

            if(string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(searchBy))
                return matchingPersons;

            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons.Where(p => string.IsNullOrEmpty(p.PersonName)? true : p.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons.Where(p => string.IsNullOrEmpty(p.Email) ? true : p.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons.Where(p => p.DateOfBirth == null ? true : p.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString)).ToList();
                    break;
                case nameof(PersonResponse.Gender):
                    matchingPersons = allPersons.Where(p => string.IsNullOrEmpty(p.Gender) ? true : p.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case nameof(PersonResponse.CountryID):
                    matchingPersons = allPersons.Where(p => string.IsNullOrEmpty(p.Country) ? true : p.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case nameof(PersonResponse.Address):
                    matchingPersons = allPersons.Where(p => string.IsNullOrEmpty(p.Address) ? true : p.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                default:
                    matchingPersons = allPersons;
                    break;
            }

            return matchingPersons;


        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
                return allPersons;

            List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => allPersons
                    .OrderBy(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allPersons
                    .OrderByDescending(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons
                    .OrderBy(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons
                    .OrderByDescending(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(p => p.DateOfBirth)
                    .ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons
                    .OrderByDescending(p => p.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(p => p.Age).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.DESC) =>
                    allPersons.OrderByDescending(p => p.Age).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons
                    .OrderBy(p => p.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons
                    .OrderByDescending(p => p.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC) => allPersons
                    .OrderBy(p => p.Country, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Country), SortOrderOptions.DESC) => allPersons
                    .OrderByDescending(p => p.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons
                    .OrderBy(p => p.Address, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons
                    .OrderByDescending(p => p.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons
                    .OrderBy(p => p.ReceiveNewsLetters).ToList(),
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons
                    .OrderByDescending(p => p.ReceiveNewsLetters).ToList(),

                _ => allPersons

            };

            return sortedPersons;

        }

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            // Validate input
            if (personUpdateRequest is null)
                throw new ArgumentNullException(nameof(personUpdateRequest));

            ValidationHelper.ModelValidation(personUpdateRequest);

            // Find the person to update
            Person? existingPerson = _db.Persons.FirstOrDefault(p => p.PersonID == personUpdateRequest.PersonID);
            if (existingPerson is null)
                throw new ArgumentException("Person with the given ID does not exist.", nameof(personUpdateRequest));

            // Update properties
            existingPerson.PersonName = personUpdateRequest.PersonName;
            existingPerson.Email = personUpdateRequest.Email;
            existingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            existingPerson.Gender = personUpdateRequest.Gender?.ToString();
            existingPerson.Address = personUpdateRequest.Address;
            existingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
            existingPerson.CountryID = personUpdateRequest.CountryID;

            _db.SaveChanges();
            // Return updated response
            //return ConvertPersonToPersonResponse(existingPerson);
            return ConvertPersonToPersonResponse(existingPerson);
        }

        public bool DeletePerson(Guid? personID)
        {
            if(personID is null)
                throw new ArgumentNullException(nameof(personID));

            Person? person=_db.Persons.FirstOrDefault(p => p.PersonID == personID);
            if (person is null)
                return false;

            _db.Persons.Remove(_db.Persons.First(  p => p.PersonID == personID));
            _db.SaveChanges();
            return true;
        }
    }
}
