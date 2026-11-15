using System;
using System.Collections.Generic;
using UniversityIS.Uml;

namespace UniversityIS
{
    /// <summary>Кафедра: название и адрес.</summary>
    public class Institute
    {
        public string Name { get; private set; }
        public string Address { get; private set; }

        [Relation(RelationKind.Aggregation, "1..*", "1..*")]
        private List<ResearchAssociate> researchers = new List<ResearchAssociate>();

        public Institute(string name, string address)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название кафедры не может быть пустым.");
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Адрес кафедры не может быть пустым.");
            Name = name.Trim();
            Address = address.Trim();
        }

        public void AddResearcher(ResearchAssociate researcher)
        {
            if (researcher == null)
                throw new ArgumentNullException("researcher");
            if (researchers.Contains(researcher))
                return;
            researchers.Add(researcher);
            researcher.AttachTo(this);
        }

        public IList<ResearchAssociate> GetResearchers()
        {
            return researchers.AsReadOnly();
        }
    }
}
