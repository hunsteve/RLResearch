using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTreeCSharp
{    
    class XTDirNode<T> : XTNode<T> where T : XTData 
    {        
        public const int MAX_ENTRY_COUNT = 10;
        internal List<XTDirEntry<T>> entries;
        bool son_is_data;
        XTree<T> tree;
        
        public XTDirNode(XTree<T> tree) {
            this.tree = tree;
            entries = new List<XTDirEntry<T>>();
        }

        public override XTNode<T> Insert(T data)
        {           
            XTDirEntry<T> selectedEntry = GetBestSubtree(data.GetMBR());
            selectedEntry.Enlarge(data.GetMBR());

            XTNode<T> child = selectedEntry.GetChildNode();
            XTNode<T> newChild = child.Insert(data);

            if (newChild != null)
            {                
                selectedEntry.SetMBR(child.GetMBR());
                
                entries.Add(new XTDirEntry<T>(newChild, this));

                if (entries.Count > MAX_ENTRY_COUNT)
                {
                    return Split(); //return brother node
                }              
            }

            return null;
        }


        public override MBR GetMBR()
        {
            return GetMBROfEntrySet(entries, 0, entries.Count);
        }

        public override void DebugListMBRs(List<DebugMBR> list, int level)
        {
            if (parentEntry != null)
            {
                list.Add(new DebugMBR(parentEntry.GetMBR(), level));
            }
            else
            {
                list.Add(new DebugMBR(GetMBR(), level));
            }

            foreach(XTDirEntry<T> e in entries) {
                e.GetChildNode().DebugListMBRs(list, level + 1);
            }             
        }


        private XTNode<T> Split()
        {
            List<XTDirEntry<T>> set1;
            List<XTDirEntry<T>> set2;
            TopologicalSplit(out set1, out set2);

            entries = set1;

            XTDirNode<T> ret = new XTDirNode<T>(tree);
            ret.entries = set2;
            foreach (XTDirEntry<T> e in ret.entries) 
            {
                e.SetParentNode(ret);
            }


            return ret;

            //topological split
            //ha tul nagy az atfedes
                //overlap-free split
                //ha unbalanced nodeok
                    //supernode
                //amugy
                    //split
            //amugy
                //uj testver lehet supernode is                        
        }


        private void TopologicalSplit(out List<XTDirEntry<T>> set1, out List<XTDirEntry<T>> set2)
        {
            int dimension = entries[0].GetMBR().Dimension;
            int m1 = (int) ((float)entries.Count * 0.40f);


            //Find minimal margin split axis
            double minmarg = double.MaxValue;
            int split_axis = 0;

            for (int i = 0; i < dimension; ++i)
            {
                List<XTDirEntry<T>> lower = new List<XTDirEntry<T>>(entries);
                lower.Sort(new XTDirEntryComparer<T>(i, true));
                List<XTDirEntry<T>> upper = new List<XTDirEntry<T>>(entries);
                upper.Sort(new XTDirEntryComparer<T>(i, false));

                double marg = 0.0f;

                for (int k = 0; k < entries.Count - 2 * m1 + 1; ++k)
                {                                   
                    marg += GetMBROfEntrySet(lower, 0, m1 + k).Margin();
                    marg += GetMBROfEntrySet(lower, m1 + k, lower.Count).Margin();               
                    marg += GetMBROfEntrySet(upper, 0, m1 + k).Margin();
                    marg += GetMBROfEntrySet(upper, m1 + k, upper.Count).Margin();
                }

                if (marg < minmarg)
                {
                    split_axis = i;
                    minmarg = marg;
                }
            }


            //Find minimal overlap split
            List<XTDirEntry<T>> lower2 = new List<XTDirEntry<T>>(entries);
            lower2.Sort(new XTDirEntryComparer<T>(split_axis, true));
            List<XTDirEntry<T>> upper2 = new List<XTDirEntry<T>>(entries);
            upper2.Sort(new XTDirEntryComparer<T>(split_axis, false));
            
            double minDeadArea = double.MaxValue;
            double minOverlap = double.MaxValue;
            int dist = 0;
            bool isLower = true;
            for (int k = 0; k < entries.Count - 2 * m1 + 1; k++)
            {
                double deadArea = 0;
                MBR r1 = GetMBROfEntrySet(lower2, 0, m1 + k);
                MBR r2 = GetMBROfEntrySet(lower2, m1 + k, lower2.Count);
                deadArea += r1.Area();
                deadArea += r2.Area();
                for (int i = 0; i < lower2.Count; ++i) { deadArea -= lower2[i].GetMBR().Area(); }
                double overlap = r1.Overlap(r2);

                if ((overlap < minOverlap) ||
                    ((overlap == minOverlap) && (deadArea < minDeadArea)))
                {
                    minOverlap = overlap;
                    minDeadArea = deadArea;
                    dist = m1 + k;
                    isLower = true;
                }
                
                deadArea = 0;
                r1 = GetMBROfEntrySet(upper2, 0, m1 + k);
                r2 = GetMBROfEntrySet(upper2, m1 + k, upper2.Count);
                deadArea += r1.Area();
                deadArea += r2.Area();
                for (int i = 0; i < upper2.Count; ++i) { deadArea -= upper2[i].GetMBR().Area(); }
                overlap = r1.Overlap(r2);

                if ((overlap < minOverlap) ||
                    ((overlap == minOverlap) && (deadArea < minDeadArea)))
                {
                    minOverlap = overlap;
                    minDeadArea = deadArea;
                    dist = m1 + k;
                    isLower = false;
                }
            }

            List<XTDirEntry<T>> splitSet = isLower ? lower2 : upper2;
            set1 = splitSet.GetRange(0, dist);
            set2 = splitSet.GetRange(dist, splitSet.Count - dist);
        }


        private MBR GetMBROfEntrySet(List<XTDirEntry<T>> entrySet, int startIndex, int endIndex)
        {
            int dimension = entrySet[startIndex].GetMBR().Dimension;

            MBR ret = MBR.EmptyMBR(dimension);
            int l = 0;
            for (l = startIndex; l < endIndex; ++l)
            {
                ret.Enlarge(entrySet[l].GetMBR());
            }

            return ret;            
        }


        private XTDirEntry<T> GetBestSubtree(MBR mbr)
        {
            int ret;

            List<int> inside = new List<int>();
            for (int i = 0; i < entries.Count; ++i)
            {
                if (entries[i].GetMBR().Inside(mbr))
                {
                    inside.Add(i);
                }
            }

            if (inside.Count == 1)
            {
                //case 1: choose the one that contains the MBR
                ret = inside[0];
            }
            else if (inside.Count > 1)
            {
                //case 2 : multiple entry contains the MBR, choose the one with the smallest area
                double dmin = double.MaxValue;
                int minindex = -1;
                for (int i = 0; i < inside.Count; i++)
                {
                    double d = entries[inside[i]].GetMBR().Area();
                    if (d < dmin)
                    {
                        minindex = i;
                        dmin = d;
                    }
                }
                ret = inside[minindex];
            }
            else
            {
                //case 3: rectangle falls into an entry ->
                // For nodes that point to internal node:
                // Pick the entry that is enlarged at the least;
                // At the same magnification:
                // Take the record, has the lowest Total area
                //
                // For nodes that point to data nodes:
                // Take the one who caused the least overlap
                // With the same overlap:
                // Pick the entry that is enlarged at the least;
                // At the same magnification:
                // Take the record, has the lowest Total area
                int minindex = -1;

                if (son_is_data)
                {
                    double omin = double.MaxValue;
                    double fmin = double.MaxValue;
                    double amin = double.MaxValue;

                    for (int i = 0; i < entries.Count; i++)
                    {                        
                        MBR bmbr = MBR.Enlarge(entries[i].GetMBR(), mbr);
                                               
                        // calculate area and area enlargement
                        double a = entries[i].GetMBR().Area();
                        double f = bmbr.Area() - a;

                        // calculate overlap before enlarging entry_i
                        double old_o = 0.0;
                        double o = old_o;

                        for (int j = 0; j < entries.Count; j++)
                        {
                            if (j != i)
                            {
                                old_o += entries[i].GetMBR().Overlap(entries[j].GetMBR());
                                o += bmbr.Overlap(entries[j].GetMBR());
                            }
                        }
                        o -= old_o;

                        // is this entry better than the former optimum ?
                        if ((o < omin) || (o == omin && f < fmin) || (o == omin && f == fmin && a < amin))
                        {
                            minindex = i;
                            omin = o;
                            fmin = f;
                            amin = a;
                        }
                    }
                }
                else
                {
                    double fmin = double.MaxValue;
                    double amin = double.MaxValue;
                    for (int i = 0; i < entries.Count; i++)
                    {
                        MBR bmbr = MBR.Enlarge(entries[i].GetMBR(), mbr);

                        // calculate area and area enlargement
                        double a = entries[i].GetMBR().Area();
                        double f = bmbr.Area() - a;

                        // is this entry better than the former optimum ?
                        if ((f < fmin) || (f == fmin && a < amin))
                        {
                            minindex = i;
                            fmin = f;
                            amin = a;
                        }                        
                    }
                }                

                ret = minindex;
            }

            return entries[ret];
        }

        internal bool DeleteRecalcMBR()
        {
            if (parentEntry != null)
            {
                if (entries.Count > 0)
                {
                    MBR newMBR = GetMBR();
                    if (!parentEntry.GetMBR().EqualsByValue(newMBR))
                    {
                        return parentEntry.SetMBRUpwards(newMBR);
                    }
                    return true;
                }
                else
                {
                    return parentEntry.Delete();
                }
            }
            else
            {
                if (entries.Count > 0) return true;
                else return false;
            }
        }

        public override void pointQuery(XTVector p, List<T> ret)
        {
            foreach (XTDirEntry<T> e in entries)
            {
                if (e.GetMBR().Inside(p))
                {
                    e.GetChildNode().pointQuery(p, ret);
                }
            }
        }


        public override void rangeQuery(MBR mbr, List<T> ret)
        {
            foreach (XTDirEntry<T> e in entries)
            {
                if (e.GetMBR().Overlaps(mbr))
                {
                    e.GetChildNode().rangeQuery(mbr, ret);
                }
            }          
        }
    }
}
