using System;
using CsvHelper.Configuration.Attributes;

namespace gitdowntonight.Models
{
    public class Contribution : IEquatable<Contribution>
    {
        [Index(0)]
        public string Name { get; set; }
        [Index(1)]
        public long NumberOfContributions { get; set; }


        //Override the equals stuff so we can use .SequenceEqual when comparing what is in the txt file
        public bool Equals(Contribution other)
        {
            if (other is null)
                return false;

            return this.Name == other.Name && this.NumberOfContributions == other.NumberOfContributions;
        }

        public override bool Equals(object obj) => Equals(obj as Contribution);
        public override int GetHashCode() => (Name, NumberOfContributions).GetHashCode();
    }
}