namespace WCore.Interface {
  using System;
  public interface IEventService {
    void Listen<Event>(Action<Event> callback);
    void Ignore<Event>(Action<Event> callback = null);
    void Notify<Event>(Action<Event> setter);
    void Notify<Event>(Event evt);
  }
}