using Microsoft.Azure.Kinect.Sensor;
using System.Collections;
using UnityEngine;
using PclSharp;

public class KinectMeshGenerator : MonoBehaviour {
    [SerializeField]
    float updateInterval = 0.1f;

    public float closeClip = 500f; // Close clip distance in millimeters
    public float farClip = 3000f; // Far clip distance in millimeters

    public bool renderPointCloud;

    public float distancePixelMiddle;

    Device kinect;
    int depthWidth, depthHeight, num;
    Mesh mesh;
    Vector3[] vertices;
    Color32[] colors;
    int[] indeces;
    Texture2D texture;
    Transformation transformation;

    void OnDestroy() => kinect.StopCameras();

    void Start() {
        InitKinect();
        InitMesh();
        StartCoroutine(KinectLoop(kinect));
    }

    void InitKinect() {
        kinect = Device.Open(0);

        kinect.StartCameras(new() {
            ColorFormat = ImageFormat.ColorBGRA32,
            ColorResolution = ColorResolution.R720p,
            DepthMode = DepthMode.NFOV_Unbinned,
            SynchronizedImagesOnly = true,
            CameraFPS = FPS.FPS30,
        });

        transformation = kinect.GetCalibration().CreateTransformation();
    }

    void InitMesh() {
        depthWidth = kinect.GetCalibration().DepthCameraCalibration.ResolutionWidth;
        depthHeight = kinect.GetCalibration().DepthCameraCalibration.ResolutionHeight;
        num = depthWidth * depthHeight;

        mesh = new();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        vertices = new Vector3[num];
        colors = new Color32[num];
        texture = new(depthWidth, depthHeight);
        var uv = new Vector2[num];
        var normals = new Vector3[num];
        indeces = new int[6 * (depthWidth - 1) * (depthHeight - 1)];

        var index = 0;

        for (int y = 0; y < depthHeight; y++) {
            for (int x = 0; x < depthWidth; x++) {
                uv[index] = new(((float)(x + 0.5f) / (float)(depthWidth)), ((float)(y + 0.5f) / ((float)(depthHeight))));
                normals[index] = new(0, -1, 0);
                index++;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.normals = normals;

        gameObject.GetComponent<MeshRenderer>().materials[0].mainTexture = texture;
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
    }

    IEnumerator KinectLoop(Device device) {
        while (true) {
            using (Capture capture = device.GetCapture()) {
                var modifiedColor = transformation.ColorImageToDepthCamera(capture);
                var colorArray = modifiedColor.GetPixels<BGRA>().ToArray();

                var cloudImage = transformation.DepthImageToPointCloud(capture.Depth);
                var PointCloud = cloudImage.GetPixels<Short3>().ToArray();

                var middlePixelIndex = ((depthHeight / 2) * depthWidth) + (depthWidth / 2);
                var middlePoint = PointCloud[middlePixelIndex];
                distancePixelMiddle = Mathf.Sqrt(Mathf.Pow(middlePoint.X, 2) + Mathf.Pow(middlePoint.Y, 2) + Mathf.Pow(middlePoint.Z, 2)) * 0.001f;

                var triangleIndex = 0;
                var pointIndex = 0;
                int topLeft, topRight, bottomLeft, bottomRight;
                int tl, tr, bl, br;

                for (int y = 0; y < depthHeight; y++) {
                    for (int x = 0; x < depthWidth; x++) {
                        vertices[pointIndex].x = PointCloud[pointIndex].X * 0.001f;
                        vertices[pointIndex].y = -PointCloud[pointIndex].Y * 0.001f;
                        vertices[pointIndex].z = PointCloud[pointIndex].Z * 0.001f;

                        colors[pointIndex].a = 255;
                        colors[pointIndex].b = colorArray[pointIndex].B;
                        colors[pointIndex].g = colorArray[pointIndex].G;
                        colors[pointIndex].r = colorArray[pointIndex].R;

                        if (x != (depthWidth - 1) && y != (depthHeight - 1)) {
                            topLeft = pointIndex;
                            topRight = topLeft + 1;
                            bottomLeft = topLeft + depthWidth;
                            bottomRight = bottomLeft + 1;
                            tl = PointCloud[topLeft].Z;
                            tr = PointCloud[topRight].Z;
                            bl = PointCloud[bottomLeft].Z;
                            br = PointCloud[bottomRight].Z;

                            if (tl > closeClip && tl < farClip && tr > closeClip && tr < farClip && bl > closeClip && bl < farClip) {
                                indeces[triangleIndex++] = topLeft;
                                indeces[triangleIndex++] = topRight;
                                indeces[triangleIndex++] = bottomLeft;
                            }
                            else {
                                indeces[triangleIndex++] = 0;
                                indeces[triangleIndex++] = 0;
                                indeces[triangleIndex++] = 0;
                            }

                            if (bl > closeClip && bl < farClip && tr > closeClip && tr < farClip && br > closeClip && br < farClip) {
                                indeces[triangleIndex++] = bottomLeft;
                                indeces[triangleIndex++] = topRight;
                                indeces[triangleIndex++] = bottomRight;
                            }
                            else {
                                indeces[triangleIndex++] = 0;
                                indeces[triangleIndex++] = 0;
                                indeces[triangleIndex++] = 0;
                            }
                        }

                        pointIndex++;
                    }
                }

                texture.SetPixels32(colors);
                texture.Apply();

                mesh.vertices = vertices;

                if (renderPointCloud) {
                    mesh.SetIndices(indeces, MeshTopology.Points, 0);
                }
                else {
                    mesh.triangles = indeces;
                    mesh.RecalculateBounds();
                }

                yield return new WaitForSeconds(updateInterval);
            }
        }
    }
}