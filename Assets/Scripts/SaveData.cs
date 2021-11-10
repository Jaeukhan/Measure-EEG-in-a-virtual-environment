using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Looxid.Link;

namespace Vrwave {
    public class SaveData : MonoBehaviour
    {

        private EEGdatas eEGdatas = new EEGdatas();
        private double[] datalist;
        private bool saving = true;
        //private double[] datalist = new double[2000];
        // Start is called before the first frame update

        void Update()
        {
            if (_VisualizerManager.savestate && saving)
            {
                StartCoroutine(eegDataAppend());
                saving = false;
            }
        }
        // Update is called once per frame
        IEnumerator eegDataAppend()
        {
            //Debug.Log(eEGdatas.eeglist.ToArray().Length);
            while (true)
            {
                for (int i = 0; i < datalist.Length; i++)
                {
                    eEGdatas.eeglist.Add(datalist[i]);
                }
                yield return new WaitForSeconds(4);
            }
        }

        public void SetValue(double[] datalist)
        {
            this.datalist = datalist;
        }

        public List<double> GetValue()
        {
            return eEGdatas.eeglist;
        }
        public int GetLength()
        {
            return eEGdatas.eeglist.ToArray().Length;
        }
        public double GetValueOne(int i)
        {
            return eEGdatas.eeglist[i];
        }
        public void Printdatalist()
        {
            Debug.Log(datalist.Length);
        }

        public void Printeeg()
        {
            Debug.Log(eEGdatas.eeglist.ToArray().Length);
        }

        public void Save(int num)
        {
            Debug.Log(eEGdatas.eeglist.ToArray().Length);
            string filePath = Path.Combine("Assets/WaveResults", (EEGSensorID)num + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json");
            File.WriteAllText(filePath, JsonUtility.ToJson(eEGdatas));
        }
        public void Save(string name)
        {
            string filePath = Path.Combine("Assets/WaveResults", name + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json");
            File.WriteAllText(filePath, JsonUtility.ToJson(eEGdatas));
        }
 
        public class EEGdatas
        { 
            public List<double> eeglist = new List<double>();
        }
    }

}