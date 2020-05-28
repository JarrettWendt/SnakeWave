using System.Collections;
using UnityEngine;

public class DropTail : PowerUp
{
	float decayDelay = 15f;

	public override void Aquire(PlayerController playerController)
	{
		// Detatch the old trail.
		playerController.particleTrail.SetExistingParticlesStartLifetime(float.PositiveInfinity);

		// When changing the parent, even though keepWorldSpace = true, this changes the position ever so slightly.
		// This can rarely lead to an unwanted collision, so we temporarily disable collisions during this move.
		ParticleSystem.CollisionModule collision = playerController.particleTrail.collision;
		collision.enabled = false;
		playerController.particleTrail.transform.SetParent(null, true);
		GameManager.Instance.StartCoroutine(renableCollision());
		IEnumerator renableCollision()
		{
			yield return new WaitForSecondsRealtime(0.1f);
			// These three lines are less stupid than they seem.
			// This is for re-enabling the old and the new tail.
			// I'm not sure why both need to be re-enabled. Theoretically only the old tail should need to be re-enabled. 
			// This seems to fix a bug where the new tail had no collsions.
			collision.enabled = true;
			collision = playerController.particleTrail.collision;
			collision.enabled = true;
		}

		ParticleSystem oldTrail = playerController.particleTrail;
		playerController.GetNewTrail();

		GameManager.Instance.StartCoroutine(clearSkillUI());
		IEnumerator clearSkillUI()
		{
			yield return new WaitForSecondsRealtime(1f);
			playerController.ui?.ClearSkillUIFor(this);
		}

		// Increase the player's tail growth for a short while to sort of catch them back up to their old length.
		playerController.growSpeed *= 2;
		GameManager.Instance.StartCoroutine(resetGrowSpeed());
		IEnumerator resetGrowSpeed()
		{
			yield return new WaitForSecondsRealtime(5f);
			playerController.growSpeed /= 2f;
		}

		// Delete the tails after some time if in Endless mode or else the map will be just too polluted.
		if (GameManager.GameMode == GameMode.Endless)
		{
			GameManager.Instance.StartCoroutine(decay());
			IEnumerator decay()
			{
				yield return new WaitForSecondsRealtime(decayDelay);
				oldTrail.SetExistingParticlesStartLifetime(1f);
			}
		}
	}

	public override void Activate(PlayerController playerController) { }
}
