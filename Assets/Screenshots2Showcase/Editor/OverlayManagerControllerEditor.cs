using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OverlayManagerController))]
public class OverlayManagerControllerEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var t = target as OverlayManagerController;

        if (GUILayout.Button("Previous"))
        {
            t.visibleChildIndex--;
            t.UpdateOverlays();
        }

        if (GUILayout.Button("Next"))
        {
            t.visibleChildIndex++;
            t.UpdateOverlays();
        }

        if (GUILayout.Button("Add Empty Overlay"))
        {
            var overlay = new GameObject();
            overlay.AddComponent<OverlayController>();
            overlay.name = "Overlay";
            overlay.transform.parent = t.transform;
        }

        if (t.transform.childCount > 0)
        {
            var current = t.transform.GetChild(t.visibleChildIndex).gameObject;

            if (GUILayout.Button("Duplicate Overlay"))
            {
                var overlay = (GameObject) Instantiate(current);
                overlay.transform.parent = t.transform;
                overlay.name = current.name;
            }
        }
    }
}
