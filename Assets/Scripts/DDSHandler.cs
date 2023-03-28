using Rti.Dds.Core;
using Rti.Dds.Domain;
using Rti.Dds.Publication;
using Rti.Dds.Subscription;
using Rti.Dds.Topics;
using Rti.Types.Dynamic;
using Rti.Utility;
using System;
using Unity.VisualScripting;
using UnityEngine;
using static Rti.Utility.HeapMonitor;

namespace Telexistence
{
    public class DDSHandler : MonoBehaviour
    {
        [SerializeField]
        private string qosXmlFile = "TX.xml";
        [SerializeField]
        private string qosProfile = "QosTX::QosTXprofile";

        private static DDSHandler _instance;
        public static DDSHandler Instance => _instance;

        private QosProvider provider;
        [HideInInspector]
        public DomainParticipant participant;
        [HideInInspector]
        public DataReaderQos cachedReaderQos;
        [HideInInspector]
        public DataWriterQos cachedWriterQos;
        [HideInInspector]
        public SubscriberQos cachedSubscriberQos;
        [HideInInspector]
        public PublisherQos cachedPublisherQos;
        [HideInInspector]

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            try
            {
                provider = new QosProvider(qosXmlFile);
                Debug.Log("QosProvider created successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to create QosProvider: " + ex.Message);
                return;
            }

            try
            {
                participant = DomainParticipantFactory.Instance.CreateParticipant(0, provider.GetDomainParticipantQos(qosProfile));
                Debug.Log("DomainParticipant created successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to create DomainParticipant: " + ex.Message);
                return;
            }

            try
            {
                cachedSubscriberQos = provider.GetSubscriberQos(qosProfile);
                cachedPublisherQos = provider.GetPublisherQos(qosProfile);
                cachedReaderQos = provider.GetDataReaderQos(qosProfile);
                cachedWriterQos = provider.GetDataWriterQos(qosProfile);
                Debug.Log("QoS profiles loaded successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to get QoS profiles: " + ex.Message);
                return;
            }
        }

        private void OnDestroy()
        {
            DisposeParticipant();
        }

        private void OnApplicationQuit()
        {
            DisposeParticipant();
        }

        private void DisposeParticipant()
        {
            if (participant != null)
            {
                participant.Dispose();
                participant = null;
            }
        }
    }
}