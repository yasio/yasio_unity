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

        public abstract class Message
        {
            public virtual void print() { }
            public virtual Span<byte> encode() { return null; }
            public virtual unsafe void decode(Span<byte> ud) { }

            protected BinaryWriter m_writer = null;

            public BinaryWriter writer() {
                if (m_writer != null) m_writer.Dispose();
                return new BinaryWriter(new MemoryStream());
            }

            public void Dispose()
            {
                if(m_writer != null) m_writer.Dispose();
                m_writer = null;
            }
        }


        public class LoginReq : Message
        {
            public int uid = 0;

            public override void print()
            {
                UnityEngine.Debug.LogFormat("---> LoginReq: \r\n    uid={0}", uid);
            }

            public override unsafe Span<byte> encode()
            {
                var w = writer();

                w.Write(IPAddress.HostToNetworkOrder(uid));

                var ms = (MemoryStream)w.BaseStream;
                var buf = ms.GetBuffer();
                return new Span<byte>(buf, 0, (int)ms.Length);
            }

            public override unsafe void decode(Span<byte> ud)
            {
                fixed(byte* ptr = ud)
                {
                    using (UnmanagedMemoryStream ums = new UnmanagedMemoryStream(ptr, ud.Length))
                    {
                        BinaryReader reader = new BinaryReader(ums);
                        uid = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                        reader.Dispose();
                    }
                }
            }
        }

        public class LoginResp : Message
        {
            public override void print()
            {
                UnityEngine.Debug.LogFormat("---> LoginResp: \r\n    uid={0}, status={1}", uid, status);
            }
            public override unsafe Span<byte> encode()
            {
                var w = writer();

                w.Write(IPAddress.HostToNetworkOrder(uid));
                w.Write(IPAddress.HostToNetworkOrder(status));

                var ms = (MemoryStream)w.BaseStream;
                var buf = ms.GetBuffer();
                return new Span<byte>(buf, 0, (int)ms.Length);
            }

            public override unsafe void decode(Span<byte> ud)
            {
                fixed (byte* ptr = ud)
                {
                    using (UnmanagedMemoryStream ums = new UnmanagedMemoryStream(ptr, ud.Length))
                    {
                        BinaryReader reader = new BinaryReader(ums);
                        uid = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                        status = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                        reader.Dispose();
                    }
                }
            }
            public int uid;
            public int status;
        }
        #endregion
    }
}
