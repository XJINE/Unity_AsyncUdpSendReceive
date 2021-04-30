using UnityEngine;

public class Fps : MonoBehaviour
{
    GUIStyle style;
    float    fps;

    private void Update()
    {
        fps  = 1f / Time.deltaTime;
    }

    void OnGUI()
    {
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 30,
                alignment = TextAnchor.MiddleCenter
            };
        }

        var rect = new Rect(0, 0, Screen.width, Screen.height); 
        GUI.Label(rect, "FPS : " + fps, style);
    }
}