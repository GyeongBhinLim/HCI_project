/*using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class movingDetect : MonoBehaviour
{
    private string[] m_ColumnHeadings = { "plamtranx", "plamtrany", "plamtranz", "plamnormx", "plamnormy", "plamnormx",
                                          "isextended1", "isextended2", "isextended3", "isextended4", "isextended5",
                                          "f1b1x", "f1b1y", "f1b1z", "f1b2x", "f1b2y", "f1b2z", "f1b3x", "f1b3y", "f1b3z", "f1b4x", "f1b4y", "f1b4z",
                                          "f2b1x", "f2b1y", "f2b1z", "f2b2x", "f2b2y", "f2b2z", "f2b3x", "f2b3y", "f2b3z", "f2b4x", "f2b4y", "f2b4z",
                                          "f3b1x", "f3b1y", "f3b1z", "f3b2x", "f3b2y", "f3b2z", "f3b3x", "f3b3y", "f3b3z", "f3b4x", "f3b4y", "f3b4z",
                                          "f4b1x", "f4b1y", "f4b1z", "f4b2x", "f4b2y", "f4b2z", "f4b3x", "f4b3y", "f4b3z", "f4b4x", "f4b4y", "f4b4z",
                                          "f5b1x", "f5b1y", "f5b1z", "f5b2x", "f5b2y", "f5b2z", "f5b3x", "f5b3y", "f5b3z", "f5b4x", "f5b4y", "f5b4z"};

    public LeapServiceProvider LeapServiceProvider;

    private Vector3 palm;
    private Vector3 bone;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello, World");
        SceneManager.LoadScene("movingDetect");
    }

    // Update is called once per frame
    void Update()
    {
        defaultDebug();
    }

    void defaultDebug()
    {
        Debug.Log("Debug");

        for (int i = 0; i < LeapServiceProvider.CurrentFrame.Hands.Count; i++)
        {
            Hand _hand = LeapServiceProvider.CurrentFrame.Hands[i];
            //Debug.Log("LeapServiceProvider.CurrentFrame.Hands.Count : " + LeapServiceProvider.CurrentFrame.Hands.Count.ToString());

            palm = _hand.PalmPosition;
            Debug.Log(palm.x.ToString() + " " + palm.x.ToString() + " " + palm.z.ToString() + " ");
            palm = _hand.PalmNormal;
            Debug.Log(palm.x.ToString() + " " + palm.x.ToString() + " " + palm.z.ToString() + " ");

            //m_WriteRowData.Add(palm.x.ToString());

            for (int f = 0; f < _hand.Fingers.Count; f++)
            {
                Finger finger_ = _hand.Fingers[f];
                bool extended = finger_.IsExtended;
                if (extended)
                    Debug.Log(1.ToString() + " ");
                else
                    Debug.Log(0.ToString() + " ");
            }

            for (int j = 0; j < _hand.Fingers.Count; j++)
            {
                Finger finger_ = _hand.Fingers[j];
                Bone[] bones_ = finger_.bones;
                for (int k = 0; k < bones_.Length; k++)
                {
                    //Debug.Log(time.ToString() + "Hand Finger index : " + i.ToString() + "  " + "Finger Bone index : " + j.ToString());
                    //Debug.Log(bones_[k].Center.x.ToString() + " " + bones_[k].Center.y.ToString() + bones_[k].Center.z.ToString());

                    bone = bones_[k].Center;
                    Debug.Log(bone.x.ToString() + " " + bone.x.ToString() + " " + bone.z.ToString() + " ");
                }
            }
        }
    }
}*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Leap;
using Leap.Unity;
using TMPro;
using Unity.VisualScripting;

public class movingDetect : MonoBehaviour
{
    public LeapServiceProvider LeapServiceProvider;
    public TextMeshProUGUI TextTMP;

    float time = 0;
    int trial = 1;
    static int maximum_trial = 10;
    bool is_started = false;
    bool is_stopped = false;

    Vector3 palm;
    Vector3 relative_bone;

    // 10개의 trial
    // 60(20x3)개의 좌표
    Vector3[,] stored_features = new Vector3[maximum_trial, 20];
    Vector3[] current_features = new Vector3[20];


    // 결과를 출력하는 Text에 대한 오브젝트를 담고 있는 배열
    public TextMeshProUGUI[] Result_TextTMP = new TextMeshProUGUI[maximum_trial];

    public void PressButton()
    {
        is_started = true;
        TextTMP.text = "trial : " + 0.ToString() + " / " + maximum_trial.ToString() + ",           " + time.ToString("N1") + " seconds";
    }

    // Update is called once per frame
    void Update()
    {
        // Update에서 while쓰면 바로 유니티 멈춤
        if (is_started == true)
        {
            if (trial > maximum_trial)
            {
                is_started = false;
                is_stopped = true;
            }

            time += Time.deltaTime;
            TextTMP.text = "trial : " + trial.ToString() + " / " + maximum_trial.ToString() + ",           " + time.ToString("N1") + " seconds";

            // [0, 1] 초는 기다리는 시간으로 하여 측정하지 않음.
            if (time >= 0.9f && (int)time == trial)
            {
                FeatureExtraction();
                trial += 1;
            }
        }

        if (is_stopped == true)
        {
            FeatureComparison();
        }
    }

    void FeatureExtraction()
    {

        for (int i = 0; i < LeapServiceProvider.CurrentFrame.Hands.Count; i++)
        {
            Hand _hand = LeapServiceProvider.CurrentFrame.Hands[i];
            //Debug.Log("LeapServiceProvider.CurrentFrame.Hands.Count : " + LeapServiceProvider.CurrentFrame.Hands.Count.ToString());

            palm = _hand.PalmPosition;

            for (int j = 0; j < _hand.Fingers.Count; j++)
            {
                Finger finger_ = _hand.Fingers[j];
                Bone[] bones_ = finger_.bones;
                for (int k = 0; k < bones_.Length; k++)
                {
                    //Debug.Log(time.ToString() + "Hand Finger index : " + i.ToString() + "  " + "Finger Bone index : " + j.ToString());
                    //Debug.Log(bones_[k].Center.x.ToString() + " " + bones_[k].Center.y.ToString() + bones_[k].Center.z.ToString());

                    relative_bone = bones_[k].Center - palm;
                    stored_features[trial - 1, j * bones_.Length + k] = relative_bone;
                    Debug.Log((trial - 1).ToString() + "            " + (j * bones_.Length + k).ToString());
                }
            }
        }
    }
    void FeatureComparison()
    {
        for (int i = 0; i < LeapServiceProvider.CurrentFrame.Hands.Count; i++)
        {
            Hand _hand = LeapServiceProvider.CurrentFrame.Hands[i];
            palm = _hand.PalmPosition;

            for (int j = 0; j < _hand.Fingers.Count; j++)
            {
                Finger finger_ = _hand.Fingers[j];
                Bone[] bones_ = finger_.bones;
                for (int k = 0; k < bones_.Length; k++)
                {

                    relative_bone = bones_[k].Center - palm;
                    current_features[j * bones_.Length + k] = relative_bone;
                }
            }
        }

        // 유사도 구한 방법: 20개의 bone을 각각 cosine similarity 구하고 이를 평균내었다.
        for (int j = 0; j < maximum_trial; j++)
        {
            float similarity = 0f;
            for (int k = 0; k < 20; k++)
            {
                similarity += (Vector3.Dot(stored_features[j, k], current_features[k]) / (Vector3.Magnitude(stored_features[j, k]) * Vector3.Magnitude(current_features[k])));
            }
            similarity = similarity / 20;

            Result_TextTMP[j].text = "trial " + (j + 1).ToString() + "'s Cosine Sim : " + similarity.ToString("N6");
        }
    }
}
