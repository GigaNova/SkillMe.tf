using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillMe.tf.Models
{
    public enum Classes
    {
        All,
        Scout,
        Soldier,
        Pyro,
        Heavy,
        Demoman,
        Spy,
        Engineer,
        Sniper,
        Medic
    }

    public class ClassesHelper
    {
        public static Classes FromString(string name)
        {
            switch (name)
            {
                case "scout":
                    return Classes.Scout;
                case "soldier":
                    return Classes.Soldier;
                case "pyro":
                    return Classes.Pyro;
                case "heavy":
                case "heavyweapons":
                    return Classes.Heavy;
                case "demoman":
                    return Classes.Demoman;
                case "spy":
                    return Classes.Spy;
                case "engineer":
                    return Classes.Engineer;
                case "sniper":
                    return Classes.Sniper;
                case "medic":
                    return Classes.Medic;
            }

            return Classes.All;
        }
    }
}
