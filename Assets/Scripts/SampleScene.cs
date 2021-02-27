//
// Copyright (c) Bytedance Inc 2021. All right reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
using UnityEngine;
using NSM2;
using System;

public class SampleScene : MonoBehaviour, NetworkEventListener
{
    NetworkServiceManager nsm;
	
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start Game!");
        nsm = NetworkServiceManager.Instance;
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
            (var ud, var ms) = loginReq.encode();

            nsm.SendSerializedMsg(AppProtocol.CMD_LOGIN_REQ, ud, AppProtocol.CLIENT_CHANNEL);

            ms.Dispose();
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
