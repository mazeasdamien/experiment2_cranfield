using Rti.Dds.Publication;
using Rti.Dds.Topics;
using System.Collections;
using UnityEngine;

namespace Telexistence 
{
    public class RobotStatePublisher : MonoBehaviour {
        private FanucHandler fanucHandler;
        public DDSHandler DDSHandler;
        private DataWriter<RobotState> robotStateWriter;
        public int dataWriteCount = 0;

        private RobotState previousRobotState;

        private void Awake() {
            fanucHandler = gameObject.GetComponent<FanucHandler>();
        }

        void Start() {
            if (InitializeRobotStateType()) Debug.Log("Robot State Publisher is initialized - OK!");
            StartCoroutine(UpdateRobotStateCoroutine());
        }

        private bool InitializeRobotStateType() {
            if (DDSHandler.participant == null)
            {
                Debug.LogError("Participant is null. Please check the participant initialization.");
                return false;
            }

            Topic<RobotState> topic = DDSHandler.participant.CreateTopic<RobotState>("RobotStateTopic");
            if (topic == null)
            {
                Debug.LogError($"Failed to create topic: RobotStateTopic");
                return false;
            }

            Publisher publisher = DDSHandler.participant.CreatePublisher(DDSHandler.cachedPublisherQos);
            robotStateWriter = publisher.CreateDataWriter(topic, DDSHandler.cachedWriterQos);
            return true;
        }

        private IEnumerator UpdateRobotStateCoroutine() {
            while (true) {
                UpdateRobotState();
                yield return new WaitForSeconds(0.02f);
            }
        }

        private void UpdateRobotState()
        {
            if (fanucHandler == null || !fanucHandler.fanucRobotConnected ||
                fanucHandler.currentJointPositions == null || fanucHandler.currentPosition == null || fanucHandler.currentRotation == null)
            {
                return;
            }

            RobotState currentRobotState = new RobotState(
                fanucHandler.currentJointPositions[0], fanucHandler.currentJointPositions[1], fanucHandler.currentJointPositions[2],
                fanucHandler.currentJointPositions[3], fanucHandler.currentJointPositions[4], fanucHandler.currentJointPositions[5],
                fanucHandler.currentPosition.x, fanucHandler.currentPosition.y, fanucHandler.currentPosition.z,
                fanucHandler.currentRotation.x, fanucHandler.currentRotation.y, fanucHandler.currentRotation.z
            );

            // Only send the robot state if it has changed
            if (previousRobotState == null || !currentRobotState.Equals(previousRobotState))
            {
                SendRobotState(currentRobotState);
                previousRobotState = currentRobotState;
            }
        }
        private void SendRobotState(RobotState robotState)
        {
            // Publish the RobotState
            robotStateWriter.Write(robotState);
            dataWriteCount++;
        }

        private void OnApplicationQuit()
        {
            if (robotStateWriter != null)
            {
                Publisher publisher = robotStateWriter.Publisher;
                robotStateWriter.Dispose();
                robotStateWriter = null;

                if (publisher != null)
                {
                    publisher.Dispose();
                    publisher = null;
                }
            }
        }
    }
}