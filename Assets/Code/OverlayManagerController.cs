using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class OverlayManagerController : MonoBehaviour
{
    void Update()
    {
        UpdateOverlays();

        if (_isTakingScreenshots)
        {
            if (_screenshotSteps.Any())
            {
                var next = _screenshotSteps.First();
                _screenshotSteps.RemoveAt(0);
                next();
            }
            else
            {
                _isTakingScreenshots = false;
            }
        }
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
            if (transform.childCount > 0)
            {
                transform.GetChild(visibleChildIndex).gameObject.SetActive(true);
            }
        }

        _lastVisibleChildIndex = visibleChildIndex;
    }

    private void RotateChildIndex()
    {
        var childCount = transform.childCount;

        if (childCount == 0)
        {
            visibleChildIndex = 0;
            return;
        }

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
    private bool _isTakingScreenshots = false;

    void OnGUI()
    {
        if (!IsEditMode && !_isTakingScreenshots)
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

                if (GUI.Button(new Rect(Screen.width * 0.45f, 10 + Screen.height * 0.05f + 10, Screen.width * 0.1f, Screen.height * 0.05f), "SCREENSHOT"))
                {
                    _isTakingScreenshots = true;
                    CreateScreenshotSteps();
                }
            }
        }


    }

    private List<Action> _screenshotSteps;


    private static bool _hasShownExplorer = false;
    void CreateScreenshotSteps()
    {
        Debug.Log("CreateScreenshotSteps Start");

        _screenshotSteps = new List<Action>();

        string folderPath = Application.persistentDataPath + "/Screenshots/";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (!_hasShownExplorer)
        {
            _hasShownExplorer = true;
            ExplorerHelper.ShowInExplorer(folderPath);
        }

        var dateTimeString = DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss");

        var sizes = new List<Rect>();
        sizes.Add(new Rect(0, 0, 100, 100));
        sizes.Add(new Rect(0, 0, 150, 100));
        sizes.Add(new Rect(0, 0, 100, 150));

        var i = 0;

        foreach (var s in sizes)
        {
            var size = s;
            var index = i;

            _screenshotSteps.Add(() =>
            {
                // Take Screenshot
                Screen.SetResolution((int)size.width, (int)size.height, false);
            });

            _screenshotSteps.Add(() =>
            {
                Application.CaptureScreenshot(
                    folderPath
                    + dateTimeString
                    + " - " + index
                    + " - " + (int)size.width + "x" + (int)size.height + ".png", 2);
            });

            i++;
        }

        _screenshotSteps.Add(() =>
        {
            _isTakingScreenshots = false;
        });

        Debug.Log("CreateScreenshotSteps End");
    }

}


public static class ExplorerHelper
{
    public static void ShowInExplorer(string path)
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        path = path.Replace(@"/", @"\");
        //System.Diagnostics.Process.Start("explorer.exe", "/select," + path);
        System.Diagnostics.Process.Start("explorer.exe", path);
#endif
    }
}

