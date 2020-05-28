using System.Collections;
using UnityEngine;

public class Laser : PowerUp
{
	[SerializeField]
	private float safeDistance = 1.5f, timeout = 2f, mapRotationMultiplier = 6f;

	public override void Aquire(PlayerController playerController) { }

	public override void Activate(PlayerController playerController)
	{
		AudioManager.Instance?.Play("Laser");
		playerController.ui.ClearSkillUIFor(this);

		ParticleSystem particleSystem = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Particles/LaserParticle")).GetComponent<ParticleSystem>();
		particleSystem.name = playerController.name + "'s Laser";
		particleSystem.gameObject.transform.SetParent(playerController.transform);
		particleSystem.gameObject.transform.localPosition = new Vector2(0f, safeDistance);
		particleSystem.gameObject.transform.localEulerAngles = new Vector2(-90f, safeDistance);
		ParticleSystem.MainModule main = particleSystem.main;
		main.startColor = playerController.color;
		playerController.canTurn = false;
		ParticleSystem.EmissionModule emission = particleSystem.emission;

		MapRenderer.Instance.rotationMultiplier = mapRotationMultiplier;

		GameManager.Instance.StartCoroutine(coroutine());
		IEnumerator coroutine()
		{
			yield return new WaitForSecondsRealtime(timeout);
			emission.enabled = false;
			// Don't destroy the particleSystem immediately, let the emissions die out. Have faith in Unity's garbage collections.
			//Object.Destroy(particleSystem);
			// Detatch it from the parent so that when they turn the whole particleSystem won't move with them.
			particleSystem.gameObject.transform.SetParent(null, true);
			playerController.canTurn = true;
			MapRenderer.Instance.ResetRotationMultiplier();
		};
	}
}
