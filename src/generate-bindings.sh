#!/bin/sh

set -ex

# TODO: switch to crates.io release once https://github.com/bytecodealliance/wit-bindgen/pull/1040 is merged and released
cargo install --locked --no-default-features --features csharp --git https://github.com/dicej/wit-bindgen --rev 694fd927 wit-bindgen-cli --root $(pwd)
./bin/wit-bindgen c-sharp -w spin-http -r native-aot wit
rm SpinHttpWorld_wasm_import_linkage_attribute.cs
