using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResizeCanvas : MonoBehaviour
{
    Vector2 resolution;
    float ratio;
    CanvasScaler canvasScaler;

	public float min = 0f;
	public float max = 1f;


    void Awake()
    {
        canvasScaler = GetComponent<CanvasScaler>();

        resolution = new Vector2(Screen.width, Screen.height);
        ratio = (float)Screen.width / (float)Screen.height;

        canvasScaler.matchWidthOrHeight = ratio < 1.777f ? min : max;
    }

    void FixedUpdate()
    {
        if (resolution.x != Screen.width || resolution.y != Screen.height)
        {

            resolution.x = Screen.width;
            resolution.y = Screen.height;

            ratio = (float)resolution.x / (float)resolution.y;

            canvasScaler.matchWidthOrHeight = ratio < 1.777f ? min : max;
        }

    }
}
