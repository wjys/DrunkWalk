using UnityEngine;
using System.Collections;

public class LightColor : MonoBehaviour {
    public float duration = 0.1F;
    public Color color0 = Color.red;
    public Color color1 = Color.blue;
    void Update() {
        float t = Mathf.PingPong(Time.time, duration) / duration;
        light.color = Color.Lerp(color0, color1, t);
    }
}