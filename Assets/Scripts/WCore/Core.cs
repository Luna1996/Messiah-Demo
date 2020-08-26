namespace WCore {
  using System;
  using Interface;
  using Provider;
  using Binds = System.Collections.Generic.Dictionary<System.Type, WCore.Provider.BaseProvider>;
  using Plugs = System.Collections.Generic.List<WCore.Core>;

  public sealed partial class Core {
    #region 默认行为
    private static (Type, Type)[] defaultRules = {
      (typeof(IPoolService),typeof(PoolProvider))
    };

    public Core((Type, Type)[] initRules = null) {
      binds = new Binds();
      plugs = new Plugs();
      if (initRules == null)
        Bind(defaultRules);
      else
        Bind(initRules);
    }
    #endregion

    #region 依赖相关
    public Action<Type, BaseProvider, BaseProvider> onProviderChange;

    private void OnProviderChange(Type type, BaseProvider oldP, BaseProvider newP) {
    }
    #endregion

    #region 服务相关
    private Binds binds;

    public void Bind<I, P>()
    where I : class
    where P : BaseProvider, I, new() {
      I oldS;
      I newS;
      // 已注册服务
      if (IsLocal(out oldS)) {
        // 已绑定相同提供者，无动作
        if (oldS.GetType() == typeof(P))
          return;
        // 已绑定不同提供者，预释放
        binds[typeof(I)].onDetach(this, typeof(I));
      } else
        IsExtern(out oldS);
      // 提供者已绑定到其他服务
      foreach (var pair in binds)
        if (pair.Value.GetType() == typeof(P)) {
          binds[typeof(I)] = pair.Value;
          newS = (P)pair.Value;
          goto HandleRely;
        }
      // 提供者首次绑定
      P provider = new P();
      binds[typeof(I)] = provider;
      provider.onAttach(this, typeof(I));
      newS = provider;
    HandleRely:
      // 处理依赖
      P oldP = (P)oldS;
      P newP = (P)newS;
      OnProviderChange(typeof(I), oldP, newP);
    }

    public void Bind((Type, Type)[] bindRules) {
      foreach (var bindRule in bindRules)
        typeof(Core)
          .GetMethod(nameof(Core.Bind))
          .MakeGenericMethod(bindRule.Item1, bindRule.Item2)
          .Invoke(this, null);
    }

    public void UnBind<I>()
    where I : class {
      if (binds.ContainsKey(typeof(I)))
        binds[typeof(I)].onDetach(this, typeof(I));
      binds.Remove(typeof(I));
    }

    public void UnBindAll() {
      foreach (var pair in binds)
        pair.Value.onDetach(this, pair.Key);
      binds.Clear();
    }

    public bool Has<I>()
    where I : class {
      return IsLocal<I>() || IsExtern<I>();
    }

    public bool Has<I>(out I iservice)
    where I : class {
      return IsLocal(out iservice) || IsExtern(out iservice);
    }

    public bool IsLocal<I>()
    where I : class {
      return binds.ContainsKey(typeof(I));
    }

    public bool IsLocal<I>(out I iservice)
    where I : class {
      BaseProvider service;
      var result = binds.TryGetValue(typeof(I), out service);
      iservice = (I)(object)service;
      return result;
    }

    public bool IsExtern<I>()
    where I : class {
      foreach (var core in plugs)
        if (core.Has<I>())
          return true;
      return false;
    }

    public bool IsExtern<I>(out I iservice)
    where I : class {
      foreach (var core in plugs)
        if (core.Has(out iservice))
          return true;
      iservice = default(I);
      return false;
    }

    public I Get<I>()
    where I : class {
      I iservice;
      if (Has(out iservice))
        return iservice;
      throw new UnprovidedServiceException(typeof(I).ToString());
    }
    #endregion

    #region 插件相关
    private Plugs plugs;

    public void Plug(Core core) {
      if (!plugs.Contains(core))
        plugs.Add(core);
    }

    public void UnPlug(Core core, bool shouldPurge = false) {
      plugs.Remove(core);
      if (shouldPurge)
        core.Purge();
    }

    public void UnPlugAll(bool shouldPurge = false) {
      if (shouldPurge)
        foreach (var core in plugs)
          core.Purge();
      plugs.Clear();
    }

    public void Purge() {
      UnBindAll();
      UnPlugAll(true);
    }
    #endregion

    #region 内部异常
    public class CoreExeption : Exception {
      public CoreExeption(string desc) : base(desc) { }
    }
    public class UnprovidedServiceException : CoreExeption {
      public UnprovidedServiceException(string desc) : base(desc) { }
    }
    #endregion
  }
}