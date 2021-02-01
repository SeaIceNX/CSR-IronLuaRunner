using System;
using CSR;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IronPythonRunner
{
    class Hook
    {
		private delegate void TP(IntPtr pl, Vec3 pos, int a3, int a4, ulong uid);
		public static void tp(MCCSAPI api,int funcaddr, int dimid)
		{
			var functpr = api.dlsym(funcaddr);
			var _tp = (TP)Marshal.functpr(typeof(TP));
			_tp(pl,0, dimid, 0, new CsPlayer.UniqueId);
		}
	}
}
