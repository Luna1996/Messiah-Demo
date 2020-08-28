namespace WCore.Provider {
  using System;
  using System.Collections.Generic;
  using WCore.Interface;
  using Pools = System.Collections.Generic.List<object>;

  public class PoolProvider : BaseProvider, IPoolService {

    private Pools pools;

    public PoolProvider() {
      onAttach += (Core core, Type type) => {
        if (pools == null)
          pools = new Pools();
      };
      onDetach += (Core core, Type type) => {
        foreach (var pool in pools)
          pool.GetType().GetMethod("Clear").Invoke(pool, null);
        pools.Clear();
      };
    }

    public IPool<Item> CreatePool<Item>(int capacity)
    where Item : new() {
      var pool = new Pool<Item>(capacity);
      pools.Add(pool);
      return pool;
    }

    public void DeletePool<Item>(IPool<Item> pool, bool wait) {
      pools.Remove(pool);
      pool.Clear();
    }

  }

  public class Pool<Item> : IPool<Item>
  where Item : new() {
    private Queue<Item> pool;

    public Pool(int capacity) {
      pool = new Queue<Item>(capacity);
    }

    public Item Borrow(bool wait) {
      if (pool.Count == 0)
        return default(Item);
      return pool.Dequeue();
    }

    public void Return(Item item) {
      pool.Enqueue(item);
    }

    public void Clear(bool wait) {
      pool.Clear();
    }
  }

}