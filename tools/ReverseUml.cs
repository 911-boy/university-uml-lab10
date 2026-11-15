// Обратная разработка (reverse engineering): читает скомпилированную сборку
// через рефлексию и строит по ней диаграмму классов PlantUML.
// Запуск: ReverseUml.exe <сборка.exe> <пространство имён> <выход.puml>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UniversityIS.Uml;

namespace ReverseUml
{
    public static class Program
    {
        private const BindingFlags Declared =
            BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.Public | BindingFlags.NonPublic;

        public static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            if (args.Length < 3)
            {
                Console.WriteLine("Использование: ReverseUml.exe <сборка> <namespace> <выход.puml>");
                return 1;
            }
            Assembly asm = Assembly.LoadFrom(args[0]);
            string ns = args[1];
            List<Type> types = asm.GetTypes()
                .Where(t => t.Namespace == ns && !IsCompilerGenerated(t) && !t.IsNested)
                .OrderBy(t => t.Name)
                .ToList();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("@startuml");
            sb.AppendLine("' Сгенерировано автоматически ReverseUml.exe из " + Path.GetFileName(args[0]) + " — не править руками");
            sb.AppendLine("title Диаграмма классов (обратная разработка из " + Path.GetFileName(args[0]) + ")");
            sb.AppendLine("skinparam classAttributeIconSize 0");
            sb.AppendLine("skinparam shadowing false");
            sb.AppendLine("skinparam linetype ortho");
            sb.AppendLine();

            foreach (Type t in types)
                WriteType(sb, t, types);

            sb.AppendLine();
            foreach (Type t in types)
                WriteRelations(sb, t, types);

            sb.AppendLine("@enduml");
            File.WriteAllText(args[2], sb.ToString(), new UTF8Encoding(false));
            Console.WriteLine("Классов: " + types.Count + ". Модель записана в " + args[2]);
            return 0;
        }

        private static void WriteType(StringBuilder sb, Type t, List<Type> model)
        {
            if (t.IsEnum)
            {
                sb.AppendLine("enum " + t.Name + " {");
                foreach (string n in Enum.GetNames(t))
                    sb.AppendLine("  " + n);
                sb.AppendLine("}");
                sb.AppendLine();
                return;
            }

            string head;
            if (t.IsAbstract && t.IsSealed) head = "class " + t.Name + " <<static>>";
            else if (t.IsAbstract) head = "abstract class " + t.Name;
            else head = "class " + t.Name;
            if (t.GetCustomAttributes(typeof(AssociationClassAttribute), false).Length > 0)
                head += " <<association class>>";
            sb.AppendLine(head + " {");

            // Атрибуты: поля и свойства, кроме концов отношений (они рисуются линиями).
            foreach (FieldInfo f in t.GetFields(Declared))
            {
                if (IsCompilerGenerated(f) || f.Name.Contains("<")) continue;
                if (RefersToModelClass(f.FieldType, model)) continue;
                sb.AppendLine("  " + (f.IsStatic ? "{static} " : "") + Visibility(f) + f.Name + " : " + TypeName(f.FieldType));
            }
            foreach (PropertyInfo p in t.GetProperties(Declared))
            {
                if (RefersToModelClass(p.PropertyType, model)) continue;
                MethodInfo getter = p.GetGetMethod(true);
                bool isStatic = getter != null && getter.IsStatic;
                sb.AppendLine("  " + (isStatic ? "{static} " : "") + Visibility(getter) + p.Name + " : " + TypeName(p.PropertyType));
            }

            // Операции: конструкторы и методы (без служебных get_/set_).
            foreach (ConstructorInfo c in t.GetConstructors(Declared))
            {
                if (c.IsStatic || c.IsPrivate) continue;
                sb.AppendLine("  " + Visibility(c) + t.Name + "(" + Params(c) + ")");
            }
            foreach (MethodInfo m in t.GetMethods(Declared))
            {
                if (m.IsSpecialName || IsCompilerGenerated(m) || m.Name.Contains("<")) continue;
                string mods = (m.IsStatic ? "{static} " : "") + (m.IsAbstract ? "{abstract} " : "");
                sb.AppendLine("  " + mods + Visibility(m) + m.Name + "(" + Params(m) + ") : " + TypeName(m.ReturnType));
            }
            sb.AppendLine("}");
            sb.AppendLine();
        }

        private static void WriteRelations(StringBuilder sb, Type t, List<Type> model)
        {
            if (t.BaseType != null && model.Contains(t.BaseType))
                sb.AppendLine(t.BaseType.Name + " <|-- " + t.Name);

            foreach (FieldInfo f in t.GetFields(Declared))
            {
                RelationAttribute rel = (RelationAttribute)Attribute.GetCustomAttribute(f, typeof(RelationAttribute));
                if (rel == null) continue;
                Type target = ElementType(f.FieldType);
                string arrow = rel.Kind == RelationKind.Composition ? "*--"
                             : rel.Kind == RelationKind.Aggregation ? "o--" : "--";
                string line = t.Name + " \"" + rel.OwnerMultiplicity + "\" " + arrow + " \""
                              + rel.TargetMultiplicity + "\" " + target.Name;
                if (!string.IsNullOrEmpty(rel.Label))
                    line += " : " + rel.Label;
                sb.AppendLine(line);
            }

            AssociationClassAttribute ac = (AssociationClassAttribute)
                Attribute.GetCustomAttribute(t, typeof(AssociationClassAttribute));
            if (ac != null)
            {
                sb.AppendLine(ac.Left.Name + " \"" + ac.LeftMultiplicity + "\" -- \"" + ac.RightMultiplicity + "\" " + ac.Right.Name);
                sb.AppendLine("(" + ac.Left.Name + ", " + ac.Right.Name + ") .. " + t.Name);
            }
        }

        // ---------- вспомогательное ----------

        private static bool IsCompilerGenerated(MemberInfo m)
        {
            return m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false).Length > 0;
        }

        private static Type ElementType(Type t)
        {
            if (t.IsGenericType && t.GetGenericArguments().Length == 1)
                return t.GetGenericArguments()[0];
            return t;
        }

        private static bool RefersToModelClass(Type t, List<Type> model)
        {
            Type e = ElementType(t);
            return model.Contains(e) && !e.IsEnum;
        }

        private static string Visibility(MethodBase m)
        {
            if (m == null) return "";
            if (m.IsPublic) return "+";
            if (m.IsFamily) return "#";
            if (m.IsAssembly) return "~";
            return "-";
        }

        private static string Visibility(FieldInfo f)
        {
            if (f.IsPublic) return "+";
            if (f.IsFamily) return "#";
            if (f.IsAssembly) return "~";
            return "-";
        }

        private static string Params(MethodBase m)
        {
            return string.Join(", ", m.GetParameters().Select(p => p.Name + " : " + TypeName(p.ParameterType)).ToArray());
        }

        private static string TypeName(Type t)
        {
            if (t == typeof(void)) return "void";
            if (t == typeof(int)) return "int";
            if (t == typeof(float)) return "float";
            if (t == typeof(bool)) return "bool";
            if (t == typeof(string)) return "String";
            if (t == typeof(DateTime)) return "Date";
            if (t.IsArray) return TypeName(t.GetElementType()) + "[]";
            if (t.IsGenericType)
            {
                string n = t.Name.Substring(0, t.Name.IndexOf('`'));
                return n + "<" + string.Join(", ", t.GetGenericArguments().Select(TypeName).ToArray()) + ">";
            }
            return t.Name;
        }
    }
}
