
using Unity.VisualScripting;
using UnityEngine;

namespace yourvrexperience.Utils
{
    public class IgnoreZTestComponent : MonoBehaviour
    {
        void Start()
        {
            Utilities.ApplyZTestTop(this.transform);
        }
    }
}