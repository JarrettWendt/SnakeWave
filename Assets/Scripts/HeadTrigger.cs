using System.Collections.Generic;
using UnityEngine;

/* 
 * This script's entire purpose is to deal with all possible collisions a player might be involved in.
 * The possible collision messages sent between Unity's different colliders make up a very complex Venn Diagram.
 * This game runs off of Unity's ParticleSystem, which can ONLY collider with NON-TRIGGER Static, Kinematic, and Dynamic colliders.
 * Static and Kinematic colliders cannot collide with each other or themselves. If they're triggers though then they'll send trigger messages.
 * 
 * Dynamic colliders can collide with anything, but they use physics. This game does not need any physics.
 * We could attempt to get away with using Dynamic colliders anyway but setting friction and mass to high values such that they're essentially immovable, but that's hacky.
 * 
 * So this is what we're doing:
 * - Terrain is Static Non-Trigger
 * - Players have 2 colliders:
 *		- Kinematic Non-Trigger	for colliding with particles
 *		- Kinematic Trigger for colliding with everything else
 *	
 *	Having two separate colliders on the player is a better solution than using Dynamic colliders, but still less than ideal.
*/

public class HeadTrigger : MonoBehaviour
{
	public PlayerController playerController;
	public readonly HashSet<string> ignoredTags = new HashSet<string>();

	private PolygonCollider2D polygonCollider;

	#region MonoBehaviour
	private void Start()
	{
		polygonCollider = GetComponent<PolygonCollider2D>();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!playerController.Alive || !enabled || ignoredTags.Contains(collision.otherCollider.tag))
		{
			return;
		}
		playerController.Kill();
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		// Checking if they have the same parent to avoid the situation where the Player's two colliders collide.
		if (!playerController.Alive || !enabled || ignoredTags.Contains(collider.tag) || collider.gameObject.HasSameParent(gameObject))
		{
			return;
		}
		if (collider.tag == "PowerUp" && collider.isTrigger)
		{
			PowerUpManager.AwardPowerUp(playerController);
			Destroy(collider.gameObject);
		}
		else
		{
			playerController.Kill();
		}
	}

	private void OnParticleCollision(GameObject other)
	{
		if (!playerController.Alive || !enabled || ignoredTags.ContainsAny("Particles", other.tag) || other.name.Contains(playerController.name))
		{
			return;
		}
		playerController.Kill();
	}
	#endregion
}
