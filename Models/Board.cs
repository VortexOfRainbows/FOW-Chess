using Chess.Animations;
using Chess.IO;
using Chess.Managers;
using Chess.Models.Pieces;
using Microsoft.VisualBasic.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Models
{
    enum Turn
    {
        Player1,
        Player2,
        Player1End,
        Player2End
    }
    enum ChessColor
    {
        Black,
        White
    }
    class Board
    {
        MarkableButtonPanel whites;
        MarkableButtonPanel blacks;

        Sprite2D[,] grid;
        CloudGroup[,] clouds;
        Piece[,] board;
        public Turn Turn { get; private set; } = Turn.Player1;

        public Piece LastPieceMoved;

        public Board()
        {
            Texture2D gridSquares = ContentService.Instance.Textures["Empty"];
            grid = new Sprite2D[8, 8];
            clouds = new CloudGroup[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    grid[i, j] = new Sprite2D(gridSquares, new Rectangle(j * Constants.TILESIZE, i * Constants.TILESIZE, Constants.TILESIZE, Constants.TILESIZE), Color.DarkGray);
                    if ((i + j) % 2 == 0) grid[i, j].Color = Color.White;
                    clouds[i, j] = new CloudGroup(new Point(i, j));
                }
            }
            InitializePieces();
            EndOfTurnCalculations(true); 
            ReverseTurnEndLogic = false;
        }

        private void InitializePieces()
        {
            board = new Piece[8, 8];

            whites = new MarkableButtonPanel();
            blacks = new MarkableButtonPanel();

            int pieceSize = Constants.PIESESIZE;
            int markSize = Constants.MARKED_PIESESIZE;
            for (int i = 0; i < 8; i++)
            {
                Piece temp = new Pawn(new Sprite2D(ContentService.Instance.Textures["WhitePawn"], new Rectangle(0, 0, pieceSize, pieceSize)), 6, i, ChessColor.White,this);
                temp.Center(grid[6, i].Bounds);
                board[6, i] = temp;
            }
            Piece x = new Rook(new Sprite2D(ContentService.Instance.Textures["WhiteRook"], new Rectangle(0, 0, pieceSize, pieceSize)), 7, 0, ChessColor.White, this);
            x.Center(grid[7, 0].Bounds);
            board[7, 0] = x;
            x = new Rook(new Sprite2D(ContentService.Instance.Textures["WhiteRook"], new Rectangle(0, 0, pieceSize, pieceSize)), 7, 7, ChessColor.White,this);
            x.Center(grid[7, 7].Bounds);
            board[7, 7] = x;
            x = new Knight(new Sprite2D(ContentService.Instance.Textures["WhiteKnight"], new Rectangle(0, 0, pieceSize, pieceSize)), 7, 1, ChessColor.White,this);
            x.Center(grid[7, 1].Bounds);
            board[7, 1] = x;
            x = new Knight(new Sprite2D(ContentService.Instance.Textures["WhiteKnight"], new Rectangle(0, 0, pieceSize, pieceSize)), 7, 6, ChessColor.White,this);
            x.Center(grid[7, 6].Bounds);
            board[7, 6] = x;
            x = new Bishop(new Sprite2D(ContentService.Instance.Textures["WhiteBishop"], new Rectangle(0, 0, pieceSize, pieceSize)), 7, 2, ChessColor.White,this);
            x.Center(grid[7, 2].Bounds);
            board[7, 2] = x;
            x = new Bishop(new Sprite2D(ContentService.Instance.Textures["WhiteBishop"], new Rectangle(0, 0, pieceSize, pieceSize)), 7, 5, ChessColor.White,this);
            x.Center(grid[7, 5].Bounds);
            board[7, 5] = x;
            x = new Queen(new Sprite2D(ContentService.Instance.Textures["WhiteQueen"], new Rectangle(0, 0, pieceSize, pieceSize)), 7, 3, ChessColor.White,this);
            x.Center(grid[7, 3].Bounds);
            board[7, 3] = x;
            x = new King(new Sprite2D(ContentService.Instance.Textures["WhiteKing"], new Rectangle(0, 0, pieceSize, pieceSize)), 7, 4, ChessColor.White,this);
            x.Center(grid[7, 4].Bounds);
            board[7, 4] = x;



            for (int i = 0; i < 8; i++)
            {
                Piece temp = new Pawn(new Sprite2D(ContentService.Instance.Textures["BlackPawn"], new Rectangle(0, 0, pieceSize, pieceSize)), 1, i, ChessColor.Black,this);
                temp.Center(grid[1, i].Bounds);
                board[1, i] = temp;
            }
            x = new Rook(new Sprite2D(ContentService.Instance.Textures["BlackRook"], new Rectangle(0, 0, pieceSize, pieceSize)), 0, 0, ChessColor.Black,this);
            x.Center(grid[0, 0].Bounds);
            board[0, 0] = x;
            x = new Rook(new Sprite2D(ContentService.Instance.Textures["BlackRook"], new Rectangle(0, 0, pieceSize, pieceSize)), 0, 7, ChessColor.Black,this);
            x.Center(grid[0, 7].Bounds);
            board[0, 7] = x;
            x = new Knight(new Sprite2D(ContentService.Instance.Textures["BlackKnight"], new Rectangle(0, 0, pieceSize, pieceSize)), 0, 1, ChessColor.Black,this);
            x.Center(grid[0, 1].Bounds);
            board[0, 1] = x;
            x = new Knight(new Sprite2D(ContentService.Instance.Textures["BlackKnight"], new Rectangle(0, 0, pieceSize, pieceSize)), 0, 6, ChessColor.Black,this);
            x.Center(grid[0, 6].Bounds);
            board[0, 6] = x;
            x = new Bishop(new Sprite2D(ContentService.Instance.Textures["BlackBishop"], new Rectangle(0, 0, pieceSize, pieceSize)), 0, 2, ChessColor.Black,this);
            x.Center(grid[0, 2].Bounds);
            board[0, 2] = x;
            x = new Bishop(new Sprite2D(ContentService.Instance.Textures["BlackBishop"], new Rectangle(0, 0, pieceSize, pieceSize)), 0, 5, ChessColor.Black,this);
            x.Center(grid[0, 5].Bounds);
            board[0, 5] = x;
            x = new Queen(new Sprite2D(ContentService.Instance.Textures["BlackQueen"], new Rectangle(0, 0, pieceSize, pieceSize)), 0, 3, ChessColor.Black,this);
            x.Center(grid[0, 3].Bounds);
            board[0, 3] = x;
            x = new King(new Sprite2D(ContentService.Instance.Textures["BlackKing"], new Rectangle(0, 0, pieceSize, pieceSize)), 0, 4, ChessColor.Black,this);
            x.Center(grid[0, 4].Bounds);
            board[0, 4] = x;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] == null) continue;
                    board[i, j].MarkAnimation = new ButtonAnimation(null, new Rectangle(board[i, j].Bounds.Location, new Point(markSize, markSize)), null, true);
                    board[i, j].UnMarkAnimation = new ButtonAnimation(null, new Rectangle(board[i, j].Bounds.Location, new Point(pieceSize, pieceSize)), null, true);
                    if (board[i, j].ChessColor == ChessColor.Black) blacks.Add(board[i, j]);
                    else whites.Add(board[i, j]);
                }
            }
        }
        int EndTurnCounter = -1;
        bool moveMade = false;
        internal void Update(GameTime gameTime, Input curInput, Input prevInput)
        {
            switch (Turn)
            {
                case Turn.Player2:
                    blacks.Update(curInput, prevInput);
                    //Use stockfish to calculate next oponent move
                    //https://github.com/official-stockfish/Stockfish
                    break;
                case Turn.Player1:
                    whites.Update(curInput, prevInput);
                    break;
            }
            foreach(CloudGroup cloud in clouds)
            {
                cloud.Update();
            }
            if (moveMade)
            {
                ReverseTurnEndLogic = true;
                UpdateVisibility();
                ReverseTurnEndLogic = false;
                EndTurnCounter = 60 * 10;
                moveMade = false;
            }
            if (EndTurnCounter > 0)
            {
                whites.UnmarkAll();
                blacks.UnmarkAll();
                EndTurnCounter--;
            }
            bool nextTurn = false;
            if (EndTurnCounter == 0)
            {
                whites.UnmarkAll();
                blacks.UnmarkAll();
                Piece.VisiblePoints = new HashSet<Point>();
                if (Turn == Turn.Player2)
                    Turn = Turn.Player2End;
                else if(Turn == Turn.Player1)
                    Turn = Turn.Player1End;
                if(!prevInput.Keyboard.IsKeyDown(Keys.Space) && curInput.Keyboard.IsKeyDown(Keys.Space))
                {
                    nextTurn = true;
                }
            }
            else if(EndTurnCounter > 0)
            {
                if (!prevInput.Keyboard.IsKeyDown(Keys.Space) && curInput.Keyboard.IsKeyDown(Keys.Space))
                {
                    EndTurnCounter = 0;
                }
            }
            if(nextTurn)
            {
                if (Turn == Turn.Player1End)
                    Turn = Turn.Player2;
                else if (Turn == Turn.Player2End)
                    Turn = Turn.Player1;
                ReverseTurnEndLogic = true;
                UpdateVisibility();
                ReverseTurnEndLogic = false;
                EndTurnCounter = -1;
            }
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    grid[i, j].Draw(spriteBatch); //THIS DRAWS THE SQUARES
                    if((Piece.PointsThatLeadToCheck.Contains(new Point(i, j)) || Piece.FuturePointsThatLeadToCheck.Contains(new Point(i, j))) && Turn != Turn.Player1End && Turn != Turn.Player2End)
                    {
                        Texture2D circle = ContentService.Instance.Textures["Pinned"];
                        Vector2 origin = new Vector2(circle.Width / 2, circle.Height / 2);
                        Vector2 drawCenter = grid[i, j].Location + new Vector2(grid[i, j].Width, grid[i, j].Height) * 0.5f;
                        spriteBatch.Draw(circle, drawCenter, null, Color.Red * 0.9f, 0f, origin, 0.5f, SpriteEffects.None, 0f);
                    }
                    else if ((Piece.VisiblePointsThatLeadToCheck.Contains(new Point(i, j)) || Piece.FutureVisiblePointsThatLeadToCheck.Contains(new Point(i, j))) && Turn != Turn.Player1End && Turn != Turn.Player2End)
                    {
                        Texture2D circle = ContentService.Instance.Textures["Danger"];
                        Vector2 origin = new Vector2(circle.Width / 2, circle.Height / 2);
                        Vector2 drawCenter = grid[i, j].Location + new Vector2(grid[i, j].Width, grid[i, j].Height) * 0.5f;
                        spriteBatch.Draw(circle, drawCenter, null, Color.Red * 0.9f, 0f, origin, 0.5f, SpriteEffects.None, 0f);
                    }
                }
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Vector2 drawCenter = grid[i, j].Location + new Vector2(grid[i, j].Width, grid[i, j].Height) * 0.5f;
                    clouds[i, j].Draw(drawCenter, spriteBatch);
                }
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Vector2 drawCenter = grid[i, j].Location + new Vector2(grid[i, j].Width, grid[i, j].Height) * 0.5f;
                    clouds[i, j].Draw(drawCenter, spriteBatch, true);
                }
            }
            whites.Draw(spriteBatch); //THIS DRAWS THE PIECES
            blacks.Draw(spriteBatch);
            Vector2 bottomLeft = new Vector2(Constants.TILESIZE * 1, Constants.TILESIZE * 7);
            if (EndTurnCounter > 0)
            {
                DrawNumber(bottomLeft, spriteBatch, EndTurnCounter / 60);
            }
            else if(EndTurnCounter == 0)
            {
                bottomLeft.X += 60;
                bottomLeft.Y -= 40;
                int frameNum = 0;
                if(Turn.Player2End == Turn)
                {
                    frameNum = 1;
                }
                Texture2D spaceBar = ContentService.Instance.Textures["Ready"];
                Vector2 origin = new Vector2(spaceBar.Width / 2, spaceBar.Height / 4);
                Rectangle frame = new Rectangle(0, 200 * frameNum, 200, 200);
                for (int i = 0; i < 20; i++)
                {
                    Vector2 circular = new Vector2(7, 0).RotatedBy(MathHelper.ToRadians(i * 18));
                    spriteBatch.Draw(spaceBar, bottomLeft + circular, frame, Color.Black, 0f, origin, 1f, SpriteEffects.None, 0f);
                }
                spriteBatch.Draw(spaceBar, bottomLeft, frame, Color.WhiteSmoke, 0f, origin, 1f, SpriteEffects.None, 0f);
            }
        }
        public void DrawNumber(Vector2 pos, SpriteBatch spriteBatch, int num)
        {
            Texture2D numbers = ContentService.Instance.Textures["Numbers"];
            Texture2D spaceBar = ContentService.Instance.Textures["Spacebar"];
            Vector2 origin = new Vector2(numbers.Width / 2, numbers.Height / 20);
            Rectangle frame = new Rectangle(0, 200 * num, 200, 200);
            for(int i = 0; i < 20; i++)
            {
                Vector2 circular = new Vector2(7, 0).RotatedBy(MathHelper.ToRadians(i * 18));
                spriteBatch.Draw(numbers, pos + circular, frame, Color.Black, 0f, origin, 0.5f, SpriteEffects.None, 0f);
                spriteBatch.Draw(spaceBar, pos + new Vector2(120, 0) + circular, null, Color.Black, 0f, origin, 1f, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(numbers, pos, frame, Color.WhiteSmoke, 0f, origin, 0.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(spaceBar, pos + new Vector2(120, 0), null, Color.WhiteSmoke, 0f, origin, 1f, SpriteEffects.None, 0f);
        }
        public bool IsEmpty(int r, int c)
        {
            return board[r,c] == null;
        }

        /// <summary>
        /// Checks if a move is legal in the sense that the move does not threaten own king
        /// Does not check if the move itself follows the rules of chess
        /// </summary>
        public bool IsLegalMove(Piece p, int tR, int tC)
        {
            Piece.CheckIsForFutureMoves = true;
            bool ret = false;
            if(IsEmpty(tR, tC) || board[tR, tC].ChessColor != p.ChessColor || board[tR, tC] == p)
            {
                Piece temp = board[tR,tC];
                board[tR, tC] = p;
                board[p.Row, p.Col] = null;
                ret = true;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (board[i, j] == null) continue;
                        if (board[i, j].ChessColor != p.ChessColor && board[i, j].SetsCheck())
                        {
                            ret = false;
                        }
                    }
                }
                board[p.Row, p.Col] = p;
                board[tR, tC] = temp;
            }
            Piece.CheckIsForFutureMoves = false;
            if (UpdateVisibilityThisCycle)
            {
                if ((!ReverseTurnEndLogic && !p.IsNextPlayersPiece()) || (ReverseTurnEndLogic && p.IsNextPlayersPiece()))
                    Piece.VisiblePoints.Add(new Point(tR, tC));
            }
            return ret;
        }

        /// <summary>
        /// Checks if a move is legal in the sense that the move does not threaten own king
        /// Does not check if the move itself follows the rules of chess
        /// This method starts by removing p2 from the board before the check
        /// </summary>
        public bool IsLegalMove(Piece p, int tR, int tC, Piece p2)
        {
            board[p2.Row, p2.Col] = null;
            bool ret = IsLegalMove(p, tR, tC);
            board[p2.Row, p2.Col] = p2;
            return ret;
        }

        public void Move(Piece p, int tr, int tc)
        {
            int r = p.Row;
            int c = p.Col;
            LastPieceMoved = p;
            if (!IsEmpty(tr, tc))
            {
                if (p.ChessColor == ChessColor.Black)
                {
                    whites.Remove(board[tr, tc]);
                }
                else
                {
                    blacks.Remove(board[tr, tc]);
                }
            }
            board[tr, tc] = board[r, c];
            board[r, c] = null;
            board[tr, tc].Col = tc;
            board[tr, tc].Row = tr;
            if (board[tr, tc].ChessPiece == ChessPiece.Pawn && (board[tr, tc].Row == 0 || board[tr, tc].Row == 7))
            {
                int col = tc;
                int row = tr;
                if (p.ChessColor == ChessColor.Black)
                {
                    Queen piece = new Queen(
                        new Sprite2D(
                            ContentService.Instance.Textures["BlackQueen"],
                            new Rectangle(col * Constants.TILESIZE, row * Constants.TILESIZE, Constants.PIESESIZE, Constants.PIESESIZE)
                            ), row, col, p.ChessColor, this);
                    blacks.Add(piece);
                    blacks.Remove(board[row, col]);
                    board[row, col] = piece;
                    piece.MarkAnimation = new ButtonAnimation(null, new Rectangle(board[row, col].Bounds.Location, new Point(Constants.MARKED_PIESESIZE, Constants.MARKED_PIESESIZE)), null, true);
                    piece.UnMarkAnimation = new ButtonAnimation(null, new Rectangle(board[row, col].Bounds.Location, new Point(Constants.PIESESIZE, Constants.PIESESIZE)), null, true);
                }
                else
                {
                    Queen piece = new Queen(
                        new Sprite2D(
                            ContentService.Instance.Textures["WhiteQueen"],
                            new Rectangle(col * Constants.TILESIZE, row * Constants.TILESIZE, Constants.PIESESIZE, Constants.PIESESIZE)
                            ), row, col, p.ChessColor, this);
                    whites.Add(piece);
                    whites.Remove(board[row, col]);
                    board[row, col] = piece;
                    piece.MarkAnimation = new ButtonAnimation(null, new Rectangle(board[row, col].Bounds.Location, new Point(Constants.MARKED_PIESESIZE, Constants.MARKED_PIESESIZE)), null, true);
                    piece.UnMarkAnimation = new ButtonAnimation(null, new Rectangle(board[row, col].Bounds.Location, new Point(Constants.PIESESIZE, Constants.PIESESIZE)), null, true);
                }
            }
            EndOfTurnCalculations(false);
            moveMade = true;

        }
        public bool ReverseTurnEndLogic = true;
        public bool UpdateVisibilityThisCycle = false;
        public void EndOfTurnCalculations(bool updateVis)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] != null)
                    {
                        board[i, j].CalculateLegalMoves();
                    }
                }
            }
            if(updateVis)
            {
                UpdateVisibility();
            }
        }
        public void UpdateCheckList()
        {
            Piece.ResetCheckList();
            Piece.IsUpdatingCheckDisplay = true;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] != null)
                    {
                        board[i, j].CalculateLegalMoves();
                        board[i, j].SetsCheck();
                    }
                }
            }
            Piece.IsUpdatingCheckDisplay = false;
        }
        public void UpdateVisibility()
        {
            Piece.VisiblePoints = new HashSet<Point>();
            UpdateVisibilityThisCycle = true;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] != null)
                    {
                        board[i, j].CalculateLegalMoves();
                        if ((!ReverseTurnEndLogic && !board[i, j].IsNextPlayersPiece()) || (ReverseTurnEndLogic && board[i, j].IsNextPlayersPiece()))
                        {
                            for (int i2 = -1; i2 <= 1; i2++)
                            {
                                for (int j2 = -1; j2 <= 1; j2++)
                                {
                                    Point p2 = new Point(i + i2, j + j2);
                                    if (!Piece.VisiblePoints.Contains(p2) && InGrid(p2.X, p2.Y))
                                    {
                                        Piece.VisiblePoints.Add(p2);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            UpdateVisibilityThisCycle = false;
            UpdateCheckList();
        }
        public bool InGrid(int r, int c)
        {
            return r >= 0 && r < 8 && c >= 0 && c < 8;
        }

        public Piece GetPiece(int r, int c)
        {
            return board[r, c];
        }
    }
}
