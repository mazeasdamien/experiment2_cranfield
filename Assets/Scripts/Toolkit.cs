using Microsoft.Azure.Kinect.Sensor;
using OpenCvSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Telexistence {
    public static class Toolkit {
        public static void ProcessPointCloudData(Short3[] PointCloud, BGRA[] colorArray, int depthHeight, int depthWidth, float minDistance, int areaWidth, int areaHeight, int[] indices, Vector3[] vertices, Color32[] colours, float closeClip, float farClip) {
            var middlePixelIndex = ((depthHeight / 2) * depthWidth) + (depthWidth / 2);
            var middlePoint = PointCloud[middlePixelIndex];
            minDistance = float.MaxValue;
            var halfWidth = areaWidth / 2;
            var halfHeight = areaHeight / 2;
            var minX = (depthWidth / 2) - halfWidth;
            var maxX = (depthWidth / 2) + halfWidth;
            var minY = (depthHeight / 2) - halfHeight;
            var maxY = (depthHeight / 2) + halfHeight;

            var triangleIndex = 0;
            var pointIndex = 0;
            int topLeft, topRight, bottomLeft, bottomRight;
            int tl, tr, bl, br;

            for (int y = 0; y < depthHeight; y++) {
                for (int x = 0; x < depthWidth; x++) {
                    vertices[pointIndex].x = PointCloud[pointIndex].X * 0.001f;
                    vertices[pointIndex].y = -PointCloud[pointIndex].Y * 0.001f;
                    vertices[pointIndex].z = PointCloud[pointIndex].Z * 0.001f;

                    colours[pointIndex].a = 255;
                    colours[pointIndex].b = colorArray[pointIndex].B;
                    colours[pointIndex].g = colorArray[pointIndex].G;
                    colours[pointIndex].r = colorArray[pointIndex].R;

                    if (x >= minX && x <= maxX && y >= minY && y <= maxY) {
                        var currentDistance = vertices[pointIndex].z;

                        if (currentDistance > 0 && currentDistance < minDistance) {
                            minDistance = currentDistance;
                        }
                    }

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
                            indices[triangleIndex++] = topLeft;
                            indices[triangleIndex++] = topRight;
                            indices[triangleIndex++] = bottomLeft;
                        }
                        else {
                            indices[triangleIndex++] = 0;
                            indices[triangleIndex++] = 0;
                            indices[triangleIndex++] = 0;
                        }

                        if (bl > closeClip && bl < farClip && tr > closeClip && tr < farClip && br > closeClip && br < farClip) {
                            indices[triangleIndex++] = bottomLeft;
                            indices[triangleIndex++] = topRight;
                            indices[triangleIndex++] = bottomRight;
                        }
                        else {
                            indices[triangleIndex++] = 0;
                            indices[triangleIndex++] = 0;
                            indices[triangleIndex++] = 0;
                        }
                    }

                    pointIndex++;
                }
            }
        }

        public static Texture2D MatToTexture2D(Mat mat) {
            int width = mat.Width;
            int height = mat.Height;
            int channels = mat.Channels();
            Texture2D texture;

            // Convert the Mat's data to a byte array
            byte[] data = new byte[width * height * mat.ElemSize()];
            Marshal.Copy(mat.Data, data, 0, data.Length);

            if (channels == 4) {
                texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

                // Swap the red and blue channels
                for (int i = 0; i < data.Length; i += 4) {
                    byte temp = data[i];
                    data[i] = data[i + 2];
                    data[i + 2] = temp;
                }
            }
            else if (channels == 3) {
                texture = new Texture2D(width, height, TextureFormat.RGB24, false);

                // Swap the red and blue channels
                for (int i = 0; i < data.Length; i += 3) {
                    byte temp = data[i];
                    data[i] = data[i + 2];
                    data[i + 2] = temp;
                }
            }
            else {
                throw new ArgumentException("Input Mat must have 3 or 4 channels.");
            }

            // Load the byte array into the texture
            texture.LoadRawTextureData(data);
            texture.Apply();

            return texture;
        }

        public static void DrawAxis(Mat image, Vec3d rvec, Vec3d tvec, float length, Mat cameraMatrix, Mat distCoeffs) {
            float[,] axisPoints = new float[,] {
    {0, 0, 0},
    {length, 0, 0},
    {0, length, 0},
    {0, 0, length}
    };
            Mat objectPoints = new Mat(4, 3, MatType.CV_32FC1, axisPoints);
            Mat imagePoints = new Mat();
            Cv2.ProjectPoints(objectPoints, rvec, tvec, cameraMatrix, distCoeffs, imagePoints);

            Cv2.Line(image, new Point((int)imagePoints.Get<Point2f>(0).X, (int)imagePoints.Get<Point2f>(0).Y), new Point((int)imagePoints.Get<Point2f>(1).X, (int)imagePoints.Get<Point2f>(1).Y), Scalar.Red, 2);
            Cv2.Line(image, new Point((int)imagePoints.Get<Point2f>(0).X, (int)imagePoints.Get<Point2f>(0).Y), new Point((int)imagePoints.Get<Point2f>(2).X, (int)imagePoints.Get<Point2f>(2).Y), Scalar.Green, 2);
            Cv2.Line(image, new Point((int)imagePoints.Get<Point2f>(0).X, (int)imagePoints.Get<Point2f>(0).Y), new Point((int)imagePoints.Get<Point2f>(3).X, (int)imagePoints.Get<Point2f>(3).Y), Scalar.Blue, 2);
        }

        public static void InitCameraMatrixAndDistCoeffs(Calibration calibration, Mat cameraMatrix, Mat distCoeffs) 
        {
            Intrinsics Intrinsics = calibration.ColorCameraCalibration.Intrinsics;
            // Create the camera matrix
            cameraMatrix.Set(0, 0, Intrinsics.Parameters[2]);
            cameraMatrix.Set(0, 1, 0);
            cameraMatrix.Set(0, 2, Intrinsics.Parameters[0]);
            cameraMatrix.Set(1, 0, 0);
            cameraMatrix.Set(1, 1, Intrinsics.Parameters[3]);
            cameraMatrix.Set(1, 2, Intrinsics.Parameters[1]);
            cameraMatrix.Set(2, 0, 0);
            cameraMatrix.Set(2, 1, 0);
            cameraMatrix.Set(2, 2, 1f);

            // Create the distortion coefficients
            distCoeffs.Set(0, 0, Intrinsics.Parameters[4]);
            distCoeffs.Set(0, 1, Intrinsics.Parameters[5]);
            distCoeffs.Set(0, 2, Intrinsics.Parameters[13]);
            distCoeffs.Set(0, 3, Intrinsics.Parameters[12]);
            distCoeffs.Set(0, 4, Intrinsics.Parameters[6]);
            distCoeffs.Set(0, 5, Intrinsics.Parameters[7]);
            distCoeffs.Set(0, 6, Intrinsics.Parameters[8]);
            distCoeffs.Set(0, 7, Intrinsics.Parameters[9]);
        }
    }
}