using CUESDK;
using System;
using UnityEngine;

public class LED : MonoBehaviour {
    public KinectMeshGenerator kinectMeshGenerator;
    CorsairLedColor currentColor = new() { R = 0, G = 0, B = 255 };
    int deviceCount;

    private void Start() {
        CorsairLightingSDK.PerformProtocolHandshake();

        if (CorsairLightingSDK.GetLastError() != CorsairError.Success) {
            Debug.LogError("Failed to connect to iCUE");
            return;
        }

        CorsairLightingSDK.RequestControl(CorsairAccessMode.ExclusiveLightingControl);

        deviceCount = CorsairLightingSDK.GetDeviceCount();
    }

    void Update() {
        if (kinectMeshGenerator.distancePixelMiddle >= 0.40 && kinectMeshGenerator.distancePixelMiddle <= 0.65) {
            currentColor.R = 0;
            currentColor.G = 255;
            currentColor.B = 0;
        }
        else {
            currentColor.B = 0;
            currentColor.G = 0;
            currentColor.R = 255;
        }

        SetDeviceColors();
    }

    private void SetDeviceColors() {
        for (var i = 0; i < deviceCount; i++) {
            var deviceLeds = CorsairLightingSDK.GetLedPositionsByDeviceIndex(i);
            var buffer = new CorsairLedColor[deviceLeds.NumberOfLeds];

            for (var j = 0; j < deviceLeds.NumberOfLeds; j++) {
                buffer[j] = currentColor;
                buffer[j].LedId = deviceLeds.LedPosition[j].LedId;
            }

            CorsairLightingSDK.SetLedsColorsBufferByDeviceIndex(i, buffer);
            CorsairLightingSDK.SetLedsColorsFlushBuffer();
        }
    }
}