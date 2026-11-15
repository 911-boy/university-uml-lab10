using System;

namespace UniversityIS.Uml
{
    /// <summary>Вид отношения между классами (для обратной разработки модели из кода).</summary>
    public enum RelationKind
    {
        Association,
        Aggregation,
        Composition
    }

    /// <summary>
    /// Помечает поле-ссылку (или список ссылок) как конец UML-отношения.
    /// Владелец поля — «целое» для агрегации/композиции.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class RelationAttribute : Attribute
    {
        public RelationKind Kind { get; private set; }
        public string OwnerMultiplicity { get; private set; }
        public string TargetMultiplicity { get; private set; }
        public string Label { get; set; }

        public RelationAttribute(RelationKind kind, string ownerMultiplicity, string targetMultiplicity)
        {
            Kind = kind;
            OwnerMultiplicity = ownerMultiplicity;
            TargetMultiplicity = targetMultiplicity;
        }
    }

    /// <summary>Помечает класс как класс-ассоциацию между двумя классами.</summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AssociationClassAttribute : Attribute
    {
        public Type Left { get; private set; }
        public Type Right { get; private set; }
        public string LeftMultiplicity { get; private set; }
        public string RightMultiplicity { get; private set; }

        public AssociationClassAttribute(Type left, string leftMultiplicity, Type right, string rightMultiplicity)
        {
            Left = left;
            Right = right;
            LeftMultiplicity = leftMultiplicity;
            RightMultiplicity = rightMultiplicity;
        }
    }
}
