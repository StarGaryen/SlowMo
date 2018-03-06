using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;
public interface MatchJoiner
{
    void connect(NetworkManager networkManager);
    string GetName();
    int GetSize();
    int GetPlayerCount();
}
