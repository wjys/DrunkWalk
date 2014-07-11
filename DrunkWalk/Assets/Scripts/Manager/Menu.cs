using UnityEngine;

public class Menu : MonoBehaviour {
	private Tween.Interpolation flickInterpolation = Tween.InOut(Tween.Sinusoidal);

	public delegate void Command ();

	public struct Item {
		public string name;
		public Command command;
		public Item (string name, Command command) {
			this.name = name;
			this.command = command;
		}
	}

	public void GUIMenu (int idx, float pos, float interval, Item[] items, float time) {
		int i = 0;
		foreach (var item in items) {
			GUI.Label(new Rect(0, pos + i * interval, Screen.width, Screen.height), item.name);
			++i;
		}
	}
}