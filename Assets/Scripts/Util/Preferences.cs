using UnityEngine;

public static class Preferences
{
	// TODO: This can be fleshed out with more things like controls, volume, etc.
	public static Color rightColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
	// Opposite hue because generating a second random one successively would be likely to make a similar hue.
	public static Color leftColor = leftColor.SufficientlyDifferentColorHSV(0.1f, 0.1f, 0.1f);

	public static GameMode gameMode = GameMode.Endless;
	public static int lives = 1;
}
