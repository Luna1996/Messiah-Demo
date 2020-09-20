using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class card_drag : MonoBehaviour,IDragHandler,IPointerDownHandler,IPointerUpHandler
{
    private Vector2 offsetPos;
    private Vector2 initPos;
    //private bool wanna_spell;
    // Start is called before the first frame update
    void Start()
    {
        initPos = (Vector2)transform.position;
        //wanna_spell = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void OnPointerDown(PointerEventData eventData)
    {
        offsetPos = eventData.position - (Vector2)transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position - offsetPos;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if ((eventData.position - offsetPos - initPos)[1] > 20)
        {
            //wanna_spell = true;
            //打出这张卡
        }

        transform.position = initPos;
    }
}
