record Aggregate(string Codon, int ReferenceCount, int ConservationCount, double ConservationRate)
{
    public static Aggregate[] FromRates(IEnumerable<CodonConservationRate> rates)
    {
        var aggregate =

            // iterate all conservation rates
            from r in rates

            // group them by codon
            group r by r.Codon
            
            //into a group variable named 'g'
            into g

            // sum the total number of references from the group
            let referenceCount = g.Sum(c => c.ReferenceCount)
            
            // sum the total number of conservation count from the group
            let conservationCount = g.Sum(c => c.ConservationCount)
            
            // calculate conservation rate
            let conservationRate =
                referenceCount > 0
                ? (double)conservationCount / referenceCount
                : 0d

            // create a record with all these results
            select new Aggregate(g.Key, referenceCount, conservationCount, conservationRate);

        // Materialize results into an array
        return aggregate.ToArray();
    }
}