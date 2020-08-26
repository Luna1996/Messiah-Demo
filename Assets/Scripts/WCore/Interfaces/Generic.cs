namespace WCore.Generic {
  public interface IReset {
    bool NeedReset { get; set; }
    void Reset();
  }
}