namespace Messiah {
  using System.Collections.Generic;
  using UnityEngine;
  using UnityEngine.AddressableAssets;

  public class HandsUICtrl : MonoBehaviour {
    public float curvature;

    List<Card> hands;

    public void AddCard(Card card) { }

    public void RemoveCard(Card card) { }

    #region 调试函数
    public void SetFakeHands(int num) {

    }
    #endregion

  }
}