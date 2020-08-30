namespace WCore.Provider {
  using System;
  using Interface;
  using Actions = System.Collections.Generic.Dictionary<System.Enum, System.Delegate>;

  public class EventProvider : BaseProvider, IEventService {
    private Actions actions;
    [Inject]
    private readonly IEventService eventService;

    public EventProvider() => actions = new Actions();

    #region 零
    public void Notify(Enum id) {
      if (actions.ContainsKey(id))
        ((Action)actions[id])();
    }

    public void Listen(Enum id, Action callback) {
      if (actions.ContainsKey(id)) {
        var action = (Action)actions[id];
        action += callback;
      } else
        actions.Add(id, callback);
    }

    public void Ignore(Enum id, Action callback) {
      if (actions.ContainsKey(id)) {
        var action = (Action)actions[id];
        action -= callback;
        if (action.GetInvocationList().Length == 0)
          actions.Remove(id);
      }
    }
    #endregion

    #region 一
    public void Notify<A>(Enum id, A a) {
      if (actions.ContainsKey(id))
        ((Action<A>)actions[id])(a);
    }

    public void Listen<A>(Enum id, Action<A> callback) {
      if (actions.ContainsKey(id)) {
        var action = (Action<A>)actions[id];
        action += callback;
      } else
        actions.Add(id, callback);
    }

    public void Ignore<A>(Enum id, Action<A> callback) {
      if (actions.ContainsKey(id)) {
        var action = (Action<A>)actions[id];
        action -= callback;
        if (action == null)
          actions.Remove(id);
      }
    }
    #endregion

    ~EventProvider() => actions.Clear();
  }
}