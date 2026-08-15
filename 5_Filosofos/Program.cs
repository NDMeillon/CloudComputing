
// Philosophers Problem, C# edition
// MAMR, 33084
// Console app.
using System;
using System.Threading;

enum State
{
    Thinking, // 0
    Hungry, // 1
    Eating // 2
}

class Fork
{
    public int Id { get; }

    public Fork(int id)
    {
        Id = id;
    }
}
class Philosopher
{
    // Properties
    private int id;
    private State state;
    private int eatCount;
    private Fork leftFork;
    private Fork rightFork;
    public Philosopher(int id, Fork leftFork, Fork rightFork)
    {
        this.id = id;
        this.leftFork = leftFork;
        this.rightFork = rightFork;
        this.state = State.Thinking;
        this.eatCount = 0;
    }

    public void Run() // Main execution loop for the thread
    {
        while (eatCount < 3)
        {
            Think();
            TryToEat();
        }

        Console.WriteLine($"[Finished] Philosopher {id} has eaten 3 times and is satisfied.");
    }

    private void Think()
    {
        this.state = State.Thinking;
        Console.WriteLine($"Philosopher {id} is Thinking...");

        int waitTime = Random.Shared.Next(1000, 3000);
        Thread.Sleep(waitTime);

        this.state = State.Hungry;
        Console.WriteLine($"--> Philosopher {id} is now Hungry.");
    }

    private void TryToEat()
    {
        // Deadlock prevention: always lock the lower-numbered fork index first
        Fork firstFork = leftFork.Id < rightFork.Id ? leftFork : rightFork;
        Fork secondFork = leftFork.Id < rightFork.Id ? rightFork : leftFork;

        lock (firstFork)
        {
            lock (secondFork)
            {
                Eat();
            }
        }
    }

    private void Eat()
    {
        this.state = State.Eating;
        this.eatCount++;
        Console.WriteLine($"*** Philosopher {id} is EATING (Meal #{eatCount}) ***");

        int waitTime = Random.Shared.Next(1000, 3000);
        Thread.Sleep(waitTime);

        this.state = State.Thinking;
        Console.WriteLine($"Philosopher {id} finished eating.");
    }

    public int GetId() => this.id;
    public State GetState() => this.state;
    public int GetEatCount() => this.eatCount;
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Philosophers Problem Simulation...");

        const int count = 5;

        Fork[] forks = new Fork[count];
        for (int i=0; i < count; i++)
        {
            forks[i] = new Fork(i);
        }

        Philosopher[] philosophers = new Philosopher[count];
        Thread[] threads = new Thread[count];

        for (int i = 0; i < count; i++)
        {
            int leftForkIndex = i;
            int rightForkIndex = (i + 1) % count;

            philosophers[i] = new Philosopher(
                id: i,
                leftFork: forks[leftForkIndex],
                rightFork: forks[rightForkIndex]
            );

            // Create and start a dedicated thread for each philosopher
            threads[i] = new Thread(philosophers[i].Run);
            threads[i].Start();
        }

        for (int i = 0; i < count; i++)
        {
            threads[i].Join();
        }

        Console.WriteLine("\nAll philosophers have finished eating. Simulation complete.");
    }
}
