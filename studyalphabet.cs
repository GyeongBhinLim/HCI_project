using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap.Interaction;
using Leap.Unity.Interaction;
using UnityEngine.SceneManagement;

public class studyalphabet : MonoBehaviour
{
    void Start()
    {
        //imageObj = GameObject.FindGameObjectWithTag("userTag1");
        //img = imageObj.GetComponent();
        /*
        GameObject.Find("Wrong").SetActive(false);
        GameObject.Find("TryAgain").SetActive(false);
        GameObject.Find("Correct").SetActive(false);
        */
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void convert_next()
    {
    }
    
    public void convert_previous()
    {
        SceneManager.LoadScene("scene_selectalphabet");
    }
    
    public void convert_score()
    {
        SceneManager.LoadScene("scene_score");
    }


}
