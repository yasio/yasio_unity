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
using UnityEngine;

namespace NSM2
{
    public class Singleton<T> : IDisposable where T : class, IDisposable, new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }

        public virtual void Dispose()
        {
            _instance = null;
            GC.SuppressFinalize(this);
        }

        public static bool HasInstance()
        {
            return _instance != null;
        }

        public static void DestroyInstance()
        {
            if (_instance == null) return;

            Debug.Log($"Destroying instance of {_instance.GetType().Name}");

            _instance.Dispose();
            _instance = null;
        }

        ~Singleton()
        {
            Dispose();
        }
    }
}
