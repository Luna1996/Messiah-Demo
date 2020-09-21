namespace Messiah {
  using UnityEngine;
  using DG.Tweening;
  using UnityEngine.UI;

  public class CardUI : MonoBehaviour {
    public Card card;
    public Sequence seq;
    Image image;

    void Start() {
      image = GetComponent<Image>();
    }

    public void SetCardByID(uint id) { }
  }
}