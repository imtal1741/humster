using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class FP_Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform _canvas;
    public RectTransform stick;                     //stick image;
    public GameObject touchZone;
    public float returnRate = 15.0F;				//default position returning speed;
	public float dragRadius = 65.0f;				//drag radius;
	public AlphaControll colorAlpha;
	
	public event Action<FP_Joystick, Vector2> OnStartJoystickMovement;
	public event Action<FP_Joystick, Vector2> OnJoystickMovement;
	public event Action<FP_Joystick> OnEndJoystickMovement;
    
    [HideInInspector] public bool _returnHandle, pressed, isEnabled = true;
    private Vector3 startPos;
    private Vector3 globalStickPos;
    private Vector2 stickOffset;
	private CanvasGroup canvasGroup;
	
	Vector2 Coordinates
	{
		get
		{
			if (stick.anchoredPosition.magnitude < dragRadius)
				return stick.anchoredPosition / dragRadius;
			return stick.anchoredPosition.normalized;
		}
	}
	
	void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
	{
		pressed = true;
		_returnHandle = false;
        touchZone.SetActive(false);
        stickOffset = GetJoystickOffset(eventData, false);
		stick.anchoredPosition = stickOffset;
        if (OnStartJoystickMovement != null)
			OnStartJoystickMovement(this, Coordinates);
	}
	
	void IDragHandler.OnDrag(PointerEventData eventData)
	{
		stickOffset = GetJoystickOffset(eventData, true);
		stick.anchoredPosition = stickOffset;
		if (OnJoystickMovement != null)
			OnJoystickMovement(this, Coordinates);
	}
	
	void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
	{
		pressed = false;
        transform.position = startPos;
        touchZone.SetActive(true);
        _returnHandle = true;
		if (OnEndJoystickMovement != null)
			OnEndJoystickMovement(this);
	}
	
	private Vector2 GetJoystickOffset(PointerEventData eventData, bool isDrag)
	{
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_canvas, eventData.position, eventData.pressEventCamera, out globalStickPos))
        {
            if (isDrag)
            {
                
            }
            else
            {
                transform.position = globalStickPos;
            }

            stick.position = globalStickPos;
        }
		var handleOffset = stick.anchoredPosition;
		if (handleOffset.magnitude > dragRadius)
		{
			handleOffset = handleOffset.normalized * dragRadius;
			stick.anchoredPosition = handleOffset;
		}
		return handleOffset;
	}
	
	private void Start()
	{
        canvasGroup = GetComponent ("CanvasGroup") as CanvasGroup;
		_returnHandle = true;
		var touchZoneJoystick = GetComponent("RectTransform") as RectTransform;
        touchZoneJoystick.pivot = Vector2.one * 0.5F;
        startPos = touchZoneJoystick.position;
        //stick.transform.SetParent(transform);
        //var curTransform = transform;
        //do
        //{
        //	if (curTransform.GetComponent<Canvas>() != null)
        //	{
        //		_canvas = curTransform.GetComponent("RectTransform") as RectTransform;
        //		break;
        //	}
        //	curTransform = transform.parent;
        //}
        //while (curTransform != null);
    }
	
	private void FixedUpdate()
	{
		if (_returnHandle)
            //if (stick.anchoredPosition.magnitude > Mathf.Epsilon)
            //	stick.anchoredPosition -= new Vector2(stick.anchoredPosition.x * returnRate, 
            //	                                      stick.anchoredPosition.y * returnRate) * Time.deltaTime;
            if (stick.anchoredPosition.magnitude > Mathf.Epsilon)
            	stick.anchoredPosition = Vector2.zero;
            else
                _returnHandle = false;

		switch(isEnabled)
		{
		case true:
			canvasGroup.alpha = pressed ? colorAlpha.pressedAlpha : colorAlpha.idleAlpha;
			canvasGroup.interactable = canvasGroup.blocksRaycasts = true;
			break;
		case false:
			canvasGroup.alpha = 0;
			canvasGroup.interactable = canvasGroup.blocksRaycasts = false;
			break;
		}
	}


	public Vector3 MoveInput()
	{
		return new Vector3 (Coordinates.x, 0, Coordinates.y);
	}

	public void Rotate(Transform transformToRotate, float speed)
	{
		if(Coordinates != Vector2.zero)
			transformToRotate.rotation = Quaternion.Slerp (transformToRotate.rotation,
			                                              Quaternion.LookRotation (new Vector3 (Coordinates.x, 0, Coordinates.y)),
			                                              speed * Time.deltaTime);
	}

	public bool IsPressed()
	{
		return pressed;
	}

	public void Enable(bool enable)
	{
		isEnabled = enable;
	}

    void OnDisable()
    {
        pressed = false;
        transform.position = startPos;
        touchZone.SetActive(true);
        _returnHandle = true;
        if (OnEndJoystickMovement != null)
            OnEndJoystickMovement(this);
    }
}


[Serializable]
public class AlphaControll
{
	public float idleAlpha = 0.5F, pressedAlpha = 1.0F;
}















