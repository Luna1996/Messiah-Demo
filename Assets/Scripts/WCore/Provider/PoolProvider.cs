namespace WCore.Provider {
  using System;
  using System.Collections.Generic;
  using WCore.Generic;
  using WCore.Interface;
  using Pools = System.Collections.Generic.List<WCore.Interface.IPool<WCore.Generic.IReset>>;

  public class PoolProvider : BaseProvider, IPoolService {

    private Pools pools;

    private PoolProvider() {
      onAttach += (Core core, Type type) => {
        if (pools == null)
          pools = new Pools();
      };
      onDetach += (Core core, Type type) => {
        foreach (var pool in pools)
          pool.Clear();
        pools.Clear();
      };
    }

    public IPool<Item> CreatePool<Item>()
    where Item : IReset, new() {
      var pool = new Pool<Item>();
      pools.Add((IPool<IReset>)pool);
      return pool;
    }

    public void DeletePool<Item>(IPool<Item> pool, bool wait)
    where Item : IReset {
      pools.Remove((IPool<IReset>)pool);
    }

  }

  public class Pool<Item> : IPool<Item>
  where Item : IReset, new() {
    private List<Item> pool;
    public int Capacity {
      get { return pool.Count; }
      set {
        if (value >= Capacity) {
          pool.Capacity = value;
          while (value != Capacity) {
            var item = new Item();
            item.NeedReset = false;
            item.Reset();
            pool.Add(item);
          }
        } else if (value >= Occupied) {
          pool.RemoveAll(i => !i.NeedReset && value != Occupied);
          pool.TrimExcess();
        } else {
          pool.RemoveAll(i => !i.NeedReset);
          pool.TrimExcess();
          // 报错或者等待
        }
      }
    }
    public int Occupied { get; set; }

    public Pool() {
      pool = new List<Item>();
      Occupied = 0;
    }

    public Item Borrow(bool wait) {
      if (Occupied == Capacity)
        return default(Item);
      var item = pool.Find(i => !i.NeedReset);
      if (item != null) {
        item.NeedReset = true;
        Occupied++;
      }
      return item;
    }

    public void Return(Item item) {
      item.Reset();
      item.NeedReset = false;
      Occupied--;
    }

    public void Clear(bool wait) {
      foreach (var item in pool)
        if (item.NeedReset)
          item.Reset();
      pool.Clear();
      pool.Capacity = 0;
    }
  }

}