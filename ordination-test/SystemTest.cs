using Data;
using Microsoft.EntityFrameworkCore;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ordination_test

    
{
    internal class SystemTest
    {
        private DataService service;

        [TestInitialize]
        public void SetupBeforeEachTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrdinationContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "test-database");
            var context = new OrdinationContext(optionsBuilder.Options);
            service = new DataService(context);
            service.SeedData();
        }

        [TestMethod]
        public void SøgPatient()
        {
            ///TODO
        }
        [TestMethod]
        public void PNDoegnDosis()
        {
            ///TODO
        }
        [TestMethod]
        public void OpretDagligFast()
        {
            ///TODO
        }
        [TestMethod]
        public void OpretDagligSkæv()
        {
            ///TODO
        }
        [TestMethod]
        public void OpretPN()
        {
            ///TODO
        }
        [TestMethod]
        public void Exception()
        {
            ///TODO
        }
    }
}
