using NUnit.Framework;
using System;
using System.Collections.Generic;
using WCore;
using UnityEngine;

public class TestWCore {
  [Test]
  public void GeneralTest() {
    Dictionary<int, object> a = new Dictionary<int, object>();
    a[10] = 1;
    Debug.Log(a[10]);
  }
}
