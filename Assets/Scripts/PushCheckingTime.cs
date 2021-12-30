using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vrwave;
using System.Text;
using System.IO;
using System;

public class PushCheckingTime : MonoBehaviour
{
    public _VisualizerManager visualizerManager;
    
    public ActionTest actionTest;

    StringBuilder sb = new StringBuilder();
    private float actiontime = 0.0f;
    private bool startstate = true;
    private float starttime =0.0f;
    private int count = 0;
    // Update is called once per frame
    void Update()
    {
        if(_VisualizerManager.savestate && startstate)
        {
            starttime = Time.time;
            startstate = false;
        }
        //push controller checking time        
    }
    public void Save()
    {
        string filePath = Path.Combine("Assets/WaveResults", "rotation" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv");
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
    }
}
