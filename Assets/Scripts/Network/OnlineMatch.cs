using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class OnlineMatch : MatchJoiner,MatchCreator {

    private MatchInfoSnapshot matchInfo;

    private string roomName;
    private int roomSize;
    private int playerCount;

    #region Private Constructors

    private OnlineMatch(MatchInfoSnapshot _matchInfo)
    {
        roomName = "default";
        roomSize = 10;
        playerCount = 0;
        matchInfo = _matchInfo;
    }

    private OnlineMatch()
    {
        roomName = "default";
        roomSize = 10;
        playerCount = 0;
        matchInfo = null;
    }

    #endregion

    #region Public Constructors

    public static MatchCreator GetMatchCreator()
    {
        return new OnlineMatch();
    }

    public static MatchJoiner GetMatchJoiner(MatchInfoSnapshot _matchInfo)
    {
        return new OnlineMatch(_matchInfo);
    }

    #endregion

    #region MatchJoiner Methods
    public void connect(NetworkManager networkManager)
    {
        networkManager.matchMaker.JoinMatch(matchInfo.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
    }

    public string GetName()
    {
        return matchInfo.name;
    }

    public int GetPlayerCount()
    {
        return matchInfo.currentSize;
    }

    public int GetSize()
    {
        return matchInfo.maxSize;
    }
    #endregion

    #region MatchCreator  Methods

    public void setCurrentPlayerCount(int _count)
    {
        if(_count > 0)
        {
            playerCount = _count;
        }
    }

    public void CreateRoom(NetworkManager networkManager,string _roomName,int _roomSize)
    {
        roomSize = _roomSize;
        setName( _roomName);
        Debug.Log("Creating room : " + roomName + " for " + roomSize + " players.");

        if (networkManager.matchMaker == null)
            networkManager.StartMatchMaker();
        networkManager.matchMaker.CreateMatch(roomName, (uint)roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
    }

    #endregion

    private void setName(string _name)
    {
        if (_name != null && _name != "")
        {
            roomName = _name;
        }
        else
        {
            roomName = "default";
        }        
    }
}
