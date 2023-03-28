using FRRobot;
using System;
using System.Collections;
using UnityEngine;

namespace Telexistence {
    public class FanucHandler : MonoBehaviour {
        public string robotIpAddress = "127.0.0.1";
        FRCRobot _robot;
        public double[] currentJointPositions;
        public Vector3 currentPosition;
        public Quaternion currentRotation;
        public bool fanucRobotConnected;

        void Start() {
            ConnectToRobot();
            StartCoroutine(CheckConnectionRoutine());
        }

        void Update() {
            if (_robot != null && _robot.IsConnected) {
                GetCurrentJointPositions();
            }
        }

        void ConnectToRobot() {
            try {
                _robot = new();
                _robot.ConnectEx(robotIpAddress, false, 10, 1);
                Debug.Log("Connected to robot successfully.");
            }
            catch (Exception e) {
                Debug.LogError("Failed to connect to robot: " + e.Message);
            }
        }

        IEnumerator CheckConnectionRoutine() {
            while (true) {
                CheckConnection();
                yield return new WaitForSeconds(0.5f);
            }
        }

        void CheckConnection() {
            if (_robot != null) {
                if (_robot.IsConnected) {
                    fanucRobotConnected = true;
                    //Debug.Log("Robot is connected.");
                }
                else {
                    fanucRobotConnected = false;
                    Debug.LogWarning("Connection to robot lost.");
                }
            }
        }

        void GetCurrentJointPositions() {
            try {
                var curPosition = _robot.CurPosition;
                var groupPositionJoint = curPosition.Group[1, FRECurPositionConstants.frJointDisplayType];
                var groupPositionWorld = curPosition.Group[1, FRECurPositionConstants.frWorldDisplayType];
                groupPositionJoint.Refresh();
                var joint = (FRCJoint)groupPositionJoint.Formats[FRETypeCodeConstants.frJoint];
                var xyzWpr = (FRCXyzWpr)groupPositionWorld.Formats[FRETypeCodeConstants.frXyzWpr];

                for (int i = 1; i < joint.Count + 1; i++) {
                    currentJointPositions[i - 1] = joint[(short)i];
                }

                // Set currentPosition and currentRotation
                currentPosition = new Vector3((float)xyzWpr.X, (float)xyzWpr.Y, (float)xyzWpr.Z);
                currentRotation = Quaternion.Euler((float)xyzWpr.W, (float)xyzWpr.P, (float)xyzWpr.R);
            }
            catch (Exception e) {
                Debug.LogError("Failed: " + e.Message);
            }
        }
    }
}