namespace Messiah {
  using System;
  using UnityEngine;
  using WCore;
  using WCore.Interface;
  using UnityEngine.AddressableAssets;
  using UnityEngine.ResourceManagement.AsyncOperations;

  public class Messiah : MonoBehaviour {
    public static event Action update;

    void Awake() {
      DontDestroyOnLoad(gameObject);
      Core.MainCore.Get<IStateMachineService>().Goto(new InGameState());
    }

    void Update() {
      update?.Invoke();
    }
  }
}