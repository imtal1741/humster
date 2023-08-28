using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour {

    Rigidbody rigid;
    public spawner spawnerScript;

    public Joystick joystick;
    float h;
    float y;

    public bool canStart;
    public bool isMobile;

    public YandexSDK yandexSDK;

    void Start ()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = true;

        float x = Random.Range(0f, 100f);
        if (x <= 50f)
        {
            yandexSDK.ShowAdvert();
        }
    }
	
	void Update ()
    {
        if (canStart)
        {
            if (isMobile)
            {
                h = joystick.Horizontal;
                y = joystick.Vertical;
            }
            else
            {
                h = Input.GetAxisRaw("Horizontal");
                y = Input.GetAxisRaw("Vertical");
            }

            transform.rotation *= Quaternion.Euler(0, 0, 2 * Time.deltaTime);
            Time.timeScale += Time.fixedDeltaTime * 0.01f;
            rigid.velocity += transform.rotation * (Vector3.right * h * 8f * Time.deltaTime);
            rigid.velocity += transform.rotation * (Vector3.up * y * 8f * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Time.timeScale = 1f;


        if ((int)spawnerScript.Score > yandexSDK.savedRecord)
        {
            yandexSDK.Save((int)spawnerScript.Score);
        }


        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartGame()
    {
        if (isMobile)
        {
            joystick.gameObject.SetActive(true);
        }

        spawnerScript.canStart = true;
        canStart = true;
        rigid.isKinematic = false;
    }

}
