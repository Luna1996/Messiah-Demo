namespace Messiah {
  using System.Collections;
  using UnityEngine;
  using UnityEngine.UI;
  public class RandomBlink : MonoBehaviour {
    public Behaviour blinkingComp;

    void Start() {
      StartCoroutine(Blink());
    }

    IEnumerator Blink() {
      while (true) {
        var long_gap = Random.Range(5f, 10f);
        yield return new WaitForSeconds(long_gap);
        var blink_time = Random.Range(2, 4) * 2;
        for (int i = 0; i < blink_time; i++) {
          blinkingComp.enabled = !blinkingComp.enabled;
          var short_gap = Random.Range(0, 0.2f);
          yield return new WaitForSeconds(short_gap);
        }
      }
    }
  }
}