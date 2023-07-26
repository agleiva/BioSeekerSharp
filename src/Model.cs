record Sequence(string Header, string Data)
{
    private const int CodonSize = 3;
    private const int BicodonSize = 6;

    // Get codons from sequence (fragments of size CodonSize)
    public string[] GetCodons(int ORF) => GetFragments(ORF, CodonSize);

    // Get bicodons from sequence (fragments of size BicodonSize)
    public string[] GetBicodons(int ORF) => GetFragments(ORF, BicodonSize);

    // Get fragments from sequence
    private string[] GetFragments(int ORF, int size) =>
        // start reading Data string at character index ORF
        Data.Skip(ORF)

            // split the string in "chunks" of size 'size'
            .Chunk(size)
            
            // create a string from each one of these char chunks: ['A','B','C'] becomes "ABC"
            .Select(chars => new string(chars))
            
            // return all the results as a string[] array 
            .ToArray();
}

record ReferenceCodon(string Codon)
{
    public int History { get; set; }
    public int Rate { get; set; }
}

record CodonConservationRate(string Codon, int ReferenceCount, int ConservationCount);