using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JankFix : MonoBehaviour
{

    // Update is called once per frame
    void Start()
    {
        if (gameObject.transform.rotation.x == -180f) {

            var rot = transform.rotation;
            rot.x = 0;
            rot.z = 90;
            rot.y = 0;
            transform.rotation = rot;
        }
     
    }
}
