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

        [TestMethod]
        public void TestDoegnDosis()
        {
            // Arrange
            DateTime startDen = DateTime.Now;
            DateTime slutDen = DateTime.Now.AddDays(3);
            double antalEnheder = 3;
            PN pnOrdination = new PN(startDen, slutDen, antalEnheder, new Laegemiddel());

            // Add sample dates
            pnOrdination.dates.Add(new Dato { dato = DateTime.Now.AddDays(1).AddHours(8).AddMinutes(30) }); // Eksempel på en dato med specifikt tidspunkt
            pnOrdination.dates.Add(new Dato { dato = DateTime.Now.AddDays(2).AddHours(10).AddMinutes(15) });
            pnOrdination.dates.Add(new Dato { dato = DateTime.Now.AddDays(3).AddHours(9).AddMinutes(45) });

            // Expected average dose per day = (3 * 3) / 3 = 3

            // Act
            double result = pnOrdination.doegnDosis();

            // Assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void AnvendPNOrdinationt() {
        ///TODO
        }
    }
}