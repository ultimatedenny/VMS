using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VMS.Library
{
    public class WindowsAuth : IDisposable
    {
        bool disposed = false;
        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(
        string lpszUsername,
        string lpszDomain,
        string lpszPassword,
        int dwLogonType,
        int dwLogonProvider,
        out IntPtr phToken);
        [DllImport("advapi32.DLL")]
        public static extern int ImpersonateLoggedOnUser(IntPtr hToken); //handle to token for logged-on user

        [DllImport("advapi32.DLL")]
        public static extern bool RevertToSelf();

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hToken);

        enum LogonProvider
        {
            LOGON32_PROVIDER_DEFAULT = 0,
            LOGON32_PROVIDER_WINNT50 = 3,
            LOGON32_PROVIDER_WINNT40 = 2,
            LOGON32_PROVIDER_WINNT35 = 1,
            LOGON32_LOGON_INTERACTIVE = 2,
            LOGON32_LOGON_NETWORK = 3,
            LOGON32_LOGON_BATCH = 4,
            LOGON32_LOGON_SERVICE = 5,
            LOGON32_LOGON_UNLOCK = 7
        }

        public IntPtr admin_token = IntPtr.Zero;
        /// <summary>
        /// Perform windows authentication
        /// </summary>
        /// <param name="UsrNam">Windows User ID</param>
        /// <param name="PasWrd">Windows Password</param>
        /// <param name="_impersonation">Decide whether will impersonate or not</param>
        /// <param name="_domain">Domain name</param>
        /// <returns></returns>
        public Boolean WinAuth(String UsrNam, String PasWrd, bool _impersonation, string _domain)
        {
            String ssDomain = Environment.UserDomainName;
            if (!_impersonation)
            {
                ssDomain = _domain;
            }
            IntPtr phToken = IntPtr.Zero;

            int valid = LogonUser(
                UsrNam,
                ssDomain,
                PasWrd,
                (int)LogonProvider.LOGON32_LOGON_INTERACTIVE,
                (int)LogonProvider.LOGON32_PROVIDER_DEFAULT,
                out admin_token);

            if (valid != 0)
            {
                if (_impersonation)
                {
                    int IPI = ImpersonateLoggedOnUser(admin_token);
                    if (IPI != 0)
                    {
                        CloseHandle(admin_token);
                        RevertToSelf();
                        admin_token = IntPtr.Zero;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }
        ~WindowsAuth()
        {
            Dispose(false);
        }
    }
}
