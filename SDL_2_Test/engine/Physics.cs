using SDL2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SDL_2_Test.engine
{
    public class Vector
    {
        public double x { get; set; }
        public double y { get; set; }

        public Vector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double Length()
        {
            double result = 0;
            result = Math.Sqrt(x * x + y * y);
            return result;
        }

        public Vector Normalized()
        {
            return this * (1 / Length());
        }
        public static Vector operator *(Vector vector, double scalar)
        {
            vector.x *= scalar;
            vector.y *= scalar;
            return vector;
        }

        public static double Dot(Vector vector1, Vector vector2)
        {
            return vector1.x * vector2.x + vector1.y * vector2.y;
        }

        public static double Cross(Vector vector1, Vector vector2)
        {

            return vector1.x * vector2.y + vector1.y * vector2.x;
        }

        public static Vector Cross(Vector vector, double scalar)
        {
            vector.x = vector.y * scalar;
            vector.y = vector.x * -scalar;
            return vector;
        }

        public static Vector Cross(double scalar, Vector vector)
        {
            vector.x = vector.y * -scalar;
            vector.y = vector.x * scalar;
            return vector;
        }
    }
    public class Physics
    {
        public static void Update(double elapsed)
        {
            foreach (Entity entity in Variables.Entities.ToList())
            {
                if (entity.GetMoveableObject())
                    Update(entity, elapsed);
            }
        }

        public static void Update(Entity entity, double elapsed)
        {
            Vector force = entity.GetForce();
            var isGrounded = entity.Grounded();
            elapsed = elapsed / 1000000000;
            //20px = 1m

            if (isGrounded)
            {
                entity.SetForce(new Vector(0, Variables.GravitationalPull * entity.GetMass()));
                entity.SetVelocity(0);
            }
            else
            {

                var yForce = Variables.GravitationalPull * entity.GetMass();
                if (force.y < yForce)
                {
                    force.y += 40 * elapsed;
                }
                else if (force.y > yForce)
                    force.y = yForce;

                var xForce = 0;
                if(force.x < xForce)
                {
                    force.x += 40 * elapsed;
                } else if(force.x > xForce) {
                    force.x -= 40 * elapsed;
                }
                entity.SetForce(force);
            }

            double velocity = entity.GetVelocity();
            double mass = entity.GetMass();

            double forceLength;
            if (force.y < 0)
                forceLength = -force.Length();
            else
                forceLength = force.Length();

            double acceleration = forceLength / mass;
            velocity += elapsed * acceleration;

            float x;
            float y;

            x = (float)(Math.Sign(force.x) * velocity * elapsed) * Variables.Meter;  // 1m/s = 20px/s 
            y = (float)(Math.Sign(force.y) * velocity * elapsed) * Variables.Meter; // 1m/s = 20px/s 
            entity.SetVelocity(velocity);

            var keyArray = Variables.KeyArray;
            MainProgram.Input(keyArray, entity, elapsed, isGrounded, x, y);

            List<Entity> list;
            list = CheckCollision(entity);
            if(list.Count() > 0)
            {
                foreach(Entity collider in list)
                {
                    SolveCollision(entity, collider);
                }
            }
           
        }

        public static List<Entity> CheckCollision(Entity entity)
        {
            List<Entity> list = new List<Entity>();
            int i = 0;
            if (Variables.Entities[i] == null)
                return list;
            while (i < Variables.Entities.Count)
            {
                if (!Variables.Entities[i].GetSolid() || entity == Variables.Entities[i])
                {
                    i++;
                    continue;
                }

                var hitbox = entity.GetHitbox();
                var collider = Variables.Entities[i].GetHitbox();

                float x1 = collider.x - (hitbox.x + hitbox.w);
                float y1 = collider.y - (hitbox.y + hitbox.h);
                float x2 = hitbox.x - (collider.x + collider.w);
                float y2 = hitbox.y - (collider.y + collider.h);

                if (x1 > 0 || y1 > 0 || x2 > 0 || y2 > 0)
                {
                    i++;
                    continue;
                }

                if (x1 == 0 || y1 == 0 || x2 == 0 || y2 == 0)
                {
                    i++;
                    continue;
                }
                list.Add(Variables.Entities[i]);
                i++;
            }
            return list;
        }
        public static void SolveCollision(Entity entity, Entity collider)
        {

            if (MainProgram.CollisionResolve(entity, collider))
                return;


            var eMid = entity.GetMid();
            var cMid = collider.GetMid();

            float dx = (cMid.Item1 - eMid.Item1) / collider.HalfWidth();
            float dy = (cMid.Item2 - eMid.Item2) / collider.HalfHeight();

            var absDx = Math.Abs(dx);
            var absDy = Math.Abs(dy);

            //corner
            if (Math.Abs(absDx - absDy) < .1)
            {
                //from positive x
                if (dx < 0)
                {
                    var length = collider.GetRight() - entity.GetLeft();
                    entity.SetHitbox(length, 0);
                    //negative x
                }
                else
                {
                    var length = entity.GetRight() - collider.GetLeft();
                    entity.SetHitbox(-length, 0);
                }
                //from positive y
                if (dy < 0)
                {
                    var length = collider.GetBottom() - entity.GetTop();
                    entity.SetHitbox(0, length);
                }
                //negative y
                else
                {
                    var length = entity.GetBottom() - collider.GetTop();
                    entity.SetHitbox(0, -length);
                }
                //sides
            }
            else if (absDx > absDy)
            {
                //positive x
                if (dx < 0)
                {
                    var length = collider.GetRight() - entity.GetLeft();
                    entity.SetHitbox(length, 0);
                }
                else //negative x
                {
                    var length = entity.GetRight() - collider.GetLeft();
                    entity.SetHitbox(-length, 0);
                }
            }
            else // top or bottom
            {
                //positive y
                if (dy < 0)
                {
                    var length = collider.GetBottom() - entity.GetTop();
                    entity.SetHitbox(0, length);
                    entity.SetVelocity(0);
                }
                else //negative y
                {
                    var length = entity.GetBottom() - collider.GetTop();
                    entity.SetHitbox(0, -length);
                }
            }
        }
    }
}
