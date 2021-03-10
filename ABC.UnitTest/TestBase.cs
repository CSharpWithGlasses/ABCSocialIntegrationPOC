using ABC.Data;
using ABC.Shared.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ABC.UnitTest
{
    public class TestBase
    {
        protected virtual IRepository SetupABCDBRepo()
        {
            var options = new DbContextOptionsBuilder<ABCDbContext>()
                           .UseInMemoryDatabase(databaseName: "ABCDB")
                           .Options;

            return new ABCRepository(new ABCDbContext(options));
        }

        protected virtual IConfiguration GetConfigSetings()
        {
            IConfiguration config = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", false, true)
                                .Build();

            return config;
        }
    }
}
