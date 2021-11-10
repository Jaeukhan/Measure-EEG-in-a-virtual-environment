using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vrwave;

public class CollisonTrigger : MonoBehaviour
{
    public _VisualizerManager visualizerManager;

    public void OnTriggerEnter(Collider other)
    {        
        visualizerManager.SphereCollision();
    }

}
