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

namespace NSM2
{
    public interface INetworkPacketHandler
    {
        /// <summary>
        /// 获取拆包参数
        /// </summary>
        /// <param name="channel"></param>
        /// <returns>分号分割的字符串参数，示例: $"{channelIndex};{maxFrameLength};{lenghtFieldOffset};{lengthFieldLength};{lengthAdjustment}"</returns>
        string GetOptions(int channel);

        /// <summary>
        /// 获取协议头所需字节数
        /// </summary>
        /// <returns></returns>
        int GetHeaderSize();

        /// <summary>
        /// 填充应用层协议单元（协议头+PB数据)
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="ud">The user data(such as pb buffer)</param>
        /// <param name="ob">The pdu wrapped with c++ obstream*) for Encode output</param>
        void EncodePDU(int cmd, Span<byte> ud, IntPtr ob);

        /// <summary>
        /// 处理应用层协议单元（协议头+PB数据)
        /// </summary>
        /// <param name="d">The pointer of first byte of whole PDU</param>
        /// <param name="len">The length of PDU</param>
        /// <param name="channel"></param>
        (int, Stream) DecodePDU(IntPtr d, int len, out Span<byte> msg);

        /// <summary>
        /// 处理网络事件
        /// </summary>
        /// <param name="ev">事件类型</param>
        /// <param name="cmd">消息码</param>
        /// <param name="ud">消息</param>
        /// <param name="channel">信道</param>
        void HandleEvent(NetworkEvent ev, int cmd, Span<byte> ud, int channel);
    }
}
