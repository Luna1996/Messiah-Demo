namespace WCore {
  using System;
  public class Service : IService {
    public Type Type { set; get; }
    public Core Core { set; get; }
    public Phase Creation { set; get; }
    public Service(Type type, Core core) {
      Type = type;
      Core = Core;
    }
    public void OnAttach() { }
    public void OnDetach() { }
  }
}