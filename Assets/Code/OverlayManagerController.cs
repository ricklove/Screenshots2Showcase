using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class OverlayManagerController : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {
        DisplayChildren();
    }

    private int childIndex = 0;

    private void DisplayChildren()
    {
        foreach (var c in GetComponentsInChildren<OverlayController>())
        {
            c.gameObject.SetActive(false);
        }

        if (_isPaused)
        {
            RotateChildIndex();
            transform.GetChild(childIndex).gameObject.SetActive(true);
        }
    }

    private void RotateChildIndex()
    {
        var childCount = transform.childCount;

        if (childIndex < 0)
        {
            childIndex = childCount - 1;
        }
        else if (childIndex > childCount - 1)
        {
            childIndex = 0;
        }
    }


    private bool _isPaused = false;

    void OnGUI()
    {
        GUI.depth = -10000;

        if (!_isPaused)
        {
            if (GUI.Button(new Rect(Screen.width * 0.45f, 10, Screen.width * 0.1f, Screen.height * 0.05f), "FREEZE"))
            {
                _isPaused = true;
                DisplayChildren();
            }
        }
        else
        {
            if (GUI.Button(new Rect(Screen.width * 0.45f, 10, Screen.width * 0.1f, Screen.height * 0.05f), "PLAY"))
            {
                _isPaused = false;
                DisplayChildren();
            }

            if (GUI.Button(new Rect(Screen.width * 0.3f, 10, Screen.width * 0.1f, Screen.height * 0.05f), "PREV"))
            {
                childIndex--;
                RotateChildIndex();
                DisplayChildren();
            }

            if (GUI.Button(new Rect(Screen.width * 0.6f, 10, Screen.width * 0.1f, Screen.height * 0.05f), "NEXT"))
            {
                childIndex++;
                RotateChildIndex();
                DisplayChildren();
            }

        }
    }
}
