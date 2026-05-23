using System;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class Killa : Bot
{
    private double moveDirection = 1;
    private int? targetId = null;
    private double lowestEnergy = double.MaxValue; // Heuristic: Energi terkecil

    static void Main(string[] args) { new Killa().Start(); }
    Killa() : base(BotInfo.FromFile("Killa.json")) { }

    public override void Run()
    {
        BodyColor = Color.DarkRed;
        AdjustRadarForGunTurn = true; AdjustGunForBodyTurn = true; AdjustRadarForBodyTurn = true;

        while (IsRunning)
        {
            SetTurnRadarRight(360);
            Go();
        }
    }

    public override void OnScannedBot(ScannedBotEvent e)
    {
        // --- GREEDY ENERGY / HEALTH ---
        // Incar musuh yang darah atau energinya paling sedikit
        if (targetId == null || e.ScannedBotId == targetId || e.Energy < lowestEnergy)
        {
            targetId = e.ScannedBotId;
            lowestEnergy = e.Energy; // Update energi terendah saat ini

            double distance = DistanceTo(e.X, e.Y);
            double firePower = (distance < 200) ? 3 : 2;

            // --- PREDICTIVE TARGETING ---
            double bulletSpeed = 20 - (3 * firePower);
            double timeToReach = distance / bulletSpeed;
            double enemyRadians = e.Direction * Math.PI / 180.0;
            double futureX = e.X + Math.Sin(enemyRadians) * e.Speed * timeToReach;
            double futureY = e.Y + Math.Cos(enemyRadians) * e.Speed * timeToReach;

            double gunTurn = NormalizeRelativeAngle(DirectionTo(futureX, futureY) - GunDirection);
            SetTurnGunRight(gunTurn);

            double radarTurn = NormalizeRelativeAngle(DirectionTo(e.X, e.Y) - RadarDirection);
            radarTurn += (radarTurn >= 0 ? 5 : -5);
            SetTurnRadarRight(radarTurn);

            if (GunHeat == 0 && Math.Abs(gunTurn) < 5) SetFire(firePower);

            // Movement Agresif mendekat ke mangsa (sudut spiral tajam)
            double turnAngle = DirectionTo(e.X, e.Y) - Direction + (60 * moveDirection);
            SetTurnRight(NormalizeRelativeAngle(turnAngle));
            SetForward(200 * moveDirection);
        }
    }

    public override void OnBotDeath(BotDeathEvent e)
    {
        if (e.VictimId == targetId)
        {
            targetId = null;
            lowestEnergy = double.MaxValue; // Reset pencarian
        }
    }
    public override void OnHitByBullet(HitByBulletEvent e) { moveDirection *= -1; }
    public override void OnHitWall(HitWallEvent e) { moveDirection *= -1; }
}