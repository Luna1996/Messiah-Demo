using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class panel_saint_scroll : MonoBehaviour
{ 
    private bool show;
    private bool changed;
    private float w;
    private float h;
    private Vector3 trans;
    // Start is called before the first frame update
    void Start()
    {
        show = false;
        changed = false;
        w = GetComponent<RectTransform>().rect.width;
        h = GetComponent<RectTransform>().rect.height;
        //print(w);
        
        //trans = new Vector3(w,0,0);
        //print(trans);
    }

    public void change(string direction)
    {
        if (show == false){
            show = true;
        }
        else{
            show=false;
        }
        switch(direction){
            case "left":
                trans = new Vector3(w,0,0);
                break;
            case "right":
                trans = new Vector3(-w,0,0);
                break;
            case "down":
                trans = new Vector3(0,h,0);
                break;
            case "up":
                trans = new Vector3(0,-h,0);
                break;
            default:
                Debug.Log("Invalid direction for panel scroll");
                break;
        }

        changed=true;
    }
    // Update is called once per frame
    void Update()
    {
        //print(show);
        //print(changed);
        if(changed){
            if (show==true){
                transform.position = transform.position - trans;
            }
            else{
                transform.position = transform.position + trans;
            }
            changed=false;
        }
    }
}
