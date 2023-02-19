using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;
using UnityEngine.SceneManagement;

namespace SteamTrain
{
    public class SteamP2PManager : MonoBehaviour
    {
        private Callback<P2PSessionRequest_t> _p2PSessionRequestCallback;

        private void Start()
        {
            // setup the callback method
            Debug.Log(SteamUser.GetSteamID().m_SteamID);
            _p2PSessionRequestCallback = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
            SendPositionPacket(Vector3.one);
            SendSceneNamePacket(SceneManager.GetActiveScene().name);
        }

        private static void OnP2PSessionRequest(P2PSessionRequest_t request)
        {
            CSteamID clientId = request.m_steamIDRemote;
            Debug.Log("Request from " + SteamFriends.GetFriendPersonaName(clientId));
            SteamNetworking.AcceptP2PSessionWithUser(clientId);
        }

        public static void SendPositionPacket(Vector3 pos)
        {
            byte[] positionPacket = new byte[sizeof(float) * 3];
            byte[] xBytes = BitConverter.GetBytes(pos.x);
            byte[] yBytes = BitConverter.GetBytes(pos.y);
            byte[] zBytes = BitConverter.GetBytes(pos.z);
            for(uint i = 0; i < sizeof(float); ++i)
            {
                positionPacket[i] = xBytes[i];
                positionPacket[i + sizeof(float)] = yBytes[i];
                positionPacket[i + (2 * sizeof(float))] = zBytes[i];
            }
            SteamNetworking.SendP2PPacket(SteamUser.GetSteamID(), positionPacket, sizeof(float)*3, EP2PSend.k_EP2PSendReliable);
        }

        public static Vector3 TranslateToPositionPacket(byte[] packet)
        {
            ReadOnlySpan<byte> bytes = new ReadOnlySpan<byte>(packet);
            return new Vector3(BitConverter.ToSingle(bytes.Slice(0, sizeof(float))),
                               BitConverter.ToSingle(bytes.Slice(sizeof(float), sizeof(float))),
                               BitConverter.ToSingle(bytes.Slice(sizeof(float) * 2, sizeof(float))));
        }

        public static void SendSceneNamePacket(string name)
        {
            byte[] sceneIndexPacket = BitConverter.GetBytes(SceneManager.GetSceneByName(name).buildIndex);
            SteamNetworking.SendP2PPacket(SteamUser.GetSteamID(), sceneIndexPacket, sizeof(int), EP2PSend.k_EP2PSendReliable);
        }

        public static string TranslateToSceneNamePacket(byte[] packet)
        {
            return SceneManager.GetSceneByBuildIndex(BitConverter.ToInt32(new ReadOnlySpan<byte>(packet))).name;
        }

        private void Update()
        {
            uint size;

            // repeat while there's a P2P message available
            // will write its size to size variable
            if (SteamNetworking.IsP2PPacketAvailable(out size))
            {
                // allocate buffer and needed variables
                var buffer = new byte[size];
                uint bytesRead;
                CSteamID remoteId;

                // read the message into the buffer
                if (SteamNetworking.ReadP2PPacket(buffer, size, out bytesRead, out remoteId))
                {
                    if(size == sizeof(float)*3)
                        Debug.Log("Received some Vector3 packet that was readable: " + TranslateToPositionPacket(buffer));
                    else if(size == sizeof(int))
                        Debug.Log("Received some scene name packet that was readable: " + TranslateToSceneNamePacket(buffer));
                }
                else
                    Debug.Log("Received some packet that was NOT readable");
            }
        }
    }


}
