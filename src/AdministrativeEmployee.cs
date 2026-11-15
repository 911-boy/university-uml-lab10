namespace UniversityIS
{
    /// <summary>Административный сотрудник.</summary>
    public class AdministrativeEmployee : Employee
    {
        public AdministrativeEmployee(int ssNo, string name, string email)
            : base(ssNo, name, email)
        {
        }

        public override string GetPosition()
        {
            return "Административный сотрудник";
        }
    }
}
