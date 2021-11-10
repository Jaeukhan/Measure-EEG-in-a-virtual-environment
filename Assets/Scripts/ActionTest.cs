using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Vrwave;

public class ActionTest : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean oddballCount;


    private bool saving = true;
    // Update is called once per frame
    void Update()
    {
    }

    public bool GetoddballCount()
    {
        return oddballCount.GetState(handType);
    }
}
