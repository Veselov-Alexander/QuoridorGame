using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Quoridor
{
    class Quoridor
    {
        private int players;
        private int cil;
        private Point[] playerPositions { get; }
        private int[] tilesCount { get; }
        private int[,] last;
        private bool[,,] tiles { get; }

        private int currentPlayer;
        private int winner;
        private bool gameIsEnded;

        public Quoridor(int cil, int players)
        {
            this.players = players;
            this.cil = cil;
            tiles = new bool[cil - 1, cil - 1, 2];
            playerPositions = new Point[players];
            tilesCount = new int[players];
            for (int i = 0; i < players; ++i)
            {
                tilesCount[i] = 2 * (cil + 1) / players;
            }
            playerPositions[0] = new Point(cil - 1, cil / 2);
            playerPositions[1] = new Point(0, cil / 2);
            if (players == 4)
            {
                playerPositions[2] = new Point(cil / 2, cil - 1);
                playerPositions[3] = new Point(cil / 2, 0);
            }
            last = new int[,]{ { 0, -1 }, { cil - 1, -1 }, { -1, 0 }, { -1, cil - 1 } };
            currentPlayer = 0;
            gameIsEnded = false;
        }

        public Point[] GetPlayerPositions()
        {
            return playerPositions;
        }

        public int[] GetPlayerTiles()
        {
            return tilesCount;
        }

        public bool[,,] GetFieldTiles()
        {
            return tiles;
        }

        public int GetCurrentPlayer()
        {
            return currentPlayer;
        }

        public bool GameIsEnded()
        {
            return gameIsEnded;
        }

        public int GetWinner()
        {
            return winner;
        }

        public List<Point> GetAvailableMoves(int player)
        {
            Point currentPosition = playerPositions[currentPlayer];
            return GetAvailableMoves(currentPosition.X, currentPosition.Y);
        }

        public List<Point> GetAvailableMoves(int x, int y)
        {
            List<Point> points = new List<Point>();

            for (int row = x - 2; row <= x + 2; ++row)
            {
                for (int column = y - 2; column <= y + 2; ++column)
                {
                    if (MoveIsAvailable(x, y, row, column))
                    {
                        points.Add(new Point(row, column));
                    }
                }
            }
            return points;
        }

        public bool Move(int row, int column)
        {
            Point currentPosition = playerPositions[currentPlayer];
            if (MoveIsAvailable(currentPosition.X, currentPosition.Y, row, column))
            {
                playerPositions[currentPlayer] = new Point(row, column);
                currentPlayer = (currentPlayer + 1) % players;
                CheckGameEnd();
                return true;
            }
            return true;
        }

        public bool SetTile(int row, int column, int dir)
        {
            if (CheckTile(row, column, dir) && tilesCount[currentPlayer] != 0)
            {
                tiles[row, column, dir] = true;
                --tilesCount[currentPlayer];
                currentPlayer = (currentPlayer + 1) % players;
                return true;
            }
            return false;
        }

        private void CheckGameEnd()
        {
            for (int i = 0; i < players; ++i)
            {
                if (playerPositions[i].X == last[i, 0] || playerPositions[i].Y == last[i, 1])
                {
                    gameIsEnded = true;
                    winner = i;                  
                    break;
                }
            }
        }

        private bool MoveIsAvailable(int x1, int y1, int x2, int y2)
        {
            if (x2 < 0 || y2 < 0 || x2 >= cil || y2 >= cil || !CellIsFree(x2, y2))
            {
                return false;
            }

            int dr = x1 - x2;
            int dc = y1 - y2;

            if (Math.Abs(dr) == 2 && Math.Abs(dc) == 0 || 
                Math.Abs(dr) == 0 && Math.Abs(dc) == 2)
            {
                int cx = x1 - dr / 2;
                int cy = y1 - dc / 2;
                if (!CellIsFree(cx, cy) && PassageExists(x1, y1, cx, cy) && PassageExists(cx, cy, x2, y2))
                {
                    return true;
                }
            }
            
            if (Math.Abs(dr) == 1 && Math.Abs(dc) == 1)
            {
                if (!PassageExists(x1, y1 - dc, x1, y1 - dc * 2))
                {
                    if (!CellIsFree(x1, y1 - dc) && CellIsFree(x2, y2) && PassageExists(x1, y1 - dc, x2, y2))
                    {
                        return true;
                    }
                }
                if (!PassageExists(x1 - dr, y1, x1 - dr * 2, y1))
                {
                    if (!CellIsFree(x1 - dr, y1) && CellIsFree(x2, y2) && PassageExists(x1 - dr, y1, x2, y2))
                    {
                        return true;
                    }
                }
            }

            return PassageExists(x1, y1, x2, y2);
        }

        private bool PassageExists(int x1, int y1, int x2, int y2)
        {
            int dr = x1 - x2;
            int dc = y1 - y2;
            if (Math.Abs(dr) + Math.Abs(dc) != 1)
            {
                return false;
            }

            if (Math.Abs(dr) == 1)
            {
                if (dr > 0)
                {
                    // up
                    if (GetTile(x2, y2, 1) || GetTile(x2, y2 - 1, 1))
                    {
                        return false;
                    }
                }
                else
                {
                    // down
                    if (GetTile(x2 - 1, y2, 1) || GetTile(x2 - 1, y2 - 1, 1))
                    {
                        return false;
                    }
                }
            }

            if (Math.Abs(dc) == 1)
            {
                if (dc > 0)
                {
                    // left
                    if (GetTile(x2, y2, 0) || GetTile(x2 - 1, y2, 0))
                    {
                        return false;
                    }
                }
                else
                {
                    // right
                    if (GetTile(x2, y2 - 1, 0) || GetTile(x2 - 1, y2 - 1, 0))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool GetTile(int row, int column, int dir)
        {
            if (row < 0 || column < 0 || 
                row >= cil - 1 || column >= cil - 1 || 
                dir < 0 || dir > 2)
            {
                return false;
            }
            return tiles[row, column, dir];
        }

        private bool CellIsFree(int row, int column)
        {
            for (int i = 0; i < players; ++i)
            {
                Point point = playerPositions[i];
                if (point.Y == column && point.X == row)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckTile(int row, int column, int dir)
        {
            if (tiles[row, column, dir])
            {
                return false;
            }
            tiles[row, column, dir] = true;
            bool pathExists = PathExists();
            tiles[row, column, dir] = false;

            if (!pathExists)
            {
                return false;
            }

            if (dir == 1)
            {
                bool toTheLeft = false, toTheRight = false;
                if (column - 1 >= 0 && tiles[row, column - 1, dir])
                {
                    toTheLeft = true;
                }
                if (column + 1 < cil - 1 && tiles[row, column + 1, dir])
                {
                    toTheRight = true;
                }
                if (!toTheLeft && !toTheRight && !tiles[row, column, 0])
                {                   
                    return true;
                }
            }
            if (dir == 0)
            {
                bool toTheUp = false, toTheDown = false;
                if (row - 1 >= 0 && tiles[row - 1, column, dir])
                {
                    toTheUp = true;
                }
                if (row + 1 < cil - 1 && tiles[row + 1, column, dir])
                {
                    toTheDown = true;
                }
                if (!toTheUp && !toTheDown && !tiles[row, column, 1])
                {
                    return true;
                }
            }
            return false;
        }

        private bool PathExists()
        {         
            for (int i = 0; i < players; ++i)
            {
                bool[,] visited = new bool[cil, cil];
                if (!DFS(playerPositions[i].X, playerPositions[i].Y, last[i, 0], last[i, 1], visited))
                {
                    return false;
                }
            }
            return true;
        }

        private bool DFS(int x, int y, int lastX, int lastY, bool[,] visited)
        {
            
            if (y == lastY || x == lastX)
            {
                return true;
            }

            if (visited[x, y])
            {
                return false;
            }
            visited[x, y] = true;
            var points = GetAvailableMoves(x, y);
            foreach (var point in points)
            {
                if (DFS(point.X, point.Y, lastX, lastY, visited))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
