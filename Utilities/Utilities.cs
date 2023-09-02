using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace yourvrexperience.Utils
{
	public static class Utilities
	{
		public const string SeparatorBasicTypes = ";";

		public static Vector3 GetDirection(Vector3 target, Vector3 origin)
		{
			return (target - origin).normalized;
		}

		public static float IsInsideCone(GameObject source, float angle, GameObject objective, float rangeDetection, float angleDetection)
		{
			float distance = Vector3.Distance(new Vector3(source.transform.position.x, 0, source.transform.position.z),
											 new Vector3(objective.transform.position.x, 0, objective.transform.position.z));
			if (distance < rangeDetection)
			{
				float yaw = angle * Mathf.Deg2Rad;
				Vector2 pos = new Vector2(source.transform.position.x, source.transform.position.z);

				Vector2 v1 = new Vector2((float)Mathf.Cos(yaw), (float)Mathf.Sin(yaw));
				Vector2 v2 = new Vector2(objective.transform.position.x - pos.x, objective.transform.position.z - pos.y);

				// Angle detection
				float moduloV2 = v2.magnitude;
				if (moduloV2 == 0)
				{
					v2.x = 0.0f;
					v2.y = 0.0f;
				}
				else
				{
					v2.x = v2.x / moduloV2;
					v2.y = v2.y / moduloV2;
				}
				float angleCreated = (v1.x * v2.x) + (v1.y * v2.y);
				float angleResult = Mathf.Cos(angleDetection * Mathf.Deg2Rad);

				if (angleCreated > angleResult)
				{
					return (distance);
				}
				else
				{
					return (-1);
				}
			}
			else
			{
				return (-1);
			}
		}

		public static void ActivatePhysics(GameObject target, bool activated)
		{
			if (target.GetComponent<Rigidbody>() != null)
			{
				target.GetComponent<Rigidbody>().isKinematic = !activated;
				target.GetComponent<Rigidbody>().useGravity = activated;
			}
			if (target.GetComponent<Collider>() != null)
			{
				target.GetComponent<Collider>().isTrigger = !activated;
			}
		}


		public static GameObject GetCollisionMouseWithLayers(params string[] masksToConsider)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			int layerMask = 0;
			if (masksToConsider != null)
			{
				for (int i = 0; i < masksToConsider.Length; i++)
				{
					layerMask |= (1 << LayerMask.NameToLayer(masksToConsider[i]));
				}
			}
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
			{
				return hit.collider.gameObject;
			}
			else
			{
				return null;
			}
		}

		public static float DistanceXZ(Vector3 one, Vector3 two)
		{
			float x = (one.x - two.x);
			float z = (one.z - two.z);
			return Mathf.Sqrt((x * x) + (z * z));
		}

		public static List<Vector3> GetBoundaryPoints(Vector3 origin, Vector3 target)
		{
			List<Vector3> pointsPlane = new List<Vector3>();

			float topX = ((origin.x < target.x) ? target.x : origin.x);
			float topY = ((origin.z < target.z) ? target.z : origin.z);

			float bottomX = ((origin.x > target.x) ? target.x : origin.x);
			float bottomY = ((origin.z > target.z) ? target.z : origin.z);

			pointsPlane.Add(new Vector3(bottomX, 0, bottomY));
			pointsPlane.Add(new Vector3(bottomX, 0, topY));
			pointsPlane.Add(new Vector3(topX, 0, topY));
			pointsPlane.Add(new Vector3(topX, 0, bottomY));

			return pointsPlane;
		}

		public static void DebugLogColor(string message, Color color)
		{
			if (color == Color.red)
			{
				Debug.Log("<color=red>" + message + "</color>");
			}
			if (color == Color.blue)
			{
				Debug.Log("<color=blue>" + message + "</color>");
			}
			if (color == Color.green)
			{
				Debug.Log("<color=green>" + message + "</color>");
			}
			if (color == Color.yellow)
			{
				Debug.Log("<color=yellow>" + message + "</color>");
			}
		}

		public static string RandomCodeGeneration(int length)
		{
			string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var stringChars = new char[length];
			var random = new System.Random();

			for (int i = 0; i < length; i++)
			{
				stringChars[i] = chars[random.Next(chars.Length)];
			}

			string finalString = new String(stringChars);
			return finalString;
		}

		public static string SerializeVector3(Vector3 data)
		{
			return data.x + SeparatorBasicTypes + data.y + SeparatorBasicTypes + data.z;
		}

		public static Vector3 DeserializeVector3(string data)
		{
			string[] buffer = data.Split(';');
			return new Vector3(float.Parse(buffer[0]), float.Parse(buffer[1]), float.Parse(buffer[2]));
		}

		public static string SerializeVector2(Vector2 data)
		{
			return data.x + SeparatorBasicTypes + data.y;
		}

		public static Vector2 DeserializeVector2(string data)
		{
			string[] buffer = data.Split(new String[]{SeparatorBasicTypes}, data.Length, StringSplitOptions.None);
			return new Vector2(float.Parse(buffer[0]), float.Parse(buffer[1]));
		}

		public static string PackColor(Color color)
		{
			return color.r + SeparatorBasicTypes + color.g + SeparatorBasicTypes + color.b + SeparatorBasicTypes + color.a;
		}

		public static string SerializeQuaternion(Quaternion data)
		{
			return data.x + SeparatorBasicTypes + data.y + SeparatorBasicTypes + data.z + SeparatorBasicTypes + data.w;
		}

		public static Quaternion DeserializeQuaternion(string data)
		{
			string[] buffer = data.Split(new String[]{SeparatorBasicTypes}, data.Length, StringSplitOptions.None);
			return new Quaternion(float.Parse(buffer[0]), float.Parse(buffer[1]), float.Parse(buffer[2]), float.Parse(buffer[3]));
		}

		public static Color UnpackColor(string color)
		{
			string[] colorData = color.Split(new String[]{SeparatorBasicTypes}, color.Length, StringSplitOptions.None);
			return new Color(float.Parse(colorData[0]),float.Parse(colorData[1]),float.Parse(colorData[2]),float.Parse(colorData[3]));
		}

		public static string SerializeIntList(List<int> data)
		{
			string output = "";
			for (int i = 0; i < data.Count; i++)
			{
				output += data[i].ToString();
				if (i < data.Count - 1) output += SeparatorBasicTypes;
			}
			return output;
		}
		public static void DeserializeIntList(string data, ref List<int> list)
		{
			string[] listData = data.Split(new String[]{SeparatorBasicTypes}, data.Length, StringSplitOptions.None);
			list = new List<int>();
			for (int i = 0; i < listData.Length; i++)
			{
				list.Add(int.Parse(listData[i]));
			}
		}

		private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long GetTimestamp()
		{
			return (long)((DateTime.UtcNow - Jan1St1970).TotalMilliseconds);
		}

		public static Vector2 RotatePoint(Vector2 pointToRotate, Vector2 centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Vector2
            {
                x = (float)
                    (cosTheta * (pointToRotate.x - centerPoint.x) -
                    sinTheta * (pointToRotate.y - centerPoint.y) + centerPoint.x),
                y = (float)
                    (sinTheta * (pointToRotate.x - centerPoint.x) +
                    cosTheta * (pointToRotate.y - centerPoint.y) + centerPoint.y)
            };
        }
		
		public static float GetAngleFromNormal(Vector2 normal)
		{	
   			return (Mathf.Atan2(normal.x, normal.y) / Mathf.PI) * 180;
		}

		public static void ApplyColor(Transform target, Color color)
		{
			if (target.GetComponent<MeshRenderer>() != null)
			{
				target.GetComponent<MeshRenderer>().material.color = color;
			}
			foreach (Transform item in target)
			{
				ApplyColor(item, color);
			}
		}

		public static void ApplyMaterialOnImages(Transform target, Material material)
		{
			if (target.GetComponent<Image>() != null)
			{
				target.GetComponent<Image>().material = material;
			}
			foreach (Transform item in target)
			{
				ApplyMaterialOnImages(item, material);
			}
		}

		public static void ApplyInteractivity(Transform target, bool interaction)
		{
			if (target.GetComponent<Selectable>() != null)
			{
				target.GetComponent<Selectable>().interactable = interaction;
			}
			foreach (Transform item in target)
			{
				ApplyInteractivity(item, interaction);
			}
		}

		public static void ApplyEnabledInteraction(Transform target, bool interaction)
		{
			if (target.GetComponent<Selectable>() != null)
			{
				target.GetComponent<Selectable>().enabled = interaction;
			}
			foreach (Transform item in target)
			{
				ApplyEnabledInteraction(item, interaction);
			}
		}

		public static void SetIsTrigger(Transform target, bool isTrigger)
		{
			if (target.GetComponent<Collider>() != null)
			{
				target.GetComponent<Collider>().isTrigger = isTrigger;
			}
			foreach (Transform item in target)
			{
				SetIsTrigger(item, isTrigger);
			}
		}

		public static void EnableRenderers(Transform target, bool enabled)
		{
			if (target.GetComponent<Renderer>() != null)
			{
				target.GetComponent<Renderer>().enabled = enabled;
			}
			foreach (Transform item in target)
			{
				EnableRenderers(item, enabled);
			}
		}

		public static void EnablerRenderers(Transform target, bool enabled)
		{
			if (target.GetComponent<EnablerRenderer>() != null)
			{
				target.GetComponent<EnablerRenderer>().Activate(enabled);
			}
			foreach (Transform item in target)
			{
				EnablerRenderers(item, enabled);
			}
		}
		public static void ApplyLayer(Transform target, int layer)
		{
			if (target != null)
			{
				target.gameObject.layer = layer;
			}
			foreach (Transform item in target)
			{
				ApplyLayer(item, layer);
			}
		}


		public static string GetFullPathNameGO(GameObject go)
		{
			if (go == null || go.transform == null)
			{
				return "";
			}
			if (go.transform.parent == null)
			{
				return go.name;
			}

			return GetFullPathNameGO(go.transform.parent.gameObject) + "," + go.name;
		}

		public static GameObject AddChild(Transform parent, GameObject prefab)
		{
			GameObject newObj = GameObject.Instantiate(prefab);
			newObj.transform.SetParent(parent, false);
			return newObj;
		}

        public static GameObject AttachChild(Transform parent, GameObject prefab)
        {
            prefab.transform.SetParent(parent, false);
            return prefab;
        }

		public static void FixObject(GameObject objectToFix)
        {
			if (objectToFix != null)
			{
				if (objectToFix.GetComponent<Collider>() != null)
				{
					objectToFix.GetComponent<Collider>().isTrigger = true;
				}
				if (objectToFix.GetComponent<Rigidbody>() != null)
				{
					objectToFix.GetComponent<Rigidbody>().isKinematic = true;
					objectToFix.GetComponent<Rigidbody>().useGravity = false;
				}
			}
        }

        public static bool FindGameObjectInChilds(GameObject go, GameObject target)
        {
            if (go == target)
            {
                return true;
            }
            bool output = false;
            foreach (Transform child in go.transform)
            {
                output = output || FindGameObjectInChilds(child.gameObject, target);
            }
            return output;
        }

		public static void Shuffle<T>(this IList<T> list, System.Random rnd)
		{
			for(var i=list.Count; i > 0; i--)
				list.Swap(0, rnd.Next(0, i));
		}

		public static void Swap<T>(this IList<T> list, int i, int j)
		{
			var temp = list[i];
			list[i] = list[j];
			list[j] = temp;
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

		public static string GetFormattedTimeMinutes(int time)
		{
			int minutes = (int)time / 60;
			int seconds = (int)time % 60;

			// SECONDS
			String secondsBuf;
			if (seconds < 10)
			{
				secondsBuf = "0" + seconds;
			}
			else
			{
				secondsBuf = "" + seconds;
			}

			// MINUTES
			String minutesBuf;
			if (minutes < 10)
			{
				minutesBuf = "0" + minutes;
			}
			else
			{
				minutesBuf = "" + minutes;
			}

			return (minutesBuf + ":" + secondsBuf);
		}

		public static bool IsVisibleFrom(Bounds _bounds, Camera _camera)
		{
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_camera);
			return GeometryUtility.TestPlanesAABB(planes, _bounds);
		}

		public static string RandomCodeIV(int _size)
		{
			string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789#!@+=-*";
			var stringChars = new char[_size];
			var random = new System.Random();

			for (int i = 0; i < _size; i++)
			{
				stringChars[i] = chars[random.Next(chars.Length)];
			}

			string finalString = new String(stringChars);
			return finalString;
		}

		public static Bounds GetMaxBounds(GameObject g) 
		{
			var renderers = g.GetComponentsInChildren<Renderer>();
			if (renderers.Length == 0) return new Bounds(g.transform.position, Vector3.zero);
			var b = renderers[0].bounds;
			foreach (Renderer r in renderers) 
			{
				b.Encapsulate(r.bounds);
			}
			return b;
		}

		public static List<string> GetAnimationNames(GameObject target, string separator = "")
		{
			List<string> animations = new List<string>();
			Animator animatorContainer = target.GetComponentInChildren<Animator>();
			if (animatorContainer != null)
			{
				if (animatorContainer.runtimeAnimatorController != null)
				{
					RuntimeAnimatorController runtimeAnimator = animatorContainer.runtimeAnimatorController;
					AnimationClip[] animationClips = runtimeAnimator.animationClips;
					foreach (AnimationClip item in animationClips)
					{
						string triggerAnimation = item.name;
						// THIS CODE WON'T BE HERE BECAUSE THE ANIMATIONS WILL HAVE THE SAME NAME AS THE TRIGGERS
						int indexSpecialAnimation = triggerAnimation.IndexOf(separator);
						if (indexSpecialAnimation != -1)
						{
							triggerAnimation = triggerAnimation.Substring(indexSpecialAnimation + 1, triggerAnimation.Length - (indexSpecialAnimation + 1)).ToLower();
						}
						else
						{
							triggerAnimation = triggerAnimation.ToLower();
						}
						animations.Add(triggerAnimation);
					}
				}
			}
			return animations;
		}

		public static void TrimArray(string[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Trim();
			}
		}
		
		public static void TrimArray(List<string> array)
		{
			for (int i = 0; i < array.Count; i++)
			{
				array[i] = array[i].Trim();
			}
		}

		public static string ReplaceTextPosition(string original, int start, int end, string replace)
		{
			int index = Mathf.Min(start, end);
			int size = Mathf.Abs(end - start);
			return original.Substring(0, index) + replace + original.Substring(index + size, original.Length - (index + size));
		}

 		public static string ReplaceWord(string text, string wordToReplace, string wordToReplaceWith)
    	{
			string pattern = string.Format(@"\b{0}\b", wordToReplace);
			string replacedText = Regex.Replace(text, pattern, wordToReplaceWith, RegexOptions.IgnoreCase);
			return replacedText;
	    }

		public static string RemoveNonStandardCharacters(string original)
		{
			  return Regex.Replace(original, @"[^a-zA-Z0-9]+", string.Empty);
		}

		public static string RemoveSpaces(string original)
		{
			string output = original;
			output = output.Replace(" ", "");
			output = output.Replace("\n", "");
			return output;
		}

		public static string ExtractXML(string tag, string data)
		{
			string startTag = "<" + tag + " ";
			string endTag = "</" + tag + ">";
			string buf = data;
			string output = "";
			while (buf.IndexOf(startTag) != -1)
			{
				output += buf.Substring(buf.IndexOf(startTag), (buf.IndexOf(endTag) + endTag.Length) - buf.IndexOf(startTag));
				buf = buf.Substring(buf.IndexOf(endTag) + endTag.Length, buf.Length - (buf.IndexOf(endTag) + endTag.Length));
			}
			return output;
		}

		public static string CalculateChecksum(string input)
		{
			using (SHA256 sha256Hash = SHA256.Create())
			{
				// Convert the input string to a byte array and compute the hash.
				byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

				// Convert byte array to a string.
				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < bytes.Length; i++)
				{
					builder.Append(bytes[i].ToString("x2"));
				}
				return builder.ToString();
			}
		}		
	}
}
