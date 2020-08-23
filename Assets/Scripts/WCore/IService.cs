namespace WCore {
  using System;
  public interface IService {
    void Release();
  }

  [AttributeUsage(AttributeTargets.Interface)]
  public class SingletonAttribute : Attribute { }

  [AttributeUsage(AttributeTargets.Property)]
  public class InjectAttribute : Attribute { }
}