using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;
using UnityEngine.SceneManagement;

namespace SteamTrain
{
    public class SteamP2PManager
    {
        // steam handles these callbacks
        // this callback is called when the user receives any steam packet
        private static Callback<P2PSessionRequest_t> _p2PSessionRequestCallback;
        // this callback is called when the user attempts to join a lobby (which I believe includes from Steam Join)
        private static Callback<GameLobbyJoinRequested_t> _gameLobbyJoinRequestedCallback;
        // this call back is called when someone joins the lobby the user is in
        private static Callback<LobbyChatUpdate_t> _gameLobbyUpdatedCallback;

        private static CallResult<LobbyCreated_t> callResultMakeLobby = new CallResult<LobbyCreated_t>();
        private static CallResult<LobbyEnter_t> callResultJoinLobby = new CallResult<LobbyEnter_t>();

        public static CSteamID lobbyID { get; private set; }
        
        public static bool joiningLobby { get; private set; } = false;
        public static bool joinedLobby { get; private set; } = false;

        public static Dictionary<CSteamID, int> lobbyMemberSceneDict = new Dictionary<CSteamID, int>();
        public static Dictionary<CSteamID, Vector3> lobbyMemberLastPosDict = new Dictionary<CSteamID, Vector3>();

        // For multiplayer
        public static Action<string> OnPlayerJoinedGame = delegate { }; // playerName
        public static Action<string> OnPlayerLeftGame = delegate { }; // playerName

        public enum PacketID
        {
            Pos = 0,
            SceneIndex
        }

        public static void Init()
        {
            // setup the callback method
            Debug.Log("Networking Initializing");
            _p2PSessionRequestCallback = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
            _gameLobbyJoinRequestedCallback = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequest);
            _gameLobbyUpdatedCallback = Callback<LobbyChatUpdate_t>.Create(OnLobbyUpdate);
            CreateP2PLobby();
        }

        // only broadcast to users in the same scene
        public static void BroadcastPositionToLobby(Vector3 pos)
        {
            Debug.Log("Broadcast position.");
            foreach (var dest in lobbyMemberSceneDict)
                if(dest.Key != SteamUser.GetSteamID() &&
                    dest.Value == lobbyMemberSceneDict[SteamUser.GetSteamID()])
                    SendPositionPacket(pos, dest.Key);
        }

        // call this on scene change once
        public static void BroadcastCurrentSceneToLobby()
        {
            Debug.Log("Broadcast scene name.");
            foreach (var dest in lobbyMemberSceneDict)
              SendSceneNamePacket(SceneManager.GetActiveScene().name, dest.Key);
        }

        public static void CreateP2PLobby(int lobbySize = 4)
        {
            SteamAPICall_t cb = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, lobbySize);
            callResultMakeLobby.Set(cb, OnCreateP2PLobby);
            joiningLobby = true;
            joinedLobby = false;
        }

        private static void JoinP2PLobby(CSteamID lobbyID)
        {
            SteamAPICall_t cb = SteamMatchmaking.JoinLobby(lobbyID);
            callResultJoinLobby.Set(cb, OnJoinP2PLobby);
            joiningLobby = true;
            joinedLobby = false;
        }

        public static List<CSteamID> GetCurrentLobbyMembers()
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
                Debug.Log("Successfully made lobby of id " + r.m_ulSteamIDLobby.ToString());
                lobbyID = new CSteamID(r.m_ulSteamIDLobby);
                Debug.Log(GetCurrentLobbyMembers().Count.ToString() + " in this Lobby.");
                lobbyMemberSceneDict[SteamUser.GetSteamID()] = SceneManager.GetActiveScene().buildIndex;
                Debug.Log(lobbyMemberSceneDict[SteamUser.GetSteamID()]);
                SendSceneNamePacket(SceneManager.GetActiveScene().name, SteamUser.GetSteamID());
                joinedLobby = true;
            }
            else
                Debug.Log("Failed to make lobby.");
        }

        private static void OnJoinP2PLobby(LobbyEnter_t r, bool failure)
        {
            joiningLobby = false;
            if (r.m_EChatRoomEnterResponse == (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess && !failure)
            {
                Debug.Log("Successfully joined lobby of id " + r.m_ulSteamIDLobby.ToString());
                lobbyID = new CSteamID(r.m_ulSteamIDLobby);
                Debug.Log(GetCurrentLobbyMembers().Count.ToString() + " in this Lobby.");
                joinedLobby = true;
            }
            else
                Debug.Log("Failed to join lobby.");
        }

        private static void OnP2PSessionRequest(P2PSessionRequest_t request)
        {
            CSteamID clientId = request.m_steamIDRemote;
            Debug.Log("Request from " + SteamFriends.GetFriendPersonaName(clientId));
            SteamNetworking.AcceptP2PSessionWithUser(clientId);
        }

        private static void OnLobbyJoinRequest(GameLobbyJoinRequested_t request)
        {
            CSteamID lobbyId = request.m_steamIDLobby;
            Debug.Log("Requesting to join lobby of id " + lobbyId.ToString());
            JoinP2PLobby(request.m_steamIDLobby);
        }

        private static void OnLobbyUpdate(LobbyChatUpdate_t request)
        {
            Debug.Log("Something be happening to the lobby.");
            if(request.m_rgfChatMemberStateChange == (uint)EChatMemberStateChange.k_EChatMemberStateChangeEntered)
            {
                Debug.Log("OMG " + request.m_ulSteamIDUserChanged.ToString() + " JOINED!!!!!!");
                CSteamID newguy = new CSteamID(request.m_ulSteamIDUserChanged);
                SendSceneNamePacket(SceneManager.GetActiveScene().name, newguy);
                OnPlayerJoinedGame.Invoke(SteamFriends.GetFriendPersonaName(newguy));
            }
            else if(request.m_rgfChatMemberStateChange == (uint)EChatMemberStateChange.k_EChatMemberStateChangeLeft)
            {
                Debug.Log("BRUH, " + request.m_ulSteamIDUserChanged.ToString() + " LEFT!!!!!!");
                CSteamID newguy = new CSteamID(request.m_ulSteamIDUserChanged);
                SendSceneNamePacket(SceneManager.GetActiveScene().name, newguy);
                OnPlayerLeftGame.Invoke(SteamFriends.GetFriendPersonaName(newguy));
            }
        }

        private static byte[] AddPacketHeader(PacketID pid, byte[] packet)
        {
            byte[] headeredPacket = new byte[sizeof(uint) + packet.Length];
            // packet id
            byte[] packetid = BitConverter.GetBytes((uint)pid);
            for (uint i = 0; i < sizeof(uint); ++i)
                headeredPacket[i] = packetid[i];
            // body
            for (uint i = sizeof(uint); i < sizeof(uint) + packet.Length; ++i)
                headeredPacket[i] = packet[i - sizeof(uint)];
            return headeredPacket;
        }

        private static byte[] SendPacketWithHeader(CSteamID dest, PacketID pid, byte[] packet, EP2PSend r)
        {
            byte[] toSend = AddPacketHeader(pid, packet);
            SteamNetworking.SendP2PPacket(dest, toSend, (uint)toSend.Length, r);
            return toSend;
        }

        public static void SendPositionPacket(Vector3 pos, CSteamID dest)
        {
            byte[] positionPacket = new byte[(sizeof(float)*3)];
            byte[] xBytes = BitConverter.GetBytes(pos.x);
            byte[] yBytes = BitConverter.GetBytes(pos.y);
            byte[] zBytes = BitConverter.GetBytes(pos.z);
            for (uint i = 0; i < sizeof(float); ++i)
            {
                positionPacket[i] = xBytes[i];
                positionPacket[i + sizeof(float)] = yBytes[i];
                positionPacket[i + (2 * sizeof(float))] = zBytes[i];
            }
            SendPacketWithHeader(dest, PacketID.Pos, 
                                 positionPacket, 
                                 EP2PSend.k_EP2PSendUnreliableNoDelay);
        }

        // we'll use this for 2 things: one, to initialize the P2P session, and two, to decide whether or not to send packets
        public static void SendSceneNamePacket(string name, CSteamID dest)
        {
            SendPacketWithHeader(dest, PacketID.SceneIndex,
                                 BitConverter.GetBytes(SceneManager.GetSceneByName(name).buildIndex),
                                 EP2PSend.k_EP2PSendReliable);
        }

        private static Vector3 TranslateToPositionPacket(byte[] packet)
        {
            ReadOnlySpan<byte> bytes = new ReadOnlySpan<byte>(packet);
            return new Vector3(BitConverter.ToSingle(bytes.Slice(sizeof(uint), sizeof(float))),
                               BitConverter.ToSingle(bytes.Slice(sizeof(float)+sizeof(uint), sizeof(float))),
                               BitConverter.ToSingle(bytes.Slice(sizeof(uint)+ (sizeof(float) * 2), sizeof(float))));
        }

        private static string TranslateToSceneNamePacket(byte[] packet)
        {
            ReadOnlySpan<byte> bytes = new ReadOnlySpan<byte>(packet);
            return SceneManager.GetSceneByBuildIndex(BitConverter.ToInt32(bytes.Slice(sizeof(uint), sizeof(int)))).name;
        }

        private static int TranslateToSceneIntPacket(byte[] packet)
        {
            ReadOnlySpan<byte> bytes = new ReadOnlySpan<byte>(packet);
            return BitConverter.ToInt32(bytes.Slice(sizeof(uint), sizeof(int)));
        }

        public static void HandlePackets()
        {
            uint size;

            while (SteamNetworking.IsP2PPacketAvailable(out size))
            {
                // allocate buffer and needed variables
                var buffer = new byte[size];
                uint bytesRead;
                CSteamID remoteId;

                // read the message into the buffer
                if (SteamNetworking.ReadP2PPacket(buffer, size, out bytesRead, out remoteId))
                {
                    ReadOnlySpan<byte> bytes = new ReadOnlySpan<byte>(buffer);
                    PacketID pid = (PacketID)BitConverter.ToUInt32(bytes.Slice(0, sizeof(uint)));
                    switch(pid)
                    {
                        case PacketID.Pos:
                            lobbyMemberLastPosDict[remoteId] = TranslateToPositionPacket(buffer);
                            Debug.Log("Packet from " + remoteId.m_SteamID.ToString() + 
                                        ": Position " + lobbyMemberLastPosDict[remoteId]);
                            break;
                        case PacketID.SceneIndex:
                            lobbyMemberSceneDict[remoteId] = TranslateToSceneIntPacket(buffer);
                            Debug.Log("Packet from " + remoteId.m_SteamID.ToString() + 
                                        ": Scene " + lobbyMemberSceneDict[remoteId]);
                            break;
                    }
                }
                else
                    Debug.Log("Received some packet that was NOT readable");
            }
        }
    }


}
