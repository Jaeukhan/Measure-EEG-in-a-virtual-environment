using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class CSVFileWriter : MonoBehaviour
{
    private List<string[]> studentData = new List<string[]>();

    void Write()
    {

        string[] tempStudentData = new string[6];
        string m_FilePrefix = "EEG";
        tempStudentData[0] = "Fp1";
        tempStudentData[1] = "Fp2";
        tempStudentData[2] = "Af3";
        tempStudentData[3] = "Af4";
        tempStudentData[4] = "Af7";
        tempStudentData[5] = "Af8";

        studentData.Add(tempStudentData);
        for (int i = 0; i < 10; i++)
        {
            tempStudentData = new string[6];
            tempStudentData[0] = "Micheal" + i; // Name
            tempStudentData[1] = (i + 20).ToString(); // Age
            tempStudentData[2] = i.ToString(); // ID
            studentData.Add(tempStudentData);
        }

        string[][] output = new string[studentData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = studentData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
        {
            sb.AppendLine(string.Join(delimiter, output[index]));
        }

        string filePath = Application.streamingAssetsPath + @"\" + m_FilePrefix+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";

        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
    }

}
