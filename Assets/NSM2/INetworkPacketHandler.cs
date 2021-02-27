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
        void EncodePDU(int cmd, NativeDataView ud, IntPtr ob);

        /// <summary>
        /// 处理应用层协议单元（协议头+PB数据)
        /// </summary>
        /// <param name="d">The pointer of first byte of whole PDU</param>
        /// <param name="len">The length of PDU</param>
        /// <param name="channel"></param>
        (int, NativeDataView,Stream) DecodePDU(IntPtr d, int len);

        /// <summary>
        /// 处理网络事件
        /// </summary>
        /// <param name="ev">事件类型</param>
        /// <param name="cmd">消息码</param>
        /// <param name="ud">消息</param>
        /// <param name="channel">信道</param>
        void HandleEvent(NetworkEvent ev, int cmd, NativeDataView ud, int channel);
    }
}
