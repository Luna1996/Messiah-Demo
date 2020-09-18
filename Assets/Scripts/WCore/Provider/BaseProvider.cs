namespace WCore.Provider {
  public abstract class BaseProvider {
    public virtual void OnAttach(Core core) { }
    public virtual void OnDetach(Core core) { }
    public virtual void OnInit(Core core) { }
    public virtual void OnTerminate(Core core) { }
  }
}