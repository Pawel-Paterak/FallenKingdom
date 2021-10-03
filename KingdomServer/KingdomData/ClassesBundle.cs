using System;
using System.Collections.Generic;
using System.Linq;

namespace KingdomData
{
    public class Account
    {
        public string login;
        public string password;

        public List<string> character = new List<string>();

        public Account()
        {

        }

        public Account(string login, string password, List<string> character)
        {
            this.login = login;
            this.password = password;
            this.character = character;
        }

        public ByteBuffer Write()
        {
            ByteBuffer data = new ByteBuffer();
            data.WriteString(login);
            data.WriteString(password);
            data.WriteInteger(character.Count);
            for (int i = 0; i < character.Count; i++)
                data.WriteString(character[i]);
            return data;
        }
        public ByteBuffer Read(ByteBuffer data)
        {
            login = data.ReadString();
            password = data.ReadString();
            int count = data.ReadInteger();
            character.Clear();
            for (int i = 0; i < count; i++)
                character.Add(data.ReadString());

            return data;
        }

        public override bool Equals(object obj)
        {
            var account = obj as Account;
            return account != null &&
                   login == account.login &&
                   password == account.password &&
                   EqualityComparer<List<string>>.Default.Equals(character, account.character);
        }

        public override int GetHashCode()
        {
            var hashCode = -9824966;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(login);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(password);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(character);
            return hashCode;
        }



    }
    public class Character
    {
        public string name;
        public int sexId = 0;
        public int idCharacter = 0;
        public int lvl = 1;
        public int life = 50;
        public int mana = 50;
        public Vector3Dfloat position = new Vector3Dfloat();

        public InventoryPlayer inventory = new InventoryPlayer();

        public Character()
        {
        }
        public Character(Character character)
        {
            name = character.name;
            sexId = character.sexId;
            idCharacter = character.idCharacter;

            lvl = character.lvl;
            life = character.life;
            mana = character.mana;

            position = character.position;
            inventory = character.inventory;
        }
        public Character(string name)
        {
            this.name = name;
        }
        public Character(string name, int sexId)
        {
            this.name = name;
            this.sexId = sexId;
        }
        public Character(string name, int sexId, int idCharacter)
        {
            this.name = name;
            this.sexId = sexId;
            this.idCharacter = idCharacter;
        }
        public Character(string name, int sexId, int idCharacter, int life, int mana)
        {
            this.name = name;
            this.sexId = sexId;
            this.idCharacter = idCharacter;
            this.life = life;
            this.mana = mana;
        }
        public Character(string name, int sexId, int idCharacter, int life, int mana, int lvl, Vector3Dfloat position, InventoryPlayer iventory)
        {
            this.name = name;
            this.sexId = sexId;
            this.idCharacter = idCharacter;
            this.lvl = lvl;
            this.life = life;
            this.mana = mana;
            this.position = position;
            this.inventory = iventory;
        }

        public ByteBuffer Write()
        {
            ByteBuffer data = new ByteBuffer();
            data.WriteString(name);
            data.WriteInteger(sexId);
            data.WriteInteger(idCharacter);
            data.WriteInteger(lvl);
            data.WriteInteger(life);
            data.WriteInteger(mana);
            data.WriteBytes(position.Write().ToArray());
            data.WriteBytes(inventory.Write().ToArray());
            return data;
        }
        public ByteBuffer Read(ByteBuffer data)
        {
            name = data.ReadString();
            sexId = data.ReadInteger();
            idCharacter = data.ReadInteger();
            lvl = data.ReadInteger();
            life = data.ReadInteger();
            mana = data.ReadInteger();
            data = position.Read(data);
            data = inventory.Read(data);

            return data;
        }
    }
    public class InventoryPlayer
    {
       
        public Inventory head = new Inventory();
        public Inventory breastplate = new Inventory();
        public Inventory legs = new Inventory();
        public Inventory boots = new Inventory();
        public Inventory leftHand = new Inventory();
        public Inventory rightHand = new Inventory();
        public Inventory backpack = new Inventory();

        public enum TypeInventory { Head, Breastplate, Legs, Boots, LeftHand, RightHand, Backpack, Null};

        public ByteBuffer Write()
        {
            ByteBuffer data = new ByteBuffer();
            data.WriteBytes(head.Write().ToArray());
            data.WriteBytes(breastplate.Write().ToArray());
            data.WriteBytes(legs.Write().ToArray());
            data.WriteBytes(boots.Write().ToArray());
            data.WriteBytes(leftHand.Write().ToArray());
            data.WriteBytes(rightHand.Write().ToArray());
            data.WriteBytes(backpack.Write().ToArray());

            return data;
        }
        public ByteBuffer Read(ByteBuffer data)
        {
            data = head.Read(data);
            data = breastplate.Read(data);
            data = legs.Read(data);
            data = boots.Read(data);
            data = leftHand.Read(data);
            data = rightHand.Read(data);
            data = backpack.Read(data);

            return data;
        }

        public InventoryPlayer()
        {

        }
        public InventoryPlayer(Inventory head, Inventory breastplate, Inventory legs, Inventory boots, Inventory leftHand, Inventory rightHand, Inventory backpack)
        {
            this.head = head;
            this.breastplate = breastplate;
            this.legs = legs;
            this.boots = boots;
            this.leftHand = leftHand;
            this.rightHand = rightHand;
            this.backpack = backpack;
        }
    }
    public class Monster
    {
        public string name;
        public int life;
        public int mana;
        public int distanceAtack;
        public Vector3Dfloat position;
    }
    public class Vector3Dfloat
    {
        public float x;
        public float y;
        public float z;

        #region Direction
        public static Vector3Dfloat Forward { get; } = new Vector3Dfloat(0, 0, 1);
        public static Vector3Dfloat Back { get; } = new Vector3Dfloat(0, 0, -1);
        public static Vector3Dfloat Left { get; } = new Vector3Dfloat(-1, 0, 0);
        public static Vector3Dfloat Right { get; } = new Vector3Dfloat(1, 0, 0);
        public static Vector3Dfloat Up { get; } = new Vector3Dfloat(0, 1, 0);
        public static Vector3Dfloat Down { get; } = new Vector3Dfloat(0, -1, 0);
        #endregion

        public Vector3Dfloat()
        {


        }
        public Vector3Dfloat(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vector3Dfloat(Vector3Dfloat vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public ByteBuffer Write()
        {
            ByteBuffer data = new ByteBuffer();
            data.WriteFloat(x);
            data.WriteFloat(y);
            data.WriteFloat(z);
            return data;
        }
        public ByteBuffer Read(ByteBuffer data)
        {
            x = data.ReadFloat();
            y = data.ReadFloat();
            z = data.ReadFloat();

            return data;
        }

        public static float Distance(Vector3Dfloat a, Vector3Dfloat b)
        {
            Vector3Dfloat vectorOne = new Vector3Dfloat(b.x, a.y, a.z);
            Vector3Dfloat vectorTwo = new Vector3Dfloat(b.x, a.y, b.z);

            float lenghtFromAtoVecOne = vectorOne.x - a.x;
            float lenghtFromVecOnetoVecTwo = vectorTwo.z - vectorOne.z;
            float lenghtFromVecTwoToB = b.y - vectorTwo.y;

            double distance = Math.Sqrt((Math.Pow(lenghtFromAtoVecOne, 2) + Math.Pow(lenghtFromVecOnetoVecTwo, 2)) + Math.Pow(lenghtFromVecTwoToB, 2));

            return (float)distance;
        }
        public static float Distance2D(Vector3Dfloat a, Vector3Dfloat b)
        {
            float distanceAToPoint = b.z - a.z;
            float distanceBToPoint = b.x - a.x;
            double distance = Math.Sqrt(Math.Pow(distanceAToPoint, 2) + Math.Pow(distanceBToPoint, 2));
            return (float)distance;
        }
        public static bool IsNull(Vector3Dfloat vector)
        {
            try
            {
                Vector3Dfloat vec = new Vector3Dfloat(vector);
            }
            catch (Exception)
            {
                return true;
            }
            return false;
        }

        #region Operators
        public static bool operator ==(Vector3Dfloat left, Vector3Dfloat right)
        {
            return left.x == right.x && left.y == right.y && left.z == right.z;
        }
        public static bool operator !=(Vector3Dfloat left, Vector3Dfloat right)
        {
            return left.x != right.x && left.y != right.y && left.z != right.z;
        }
        public static Vector3Dfloat operator +(Vector3Dfloat left, Vector3Dfloat right)
        {
            return new Vector3Dfloat(left.x + right.x, left.y + right.y, left.z + right.z);
        }
        public static Vector3Dfloat operator -(Vector3Dfloat left, Vector3Dfloat right)
        {
            return new Vector3Dfloat(left.x - right.x, left.y - right.y, left.z - right.z);
        }
        public static Vector3Dfloat operator *(Vector3Dfloat left, float intiger)
        {
            return new Vector3Dfloat(left.x * intiger, left.y * intiger, left.z * intiger);
        }
        public static Vector3Dfloat operator /(Vector3Dfloat left, float intiger)
        {
            return new Vector3Dfloat(left.x / intiger,left.y / intiger, left.z / intiger);
        }

        public static implicit operator string(Vector3Dfloat vector)
        {
            return "X: " + vector.x + " Y: " + vector.y + " Z: " + vector.z;
        }
        #endregion
    }
    public class Rotation
    {
        public float x;
        public float y;
        public float z;

        public Rotation()
        {


        }
        public Rotation(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Rotation(Rotation rotation)
        {
            x = rotation.x;
            y = rotation.y;
            z = rotation.z;
        }

        public ByteBuffer Write()
        {
            ByteBuffer data = new ByteBuffer();
            data.WriteFloat(x);
            data.WriteFloat(y);
            data.WriteFloat(z);
            return data;
        }
        public ByteBuffer Read(ByteBuffer data)
        {
            x = data.ReadFloat();
            y = data.ReadFloat();
            z = data.ReadFloat();

            return data;
        }

        public static implicit operator string(Rotation rotation)
        {
            return "X: " + rotation.x + " Y: " + rotation.y + " Z: " + rotation.z;
        }
    }
    public class AtackData
    {
        public string who = "";
        public int id = -1;
        public enum Type {Player,Monster, Null};
        public Type type = Type.Null;

        public ByteBuffer Write()
        {
            ByteBuffer data = new ByteBuffer();
            data.WriteInteger((int)type);
            if (type == Type.Player)
                data.WriteString(who);
            else
                data.WriteInteger(id);
            return data;
        }
        public ByteBuffer Read(ByteBuffer data)
        {
            type = (Type)data.ReadInteger();
            if (type == Type.Player)
                who = data.ReadString();
            else
                id = data.ReadInteger();

            return data;
        }

        public AtackData()
        {

        }
        public AtackData(string who)
        {
            this.who = who;
            type = Type.Player;
        }
        public AtackData(int id)
        {
            this.id = id;
            type = Type.Monster;
        }
        public AtackData(string who, int id, Type type)
        {
            this.who = who;
            this.id = id;
            this.type = type;
        }
    }
    public class World
    {
        public string name;
        public List<MapTerrain> terrains = new List<MapTerrain>();
        public List<MapSpawn> spawns = new List<MapSpawn>();
        public List<MapObject> objects = new List<MapObject>();


        public World()
        {

        }
        public World(string name, List<MapTerrain> terrains, List<MapSpawn> spawns, List<MapObject> objects)
        {
            this.name = name;
            this.terrains = terrains;
            this.spawns = spawns;
            this.objects = objects;
        }

        public MapObject GetObjectFromPosition(Vector3Dfloat position)
        {
            foreach (MapObject obj in objects)
                if (obj.position == position)
                    return obj;

            return null;
        }
        public MapTerrain GetTerrainFromPosition(Vector3Dfloat position)
        {
            foreach (MapTerrain terrain in terrains)
                if (terrain.position == position)
                    return terrain;
            return null;
        }
        public MapObject GetObjectFromIndex(int index)
        {
            if (objects.Count >= index)
                return null;
            else
                return objects[index];
        }
        public int GetObjectIndexFromPosition(Vector3Dfloat position)
        {
            for (int i = 0; i < objects.ToArray().Length; i++)
                if (objects[i].position == position)
                    return i;

            return -1;
        }
    }
    public class MapTerrain
    {
        public string name;
        public Vector3Dfloat position;
        public bool blocking;

        public MapTerrain()
        {

        }

        public MapTerrain(string name, Vector3Dfloat position)
        {
            this.name = name;
            this.position = position;
        }
    }
    public class MapSpawn
    {
        public string name;
        public float timeRespawn;
        public Vector3Dfloat position;

        public MapSpawn()
        {

        }

        public MapSpawn(string name, float timeRespawn, Vector3Dfloat position)
        {
            this.name = name;
            this.timeRespawn = timeRespawn;
            this.position = position;
        }
    }
    public class MapObject
    {
        public string nameObject;
        public Vector3Dfloat position;
        public Rotation rotation;

        public MapObject()
        {

        }

        public MapObject(string nameObject, Vector3Dfloat position, Rotation rotation)
        {
            this.nameObject = nameObject;
            this.position = position;
            this.rotation = rotation;
        }
    }
    public class Paths
    {
        public int nextId { get; set;}
        public List<Path> paths { get; set; } = new List<Path>();
        public Paths()
        {

        }
        public Paths(List<Path> paths)
        {
            this.paths = paths;

        }
        public void Add(Path path)
        {
            path.id = nextId;
            paths.Add(path);
            nextId++;
        }
    }
    public class Path
    {
        public int id { get; set; }
        public string path { get; set; }
        public Item.TypeItem typeItem { get; set; }

        public Path()
        {

        }
        public Path(int id, string path, Item.TypeItem typeItem)
        {
            this.id = id;
            this.path = path;
            this.typeItem = typeItem;
        }
    }
    public class Item
    {
        public enum TypeItem {Item,Weapon,Food}
        public string name { get; set; }
        public string nameTexture { get; set; }
        public TypeItem typeItem { get; set; }
        public float weight { get; set; }

        public Item()
        {

        }
        public Item(string name, string nameTexture, TypeItem typeItem, float weight)
        {
            this.name = name;
            this.nameTexture = nameTexture;
            this.typeItem = typeItem;
            this.weight = weight;
        }
    }
    public class Weapon : Item
    {
        public int damage;
        public int defense;
        public float distanceAtack;
        public Weapon()
        {
            
        }
        public Weapon(string name, string nameTexture,TypeItem typeItem, float weight, int damage, int defense, float distanceAtack)
        {
            this.name = name;
            this.nameTexture = nameTexture;
            this.typeItem = typeItem;
            this.weight = weight;
            this.damage = damage;
            this.defense = defense;
            this.distanceAtack = distanceAtack;

        }
    }
    public class Food : Item
    {
        public int health;
        public int mana;

        public Food()
        {

        }
        public Food(string name, string nameTexture,TypeItem typeItem, float weight, int health, int mana)
        {
            this.name = name;
            this.nameTexture = nameTexture;
            this.typeItem = typeItem;
            this.weight = weight;
            this.health = health;
            this.mana = mana;
        }
    }
    public class Inventory : ParseByteBuffer
    {
        private int id = -1;
        public int Id { get { return id; } set { id = value; RefreshParentId(); } }
        public Slots slots { get; set; } = new Slots();
        public bool isExists;

        public Inventory()
        {
            slots = new Slots();
        }
        public Inventory(Slots slots)
        {
            this.slots = slots;
        }

        public void RefreshParentId()
        {
            slots.SetParentId(id);
        }
        public void Add(Slots slots, List<int> road)
        {
            if (slots == null)
                return;

            slots.parentId = Id;
            if (road == null || road.Count <= 0)
                this.slots = slots;
            else
                this.slots.Add(slots, road);
        }
        public void Change(Slots slots, List<int> road)
        {
            if (slots == null)
                return;

            slots.parentId = Id;
            if (road == null || road.Count <= 0)
                this.slots = slots;
            else
                this.slots.Change(slots, road);
        }
        public List<int> FindRoad(Slots slots)
        {
            if (this.slots == slots)
                return new List<int> { };
            else
            {
                Console.WriteLine("I go to " + this.slots);
                List<int> road = this.slots.FindRoad(slots);
                road.Reverse();
                return road;
            }
        }
        public override ByteBuffer Write()
        {
            ByteBuffer data = new ByteBuffer();
            data.WriteInteger(Id);
            data.WriteBytes(slots.Write().ToArray());
            return data;
        }
        public override ByteBuffer Read(ByteBuffer data)
        {
            Id = data.ReadInteger();
            data = slots.Read(data);
            return data;
        }
    }
    public class Slots : ParseByteBuffer
    {
        public int id { get; set; }
        public int count { get; set; }
        public int parentId = -1;
        public List<Slots> slots { get; set; } = new List<Slots>();

        public Slots()
        {
            
        }
        public Slots(int id, int count)
        {
            this.id = id;
            this.count = count;
        }
        public Slots(int id, int count, int parentId)
        {
            this.id = id;
            this.count = count;
            this.parentId = parentId;
        }
        public Slots(int id, int count, int parentId, List<Slots> slots)
        {
            this.id = id;
            this.count = count;
            this.parentId = parentId;
            this.slots = slots;
        }

        public void Add(Slots slots, List<int> road)
        {
            int index =  road[0];
                
            if (index >= 0)
            {
                if (road.Count >= 0 && road.Count < 2)
                    this.slots.Add(slots);
                else
                {
                    road.RemoveAt(0);
                    this.slots[index].Add(slots, road);
                }
            }
        }
        public void Change(Slots slots, List<int> road)
        {
            int index = 0;
            if (road != null && road.Count > 0)
                index = road[0];
                
            if (index >= 0)
            {
                if (road.Count >= 0 && road.Count < 2 && index > -1 && index < slots.count)
                    this.slots[index] = slots;
                else if(index > -1 && index < slots.count)
                {
                    road.RemoveAt(0);
                    this.slots[index].Change(slots, road);
                }
            }
        }
        public void SetParentId(int id)
        {
            parentId = id;
            foreach (Slots slots in slots.ToArray())
                slots.SetParentId(id);
        }
        public override ByteBuffer Write()
        {
            ByteBuffer data = new ByteBuffer();
            data.WriteInteger(id);
            data.WriteInteger(count);
            data.WriteInteger(parentId);
            data.WriteInteger(slots.Count);
            foreach (Slots coor in slots)
                data.WriteBytes(coor.Write().ToArray());

            return data;
        }
        public override ByteBuffer Read(ByteBuffer data)
        {
            id = data.ReadInteger();
            count = data.ReadInteger();
            parentId = data.ReadInteger();
            int countSlots = data.ReadInteger();
            for (int i = 0; i < countSlots; i++)
            {
                Slots newSlots = new Slots();
                data = newSlots.Read(data);
                slots.Add(newSlots);
            }
            return data;
        }
        public Slots GetSlotsFromRoad(List<int> road)
        {
            if (road.Count <= 0)
                return this;
            else
            {
                int index = road[0];
                road.RemoveAt(0);
                if (index >= 0 && index < slots.Count)
                    return slots[index].GetSlotsFromRoad(road);
                else
                    return null;

            }
        }
        public List<int> FindRoad(Slots slots)
        {
            for (int i = 0; i < this.slots.Count; i++)
                if (this.slots[i] == slots)
                {
                    Console.WriteLine("this.Slots[i] == slots");
                    Console.WriteLine("this.Slots["+i+"]: "+this.slots[i]);
                    Console.WriteLine("slots: "+slots);
                    return new List<int> { i };
                }
            
            for(int i=0; i<this.slots.Count; i++)
            {
                List<int> road = this.slots[i].FindRoad(slots);
                if (road != null)
                {
                    road.Add(i);
                    return road;
                }
            }

            return null;
        }
        public static void Move(ref Slots slots1, ref Slots slots2, bool moveFromParentId = false)
        {
            if (moveFromParentId)
            {
                Slots slots = slots1;
                slots1 = slots2;
                slots2 = slots;
            }
            else
            {
                int parentIdSlots1 = slots1.parentId;
                int parentIdSlots2 = slots2.parentId;
                Slots slots = slots1;
                slots1 = slots2;
                slots2 = slots;
                slots1.parentId = parentIdSlots1;
                slots2.parentId = parentIdSlots2;
            }
        }
        public static implicit operator string(Slots slots)
        {
            return "Id: " + slots.id + " Count: " + slots.count + " Parent ID: " + slots.parentId + " Count Slots: " + slots.slots.Count;
        }
    }
    public class SlotsMovment : ParseByteBuffer
    {
        public Slots from = new Slots();
        public List<int> roadFrom = new List<int>();
        public Slots to = new Slots();
        public List<int> roadTo = new List<int>();

        public SlotsMovment()
        {

        }
        public SlotsMovment(Slots from, List<int> roadFrom, Slots to, List<int> roadTo)
        {
            this.from = from;
            this.roadFrom = roadFrom;
            this.to = to;
            this.roadTo = roadTo;
        }

        public override ByteBuffer Read(ByteBuffer data)
        {
            data = from.Read(data);
            int countFrom = data.ReadInteger();
            for (int i = 0; i < countFrom; i++)
                roadFrom.Add(data.ReadInteger());

            data = to.Read(data);
            int countTo = data.ReadInteger();
            for (int i = 0; i < countTo; i++)
                roadTo.Add(data.ReadInteger());
            return data;
        }
        public override ByteBuffer Write()
        {
            ByteBuffer data = new ByteBuffer();
            data.WriteBytes(from.Write().ToArray());
            data.WriteInteger(roadFrom.Count);
            foreach (int indexRoad in roadFrom)
                data.WriteInteger(indexRoad);

            data.WriteBytes(to.Write().ToArray());
            data.WriteInteger(roadTo.Count);
            foreach (int indexRoad in roadTo)
                data.WriteInteger(indexRoad);
            return data;
        }
    }
    public class MoveSlots : ParseByteBuffer
    {
        public int idInventoryFrom;
        public List<int> roadSlotsFrom = new List<int>();
        public int idInventoryTo;
        public List<int> roadSlotsTo = new List<int>();

        public override ByteBuffer Write()
        {
            ByteBuffer data = new ByteBuffer();

            data.WriteInteger(idInventoryFrom);
            data.WriteInteger(roadSlotsFrom.Count);
            foreach (int integer in roadSlotsFrom)
                data.WriteInteger(integer);

            data.WriteInteger(idInventoryTo);
            data.WriteInteger(roadSlotsTo.Count);
            foreach (int integer in roadSlotsTo)
                data.WriteInteger(integer);

            return data;
        }
        public override ByteBuffer Read(ByteBuffer data)
        {
            idInventoryFrom = data.ReadInteger();
            int countSlotsFrom = data.ReadInteger();
            for (int i = 0; i < countSlotsFrom; i++)
                roadSlotsFrom.Add(data.ReadInteger());

            idInventoryTo = data.ReadInteger();
            int countSlotsTo = data.ReadInteger();
            for (int i = 0; i < countSlotsTo; i++)
                roadSlotsTo.Add(data.ReadInteger());

            return data;
        }

        public MoveSlots()
        {

        }
        public MoveSlots(int idInventoryFrom, List<int> roadSlotsFrom, int idInventoryTo, List<int> roadSlotsTo)
        {
            this.idInventoryFrom = idInventoryFrom;
            this.roadSlotsFrom = roadSlotsFrom;
            this.idInventoryTo = idInventoryTo;
            this.roadSlotsTo = roadSlotsTo;
        }
    }
    public class FKObjectPart : ParseByteBuffer
    {
        public long id { get; private set; }
        public Vector3Dfloat position = new Vector3Dfloat();

        public FKObjectPart()
        {

        }
        public FKObjectPart(long id, Vector3Dfloat position)
        {
            this.id = id;
            this.position = position;
        }

        public override ByteBuffer Write()
        {
            ByteBuffer data = new ByteBuffer();
            data.WriteLong(id);
            data.WriteBytes(position.Write().ToArray());
            return data;
        }
        public override ByteBuffer Read(ByteBuffer data)
        {
            id = data.ReadLong(); 
            data = position.Read(data);
            return data;
        }
    }

    public abstract class ParseByteBuffer
    {
        public abstract ByteBuffer Write();
        public abstract ByteBuffer Read(ByteBuffer data);
    }
   
}
