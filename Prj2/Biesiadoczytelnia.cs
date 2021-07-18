using System;
using System.Threading;

/*
Mamy sytuację przypominającą ucztujących filozofów, ale z dwoma modyfikacjami.
1. Nasi 'filozofowie' przy jedzeniu czytają. Mamy do dyspozycji k książek (mogą też być zaimplementowane jako zamki lub semafory) 
i każdy przed rozpoczęciem jedzenia bierze sobie jedną z książek - za każdym razem inną.
2. Mogą za każdym razem usiąść przy innym miejscu. Widelce biorą zawsze z sąsiedztwa wybranego nakrycia, ale miejsce wybierają również losowo.
 
Zapętlony ciąg działań czytelnika:
Czytelnik 1 myśli...
Czytelnik 1 zgłodniał.
Czytelnik 1 usiadł przy nakryciu 2.             <- będzie próbował wziąć widelce z tego nakrycia; samo nakrycie jest wylosowane
Czytelnik 1 chwycił za książkę 5.               <- książka wylosowana z k książek
Czytelnik 1 zaczyna jeść i czytać książkę 5...
Czytelnik 1 skończył jeść i czytać książkę 5.
Czytelnik 1 odszedł od nakrycia 2.
*/

class Czytelnik
{
    int nr;         // nr czytelnika
    int nr_k;       // nr książki
    int nr_n;       // nr nakrycia
    SemaphoreSlim[] widelce;
    SemaphoreSlim[] ksiazki;
    SemaphoreSlim[] nakrycia;
    SemaphoreSlim stol;
    int lewy, prawy;
    Random rand = new Random();

    public Czytelnik(int nr_, SemaphoreSlim[] widelce_, SemaphoreSlim[] ksiazki_, SemaphoreSlim[] nakrycia_, SemaphoreSlim stol_)
    {
        nr = nr_;
        widelce = widelce_;
        ksiazki = ksiazki_;
        nakrycia = nakrycia_;
        stol = stol_;
    }

    public void Mysl()
    {
        Console.WriteLine($"Czytelnik {nr} myśli...");
        Thread.Sleep(rand.Next(1000, 2000));
        Console.WriteLine($"Czytelnik {nr} zgłodniał.");
    }

    public void Usiadz()
    {
        int num;
        while (true)
        {
            num = rand.Next(0, nakrycia.Length);
            if (nakrycia[num].Wait(0))
            {
                nr_n = num;
                break;
            }
        }
        Console.WriteLine($"Czytelnik {nr} usiadł przy nakryciu {nr_n}.");
    }

    public void WezKsiazke()
    {
        int num;
        while (true)
        {
            num = rand.Next(0, ksiazki.Length);
            if (ksiazki[num].Wait(0))
            {
                nr_k = num;
                break;
            }
        }
        Console.WriteLine($"Czytelnik {nr} chwycił za książkę {nr_k}.");
    }

    public void Jedz()
    {
        Console.WriteLine($"Czytelnik {nr} zaczyna jeść i czytać książkę {nr_k}...");
        Thread.Sleep(rand.Next(1000, 2000));
        Console.WriteLine($"Czytelnik {nr} skończył jeść i czytać książkę {nr_k}.");
        Console.WriteLine($"Czytelnik {nr} odszedł od nakrycia {nr_n}.");
    }

    public void Dzialanie()
    {
        while (true)
        {
            Mysl();
            stol.Wait();
            Usiadz();
            lewy = nr_n;
            prawy = (nr_n + 1) % widelce.Length;
            widelce[lewy].Wait();
            widelce[prawy].Wait();
            WezKsiazke();
            Jedz();
            ksiazki[nr_k].Release();
            widelce[prawy].Release();
            widelce[lewy].Release();
            nakrycia[nr_n].Release();
            stol.Release();
        }
    }
}

class Biesiadoczytelnia
{
    static void Main(string[] args)
    {
        int licz_czyt = 10;      // liczba czytelników
        int k = 30;             // liczba książek
        SemaphoreSlim[] widelce = new SemaphoreSlim[licz_czyt];
        for (int i = 0; i < licz_czyt; i++)
            widelce[i] = new SemaphoreSlim(1, 1);
        SemaphoreSlim stol = new SemaphoreSlim(licz_czyt - 1, licz_czyt - 1);
        SemaphoreSlim[] ksiazki = new SemaphoreSlim[k];
        for (int i = 0; i < k; i++)
            ksiazki[i] = new SemaphoreSlim(1, 1);
        SemaphoreSlim[] nakrycia = new SemaphoreSlim[licz_czyt];
        for (int i = 0; i < licz_czyt; i++)
            nakrycia[i] = new SemaphoreSlim(1, 1);
        Czytelnik[] czytelnicy = new Czytelnik[licz_czyt];
        Thread[] watki = new Thread[licz_czyt];
        for (int i = 0; i < licz_czyt; i++)
        {
            czytelnicy[i] = new Czytelnik(i, widelce, ksiazki, nakrycia, stol);
            watki[i] = new Thread(czytelnicy[i].Dzialanie);
        }
        foreach (var watek in watki) watek.Start();
        foreach (var watek in watki) watek.Join();
    }
}

