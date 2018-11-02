using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using SkillMe.tf.Models;

namespace SkillMe.tf.ViewModel
{
    public class StatViewModel : ViewModelBase
    {
        public Classes StatClass { get; set; }

        public String Header { get; set; }

        public String LowerHeader { get; set; }

        public String MainText { get; set; }
    }
}
