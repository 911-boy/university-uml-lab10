using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversityIS
{
    /// <summary>Учебный курс: уникальный номер, название, часов в неделю.</summary>
    public class Course
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public float WeeklyDuration { get; private set; }

        // Обратный конец ассоциации Lecturer "teaches" Course.
        private List<Lecturer> lecturers = new List<Lecturer>();

        public Course(int id, string name, float weeklyDuration)
        {
            if (id <= 0)
                throw new ArgumentException("Номер курса должен быть положительным.");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название курса не может быть пустым.");
            if (weeklyDuration <= 0)
                throw new ArgumentException("Часов в неделю должно быть больше нуля.");
            Id = id;
            Name = name.Trim();
            WeeklyDuration = weeklyDuration;
        }

        public void AddLecturer(Lecturer lecturer)
        {
            if (lecturer == null)
                throw new ArgumentNullException("lecturer");
            if (lecturers.Contains(lecturer))
                return;
            lecturers.Add(lecturer);
            lecturer.Teach(this);
        }

        public IList<Lecturer> GetLecturers()
        {
            return lecturers.AsReadOnly();
        }

        public string GetInfo()
        {
            return string.Format("Курс {0} «{1}», {2} ч/нед, ведут: {3}", Id, Name, WeeklyDuration,
                lecturers.Count == 0 ? "(нет преподавателя)" : string.Join(", ", lecturers.Select(l => l.Name).ToArray()));
        }
    }
}
