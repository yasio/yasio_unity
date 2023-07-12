//
// Copyright (c) Bytedance Inc 2021-2022. All right reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

/* Match with yasio-4.0.x+ */
using System;
using System.Runtime.InteropServices;

namespace NSM2
{
    public class YASIO_NI
    {
#if (UNITY_IPHONE || UNITY_WEBGL || UNITY_SWITCH) && !UNITY_EDITOR
        public const string LIBNAME = "__Internal";
#else
        public const string LIBNAME = "yasio";
#endif
        // match with yasio/binding/yasio_ni.cpp: struct yasio_event_data
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct EventData
        {
            public int kind;
            public int status;
            public int channel;
            public IntPtr session; // transport
            public IntPtr packet;
            public IntPtr user; // the user data

            public static readonly int cbSize = sizeof(EventData);
        }

        public delegate void YNIEventDelegate(ref EventData eventData);
        public delegate int YNIResolvDelegate(string host, IntPtr sbuf);
        public delegate void YNIPrintDelegate(int level, string msg);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void yasio_init_globals(YNIPrintDelegate fnPrint);
        /// <summary>
        /// Create a low level socket io service
        /// </summary>
        /// <param name="strParam">
        /// format: "ip:port;ip:port;ip:port"
        /// </param>
        /// <param name="d"></param>
        /// <returns></returns>
        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr yasio_create_service(int channel_count, YNIEventDelegate d, IntPtr user);
        public static IntPtr yasio_create_service(int channel_count, YNIEventDelegate d)
        {
            return yasio_create_service(channel_count, d, IntPtr.Zero);
        }

        /// <summary>
        /// Destroy the low level socket io service
        /// </summary>
        /// <param name="service"></param>
        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void yasio_destroy_service(IntPtr service);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void yasio_open(IntPtr service, int cindex, int cmask);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void yasio_close(IntPtr service, int cindex);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void yasio_close_handle(IntPtr service, IntPtr sid);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void yasio_write(IntPtr service, IntPtr sid, IntPtr bytes, int len);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void yasio_write_ob(IntPtr service, IntPtr sid, IntPtr ob);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint yasio_tcp_rtt(IntPtr sid);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void yasio_dispatch(IntPtr service, int count);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern long yasio_bytes_transferred(IntPtr service, int cindex);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint yasio_connect_id(IntPtr service, int cindex);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void yasio_set_option(IntPtr service, int opt, string strParam);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void yasio_set_resolv_fn(IntPtr service, YNIResolvDelegate fnResolv);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void yasio_set_print_fn(IntPtr service, YNIPrintDelegate fnPrint);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr yasio_ob_new(int capacity);
        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void yasio_ob_release(IntPtr ob);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void yasio_ob_write_short(IntPtr obs_ptr, short value);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void yasio_ob_write_int(IntPtr obs_ptr, int value);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void yasio_ob_write_bytes(IntPtr obs_ptr, IntPtr bytes, int len);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern long yasio_highp_time();

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern long yasio_highp_clock();

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr yasio_unwrap_ptr(IntPtr opaque, int offset);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int yasio_unwrap_len(IntPtr opaque, int offset);

        /// <summary>
        /// The yasio constants
        /// </summary>
        public enum YEnums {
            #region Channel mask enums, copy from yasio/io_service.hpp
            YCM_CLIENT = 1,
            YCM_SERVER = 1 << 1,
            YCM_TCP = 1 << 2,
            YCM_UDP = 1 << 3,
            YCM_KCP = 1 << 4,
            YCM_SSL = 1 << 5,
            YCM_UDS = 1 << 6, // IPC: posix domain socket
            YCK_TCP_CLIENT = YCM_TCP | YCM_CLIENT,
            YCK_TCP_SERVER = YCM_TCP | YCM_SERVER,
            YCK_UDP_CLIENT = YCM_UDP | YCM_CLIENT,
            YCK_UDP_SERVER = YCM_UDP | YCM_SERVER,
            YCK_KCP_CLIENT = YCM_KCP | YCM_CLIENT | YCM_UDP,
            YCK_KCP_SERVER = YCM_KCP | YCM_SERVER | YCM_UDP,
            YCK_SSL_CLIENT = YCK_TCP_CLIENT | YCM_SSL,
            YCK_SSL_SERVER = YCK_TCP_SERVER | YCM_SSL,
            #endregion

            #region Event kind enums, copy from yasio.hpp
            YEK_ON_OPEN = 1,
            YEK_ON_CLOSE,
            YEK_ON_PACKET,
            YEK_CONNECT_RESPONSE = YEK_ON_OPEN,   // implicit deprecated alias
            YEK_CONNECTION_LOST = YEK_ON_CLOSE,  // implicit deprecated alias
            YEK_PACKET = YEK_ON_PACKET, // implicit deprecated alias
            #endregion

            #region Channel flags
            /* Whether setsockopt SO_REUSEADDR and SO_REUSEPORT */
            YCF_REUSEADDR = 1 << 9,

            /* For winsock security issue, see:
               https://docs.microsoft.com/en-us/windows/win32/winsock/using-so-reuseaddr-and-so-exclusiveaddruse
            */
            YCF_EXCLUSIVEADDRUSE = 1 << 10,
            #endregion

            #region All supported options by native, copy from yasio.hpp
            // Set whether disable internal dispatch, if yes
            // user must invoke dispatch on thread which care about
            // network events, it's useful for game engine update ui
            // when recv network events.
            // params: no_dispatch:int(0)
            YOPT_S_NO_DISPATCH = 1,

            // Set custom resolve function, native C++ ONLY.
            // params: func:resolv_fn_t*
            // remarks: you must ensure thread safe of it.
            YOPT_S_RESOLV_FN,

            // Set custom print function, native C++ ONLY.
            // parmas: func:print_fn_t
            // remarks: you must ensure thread safe of it.
            YOPT_S_PRINT_FN,

            // Set custom print function, native C++ ONLY.
            // parmas: func:print_fn2_t
            // remarks: you must ensure thread safe of it.
            YOPT_S_PRINT_FN2,

            // Set event callback
            // params: func:event_cb_t*
            // remarks: this callback will be invoke at io_service::dispatch caller thread
            YOPT_S_EVENT_CB,

            // Sets callback before enque event to defer queue.
            // params: func:defer_event_cb_t*
            // remarks: this callback invoke at io_service thread
            YOPT_S_DEFER_EVENT_CB,

            // Set tcp keepalive in seconds, probes is tries.
            // params: idle:int(7200), interal:int(75), probes:int(10)
            YOPT_S_TCP_KEEPALIVE,

            // Don't start a new thread to run event loop
            // params: value:int(0)
            YOPT_S_NO_NEW_THREAD,

            // Sets ssl verification cert, if empty, don't verify
            // params: path:const char*
            YOPT_S_SSL_CACERT,

            // Set connect timeout in seconds
            // params: connect_timeout:int(10)
            YOPT_S_CONNECT_TIMEOUT,

            // Set connect timeout in milliseconds
            // params: connect_timeout : int(10000),
            YOPT_S_CONNECT_TIMEOUTMS,

            // Set dns cache timeout in seconds
            // params: dns_cache_timeout : int(600),
            YOPT_S_DNS_CACHE_TIMEOUT,

            // Set dns cache timeout in milliseconds
            // params: dns_cache_timeout : int(600000),
            YOPT_S_DNS_CACHE_TIMEOUTMS,

            // Set dns queries timeout in seconds, default is: 5
            // params: dns_queries_timeout : int(5)
            // remarks:
            //         a. this option must be set before 'io_service::start'
            //         b. only works when have c-ares
            //         c. the timeout algorithm of c-ares is complicated, usually, by default, dns queries
            //         will failed with timeout after more than 75 seconds.
            //         d. for more detail, please see:
            //         https://c-ares.haxx.se/ares_init_options.html
            YOPT_S_DNS_QUERIES_TIMEOUT,

            // Set dns queries timeout in milliseconds, default is: 5000
            // see also: YOPT_S_DNS_QUERIES_TIMEOUT
            YOPT_S_DNS_QUERIES_TIMEOUTMS,

            // Set dns queries tries when timeout reached, default is: 4
            // params: dns_queries_tries : int(4)
            // remarks:
            //        a. this option must be set before 'io_service::start'
            //        b. relative option: YOPT_S_DNS_QUERIES_TIMEOUT
            YOPT_S_DNS_QUERIES_TRIES,

            // Set dns server dirty
            // params: reserved : int(1)
            // remarks: you should set this option after your device network changed
            YOPT_S_DNS_DIRTY,

            // Set custom dns servers
            // params: servers: const char*
            // remarks:
            //  a. IPv4 address is 8.8.8.8 or 8.8.8.8:53, the port is optional
            //  b. IPv6 addresses with ports require square brackets [fe80::1%lo0]:53
            YOPT_S_DNS_LIST,

            // Set ssl server cert and private key file
            // params:
            //   crtfile: const char*
            //   keyfile: const char*
            YOPT_S_SSL_CERT,

            // Set whether forward packet without GC alloc
            // params: forward: int(0)
            // reamrks:
            //   when forward packet enabled, the packet will always dispach when recv data from OS kernel immediately
            YOPT_S_FORWARD_PACKET,

            // Sets channel length field based frame decode function, native C++ ONLY
            // params: index:int, func:decode_len_fn_t*
            YOPT_C_UNPACK_FN = 101,
            YOPT_C_LFBFD_FN = YOPT_C_UNPACK_FN,

            // Sets channel length field based frame decode params
            // params:
            //     index:int,
            //     max_frame_length:int(10MBytes),
            //     length_field_offset:int(-1),
            //     length_field_length:int(4),
            //     length_adjustment:int(0),
            YOPT_C_UNPACK_PARAMS,
            YOPT_C_LFBFD_PARAMS = YOPT_C_UNPACK_PARAMS,

            // Sets channel length field based frame decode initial bytes to strip
            // params:
            //     index:int,
            //     initial_bytes_to_strip:int(0)
            YOPT_C_UNPACK_STRIP,
            YOPT_C_LFBFD_IBTS = YOPT_C_UNPACK_STRIP,

            // Sets channel remote host
            // params: index:int, ip:const char*
            YOPT_C_REMOTE_HOST,

            // Sets channel remote port
            // params: index:int, port:int
            YOPT_C_REMOTE_PORT,

            // Sets channel remote endpoint
            // params: index:int, ip:const char*, port:int
            YOPT_C_REMOTE_ENDPOINT,

            // Sets local host for client channel only
            // params: index:int, ip:const char*
            YOPT_C_LOCAL_HOST,

            // Sets local port for client channel only
            // params: index:int, port:int
            YOPT_C_LOCAL_PORT,

            // Sets local endpoint for client channel only
            // params: index:int, ip:const char*, port:int
            YOPT_C_LOCAL_ENDPOINT,

            // Mods channl flags
            // params: index:int, flagsToAdd:int, flagsToRemove:int
            YOPT_C_MOD_FLAGS,

            // Sets channel multicast interface, required on BSD-like system
            // params: index:int, multi_ifaddr:const char*
            // remarks:
            //   a. On BSD-like(APPLE, etc...) system: ipv6 addr must be "::1%lo0" or "::%en0"
            YOPT_C_MCAST_IF,

            // Enable channel multicast mode
            // params: index:int, multi_addr:const char*, loopback:int,
            // remarks:
            //   a. On BSD-like(APPLE, etc...) system: ipv6 addr must be: "ff02::1%lo0" or "ff02::1%en0"
            // refer to: https://www.tldp.org/HOWTO/Multicast-HOWTO-2.html
            YOPT_C_ENABLE_MCAST,

            // Disable channel multicast mode
            // params: index:int
            YOPT_C_DISABLE_MCAST,

            // The kcp conv id, must equal in two endpoint from the same connection
            // params: index:int, conv:int
            YOPT_C_KCP_CONV,

            // The setting for kcp nodelay config.
            // refer to:https://github.com/skywind3000/kcp/wiki/KCP-Basic-Usage
            // params: index:int, nodelay:int, interval:int, resend:int, nc:int.
            YOPT_C_KCP_NODELAY,

            // The setting for kcp window size config.
            // refer to:https://github.com/skywind3000/kcp/wiki/KCP-Basic-Usage
            // params: index:int, sndWnd:int, rcvwnd:int
            YOPT_C_KCP_WINDOW_SIZE,

            // The setting for kcp MTU config.
            // refer to:https://github.com/skywind3000/kcp/wiki/KCP-Basic-Usage
            // params: index:int,mtu:int
            YOPT_C_KCP_MTU,

            // The setting for kcp min RTO config.
            // refer to:https://github.com/skywind3000/kcp/wiki/KCP-Basic-Usage
            // params: index:int,minRTO:int
            YOPT_C_KCP_RTO_MIN,

            // Whether never perform bswap for length field
            // params: index:int, no_bswap:int(0)
            YOPT_C_UNPACK_NO_BSWAP,

            // Change 4-tuple association for io_transport_udp
            // params: transport:transport_handle_t
            // remarks: only works for udp client transport
            YOPT_T_CONNECT,

            // Dissolve 4-tuple association for io_transport_udp
            // params: transport:transport_handle_t
            // remarks: only works for udp client transport
            YOPT_T_DISCONNECT,

            // Sets io_base sockopt
            // params: io_base*,level:int,optname:int,optval:int,optlen:int
            YOPT_B_SOCKOPT = 201,
            #endregion
        };


        unsafe public static string DumpHex(IntPtr buf, int len, string prefix = "")
        {
            if (buf != IntPtr.Zero)
            {
                byte* pData = (byte*)buf;
                string s = "";
                string fmt = prefix + "{0:X2},";
                for (uint i = (uint)0; i < len; ++i)
                {
                    s += String.Format(fmt, pData[i]);
                }
                return s;
            }
            return "";
        }
    }
}
