using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GillSoft.SvnMissingMerges
{
    class PreambleHelper
    {
        private readonly IInputOutputHelper io;
        public PreambleHelper(IInputOutputHelper io)
        {
            this.io = io;
        }

        public void Show()
        {
            io.WriteLine("SvnMissingMerges: Find revisions missed in merging of two branches");
            io.WriteLine();
            io.WriteLine("***********************************************************************************");
            io.WriteLine("Disclaimer:");
            io.WriteLine("    This software does not come with any warranty. Make it sure you have a backup");
            io.WriteLine("    of your data. GillSoft will not responsible for any loss due to the use of this");
            io.WriteLine("    software. Every precaution has been made to make the software safe. Also there is");
            io.WriteLine("    no malicious code in this software. There is no data sent back to GillSoft. If you");
            io.WriteLine("    think it is not safe to run this software in your environment, then DON'T!.");
            io.WriteLine("    YOU MAY CONTINUE TO RUN THIS SOFTWARE AT YOUR OWN RISK!!!");
            io.WriteLine("***********************************************************************************");
            io.WriteLine();
        }
    }
}
