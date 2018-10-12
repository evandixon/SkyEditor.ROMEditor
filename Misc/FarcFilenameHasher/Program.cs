using SkyEditor.ROMEditor.MysteryDungeon.PSMD;
using System;

namespace FarcFilenameHasher
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Enter file name to hash: ");
                var hash = PmdFunctions.Crc32Hash(Console.ReadLine());
                Console.WriteLine(hash.ToString("X").PadLeft(8, '0'));
            }
        }
    }
}
