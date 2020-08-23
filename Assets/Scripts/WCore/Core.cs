namespace WCore {
  using System;
  using System.Collections.Generic;
  using BindList = System.Collections.Generic.Dictionary<System.Type, WCore.Core.ServiceList>;

  public sealed partial class Core {
    #region 服务相关
    private BindList binds { get; } = new BindList();

    public void Bind<I, S>()
    where I : IService
    where S : I {
      if (binds.ContainsKey(typeof(I)))
        UnBind<I>();
      var serviceList = new ServiceList(typeof(S));
      binds[typeof(I)] = serviceList;
    }

    public void UnBind<I>()
    where I : IService {
      if (binds.ContainsKey(typeof(I)))
        foreach (var service in binds[typeof(I)].serviceList)
          service.OnDetach();
      binds.Remove(typeof(I));
    }

    public void UnBind<I>(I i)
    where I : IService {
      if (binds.ContainsKey(typeof(I))) {
        binds[typeof(I)].serviceList.Remove(i);
        i.OnDetach();
      }
    }

    public void UnBindAll() {
      foreach (var pair in binds)
        foreach (var service in pair.Value.serviceList)
          service.OnDetach();
      binds.Clear();
    }

    public I Get<I>()
    where I : class, IService {
      ServiceList serviceList;
      // 判断是否注册，未注册直接返回null
      if (TryGetServiceList<I>(out serviceList)) {
        // 单例且已有实例，直接返回
        if (Utility.HasAttribute<SingleAttribute>(typeof(I))
        && serviceList.serviceList.Count == 1)
          return (I)serviceList.serviceList[0];
        return Make<I>(serviceList);
      } else {
        return null;
      }
    }

    private I Make<I>(ServiceList serviceList)
    where I : class, IService {
      switch (serviceList.preparePhase) {
        // 进入准备
        case PreparePhase.UnPrepared:
          serviceList.preparePhase = PreparePhase.Preparing;
          goto case PreparePhase.Prepared;
        // 检测到循环依赖错误
        case PreparePhase.Preparing:
          serviceList.preparePhase = PreparePhase.UnPrepared;
          throw new LoopRelyException(typeof(I).ToString());
        // 创建实例，依赖注入
        case PreparePhase.Prepared:
          var service = (I)Activator.CreateInstance(typeof(I));
          try {
            Inject(service);
          } catch (CoreExeption) {
            serviceList.preparePhase = PreparePhase.UnPrepared;
            throw;
          }
          serviceList.preparePhase = PreparePhase.Prepared;
          serviceList.serviceList.Add(service);
          service.OnAttach();
          return service;
        default:
          return null;
      }
    }

    public void Inject<I>(I service)
    where I : IService {
      foreach (var property in typeof(I).GetProperties()) {
        if (Utility.HasAttribute<InjectAttribute>(property.PropertyType)) {
          var inject = typeof(Core).GetMethod("Get").MakeGenericMethod(property.PropertyType).Invoke(this, null);
          if (inject == null)
            throw new UnresolvedRelyException(property.PropertyType.ToString());
          property.SetValue(service, inject);
        }
      }
      typeof(I).GetProperty("Core").SetValue(service, this);
    }

    private bool TryGetServiceList<I>(out ServiceList serviceList)
        where I : IService {
      if (binds.TryGetValue(typeof(I), out serviceList))
        return true;
      foreach (var core in plugs)
        if (core.TryGetServiceList<I>(out serviceList))
          return true;
      return false;
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

    #region 辅助部件
    public class ServiceList {
      public PreparePhase preparePhase;
      public readonly Type serviceType;
      public readonly List<IService> serviceList;
      public ServiceList(Type t) {
        preparePhase = PreparePhase.UnPrepared;
        serviceType = t;
        serviceList = new List<IService>();
      }
    }
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
    #endregion

  }
}