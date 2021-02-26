using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
    [System.Serializable]
    public struct PlanetData
    {
        public SerializableVector3 position;
        public float mass;
        public SerializableVector3 velocity;
        public SerializableVector3 color;
        public int isEnabled;
        public int collided;
    }

}
