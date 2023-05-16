using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Leap.Interaction;
using Leap.Unity.Interaction;
using UnityEngine.SceneManagement;

using System.IO;
using System.Text;
using System.Globalization;
using System.Reflection.Emit;
using System.Linq;
using Leap.Unity;
using Leap;
using Unity.VisualScripting;


public class staticVar : MonoBehaviour
{
    public LeapServiceProvider LeapServiceProvider;
    public UnityEngine.UI.Image wrong_feedback;
    public UnityEngine.UI.Image correct_feedback;
    public UnityEngine.UI.Image tryagain_feedback;

    public UnityEngine.UI.Image KSL_r; //ㄱ
    public UnityEngine.UI.Image KSL_s; //ㄴ
    public UnityEngine.UI.Image KSL_e; //ㄷ
    public UnityEngine.UI.Image KSL_f; //ㄹ
    public UnityEngine.UI.Image KSL_a; //ㅁ
    public UnityEngine.UI.Image KSL_q; //ㅂ
    public UnityEngine.UI.Image KSL_t; //ㅅ
    public UnityEngine.UI.Image KSL_d; //ㅇ
    public UnityEngine.UI.Image KSL_w; //ㅈ
    public UnityEngine.UI.Image KSL_c; //ㅊ
    public UnityEngine.UI.Image KSL_z; //ㅋ
    public UnityEngine.UI.Image KSL_x; //ㅌ
    public UnityEngine.UI.Image KSL_v; //ㅍ
    public UnityEngine.UI.Image KSL_g; //ㅎ
    public UnityEngine.UI.Image KSL_k; //ㅏ
    public UnityEngine.UI.Image KSL_i; //ㅑ
    public UnityEngine.UI.Image KSL_j; //ㅓ
    public UnityEngine.UI.Image KSL_u; //ㅕ
    public UnityEngine.UI.Image KSL_h; //ㅗ
    public UnityEngine.UI.Image KSL_y; //ㅛ
    public UnityEngine.UI.Image KSL_n; //ㅜ
    public UnityEngine.UI.Image KSL_b; //ㅠ
    public UnityEngine.UI.Image KSL_m; //ㅡ
    public UnityEngine.UI.Image KSL_l; //ㅣ

    public int test_num = 20;
    private int[] test_sample = { 2, 4, 5, 6, 8, 13, 14, 15, 16, 19, 20, 23, 24};

// Variables for Feature Comparison
    float PalmNormThreshold = 0.7f;
    int ExtensionThreshold = 3;
    float HandThreshold = 0.7f;

    private float time_start;
    private float time_current;

    public float time_limit_tryAgain = 3f;
    public float time_limit_failure = 10f;

    private bool tryAgain = true;

    public static int clicked = 0;
    public static bool isQuiz = false;
    public TextMeshProUGUI TextTMP;
    public TextMeshProUGUI TextCnt;
    public static TextMeshProUGUI TextO;
    public static TextMeshProUGUI TextX;
    public static string alpha = " ";

    public AudioSource wrongSource;
    public AudioSource trySource;
    public AudioSource correctSource;

    private string tmp = " ";

    public featuretest_1120 FeatureMethod = new featuretest_1120();
    public studyalphabet studyalphabet = new studyalphabet();
    public FeatureStorage features = new FeatureStorage();

    float[] referenceVector;
    float[] subjectVector;

    public AlphabetReference alphabetReference;

    float[] _reference_features;
    float[] _test_features;

    static string label;
    public string m_Path = System.Environment.CurrentDirectory + @"\Assets\featureSave\";
    string m_FilePath = @"\Alphabet_.csv";

    // 결과를 출력하는 Text에 대한 오브젝트를 담고 있는 배열
    public TextMeshProUGUI[] Result_TextTMP = new TextMeshProUGUI[10];

    Queue<int> probQueue = new Queue<int>();
    Queue<string> scoreQueue = new Queue<string>();

    private float time_correct;
    private float time_wrong;
    private float time_tryagain;

    int test_count = 0;
    int correct_count = 0;
    bool bCorrect = false;
    bool bOnce_correct = true;
    bool bWrong = false;
    bool bOnce_wrong = true;
    bool bTry = false;
    bool bOnce_try = true;
    bool quizFinish = false;

    static int score = 0;
    int count_wrong = 0;
    string result_score = "";

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Init StaticVar");
        time_start = Time.time;
        wrong_feedback.enabled = false;
        correct_feedback.enabled = false;
        tryagain_feedback.enabled = false;
        probQueue.Clear();
        scoreQueue.Clear();
        TextCnt.text = "";
        if (isQuiz)
        {
            Button_quiz();
        }
    }

    // Update is called once per frame
    void Update()
    {
        TextTMP.text = alpha;

        if (isQuiz)
        {
            resetKSL();

            if (test_count == test_num)
            {
                isQuiz = false;

                Debug.Log(result_score);
                Debug.Log("Score : " + score.ToString() + " out of " + test_num.ToString() + "!");
                resetKSL();

                quizFinish = true;

                TextO.text = score.ToString();
                TextX.text = (test_num - score).ToString();
                Debug.Log(score.ToString() + " : " + (test_num - score).ToString());
                //convert_selectalphabet();
                SceneManager.LoadScene("scene_score");
            }
            else
            {
                TextCnt.text = score.ToString() + " / " + (test_count + 1).ToString();
                Debug.Log("Quiz No. " + (test_count + 1).ToString());
            }

        }

        if (!isQuiz && bCorrect && !bOnce_correct)
        {
            correct_count = 0;
            if (Time.time - time_correct > 2.0)
            {
                correct_feedback.enabled = false;
                time_correct = float.MaxValue;
                correct_count = 0;
                bCorrect = false;
            }
        }

        if (!quizFinish)
        {
            if (isQuiz && bCorrect && !bOnce_correct)
            {
                correct_count = 0;
                if (Time.time - time_correct > 2.0)
                {
                    correct_feedback.enabled = false;
                    time_correct = float.MaxValue;
                    correct_count = 0;
                    bCorrect = false;
                    bOnce_correct = true;
                    bOnce_wrong = true;
                    bOnce_try = true;
                    test_count++;
                    if (test_count < test_num)
                        Button_quiz();
                    result_score += label + ": O ";
                }
            }
            if (isQuiz && bWrong && !bOnce_wrong)
            {
                correct_count = 0;
                if (Time.time - time_wrong > 2.0)
                {
                    wrong_feedback.enabled = false;
                    time_wrong = float.MaxValue;
                    bWrong = false;
                    bOnce_correct = true;
                    bOnce_wrong = true;
                    bOnce_try = true;
                    count_wrong++;
                    test_count++;
                    if (test_count < test_num)
                        Button_quiz();
                    result_score += label + ": X ";
                }
            }
            if (isQuiz && bTry && !bOnce_try)
            {
                correct_count = 0;
                if (Time.time - time_tryagain > 1.0)
                {
                    tryagain_feedback.enabled = false;
                    time_tryagain = float.MaxValue;
                    correct_count = 0;
                    bTry = false;
                }
            }

            if (bCorrect && bOnce_correct)
            {
                correctSource.Play();
                wrong_feedback.enabled = false;
                tryagain_feedback.enabled = false;
                correct_feedback.enabled = true;
                bOnce_correct = false;
                bOnce_try = false;
                time_correct = Time.time;
                score++;
                correct_count = 0;
                scoreQueue.Enqueue("O");
            }
            else if (isQuiz && bOnce_wrong && (Time.time - time_start > time_limit_failure))
            {
                Debug.Log(time_limit_failure.ToString() + " seconds has passed. You has failed.");

                wrong_feedback.enabled = true && !quizFinish;
                if(!quizFinish) wrongSource.Play();

                correct_count = 0;
                time_wrong = Time.time;
                bWrong = true;
                bOnce_wrong = false;
                bOnce_try = false;
                bOnce_correct = false;
                scoreQueue.Enqueue("X");
            }
            else if (isQuiz && bOnce_try && (Time.time - time_start > time_limit_tryAgain))
            {
                Debug.Log(time_limit_tryAgain.ToString() + " seconds has passed.");

                tryagain_feedback.enabled = true && !quizFinish;
                if (!quizFinish) trySource.Play();

                correct_count = 0;

                time_tryagain = Time.time;
                bTry = true;
                bOnce_try = false;
            }

            m_FilePath = @"\Alphabet_" + label + ".csv";
            Debug.Log("label is : " + label);

            features = GetRefSubVector();
            alphabetReference = ReadFeatureResult(label);
            bool featureComparisionResult = alphabetReference.Comparision(features);
            Debug.Log(featureComparisionResult);
            if (featureComparisionResult)
            {
                correct_count++;
                if (correct_count == 10)
                {
                    Debug.Log("CORRECT!");
                    bCorrect = true;
                    bOnce_try = false;
                    bOnce_wrong = false;
                    correct_count = 0;
                }
            }
        }

        if (!isQuiz && test_count == 0)
        {
            resetKSL();
            switch (alpha)
            {
                case "ㄱ ":
                    KSL_r.enabled = true;
                    break;
                case "ㄴ ":
                    KSL_s.enabled = true;
                    break;
                case "ㄷ ":
                    KSL_e.enabled = true;
                    break;
                case "ㄹ ":
                    KSL_f.enabled = true;
                    break;
                case "ㅁ ":
                    KSL_a.enabled = true;
                    break;
                case "ㅂ ":
                    KSL_q.enabled = true;
                    break;
                case "ㅅ ":
                    KSL_t.enabled = true;
                    break;
                case "ㅇ ":
                    KSL_d.enabled = true;
                    break;
                case "ㅈ ":
                    KSL_w.enabled = true;
                    break;
                case "ㅊ ":
                    KSL_c.enabled = true;
                    break;
                case "ㅋ ":
                    KSL_z.enabled = true;
                    break;
                case "ㅌ ":
                    KSL_x.enabled = true;
                    break;
                case "ㅍ ":
                    KSL_v.enabled = true;
                    break;
                case "ㅎ ":
                    KSL_g.enabled = true;
                    break;
                case "ㅏ ":
                    KSL_k.enabled = true;
                    break;
                case "ㅑ ":
                    KSL_i.enabled = true;
                    break;
                case "ㅓ ":
                    KSL_j.enabled = true;
                    break;
                case "ㅕ ":
                    KSL_u.enabled = true;
                    break;
                case "ㅗ ":
                    KSL_h.enabled = true;
                    break;
                case "ㅛ ":
                    KSL_y.enabled = true;
                    break;
                case "ㅜ ":
                    KSL_n.enabled = true;
                    break;
                case "ㅠ ":
                    KSL_b.enabled = true;
                    break;
                case "ㅡ ":
                    KSL_m.enabled = true;
                    break;
                case "ㅣ ":
                    KSL_l.enabled = true;
                    break;
                default:
                    break;
            }
        }
    }

    public bool contains(int a)
    {
        for(int i = 0; i < 13; i++)
        {
            if (a == test_sample[i])
                return true;
        }

        return false;
    }

    public void resetKSL()
    {
        KSL_r.enabled = false; //ㄱ
        KSL_s.enabled = false; //ㄴ
        KSL_e.enabled = false; //ㄷ
        KSL_f.enabled = false; //ㄹ
        KSL_a.enabled = false; //ㅁ
        KSL_q.enabled = false; //ㅂ
        KSL_t.enabled = false; //ㅅ
        KSL_d.enabled = false; //ㅇ
        KSL_w.enabled = false; //ㅈ
        KSL_c.enabled = false; //ㅊ
        KSL_z.enabled = false; //ㅋ
        KSL_x.enabled = false; //ㅌ
        KSL_v.enabled = false; //ㅍ
        KSL_g.enabled = false; //ㅎ
        KSL_k.enabled = false; //ㅏ
        KSL_i.enabled = false; //ㅑ
        KSL_j.enabled = false; //ㅓ
        KSL_u.enabled = false; //ㅕ
        KSL_h.enabled = false; //ㅗ
        KSL_y.enabled = false; //ㅛ
        KSL_n.enabled = false; //ㅜ
        KSL_b.enabled = false; //ㅠ
        KSL_m.enabled = false; //ㅡ
        KSL_l.enabled = false; //ㅣ
    }

    public void resetFeedback()
    {
        wrong_feedback.enabled = false;
        correct_feedback.enabled = false;
        tryagain_feedback.enabled = false;
    }

    public void Button_quiz()
    {
        isQuiz = true;
        System.Random rand = new System.Random();
        int a = rand.Next(1, 24);

        while (probQueue.Contains(a) || !contains(a))
        {
            a = a % 24;
            a++;
            Debug.Log("New alpha : " +a.ToString());
        }

        Debug.Log("Random : " + a);
        probQueue.Enqueue(a);

        //Debug.Log(contains(a));
        // ㄴ, ㄷ, ㄹ, ㅁ, ㅂ, ㅇ, ㅈ, ㅊ, ㅋ, ㅌ, ㅍ, ㅎ, ㅏ, ㅑ, ㅗ, ㅛ, ㅡ, ㅣ

        switch(a)
        {
            case 1 :
                alpha = "ㄱ ";
                label = "r";
                break;
            case 2:
                alpha = "ㄴ ";
                label = "s";
                break;
            case 3:
                alpha = "ㄷ ";
                label = "e";
                break;
            case 4:
                alpha = "ㄹ ";
                label = "f";
                break;
            case 5:
                alpha = "ㅁ ";
                label = "a";
                break;
            case 6:
                alpha = "ㅂ ";
                label = "q";
                break;
            case 7:
                alpha = "ㅅ ";
                label = "t";
                break;
            case 8:
                alpha = "ㅇ ";
                label = "d";
                break;
            case 9:
                alpha = "ㅈ ";
                label = "w";
                break;
            case 10:
                alpha = "ㅊ ";
                label = "c";
                break;
            case 11:
                alpha = "ㅋ ";
                label = "z";
                break;
            case 12:
                alpha = "ㅌ ";
                label = "x";
                break;
            case 13:
                alpha = "ㅍ ";
                label = "v";
                break;
            case 14:
                alpha = "ㅎ ";
                label = "g";
                break;
            case 15:
                alpha = "ㅏ ";
                label = "k";
                break;
            case 16:
                alpha = "ㅑ ";
                label = "i";
                break;
            case 17:
                alpha = "ㅓ ";
                label = "j";
                break;
            case 18:
                alpha = "ㅕ ";
                label = "u";
                break;
            case 19:
                alpha = "ㅗ ";
                label = "h";
                break;
            case 20:
                alpha = "ㅛ ";
                label = "y";
                break;
            case 21:
                alpha = "ㅜ ";
                label = "n";
                break;
            case 22:
                alpha = "ㅠ ";
                label = "b";
                break;
            case 23:
                alpha = "ㅡ ";
                label = "m";
                break;
            case 24:
                alpha = "ㅣ ";
                label = "l";
                break;
            default :
                break;
        }

        time_start = Time.time;
    }

    public void disableTryAgain()
    {
        tryagain_feedback.enabled = false;

        tryAgain = false;
    }

    public void disableWrong()
    {
        wrong_feedback.enabled = false;
    }

    public void wrong()
    {
        Debug.Log("Wrong!!");

        //wrongSource.enabled = true;

        wrong_feedback.enabled = true;
    }

    // 자음
    public void Button_r() // "ㄱ"
    {
        clicked = 1;
        alpha = "ㄱ ";
        label = "r";
        isQuiz = false;
        Debug.Log("Button R : " + alpha);
    }
    public void Button_s()
    {
        alpha = "ㄴ ";
        label = "s";
        isQuiz = false;
    }
    public void Button_e()
    {
        alpha = "ㄷ ";
        label = "e";
        isQuiz = false;
    }
    public void Button_f()
    {
        alpha = "ㄹ ";
        label = "f";
        isQuiz = false;
    }
    public void Button_a()
    {
        alpha = "ㅁ ";
        label = "a";
        isQuiz = false;
    }
    public void Button_q()
    {
        alpha = "ㅂ ";
        label = "q";
        isQuiz = false;
    }
    public void Button_t()
    {
        alpha = "ㅅ ";
        label = "t";
        isQuiz = false;
    }
    public void Button_d()
    {
        alpha = "ㅇ ";
        label = "d";
        isQuiz = false;
    }
    public void Button_w()
    {
        alpha = "ㅈ ";
        label = "w";
        isQuiz = false;
    }
    public void Button_c()
    {
        alpha = "ㅊ ";
        label = "c";
        isQuiz = false;
    }
    public void Button_z()
    {
        alpha = "ㅋ ";
        label = "z";
        isQuiz = false;
    }
    public void Button_x()
    {
        alpha = "ㅌ ";
        label = "x";
        isQuiz = false;
    }
    public void Button_v()
    {
        alpha = "ㅍ ";
        label = "v";
        isQuiz = false;
    }
    public void Button_g()
    {
        alpha = "ㅎ ";
        label = "g";
        isQuiz = false;
    }

    // 모음
    public void Button_k()
    {
        alpha = "ㅏ ";
        label = "k";
        isQuiz = false;
    }
    public void Button_i()
    {
        alpha = "ㅑ ";
        label = "i";
        isQuiz = false;
    }
    public void Button_j()
    {
        alpha = "ㅓ ";
        label = "j";
        isQuiz = false;
    }
    public void Button_u()
    {
        alpha = "ㅕ ";
        label = "u";
        isQuiz = false;
    }
    public void Button_h()
    {
        alpha = "ㅗ ";
        label = "h";
        isQuiz = false;
    }
    public void Button_y()
    {
        alpha = "ㅛ ";
        label = "y";
        isQuiz = false;
    }
    public void Button_n()
    {
        alpha = "ㅜ ";
        label = "n";
        isQuiz = false;
    }
    public void Button_b()
    {
        alpha = "ㅠ ";
        label = "b";
        isQuiz = false;
    }
    public void Button_m()
    {
        alpha = "ㅡ ";
        label = "m";
        isQuiz = false;
    }
    public void Button_l()
    {
        alpha = "ㅣ ";
        label = "l";
        isQuiz = false;
    }

    public void convert_selectalphabet()
    {
        Debug.Log("COnvert");

        if(!isQuiz)
            SceneManager.LoadScene("scene_selectalphabet");
        else
        {
            isQuiz = false;
            test_count = 0;
            correct_count = 0;
            score = 0;
            SceneManager.LoadScene("scene_studyorquiz");
        }
    }
    public void convert_studyorquiz()
    {
        SceneManager.LoadScene("scene_studyorquiz");
    }



    //##################################### Methods ########################################### 3

    public float[] GetReferenceFeature(string filePath)
    {
        //Debug.Log("GetReferenceFeature");
        //Debug.Log(m_Path + filePath);
        float[] sum = new float[71];

        string[] lines = System.IO.File.ReadAllLines(m_Path + filePath);
        //Debug.Log("lines length : " + lines.Length);

        for (int n = 0; n < sum.Length; n++)
            sum[n] = 0;

        for (int i = 0; i < lines.Length - 1; i++)
        {
            //Debug.Log("i : " + i);
            string[] temp = lines[i].Split(',');

            string[] csv_input = temp.Take(temp.Length - 1).ToArray();
            if (csv_input.Length != sum.Length)
            {
                //Debug.Log("[ReadCsv] csv input doesn't match");
            }
            for (int n = 0; n < sum.Length; n++)
            {
                float temp2 = float.Parse(temp[n]);
                sum[n] += temp2;
            }
        }

        for (int i = 0; i < sum.Length - 1; i++)
            sum[i] = sum[i] / (lines.Length - 1);



        //Debug.Log("feature Length:" + sum.Length);

        foreach (var item in sum)
        {
            //Debug.Log(item);
        }
        return sum;
    }

    public float[] FeatureExtraction()
    {
        float[] features = new float[142];

        if (LeapServiceProvider.CurrentFrame.Hands.Count > 2)
        {
            //Debug.Log("[FeatureExtraction] Use at most two hands! The result may not be correct.");
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
            features[p + 0] = _hand.PalmPosition.x;
            features[p + 1] = _hand.PalmPosition.y;
            features[p + 2] = _hand.PalmPosition.z;
            features[p + 3] = _hand.PalmNormal.x;
            features[p + 4] = _hand.PalmNormal.y;
            features[p + 5] = _hand.PalmNormal.z;

            //finger Extended
            for (int f = 0; f < _hand.Fingers.Count; f++)
            {
                Finger finger_ = _hand.Fingers[f];
                Bone[] bones_ = finger_.bones;
                if (finger_.IsExtended)
                    features[p + f + 6] = 1;
                else
                    features[p + f + 6] = 0;
                for (int k = 0; k < bones_.Length; k++)
                {
                    Vector3 bone = bones_[k].Center;
                    features[p + 12 * f + 3 * k + 11] = bone.x;
                    features[p + 12 * f + 3 * k + 12] = bone.y;
                    features[p + 12 * f + 3 * k + 13] = bone.z;
                }
            }
        }

        return features;
    }

    public bool FeatureComparison(float[] reference_features, float[] test_features)
    {
        // You can add a condition that If the test_features has left hand data, return false 

        bool PalmNormVerdict = true;
        bool ExtensionVerdict = true;
        bool HandVerdict = true;
        bool[] FingerVerdict = { true, true, true, true, true };
        bool TotalVerdict = true;
        float rx, ry, rz, tx, ty, tz = 0f;
        int p = 0;

        //################################################################################################## 5
        //int p = 71;

        //1. Calculating Palm normal vector

        float[] reference_palm_normal = { reference_features[p + 3], reference_features[p + 4], reference_features[p + 5] };
        float[] test_palm_normal = { test_features[p + 3], test_features[p + 4], test_features[p + 5] };
        float PalmNormSimilarity = CalCosSimilarity(reference_palm_normal, test_palm_normal);

        //2. Comparing finger extension

        int ExtensionMatch = 0;
        for (int i = 0; i < 5; i++)
        {
            if (reference_features[p + i + 6] == test_features[p + i + 6])
                ExtensionMatch++;
        }

        //3. Comparing hand gesture
        float HandSimilarity = 0f;

        for (int i = 0; i < 20; i++)
        {
            rx = reference_features[p + i * 3 + 11] - reference_features[p + 0];
            ry = reference_features[p + i * 3 + 12] - reference_features[p + 1];
            rz = reference_features[p + i * 3 + 13] - reference_features[p + 2];
            tx = test_features[p + i * 3 + 11] - test_features[p + 0];
            ty = test_features[p + i * 3 + 12] - test_features[p + 1];
            tz = test_features[p + i * 3 + 13] - test_features[p + 2];

            HandSimilarity += CalCosSimilarity(new float[] { rx, ry, rz }, new float[] { tx, ty, tz });
        }
        HandSimilarity = HandSimilarity / 20;

        //4. Calculating Similarities of each finger
        float[] reference_fingerExtended = { 0, 0, 0, 0, 0 };
        float[] test_fingerExtended = { 0, 0, 0, 0, 0 };

        for (int i = 0; i < 5; i++)
        {
            reference_fingerExtended[i] = HowMuchExtended(reference_features[(12 * i + 11)..(12 * i + 23)]);
            test_fingerExtended[i] = HowMuchExtended(test_features[(12 * i + 11)..(12 * i + 23)]);
        }

        ///////////////////////////////////////////////how to add finger Extension?

        //verdict
        if (PalmNormSimilarity < PalmNormThreshold)
            PalmNormVerdict = false;
        if (ExtensionMatch < ExtensionThreshold)
            ExtensionVerdict = false;
        if (HandSimilarity < HandThreshold)
            HandVerdict = false;
        TotalVerdict = PalmNormVerdict && ExtensionVerdict && HandVerdict;

        //################################################################################################### 6
        PrintStatus(PalmNormSimilarity, ExtensionMatch, HandSimilarity, PalmNormVerdict, ExtensionVerdict, HandVerdict);

        /*
        for(int i=0;i<5;i++)
        {
            Result_TextTMP[2*i].text = reference_fingerExtended[i].ToString();
            Result_TextTMP[2*i+1].text = test_fingerExtended[i].ToString();
        }
        */

        return TotalVerdict;
    }

    public float HowMuchExtended(float[] finger)
    {
        float ax, ay, az, bx, by, bz = 0f;

        if (finger.Length != 12)
        {
            //
            //Debug.Log("[HowMuchExtended] : finger data format dosen't fit");
            return -1;
        }

        ax = finger[0] - finger[3];
        ay = finger[1] - finger[4];
        az = finger[2] - finger[5];
        bx = finger[6] - finger[9];
        by = finger[7] - finger[10];
        bz = finger[8] - finger[11];

        return CalCosSimilarity(new float[] { ax, ay, az }, new float[] { bx, by, bz });
    }

    public void PrintStatus(float PalmNormSimilarity, int ExtensionMatch, float HandSimilarity, bool PalmNormVerdict, bool ExtensionVerdict, bool HandVerdict)
    {
        //output result
        Result_TextTMP[0].text = "Palm Normal Sim : " + PalmNormSimilarity.ToString();
        //Debug.Log("Palm Normal Sim : " + PalmNormSimilarity.ToString());
        Result_TextTMP[1].text = "Finger Extension Match : " + ExtensionMatch.ToString();
        //Debug.Log("Finger Extension Match : " + ExtensionMatch.ToString());
        Result_TextTMP[2].text = "Hand Gesture Sim :" + HandSimilarity.ToString();
        //Debug.Log("Hand Gesture Sim :" + HandSimilarity.ToString());

        if (PalmNormVerdict)
            Result_TextTMP[3].text = "Right Palm Direction";
        else
            Result_TextTMP[3].text = "Wrong Palm Direction";

        if (ExtensionVerdict)
            Result_TextTMP[4].text = "Right Finger Extension";
        else
            Result_TextTMP[4].text = "Wrong Finger Extension";

        if (HandVerdict)
            Result_TextTMP[5].text = "Right Hand Gesture";
        else
            Result_TextTMP[5].text = "Wrong Hand Gesture";

        if (PalmNormVerdict && ExtensionVerdict && HandVerdict)
            Result_TextTMP[6].text = "Right";
        else
            Result_TextTMP[6].text = "Wrong";

        return;
    }

    public float CalCosSimilarity(float[] vec1, float[] vec2)
    {
        if (vec1.Length != vec2.Length)
        {
            //Debug.Log("[CalCosSimilarity] vector sizes don't match");
            return -1;
        }

        float magnitude1 = 0;
        float magnitude2 = 0;
        float multiple = 0;

        for (int i = 0; i < vec1.Length; i++)
        {
            magnitude1 += vec1[i] * vec1[i];
            magnitude2 += vec2[i] * vec2[i];
            multiple += vec1[i] * vec2[i];
        }
        magnitude1 = (float)Math.Sqrt(magnitude1);
        magnitude2 = (float)Math.Sqrt(magnitude2);

        return multiple / (magnitude1 * magnitude2);
    }




    // 다른 스크립트에서 사용할 경우 해당 메소드를 이용하여 한번에 사용하시면 됩니다.
    // 알파벳이 주어졌을 때, 저장된 reference vector와 제약조건을 불러옴.
    // 그리고 leapmotion provider를 통해 subject vector 값을 읽고 이를 비교하여 TF값 반환.
    public bool GetComparisionResult(LeapServiceProvider provider, string alphabet)
    {
        string refFilePath_ = @"Alphabet_" + alphabet + ".csv";

        float[] referenceVector = FeatureMethod.GetReferenceFeature(m_Path + refFilePath_);
        float[] subjectVector = FeatureMethod.FeatureExtraction(provider);
        FeatureStorage features_ = FeatureMethod.FeatureComparison(referenceVector, subjectVector);
        AlphabetReference alphabetReference = ReadFeatureResult(alphabet);
        bool featureComparisionResult = alphabetReference.Comparision(features_);

        return featureComparisionResult;
    }

    // reference vector를 불러오고, subject vector를 읽는다.
    // 두 벡터를 이용하여 매칭되는 isExtended 개수, palm normal veector의 유사도, 각 손가락별 HowmuchExtended의 곱셈, Hand Gesture similarity 값을 가지는 클래스를 반환한다.
    public FeatureStorage GetRefSubVector()
    {
        string filepath = m_Path + m_FilePath;
        //Debug.Log(filepath);
        referenceVector = FeatureMethod.GetReferenceFeature(filepath);
        subjectVector = FeatureMethod.FeatureExtraction(LeapServiceProvider);
        FeatureStorage features_ = FeatureMethod.FeatureComparison(referenceVector, subjectVector);

        return features_;

    }

    // Alphabet_result.csv 파일을 열고 레이블에 해당하는 제약조건 5개를 가져온다.
    public AlphabetReference ReadFeatureResult(string alphabet)
    {
        string m_ResultPath = @"Alphabet_result" + ".csv";
        string[] lines = System.IO.File.ReadAllLines(m_Path + m_ResultPath);
        //Debug.Log(m_Path + m_ResultPath);
        for (int i = 0; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(',');
            string[] csv_input = columns.Take(columns.Length).ToArray();

            if (alphabet.Equals(csv_input[0]))
            {
                int handExtendedNum = int.Parse(csv_input[1]);
                float palmNormalVectorMag = float.Parse(csv_input[2]);
                int zeroCrossingNum = int.Parse(csv_input[3]);
                float zeroCrossAvg = float.Parse(csv_input[4]);
                float handGesturesim = float.Parse(csv_input[5]);

                AlphabetReference reference = new AlphabetReference();
                //reference.Initialization(0,0,0,0,0);
                reference.Initialization(handExtendedNum, palmNormalVectorMag, zeroCrossingNum, zeroCrossAvg, handGesturesim);
                //Debug.Log(handExtendedNum.ToString() + ", " + palmNormalVectorMag.ToString() + ", " + zeroCrossingNum.ToString() + ", " + zeroCrossAvg.ToString() + ", " + handGesturesim.ToString());

                return reference;
            }
        }
        return null;
    }
}


// Alphabet_result.csv에서 불러온 제약조건을 가지고 있는 클래스.
// reference vector와 subject vector를 비교하여 얻은 4개의 값들을 이용하여 제약조건에 일치하는지 확인하는 메소드를 가지고 있음.
public class AlphabetReference
{
    // Leapmotion API의 isExtended가 매칭되는 개수
    public int handExtendedNum;
    // PlamNormal vector의 유사도 treshold
    public float palmNormalVectorMag;
    // reference hand와 subject vector의 코사인 유사도
    public float handGesturesim;
    // reference finger와 subject finger의 굽힘 방향이 +일 때 양수 값을 갖는데, 양수 값의 개수의 treshold
    public int zeroCrossingNum;
    // reference finger의 HowMuchExtended와 subject finger의 HowMuchExtended 갑을 곱하고, 그 곱한 값의 평균에 대한 treshold
    public float zeroCrossAvg;


    public void Initialization(int hen, float pavm, int zcn, float zca, float hgs)
    {
        handExtendedNum = hen;
        palmNormalVectorMag = pavm;
        handGesturesim = hgs;
        zeroCrossingNum = zcn;
        zeroCrossAvg = zca;
    }

    // 4개의 값을 이용하여 5개의 제약조건을 만족하는지 확인한다.
    // 이 때 5개의 제약조건의 값은 Alphabet_result.csv를 통해 직접 설정해야 한다.
    public bool Comparision(FeatureStorage feat_)
    {
        int sub_extensionMatch = feat_.extensionMatch;
        float sub_handSimilarity = feat_.handSimilarity;
        float sub_palmNormSimilarity = feat_.palmNormSimilarity;
        float[] howMuchExtend = feat_.howMuchExtend;

        // isExtended의 매칭 개수가 우리의 설정값보다 같거나 그 이상인가?
        bool extention = (sub_extensionMatch >= handExtendedNum);

        // Palm의 Normal vector가 우리가 설정한 Vector와 같거나 그 이상인가?
        bool palmNorm = (sub_palmNormSimilarity >= palmNormalVectorMag);

        // Hand gesture similiarity가 우리가 설정한 값과 같거나 그 이상인가?
        bool handSim = (sub_handSimilarity >= handGesturesim);

        // howMuchExtend[i] > 0 은 reference 에서의 굽힘의 부호와 subject에서의 굽힘의 부호가 같을 경우이다. 다르다면 마이너스 값을 가진다.
        // 부호가 같다는 것 reference와 subject에서 굽힘의 방향이 같음을 의미한다.
        // zeroCrossingNum은 각 손가락형 굽힘의 방향이 같은 개수를 확인하고, 우리가 설정한 개수과 같은지, 그 이상인지 확인한다.
        int sub_zeroCrossingNum = 0;
        for (int i = 0; i < 5; i++)
            sub_zeroCrossingNum += System.Convert.ToInt16(howMuchExtend[i] >= 0);
        bool zeronum = (sub_zeroCrossingNum >= zeroCrossingNum);

        // zeroCrossingRate는 굽힘의 방향이 같을 때 +, 다르면 -이다.
        // 즉, zeroCrossingRate의 평균인 zeroCrossAvg은 그 값이 클수록 reference 굽힘의 정도와 subject 굽힘의 정도가 일치해짐을 의미한다.
        // 이를 비교하여, 우리가 설정한 굽힘의 정도보다 같거나 크다면 True, otherwise False.
        float sub_zeroCrossingAvg = 0;
        for (int i = 0; i < 5; i++)
            sub_zeroCrossingAvg += howMuchExtend[i];
        sub_zeroCrossingAvg = sub_zeroCrossingAvg / 5;
        bool zeroavg = (sub_zeroCrossingAvg >= zeroCrossAvg);

        //Debug.Log(extention.ToString() + " " + palmNorm.ToString() + " " + handSim.ToString() + " " + zeronum.ToString() + " " + zeroavg.ToString());
        //Debug.Log(sub_zeroCrossingNum.ToString() + zeroCrossingNum.ToString());
        ////Result_TextTMP[6].text = extention.ToString() + " " + palmNorm.ToString() + " " + handSim.ToString() + " " + zeronum.ToString() + " " + zeroavg.ToString();
        bool result = extention && palmNorm && handSim && zeronum && zeroavg;

        return result;
    }
}
