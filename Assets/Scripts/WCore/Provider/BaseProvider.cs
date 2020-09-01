namespace WCore.Provider {
  public abstract class BaseProvider {
    public void OnAttach(Core core) { }
    public void OnDetach(Core core) { }
    public void OnInit(Core core) { }
    public void OnTerminate(Core core) { }
  }
}