# Experimental Spin SDK for .NET based on NativeAOT-LLVM

This is an in-progress [Spin](https://github.com/fermyon/spin) SDK for .NET
based on the experimental
[NativeAOT-LLVM](https://github.com/dotnet/runtimelab/tree/feature/NativeAOT-LLVM)
fork of the .NET runtime.

As of this writing, .NET support for WASIp2 is still under development, so this
library relies on unofficial builds of the NativeAOT-LLVM compiler and standard
library.  Once WASIp2 support has been fully merged into the
`feature/NativeAOT-LLVM` and CI has been updated to publish official packages
for all major platforms (i.e. Windows, MacOS, and Linux), we'll switch to those
and publish an official package for this library.  Meanwhile, you'll need to
download the artifacts manually and point NuGet to them as described below.

## Running the Sample(s)

### Prerequisite(s)

- [.NET 9.0 Preview 6 or later](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Spin 2.7.0 or later](https://github.com/fermyon/spin/releases/tag/v2.7.0)
- The following files, downloaded to a local directory on your computer:
    - [Fermyon.Spin.SDK.0.1.0-dev.nupkg](https://github.com/dicej/spin-dotnet-sdk/releases/download/canary/Fermyon.Spin.SDK.0.1.0-dev.nupkg)
    - [Microsoft.DotNet.ILCompiler.LLVM.9.0.0-dev.nupkg](https://github.com/dicej/spin-dotnet-sdk/releases/download/canary/Microsoft.DotNet.ILCompiler.LLVM.9.0.0-dev.nupkg)
    - [runtime.wasi-wasm.Microsoft.DotNet.ILCompiler.LLVM.9.0.0-dev.nupkg](https://github.com/dicej/spin-dotnet-sdk/releases/download/canary/runtime.wasi-wasm.Microsoft.DotNet.ILCompiler.LLVM.9.0.0-dev.nupkg)
    - Either [runtime.linux-arm64.Microsoft.DotNet.ILCompiler.LLVM.9.0.0-dev.nupkg](https://github.com/dicej/spin-dotnet-sdk/releases/download/canary/runtime.linux-arm64.Microsoft.DotNet.ILCompiler.LLVM.9.0.0-dev.nupkg), [runtime.linux-x64.Microsoft.DotNet.ILCompiler.LLVM.9.0.0-dev.nupkg](https://github.com/dicej/spin-dotnet-sdk/releases/download/canary/runtime.linux-x64.Microsoft.DotNet.ILCompiler.LLVM.9.0.0-dev.nupkg), [runtime.win-x64.Microsoft.DotNet.ILCompiler.LLVM.9.0.0-dev.nupkg](https://github.com/dicej/spin-dotnet-sdk/releases/download/canary/runtime.win-arm64.Microsoft.DotNet.ILCompiler.LLVM.9.0.0-dev.nupkg), or [runtime.osx-arm64.Microsoft.DotNet.ILCompiler.LLVM.9.0.0-dev.nupkg](https://github.com/dicej/spin-dotnet-sdk/releases/download/canary/runtime.osx-arm64.Microsoft.DotNet.ILCompiler.LLVM.9.0.0-dev.nupkg), depending on the platform you're using.  If there isn't a build yet for your platform, please file an issue.

Once you've downloaded the above files, please set the `NUGET_LOCAL_PATH`
environment variable to point to the absolute path of the directory you chose,
e.g. `export NUGET_LOCAL_PATH=$HOME/Downloads`.

### Building and Running

```
cd samples/http-hello
spin build -u
```

While Spin is running, you can switch to another terminal and run e.g.

```
curl -i localhost:3000
```

which should yield something like:

```
HTTP/1.1 200 OK
content-type: text/plain
transfer-encoding: chunked
date: Fri, 02 Aug 2024 19:45:33 GMT

hello, world!
```

Please file an issue if you have any trouble.

## TODOs and Open Questions

- This currently borrows (and tweaks) some MSBuild configuration from [componentize-dotnet](https://github.com/bytecodealliance/componentize-dotnet).  We should upstream the changes and use the `ByteCodeAlliance.Componentize.DotNet.WitBindgen` package instead.
- Currently, application code must explicitly add the `runtime.wasi-wasm.Microsoft.DotNet.ILCompiler.LLVM` and `runtime.${os}-${arch}.Microsoft.DotNet.ILCompiler.LLVM` `PackageReference`s as well as the `Fermyon.Spin.SDK` reference.  Is it possible to use only the latter and have the former pulled in automatically?
