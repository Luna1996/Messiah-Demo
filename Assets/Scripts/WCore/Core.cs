namespace WCore {
  using System;
  using Interface;
  using Provider;
  using Binds = System.Collections.Generic.Dictionary<System.Type, WCore.Provider.BaseProvider>;
  using Plugs = System.Collections.Generic.List<WCore.Core>;
  using Relys = System.Collections.Generic.Dictionary<WCore.Provider.BaseProvider, System.Reflection.FieldInfo[]>;

  public sealed partial class Core {
    #region 默认行为
    private static (Type, Type)[] defaultRules = {
      (typeof(IPoolService),typeof(PoolProvider)),
      (typeof(IEventService), typeof(EventProvider))
    };

    public Core((Type, Type)[] initRules = null) {
      binds = new Binds();
      relys = new Relys();
      plugs = new Plugs();
      if (initRules == null)
        BindList(defaultRules);
      else
        BindList(initRules);
    }
    #endregion

    #region 回调事件
    public Action<object, object> onProviderChanged;
    #endregion

    #region 服务相关
    private Binds binds;
    private Relys relys;

    public void Bind<I, P>()
    where I : class
    where P : BaseProvider, I, new() {
      I oldS; P oldP;
      I newS; P newP;
      if (IsLocal(out oldS)) {
        // 本地已绑定服务
        oldP = (P)oldS;
        if (oldP.GetType() == typeof(P))
          // 相同提供者，无动作
          return;
        else {
          // 不同提供者，预释放
          oldP.onDetach?.Invoke(this, typeof(I));
          relys.Remove(oldP);
        }
      } else
        IsExtern(out oldS);
      // 提供者已绑定到其他服务
      foreach (var pair in binds)
        if (pair.Value.GetType() == typeof(P)) {
          newP = (P)pair.Value;
          newS = newP;
          goto HandleRely;
        }
      // 提供者首次绑定
      newP = new P();
      newS = newP;
      relys[newP] = Utility.GetInjects(typeof(P));
    HandleRely:
      // 处理依赖
      binds[typeof(I)] = newP;
      // 别人 -> 自己
      foreach (var field in relys[newP])
        field.SetValue(newP,
          typeof(Core)
            .GetMethod(nameof(Core.Get))
            .MakeGenericMethod(field.FieldType)
            .Invoke(this, null));
      // 自己 -> 别人
      foreach (var pair in relys)
        if (pair.Key != newP)
          foreach (var field in pair.Value)
            if (field.FieldType == typeof(I))
              field.SetValue(pair.Key, newP);
      newP.onAttach?.Invoke(this, typeof(I));
      onProviderChanged?.Invoke(oldS, newS);
    }

    public void BindList((Type s, Type p)[] bindRules) {
      foreach (var bindRule in bindRules) {
        typeof(Core)
          .GetMethod(nameof(Core.Bind))
          .MakeGenericMethod(bindRule.s, bindRule.p)
          .Invoke(this, null);
      }
    }

    public void UnBind<I>()
    where I : class {
      if (IsLocal<I>())
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