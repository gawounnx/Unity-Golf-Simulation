using UnityEngine;
using UnityEngine.UI;

public class PowerUI : MonoBehaviour
{
    public GolfBallController golfBall;
    public Slider powerSlider;

    void Update()
    {
        powerSlider.value = golfBall.currentPower;
    }
}