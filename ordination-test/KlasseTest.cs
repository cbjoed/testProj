using Data;
using Microsoft.EntityFrameworkCore;
using Service;
using shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ordination_test
{
    [TestClass]

    public class KlasseTest

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
        public void BeregnDoegnDosisForDagligFast()
        {
            // Arrange
            Patient patient = service.GetPatienter().First();
            Laegemiddel lm = service.GetLaegemidler().First();
            DateTime startDato = DateTime.Now;
            DateTime slutDato = DateTime.Now.AddDays(3);
            double morgenAntal = 2;
            double middagAntal = 2;
            double aftenAntal = 1;
            double natAntal = 0;

            // Opret en instans af DagligFast
            DagligFast dagligFast = new DagligFast(startDato, slutDato, lm, morgenAntal, middagAntal, aftenAntal, natAntal);

            // Act
            double forventetDoegnDosis = morgenAntal + middagAntal + aftenAntal + natAntal;
            double beregnetDoegnDosis = dagligFast.doegnDosis();

            // Assert
            Assert.AreEqual(forventetDoegnDosis, beregnetDoegnDosis);
        }

    }
}
