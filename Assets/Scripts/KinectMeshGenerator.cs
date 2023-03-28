using Microsoft.Azure.Kinect.Sensor;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Telexistence {
    public class KinectMeshGenerator : MonoBehaviour {
        [Header("OpenCV Settings")]
        private VideoCapture cap_opencv;
        public RawImage rawImage;

        [Header("Mesh Settings")]
        public GameObject meshPrefab;
        private static int instanceCounter = 0;
        public float updateInterval = 0.1f;
        public float closeClip = 500f;
        public float farClip = 3000f;
        public float minDistance;
        public int areaWidth = 740, areaHeight = 360;
        public int thickness;
        private BGRA[] colorData;
        private Mat colorMat;
        private Mat bgrMat;

        [Header("Capture Frame Rate")]
        public float captureFrameRate = 24f;

        [Header("Kinect and Mesh Data")]
        public MeshPublisher meshPublisher;
        private Device Kinect;
        private int depthWidth, depthHeight, num;
        private Mesh mesh;
        private Vector3[] vertices;
        private Color32[] colours;
        private int[] indices;
        private Texture2D texture;
        private Transformation transformation;
        private Calibration calibration;

        [Header("Aruco and Marker Settings")]
        private Coroutine processOpenCVCoroutine;
        public GameObject kinectModel;
        public Transform markerTransform;
        private Dictionary arucoDictionary;
        private DetectorParameters detectorParameters;
        private Mat cameraMatrix = new Mat(3, 3, MatType.CV_32FC1);
        private Mat distCoeffs = new Mat(1, 8, MatType.CV_32FC1);
        public float markerLength = 0.05f;

        void OnDestroy() {
            Kinect.StopCameras();
            cap_opencv.Dispose();
            cameraMatrix.Dispose();
            distCoeffs.Dispose();
            colorMat.Dispose();
            bgrMat.Dispose();
            if (processOpenCVCoroutine != null) {
                StopCoroutine(processOpenCVCoroutine);
            }
        }

        void Start() {
            // Initialize video capture and face detection
            cap_opencv = new VideoCapture();
            cap_opencv.Open(1, VideoCaptureAPIs.ANY);

            // Initialize ArUco marker detection
            arucoDictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict6X6_250);
            detectorParameters = new DetectorParameters();
            detectorParameters.CornerRefinementMethod = CornerRefineMethod.Subpix;
            detectorParameters.CornerRefinementWinSize = 9;

            // Initialize Kinect and Mesh
            int colorWidth = calibration.ColorCameraCalibration.ResolutionWidth;
            int colorHeight = calibration.ColorCameraCalibration.ResolutionHeight;
            colorData = new BGRA[colorWidth * colorHeight];
            colorMat = new Mat(colorHeight, colorWidth, MatType.CV_8UC4);
            bgrMat = new Mat(colorHeight, colorWidth, MatType.CV_8UC3);
            InitKinect();
            InitMesh();
            processOpenCVCoroutine = StartCoroutine(ProcessOpenCVwork());
        }

        IEnumerator ProcessOpenCVwork() {
            while (true) {
                yield return new WaitForSeconds(1f / captureFrameRate);
                DetectAndDrawMarkers();
            }
        }

        void DetectAndDrawMarkers() {
            using (var capture = Kinect.GetCapture()) {
                // Capture raw image
                Microsoft.Azure.Kinect.Sensor.Image colourImage = capture.Color;

                if (colourImage != null && colourImage.WidthPixels > 0 && colourImage.HeightPixels > 0) {
                    // Update colorData with the latest pixel data
                    colorData = colourImage.GetPixels<BGRA>().ToArray();

                    // Update colorMat with the latest colorData
                    GCHandle pinnedArray = GCHandle.Alloc(colorData, GCHandleType.Pinned);
                    IntPtr colorDataPtr = pinnedArray.AddrOfPinnedObject();
                    using (Mat colourMat = new Mat(colourImage.HeightPixels, colourImage.WidthPixels, MatType.CV_8UC4, colorDataPtr)) {
                        // Convert BGRA to BGR format
                        Cv2.CvtColor(colourMat, bgrMat, ColorConversionCodes.BGRA2BGR);

                        CvAruco.DetectMarkers(bgrMat, arucoDictionary, out var corners, out var ids, detectorParameters, out var rejectedPoints);
                        CvAruco.DrawDetectedMarkers(bgrMat, corners, ids, Scalar.Green);

                        if (ids.Length > 0) {
                            using (Mat rvecsMat = new Mat())
                            using (Mat tvecsMat = new Mat()) {
                                CvAruco.EstimatePoseSingleMarkers(corners, markerLength, cameraMatrix, distCoeffs, rvecsMat, tvecsMat);

                                for (int i = 0; i < ids.Length; i++) {
                                    Vec3d rvec = rvecsMat.Get<Vec3d>(i);
                                    Vec3d tvec = tvecsMat.Get<Vec3d>(i);

                                    Toolkit.DrawAxis(bgrMat, rvec, tvec, markerLength, cameraMatrix, distCoeffs);
                                }
                            }
                        }

                        if (rawImage.texture != null) {
                            Destroy(rawImage.texture);
                        }
                        rawImage.texture = Toolkit.MatToTexture2D(bgrMat);

                    }
                    pinnedArray.Free();
                }
            }
        }


        public void InstantiateMesh() {
            KinectLoop();
            // Instantiate the prefab at the current GameObject's position
            GameObject newMeshObject = Instantiate(meshPrefab, transform.position, Quaternion.identity);

            // Increment the instance counter and rename the new GameObject
            instanceCounter++;
            newMeshObject.name = "MeshInstance_" + instanceCounter;

            // Get the MeshFilter and MeshRenderer components
            MeshFilter meshFilter = newMeshObject.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = newMeshObject.GetComponent<MeshRenderer>();

            // Create a deep copy of the current mesh and assign it to the new GameObject
            Mesh snapshotMesh = DeepCopyMesh(mesh);
            snapshotMesh.name = "MeshInstance_" + instanceCounter;
            meshFilter.mesh = snapshotMesh;

            // Create a copy of the current texture and assign it to the new GameObject
            Texture2D snapshotTexture = meshRenderer.material.mainTexture as Texture2D;
            if (snapshotTexture != null) {
                Destroy(snapshotTexture);
            }
            snapshotTexture = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 1);
            Graphics.CopyTexture(texture, snapshotTexture);
            snapshotTexture.Apply();


            // Create a new material with the texture and assign it to the new GameObject
            Material snapshotMaterial = new Material(gameObject.GetComponent<MeshRenderer>().material) {
                mainTexture = snapshotTexture
            };
            meshRenderer.material = snapshotMaterial;
            meshPublisher.SendMeshData(snapshotMesh, snapshotTexture);
        }

        Mesh DeepCopyMesh(Mesh originalMesh) {
            Mesh copiedMesh = new Mesh();

            copiedMesh.indexFormat = originalMesh.indexFormat;
            copiedMesh.vertices = (Vector3[])originalMesh.vertices.Clone();
            copiedMesh.normals = (Vector3[])originalMesh.normals.Clone();
            copiedMesh.uv = (Vector2[])originalMesh.uv.Clone();
            copiedMesh.colors32 = (Color32[])originalMesh.colors32.Clone();
            copiedMesh.bounds = originalMesh.bounds;

            // Copy submeshes (triangle indices)
            copiedMesh.subMeshCount = originalMesh.subMeshCount;
            for (int i = 0; i < originalMesh.subMeshCount; i++) {
                copiedMesh.SetTriangles(originalMesh.GetTriangles(i), i);
            }

            return copiedMesh;
        }

        void InitKinect() {
            Kinect = Device.Open(0);

            Kinect.StartCameras(new DeviceConfiguration {
                ColorFormat = ImageFormat.ColorBGRA32,
                ColorResolution = ColorResolution.R720p,
                DepthMode = DepthMode.NFOV_Unbinned,
                SynchronizedImagesOnly = true,
                CameraFPS = FPS.FPS30,
            });

            calibration = Kinect.GetCalibration();
            transformation = calibration.CreateTransformation();
            Toolkit.InitCameraMatrixAndDistCoeffs(calibration, cameraMatrix, distCoeffs);
        }

        void InitMesh() {
            // Get depth camera calibration data
            depthWidth = Kinect.GetCalibration().DepthCameraCalibration.ResolutionWidth;
            depthHeight = Kinect.GetCalibration().DepthCameraCalibration.ResolutionHeight;
            num = depthWidth * depthHeight;

            // Initialize mesh and related data structures
            mesh = new Mesh {
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
            };

            vertices = new Vector3[num];
            colours = new Color32[num];
            texture = new Texture2D(depthWidth, depthHeight);
            var uv = new Vector2[num];
            var normals = new Vector3[num];
            indices = new int[6 * (depthWidth - 1) * (depthHeight - 1)];

            int index = 0;

            // Set mesh UVs and normals
            for (int y = 0; y < depthHeight; y++) {
                for (int x = 0; x < depthWidth; x++) {
                    uv[index] = new Vector2(((float)(x + 0.5f) / (float)(depthWidth)), ((float)(y + 0.5f) / ((float)(depthHeight))));
                    normals[index] = new Vector3(0, -1, 0);
                    index++;
                }
            }

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.normals = normals;
        }

        void KinectLoop() {
            try {
                using (var capture = Kinect.GetCapture()) {
                    var modifiedColor = transformation.ColorImageToDepthCamera(capture);

                    var colorArray = modifiedColor.GetPixels<BGRA>().ToArray();

                    // Create point cloud from depth image
                    var cloudImage = transformation.DepthImageToPointCloud(capture.Depth);
                    var PointCloud = cloudImage.GetPixels<Short3>().ToArray();

                    // Process point cloud data
                    Toolkit.ProcessPointCloudData(PointCloud, colorArray, depthHeight, depthWidth, minDistance, areaWidth, areaHeight, indices, vertices, colours, closeClip, farClip);

                    texture.SetPixels32(colours);
                    texture.Apply();

                    mesh.vertices = vertices;

                    mesh.triangles = indices;
                    mesh.RecalculateBounds();
                }
            }
            catch (AzureKinectException ex) {
                Debug.LogError("Failed to get capture from Azure Kinect: " + ex.Message);
            }
        }

        void OnDisable() {
            if (processOpenCVCoroutine != null) {
                StopCoroutine(processOpenCVCoroutine);
            }
        }
    }
}