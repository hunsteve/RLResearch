using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTreeCSharp
{
    public class DebugMBR {
        public MBR mbr;
        public int level;


        public DebugMBR(MBR mbr, int level)
        {            
            this.mbr = mbr;
            this.level = level;
        }
    }

    abstract class XTNode<T> where T : XTData    
    {
        protected XTDirEntry<T> parentEntry;

        public abstract XTNode<T> Insert(T data);

        public abstract MBR GetMBR();

        public abstract void DebugListMBRs(List<DebugMBR> list, int level);

        public void SetParentEntry(XTDirEntry<T> parentEntry)
        {
            this.parentEntry = parentEntry;
        }

        public abstract void pointQuery(XTVector p, List<T> ret);

        public abstract void rangeQuery(MBR mbr, List<T> ret);
        
    }
}
