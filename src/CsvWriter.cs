using System.Globalization;

class CsvWriter
{
    public static void WriteCodons(Aggregate[] aggregates, int ORF) =>
        Write(aggregates, "Codons", ORF);

    public static void WriteBicodons(Aggregate[] aggregates, int ORF) =>
        Write(aggregates, "Bicodons", ORF);

    private static void Write(Aggregate[] aggregates, string filePrefix, int ORF)
    {
        var header = $"{filePrefix},ConservationCount,ReferenceCount,ConservationRate";

        var fileName = $"{filePrefix}_data_ORF+{ORF}.csv".ToLower();

        Console.WriteLine($"Writing {filePrefix} with ORF {ORF} to file: {fileName}");
        File.WriteAllLines(fileName, new[] { header });

        var fileLines =
            aggregates.Select(a => $"{a.Codon},{a.ConservationCount},{a.ReferenceCount},{a.ConservationRate.ToString(CultureInfo.InvariantCulture)}")
                      .ToArray();

        File.AppendAllLines(fileName, fileLines);
    }
}