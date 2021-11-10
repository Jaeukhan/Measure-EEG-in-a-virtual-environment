using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
namespace Vrwave
{
    public class LineData : MonoBehaviour
    {
  
        private SaveData saveData = new SaveData();


        public void SetValue(double[] datalist)
        {
            //saveData.timestamp.Add(time);
            for (int i = 0; i < datalist.Length; i++)
            {
                saveData.datalist.Add(datalist[i]);
            }
            
 
        }

        public void Save(int num)
        {
            string filePath = Path.Combine("Assets/WaveResults", num.ToString() + "linewave.json");
            File.WriteAllText(filePath, JsonUtility.ToJson(saveData));
        }

        public void Initialize()
        {
            saveData.datalist.Clear();
            //saveData.timestamp.Clear();
        }

        public class SaveData
        {
            public List<double> datalist = new List<double>();
        }
    }



}

