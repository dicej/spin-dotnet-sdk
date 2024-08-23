using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SpinHttpWorld.wit.imports.wasi.io.v0_2_0;

namespace Spin.Http;

internal static class WasiEventLoop
{
    internal static Task Register(IPoll.Pollable pollable)
    {
        var handle = pollable.Handle;
        pollable.Handle = 0;
        return CallRegister((Thread)null!, handle);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "RegisterWasiPollable")]
        static extern Task CallRegister(Thread t, int handle);
    }

    internal static void Dispatch()
    {
        CallDispatch((Thread)null!);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "DispatchWasiEventLoop")]
        static extern void CallDispatch(Thread t);
    }
}
