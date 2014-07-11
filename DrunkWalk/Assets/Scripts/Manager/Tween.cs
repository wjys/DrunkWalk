using UnityEngine;

public class Tween {
	public delegate float Interpolation (float k);

	public static Interpolation Out (Interpolation func) {
		return delegate (float k) {
			return 1 - func(1 - k);
		};
	}

	public static Interpolation InOut (Interpolation func) {
		return delegate (float k) {
			if (k * 2 < 1) return func(k / 0.5f);
			else return 1 - func((k - 0.5f) * 2);
		};
	}

	public static float Linear (float k) { return k; }
	public static float Cubic (float k) { return k * k * k; }
	public static float Sinusoidal (float k) { return 1 - Mathf.Cos(k * Mathf.PI / 2); }
}