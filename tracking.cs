using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Leap;
using Leap.Unity;

public class tracking : MonoBehaviour
{
    public LeapServiceProvider LeapServiceProvider;
    float time = 0;

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time < 1.0f)
        {
            for (int i = 0; i < LeapServiceProvider.CurrentFrame.Hands.Count; i++)
            {
                Hand _hand = LeapServiceProvider.CurrentFrame.Hands[i];

                for (i = 0; i < _hand.Fingers.Count; i++)
                {
                    Finger finger_ = _hand.Fingers[i];
                    Bone[] bones_ = finger_.bones;
                    for (int j = 0; j < bones_.Length; j++)
                    {
                        Debug.Log(time.ToString() + "Hand Finger index : " + i.ToString() + "  " + "Finger Bone index : " + j.ToString());
                        Debug.Log(bones_[j].Center.x.ToString() + " " + bones_[j].Center.y.ToString() + bones_[j].Center.z.ToString());
                    }

                }
            }
        }
    }
}
