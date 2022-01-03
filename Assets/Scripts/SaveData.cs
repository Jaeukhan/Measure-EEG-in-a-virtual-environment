using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Looxid.Link;

namespace Vrwave {
    public class SaveData : MonoBehaviour
    {
        public int Width = 265;
        public int Height = 90;
        public bool center_is_zero = true;
        private List<double> eeglist = new List<double>();

        // private EEGdatas eEGdatas = new EEGdatas();
        private double[] datalist;
        private bool saving = true;
        private LineRenderer line;
        //private double[] datalist = new double[2000];
        // Start is called before the first frame update
        private void OnEnable()
        {
            line = GetComponent<LineRenderer>();
            StartCoroutine(DrawChart());
        }
        void Update()
        {
            if (_VisualizerManager.savestate && saving)
            {
                StartCoroutine(eegDataAppend());
                saving = false;
            }
            
        }
        

        IEnumerator DrawChart()
        {
            while (this.gameObject.activeSelf)
            {
                yield return new WaitForSeconds(0.1f);

                if (datalist != null)
                {
                    if (datalist.Length > 1)
                    {
                        line.positionCount = datalist.Length;
                        float xDist = (float)Width / (float)(datalist.Length - 1);

                        for (int x = 0; x < datalist.Length; x++)
                        {
                            int dataHeight = Height / 2;

                            if (x < datalist.Length)
                            {
                                if (double.IsNaN(datalist[x]))
                                {
                                    dataHeight = 0;
                                }
                                else
                                {
                                    if (center_is_zero)
                                    {
                                        dataHeight = Mathf.FloorToInt((float)(datalist[x] + 0.5) * (float)Height);
                                    }
                                    else
                                    {
                                        dataHeight = Mathf.FloorToInt((float)datalist[x] * (float)Height);
                                    }
                                }
                            }

                            float pos_x = xDist * (float)x;
                            line.SetPosition(x, new Vector3(pos_x, dataHeight, 0.0f));
                        }
                    }
                }
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
                    eeglist.Add(datalist[i]);
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
            return eeglist;
        }
        public int GetLength()
        {
            return eeglist.ToArray().Length;
        }
        public double GetValueOne(int i)
        {
            return eeglist[i];
        }
        public void Printdatalist()
        {
            Debug.Log(datalist.Length);
        }

        public void Printeeg()
        {
            Debug.Log(eeglist.ToArray().Length);
        }

        public void Save(int num)
        {
            Debug.Log(eeglist.ToArray().Length);
            string filePath = Path.Combine("Assets/WaveResults", (EEGSensorID)num + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json");
            File.WriteAllText(filePath, JsonUtility.ToJson(eeglist));
        }
        public void Save(string name)
        {
            string filePath = Path.Combine("Assets/WaveResults", name + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json");
            File.WriteAllText(filePath, JsonUtility.ToJson(eeglist));
        }
        public void DataClear()
        {
            eeglist.Clear();
        }
 
        // public class EEGdatas
        // { 
        //     public List<double> eeglist = new List<double>();
        // }
    }

}