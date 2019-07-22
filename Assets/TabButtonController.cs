using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//taken from: https://stackoverflow.com/questions/51016364/how-to-focus-inputfields-while-click-on-tab-button-in-unity-3d/51019562


public class TabButtonController : MonoBehaviour, IUpdateSelectedHandler
{
    public Selectable nextField;

    public void OnUpdateSelected(BaseEventData data)
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            nextField.Select();
    }
}
