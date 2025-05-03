using System;

namespace MTT
{
    public class Weights
    {
        public Weights(float net, float tare)
        {
            this.net = net;
            this.tare = tare;
        }

        public float net { get; set; }
        public float tare { get; set; }
    } 
}
