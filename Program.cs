using System;
using System.Linq;
using System.Threading;
using SharpDX.XInput;
using System.Media;
using DDDJConsole.Properties;

/*
    Spin at any time for a d20 roll
    Euphoria button for battle mode, dial for battle music volume
    Start to show current magic
    Switch between Magic and Undead commands with crossfader
    Magic Mode: G and B to scroll through magic.  Red + Spin to cast spell
    Undead Mode: G + Spin for Zombie Attack Rolls, G + R + Spin for Zombie Damage
        B + Spin for Skelly Attack Rolls, B + R + Spin for Skelly Damage
    Hold Back button and G, R, B and R+B+G become soundboard buttons.

*/

//Mapping of DJ Hero buttons: http://xboxforums.create.msdn.com/forums/t/40673.aspx
//Initial button example based on: https://github.com/sharpdx/SharpDX-Samples/blob/master/Desktop/XInput/XGamepadApp/Program.cs

namespace DDDJConsole
{
    class Program
    {
                
        public static bool spinning;
        public static short leftDialValue; 
        public static System.Windows.Media.MediaPlayer battleTheme; 
        public static string CurrentCrossFade;
        public static SpellList CurrentSpell;
                  
        static void Main(string[] args)
        {  
            leftDialValue = 0;
            ChangeSpell(SpellList.Lightning);

            Console.WriteLine(@"Start DDDJ");
            Console.WriteLine(@"        _,--*dSS|''I$$$SS%cccc,_");
            Console.WriteLine(@"      <$$$b |$$$l  j$$$$$$$$$$$$$Sbp");
            Console.WriteLine(@"       ?$$$b|$$$$  d$$$$$$$$$$$$$$P");
            Console.WriteLine(@"        ?$$$$$$$$; $$$$$$$$$$$$$$P");
            Console.WriteLine(@"         ?$$$$$$$| $$$$$$$$$$$$$P");
            Console.WriteLine(@"          )$$$$$$$_$SSSSS$$$$$$(");
            Console.WriteLine(@"          Y''               '''P");
            Console.WriteLine(@"          (                    )");
            Console.WriteLine(@"  _.,cccccd%S$$$$$$$$$$$$$$$SS%dcccc,._");
            Console.WriteLine(@"($$$$$$$$$$SSSSSSSSSSSSSSS$$$$$$$$$$$$$$$)");
            Console.WriteLine(@"  `''''Y''                  `'$$$$$$$P''");
            Console.WriteLine("");
            Console.WriteLine("Current Zombies: {0}", Resources.NumZombies);
            Console.WriteLine("Current Skeletons: {0}", Resources.NumZombies);
            Console.WriteLine("Current Profeciency Bonus: {0}", Resources.PB);
            Console.WriteLine("Current INT: {0}", Resources.INT);




            // Initialize XInput
            var controllers = new[] { new Controller(UserIndex.One), new Controller(UserIndex.Two), new Controller(UserIndex.Three), new Controller(UserIndex.Four) };
            
            // Get 1st controller available
            Controller controller = null;
            foreach (var selectControler in controllers)
            {
                if (selectControler.IsConnected)
                {
                    controller = selectControler;
                    break;
                }
            }

            if (controller == null)
            {
                Console.WriteLine("No XInput controller installed");
                Console.ReadKey();
            }
            else
            {

                Console.WriteLine("Found a XInput controller available.  Readying disks...");
                Console.WriteLine("Press escape key to exit... ");

                // Poll events from joystick
                var previousState = controller.GetState();

                ChangeCrossfadeMode(GetCrossFadeMode(previousState));
                while (controller.IsConnected)
                {
                    if (IsKeyPressed(ConsoleKey.Escape))
                    {
                        break;
                    }
                    var state = controller.GetState();

                    if (previousState.PacketNumber != state.PacketNumber)
                        ReadController(controller, state);
                    LoopBattleTheme();
                    Thread.Sleep(10);
                    previousState = state;
                }
            }
            Console.WriteLine("End DDDJ");
        }

        private static void ChangeSpell(SpellList spell)
        {
            CurrentSpell = spell;

            Console.WriteLine("({0}) {1} Ready", (int)CurrentSpell, CurrentSpell);
        }

        private static void LoopBattleTheme()
        {
            if (battleTheme == null) return;

            if (battleTheme.Position > new TimeSpan(0, 2, 25))
            {
                battleTheme.Position = new TimeSpan(0, 0, 20);
            }
        }

        private static void ReadController(Controller controller, State state)
        {

            if (IsButtonPressed(state, "Back"))
            {
                if (IsButtonPressed(state, "A") && IsButtonPressed(state, "B") && IsButtonPressed(state, "X"))
                {
                    (new SoundPlayer(@"..\..\Sounds\Boomshakalaka.wav")).Play();
                    System.Threading.Thread.Sleep(100);
                }
                else if (IsButtonPressed(state, "A"))
                {
                    (new SoundPlayer(@"..\..\Sounds\golemcast3.wav")).Play();
                    System.Threading.Thread.Sleep(100);
                }
                else if (IsButtonPressed(state, "B"))
                {
                    (new SoundPlayer(@"..\..\Sounds\airhorn.wav")).Play();
                    System.Threading.Thread.Sleep(100);
                }
                else if (IsButtonPressed(state, "X"))
                {
                    (new SoundPlayer(@"..\..\Sounds\dubwub.wav")).Play();
                    System.Threading.Thread.Sleep(100);
                }
                return;

            }

            if (SpinningFast(state))
            {
                if (!spinning)
                {
                    SpinActivated(state);                   
                }

                spinning = true;
            }
            else
            {
                if (SpinningLow(state)) spinning = false;

                CheckNoSpinButtonPress(state);
            }

            if (state.Gamepad.RightThumbX != leftDialValue)
            {
                if (battleTheme != null && battleTheme.Position != new TimeSpan(0)) {
                    short modifier;

                    if (state.Gamepad.RightThumbX - leftDialValue > 0)
                    {
                        modifier = 1;
                    }
                    else modifier = -1;
                    battleTheme.Volume += .01 * modifier;
                    leftDialValue = state.Gamepad.RightThumbX;
                }

            }


            String newCrossFadeMode = GetCrossFadeMode(state);
            if (newCrossFadeMode != CurrentCrossFade) 
            {
                ChangeCrossfadeMode(newCrossFadeMode);               
               
            }

            

            if (IsButtonPressed(state, "Y"))
            {
                if (battleTheme == null)
                {
                    battleTheme = new System.Windows.Media.MediaPlayer(); 

                    battleTheme.Open(new System.Uri(
                    (new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)).Parent.Parent).FullName +
                    @"\Sounds\LockAndLoad.wav"));
                     
                }

                if (battleTheme.Position == new TimeSpan(0))
                {
                    ChangeBattleLight(controller, true);                   
                    battleTheme.Play();
                }
                else
                {
                    ChangeBattleLight(controller, false);

                    (new SoundPlayer(@"..\..\Sounds\VictoryGong.wav")).Play();
                    battleTheme.Stop();
                    battleTheme.Volume = .5;
                }
                
                System.Threading.Thread.Sleep(500);
                 

            }

            if (IsButtonPressed(state, "Start"))
            {
                Console.WriteLine("Current Spell: {0}", CurrentSpell);             
            }
             
        }
        
        //private static void Media_Ended(object sender, EventArgs e)
        //{
        //    battleTheme.Position = TimeSpan.Zero;
        //    battleTheme.Play();
        //}

        private static void ChangeCrossfadeMode(string newCrossFadeMode)
        {
            CurrentCrossFade = newCrossFadeMode;

            if (CurrentCrossFade == CrossFadeMode.UNDEAD)
            {
                (new SoundPlayer(@"..\..\Sounds\amplifydamage.wav")).Play();
                Console.WriteLine("Skelebros Ready");
            }
            else if (CurrentCrossFade == CrossFadeMode.MAGIC)
            {
                (new SoundPlayer(@"..\..\Sounds\shivers.wav")).Play();
                Console.WriteLine("Magic Ready");
            }
        }

        private static void CheckNoSpinButtonPress(State state)
        {
            if (CurrentCrossFade == CrossFadeMode.MAGIC)
            {
                if (IsButtonPressed(state, "X"))
                {
                    SpellList newSpell;
                    newSpell = CurrentSpell + 1;

                    if (newSpell > Enum.GetValues(typeof(SpellList)).Cast<SpellList>().Max()) newSpell = 0;
                    ChangeSpell(newSpell);

                    Thread.Sleep(200);
                }

                if (IsButtonPressed(state, "A"))
                {
                    SpellList newSpell;
                    newSpell = CurrentSpell - 1;

                    if (newSpell < 0) newSpell = Enum.GetValues(typeof(SpellList)).Cast<SpellList>().Max();
                    ChangeSpell(newSpell);

                    Thread.Sleep(200);
                }
            }
        }

        private static bool IsButtonPressed(State state, string v)
        {
            String pressed = state.Gamepad.Buttons.ToString();

            if (!pressed.Contains(",")) {
                return (pressed == v);
            } else
            {
                var pressedList = pressed.Split(Convert.ToChar(","));
                foreach (string str in pressedList)
                    if (str.Trim() == v) return true;
                return false;
            }


            
        }

        private static bool SpinningFast(State state)
        {
            return (state.Gamepad.LeftThumbY > 20 || state.Gamepad.LeftThumbY < -20);
        }

        private static bool SpinningLow(State state)
        {
            return (state.Gamepad.LeftThumbY > -10 && state.Gamepad.LeftThumbY < 10);
        }

        private static void ChangeBattleLight(Controller controller, bool activate)
        {
            if (activate)
            {
                SharpDX.XInput.Vibration x = new SharpDX.XInput.Vibration();
                x.RightMotorSpeed = 65535;  //Max value
                controller.SetVibration(x);                
            }
            else
            {
                SharpDX.XInput.Vibration x = new SharpDX.XInput.Vibration();
                x.RightMotorSpeed = 0;  //Minimum value
                controller.SetVibration(x);
            }
            
        }

        private static String GetCrossFadeMode(State state)
        {
            
            if (state.Gamepad.RightThumbY < 0)
            {
                return CrossFadeMode.MAGIC;
            }
            else 
            {
                return CrossFadeMode.UNDEAD;
            }
        }

        private static void SpinActivated(State state)
        {
            bool SpellActivated = false;

            switch (CurrentCrossFade)
            {
                case CrossFadeMode.MAGIC:
                    if (IsButtonPressed(state, "B"))
                    {
                        DoSpell();
                        SpellActivated = true;

                    }
                    
                    break;

                case CrossFadeMode.UNDEAD:
                    if (IsButtonPressed(state, "A"))
                    {
                        if (IsButtonPressed(state, "B"))
                        {
                            new ZombiesDamage().Roll();                            
                            SpellActivated = true;
                        }
                        else {
                            new ZombiesToHit().Roll();
                            SpellActivated = true;
                        }
                    }
                    if (IsButtonPressed(state, "X"))
                    {
                        if (IsButtonPressed(state, "B"))
                        {
                            new SkeletonDamage().Roll();
                            SpellActivated = true;
                        }
                        else {
                            new SkeletonsToHit().Roll();
                            SpellActivated = true;
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (!SpellActivated)
            {
                new d20().Roll();
                if (state.Gamepad.LeftThumbY < 0)
                    (new SoundPlayer(@"..\..\Sounds\DJSpinReverse.wav")).Play();
            }

            Console.WriteLine();

        }

        private static void DoSpell()
        {           
                        
            Console.WriteLine("Casting " + CurrentSpell);

            //Spell casting works through reflection.  Gather the class from the assembly and invoke Roll()
            var spellType = Type.GetType("DDDJConsole." + CurrentSpell);
         
            if (spellType == null) { Console.WriteLine("Spell not constructed!"); }
            else { object spellObject = Activator.CreateInstance(spellType);
                
                Type newType = spellObject.GetType();
                System.Reflection.MethodInfo theMethod = newType.GetMethod("Roll");
                theMethod.Invoke(spellObject, null);                
            }

        }


        /// <summary>
        /// Determines whether the specified key is pressed.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key is pressed; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsKeyPressed(ConsoleKey key)
        {
              return Console.KeyAvailable && Console.ReadKey(true).Key == key;
        }
    }
}
 
