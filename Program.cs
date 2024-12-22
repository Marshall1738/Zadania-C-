using System;
using System.Collections.Generic;

// Interfejs IPlayer
public interface IPlayer
{
    string Name { get; }
    int Position { get; set; }
    int Score { get; set; }
    void Move(int steps);
    void UpdateScore(int points);
}

// Klasa bazowa Player
public abstract class Player : IPlayer
{
    public string Name { get; private set; }
    public int Position { get; set; }
    public int Score { get; set; }

    protected Player(string name)
    {
        Name = name;
        Position = 0; // startowa pozycja
        Score = 0; // startowy wynik
    }

    public void Move(int steps)
    {
        Position += steps;
    }

    public void UpdateScore(int points)
    {
        Score += points;
    }
}

// Klasa Wojownik
public class Warrior : Player
{
    public Warrior(string name) : base(name) { }

    public void Attack()
    {
        Console.WriteLine($"{Name} atakuje!");
    }
}

// Klasa Mag
public class Mage : Player
{
    public Mage(string name) : base(name) { }

    public void CastSpell()
    {
        Console.WriteLine($"{Name} rzuca zaklęcie!");
    }
}

// Klasa Healer
public class Healer : Player
{
    public Healer(string name) : base(name) { }

    public void Heal()
    {
        Console.WriteLine($"{Name} leczy sojusznika!");
    }
}

// Klasa Board
public class Board
{
    private int size;
    private Dictionary<int, int> rewards; // pole -> punkty

    public Board(int size)
    {
        this.size = size;
        rewards = new Dictionary<int, int>();
        GenerateRewards();
    }

    private void GenerateRewards()
    {
        Random rand = new Random();
        for (int i = 1; i < size; i++)
        {
            if (rand.Next(0, 2) == 0) // 50% szans na nagrodę
            {
                rewards[i] = rand.Next(1, 11); // punkty od 1 do 10
            }
        }
    }

    public int CheckReward(int position)
    {
        return rewards.ContainsKey(position) ? rewards[position] : 0;
    }

    public int GetSize()
    {
        return size;
    }
}

// Klasa Game
public class Game
{
    private List<IPlayer> players;
    private Board board;
    private int currentPlayerIndex;

    public event Action<string> OnItemCollected;

    public Game(int boardSize, List<string> playerNames)
    {
        board = new Board(boardSize);
        players = new List<IPlayer>();

        foreach (var name in playerNames)
        {
            players.Add(new Warrior(name)); // Zmieniono na Wojownik, można zmieniać typy
            // players.Add(new Mage(name)); // Alternatywnie
            // players.Add(new Healer(name)); // Alternatywnie
        }

        currentPlayerIndex = 0;
    }

    public void StartGame()
    {
        while (true)
        {
            PlayTurn();
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;

            // Zakończenie gry po 10 turach
            if (currentPlayerIndex == 0)
            {
                Console.WriteLine("Gra zakończona!");
                ShowScores();
                break;
            }
        }
    }

    private void PlayTurn()
    {
        var player = players[currentPlayerIndex];
        Console.WriteLine($"{player.Name} jest na turze. Rzuć kostką!");

        Random rand = new Random();
        int diceRoll = rand.Next(1, 7); // Rzucanie kostką (1-6)
        Console.WriteLine($"Wyrzucono: {diceRoll}");

        player.Move(diceRoll);
        int position = player.Position;

        // Sprawdzanie, czy pozycja jest na planszy
        if (position >= board.GetSize())
        {
            Console.WriteLine($"{player.Name} nie może się poruszyć dalej!");
            player.Position = board.GetSize() - 1; // zatrzymanie na końcu planszy
            return;
        }

        int reward = board.CheckReward(position);
        if (reward > 0)
        {
            player.UpdateScore(reward);
            OnItemCollected?.Invoke($"{player.Name} zebrał {reward} punkty na polu {position}!");
        }

        Console.WriteLine($"{player.Name} jest teraz na pozycji {position} z {player.Score} punktami.");
    }

    private void ShowScores()
    {
        foreach (var player in players)
        {
            Console.WriteLine($"{player.Name}: {player.Score} punktów");
        }
    }
}

// Klasa Program
class Program
{
    static void Main(string[] args)
    {
        List<string> playerNames = new List<string> { "Wojownik", "Mag", "Healer" };
        Game game = new Game(20, playerNames);

        game.OnItemCollected += (message) => Console.WriteLine(message); // Delegat do obsługi zdarzeń

        game.StartGame();
    }
}
