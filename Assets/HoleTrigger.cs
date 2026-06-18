using UnityEngine;
using TMPro;

public class HoleTrigger : MonoBehaviour
{
    public TextMeshProUGUI holeInText;

    private void Start()
    {
        if (holeInText != null)
        {
            holeInText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("들어옴 : " + other.name);


        if (other.CompareTag("GolfBall"))
        {
            Debug.Log("HOLE IN!");

            if (holeInText != null)
            {
                holeInText.gameObject.SetActive(true);
            }

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            GolfBallController ball =
    other.GetComponent<GolfBallController>();

            if (ball != null)
            {
                ball.gameClear = true;
            }
        }
    }
}