using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetworking : MonoBehaviour
{
    [SerializeField]
    private Transform playerDummyPrefab;

    private static Dictionary<Steamworks.CSteamID, Rigidbody> dummyDict = new Dictionary<Steamworks.CSteamID, Rigidbody>();
    
    private void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        SteamTrain.SteamP2PManager.Init();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SteamTrain.SteamP2PManager.HandlePackets();
        if (SteamTrain.SteamP2PManager.joinedLobby && Player.Instance != null)
        {
            int currScene = SceneManager.GetActiveScene().buildIndex;
            SteamTrain.SteamP2PManager.BroadcastPositionToLobby(Player.Instance.transform.position);
            // angrily ping my position
            
            // synchronize positions and spawn players if needed
            foreach(var lobbyMembers in SteamTrain.SteamP2PManager.lobbyMemberSceneDict)
            {
                Debug.Log("Replicating user: " + lobbyMembers.Key.m_SteamID.ToString());
                if (lobbyMembers.Value != currScene || lobbyMembers.Key == Steamworks.SteamUser.GetSteamID())
                    continue;
                if(!dummyDict.ContainsKey(lobbyMembers.Key))
                {
                    Debug.Log("A new player has been made.");
                    dummyDict[lobbyMembers.Key] = Instantiate(playerDummyPrefab.gameObject).GetComponent<Rigidbody>();
                    dummyDict[lobbyMembers.Key].GetComponent<PlayerDummyName>().SetText(
                                                Steamworks.SteamFriends.GetFriendPersonaName(lobbyMembers.Key));
                }
                if(SteamTrain.SteamP2PManager.lobbyMemberLastPosDict.ContainsKey(lobbyMembers.Key))
                    dummyDict[lobbyMembers.Key].MovePosition(SteamTrain.SteamP2PManager.lobbyMemberLastPosDict[lobbyMembers.Key]);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Additive)
            return;

        dummyDict.Clear();
        SteamTrain.SteamP2PManager.BroadcastCurrentSceneToLobby();
    }
}