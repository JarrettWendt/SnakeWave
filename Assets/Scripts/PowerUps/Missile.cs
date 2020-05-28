using UnityEngine;

public class Missile : PowerUp
{
	[SerializeField]
	private float safeDistance = 1.5f, speed = 6f;

	public override void Aquire(PlayerController playerController) { }

	public override void Activate(PlayerController playerController)
	{
		playerController.ui?.ClearSkillUIFor(this);
		AudioManager.Instance?.Play("MissileShotSFX");

		MissileController projectile = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Missile")).GetComponent<MissileController>();
		projectile.transform.position = playerController.transform.position + playerController.direction.ToVector3() * safeDistance;
		projectile.Rigidbody2D.velocity = (playerController.moveSpeed + speed) * playerController.direction;
		projectile.color = playerController.color;
	}
}
