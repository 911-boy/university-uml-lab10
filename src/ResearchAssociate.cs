using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversityIS
{
    /// <summary>Научный сотрудник. Прикреплён минимум к одной кафедре, может участвовать в проектах.</summary>
    public class ResearchAssociate : Employee
    {
        public string FieldOfStudy { get; private set; }

        // Обратные концы отношений: агрегация Institute o-- ResearchAssociate
        // и класс-ассоциация Participation (ResearchAssociate -- Project).
        private List<Institute> institutes = new List<Institute>();
        private List<Participation> participations = new List<Participation>();

        public ResearchAssociate(int ssNo, string name, string email, string fieldOfStudy, Institute institute)
            : base(ssNo, name, email)
        {
            if (string.IsNullOrWhiteSpace(fieldOfStudy))
                throw new ArgumentException("Область исследований не может быть пустой.");
            if (institute == null)
                throw new ArgumentException("Научный сотрудник должен быть прикреплён хотя бы к одной кафедре.");
            FieldOfStudy = fieldOfStudy.Trim();
            AttachTo(institute);
        }

        public void AttachTo(Institute institute)
        {
            if (institute == null)
                throw new ArgumentNullException("institute");
            if (institutes.Contains(institute))
                return;
            institutes.Add(institute);
            institute.AddResearcher(this);
        }

        public IList<Institute> GetInstitutes()
        {
            return institutes.AsReadOnly();
        }

        /// <summary>Включает сотрудника в проект на заданное число часов.</summary>
        public Participation JoinProject(Project project, int hours)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            if (!project.CanAcceptParticipants())
                throw new InvalidOperationException(
                    "Проект «" + project.Name + "» в состоянии " + project.State + ": включать участников нельзя.");
            if (participations.Any(p => p.Project == project))
                throw new InvalidOperationException(Name + " уже участвует в проекте «" + project.Name + "».");

            Participation participation = new Participation(this, project, hours);
            participations.Add(participation);
            project.AddParticipation(participation);
            return participation;
        }

        public IList<Participation> GetParticipations()
        {
            return participations.AsReadOnly();
        }

        public int GetTotalHours()
        {
            return participations.Sum(p => p.Hours);
        }

        public override string GetPosition()
        {
            return "Научный сотрудник";
        }

        public override string GetInfo()
        {
            string info = base.GetInfo() + ", область: " + FieldOfStudy
                + "\n      кафедры: " + string.Join("; ", institutes.Select(i => i.Name).ToArray());
            if (participations.Count > 0)
                info += "\n      проекты: " + string.Join("; ",
                    participations.Select(p => p.Project.Name + " (" + p.Hours + " ч)").ToArray())
                    + ", всего " + GetTotalHours() + " ч";
            return info;
        }
    }
}
