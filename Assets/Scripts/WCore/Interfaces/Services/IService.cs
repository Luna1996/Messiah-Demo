namespace WCore {
  using System;
  public interface IService {
    Core Core { set; get; }
    Type Type { set; get; }
    Phase Creation { set; get; }
    void OnAttach();
    void OnDetach();
  }

  [AttributeUsage(AttributeTargets.Property)]
  public class InjectAttribute : Attribute { }
}