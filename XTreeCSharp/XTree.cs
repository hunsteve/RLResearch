using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTreeCSharp
{
    public class XTree<T> where T : XTData
    {
        XTNode<T> root;
        Dictionary<T, XTDataNode<T>> dataMapping;

        public XTree()
        {
            root = null;
            dataMapping = new Dictionary<T, XTDataNode<T>>();
        }

        public List<DebugMBR> DebugListMBRs()
        {
            List<DebugMBR> ret = new List<DebugMBR>();
            if (root != null) root.DebugListMBRs(ret, 0);
            return ret;
        }

        public void Insert(T data)
        {
            if (!dataMapping.ContainsKey(data))
            {
                if (root != null)
                {
                    XTNode<T> rootBrother = root.Insert(data);
                    if (rootBrother != null)
                    {
                        XTDirNode<T> newRoot = new XTDirNode<T>(this);
                        newRoot.entries.Add(new XTDirEntry<T>(root, newRoot));
                        newRoot.entries.Add(new XTDirEntry<T>(rootBrother, newRoot));

                        root = newRoot;
                    }
                }
                else
                {
                    root = new XTDataNode<T>(data, this);
                }
            }
        }

        public void Delete(T data)
        {
            if (!dataMapping.ContainsKey(data))
            {
                throw new KeyNotFoundException();
            }
            else {
                if (!dataMapping[data].Delete())
                {
                    root = null;
                }
                dataMapping.Remove(data);
            }
        }

        internal void AddDataToMapping(T data, XTDataNode<T> node)
        {
            dataMapping.Add(data, node);
        }


   

        public List<T> rangeQuery(XTVector p, float radius)
        {
            throw new NotImplementedException();
        }

        public List<T> rangeQuery(MBR mbr)
        {
            List<T> ret = new List<T>();
            if (root != null)
            {
                root.rangeQuery(mbr, ret);
            }

            return ret;         
        }

        public List<T> pointQuery(XTVector p)
        {
            List<T> ret = new List<T>();

            if (root != null)
            {
                root.pointQuery(p, ret);
            }

            return ret;
        }

        public T nearestNeighbourSearch(T queryPoint)
        {
            throw new NotImplementedException();
        }

        public List<T> KNearestNeighbourSearch(T queryPoint, int k)
        {
            throw new NotImplementedException();
        }
    }
}
