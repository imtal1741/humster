using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraInputMobile : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{
    public bool pressed = false;
    public float sensitivity;
    private int fingerId;
    private int checkFinger;

    public float Horizontal;
    public float Vertical;

    float oldDeltaPos;


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            pressed = true;
            fingerId = eventData.pointerId;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pressed = false;
        Horizontal = 0;
        Vertical = 0;
    }



    void Update()
    {
        if (pressed)
        {
            checkFinger = 0;
            foreach (Touch touch in Input.touches)
            {
                if (touch.fingerId == fingerId)
                {
                    checkFinger++;

                    oldDeltaPos = touch.deltaPosition.magnitude;

                    if (touch.phase == TouchPhase.Moved)
                    {
                        if (touch.deltaPosition.magnitude > 1.1f && touch.deltaPosition.magnitude <= 90)
                        {
                            Horizontal = sensitivity * -touch.deltaPosition.x;
                            Vertical = sensitivity * touch.deltaPosition.y;
                        }
                        else
                        {
                            Horizontal = 0;
                            Vertical = 0;
                        }
                    }
                    if (touch.phase == TouchPhase.Stationary)
                    {
                        Horizontal = 0;
                        Vertical = 0;
                    }
                }
            }

            if (checkFinger == 0)
            {
                pressed = false;
                Horizontal = 0;
                Vertical = 0;
            }
        }
    }
}
