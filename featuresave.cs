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

public class featuresave : MonoBehaviour
{
    // 71개
    // palmtranx, palmtrany, plamtranz : plam translation(position)
    // palmnormx, palmnormy, palmnormz : plam normal vector
    // fn bm axis 예시
    // f1b1x : first finger, first bone, x-axis
    // f3b4y : third finger, fourth bone, y-axis


    // 1104 쓰기
    private string[] m_ColumnHeadings = { "plamtranx", "plamtrany", "plamtranz", "plamnormx", "plamnormy", "plamnormx",
                                          "isextended1", "isextended2", "isextended3", "isextended4", "isextended5",
                                          "f1b1x", "f1b1y", "f1b1z", "f1b2x", "f1b2y", "f1b2z", "f1b3x", "f1b3y", "f1b3z", "f1b4x", "f1b4y", "f1b4z",
                                          "f2b1x", "f2b1y", "f2b1z", "f2b2x", "f2b2y", "f2b2z", "f2b3x", "f2b3y", "f2b3z", "f2b4x", "f2b4y", "f2b4z",
                                          "f3b1x", "f3b1y", "f3b1z", "f3b2x", "f3b2y", "f3b2z", "f3b3x", "f3b3y", "f3b3z", "f3b4x", "f3b4y", "f3b4z",
                                          "f4b1x", "f4b1y", "f4b1z", "f4b2x", "f4b2y", "f4b2z", "f4b3x", "f4b3y", "f4b3z", "f4b4x", "f4b4y", "f4b4z",
                                          "f5b1x", "f5b1y", "f5b1z", "f5b2x", "f5b2y", "f5b2z", "f5b3x", "f5b3y", "f5b3z", "f5b4x", "f5b4y", "f5b4z"};

    public LeapServiceProvider LeapServiceProvider;

    private string m_FilePath;
    public string m_Path = Application.streamingAssetsPath;
    //public string p_Path = "C:\\Users\\user\\Desktop\\HCI\\Project\\1105\\Assets\\StreamingAssets";

    private Vector3 palm;
    private Vector3 bone;

    //private string resultString;
    // https://asta8080.tistory.com/6
    StringBuilder resultString = new StringBuilder();

    // 버튼을 누른 뒤 측정까지 걸리는 시간.
    public float waitngTime = 3.0f;

    private float time;
    public TMP_InputField LabelTMP;
    public TextMeshProUGUI TimeTMP;
    private bool isButtonPressed = false;
    private bool isLoadCsv = false;

    string label;

    private string[] resultArray;
    private List<string[]> m_WriteRowData = new List<string[]>();

    // Start is called before the first frame update
    void Start()
    {
        time = waitngTime;
        //column_size = m_ColumnHeadings.Length;
        label = LabelTMP.text;
        m_FilePath = "/Alphabet_" + label + ".csv";
    }
    private void Update()
    {

        label = LabelTMP.text;
        m_FilePath = "/Alphabet_" + label + ".csv";
        Debug.Log(m_Path + m_FilePath);
        //Debug.Log(p_Path + m_FilePath);
        if (isButtonPressed)
        {
            time -= Time.deltaTime;
            TimeTMP.text = "Time: " + time.ToString("N2") + " sec";
            if (time < 0.0f)
            {
                // 제스처의 특징을 resultString에 저장
                FeatureToArray();
                
                // csv 파일 불러오기
                if(!isLoadCsv)
                {
                    // 만약 디렉토리에 이미 csv 파일이 있다면, 저장된 값들을 불러온다.
                    // 불러오지 않으면 기존의 값들이 사라짐.
                    ReadCsv(m_WriteRowData, m_FilePath);
                    isLoadCsv = true;
                }

                //resultString 값들을 csv파일에 저장
                resultArray = resultString.ToString().Split(" ");
                m_WriteRowData.Add(resultArray);
                WriteCsv(m_WriteRowData, m_FilePath);

                // WriteCsv 실행될 때마다 파일을 새로 쓰게 된다.
                // 즉, 기존의 값들이 있다면 그걸 지우고 새로 씀. 따라서 ReadCsv로 기존의 값들을 모두 m_WriteRowData에 저장해야 한다.
                // Clear()쓰면 안됨. //m_WriteRowData.Clear();

                resultString.Clear();
                Debug.Log("resultString Length : " + resultString.Length);

                isButtonPressed = false;
                time = waitngTime;
            }
        }

    }

    void FeatureToArray()
    {

        for (int i = 0; i < LeapServiceProvider.CurrentFrame.Hands.Count; i++)
        {
            Hand _hand = LeapServiceProvider.CurrentFrame.Hands[i];
            //Debug.Log("LeapServiceProvider.CurrentFrame.Hands.Count : " + LeapServiceProvider.CurrentFrame.Hands.Count.ToString());

            palm = _hand.PalmPosition;
            resultString.Append(palm.x.ToString() + " " + palm.x.ToString() + " " + palm.z.ToString() + " ");
            palm = _hand.PalmNormal;
            resultString.Append(palm.x.ToString() + " " + palm.x.ToString() + " " + palm.z.ToString() + " ");

            //m_WriteRowData.Add(palm.x.ToString());

            for(int f = 0; f < _hand.Fingers.Count; f++)
            {
                Finger finger_ = _hand.Fingers[f];
                bool extended = finger_.IsExtended;
                if (extended)
                    resultString.Append(1.ToString() + " ");
                else
                    resultString.Append(0.ToString() + " ");
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
                    resultString.Append(bone.x.ToString() + " " + bone.x.ToString() + " " + bone.z.ToString() + " ");
                }
            }
        }
    }

    public void ButtonPress()
    {
        isButtonPressed = true;
    }

    public void ReadCsv(List<string[]> rowData, string filePath)
    {
        try
        {
            string[] lines = System.IO.File.ReadAllLines(m_Path + filePath);
            //string[] lines = System.IO.File.ReadAllLines(p_Path + filePath);
            for (int i = 0; i < lines.Length -1; i++)
            {
                string[] columns = lines[i].Split(',');
                string[] csv_input = columns.Take(columns.Length - 1).ToArray();
                Debug.Log("Read csv " + csv_input.Length);
                Debug.Log("Read csv " + csv_input);

                rowData.Add(csv_input);
                WriteCsv(rowData, filePath);
                rowData.Clear();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("csv 파일이 없습니다 : {0}", e.Message);
        }
    }

    public void WriteCsv(List<string[]> rowData, string filePath)
    {
        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder stringBuilder = new StringBuilder();

        for (int index = 0; index < length; index++)
            stringBuilder.AppendLine(string.Join(delimiter, output[index]));

        //Stream fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
        // StreamWriter outStream = new StreamWriter(fileStream, Encoding.UTF8);
        //   m_Path = "C:\\Users\\user\\Desktop\\HCI\\Project\\1105\\Assets\\StreamingAssets";
        string filepath = m_Path;
        //string filepath = p_Path;

        Debug.Log("m_Path : " + m_Path);
        //Debug.Log("p_Path : " + p_Path);

        if (!Directory.Exists(filepath))
        {
            Debug.Log("경로 생성");
            Directory.CreateDirectory(filepath);
        }
        //string fileName = m_FilePrefix +
        //            DateTime.Now.ToString("MMddHHmmss") + ".csv"; ;

        StreamWriter outStream = System.IO.File.CreateText(m_Path + filePath);
        //StreamWriter outStream = System.IO.File.CreateText(p_Path + filePath);
        outStream.WriteLine(stringBuilder);
        outStream.Close();
    }

}

