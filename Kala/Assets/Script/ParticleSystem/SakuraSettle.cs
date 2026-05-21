using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SakuraSettle : MonoBehaviour
{
 [Header("Detection")]
    [Tooltip("Speed below this = petal has landed on tilemap")]
    [Range(0.01f, 0.5f)]
    public float landSpeedThreshold = 0.12f;

    [Header("Fade")]
    [Range(0.1f, 5f)]
    [Tooltip("Higher = faster fade after landing")]
    public float fadeSpeed = 1.0f;

    [Tooltip("Seconds to stay visible before fading starts")]
    public float restDuration = 0.5f;

    ParticleSystem ps;
    ParticleSystem.Particle[] buffer;

    // Tracks per-particle rest timer using stable random seed as key
    System.Collections.Generic.Dictionary<uint, float> restTimers
        = new System.Collections.Generic.Dictionary<uint, float>();

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        buffer = new ParticleSystem.Particle[ps.main.maxParticles];
    }

    void LateUpdate()
    {
        int count = ps.GetParticles(buffer);

        for (int i = 0; i < count; i++)
        {
            float speed = buffer[i].velocity.magnitude;
            uint id = buffer[i].randomSeed; // stable unique ID per particle

            if (speed < landSpeedThreshold)
            {
                // -- Stop the petal completely --
                buffer[i].velocity        = Vector3.zero;
                buffer[i].angularVelocity = 0f;
                buffer[i].angularVelocity3D = Vector3.zero;

                // -- Rest timer before fade begins --
                if (!restTimers.ContainsKey(id))
                    restTimers[id] = 0f;

                restTimers[id] += Time.deltaTime;

                if (restTimers[id] >= restDuration)
                {
                    // Drain lifetime → Color over Lifetime fades alpha to 0
                    buffer[i].remainingLifetime -= fadeSpeed * Time.deltaTime;
                }
            }
            else
            {
                // Still falling — remove timer if it somehow existed
                restTimers.Remove(id);
            }
        }

        ps.SetParticles(buffer, count);

        // Prune dead keys to avoid memory leak
        if (restTimers.Count > ps.main.maxParticles)
            restTimers.Clear();
    }
}