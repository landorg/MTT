using System;

namespace MTT
{
    public class Weights
    {
        public Weights(float net, float tare, float gross)
        {
            this.net = net;
            this.tare = tare;
            this.gross = gross;
        }

        public float net { get; set; }
        public float tare { get; set; }
        public float gross { get; set; }
    } 
}
