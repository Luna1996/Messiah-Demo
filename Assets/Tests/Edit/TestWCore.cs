using NUnit.Framework;
using System;
using UnityEngine;
using System.Reflection;
using WCore;

public class TestWCore {
  public Action a;
  [Test]
  public void GeneralTest() {
  }
}

public class T {
  public static void F() {
    var a = new TestWCore();
    a.a();
  }
}