using UnityEngine;
using System.Collections;

public class AnimatedTexture : MonoBehaviour
{
    public float fps = 30.0f;
    public Texture2D[] frames;

    private int frameIndex;
    private MeshRenderer rendererMy;

    void OnEnable()
    {
        frameIndex = 0;
    }

    void Start()
    {
        rendererMy = GetComponent<MeshRenderer>();
        NextFrame();
        InvokeRepeating("NextFrame", 1 / fps, 1 / fps);
    }

    void NextFrame()
    {
        rendererMy.sharedMaterial.SetTexture("_MainTex", frames[frameIndex]);

        if (frameIndex == frames.Length - 1)
        {
            gameObject.SetActive(false);
        }
        else
        {
            frameIndex = (frameIndex + 1) % frames.Length;
        }
    }
}