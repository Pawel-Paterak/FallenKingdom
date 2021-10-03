using System;
using System.Globalization;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KingdomData;

namespace CoreKingdom
{
    public class EngineFallenKingdom
    {
        public bool isWorking { get; private set; }
        public static EngineFallenKingdom singleton;
        private Thread threadEngine;
        public SettingsEngine settings = new SettingsEngine();
        private long nextId = 0;
        public List<Component> objects = new List<Component>();

        public void Start()
        {
            if (singleton != null)
                singleton = null;

            singleton = this;
            isWorking = true;
            threadEngine = new Thread(Working);
            threadEngine.Start();
        }
        public void Stop()
        {
            isWorking = false;
        }
        private void Working()
        {
            while (isWorking)
            {
                if (objects.Count > 0)
                {
                    Update();
                    Collisions();
                }
            }
        }
        private void Update()
        {
            if (objects.Count > 0)
                foreach (Component coor in objects.ToArray())
                    if (coor != null)
                        coor.Update();
        }
        private void Collisions()
        {
            foreach (Component myComponent in objects.ToArray())
                if (myComponent != null && myComponent is FKObject)
                {

                    var myCoor = myComponent as FKObject;
                    Collider myCollider = myCoor.GetComponent<Collider>();
                    if (myCollider != null)
                        foreach (Component component in objects.ToArray())
                            if (component != null && myCoor != component && component is FKObject)
                            {
                                var coor = component as FKObject;
                                Collider collider = coor.GetComponent<Collider>();
                                if (collider != null)
                                {
                                   
                                    Vector3Dfloat myCoorPosition = myCoor.transform.position;
                                    Vector3Dfloat coorPosition = coor.transform.position;
                                    Vector3Dfloat myColliderPosition = new Vector3Dfloat();
                                    Vector3Dfloat colliderPosition = new Vector3Dfloat();

                                    bool collision = false;
                                    for (float mCX = -myCollider.size.x; mCX <=myCollider.size.x; mCX += settings.veryficationColsion)
                                    {
                                        for (float mCZ = -myCollider.size.z; mCZ <=myCollider.size.z; mCZ += settings.veryficationColsion)
                                        {
                                            myColliderPosition = new Vector3Dfloat(mCX + myCoorPosition.x, myCoorPosition.y, mCZ + myCoorPosition.z);
                                            for (float cX = -collider.size.x; cX <=collider.size.x; cX += settings.veryficationColsion)
                                            {
                                                for (float cZ = -collider.size.z; cZ <=collider.size.z; cZ += settings.veryficationColsion)
                                                {
                                                    colliderPosition = new Vector3Dfloat(cX + coorPosition.x, coorPosition.y, cZ + coorPosition.z);
                                                    if (myColliderPosition == colliderPosition)
                                                    {
                                                        collision = true;
                                                        break;
                                                    }
                                                }
                                                if (collision)
                                                    break;
                                            }
                                        }
                                        if (collision)
                                            break;
                                    }
                                    if (collision)
                                    {
                                        myCoor.CollisionStay(new Collision(coor, myColliderPosition));
                                        coor.CollisionStay(new Collision(myCoor, myColliderPosition));
                                    }

                                    continue;
                                }
                            }
                }
        }
        public bool Collision(Vector3Dfloat position, out FKObject obj,float distance = 999999999)
        {
            
            foreach(Component component in objects)
                if(component != null && component is FKObject)
                {
                    obj = component as FKObject;
                    if (obj.transform.position.y == position.y && Vector3Dfloat.Distance(position, obj.transform.position) <= distance)
                    {
                        var coll = obj.GetComponent<Collider>();
                        if (coll != null)
                        {
                            Vector3Dfloat startVertex = obj.transform.position - new Vector3Dfloat(coll.size.x, 0, coll.size.z);
                            Vector3Dfloat endVertex = obj.transform.position + new Vector3Dfloat(coll.size.x, 0, coll.size.z);

                            float.TryParse(String.Format(CultureInfo.InvariantCulture, "{0:0.00}", position.x), out float positionX);
                            float.TryParse(String.Format(CultureInfo.InvariantCulture, "{0:0.00}", position.z), out float positionZ);

                            float.TryParse(String.Format(CultureInfo.InvariantCulture, "{0:0.00}", startVertex.x), out float startX);
                            float.TryParse(String.Format(CultureInfo.InvariantCulture, "{0:0.00}", startVertex.z), out float startZ);
                            float.TryParse(String.Format(CultureInfo.InvariantCulture, "{0:0.00}", endVertex.x), out float endX);
                            float.TryParse(String.Format(CultureInfo.InvariantCulture, "{0:0.00}", endVertex.z), out float endZ);

                            Console.WriteLine("Value:  "+ startVertex.z+ " " + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", startVertex.z) + " Float: "+ startZ);

                            Console.WriteLine(position+" ");
                            Console.WriteLine("{0} <= {1} , {2} <= {3} ({4},{5})" , startX, positionX, startZ, positionZ, startX <= positionX, startZ <= positionZ);
                            Console.WriteLine("{0} >= {1} , {2} >= {3} ({4},{5})", endX, positionX, endZ, positionZ, endX >= positionX, endZ >= positionZ);
                            
                            if (startX <= positionX && startZ <= positionZ)
                            {
                                if (endX >= positionX && endZ >= positionZ)
                                {
                                    Console.WriteLine("(hit)");
                                    return true;
                                }
                            }
                        }
                    }
                }
            obj = null;
            return false;
        }

        public long AddComponent(Component component)
        {
            long id = nextId;
            nextId++;
            objects.Add(component);
            return id;
        }
        public FKObject FindObject(string name)
        {
            foreach(Component coor in objects)
                if(coor is FKObject)
                {
                    FKObject obj = coor as FKObject;
                    if (obj.name == name)
                        return obj;
                }
            return null;
                    
        }
        public FKObject FindObjectFromTag(string tag)
        {
            foreach (Component coor in objects)
                if (coor is FKObject)
                {
                    FKObject obj = coor as FKObject;
                    if (obj.tag == tag)
                        return obj;
                }
            return null;

        }
    }

    public class SettingsEngine
    {
        public readonly float veryficationColsion = 0.1f;
    }
}
