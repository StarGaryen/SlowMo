using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HostGame : MonoBehaviour {
    [Header("Room Settings")]   
    [SerializeField]
    private int roomSize = 6;
    private string roomName;

    [Header("UI")]
    [SerializeField]
    private GameObject WaitScreen;


    //Bose of all
    private NetworkManager networkManager;
    private OverrideNeworkDiscovery networkDiscovery;
    
    
    private MatchCreator match;

    /// <summary>
    ///  Stores name fron UI Feild.This fuction is called by UI event.
    /// </summary>
    /// <param name="_name"> name od room</param>
    public void SetRoomName( string _name)
    {
        roomName = _name;
    }

    /// <summary>
    /// Create object of LANMatch/OnlineMatch. This function is called by UI Event.
    /// </summary>
    public void SetServerTypeLocal(bool isLocalServer)
    {
        if (isLocalServer)
        {
            match = LANMatch.GetMatchCreator(networkDiscovery);
                    
        }
        else
        {
            match = OnlineMatch.GetMatchCreator();
        }
    }

 
    /// <summary>
    /// save Referances to networkManager and networkDiscovery
    /// </summary>
    public void Start()
    {
        Debug.Log("saving referance to NetworkManager");
        networkManager = NetworkManager.singleton;
        networkDiscovery = OverrideNeworkDiscovery.singleton;
        if (networkManager == null)
        {
            Debug.Log("NetworkManager is null");
        }
        if (networkDiscovery== null)
        {
            Debug.Log("NetworkDiscovery is null");
        }
    }


    /// <summary>
    ///  start RoomCreator and WaitScreen manager
    /// </summary>
    public void CreateRoom()
    {
        match.CreateRoom(networkManager,roomName,roomSize);
        StartCoroutine(WaitForHostToStart());        
    }
    
   /// <summary>
   /// wait for connection if timeout then remove the waitscreen
   /// </summary>
   /// <returns></returns>
    IEnumerator WaitForHostToStart()
    {
        WaitScreen.SetActive(true);

        int countdown = 8;
        while(countdown > 0)
        {
            yield return new WaitForSeconds(1f);
            countdown--;
        }


        networkManager.StopHost();

        if (networkDiscovery.isServer)
            networkDiscovery.StopBroadcast();
        
        WaitScreen.SetActive(false);
    }

}
