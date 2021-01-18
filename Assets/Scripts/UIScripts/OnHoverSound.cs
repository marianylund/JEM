using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnHoverSound : MonoBehaviour, IPointerEnterHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameObject.activeInHierarchy)
        {
            //Debug.Log("OnHover: " + gameObject.name);
            MusicControls.i_.PlayOnHover();
        }
    }

    
}
