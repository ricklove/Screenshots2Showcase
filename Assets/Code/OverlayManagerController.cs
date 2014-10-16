﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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
#if UNITY_EDITOR
            return Application.isEditor && !UnityEditor.EditorApplication.isPlaying;
#else
            return false;
#endif
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

        var originalSize = new Rect(0, 0, Screen.width, Screen.height);

        //var sizes = new List<Rect>();

        //sizes.Add(new Rect(0, 0, 800, 600));
        //sizes.Add(new Rect(0, 0, 150, 100));
        //sizes.Add(new Rect(0, 0, 100, 150));

        var sizes = ScreenshotSizeHelper.ScreenShotSizes;

        var i = 0;

        foreach (var s in sizes)
        {
            var size = s;
            var index = i;

            // Skip impossible sizes
            var resolution = Screen.currentResolution;
            if (size.width > resolution.width
                || size.height > resolution.height)
            {
                Debug.Log("Resolution not big enough for size: " + size.width + "x" + size.height);
                continue;
            }

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
                    + " - " + (int)size.width + "x" + (int)size.height + ".png", 1);
            });

            i++;
        }

        _screenshotSteps.Add(() =>
        {
            Screen.SetResolution((int)originalSize.width, (int)originalSize.height, false);
        });

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

public static class ScreenshotSizeHelper
{
    private static List<Rect> _screenShotSizes;
    public static List<Rect> ScreenShotSizes
    {
        get
        {
            if (_screenShotSizes == null)
            {
                var lines = ScreenShotsSizesText.Split('\r', '\n')
                    .Where(l => !string.IsNullOrEmpty(l) && !string.IsNullOrEmpty(l.Trim()) && !l.StartsWith("//"))
                    .ToList();

                var sizes = new List<Rect>();

                foreach (var line in lines)
                {
                    var parts = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    int a;
                    int b;

                    if (parts.Length >= 2 && int.TryParse(parts[0], out a) && int.TryParse(parts[1], out b))
                    {
                        sizes.Add(new Rect(0, 0, a, b));
                        sizes.Add(new Rect(0, 0, b, a));
                    }
                    else
                    {
                        Debug.Log("Invalid screen size entry: " + line);
                    }
                }

                _screenShotSizes = sizes;
            }

            return _screenShotSizes;
        }
    }

    public static string ScreenShotsSizesText = @"
// width	height		zoom	name
// All sizes are portrait

// Apple
640 	960			2		iPhone4			
640 	1136		2		iPhone5			
768 	1024		1		iPad			
1536 	2048		2		iPadHi			

// Android

// From http://stackoverflow.com/questions/6272384/most-popular-screen-sizes-resolutions-on-android-phones
// http://developer.android.com/guide/practices/screens_support.html#testing

1600		2560	2		Android_XLarge
//768		1366	1.5		Android_XLarge

1200	1920		2		Android_XLarge
800		1280		1.5		Android_XLarge	
//768		1280		1.5		Android_XLarge
//800		1024		1		Android_XLarge
//768		1024		1		Android_XLarge
600		1024		1		Android_Large
							
640		960		2		Android_Normal
//540		960		2		Android_Normal
//480		854		1.5		Android_Normal
//600		800		1.5		Android_Normal
480		800			1.5		Android_Normal
//400		800			1.5		Android_Normal
320		480			1		Android_Normal

240		320			1		Android_Small
		
// Kindle Fire (3rd generation Covered by other x-large androids)

//2560	1600		2		KindleFireHDX_3rd_8_9
//1920	1200		2		KindleFireHDX_3rd_7
//1280	800			1.5		KindleFireHD_3rd_7

//1920	1200		1.5		KindleFireHD_2nd_8_9
//1280	800			1.5		KindleFireHD_2nd_7
//1024	600			1		KindleFire_2nd

//1024	600			1		KindleFire_1st
";
}

