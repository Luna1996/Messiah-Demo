using NUnit.Framework;
using System;
using UnityEngine;
using System.Reflection;
using WCore;
using WCore.Interface;
using WCore.Provider;
using System.Collections.Generic;

public class TestWCore {
  public class T : IReset {
    public bool NeedReset { get; set; }
    public void Reset() { Debug.Log("Reset"); }
  }
  [Test]
  public void GeneralTest() {
    Core core = new Core();
    var pools = core.Get<IPoolService>();
    var pool = pools.CreatePool<T>(9);
    for (int i = 0; i < 10; i++)
      Debug.Log(pool.Borrow());
  }
}
