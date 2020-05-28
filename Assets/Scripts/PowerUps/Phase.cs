using System.Collections;
using UnityEngine;

public class Phase : PowerUp
{
	public override void Aquire(PlayerController playerController)
	{
		Color color = playerController.spriteRenderer.color;

		// Make this ONLY care about collisions with terrain and outer map walls.
		playerController.headTrigger.ignoredTags.Add("Particles");
		playerController.headNonTrigger.ignoredTags.Add("Particles");
		color.a = 0.5f;
		playerController.spriteRenderer.color = color;

		GameManager.Instance.StartCoroutine(coroutine());
		IEnumerator coroutine()
		{
			yield return new WaitForSecondsRealtime(5f);
			// Make this collide with everything again as normal.
			playerController.headTrigger.ignoredTags.Remove("Particles");
			playerController.headNonTrigger.ignoredTags.Remove("Particles");
			color.a = 1f;
			playerController.spriteRenderer.color = color;
			playerController.ui?.ClearSkillUIFor(this);
		}
	}

	public override void Activate(PlayerController playerController) { }
}
