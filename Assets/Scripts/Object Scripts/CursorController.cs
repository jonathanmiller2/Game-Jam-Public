using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Material[] material;
    public GameObject cursor;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = false;
        gameObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 1f);
        // Debug.Log(gameObject.transform.position);

        if (Input.GetMouseButtonDown(0))
        {
            foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.material = material[1];
            }
        }

        else if (Input.GetMouseButtonUp(0))
        {
            foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.material = material[0];
            }
        }
    }
}
