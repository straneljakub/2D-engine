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
                
                */
            Level.LoadLevel("level1.txt");

            Variables.PlayButton = new Button(Variables.ScreenWidth / 2 - 300 / 2, 100, 300, 70, "Play", 0);
            Variables.SaveButton = new Button(Variables.ScreenWidth / 2 - 300 / 2, 250, 300, 70, "Save", 0);
            Variables.QuitButton = new Button(Variables.ScreenWidth / 2 - 300 / 2, 400, 300, 70, "Quit", 0);

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
                    entity.SetForce(0, (float)(entity.GetMass() * 9.8) * -10);
                    entity.SetVelocity(40);
                    entity.SetHitbox(0, -1);
                    Audio.PlayEffect(Assets.AssetsArray[3]);
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

        public static void GameLoop()
        {

        }
        public static void MenuLoop()
        {

        }
    }
}
