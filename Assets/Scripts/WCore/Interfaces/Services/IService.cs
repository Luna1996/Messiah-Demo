namespace WCore {
  public interface IService {
    Core Core { get; set; }
    void OnAttach<I>() where I : IService;
    void OnDetach<I>() where I : IService;
  }
}