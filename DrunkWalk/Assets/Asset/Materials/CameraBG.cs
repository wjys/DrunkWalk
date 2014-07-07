using UnityEngine;
using System.Collections;

public class CameraBG : MonoBehaviour {
    public Color color1;
    public Color color2;
    public float duration = 15.0F;
    void Update() {
        float t = Mathf.PingPong(Time.time/5, duration) / duration;
        camera.backgroundColor = Color.Lerp(color1, color2, t);
    }
    void Example() {
        camera.clearFlags = CameraClearFlags.SolidColor;
    }
}