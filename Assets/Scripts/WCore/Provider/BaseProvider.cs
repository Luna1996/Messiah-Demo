namespace WCore.Provider {
  using System;
  using WCore.Interface;
  public abstract class BaseProvider {
    public Action<Core, Type> onAttach;
    public Action<Core, Type> onDetach;
    public bool IsReady() {
      var injects = Utility.GetInjects(this.GetType());
      foreach (var inject in injects)
        if (((BaseProvider)inject.GetValue(this)).IsReady())
          return false;
      return true;
    }
  }
}