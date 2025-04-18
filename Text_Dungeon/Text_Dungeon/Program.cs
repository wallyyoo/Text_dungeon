using System.Globalization;
using System.Reflection.Emit;
using System.Xml.Linq;


namespace Text_Dungeon
{
    internal class Program
    {

        public class Player
        {
            public string job { get; set; } = "Nomad";
            public int Lv { get; set; } = 01;

            public int p_att { get; set; }
            public int p_def { get; set; }
            public int p_hp { get; set; }
            public int base_att { get; set; } = 10;
            public int base_def { get; set; } = 5;
            public int base_hp { get; set; } = 100;

            public int money { get; set; } = 1500;

            public List<Item> Inventory { get; set; } = new List<Item>();
            public List<Item> EquippedItems { get; set; } = new List<Item> { };

            public void ShowStat()
            {
                Console.Clear();

                Console.WriteLine("\n");
                Console.WriteLine("=== [상태창] ===");
                Console.WriteLine($"\n직업: {job} ");
                Console.WriteLine($"\n레벨: {Lv} ");

                int total_att = base_att;
                int total_def = base_def;
                int total_hp = base_hp;

                foreach (Item item in EquippedItems)
                {
                    total_att += item.i_att;
                    total_def += item.i_def;
                    total_hp += item.i_hp;
                }

                Console.WriteLine($"\n공격력: {base_att}(+{total_att - base_att})={total_att} ");
                Console.WriteLine($"\n방어력: {base_def}(+{total_def - base_def})={total_def}");
                Console.WriteLine($"\n체력: {base_hp}(+{total_hp - base_hp})={total_hp} ");
                Console.WriteLine($"\n자금: {money} $$ ");
                Console.WriteLine("\n=========================");

            }
            public void ShowInventory()
            {
                Console.Clear();
                Console.WriteLine("=== [인벤토리] ===x");
                Console.WriteLine("\n 보유하고 있는 아이템을 확인하세요.");
                Console.WriteLine("\n\n <아이템 목록> ");

                if (Inventory.Count == 0)
                {
                    Console.WriteLine(" 비어있습니다.");
                }
                else
                {
                    foreach (Item item in Inventory)
                    {
                        string equipped = item.equip ? " [E] " : " [-] ";
                        string stats = "";

                        if (item.i_att != 0) stats += $" 공격력 + {item.i_att}";
                        if (item.i_def != 0) stats += $" 방어력 + {item.i_def}";
                        if (item.i_hp != 0) stats += $" 체력 + {item.i_hp}";

                        Console.WriteLine($"\n {equipped}{item.i_name} | {stats}");
                        Console.WriteLine($"\n\t ㄴ>{item.description}");
                    }
                }
            }
            public void EquipItem(int index)
            {
                index -= 1;
                if (index >= 0 && index < Inventory.Count)
                {
                    Item item = Inventory[index];
                    if (!item.equip)
                    {
                        item.equip = true;
                        EquippedItems.Add(item);

                        p_att += item.i_att;
                        p_def += item.i_def;
                        p_hp += item.i_hp;

                        Console.WriteLine($"{item.i_name} 장착 ");
                    }
                    else
                    {
                        item.equip = false;
                        EquippedItems.Remove(item);

                        p_att -= item.i_att;
                        p_def -= item.i_def;
                        p_hp -= item.i_hp;

                        Console.WriteLine($"{item.i_name} 해제 ");
                    }
                }


            }


        }
        public class Item
        {
            public string i_name { get; set; }
            public string description { get; set; }
            public bool equip { get; set; } = false;
            public int price { get; set; } = 0;

            public int i_att { get; set; } = 0;
            public int i_def { get; set; } = 0;
            public int i_hp { get; set; } = 0;

            public Item(string name, string description, int att = 0, int def = 0, int hp = 0, int price = 0)
            {
                this.i_name = name;
                this.description = description;
                this.i_att = att;
                this.i_def = def;
                this.i_hp = hp;
                this.price = price;
            }

        }
        public class Weapon : Item
        {
            public Weapon(string name, string description, int att, int price) : base(name, description, att, 0, 0, price) { }

        }
        public class Armor : Item
        {
            public Armor(string name, string description, int def, int price) : base(name, description, 0, def, 0, price) { }

        }
        public class Accessory : Item
        {
            public Accessory(string name, string description, int hp, int price) : base(name, description, 0, 0, hp, price) { }

        }
        public class NightCity
        {
            public List<Item> AvailableItems { get; set; }
            public Player Player { get; set; }

            public NightCity(Player player)
            {
                this.Player = player;
                AvailableItems = new List<Item>
                {
                    new Weapon("야마토 캐논", "\n로딩이 걸리지만 모든걸 파괴시킬 수 있습니다.", 100, 7000),
                    new Weapon("레이저 카타나", "\n로망이 살아있는 무기, 야간에는 더 멋집니다.", 65, 3000),
                    new Weapon("리볼버 샷컨", "\n한번에 여섯발씩! 화끈한 무기가 바로 여기에", 20, 4200),
                    new Armor("경량 방탄복", "\n움직일때는 좋지만 맞을땐?...", 4, 600),
                    new Armor("파워점프 슈트", "\n머리빼고 다 지켜줍니다.", 20, 5000),
                    new Accessory("인공심장", "\n진짜로 심장이 두 개면 어떤기분일까요?", 50, 10000),

                };

            }
            public void ShowNightCity()
            {
                Console.Clear();
                Console.WriteLine(" [보유 $$ ]\n{0}", Player.money);
                Console.WriteLine("\n [아이템 목록] ");

                foreach (Item item in AvailableItems)
                {
                    string status = item.equip ? "구매완료" : $"|{item.price} $$";
                    Console.WriteLine($" - \n{item.i_name} | {item.i_att + item.i_def + item.i_hp} | {status}");
                    Console.WriteLine($" \n\n{item.description} ");
                    Console.WriteLine ("==============================================");
                }

                Console.WriteLine("\n 아이템 구매");

                BuyItem();
            }
            public void BuyItem()
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("[보유 $$]\n{0}", Player.money);
                    Console.WriteLine("\n [아이템 목록]");

                    for (int i = 0; i < AvailableItems.Count; i++)
                    {
                        var item = AvailableItems[i];
                        string status = item.equip ? "구매완료" : $"{item.price} $$";
                        Console.WriteLine($" - {item.i_name} | {item.i_att + item.i_def + item.i_hp}{status}");
                        Console.WriteLine($" {item.description} ");
                    }
                    string input = Choice("\n구매할 번호를 입력하거나 0을 눌러 나가세요. >> ");
                    if (int.TryParse(input, out int index) && index > 0 && index <= AvailableItems.Count)
                    {
                        var item = AvailableItems[index - 1];
                        if (!item.equip && Player.money >= item.price)
                        {
                            Player.money -= item.price;
                            item.equip = true;
                            Player.Inventory.Add(item);
                            Console.WriteLine($"{item.i_name} 아이템을 구매!");
                            GoBack();
                            break;
                        }
                        else if (item.equip)
                        {
                            Console.WriteLine($"{item.i_name}은 이미 구매완료입니다.");
                        }
                        else
                        {
                            Console.WriteLine("금액이 부족합니다.");
                        }
                    }
                    else if (input == "0")
                    {
                        break; // GoBack 호출을 이 부분에서 처리하지 않고 바로 종료
                    }
                    else
                    {
                        Console.WriteLine("잘못된 입력입니다.");
                    }
                }




            }

            static void Main(string[] args)
            {

                Player player = new Player();


                while (true)
                {

                    Console.Clear();

                    Console.WriteLine("\n ---------------------------------------------");
                    Console.WriteLine("\n 사이버펑크 세계에 오신 당신을 환영합니다! ");
                    Console.WriteLine("\n 당신이 선택할 수 있는 건 이정도..려나요?? ");
                    Console.WriteLine("\n ---------------------------------------------");

                    Console.WriteLine("\n 1. 상태창 보기 ");
                    Console.WriteLine("\n 2. 인벤토리 확인하기 ");
                    Console.WriteLine("\n 3. 나이트 시티로 들어가기 ");
                    Console.WriteLine("\n 4. 황무지로 가기 ");
                    Console.WriteLine("\n 5. 알게 뭐야 x발! ");

                    string input = Choice("\n\n 번호입력>> ");



                    switch (input)

                    {
                        case "1":

                            while (true)
                            {
                                player.ShowStat();
                                GoBack();
                                break;
                            }
                            break;

                        case "2":

                            while (true)
                            {
                                player.ShowInventory();

                                Console.WriteLine("\n 1. 장비착용/해제");
                                string equipInput = Choice("\n 아이템 번호를 누르세요 (1부터 시작)");
                                if (int.TryParse(equipInput, out int index))
                                {
                                    player.EquipItem(index);
                                }
                                GoBack();
                                break;
                            }

                            break;

                        case "3":
                            NightCity shop = new NightCity(player);
                            shop.ShowNightCity();

                            break;

                        case "4":

                            Console.WriteLine("\n ===황무지===x");
                            break;

                        case "5":
                            Console.WriteLine(" \n 오우 이런.. 예의가 없으시군요. ");
                            Console.WriteLine(" \n 무례한 사람은 어느쪽에서도 원치않는답니다.");
                            Console.WriteLine(" \n\n 예절 주입기 가동! ");
                            break;

                        default:
                            Console.WriteLine("\n 말귀를 못 알아들으시는 건지.. ");
                            Console.WriteLine("\n 아니면 말을 못하시는 건지? 다시 묻겠습니다.");
                            break;


                    }

                }


            }
            static string Choice(string text)
            {
                Console.Write(text);
                return Console.ReadLine();
            }

            static void GoBack(string prompt = "\n\n 0. 돌아가기  ")
            {
                while (true)
                {
                    string input = Choice(prompt);
                    if (input == "0")
                    { break; }
                }
            }
            static void Humble(string prompt = " 예절이 강제로 주입됩니다. ")
            {
                while (true)
                {
                    string input = Choice(prompt);
                    if (input == "9")
                    {

                        break;
                    }
                }
            }
        }
    }
}
