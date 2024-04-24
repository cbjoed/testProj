namespace shared.Model;

public class PN : Ordination
{
    public double antalEnheder { get; set; }
    public List<Dato> dates { get; set; } = new List<Dato>();

    public PN(DateTime startDen, DateTime slutDen, double antalEnheder, Laegemiddel laegemiddel) : base(laegemiddel, startDen, slutDen)
    {
        this.antalEnheder = antalEnheder;
    }

    public PN() : base(null!, new DateTime(), new DateTime())
    {
    }

    /// <summary>
    /// Registrerer at der er givet en dosis på dagen givesDen
    /// Returnerer true hvis givesDen er inden for ordinationens gyldighedsperiode og datoen huskes
    /// Returner false ellers og datoen givesDen ignoreres
    /// </summary>
    ///

    public bool givDosis(Dato givesDen)
    {
        // Tjek om givesDen er inden for ordinationens gyldighedsperiode
        if (givesDen.dato >= startDen && givesDen.dato <= slutDen)
        {
            // Tilføj givesDen til listen over dosis datoer
            dates.Add(givesDen);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override double doegnDosis()
    {
        if (dates.Count == 0)
        {
            return 0; // Hvis der ikke er givet nogen doser, er døgndosis 0
        }
        else
        {
            // Opret en dictionary til at holde styr på total dosis for hver dag
            Dictionary<DateTime, double> doserPerDag = new Dictionary<DateTime, double>();

            // Beregn total dosis for hver dag
            foreach (var dato in dates)
            {
                // Hvis datoen allerede eksisterer i dictionary, læg dosis til den eksisterende værdi
                if (doserPerDag.ContainsKey(dato.dato.Date))
                {
                    doserPerDag[dato.dato.Date] += this.antalEnheder;
                }
                // Ellers tilføj datoen til dictionary med dosis som værdi
                else
                {
                    doserPerDag.Add(dato.dato.Date, this.antalEnheder);
                }
            }

            // Tæl antallet af unikke dage
            int antalUnikkeDage = doserPerDag.Count;

            // Beregn og returner den gennemsnitlige døgndosis
            double totalDosis = doserPerDag.Sum(kv => kv.Value);
            double døgndosis = totalDosis / antalUnikkeDage;

            return døgndosis;
        }
    }



    public override double samletDosis()
    {
        return dates.Count() * antalEnheder;
    }

    public int getAntalGangeGivet()
    {
        return dates.Count();
    }

    public override String getType()
    {
        return "PN";
    }
}
