namespace WCore {
  using System;
  public static class Utility {
    public static bool HasAttribute<A>(Type type)
    where A : Attribute {
      return type.GetCustomAttributes(typeof(A), true).Length > 0;
    }
  }
}