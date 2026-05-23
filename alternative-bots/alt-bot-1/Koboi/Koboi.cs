using System;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class Koboi : Bot
{
    private double moveDirection = 1;
    private int? targetId = null;
    private double smallestGunTurn = double.MaxValue; // Heuristic: Sudut putar meriam terkecil

    static void Main(string[] args) { new Koboi().Start(); }
    Koboi() : base(BotInfo.FromFile("Koboi.json")) { }

    public override void Run()
    {
        BodyColor = Color.SaddleBrown;
        AdjustRadarForGunTurn = true; AdjustGunForBodyTurn = true; AdjustRadarForBodyTurn = true;

        while (IsRunning)
        {
            SetTurnRadarRight(360);
            Go();
        }
    }

    public override void OnScannedBot(ScannedBotEvent e)
    {
        double directionToEnemy = DirectionTo(e.X, e.Y);
        // Hitung berapa derajat meriam harus berputar untuk membidik target ini
        double requiredGunTurn = Math.Abs(NormalizeRelativeAngle(directionToEnemy - GunDirection));

        // --- GREEDY AIMING --- 
        // Incar musuh yang paling searah dengan moncong meriam
        if (targetId == null || e.ScannedBotId == targetId || requiredGunTurn < smallestGunTurn)
        {
            targetId = e.ScannedBotId;
            smallestGunTurn = requiredGunTurn;

            double distance = DistanceTo(e.X, e.Y);
            double firePower = (distance < 250) ? 3 : 1.5;

            // --- PREDICTIVE TARGETING ---
            double bulletSpeed = 20 - (3 * firePower);
            double timeToReach = distance / bulletSpeed;
            double enemyRadians = e.Direction * Math.PI / 180.0;
            double futureX = e.X + Math.Sin(enemyRadians) * e.Speed * timeToReach;
            double futureY = e.Y + Math.Cos(enemyRadians) * e.Speed * timeToReach;

            double gunTurn = NormalizeRelativeAngle(DirectionTo(futureX, futureY) - GunDirection);
            SetTurnGunRight(gunTurn);

            double radarTurn = NormalizeRelativeAngle(directionToEnemy - RadarDirection);
            radarTurn += (radarTurn >= 0 ? 5 : -5);
            SetTurnRadarRight(radarTurn);

            // Karena target udah searah meriam, tembakan bakal dieksekusi sangat instan
            if (GunHeat == 0 && Math.Abs(gunTurn) < 5) SetFire(firePower);

            // Movement: Random strafing (Bebas)
            double turnAngle = directionToEnemy - Direction + (85 * moveDirection);
            SetTurnRight(NormalizeRelativeAngle(turnAngle));
            SetForward(120 * moveDirection);
        }
    }

    public override void OnBotDeath(BotDeathEvent e)
    {
        if (e.VictimId == targetId)
        {
            targetId = null;
            smallestGunTurn = double.MaxValue;
        }
    }
    public override void OnHitByBullet(HitByBulletEvent e) { moveDirection *= -1; }
    public override void OnHitWall(HitWallEvent e) { moveDirection *= -1; }
}