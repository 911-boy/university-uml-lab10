using System;
using UniversityIS.Uml;

namespace UniversityIS
{
    /// <summary>Класс-ассоциация: участие научного сотрудника в проекте на hours часов.</summary>
    [AssociationClass(typeof(ResearchAssociate), "1..*", typeof(Project), "0..*")]
    public class Participation
    {
        public int Hours { get; private set; }
        public ResearchAssociate Researcher { get; private set; }
        public Project Project { get; private set; }

        public Participation(ResearchAssociate researcher, Project project, int hours)
        {
            if (researcher == null)
                throw new ArgumentNullException("researcher");
            if (project == null)
                throw new ArgumentNullException("project");
            if (hours <= 0)
                throw new ArgumentException("Число часов должно быть больше нуля.");
            Researcher = researcher;
            Project = project;
            Hours = hours;
        }
    }
}
