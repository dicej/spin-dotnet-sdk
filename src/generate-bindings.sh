#!/bin/sh

set -ex

cargo install --locked --no-default-features --features csharp wit-bindgen-cli --root $(pwd)
./bin/wit-bindgen c-sharp -w spin-http -r native-aot wit
rm SpinHttpWorld_wasm_import_linkage_attribute.cs
