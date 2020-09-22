using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSpell : MonoBehaviour,IDragHandler,IPointerDownHandler,IPointerUpHandler
{

    Vector3 cardSelected = new Vector3(1.5f, 1.5f, 1);
    Vector3 cardOnDrag = new Vector3(1, 1, 1);
   //private RectTransform cardRect;
    private Vector2 offsetPos;
    private Vector2 initPos;
    private Vector3 cardRect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {        
        transform.localScale = cardSelected;
        initPos = (Vector2)transform.localPosition;
        offsetPos = eventData.position;
        //print(initPos);
        //print(offsetPos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //print(eventData.position);
        
        //transform.position = (Vector2)event.position - offsetPos + initPos;
        transform.localPosition = (Vector2)eventData.position - offsetPos + initPos;
        //print(eventData.position - offsetPos);
        //print(transform.localPosition);
        transform.localScale = cardOnDrag;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = cardOnDrag;
        if ((eventData.position - offsetPos)[1] > 100)
        {
            print("card spelt!");
        }
        transform.localPosition = initPos;
    }
}
