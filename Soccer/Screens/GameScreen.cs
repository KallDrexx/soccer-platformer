using System;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Math;
using FlatRedBall.TileCollisions;
using FlatRedBall.TileEntities;
using Microsoft.Xna.Framework;
using Soccer.Entities;

namespace Soccer.Screens
{
    public partial class GameScreen
    {
        private bool _isInKickSelectionMode;
        private bool _playerAttemptingToCatchBall;
        private Ball _caughtBall;

        void CustomInitialize()
        {
            Camera.Main.OrthogonalHeight /= BaseZoomFactor;
            Camera.Main.OrthogonalWidth /= BaseZoomFactor;
            Camera.Main.AttachTo(PlayerInstance);
            Camera.Main.RelativeZ = 100;
            
            BallListCollisionCircleVsGoalCollision.CollisionOccurred = BallGoalCollisionOccurred;
            PlayerInstanceBallCatchAreaVsBallListCollisionCircle.CollisionOccurred = BallInPlayerRegion;
            PlayerInstanceAxisAlignedRectangleInstanceVsUpSpringCollision.CollisionOccurred =
                (player, collection) => player.YVelocity = PlayerSpringAmount;

            BallListCollisionCircleVsUpSpringCollision.CollisionOccurred =
                (ball, collection) => ball.YVelocity = BallSpringAmount;
            
            GoalDisplay.AttachTo(Camera.Main);
            GoalDisplay.RelativeZ = -1;
            GoalDisplay.RelativeY = GoalDisplayYOffset;
            
            TileEntityInstantiator.CreateEntitiesFrom(Map);
        }

        void CustomActivity(bool firstTimeCalled)
        {
            _playerAttemptingToCatchBall = InputManager.Xbox360GamePads[0].ButtonDown(Xbox360GamePad.Button.RightTrigger);
            if (!_playerAttemptingToCatchBall && _isInKickSelectionMode)
            {
                DisableKickDirectionSelection();
            }

            if (_isInKickSelectionMode)
            {
                // Control ball arrow via analog stick
                _caughtBall.KickIndicatorInstance.RelativeRotationZ = (float) InputManager.Xbox360GamePads[0].LeftStick.Angle;
            }
        }

        void CustomDestroy()
        {
        }

        static void CustomLoadStaticContent(string contentManagerName)
        {
        }

        private void EnableKickDirectionSelection(Ball ball)
        {
            TimeManager.TimeFactor = 1 / 10f;
            PlayerInstance. InputEnabled = false;
            _isInKickSelectionMode = true;
            ball.KickIndicatorInstance.SpriteInstanceVisible = true;

            _caughtBall = ball;
        }

        private void DisableKickDirectionSelection()
        {
            TimeManager.TimeFactor = 1;
            PlayerInstance.InputEnabled = true;
            _isInKickSelectionMode = false;
            _caughtBall.KickIndicatorInstance.SpriteInstanceVisible = false;
            
            // If we are still colliding with the ball, then we are stopping kick selection
            // due to releasing the button, so we want to propel the ball in the specified direction
            if (PlayerInstance.BallCatchArea.CollideAgainst(_caughtBall.CollisionCircle))
            {
                var angle = InputManager.Xbox360GamePads[0].LeftStick.Angle;
                var ballVelocity = new Vector3(
                    (float)Math.Cos(angle) * PlayerInstance.KickVelocity,
                    (float)Math.Sin(angle) * PlayerInstance.KickVelocity,
                    0);
                
                var kickBackVelocity = new Vector3(
                    (float)Math.Cos(angle) * PlayerInstance.KickBackVelocity,
                    (float)Math.Sin(angle) * PlayerInstance.KickBackVelocity,
                    0); 
                
                _caughtBall.Velocity = ballVelocity;
                PlayerInstance.Velocity += -kickBackVelocity;
            }

            _caughtBall = null;
        }

        private void BallInPlayerRegion(Player player, Ball ball)
        {
            // If we already have a ball caught, don't change anything
            if (_caughtBall != null)
            {
                return;
            }
            
            if (_playerAttemptingToCatchBall && !_isInKickSelectionMode)
            {
                EnableKickDirectionSelection(ball);
            }
        }

        private void BallGoalCollisionOccurred(Ball ball, TileShapeCollection map)
        {
            ball.Destroy();
            GoalDisplay.Visible = true;
        }
    }
}
