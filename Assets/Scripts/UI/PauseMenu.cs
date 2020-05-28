using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : CanvasOverlay
{
	#region MonoBehaviour
	// Deactivated at start.
	private void Start()
	{
		canvas.enabled = false;
	}

	// Escape toggles pause menu.
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (canvas.enabled)
			{
				Resume();
			}
			else
			{
				Pause();
			}
		}
	}
	#endregion

	public void Pause()
	{
		AudioManager.Instance?.Play("MenuButtonSFX");
		AudioManager.Instance?.Stop("BackgroundMusic");

		StartCoroutine(FadeIn(fadeTime));
		Time.timeScale = 0f;
	}

	public void Resume()
	{
		AudioManager.Instance?.Play("MenuButtonSFX");
		AudioManager.Instance?.Play("BackgroundMusic");

		StartCoroutine(FadeOut(fadeTime));
		Time.timeScale = 1f;
	}

	public void Reload()
	{
		AudioManager.Instance?.Play("MenuButtonSFX");
		AudioManager.Instance?.Stop("BackgroundMusic");
		AudioManager.Instance?.Play("AnnouncerCountdown");

		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		Time.timeScale = 1f;
	}

	public void Quit()
	{
		AudioManager.Instance?.Play("MenuButtonSFX");

		Application.Quit();
	}
}
