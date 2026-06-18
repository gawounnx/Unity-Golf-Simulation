using UnityEngine;

public class WindManager : MonoBehaviour
{
    public Vector3 windForce;

    void Start()
    {
        windForce = new Vector3(
            Random.Range(-2f, 2f),
            0,
            Random.Range(-2f, 2f)
        );
    }
}