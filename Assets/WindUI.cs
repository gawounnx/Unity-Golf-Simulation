using UnityEngine;
using TMPro;

public class WindUI : MonoBehaviour
{
    public WindManager windManager;
    public TextMeshProUGUI windText;

    void Update()
    {   
        Debug.Log("WindUI 실행");
        if (windManager == null || windText == null)
            return;

        Vector3 wind = windManager.windForce;

        string direction = "";

        if (Mathf.Abs(wind.x) > Mathf.Abs(wind.z))
        {
            direction = wind.x > 0 ? "→" : "←";
        }
        else
        {
            direction = wind.z > 0 ? "↑" : "↓";
        }

        float strength = wind.magnitude;


        Debug.Log(direction);
        Debug.Log(wind.magnitude);
        
        windText.text =
            "Wind : "
            + direction
            + " "
            + strength.ToString("F1");
    }
}