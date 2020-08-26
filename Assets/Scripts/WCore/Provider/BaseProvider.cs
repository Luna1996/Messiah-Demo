namespace WCore.Provider {
  using System;
  using WCore.Interface;
  public abstract class BaseProvider {
    public Action<Core, Type> onAttach;
    public Action<Core, Type> onDetach;
  }
}