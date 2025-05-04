using System;

namespace MTT
{
    public class Weights
    {
        public Weights(decimal net, decimal tare, decimal gross)
        {
            this.net = net;
            this.tare = tare;
            this.gross = gross;
        }

        public decimal net { get; set; }
        public decimal tare { get; set; }
        public decimal gross { get; set; }
    } 
}
