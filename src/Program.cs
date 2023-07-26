using System.Diagnostics;

Console.WriteLine(
    """
    BioSeekerSharp v0.0.1
    Author: fberasa
    Official repository: https://github.com/agleiva/BioSeekerSharp
    License: MIT License, see https://github.com/agleiva/BioSeekerSharp/blob/main/LICENSE
    Wiki: https://github.com/fx-biocoder/BioSeeker/wiki
    --------------------------------
    Press any key to start the analysis on the current directory...

    """);

Console.ReadKey();

// Start a stopwatch that will help us report the total process time when completed.
var stopwatch = Stopwatch.StartNew();

var basePath = Environment.CurrentDirectory;

Console.WriteLine($"Program initiated. Creating list of FASTA files in directory: {basePath}");
var files =
    Directory.GetFiles(basePath, "*.afa")
             .ToList();

Console.WriteLine($"List created successfully. {files.Count} files found.");

var sequences = new List<Sequence[]>();
var errors = new List<(string FileName, Exception Exception)>();

foreach (var file in files)
{
    try
    {
        Console.WriteLine($"Extracting alignments from file {Path.GetFileName(file)}");
        sequences.Add(ExtractSequences.FromFile(file));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error was found during the analysis of {file}. It will be added to bioseeker_errors.txt.\r\nError Details: {ex}");
        errors.Add((file, ex));
    }
}

if (sequences.Any())
{
    // Iterate ORF from 0 to 2
    for (int ORF = 0; ORF <= 2; ORF++)
    {
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine($"Calculating Conservation Rates for ORF: {ORF}");

        // Get all conservation rates for codons and bicodons from the sequences obtained from the files
        var conservationRates =
            sequences.Select(s => CalculateConservationRates.FromSequences(s, ORF))
                     .ToArray();

        // get all codon and bicodon conservation rates
        var codonRates   = conservationRates.SelectMany(x => x.CodonRates);
        var bicodonRates = conservationRates.SelectMany(x => x.BicodonRates);

        // aggregate all codon and bicodon conservation rates into a global result
        
        Console.WriteLine($"Aggregating codon conservation rates");
        var codonAggregate   = Aggregate.FromRates(codonRates)  .ToArray();
        
        Console.WriteLine($"Aggregating bicodon conservation rates");
        var bicodonAggregate = Aggregate.FromRates(bicodonRates).ToArray();

        // write final result into a codon and bicodon CSV file respectively, with the current ORF number.
        CsvWriter.WriteCodons  (codonAggregate,   ORF);
        CsvWriter.WriteBicodons(bicodonAggregate, ORF);
    }
}

var errorsFile = "bioseeker_errors.txt";
if (errors.Any())
{
    var errorLines =
        errors.Select(x => $"Error processing file: {x.FileName}. Error Details: {x.Exception}");

    Console.WriteLine("-------------------------------------------");
    Console.WriteLine($"Writing error details to file: {errorsFile}");
    File.WriteAllLines(errorsFile, errorLines);
}
else if (File.Exists(errorsFile))
{
    File.Delete(errorsFile);
}

Console.WriteLine("-------------------------------------------");
Console.WriteLine($"Process Completed in: {stopwatch.Elapsed}");
