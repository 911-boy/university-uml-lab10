using System;

namespace UniversityIS
{
    /// <summary>Сотрудник университета (абстрактный класс).</summary>
    public abstract class Employee
    {
        // Общее число сотрудников (number of employees) — статический счётчик.
        private static int counter = 0;

        public int SsNo { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }

        protected Employee(int ssNo, string name, string email)
        {
            if (ssNo <= 0)
                throw new ArgumentException("Социальный код должен быть положительным числом.");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Имя сотрудника не может быть пустым.");
            if (email == null || !email.Contains("@"))
                throw new ArgumentException("Некорректный e-mail: " + email);

            SsNo = ssNo;
            Name = name.Trim();
            Email = email.Trim();
            counter++;
        }

        public static int GetCounter()
        {
            return counter;
        }

        public abstract string GetPosition();

        public virtual string GetInfo()
        {
            return string.Format("[{0}] {1}, соц. код {2}, {3}", GetPosition(), Name, SsNo, Email);
        }
    }
}
