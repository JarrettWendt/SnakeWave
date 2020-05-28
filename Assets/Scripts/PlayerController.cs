using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public bool canTurn = true;
	public float maxTurnRadiansDelta = 0.1f, maxTurnMagnitudeDelta = 0.1f, growSpeed = 0.1f, startMoveSpeed = 4f, moveSpeedDelta = 0.1f, maxMoveSpeed = 10f, respawnDelay = 5f;
	public string horizontalAxis, verticalAxis, actionButton;
	public Vector2 spawnPoint;
	public Color color;
	public HeadTrigger headTrigger;
	public HeadNonTrigger headNonTrigger;
	public PowerUp powerUp;
	public GameObject deathParticlePrefab;
	public PlayerUI ui;
	public SpriteRenderer spriteRenderer;

	public bool Alive { get; private set; } = true;
	public int Deaths { get; private set; } = 0;
	public string ColoredName { get { return gameObject.name.ToStringColored(color).Bold(); } }

	[HideInInspector]
	public float moveSpeed;

	public Vector2 direction = Vector2.zero;
	private Vector2 desiredDirection = Vector2.zero;

	public ParticleSystem particleTrail;
	private ParticleSystem.MainModule main;
	private ParticleSystem.MinMaxCurve startLifetime;
	private ParticleSystem.EmissionModule emissions;

	private readonly List<PolygonCollider2D> colliders = new List<PolygonCollider2D>();
	private bool CollisionsEnabled
	{
		get { return colliders.Exists(collider => collider.enabled); }
		set { colliders.ForEach(collider => collider.enabled = value); }
	}

	private bool Action { get { return Input.GetAxis(actionButton).ToBool(); } }

	#region MonoBehaviour
	private void Start()
	{
		// HACK
		if (name.Contains("Left"))
		{
			verticalAxis = "Vertical1";
			horizontalAxis = "Horizontal1";
			actionButton = "Fire1";
			color = Preferences.leftColor;
		}
		else
		{
			verticalAxis = "Vertical2";
			horizontalAxis = "Horizontal2";
			actionButton = "Fire2";
			color = Preferences.rightColor;
		}

		colliders.AddRange(GetComponentsInChildren<PolygonCollider2D>());
		spriteRenderer = spriteRenderer == default ? GetComponentInChildren<SpriteRenderer>() : spriteRenderer;

		moveSpeed = startMoveSpeed;

		InitTailModules();
		emissions.enabled = false;
		spawnPoint = transform.position;
		main.startColor = color;
		if (ui != default)
		{
			ui.Color = color;
		}
	}

	// For user input to make it snappy.
	private void Update()
	{
		// Don't bother doing anything if they're dead.
		if (!Alive)
		{
			return;
		}
		if (Action && powerUp != default)
		{
			powerUp.Activate(this);
			powerUp = default;
		}
		desiredDirection = GetInput();
		if (desiredDirection == default)
		{
			desiredDirection = direction;
		}
		if (direction == default)
		{
			direction = desiredDirection;
		}
	}

	// For things that use deltaTime.
	private void FixedUpdate()
	{
		if (Alive && direction != default)
		{
			Turn();
			Move();
			Grow();
		}
	}
	#endregion

	#region MovementAndGrowth
	private Vector2 GetInput()
	{
		return new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis));
	}

	private void Turn()
	{
		if (canTurn)
		{
			direction = Vector3.RotateTowards(direction, desiredDirection, maxTurnRadiansDelta, maxTurnMagnitudeDelta).normalized;
			// Update the direction the sprite is facing.
			transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, direction));

			if (!emissions.enabled)
			{
				StartCoroutine(renableCollisions());
				IEnumerator renableCollisions()
				{
					yield return new WaitForSecondsRealtime(0.1f);
					emissions.enabled = true;
				}
			}
		}
	}

	private void Move()
	{
		transform.Translate(Vector2.up * Mathf.Min(moveSpeed, maxMoveSpeed) * Time.deltaTime);
		moveSpeed += moveSpeedDelta * Time.deltaTime;
	}

	private void Grow()
	{
		startLifetime.constant += growSpeed * Time.deltaTime;
		main.startLifetime = startLifetime;
	}
	#endregion

	public void InitTailModules()
	{
		main = particleTrail.main;
		emissions = particleTrail.emission;
		startLifetime = main.startLifetime;
	}

	public void GetNewTrail()
	{
		GameObject newTrail = Instantiate(particleTrail.gameObject);
		newTrail.transform.SetParent(transform);
		newTrail.transform.localPosition = new Vector2(0, -0.5f);
		particleTrail = newTrail.GetComponent<ParticleSystem>();

		// I have no idea why I need a local version of main here. I'd think I could just change the startLifetime after InitTailModules().
		ParticleSystem.MainModule main = particleTrail.main;
		main.startLifetime = 0f;

		InitTailModules();
	}

	public void Kill()
	{
		// Multiple threads could be killing this player at once, but this player should only be able to die once per life.
		if (Alive && System.Threading.Monitor.TryEnter(this))
		{
			try
			{
				AudioManager.Instance?.Play("CrashSFX");

				Alive = false;

				spriteRenderer.enabled = false;
				CollisionsEnabled = false;

				GameObject go = Instantiate(deathParticlePrefab);
				go.transform.position = transform.position;
				ParticleSystem.MainModule main = go.GetComponent<ParticleSystem>().main;
				main.startColor = color;

				powerUp = default;
				ui.UnconditionallyClearSkillUI();

				Deaths++;
				switch (GameManager.GameMode)
				{
					case GameMode.Endless:
						ui.deathCountText.text = Deaths.ToString();
						StartCoroutine(Respawn());
						break;
					case GameMode.Survival:
						ui.deathCountText.text = (Preferences.lives - Deaths).ToString();
						if (Deaths < Preferences.lives)
						{
							StartCoroutine(Respawn());
						}
						break;
					default:
						break;
				}
			}
			finally
			{
				System.Threading.Monitor.Exit(this);
			}
		}
	}

	private IEnumerator Respawn()
	{
		particleTrail.transform.SetParent(null, true);
		GetNewTrail();
		emissions.enabled = false;
		direction = desiredDirection = Vector2.zero;

		yield return new WaitForSecondsRealtime(respawnDelay);

		moveSpeed = startMoveSpeed;
		new Phase().Aquire(this);
		main.startLifetime = 0f;

		spriteRenderer.enabled = true;
		CollisionsEnabled = true;

		transform.position = spawnPoint;
		Alive = true;

		headTrigger.gameObject.transform.localEulerAngles = Vector3.zero;
		headTrigger.gameObject.transform.localPosition = Vector3.zero;
		headNonTrigger.gameObject.transform.localEulerAngles = Vector3.zero;
		headNonTrigger.gameObject.transform.localPosition = Vector3.zero;
	}
}
