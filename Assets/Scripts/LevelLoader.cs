using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
	public void LoadScene(string name)
	{
		AudioManager.Instance?.Stop("GameOverMusic");
		AudioManager.Instance?.Play("MenuButtonSFX");

		SceneManager.LoadScene(name);
		Time.timeScale = 1f;
	}

	public void LoadRandomLevel()
	{
		AudioManager.Instance?.Play("MenuButtonSFX");
		AudioManager.Instance?.Stop("GameOverMusic");
		AudioManager.Instance?.Stop("StartMenuMusic");
		AudioManager.Instance?.Play("AnnouncerCountdown");

		SceneManager.LoadScene("LevelExp" + Random.Range(0, 8));
	}

	// Reloads current scene (update for build settings).
	public void ReloadScene()
	{
		AudioManager.Instance?.Stop("GameOverMusic");
		AudioManager.Instance?.Play("MenuButtonSFX");
		AudioManager.Instance?.Play("AnnouncerCountdown");

		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		Time.timeScale = 1f;
	}

	// Exits game.
	public void QuitGame()
	{
		AudioManager.Instance?.Stop("GameOverMusic");
		AudioManager.Instance?.Play("MenuButtonSFX");

		Application.Quit();
	}
}
