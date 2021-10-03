using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KingdomData;
using KingdomServer;

namespace CoreKingdom
{
    public class CoreFallenKingdom : FKObject
    {
       
        public CoreFallenKingdom()
        {
            
        }
    }
    public class RayCastHit
    {
        public Vector3Dfloat start;
        public Vector3Dfloat end;
        public Collision collision;
    }
    public class Ray
    {

        public bool RayCast(Vector3Dfloat position, Vector3Dfloat direction, out RayCastHit hit, float distance)
        {
            direction.y = direction.y % 360;
            direction.y = (direction.y < 0) ? -direction.y : direction.y;
            float quarter = direction.y / 90;
            float directionX = direction.y % 90;
            float angle = 90 - directionX;
            float angleXScale = directionX / 90;
            float angleYScale = angle / 90;

            bool next = true;
            float point = 0;

            while(next)
            {
                float xf = angleXScale * point;
                float yf = point * angleYScale;
                switch (quarter)
                {

                    case float val when (val >= 1 && val < 2):
                            float minusY = yf;
                            yf = xf;
                            xf = minusY;
                            break;
                    case float val when (val >= 2 && val < 3):
                            xf = -xf;
                            break;
                    case float val when (val >= 3 && val <= 4):
                            float minusX = -xf;
                            xf = -yf;
                            yf = minusX;
                            break;
                    default:
                        break;
                }
                Vector3Dfloat pointRayPosition = new Vector3Dfloat(position.x + xf, position.y, position.z + yf);
                float distancePoint = Vector3Dfloat.Distance(position, pointRayPosition);
                if (distancePoint > distance)
                {
                    next = false;
                    break;
                }

                if (EngineFallenKingdom.singleton.Collision(pointRayPosition, out FKObject obj))
                {
                    hit = new RayCastHit();
                    hit.start = position;
                    hit.collision = new Collision(obj, pointRayPosition);
                    return true;
                }
                point += EngineFallenKingdom.singleton.settings.veryficationColsion;
            }

            hit = null;
            return false;
        }
    }
    public class FKObject : Component
    {
        public string name = "Default";
        public string tag = "Default";
        public Transform transform { get; } = new Transform();
        private List<Component> components = new List<Component>();

        sealed public override T AddComponent<T>()
        {
            Component component = new T() as Component;
            components.Add(component);
            return (T)component;
            
        }
        sealed public override T GetComponent<T>()
        {
            foreach (Component coor in components.ToArray())
                if (coor != null && coor is T)
                    return (T)coor;
            
            return null;
        }

        public static FKObject Find(string name)
        {
            return EngineFallenKingdom.singleton.FindObject(name);
        }
        public static FKObject FindTag(string name)
        {
            return EngineFallenKingdom.singleton.FindObjectFromTag(name);
        }
    }
    public class Transform : Component
    {
        public Vector3Dfloat position { get; set; } = new Vector3Dfloat();
        public Rotation rotation { get; set; } = new Rotation();

        private void RefreshPosition()
        {
            FKObjectPart part = new FKObjectPart(id,position);

            TcpServer.singleton.SendDataToAllClients(part.Write(), PacketsConnection.PacketClient.Game, (int)PacketsConnection.InputClientGame.NewEngine);
        }
    }
    public class Component
    {
        public long id { get; private set; }

        public virtual void Start()
        {
          
        }
        public virtual void Update()
        {

        }
        public virtual void CollisionStay(Collision collision)
        {

        }
        public virtual T AddComponent<T>() where T : Component, new()
        {
            return null;
        }
        public virtual T GetComponent<T>() where T : Component
        {
            return null;
        }

        public Component()
        {
            id = EngineFallenKingdom.singleton.AddComponent(this);
            Start();
        }
    }
    public class Collider : Component
    {
        public enum TypeCollider {Square}
        public TypeCollider typeCollider { get; private set; }
        public Vector3Dfloat size = new Vector3Dfloat();

        public Collider()
        {

        }
        public Collider (TypeCollider typeCollider)
        {
            this.typeCollider = typeCollider;
        }
        public Collider(TypeCollider typeCollider, Vector3Dfloat size)
        {
            this.typeCollider = typeCollider;
            this.size = size;
        }
    }
    public class Collision
    {
        public FKObject obj { get; private set; }
        public Vector3Dfloat position { get; private set;}

        public Collision(FKObject obj, Vector3Dfloat position)
        {
            this.obj = obj;
            this.position = position;
        }
    }
    public class ItemFK : Component
    {
        public Item item = new Item();
        public override void Start()
        {
            
        }
        public override void Update()
        {
            
        }
    }
}
