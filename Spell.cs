using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;

namespace DDDJConsole
{
    interface ISpell
    {
        void Roll();
    }

    public class Spell : ISpell
    {
        public int ID;
        public string Sound;
        public void Roll()
        {
            (new SoundPlayer(Sound)).Play();
        }

        public int RollDie(int numRolls, int sides, int modifierPerDie)
        {
            int total = 0;
            int roll;

            Random rnd = new Random();

            for (int i = 1; i <= numRolls; i++)
            {                
                roll = rnd.Next(1, sides + 1);

                if (modifierPerDie > 0)
                {
                    Console.WriteLine("[{0}]: D{1} rolled a {2} +{3} = {4}", i, sides, roll, modifierPerDie, roll + modifierPerDie);
                }else
                {
                    Console.WriteLine("[{0}]: D{1} rolled a {2}", i, sides, roll);
                }
                
                if (roll == 20) (new SoundPlayer(@"..\..\Sounds\Boomshakalaka.wav")).Play();

                total += roll + modifierPerDie;
            }
            
            if (numRolls > 1)  Console.WriteLine("Total = [{0}]" , total); 
            return total;
        }

        protected int SpellAttackBonus()
        {
            return Convert.ToInt32(Properties.Resources.PB) + Convert.ToInt32(Properties.Resources.INT);
        }
    }


    public class d20 : Spell
    {

        public d20()
        {
            ID = -1;
            Sound = @"..\..\Sounds\DJSpin.wav";
        }
        

        new public void Roll()
        {
            base.Roll();

            RollDie(1, 20,0);
        }

    }

    public class ZombiesToHit : Spell
    {

        public ZombiesToHit()
        {
            ID = (int)SpellList.ZombiesToHit;
            Sound = @"..\..\Sounds\ZombieToHit.wav";
        }


        new public void Roll()
        {
            base.Roll();

            Console.WriteLine("{0} Zombies attack", Properties.Resources.NumZombies);

            RollDie(Convert.ToInt32(Properties.Resources.NumZombies), 20, 4);
        }

    }

    public class ZombiesDamage : Spell
    {

        public ZombiesDamage()
        {
            ID = (int)SpellList.ZombiesDamage;
            Sound = @"..\..\Sounds\UndeadDamage.wav";
        }


        new public void Roll()
        {
            base.Roll();

            Console.WriteLine("{0} Zombies Hit", Properties.Resources.NumZombies);

            RollDie(Convert.ToInt32(Properties.Resources.NumZombies), 6, 1 + Convert.ToInt32(Properties.Resources.PB));
        }

    }

    public class SkeletonsToHit : Spell
    {

        public SkeletonsToHit()
        {
            ID = (int)SpellList.SkeletonsToHit;
            Sound = @"..\..\Sounds\SkeletonToHit.wav";
        }


        new public void Roll()
        {
            base.Roll();

            Console.WriteLine("{0} Skeletons attack", Properties.Resources.NumSkeletons);

            RollDie(Convert.ToInt32(Properties.Resources.NumSkeletons), 20, 4);
        }

    }

    public class SkeletonDamage : Spell
    {

        public SkeletonDamage()
        {
            ID = (int)SpellList.SkeletonsDamage;
            Sound = @"..\..\Sounds\UndeadDamage.wav";
        }


        new public void Roll()
        {
            base.Roll();

            Console.WriteLine("{0} Skeletons Hit", Properties.Resources.NumSkeletons);

            RollDie(Convert.ToInt32(Properties.Resources.NumSkeletons), 6, 2 + Convert.ToInt32(Properties.Resources.PB));
        }

    }

    public class ChillTouch : Spell
    {

        public ChillTouch()
        {
            ID = (int)SpellList.ChillTouch;
            Sound = @"..\..\Sounds\taint.wav";
        }


        new public void Roll()
        {
            base.Roll();

            Console.WriteLine("Enemy Can't Regen.  Undead have disadv on attack rolls");
            Console.WriteLine("Rolling To Hit:");
            RollDie(1, 20, SpellAttackBonus());
            Console.WriteLine("Rolling Damage:");
            RollDie(2, 8, 0);
        }

    }

    public class PoisonSpray : Spell
    {

        public PoisonSpray()
        {
            ID = (int)SpellList.PoisonSpray;
            Sound = @"..\..\Sounds\poisonnova.wav";
        }


        new public void Roll()
        {
            base.Roll();

            Console.WriteLine("Target must do a CON saving throw.");
            Console.WriteLine("Rolling Damage:");
            RollDie(2, 12, 0);
        }

    }

    public class RayOfSickness : Spell
    {

        public RayOfSickness()
        {
            ID = (int)SpellList.RayOfSickness;
            Sound = @"..\..\Sounds\lowerresist.wav";
        }


        new public void Roll()
        {
            base.Roll();

            Console.WriteLine("Target must do CON saving throw; poisoned on fail.");
            Console.WriteLine("Rolling To Hit:");
            RollDie(1, 20, SpellAttackBonus());
            Console.WriteLine("Rolling Damage:");
            RollDie(2, 8, 0);
        }        
    }

    public class Sleep : Spell
    {

        public Sleep()
        {
            ID = (int)SpellList.Sleep;
            Sound = @"..\..\Sounds\weaken.wav";
        }


        new public void Roll()
        {
            base.Roll();

            Console.WriteLine("Creatures with Lowest HP fall asleep.  Their HP must be less than remaining dice value");
            Console.WriteLine("Add another spin per slot above 1");
            Console.WriteLine("Rolling To HP to sleep:");
            RollDie(5, 8, 0);
            
        }

    }

    public class BurningHands : Spell
    {

        public BurningHands()
        {
            ID = (int)SpellList.BurningHands;
            Sound = @"..\..\Sounds\firebolt3.wav";
        }


        new public void Roll()
        {
            base.Roll();

            Console.WriteLine("Targets within 15ft cone must make DEX save (Half on success)");
            Console.WriteLine("Add another 1d6 for each slot above 1.");
            Console.WriteLine("Rolling damage:");
            RollDie(3, 6, 0);

        }

    }

    public class FlamingSphere : Spell
    {

        public FlamingSphere()
        {
            ID = (int)SpellList.FlamingSphere;
            Sound = @"..\..\Sounds\firebolt3.wav";
        }


        new public void Roll()
        {
            base.Roll();

            Console.WriteLine("Targets ending turn must make DEX save to escape");
            Console.WriteLine("Add another 1d6 for each slot above 2.");
            Console.WriteLine("Rolling damage:");
            RollDie(2, 6, 0);

        }

    }

    public class RayOfEnfeeblement : Spell
    {

        public RayOfEnfeeblement()
        {
            ID = (int)SpellList.RayOfEnfeeblement;
            Sound = @"..\..\Sounds\weaken.wav";
        }


        new public void Roll()
        {
            base.Roll();

            Console.WriteLine("Targets ending turn must make CON save to end it");            
            Console.WriteLine("Rolling to hit:");
            RollDie(1, 20, SpellAttackBonus());

        }

    }

    public class VampiricTouch : Spell
    {

        public VampiricTouch()
        {
            ID = (int)SpellList.VampiricTouch;
            Sound = @"..\..\Sounds\lowerresist.wav";
        }


        new public void Roll()
        {
            base.Roll();

            Console.WriteLine("Gain half HP back from dmg.");
            Console.WriteLine("You can continues as an action for 1 min.");
            Console.WriteLine("Add 1d6 damage above 3.");
            Console.WriteLine("Rolling To Hit:");
            RollDie(1, 20, SpellAttackBonus());
            Console.WriteLine("Rolling Damage:");
            RollDie(3, 6, 0);

        }

    }
    
    public class Lightning : Spell
    {

        public Lightning()
        {
            ID = (int)SpellList.Lightning;
            Sound = @"..\..\Sounds\Lightning1.wav";
        }


        new public void Roll()
        {
            base.Roll();

            Console.WriteLine("Targets need to DEX save (Half if success)");
                        
            RollDie(8, 6,0);            
        }

    }

    public class Blight : Spell
    {

        public Blight()
        {
            ID = (int)SpellList.Blight;
            Sound = @"..\..\Sounds\ironmaiden.wav";
        }


        new public void Roll()
        {
            base.Roll();

            Console.WriteLine("Targets need to CON save (Half if success)");

            RollDie(8, 8, 0);
        }

    }


    public class PhantasmalKillerEndOFTurn : Spell
    {

        public PhantasmalKillerEndOFTurn()
        {
            ID = (int)SpellList.PhantasmalKillerEndOFTurn;
            Sound = @"..\..\Sounds\corpseexplodecast.wav";
        }


        new public void Roll()
        {
            base.Roll();

            Console.WriteLine("Targets need to WIS save (Frightened for 1 min for fail)");
            Console.WriteLine("At end of target turn, take this damage:");
            RollDie(4, 10, 0);
        }

    }

    public class BlackTentacles : Spell
    {

        public BlackTentacles()
        {
            ID = (int)SpellList.BlackTentacles;
            Sound = @"..\..\Sounds\BlackTentacle.wav";
        }


        new public void Roll()
        {
            base.Roll();

            Console.WriteLine("Targets in 20ft takes below damage and restrained");
            Console.WriteLine("Entering targets in 20ft makes DEX saving throw at start of turn (Takes damage and restrained on fail)");
            Console.WriteLine("At end of target turn, all targets can make STR or DEX save to escape.");
            RollDie(3, 6, 0);
        }

    }




    public enum SpellList {
        D20=-5,
        ZombiesToHit,
        ZombiesDamage,
        SkeletonsToHit,
        SkeletonsDamage,
        ChillTouch,
        PoisonSpray,
        RayOfSickness,
        Sleep,
        BurningHands,
        FlamingSphere,
        RayOfEnfeeblement,
        VampiricTouch,                
        Lightning,
        Blight,
        PhantasmalKillerEndOFTurn,
        BlackTentacles

    };





}
