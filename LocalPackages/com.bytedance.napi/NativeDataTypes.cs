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

namespace ByteDance.NAPI
{
    /// <summary>
    /// Could use as Natvie CStr or byte array
    /// </summary>
    public struct NativeDataView
    {
        public IntPtr ptr;
        public int len;

        public NativeDataView(IntPtr p, int l)
        {
            ptr = p;
            len = l;
        }

        public void Set(IntPtr p, int l)
        {
            ptr = p;
            len = l;
        }

        public bool IsNull()
        {
            return ptr == IntPtr.Zero;
        }

        public void SetNull()
        {
            ptr = IntPtr.Zero;
            len = 0;
        }

        public bool IsEmpty()
        {
            return ptr == IntPtr.Zero || len == 0;
        }

        public static readonly NativeDataView NullValue = new NativeDataView(IntPtr.Zero, 0);
    }

    /// <summary>
    /// Could use as any natvie UDT* view, not char*
    /// Such as Lua TString*
    /// </summary>
    public struct NativeUDTView
    {
        public NativeUDTView(IntPtr p)
        {
            ptr = p;
        }

        public bool IsNull()
        {
            return ptr == IntPtr.Zero;
        }

        public void Set(IntPtr p)
        {
            ptr = p;
        }

        public IntPtr ptr; // the lua vm TString*

        public static readonly NativeUDTView NullValue = new NativeUDTView(IntPtr.Zero);
    }
}
