//////////////////////////////////////////////////////////////////////////////////////////
// A multi-platform support c++11 library with focus on asynchronous socket I/O for any
// client application.
//////////////////////////////////////////////////////////////////////////////////////////
/*
The MIT License (MIT)

Copyright (c) 2012-2024 HALX99

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using UnityEngine;
using NSM2;
using System;

public class SampleScene : MonoBehaviour, NetworkEventListener
{
    NetworkManager nsm;
	
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start Game!");
        nsm = NetworkManager.Instance;
        nsm.Start(AppProtocol.MAX_CHANNELS, new SampleNetworkPacketHandler());
        nsm.ListenAt("127.0.0.1", AppProtocol.PORT, AppProtocol.SERVER_CHANNEL); // 启动本地TCP服务
        nsm.AddEventListener(this);
    }

    // Update is called once per frame
    void Update()
    {
        nsm.OnUpdate();
    }

    private void OnDestroy()
    {
        nsm.RemoveEventListener(this);
        nsm.Dispose();
        nsm = null;
        Debug.Log("Stop Game!");
    }

    public void StartConnect()
    {
        Debug.Log("Start Connect...");
        nsm.Connect("127.0.0.1", AppProtocol.PORT, AppProtocol.CLIENT_CHANNEL);
    }

    unsafe public void OnConnectSuccess(int channel = 0)
    {
        if (channel == AppProtocol.CLIENT_CHANNEL)
        {
            Debug.LogFormat("Connect success, channel={0}", channel);

            // 模拟登录协议
            var loginReq = new AppProtocol.LoginReq();
            loginReq.uid = 1219;
            Span<byte> ud = loginReq.encode();

            nsm.SendSerializedMsg(AppProtocol.CMD_LOGIN_REQ, ud, AppProtocol.CLIENT_CHANNEL);

            loginReq.Dispose();
        }
        else if(channel == AppProtocol.SERVER_CHANNEL)
        {
            Debug.LogFormat("A client income, channel={0}", channel);
        }
    }

    public void OnConnectFailed(int ec, int channel)
    {
        Debug.LogWarningFormat("Connect fail, channel={0}, ec={1}", ec, channel);
    }

    public void OnConnectionLost(int ec, int channel)
    {
        Debug.LogWarningFormat("Connection lost, channel={0}, ec={1}", ec, channel);
    }

}
