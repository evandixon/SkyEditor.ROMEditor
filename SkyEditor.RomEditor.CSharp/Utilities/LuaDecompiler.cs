using System;
using System.Collections.Generic;
using System.Text;
using unluac.decompile;
using unluac.parse;
using unluacNet;

namespace SkyEditor.RomEditor.Utilities
{
    public class LuaDecompiler
    {
        public static string DecompileScript(byte[] compiledScript)
        {
            var function = LoadScript(compiledScript);
            var decompiler = new Decompiler(function);
            decompiler.decompile();

            var output = new StringOutput();
            decompiler.print(output);

            return output.GetOutput();
        }

        private static LFunction LoadScript(byte[] compiledScript)
        {
            var fileData = new ByteBuffer(compiledScript);
            var header = new BHeader(fileData);

            return header.function.parse(fileData, header);
        }

        private class StringOutput : OutputProvider
        {
            public StringOutput()
            {
                output = new StringBuilder();
            }

            private readonly StringBuilder output;

            public void print(string s)
            {
                output.Append(s);
            }

            public void println()
            {
                output.AppendLine();
            }

            public string GetOutput()
            {
                return output.ToString();
            }
        }
    }
}
