// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Internal.Runtime.Augments
{
    /// <summary>For internal use only.  Exposes runtime functionality to the Environments implementation in corefx.</summary>
    public static partial class EnvironmentAugments
    {
        private static string GetEnvironmentVariableCore(string variable)
        {
            Debug.Assert(variable != null);
            return Marshal.PtrToStringAnsi(Interop.Sys.GetEnv(variable));
        }

        private static void SetEnvironmentVariableCore(string variable, string value)
        {
            Debug.Assert(variable != null);
            throw new NotImplementedException();
        }

        public static IEnumerable<KeyValuePair<string,string>> EnumerateEnvironmentVariables()
        {
            string bufferString = null;
            int currentBufferSize = 1024;

            for (;;)
            {
                char[] buffer = ArrayPool<char>.Shared.Rent(currentBufferSize);

                // Get full path to the executable image
                int charactersWritten = Interop.Sys.GetEnumerateEnvironmentVariables(buffer, buffer.Length);

                if (charactersWritten < buffer.Length)
                {
                    bufferString = new string(buffer, 0, charactersWritten);
                    ArrayPool<char>.Shared.Return(buffer);
                    break;
                }

                ArrayPool<char>.Shared.Return(buffer);
                currentBufferSize *= 2;
            }

            return ParseEnvironmentVariables(stringBuffer);
        }

        private static IEnumerable<KeyValuePair<string,string>> ParseEnvironmentVariables(string stringBuffer)
        {
            string[] pairs = stringBuffer.Split(new char[] {';'});

            for (int i = 0; i < pairs.Length; i++)
            {
                string[] key_value = pairs[i].Split(new char[] {'='});
                yield return new KeyValuePair<string, string>(key_value[0].Trim(), key_value[1].Trim());
            }
        }
    }
}
