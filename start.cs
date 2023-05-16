using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap.Interaction;
using Leap.Unity.Interaction;
using UnityEngine.SceneManagement;

public class start : MonoBehaviour
{
    float time = 0.0f;
    float start_time = 0.0f;
    public float duration = 0.0f;
    bool is_image1 = true;
    bool bOnce = true;

    public Image image1;
    public Image image2;

    void Start()
    {
        image1.enabled = true;
        image2.enabled = false;

        bOnce = true;
        start_time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {

        if(Time.time - start_time > 2.0)
        {
            image2.enabled = true;
            image1.enabled = false;
            start_time = Time.time;
            bOnce = true;
        }
        else if(Time.time - start_time > 1.0 && bOnce)
        {
            image1.enabled = true;
            image2.enabled = false;
            bOnce = false;
        }

    }

    public void convert()
    {
        SceneManager.LoadScene("scene_studyorquiz");
    }


}
