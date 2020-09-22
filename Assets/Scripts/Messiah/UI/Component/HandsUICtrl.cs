namespace Messiah {
  using System;
  using System.Collections.Generic;
  using UnityEngine;
  using UnityEngine.AddressableAssets;
  using UnityEngine.ResourceManagement.AsyncOperations;
  using DG.Tweening;

  public class HandsUICtrl : MonoBehaviour {
    [SerializeField]
    public float m_curvature = 0;

    [SerializeField]
    public float m_widthRatio = 1;

    [NonSerialized]
    GameObject cardPrefab;

    [NonSerialized]
    List<CardUI> hands;

    void Reset() {
      UpdateArcData();
      // DrivenRectTransformTracker locker = new DrivenRectTransformTracker();
      // locker.Add(this, transform as RectTransform, DrivenTransformProperties.All);

      hands = new List<CardUI>();
      Addressables.LoadAssetAsync<GameObject>("CardPrefab").Completed +=
      (AsyncOperationHandle<GameObject> opera) => {
        cardPrefab = opera.Result;
        // SetFakeHands(m_handSize);
      };
    }
    void Start() {
      Reset();
    }

    public void AddCard(Card card) {
      var dir = arcData.from;
      var rotateStep = Quaternion.AngleAxis(arcData.degree / (m_handSize + 1), -transform.forward);
      foreach (var cardui in hands) {
        dir = rotateStep * dir;
        var pos = arcData.center + dir * arcData.radius;
        var rot = Quaternion.FromToRotation(transform.up, dir);
        cardui.seq = DOTween.Sequence();
        cardui.seq.Insert(0, cardui.transform.DOMove(pos, 0.5f));
        cardui.seq.Insert(0, cardui.transform.DORotateQuaternion(rot, 0.5f));
      }
      {
        dir = rotateStep * dir;
        var pos = arcData.center + dir * arcData.radius;
        var rot = Quaternion.FromToRotation(transform.up, dir);
        var clone = Instantiate(cardPrefab, new Vector3(2, 0.5f, 0), Quaternion.identity, transform);
        clone.name = hands.Count.ToString();
        var cardui = clone.GetComponent<CardUI>();
        hands.Add(cardui);
        cardui.seq = DOTween.Sequence();
        cardui.seq.Insert(0, cardui.transform.DOMove(pos, 0.5f));
        cardui.seq.Insert(0, cardui.transform.DORotateQuaternion(rot, 0.5f));
      }
    }

    public void RemoveCard(Card card) {
      if (hands.Count == 0) return;


      var last = hands[hands.Count - 1];
      hands.RemoveAt(hands.Count - 1);
      if (last.seq != null) last.seq.Kill(true);
      Destroy(last.gameObject);
      if (hands.Count == 0) return;


      var dir = arcData.from;
      var rotateStep = Quaternion.AngleAxis(arcData.degree / (m_handSize + 1), -transform.forward);
      foreach (var cardui in hands) {
        dir = rotateStep * dir;
        var pos = arcData.center + dir * arcData.radius;
        var rot = Quaternion.FromToRotation(transform.up, dir);
        if (cardui.seq != null) cardui.seq.Kill(true);
        cardui.seq = DOTween.Sequence();
        cardui.seq.Insert(0, cardui.transform.DOMove(pos, 0.5f));
        cardui.seq.Insert(0, cardui.transform.DORotateQuaternion(rot, 0.5f));
      }

      
    }

    public void AddRandomCard()
        {
            m_handSize += 1;
            AddCard(null);
        }

    [NonSerialized]
    public (Vector3[] rect, float halfWidth) transData =
    (new Vector3[4], 0);
    [NonSerialized]
    public (Vector3 center, Vector3 from, float degree, float radius) arcData =
    (Vector3.zero, Vector3.zero, 0, 0);
    public void UpdateArcData() {
      var trans = transform as RectTransform;
      trans.GetWorldCorners(transData.rect);
      transData.halfWidth = (transData.rect[0] - transData.rect[3]).magnitude / 2 * m_widthRatio;
      trans.hasChanged = false;

      if (m_curvature == 0) return;
      var radius = 1 / m_curvature;
      var centerPos = trans.position - trans.up * radius;
      var halfArc = Mathf.Min(radius, transData.halfWidth);
      var halfRadian = Mathf.Asin(halfArc / radius);
      var startDir =
        trans.up * Mathf.Sqrt(radius * radius - halfArc * halfArc)
        - trans.right * halfArc;
      var degree = Mathf.Rad2Deg * halfRadian * 2;

      arcData.center = centerPos;
      arcData.from = startDir / startDir.magnitude;
      arcData.degree = degree;
      arcData.radius = radius;
    }

    void OnDestroy() {

    }

#if UNITY_EDITOR
    #region 调试函数
    [SerializeField]
    public int m_handSize;

    public void SetFakeHands(int num) {
      if (num < hands.Count)
        while (num != hands.Count)
          RemoveCard(null);
      else if (num > hands.Count)
        while (num != hands.Count)
          AddCard(null);
    }
    #endregion
#endif
  }
}