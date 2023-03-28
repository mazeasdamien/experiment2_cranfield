using Rti.Dds.Publication;
using Rti.Dds.Topics;
using Rti.Types.Dynamic;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Omg.Types;

namespace Telexistence {
    public class MeshPublisher : MonoBehaviour {
        public DDSHandler DDSHandler;
        private DataWriter<MeshData> meshDataWriter;
        public int dataWriteCount = 0;

        private void Start()
        {
            if (InitializeMeshDataType()) Debug.Log("Mesh Publisher is initialized - OK!");
        }

        private bool InitializeMeshDataType() {
            if (DDSHandler.participant == null)
            {
                Debug.LogError("Participant is null. Please check the participant initialization.");
                return false;
            }

            Topic<MeshData> topic = DDSHandler.participant.CreateTopic<MeshData>("MeshDataTopic");
            if (topic == null)
            {
                Debug.LogError($"Failed to create topic: MeshDataTopic");
                return false;
            }

            Publisher publisher = DDSHandler.participant.CreatePublisher(DDSHandler.cachedPublisherQos);
            meshDataWriter = publisher.CreateDataWriter(topic, DDSHandler.cachedWriterQos);
            return true;
        }

        public void SendMeshData(Mesh mesh, Texture2D texture)
        {
            // Convert Unity Mesh to MeshData
            ISequence<Vector3DDS> vertices = new Rti.Types.Sequence<Vector3DDS>(mesh.vertexCount);
            foreach (Vector3 vertex in mesh.vertices)
            {
                vertices.Add(new Vector3DDS { x = vertex.x, y = vertex.y, z = vertex.z });
            }

            ISequence<int> triangles = new Rti.Types.Sequence<int>(mesh.triangles.Length);
            foreach (int triangle in mesh.triangles)
            {
                triangles.Add(triangle);
            }

            // Convert Unity Texture2D to byte array
            byte[] textureBytes = texture.EncodeToPNG();
            ISequence<byte> textureBytesSequence = new Rti.Types.Sequence<byte>(textureBytes.Length);
            foreach (byte textureByte in textureBytes)
            {
                textureBytesSequence.Add(textureByte);
            }

            // Create MeshData object with populated sequences
            MeshData meshData = new MeshData(vertices, triangles, textureBytesSequence);
        
            // Publish the MeshData
            meshDataWriter.Write(meshData);
            dataWriteCount++;
        }


        private void OnApplicationQuit()
        {
            if (meshDataWriter != null)
            {
                Publisher publisher = meshDataWriter.Publisher;
                meshDataWriter.Dispose();
                meshDataWriter = null;

                if (publisher != null)
                {
                    publisher.Dispose();
                    publisher = null;
                }
            }
        }

    }
}