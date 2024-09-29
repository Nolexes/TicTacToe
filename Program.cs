
using System.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        char[] board = { '1', '2', '3', '4', '5', '6', '7', '8', '9' }; // Das Spielfeld wird in 9 Felder eigeteilt (Angezeigt mit 1-9 addressiert mit 0-8)

        List<(int, int, int)> WinningStates = new List<(int, int, int)> //Alle möglichen Gewinnkombinationen bei dem gegebenen Feld
        {
            (0, 1, 2),
            (3, 4, 5),
            (6, 7, 8),
            (0, 3, 6),
            (1, 4, 7),
            (2, 5, 8),
            (0, 4, 8),
            (2, 4, 6)

        };

        bool gameRunning = true; //Hält das spiel am laufen 
        bool replayb = true; //Außenloop falls replay erwünscht

        while (replayb)
        {
            ResetBoard(board);
            Console.Clear();

            while (gameRunning)
            {

                DrawBoard(board); // Wir geben das Startfeld aus

                GetInput(board); //Der Spieler wählt nun ein Feld aus.
                Console.Clear();

                if (CheckWin(board, WinningStates)) //Nach jedem Zug des Spielers wird überprüft ob gewonnen wurde 
                {
                    DrawBoard(board);
                    Console.WriteLine("Spieler gewinnt!");
                    gameRunning = false;
                    break;
                }

                if (IsBoardFull(board)) //Nach jedem Zug des Spielers wird überprüft ob das Spielfeld voll ist
                {
                    DrawBoard(board);
                    Console.WriteLine("Unentschieden!");
                    gameRunning = false;
                    break;
                }


                BotPlay(board, WinningStates);
                Console.Clear();

                if (CheckWin(board, WinningStates)) //Selbiges nach dem Zug des PC
                {
                    DrawBoard(board);
                    Console.WriteLine("Computer gewinnt!");
                    gameRunning = false;
                    break;
                }

                if (IsBoardFull(board))
                {
                    DrawBoard(board);
                    Console.WriteLine("Unentschieden!");
                    gameRunning = false;
                    break;
                }
            }
            PlayAgain(ref gameRunning, ref replayb); // Überprüft ob der Spieler das Spiel beenden oder erneut spielen will (bools müssen als ref übergeben werden)
        }

        Console.WriteLine("Danke fürs Spielen :)");
    }



    static void DrawBoard(char[] board) // malt das Spielfeld funktioniert auch mit updates
    {
        Console.WriteLine($"{board[0]} | {board[1]} | {board[2]}");
        Console.WriteLine("---------");
        Console.WriteLine($"{board[3]} | {board[4]} | {board[5]}");
        Console.WriteLine("---------");
        Console.WriteLine($"{board[6]} | {board[7]} | {board[8]}");


    }

    static bool CheckWin(char[] board, List<(int, int, int)> WinningStates) // checkt alle möglichen kombinationen nach 3 gleichen zeichen 
    {

        for (int i = 0; i < WinningStates.Count; i++)
        {
            if (board[WinningStates[i].Item1] == board[WinningStates[i].Item2] && board[WinningStates[i].Item2] == board[WinningStates[i].Item3]) //Dieser Loop geht alle möglichen WinningStates durch und überprüft obe einer der Fälle eingetreten ist
            {
                return true;
            }
        }

        return false;
    }

    static void GetInput(char[] board) // Fordert so lange Input bis eins der Felder eingegeben wurde braucht noch ne Option um zwischen X und O zu switchen
    {
        bool check = true;

        while (check)
        {
            Console.Write($"Gebe das Feld ein das du belegen möchtest (1-9): ");
            string input = Console.ReadLine();

            if (char.TryParse(input, out char slot) && slot != 'X' && slot != 'O') //Hier wird überprüft ob eine gültige Nummer eingegeben wurde (In dem Fall als char) außerdem wird überprüft das weder X noch 0 eingegeben wurden da die nicht erneut belegt werden dürfen
            {
                if (Array.Exists(board, element => element == slot)) //Hier wird überprüft das der Platz im Array noch existiert und nicht bereits ersetzt wurde und falls er frei ist mit X belegt
                {
                    board[int.Parse(input) - 1] = 'X';
                    check = false;
                }
                else
                {
                    Console.WriteLine("Stelle ist schon belegt");
                }
            }
            else
            {
                Console.WriteLine("Ungültige Eingabe");
            }
        }
    }

    static void BotPlay(char[] board, List<(int, int, int)> WinningStates)
    {

        // Überprüft ob der Computer gewinnen kann
        int winMove = GetWinningMove(board, 'O', WinningStates);
        if (winMove != -1)
        {
            board[winMove] = 'O';
            return;
        }

        // Überprüft ob spieler geblockt werden kann
        int blockMove = GetWinningMove(board, 'X', WinningStates);
        if (blockMove != -1)
        {
            board[blockMove] = 'O';
            return;
        }


        Random rand = new Random(); //Zufälliger Move auf einen nicht belegten Slot
        while (true)
        {
            int play = rand.Next(board.Length);
            if (board[play] != 'X' && board[play] != 'O') //Überprüft das nur Felder genutzt werden die noch nicht belegt sind
            {
                board[play] = 'O';
                break;
            }
        }


    }

    static void PlayAgain(ref bool gameRunning, ref bool replayb) // Überprüft ob der Spieler erneut spielen will (ref ist damit die bools außerhalb der Funktion geupdatet werden können)
    {
        bool state = true;

        while (state) {

            Console.WriteLine("Do you want to play again? (y/n): ");
            string answer = Console.ReadLine().ToLower();

            if (char.TryParse(answer, out char replay))
            {
                if (replay == 'y')
                {
                    gameRunning = true;
                    state = false;
                }
                else if (replay == 'n')
                {
                    replayb = false;
                    state = false;

                }

            }
        }

    }

    static bool IsBoardFull(char[] board) // Überprüft ob das Spielfeld voll ist und das Spiel somit unentschieden
    {
        return board.All(spot => spot == 'X' || spot == 'O'); //Wenn alle Felder entweder X oder 0 sind ist das Spielfeld voll 
    }

    static int GetWinningMove(char[] board, char player, List<(int, int, int)> WinningStates) // Überprüft ob entweder Spieler oder Computer kurz davor ist zu gewinnen (für player wird X oder O eingesetzt)
    {
        foreach (var state in WinningStates)
        {
            if (board[state.Item1] == player && board[state.Item2] == player && board[state.Item3] != 'X' && board[state.Item3] != 'O')
                return state.Item3;
            if (board[state.Item1] == player && board[state.Item3] == player && board[state.Item2] != 'X' && board[state.Item2] != 'O')
                return state.Item2;
            if (board[state.Item2] == player && board[state.Item3] == player && board[state.Item1] != 'X' && board[state.Item1] != 'O')
                return state.Item1;
        }
        return -1; // Falls kein Move gefunden wird
    }

    static void ResetBoard(char[] board) //Bringt das Board auf den ursprünglichen Zustand zurück
    {
    foreach (char c in board)
        {
            for (int i = 1; i <= board.Length; i++)
            {
                string fill = i.ToString();
                board[i - 1] = char.Parse(fill);
            }
        }
    }
}

