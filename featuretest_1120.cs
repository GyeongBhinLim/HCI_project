using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using System.Reflection.Emit;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Leap;
using Leap.Unity;
using TMPro;
using Unity.VisualScripting;


public class featuretest_1120
{

    public LeapServiceProvider LeapServiceProvider;
    public TextMeshProUGUI TextTMP;

    // Variables for Feature Comparison
    //float PalmNormThreshold = 0.7f;
    //int ExtensionThreshold = 3;
    //float HandThreshold = 0.7f;

    //Variables for reading a file 
    //#################### please change the path ########################################### 1
    //static string label = "r";
    //string m_Path = @"C:\Users\kbini\Desktop\HCI_Project\featureSave";
    //string m_FilePath = @"\Alphabet_" + label + ".csv";

    //float[] _reference_features;
    //float[] _test_features;

    //##################################### Methods ########################################### 3

    public float[] GetReferenceFeature(string filePath)
    {
        //Debug.Log("GetReferenceFeature");
        //Debug.Log(filePath);
        float[] sum = new float[71];
        
        string[] lines = System.IO.File.ReadAllLines(filePath);
        //Debug.Log("lines length : " + lines.Length);
        
        for(int n=0;n<sum.Length;n++)
            sum[n] = 0;
        
        for (int i = 0; i < lines.Length -1; i++)
        {
            //Debug.Log("i : "+i);
            string[] temp = lines[i].Split(',');
            
            string[] csv_input = temp.Take(temp.Length - 1).ToArray();
            if(csv_input.Length!=sum.Length)
            {
                Debug.Log("[ReadCsv] csv input doesn't match");
            }
            for(int n=0;n<sum.Length;n++)
            {
                float temp2 = float.Parse(temp[n]);
                sum[n] += temp2;
            }
        }   
        
        for (int i = 0; i < sum.Length-1; i++)
            sum[i] = sum[i]/(lines.Length -1);
            
        for (int i = 6; i < 11; i++)
        {
            if(sum[i]>0.5) sum[i] = 1;
            else sum[i] = 0;
        }

        //Debug.Log("feature Length:"+sum.Length);

        //foreach (var item in sum) {
        //    Debug.Log(item);
        //}
        return sum;
    }

    public float[] FeatureExtraction(LeapServiceProvider leapmotion)
    {
        float[] features = new float[142];
        LeapServiceProvider = leapmotion;

        if (LeapServiceProvider.CurrentFrame.Hands.Count>2)
        {
            Debug.Log("[FeatureExtraction] Use at most two hands! The result may not be correct.");
        }

        for (int i = 0; i < LeapServiceProvider.CurrentFrame.Hands.Count; i++)
        {
            Hand _hand = LeapServiceProvider.CurrentFrame.Hands[i];
            int p = 0;

            //##################################################################################### 4
            /*
            if(!_hand.IsLeft) //if current hand is right hand
                p = 71;
            */

            //palm information
            features[p+0] = _hand.PalmPosition.x;
            features[p+1] = _hand.PalmPosition.y;
            features[p+2] = _hand.PalmPosition.z;
            features[p+3] = _hand.PalmNormal.x;
            features[p+4] = _hand.PalmNormal.y;
            features[p+5] = _hand.PalmNormal.z;

            //finger Extended
            for(int f = 0; f < _hand.Fingers.Count; f++)
            {
                Finger finger_ = _hand.Fingers[f];
                Bone[] bones_ = finger_.bones;
                if (finger_.IsExtended)
                    features[p+f+6] = 1;
                else
                    features[p+f+6] = 0;
                for (int k = 0; k < bones_.Length; k++)
                {
                    Vector3 bone = bones_[k].Center;
                    features[p+12*f+3*k+11] = bone.x;
                    features[p+12*f+3*k+12] = bone.y;
                    features[p+12*f+3*k+13] = bone.z;
                }
            }
        }

        return features;
    }

    public FeatureStorage FeatureComparison(float[] reference_features, float[] test_features)
    {
        // You can add a condition that If the test_features has left hand data, return false 

        //bool PalmNormVerdict = true;
        //bool ExtensionVerdict = true;
        //bool HandVerdict = true;
        bool[] FingerVerdict = {true,true,true,true,true};
        //bool TotalVerdict = true;
        float rx,ry,rz,tx,ty,tz = 0f;
        int p=0;
        
        //################################################################################################## 5
        //int p = 71;

        //1. Calculating Palm normal vector

        float[] reference_palm_normal = {reference_features[p+3],reference_features[p+4],reference_features[p+5]};
        float[] test_palm_normal = {test_features[p+3],test_features[p+4],test_features[p+5]};
        float PalmNormSimilarity = CalCosSimilarity(reference_palm_normal,test_palm_normal);

        //2. Comparing finger extension

        int ExtensionMatch = 0;
        for(int i=0;i<5;i++)
        {
            if(reference_features[p+i+6]==test_features[p+i+6])
                ExtensionMatch++;
        }

        //3. Comparing hand gesture
        float HandSimilarity = 0f;

        Vector3 referenceNormalVector = new Vector3(reference_features[p+3],reference_features[p+4],reference_features[p+5]);
        Vector3 testNormalVector = new Vector3(test_features[p+3],test_features[p+4],test_features[p+5]);
        Vector3 referenceHandDirection = new Vector3(reference_features[p+35]-reference_features[p+0],
                                                        reference_features[p+36]-reference_features[p+1],
                                                        reference_features[p+37]-reference_features[p+2]);
                                                       
        Vector3 testHandDirection = new Vector3(test_features[p+35]-test_features[p+0],
                                                        test_features[p+36]-test_features[p+1],
                                                        test_features[p+37]-test_features[p+2]);
                                                                                                 

        Quaternion normal_rotation = Quaternion.FromToRotation(testNormalVector,referenceNormalVector);
        Vector3 rotatedNormalVector = normal_rotation * testNormalVector;
        Vector3 rotatedHandDirection = normal_rotation * testHandDirection;
        Quaternion direction_rotation = Quaternion.FromToRotation(rotatedHandDirection,referenceHandDirection);
        rotatedHandDirection = direction_rotation * rotatedHandDirection;
        rotatedNormalVector = direction_rotation * rotatedNormalVector;
        Quaternion normal_rotation2 = Quaternion.FromToRotation(rotatedNormalVector,referenceNormalVector);
        rotatedNormalVector = normal_rotation2 * rotatedNormalVector;
        rotatedHandDirection = normal_rotation2 * rotatedHandDirection;

        for(int i=0;i<20;i++)
        {
            rx = reference_features[p+i*3+11]-reference_features[p+0];
            ry = reference_features[p+i*3+12]-reference_features[p+1];
            rz = reference_features[p+i*3+13]-reference_features[p+2];
            tx = test_features[p+i*3+11]-test_features[p+0];
            ty = test_features[p+i*3+12]-test_features[p+1];
            tz = test_features[p+i*3+13]-test_features[p+2];

            Vector3 test_bone = new Vector3(tx,ty,tz);
            test_bone = normal_rotation2 * (direction_rotation * (normal_rotation * test_bone));

            //HandSimilarity += CalCosSimilarity(new float[]{rx,ry,rz},new float[]{test_bone.x,test_bone.y,test_bone.z});
            HandSimilarity += CalCosSimilarity(new float[]{rx,ry,rz},new float[]{tx,ty,tz});
        }
        HandSimilarity = HandSimilarity/20;

        //HandSimilarity = 0.8f;

        //4. Calculating Similarities of each finger
        float[] reference_fingerExtended = {0,0,0,0,0};
        float[] test_fingerExtended = {0,0,0,0,0};
        //float[] zeroCrossing = { 0, 0, 0, 0, 0 };
        //bool zeroCrossed;
        float[] howMuchExtend = { 0, 0, 0, 0, 0 };

        for (int i=0;i<5;i++)
        {
            reference_fingerExtended[i] = HowMuchExtended(reference_features[(12*i+11)..(12*i+23)]);
            test_fingerExtended[i] = HowMuchExtended(test_features[(12*i+11)..(12*i+23)]);
            howMuchExtend[i] = reference_fingerExtended[i] * test_fingerExtended[i];
        }

        FeatureStorage features = new FeatureStorage();
        features.Initialization(ExtensionMatch, HandSimilarity, PalmNormSimilarity, howMuchExtend);
        return features;

        ///////////////////////////////////////////////how to add finger Extension?

        //verdict
        //if(PalmNormSimilarity<PalmNormThreshold)
        //    PalmNormVerdict = false;
        //if(ExtensionMatch<ExtensionThreshold)
        //    ExtensionVerdict = false;
        //if(HandSimilarity<HandThreshold)
        //    HandVerdict = false;
        //TotalVerdict = PalmNormVerdict && ExtensionVerdict && HandVerdict;

        //################################################################################################### 6
        //PrintStatus(PalmNormSimilarity, ExtensionMatch, HandSimilarity, PalmNormVerdict, ExtensionVerdict, HandVerdict);

        /*
        for(int i=0;i<5;i++)
        {
            Result_TextTMP[2*i].text = reference_fingerExtended[i].ToString();
            Result_TextTMP[2*i+1].text = test_fingerExtended[i].ToString();
        }
        */

        //return TotalVerdict;
    }

    public float HowMuchExtended(float[] finger)
    {
        float ax,ay,az,bx,by,bz = 0f;

        if(finger.Length!=12)
        {
            Debug.Log("[HowMuchExtended] : finger data format dosen't fit");
            return -1;
        }
            
        ax = finger[0]-finger[3];
        ay = finger[1]-finger[4];
        az = finger[2]-finger[5];
        bx = finger[6]-finger[9];
        by = finger[7]-finger[10];
        bz = finger[8]-finger[11];

        return CalCosSimilarity(new float[]{ax,ay,az},new float[]{bx,by,bz});
    }

    //public void PrintStatus(float PalmNormSimilarity, int ExtensionMatch, float HandSimilarity, bool PalmNormVerdict, bool ExtensionVerdict, bool HandVerdict)
    //{
    //    //output result
    //    Result_TextTMP[0].text = "Palm Normal Sim : " + PalmNormSimilarity.ToString();
    //    Result_TextTMP[1].text = "Finger Extension Match : " + ExtensionMatch.ToString();
    //    Result_TextTMP[2].text = "Hand Gesture Sim :" + HandSimilarity.ToString();
        
    //    if(PalmNormVerdict)
    //        Result_TextTMP[3].text = "Right Palm Direction";
    //    else
    //        Result_TextTMP[3].text = "Wrong Palm Direction";

    //    if(ExtensionVerdict)
    //        Result_TextTMP[4].text = "Right Finger Extension";
    //    else
    //        Result_TextTMP[4].text = "Wrong Finger Extension";

    //    if(HandVerdict)
    //        Result_TextTMP[5].text = "Right Hand Gesture";
    //    else
    //        Result_TextTMP[5].text = "Wrong Hand Gesture";

    //    if(PalmNormVerdict && ExtensionVerdict && HandVerdict)
    //        Result_TextTMP[6].text = "Right";
    //    else
    //        Result_TextTMP[6].text = "Wrong";
        
    //    return;
    //}

    public float CalCosSimilarity(float[] vec1, float[] vec2)
    {   
        if(vec1.Length != vec2.Length)
        {
            Debug.Log("[CalCosSimilarity] vector sizes don't match");
            return -1;
        }
            
        float magnitude1 = 0;
        float magnitude2 = 0;
        float multiple = 0;

        for(int i=0;i<vec1.Length;i++)
        {
            magnitude1 += vec1[i]*vec1[i];
            magnitude2 += vec2[i]*vec2[i];
            multiple += vec1[i]*vec2[i];
        }
        magnitude1 = (float)Math.Sqrt(magnitude1);
        magnitude2 = (float)Math.Sqrt(magnitude2);

        return multiple/(magnitude1*magnitude2);
    }
}

public class FeatureStorage
{
    public int extensionMatch;
    public float handSimilarity;
    public float palmNormSimilarity;
    public float[] howMuchExtend;

    public void Initialization(int em, float hs, float pns, float[] hme)
    {
        extensionMatch = em;
        handSimilarity = hs;
        palmNormSimilarity = pns;
        howMuchExtend = hme;
    }
}