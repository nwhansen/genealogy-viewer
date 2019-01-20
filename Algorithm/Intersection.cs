using Genealogy.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithm
{
    public class Intersection
    {

        public static IEnumerable<Individual> IntersectChildren(Individual left, Individual right)
        {
            var processed = new HashSet<Individual>();
            var toProcess = new Stack<Individual>();
            toProcess.Push(left);
            while(toProcess.Count != 0)
            {
                Individual current = toProcess.Pop();
                foreach(var child in current.AllChildren)
                {
                    if(!processed.Contains(child))
                    {
                        processed.Add(child);
                        toProcess.Push(child);
                    }
                }
            }
            toProcess.Push(right);
            var processed2 = new HashSet<Individual>();

            while (toProcess.Count != 0)
            {
                Individual current = toProcess.Pop();
                foreach (var child in current.AllChildren)
                {
                    //Skip duplicate
                    if(processed2.Contains(child))
                    {
                        continue;
                    }
                    processed2.Add(child);
                    if (processed.Contains(child))
                    {
                        yield return child;
                    }
                    toProcess.Push(child);
                }
            }
        }
    }
}
