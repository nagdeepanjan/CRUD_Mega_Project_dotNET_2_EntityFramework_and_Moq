using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUD_Mega_Project_dotNET.Controllers
{
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        public PersonsController(IPersonsService personsService, ICountriesService countriesService)
        {
            _personsService = personsService;
            _countriesService = countriesService;
        }
        
        [Route("/persons/index")]
        [Route("/")]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy=nameof(PersonResponse.PersonName), SortOrderOptions sortOrder=SortOrderOptions.ASC)
        {
            ViewBag.SearchFields = new Dictionary<string, string>
            {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryID), "Country" },
                { nameof(PersonResponse.Address), "Address" }
            };

            List<PersonResponse> persons = await _personsService.GetFilteredPersons(searchBy, searchString);
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            //Sort
            List<PersonResponse> sortedPersons =await _personsService.GetSortedPersons(persons, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();


            return View(sortedPersons);  //Views/Persons/Index.cshtml
        }


        //Executes when the user clicks on "Create Person" hyperlink
        [HttpGet("/persons/create")]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries=await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(c => new SelectListItem { Text = c.CountryName, Value = c.CountryID.ToString() }); //Used for the select dropdown of countries

            return View();
        }

        
        [HttpPost("persons/create")]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = await _countriesService.GetAllCountries();
                ViewBag.Countries = countries.Select(c =>
                    new SelectListItem() { Text = c.CountryName, Value = c.CountryID.ToString() });

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }

            //call the service method
            PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);

            //navigate to Index() action method (it makes another get request to "persons/index"
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet("/persons/[action]/{personID}")]
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);
            if (personResponse is null)
                return RedirectToAction("Index");
            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            //create the countries list to be used by the dropdown
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(c =>
                new SelectListItem() { Text = c.CountryName, Value = c.CountryID.ToString() });

            return View(personUpdateRequest);
        }

        [HttpPost("/persons/[action]/{personID}")]
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse personResponse= await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);
            if (personResponse is null)
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                PersonResponse updatedPerson= await _personsService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");
            }
            else
            {
                List<CountryResponse> countries = await _countriesService.GetAllCountries();
                ViewBag.Countries = countries.Select(c =>
                    new SelectListItem() { Text = c.CountryName, Value = c.CountryID.ToString() });

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(personResponse.ToPersonUpdateRequest());
            }
        }

        [HttpGet("/persons/[action]/{personID}")]
        public async Task<IActionResult> Delete(Guid? personID)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);
            if (personResponse is null)
                return RedirectToAction("Index");

            return View(personResponse);
        }

        [HttpPost("/persons/[action]/{personID}")]
        public async Task<IActionResult> Delete(Guid personID)
        {
            PersonResponse personresponse=await _personsService.GetPersonByPersonID(personID);
            if(personresponse is null)
                return RedirectToAction("Index");

            _personsService.DeletePerson(personID);
            return RedirectToAction("Index");
        }
    }
}
