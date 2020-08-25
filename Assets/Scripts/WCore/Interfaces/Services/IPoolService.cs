namespace WCore {
  public interface IPoolService : IService {
    IPool<Item> CreatePool<Item>() where Item : IReset, new();
    void DeletePool<Item>(IPool<Item> pool, bool wait = false) where Item : IReset;
  }

  public interface IPool<Item> where Item : IReset {
    Item Borrow(bool wait = false);
    void Return(Item item);
    void Clear(bool wait = false);
  }
}
