using UnityEngine;

public class Prototype_GameManager : MonoBehaviour
{
    [SerializeField]private Transform Focus;
    [SerializeField]private Transform FocusUnit;

    private void Start()
    {
        Focus.transform.position = FocusUnit.transform.position;
    }
}
