using System;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class Sniper : Bot
{
    private double moveDirection = 1;
    private int? targetId = null;
    private double slowestSpeed = double.MaxValue; // Heuristic: Kecepatan terkecil

    static void Main(string[] args) { new Sniper().Start(); }
    Sniper() : base(BotInfo.FromFile("Sniper.json")) { }

    public override void Run()
    {
        BodyColor = Color.DarkOliveGreen;
        AdjustRadarForGunTurn = true; AdjustGunForBodyTurn = true; AdjustRadarForBodyTurn = true;

        while (IsRunning)
        {
            SetTurnRadarRight(360);
            Go();
        }
    }

    public override void OnScannedBot(ScannedBotEvent e)
    {
        // --- GREEDY SPEED ----
        // Mengincar yang jalannya paling lelet atau diam
        if (targetId == null || e.ScannedBotId == targetId || Math.Abs(e.Speed) < slowestSpeed)
        {
            targetId = e.ScannedBotId;
            slowestSpeed = Math.Abs(e.Speed);

            double distance = DistanceTo(e.X, e.Y);
            
            // Sniper rakus firepower, selalu nembak kuat dari jauh
            double firePower = 3; 

            // --- PREDICTIVE TARGETING ---
            double bulletSpeed = 20 - (3 * firePower);
            double timeToReach = distance / bulletSpeed;
            double enemyRadians = e.Direction * Math.PI / 180.0;
            double futureX = e.X + Math.Sin(enemyRadians) * e.Speed * timeToReach;
            double futureY = e.Y + Math.Cos(enemyRadians) * e.Speed * timeToReach;

            double gunTurn = NormalizeRelativeAngle(DirectionTo(futureX, futureY) - GunDirection);
            SetTurnGunRight(gunTurn);

            double directionToCurrent = DirectionTo(e.X, e.Y);
            double radarTurn = NormalizeRelativeAngle(directionToCurrent - RadarDirection);
            radarTurn += (radarTurn >= 0 ? 5 : -5);
            SetTurnRadarRight(radarTurn);

            if (GunHeat == 0 && Math.Abs(gunTurn) < 2) SetFire(firePower); // Akurasi harus super ketat (< 2 derajat)

            // Movement: Jaga jarak aman (menjauh/tegak lurus)
            double turnAngle = directionToCurrent - Direction + 90; 
            SetTurnRight(NormalizeRelativeAngle(turnAngle));
            SetForward(100 * moveDirection);
        }
    }

    public override void OnBotDeath(BotDeathEvent e)
    {
        if (e.VictimId == targetId)
        {
            targetId = null;
            slowestSpeed = double.MaxValue;
        }
    }
    public override void OnHitByBullet(HitByBulletEvent e) { moveDirection *= -1; }
    public override void OnHitWall(HitWallEvent e) { moveDirection *= -1; }
}