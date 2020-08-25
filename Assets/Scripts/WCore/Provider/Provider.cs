namespace WCore {
  public class Provider : IService {
    public Core Core { get; set; }
    public virtual void OnAttach<I>() where I : IService { }
    public virtual void OnDetach<I>() where I : IService { }
  }
}