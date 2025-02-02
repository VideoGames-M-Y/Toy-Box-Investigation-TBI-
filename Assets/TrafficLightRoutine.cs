using System.Collections;
using UnityEngine;

public class TrafficLightRoutine : MonoBehaviour
{
    [SerializeField] private GameObject redLight;
    [SerializeField] private GameObject greenLight;
    [SerializeField] private GameObject blinkingGreenLight;
    [SerializeField] private float redDuration = 5f;
    [SerializeField] private float greenDuration = 5f;
    [SerializeField] private float blinkingGreenDuration = 3f;
    [SerializeField] private float blinkInterval = 0.5f;

    private void Start()
    {
        StartCoroutine(TrafficRoutine());
    }

    private IEnumerator TrafficRoutine()
    {
        while (true)
        {
            // Red light phase
            redLight.SetActive(true);
            greenLight.SetActive(false);
            blinkingGreenLight.SetActive(false);
            yield return new WaitForSeconds(redDuration);

            // Green light phase
            redLight.SetActive(false);
            greenLight.SetActive(true);
            yield return new WaitForSeconds(greenDuration);

            // Blinking green phase
            greenLight.SetActive(false);
            for (float t = 0; t < blinkingGreenDuration; t += blinkInterval)
            {
                blinkingGreenLight.SetActive(!blinkingGreenLight.activeSelf);
                yield return new WaitForSeconds(blinkInterval);
            }
            blinkingGreenLight.SetActive(false);
        }
    }
}