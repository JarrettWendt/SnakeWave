using System.Collections;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
	// Singleton.
	public static MapRenderer Instance { get; private set; }

	public float angle = 45f, rotationMultiplier = 0.25f;
	private float originalRotationMultiplier;
	[SerializeField]
	private Material material;

	private Trig lineThickness = new Trig
	(
		verticalOffset: 0.9f,
		amplitude: 0.03f,
		phaseShift: 1f,
		angularFrequency: 1f,
		func: Mathf.Sin,
		theta: () => Time.realtimeSinceStartup
	);

	private Trig speed = new Trig
	(
		verticalOffset: 0f,
		amplitude: 0.5f,
		phaseShift: 1f,
		angularFrequency: 1f,
		func: Mathf.Sin,
		theta: () => Time.realtimeSinceStartup
	);

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
	}

	private void Start()
	{
		material = (material == default) ? GetComponent<Material>() : material;
		originalRotationMultiplier = rotationMultiplier;
	}

	// For Debugging only.
#if UNITY_EDITOR
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Wave();
		}
		//Debug.Log(material.GetFloat("_Speed") + ", " + speed.angularFrequency);
	}
#endif

	private void FixedUpdate()
	{
		material.SetFloat("_Line_Thickness", lineThickness.Value());
		material.SetFloat("_Rotation", angle += Time.deltaTime * rotationMultiplier);
		material.SetFloat("_Speed", speed.Value());
	}
	#endregion

	public void ResetRotationMultiplier()
	{
		rotationMultiplier = originalRotationMultiplier;
	}

	public void Pulse(float timeout = 0.5f, float multiplier = 5f)
	{
		lineThickness.angularFrequency *= multiplier;

		StartCoroutine(resetLineThickness());
		IEnumerator resetLineThickness()
		{
			yield return new WaitForSecondsRealtime(timeout);
			lineThickness.angularFrequency /= multiplier;
		}
	}

	public void Spin(float timeout = 1f, float multiplier = 4f)
	{
		rotationMultiplier = multiplier;

		StartCoroutine(resetRotation());
		IEnumerator resetRotation()
		{
			yield return new WaitForSecondsRealtime(timeout);
			rotationMultiplier = 0.25f;
		}
	}

	// Increasing the number of steps can make the animation smoother, but timeout should be increased accordingly.
	public void Wave(float timeout = 5f, float multiplier = 0.5f)
	{
		float angularFrequency = Util._2PI / timeout;
		float verticalOffset = speed.originalAngularFrequency + multiplier;

		// Single period from speed.originalAngularFrequency to (speed.originalAngularFrequency + 2 * multiplier).
		StartCoroutine(arcSpeed());
		IEnumerator arcSpeed()
		{
			float elapsed = 0f;
			while (elapsed < timeout)
			{
				elapsed += Time.unscaledDeltaTime;
				speed.angularFrequency = multiplier * Mathf.Sin(angularFrequency * elapsed - Util.PI_2) + verticalOffset;
				yield return null;
			}
			speed.angularFrequency = speed.originalAngularFrequency;
		}
	}
}
