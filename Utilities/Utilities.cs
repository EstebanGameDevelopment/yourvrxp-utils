using System;
using System.Collections.Generic;
using UnityEngine;

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

		public static string RandomCodeGeneration(string idUser)
		{
			string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var stringChars = new char[8];
			var random = new System.Random();

			for (int i = 0; i < stringChars.Length; i++)
			{
				stringChars[i] = chars[random.Next(chars.Length)];
			}

			string finalString = idUser + "_" + new String(stringChars);
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
	}
}
