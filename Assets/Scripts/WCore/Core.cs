namespace WCore {
  using System;
  using System.Collections.Generic;

  public sealed partial class Core {
    #region 服务相关
    Dictionary<Type, ServiceList> bindings = new Dictionary<Type, ServiceList>();

    public void Bind<I, S>()
    where I : class, IService
    where S : I {
      if (bindings.ContainsKey(typeof(I)))
        Clear<I>();
      var service_List = new ServiceList();
      service_List.creating = false;
      service_List.type = typeof(I);
      service_List.list = new List<IService>();
      bindings[typeof(I)] = service_List;
    }

    public void Clear<I>()
    where I : class, IService {
      if (bindings.ContainsKey(typeof(I)))
        foreach (var service in bindings[typeof(I)].list)
          service.Release();
    }

    public void Clear() {
      foreach (var pair in bindings)
        foreach (var service in pair.Value.list)
          service.Release();
    }

    public I Get<I>()
    where I : class, IService {
      ServiceList service_list;
      // 判断是否注册，未注册直接返回null
      if (TryGetServiceList<I>(out service_list)) {
        var i_type = typeof(I);
        if (service_list.creating) {
          // 检测到依赖循环
          service_list.creating = false;
          throw new LoopRelyException(i_type.ToString());
        } else if (Utility.HasAttribute<SingletonAttribute>(i_type)
        && service_list.list.Count == 1) {
          // 单例且已有实例，直接返回
          return (I)service_list.list[0];
        } else {
          // 创建服务实例
          service_list.creating = true;
          var service = (I)Activator.CreateInstance(i_type);
          foreach (var property in i_type.GetProperties()) {
            if (Utility.HasAttribute<InjectAttribute>(property.PropertyType)) {
              try {
                var inject = this.GetType().GetMethod("Get").MakeGenericMethod(property.PropertyType).Invoke(this, null);
                if (inject == null)
                  throw new UnresolvedRelyException(property.PropertyType.ToString());
                property.SetValue(service, inject);
              } catch (CoreExeption) {
                service_list.creating = false;
                throw;
              }
            }
          }
          service_list.creating = false;
          return service;
        }
      } else {
        return null;
      }
    }

    public bool Has<I>()
    where I : class, IService {
      if (bindings.ContainsKey(typeof(I)))
        return true;
      foreach (var core in plugins)
        if (core.Has<I>())
          return true;
      return false;
    }
    #endregion
    #region 插件相关
    List<Core> plugins = new List<Core>();

    public void Plug(Core core) {
      if (!plugins.Contains(core))
        plugins.Add(core);
    }
    #endregion
    #region 辅助类
    class ServiceList {
      public bool creating;
      public Type type;
      public List<IService> list;
    }

    bool TryGetServiceList<I>(out ServiceList service_list)
    where I : class, IService {
      if (bindings.TryGetValue(typeof(I), out service_list))
        return true;
      foreach (var core in plugins)
        if (core.TryGetServiceList<I>(out service_list))
          return true;
      return false;
    }
    #endregion
    #region 异常
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