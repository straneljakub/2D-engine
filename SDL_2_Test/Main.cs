using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL_2_Test.engine;
using SDL2;

namespace SDL_2_Test
{
    public static class MainProgram
    {
        public static double Counting = 0;
        public static int JumpSoundIndex = 3;
        public static void Init()
        {
            /*
            int index = Assets.AddAsset("./assets/grass.png");
            // Obstacles init
            var obs = new Obstacle(index, 0, Variables.LevelHeight - 50, 50, Variables.LevelWidth);
            Variables.Entities.Add(obs);

            obs = new Obstacle(index, 130, Variables.LevelHeight - 200, 50, 400);
            Variables.Entities.Add(obs);

            obs = new Obstacle(index, 300, Variables.LevelHeight - 400, 50, 400);
            Variables.Entities.Add(obs);

            index = Assets.AddAsset("./assets/ker.png");
            var tree = new Texture(index, 120, Variables.LevelHeight - 400, 200, 200);
            Variables.Entities.Add(tree);

            tree = new Texture(index, 600, Variables.LevelHeight - 250, 200, 200);
            Variables.Entities.Add(tree);

            index = Assets.AddAsset("./assets/wall.jpg");
            var wall1 = new Obstacle(index, 0, 0, Variables.LevelHeight - 50, 30);
            Variables.Entities.Add(wall1);


            var wall2 = new Obstacle(index, Variables.LevelWidth - 30, 0, Variables.LevelHeight - 50, 30);
            Variables.Entities.Add(wall2);

            index = Assets.AddAsset("./assets/jump.wav");
            index = Assets.AddAsset("./assets/music.wav");
            Audio.PlayMusic(Assets.AssetsArray[index]);


            // Fruit init
            index = Assets.AddAsset("./assets/apple.png");
            for (int i = 0; i < 40; i++)
            {
                var fruit = new Fruit(index);
                Variables.Entities.Add(fruit);
            }

            var player = new Player(Assets.AddAsset("./assets/viking.png"));
            Variables.Entities.Add(player);
            index = Assets.AddAsset("./assets/frog.png");
            var frog = new Frog(index);
            Variables.Entities.Add(frog);
            */

            Variables.CurrentLevel = "level4";
            Level.LoadLevel(Variables.CurrentLevel);

            Variables.PlayButton = new Button(Variables.ScreenWidth / 2 - 300 / 2, 100, 300, 70, "Play", 0, new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 });
            Variables.SaveButton = new Button(Variables.ScreenWidth / 2 - 300 / 2, 250, 300, 70, "Save", 0, new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 });
            Variables.QuitButton = new Button(Variables.ScreenWidth / 2 - 300 / 2, 400, 300, 70, "Quit", 0, new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 });


        }
        public static void Input(byte[] keyArray, Entity entity, double elapsed, bool isGrounded, float x, float y)
        {
            if (entity.GetType() == typeof(Player))
            {
                if (keyArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT] == 1 && keyArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT] == 1)
                    ;
                else if (keyArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT] == 1 && isGrounded)
                {
                    x -= (float)(Variables.PlayerSpeed * elapsed);
                    entity.SetFacing(0);
                }
                    
                else if (keyArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT] == 1 && isGrounded)
                {
                    x += (float)(Variables.PlayerSpeed * elapsed);
                    entity.SetFacing(1);
                }
                    
                else if (keyArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT] == 1)
                {
                    x -= (float)(Variables.PlayerSpeedAir * elapsed);
                    entity.SetFacing(0);
                }
                    
                else if (keyArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT] == 1)
                {
                    x += (float)(Variables.PlayerSpeedAir * elapsed);
                    entity.SetFacing(1);
                }

                //jump
                if (keyArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_UP] == 1 && isGrounded)
                {
                    entity.SetForce(0, (float)(entity.GetMass() * Variables.GravitationalPull) * -10);
                    entity.SetVelocity(40);
                    entity.SetHitbox(0, -1);
                }
                if (keyArray[(int)SDL.SDL_Scancode.SDL_SCANCODE_ESCAPE] == 1)
                {
                    Variables.Running = false;
                }
            }
            entity.SetHitbox(x, y);
        }
        //Has to return true when resolved, false otherwise
        public static bool CollisionResolve(Entity entity, Entity collider)
        {
            if (collider.GetLeft() <= entity.GetLeft() && collider.GetRight() >= entity.GetRight() &&
                collider.GetTop() <= entity.GetTop() && collider.GetBottom() >= entity.GetBottom())
            {
                if (entity.GetType() == typeof(Fruit))
                {
                    var hitbox = entity.GetHitbox();
                    entity.SetHitbox(-hitbox.x, -hitbox.y);
                    entity.SetHitbox(Variables.Random.Next(0, Variables.LevelWidth - 20), Variables.Random.Next(0, Variables.LevelHeight / 4));
                    return true;
                }
            }

            if (entity.GetType() == typeof(Fruit) && collider.GetType() == typeof(Fruit))
            {
                return true;
            }


            if (entity.GetType() == typeof(Player) && collider.GetType() == typeof(Fruit))
            {
                var hitbox = collider.GetHitbox();
                collider.SetHitbox(-hitbox.x, -hitbox.y);
                collider.SetHitbox(Variables.Random.Next(0, Variables.LevelWidth - 20), Variables.Random.Next(0, Variables.LevelHeight / 4));
                collider.SetVelocity(0);
                Variables.Score += 1;
                return true;
            }

            if (entity.GetType() == typeof(Fruit) && collider.GetType() == typeof(Player))
            {
                var hitbox = entity.GetHitbox();
                entity.SetHitbox(-hitbox.x, -hitbox.y);
                entity.SetHitbox(Variables.Random.Next(0, Variables.LevelWidth - 20), Variables.Random.Next(0, Variables.LevelHeight / 4));
                entity.SetVelocity(0);
                Variables.Score += 1;
                return true;
            }

            if (entity.GetType() == typeof(Frog) && collider.GetType() == typeof(Fruit))
            {
                return true;
            } else if (entity.GetType() == typeof(Fruit) && collider.GetType() == typeof(Frog))
            {
                return true;
            }

            if (entity.GetType() == typeof(Frog) && collider.GetType() == typeof(Player))
            {
                Variables.Running = false;
                Level.LoadLevel(Variables.CurrentLevel);
                Variables.Score = 0;
                Variables.GameOver = true;
            }
            else if (entity.GetType() == typeof(Player) && collider.GetType() == typeof(Frog))
            {
                Variables.Running = false;
                Variables.Score = 0;
                Level.LoadLevel(Variables.CurrentLevel);
                Variables.GameOver = true;
            }

            return false;
        }

        public static void Animations(double elapsed)
        {
            var player = EntityManager.GetPlayer();
            var arr = Variables.KeyArray;
            var grounded = player.Grounded();
            SpriteComponent sc = player.GetComponent<SpriteComponent>();

            if(!grounded)
            {
                sc.spriteRow = 18;
                sc.spriteCol = (int)(SDL.SDL_GetTicks() / sc.speed) % 6;
                player.SetSprite(sc);
            }
            else if (grounded && (arr[(int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT] == 1 || arr[(int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT] == 1))
            {
                sc.spriteRow = 2;
                sc.spriteCol = (int)(SDL.SDL_GetTicks() / sc.speed) % 8;
                player.SetSprite(sc);
            }
            else
            {
                sc.spriteRow = 0;
                sc.spriteCol = (int)(SDL.SDL_GetTicks() / sc.speed) % 8;
                player.SetSprite(sc);
            }
            
            var frog = EntityManager.GetAll<Frog>()[0];
            grounded = frog.Grounded();
            sc = frog.GetComponent<SpriteComponent>();

            if (grounded)
            {
                sc.speed = 600;
                sc.spriteRow = 0;
                sc.spriteCol = (int)(SDL.SDL_GetTicks() / sc.speed) % 2;
            } else
            {
                sc.speed = 200;
                sc.spriteRow = 2;
                sc.spriteCol = (int)(SDL.SDL_GetTicks() / sc.speed) % 2;
            }

        }

        public static void MenuMouseInput(SDL.SDL_Event e)
        {
            if (GUI.ButtonClicked(Variables.PlayButton, e))
            {
                Variables.Running = true;
            }
            else if (GUI.ButtonClicked(Variables.QuitButton, e))
            {
                Program.CleanUp();
                Variables.Quit = true;
            }
            else if (GUI.ButtonClicked(Variables.SaveButton, e))
            {
                Level.SaveLevel();
            }
        }

        public static void GameLoop(double elapsed)
        {
            Counting += elapsed;
            if(Counting > 2500000000)
            {
                Counting = 0;
                var frog = EntityManager.GetAll<Frog>()[0];
                if(frog.Grounded()) {
                    var playerLeft = EntityManager.GetPlayer().GetLeft();
                    var frogLeft = frog.GetLeft();
                    if (playerLeft < frogLeft)
                    {
                        frog.SetFacing(0);
                        frog.SetForce(-10, (float)(frog.GetMass() * Variables.GravitationalPull) * -10);
                    } else
                    {
                        frog.SetFacing(1);
                        frog.SetForce(10, (float)(frog.GetMass() * Variables.GravitationalPull) * -10);
                    }
                    frog.SetVelocity(40);
                    frog.SetHitbox(0, -1);
                    Audio.PlayEffect((Assets.AssetsArray[JumpSoundIndex]));
                }
            }
            

        }
        public static void MenuLoop()
        {
            
        }
    }

    public class Frog : Entity
    {
        public Frog(int spriteIndex)
        {
            NameComponent nameComponent = new NameComponent();
            nameComponent.Name = "Frog";
            AddComponent(nameComponent);

            SolidComponent solidComponent = new SolidComponent();
            solidComponent.Solid = true;
            AddComponent(solidComponent);

            HitboxComponent hitboxComponent = new HitboxComponent();
            hitboxComponent.Hitbox = new SDL.SDL_FRect
            {
                x = Variables.LevelWidth - 200,
                y = Variables.LevelHeight - 100,
                w = 27 * 4,
                h = 21 * 4
            };
            AddComponent(hitboxComponent);



            VelocityComponent velocityComponent = new VelocityComponent();
            velocityComponent.Velocity = 0;
            AddComponent(velocityComponent);

            MassComponent massComponent = new MassComponent();
            massComponent.Mass = 50;
            AddComponent(massComponent);

            ForceComponent forceComponent = new ForceComponent();
            forceComponent.Force = new Vector(0, Variables.GravitationalPull * 85);
            AddComponent(forceComponent);

            PhysicsObjectComponent physicsObjectComponent = new PhysicsObjectComponent();
            physicsObjectComponent.PhysicsObject = true;
            AddComponent(physicsObjectComponent);

            MoveableObjectComponent moveableObjectComponent = new MoveableObjectComponent();
            moveableObjectComponent.MoveableObject = true;
            AddComponent(moveableObjectComponent);

            SpriteComponent spriteComponent = new SpriteComponent();
            spriteComponent.assetsIndex = spriteIndex;
            spriteComponent.spriteX = 0;
            spriteComponent.spriteY = 0;
            spriteComponent.spriteWidth = 27;
            spriteComponent.spriteHeight = 21;
            spriteComponent.spriteCol = 0;
            spriteComponent.spriteRow = 0;
            spriteComponent.spriteXDiff = 27;
            spriteComponent.spriteYDiff = 21;
            spriteComponent.speed = 100;
            spriteComponent.facing = 1;
            spriteComponent.animated = true;
            AddComponent(spriteComponent);
        }
    }
}
