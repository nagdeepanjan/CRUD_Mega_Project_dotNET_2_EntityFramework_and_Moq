using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly DeepDbContext _db;

        public CountriesService(DeepDbContext deepDbContext)
        {
            _db = deepDbContext;

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
            if (_db.Countries.Any(c => c.CountryName.ToLower() == countryAddRequest.CountryName.ToLower()))
                throw new ArgumentException("Given country already exists");
            #endregion

            //Convert CountryAddRequest object to Country object
            Country country = countryAddRequest.ToCountry();

            //Generate and add a new GUID for this country
            country.CountryID = Guid.NewGuid();

            _db.Countries.Add(country);
            _db.SaveChanges();

            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _db.Countries.Select(c => c.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            if (countryID is null)
                return null;

            Country? country_from_list = _db.Countries.FirstOrDefault(c => c.CountryID == countryID);

            return country_from_list?.ToCountryResponse();


        }
    }
}
