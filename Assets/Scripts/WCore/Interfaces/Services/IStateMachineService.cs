namespace WCore.Interface {
  using System;
  public interface IStateMachineService {
    IState CurrentState { get; }
    void Goto(IState state, bool retain = false);
    void Back(IState state = null);
  }

  public interface IState {
    Action<IState> Enter { get; set; }
    Action<IState> Pause { get; set; }
    Action<IState> Resume { get; set; }
    Action<IState> Exit { get; set; }
  }
}