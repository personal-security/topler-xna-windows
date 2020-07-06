using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace perestroika
{
    public class GameClass : Microsoft.Xna.Framework.Game
    {
        #region variable
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //content
        Texture2D texfrog;
        Texture2D texfrogdeath;
        Texture2D texgamebg;
        Texture2D texmenubg;
        Texture2D texlily;
        Texture2D texmenufont;
        Texture2D texmenufontsel;  //selected
        Texture2D texgametex;
        Texture2D texcursor;
        SpriteFont hudfont;
        SpriteFont hudfont2;
        SpriteEffects effectsfrog;
        //game's consts and values
        const int herow = 100;   //scale hero in tex
        const int heroh = 100;   //
        const int mapw = 200;   //its scale of texgame
        const int maph = 200;   //but while not use
        const int x0 = 100;   //coord drawing game
        const int y0 = 50;   //
        const int hw = 128;   //scale hero in game
        const int hh = 100;   //
        const int herox0 = -5;
        const int heroy0 = 0;
        int dx = 0;   //changing moving coord
        int dy = 0;   //
        const int maxlengthusername = 10;
        //map
        const int mw = 6;   //-1//array
        const int mh = 4;   //-1//mas
        int mx = 0;// = 2;   //array 
        int my = mh - 1;// = 2;   //mas
        enum Map {l0 = 0, l1, l2, l3, l4 , l5, l6, l7, l8, l9, begin, end}
        Map[,] mas = new Map[mw, mh];
        //time
        int steptime = 0;
        const int countframesrun = 6;  //-1
        const int countframesdeath = 9; //-1
        const int countframeswinlevel = 6; //-1
        const int onesteptime = 18;  //18 frames on 1 step of animation
        const int onelilytime = 10;  //10 frames
        const int oneclicktime = 10; //10 ms for every buttons click
        const int countframeslily = 10; //10//
        const int timefrogdeath = 40;
        const int timefrogwinlevel = 40;
        //menu
        int menuitemselected = 1;// 0 - no cool
        int maxcountmenuitems = 4;//1,2,3   and 0 - empty
        int menuitemsx = 200;
        int menuitemsy = 120;
        int menuitemsw = 300;
        int menuitemh = 80;
        //game
        public enum Modegame : int { menu = 0, options, game, pause, results, death, winlevel, changeprofilename };
        Modegame modegame = 0;
        int currentnumberlevel = 1;
        int score = 0;
        int vmove = 0; //0,1,2,3,4
        int vidle = 2; //1,2,3,4
        //options
        bool controlmouse = true;//control
        //int soundvolume = 100; //enabled : 0, 100
        //int musicvolume = 100; //
        //additional parametres
        KeyboardState oldkeyboardstate; //event down -> click
        MouseState oldmousestate; //
        KeyboardState currentkeyboardstate;//Im decide to make it not local
        MouseState currentmousestate;//
        Random r = new Random();
        int timeactionpause = 0;
        const int maxtimeactionpause = 10;
        const string pathdata = "data.txt";//data
        //results
        public class Table
        {
            public int score = 0;
            public string name = "Игрок";
            public int level = 0;
            public string data;
            public Table(string name, int score, int level, string data)
            {
                this.name = name;
                this.score = score;
                this.level = level;
                this.data = data;
            }
        }
        List<Table> table;
        const int tablecounts = 10;//max
        string username = "Игрок";
        //buttons
        public class Button
        {
            public Modegame mg;//when activately
            public Vector2 v;//coord
            public SpriteFont font;//later buttons from tex
            public string name;
            public enum ButtonAction { pausetomenu, gametopause, optionstomenu, pausetogame, controlmouse, controlkeyboard, changeprofilebegin, changeprofileend };
            public ButtonAction ba;
            public Color ButtonColorOn = Color.Yellow;
            public Color ButtonColorOff = Color.White;
            public bool on = false;
            public bool enabled  = true;
            public Button(Modegame mg, Vector2 v, SpriteFont font, string name, ButtonAction ba)
            {
                this.mg = mg;
                this.v = v;
                this.font = font;
                this.name = name;
                this.ba = ba;
            }
            public Button(Modegame mg, Vector2 v, SpriteFont font, string name, ButtonAction ba, bool on)
            {
                this.mg = mg;
                this.v = v;
                this.font = font;
                this.name = name;
                this.ba = ba;
                this.on = on;
            }
            public Button(Modegame mg, Vector2 v, SpriteFont font, string name, ButtonAction ba, bool on, Color ButtonColorOn, Color ButtonColorOff) 
            {
                this.mg = mg;
                this.v = v;
                this.font = font;
                this.name = name;
                this.ba = ba;
                this.on = on;
                this.ButtonColorOff = ButtonColorOff;
                this.ButtonColorOn = ButtonColorOn;
            }     
        }
        
        List<Button> buttons;
        //debug
        //int counter = 0;// for debug
        //-------------------------------------------------------------------------------------
        #endregion
        public GameClass()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //graphics.PreferredBackBufferWidth = 1024;   //screenw
            //graphics.PreferredBackBufferHeight = 768;   //screenh
        }

        protected override void Initialize()
        {
            //Mouse.WindowHandle = Window.Handle;
            //this.IsMouseVisible = true;
            table = new List<Table>();
            buttons = new List<Button>();
            buttons.Add(new Button(Modegame.game, new Vector2(20, 20), hudfont2, "Пауза", Button.ButtonAction.gametopause));
            buttons.Add(new Button(Modegame.pause, new Vector2(20, 20), hudfont2, "Продолжить", Button.ButtonAction.pausetogame));
            buttons.Add(new Button(Modegame.pause, new Vector2(20, 100), hudfont2, "Меню", Button.ButtonAction.pausetomenu));
            buttons.Add(new Button(Modegame.options, new Vector2(20, 100), hudfont2, "Меню", Button.ButtonAction.optionstomenu));
            buttons.Add(new Button(Modegame.options, new Vector2(200, 200), hudfont2, "Мышь", Button.ButtonAction.controlmouse, true));
            buttons.Add(new Button(Modegame.options, new Vector2(450, 200), hudfont2, "Клавиатура", Button.ButtonAction.controlkeyboard));
            buttons.Add(new Button(Modegame.menu, new Vector2(510, 390), hudfont, "нажми для смены имени", Button.ButtonAction.changeprofilebegin, false, Color.Red, Color.Blue));
            buttons.Add(new Button(Modegame.changeprofilename, new Vector2(510, 390), hudfont, "нажми Enter для смены", Button.ButtonAction.changeprofileend, false, Color.Red, Color.Lime));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region loaddata
            if (File.Exists(pathdata))
            {
                System.IO.FileStream fileStream = new System.IO.FileStream(pathdata, System.IO.FileMode.Open);
                System.IO.StreamReader streamWriter = new System.IO.StreamReader(fileStream, System.Text.Encoding.Default);
                string line = "dd";
                string[] s;
                while (line != null)
                {
                    line = streamWriter.ReadLine();
                    if (line != null)
                    {
                        s = line.Split(' ');
                        if ((s[0] != null) && (s[1] != null) && (s[2] != null) && (s[3] != null))
                            table.Add(new Table(s[0], int.Parse(s[1]), int.Parse(s[2]), s[3]));
                    }
                }
                fileStream.Close();
            }
            #endregion
                     
            #region loadcontent
            texfrog = Content.Load<Texture2D>("frogtex");
            texfrogdeath = Content.Load<Texture2D>("frogdeath");
            texlily = Content.Load<Texture2D>("lilytex");
            texgamebg = Content.Load<Texture2D>("gamebg");
            texmenubg = Content.Load<Texture2D>("menubg");
            texmenufont = Content.Load<Texture2D>("menufont");
            texmenufontsel = Content.Load<Texture2D>("menufontsel");
            texgametex = Content.Load<Texture2D>("gametex");
            texcursor = Content.Load<Texture2D>("cur");
            hudfont = Content.Load<SpriteFont>("hud");
            hudfont2 = Content.Load<SpriteFont>("hud2");
            #endregion
        }

        protected override void UnloadContent()
        {
            //
        }

        protected override void Update(GameTime gameTime)
        {
            
            currentkeyboardstate = Keyboard.GetState();
            currentmousestate = Mouse.GetState();

            if (timeactionpause <= 0)
            {
                switch (modegame)
                {
                    case Modegame.pause:
                            if ((currentkeyboardstate.IsKeyDown(Keys.Escape)) && (currentkeyboardstate != oldkeyboardstate))
                                ChangeModeGame(Modegame.menu);
                            if ((currentkeyboardstate.IsKeyDown(Keys.Enter)) && (currentkeyboardstate != oldkeyboardstate))
                                ChangeModeGame(Modegame.game);
                            oldkeyboardstate = currentkeyboardstate;
                            break;
                    case Modegame.game:
                            #region Game
                            if (mas[mx, my] == Map.end)
                            {
                                ChangeModeGame(Modegame.winlevel);
                                break;
                            }
                            if (mas[mx, my] == Map.l9)
                                if ((vmove == 0) || (steptime * 10 / onesteptime >= 8))
                                {
                                    ChangeModeGame(Modegame.death);
                                    break;
                                }
                            for (int i = 0; i < mw; i++)
                                for (int j = 0; j < mh; j++)
                                    if ((mas[i, j] != Map.begin) && (mas[i, j] != Map.end))
                                        if ((currentnumberlevel > 20)||(r.Next(0, (60 / currentnumberlevel)) == 2))//костыль
                                        //if (r.Next(0, (60 / currentnumberlevel)) == 2)
                                            if (mas[i, j] == Map.l9)
                                                mas[i, j] = 0;
                                            else
                                                mas[i, j]++;
                            if (steptime <= 0)
                            {
                                if (vmove != 0)
                                {
                                    switch (vmove)
                                    {
                                        case 1:
                                            my--;
                                            break;
                                        case 2:
                                            mx++;
                                            break;
                                        case 3:
                                            my++;
                                            break;
                                        case 4:
                                            mx--;
                                            break;
                                    }
                                    vmove = 0;
                                    score += currentnumberlevel * 5;
                                    dx = 0;
                                    dy = 0;
                                }
                                if (!controlmouse)
                                    KeyMoving(currentkeyboardstate);
                                if ((currentmousestate.LeftButton == ButtonState.Pressed)&&(controlmouse))
                                    MouseMoving(currentmousestate);
                                if (vmove != 0)
                                    steptime = onesteptime;
                            }
                            else
                            {
                                steptime--;
                                switch (vmove)
                                {
                                    case 1: dy -= (maph / (2 * onesteptime)); break;
                                    case 2: dx += (mapw / (2 * onesteptime)); break;
                                    case 3: dy += (maph / (2 * onesteptime)); break;
                                    case 4: dx -= (mapw / (2 * onesteptime)); break;
                                }
                            }
                            #endregion
                            if ((currentkeyboardstate.IsKeyDown(Keys.Escape)) && (currentkeyboardstate != oldkeyboardstate))
                                ChangeModeGame(Modegame.pause);
                            break;
                    case Modegame.menu:
                            #region Menu
                            for (int i = 1; i <= maxcountmenuitems; i++)
                                if (CoordInRect(currentmousestate.X, currentmousestate.Y, new Rectangle(menuitemsx, menuitemsy + menuitemh * (i - 1), menuitemsw, menuitemh)))
                                {
                                    menuitemselected = i;
                                    if (currentmousestate.LeftButton == ButtonState.Pressed)
                                        MenuItemExecute(menuitemselected);
                                }
                            #endregion
                            if ((currentkeyboardstate.IsKeyDown(Keys.Escape)) && (currentkeyboardstate != oldkeyboardstate))
                                this.Exit();
                            //if ((keyboard.IsKeyDown(Keys.Enter)) && (keyboard != oldkeyboard))
                            //    changemodegame(2);
                            break;
                    case Modegame.results:
                            //if ((state.LeftButton == ButtonState.Pressed) || (keyboard.IsKeyDown(Keys.Escape)))
                            if (currentmousestate.LeftButton == ButtonState.Pressed)
                                ChangeModeGame(0);
                            break;
                    case Modegame.options:
                            if (currentkeyboardstate.IsKeyDown(Keys.Escape))
                                ChangeModeGame(0);
                            break;
                    case Modegame.death:
                            if (steptime <= 0)
                            {
                                if ((currentmousestate.LeftButton == ButtonState.Pressed)||(currentkeyboardstate.IsKeyDown(Keys.Enter)))
                                    ChangeModeGame(0);
                            }
                            else
                                steptime--;
                            break;
                    case Modegame.winlevel:
                        if (steptime <= 0)
                            steptime = timefrogwinlevel;
                        else
                            steptime--;
                        if ((currentmousestate.LeftButton == ButtonState.Pressed) || (currentkeyboardstate.IsKeyDown(Keys.Enter)))
                            ChangeModeGame(Modegame.game);
                        break;
                    case Modegame.changeprofilename:
                        Keys[] pressedKeys;
                        pressedKeys = currentkeyboardstate.GetPressedKeys();
                        foreach (Keys key in pressedKeys)
                        {
                            if (oldkeyboardstate.IsKeyUp(key))
                            {
                                switch (key)
                                {
                                    case Keys.Back:       // overflows
                                        if (username.Length > 0) 
                                            username = username.Remove(username.Length - 1, 1);
                                        break;
                                    case Keys.Space:
                                        if (username.Length < maxlengthusername)
                                            username = username.Insert(username.Length, " ");
                                        break;
                                    case Keys.A: case Keys.B: case Keys.C: case Keys.D: case Keys.E: case Keys.F: case Keys.G:
                                    case Keys.H: case Keys.I: case Keys.J: case Keys.K: case Keys.L: case Keys.M: case Keys.N: 
                                    case Keys.O: case Keys.P: case Keys.Q: case Keys.R: case Keys.S: case Keys.T: case Keys.U: 
                                    case Keys.V: case Keys.W: case Keys.X: case Keys.Y: case Keys.Z:
                                        if (username.Length < maxlengthusername)
                                        {
                                            if (currentkeyboardstate.IsKeyDown(Keys.LeftShift))
                                                username += key.ToString();
                                            else
                                                username += char.ToLower((char)(key)).ToString();
                                        }
                                        break;
                                }
                            }
                        }
                        if ((username != null) && (currentkeyboardstate.IsKeyDown(Keys.Enter)))
                        {
                            ChangeModeGame(Modegame.menu);
                        }
                        break;
                }
            }
            else
                timeactionpause--;
            foreach (Button b in buttons)
                ButtonExecute(b, currentmousestate);
            oldkeyboardstate = currentkeyboardstate;
            oldmousestate = currentmousestate;
            base.Update(gameTime);
        }

        void KeyMoving(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Up))
                if (my > 0)
                {
                    vmove = 1;
                    vidle = 1;
                    effectsfrog = SpriteEffects.None;
                }
            if (keyboard.IsKeyDown(Keys.Right))
                if (mx < mw - 1)
                {
                    vmove = 2;
                    vidle = 2;
                    effectsfrog = SpriteEffects.None;
                }
            if (keyboard.IsKeyDown(Keys.Down))
                if (my < mh - 1)
                {
                    vmove = 3;
                    vidle = 3;
                    effectsfrog = SpriteEffects.None;
                }
            if (keyboard.IsKeyDown(Keys.Left))
                if (mx > 0)
                {
                    vmove = 4;
                    vidle = 4;
                    effectsfrog = SpriteEffects.FlipHorizontally;
                }
        }

        void MouseMoving(MouseState mouse)
        {
            int tx = (mouse.X - x0) / 100;
            int ty = (mouse.Y - y0) / 100;
            if ((tx >= 0) && (tx < mw) && (tx - mx == 1) && (ty - my == 0))//right
            {
                vmove = 2;
                vidle = 2;
                effectsfrog = SpriteEffects.None;
            }
            if ((ty >= 0) && (ty < mh) && (ty - my == 1) && (tx - mx == 0))//down
            {
                vmove = 3;
                vidle = 3;
                effectsfrog = SpriteEffects.None;
            }
            if ((tx >= 0) && (tx < mw) && (tx - mx == -1) && (ty - my == 0))//left
            {
                vmove = 4;
                vidle = 4;
                effectsfrog = SpriteEffects.FlipHorizontally;
            }
            if ((ty >= 0) && (ty < mh) && (ty - my == -1) && (tx - mx == 0))//up
            {
                vmove = 1;
                vidle = 1;
                effectsfrog = SpriteEffects.None;
            }
        }

        void DrawGame(SpriteBatch sb, Color cl)
        {
            for (int i = 0; i < mw; i++)
                for (int j = 0; j < mh; j++)
                {
                    if (mas[i, j] == Map.begin)
                        sb.Draw(texgametex, new Rectangle(x0 + i * 100, y0 + j * 100, 100, 100), new Rectangle(400, 0, 200, 200), cl);
                    if (mas[i, j] == Map.end)
                        sb.Draw(texgametex, new Rectangle(x0 + i * 100, y0 + j * 100, 100, 100), new Rectangle(0, 0, 200, 200), cl);
                    if ((mas[i, j] != Map.begin) && (mas[i, j] != Map.end))//without 0 if 1st animation is last
                        sb.Draw(texlily, new Rectangle(x0 + i * 100, y0 + j * 100, 100, 100), new Rectangle(0, (int)(mas[i, j]) * 100, 100, 100), cl);
                }
            #region hero
            switch (modegame)
            {
                case Modegame.game:
                        switch (vmove)
                        {
                            case 0:
                                switch (vidle)
                                {
                                    case 1:
                                        sb.Draw(texfrog, new Rectangle(x0 + herox0 + mx * 100, y0 + heroy0 + my * 100 - heroh / 2, hw, hh), new Rectangle(0, heroh, herow, heroh), cl, 0, Vector2.Zero, effectsfrog, 0);
                                        break;
                                    case 2:
                                        sb.Draw(texfrog, new Rectangle(x0 + herox0 + mx * 100, y0 + heroy0 + my * 100 - heroh / 2, hw, hh), new Rectangle(0, heroh * 2, herow, heroh), cl, 0, Vector2.Zero, effectsfrog, 0);
                                        break;
                                    case 3:
                                        sb.Draw(texfrog, new Rectangle(x0 + herox0 + mx * 100, y0 + heroy0 + my * 100 - heroh / 2, hw, hh), new Rectangle(0, 0, herow, heroh), cl, 0, Vector2.Zero, effectsfrog, 0);
                                        break;
                                    case 4:
                                        sb.Draw(texfrog, new Rectangle(x0 + herox0 + mx * 100, y0 + heroy0 + my * 100 - heroh / 2, hw, hh), new Rectangle(0, heroh * 2, herow, heroh), cl, 0, Vector2.Zero, effectsfrog, 0);
                                        break;
                                }
                                break;
                            case 1:
                                sb.Draw(texfrog, new Rectangle(x0 + herox0 + mx * 100 + dx, y0 + heroy0 + my * 100 - heroh / 2 + dy, hw, hh), new Rectangle(herow * (((onesteptime - steptime) / (onesteptime / countframesrun))), heroh, herow, heroh), cl);
                                break;
                            case 2:
                                sb.Draw(texfrog, new Rectangle(x0 + herox0 + mx * 100 + dx, y0 + heroy0 + my * 100 - heroh / 2 + dy, hw, hh), new Rectangle(herow * (((onesteptime - steptime) / (onesteptime / countframesrun))), heroh * 2, herow, heroh), cl);
                                break;
                            case 3:
                                sb.Draw(texfrog, new Rectangle(x0 + herox0 + mx * 100 + dx, y0 + heroy0 + my * 100 - heroh / 2 + dy, hw, hh), new Rectangle(herow * (((onesteptime - steptime) / (onesteptime / countframesrun))), 0, herow, heroh), cl);
                                break;
                            case 4:
                                sb.Draw(texfrog, new Rectangle(x0 + herox0 + mx * 100 + dx, y0 + heroy0 + my * 100 - heroh / 2 + dy, hw, hh), new Rectangle(herow * (((onesteptime - steptime) / (onesteptime / countframesrun))), heroh * 2, herow, heroh), Color.White, 0, Vector2.Zero, effectsfrog, 0);
                                break;
                        }
                        break;
                case Modegame.death:
                        sb.Draw(texfrogdeath, new Rectangle(x0 + herox0 + mx * 100 + dx - 5, y0 + heroy0 + my * 100 - heroh / 2 + dy + 34, hw, hh), new Rectangle(0, heroh * (((timefrogdeath - steptime) / (timefrogdeath / countframesrun))), herow, heroh), cl);
                        //костыль для анимации -5;+34
                        break;
                case Modegame.winlevel:
                        sb.Draw(texfrog, new Rectangle(x0 + herox0 + mx * 100 + dx, y0 + heroy0 + my * 100 - heroh / 2 + dy, hw, hh), new Rectangle(herow * (((timefrogwinlevel - steptime) / (timefrogwinlevel / countframeswinlevel))), 0, herow, heroh), cl);
                        break;
            }
            #endregion
        }

        void CreateLevel(int n)
        {
            mx = 0;
            my = mh - 1;
            if (n == 1)
                score = 0;
            steptime = 0;
            vmove = 0;
            vidle = 2;
            currentnumberlevel = n;
            for (int i = 0; i < mw; i++)
                for (int j = 0; j < mh; j++)
                    mas[i, j] = Map.l0;
            mas[0, mh - 1] = Map.begin;
            mas[mw - 1, 0] = Map.end;
        }

        void ChangeModeGame(Modegame n)
        {
            if (n == Modegame.menu)
            {
                menuitemselected = 1;
            }
            if ((n == Modegame.death) && (modegame == Modegame.game))//modegame
            {
                if (table.Count != 0)
                {
                    if (table.Count < tablecounts)
                    {
                        table.Add(new Table(username, score, currentnumberlevel, DateTime.Now.ToString("dd.MM.yyyy")));
                        for (int i = 0; i < table.Count - 1; i++)
                            if (table[i].score < table[table.Count - 1].score)
                                ChangeTableItem(i, table.Count - 1);
                        SaveData();
                    }
                    else if (table[table.Count - 1].score < score)
                    {
                        table[table.Count - 1] = new Table(username, score, currentnumberlevel, DateTime.Now.ToString("dd.MM.yyyy"));
                        for (int i = 0; i < table.Count - 1; i++)
                            if (table[i].score < table[table.Count - 1].score)
                                ChangeTableItem(i, table.Count - 1);
                        SaveData();
                    }
                }
                else
                    table.Add(new Table(username, score, currentnumberlevel, DateTime.Now.ToString("dd.MM.yyyy")));
            }
            if ((n == Modegame.game) && (modegame == Modegame.menu))
            {
                CreateLevel(1);
            }
            if ((n == Modegame.results) && (modegame == 0))
            {
                //
            }
            if ((n == Modegame.death) && (modegame == Modegame.game))
            {
                steptime = timefrogdeath;
            }
            if (n == Modegame.winlevel)
            {
                currentnumberlevel++;
                steptime = timefrogwinlevel;
            }
            if ((n == Modegame.game) && (modegame == Modegame.winlevel))
            {
                CreateLevel(currentnumberlevel);
            }
            modegame = n;
            timeactionpause = maxtimeactionpause;
        }

        bool CoordInRect(int x, int y, Rectangle rect)
        {
            if ((x >= rect.X) && (x <= rect.X + rect.Width) && (y >= rect.Y) && (y <= rect.Y + rect.Height))
                return true;
            return false;
        }

        void MenuItemExecute(int n)
        {
            switch (n)
            {
                case 1:
                    ChangeModeGame(Modegame.game);
                    break;
                case 2:
                    ChangeModeGame(Modegame.results);
                    break;
                case 3:
                    ChangeModeGame(Modegame.options);
                    break;
                case 4:
                    this.Exit();
                    break;
            }
        }

        void SaveData()
        {
            System.IO.File.Delete("data.txt");
            if (table.Count != 0)
                for (int i = 0; i < table.Count; i++)
                    System.IO.File.AppendAllText("data.txt", table[i].name + " " + table[i].score.ToString() + " " + table[i].level.ToString() + " " + table[i].data + "\r\n", System.Text.Encoding.UTF8);
        }

        void ChangeTableItem(int x, int y)
        {
            Table t = table[x];
            table[x] = table[y];
            table[y] = t;
        }

        void ButtonExecute(Button b, MouseState ms)
        {
            if ((CoordInRect(oldmousestate.X, oldmousestate.Y, new Rectangle((int)b.v.X, (int)b.v.Y, 100, 100))) && (oldmousestate.LeftButton == ButtonState.Pressed) && (b.mg == modegame) && (b.enabled) && (timeactionpause <= 0)) 
            {
                switch (b.ba)
                {
                    case Button.ButtonAction.pausetomenu:
                        SaveData();
                        ChangeModeGame(0);
                        break;
                    case Button.ButtonAction.gametopause:
                        if (controlmouse) 
                            ChangeModeGame(Modegame.pause);
                        break;
                    case Button.ButtonAction.optionstomenu:
                        ChangeModeGame(0);
                        break;
                    case Button.ButtonAction.pausetogame:
                        ChangeModeGame(Modegame.game);
                        break;
                    case Button.ButtonAction.controlkeyboard:
                        if (controlmouse)
                        {
                            controlmouse = false;
                            b.on = true;
                            buttons[buttonindex(Button.ButtonAction.controlmouse)].on = false;
                        }
                        break;
                    case Button.ButtonAction.controlmouse:
                        if (!controlmouse)
                        {
                            controlmouse = true;
                            b.on = true;
                            buttons[buttonindex(Button.ButtonAction.controlkeyboard)].on = false;
                        }
                        break;
                    case Button.ButtonAction.changeprofileend:
                        menuitemselected = 1;
                        ChangeModeGame(Modegame.menu);
                        break;
                    case Button.ButtonAction.changeprofilebegin:
                        menuitemselected = 0;
                        ChangeModeGame(Modegame.changeprofilename);
                        break;
                }
                timeactionpause = maxtimeactionpause;
            }
        }

        int buttonindex(Button.ButtonAction ba)
        {
            for (int i = 0; i < buttons.Count; i++)
                if (buttons[i].ba == ba)
                    return i;
            return 0;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            switch (modegame)
            {
                case Modegame.menu:
                        spriteBatch.Draw(texmenubg, new Rectangle(0, 0, 800, 480), Color.White);
                        for (int i = 1; i <= maxcountmenuitems; i++)
                            if (menuitemselected == i)
                                spriteBatch.Draw(texmenufontsel, new Rectangle(menuitemsx, menuitemsy + menuitemh * (i - 1), menuitemsw, menuitemh), new Rectangle(0, menuitemh * (i - 1), menuitemsw, menuitemh), Color.White);
                            else
                                spriteBatch.Draw(texmenufont, new Rectangle(menuitemsx, menuitemsy + menuitemh * (i - 1), menuitemsw, menuitemh), new Rectangle(0, menuitemh * (i - 1), menuitemsw, menuitemh), Color.White);
                        spriteBatch.DrawString(hudfont, "Привет, " + username + "!", new Vector2(550, 360), Color.Black);
                        break;
                case Modegame.game:
                        spriteBatch.Draw(texgamebg, new Rectangle(0, 0, 800, 480), Color.White);
                        DrawGame(spriteBatch, Color.White);
                        spriteBatch.DrawString(hudfont, "Уровень : " + currentnumberlevel.ToString(), new Vector2(220, 30), Color.Black);
                        spriteBatch.DrawString(hudfont, "Очки : " + score.ToString(), new Vector2(420, 30), Color.Black);
                        break;
                case Modegame.pause:
                        spriteBatch.Draw(texgamebg, new Rectangle(0, 0, 800, 480), Color.Gray);
                        DrawGame(spriteBatch, Color.Gray);
                        spriteBatch.DrawString(hudfont, "Пауза", new Vector2(360, 220), Color.Yellow);
                        spriteBatch.DrawString(hudfont, "Нажми Enter для продолжения...", new Vector2(220, 330), Color.Black, 0, new Vector2(0, 0), 1, 0, 0);
                        break;
                case Modegame.results:
                        GraphicsDevice.Clear(Color.Gray);
                        spriteBatch.Draw(texmenubg, new Rectangle(0, 0, 800, 480), Color.Gray);
                        spriteBatch.DrawString(hudfont2, "Результаты", new Vector2(290, 20), Color.Yellow);
                        spriteBatch.DrawString(hudfont, "Имя", new Vector2(120, 120), Color.Yellow);
                        spriteBatch.DrawString(hudfont, "Очки", new Vector2(240, 120), Color.Yellow);
                        spriteBatch.DrawString(hudfont, "Уровень", new Vector2(360, 120), Color.Yellow);
                        spriteBatch.DrawString(hudfont, "Дата", new Vector2(480, 120), Color.Yellow);
                        if (table.Count != 0)
                            for (int i = 0; i < table.Count; i++)
                            {
                                spriteBatch.DrawString(hudfont, table[i].name, new Vector2(120, 150 + i * 20), Color.White);
                                spriteBatch.DrawString(hudfont, table[i].score.ToString(), new Vector2(240, 150 + i * 20), Color.White);
                                spriteBatch.DrawString(hudfont, table[i].level.ToString(), new Vector2(360, 150 + i * 20), Color.White);
                                spriteBatch.DrawString(hudfont, table[i].data, new Vector2(480, 150 + i * 20), Color.White);
                            }
                        break;
                case Modegame.options:
                        GraphicsDevice.Clear(Color.Gray);
                        spriteBatch.Draw(texmenubg, new Rectangle(0, 0, 800, 480), Color.Gray);
                        spriteBatch.DrawString(hudfont2, "Настройки", new Vector2(290, 20), Color.Yellow);
                        spriteBatch.DrawString(hudfont, "Управление", new Vector2(320, 150), Color.Lime);
                        break;
                case Modegame.death:
                        if (steptime <= 0)
                        {
                            spriteBatch.Draw(texgamebg, new Rectangle(0, 0, 800, 480), Color.Gray);
                            DrawGame(spriteBatch, Color.Gray);
                            spriteBatch.DrawString(hudfont, "Уровень : " + currentnumberlevel.ToString(), new Vector2(220, 30), Color.Black);
                            spriteBatch.DrawString(hudfont, "Очки : " + score.ToString(), new Vector2(420, 30), Color.Black);
                            spriteBatch.DrawString(hudfont2, "ТЫ УУУМЕЕЕР!!!", new Vector2(110, 50), Color.Red, 0, new Vector2(0, 0), 2, 0, 0);
                            spriteBatch.DrawString(hudfont, "Нажми Enter для продолжения...", new Vector2(220, 130), Color.Black, 0, new Vector2(0, 0), 1, 0, 0);
                        }
                        else
                        {
                            spriteBatch.Draw(texgamebg, new Rectangle(0, 0, 800, 480), Color.White);
                            DrawGame(spriteBatch, Color.White);
                            spriteBatch.DrawString(hudfont, "Уровень : " + currentnumberlevel.ToString(), new Vector2(220, 30), Color.Black);
                            spriteBatch.DrawString(hudfont, "Очки : " + score.ToString(), new Vector2(420, 30), Color.Black);
                        }
                        break;
                case Modegame.winlevel:
                        spriteBatch.Draw(texgamebg, new Rectangle(0, 0, 800, 480), Color.White);
                        DrawGame(spriteBatch, Color.White);
                        spriteBatch.DrawString(hudfont, "Уровень : " + currentnumberlevel.ToString(), new Vector2(220, 30), Color.Black);
                        spriteBatch.DrawString(hudfont, "Очки : " + score.ToString(), new Vector2(420, 30), Color.Black);
                        spriteBatch.DrawString(hudfont2, "Уровень пройден!", new Vector2(120, 50), Color.White, 0, new Vector2(0, 0), 2, 0, 0);
                        spriteBatch.DrawString(hudfont, "Нажми Enter для продолжения...", new Vector2(220, 130), Color.Black, 0, new Vector2(0, 0), 1, 0, 0);
                        break;
                case Modegame.changeprofilename:
                        spriteBatch.Draw(texmenubg, new Rectangle(0, 0, 800, 480), Color.White);
                        for (int i = 1; i <= maxcountmenuitems; i++)
                            if (menuitemselected == i)
                                spriteBatch.Draw(texmenufontsel, new Rectangle(menuitemsx, menuitemsy + menuitemh * (i - 1), menuitemsw, menuitemh), new Rectangle(0, menuitemh * (i - 1), menuitemsw, menuitemh), Color.White);
                            else
                                spriteBatch.Draw(texmenufont, new Rectangle(menuitemsx, menuitemsy + menuitemh * (i - 1), menuitemsw, menuitemh), new Rectangle(0, menuitemh * (i - 1), menuitemsw, menuitemh), Color.White);
                        spriteBatch.DrawString(hudfont, "Привет, " + username + "!", new Vector2(550, 360), Color.Black);
                        break;
            }
            foreach (Button b in buttons)
                if (b.mg == modegame)
                    if (b.on)
                        spriteBatch.DrawString(hudfont, b.name, b.v, b.ButtonColorOn);
                    else
                        spriteBatch.DrawString(hudfont, b.name, b.v, b.ButtonColorOff);
            //spriteBatch.DrawString(hudFont, counter.ToString(), new Vector2(10, 10), Color.Black); // for debug
            if ((controlmouse)||(modegame != Modegame.game))
                spriteBatch.Draw(texcursor, new Rectangle(oldmousestate.X, oldmousestate.Y, 24, 24), Color.White);//

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
