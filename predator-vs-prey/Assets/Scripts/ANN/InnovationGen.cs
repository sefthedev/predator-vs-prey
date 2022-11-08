using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    public class InnovationGen
    {
        int currentInnovation = 0;
        public int GetInnovation()
        {
            return currentInnovation++;
        }
    }
}