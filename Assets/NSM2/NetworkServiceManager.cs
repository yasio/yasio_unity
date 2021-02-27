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
using System;
using System.Collections.Generic;
using UnityEngine;
using AOT;
using ByteDance.NAPI;
using System.IO;

namespace NSM2
{
    enum NetworkEventType
    {
        CONNECT_SUCCESS,
        CONNECT_FAILED,
        CONNECTION_LOST,
    }

    public interface NetworkEventListener
    {
        void OnConnectSuccess(int channel);
        void OnConnectFailed(int ec, int channel);
        void OnConnectionLost(int ec, int channel);
    }

    public enum NetworkEvent
    {
        CONNECT_SUCCESS = 1,
        CONNECT_FAILED = 2,
        CONNECTION_LOST = 3,
        PACKET = 4,
    }


    public class NetworkServiceManager : Singleton<NetworkServiceManager>
    {
        IntPtr _service = IntPtr.Zero;
        IntPtr[] _sessions = new IntPtr[1];

        private HashSet<NetworkEventListener> eventListeners = new HashSet<NetworkEventListener>();

        private int _maxChannels = 1;

        private INetworkPacketHandler _packeter;

        public override void Dispose()
        {
            Stop();
            base.Dispose();
        }

       /// <param name="maxChannels">支持并发连接数</param>
       /// <param name="packeter">数据包处理器</param>
        public void Start(int maxChannels, INetworkPacketHandler packeter)
        {
            if (isRunning()) return;

            _packeter = packeter;

            _maxChannels = Math.Max(maxChannels, 1);

            _sessions = new IntPtr[_maxChannels];

            YASIO_NI.yasio_init_globals(HandleNativeConsolePrint);

            _service = YASIO_NI.yasio_create_service(_maxChannels, HandleNativeNetworkIoEvent);

            YASIO_NI.yasio_set_print_fn(_service, HandleNativeConsolePrint);

            for(int i = 0; i < _maxChannels; ++i)
                SetOption(YASIO_NI.YEnums.YOPT_C_LFBFD_PARAMS, _packeter.GetOptions(i));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            if (isRunning())
            {
                YASIO_NI.yasio_destroy_service(_service);
                _service = IntPtr.Zero;
            }
        }

        public bool isRunning()
        {
            return _service != IntPtr.Zero;
        }

        /// <summary>
        /// 设置网络选项
        /// </summary>
        /// <param name="opt">
        /// 
        /// </param>
        /// <param name="strParam">use ';' to split parameters, such as "0;ip138.com;80</param>

        public void SetOption(YASIO_NI.YEnums opt, string strParam)
        {
            YASIO_NI.yasio_set_option(_service, (int)opt, strParam);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="channel"></param>
        public void Connect(string address, ushort port, int channel = 0)
        {
            string strParam = string.Format("{0};{1};{2}", channel, address, port);
            SetOption(YASIO_NI.YEnums.YOPT_C_REMOTE_ENDPOINT, strParam);
            Connect(channel);
        }

        public void Disconnect(int channel = 0)
        {
            YASIO_NI.yasio_close(_service, channel);
        }

        /// <summary>
        /// 建立连接
        /// </summary>
        /// <param name="channel">连接信道</param>
        public void Connect(int channel = 0)
        {
            YASIO_NI.yasio_open(_service, channel, (int)YASIO_NI.YEnums.YCK_TCP_CLIENT);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="channel"></param>
        public void ListenAt(string address, ushort port, int channel)
        {
            string strParam = string.Format("{0};{1};{2}", channel, address, port);
            YASIO_NI.yasio_set_option(_service, (int)YASIO_NI.YEnums.YOPT_C_REMOTE_ENDPOINT, strParam);
            ListenAt(channel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        public void ListenAt(int channel)
        {
            YASIO_NI.yasio_open(_service, channel, (int)YASIO_NI.YEnums.YCK_TCP_SERVER);
        }

        /// <summary>
        /// 分派底层网络事件, tick调用
        /// </summary>
        public void OnUpdate()
        {
            YASIO_NI.yasio_dispatch(_service, 128);
        }

        /// <summary>
        /// 添加网络事件监听
        /// </summary>
        /// <param name="listener"></param>
        public void AddEventListener(NetworkEventListener listener)
        {
            if (listener == null) return;

            eventListeners.Add(listener);
        }

        /// <summary>
        /// 移除网络事件监听
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveEventListener(NetworkEventListener listener)
        {
            eventListeners.Remove(listener);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="ec"></param>
        void BroadcastEventToListeners(NetworkEventType eventType, int ec, int channel = 0)
        {
            foreach (var listener in eventListeners)
            {
                switch (eventType)
                {
                    case NetworkEventType.CONNECT_SUCCESS:
                        listener.OnConnectSuccess(channel);
                        break;
                    case NetworkEventType.CONNECT_FAILED:
                        listener.OnConnectFailed(ec, channel);
                        break;
                    case NetworkEventType.CONNECTION_LOST:
                        listener.OnConnectionLost(ec, channel);
                        break;
                }
            }
        }

        unsafe public void SendSerializedMsg(int cmd, NativeDataView ud, int channel = 0)
        {
            // The whole packet is sizeof(body) + sizeof(header), so no memory realloc
            IntPtr pdu = YASIO_NI.yasio_ob_new(ud.len + _packeter.GetHeaderSize());
            
            _packeter.EncodePDU(cmd, ud, pdu);
            if (channel < _sessions.Length)
            {
                IntPtr sid = _sessions[channel];
                if (sid != IntPtr.Zero)
                {
                    YASIO_NI.yasio_write_ob(_service, sid, pdu);
                }
                else
                {
                    Debug.LogFormat("Can't send message to the channel: {0}, the session not ok!", channel);
                }
            }
            else
            {
                Debug.LogFormat("Can't send message to the channel: {0}, the index is overflow, max allow value is:{1}", channel,
                    _sessions.Length);
            }

            YASIO_NI.yasio_ob_release(pdu);
        }

        /// <summary>
        /// 向远端发送数据
        /// </summary>
        /// <param name="bytes"></param>
        public void SendRaw(byte[] buffer, int length, int channel)
        {
            if (channel < _sessions.Length)
            {
                IntPtr sid = _sessions[channel];
                if (sid != IntPtr.Zero)
                {
                    YASIO_NI.yasio_write(_service, sid, buffer, length);
                }
                else
                {
                    Debug.LogFormat("Can't send message to the channel: {0}, the session not ok!", channel);
                }
            }
            else
            {
                Debug.LogFormat("Can't send message to the channel: {0}, the index is overflow, max allow value is:{1}", channel,
                    _sessions.Length);
            }
        }

        /// <summary>
        /// 更新连接会话
        /// </summary>
        /// <param name="cidx"></param>
        /// <param name="sid"></param>
        void UpdateSession(int channel, IntPtr sid)
        {
            if (channel < _sessions.Length)
                _sessions[channel] = sid;
        }

        /// <summary>
        /// 获取链接状态的tcp.rtt, 如果未连接，则返回0
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        uint GetRTT(int channel = 0)
        {
            if (channel < _sessions.Length)
                return YASIO_NI.yasio_tcp_rtt(_sessions[channel]);
            return 0;
        }

        [MonoPInvokeCallback(typeof(YASIO_NI.YNIPrintDelegate))]
        static void HandleNativeConsolePrint(int level, string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        /// <summary>
        /// 处理底层网络事件
        /// </summary>
        /// <param name="emask"></param>
        /// <param name="sid"></param>
        /// <param name="bytes"></param>
        /// <param name="len"></param>
        [MonoPInvokeCallback(typeof(YASIO_NI.YNIEventDelegate))]
        static void HandleNativeNetworkIoEvent(int kind, int status, int channel, IntPtr sid, IntPtr bytes, int len)
        {
            var nsm = NetworkServiceManager.Instance;
            switch ((YASIO_NI.YEnums)kind)
            {
                case YASIO_NI.YEnums.YEK_PACKET:
                    (int cmd, NativeDataView ud, Stream hold) = nsm._packeter.DecodePDU(bytes, len);
                    nsm._packeter.HandleEvent(NetworkEvent.PACKET, cmd, ud, channel);
                    hold?.Dispose();
                    break;
                case YASIO_NI.YEnums.YEK_CONNECT_RESPONSE:
                    if (status == 0)
                    {
                        nsm.UpdateSession(channel, sid);
                        nsm.BroadcastEventToListeners(NetworkEventType.CONNECT_SUCCESS, 0, channel);
                        nsm._packeter.HandleEvent(NetworkEvent.CONNECT_SUCCESS, -1, NativeDataView.NullValue, channel);
                        Debug.LogFormat("[channel:#{0}] Connect succeed, ec={1}!", channel, status);
                    }
                    else
                    {
                        nsm.BroadcastEventToListeners(NetworkEventType.CONNECT_FAILED, status, channel);
                        nsm._packeter.HandleEvent(NetworkEvent.CONNECT_FAILED, -1, NativeDataView.NullValue, channel);
                        Debug.LogWarningFormat("[channel:#{0}] Connect failed, ec={1}!", channel, status);
                    }
                    break;
                case YASIO_NI.YEnums.YEK_CONNECTION_LOST:
                    nsm.BroadcastEventToListeners(NetworkEventType.CONNECTION_LOST, status, channel);
                    nsm._packeter.HandleEvent(NetworkEvent.CONNECTION_LOST, -1, NativeDataView.NullValue, channel);
                    Debug.LogWarningFormat("[channel:#{0}] The connection of session #{1} is lost, ec={2}!", channel, sid, status);
                    break;
            }
        }
    }
}
