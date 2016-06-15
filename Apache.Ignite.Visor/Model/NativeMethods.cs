using System.Runtime.InteropServices;

namespace Apache.Ignite.Visor.Model
{
    public static class NativeMethods
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();
    }
}
