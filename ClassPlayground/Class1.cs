using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ClassPlayground
{
    internal class Group
    {
        public string name {  get; set; } //Socialists, Capitalists etc.
        public double support { get; set; } //how many of them support the player
        public double turnout { get; set; } //how many of them will vote in the next election
        public double share { get; set; } //how many of them are there in the population
        public Group(string nm, double sup, double turn, double shr)
        {
            name = nm;
            support = sup;
            turnout = turn;
            share = shr;
        }

    }
}
