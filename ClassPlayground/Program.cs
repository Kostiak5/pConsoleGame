using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;

namespace ClassPlayground
{
    internal class Program
    {
        static List<Group> createGroups() //create objects of class Group
        {
            List<string> groupNames = new List<string> {"Socialisté", "Kapitalisté", "Progresivisté", "Konzervativci", "Internacionalisté", "Nacionalisté", "Uživatelé MHD", "Motoristé"};

            List<Group> newGroups = new List<Group>();
            Random rnd = new Random();
            foreach(string name in groupNames)
            {
                newGroups.Add(new Group(name, 50, rnd.Next(45, 75), 50)); //random turnout for every object (=every voting group)
            }

            return newGroups;
        }

        static double convertTesting(string inputStr, double upperLimit, double lowerLimit) //checks whether user input is an integer in a given range
        {
            double convertedNum = 0;

            while (!Double.TryParse(inputStr, out convertedNum) || ((Double.Parse(inputStr) > upperLimit || Double.Parse(inputStr) < lowerLimit)))
            {
                Console.WriteLine("Neplatný vstup. Zadej číslo ještě jednou.");
                inputStr = Console.ReadLine();
            }

            convertedNum = Double.Parse(inputStr);
            return convertedNum;
        }

        static void checkForNonsenseOrLoss(List<Group> groups, double budget)
        {
            for (int i = 0; i < groups.Count(); i++) //checking for nonsensical scenarios and fixing them (support of the player cannot be more than 100% etc.)
            {
                if (groups[i].support > 100)
                {
                    groups[i].support = 100;
                }
                else if (groups[i].support < 0)
                {
                    groups[i].support = 0;
                }

                if (groups[i].turnout > 90)
                {
                    groups[i].turnout = 90;
                }
                else if (groups[i].turnout < 0)
                {
                    groups[i].turnout = 0;
                }

                if (groups[i].share < 0)
                {
                    groups[i].share = 0;
                }
                if (groups[i].share > 100)
                {
                    groups[i].share = 100;
                }
            }

            if (budget <= 0) //checking for budget being equal to or below 0
            {
                Console.WriteLine("Veškeré finanční prostředky státu byly bohužel vyčerpány. Prohráli jste.");
                Console.ReadKey();
                Environment.Exit(0);
            } 
        }

        static double reformList(double refNum, List<Group> groups, double budget)
        {
            string changeInputStr; //used for some reforms, where the scale of reform is set by the player (-100 to 100 or 1 to 100)
            double change; //converted changeInputStr

            switch(refNum)
            {
                case 1:
                    Console.WriteLine("Zvýšení nebo snížení DPH:");
                    Console.WriteLine("Zvýšení může být značně neoblíbené, především u kapitalistů. Výrazně to ale prospěje státnímu rozpočtu.");
                    Console.WriteLine("Zadej celé číslo od -100 do 100. -100 znamená maximální snížení, 100 znamená maximální zvýšení.");
                    changeInputStr = Console.ReadLine();
                    change = convertTesting(changeInputStr, 100, -100); //checks whether input is correct

                    for (int i = 0; i < groups.Count(); i++) //this policy affects every voting group
                    {
                        groups[i].support -= 0.5 * change; //change of support depends on player's input (input was 50 => -25% of support, input was -10 => +5% of support)
                        if (Math.Abs(change) >= 70) //if |input| >= 70, turnout of voters is affected too
                            groups[i].turnout += 15;
                    }

                    budget += 0.8 * change; //budget is affected as well
                    break;
                case 2:
                    Console.WriteLine("Zvýšení nebo snížení důchodů:");
                    Console.WriteLine("Důchody hrají velkou roli především pro loajalitu socialistů.");
                    Console.WriteLine("Zadej celé číslo od -100 do 100. -100 znamená maximální snížení, 100 znamená maximální zvýšení.");
                    changeInputStr = Console.ReadLine();
                    change = convertTesting(changeInputStr, 100, -100);

                    groups[0].support += 0.5 * change; //this policy only affects Socialists (group[0]) and Capitalists (group[1])
                    groups[1].support -= 0.4 * change;

                    budget -= 0.4 * change;

                    if (Math.Abs(change) >= 70)
                        groups[0].turnout += 30;
                    break;
                case 3:
                    Console.WriteLine("Zastropování cen:");
                    Console.WriteLine("Má dopad na popularitu, ale i na ekonomiku.");
                    //no user input needed for this policy
                    groups[0].support += 25; //no user input => change of support is a constant (25 in this case)
                    groups[1].support -= 25;

                    budget -= 20;

                    break;
                case 4:
                    Console.WriteLine("Zvýšení nebo snížení výdajů na armádu:");
                    Console.WriteLine("Armáda má velkou podporu u nacionalistů i konzervativců.");
                    Console.WriteLine("Zadej celé číslo od -100 do 100. -100 znamená maximální snížení, 100 znamená maximální zvýšení.");
                    changeInputStr = Console.ReadLine();
                    change = convertTesting(changeInputStr, 100, -100);

                    groups[2].support -= 0.3 * change;
                    groups[3].support += 0.4 * change;
                    groups[4].support -= 0.4 * change;
                    groups[5].support += 0.7 * change;

                    budget -= 0.3 * change;

                    if (Math.Abs(change) >= 70)
                    {
                        groups[4].turnout += 20;
                        groups[5].turnout += 30;
                    }
                    break;
                case 5:
                    Console.WriteLine("Zavedení povinné základní vojenské služby:");
                    Console.WriteLine("Armáda má velkou podporu u nacionalistů i konzervativců. Je to však velice nepopulární mezi mladými progresivisty.");
                    Console.WriteLine("Více mladých přijde k volbám, chtějí vyjádřit svůj nesouhlas.");

                    groups[2].support -= 40;
                    groups[3].support += 30;
                    groups[4].support -= 40;
                    groups[5].support += 50;

                    groups[2].turnout += 15;
                    break;
                case 6:
                    Console.WriteLine("Zvětšení nebo zmenšení práv policie:");
                    Console.WriteLine("Silnější ozbrojená policie má podporu u nacionalistů i konzervativců, progresivisté ale vystupují proti násilí ze strany policistů.");
                    Console.WriteLine("Zadej celé číslo od -100 do 100. -100 znamená maximální zmenšení, 100 znamená maximální zvětšení.");
                    changeInputStr = Console.ReadLine();
                    change = convertTesting(changeInputStr, 100, -100);

                    groups[2].support -= 0.5 * change;
                    groups[3].support += 0.5 * change;
                    groups[5].support += 0.3 * change;

                    if (Math.Abs(change) >= 70)
                    {
                        groups[2].turnout += 20;
                        groups[3].turnout += 20;
                    }
                    break;
                case 7:
                    Console.WriteLine("Legalizace hazardních her:");
                    Console.WriteLine("Hazardní hry mají podporu mezi kapitalisty a částečně i progresivisty. Díky zdanění také přináší určitý výdělek pro státní rozpočet.");


                    groups[0].support -= 30;
                    groups[1].support += 30;
                    groups[2].support += 12;
                    groups[3].support -= 12;

                    budget += 15;
                    break;
                case 8:
                    Console.WriteLine("Legalizace lehkých drog:");
                    Console.WriteLine("Toto opatření má podporu mezi kapitalisty a progresivisty, konzervativci jsou naopak striktně proti. Díky zdanění také přináší určitý výdělek pro státní rozpočet.");


                    groups[0].support -= 20;
                    groups[1].support += 20;
                    groups[2].support += 30;
                    groups[3].support -= 35;

                    budget += 15;
                    break;
                case 9:
                    Console.WriteLine("Zavedení trestu smrti:");
                    Console.WriteLine("Toto opatření má podporu mezi konzervativci a nacionalisty, kteří trest smrti vnímají jako spravedlivý. Naopak internacionalisté a progresivisté to vidí jako nelidské opatření, které může vést ke zhoršení vztahů s jinými demokratickými zeměmi, někteří z nich dokonce zemi opustí.");


                    groups[4].support -= 20;
                    groups[5].support += 20;
                    groups[2].support -= 30;
                    groups[3].support += 30;

                    groups[4].share -= 10;
                    groups[5].share += 10;
                    groups[2].share -= 10;
                    groups[3].share += 10;
                    break;

                case 10:
                    Console.WriteLine("Dotace na elektroauta:");
                    Console.WriteLine("Jako správný krok toto vnímají progresivisté, částečně i motoristé a socialisté. Kapitalisté vnímají dotace negativně. Opatření také povede ke zvýšení celkového počtu motoristů (vlastníků aut).");
                    Console.WriteLine("Zadej celé číslo od 1 do 100. 1 znamená nejmenší možné dotace, 100 znamená největší.");
                    changeInputStr = Console.ReadLine();
                    change = convertTesting(changeInputStr, 100, 1);

                    groups[0].support += 0.2 * change;
                    groups[1].support -= 0.2 * change;
                    groups[2].support += 0.4 * change;
                    groups[3].support -= 0.2 * change;
                    groups[7].support += 0.2 * change;

                    groups[7].share += 0.3 * change;
                    groups[6].share -= 0.3 * change;

                    budget -= 0.3 * change;
                    break;                    
                case 11:
                    Console.WriteLine("Zavádění nebo rušení oddělených autobusových pruhů:");
                    Console.WriteLine("Jako správný krok toto vnímají progresivisté a především uživatelé MHD. Motoristé to naopak vidí negativně, podle jejich názoru to přispěje ke zhoršení dopravní situace ve městech.");
                    Console.WriteLine("Opatření povede ke zvýšení celkového počtu uživatelů MHD.");
                    Console.WriteLine("Zadej celé číslo od -100 do 100. -100 znamená rušení co nejvíce pruhů, 100 znamená zavádění co nejvíce pruhů.");
                    changeInputStr = Console.ReadLine();
                    change = convertTesting(changeInputStr, 100, -100);

                    groups[2].support += 0.15 * change;
                    groups[6].support += 0.5 * change;
                    groups[7].support -= 0.5 * change;

                    groups[7].share -= 0.35 * change;
                    groups[6].share += 0.35 * change;

                    if (Math.Abs(change) >= 70)
                    {
                        groups[6].turnout += 20;
                        groups[7].turnout += 20;
                    }
                    break;
                case 12:
                    Console.WriteLine("Zvýšení nebo snížení poplatku za parkování ve městě: ");
                    Console.WriteLine("Jako správný krok vnímají progresivisté a uživatelé MHD zvýšení poplatků a méně aut. Motoristé zvýšení samozřejmě vnímají velice negativně.");
                    Console.WriteLine("Zvýšení poplatku povede ke zvýšení celkového počtu uživatelů MHD a zmenšení počtu motoristů. Také přinese určitý výdělek.");
                    Console.WriteLine("Zadej celé číslo od -100 do 100. -100 znamená největší snížení, 100 znamená zavádění co nejvíce pruhů.");
                    changeInputStr = Console.ReadLine();
                    change = convertTesting(changeInputStr, 100, -100);

                    groups[2].support += 0.15 * change;
                    groups[6].support += 0.4 * change;
                    groups[7].support -= 0.8 * change;

                    groups[7].share -= 0.35 * change;
                    groups[6].share += 0.35 * change;

                    budget += 0.3 * change;

                    if (Math.Abs(change) >= 70)
                    {
                        groups[6].turnout += 20;
                        groups[7].turnout += 20;
                    }
                    break;
                case 13:
                    Console.WriteLine("Výstavba a rozšíření dálnic: ");
                    Console.WriteLine("Výstavbu dálnic aktivně podporují motoristé. Progresivisté jsou proti, vadí jim především znečištění životního prostředí.");
                    Console.WriteLine("Zvýšení poplatku povede ke zvýšení celkového počtu motoristů.");
                    Console.WriteLine("Zadej celé číslo od 1 do 100. 1 znamená pouze nepatrné rozšíření dálnic, 100 znamená maximální rozšiřování a výstavbu.");
                    changeInputStr = Console.ReadLine();
                    change = convertTesting(changeInputStr, 100, 1);

                    groups[2].support -= 0.5 * change;
                    groups[7].support += 0.8 * change;

                    groups[7].share += 0.45 * change;
                    groups[6].share -= 0.45 * change;

                    budget -= 0.4 * change;

                    if (Math.Abs(change) >= 70)
                    {
                        groups[7].turnout += 30;
                        groups[2].turnout += 15;
                    }
                    break;
                case 14:
                    Console.WriteLine("Podpora recyklace: ");
                    Console.WriteLine("Podpora recyklace ze strany státu je podle progresivistů velice důležitým a přínosným krokem. Kapitalisté jsou proti výdajům státu.");
                    Console.WriteLine("Zadej celé číslo od 1 do 100. 1 znamená pouze nepatrnou podporu, 100 znamená maximální podporu.");
                    changeInputStr = Console.ReadLine();
                    change = convertTesting(changeInputStr, 100, 1);

                    groups[1].support -= 0.25 * change;
                    groups[2].support += 0.5 * change;

                    budget -= 0.3 * change;
                    break;
                case 15:
                    Console.WriteLine("Likvidace jaderných elektráren: ");
                    Console.WriteLine("Velice kontroverzní krok. Progresivisté jsou silně pro, avšak část se odtrhne a přejde ke konzervativcům kvůli nesouhlasu.");
                    Console.WriteLine("Důsledkem je také snížení rozpočtu.");

                    groups[2].support += 40;
                    groups[3].support -= 20;

                    groups[2].share -= 10;
                    groups[3].share += 10;

                    budget -= 15;
                    break;
                case 16:
                    Console.WriteLine("Dotace na lokální výrobky a potraviny: ");
                    Console.WriteLine("Dotace na podporu místních výrobků jsou oblíbené především mezi nacionalisty a socialisty, ale kromě kapitalistů přinesou nárůst oblíbenosti i u ostatních skupin voličů.");
                    Console.WriteLine("Zadej celé číslo od 1 do 100. 1 znamená pouze nepatrné dotace, 100 znamená maximální dotace.");
                    changeInputStr = Console.ReadLine();
                    change = convertTesting(changeInputStr, 100, 1);

                    for (int i = 0; i < groups.Count(); i++)
                    {
                        groups[i].support += 0.2 * change;
                    }
                    groups[5].support += 0.4 * change;
                    groups[0].support += 0.2 * change;

                    budget -= 0.3 * change;
                    break;
                case 17:
                    Console.WriteLine("Vstup do zahraniční aliance: ");
                    Console.WriteLine("Velice podporované ze strany internacionalistů, zvýší to i jejich volební účast. Nacionalisté jsou naopak proti. Díky ekonomickým výhodám je účast v alianci dobrá i pro rozpočet.");

                    groups[4].support += 40;
                    groups[5].support -= 35;

                    budget += 20;

                    groups[4].turnout += 20;
                    break;
                case 18:
                    Console.WriteLine("Zavedení genderových kvót: ");
                    Console.WriteLine("Velice podporované ze strany progresivistů a socialistů. Naopak konzervativci a kapitalisté krok vnímají jako zbytečný až nesmyslný.");

                    groups[0].support += 30;
                    groups[1].support -= 30;
                    groups[2].support += 30;
                    groups[3].support -= 30;
                    break;
                case 19:
                    Console.WriteLine("Uzákonění stejnopohlavních sňatků: ");
                    Console.WriteLine("Velice podporované ze strany progresivistů, zvýší to i jejich volební účast. Konzervativci to vnímají jako narušení tradiční představy rodiny a manželství.");

                    groups[2].support += 30;
                    groups[3].support -= 30;

                    groups[2].turnout += 20;
                    break;
                case 20:
                    Console.WriteLine("Příspěvek na dítě: ");
                    Console.WriteLine("Velice oblíbené mezi konzervativci i socialisty. Kapitalisté jsou proti zásahům státu do ekonomiky.");
                    Console.WriteLine("Zadej celé číslo od 1 do 100. 1 znamená pouze nepatrné příspěvky, 100 znamená maximální podporu rodin.");
                    changeInputStr = Console.ReadLine();
                    change = convertTesting(changeInputStr, 100, 1);

                    groups[0].support += 0.4 * change;
                    groups[1].support -= 0.4 * change;
                    groups[3].support += 0.5 * change;

                    budget -= 0.3 * change;

                    if (Math.Abs(change) >= 70)
                    {
                        groups[0].turnout += 15;
                        groups[1].turnout += 15;
                        groups[3].turnout += 15;
                    }

                    break;
            }

            Console.WriteLine("\nVšechny zadané reformy byly provedeny.\n");
            Console.ReadKey();
            checkForNonsenseOrLoss(groups, budget);
            return budget;
        }

        static double reform(List<Group> groups, double budget)
        {
            
            Console.WriteLine("\nTeď můžete provést jednu z těchto reforem:\n");
            Console.Write(" * (1) Zvýšení nebo snížení DPH\r\n * (2) Zvýšení nebo snížení důchodů\r\n * (3) Zastropování cen\r\n * (4) Zvýšení nebo snížení výdajů na armádu\r\n * (5) Zavedení povinné základní vojenské služby\r\n * (6) Rozšíření nebo snížení práv policie\r\n * (7) Legalizace hazardních her\r\n * (8) Legalizace lehkých drog\r\n * (9) Zavedení trestu smrti\r\n * (10) Dotace na elektroauta\r\n * (11) Zavádění nebo rušení oddělených autobusových pruhů\r\n * (12) Zvýšení nebo snížení poplatku za parkování ve městě\r\n * (13) Výstavba a rozšíření dálnic\r\n * (14) Podpora recyklace\r\n * (15) Likvidace jaderných elektráren\r\n * (16) Dotace na lokální výrobky a potraviny\r\n * (17) Vstup do zahraniční aliance\r\n * (18) Zavedení genderových kvót\r\n * (19) Uzákonění stejnopohlavních sňatků\r\n * (20) Příspěvek na dítě\r\n");
            Console.WriteLine("\nReformy můžou mít vliv na popularitu, zastoupení nebo volební účast různých volebních skupin.\n");
            Console.WriteLine("Pokud chcete jakoukoliv z nich provést, zadejte odpovídající číslo. Pokud ne, zadejte cokoliv jiného a zmáčkněte Enter.");
            string inputReformStr = Console.ReadLine(); //number of the reform chosen by player
            int convertedNum;
            if (Int32.TryParse(inputReformStr, out convertedNum) && Int32.Parse(inputReformStr) <= 20 && Int32.Parse(inputReformStr) >= 0)
            {
                convertedNum = Int32.Parse(inputReformStr);
                budget = reformList(convertedNum, groups, budget);
            }


            return budget; //returning budget because it can be affected by reforms
        }

        static double events(List<Group> groups, double budget, int rnd)
        {
            Console.WriteLine("\nNečekaná událost:");
            switch(rnd)
            {
                case 1:
                    Console.WriteLine("VÁLKA");
                    Console.WriteLine("Sousední země byla napadena jednou nejmenovanou velmocí. Ve společnosti sílí strach a národní vědomí nabývá na hodnotě.");
                    Console.WriteLine("Důsledky: Roste počet nacionalistů. Kvůli humanitární pomoci sousední zemi lehce klesá rozpočet.");

                    groups[5].share += 20;
                    groups[4].share -= 20;
                    budget -= 20;
                    break;
                case 2:
                    Console.WriteLine("EKONOMICKÁ KRIZE");
                    Console.WriteLine("Celý svět je silně postižen nedávným krachem burzy. Pro přívržence kapitalismu je to šokující zprávu, mnozí přechází k socalismu.");
                    Console.WriteLine("Důsledky: Roste počet socialistů. Státní rozpočet je silně zasažen.");

                    groups[0].share += 20;
                    groups[1].share -= 20;
                    budget -= 50;
                    break;
                case 3:
                    Console.WriteLine("PANDEMIE");
                    Console.WriteLine("V sousední zemi byl bohužel sněden netopýr a má to globální následky.");
                    Console.WriteLine("Důsledky: Lehce vzroste počet nacionalistů a motoristů (V MHD je větší šance se nakazit). Státní rozpočet je zasažen.");

                    groups[5].share += 10;
                    groups[4].share -= 10;
                    groups[7].share += 20;
                    groups[6].share -= 20;
                    budget -= 25;
                    break;
                case 4:
                    Console.WriteLine("NOVÝ VYNÁLEZ A OBROVSKÁ POPTÁVKA");
                    Console.WriteLine("V laboratořích naší země došlo k převratnému vynálezu, který navždy změnil průmyslovou výrobu.");
                    Console.WriteLine("Důsledky: Roste počet nacionalistů a progresivistů. Rozpočet je ovlivněn velice pozitivně.");

                    groups[5].share += 15;
                    groups[4].share -= 15;
                    groups[2].share += 15;
                    groups[3].share -= 15;
                    budget += 50;
                    break;
                case 5:
                    Console.WriteLine("TERORISMUS NA VZESTUPU");
                    Console.WriteLine("Během nedávného teroristického útoku zemřelo několik nevinných občanů. Společnost je v šoku, jsou požadována příslušná opatření.");
                    Console.WriteLine("Důsledky: Roste počet nacionalistů a konzervativců.");

                    groups[5].share += 20;
                    groups[4].share -= 20;
                    groups[3].share += 20;
                    groups[2].share -= 20;
                    break;
                case 6:
                    Console.WriteLine("ÚSPĚŠNÁ REKLAMNÍ KAMPAŇ PROTI AUTŮM");
                    Console.WriteLine("Jeden ze železničních dopravců uspořádal velice chytlavou reklamní kampaň, která zapůsobila i na mnohé řidiče.");
                    Console.WriteLine("Důsledky: Roste počet uživatelů MHD.");

                    groups[6].share += 20;
                    groups[7].share -= 20;
                    budget -= 20;
                    break;
                case 7:
                    Console.WriteLine("JADERNÁ HAVÁRIE");
                    Console.WriteLine("Kvůli neopatrnosti pracovníků jaderné elektrárny došlo k přehřátí jednoho z reaktorů. Zásah záchranných složek byl rychlý a bezchybný, avšak ve společnosti to vyvolalo rozruch.");
                    Console.WriteLine("Důsledky: Vzroste počet progresivistů (kteří jsou pro úplné odstavení jaderných elektráren). Mírně klesne rozpočet.");

                    groups[2].share += 20;
                    groups[3].share -= 20;
                    budget -= 15;
                    break;
                case 8:
                    Console.WriteLine("VLNA SOLIDARITY");
                    Console.WriteLine("Naše země je zasažena tornádem. Mnozí občané sousedních zemí projevili soucit, zasažené podpořili i materiálně. Podpora spolupráce se sousedními zeměmi tak nyní začala stoupat.");
                    Console.WriteLine("Důsledky: Vzroste počet internacionalistů.");

                    groups[4].share += 20;
                    groups[5].share -= 20;
                    break;
                case 9:
                    Console.WriteLine("SKANDÁL");
                    Console.WriteLine("Právě se ukázalo, že členové vaší vlády brali úplatky od jedné společnosti a zaručovali jí prioritní postavení v tendrech.");
                    Console.WriteLine("Důsledky: Vaše oblíbenost mírně klesne u všech voličů.");
                    //Console.WriteLine("POZOR, pokud jste už provedli reformu ZAVEDENÍ CENZURY, tak oblíbenost neklesne, občané se o skandálu nedozví.") //unavailable for now

                    for (int i = 0; i < groups.Count(); i++)
                    {
                        groups[i].support -= 15;
                    }
                    
                    break;
            }

            Console.ReadKey();
            checkForNonsenseOrLoss(groups, budget);
            return budget;
        }

        static bool interviewReadKey() //waits until user presses A or N and returns true/false (an answer for an interview question)
        {
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.A)
                {
                    return true;
                }
                else if (keyInfo.Key == ConsoleKey.N)
                {
                    return false;
                }
            } while (keyInfo.Key != ConsoleKey.A || keyInfo.Key != ConsoleKey.N);

            return false;
        }

        static void interview(List<Group> groups, int rnd)
        {
            Console.WriteLine("\nByli jste pozváni na rozhovor a novinář vám položil otázku, na kterou musíte odpovědět.");
            Console.WriteLine("\nVaše odpověď bude mít vliv na podporu ze strany různých voličských skupin, ale nijak se nepromítne do rozpočtu.");
            Console.WriteLine("\nPokud souhlasíte, zmáčkněte tlačítko A na klávesnici.");
            Console.WriteLine("Pokud ne, zmáčkněte tlačítko N.\n");

            switch (rnd)
            {
                case 1:
                    Console.WriteLine("Naše země v poslední době čelí narůstajícímu přílivu migrantů. Je podle Vás správným řešením zavedení přísné kontroly na hranicích?");
                    if (interviewReadKey()) //function returns true if player pressed A
                    {
                        groups[5].support += 20;
                        groups[4].support -= 20;
                        groups[3].support += 15;
                        groups[2].support -= 15;
                    }
                    else //if player pressed N
                    {
                        groups[4].support += 20;
                        groups[5].support -= 20;
                        groups[3].support -= 15;
                        groups[2].support += 15;
                    }
                    break;
                case 2:
                    Console.WriteLine("V nové petici, kterou podepsalo již několik desítek tisíc lidí, se mluví o zrušení údaje o pohlaví v občanském průkazu. Podepsal byste ji?");
                    if (interviewReadKey())
                    {
                        groups[2].support += 20;
                        groups[3].support -= 20;
                    }
                    else
                    {
                        groups[3].support += 20;
                        groups[2].support -= 20;
                    }
                    break;
                case 3:
                    Console.WriteLine("Vnímáte rozvoj nových technologií a umělé inteligence pozitivně?");
                    if (interviewReadKey())
                    {
                        groups[2].support += 10;
                        groups[3].support -= 10;
                    }
                    else
                    {
                        groups[3].support += 10;
                        groups[2].support -= 10;
                    }
                    break;
                case 4:
                    Console.WriteLine("Myslíte, že velcí zahraniční investoři prospívají rozvoji ekonomiky našeho státu?");
                    if (interviewReadKey())
                    {
                        groups[4].support += 10;
                        groups[5].support -= 10;
                    }
                    else
                    {
                        groups[5].support += 10;
                        groups[4].support -= 10;
                    }
                    break;
                case 5:
                    Console.WriteLine("Myslíte, že by se ve školách měla angličtina vzhledem ke globalizaci vyučovat ještě intenzivněji?");
                    if (interviewReadKey())
                    {
                        groups[4].support += 10;
                        groups[5].support -= 10;
                    }
                    else
                    {
                        groups[5].support += 10;
                        groups[4].support -= 10;
                    }
                    break;
                case 6:
                    Console.WriteLine("Je progresivní daň z příjmu (čím větší plat, tím větší procento z platu se odvádí) podle vás férová vůči všem?");
                    if (interviewReadKey())
                    {
                        groups[0].support += 15;
                        groups[1].support -= 15;
                    }
                    else
                    {
                        groups[1].support += 15;
                        groups[0].support -= 15;
                    }
                    break;
                case 7:
                    Console.WriteLine("Jedním z našich důležitých partnerů je stát, jehož režim je ale v poslední době znám nedodržováním lidských práv. Měli bychom s ním pokračovat ve spolupráci?");
                    if (interviewReadKey())
                    {
                        groups[1].support += 15;
                        groups[2].support -= 20;
                    }
                    else
                    {
                        groups[2].support += 20;
                        groups[1].support -= 15;
                    }
                    break;
                case 8:
                    Console.WriteLine("Inflace má v poslední době větší dopad i na ceny v obchodech. Měl by stát v této době zvýšit sociální příspěvky, a to především chudší části populace?");
                    if (interviewReadKey())
                    {
                        groups[0].support += 20;
                        groups[1].support -= 20;
                    }
                    else
                    {
                        groups[1].support += 20;
                        groups[0].support -= 20;
                    }
                    break;
                case 9:
                    Console.WriteLine("Je rovnost a spravedlnost důležitější než soukromé vlastnictví?");
                    if (interviewReadKey())
                    {
                        groups[0].support += 15;
                        groups[1].support -= 15;
                    }
                    else
                    {
                        groups[1].support += 15;
                        groups[0].support -= 15;
                    }
                    break;
                case 10:
                    Console.WriteLine("Je podle Vás rozšíření stávajících silnic řešením kolabující automobilové dopravy ve městě?");
                    if (interviewReadKey())
                    {
                        groups[7].support += 15;
                        groups[6].support -= 15;
                    }
                    else
                    {
                        groups[6].support += 15;
                        groups[7].support -= 15;
                    }
                    break;
                case 11:
                    Console.WriteLine("'Existují pouze 2 pohlaví.' Souhlasíte s tímto výrokem?");
                    if (interviewReadKey())
                    {
                        groups[3].support += 15;
                        groups[2].support -= 15;
                    }
                    else
                    {
                        groups[2].support += 15;
                        groups[3].support -= 15;
                    }
                    break;
                case 12:
                    Console.WriteLine("V sousedním státě byl náš občan zatčen kvůli použití konopí pro rekreační účely. Byl tento krok podle vás oprávněný?");
                    if (interviewReadKey())
                    {
                        groups[3].support += 15;
                        groups[2].support -= 15;
                    }
                    else
                    {
                        groups[2].support += 15;
                        groups[3].support -= 15;
                    }
                    break;
                case 13:
                    Console.WriteLine("Státní železniční podnik je silně ztrátový a nevýhodný pro rozpočet. Měl by stát tento podnik privatizovat?");
                    if (interviewReadKey())
                    {
                        groups[1].support += 15;
                        groups[0].support -= 15;
                        groups[6].support -= 15;
                        groups[7].support += 10;
                    }
                    else
                    {
                        groups[0].support += 15;
                        groups[1].support -= 15;
                        groups[7].support -= 10;
                        groups[6].support += 15;
                    }
                    break;
                case 14:
                    Console.WriteLine("Někteří si myslí, že podmínky ve stávajicích věznicích jsou příliš kruté. Mělo by se vězení především snažit o psychologickou převýchovu zločinců a zajistit dobré podmínky pro návrat do společnosti?");
                    if (interviewReadKey())
                    {
                        groups[2].support += 15;
                        groups[3].support -= 15;
                    }
                    else
                    {
                        groups[3].support += 15;
                        groups[2].support -= 15;
                    }
                    break;
                case 15:
                    Console.WriteLine("Podporujete možnost, že by se ve volbách mohlo volit i online?");
                    if (interviewReadKey())
                    {
                        groups[2].support += 10;
                        groups[3].support -= 10;
                    }
                    else
                    {
                        groups[3].support += 10;
                        groups[2].support -= 10;
                    }
                    break;
            }

            Console.WriteLine("\nVaše odpověď byla zaznamenána.\n");
            Console.ReadKey();
            checkForNonsenseOrLoss(groups, 100); //budget is not influenced by interviews, therefore I just use a random number (100) instead of real budget value here
            return;
        }
             
        static bool vote(List<Group> groups)
        {
            Console.WriteLine("Dnes proběhly volby. Hlasy byly sečteny a vítěz byl právě oznámen.");
            Console.WriteLine("Zmáčkněte jakékoliv tlačítko, abyste se dověděli výsledky.");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("Výsledky:");

            //formula for the vote result: (sum of support * turnout of each group) / number of groups * coefficient (without coefficient, the result represents how many ppl voted for the player overall [30% for him, 30% for opposition, 40% didnt vote] but we need to only take voters into account [50% for him, 50% for oppo])

            double playersTurnout = 0, oppositionsTurnout = 0;
            for (int i = 0; i < groups.Count(); i++)
            {
                Console.WriteLine(groups[i].name + " - podpora " + groups[i].support + "%      volební účast " + groups[i].turnout + "%      z celé populace tvoří tato skupina " + groups[i].share + "%");
                playersTurnout += (groups[i].support * (groups[i].turnout / 100));
                oppositionsTurnout += ((100 - groups[i].support) * (groups[i].turnout / 100));
            }

            double coefficient = 100 / ((playersTurnout / groups.Count() + (oppositionsTurnout / groups.Count())));

            Console.WriteLine();
            Console.WriteLine("\nPro tvou stranu hlasovalo        " + String.Format("{0:0.00}", (playersTurnout / groups.Count() * coefficient)) + "%   (z celé populace je to " + String.Format("{0:0.00}", (playersTurnout / groups.Count())) + "%)");
            Console.WriteLine("\nPro opoziční koalici hlasovalo   " + String.Format("{0:0.00}", (oppositionsTurnout / groups.Count() * coefficient)) + "%   (z celé populace je to " + String.Format("{0:0.00}", (oppositionsTurnout / groups.Count()) + "%)"));
            Console.WriteLine("\nCelková volební účast:  " + (playersTurnout / groups.Count() + oppositionsTurnout / groups.Count()) + "%\n");

            if ((playersTurnout / groups.Count() * coefficient) > 50)
            {
                Console.WriteLine("Gratuluji ti k vítězství ve volbách! Můžeš pokračovat ve vládnutí.");
                Console.WriteLine("Pro ukončení hry zmáčkni Esc, jakékoliv jiné tlačítko bude znamenat pokračování ve hře.");

                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.Escape)
                    return false;
                else
                    return true;
            }
            else if ((playersTurnout / groups.Count() * coefficient) == 50)
            {
                Console.WriteLine("Sice jsi získal přesných 50 procent podpory ze strany voličů, volební komise však byla podplacena opozicí a uznala několik hlasů pro tvou stranu jako neplatné.");
                Console.WriteLine("Holt jsou si všichni rovni, ale někteří jsou si rovnější. Třeba se zadaří příště.");
                Console.ReadKey();
                return false;
            }
            else
            {
                Console.WriteLine("Bohužel se tvé kompetence státovlády ukázaly jako nedostatečné. Nebo byla volební komise podplacena.");
                Console.WriteLine("Nikdo vlastně neví, ale prohrál jsi. Snad se zadaří příště.");
                Console.ReadKey();
                return false;
            }
        }

        static void report(List<Group> groups, double budget) //every year/round lets player know about their estimated vote results
        {
            double playersTurnout = 0, oppositionsTurnout = 0;
            for (int i = 0; i < groups.Count(); i++)
            {
                Console.WriteLine(groups[i].name + " - podpora " + groups[i].support + "%      očekávaná volební účast " + groups[i].turnout + "%      z celé populace tvoří tato skupina " + groups[i].share + "%");
                playersTurnout += (groups[i].support * (groups[i].turnout / 100));
                oppositionsTurnout += ((100 - groups[i].support) * (groups[i].turnout / 100));
            }

            double coefficient = 100 / ((playersTurnout / groups.Count() + (oppositionsTurnout / groups.Count())));

            Console.WriteLine("\nPro vaši stranu by mělo hlasovat " + String.Format("{0:0.00}", (playersTurnout / groups.Count() * coefficient)) + "% voličů");
            Console.WriteLine("\nOčekávaná volební účast: " + String.Format("{0:0.00}", (playersTurnout / groups.Count() + oppositionsTurnout / groups.Count())) + "% ");
            Console.WriteLine("\nRozpočet: " + budget + " mld. $");
            Console.WriteLine("\nPro pokračování zmáčkněte kterékoliv tlačítko.");
            Console.ReadKey();
            return;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Vítejte v SIMULÁTORU VLÁDY.");
            Console.Write("Hra by měla být silně zjednodušenou simulací politické vlády a voleb.\n\n Pomocí příkazů v konzoli rozhodujete o některých politických/sociálních/ekonomických otázkách státu, \n můžete provádět reformy, zvyšovat nebo snižovat daně, případně se vyjadřujete k aktuální politické události. \n\n Vaším cílem je udržet se u vlády tak, že si získáte přízeň většiny voličů (tím, že k ním budete mít blízko ideologicky) a zároveň udržet státní rozpočet nad nulou.\n");
            Console.WriteLine("Pokud si chcete pročíst více informací ke hře, najdete je na odkazu\n https://docs.google.com/document/d/1jGomLfbEbd5ZvgQu4BHEmqMU-yRZdeVo957xxa2y2ec/edit?usp=sharing \n");
            Console.WriteLine("Pro začátek hry zmáčkněte jakékoliv tlačítko.");
            Console.ReadKey();
            Console.Clear();


            List<Group> groups = createGroups();
            double budget = 100;

            int electPeriod = 1;
            do
            {
                Console.Clear();

                Random rnd = new Random();
                List<int> usedEvents = new List<int>(); //to ensure that no events occur twice during 1 electoral cycle
                List<int> usedInterviews = new List<int>(); //to ensure that no interview question is used twice during 1 electoral cycle

                for (int i = 0; i < 5; ++i)
                {
                    if (i == 0)
                    {
                        Console.WriteLine($"Právě jste vyhráli své {electPeriod}. volby\n");
                        Console.WriteLine("Nejprve se seznamte s aktuální situací v zemi:\n");
                    }
                    else if (i == 4)
                    {
                        Console.WriteLine("\nVolby se konají za 1 rok.");
                        Console.WriteLine("\nAktuální situace v zemi:\n");
                    }
                    else
                    {
                        Console.WriteLine("\nVolby se konají za " + (5 - i) + " roky.");
                        Console.WriteLine("\nAktuální situace v zemi:\n");
                    }

                    report(groups, budget);
                    Console.Clear();

                    budget = reform(groups, budget); //REFORM
                    Console.Clear();

                    if (i % 2 == 1) //random event happens every 2nd year 
                    {
                        bool eventAlreadyUsed;
                        int eventRndNum;
                        do //generate random number that wasnt used before (to avoid repetition of events during 1 electoral cycle)
                        {
                            eventAlreadyUsed = false;
                            eventRndNum = rnd.Next(1, 10);
                            for (int j = 0; j < usedEvents.Count(); j++)
                            {
                                if (usedEvents[j] == eventRndNum)
                                {
                                    eventAlreadyUsed = true;
                                    break;
                                }
                            }
                        } while (eventAlreadyUsed == true);

                        usedEvents.Add(eventRndNum);
                        budget = events(groups, budget, eventRndNum); //RANDOM EVENT
                        Console.Clear();
                    }
                    
                    bool interviewAlreadyUsed;
                    int interviewRndNum;
                    do //generate random interview question that wasnt used before (to avoid repetition of questions during 1 electoral cycle)
                    {
                        interviewAlreadyUsed = false;
                        interviewRndNum = rnd.Next(1, 16);
                        for (int j = 0; j < usedInterviews.Count(); j++)
                        {
                            if (usedInterviews[j] == interviewRndNum)
                            {
                                interviewAlreadyUsed = true;
                                break;
                            }
                        }
                    } while (interviewAlreadyUsed == true);

                    usedInterviews.Add(interviewRndNum);
                    interview(groups, interviewRndNum); //INTERVIEW QUESTION
                    Console.Clear();
                }
                electPeriod++;
            } while (vote(groups)); //while player wins the vote, continue with the next electoral cycle
        }
    }
}

/*
 * Zvýšení/snížení DPH +0.8 rozpočet, -0.8 cep
 * Zvýšení/snížení důchodů -0.4 rozpočet, +0.5 soc, -0.4 kap 
 * Rozšíření chráněných krajinných oblastí -0.1 rozpocet, +0.4 prog, +§0.4 prog, -0.2 motor
 * Podpora recyklace -0.2 rozpocet, +0.4 prog
 * Likvidace jaderných elektráren -0.2 rozpocet, +0.6 prog, +§0.4 trad, -0.8 konz
 * Dotace na lokální výrobky -0,3 rozpocet, +0.2 cep, +0.4 nacio, +0.2 soc, -0.25 kap
 * Výstavba a rozšíření dálnic -0.3 rozpocet, +0.8 motor, +§0.8 motor, -0.2 prog, -0.5 mhd
 * zastropovani cen -0.3 rozpocet, +0.5 soc, -0.5 kap
 * Zavedení/rušení oddělených autobusových pruhů +0.4 mhd, -0.5 motor, +§0.2 mhd
 * Zvýšení/snížení poplatku za parkování ve městech +0.2 mhd, +0.2 prog, -0.5 motor, +§0.3 mhd
 * Zvýšení/snížení sociální podpory 
 * Zavedení/zrušení genderových kvót +0.5 prog, -0.5 konz
 * Uzákonění stejnopohlavních sňatků P+0.4 prog, P-0.3 konz
 * Rozšíření/snížení práv policie -0.5 prog, +0.5 konz, +0.3 nacio
 * Pomoc chudším zemím třetího světa
 * Zavedení povinné základní vojenské služby
 * Zvýšení/snížení výdajů na armádu -0.3 rozpočet, +0.7 nacio, +0.4 konz, -0.3 prog, -0.4 inter
 * Legalizace hazardních her +0.3 rozpocet, +0.5 kap, -0.5 soc, +0.2 prog, -0.2 konz
 * Zavedení trestu smrti
 * Dotace na elektroauta
 * 
 * (1) Zvýšení nebo snížení DPH
 * (2) Zvýšení nebo snížení důchodů
 * (3) Zastropování cen
 * (4) Zvýšení nebo snížení výdajů na armádu
 * (5) Zavedení povinné základní vojenské služby
 * (6) Zvětšení nebo zmenšení práv policie
 * (7) Legalizace hazardních her
 * (8) Legalizace lehkých drog
 * (9) Zavedení trestu smrti
 * (10) Dotace na elektroauta
 * (11) Zavádění nebo rušení oddělených autobusových pruhů
 * (12) Zvýšení nebo snížení poplatku za parkování ve městě
 * (13) Výstavba a rozšíření dálnic
 * (14) Podpora recyklace
 * (15) Likvidace jaderných elektráren
 * (16) Dotace na lokální výrobky a potraviny
 * (17) Vstup do mezinárodní aliance
 * (18) Zavedení genderových kvót
 * (19) Uzákonění stejnopohlavních sňatků
 * (20) Příspěvek na dítě
 * */
