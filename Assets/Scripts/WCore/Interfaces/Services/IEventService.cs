namespace WCore.Interface {
  using System;
  public interface IEventService {
    void Notify(Enum id);
    void Listen(Enum id, Action callback);
    void Ignore(Enum id, Action callback);

    void Notify<A>(Enum id, A a);
    void Listen<A>(Enum id, Action<A> callback);
    void Ignore<A>(Enum id, Action<A> callback);
  }
}