using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector;
    [SerializeField] float period = 2f;


    [SerializeField] [Range(0,1)] float movementFactor;

    Vector3 startingPosition;

    // Start is called before the first frame update
    void Start()
    {
       startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon) { return;  }

            float cycle = Time.time / period;

            const float tau = Mathf.PI * 2;
            float rawSinWave = Mathf.Sin(tau * cycle);

            movementFactor = rawSinWave / 2f + 0.5f;

            Vector3 offset = movementVector * movementFactor;
            transform.position = startingPosition + offset;

    }
}
