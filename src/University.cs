using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniversityIS.Uml;

namespace UniversityIS
{
    /// <summary>Университет: факультеты, сотрудники, проекты и курсы.</summary>
    public class University
    {
        public string Name { get; private set; }

        [Relation(RelationKind.Composition, "1", "1..*")]
        private List<Faculty> faculties = new List<Faculty>();

        [Relation(RelationKind.Aggregation, "1", "0..*", Label = "employs >")]
        private List<Employee> employees = new List<Employee>();

        [Relation(RelationKind.Aggregation, "1", "0..*")]
        private List<Project> projects = new List<Project>();

        [Relation(RelationKind.Aggregation, "1", "0..*")]
        private List<Course> courses = new List<Course>();

        public University(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название университета не может быть пустым.");
            Name = name.Trim();
        }

        // ---------- Факультеты ----------
        public Faculty AddFaculty(string name)
        {
            if (FindFaculty(name) != null)
                throw new InvalidOperationException("Факультет «" + name + "» уже существует.");
            Faculty faculty = new Faculty(name);
            faculties.Add(faculty);
            return faculty;
        }

        public Faculty FindFaculty(string name)
        {
            if (name == null)
                return null;
            return faculties.FirstOrDefault(f => string.Equals(f.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public IList<Faculty> GetFaculties()
        {
            return faculties.AsReadOnly();
        }

        /// <summary>Сотрудник может руководить не более чем одним факультетом (0..1).</summary>
        public void AppointDean(Faculty faculty, Employee employee)
        {
            if (faculty == null || employee == null)
                throw new ArgumentNullException(faculty == null ? "faculty" : "employee");
            if (!employees.Contains(employee))
                throw new InvalidOperationException(employee.Name + " не является сотрудником университета.");
            Faculty other = faculties.FirstOrDefault(f => f != faculty && f.GetDean() == employee);
            if (other != null)
                throw new InvalidOperationException(employee.Name + " уже декан факультета " + other.Name + ".");
            faculty.SetDean(employee);
        }

        // ---------- Сотрудники ----------
        public void Hire(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException("employee");
            if (FindEmployee(employee.SsNo) != null)
                throw new InvalidOperationException("Сотрудник с соц. кодом " + employee.SsNo + " уже есть.");
            employees.Add(employee);
        }

        public Employee FindEmployee(int ssNo)
        {
            return employees.FirstOrDefault(e => e.SsNo == ssNo);
        }

        public IList<Employee> GetEmployees()
        {
            return employees.AsReadOnly();
        }

        public int GetNumberOfEmployees()
        {
            return employees.Count;
        }

        // ---------- Проекты ----------
        public void AddProject(Project project)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            if (FindProject(project.Name) != null)
                throw new InvalidOperationException("Проект «" + project.Name + "» уже существует.");
            projects.Add(project);
        }

        public Project FindProject(string name)
        {
            if (name == null)
                return null;
            return projects.FirstOrDefault(p => string.Equals(p.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public IList<Project> GetProjects()
        {
            return projects.AsReadOnly();
        }

        // ---------- Курсы ----------
        public void AddCourse(Course course)
        {
            if (course == null)
                throw new ArgumentNullException("course");
            if (FindCourse(course.Id) != null)
                throw new InvalidOperationException("Курс с номером " + course.Id + " уже существует.");
            courses.Add(course);
        }

        public Course FindCourse(int id)
        {
            return courses.FirstOrDefault(c => c.Id == id);
        }

        public IList<Course> GetCourses()
        {
            return courses.AsReadOnly();
        }

        // ---------- Отчёт ----------
        public void PrintReport(TextWriter output)
        {
            output.WriteLine("==================== " + Name + " ====================");
            output.WriteLine("Сотрудников в университете: " + GetNumberOfEmployees()
                + " (создано объектов Employee: " + Employee.GetCounter() + ")");
            output.WriteLine();
            output.WriteLine("ФАКУЛЬТЕТЫ И КАФЕДРЫ");
            if (faculties.Count == 0)
                output.WriteLine("  (нет)");
            foreach (Faculty f in faculties)
            {
                Employee dean = f.GetDean();
                output.WriteLine("  " + f.Name + " — декан: " + (dean == null ? "(не назначен)" : dean.Name));
                foreach (Institute i in f.GetInstitutes())
                {
                    output.WriteLine("    кафедра «" + i.Name + "», " + i.Address
                        + ", научных сотрудников: " + i.GetResearchers().Count);
                }
            }
            output.WriteLine();
            output.WriteLine("СОТРУДНИКИ");
            if (employees.Count == 0)
                output.WriteLine("  (нет)");
            foreach (Employee e in employees)
                output.WriteLine("  " + e.GetInfo());
            output.WriteLine();
            output.WriteLine("ПРОЕКТЫ");
            if (projects.Count == 0)
                output.WriteLine("  (нет)");
            foreach (Project p in projects)
                output.WriteLine("  " + p.GetInfo());
            output.WriteLine();
            output.WriteLine("КУРСЫ");
            if (courses.Count == 0)
                output.WriteLine("  (нет)");
            foreach (Course c in courses)
                output.WriteLine("  " + c.GetInfo());
            output.WriteLine(new string('=', 60));
        }
    }
}
