using System;
using System.Collections.Generic;
using System.Linq;
using UniversityIS.Uml;

namespace UniversityIS
{
    /// <summary>Преподаватель — научный сотрудник, который ведёт учебные курсы.</summary>
    public class Lecturer : ResearchAssociate
    {
        [Relation(RelationKind.Association, "1..*", "1..*", Label = "teaches >")]
        private List<Course> courses = new List<Course>();

        public Lecturer(int ssNo, string name, string email, string fieldOfStudy, Institute institute)
            : base(ssNo, name, email, fieldOfStudy, institute)
        {
        }

        public void Teach(Course course)
        {
            if (course == null)
                throw new ArgumentNullException("course");
            if (courses.Contains(course))
                return;
            courses.Add(course);
            course.AddLecturer(this);
        }

        public IList<Course> GetCourses()
        {
            return courses.AsReadOnly();
        }

        public override string GetPosition()
        {
            return "Преподаватель";
        }

        public override string GetInfo()
        {
            string info = base.GetInfo();
            info += "\n      курсы: " + (courses.Count == 0
                ? "(пока не назначены)"
                : string.Join("; ", courses.Select(c => c.Id + " " + c.Name).ToArray()));
            return info;
        }
    }
}
