namespace WCore {
  using System;
  using Binds = System.Collections.Generic.Dictionary<System.Type, WCore.Provider>;
  using Plugs = System.Collections.Generic.List<WCore.Core>;
  using BindRule = System.Tuple<System.Type, System.Type>;

  public sealed partial class Core {
    #region 服务相关
    private Binds binds;

    public void Bind<I, S>()
    where I : IService
    where S : Provider, I, new() {
      // 已注册服务
      if (IsLocal<I>()) {
        // 已绑定相同提供者，无动作
        if (binds[typeof(I)].GetType() == typeof(S))
          return;
        // 已绑定不同提供者，卸载原服务实例
        UnBind<I>();
      }
      // 已绑定其他服务的提供者
      foreach (var pair in binds)
        if (pair.Value.GetType() == typeof(S)) {
          binds[typeof(I)] = pair.Value;
          return;
        }
      // 首次绑定的提供者
      var provider = new S();
      provider.Core = this;
      binds[typeof(I)] = provider;
      provider.OnAttach<I>();
    }

    public void Bind(BindRule[] bindRules) {
      foreach (var bindRule in bindRules)
        typeof(Core)
          .GetMethod(nameof(Core.Bind))
          .MakeGenericMethod(bindRule.Item1, bindRule.Item2)
          .Invoke(this, null);
    }

    public void UnBind<I>()
    where I : IService {
      if (binds.ContainsKey(typeof(I)))
        binds[typeof(I)].OnDetach<I>();
      binds.Remove(typeof(I));
    }

    public void UnBindAll() {
      foreach (var pair in binds)
        typeof(Provider)
          .GetMethod(nameof(Provider.OnDetach))
          .MakeGenericMethod(pair.Key)
          .Invoke(pair.Value, null);
      binds.Clear();
    }

    public bool Has<I>()
    where I : IService {
      return IsLocal<I>() || IsExtern<I>();
    }

    public bool Has<I>(out I iservice)
    where I : IService {
      return IsLocal(out iservice) || IsExtern(out iservice);
    }

    public bool IsLocal<I>()
    where I : IService {
      return binds.ContainsKey(typeof(I));
    }

    public bool IsLocal<I>(out I iservice)
    where I : IService {
      Provider service;
      var result = binds.TryGetValue(typeof(I), out service);
      iservice = (I)(IService)service;
      return result;
    }

    public bool IsExtern<I>()
    where I : IService {
      foreach (var core in plugs)
        if (core.Has<I>())
          return true;
      return false;
    }

    public bool IsExtern<I>(out I iservice)
    where I : IService {
      foreach (var core in plugs)
        if (core.Has(out iservice))
          return true;
      iservice = default(I);
      return false;
    }

    public I Get<I>()
    where I : IService {
      IService iservice;
      if (Has(out iservice))
        return (I)iservice;
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

    #region 默认行为
    private static BindRule[] DefaultRules { get; set; }
    public Core(BindRule[] initRules = null) {
      binds = new Binds();
      plugs = new Plugs();
      if (initRules == null)
        Bind(DefaultRules);
      else
        Bind(initRules);
    }
    #endregion

    #region 内部异常
    public class CoreExeption : Exception {
      public CoreExeption(string desc) : base(desc) { }
    }
    public class LoopRelyException : CoreExeption {
      public LoopRelyException(string desc) : base(desc) { }
    }
    public class UnprovidedServiceException : CoreExeption {
      public UnprovidedServiceException(string desc) : base(desc) { }
    }
    #endregion
  }
}