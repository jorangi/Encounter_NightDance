using UnityEngine;

namespace Encounter.NightDance.Character
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