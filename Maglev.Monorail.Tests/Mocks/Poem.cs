using System;
using System.Collections;
using System.Diagnostics;
using Maglev.Monorail.Async;

namespace Maglev.Monorail.Tests.Mocks
{
    public class Poem
    {
        //"Death" by John Donne
        public const string poem = "\"Death\" by John Donne\n\n" +
                            "Death be not proud, though some have called thee\n" +
                            "Mighty and dreadfull, for, thou art not so,\n" +
                            "For, those, whom thou think'st, thou dost overthrow,\n" +
                            "Die not, poore death, nor yet canst thou kill me.\n" +
                            "From rest and sleepe, which but thy pictures bee,\n" +
                            "Much pleasure, then from thee, much more must flow,\n" +
                            "And soonest our best men with thee doe goe,\n" +
                            "Rest of their bones, and soules deliverie.\n" +
                            "Thou art slave to Fate, Chance, kings, and desperate men,\n" +
                            "And dost with poyson, warre, and sicknesse dwell,\n" +
                            "And poppie, or charmes can make us sleepe as well,\n" +
                            "And better then thy stroake; why swell'st thou then;\n" +
                            "One short sleepe past, wee wake eternally,\n" +
                            "And death shall be no more; death, thou shalt die.";

        public static IEnumerator ReadPoem(string poem)
        {
            //Read the poem letter by letter
            foreach (var letter in poem)
            {
                Console.Write(letter);
                switch (letter)
                {
                        //Pause for punctuation
                    case ',':
                    case ';':
                        yield return Pause(0.25f);
                        break;

                        //Long pause for full-stop
                    case '.':
                        yield return Pause(0.5f);
                        break;

                        //Short pause for anything else
                    default:
                        yield return Pause(0.005f);
                        break;
                }
            }

            //Wait for user input to close
            Console.WriteLine("\nPress any key to exit");
        }

        static IEnumerator Pause(float time)
        {
            var watch = Stopwatch.StartNew();
            while (watch.Elapsed.TotalSeconds < time)
                yield return 0;
        }
    }
}