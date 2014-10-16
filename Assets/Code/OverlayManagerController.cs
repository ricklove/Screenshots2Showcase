using UnityEngine;

[ExecuteInEditMode]
public class OverlayManagerController : MonoBehaviour
{
    void Update()
    {
        UpdateOverlays();
    }

    private int _lastVisibleChildIndex = 0;
    public int visibleChildIndex = 0;

    private static bool IsEditMode
    {
        get
        {
            return Application.isEditor && !UnityEditor.EditorApplication.isPlaying;
        }
    }

    public void UpdateOverlays()
    {
        if (IsEditMode)
        {
            var i = 0;
            foreach (Transform c in transform)
            {
                if (c.gameObject.activeSelf && i != _lastVisibleChildIndex)
                {
                    visibleChildIndex = i;
                    break;
                }
                i++;
            }
        }

        foreach (Transform c in transform)
        {
            c.gameObject.SetActive(false);
        }

        if (_isPaused || IsEditMode)
        {
            RotateChildIndex();
            transform.GetChild(visibleChildIndex).gameObject.SetActive(true);
        }

        _lastVisibleChildIndex = visibleChildIndex;
    }

    private void RotateChildIndex()
    {
        var childCount = transform.childCount;

        if (visibleChildIndex < 0)
        {
            visibleChildIndex = childCount - 1;
        }
        else if (visibleChildIndex > childCount - 1)
        {
            visibleChildIndex = 0;
        }
    }


    private bool _isPaused = false;

    void OnGUI()
    {
        if (!IsEditMode)
        {
            GUI.depth = -10000;

            if (!_isPaused)
            {
                if (GUI.Button(new Rect(Screen.width * 0.45f, 10, Screen.width * 0.1f, Screen.height * 0.05f), "FREEZE"))
                {
                    _isPaused = true;
                    UpdateOverlays();
                    Time.timeScale = 0;
                }
            }
            else
            {

                if (GUI.Button(new Rect(Screen.width * 0.45f, 10, Screen.width * 0.1f, Screen.height * 0.05f), "PLAY"))
                {
                    _isPaused = false;
                    UpdateOverlays();
                    Time.timeScale = 1;
                }

                if (GUI.Button(new Rect(Screen.width * 0.3f, 10, Screen.width * 0.1f, Screen.height * 0.05f), "PREV"))
                {
                    visibleChildIndex--;
                    RotateChildIndex();
                    UpdateOverlays();
                }

                if (GUI.Button(new Rect(Screen.width * 0.6f, 10, Screen.width * 0.1f, Screen.height * 0.05f), "NEXT"))
                {
                    visibleChildIndex++;
                    RotateChildIndex();
                    UpdateOverlays();
                }

            }
        }
    }

}
