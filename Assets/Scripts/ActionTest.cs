using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Vrwave;

public class ActionTest : MonoBehaviour
{
    public SteamVR_Action_Boolean greater;
    public SteamVR_Action_Boolean smaller;
    public SteamVR_Action_Boolean trialexit;
    [HideInInspector]
    public RedirectionManager redirectionManager;
    [HideInInspector]
    public _VisualizerManager visualizerManager;
    [HideInInspector]
    public bool countup = true;
    [HideInInspector]
    public AudioSource beep;
    public string flag = null;
    // Update is called once per frame
    void  Awake()
    {
         redirectionManager = GameObject.Find("Redirected User").transform.gameObject.GetComponent<RedirectionManager>();
         visualizerManager = GameObject.Find("LooxidManager").transform.gameObject.GetComponent<_VisualizerManager>();
         beep = GameObject.Find("beep").transform.gameObject.GetComponent<AudioSource>();
    }
    void Update()
    {
        if (_VisualizerManager.savestate &&Input.GetKeyDown(KeyCode.A))
        {   
            Debug.Log("종료");
            visualizerManager.exitstate = true;
            GameObject.Find("TurnSign").transform.GetChild(0).GetComponent<Canvas>().enabled = false; 
            GameObject.Find("TurnSign").transform.GetChild(1).GetComponent<Canvas>().enabled = false; 
            // GameObject.Find("RT").transform.GetChild(0).GetComponent<Canvas>().enabled = false; 
            // GameObject.Find("RT").transform.GetChild(1).GetComponent<Canvas>().enabled = false; 
            redirectionManager.eyesave = false;
            beep.Play();
        }
        // if(!visualizerManager.writing && greater.state)
        // {
        //     Debug.Log("greater");
        //     // RedirectionManager.Global.selection = "greater";
        //     if (countup)
        //     {
        //         visualizerManager.Gcount += 1;
        //         countup = false;
        //     }
        //     visualizerManager.possetting = true;
        //     visualizerManager.stopsign = false;
        //     Debug.Log(visualizerManager.Gcount.ToString()+','+redirectionManager.gains[visualizerManager.Gcount].ToString());
        // }
        if(!visualizerManager.writing)
        {
            if(smaller.state)
            {
                Debug.Log("smaller");
                // RedirectionManager.Global.selection = "greater";
                if (flag==null)
                {
                    flag = "t";
                    visualizerManager.Save2afc("s", visualizerManager.Gcount);
                }
                // Debug.Log(visualizerManager.Gcount.ToString()+','+redirectionManager.gains[visualizerManager.Gcount].ToString());
                if (countup)
                {
                    visualizerManager.Gcount += 1;
                    countup = false;
                }
                visualizerManager.possetting = true;
                visualizerManager.stopsign = false;

            }
            else if  (greater.state)
            {
                Debug.Log("greater");
                // RedirectionManager.Global.selection = "greater";
                if (flag==null)
                {
                    flag = "t";
                    visualizerManager.Save2afc("g", visualizerManager.Gcount);
                }
                    // Debug.Log(visualizerManager.Gcount.ToString()+','+redirectionManager.gains[visualizerManager.Gcount].ToString()); 
                if (countup)
                {
                    visualizerManager.Gcount += 1;
                    countup = false;
                }
                visualizerManager.possetting = true;
                visualizerManager.stopsign = false;
            }
        }
    }
}

