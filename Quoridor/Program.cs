using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quoridor
{
    static class Program
    {
        const int WINDOW_SIZE = 700;
        const int PLAYERS = 2;
        const int CIL = 9;    

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            int windowSize = WINDOW_SIZE;
            int players = PLAYERS;
            int cil = CIL;
            try
            {
                windowSize = int.Parse(args[0]);
            }
            catch (Exception e) { }

            try
            {
                players = int.Parse(args[1]);
            }
            catch (Exception e) { }

            try
            {
                cil = int.Parse(args[2]);
            }
            catch (Exception e) { }

            Application.Run(new Main(windowSize, players, cil));
        }
    }
}
