namespace WCore.Interface {

  public interface IPoolService {
    IPool<Item> CreatePool<Item>(int capacity = 5) where Item : IReset, new();
    void DeletePool<Item>(IPool<Item> pool, bool wait = false) where Item : IReset;
  }

  public interface IPool<Item> where Item : IReset {
    Item Borrow(bool wait = false);
    void Return(Item item);
    void Clear(bool wait = false);
  }

  public interface IReset {
    bool NeedReset { get; set; }
    void Reset();
  }
}
