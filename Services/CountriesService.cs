using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries;

        public CountriesService(bool initializa=true)
        {
            _countries = new List<Country>();

            if (initializa)
            {
                _countries.AddRange(new List<Country>{
                    new Country() {  CountryID = Guid.Parse("5201AAFC-BE35-4E2D-9FCE-DBA1E27C7EA7"), CountryName = "USA" },

                    new Country() { CountryID = Guid.Parse("9C5B67B7-6265-4C95-82E4-8A06AB579787"), CountryName = "Canada" },

                    new Country() { CountryID = Guid.Parse("8BC7F448-EC3E-48DD-8CA5-AC7E800178B0"), CountryName = "UK" },

                    new Country() { CountryID = Guid.Parse("24DE56EC-95BE-4EA7-8E49-22FE0AB7AE80"), CountryName = "India" },

                    new Country() { CountryID = Guid.Parse("20085B07-8575-47AA-B794-70D3600417FA"), CountryName = "Australia" }
                });
            }
        }

        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            #region Validations
            //Validation: countryAddRequest parameter cannot be empty
            if (countryAddRequest is null)
                throw new ArgumentNullException(nameof(countryAddRequest));


            //Validation: countryAddRequest.CountryName cannot be null
            if (countryAddRequest.CountryName is null)
                throw new ArgumentException(nameof(countryAddRequest.CountryName));


            //Validation: countryAddRequest.CountryName duplicates are not allowed
            if (_countries.Any(c => c.CountryName.ToLower() == countryAddRequest.CountryName.ToLower()))
                throw new ArgumentException("Given country already exists");
            #endregion

            //Convert CountryAddRequest object to Country object
            Country country = countryAddRequest.ToCountry();

            //Generate and add a new GUID for this country
            country.CountryID = Guid.NewGuid();

            _countries.Add(country);

            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _countries.Select(c => c.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            if (countryID is null)
                return null;

            Country? country_from_list = _countries.FirstOrDefault(c => c.CountryID == countryID);

            return country_from_list?.ToCountryResponse();


        }
    }
}
