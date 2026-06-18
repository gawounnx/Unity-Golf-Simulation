using UnityEngine;
using TMPro;

public class StrokeUI : MonoBehaviour
{
    public GolfBallController golfBall;

    private TextMeshProUGUI strokeText;

    void Start()
    {
        strokeText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (golfBall == null || strokeText == null)
            return;

        strokeText.text =
            "Stroke : " + golfBall.stroke;
    }
}