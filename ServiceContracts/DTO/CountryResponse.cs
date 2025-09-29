using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class that is used as return type for most of CountriesService methods
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryID { get; set; }
        public string? CountryName { get; set; }

        #region meant for equality comparison to use BY Value and not By Reference
        public override bool Equals(object? obj)
        {
            if (obj is CountryResponse other)
            {
                return CountryID == other.CountryID &&
                       string.Equals(CountryName, other.CountryName, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(CountryID, CountryName?.ToLowerInvariant());
        }
        #endregion
    }

    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse { CountryID = country.CountryID, CountryName = country.CountryName };
        }
    }
}
