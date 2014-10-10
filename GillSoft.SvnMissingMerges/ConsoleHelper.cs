using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GillSoft.SvnMissingMerges
{
    internal class ConsoleHelper : IInputOutputHelper
    {
        void IInputOutputHelper.Wait()
        {
            Console.Write("Press RETURN to close...");
            Console.ReadLine();
        }


        void IInputOutputHelper.WriteLine(string value, params object[] args)
        {
            Console.WriteLine(value, args);
        }

        void IInputOutputHelper.WriteLine()
        {
            Console.WriteLine();
        }

        void IInputOutputHelper.Write(string value, params object[] args)
        {
            Console.Write(value, args);
        }
    }
}
