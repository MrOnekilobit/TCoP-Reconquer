﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text;

namespace Symbolus
{
    /// <summary>
    /// Játékon kívüli funkciókhoz kezelőfelület (gombok)
    /// </summary>
    public class Menu
    {
        private int cursorPos = 1; //hanyas gombon van a "kurzor"
        public MenuPage menuPage = new MenuPage("main"); //melyik menü lap van megnyitva (main, pause, settings)
        public bool backFromPause = false; //vissza a játékba?

        public Menu() { }

        /// <summary>
        /// Betöltött menü kirajzolása, majd utasítás várása
        /// </summary>
        public void Display()
        {
            Console.Clear();
            char c;

            for (int j = 0; j <= menuPage.maxY; j++)
            {
                for (int i = 0; i < menuPage.matrix[0].Length; i++)
                {
                    c = menuPage.matrix[j][i];

                    if (c == '#')
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    else if (c == cursorPos.ToString()[0])
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else if (c >= '0' && c <= '9')
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    else if (c == 'f')
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }
                    else if (c == '~')
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                    }
                    else if (c == 'b')
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    }
                    else if (c == 'ß')
                    {
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }
                    else if (c == '@')
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else if (c == '%')
                    {
                        if (Program.musicOn)
                            Console.Write("BE");
                        else
                            Console.Write("KI");
                        i += 2;
                        c = ' ';
                    }
                    else if (c == '$')
                    {
                        if (Program.soundOn)
                            Console.Write("BE");
                        else
                            Console.Write("KI");
                        i += 2;
                        c = ' ';
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    Console.Write(c);
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.WriteLine();
            }
            Controls();
        }

        private void Back()
        {
            this.cursorPos = 1;
            if (this.backFromPause)
            {
                this.backFromPause = false;
                if (Program.combat)
                    Program.nextDisplayed = Program.Screen.Combat;
                else
                    Program.nextDisplayed = Program.Screen.Map;
            }
            else
            {
                this.menuPage = new MenuPage("main");
                Program.nextDisplayed = Program.Screen.Menu;
            }
        }

        /// <summary>
        /// Utasítás várása, fogadása és végrehajtása
        /// </summary>
        private void Controls()
        {
            ConsoleKeyInfo keyinfo;
            while (Console.KeyAvailable)
            {
                keyinfo = Console.ReadKey(true);
            }
            keyinfo = Console.ReadKey(true);

            if (keyinfo.Key != ConsoleKey.Spacebar && keyinfo.Key != ConsoleKey.Enter)
            {
                if (keyinfo.Key == ConsoleKey.W && this.cursorPos > 1)
                {
                    this.cursorPos--;
                    Program.PlaySound("menumove");
                }
                else if (keyinfo.Key == ConsoleKey.S && this.cursorPos < this.menuPage.commands.Count)
                {
                    this.cursorPos++;
                    Program.PlaySound("menumove");
                }
                else if (keyinfo.Key == ConsoleKey.Escape)
                {
                    Program.PlaySound("select");
                    Back();
                }
            }
            else
            {                
                string cmd = this.menuPage.commands[this.cursorPos-1];
                Program.PlaySound("select");
                switch (cmd)
                {
                    case "start":
                        this.cursorPos = 1;
                        if (Program.musicOn)
                        {
                            Program.musics[Program.cmusic].Stop();
                            Program.musics["confusion"].PlayLooping();
                        }
                        Program.newGame = true;
                        Program.mapScreen.LoadMap("main_start");
                        Program.nextDisplayed = Program.Screen.Map;
                        break;
                    case "quit":
                        Thread.Sleep(200);
                        Environment.Exit(0);
                        break;
                    case "back":
                        Back();
                        break;
                    case "settings":
                        this.cursorPos = 1;
                        this.menuPage = new MenuPage("settings");
                        Program.nextDisplayed = Program.Screen.Menu;
                        break;
                    case "music":
                        Program.musicOn = !Program.musicOn;
                        if (!Program.newGame || Program.combat)
                        {
                            if (Program.musicOn)
                                Program.musics[Program.cmusic].PlayLooping();
                            else
                                Program.musics[Program.cmusic].Stop();
                        }
                        else
                        {
                            if (Program.musicOn)
                                Program.musics["confusion"].PlayLooping();
                            else
                                Program.musics["confusion"].Stop();
                        }                
                        break;
                    case "sound":
                        Program.soundOn = !Program.soundOn;
                        Program.PlaySound("select", true);
                        break;
                    case "retry":
                        Program.player.HP = Program.player.MaxHP;
                        Program.player.S = Program.player.MaxS;
                        Program.combat = true;
                        //foreach (EnemyMember member in Program.cEnemy.members)
                        for (int i = 0; i < Program.cEnemy.members.Count; i++)
                        {
                            Program.cEnemy.members[i].Reset();
                        }
                        if(Program.musicOn)
                        {
                            Program.musics[Program.cmusic].Stop();
                            Program.musics["fight"].PlayLooping();
                        }
                        Program.nextDisplayed = Program.Screen.Combat;
                        break;
                }                
            }
        }
    }

    /// <summary>
    /// Menü lapok (main, pause, settings)
    /// </summary>
    public class MenuPage
    {
        public string name; //("main", "pause", "settings")
        public int maxY; //a matrix utolsó kitöltött sora
        public string[] matrix = new string[40]; //karakter-rajz
        public List<string> commands = new List<string>(); //gombokhoz tartozó utasítátok

        public MenuPage() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="page">Betöltendő lap neve</param>
        public MenuPage(string page)
        {
            this.name = page;

            using (StreamReader r = new StreamReader(@"assets\menu\" + page + ".txt", Encoding.UTF8))
            {
                int i = 0;
                string line = r.ReadLine();

                while (line[0] != '=')
                {
                    matrix[i] = line;
                    i++;
                    line = r.ReadLine();
                }
                while (!r.EndOfStream)
                {
                    maxY = i - 1;
                    commands.Add(r.ReadLine().Split(':')[1]);
                }
            }
        }
    }


    /// <summary>
    /// Győzelem utáni kezelőfelület
    /// </summary>
    public class WinScreen
    {
        public int maxY; //a matrix utolsó kitöltött sora
        public string[] matrix = new string[40]; //karakter-rajz

        public int gainedXP;
        public int gainedMoney;
        public int hits;
        public int damage;

        public WinScreen()
        {
            using (StreamReader r = new StreamReader(@"assets\menu\win.txt", Encoding.UTF8))
            {
                int i = 0;
                while (!r.EndOfStream)
                {
                    matrix[i] = r.ReadLine();
                    i++;
                }
                maxY = i - 1;
            }
        }

        public void Display(int xp, int money, int hits, int damage)
        {
            this.gainedXP = xp;
            this.gainedMoney = money;
            this.hits = hits;
            this.damage = damage;
            
            this.Display();
        }
        public void Display()
        {
            Console.Clear();
            char c;
            for (int j = 0; j <= maxY; j++)
            {
                for (int i = 0; i < matrix[0].Length; i++)
                {
                    c = matrix[j][i];
                    if (c == '#')
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }                                       
                    else if (c == 'f')
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }
                    else if (c == '~')
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                    }
                    else if (c == '"')
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else if (c == 'ß')
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(gainedXP.ToString());
                        i += gainedXP.ToString().Length;
                        c = ' ';
                    }
                    else if (c == '+')
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(gainedMoney.ToString()+" Ft");
                        i += gainedMoney.ToString().Length+3;
                        c = ' ';
                    }
                    else if (c == '&')
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(hits.ToString());
                        i += hits.ToString().Length;
                        c = ' ';
                    }
                    else if (c == '$')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(damage.ToString());
                        i += damage.ToString().Length;
                        c = ' ';
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    Console.Write(c);
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.WriteLine();
            }
            Controls();
        }

        public void Controls()
        {
            ConsoleKeyInfo keyinfo;
            while (Console.KeyAvailable)
            {
                keyinfo = Console.ReadKey(true);
            }
            keyinfo = Console.ReadKey(true);

            if (keyinfo.Key == ConsoleKey.Enter || keyinfo.Key == ConsoleKey.Spacebar)
            {
                Program.PlaySound("select");
                Program.nextDisplayed = Program.Screen.Map;

                if (Program.musicOn && Program.newGame)
                {                    
                    Program.musics[Program.cmusic].Stop();
                    Program.cmusic = "confusion";
                    Program.musics[Program.cmusic].PlayLooping();
                }
                else if(Program.musicOn)
                {
                    Program.musics[Program.cmusic].Stop();
                    Program.cmusic = Program.mapScreen.map.music;
                    Program.musics[Program.cmusic].PlayLooping();
                }

                Program.player.AddXP(gainedXP);
                Program.player.money += gainedMoney;
            }
        }
    }    
}
