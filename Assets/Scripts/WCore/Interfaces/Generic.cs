namespace WCore {
  public interface IReset {
    bool NeedReset { get; set; }
    void Reset();
  }
}