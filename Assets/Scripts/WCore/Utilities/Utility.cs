namespace WCore {
  using System;
  using System.Linq;
  using System.Reflection;

  public static class Utility {

    public static FieldInfo[] GetInjects(Type type) {
      return (from field in type.GetFields()
              where field.GetCustomAttribute<InjectAttribute>() != null
              select field)
             .ToArray();
    }

    public static void SetField(object obj, string field, object value) {
      obj.GetType().GetField(field).SetValue(obj, value);
    }
  }

  [AttributeUsage(AttributeTargets.Field)]
  public class InjectAttribute : Attribute { }
}