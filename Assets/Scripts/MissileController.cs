using System.Collections;
using UnityEngine;

public class MissileController : MonoBehaviour
{
	public Color color;

	[SerializeField]
	private GameObject sprite, explosionParticlePrefab;
	[SerializeField]
	private ParticleSystem particleAura;
	[SerializeField]
	public Rigidbody2D rb2d;
	public Rigidbody2D Rigidbody2D { get { return rb2d ?? (rb2d = GetComponent<Rigidbody2D>()); } }

	#region MonoBehaviour
	private void Start()
	{
		sprite = sprite ?? transform.Find("Sprite").gameObject;
		explosionParticlePrefab = explosionParticlePrefab ?? Resources.Load<GameObject>("Prefabs/Particles/ProjectileExplosionParticle");
		rb2d = rb2d ?? GetComponent<Rigidbody2D>();
		particleAura = particleAura ?? GetComponentInChildren<ParticleSystem>();

		ParticleSystem.MainModule main = particleAura.main;
		main.startColor = color;
		MapRenderer.Instance.rotationMultiplier = 4f;
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		// PowerUps are the only thing these should just pass though.
		if (collider.tag != "PowerUp")
		{
			AudioManager.Instance?.Play("MissileExplosionSFX");
			// Hide the projectile and make it stop moving.
			sprite.SetActive(false);
			rb2d.velocity = new Vector2(0, 0);
			// Make the explosion and put it in the right place.
			GameObject explosion = Instantiate(explosionParticlePrefab);
			explosion.transform.position = transform.position;
			// Color the explosion.
			ParticleSystem.MainModule main = explosion.GetComponent<ParticleSystem>().main;
			main.startColor = color;

			// Mess with the map shader.
			MapRenderer.Instance.rotationMultiplier = 8f;
			StartCoroutine(coroutine());
			IEnumerator coroutine()
			{
				yield return new WaitForSecondsRealtime(1f);
				MapRenderer.Instance.ResetRotationMultiplier();
			}
		}
	}
	#endregion
}
