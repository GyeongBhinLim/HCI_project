using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap.Interaction;
using Leap.Unity.Interaction;
using UnityEngine.SceneManagement;

public class selectalphabet : MonoBehaviour
{
    float time = 0.0f;

    public float timer = 0;
    private bool starttimer = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer = 0;
    }

    public void click_r()
    {
        Debug.Log("You clicked 'r'");
        SceneManager.LoadScene("scene_studyalphabet");
    }


    public void convert_next()
    {
        Debug.Log("timer : " + timer.ToString());
        timer -= 100.0f * Time.deltaTime;
        Debug.Log("timer - : " + timer.ToString());

        if (timer <= 0.0f)
        {
            SceneManager.LoadScene("scene_studyalphabet");
        }
        Debug.Log("Scene Change");
        Debug.Log("Delta : " + Time.deltaTime.ToString());
    }
    public void convert_previous()
    {
        SceneManager.LoadScene("scene_studyorquiz");
    }

    public void convert_back()
    {
        SceneManager.LoadScene("scene_selectalphabet");
    }

    public void convert_front()
    {
        SceneManager.LoadScene("scene_selectalphabet2");
    }

}
