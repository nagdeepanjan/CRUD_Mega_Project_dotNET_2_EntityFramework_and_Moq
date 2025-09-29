using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents DTO class that is used as return type of most methods of Persons Service
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public double? Age { get; set; }

        public Guid? CountryID { get; set; }
        public string? Country { get; set; }


        /// <summary>
        /// Compares  the current object with the parameter object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True or False</returns>
        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (obj is not PersonResponse other)
                return false;


            return PersonID == other.PersonID
                && PersonName == other.PersonName
                && Email == other.Email
                && DateOfBirth == other.DateOfBirth
                && Gender == other.Gender
                && Address == other.Address
                && ReceiveNewsLetters == other.ReceiveNewsLetters
                && CountryID == other.CountryID
                && Country == other.Country;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"PersonID: {PersonID}, PersonName: {PersonName}, Email: {Email}, DateOfBirth: {DateOfBirth?.ToString()}, Gender: {Gender}, Address: {Address}, ReceiveNewsLetters: {ReceiveNewsLetters}, Age: {Age}, CountryID: {CountryID}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                Address = Address,
                CountryID = CountryID,
                DateOfBirth = DateOfBirth,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true),
                ReceiveNewsLetters = ReceiveNewsLetters
            };
        }

    }
    /// <summary>
    /// An extension method to convert a Person object to PersonResponse
    /// </summary>
    /// <param name="person">The person object to convert</param>
    public static class PersonExtensions
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                Address = person.Address,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                CountryID = person.CountryID,
                Age = person.DateOfBirth.HasValue
                    ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25, 2)
                    : null
            };
        }

        
    }

}
