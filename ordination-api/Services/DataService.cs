using Microsoft.EntityFrameworkCore;
using System.Text.Json;

using shared.Model;
using static shared.Util;
using Data;

namespace Service;

public class DataService
{
    public OrdinationContext db { get; }

    public DataService(OrdinationContext db) {
        this.db = db;
    }

    /// <summary>
    /// Seeder noget nyt data i databasen, hvis det er nødvendigt.
    /// </summary>
    public void SeedData() {

        // Patients
        Patient[] patients = new Patient[5];
        patients[0] = db.Patienter.FirstOrDefault()!;

        if (patients[0] == null)
        {
            patients[0] = new Patient("121256-0512", "Jane Jensen", 63.4);
            patients[1] = new Patient("070985-1153", "Finn Madsen", 83.2);
            patients[2] = new Patient("050972-1233", "Hans Jørgensen", 89.4);
            patients[3] = new Patient("011064-1522", "Ulla Nielsen", 59.9);
            patients[4] = new Patient("123456-1234", "Ib Hansen", 87.7);

            db.Patienter.Add(patients[0]);
            db.Patienter.Add(patients[1]);
            db.Patienter.Add(patients[2]);
            db.Patienter.Add(patients[3]);
            db.Patienter.Add(patients[4]);
            db.SaveChanges();
        }

        Laegemiddel[] laegemiddler = new Laegemiddel[5];
        laegemiddler[0] = db.Laegemiddler.FirstOrDefault()!;
        if (laegemiddler[0] == null)
        {
            laegemiddler[0] = new Laegemiddel("Acetylsalicylsyre", 0.1, 0.15, 0.16, "Styk");
            laegemiddler[1] = new Laegemiddel("Paracetamol", 1, 1.5, 2, "Ml");
            laegemiddler[2] = new Laegemiddel("Fucidin", 0.025, 0.025, 0.025, "Styk");
            laegemiddler[3] = new Laegemiddel("Methotrexat", 0.01, 0.015, 0.02, "Styk");
            laegemiddler[4] = new Laegemiddel("Prednisolon", 0.1, 0.15, 0.2, "Styk");

            db.Laegemiddler.Add(laegemiddler[0]);
            db.Laegemiddler.Add(laegemiddler[1]);
            db.Laegemiddler.Add(laegemiddler[2]);
            db.Laegemiddler.Add(laegemiddler[3]);
            db.Laegemiddler.Add(laegemiddler[4]);

            db.SaveChanges();
        }

        Ordination[] ordinationer = new Ordination[6];
        ordinationer[0] = db.Ordinationer.FirstOrDefault()!;
        if (ordinationer[0] == null) {
            Laegemiddel[] lm = db.Laegemiddler.ToArray();
            Patient[] p = db.Patienter.ToArray();

            ordinationer[0] = new PN(new DateTime(2021, 1, 1), new DateTime(2021, 1, 12), 123, lm[1]);    
            ordinationer[1] = new PN(new DateTime(2021, 2, 12), new DateTime(2021, 2, 14), 3, lm[0]);    
            ordinationer[2] = new PN(new DateTime(2021, 1, 20), new DateTime(2021, 1, 25), 5, lm[2]);    
            ordinationer[3] = new PN(new DateTime(2021, 1, 1), new DateTime(2021, 1, 12), 123, lm[1]);
            ordinationer[4] = new DagligFast(new DateTime(2021, 1, 10), new DateTime(2021, 1, 12), lm[1], 2, 0, 1, 0);
            ordinationer[5] = new DagligSkæv(new DateTime(2021, 1, 23), new DateTime(2021, 1, 24), lm[2]);
            
            ((DagligSkæv) ordinationer[5]).doser = new Dosis[] { 
                new Dosis(CreateTimeOnly(12, 0, 0), 0.5),
                new Dosis(CreateTimeOnly(12, 40, 0), 1),
                new Dosis(CreateTimeOnly(16, 0, 0), 2.5),
                new Dosis(CreateTimeOnly(18, 45, 0), 3)        
            }.ToList();
            

            db.Ordinationer.Add(ordinationer[0]);
            db.Ordinationer.Add(ordinationer[1]);
            db.Ordinationer.Add(ordinationer[2]);
            db.Ordinationer.Add(ordinationer[3]);
            db.Ordinationer.Add(ordinationer[4]);
            db.Ordinationer.Add(ordinationer[5]);

            db.SaveChanges();

            p[0].ordinationer.Add(ordinationer[0]);
            p[0].ordinationer.Add(ordinationer[1]);
            p[2].ordinationer.Add(ordinationer[2]);
            p[3].ordinationer.Add(ordinationer[3]);
            p[1].ordinationer.Add(ordinationer[4]);
            p[1].ordinationer.Add(ordinationer[5]);

            db.SaveChanges();
        }
    }

    
    public List<PN> GetPNs() {
        return db.PNs.Include(o => o.laegemiddel).Include(o => o.dates).ToList();
    }

    public List<DagligFast> GetDagligFaste() {
        return db.DagligFaste
            .Include(o => o.laegemiddel)
            .Include(o => o.MorgenDosis)
            .Include(o => o.MiddagDosis)
            .Include(o => o.AftenDosis)            
            .Include(o => o.NatDosis)            
            .ToList();
    }

    public List<DagligSkæv> GetDagligSkæve() {
        return db.DagligSkæve
            .Include(o => o.laegemiddel)
            .Include(o => o.doser)
            .ToList();
    }

    public List<Patient> GetPatienter() {
        return db.Patienter.Include(p => p.ordinationer).ToList();
    }

    public List<Laegemiddel> GetLaegemidler() {
        return db.Laegemiddler.ToList();
    }

    public PN OpretPN(int patientId, int laegemiddelId, double antal, DateTime startDato, DateTime slutDato) {
    
        {
            // Find patienten og lægemidlet baseret på de angivne id'er
            Patient patient = db.Patienter.Find(patientId);
            Laegemiddel laegemiddel = db.Laegemiddler.Find(laegemiddelId);

            // Tjek om patienten og lægemidlet eksisterer
            if (patient == null || laegemiddel == null)
            {
                throw new ArgumentException("Ugyldig patient eller lægemiddel.");
            }
            if (slutDato < startDato)
            {
                throw new ArgumentException("Slutdatoen kan ikke være før startdatoen.");
            }

            // Opret en ny PN med de angivne oplysninger
            PN pn = new PN(startDato, slutDato, antal, laegemiddel);

            // Tilføj PN til patientens ordinationer
            patient.ordinationer.Add(pn);

            // Gem ændringer i databasen
            db.SaveChanges();

            return pn;
        }

    }

    public DagligFast OpretDagligFast(int patientId, int laegemiddelId,
      double antalMorgen, double antalMiddag, double antalAften, double antalNat,
      DateTime startDato, DateTime slutDato)
    {

        // Find patienten og lægemidlet baseret på de angivne id'er
        Patient patient = db.Patienter.Find(patientId);
        Laegemiddel laegemiddel = db.Laegemiddler.Find(laegemiddelId);

        // Tjek om patienten og lægemidlet eksisterer
        if (patient == null || laegemiddel == null)
        {
            throw new ArgumentException("Ugyldig patient eller lægemiddel.");
        }

        if (slutDato < startDato)
        {
            throw new ArgumentException("Slutdatoen kan ikke være før startdatoen.");
        }

        // Opret en ny DagligFast med de angivne oplysninger
        DagligFast dagligFast = new DagligFast(startDato, slutDato, laegemiddel, antalMorgen, antalMiddag, antalAften, antalNat);

        // Tilføj DagligFast til patientens ordinationer
        patient.ordinationer.Add(dagligFast);

        // Gem ændringer i databasen
        db.SaveChanges();

        return dagligFast;
    }


    public DagligSkæv OpretDagligSkaev(int patientId, int laegemiddelId, Dosis[] doser, DateTime startDato, DateTime slutDato)
    {
        // Find patienten og lægemidlet baseret på de angivne id'er
        Patient patient = db.Patienter.Find(patientId);
        Laegemiddel laegemiddel = db.Laegemiddler.Find(laegemiddelId);

        // Tjek om patienten og lægemidlet eksisterer
        if (patient == null || laegemiddel == null)
        {
            throw new ArgumentException("Ugyldig patient eller lægemiddel.");
        }
        if (slutDato < startDato)
        {
            throw new ArgumentException("Slutdatoen kan ikke være før startdatoen.");
        }

        // Opret en ny DagligSkæv med de angivne oplysninger
        DagligSkæv dagligSkæv = new DagligSkæv(startDato, slutDato, laegemiddel);

        // Tilføj doserne til en liste inde i DagligSkæv
        dagligSkæv.doser.AddRange(doser.ToList());

        // Tilføj DagligSkæv til patientens ordinationer
        patient.ordinationer.Add(dagligSkæv);

        // Gem ændringer i databasen
        db.SaveChanges();

        return dagligSkæv;
    }


    public string AnvendOrdination(int id, Dato dato)
    {
        // Find PN-ordinationen baseret på det angivne id
        PN ordination = db.PNs.Find(id);

        // Tjek om ordinationen blev fundet
        if (ordination == null)
        {
            return "Ordinationen blev ikke fundet.";
        }
        
        // Tjek om den angivne dato er inden for ordinationens gyldighedsperiode

        // Anvend ordinationen ved at tilføje datoen til listen over doser
        if (ordination.givDosis(dato))
        {
            db.SaveChanges();
            
            return "Ordinationen blev anvendt på " + dato.dato.ToString();
        }
        else
        {
            return "Fejl: Den angivne dato er uden for ordinationens gyldighedsperiode.";
        }
    }


    /// <summary>
    /// Den anbefalede dosis for den pågældende patient, per døgn, hvor der skal tages hensyn til
	/// patientens vægt. Enheden afhænger af lægemidlet. Patient og lægemiddel må ikke være null.
    /// </summary>
    /// <param name="patient"></param>
    /// <param name="laegemiddel"></param>
    /// <returns></returns>

    private Patient getPatientById(int patientId)
    {
        return db.Patienter.Find(patientId);
    }

    // Metode til at hente lægemiddel fra databasen baseret på lægemidlets ID
    private Laegemiddel getLaegemiddelById(int laegemiddelId)
    {
        return db.Laegemiddler.Find(laegemiddelId);
    }

    // Metode til at bestemme vægtklassen baseret på patientens vægt
    private String bestemVægtKlasse(double vægt)
    {
        if (vægt < 25)
        {
            return "Under 25";
        }
        else if (vægt <= 120)
        {
            return "Normal";
        }
        else
        {
            return "Over 120";
        }
    }

    // Metode til at beregne den anbefalede dosis per døgn baseret på patientens ID og lægemidlets ID
    public double GetAnbefaletDosisPerDøgn(int patientId, int laegemiddelId)
    {
        // Hent patient og lægemiddel fra databasen
        Patient patient = getPatientById(patientId);
        Laegemiddel lægemiddel = getLaegemiddelById(laegemiddelId);

        // Tjek om patient og lægemiddel blev fundet
        if (patient == null || lægemiddel == null)
        {
            throw new ArgumentException("Ugyldig patient eller lægemiddel.");
        }

        // Beregn den anbefalede dosis
        String vægtKlasse = bestemVægtKlasse(patient.vaegt);
        double dosis = 0;

        switch (vægtKlasse)
        {
            case "Under 25":
                dosis = patient.vaegt * lægemiddel.enhedPrKgPrDoegnLet;
                break;
            case "Normal":
                dosis = patient.vaegt * lægemiddel.enhedPrKgPrDoegnNormal;
                break;
            case "Over 120":
                dosis = patient.vaegt * lægemiddel.enhedPrKgPrDoegnTung;
                break;
            default:
                throw new InvalidOperationException("Ugyldig vægtklasse.");
        }

        return dosis;
    }

}