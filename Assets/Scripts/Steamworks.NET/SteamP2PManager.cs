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
        // steam handles these callbacks
        // this callback is called when the user receives any steam packet
        private Callback<P2PSessionRequest_t> _p2PSessionRequestCallback;
        // this callback is called when the user attempts to join a lobby (which I believe includes from Steam Join)
        private Callback<GameLobbyJoinRequested_t> _gameLobbyJoinRequestedCallback;
        // this call back is called when someone joins the lobby the user is in
        private Callback<LobbyChatUpdate_t> _gameLobbyUpdatedCallback;

        private static CallResult<LobbyCreated_t> callResultMakeLobby = new CallResult<LobbyCreated_t>();
        private static CallResult<LobbyEnter_t> callResultJoinLobby = new CallResult<LobbyEnter_t>();

        public static CSteamID lobbyID { get; private set; }
        
        public static bool joiningLobby { get; private set; } = false;
        public static bool joinedLobby { get; private set; } = false;

        private static Dictionary<CSteamID, string> lobbyMemberSceneDict = new Dictionary<CSteamID, string>();

        private void Start()
        {
            // setup the callback method
            Debug.Log(SteamUser.GetSteamID().m_SteamID);
            _p2PSessionRequestCallback = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
            _gameLobbyJoinRequestedCallback = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequest);
            _gameLobbyUpdatedCallback = Callback<LobbyChatUpdate_t>.Create(OnLobbyUpdate);
            CreateP2PLobby();
        }

        public static void CreateP2PLobby(int lobbySize = 4)
        {
            SteamAPICall_t cb = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, lobbySize);
            callResultMakeLobby.Set(cb, OnCreateP2PLobby);
            joiningLobby = true;
            joinedLobby = false;
        }

        public static void JoinP2PLobby(CSteamID lobbyID)
        {
            SteamAPICall_t cb = SteamMatchmaking.JoinLobby(lobbyID);
            callResultJoinLobby.Set(cb, OnJoinP2PLobby);
            joiningLobby = true;
            joinedLobby = false;
        }

        private static List<CSteamID> GetCurrentLobbyMembers()
        {
            List<CSteamID> lobbymembers = new List<CSteamID>();
            int lobbyMemberCount = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
            for(int i = 0; i < lobbyMemberCount; ++i)
            {
                lobbymembers.Add(SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i));
            }
            return lobbymembers;
        }

        private static void OnCreateP2PLobby(LobbyCreated_t r, bool failure)
        {
            joiningLobby = false;
            if (r.m_eResult == EResult.k_EResultOK && !failure)
            {
                joinedLobby = true;
                Debug.Log("Successfully made lobby of id " + r.m_ulSteamIDLobby.ToString());
                lobbyID = new CSteamID(r.m_ulSteamIDLobby);
                Debug.Log(GetCurrentLobbyMembers().Count.ToString() + " in this Lobby.");
                lobbyMemberSceneDict[SteamUser.GetSteamID()] = SceneManager.GetActiveScene().name;
                Debug.Log(lobbyMemberSceneDict[SteamUser.GetSteamID()]);
                SendSceneNamePacket(lobbyMemberSceneDict[SteamUser.GetSteamID()], SteamUser.GetSteamID());
            }
            else
                Debug.Log("Failed to make lobby.");
        }

        private static void OnJoinP2PLobby(LobbyEnter_t r, bool failure)
        {
            joiningLobby = false;
            if (r.m_EChatRoomEnterResponse == (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess && !failure)
            {
                joinedLobby = true;
                Debug.Log("Successfully joined lobby of id " + r.m_ulSteamIDLobby.ToString());
                lobbyID = new CSteamID(r.m_ulSteamIDLobby);
                Debug.Log(GetCurrentLobbyMembers().Count.ToString() + " in this Lobby.");
            }
            else
                Debug.Log("Failed to join lobby.");
        }

        public static void OnP2PSessionRequest(P2PSessionRequest_t request)
        {
            CSteamID clientId = request.m_steamIDRemote;
            Debug.Log("Request from " + SteamFriends.GetFriendPersonaName(clientId));
            SteamNetworking.AcceptP2PSessionWithUser(clientId);
        }

        public static void OnLobbyJoinRequest(GameLobbyJoinRequested_t request)
        {
            CSteamID lobbyId = request.m_steamIDLobby;
            Debug.Log("Requesting to join lobby of id " + lobbyId.ToString());
            JoinP2PLobby(request.m_steamIDLobby);
        }

        public static void OnLobbyUpdate(LobbyChatUpdate_t request)
        {
            Debug.Log("Something be happening to the lobby.");
            if(request.m_rgfChatMemberStateChange == (uint)EChatMemberStateChange.k_EChatMemberStateChangeEntered)
            {
                Debug.Log("OMG" + request.m_ulSteamIDUserChanged.ToString() + " JOINED!!!!!!");
                SendSceneNamePacket(lobbyMemberSceneDict[SteamUser.GetSteamID()], new CSteamID(request.m_ulSteamIDUserChanged));
            }
        }

        public static void SendPositionPacket(Vector3 pos, CSteamID dest)
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
            SteamNetworking.SendP2PPacket(dest, positionPacket, sizeof(float)*3, EP2PSend.k_EP2PSendReliable);
        }

        public static Vector3 TranslateToPositionPacket(byte[] packet)
        {
            ReadOnlySpan<byte> bytes = new ReadOnlySpan<byte>(packet);
            return new Vector3(BitConverter.ToSingle(bytes.Slice(0, sizeof(float))),
                               BitConverter.ToSingle(bytes.Slice(sizeof(float), sizeof(float))),
                               BitConverter.ToSingle(bytes.Slice(sizeof(float) * 2, sizeof(float))));
        }

        // we'll use this for 2 things: one, to initialize the P2P session, and two, to decide whether or not to send packets
        public static void SendSceneNamePacket(string name, CSteamID dest)
        {
            byte[] sceneIndexPacket = BitConverter.GetBytes(SceneManager.GetSceneByName(name).buildIndex);
            SteamNetworking.SendP2PPacket(dest, sceneIndexPacket, sizeof(int), EP2PSend.k_EP2PSendReliable);
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
