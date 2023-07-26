static class CalculateConservationRates
{
    /// <summary>
    /// Function to calculate codon and codon pair conservation rates
    /// </summary>
    /// <param name="sequences">An array of Sequence objects</param>
    /// <param name="ORF">Reading frame. Can only take values 0,1 or 2</param>
    public static ConservationRates FromSequences(Sequence[] sequences, int ORF)
    {
        // Validate ORF
        if (ORF is not >= 0 and <= 2)
            throw new Exception("Error: 'ORF' must be a value between 0 and 2.");

        // Get codons and bicodons matrixes from sequences starting at ORF
        var codonsMatrix   = sequences.Select(x => x.GetCodons  (ORF)).ToArray();
        var bicodonsMatrix = sequences.Select(x => x.GetBicodons(ORF)).ToArray();

        // Process both matrixes
        var codonRates   = Process(codonsMatrix,   GeneticCode.Codons);
        var bicodonRates = Process(bicodonsMatrix, GeneticCode.CodonPairs);

        // generate result
        return new ConservationRates(codonRates, bicodonRates);
    }

    private static CodonConservationRate[] Process(string[][] codonsMatrix, string[] geneticCode)
    {
        // create reference codons list, which is used to compute and store conservation rate
        var referenceCodons =
            codonsMatrix.First()
                        .Select(x => new ReferenceCodon(x))
                        .ToArray();

        // get row count from codons matrix
        var rowCount = codonsMatrix.GetLength(0);

        // iterate reference codons list
        for (int i = 0; i < referenceCodons.Length; i++)
        {
            // current item in the iteration
            var referenceCodon = referenceCodons[i];

            // get corresponding column from codons matrix
            var column =
                codonsMatrix.Select(x => x[i])
                            .ToList();

            // calculate history: count the number of repetitions of the same codon throughout the column
            referenceCodon.History = column.Count(x => x == referenceCodon.Codon);

            // calculate conservation rate
            referenceCodon.Rate =
                ((double)referenceCodon.History / rowCount) > 0.9d
                ? 1
                : 0;
        }

        // Calculate result
        var result =

            // iterate GeneticCode (codons or bicodons list, passed as parameter)
            from c in geneticCode

            // find matches in reference list (where codons in the reference list match the current codon in the GeneticCode list)
            let matching = referenceCodons.Where(x => x.Codon == c)

            // count the references
            let reference = matching.Count()

            // count the conservations (matching references where conservation rate > 0)
            let conservationCount = matching.Count(x => x.Rate > 0)

            // create a record with all these results
            select new CodonConservationRate(c, reference, conservationCount);

        // Materialize results into an array
        return result.ToArray();
    }
}