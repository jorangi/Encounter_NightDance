using UnityEngine;

public class TestCharacter_RotationCamera : MonoBehaviour
{
    void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
