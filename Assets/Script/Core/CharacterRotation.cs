using UnityEngine;

namespace Encounter.NightDance.Core
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class CharacterRotation : MonoBehaviour
    {
        void LateUpdate()
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}