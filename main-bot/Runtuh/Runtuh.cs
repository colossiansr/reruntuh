using System;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class Runtuh : Bot
{
    // state buat algoritma greedy (himpunan solusi sementara)
    private int? targetId = null;
    private double closestDistance = double.MaxValue;

    static void Main(string[] args) { new Runtuh().Start(); }
    Runtuh() : base(BotInfo.FromFile("Runtuh.json")) { }

    public override void Run()
    {
        BodyColor = Color.Black;
        GunColor = Color.Red;
        RadarColor = Color.Yellow;

        Random rand = new Random();

        // pergerakan random agar tidak gampang ditebak musuh
        while (IsRunning)
        {
            // random action 0-3 untuk maju mundur kiri kanan
            int action = rand.Next(4);
            
            // jarak dan sudutnya dirandom juga
            double randomDistance = rand.Next(60, 130);
            double randomAngle = rand.Next(45, 105);

            switch (action)
            {
                case 0: // maju
                    Forward(randomDistance);
                    TurnGunRight(180); // putar meriam buat nyari musuh
                    break;

                case 1: // mundur
                    Back(randomDistance);
                    TurnGunRight(180); 
                    break;

                case 2: // belok kiri trus maju
                    TurnLeft(randomAngle);
                    Forward(randomDistance);
                    break;

                case 3: // belok kanan trus maju
                    TurnRight(randomAngle);
                    Forward(randomDistance);
                    break;
            }
        }
    }

    public override void OnScannedBot(ScannedBotEvent evt)
    {
        // himpunan kandidat dapet dari semua bot musuh yg ke-scan radar
        double distance = DistanceTo(evt.X, evt.Y);

        // fungsi seleksi greedy: mengecek apakah ada musuh yang jaraknya lebih dekat
        // fungsi objektif: memilih jarak paling minimal agara peluru tidak gampang meleset
        if (targetId == null || evt.ScannedBotId == targetId || distance < closestDistance)
        {
            // update himpunan solusi
            targetId = evt.ScannedBotId;
            closestDistance = distance;

            double firePower = (distance < 200) ? 3.0 : 1.0;
            double gunTurn;

            // menentukan cara nembak berdasarkan jarak (adaptive)
            if (distance < 200)
            {
                // kalau dekat langsung arahin ke badannya
                gunTurn = NormalizeRelativeAngle(DirectionTo(evt.X, evt.Y) - GunDirection);
            }
            else
            {
                // kalau jauh pakai prediksi gerak musuh (linear)
                double bulletSpeed = 20 - (3 * firePower);
                double timeToReach = distance / bulletSpeed;
                double enemyRadians = evt.Direction * Math.PI / 180.0;
                
                double futureX = evt.X + Math.Sin(enemyRadians) * evt.Speed * timeToReach;
                double futureY = evt.Y + Math.Cos(enemyRadians) * evt.Speed * timeToReach;
                
                gunTurn = NormalizeRelativeAngle(DirectionTo(futureX, futureY) - GunDirection);
            }

            // putar meriam lalu tembak
            TurnGunRight(gunTurn);

            if (Math.Abs(gunTurn) < 10)
            {
                Fire(firePower);
            }
        }
    }

    public override void OnBotDeath(BotDeathEvent evt)
    {
        // fungsi kelayakan greedy: cek apakah target masih hidup
        // kalau sudah mati, reset memori agar mencari kandidat baru
        if (evt.VictimId == targetId)
        {
            targetId = null;
            closestDistance = double.MaxValue;
        }
    }

    public override void OnHitWall(HitWallEvent evt)
    {
        // kalau nabrak tembok, paksa mundur terus belok biar ga nyangkut
        Back(80);
        
        Random rand = new Random();
        if (rand.NextDouble() > 0.5)
        {
            TurnRight(90);
        }
        else
        {
            TurnLeft(90);
        }
    }
}