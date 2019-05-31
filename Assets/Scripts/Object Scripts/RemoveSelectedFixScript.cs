using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RemoveSelectedFixScript : MonoBehaviour
{
    public void ClearSelected()
    {
    	EventSystem.current.SetSelectedGameObject(null);
    }
}
