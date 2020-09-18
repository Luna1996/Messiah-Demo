namespace WCore.Interface {
  using System;
  public interface IFSMService {
    void Enter(Enum state);
    void Trans(Enum state);
  }
}