namespace Messiah {
  using System;
  using UnityEngine;
  using WCore.Interface;

  public class InGameState : IState {
    public Action<IState> Enter { get; set; }
    public Action<IState> Pause { get; set; }
    public Action<IState> Resume { get; set; }
    public Action<IState> Exit { get; set; }

    public InGameState() {
      Enter += OnEnter;
      Pause += OnPause;
      Resume += OnResume;
      Exit += OnExit;
    }

    void OnEnter(IState state) {
      Messiah.update += Update;
    }

    void OnPause(IState state) {
      Messiah.update -= Update;
    }

    void OnResume(IState state) {
      Messiah.update += Update;
    }

    void OnExit(IState state) {
      Messiah.update -= Update;
    }

    void Update() {
    }
  }
}