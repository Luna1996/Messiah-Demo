using NUnit.Framework;
using System;
using UnityEngine;
using System.Reflection;
using WCore;
using WCore.Interface;
using WCore.Provider;

public class TestWCore {
  public class T : IReset {
    public bool NeedReset { get; set; }
    public void Reset() { }
  }
  [Test]
  public void GeneralTest() {
    Core core = new Core();
    var pools = core.Get<IPoolService>();
    var pool = pools.CreatePool<T>();
    for (int i = 0; i < 10; i++)
      Debug.Log(null == pool.Borrow());
  }
}
