using System;
using System.Globalization;
using System.IO;
using UniversityIS.Uml;

namespace UniversityIS
{
    /// <summary>Консольный интерфейс: каждый пункт меню — один прецедент (use case).</summary>
    public class ConsoleUI
    {
        [Relation(RelationKind.Association, "1", "1", Label = "manages >")]
        private University university;

        private TextReader input;
        private TextWriter output;

        public ConsoleUI(University university, TextReader input, TextWriter output)
        {
            this.university = university;
            this.input = input;
            this.output = output;
        }

        public void Run()
        {
            while (true)
            {
                PrintMenu();
                string choice = ReadLine("Выберите пункт");
                if (choice == null || choice == "0")
                {
                    output.WriteLine("Работа завершена.");
                    return;
                }
                try
                {
                    switch (choice)
                    {
                        case "1": AddFaculty(); break;
                        case "2": AddInstitute(); break;
                        case "3": HireEmployee(); break;
                        case "4": AppointDean(); break;
                        case "5": AttachToInstitute(); break;
                        case "6": CreateProject(); break;
                        case "7": IncludeInProject(); break;
                        case "8": ChangeProjectState(); break;
                        case "9": CreateCourse(); break;
                        case "10": AssignLecturer(); break;
                        case "11": ShowReport(); break;
                        case "12": LoadDemo(); break;
                        default: output.WriteLine("Нет такого пункта."); break;
                    }
                }
                catch (Exception ex)
                {
                    // Ошибки ввода и нарушения правил модели не роняют программу.
                    output.WriteLine("ОШИБКА: " + ex.Message);
                }
                output.WriteLine();
            }
        }

        private void PrintMenu()
        {
            output.WriteLine("---------- " + university.Name + " ----------");
            output.WriteLine(" 1. Добавить факультет");
            output.WriteLine(" 2. Добавить кафедру на факультет");
            output.WriteLine(" 3. Принять сотрудника");
            output.WriteLine(" 4. Назначить декана");
            output.WriteLine(" 5. Прикрепить научного сотрудника к кафедре");
            output.WriteLine(" 6. Создать проект");
            output.WriteLine(" 7. Включить научного сотрудника в проект");
            output.WriteLine(" 8. Изменить состояние проекта");
            output.WriteLine(" 9. Создать курс");
            output.WriteLine("10. Назначить преподавателя на курс");
            output.WriteLine("11. Показать отчёт");
            output.WriteLine("12. Загрузить демо-данные");
            output.WriteLine(" 0. Выход");
        }

        // ---------- Прецеденты ----------

        public void AddFaculty()
        {
            Faculty f = university.AddFaculty(ReadLine("Название факультета"));
            output.WriteLine("Факультет «" + f.Name + "» добавлен.");
        }

        public void AddInstitute()
        {
            Faculty f = RequireFaculty(ReadLine("Факультет"));
            Institute i = f.AddInstitute(ReadLine("Название кафедры"), ReadLine("Адрес кафедры"));
            output.WriteLine("Кафедра «" + i.Name + "» добавлена на факультет " + f.Name + ".");
        }

        public void HireEmployee()
        {
            string type = ReadLine("Тип (1 — административный, 2 — научный, 3 — преподаватель)");
            int ssNo = ReadInt("Социальный код");
            if (university.FindEmployee(ssNo) != null)
                throw new InvalidOperationException("Сотрудник с соц. кодом " + ssNo + " уже есть.");
            string name = ReadLine("Имя");
            string email = ReadLine("E-mail");
            if (email == null || !email.Contains("@"))
                throw new FormatException("Некорректный e-mail: " + email);

            Employee employee;
            if (type == "1")
            {
                employee = new AdministrativeEmployee(ssNo, name, email);
            }
            else if (type == "2" || type == "3")
            {
                string field = ReadLine("Область исследований");
                Institute institute = RequireInstitute(ReadLine("Факультет кафедры"), ReadLine("Кафедра"));
                if (type == "2")
                    employee = new ResearchAssociate(ssNo, name, email, field, institute);
                else
                    employee = new Lecturer(ssNo, name, email, field, institute);
            }
            else
            {
                throw new ArgumentException("Неизвестный тип сотрудника: " + type);
            }
            university.Hire(employee);
            output.WriteLine("Принят: " + employee.GetInfo());
        }

        public void AppointDean()
        {
            Faculty f = RequireFaculty(ReadLine("Факультет"));
            Employee e = RequireEmployee(ReadInt("Соц. код будущего декана"));
            university.AppointDean(f, e);
            output.WriteLine(e.Name + " назначен(а) деканом факультета " + f.Name + ".");
        }

        public void AttachToInstitute()
        {
            ResearchAssociate ra = RequireResearcher(ReadInt("Соц. код научного сотрудника"));
            Institute i = RequireInstitute(ReadLine("Факультет кафедры"), ReadLine("Кафедра"));
            ra.AttachTo(i);
            output.WriteLine(ra.Name + " прикреплён(а) к кафедре «" + i.Name + "».");
        }

        public void CreateProject()
        {
            string name = ReadLine("Название проекта");
            DateTime start = ReadDate("Дата начала (дд.мм.гггг)");
            DateTime end = ReadDate("Дата окончания (дд.мм.гггг)");
            Project p = new Project(name, start, end);
            university.AddProject(p);
            output.WriteLine("Проект «" + p.Name + "» создан, состояние: " + p.State + ".");
        }

        /// <summary>Прецедент «Включить научного сотрудника в проект» (см. диаграмму последовательностей).</summary>
        public void IncludeInProject()
        {
            ResearchAssociate ra = RequireResearcher(ReadInt("Соц. код научного сотрудника"));
            Project p = RequireProject(ReadLine("Проект"));
            int hours = ReadInt("Часов в проекте");
            Participation part = ra.JoinProject(p, hours);
            output.WriteLine(ra.Name + " включён(а) в проект «" + p.Name + "» на " + part.Hours + " ч.");
        }

        public void ChangeProjectState()
        {
            Project p = RequireProject(ReadLine("Проект"));
            output.WriteLine("Сейчас: " + p.State);
            string action = ReadLine("Действие (1 — начать, 2 — завершить, 3 — отменить)");
            if (action == "1") p.Start();
            else if (action == "2") p.Finish();
            else if (action == "3") p.Cancel();
            else throw new ArgumentException("Неизвестное действие: " + action);
            output.WriteLine("Проект «" + p.Name + "» теперь в состоянии " + p.State + ".");
        }

        public void CreateCourse()
        {
            int id = ReadInt("Уникальный номер курса");
            string name = ReadLine("Название курса");
            float weekly = ReadFloat("Часов в неделю");
            Lecturer lecturer = RequireLecturer(ReadInt("Соц. код преподавателя"));
            Course c = new Course(id, name, weekly);
            university.AddCourse(c);
            lecturer.Teach(c);
            output.WriteLine("Создан: " + c.GetInfo());
        }

        public void AssignLecturer()
        {
            Course c = RequireCourse(ReadInt("Номер курса"));
            Lecturer lecturer = RequireLecturer(ReadInt("Соц. код преподавателя"));
            lecturer.Teach(c);
            output.WriteLine("Назначен: " + c.GetInfo());
        }

        public void ShowReport()
        {
            university.PrintReport(output);
        }

        /// <summary>Тестовые данные (для быстрой проверки, не прецедент).</summary>
        public void LoadDemo()
        {
            Faculty fcs = university.AddFaculty("ФКН");
            Institute prog = fcs.AddInstitute("Программирования и ИТ", "Университетская пл., 1, корп. 1а, ауд. 290");
            Institute infsys = fcs.AddInstitute("Информационных систем", "Университетская пл., 1, корп. 1а, ауд. 305");
            Faculty math = university.AddFaculty("Математический");
            Institute analysis = math.AddInstitute("Математического анализа", "Университетская пл., 1, ауд. 333");

            AdministrativeEmployee admin = new AdministrativeEmployee(1001, "Иванова Ольга", "ivanova@uni.example");
            Lecturer petrov = new Lecturer(2001, "Петров Сергей", "petrov@uni.example", "Проектирование ИС", infsys);
            Lecturer orlova = new Lecturer(2002, "Орлова Мария", "orlova@uni.example", "Базы данных", prog);
            ResearchAssociate kozlov = new ResearchAssociate(3001, "Козлов Игорь", "kozlov@uni.example", "Машинное обучение", prog);
            kozlov.AttachTo(analysis);
            university.Hire(admin);
            university.Hire(petrov);
            university.Hire(orlova);
            university.Hire(kozlov);

            university.AppointDean(fcs, petrov);
            university.AppointDean(math, admin);

            Project campus = new Project("Цифровой кампус", new DateTime(2026, 2, 1), new DateTime(2026, 12, 31));
            Project ai = new Project("ИИ в образовании", new DateTime(2027, 1, 15), new DateTime(2027, 6, 30));
            university.AddProject(campus);
            university.AddProject(ai);
            campus.Start();
            kozlov.JoinProject(campus, 120);
            petrov.JoinProject(campus, 40);
            orlova.JoinProject(ai, 60);

            Course pis = new Course(101, "Проектирование информационных систем", 4f);
            Course db = new Course(102, "Базы данных", 3f);
            university.AddCourse(pis);
            university.AddCourse(db);
            petrov.Teach(pis);
            orlova.Teach(db);
            orlova.Teach(pis);

            output.WriteLine("Демо-данные загружены: 2 факультета, 3 кафедры, 4 сотрудника, 2 проекта, 2 курса.");
        }

        // ---------- Поиск с проверкой ----------

        private Faculty RequireFaculty(string name)
        {
            Faculty f = university.FindFaculty(name);
            if (f == null)
                throw new InvalidOperationException("Факультет «" + name + "» не найден.");
            return f;
        }

        private Institute RequireInstitute(string facultyName, string instituteName)
        {
            Institute i = RequireFaculty(facultyName).FindInstitute(instituteName);
            if (i == null)
                throw new InvalidOperationException("Кафедра «" + instituteName + "» не найдена.");
            return i;
        }

        private Employee RequireEmployee(int ssNo)
        {
            Employee e = university.FindEmployee(ssNo);
            if (e == null)
                throw new InvalidOperationException("Сотрудник с соц. кодом " + ssNo + " не найден.");
            return e;
        }

        private ResearchAssociate RequireResearcher(int ssNo)
        {
            ResearchAssociate ra = RequireEmployee(ssNo) as ResearchAssociate;
            if (ra == null)
                throw new InvalidOperationException("Сотрудник " + ssNo + " не научный сотрудник.");
            return ra;
        }

        private Lecturer RequireLecturer(int ssNo)
        {
            Lecturer l = RequireEmployee(ssNo) as Lecturer;
            if (l == null)
                throw new InvalidOperationException("Сотрудник " + ssNo + " не преподаватель.");
            return l;
        }

        private Project RequireProject(string name)
        {
            Project p = university.FindProject(name);
            if (p == null)
                throw new InvalidOperationException("Проект «" + name + "» не найден.");
            return p;
        }

        private Course RequireCourse(int id)
        {
            Course c = university.FindCourse(id);
            if (c == null)
                throw new InvalidOperationException("Курс с номером " + id + " не найден.");
            return c;
        }

        // ---------- Ввод ----------

        private string ReadLine(string prompt)
        {
            output.Write(prompt + ": ");
            string line = input.ReadLine();
            if (line == null)
                return null;
            line = line.Trim();
            if (input != Console.In || Console.IsInputRedirected)
                output.WriteLine(line); // эхо при вводе из файла, чтобы протокол был читаемым
            return line;
        }

        private int ReadInt(string prompt)
        {
            string s = ReadLine(prompt);
            int value;
            if (!int.TryParse(s, out value))
                throw new FormatException("Ожидалось целое число, введено: «" + s + "».");
            return value;
        }

        private float ReadFloat(string prompt)
        {
            string s = ReadLine(prompt);
            float value;
            if (s == null || !float.TryParse(s.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                throw new FormatException("Ожидалось число, введено: «" + s + "».");
            return value;
        }

        private DateTime ReadDate(string prompt)
        {
            string s = ReadLine(prompt);
            DateTime value;
            if (!DateTime.TryParseExact(s, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
                throw new FormatException("Дата должна быть в формате дд.мм.гггг, введено: «" + s + "».");
            return value;
        }
    }
}
