using System.Collections;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;

namespace SimplePlatformer.Assets.Scripts.Managers
{

    public class StartNetwork : MonoBehaviour
    {
        private UnityTransport UnityTransport;
        public string ip;
        public ushort port;

        public TMP_InputField IpField;
        public TMP_InputField PortField;
        public bool enableAutoStart = false;

        // Start is called before the first frame update
        void Start()
        {
            UnityTransport = GetComponent<UnityTransport>();
            if (Debug.isDebugBuild && enableAutoStart)
            {
                Debug.Log("The game is running in debug mode.");
                Host();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SaveIpDirection()
        {
            this.ip = IpField.text;
            this.port = ushort.Parse(PortField.text);
            InitializeTransport();
        }

        public void InitializeTransport()
        {
            UnityTransport.SetConnectionData(ip, this.port);
        }

        public void Join()
        {
            if (UnityTransport != null)
            {
                InitializeTransport();

                // Set the IP address for the client to connect to
                UnityTransport.SetConnectionData(this.ip, 7777); // Port should match the server's port
                                                                 // Start the client
                NetworkManager.Singleton.StartClient();
            }
            else
            {
                Debug.LogError("UnityTransport component not found on NetworkManager.");
            }

            NetworkManager.Singleton.StartClient();
        }

        public void Host()
        {
            NetworkManager.Singleton.StartHost();
        }
    }

}