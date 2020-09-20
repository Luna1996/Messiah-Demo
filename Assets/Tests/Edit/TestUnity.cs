using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TestUnity {
  [System.Serializable]
  class Tarray {
    public T[] list = null;
    public int[] t = null;
  }
  [System.Serializable]
  class T {
    public int[] t = null;
  }
  [Test]
  public void JsonUtilityTest() {
    var test = JsonUtility.FromJson<Tarray>(
      "{\"list\":[{\"t\": [1,2,3,4]}], \"t\":[5,7,5]}");
    Debug.Log(test.list);
  }

  [Test]
  public void PlayerPrefsTest() {
    // PlayerPrefs.SetString("config", "asdfefzdxscedsf\\asdf[]{}");
    var config = PlayerPrefs.GetString("config");
    Debug.Log(config);
  }

  [Test]
  public void AATest() {
    Addressables.LoadAssetAsync<TextAsset>("DefaultPlayerConfig").Completed +=
    (AsyncOperationHandle<TextAsset> text) => {
      Debug.Log(text.Result);
    };
  }
}