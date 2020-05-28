using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerUpManager : MonoBehaviour
{
	// Seconds.
	public float powerUpSpawnDelay = 10f;
	private float timeSinceLastSpawn;

	// Get list of all PowerUps once - the first time it's asked for.
	private static List<Type> powerUps;
	private static List<Type> PowerUps { get { return powerUps ?? (powerUps = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(PowerUp))).ToList()); } }

	[SerializeField]
	private GameObject powerUpPrefab;

	#region MonoBehaviour
	private void Start()
	{
		powerUpPrefab = Resources.Load<GameObject>("Prefabs/PowerUp");
	}

	private void Update()
	{
		if (timeSinceLastSpawn > powerUpSpawnDelay)
		{
			SpawnPowerUp();
			timeSinceLastSpawn = 0f;
		}
		else
		{
			timeSinceLastSpawn += Time.deltaTime;
		}
	}
	#endregion

	private void SpawnPowerUp()
	{
		// Ensure we get a spawnpoint that doesn't collide with anything else.
		Vector2 position;
		do
		{
			position = Util.RandomVector(GameManager.Instance.mapBottomLeftCorner, GameManager.Instance.mapTopRightCorner);
		}
		while (Physics2D.Raycast(position, Vector2.zero));

		GameObject powerUp = Instantiate(powerUpPrefab);
		powerUp.transform.position = position;
		powerUp.SetActive(true);
	}

	public static void AwardPowerUp(PlayerController playerController)
	{
		AudioManager.Instance?.Play("EquipSFX");
		MapRenderer.Instance.Pulse();

		playerController.powerUp = Activator.CreateInstance(PowerUps[Random.Range(0, powerUps.Count)]) as PowerUp;
		// DEBUG: For testing new PowerUps.
		//playerController.powerUp = new Phase();
		playerController.powerUp.Aquire(playerController);
		playerController.ui?.SetSkillUI(playerController.powerUp);

		Debug.Log(playerController.ColoredName + " got " + playerController.powerUp);
	}
}
