using UnityEngine;
using System.Collections;
using System;

namespace yourvrexperience.Utils
{
	[RequireComponent(typeof(MeshFilter))]
	public class CameraFader : MonoBehaviour 
	{
		public const string EventCameraFaderFadeCompleted = "EventCameraFaderFadeCompleted";

		private static CameraFader _instance;
		public static CameraFader Instance
		{
			get
			{
				if (!_instance)
				{
					_instance = GameObject.FindObjectOfType<CameraFader>();
				}
				return _instance;
			}
		}

		[SerializeField] private float inAlpha = 1.0f;
		[SerializeField] private float outAlpha = 0.0f;

		private bool _fading;
		private float _fadeTimer;

		private Color _fromColor;
		private Color _toColor;
		private Material _material;

		private bool _isFadeIn;

		private void Awake()
		{
			_fading = false;
			_fadeTimer = 0;

			_material = gameObject.GetComponent<Renderer>().material;
			_fromColor = _material.color;
			_toColor = _material.color;

			CameraFader.ReverseNormals(gameObject);
		}

		void Start()
		{
			SystemEventController.Instance.Event += OnSystemEvent;
		}

		void OnDestroy()
		{
			SystemEventController.Instance.Event -= OnSystemEvent;
		}

		private void OnSystemEvent(string _nameEvent, object[] _parameters)
		{
			if (_nameEvent.Equals(SystemEventController.EventSystemEventControllerReleaseAllResources))
			{
				_instance = null;
			}
		}

		void Update()
		{
			if (_fading == false)
				return;

			_fadeTimer += Time.deltaTime;
			_material.color = Color.Lerp(_fromColor, _toColor, _fadeTimer);
			if(_material.color == _toColor)
			{
				_fading = false;
				_fadeTimer = 0;
				SystemEventController.Instance.DispatchSystemEvent(EventCameraFaderFadeCompleted, _isFadeIn);
			}
		}

		public void FadeOut()
		{
			_isFadeIn = false;
			// Fade the overlay to `out_alpha`.
			if (this != null)
            {
				if (_material != null)
                {
					_material.color = new Color(0, 0, 0, 1);
					_fromColor.a = inAlpha;
					_toColor.a = outAlpha;
					if (_toColor != _material.color)
					{
						_fading = true;
					}
				}
			}
		}

		public void FadeIn()
		{
			_isFadeIn = true;
			if (this != null)
			{
				if (_material != null)
				{
					_material.color = new Color(0, 0, 0, 0);
					_fromColor.a = outAlpha;
					_toColor.a = inAlpha;
					if (_toColor != _material.color)
					{
						_fading = true;
					}
				}
			}
		}

		public static void ReverseNormals(GameObject gameObject)
		{
			// Renders interior of the overlay instead of exterior.
			// Included for ease-of-use. 
			// Public so you can use it, too.
			MeshFilter filter = gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
			if(filter != null)
			{
				Mesh mesh = filter.mesh;
				Vector3[] normals = mesh.normals;
				for(int i = 0; i < normals.Length; i++)
					normals[i] = -normals[i];
				mesh.normals = normals;

				for(int m = 0; m < mesh.subMeshCount; m++)
				{
					int[] triangles = mesh.GetTriangles(m);
					for(int i = 0; i < triangles.Length; i += 3)
					{
						int temp = triangles[i + 0];
						triangles[i + 0] = triangles[i + 1];
						triangles[i + 1] = temp;
					}
					mesh.SetTriangles(triangles, m);
				}
			}
		}
	}
}