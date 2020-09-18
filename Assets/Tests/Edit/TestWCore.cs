using NUnit.Framework;
using System;
using UnityEngine;
using WCore;
using WCore.Interface;
using System.Collections.Generic;
using StopWatch = System.Diagnostics.Stopwatch;

public class TestWCore {
  public class T {
    List<int> a = new List<int>();
    public T() { Debug.Log("T"); }
    ~T() { Debug.Log("~T"); }
  }

  public enum Name {
    I = 0
  }

  public enum Name2 {
    I = 0
  }

  [Test]
  public void ReflectionTest() {
    var f = typeof(T).GetMethod("FUCK");
    Debug.Log(f);
  }

  [Test]
  public void EnumTest() {
    Enum a = Name.I;
    Enum b = Name2.I;
    Debug.Log(a == b);
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
  public void CallTest() {
    List<int> a = new List<int>();
    Tuple<int, List<int>> t = Tuple.Create(1, a);
    var p = new object[] { t };
    var f1 = typeof(TestWCore).GetMethod(nameof(TestWCore.Test));
    StopWatch s = new StopWatch();


    s.Start();
    for (int i = 0; i < 100000; i++)
      f1.Invoke(this, p);
    s.Stop();
    Debug.Log($"Reflection: {s.ElapsedTicks} ticks");


    Action<Tuple<int, List<int>>> f3 = Test;
    s.Restart();
    for (int i = 0; i < 100000; i++)
      f3.Invoke(t);
    s.Stop();
    Debug.Log($"Delegate:   {s.ElapsedTicks} ticks");


    s.Restart();
    for (int i = 0; i < 100000; i++)
      Test(t);
    s.Stop();
    Debug.Log($"Nomal:      {s.ElapsedTicks} ticks");

    Action<Tuple<int, List<int>>> f4 = (Action<Tuple<int, List<int>>>)Delegate.CreateDelegate(typeof(Action<Tuple<int, List<int>>>), typeof(TestWCore), "Test");
    s.Restart();
    for (int i = 0; i < 100000; i++)
      f4(t);
    s.Stop();
    Debug.Log($"Fast Ref:   {s.ElapsedTicks} ticks");
  }

  public static void Test(Tuple<int, List<int>> t) {
  }

  [Test]
  public void EventTest() {
    Core core = new Core();
    var evt = core.Get<IEventService>();
    evt.Listen<string>(Name.I, Debug.Log);
    evt.Notify(Name.I, "Send!");
    evt.Ignore<string>(Name.I, Debug.Log);
    evt.Notify(Name.I, "Can't Sent!");
    evt.Listen<string>(Name.I, Debug.Log);
    evt.Notify(Name.I, "Send!");
    evt.Ignore<string>(Name.I, Debug.Log);
    evt.Notify(Name.I, "Can't Sent!");
  }
}
