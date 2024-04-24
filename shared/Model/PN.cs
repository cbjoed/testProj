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
            // Find antallet af dage mellem første og sidste givning
            TimeSpan span = dates.Last().dato.Date - dates.First().dato.Date;
            int antalDageMellemGivninger = span.Days + 1; // Inklusive både første og sidste dag

            // Beregn og returner den reelle døgndosis
            double døgndosis = (dates.Count * antalEnheder) / antalDageMellemGivninger;

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
