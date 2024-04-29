namespace ordination_test;

using Microsoft.EntityFrameworkCore;

using Service;
using Data;
using shared.Model;

[TestClass]
public class ServiceTest
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
    public void PatientsExist()
    {
        Assert.IsNotNull(service.GetPatienter());
    }

    [TestMethod]
    public void OpretDagligFast()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(1, service.GetDagligFaste().Count());

        service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(2, service.GetDagligFaste().Count());
    }


    [TestMethod]
    public void OpretDagligSkaev()
    {

        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        Dosis[] doser = new Dosis[] { }; 
        DateTime startDato = DateTime.Now;
        DateTime slutDato = DateTime.Now.AddDays(3);

        
        DagligSkæv result = service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, doser, startDato, slutDato);

        
        Assert.IsNotNull(result); 
    }

    [TestMethod]
    public void OpretPN()
    {
        ///TODO
    }

    [TestMethod]
    public void AnbefaletDosis()
    {

        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();


        double recommendedDose = service.GetAnbefaletDosisPerDøgn(patient.PatientId, lm.LaegemiddelId);

      
        Assert.IsTrue(recommendedDose == 9.51, "Recommended dose should be 9.51");
    }
    [TestMethod]
    public void SøgPatient()
    {
        
        string cprNumberToSearch = "121256-0512";

        
        var patients = service.GetPatienter();

        
        bool patientFound = false;
        foreach (var patient in patients)
        {
            if (patient.cprnr == cprNumberToSearch)
            {
                patientFound = true;
                break;
            }
        }

    }

    [TestMethod]
    [ExpectedException(typeof(System.InvalidOperationException))]
    public void GetAnbefaletDosisPerDøgn_ThrowsExceptionWhenPatientIdIsNull()
    {
        // Arrange
        int? nullPatientId = null;
        int existingLægemiddelId = 1; 

        // Act & Assert
        service.GetAnbefaletDosisPerDøgn(nullPatientId.Value, existingLægemiddelId);
    }

}