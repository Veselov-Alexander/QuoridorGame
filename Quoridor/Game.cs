using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Quoridor
{
    class Game
    {
        private int players;
        private int cil;

        private Quoridor quoridor;

        private int windowSize;
        private Form form;
        private Panel drawPanel;

        int boardSize, startCoord, cellSize, spaceSize;

        private Bitmap field;

        private static Color boardColor = Color.FromArgb(90, 50, 50);
        private static Color tileColor = Color.FromArgb(220, 220, 160);
        private static Color cellColor = Color.FromArgb(110, 110, 110);
        private static Color helpColor = Color.FromArgb(255, 255, 255);

        private SolidBrush[] playerColors;

        private static SolidBrush cellBrush = new SolidBrush(cellColor);
        private static SolidBrush tileBrush = new SolidBrush(tileColor);
        private static SolidBrush helpBrush;

        private static Pen helpPen;

        public Game(Form form, Panel drawPanel, int windowSize, int players, int cil)
        {
            quoridor = new Quoridor(cil, players);

            this.windowSize = windowSize;
            this.form = form;
            this.drawPanel = drawPanel;
            this.players = players;
            this.cil = cil;

            int BORDER = 10;
            int width = windowSize + 2 * BORDER + 20;
            int height = windowSize + 2 * BORDER + 50;
            form.Size = new Size(width, height);
            drawPanel.Size = new Size(windowSize, windowSize);
            drawPanel.Location = new Point(BORDER, BORDER);
            drawPanel.BackColor = Color.Gray;

            boardSize = (int)Math.Round(windowSize * cil / (cil + 4.0));
            cellSize = (int)Math.Round(0.8 * windowSize / (cil + 4.0));
            spaceSize = (int)Math.Round(0.2 * windowSize / (cil + 4.0));
            boardSize = (cellSize + spaceSize) * cil;
            startCoord = (int)Math.Round((windowSize - boardSize + spaceSize) / 2.0);

            playerColors = new SolidBrush[players];
            playerColors[0] = new SolidBrush(tileColor);
            playerColors[1] = new SolidBrush(boardColor);
            if (players == 4)
            {
                playerColors[2] = new SolidBrush(Color.FromArgb(220, 220, 80));
                playerColors[3] = new SolidBrush(Color.FromArgb(150, 50, 50));
            }
            helpPen = new Pen(helpColor, spaceSize / 2);
            helpBrush = new SolidBrush(helpColor);
            field = new Bitmap(windowSize, windowSize);
            DrawField(Graphics.FromImage(field));
        }

        public void Update()
        {
            Draw();
        }

        public void ProcessMouseClick(Point point)
        {
            if (quoridor.GameIsEnded())
            {
                return;
            }
            var tilePoint = GetTilePoint(point);
            if (tilePoint.Item3 == 2) // on cell
            {
                quoridor.Move(tilePoint.Item1, tilePoint.Item2);
            }
            if (tilePoint.Item3 == 0 || tilePoint.Item3 == 1) // on border
            {
                quoridor.SetTile(tilePoint.Item1, tilePoint.Item2, tilePoint.Item3);
            }
        }

        private Tuple<int, int, int> GetTilePoint(Point point)
        {
            int row = (point.Y - startCoord) / (cellSize + spaceSize);
            int column = (point.X - startCoord) / (cellSize + spaceSize);
            if (row < 0 || column < 0 || row >= cil || column >= cil)
            {
                return new Tuple<int, int, int>(-1, -1, -1);
            }
            if (startCoord + row * (cellSize + spaceSize) <= point.Y &&
                point.Y < startCoord + row * (cellSize + spaceSize) + cellSize &&
                startCoord + column * (cellSize + spaceSize) <= point.X &&
                point.X < startCoord + column * (cellSize + spaceSize) + cellSize)
            {
                return new Tuple<int, int, int>(row, column, 2);
            }
            else
            {
                if (row == cil - 1 || column == cil - 1)
                {
                    return new Tuple<int, int, int>(-1, -1, -1);
                }
                if (point.X > startCoord + column * (cellSize + spaceSize) + cellSize &&
                    point.Y < startCoord + row * (cellSize + spaceSize) + cellSize)
                {
                    return new Tuple<int, int, int>(row, column, 0);
                }
                if (point.X < startCoord + column * (cellSize + spaceSize) + cellSize &&
                    point.Y > startCoord + row * (cellSize + spaceSize) + cellSize)
                {
                    return new Tuple<int, int, int>(row, column, 1);
                }
            }
            return new Tuple<int, int, int>(-1, -1, -1);
        }

        private void Draw()
        {
            Bitmap image = new Bitmap(drawPanel.Width, drawPanel.Height);
            Graphics graphics = Graphics.FromImage(image);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.DrawImage(field, new Point(0, 0));
            DrawTiles(graphics);
            DrawPlayers(graphics);
            DrawAvailableTiles(graphics);
            if (quoridor.GameIsEnded())
            {
                DrawEndGame(graphics);
            }
            else
            {
                DrawAvailableCells(graphics);
            }
            drawPanel.CreateGraphics().DrawImage(image, new Point(0, 0));
        }

        private void DrawEndGame(Graphics graphics)
        {
            int x = (int)Math.Round(windowSize * 0.1);
            int y = (int)Math.Round(windowSize * 0.4);
            int width = windowSize - 2 * x;
            int height = windowSize - 2 * y;
            graphics.FillRectangle(tileBrush, new Rectangle(x, y, width, height));
            String text = "Player " + (quoridor.GetWinner() + 1).ToString() + " wins.";
            Font font = new Font(new FontFamily("Times New Roman"), windowSize * 0.05f, FontStyle.Regular, GraphicsUnit.Point);
            Point textPoint = new Point((int)Math.Round(x + width * 0.2), (int)Math.Round(y + height * 0.25));
            graphics.DrawString(text, font, cellBrush, textPoint);
        }

        private void DrawTiles(Graphics graphics)
        {
            var tiles = quoridor.GetFieldTiles();
            for (int row = 0; row < tiles.GetLength(0); ++row)
            {
                for (int column = 0; column < tiles.GetLength(1); ++column)
                {
                    for (int dir = 0; dir < tiles.GetLength(2); ++dir)
                    {
                        if (tiles[row, column, dir])
                        {
                            int left = 0, top = 0, right = 0, bottom = 0;
                            if (dir == 0)
                            {
                                left = startCoord + column * (cellSize + spaceSize) + cellSize;
                                top = startCoord + row * (cellSize + spaceSize);
                                right = left + spaceSize;
                                bottom = top + 2 * cellSize + spaceSize;
                            }
                            else
                            {
                                left = startCoord + column * (cellSize + spaceSize);
                                top = startCoord + row * (cellSize + spaceSize) + cellSize;
                                right = left + 2 * cellSize + spaceSize;
                                bottom = top + spaceSize;
                            }
                            Rectangle rect = Rectangle.FromLTRB(left, top, right, bottom);
                            graphics.FillRectangle(tileBrush, rect);
                        }
                    }
                }
            }
        }

        private void DrawAvailableCells(Graphics graphics)
        {
            int shiftSize = (int)Math.Round(cellSize * 0.3);
            int circleSize = cellSize - 2 * shiftSize - 1;
            int tileSize = cellSize + spaceSize;
            Size size = new Size(circleSize, circleSize);
            List<Point> points = quoridor.GetAvailableMoves(quoridor.GetCurrentPlayer());
            for (int i = 0; i < points.Count; ++i)
            {
                Rectangle rect = new Rectangle(startCoord + points[i].Y * tileSize + shiftSize,
                                               startCoord + points[i].X * tileSize + shiftSize,
                                               circleSize, circleSize);
                graphics.FillEllipse(helpBrush, rect);
            }
        }

        private void DrawAvailableTiles(Graphics graphics)
        {
            var tilesCount = quoridor.GetPlayerTiles();
            for (int column = 0; column < tilesCount[0]; ++column)
            {
                int left = startCoord + column * (cellSize + spaceSize) - spaceSize;
                int top = startCoord - 2 * (cellSize + spaceSize) + spaceSize / 2;
                int right = left + spaceSize;
                int bottom = top + 2 * cellSize + spaceSize;
                Rectangle rect = Rectangle.FromLTRB(left, top, right, bottom);
                graphics.FillRectangle(tileBrush, rect);
                if (players == 4)
                {
                    left = startCoord + (column + cil / 2 + 1) * (cellSize + spaceSize) - spaceSize;
                    right = left + spaceSize;
                    rect = Rectangle.FromLTRB(top, left, bottom, right);
                    graphics.FillRectangle(tileBrush, rect);
                }
            }
            for (int column = 0; column < tilesCount[1]; ++column)
            {
                int left = startCoord + boardSize - column * (cellSize + spaceSize) - spaceSize;
                int top = startCoord + boardSize - spaceSize / 2;
                int right = left + spaceSize;
                int bottom = top + 2 * cellSize + spaceSize;
                Rectangle rect = Rectangle.FromLTRB(left, top, right, bottom);
                graphics.FillRectangle(tileBrush, rect);
                if (players == 4)
                {
                    left = startCoord + boardSize - (column + cil / 2 + 1) * (cellSize + spaceSize) - spaceSize;
                    right = left + spaceSize;
                    rect = Rectangle.FromLTRB(top, left, bottom, right);
                    graphics.FillRectangle(tileBrush, rect);
                }
            }
        }

        private void DrawPlayers(Graphics graphics)
        {
            int shiftSize = (int)Math.Round(cellSize * 0.15);
            int circleSize = cellSize - 2 * shiftSize - 1;
            int tileSize = cellSize + spaceSize;
            Size size = new Size(circleSize, circleSize);
            Point[] points = quoridor.GetPlayerPositions();
            if (!quoridor.GameIsEnded())
            {
                int currentPlayer = quoridor.GetCurrentPlayer();
                Point currentPlayerPosition = points[currentPlayer];
                Rectangle rect = new Rectangle(startCoord + currentPlayerPosition.Y * tileSize + shiftSize,
                                               startCoord + currentPlayerPosition.X * tileSize + shiftSize,
                                               circleSize, circleSize);
                graphics.DrawEllipse(helpPen, rect);
            }
            
            for (int i = 0; i < players; ++i)
            {
                Rectangle rect = new Rectangle(startCoord + points[i].Y * tileSize + shiftSize,
                                               startCoord + points[i].X * tileSize + shiftSize,
                                               circleSize, circleSize);
                graphics.FillEllipse(playerColors[i], rect);
            }       
        }

        private void DrawField(Graphics graphics)
        {
            graphics.Clear(boardColor);
            for (int row = 0; row < cil; ++row)
            {
                int y = row * (cellSize + spaceSize) + startCoord;
                for (int column = 0; column < cil; ++column)
                {                 
                    int x = column * (cellSize + spaceSize) + startCoord;
                    graphics.FillRectangle(cellBrush, x, y, cellSize, cellSize);
                }
                int x1 = startCoord - 2 * cellSize - spaceSize;
                int x2 = startCoord + boardSize;
                graphics.FillRectangle(cellBrush, y, x1, cellSize, cellSize * 2);
                graphics.FillRectangle(cellBrush, y, x2, cellSize, cellSize * 2);
                graphics.FillRectangle(cellBrush, x1, y, cellSize * 2, cellSize);
                graphics.FillRectangle(cellBrush, x2, y, cellSize * 2, cellSize);
            }
        }
    }
}
