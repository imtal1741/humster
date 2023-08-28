using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(EventTrigger))]
[RequireComponent(typeof(CanvasGroup))]
public class FP_Lookpad : MonoBehaviour {

    public RectTransform myCanvas;
    private Vector2 touchInput, prevDelta, dragInput;
    private bool isPressed;
    private EventTrigger eventTrigger;
    private CanvasGroup canvasGroup;

    //public Text textUI;
    //public Text textUI2;
    //public Transform circle;
    //float nextShoot;
    //float maxShoot;
    Vector2 lastInput;

    void Start()
    {
        SetupListeners();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //  && dragInput.x < (myCanvas.sizeDelta.x * myCanvas.localScale.x) / 2
        //if ((savePressed && !fp_Joystick.pressed) && (Mathf.Abs((prevDelta - dragInput).x) > myCanvas.sizeDelta.x * 0.6f || Mathf.Abs((prevDelta - dragInput).y) > myCanvas.sizeDelta.y * 0.1f))
        Vector2 tempInput = (dragInput - prevDelta) / Time.deltaTime;
        if (Mathf.Abs(tempInput.magnitude - lastInput.magnitude) > 3000f)
        {
            touchInput = Vector2.zero;
            prevDelta = dragInput;
            lastInput = touchInput;
            return;
        }


        touchInput = tempInput;
        prevDelta = dragInput;

        //circle.position = dragInput;
        //if (textUI2.text != dragInput.ToString())
        //    textUI2.text = dragInput.ToString();

        //if (Time.time > nextShoot)
        //{
        //    nextShoot = Time.time + 1;

        //    maxShoot = 0;
        //    textUI.text = maxShoot.ToString();
        //}

        //if (touchInput.magnitude > maxShoot)
        //{
        //    maxShoot = touchInput.magnitude - lastInput.magnitude;
        //    textUI.text = maxShoot.ToString();
        //}

        lastInput = touchInput;
    }

    //Setup events;
    void SetupListeners()
    {
        eventTrigger = gameObject.GetComponent<EventTrigger>();

        var a = new EventTrigger.TriggerEvent();
        a.AddListener(data =>
        {
            var evData = (PointerEventData)data;
            data.Use();
            isPressed = true;
            prevDelta = dragInput = evData.position;
        });

        eventTrigger.triggers.Add(new EventTrigger.Entry { callback = a, eventID = EventTriggerType.PointerDown });


        var b = new EventTrigger.TriggerEvent();
        b.AddListener(data =>
        {
            var evData = (PointerEventData)data;
            data.Use();
            dragInput = evData.position;
        });

        eventTrigger.triggers.Add(new EventTrigger.Entry { callback = b, eventID = EventTriggerType.Drag });


        var c = new EventTrigger.TriggerEvent();
        c.AddListener(data =>
        {
            touchInput = Vector2.zero;
            isPressed = false;
        });

        eventTrigger.triggers.Add(new EventTrigger.Entry { callback = c, eventID = EventTriggerType.EndDrag });
    }

    //Returns drag vector;
    public Vector2 LookInput()
    {
        return touchInput * new Vector2(1, -1) * Time.deltaTime;
    }

    //Returns whether or not button is pressed
    public bool IsPressed()
    {
        return isPressed;
    }
}
