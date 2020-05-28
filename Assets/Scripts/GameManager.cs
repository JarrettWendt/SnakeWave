using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode { Survival, Endless }

public class GameManager : MonoBehaviour
{
	// Singleton.
	public static GameManager Instance { get; private set; }

	// Used for deciding where to spawn powerups.
	public Vector2 mapBottomLeftCorner, mapTopRightCorner;

	public static bool GameOver { get; private set; }
	public static GameMode GameMode { get; private set; }

	[SerializeField]
	private List<PlayerController> players;
	public static List<PlayerController> Players { get { return Instance.players ?? (Instance.players = FindObjectsOfType<PlayerController>().ToList()); } }
	[SerializeField]
	private CanvasOverlay gameOverScreen, countdown;
	[SerializeField]
	private Canvas deathCanvas;

	#region MonoBehaviour
	private void Awake()
	{
		// Singleton.
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}

		GameOver = false;

		// It is very important for non-static members of singletons to be accessed through Instance in Awake().
		// This Awake() gets to run every time the scene is reloaded but that is just because this is the Awake() of a new GameObject, not the original Instance.
		Instance.players = FindObjectsOfType<PlayerController>().ToList();
		Instance.gameOverScreen = FindObjectsOfType<CanvasOverlay>().First(co => co.name == "GameOver");
		Instance.countdown = FindObjectsOfType<CanvasOverlay>().First(co => co.name == "Countdown");
		Instance.countdown = GameObject.Find("Countdown").GetComponent<CanvasOverlay>();
		Instance.deathCanvas = GameObject.Find("DeathCanvas").GetComponent<Canvas>();

		Time.timeScale = 0f;
		// Coroutines are attached to a GameObject so it's a bad idea to attach it to this.gameObject since it might get destroyed after this Awake().
		Instance.StartCoroutine(Instance.countdown.Countdown(delegate { Time.timeScale = 1f; }));

		GameMode = Preferences.gameMode;
		Instance.deathCanvas.GetComponentInChildren<TextMeshProUGUI>().text = GameMode == GameMode.Survival ? "Lives" : "Deaths";
	}

	private void FixedUpdate()
	{
		if (GameMode == GameMode.Survival && !GameOver && Util.OneOrNone(players, out PlayerController winner, pc => pc.Deaths < Preferences.lives))
		{
			EndGame(winner);
		}
	}
	#endregion

	public void EndGame(PlayerController winner)
	{
		AudioManager.Instance?.Stop("BackgroundMusic");
		AudioManager.Instance?.Play("AnnouncerGameOver0" + Random.Range(1, 9), 1f);
		AudioManager.Instance?.Play("GameOverMusic", 1f);

		GameOver = true;
		gameOverScreen.text.text = "Game Over\n" + (winner == default ? "Draw" : (winner.ColoredName + " Won"));
		Time.timeScale = 0.5f;
		StartCoroutine(gameOverScreen.FadeIn(0.5f));
	}
}
