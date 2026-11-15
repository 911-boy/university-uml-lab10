using System;
using System.Collections.Generic;
using System.Linq;
using UniversityIS.Uml;

namespace UniversityIS
{
    /// <summary>Факультет: название, декан, кафедры (композиция).</summary>
    public class Faculty
    {
        public string Name { get; private set; }

        [Relation(RelationKind.Association, "0..1", "1 +dean", Label = "< leads")]
        private Employee dean;

        [Relation(RelationKind.Composition, "1", "1..*")]
        private List<Institute> institutes = new List<Institute>();

        public Faculty(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название факультета не может быть пустым.");
            Name = name.Trim();
        }

        /// <summary>Кафедра создаётся только внутри факультета (композиция).</summary>
        public Institute AddInstitute(string name, string address)
        {
            if (FindInstitute(name) != null)
                throw new InvalidOperationException("Кафедра «" + name + "» уже есть на факультете " + Name + ".");
            Institute institute = new Institute(name, address);
            institutes.Add(institute);
            return institute;
        }

        public Institute FindInstitute(string name)
        {
            if (name == null)
                return null;
            return institutes.FirstOrDefault(i => string.Equals(i.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public IList<Institute> GetInstitutes()
        {
            return institutes.AsReadOnly();
        }

        public void SetDean(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException("employee");
            dean = employee;
        }

        public Employee GetDean()
        {
            return dean;
        }
    }
}
