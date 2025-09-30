using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUD_Tests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            //_countriesService = new CountriesService(false);
            _countriesService = new CountriesService(new DeepDbContext( new DbContextOptionsBuilder<DeepDbContext>().Options));

        }

        #region TEST AddCountry()
        //When CountryAddRequest is null, throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {
                //Act
                await _countriesService.AddCountry(request);
            });
        }

        //When CountryName is null, throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest { CountryName = null };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                //Act
                await _countriesService.AddCountry(request);
            });
        }
        //When CountryName is duplicate, throw ArgumentException
        [Fact]
        public void AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest { CountryName = "Alpha" };
            CountryAddRequest? request2 = new CountryAddRequest { CountryName = "Alpha" };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _countriesService.AddCountry(request);
                _countriesService.AddCountry(request2);
            });
        }

        //When proper CountryName is supplied, it should be added to the list of countries.
        [Fact]
        public async Task AddCountry_OK()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest { CountryName = "Sweden" };

            //Act
            CountryResponse response = await _countriesService.AddCountry(request);
            List<CountryResponse> countries_from_GetAllCountries = await _countriesService.GetAllCountries();

            //Assert
            Assert.True(response.CountryID != Guid.Empty);
            Assert.Contains(response, countries_from_GetAllCountries);
        }

        #endregion


        #region  TEST GetAllCountries()

        //The list of countries should be empty by default (before adding other countries)
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            //Arrange
            //Act
            List<CountryResponse> real_country_response_list = await _countriesService.GetAllCountries();

            //Assert
            Assert.Empty(real_country_response_list);
        }

        //
        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<CountryAddRequest> country_request_list = new List<CountryAddRequest> { new CountryAddRequest { CountryName = "India" }, new CountryAddRequest { CountryName = "USA" }, new CountryAddRequest { CountryName = "UK" } };

            //Act
            List<CountryResponse> country_response_list = new List<CountryResponse>();
            //country_request_list.ForEach(countryAddRequest =>
            //{
            //    country_response_list.Add(_countriesService.AddCountry(countryAddRequest));
            //});
            foreach (var countryAddRequest in country_request_list)
            {
                country_response_list.Add(await _countriesService.AddCountry(countryAddRequest));
            }

            List<CountryResponse> realCountryResponseList = await _countriesService.GetAllCountries();

            //read each element from countries_list_from_add_country
            foreach (CountryResponse expected_country in country_response_list)
            {
                Assert.Contains(expected_country, realCountryResponseList);
            }
        }


        #endregion

        #region TEST GetCountryByCountryID()
        // When countryId is null, a null is returned
        [Fact]
        public async Task GetCountryByCountryID_NullCounryID()
        {
            //Arrange
            Guid? countryID = null;

            //Act
            CountryResponse? country_response_from_get_method = await _countriesService.GetCountryByCountryID(countryID);

            //Assert
            Assert.Null(country_response_from_get_method);
        }

        //If we supply a valid country id, it should return the matching country details as CountryResponse object
        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            CountryAddRequest? countryAddRequest = new CountryAddRequest { CountryName = "Vietnam" };
            CountryResponse countryResponse_from_add = await _countriesService.AddCountry(countryAddRequest);

            //Act
            CountryResponse? countryResponse_from_get = await _countriesService.GetCountryByCountryID(countryResponse_from_add.CountryID);

            //Assert
            Assert.Equal(countryResponse_from_get, countryResponse_from_add);
        }
        #endregion
    }
}
