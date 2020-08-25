using NUnit.Framework;
using System;
using UnityEngine;

public class TestWCore {
  public class MyClass {
    public MyClass(bool test = true) {
      Debug.LogFormat("MyClass：默认构造函数，class={0}", this.GetType());
    }
    public MyClass(int a, int b) {
      Debug.LogFormat("MyClass带参构造:a={0}, b={1}.", a, b);
    }
  }

  public class MyClass2 : MyClass {
    public MyClass2(int a, int b) {
      Debug.LogFormat("MyClass2带参构造:a={0}, b={1}.", a, b);
    }
  }

  //C#继承构造函数实现及调用
  public class MyClass3 : MyClass2 {
    public MyClass3(int a, int b) : base(a, b) {
      Debug.LogFormat("MyClass3带参构造:a={0}, b={1}.", a, b);
    }
  }

  [Test]
  public void GeneralTest() {
    MyClass my = new MyClass3(3, 4);
    Debug.LogFormat("{0}", ((MyClass2)my).GetType() == typeof(MyClass3));
  }
}
