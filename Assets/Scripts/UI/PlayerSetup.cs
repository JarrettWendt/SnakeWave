using TMPro;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
	public ColorPicker leftColorPicker, rightColorPicker;
	public TMP_Dropdown gameModeSelect;
	public TMP_InputField lives;

	#region MonoBehaviour
	private void Awake()
	{
		leftColorPicker.onValueChange = SetLeftColor;
		rightColorPicker.onValueChange = SetRightColor;
		gameModeSelect.onValueChanged.AddListener(delegate { SetGameMode(); });
		lives.onValueChanged.AddListener(delegate { SetLives(); });
	}

	private void Start()
	{
		// The colors will be populated based on RNG.
		leftColorPicker.Color = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.7f, 1f);
		rightColorPicker.Color = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.7f, 1f);
		// Populate these preferences based on the initial values of the scene.
		SetGameMode();
		SetLives();
	}
	#endregion

	private void SetLeftColor(Color color)
	{
		Preferences.leftColor = color;
	}

	private void SetRightColor(Color color)
	{
		Preferences.rightColor = color;
	}

	private void SetGameMode()
	{
		if (gameModeSelect.captionText.text == GameMode.Endless.ToString())
		{
			Preferences.gameMode = GameMode.Endless;
			lives.gameObject.SetActive(false);
		}
		else
		{
			Preferences.gameMode = GameMode.Survival;
			lives.gameObject.SetActive(true);
		}
	}

	private void SetLives()
	{
		if (!int.TryParse(lives.text, out Preferences.lives) || Preferences.lives <= 0)
		{
			Preferences.lives = 1;
			lives.text = "1";
		}
	}
}
