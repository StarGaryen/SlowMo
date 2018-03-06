using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;
public interface MatchCreator
{
    void CreateRoom(NetworkManager networkManager,string roomName,int size);
    void setCurrentPlayerCount(int _count);
}
