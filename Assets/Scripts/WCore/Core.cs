namespace WCore {
  using System;
  using System.Collections.Generic;

  public sealed partial class Core {
    #region 服务相关
    readonly Dictionary<Type, ServiceList> binds = new Dictionary<Type, ServiceList>();

    public void Bind<I, S>()
    where I : IService
    where S : I {
      if (binds.ContainsKey(typeof(I)))
        Clear<I>();
      var serviceList = new ServiceList(typeof(S));
      binds[typeof(I)] = serviceList;
    }

    public void Clear<I>()
    where I : IService {
      if (binds.ContainsKey(typeof(I)))
        foreach (var service in binds[typeof(I)].serviceList)
          service.OnAttach();
      binds.Remove(typeof(I));
    }

    public void Clear<I>(I i)
    where I : IService {
      if (binds.ContainsKey(typeof(I))) {
        binds[typeof(I)].serviceList.Remove(i);
        i.OnAttach();
      }
    }

    public void Clear() {
      foreach (var pair in binds)
        foreach (var service in pair.Value.serviceList)
          service.OnAttach();
      binds.Clear();
    }

    public I Get<I>()
    where I : class, IService {
      ServiceList serviceList;
      // 判断是否注册，未注册直接返回null
      if (TryGetServiceList<I>(out serviceList)) {
        var i_type = typeof(I);
        if (serviceList.isCreating) {
          // 检测到依赖循环
          serviceList.isCreating = false;
          throw new LoopRelyException(i_type.ToString());
        } else if (Utility.HasAttribute<SingleAttribute>(i_type)
        && serviceList.serviceList.Count == 1) {
          // 单例且已有实例，直接返回
          return (I)serviceList.serviceList[0];
        } else {
          // 创建实例
          serviceList.isCreating = true;
          var service = (I)Activator.CreateInstance(i_type);
          foreach (var property in i_type.GetProperties()) {
            if (Utility.HasAttribute<InjectAttribute>(property.PropertyType)) {
              try {
                var inject = this.GetType().GetMethod("Get").MakeGenericMethod(property.PropertyType).Invoke(this, null);
                if (inject == null)
                  throw new UnresolvedRelyException(property.PropertyType.ToString());
                property.SetValue(service, inject);
              } catch (CoreExeption) {
                serviceList.isCreating = false;
                throw;
              }
            }
          }
          typeof(I).GetProperty("Core").SetValue(service, this);
          serviceList.isCreating = false;
          serviceList.serviceList.Add(service);
          service.OnAttach();
          return service;
        }
      } else {
        return null;
      }
    }

    public bool Has<I>()
    where I : IService {
      if (binds.ContainsKey(typeof(I)))
        return true;
      foreach (var core in plugins)
        if (core.Has<I>())
          return true;
      return false;
    }
    #endregion
    #region 插件相关
    readonly List<Core> plugins = new List<Core>();

    public void Plug(Core core) {
      if (!plugins.Contains(core))
        plugins.Add(core);
    }

    public void Pull(Core core) {
      plugins.Remove(core);
    }

    public void Pull() {
      plugins.Clear();
    }

    public void Release() {

    }
    #endregion
    #region 辅助部件
    class ServiceList {
      public bool isCreating;
      public readonly Type serviceType;
      public readonly List<IService> serviceList;
      public ServiceList(Type t) {
        isCreating = false;
        serviceType = t;
        serviceList = new List<IService>();
      }
    }

    bool TryGetServiceList<I>(out ServiceList serviceList)
    where I : IService {
      if (binds.TryGetValue(typeof(I), out serviceList))
        return true;
      foreach (var core in plugins)
        if (core.TryGetServiceList<I>(out serviceList))
          return true;
      return false;
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