using System.Collections;
using UnityEngine;

public class Boost : PowerUp
{
	public float multiplier = 1.7f;

	public override void Aquire(PlayerController playerController)
	{
		playerController.moveSpeed *= multiplier;
		GameManager.Instance.StartCoroutine(coroutine());
		IEnumerator coroutine()
		{
			yield return new WaitForSecondsRealtime(4f);
			playerController.moveSpeed /= multiplier;
			playerController.ui?.ClearSkillUIFor(this);
		}
	}

	public override void Activate(PlayerController playerController) { }
}
