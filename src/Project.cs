using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversityIS
{
    /// <summary>Состояния проекта (см. диаграмму состояний).</summary>
    public enum ProjectState
    {
        Planned,
        Active,
        Completed,
        Cancelled
    }

    /// <summary>Научный проект: название, даты начала и окончания, состояние.</summary>
    public class Project
    {
        public string Name { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public ProjectState State { get; private set; }

        // Обратный конец класса-ассоциации Participation.
        private List<Participation> participations = new List<Participation>();

        public Project(string name, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название проекта не может быть пустым.");
            if (endDate < startDate)
                throw new ArgumentException("Дата окончания раньше даты начала.");
            Name = name.Trim();
            StartDate = startDate;
            EndDate = endDate;
            State = ProjectState.Planned;
        }

        /// <summary>Planned → Active.</summary>
        public void Start()
        {
            if (State != ProjectState.Planned)
                throw new InvalidOperationException("Начать можно только запланированный проект (сейчас " + State + ").");
            State = ProjectState.Active;
        }

        /// <summary>Active → Completed. Нужен хотя бы один участник.</summary>
        public void Finish()
        {
            if (State != ProjectState.Active)
                throw new InvalidOperationException("Завершить можно только активный проект (сейчас " + State + ").");
            if (participations.Count == 0)
                throw new InvalidOperationException("Нельзя завершить проект без участников.");
            State = ProjectState.Completed;
        }

        /// <summary>Planned/Active → Cancelled.</summary>
        public void Cancel()
        {
            if (State == ProjectState.Completed || State == ProjectState.Cancelled)
                throw new InvalidOperationException("Проект уже закрыт (" + State + ").");
            State = ProjectState.Cancelled;
        }

        public bool CanAcceptParticipants()
        {
            return State == ProjectState.Planned || State == ProjectState.Active;
        }

        public void AddParticipation(Participation participation)
        {
            if (participation == null || participation.Project != this)
                throw new ArgumentException("Участие относится к другому проекту.");
            if (!CanAcceptParticipants())
                throw new InvalidOperationException("Проект закрыт для новых участников.");
            if (!participations.Contains(participation))
                participations.Add(participation);
        }

        public IList<Participation> GetParticipations()
        {
            return participations.AsReadOnly();
        }

        public int GetTotalHours()
        {
            return participations.Sum(p => p.Hours);
        }

        public string GetInfo()
        {
            string info = string.Format("Проект «{0}» {1:dd.MM.yyyy}–{2:dd.MM.yyyy}, состояние: {3}, часов всего: {4}",
                Name, StartDate, EndDate, State, GetTotalHours());
            foreach (Participation p in participations)
                info += "\n      - " + p.Researcher.Name + ": " + p.Hours + " ч";
            return info;
        }
    }
}
