using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        
        public virtual DbSet<Country> Countries { get; set; }           //Virtual is needed for Mocking
        public virtual DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)   //defines how model binds to database. Also used during migrations
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
            
            //modelBuilder.Entity<Person>().HasIndex(i => i.TIN).IsUnique();
            //modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "len([TIN])=8");
            
            //Table Relations (this is generally not necessary if the navigation properties are already being used in the entities)
            //modelBuilder.Entity<Person>(p => { p.HasOne<Country>(c => c.Country).WithMany(c => c.Persons).HasForeignKey(p=>p.CountryID); });

        }

        //public List<Person> sp_GetAllPersons()              //Demonstrating a STORED Procedure
        //{
        //    return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        //}
    }
}
