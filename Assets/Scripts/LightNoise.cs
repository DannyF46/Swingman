using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightNoise : MonoBehaviour
{
    public enum NoiseDirections
    {
        X_only, Y_only, XY
    };

    private Light2D lightComp;
    private Transform LightTransform;
    private Vector2 initialLightPos;
    private float initialLightIntensity;
    
    [SerializeField] private bool MotionNoise;
    [SerializeField] private NoiseDirections NoiseDirection = new();
    [SerializeField] private float motionNoiseMagnitude;
    [SerializeField] private float motionNoiseFrequency;

    [SerializeField] private bool IntensityNoise;
    [SerializeField] float intensityNoiseMagnitude;
    [SerializeField] private float intensityNoiseFrequency;

    private float t_motion;
    private float t_intensity;
    private Vector2 posPerterbation;
    private float intensityPerterbation;

    // Start is called before the first frame update
    void Start()
    {
        lightComp = this.GetComponent<Light2D>();
        LightTransform = this.GetComponent<Transform>();
        initialLightPos = LightTransform.position;
        initialLightIntensity = lightComp.intensity;

        posPerterbation = initialLightPos + Random.insideUnitCircle * motionNoiseMagnitude;
        intensityPerterbation = initialLightIntensity + Random.Range(-1,1) * intensityNoiseMagnitude;

    }

    // Update is called once per frame
    void Update()
    {
        if (MotionNoise)
        {
            if (t_motion < 1 / motionNoiseFrequency)
            {
                if (NoiseDirection == NoiseDirections.X_only)
                {
                    LightTransform.position = Vector3.Lerp(LightTransform.position, new(posPerterbation.x, LightTransform.position.y), t_motion * motionNoiseFrequency);
                }
                else if (NoiseDirection == NoiseDirections.Y_only)
                {
                    LightTransform.position = Vector3.Lerp(LightTransform.position, new(LightTransform.position.x, posPerterbation.y), t_motion * motionNoiseFrequency);
                }
                else if (NoiseDirection == NoiseDirections.XY)
                {
                    LightTransform.position = Vector3.Lerp(LightTransform.position, posPerterbation, t_motion * motionNoiseFrequency);
                }

                t_motion += Time.deltaTime;
            }
            else
            {
                t_motion = 0;
                posPerterbation = initialLightPos + Random.insideUnitCircle * motionNoiseMagnitude;
            }
        }

        if (IntensityNoise)
        {
            if (t_intensity < 1 / intensityNoiseFrequency)
            {
                lightComp.intensity = Mathf.Lerp(lightComp.intensity, intensityPerterbation, t_intensity * intensityNoiseFrequency);
                t_intensity += Time.deltaTime;
            }
            else
            {
                t_intensity = 0;
                intensityPerterbation = initialLightIntensity + Random.Range(-1, 1) * intensityNoiseMagnitude;
            }
        }
        



            
        
    }
}
