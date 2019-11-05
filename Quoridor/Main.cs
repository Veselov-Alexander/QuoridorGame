using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quoridor
{
    public partial class Main : Form
    {
        Game game;

        public Main(int windowSize, int players, int cil)
        {
            InitializeComponent();
            game = new Game(this, drawPanel, windowSize, players, cil);
        }      

        private void drawPanel_MouseClick(object sender, MouseEventArgs e)
        {
            game.ProcessMouseClick(e.Location);
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            game.Update();
        }
    }
}
