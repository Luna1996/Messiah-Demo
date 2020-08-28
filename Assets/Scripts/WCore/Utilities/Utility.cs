namespace WCore {
  using System;
  using System.Linq;
  using System.Reflection;
  using System.Collections.Generic;
  using Provider;

  public static class Utility {

    public static FieldInfo[] GetInjects<I>() {
      return (from field in typeof(I).GetFields()
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

  public enum Phase : byte {
    Before, During, After
  }
}