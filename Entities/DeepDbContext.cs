using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class DeepDbContext: DbContext
    {
        public DeepDbContext(DbContextOptions options) : base(options)
        {
        }

        
        public DbSet<Country> Countries { get; set; }
        public DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            //Seed to Countries
            string countriesJson = File.ReadAllText("countries.json");
            List<Country> countries = JsonSerializer.Deserialize<List<Country>>(countriesJson);

            foreach (Country country in countries)
                modelBuilder.Entity<Country>().HasData(country);


            //Seed to Persons
            string personsJson = File.ReadAllText("persons.json");
            List<Person> persons = JsonSerializer.Deserialize<List<Person>>(personsJson);

            foreach (Person person in persons)
                modelBuilder.Entity<Person>().HasData(person);

            //Fluent API
            modelBuilder.Entity<Person>().Property(p => p.TIN)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue("ABC12345");

            //Table Relations (this is generally not necessary if the navigation properties are already being used in the entities)
            //modelBuilder.Entity<Person>(p => { p.HasOne<Country>(c => c.Country).WithMany(c => c.Persons).HasForeignKey(p=>p.CountryID); });

        }

        //public List<Person> sp_GetAllPersons()              //Demonstrating a STORED Procedure
        //{
        //    return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        //}
    }
}
