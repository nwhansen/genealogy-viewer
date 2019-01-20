using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genealogy.Model
{
    internal class ParentChangedEventArgs : EventArgs
    {
        public Individual OldParent { get; private set; }
        public Individual NewParent { get; private set; }

        public ParentChangedEventArgs(Individual oldParent, Individual newParent)
        {
            OldParent = oldParent;
            NewParent = newParent;
        }

    }
}
