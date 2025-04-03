using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxBackground : MonoBehaviour
{
    public ParallaxCamera parallaxCamera;
    List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

    void Start()
    {
        TryAssignCamera();
        SetLayers();
    }

    void Update()
    {
        if (parallaxCamera == null)
        {
            TryAssignCamera();
        }
    }

    void TryAssignCamera()
    {
        if (parallaxCamera == null)
        {
            GameObject cameraObject = GameObject.FindWithTag("MainCamera"); // Busca la cámara con tag "MainCamera"

            if (cameraObject != null)
            {
                parallaxCamera = cameraObject.GetComponent<ParallaxCamera>();

                if (parallaxCamera != null)
                {
                    parallaxCamera.onCameraTranslate += Move;
                    Debug.Log("ParallaxCamera asignada correctamente.");
                }
            }
        }
    }

    void SetLayers()
    {
        parallaxLayers.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();

            if (layer != null)
            {
                layer.name = "Layer-" + i;
                parallaxLayers.Add(layer);
            }
        }
    }

    void Move(float delta)
    {
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            layer.Move(delta);
        }
    }
}
