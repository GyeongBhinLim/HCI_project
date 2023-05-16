using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap.Interaction;
using Leap.Unity.Interaction;
using UnityEngine.SceneManagement;
using TMPro;

public class studyorquiz : MonoBehaviour
{
    float time = 0.0f;


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void convert_study()
    {
        SceneManager.LoadScene("scene_selectalphabet");
    }
        public void convert_quiz()
    {
        SceneManager.LoadScene("scene_studyalphabet");
    }


}
