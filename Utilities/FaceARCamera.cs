using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace yourvrexperience.Utils
{
    public class FaceARCamera : MonoBehaviour
    {
        void Update()
        {
            FaceTowardsCamera();
        }

        void FaceTowardsCamera()
        {
            Vector3 toCamera = Camera.main.transform.position - transform.position;
#if UNITY_EDITOR
            this.transform.localRotation = Quaternion.Euler(new Vector3(0,Utilities.GetAngleFromNormal(new Vector2(toCamera.x, toCamera.z)),0));
#else
            this.transform.localRotation = Quaternion.Euler(new Vector3(0,Utilities.GetAngleFromNormal(new Vector2(toCamera.x, toCamera.y)),0));
#endif 
        }
    }
}