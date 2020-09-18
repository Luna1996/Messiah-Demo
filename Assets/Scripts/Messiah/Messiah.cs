namespace Messiah {
  using UnityEngine;
  using WCore;
  using WCore.Provider;
  using UnityEngine.AddressableAssets;
  using UnityEngine.ResourceManagement.AsyncOperations;

  public class Messiah : MonoBehaviour {
    public static readonly Core core = new Core();

    void Awake() {
      DontDestroyOnLoad(gameObject);
    }
  }
}