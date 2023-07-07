using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace SDL_2_Test.engine
{
    public static class EcsId
    {
        private static int FreeId { get; set; } = 0;
        public static Stack<int> FreeIds = new Stack<int>();

        public static int GetId()
        {
            if (FreeIds.Count > 0)
                return FreeIds.Pop();

            int id = FreeId;
            FreeId += 1;
            return id;
        }
    }
    #region Entity Manager
    public static class EntityManager
    {
        public static Entity GetPlayer()
        {
            foreach (Entity entity in Variables.Entities)
            {
                if (entity.GetType() == typeof(Player))
                {
                    return entity;
                }
            }
            return null;
        }

        public static List<Entity> GetAll<T>()
        {
            List<Entity> list = new List<Entity>();
            foreach (Entity entity in Variables.Entities)
            {
                if (entity.GetType() == typeof(T))
                {
                    list.Add(entity);
                }
            }
            return list;
        }
    }
    #endregion

    #region Components
    public class Component
    {
        public Entity Entity;
        public void Update()
        {
        }
    }
    public class NameComponent : Component
    {
        public string Name { get; set; } = "Entity";
    }

    public class SolidComponent : Component
    {
        public bool Solid { get; set; } = false;
    }

    public class HitboxComponent : Component
    {
        public SDL.SDL_FRect Hitbox;
    }

    public class ShapeComponent : Component
    {
        public (SDL.SDL_FRect, SDL.SDL_Color)[] Shape;
        public int ShapesCount { get; set; }
    }

    public class VelocityComponent : Component
    {
        public double Velocity;
    }

    public class MassComponent : Component
    {
        public double Mass;
    }

    public class ForceComponent : Component
    {
        public Vector Force;
    }

    public class PhysicsObjectComponent : Component
    {
        public bool PhysicsObject;
    }

    public class MoveableObjectComponent : Component
    {
        public bool MoveableObject;
    }

    public class SpriteComponent : Component
    {
        public int assetsIndex;
        public int spriteX;
        public int spriteY;
        public int spriteWidth;
        public int spriteHeight;
        public int spriteRow;
        public int spriteCol;
        public int spriteXDiff;
        public int spriteYDiff;
        public int speed;
        public short facing;
        public bool animated;
    }

    #endregion

    #region Entities

    public class Entity
    {
        public int Id { get; set; }
        private  List<Component> Components = new List<Component>();

        public Entity()
        {
            Id = EcsId.GetId();
        }

        public bool HasComponent<T>()
        {
            foreach (Component component in Components)
            {
                if (component.GetType().Equals(typeof(T)))
                {
                    return true;
                }
            }
            return false;
        }
        public bool Grounded()
        {
            var obstacles = EntityManager.GetAll<Obstacle>();
            foreach (var obstacle in obstacles)
            {
                if (obstacle.GetLeft() - GetHitbox().w < GetLeft()
                    && GetRight() < obstacle.GetRight() + GetHitbox().w
                    && GetBottom() + 0.1 > obstacle.GetTop()
                    && GetBottom() + 0.1 < obstacle.GetBottom())
                    return true;
            }
            return false;
        }

        public (float, float) GetMid()
        {
            var hitbox = GetHitbox();
            (float, float) mid;
            mid.Item1 = hitbox.x + hitbox.w / 2;
            mid.Item2 = hitbox.y + hitbox.h / 2;
            return mid;
        }

        public float HalfWidth()
        {
            var hitbox = GetHitbox();
            return hitbox.w / 2;
        }

        public float HalfHeight()
        {
            var hitbox = GetHitbox();
            return hitbox.h / 2;
        }

        public float GetTop()
        {
            var hitbox = GetHitbox();
            return hitbox.y;
        }

        public float GetBottom()
        {
            var hitbox = GetHitbox();
            return hitbox.y + hitbox.h;
        }

        public float GetLeft()
        {
            var hitbox = GetHitbox();
            return hitbox.x;
        }

        public float GetRight()
        {
            var hitbox = GetHitbox();
            return hitbox.x + hitbox.w;
        }
        public void AddComponent(Component component)
        {
            Components.Add(component);
            component.Entity = this;
        }

        public T GetComponent<T>() where T : Component
        {
            foreach (Component component in Components)
            {
                if (component.GetType().Equals(typeof(T)))
                    return (T)component;
            }
            return null;
        }

        public bool GetSolid()
        {
            return GetComponent<SolidComponent>().Solid;
        }

        public bool GetPhysicsObject()
        {
            return GetComponent<PhysicsObjectComponent>().PhysicsObject;
        }

        public bool GetMoveableObject()
        {
            return GetComponent<MoveableObjectComponent>().MoveableObject;
        }

        public string GetName()
        {
            return GetComponent<NameComponent>().Name;
        }

        public (SDL.SDL_FRect, SDL.SDL_Color)[] GetShape()
        {
            return GetComponent<ShapeComponent>().Shape;
        }

        public void SetShape((SDL.SDL_FRect, SDL.SDL_Color)[] shape)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].GetType().Equals(typeof(ShapeComponent)))
                {
                    ShapeComponent component = (ShapeComponent)Components[i];
                    component.Shape = shape;
                    component.ShapesCount = shape.Count();
                    Components[i] = component;
                }
            }
        }
        public int GetShapesCount()
        {
            return GetComponent<ShapeComponent>().ShapesCount;
        }

        public void IncShapes(double x, double y)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].GetType().Equals(typeof(ShapeComponent)))
                {
                    ShapeComponent component = (ShapeComponent)Components[i];
                    for (int j = 0; j < component.ShapesCount; j++)
                    {
                        component.Shape[j].Item1.x += (float)x;
                        component.Shape[j].Item1.y += (float)y;
                    }
                    Components[i] = component;
                }
            }
        }

        public SDL.SDL_FRect GetHitbox()
        {
            return GetComponent<HitboxComponent>().Hitbox;
        }

        public void SetHitbox(float x, float y)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].GetType().Equals(typeof(HitboxComponent)))
                {
                    HitboxComponent component = (HitboxComponent)Components[i];
                    component.Hitbox.x += x;
                    component.Hitbox.y += y;
                    Components[i] = component;
                    IncShapes(x, y);
                }
            }
        }

        public void SetHitbox(SDL.SDL_FRect hitbox)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].GetType().Equals(typeof(HitboxComponent)))
                {
                    HitboxComponent component = (HitboxComponent)Components[i];
                    component.Hitbox = hitbox;
                    Components[i] = component;
                }
            }
        }

        public double GetMass()
        {
            return GetComponent<MassComponent>().Mass;
        }

        public void SetMass(double mass)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].GetType().Equals(typeof(MassComponent)))
                {
                    MassComponent component = (MassComponent)Components[i];
                    component.Mass = mass;
                    Components[i] = component;
                }
            }
        }

        public Vector GetForce()
        {
            return GetComponent<ForceComponent>().Force;
        }

        public void SetForce(Vector force)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].GetType().Equals(typeof(ForceComponent)))
                {
                    ForceComponent component = (ForceComponent)Components[i];
                    component.Force = force;
                    Components[i] = component;
                }
            }
        }

        public void SetForce(float x, float y)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].GetType().Equals(typeof(ForceComponent)))
                {
                    ForceComponent component = (ForceComponent)Components[i];
                    component.Force.x += x;
                    component.Force.y += y;
                    Components[i] = component;
                }
            }
        }
        public double GetVelocity()
        {
            return GetComponent<VelocityComponent>().Velocity;
        }

        public void SetVelocity(double velocity)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].GetType().Equals(typeof(VelocityComponent)))
                {
                    VelocityComponent component = (VelocityComponent)Components[i];
                    component.Velocity = velocity;
                    Components[i] = component;
                }
            }
        }

        public void SetSprite(SpriteComponent spriteComponent)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].GetType().Equals(typeof(SpriteComponent)))
                {
                    Components[i] = spriteComponent;
                }
            }
        }

        public short GetFacing()
        {
            var c = GetComponent<SpriteComponent>();
            return c.facing;
        }

        public void SetFacing(short facing)
        {
            var c = GetComponent<SpriteComponent>();
            c.facing = facing;
            SetSprite(c);
        }


    }


    public class Player : Entity
    {
        public Player(int spriteIndex)
        {
            NameComponent nameComponent = new NameComponent();
            nameComponent.Name = "Player";
            AddComponent(nameComponent);

            SolidComponent solidComponent = new SolidComponent();
            solidComponent.Solid = true;
            AddComponent(solidComponent);

            HitboxComponent hitboxComponent = new HitboxComponent();
            hitboxComponent.Hitbox = new SDL.SDL_FRect
            {
                x = 300,
                y = 200,
                w = 66,
                h = 92
            };
            AddComponent(hitboxComponent);



            VelocityComponent velocityComponent = new VelocityComponent();
            velocityComponent.Velocity = 0;
            AddComponent(velocityComponent);

            MassComponent massComponent = new MassComponent();
            massComponent.Mass = 85;
            AddComponent(massComponent);

            ForceComponent forceComponent = new ForceComponent();
            forceComponent.Force = new Vector(0, 9.8 * 85);
            AddComponent(forceComponent);

            PhysicsObjectComponent physicsObjectComponent = new PhysicsObjectComponent();
            physicsObjectComponent.PhysicsObject = true;
            AddComponent(physicsObjectComponent);

            MoveableObjectComponent moveableObjectComponent = new MoveableObjectComponent();
            moveableObjectComponent.MoveableObject = true;
            AddComponent(moveableObjectComponent);
            
            SpriteComponent spriteComponent = new SpriteComponent();
            spriteComponent.assetsIndex = spriteIndex;
            spriteComponent.spriteX = 45;
            spriteComponent.spriteY = 25;
            spriteComponent.spriteWidth = 33;
            spriteComponent.spriteHeight = 46;
            spriteComponent.spriteCol = 0;
            spriteComponent.spriteRow = 0;
            spriteComponent.spriteXDiff = 115;
            spriteComponent.spriteYDiff = 84;
            spriteComponent.speed = 100;
            spriteComponent.facing = 1;
            spriteComponent.animated = true;
            AddComponent(spriteComponent);
        }

        public (SDL.SDL_FRect, SDL.SDL_Color)[] GetTexture()
        {
            return Textures.PlayerShape();
        }
    }
    public class Obstacle : Entity
    {
        public Obstacle(int spriteIndex, float x, float y, float h, float w)
        {
            NameComponent nameComponent = new NameComponent();
            nameComponent.Name = "Obstacle";
            AddComponent(nameComponent);

            SolidComponent solidComponent = new SolidComponent();
            solidComponent.Solid = true;
            AddComponent(solidComponent);

            HitboxComponent hitboxComponent = new HitboxComponent();
            hitboxComponent.Hitbox = new SDL.SDL_FRect
            {
                x = x,
                y = y,
                h = h,
                w = w,
            };
            AddComponent(hitboxComponent);

            PhysicsObjectComponent physicsObjectComponent = new PhysicsObjectComponent();
            physicsObjectComponent.PhysicsObject = true;
            AddComponent(physicsObjectComponent);

            MoveableObjectComponent moveableObjectComponent = new MoveableObjectComponent();
            moveableObjectComponent.MoveableObject = false;
            AddComponent(moveableObjectComponent);

            SpriteComponent sc = new SpriteComponent();
            sc.animated = false;
            sc.assetsIndex = spriteIndex;
            AddComponent(sc);
        }

        public (SDL.SDL_FRect, SDL.SDL_Color)[] GetTexture()
        {
            SDL.SDL_FRect hitbox = GetComponent<HitboxComponent>().Hitbox;
            return Textures.ObstacleShape(hitbox.x, hitbox.y);
        }
    }
    public class Fruit : Entity
    {
        public Fruit(int spriteIndex)
        {

            NameComponent nameC = new NameComponent();
            nameC.Name = "Fruit";
            AddComponent(nameC);

            SolidComponent solidC = new SolidComponent();
            solidC.Solid = true;
            AddComponent(solidC);

            HitboxComponent hitboxComponent = new HitboxComponent();
            hitboxComponent.Hitbox = new SDL.SDL_FRect
            {
                x = Variables.Random.Next(0, Variables.LevelWidth - 20),
                y = Variables.Random.Next(0, Variables.LevelHeight - 20),
                h = 20,
                w = 20,
            };
            AddComponent(hitboxComponent);

            ForceComponent forceComponent = new ForceComponent();
            forceComponent.Force = new Vector(0, 9.8 * 0.3);
            AddComponent(forceComponent);

            VelocityComponent velocityComponent = new VelocityComponent();
            velocityComponent.Velocity = 0;
            AddComponent(velocityComponent);

            MassComponent massComponent = new MassComponent();
            massComponent.Mass = 0.3;
            AddComponent(massComponent);

            PhysicsObjectComponent physicsObjectComponent = new PhysicsObjectComponent();
            physicsObjectComponent.PhysicsObject = true;
            AddComponent(physicsObjectComponent);

            MoveableObjectComponent moveableObjectComponent = new MoveableObjectComponent();
            moveableObjectComponent.MoveableObject = true;
            AddComponent(moveableObjectComponent);

            SpriteComponent sc = new SpriteComponent();
            sc.animated = false;
            sc.assetsIndex = spriteIndex;
            AddComponent(sc);
        }

        public (SDL.SDL_FRect, SDL.SDL_Color)[] GetTexture()
        {
            SDL.SDL_FRect hitbox = GetComponent<HitboxComponent>().Hitbox;
            return Textures.FruitShape(hitbox.x, hitbox.y);
        }
    }

    public class Texture : Entity
    {
        public Texture(int spriteIndex, int x, int y, int w, int h)
        {
            MoveableObjectComponent moveableObjectComponent = new MoveableObjectComponent();
            moveableObjectComponent.MoveableObject = false;
            AddComponent(moveableObjectComponent);

            SolidComponent solidC = new SolidComponent();
            solidC.Solid = false;
            AddComponent(solidC);

            HitboxComponent hitboxComponent = new HitboxComponent();
            hitboxComponent.Hitbox = new SDL.SDL_FRect
            {
                x = x,
                y = y,
                h = h,
                w = w,
            };
            AddComponent(hitboxComponent);

            SpriteComponent sc = new SpriteComponent();
            sc.animated = false;
            sc.assetsIndex = spriteIndex;
            AddComponent(sc);
        }
    }

    public class Frog : Entity
    {
        public Frog(int spriteIndex, int x, int y, int w, int h)
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
                x = 300,
                y = 200,
                w = 81,
                h = 63
            };
            AddComponent(hitboxComponent);

            VelocityComponent velocityComponent = new VelocityComponent();
            velocityComponent.Velocity = 0;
            AddComponent(velocityComponent);

            MassComponent massComponent = new MassComponent();
            massComponent.Mass = 85;
            AddComponent(massComponent);

            ForceComponent forceComponent = new ForceComponent();
            forceComponent.Force = new Vector(0, 9.8 * 85);
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
            spriteComponent.speed = 350;
            spriteComponent.facing = 1;
            spriteComponent.animated = true;
            AddComponent(spriteComponent);
        }
    }
}

#endregion
