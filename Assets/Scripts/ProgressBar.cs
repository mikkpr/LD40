using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public GameObject progressBar;
    float value = 0;
    float initialX;
    float progressWidth = 240f;

    void Start()
    {
        GameObject marker = progressBar.transform.Find("Marker").gameObject;
        initialX = marker.transform.localPosition.x;
    }
    public void SetValue(float value)
    {
        float clampedValue = Mathf.Clamp(value, 0, 100);
        this.value = clampedValue;
    }
    public float GetValue(float value)
    {
        return this.value;
    }

    public void Decrement(float amount)
    {
        SetValue(this.value - amount);
    }

    public void Increment(float amount)
    {
        SetValue(this.value + amount);
    }

    void Update()
    {
        GameObject marker = progressBar.transform.Find("Marker").gameObject;
        Vector3 newPos = new Vector3(
            initialX + progressWidth * this.value / 100f,
            marker.transform.localPosition.y,
            marker.transform.localPosition.z
        );
        marker.transform.localPosition = newPos;
    }

    public void OnIncrementButtonClick()
    {
        this.Increment(10f);
    }

    public void OnDecrementButtonClick()
    {

        this.Decrement(10f);
    }
}
