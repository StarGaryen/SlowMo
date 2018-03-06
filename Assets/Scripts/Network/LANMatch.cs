using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public  class LANMatch : MatchJoiner,MatchCreator  {

    public string address;
    public int port;
    private string name;
    private int roomSize;
    private int playerCount;
    private OverrideNeworkDiscovery networkDiscovery;

    #region Private Construction Methods

    private LANMatch(string fromAddress, string data, out bool validData)
    {
        address = fromAddress.Substring(7);
        port = 7777;
        name = "";
        roomSize = 0;
        playerCount = 0;

        string[] matchdata = new string[3];
        int currData = 0;
        foreach (char x in data)
        {
            if (x == ':')
            {
                currData++;
                if (currData > 2)
                {
                    Debug.Log("Currupt data recieved from broadcast");
                    validData = false;
                    return;
                }

            }
            else
            {
                matchdata[currData] += x;
            }

        }

       
        name = matchdata[1];
        roomSize = (int)Util.StringToUint(matchdata[2]);
        playerCount = (int)Util.StringToUint(matchdata[0]);
        validData = true;
        Debug.Log("Valid Match Revieved address = " + address + " name = " + name);
        return;
    }

    private LANMatch(OverrideNeworkDiscovery _networkDiscovery)
    {
        address = "localhost";
        port = 7777;
        name = "Default";
        roomSize = 6;
        playerCount = 1;
        networkDiscovery = _networkDiscovery;
    }

    #endregion

    #region Public Construction Methods

    public static MatchCreator GetMatchCreator(OverrideNeworkDiscovery netDiscovery)
    {
        MatchCreator creator = new LANMatch(netDiscovery);

        return creator;
    }

    public static MatchJoiner GetMatchJoiner(string fromAddress,string data,out bool validData)
    {
        MatchJoiner joiner = new LANMatch(fromAddress,data,out validData);

        return joiner;
    }

    #endregion

    #region MatchJoiner methods
    public void connect(NetworkManager networkManager)
    {
        if (CleanForLocalConnection(networkManager))
        {
            networkManager.networkAddress = this.address;
            networkManager.networkPort = (int)this.port;
            networkManager.StartClient();
        }
        else
        {
            Debug.Log("Not ready to connect");
        }
        
    }

    public string GetName()
    {
        return name;
    }

    public int GetSize()
    {
        return roomSize;
    }

    public int GetPlayerCount()
    {
        return playerCount;
    }

    #endregion

    #region MatchCretor Methods

    public void CreateRoom(NetworkManager networkManager,string _name, int _size)
    {
        networkManager.matchName = this.name = _name;
        networkManager.matchSize = (uint)_size;
        this.roomSize = _size;
        networkManager.networkPort = this.port;
        if(CleanForLocalConnection(networkManager))
            networkManager.StartHost();
        networkDiscovery.StartBroadcasting(this);
    }

    public void setCurrentPlayerCount(int _count)
    {
        playerCount = _count;
    }

    #endregion

    #region Utility Methods

    private bool CleanForLocalConnection(NetworkManager networkManager)
    {
        if (networkManager.matchMaker != null)
            networkManager.StopMatchMaker();
        return (!NetworkClient.active && !NetworkServer.active);
    }

    public string GetDataString()
    {
        string data = "";
        //add playercount
        data = this.playerCount + ":";
        //add server name
        data = data + this.name + ":";
        //add roomsize
        data = data + this.roomSize;
        Debug.Log("Compressed string :: " + data);
        return data;
    }

    #endregion
}

