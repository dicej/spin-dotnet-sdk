// Generated by `wit-bindgen` 0.29.0. DO NOT EDIT!
// <auto-generated />
#nullable enable

using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0;

public interface IVariables {

    /**
    * The set of errors which may be raised by functions in this interface.
    */

    public class Error {
        public readonly byte Tag;
        private readonly object? value;

        private Error(byte tag, object? value) {
            this.Tag = tag;
            this.value = value;
        }

        public static Error invalidName(string invalidName) {
            return new Error(INVALID_NAME, invalidName);
        }

        public static Error undefined(string undefined) {
            return new Error(UNDEFINED, undefined);
        }

        public static Error provider(string provider) {
            return new Error(PROVIDER, provider);
        }

        public static Error other(string other) {
            return new Error(OTHER, other);
        }

        public string AsInvalidName
        {
            get
            {
                if (Tag == INVALID_NAME)
                return (string)value!;
                else
                throw new ArgumentException("expected INVALID_NAME, got " + Tag);
            }
        }

        public string AsUndefined
        {
            get
            {
                if (Tag == UNDEFINED)
                return (string)value!;
                else
                throw new ArgumentException("expected UNDEFINED, got " + Tag);
            }
        }

        public string AsProvider
        {
            get
            {
                if (Tag == PROVIDER)
                return (string)value!;
                else
                throw new ArgumentException("expected PROVIDER, got " + Tag);
            }
        }

        public string AsOther
        {
            get
            {
                if (Tag == OTHER)
                return (string)value!;
                else
                throw new ArgumentException("expected OTHER, got " + Tag);
            }
        }

        public const byte INVALID_NAME = 0;
        public const byte UNDEFINED = 1;
        public const byte PROVIDER = 2;
        public const byte OTHER = 3;
    }

}