namespace Robofight;

class Program{
    static void Main(string[] args)
    {
        Game game = new Game();
        game.Select_Robot();
        game.Start_Game();
    }
}
public class Game{
    Robot Player;
    string Jenis_Robot;
    Robot Enemy;
    Kemampuan ability_player;
    Kemampuan ability_enemy;
    public Kemampuan SkillList;

    int turn = 1; 
    public Game(){
        this.Enemy = new Boss_Robot();
        this.ability_enemy = new Kemampuan();
        this.ability_player = new Kemampuan();
    }

    public void Select_Robot(){
        string opsi;
        do{
            Console.WriteLine("Silahkan Memilih Robot yang Akan dimainkan\n1. Robot Electro\n2. Robot Plasma");
            Console.Write("Pilihan Robot : ");
            opsi = Console.ReadLine();
            Console.Clear();
        }while(opsi != "1" && opsi != "2");
        switch(opsi){
            case "1":
                this.Player = new Robot_Electric();
                this.Jenis_Robot = "Robot Electric";
                break;
            case "2":
                this.Player = new Robot_Plasma();
                this.Jenis_Robot = "Robot Plasma";
                break;
        }
    }

    public void Start_Game(){
        while(true){
            this.Reduce_Countdown();
            this.Player_session();
            if (Player.energi < 0 || Enemy.energi < 0 || turn == 12){
                System.Console.WriteLine("Game Berakhir");
                System.Console.WriteLine($"Game Dimenangkan Oleh {this.get_winner()}");
                break;
            }
            System.Console.Write("Tekan Enter Untuk Lanjut");
            Console.ReadLine(); 
            this.Enemy_session();

            if (Player.energi < 0 || Enemy.energi < 0 || turn == 12){
                System.Console.WriteLine("Game Berakhir");
                System.Console.WriteLine($"Game Dimenangkan Oleh {this.get_winner()}");
                break;
            } 
            this.Player.Gunakan_kemampuan(this.ability_player,this.Player,"Regen");
            this.turn ++;
            System.Console.Write("Tekan Enter Untuk Lanjut");
            Console.ReadLine(); 
        }
    }
    public void Player_session(){
        bool status_aksi = false;
        String ability;
        if(this.Jenis_Robot == "Robot Plasma"){
            ability = "Plasma";
        }else{
            ability = "Electric";
        }
        string opsi;
        do{
            Console.Clear();
            Console.WriteLine($"Turn : {this.turn}");
            Player.Cek_informasi();
            Console.WriteLine("1. Serang\n2. Gunakan Kemampuan Spesial\n3. Aktifkan Pertahanan Tambahan");
            Console.Write("Pilihan : ");
            opsi = Console.ReadLine();
        }while(opsi != "1" && opsi != "2" && opsi != "3");
        switch(opsi){
            case "1":
                Player.Serang(this.Enemy);
                status_aksi = true;
                break;
            case "2":
                status_aksi = (Player.Gunakan_kemampuan(this.ability_player,this.Enemy,ability));
                break;
            case "3":
                status_aksi = (Player.Gunakan_kemampuan(this.ability_player,this.Player,"Shield"));
                break;
        } 
        if(!(status_aksi)){;
            this.Player_session();}
        }
        
    public void Enemy_session(){
        Console.Clear();
        Enemy.Cek_informasi();
        Random random = new Random();
        bool status_aksi = false;
        string[] aksi = {"Electric","Plasma","Serang"};
        string opsi = aksi[random.Next(aksi.Length)];
        switch(opsi){
            case "Serang":
                Player.Serang(this.Player);
                status_aksi = true;
                break;
            case "Electric":
                status_aksi = (Enemy.Gunakan_kemampuan(this.ability_enemy,this.Player,"Plasma"));
                break;
            case "Plasma":
                status_aksi = (Enemy.Gunakan_kemampuan(this.ability_enemy,this.Player,"Electric"));
                break;
        } 
        if(!(status_aksi)){this.Enemy_session();}
    }

    public string get_winner(){
        if (Player.energi > Enemy.energi){
            return Player.nama;
        }else{
            return Enemy.nama;
        }
    }

    public void Reduce_Countdown(){
        if(ability_enemy.electric.current_countdown_time > 0){
            ability_enemy.electric.current_countdown_time --;
        }
        if(ability_enemy.super.current_countdown_time > 0){
            ability_enemy.super.current_countdown_time --;
        }
        if(ability_enemy.plasma.current_countdown_time > 0){
            ability_enemy.plasma.current_countdown_time --;
        } 
        if(ability_player.electric.current_countdown_time > 0){
            ability_player.electric.current_countdown_time --;
        }
        if(ability_player.super.current_countdown_time > 0){
            ability_player.super.current_countdown_time --;
        }
        if(ability_player.plasma.current_countdown_time > 0){
            ability_player.plasma.current_countdown_time --;
        }

        if(ability_player.super.remaining_time == 0){
            if(Player.armor < ability_player.super.amount){
                Player.armor = 0;
            }else{
                Player.armor -= ability_player.super.amount;
            }
        }

        if(ability_enemy.super.remaining_time == 0){
            if(Enemy.armor < ability_enemy.super.amount){
                Enemy.armor = 0;
            }else{
                Enemy.armor -= ability_enemy.super.amount;
            }
        }
    }
}

public abstract class Robot{
    // cek cd,if else berdasarkan skill, kondisi punya armor/tidak, punya armor tapi damage melebihi armor
    public string nama;
    public int energi,armor,serangan;
    public bool Gunakan_kemampuan(Kemampuan kemampuan,Robot tujuan,string Skill){
        switch(Skill){
            case "Plasma":
                if(kemampuan.plasma.current_countdown_time > 0){
                    return false;
                }
                if(tujuan.armor > kemampuan.plasma.damage_scale * this.serangan){
                    tujuan.armor -= Convert.ToInt32(kemampuan.Serangan_Plasma(kemampuan.plasma) * this.serangan);
                }else if (tujuan.armor == 0){
                    tujuan.energi -= Convert.ToInt32(kemampuan.Serangan_Plasma(kemampuan.plasma) * this.serangan);
                }else{
                    kemampuan.Serangan_Plasma(kemampuan.plasma);
                    tujuan.energi -= Convert.ToInt32(kemampuan.plasma.damage_scale * this.serangan) - tujuan.armor;
                    tujuan.armor = 0;
                }
                return true;
            case "Electric":
                if(kemampuan.electric.current_countdown_time > 0){
                    return false;
                }
                if(tujuan.armor > kemampuan.electric.damage_scale * this.serangan){
                    tujuan.armor -= Convert.ToInt32(kemampuan.Seranga_Listrik(kemampuan.electric) * this.serangan);
                    
                }else if (tujuan.armor == 0){
                    tujuan.energi -= Convert.ToInt32(kemampuan.Seranga_Listrik(kemampuan.electric) * this.serangan);
                }else{
                    kemampuan.Seranga_Listrik(kemampuan.electric);
                    tujuan.energi -= Convert.ToInt32(kemampuan.electric.damage_scale * this.serangan) - tujuan.armor;
                    tujuan.armor = 0;
                }
                return true;
            case "Shield":
                if(kemampuan.super.remaining_time > 0 || kemampuan.super.current_countdown_time > 0){
                    return false;
                }
                tujuan.armor += kemampuan.Pertahanan_Super(kemampuan.super);
                return true;
            case "Regen":
                tujuan.energi += kemampuan.perbaikan(kemampuan.regen);
                if (tujuan.energi > 100){
                    tujuan.energi = 100;
                }
                return true;
        }
        return true;
    }
    public void Cek_informasi(){
        Console.WriteLine($"Nama : {nama}\nEnergi : {energi}\nArmor : {armor}");
    }
    public void Serang(Robot target){
        Console.WriteLine($"Menyerang Robot {target.nama} !!");
        if(target.armor > this.serangan){
            target.armor -= this.serangan;          
        }else if (target.armor == 0){
            target.energi -= this.serangan;
        }else{
            target.energi -= this.serangan - target.armor;
            target.armor = 0;
        }
    }
}


public class Boss_Robot : Robot{
    public Boss_Robot(){
        this.nama = "Boss";
        this.energi = 200;
        this.armor = 0 ;
        this.serangan = 32;
    }
    public void diserangan(Robot penyerang){

    }
}

public class Robot_Electric : Robot{
    public Robot_Electric(){
        this.nama = "Electro Bot";
        this.energi = 100;
        this.armor = 50;
        this.serangan = 30;
    }
}

public class Robot_Plasma : Robot{
    public Robot_Plasma(){
        this.nama = "Plasma Bot";
        this.energi = 100;
        this.armor = 50;
        this.serangan = 30;
    }
}

public abstract class Skill{
    public string name;
    public double damage_scale;
    public int couldown_time;
    public int current_countdown_time;

}

interface IKemampuan{
    int perbaikan(Regen regen);
    double Seranga_Listrik(Electric electric);
    double Serangan_Plasma (Plasma plasma);
    int Pertahanan_Super(Super super);
}

public class Kemampuan:IKemampuan{
    public Electric electric;
    public Plasma plasma;
    public Super super;
    public Regen regen;

    public Kemampuan(){
        this.plasma = new Plasma();
        this.super = new Super();
        this.regen = new Regen();
        this.electric = new Electric();

    }
    public int perbaikan(Regen regen) {
        return regen.energy;
    }

    public double Seranga_Listrik(Electric electric){
        System.Console.WriteLine("Melancarkan Serangan Electric Shock");
        electric.current_countdown_time = electric.couldown_time + 1;
        return electric.damage_scale;
    }

    public double Serangan_Plasma(Plasma plasma){
        System.Console.WriteLine("Menyerang Menggunakan Plasma Canon !");
        plasma.current_countdown_time = plasma.couldown_time + 1;
        return plasma.damage_scale;
    }
    
    public int Pertahanan_Super(Super super){
        System.Console.WriteLine("Mengaktifkan Pelindung Tambahan !");
        super.current_countdown_time = super.couldown_time + 1;
        super.remaining_time = super.durations;
        return super.amount;
    }
}
public class Electric : Skill{

    public Electric(){
        this.name = "Electric Shock";
        this.couldown_time = 1;
        this.damage_scale = 1.5;
    }
}

public class Plasma : Skill{
    public Plasma(){
        this.name = "Electric Shock";
        this.couldown_time = 2;
        this.damage_scale = 2;
    }
}

public class Super{
    public int durations, amount,current_countdown_time,remaining_time;
    public int couldown_time = 4;
    public Super(){
        this.durations = 2;
        this.amount = 20;
    }
}

public class Regen{
    public int energy = 40;
}