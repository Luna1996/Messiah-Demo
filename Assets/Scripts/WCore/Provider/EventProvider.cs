namespace WCore.Provider {
  using System;
  using Interface;
  using Events = System.Collections.Generic.Dictionary<System.Type, System.Delegate>;

  public class EventProvider : IEventService {
    [Inject]
    private readonly IPoolService iPoolService;

    private Events events;

    public void Listen<Event>(Action<Event> callback) {
      var type = typeof(Event);
      Action<Event> evt;
      if (events.ContainsKey(type)) {
        evt = (Action<Event>)events[type];
        evt += callback;
      } else {
        evt = null;
        evt += callback;
        events[type] = evt;
      }
    }

    public void Ignore<Event>(Action<Event> callback = null) {
      var type = typeof(Event);
      if (events.ContainsKey(type)) {
        if (callback == null) { events.Remove(type); } else {
          var evt = (Action<Event>)events[type];
          evt -= callback;
        }
      }
    }

    public void Notify<Event>(Action<Event> setter) { 
    }

    public void Notify<Event>(Event evt) { }
    public void Purge() { }
  }
}