namespace WCore.Provider {
  using System;
  using System.Collections.Generic;
  using WCore.Interface;
  using Pools = System.Collections.Generic.Dictionary<System.Type, object>;

  public class PoolProvider : BaseProvider, IPoolService {

    private Pools pools = new Pools();

    public Item Borrow<Item>()
    where Item : class, new() {
      var type = typeof(Item);

      Pool<Item> pool;
      if (pools.ContainsKey(type))
        pool = (Pool<Item>)pools[type];
      else {
        pool = new Pool<Item>();
        pools.Add(type, pool);
      }

      Item item;
      if (pool.reserved.Count > 0)
        item = pool.reserved.Dequeue();
      else {
        item = new Item();
      }

      pool.borrowed.Add(item);
      return item;
    }

    public void Return<Item>(Item item)
    where Item : class, new() {
      var type = typeof(Item);
      if (pools.ContainsKey(type)) {
        var pool = (Pool<Item>)pools[type];
        if (item == null) {
          pools.Remove(type);
          if (pool.borrowed.Count != 0)
            throw new NeverReturnedException(
              string.Format("{0}×{1}", pool.borrowed.Count, type));
        } else if (pool.borrowed.Contains(item)) {
          pool.borrowed.Remove(item);
          pool.reserved.Enqueue(item);
        } else
          throw new ItemNotInRecordException(pool.borrowed.Count.ToString());
      } else
        throw new ItemNotInRecordException(type.ToString());
    }


    ~PoolProvider() {
      UnityEngine.Debug.Log("~PoolProvider");
      foreach (var type in pools.Keys)
        typeof(PoolProvider)
          .GetMethod(nameof(PoolProvider.Return))
          .MakeGenericMethod(type)
          .Invoke(this, null);
    }

    private class Pool<Item> {
      public Queue<Item> reserved = new Queue<Item>();
      public HashSet<Item> borrowed = new HashSet<Item>();
    }

    private class PoolException : Exception {
      public PoolException(string msg) : base(msg) { }
    }

    private class NeverReturnedException : PoolException {
      public NeverReturnedException(string msg) : base(msg) { }
    }

    private class ItemNotInRecordException : PoolException {
      public ItemNotInRecordException(string msg) : base(msg) { }
    }
  }
}