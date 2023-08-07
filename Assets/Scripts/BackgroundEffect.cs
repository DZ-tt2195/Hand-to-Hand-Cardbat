using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundEffect : MonoBehaviour
{
    public Color[] colors = new Color[3];
    int nextIndex;
    Camera cam;
    float changeSpeed = 2;
    float currentTime;
    float time = 2f;

    private void Awake()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    private void Update()
    {
        ColorChange();
        ChangeTime();
    }

    void ColorChange()
    {
        cam.backgroundColor = Color.Lerp(cam.backgroundColor, colors[nextIndex], changeSpeed * Time.deltaTime);
    }

    void ChangeTime()
    {
        if (currentTime <= 0)
        {
            nextIndex = RollValue();
            currentTime = time;
        }
        else
        {
            currentTime -= Time.deltaTime;
        }
    }

    int RollValue()
    {
        int nextRoll = nextIndex;
        while (nextRoll == nextIndex)
            nextRoll = Random.Range(0, 3);
        return nextRoll;
    }
}
