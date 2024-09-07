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
using System.Text;

namespace NSM2
{
    public class SampleNetworkPacketHandler : INetworkPacketHandler
    {
        /*
         * 自定义二进制协议定义(这里只为简单演示，实际使用请自行定义):
         * - 协议头
         *   - cmd(消息号) ---------------------------------------> short(2bytes)
         *   - dataLen(消息数据大小, 不包含协议头本身) ---------------> int(4bytes)
         *   - checksum(协议校验码，可以是业务数据摘要或者固定值) ------> int(4bytes)
         * - 协议体/业务数据(任意数据格式，例如: 二进制，protobuffers, flatbuffers等)
         */

        public bool DUMP_RECV_HEX = true; // DUMP所有接受消息的二进制HEX值
        const int PROTO_HEADER_SIZE = sizeof(ushort) + sizeof(uint) + sizeof(int);
        
        // 为了简单演示，协议校验码采用固定值
        const int VERIFIED_MAGIC_NUM = 0x55AA55AA;

        public string GetOptions(int channel)
        {
            // --channelIndex
            // --maxFrameLength, 最大包长度
            // --lenghtFieldOffset, 长度字段偏移，相对于包起始字节
            // --lengthFieldLength, 长度字段大小，支持1字节，2字节，3字节，4字节
            // --lengthAdjustment：如果长度字段字节大小包含包头，则为0， 否则，这里 = 包头大小
            return $"{channel};10485760;2;4;{PROTO_HEADER_SIZE}";
        }

        public int GetHeaderSize()
        {
            return PROTO_HEADER_SIZE;
        }

        unsafe public void EncodePDU(int cmd, Span<byte> ud, IntPtr ob)
        {
            YASIO_NI.yasio_ob_write_short(ob, (short)cmd);
            YASIO_NI.yasio_ob_write_int(ob, ud.Length); // packet size
            YASIO_NI.yasio_ob_write_int(ob, VERIFIED_MAGIC_NUM);  // magic number
            fixed (void* bytes = ud)
            {
                YASIO_NI.yasio_ob_write_bytes(ob, (IntPtr)bytes, ud.Length); // ud
            }
        }

        unsafe public (int, Stream) DecodePDU(IntPtr bytes, int len, out Span<byte> msg)
        {
            Stream dataStream = new UnmanagedMemoryStream((byte*)bytes, len);
            try
            {
                using (var reader = new BinaryReader(dataStream, Encoding.ASCII, true))
                {
                    // 读取包头信息,magic number检验(这里也可以用hash摘要算法，计算消息体数据摘要)
                    int cmd = IPAddress.NetworkToHostOrder(reader.ReadInt16());
                    int udLen = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                    int magicNum = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                    reader.Dispose();
                    
                    if (magicNum == VERIFIED_MAGIC_NUM)
                    {
                        // magic number校验正确
#if UNITY_EDITOR
                        if (DUMP_RECV_HEX)
                        {
                            int bodyLen = len - PROTO_HEADER_SIZE; // the udLen===bodyLen
                            UnityEngine.Debug.LogFormat("cmd={0}, udLen/bodyLen={1}/{2}, msg: {3}\n", cmd, udLen, bodyLen, YASIO_NI.DumpHex(bytes, len));
                            UnityEngine.Debug.LogFormat("cmd={0}, payload: {1}\n", cmd, YASIO_NI.DumpHex(bytes + PROTO_HEADER_SIZE, bodyLen));
                        }
#endif
                        msg = new Span<byte>((bytes + PROTO_HEADER_SIZE).ToPointer(), len - PROTO_HEADER_SIZE);

                        return (cmd, dataStream);
                    }
                    else
                    {
                        UnityEngine.Debug.LogErrorFormat("SampleNetworkPacketHandler.DecodePDU: check magic number failed, magicNum={0}, VERIFIED_MAGIC_NUM={1}", magicNum, VERIFIED_MAGIC_NUM);
                    }
                }
            }
            finally
            {
                dataStream?.Dispose();
            }

            msg = Span<byte>.Empty;
            return (-1, null);
        }

        /// <summary>
        /// 业务只需要具体实现此函数的分发即可, 此处只是简单示例
        /// </summary>
        /// <param name="ev"></param>
        /// <param name="cmd"></param>
        /// <param name="ud"></param>
        /// <param name="channel"></param>
        public void HandleEvent(NetworkEvent ev, int cmd, Span<byte> ud, int channel)
        {
            UnityEngine.Debug.LogFormat("SampleNetworkPacketHandler.HandleEvent, event={0}, cmd={1}, channel={2}", ev, cmd, channel);
            if(cmd == AppProtocol.CMD_LOGIN_REQ)
            { // SampelScene应该是 channel:1 收到
                var msg = new AppProtocol.LoginReq();
                msg.decode(ud);
                msg.print();

                // 回应客户端
                var reply = new AppProtocol.LoginResp();
                reply.uid = msg.uid;
                reply.status = 200; // 200 表示success
                Span<byte> udReply = reply.encode();
                NetworkManager.Instance.SendSerializedMsg(AppProtocol.CMD_LOGIN_RESP, udReply, AppProtocol.SERVER_CHANNEL);
            }
            else if(cmd == AppProtocol.CMD_LOGIN_RESP)
            { // SampelScene应该是 channel:0 收到
                // 打印登录响应消息
                var msg = new AppProtocol.LoginResp();
                msg.decode(ud);
                msg.print();
            }
        }
    }
}
