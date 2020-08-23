using NUnit.Framework;
using System;
using System.Collections.Generic;
using WCore;
using UnityEngine;

public class TestWCore {
  struct A {
    public int a { set; get; }
  }
  [Test]
  public void GeneralTest() {
    var type = typeof(A);
    var pro = type.GetProperty("a");
    Debug.Log(pro.PropertyType);
    Assert.True(pro.PropertyType == typeof(int));
  }
}
