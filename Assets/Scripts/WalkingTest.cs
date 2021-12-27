using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using System;
using Vrwave;

public class WalkingTest : MonoBehaviour
{
    [SerializeField]
    private GameObject tracker1;

    [SerializeField]
    private GameObject tracker2;
    [HideInInspector]
    public RedirectionManager redirectionManager;
    [HideInInspector]
    public _VisualizerManager visualizerManager;


    
    private bool bInit = false;
    private bool bswitch = true;

    private float initOffset = 0.0f;

    StringBuilder sb = new StringBuilder();
    void Start()
    {
        initOffset = Mathf.Abs(tracker1.transform.position.y - tracker2.transform.position.y);
        // GetRedirectManager();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (!bInit)
            {
                bInit = true;

                initOffset = Mathf.Abs(tracker1.transform.position.y - tracker2.transform.position.y);
            }
        }

    }

    private void FixedUpdate()
    {
        // if (bInit)
        // {
        if(_VisualizerManager.savestate)
        {
           
        float height_diff = Mathf.Abs(tracker1.transform.position.y - tracker2.transform.position.y) - initOffset;
        if (height_diff >= 0.01f)
        {
            if (bswitch)
            {
                bswitch = false;
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine();
            }

            sb.Append(string.Format("{0:f4}", height_diff) + ",");
            Debug.Log("jeogjt");
        }
        else
        {
            if (!bswitch)
            {
                bswitch = true;
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine();
            }

            sb.Append(string.Format("{0:f4}", height_diff) + ",");
            Debug.Log("jeogjt");
        }
        }
             
    }

    // }
    public void WalkingTestSave()
    {
        Debug.Log(sb);
        string filePath = Path.Combine("Assets/WaveResults", "Walking_" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv");
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
    }

    public void GetRedirectManager()
    {
        redirectionManager = GameObject.Find("Redirected User").transform.gameObject.GetComponent<RedirectionManager>();
    }
    public void LooxidManager()
    {
        visualizerManager = GameObject.Find("LooxidManager").transform.gameObject.GetComponent<_VisualizerManager>();
    }

}
