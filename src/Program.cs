using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ilf.pgn;
using ilf.pgn.Data;
using ilf.pgn.Data.MoveText;


namespace PgnTool
{
    class Program
    {
        static int Main(string[] args)
        {


            return CommandLine.Parser.Default.ParseArguments<CropOptions>(args)
              .MapResult(
                (CropOptions opts) => CropFile(opts),
                errs => 1);
        }

        static Database ReadPgn(string fileName)
        {
            var reader = new PgnReader();
            return reader.ReadFromFile(fileName);
        }

        static Func<CropOptions, int> CropFile = (opts) =>
       {
           var inputDb = ReadPgn(opts.InputPgnPath);
           StringBuilder output = new StringBuilder();
           var keepCount = opts.MovesToKeep;
           foreach (var entry in inputDb.Games)
           {
               StringBuilder sb = new StringBuilder();
               var newMoveText = entry.MoveText.Take(keepCount).ToList();
               entry.MoveText = new MoveTextEntryList();
               entry.MoveText.AddRange(newMoveText);
           }
           WriteOutputFile(inputDb, opts);
           return 0;
       };



        private static void WriteOutputFile(Database db, CommandLineOptions opts)
        {
            if (opts.OutputPgnPath == string.Empty)
            {
                foreach (var entry in db.Games)
                {
                    Console.WriteLine(entry);
                }
            }
            else
            {
                WriteOutputFile(db, opts.OutputPgnPath);
            }
        }

        private static void WriteOutputFile(Database db, string outputPgnPath)
        {
            var sb = new StringBuilder();
            foreach (var game in db.Games)
            {
                var fen = new GameInfo("FEN", game.BoardSetup.GetFen());
                if (game.Tags["FEN"] != null)
                {
                    if (game.AdditionalInfo.Where(x => x.Name == "FEN").Any())
                    {
                        var info = game.AdditionalInfo.FirstOrDefault(x => x.Name == "FEN");
                        info = fen;
                    }
                    else
                    {
                        game.AdditionalInfo.Add(fen);
                    }
                    sb.AppendLine(game.ToString());
                }
            }
            System.IO.File.WriteAllText(outputPgnPath, sb.ToString());
        }
    }
    public static class GameHelper
    {
        private static string PieceToString(Piece p)
        {
            if (p == null) return "   ";

            var str = "";
            switch (p.PieceType)
            {
                case PieceType.Pawn:
                    str = " p ";
                    break;
                case PieceType.Knight:
                    str = " n ";
                    break;
                case PieceType.Bishop:
                    str = " b ";
                    break;
                case PieceType.Rook:
                    str = " r ";
                    break;
                case PieceType.Queen:
                    str = " q ";
                    break;
                case PieceType.King:
                    str = " k ";
                    break;
            }

            if (p.Color == Color.White)
                return str.ToUpper();

            return str;
        }
        public static string GetFen(this BoardSetup b)
        {
            string output = "";

            Piece piece;
            string row, pieceStr, castleStr, epStr;
            int emptySquares;

            for (int iRank = 0; iRank < 8; ++iRank)
            {
                row = "";
                emptySquares = 0;
                for (int iFile = 0; iFile < 8; ++iFile)
                {
                    piece = b[iFile, iRank];
                    if (piece == null)
                    {
                        emptySquares += 1;
                    }
                    else
                    {
                        if (emptySquares > 0) row += emptySquares.ToString();
                        pieceStr = PieceToString(b[iFile, iRank]);
                        row += pieceStr.Trim();
                        emptySquares = 0;
                    }
                    if (iFile > 0 & iFile % 7 == 0)
                    {
                        if (emptySquares > 0) row += emptySquares.ToString();
                        output += row;
                        if (iRank < 7) output += "/";
                    }
                }
            }

            if (b.IsWhiteMove)
                output += " w";
            else
                output += " b";

            castleStr = " ";
            if (b.CanWhiteCastleKingSide)
                castleStr += "K";
            if (b.CanWhiteCastleQueenSide)
                castleStr += "Q";
            if (b.CanBlackCastleKingSide)
                castleStr += "k";
            if (b.CanBlackCastleQueenSide)
                castleStr += "q";

            if (castleStr.Length > 1)
                output += castleStr;
            else
                output += " -";

            epStr = "-";
            if (b.EnPassantSquare != null)
                epStr = b.EnPassantSquare.ToString();
            output += " " + epStr;

            output += " " + b.HalfMoveClock.ToString();
            output += " " + b.FullMoveCount.ToString();

            return output;
        }
    }
}
