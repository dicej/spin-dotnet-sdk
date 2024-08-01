#!/bin/sh

set -ex

# TODO: update to next release containing https://github.com/bytecodealliance/wit-bindgen/pull/1017
cargo install --locked --no-default-features --features csharp --git https://github.com/dicej/wit-bindgen --rev e9430c5a wit-bindgen-cli --root $(pwd)
./bin/wit-bindgen c-sharp -w spin-http -r native-aot wit
rm SpinHttpWorld_component_type.o SpinHttpWorld_cabi_realloc.c SpinHttpWorld_wasm_import_linkage_attribute.cs SpinHttpWorld.wit.exports.wasi.http.v0_2_0.IIncomingHandler.cs SpinHttpWorld.wit.exports.wasi.http.v0_2_0.IncomingHandlerInterop.cs
