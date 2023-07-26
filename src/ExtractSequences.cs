static class ExtractSequences
{
    /// <summary>
    /// Function to extract sequences from FASTA files
    /// This function takes as an argument a file path from the list created in program.cs
    /// to open it and make a sorting of the lines present in it.
    /// </summary>
    /// <param name="file">file that contains sequences to be sorted</param>
    /// <exception> Exception: Invalid file extension. The function only runs with FASTA files.</exception>
    /// <exception> Exception: Unexpected error encountered when reading the file</exception>
    /// <returns>array of Sequence objects representing the species names and the aligned sequences</returns>
    internal static Sequence[] FromFile(string file)
    {
        var result = new List<Sequence>();

        var extension = Path.GetExtension(file);
        if (extension != ".afa")
            throw new Exception($"Error: invalid file extension '{extension}', only '.afa' file extension is supported. Have you modified 'program.cs'?");

        file = Path.GetFullPath(file);

        var lines = File.ReadAllLines(file);

        string currentHeader = "";
        foreach (var line in lines)
            if (line.StartsWith(">"))
                currentHeader = line.TrimStart('>');
            else
                result.Add(new Sequence(currentHeader, line.Trim()));

        return result.ToArray();
    }
}
