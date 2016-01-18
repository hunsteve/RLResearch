using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTreeCSharp
{
    class XTDirEntryComparer<T> : IComparer<XTDirEntry<T>> where T : XTData
    {        
        MBRComparer comp;

        public XTDirEntryComparer(int dimIndex, bool sortByMin)
        {
            comp = new MBRComparer(dimIndex, sortByMin);
        }

        public int Compare(XTDirEntry<T> x, XTDirEntry<T> y)
        {
            return comp.Compare(x.GetMBR(), y.GetMBR());
        }     
    }


    class XTDirEntry<T> where T : XTData
    {
        MBR mbr;
        XTNode<T> childNode;
        XTDirNode<T> parentNode;
        public int History
        {
            get;
            set;
        }


        public XTDirEntry(XTNode<T> childNode, XTDirNode<T> parentNode)
        {
            this.childNode = childNode;
            SetMBR(childNode.GetMBR());
            this.parentNode = parentNode;
            childNode.SetParentEntry(this);
        }

        public void SetParentNode(XTDirNode<T> parentNode)
        {
            this.parentNode = parentNode;
        }
        
        public void Enlarge(MBR aMbr)
        {
            mbr = MBR.Enlarge(mbr, aMbr);            
        }

        public MBR GetMBR()
        {
            return mbr;
        }

        public void SetMBR(MBR aMbr)
        {
            mbr = aMbr;
        }

        public XTNode<T> GetChildNode()
        {
            return childNode;
        }

        internal bool Delete()
        {
            parentNode.entries.Remove(this);
            return parentNode.DeleteRecalcMBR();
        }

        internal bool SetMBRUpwards(MBR mbr)
        {
            SetMBR(mbr);
            return parentNode.DeleteRecalcMBR();
        }
    }
}
