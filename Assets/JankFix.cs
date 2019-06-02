using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JankFix : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        var rot = transform.rotation;
        rot.x = 0;
        rot.z = 0;
        rot.y = 0;
        transform.rotation = rot;
    }
}
