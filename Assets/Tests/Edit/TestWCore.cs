using NUnit.Framework;
using System;
using UnityEngine;
using System.Reflection;
using WCore;
using WCore.Interface;
using WCore.Provider;
using System.Collections.Generic;

public class TestWCore {
  public class T {
    ~T() { Debug.Log("~T"); }
  }

  [Test]
  public void PoolServiceTest() {
    Core core = new Core();
    var pool = core.Get<IPoolService>();
    Queue<T> t = new Queue<T>();
    for (int i = 0; i < 10; i++) {
      t.Enqueue(pool.Borrow<T>());
      pool.Return(t.Dequeue());
    }
    t.Clear();
    pool.Return<T>();
    var g = new T();
    g = null;
    System.GC.Collect();
    System.GC.WaitForPendingFinalizers();
  }

  [Test]
  public void ActionTest() {
    Action a = null;
    Action b = null;
    a += ActionTest;
    b += ActionTest;
    Debug.Log(a == b);
  }

  [Test]
  public void TrivalTest() {
  }
}
