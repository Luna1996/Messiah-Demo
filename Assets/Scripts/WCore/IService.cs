namespace WCore {
  using System;
  public interface IService {
    Core Core { get; }
    void OnAttach();
    void OnDetach();
  }

  [AttributeUsage(AttributeTargets.Class)]
  public class SingleAttribute : Attribute { }

  [AttributeUsage(AttributeTargets.Property)]
  public class InjectAttribute : Attribute { }
}