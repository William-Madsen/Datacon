using System;
using System.Globalization;
using System.IO;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        string inputFil = "\Users\willi\OneDrive\Dokumenter\pricelist\prisliste.csv";
        string outputFil = "\Users\willi\OneDrive\Dokumenter\pricelist\prisliste.csv";

        //string inputFil = "/Users/williamlundmadsen/PycharmProjects/karakterbog/Datakonvertering/Prislist/pricelist.csv";
        //string outputFil = "/Users/williamlundmadsen/PycharmProjects/karakterbog/Datakonvertering/Prislist/prisliste.csv";

        if (!File.Exists(inputFil))
        {
            Console.WriteLine("Filen blev ikke fundet ;)");
            Console.Read();
            return;
        }

        var culture = new CultureInfo("da-DK");
        double eurToDkk = 7.46;

        string[] linjer = File.ReadAllLines(inputFil);
        List<string> nyeLinjer = new List<string>();

        for (int i = 0; i < linjer.Length; i++)
        {
            string linje = linjer[i];

            linje = linje.Replace("black", "Sort");
            linje = linje.Replace("item", "vare");
            linje = linje.Replace("article description", "beskrivelse");
            linje = linje.Replace("unit", "enhed");
            linje = linje.Replace("cost price", "kostpris");
            linje = linje.Replace("currency", "valuta");
            linje = linje.Replace("price unit", "prisenhed");
            linje = linje.Replace("price group", "prisgruppe");
            linje = linje.Replace("date of issuance", "udstedelsesdato");
            linje = linje.Replace("EUR", "DKK");

            var dele = linje.Split(';');

            if (dele.Length > 3 && dele[3].Contains("€"))
            {
                string prisStr = dele[3].Replace("€", "").Trim();
                if (double.TryParse(prisStr, NumberStyles.AllowDecimalPoint, culture, out double eur))
                {
                    double dkk = eur * eurToDkk;
                    dele[3] = dkk.ToString("0.00", culture) + " DKK";
                }
            }

            linje = string.Join(";", dele);
            dele = linje.Split(';');

            string salgsprisStr = "";
            if (dele.Length > 4)
            {
                string priceGroupStr = dele[4].Trim().Replace(",", ".");
                if (decimal.TryParse(priceGroupStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal priceGroupDecimal))
                {
                    int priceGroupInt = (int)priceGroupDecimal;
                    decimal percent = priceGroupInt switch
                    {
                        21 => 0.08m,
                        22 => 0.12m,
                        23 => 0.15m,
                        24 => 0.175m,
                        25 => 0.20m,
                        _ => 0m
                    };

                    if (decimal.TryParse(dele[3].Replace("DKK", "").Trim(), NumberStyles.Any, culture, out decimal kostpris))
                    {
                        decimal salgspris = Math.Round(kostpris * (1 + percent), 2);
                        salgsprisStr = salgspris.ToString("N2", new CultureInfo("da-DK")) + " DKK";
                    }
                }
            }

            var nyDele = new List<string>(dele);
            nyDele.Add(salgsprisStr);
            nyeLinjer.Add(string.Join(";", nyDele));

        }

        File.WriteAllLines(outputFil, nyeLinjer);
        Console.WriteLine("Færdig! Fil gemt som: " + outputFil);
        Console.Read();
    }
}