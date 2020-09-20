namespace WCore.Provider {
  using System;
  using System.Collections.Generic;

  using WCore.Interface;

  public class StateMachineProvider : IStateMachineService {
    Queue<IState> history = new Queue<IState>();

    public IState CurrentState {
      get {
        if (history.Count > 0)
          return history.Peek();
        return null;
      }
    }

    public void Goto(IState state, bool retain) {
      var oldState = CurrentState;
      if (oldState == state) return;
      if (oldState != null) {
        if (retain) {
          oldState.Pause?.Invoke(state);
        } else {
          history.Dequeue();
          oldState.Exit?.Invoke(state);
        }
      }
      history.Enqueue(state);
      state.Enter?.Invoke(oldState);
    }

    public void Back(IState state) {
      if (state != null && !history.Contains(state))
        return;
      IState orignalState = CurrentState;
      IState currentState = orignalState;
      while (currentState != state) {
        history.Dequeue().Exit?.Invoke(state);
        currentState = CurrentState;
      }
      state?.Resume?.Invoke(orignalState);
    }
  }
}