using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private float zPos = -10f;
	[SerializeField]
	private float minZoom = 4f, maxZoom = 10f, minFOV = 60f, maxFOV = 90f, maxPlayerDistance = 60f, minPlayerDistance = 0f;

	[SerializeField]
	private new Camera camera;
	public Camera Camera { get { return camera ?? (camera = GetComponent<Camera>()); } }

	#region MonoBehaviour
	private void Start()
	{
		zPos = transform.position.z;
		camera = GetComponent<Camera>();
		//maxPlayerDistance = (maxPlayerDistance != default) ? maxPlayerDistance : GameManager.Instance.mapTopRightCorner.y - GameManager.Instance.mapBottomLeftCorner.y;
	}

	private void FixedUpdate()
	{
		List<Vector3> positions = GameManager.Players.ConvertAll(player => player.transform.position);
		if (Camera.orthographic)
		{
			Camera.orthographicSize = Mathf.Clamp(Util.Max(positions, Vector3.Distance), minZoom, maxZoom);
		}
		else
		{
			Camera.fieldOfView = Util.Max(positions, Vector3.Distance).ReMap(minPlayerDistance, maxPlayerDistance, minFOV, maxFOV);
		}
		Camera.transform.position = Util.Centroid(positions).SetZ(zPos);
	}
	#endregion
}
