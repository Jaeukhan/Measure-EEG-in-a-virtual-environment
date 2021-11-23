using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomorder : MonoBehaviour
{
    int[] a = new int[11];
    int ran = 0;

    void Start()
    {

    for(int i = 0; i < a.Length; i++)
    {
        a[i] = i+5;
    }



    for (int i = 0; i < a.Length; i++)
    {

        int temp = a[i];

        ran = UnityEngine.Random.Range(0, 10);

        a[i] = a[ran];

        a[ran] = temp;
            Debug.Log(a[i]);
    }
        
    }

}
