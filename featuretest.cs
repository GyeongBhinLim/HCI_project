/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Leap;
using Leap.Unity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.XR;
using System.IO;
using System.Text;
using UnityEngine.UI;
using System;
using Leap.Interaction;
using Leap.Unity.Interaction;
using static UnityEngine.ParticleSystem;
using UnityEngine.UIElements;
using System.Globalization;
using System.Reflection.Emit;
using System.Linq;*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System;
using Leap.Interaction;
using Leap.Unity.Interaction;
using Leap.Unity;
using Leap;
using static UnityEngine.ParticleSystem;
using UnityEngine.UIElements;
using TMPro;
using System.Globalization;
using System.Reflection.Emit;
using System.Linq;
using Unity.VisualScripting;

using UnityEngine.SceneManagement;


public class featuretest : MonoBehaviour
{
    public LeapServiceProvider LeapServiceProvider;
    public TextMeshProUGUI TextTMP;
    private LineRenderer lineRenderer;
    private LineRenderer line0;
    private LineRenderer line1;
    private LineRenderer line2;
    private LineRenderer line3;
    private LineRenderer line4;
    private TrailRenderer trail;

    Queue<Vector3> palmQueue = new Queue<Vector3>();

    private string[] m_ColumnHeadings = { "plamtranx", "plamtrany", "plamtranz", "plamnormx", "plamnormy", "plamnormx",
                                          "isextended1", "isextended2", "isextended3", "isextended4", "isextended5",
                                          "f1b1x", "f1b1y", "f1b1z", "f1b2x", "f1b2y", "f1b2z", "f1b3x", "f1b3y", "f1b3z", "f1b4x", "f1b4y", "f1b4z",
                                          "f2b1x", "f2b1y", "f2b1z", "f2b2x", "f2b2y", "f2b2z", "f2b3x", "f2b3y", "f2b3z", "f2b4x", "f2b4y", "f2b4z",
                                          "f3b1x", "f3b1y", "f3b1z", "f3b2x", "f3b2y", "f3b2z", "f3b3x", "f3b3y", "f3b3z", "f3b4x", "f3b4y", "f3b4z",
                                          "f4b1x", "f4b1y", "f4b1z", "f4b2x", "f4b2y", "f4b2z", "f4b3x", "f4b3y", "f4b3z", "f4b4x", "f4b4y", "f4b4z",
                                          "f5b1x", "f5b1y", "f5b1z", "f5b2x", "f5b2y", "f5b2z", "f5b3x", "f5b3y", "f5b3z", "f5b4x", "f5b4y", "f5b4z"};

    float time = 0;
    int trial = 1;
    static int maximum_trial = 10;
    bool is_started = false;
    bool is_stopped = false;


    private Vector3 palm;
    private Vector3 bone;

    //private string resultString;
    // https://asta8080.tistory.com/6
    StringBuilder resultString = new StringBuilder();

    // Vector3 palm;
    Vector3 relative_bone;
   
    // 10개의 trial
    // 60(20x3)개의 좌표
    Vector3[,] stored_features  = new Vector3[maximum_trial, 20];
    Vector3[]  current_features = new Vector3[20];

    
    // 결과를 출력하는 Text에 대한 오브젝트를 담고 있는 배열
    public TextMeshProUGUI[] Result_TextTMP = new TextMeshProUGUI[maximum_trial];

    public void PressButton()
    {
        is_started = true;
        TextTMP.text = "trial : " + 0.ToString() + " / " + maximum_trial.ToString() + ",           " + time.ToString("N1") + " seconds";
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetWidth(0.01f, 0.01f);

        trail = GetComponent<TrailRenderer>();
        trail.SetPosition(0, Vector3.zero);

        //SceneManager.LoadScene("alphabet");
    }

    // Update is called once per frame
    void Update()
    {
        /*
        // Update에서 while쓰면 바로 유니티 멈춤
        if(is_started == true)
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
        }*/

        //if(is_stopped == true)
        //{
            //FeatureComparison();
            FeatureToArray();
        //}
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
                    stored_features[trial-1, j * bones_.Length + k] = relative_bone;
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

    void FeatureToArray()
    {
        Vector3 palmCoord = Vector3.zero;
        Vector3 palmNorm = Vector3.zero;
        double normVal = 0;

        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
        
        for (int i = 0; i < LeapServiceProvider.CurrentFrame.Hands.Count; i++)
        {
            Hand _hand = LeapServiceProvider.CurrentFrame.Hands[i];

            palm = _hand.PalmPosition;
            resultString.Append(palm.x.ToString() + " " + palm.y.ToString() + " " + palm.z.ToString() + " ");
            Result_TextTMP[0].text = "Palm Position : " + string.Format("{0:0.0000} ", palm.x) + " " + string.Format("{0:0.0000} ", palm.y) + " " + string.Format("{0:0.0000} ", palm.z) + " ";

            palmCoord = new Vector3(palm.x, palm.y, palm.z);
            lineRenderer.SetPosition(0, palmCoord);
            /*
            line0.SetPosition(0, palmCoord);
            line1.SetPosition(0, palmCoord);
            line2.SetPosition(0, palmCoord);
            line3.SetPosition(0, palmCoord);
            line4.SetPosition(0, palmCoord);
            */
            //trail.SetPosition(1, palmCoord);
            //trail.AddPosition(palmCoord);

            palm = _hand.PalmNormal;
            resultString.Append(palm.x.ToString() + " " + palm.y.ToString() + " " + palm.z.ToString() + " ");
            Result_TextTMP[1].text = "Palm Normal : " + string.Format("{0:0.0000} ", palm.x) + " " + string.Format("{0:0.0000} ", palm.y) + " " + string.Format("{0:0.0000} ", palm.z) + " ";

            palmNorm = new Vector3(palm.x, palm.y, palm.z);
            lineRenderer.SetPosition(1, palmCoord + (palmNorm/10));
            rotateY(palmNorm, 45);


            for (int j = 0; j < _hand.Fingers.Count; j++)
            {
                Finger finger_ = _hand.Fingers[j];
                Bone[] bones_ = finger_.bones;
                for (int k = 0; k < bones_.Length; k++)
                {
                    bone = bones_[k].Center;
                    resultString.Append(bone.x.ToString() + " " + bone.x.ToString() + " " + bone.z.ToString() + " ");

                }
                
                switch(j)
                {
                    case 0:
                        //line0.SetPosition(1, new Vector3(bone.x, bone.y, bone.z));
                        Result_TextTMP[5 + j].text = "Thumbs coord : " + string.Format("{0:0.0000} ", bone.x - palmCoord.x) + ", " + string.Format("{0:0.0000} ", bone.y - palmCoord.y) + ", " + string.Format("{0:0.0000} ", bone.z - palmCoord.z);
                        break;
                    case 1:
                        trail.AddPosition(new Vector3(bone.x, bone.y, bone.z));
                        //line1.SetPosition(1, new Vector3(bone.x, bone.y, bone.z));
                        Result_TextTMP[5 + j].text = "Index coord : " + string.Format("{0:0.0000} ", bone.x - palmCoord.x) + ", " + string.Format("{0:0.0000} ", bone.y - palmCoord.y) + ", " + string.Format("{0:0.0000} ", bone.z - palmCoord.z);
                        break;
                    case 2:
                        //line2.SetPosition(1, new Vector3(bone.x, bone.y, bone.z));
                        normVal = dist(new Vector3(bone.x, bone.y, bone.z), palmCoord);
                        //Debug.Log("Finger Length : " + normVal.ToString());
                        Result_TextTMP[j].text = "Middle coord : " + string.Format("{0:0.0000} ", (bone.x - palmCoord.x)/normVal) + ", " + string.Format("{0:0.0000} ", (bone.y - palmCoord.y) / normVal) + ", " + string.Format("{0:0.0000} ", (bone.z - palmCoord.z) / normVal);

                        Result_TextTMP[5 + j].text = "Middle coord : " + string.Format("{0:0.0000} ", bone.x - palmCoord.x) + ", " + string.Format("{0:0.0000} ", bone.y - palmCoord.y) + ", " + string.Format("{0:0.0000} ", bone.z - palmCoord.z);
                        break;
                    case 3:
                        //line3.SetPosition(1, new Vector3(bone.x, bone.y, bone.z));
                        Result_TextTMP[5 + j].text = "Ring coord : " + string.Format("{0:0.0000} ", bone.x - palmCoord.x) + ", " + string.Format("{0:0.0000} ", bone.y - palmCoord.y) + ", " + string.Format("{0:0.0000} ", bone.z - palmCoord.z);
                        break;
                    case 4:
                        //line4.SetPosition(1, new Vector3(bone.x, bone.y, bone.z));
                        Result_TextTMP[j].text = "Pinky coord : " + string.Format("{0:0.0000} ", bone.x) + ", " + string.Format("{0:0.0000} ", bone.y) + ", " + string.Format("{0:0.0000} ", bone.z);

                        Result_TextTMP[5 + j].text = "Pinky coord : " + string.Format("{0:0.0000} ", bone.x - palmCoord.x) + ", " + string.Format("{0:0.0000} ", bone.y - palmCoord.y) + ", " + string.Format("{0:0.0000} ", bone.z - palmCoord.z);
                        break;
                    default:
                        break;
                }

            }
        }
    }

    double dist(Vector3 a, Vector3 b) {
        double val = 0;

        val = Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2) + Math.Pow(a.z - b.z, 2));
        //Debug.Log(val.ToString());

        return val;
    }

    Vector3 rotateY(Vector3 v, double angle)
    {
        Vector3 vec = new Vector3(v.x, v.y, v.z);

        float cos = (float) Math.Cos(angle);
        float sin = (float) Math.Sin(angle);

        vec = new Vector3(v.z * sin + v.x * cos, v.y, v.z * cos - v.x * sin);

        if (vec.x >= 0 && vec.z >= 0)
            Debug.Log("++");
        else if (vec.x >= 0 && vec.z < 0)
            Debug.Log("+-");
        else if (vec.x < 0 && vec.z < 0)
            Debug.Log("--");
        else if (vec.x < 0 && vec.z >= 0)
            Debug.Log("-+");
        else
            Debug.Log("ERROR");

        return vec;
    }

}
