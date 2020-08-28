namespace WCore.Interface {

  public interface IPoolService {
    IPool<Item> CreatePool<Item>(int capacity = 5) where Item : new();
    void DeletePool<Item>(IPool<Item> pool, bool wait = false);
  }

  public interface IPool<Item> {
    Item Borrow(bool wait = false);
    void Return(Item item);
    void Clear(bool wait = false);
  }
}
