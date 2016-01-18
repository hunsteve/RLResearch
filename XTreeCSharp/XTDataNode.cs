using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTreeCSharp
{
    class XTDataNode<T> : XTNode<T> where T : XTData    
    {
        T data;
        XTree<T> tree;

        public XTDataNode(T data, XTree<T> tree)
        {
            this.data = data;
            this.tree = tree;
            tree.AddDataToMapping(data, this);
        }

        public override MBR GetMBR()
        {
            return data.GetMBR();
        }

        public override XTNode<T> Insert(T data)
        {
            XTDataNode<T> brotherDataNode = new XTDataNode<T>(data, tree);            
            return brotherDataNode;
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
        }


        internal bool Delete()
        {
            if (parentEntry != null)
            {
                return parentEntry.Delete();
            }
            else return false;
        }

        public override void pointQuery(XTVector p, List<T> ret)
        {
            if (data.GetMBR().Inside(p))
            {
                ret.Add(data);
            }
        }

        public override void rangeQuery(MBR mbr, List<T> ret)
        {
            if (data.GetMBR().Overlaps(mbr))
            {
                ret.Add(data);
            }
        }
    }
}
