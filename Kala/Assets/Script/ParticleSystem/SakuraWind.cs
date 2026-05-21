using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SakuraWind : MonoBehaviour
{
    public float windStrength = 0.5f;
    public float windCycleSpeed = 0.4f;
    private ParticleSystem ps;

    void Start() {
        ps = GetComponent<ParticleSystem>();
    }

    void Update() {
        float wind = Mathf.Sin(Time.time * windCycleSpeed)
                     * windStrength;

        var vel = ps.velocityOverLifetime;
        vel.x = new ParticleSystem.MinMaxCurve(
            -wind, wind);
    }
}
