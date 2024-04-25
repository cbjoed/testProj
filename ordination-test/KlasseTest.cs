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
        public void PNDoegnDosis()
        {
            // input
            Laegemiddel lm = service.GetLaegemidler().First();
            DateTime startDato = DateTime.Now;
            DateTime slutDato = DateTime.Now.AddDays(3);
            double antalEnheder = 3;

            // Opret en PN
            PN pnOrdination = new PN(startDato, slutDato, antalEnheder, lm);

            // Opret en liste af Dato-objekter hvor doseringen gives
            List<Dato> doseringsDatoer = new List<Dato>
            {
                new Dato { dato = DateTime.Now.AddDays(-3) },
                new Dato { dato = DateTime.Now.AddDays(-2) },
                new Dato { dato = DateTime.Now.AddDays(-1) }
            };

            // Giv dosis på forskellige datoer
            foreach (var doseringsDato in doseringsDatoer)
            {
                pnOrdination.givDosis(doseringsDato);
            }

            // Beregn antal dage, hvor dosis er givet
            double antalAnvendteDage = doseringsDatoer.Count;

            // Beregn antal mulige datoer, hvor dosis kan gives
            double antalMuligeDatoer = (slutDato - startDato).Days + 1;

            // Beregn forventet døgndosis
            double forventetDoegnDosis = (antalEnheder * antalAnvendteDage) / antalMuligeDatoer;

            // Act
            double beregnetDoegnDosis = pnOrdination.doegnDosis();

            // Assert
            Assert.AreEqual(forventetDoegnDosis, beregnetDoegnDosis);
        }

        [TestMethod]
        public void AnvendPNOrdinationt() {
        ///TODO
        }
    }
}