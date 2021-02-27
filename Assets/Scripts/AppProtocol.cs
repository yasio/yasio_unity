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
using ByteDance.NAPI;
using System;
using System.IO;
using System.Net;

namespace NSM2
{
    public class AppProtocol
    {
        public const int PORT = 12191;

        #region 示例消息定义
        public const int MAX_CHANNELS = 2; // 网络服务支持信道最大数

        public const int CLIENT_CHANNEL = 0; // 客户端信道索引
        public const int SERVER_CHANNEL = 1; // 服务器信道索引

        public const short CMD_LOGIN_REQ = 2021;
        public const int CMD_LOGIN_RESP = 2022;

        public interface Message
        {
            void print();
            unsafe (NativeDataView, Stream) encode();
            unsafe void decode(NativeDataView ud);
        }

        public class LoginReq : Message
        {
            public int uid = 0;

            public void print()
            {
                UnityEngine.Debug.LogFormat("---> LoginReq: \r\n    uid={0}", uid);
            }

            public unsafe (NativeDataView, Stream) encode()
            {
                MemoryStream ms = new MemoryStream();
                var writer = new BinaryWriter(ms);
                writer.Write(IPAddress.HostToNetworkOrder(uid));
                var buf = ms.GetBuffer();
                fixed (byte* data = buf)
                {
                    return (new NativeDataView((IntPtr)data, (int)ms.Length), ms);
                }
            }

            public unsafe void decode(NativeDataView ud)
            {
                using (UnmanagedMemoryStream ums = new UnmanagedMemoryStream((byte*)ud.ptr, ud.len))
                {
                    BinaryReader reader = new BinaryReader(ums);
                    uid = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                    reader.Dispose();
                }
            }
        }

        public class LoginResp
        {
            public void print()
            {
                UnityEngine.Debug.LogFormat("---> LoginResp: \r\n    uid={0}, status={1}", uid, status);
            }
            public unsafe (NativeDataView, Stream) encode()
            {
                MemoryStream ms = new MemoryStream();
                
                var writer = new BinaryWriter(ms);
                writer.Write(IPAddress.HostToNetworkOrder(uid));
                writer.Write(IPAddress.HostToNetworkOrder(status));
                var buf = ms.GetBuffer();
                fixed (byte* data = buf)
                {
                    return (new NativeDataView((IntPtr)data, (int)ms.Length), ms);
                }
            }

            public unsafe void decode(NativeDataView ud)
            {
                using (UnmanagedMemoryStream ums = new UnmanagedMemoryStream((byte*)ud.ptr, ud.len))
                {
                    BinaryReader reader = new BinaryReader(ums);
                    uid = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                    status = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                    reader.Dispose();
                }
            }
            public int uid;
            public int status;
        }
        #endregion
    }
}
