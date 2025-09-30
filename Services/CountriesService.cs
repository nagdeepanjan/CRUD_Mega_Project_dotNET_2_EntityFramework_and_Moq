using Entities;
using Microsoft.EntityFrameworkCore;
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

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
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

            await _db.Countries.AddAsync(country);
            await _db.SaveChangesAsync();

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return await _db.Countries.Select(c => c.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID is null)
                return null;

            Country? country_from_list = await _db.Countries.FirstOrDefaultAsync(c => c.CountryID == countryID);

            return country_from_list?.ToCountryResponse();


        }
    }
}
