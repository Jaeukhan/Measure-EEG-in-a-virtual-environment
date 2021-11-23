using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Looxid.Link;
using System;
using System.IO;
using System.Text;

namespace Vrwave
{
    public class _VisualizerManager : MonoBehaviour
    {
        [Header("Colors")]
        public Color ConnectedColor = Color.white;
        public Color DisconnectedColor = Color.white;

        [Header("Device Status")]
        public Transform VRCamera;

        [Header("Raw Signals")]
        public SaveData[] saveDatas;
        [Header("Action Test")]
        public ActionTest actionTest;
        [HideInInspector]
        CollisonTrigger collisonTrigger;
        [HideInInspector]
        RedirectionManager redirectionManager;
        [HideInInspector]
        SimulationManager simulationManager;
        [HideInInspector]
        PushCheckingTime pushCheckingTime;
        public WalkingTest walkingTest;
        public static bool savestate = false;
        public bool writing = false;
        public bool exitstate = false;
        public int display_num = 10;

        public bool oddballevent = true;

        private float starttime = 0;
        private const int linkFequency = 5;

        private EEGSensor sensorStatusData;
        private EEGRawSignal rawSignalData;
        private int index = 0;
        // StringBuilder timersb = new StringBuilder();



        int stand_num;
        int odd_num;
        int ordernum = 0;
        float timer;

        float waitingTime;
        List<int> randomOrder = new List<int>(); // 0 : 8개, 1 : 2개
        int[] displayOrder;
        Vector3 z1 = new Vector3(0, 0, 1);


        void Awake()
        {
            GetRedirectManager();
            GetSimulatorManager();
            GetCheckingTime();
        }

        void Start()
        {
            LooxidLinkManager.Instance.SetDebug(true);
            LooxidLinkManager.Instance.Initialize();
            LooxidLinkManager.Instance.SetDisplaySensorOffMessage(false);
            LooxidLinkManager.Instance.SetDisplayNoiseSignalMessage(false);

            timer = 0.0f;
            //stand_num = (int)(display_num * 0.8);
            //odd_num = display_num - stand_num;
            //for (int i = 0; i < stand_num; i++)
            //{
            //    randomOrder.Add(0);
            //}
            //for (int i = 0; i < odd_num; i++)
            //{
            //    randomOrder.Add(1);
            //}
            //displayOrder = Shuffle(randomOrder.ToArray());

            //GameObject.Find("Objects").transform.GetChild(0).gameObject.SetActive(true);
            //GameObject.Find("Objects").transform.GetChild(1).gameObject.SetActive(false);
        }

        void OnEnable()
        {
            LooxidLinkData.OnReceiveEEGRawSignals += OnReceiveEEGRawSignals;
        }
        void OnDisable()
        {
            LooxidLinkData.OnReceiveEEGRawSignals -= OnReceiveEEGRawSignals;
        }



        // Data Subscription
        void OnReceiveEEGSensorStatus(EEGSensor sensorStatusData)
        {
            this.sensorStatusData = sensorStatusData;
        }

        void OnReceiveEEGRawSignals(EEGRawSignal rawSignalData)
        {
            if (!writing) return;
            savestate = true;
            int numChannel = System.Enum.GetValues(typeof(EEGSensorID)).Length;
            for (int i = 0; i < numChannel; i++)
            {
                saveDatas[i].SetValue(rawSignalData.FilteredRawSignal((EEGSensorID)i));
            }
        }

        void FixedUpdate()
        {

            //if (savestate && oddballevent)
            //{
            //    StartCoroutine(Displaytarget());
            //    oddballevent = false;
            //}

        }
        IEnumerator Displaytarget()
        {
            while (true)
            {
                if (RedirectionManager.usingreset)
                {
                    yield return new WaitForSeconds(1.1f);
                }
                waitingTime = UnityEngine.Random.Range(0.5f, 1.5f);//0.5~1.5초 사이동안 나옴
                yield return new WaitForSeconds(waitingTime);


                if (displayOrder[ordernum] == 0)
                {
                    GameObject.Find("targets").transform.GetChild(0).gameObject.SetActive(true);//standard

                    yield return new WaitForSeconds(0.5f);
                    GameObject.Find("targets").transform.GetChild(0).gameObject.SetActive(false);//standar
                    if (actionTest.GetoddballCount())
                    {
                        Debug.Log(timer);
                        Debug.Log(ordernum + 1);
                    }
                }
                else if (displayOrder[ordernum] == 1)
                {
                    GameObject.Find("targets").transform.GetChild(1).gameObject.SetActive(true); //oddball

                    yield return new WaitForSeconds(0.5f);
                    GameObject.Find("targets").transform.GetChild(1).gameObject.SetActive(false);//oddball     
                    if (actionTest.GetoddballCount())
                    {
                        Debug.Log(timer);
                        Debug.Log(ordernum + 1);
                    }

                }

                ordernum++;
                if (ordernum == display_num)
                {
                    ordernum = 0;
                    exitstate = true;
                    break;
                }
            }
        }


        void Update()
        {
            // if (savestate)
            // {
            //     starttime = Time.time;
            // }

            PushExit();
        }
        void PushExit()
        {
             if (writing && exitstate)
            {
                Debug.Log("writing");
                //Write();
                EEG2Csv();
                savestate = false;
                writing = false;
                exitstate = false;
            }
        }
        
        void Write()
        {
            for (int i = 0; i < saveDatas.Length; i++)
            {
                saveDatas[i].Save(i);
            }
        }

        public int[] Shuffle(int[] deck)
        {
            for (int i = 0; i < deck.Length; i++)
            {
                int temp = deck[i];
                int randomIndex = UnityEngine.Random.Range(0, deck.Length);
                deck[i] = deck[randomIndex];
                deck[randomIndex] = temp;
            }
            return deck;
        }

        public float[] Shuffle(float[] deck)
        {
            for (int i = 0; i < deck.Length; i++)
            {
                float temp = deck[i];
                int randomIndex = UnityEngine.Random.Range(0, deck.Length);
                deck[i] = deck[randomIndex];
                deck[randomIndex] = temp;
            }
            return deck;
        }

            public double Min(List<double> minList)
        {
            if (minList.Count <= 0) return 0.0;
            double min = minList[0];
            for (int i = 0; i < minList.Count; i++)
            {
                if (!double.IsNaN(minList[i]))
                {
                    if (minList[i] < min) min = minList[i];
                }
            }
            return min;
        }
        public double Max(List<double> maxList)
        {
            if (maxList.Count <= 0) return 0.0;
            double max = maxList[0];
            for (int i = 0; i < maxList.Count; i++)
            {
                if (!double.IsNaN(maxList[i]))
                {
                    if (maxList[i] > max) max = maxList[i];
                }
            }
            return max;
        }
        public void SphereCollision()
        {
            index++;
            Debug.Log(timer);
            BallSetActive();
            GainChangeExperiment();
        }
        public void BallSetActive()
        {
            if(index %2 == 0)
            {
                GameObject.Find("Objects").transform.GetChild(0).gameObject.SetActive(true);
                GameObject.Find("Objects").transform.GetChild(1).gameObject.SetActive(false);
            }
            else if(index %2 ==  1)
            {
                GameObject.Find("Objects").transform.GetChild(0).gameObject.SetActive(false);
                GameObject.Find("Objects").transform.GetChild(1).gameObject.SetActive(true);
            }
            if(index == 2)
            {
               exitstate =true;
            }
        }

        public void EEG2Csv()
        {
            List<string[]> userData = new List<string[]>();
            string[] tempData = new string[7] {"", "AF3", "AF4", "Fp1", "Fp2", "AF7", "AF8" };

            userData.Add(tempData);
            for (int i = 0; i < saveDatas[5].GetLength(); i++)
            {
                tempData = new string[7];
                tempData[0] = i.ToString();
                tempData[1] = saveDatas[0].GetValueOne(i).ToString();
                tempData[2] = saveDatas[1].GetValueOne(i).ToString();
                tempData[3] = saveDatas[2].GetValueOne(i).ToString();
                tempData[4] = saveDatas[3].GetValueOne(i).ToString();
                tempData[5] = saveDatas[4].GetValueOne(i).ToString();
                tempData[6] = saveDatas[5].GetValueOne(i).ToString();
                userData.Add(tempData);
            }
            string[][] output = new string[userData.Count][];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = userData[i];
            }
            int length = output.GetLength(0);
            string delimiter = ",";
            StringBuilder sb = new StringBuilder();

            for (int index = 0; index < length; index++)
            {
                sb.AppendLine(string.Join(delimiter, output[index]));
            }
            string filePath = Path.Combine("Assets/WaveResults", "EEG_"+DateTime.Now.ToString("yyyy-MM-dd") + ".csv");
            StreamWriter outStream = System.IO.File.CreateText(filePath);
            outStream.WriteLine(sb);
            outStream.Close();
            saveDatas.Initialize();
            // walkingTest.WalkingTestSave();
            // string resetpath = Path.Combine("Assets/WaveResults", "Resettime" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv");
            // StreamWriter outStream2 = System.IO.File.CreateText(resetpath);
            // outStream2.WriteLine(sb);
            // outStream2.Close();
            // redirectionManager.ExitGame();
        }

        public void EEG2Csv(string name)
        {
            List<string[]> userData = new List<string[]>();
            string[] tempData = new string[7] { "", "AF3", "AF4", "Fp1", "Fp2", "AF7", "AF8" };

            userData.Add(tempData);
            for (int i = 0; i < saveDatas[5].GetLength(); i++)
            {
                tempData = new string[7];
                tempData[0] = i.ToString();
                tempData[1] = saveDatas[0].GetValueOne(i).ToString();
                tempData[2] = saveDatas[1].GetValueOne(i).ToString();
                tempData[3] = saveDatas[2].GetValueOne(i).ToString();
                tempData[4] = saveDatas[3].GetValueOne(i).ToString();
                tempData[5] = saveDatas[4].GetValueOne(i).ToString();
                tempData[6] = saveDatas[5].GetValueOne(i).ToString();
                userData.Add(tempData);
            }
            string[][] output = new string[userData.Count][];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = userData[i];
            }
            int length = output.GetLength(0);
            string delimiter = ",";
            StringBuilder sb = new StringBuilder();

            for (int index = 0; index < length; index++)
            {
                sb.AppendLine(string.Join(delimiter, output[index]));
            }
            string filePath = Path.Combine("Assets/WaveResults", "EEG_"+name+'_' + DateTime.Now.ToString("yyyy-MM-dd") + ".csv");
            StreamWriter outStream = System.IO.File.CreateText(filePath);
            outStream.WriteLine(sb);
            outStream.Close();
            saveDatas.Initialize();
        }

        public void GetRedirectManager()
        {
            redirectionManager = GameObject.Find("Redirected User").transform.gameObject.GetComponent<RedirectionManager>();
        }
        public void GetSimulatorManager()
        {
            simulationManager = GameObject.Find("Redirected User").transform.gameObject.GetComponent<SimulationManager>();
        }
        public void GetCheckingTime()
        {
            pushCheckingTime  = GameObject.Find("LooxidManager").transform.gameObject.GetComponent<PushCheckingTime>();
        }
        public void GainChangeExperiment()
        {
            if(index == 1)
            {
                simulationManager.SetCondAlogorithm("S2C");
                redirectionManager.MAX_TRANS_GAIN = 0.26f;
                redirectionManager.MIN_TRANS_GAIN = -0.14f;
                redirectionManager.MAX_ROT_GAIN = 1.49f;
                redirectionManager.MIN_ROT_GAIN = 0.8f;
                redirectionManager.CURVATURE_RADIUS = 6.4f;
            }
            //if(index == 2)
            //{
            //    simulationManager.SetCondAlogorithm("S2C");
            //    redirectionManager.MAX_TRANS_GAIN = 0.26f;
            //    redirectionManager.MIN_TRANS_GAIN = -0.079f;
            //    redirectionManager.MAX_ROT_GAIN = 0.49f;
            //    redirectionManager.MIN_ROT_GAIN = -0.2f;
            //    redirectionManager.CURVATURE_RADIUS = 7.5f;
            //}
        }
        // public void ResetTimepint()
        // {
        //     if(RedirectionManager.usingreset)
        //     {
        //         timersb.Append(string.Format("{0:f2}", timer) + ",");
        //     }
        // }

    }
}
