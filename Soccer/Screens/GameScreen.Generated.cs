#if ANDROID || IOS || DESKTOP_GL
#define REQUIRES_PRIMARY_THREAD_LOADING
#endif
using Color = Microsoft.Xna.Framework.Color;
using System.Linq;
using FlatRedBall;
using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall.Math;
namespace Soccer.Screens
{
    public partial class GameScreen : FlatRedBall.Screens.Screen
    {
        #if DEBUG
        static bool HasBeenLoadedWithGlobalContentManager = false;
        #endif
        protected static Microsoft.Xna.Framework.Graphics.Texture2D Tiles;
        protected static Microsoft.Xna.Framework.Graphics.Texture2D excellent;
        
        private FlatRedBall.TileCollisions.TileShapeCollection SolidCollision;
        private Soccer.Entities.Player PlayerInstance;
        private FlatRedBall.Math.Collision.DelegateCollisionRelationship<Soccer.Entities.Player, FlatRedBall.TileCollisions.TileShapeCollection> PlayerInstanceAxisAlignedRectangleInstanceVsSolidCollision;
        protected FlatRedBall.TileGraphics.LayeredTileMap Map;
        private FlatRedBall.Math.PositionedObjectList<Soccer.Entities.Ball> BallList;
        private FlatRedBall.Math.Collision.ListVsPositionedObjectRelationship<Entities.Ball, Soccer.Entities.Player> BallListVsPlayerInstanceAxisAlignedRectangleInstance;
        private FlatRedBall.Math.Collision.CollidableListVsTileShapeCollectionRelationship<Entities.Ball> BallListVsSolidCollision;
        private FlatRedBall.TileCollisions.TileShapeCollection GoalCollision;
        private FlatRedBall.Math.Collision.DelegateCollisionRelationship<Soccer.Entities.Player, FlatRedBall.TileCollisions.TileShapeCollection> PlayerInstanceAxisAlignedRectangleInstanceVsGoalCollision;
        private FlatRedBall.Math.Collision.CollidableListVsTileShapeCollectionRelationship<Entities.Ball> BallListCollisionCircleVsGoalCollision;
        private FlatRedBall.Sprite GoalDisplay;
        private FlatRedBall.Math.Collision.PositionedObjectVsListRelationship<Soccer.Entities.Player, Entities.Ball> PlayerInstanceBallCatchAreaVsBallListCollisionCircle;
        private FlatRedBall.TileCollisions.TileShapeCollection UpSpringCollision;
        private FlatRedBall.Math.Collision.CollidableVsTileShapeCollectionRelationship<Soccer.Entities.Player> PlayerInstanceAxisAlignedRectangleInstanceVsUpSpringCollision;
        private FlatRedBall.Math.Collision.CollidableListVsTileShapeCollectionRelationship<Entities.Ball> BallListCollisionCircleVsUpSpringCollision;
        public int GoalDisplayYOffset = 75;
        public float BaseZoomFactor = 2.5f;
        public int PlayerSpringAmount = 400;
        public int BallSpringAmount = 250;
        public GameScreen () 
        	: base ("GameScreen")
        {
        }
        public override void Initialize (bool addToManagers) 
        {
            LoadStaticContent(ContentManagerName);
            SolidCollision = new FlatRedBall.TileCollisions.TileShapeCollection();
            PlayerInstance = new Soccer.Entities.Player(ContentManagerName, false);
            PlayerInstance.Name = "PlayerInstance";
                {
        var temp = new FlatRedBall.Math.Collision.DelegateCollisionRelationship<Soccer.Entities.Player, FlatRedBall.TileCollisions.TileShapeCollection>(PlayerInstance, SolidCollision);
        var isCloud = false;
        temp.CollisionFunction = (first, second) =>
        {
            return first.CollideAgainst(second, first.AxisAlignedRectangleInstance, isCloud);
        }
        ;
        FlatRedBall.Math.Collision.CollisionManager.Self.Relationships.Add(temp);
        PlayerInstanceAxisAlignedRectangleInstanceVsSolidCollision = temp;
    }
    PlayerInstanceAxisAlignedRectangleInstanceVsSolidCollision.Name = "PlayerInstanceAxisAlignedRectangleInstanceVsSolidCollision";

            // Not instantiating for LayeredTileMap Map in Screens\GameScreen (Screen) because properties on the object prevent it
            BallList = new FlatRedBall.Math.PositionedObjectList<Soccer.Entities.Ball>();
            BallList.Name = "BallList";
                BallListVsPlayerInstanceAxisAlignedRectangleInstance = FlatRedBall.Math.Collision.CollisionManager.Self.CreateRelationship(BallList, PlayerInstance);
    BallListVsPlayerInstanceAxisAlignedRectangleInstance.SetSecondSubCollision(item => item.AxisAlignedRectangleInstance);
    BallListVsPlayerInstanceAxisAlignedRectangleInstance.Name = "BallListVsPlayerInstanceAxisAlignedRectangleInstance";
    BallListVsPlayerInstanceAxisAlignedRectangleInstance.SetBounceCollision(0f, 1f, 0.5f);

                BallListVsSolidCollision = FlatRedBall.Math.Collision.CollisionManagerTileShapeCollectionExtensions.CreateTileRelationship(FlatRedBall.Math.Collision.CollisionManager.Self, BallList, SolidCollision);
    BallListVsSolidCollision.Name = "BallListVsSolidCollision";
    BallListVsSolidCollision.SetBounceCollision(0f, 1f, 0.8f);

            GoalCollision = new FlatRedBall.TileCollisions.TileShapeCollection();
                {
        var temp = new FlatRedBall.Math.Collision.DelegateCollisionRelationship<Soccer.Entities.Player, FlatRedBall.TileCollisions.TileShapeCollection>(PlayerInstance, GoalCollision);
        var isCloud = false;
        temp.CollisionFunction = (first, second) =>
        {
            return first.CollideAgainst(second, first.AxisAlignedRectangleInstance, isCloud);
        }
        ;
        FlatRedBall.Math.Collision.CollisionManager.Self.Relationships.Add(temp);
        PlayerInstanceAxisAlignedRectangleInstanceVsGoalCollision = temp;
    }
    PlayerInstanceAxisAlignedRectangleInstanceVsGoalCollision.Name = "PlayerInstanceAxisAlignedRectangleInstanceVsGoalCollision";

                BallListCollisionCircleVsGoalCollision = FlatRedBall.Math.Collision.CollisionManagerTileShapeCollectionExtensions.CreateTileRelationship(FlatRedBall.Math.Collision.CollisionManager.Self, BallList, GoalCollision);
    BallListCollisionCircleVsGoalCollision.SetFirstSubCollision(item => item.CollisionCircle);
    BallListCollisionCircleVsGoalCollision.Name = "BallListCollisionCircleVsGoalCollision";

            GoalDisplay = new FlatRedBall.Sprite();
            GoalDisplay.Name = "GoalDisplay";
                PlayerInstanceBallCatchAreaVsBallListCollisionCircle = FlatRedBall.Math.Collision.CollisionManager.Self.CreateRelationship(PlayerInstance, BallList);
    PlayerInstanceBallCatchAreaVsBallListCollisionCircle.SetFirstSubCollision(item => item.BallCatchArea);
    PlayerInstanceBallCatchAreaVsBallListCollisionCircle.SetSecondSubCollision(item => item.CollisionCircle);
    PlayerInstanceBallCatchAreaVsBallListCollisionCircle.Name = "PlayerInstanceBallCatchAreaVsBallListCollisionCircle";

            UpSpringCollision = new FlatRedBall.TileCollisions.TileShapeCollection();
                PlayerInstanceAxisAlignedRectangleInstanceVsUpSpringCollision = FlatRedBall.Math.Collision.CollisionManagerTileShapeCollectionExtensions.CreateTileRelationship(FlatRedBall.Math.Collision.CollisionManager.Self, PlayerInstance, UpSpringCollision);
    PlayerInstanceAxisAlignedRectangleInstanceVsUpSpringCollision.SetFirstSubCollision(item => item.AxisAlignedRectangleInstance);
    PlayerInstanceAxisAlignedRectangleInstanceVsUpSpringCollision.Name = "PlayerInstanceAxisAlignedRectangleInstanceVsUpSpringCollision";

                BallListCollisionCircleVsUpSpringCollision = FlatRedBall.Math.Collision.CollisionManagerTileShapeCollectionExtensions.CreateTileRelationship(FlatRedBall.Math.Collision.CollisionManager.Self, BallList, UpSpringCollision);
    BallListCollisionCircleVsUpSpringCollision.SetFirstSubCollision(item => item.CollisionCircle);
    BallListCollisionCircleVsUpSpringCollision.Name = "BallListCollisionCircleVsUpSpringCollision";

            // normally we wait to set variables until after the object is created, but in this case if the
            // TileShapeCollection doesn't have its Visible set before creating the tiles, it can result in
            // really bad performance issues, as shapes will be made visible, then invisible. Really bad perf!
            if (SolidCollision != null)
            {
                SolidCollision.Visible = false;
            }
            FlatRedBall.TileCollisions.TileShapeCollectionLayeredTileMapExtensions.AddCollisionFromTilesWithType(SolidCollision, Map, "Solid");
            // normally we wait to set variables until after the object is created, but in this case if the
            // TileShapeCollection doesn't have its Visible set before creating the tiles, it can result in
            // really bad performance issues, as shapes will be made visible, then invisible. Really bad perf!
            if (GoalCollision != null)
            {
                GoalCollision.Visible = false;
            }
            FlatRedBall.TileCollisions.TileShapeCollectionLayeredTileMapExtensions.AddCollisionFromTilesWithType(GoalCollision, Map, "Goal");
            // normally we wait to set variables until after the object is created, but in this case if the
            // TileShapeCollection doesn't have its Visible set before creating the tiles, it can result in
            // really bad performance issues, as shapes will be made visible, then invisible. Really bad perf!
            if (UpSpringCollision != null)
            {
                UpSpringCollision.Visible = false;
            }
            FlatRedBall.TileCollisions.TileShapeCollectionLayeredTileMapExtensions.AddCollisionFromTilesWithType(UpSpringCollision, Map, "Spring");
            
            
            PostInitialize();
            base.Initialize(addToManagers);
            if (addToManagers)
            {
                AddToManagers();
            }
        }
        public override void AddToManagers () 
        {
            Factories.BallFactory.Initialize(ContentManagerName);
            Factories.BallFactory.AddList(BallList);
            PlayerInstance.AddToManagers(mLayer);
            FlatRedBall.SpriteManager.AddSprite(GoalDisplay);
            base.AddToManagers();
            AddToManagersBottomUp();
            CustomInitialize();
        }
        public override void Activity (bool firstTimeCalled) 
        {
            if (!IsPaused)
            {
                
                PlayerInstance.Activity();
                for (int i = BallList.Count - 1; i > -1; i--)
                {
                    if (i < BallList.Count)
                    {
                        // We do the extra if-check because activity could destroy any number of entities
                        BallList[i].Activity();
                    }
                }
            }
            else
            {
            }
            base.Activity(firstTimeCalled);
            if (!IsActivityFinished)
            {
                CustomActivity(firstTimeCalled);
            }
        }
        public override void Destroy () 
        {
            base.Destroy();
            Factories.BallFactory.Destroy();
            Tiles = null;
            excellent = null;
            
            BallList.MakeOneWay();
            if (SolidCollision != null)
            {
                SolidCollision.Visible = false;
            }
            if (PlayerInstance != null)
            {
                PlayerInstance.Destroy();
                PlayerInstance.Detach();
            }
            for (int i = BallList.Count - 1; i > -1; i--)
            {
                BallList[i].Destroy();
            }
            if (GoalCollision != null)
            {
                GoalCollision.Visible = false;
            }
            if (GoalDisplay != null)
            {
                FlatRedBall.SpriteManager.RemoveSprite(GoalDisplay);
            }
            if (UpSpringCollision != null)
            {
                UpSpringCollision.Visible = false;
            }
            BallList.MakeTwoWay();
            FlatRedBall.Math.Collision.CollisionManager.Self.Relationships.Clear();
            CustomDestroy();
        }
        public virtual void PostInitialize () 
        {
            bool oldShapeManagerSuppressAdd = FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue;
            FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue = true;
            SolidCollision.Visible = false;
            if (PlayerInstance.Parent == null)
            {
                PlayerInstance.X = 300f;
            }
            else
            {
                PlayerInstance.RelativeX = 300f;
            }
            if (PlayerInstance.Parent == null)
            {
                PlayerInstance.Y = -100f;
            }
            else
            {
                PlayerInstance.RelativeY = -100f;
            }
            if (Map!= null)
            {
                if (Map.Parent == null)
                {
                    Map.Z = -50f;
                }
                else
                {
                    Map.RelativeZ = -50f;
                }
            }
            GoalCollision.Visible = false;
            GoalDisplay.Texture = excellent;
            GoalDisplay.TextureScale = 0.1f;
            GoalDisplay.Visible = false;
            FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue = oldShapeManagerSuppressAdd;
        }
        public virtual void AddToManagersBottomUp () 
        {
            CameraSetup.ResetCamera(SpriteManager.Camera);
            AssignCustomVariables(false);
        }
        public virtual void RemoveFromManagers () 
        {
            if (SolidCollision != null)
            {
                SolidCollision.Visible = false;
            }
            PlayerInstance.RemoveFromManagers();
            for (int i = BallList.Count - 1; i > -1; i--)
            {
                BallList[i].Destroy();
            }
            if (GoalCollision != null)
            {
                GoalCollision.Visible = false;
            }
            if (GoalDisplay != null)
            {
                FlatRedBall.SpriteManager.RemoveSpriteOneWay(GoalDisplay);
            }
            if (UpSpringCollision != null)
            {
                UpSpringCollision.Visible = false;
            }
        }
        public virtual void AssignCustomVariables (bool callOnContainedElements) 
        {
            if (callOnContainedElements)
            {
                PlayerInstance.AssignCustomVariables(true);
            }
            SolidCollision.Visible = false;
            if (PlayerInstance.Parent == null)
            {
                PlayerInstance.X = 300f;
            }
            else
            {
                PlayerInstance.RelativeX = 300f;
            }
            if (PlayerInstance.Parent == null)
            {
                PlayerInstance.Y = -100f;
            }
            else
            {
                PlayerInstance.RelativeY = -100f;
            }
            GoalCollision.Visible = false;
            GoalDisplay.Texture = excellent;
            GoalDisplay.TextureScale = 0.1f;
            GoalDisplay.Visible = false;
            GoalDisplayYOffset = 75;
            BaseZoomFactor = 2.5f;
            PlayerSpringAmount = 400;
            BallSpringAmount = 250;
        }
        public virtual void ConvertToManuallyUpdated () 
        {
            PlayerInstance.ConvertToManuallyUpdated();
            if (Map != null)
            {
            }
            for (int i = 0; i < BallList.Count; i++)
            {
                BallList[i].ConvertToManuallyUpdated();
            }
            FlatRedBall.SpriteManager.ConvertToManuallyUpdated(GoalDisplay);
        }
        public static void LoadStaticContent (string contentManagerName) 
        {
            if (string.IsNullOrEmpty(contentManagerName))
            {
                throw new System.ArgumentException("contentManagerName cannot be empty or null");
            }
            #if DEBUG
            if (contentManagerName == FlatRedBall.FlatRedBallServices.GlobalContentManager)
            {
                HasBeenLoadedWithGlobalContentManager = true;
            }
            else if (HasBeenLoadedWithGlobalContentManager)
            {
                throw new System.Exception("This type has been loaded with a Global content manager, then loaded with a non-global.  This can lead to a lot of bugs");
            }
            #endif
            Tiles = FlatRedBall.FlatRedBallServices.Load<Microsoft.Xna.Framework.Graphics.Texture2D>(@"content/screens/gamescreen/tiles.png", contentManagerName);
            excellent = FlatRedBall.FlatRedBallServices.Load<Microsoft.Xna.Framework.Graphics.Texture2D>(@"content/screens/gamescreen/excellent.png", contentManagerName);
            Soccer.Entities.Player.LoadStaticContent(contentManagerName);
            CustomLoadStaticContent(contentManagerName);
        }
        public override void PauseThisScreen () 
        {
            StateInterpolationPlugin.TweenerManager.Self.Pause();
            base.PauseThisScreen();
        }
        public override void UnpauseThisScreen () 
        {
            StateInterpolationPlugin.TweenerManager.Self.Unpause();
            base.UnpauseThisScreen();
        }
        [System.Obsolete("Use GetFile instead")]
        public static object GetStaticMember (string memberName) 
        {
            switch(memberName)
            {
                case  "Tiles":
                    return Tiles;
                case  "excellent":
                    return excellent;
            }
            return null;
        }
        public static object GetFile (string memberName) 
        {
            switch(memberName)
            {
                case  "Tiles":
                    return Tiles;
                case  "excellent":
                    return excellent;
            }
            return null;
        }
        object GetMember (string memberName) 
        {
            switch(memberName)
            {
                case  "Tiles":
                    return Tiles;
                case  "excellent":
                    return excellent;
            }
            return null;
        }
    }
}
