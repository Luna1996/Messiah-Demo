namespace WCore.Interface {
  using System;
  
  public interface IPoolService {
    void With<Item>(Action<Item> proc) where Item : class, new();
    Item Borrow<Item>() where Item : class, new();
    void Return<Item>(Item item = null) where Item : class, new();
  }
}
