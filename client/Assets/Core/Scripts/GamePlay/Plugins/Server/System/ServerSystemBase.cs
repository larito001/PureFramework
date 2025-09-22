using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ServerSystemBase
{
    protected DemoGameServer _server;

    public  void Init(DemoGameServer server)
    {
        _server = server;
    }
    public abstract void AddEvent();
    public abstract void RemoveEvent();

}
