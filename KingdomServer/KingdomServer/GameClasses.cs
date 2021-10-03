using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KingdomData;

namespace KingdomServer
{
    public class Health
    {
        public int health;
        public int mana;

        public bool died = false;

        public Health()
        {

        }
        public Health(int health, int mana)
        {
            this.health = health;
            this.mana = mana;
        }

        public void TakeDamage(int health)
        {
            this.health -= health;
            Checking();
        }
        public void RemoveMana(int mana)
        {
            this.mana -= mana;
            Checking();
        }
        private void Checking()
        {
            if (health <= 0)
                died = true;
        }
    }
    public class MapMonster
    {
        public int id;
        public Vector3Dfloat position;
        public Vector3Dfloat direction;
        public Health health;

        public MapMonster()
        {

        }
        public MapMonster(int id, Vector3Dfloat position, Health health)
        {
            this.id = id;
            this.position = position;
            this.health = health;
        }
    }
}
