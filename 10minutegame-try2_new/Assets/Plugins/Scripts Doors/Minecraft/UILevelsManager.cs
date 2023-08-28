using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UILevelsManager : MonoBehaviour
{

    public List<Button> Buttons = new List<Button>();

    public Animator BlackBorders_anim;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }


    public void UpdateLevels(int record)
    {

        for (int i = 0; i < Buttons.Count; i++)
        {
            if (i + 1 <= record + 1)
            {
                // Unlock
                //Buttons[i].enabled = true;
                //Buttons[i].GetComponent<Image>().color = new Color32(255, 255, 237, 255);
            }
            else
            {
                // Close
                Buttons[i].enabled = false;
                Buttons[i].GetComponent<Image>().color = new Color32(176, 176, 160, 255);
            }
        }

    }

    public void LoadLevel_IE(int level)
    {
        StartCoroutine(LoadLevel(level));
    }

    IEnumerator LoadLevel(int level)
    {
        if (!BlackBorders_anim.GetBool("Close"))
        {
            BlackBorders_anim.SetBool("Close", true);
            yield return new WaitForSeconds(0.6f);
            SceneManager.LoadScene(level);
        }

    }


}
