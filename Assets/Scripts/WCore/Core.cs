namespace WCore {
  using System;
  using System.Collections.Generic;
  using BindList = System.Collections.Generic.Dictionary<System.Type, WCore.IService>;

  public sealed partial class Core {
    #region 服务相关
    private BindList binds { get; } = new BindList();

    public void Bind<I, S>()
    where I : IService
    where S : Service {
      if (binds.ContainsKey(typeof(I)))
        UnBind<I>();
      binds[typeof(I)] = new Service(typeof(S), this);
    }

    public void UnBind<I>()
    where I : IService {
      if (binds.ContainsKey(typeof(I)))
        binds[typeof(I)].OnDetach();
      binds.Remove(typeof(I));
    }

    public void UnBindAll() {
      foreach (var pair in binds)
        pair.Value.OnDetach();
      binds.Clear();
    }

    public bool Has<I>()
        where I : IService {
      if (binds.ContainsKey(typeof(I)))
        return true;
      foreach (var core in plugs)
        if (core.Has<I>())
          return true;
      return false;
    }

    public bool TryGet<I>(out IService service)
          where I : IService {
      if (binds.TryGetValue(typeof(I), out service))
        return true;
      foreach (var core in plugs)
        if (core.TryGet<I>(out service))
          return true;
      return false;
    }

    public I Get<I>()
    where I : class, IService {
      IService service;
      // 判断是否注册，未注册直接抛出异常
      if (TryGet<I>(out service)) {
        switch (service.Creation) {
          // 首次唤起，依赖注入
          case Phase.Before:
            service.Creation = Phase.During;
            try {
              Inject(service);
            } catch (CoreExeption) {
              service.Creation = Phase.Before;
              throw;
            }
            service.Creation = Phase.After;
            break;
          // 检测到循环依赖，抛出异常
          case Phase.During:
            service.Creation = Phase.Before;
            throw new LoopRelyException(typeof(I).ToString());
        }
        return (I)service;
      } else {
        throw new UnresolvedRelyException(typeof(I).ToString());
      }
    }

    public void Inject<I>(I service)
    where I : IService {
      foreach (var property in typeof(I).GetProperties()) {
        if (Utility.HasAttribute<InjectAttribute>(property.PropertyType)) {
          var inject = typeof(Core).GetMethod("Get").MakeGenericMethod(property.PropertyType).Invoke(this, null);
          property.SetValue(service, inject);
        }
      }
      typeof(I).GetProperty("Core").SetValue(service, this);
    }
    #endregion

    #region 插件相关
    private List<Core> plugs { get; } = new List<Core>();

    public void Plug(Core core) {
      if (!plugs.Contains(core))
        plugs.Add(core);
    }

    public void UnPlug(Core core) {
      plugs.Remove(core);
      core.Purge();
    }

    public void UnPlug() {
      foreach (var core in plugs)
        core.Purge();
      plugs.Clear();
    }

    public void Purge() {
    }
    #endregion

    #region 内部异常
    public class CoreExeption : Exception {
      public CoreExeption(string desc) : base(desc) { }
    }
    public class LoopRelyException : CoreExeption {
      public LoopRelyException(string desc) : base(desc) { }
    }
    public class UnresolvedRelyException : CoreExeption {
      public UnresolvedRelyException(string desc) : base(desc) { }
    }
    #endregion
  }
}