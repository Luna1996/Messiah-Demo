using NUnit.Framework;
using System;
using System.Collections.Generic;
using WCore;

public class TestWCore {
  interface IA {
  }
  interface IB : IA {
    int a { set; get; }
  }
  interface IC : IA {
    int a { set; get; }
  }
  class D : IB, IC {
    public int a { set; get; }
  }

  [Test]
  public void GeneralTest<I, S>()
  where S : I {
    D d = new D();
  }
}
