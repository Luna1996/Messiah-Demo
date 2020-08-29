namespace WCore.Interface {
  public interface IPoolService {
    Item Borrow<Item>() where Item : class, new();
    void Return<Item>(Item item = null) where Item : class, new();
  }
}
