#!/bin/sh

set -ex

# TODO: update to next wit-bindgen release once
# https://github.com/bytecodealliance/wit-bindgen/pull/1026 and
# https://github.com/bytecodealliance/wit-bindgen/pull/1027 are merged and
# released
cargo install --locked --no-default-features --features csharp --git https://github.com/dicej/wit-bindgen --branch csharp-public-imports-tmp wit-bindgen-cli --root $(pwd)
./bin/wit-bindgen c-sharp -w spin-http -r native-aot wit
rm SpinHttpWorld_wasm_import_linkage_attribute.cs
