using System;

namespace MTT
{
    public class Weights
    {
        public Weights(decimal net, decimal tare, decimal gross, bool instable)
        {
            this.net = net;
            this.tare = tare;
            this.gross = gross;
            this.instable = instable;
        }

        public decimal net { get; set; }
        public decimal tare { get; set; }
        public decimal gross { get; set; }
        public bool instable { get; set; }
    } 
}
