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

  

    public override double samletDosis()
    {
        return dates.Count() * antalEnheder;
    }

    public int getAntalGangeGivet()
    {
        return dates.Count();
    }


    public override double doegnDosis()
    {
        if (dates.Count == 0)
            return 0;

        DateTime min = dates.Min(d => d.dato);
        DateTime max = dates.Max(d => d.dato);

        int dage = (int)(max - min).TotalDays + 1;
        double gennemsnitligDosisPerDag = samletDosis() / dage;

        return gennemsnitligDosisPerDag;
    }


    public override String getType()
    {
        return "PN";
    }
}
